using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.SceneManagement;
using HedgehogTeam.EasyTouch;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;
using GS;
using Facebook.Unity;

public struct TurnData
{
    public List<Vector2> movements;
    public int ballId;

    public TurnData(List<Vector2> _movements, int _ballId)
    {
        movements = _movements;
        ballId = _ballId;
    }
}

public class GridManager : PunBehaviour
{
    static private GridManager instance;
    static public GridManager Instance { get { return instance; } }

    private ModelGrid modelGrid;
    public ModelGrid ModelGrid { get { return modelGrid; } }

    private OptimizedGrid optiGrid;
    public OptimizedGrid OptiGrid { get { return optiGrid; } }

    [SerializeField]
    private AIEvaluationData aiEvaluationData;
    [SerializeField]
    private Transform board;
    private Animation boardAnimation;

    [SerializeField]
    public List<GameObject> blackStartPosition;
    [SerializeField]
    public List<GameObject> whiteStartPosition;

    [SerializeField]
    public List<Ball> blackBalls;
    [SerializeField]
    public List<Ball> whiteBalls;

    private bool isEqualityTurn = false;
    public bool IsEqualityTurn { get { return isEqualityTurn; } }

    private bool isPlayingTutorial = false;

    private bool opponentGoesNextRound = false;
    private bool playerGoesNextRound = false;

    private int roundNumber;
    private int numberOfRound = 2;

    private List<Vector2> lastTurnMoves;
    private int lastTurnBallId;
    public bool AlreadySentLastTurnData = false;

    private BallColor actualTurn;
    public BallColor ActualTurn { get { return actualTurn; } }
    public BallColor NotActualTurn { get { return (actualTurn == BallColor.White) ? BallColor.Black : BallColor.White; } }

    [Header("Victory Animation")]
    [SerializeField]
    public float timeBeforeVictoryAnimation;
    [SerializeField]
    private float timeBetweenBallsPhase1AnimBegin;
    [SerializeField]
    private float timeBeforePhase2AnimBegin;
    [SerializeField]
    private float timeFromPhase2AnimBeginToRoundResultPanel;

    [HideInInspector]
    public WinningPattern actualWinningPattern;

    private int animatorHashPlaceBall;
    private int animatorHashMove;
    private int animatorHashWinPhase1;
    private int animatorHashWinPhase2;
    private int animatorHashScoreCounting;

    private void Awake()
    {
        animatorHashPlaceBall = Animator.StringToHash("PlaceBall");
        animatorHashMove = Animator.StringToHash("Move");
        animatorHashWinPhase1 = Animator.StringToHash("WinPhase1");
        animatorHashWinPhase2 = Animator.StringToHash("WinPhase2");
        animatorHashScoreCounting = Animator.StringToHash("ScoreCounting");

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        boardAnimation = board.GetComponent<Animation>();
    }

    void Start()
    {
        roundNumber = 1;

        UIManager.Instance.NextRound += OnNextRound;
        UIManager.Instance.InviteFriend += OnInviteFriend;
        UIManager.Instance.FinishTurn += OnFinishTurn;
    }

    public void StartTurns()
    {
        if (roundNumber == 1)
            actualTurn = BallColor.White;
        else
            actualTurn = BallColor.Black;

        PlayerManager.Instance.GetPlayer(actualTurn).StartTurn();

        if (PlayerManager.Instance.Player1.Color == actualTurn)
        {
            UIManager.Instance.SetPlayer1Turn();
        }
        else
        {
            UIManager.Instance.SetPlayer2Turn();
        }
    }

    public void InitForGameStart()
    {
        DisplayBoard(true);
        CreateGrid();
    }

    public void DisplayBoard(bool isShown)
    {
        if (isShown)
            boardAnimation.Play("BoardPopIn", PlayMode.StopAll);
        else
            boardAnimation.Play("BoardPopOut", PlayMode.StopAll);
    }

    private void CreateGrid()
    {
        modelGrid = new ModelGrid(5, 9, FindObjectsOfType<Cell>().ToList());
        optiGrid = new OptimizedGrid(5, 9);
        optiGrid.SetPatternData(aiEvaluationData);
    }

    public void SetPlayingTuto(bool isPlaying)
    {
        isPlayingTutorial = isPlaying;
    }

    public void ResetGame()
    {
        roundNumber = 1;
        UIManager.Instance.StopPlayerTurns();
        CleanBoard();
        PlayerManager.Instance.DeletePlayers();
    }

    public void CleanBoard()
    {
        alreadyAnimatedPattern.Clear();

        optiGrid.Reset();
        modelGrid.Reset();

        Ball.resetPlaySound = true;

        Ball[] balls = FindObjectsOfType<Ball>();
        foreach (Ball b in balls)
        {
            if (b.isPickedUp)
                b.PutDownBall();

            b.Reset();
        }

        ReplaceBalls();

        isEqualityTurn = false;
        AlreadySentLastTurnData = false;
        lastTurnBallId = 0;
        lastTurnMoves = null;

        PlayerManager.Instance.Player1.Reset();
        PlayerManager.Instance.Player2.Reset();

        UIManager.Instance.ResetGame();
    }

    public void SetPawnsStartPosition(BallColor ballColor, BallColor startPositionColor)
    {
        if (ballColor == BallColor.Black)
        {
            foreach (Ball blackB in blackBalls)
            {
                if (startPositionColor == BallColor.Black)
                {
                    blackB.startPosition = blackStartPosition[blackB.ballId].transform.localPosition;
                }
                else
                {
                    blackB.startPosition = whiteStartPosition[blackB.ballId].transform.localPosition;
                }
            }
        }
        else
        {
            foreach (Ball whiteB in whiteBalls)
            {
                if (startPositionColor == BallColor.Black)
                {
                    whiteB.startPosition = blackStartPosition[whiteB.ballId].transform.localPosition;
                }
                else
                {
                    whiteB.startPosition = whiteStartPosition[whiteB.ballId].transform.localPosition;
                }
            }
        }
    }

    public void ReplaceBalls()
    {
        Ball[] balls = FindObjectsOfType<Ball>();
        foreach (Ball b in balls)
        {
            b.MoveToResetPosition();
        }
    }

    public void GoToNextRound()
    {
        roundNumber++;

        CleanBoard();

        StartTurns();
    }

    public Ball GetBall(BallColor color, int id)
    {
        if (color == BallColor.White)
        {
            foreach (Ball ball in whiteBalls)
            {
                if (ball.ballId == id)
                    return ball;
            }
        }
        else
        {
            foreach (Ball ball in blackBalls)
            {
                if (ball.ballId == id)
                    return ball;
            }
        }

        return null;
    }

    public Ball ChangeBallPosition(Cell firstCell, Cell secondCell)
    {
        Ball ball = firstCell.ball;

        secondCell.ball = ball;
        firstCell.ball = null;

        ball.owner = secondCell;

        Move move = new Move();
        move.fromX = firstCell.y;
        move.fromY = firstCell.x;
        move.toX = secondCell.y;
        move.toY = secondCell.x;
        move.color = (CellColor)ball.Color;
        move.isPoint = ball.Score > 0 ? true : false;

        GridManager.Instance.OptiGrid.DoMove(move);

        if (ball.isPickedUp)
            ball.Animator.SetTrigger(animatorHashPlaceBall);
        else
            ball.Animator.SetTrigger(animatorHashMove);

        ball.isPickedUp = false;
        ball.FixSortingLayer(true);

        ball.transform.DOMove(secondCell.transform.position, 0.75f).OnComplete(() =>
        {
            ball.transform.position = secondCell.transform.position;
            ball.FixSortingLayer(false);
        });

        return ball;
    }

    public Ball PlaceBall(BallColor color, bool isPoint, Cell cell)
    {
        Ball ball;
        int score = (isPoint == true) ? 1 : 0;

        if (color == BallColor.Black)
        {
            int index = blackBalls.FindIndex(x => (x.Score == score) && (x.owner == null));
            ball = blackBalls[index];
        }
        else
        {
            int index = whiteBalls.FindIndex(x => (x.Score == score) && (x.owner == null));
            ball = whiteBalls[index];
        }

        ball.DOPlace(cell);
        return ball;
    }

    public void NextTurn()
    {
        if (ActualTurn == PlayerManager.Instance.Player1.Color)
        {
            actualTurn = PlayerManager.Instance.Player2.Color;
            UIManager.Instance.SetPlayer2Turn();
            PlayerManager.Instance.Player2.StartTurn();
        }
        else
        {
            actualTurn = PlayerManager.Instance.Player1.Color;
            UIManager.Instance.SetPlayer1Turn();
            PlayerManager.Instance.Player1.StartTurn();
        }
    }

    public void PlayerTurnEnded(List<Vector2> movements, int ballId)
    {
        lastTurnMoves = movements;
        lastTurnBallId = ballId;

        Cell cell = modelGrid.GetCellFromModel((int)movements[movements.Count - 1].x, (int)movements[movements.Count - 1].y);

        bool justWon = Utils.CheckWin(modelGrid, cell, false);
        if (justWon || isEqualityTurn)
        {
            if (justWon)
            {
                DOVirtual.DelayedCall(timeBeforeVictoryAnimation, PlayVictoryAnimation);
                //SendLastTurnData();
                //AlreadySentLastTurnData = true;
            }

            if (!isEqualityTurn && (PlayerManager.Instance.Player1.NbOfTurn != PlayerManager.Instance.Player2.NbOfTurn))
            {
                isEqualityTurn = true;
            }
            else
            {
                EndGame(justWon);
                return;
            }
        }

        if (PlayerManager.Instance.GetPlayer(BallColor.Black).NbOfTurn == 10 && PlayerManager.Instance.GetPlayer(BallColor.White).NbOfTurn == 10 && isPlayingTutorial)
        {
            OnPhase2Begin();
            return;
        }

        if (!justWon && !isEqualityTurn)
        {
            NextTurn();
        }
    }

    private void OnPhase2Begin()
    {
        optiGrid.UpdateOptimizedGridPoints(modelGrid);
        DOVirtual.DelayedCall(1.5f, UIManager.Instance.DisplayTutorialPhase2Movement);
    }

    public void EndGame(bool justWon)
    {
        if (GameManager.Instance.GameMode == GameMode.Remote && ActualTurn == PlayerManager.Instance.Player1.Color)
            SendLastTurnData();

        if (!justWon)
            DOVirtual.DelayedCall(1.5f, DisplayRoundResult, true);
    }

    private void DisplayRoundResult()
    {
        if (roundNumber == numberOfRound)
        {
            if (PlayerManager.Instance.Player1.roundScore > PlayerManager.Instance.Player2.roundScore)
            {
                AudioManager.Instance.PlayJingleAboveMusic(SoundID.JingleWin);
            }
            else if (PlayerManager.Instance.Player2.roundScore > PlayerManager.Instance.Player1.roundScore)
            {
                AudioManager.Instance.PlayJingleAboveMusic(SoundID.JingleLoose);
            }
            else
            {
                AudioManager.Instance.PlayJingleAboveMusic(SoundID.JingleDraw);
            }

            UIManager.Instance.StopPortraitAnimation();
            UIManager.Instance.DisplayEndGame();
            GameManager.Instance.GameEnded();
            DOVirtual.DelayedCall(2.0f, () => { UIManager.Instance.endGamePanel.Display(true); });
        }
        else
        {
            UIManager.Instance.player1.StopPortraitAnimation();
            UIManager.Instance.player2.StopPortraitAnimation();

            UIManager.Instance.roundResultPanel.Display(true);
        }
    }

    List<WinningPattern> alreadyAnimatedPattern = new List<WinningPattern>();
    public void PlayVictoryAnimation()
    {
        List<WinningPattern> winningPatterns = new List<WinningPattern>();
        optiGrid.GetWinningPatterns(out winningPatterns);

        WinningPattern toKeep = new WinningPattern();
        int bestScore = -1;
        foreach (WinningPattern pattern in winningPatterns)
        {
            bool alreadyDone = false;

            foreach (WinningPattern alreadyDonePattern in alreadyAnimatedPattern)
            {
                if (pattern.IsSame(alreadyDonePattern))
                {
                    alreadyDone = true;
                    break;
                }
            }

            if (alreadyDone)
                continue;

            alreadyAnimatedPattern.Add(pattern);

            int patternScore = pattern.GetScore(modelGrid);
            if (bestScore <= patternScore)
            {
                toKeep = pattern;
                bestScore = patternScore;
            }
        }
        StartCoroutine(playVictoryAnimationPhase1(toKeep));

        AudioManager.Instance.PlayAudio(SoundID.ComboRumble);
        AudioManager.Instance.ResetVictoryAnimationSounds();
    }

    IEnumerator playVictoryAnimationPhase1(WinningPattern pattern)
    {
        UIManager.Instance.DisableBackToMainMenuButton(false);

        Ball ball = modelGrid.GetCellFromModel((int)pattern.cells[0].y, (int)pattern.cells[0].x).ball;
        ball.FixSortingLayer(true);
        ball.Animator.SetTrigger(animatorHashWinPhase1);
        yield return new WaitForSeconds(timeBetweenBallsPhase1AnimBegin);

        ball = modelGrid.GetCellFromModel((int)pattern.cells[1].y, (int)pattern.cells[1].x).ball;
        ball.FixSortingLayer(true);
        ball.Animator.SetTrigger(animatorHashWinPhase1);
        yield return new WaitForSeconds(timeBetweenBallsPhase1AnimBegin);

        ball = modelGrid.GetCellFromModel((int)pattern.cells[2].y, (int)pattern.cells[2].x).ball;
        ball.FixSortingLayer(true);
        ball.Animator.SetTrigger(animatorHashWinPhase1);
        yield return new WaitForSeconds(timeBetweenBallsPhase1AnimBegin);

        ball = modelGrid.GetCellFromModel((int)pattern.cells[3].y, (int)pattern.cells[3].x).ball;
        ball.FixSortingLayer(true);
        ball.Animator.SetTrigger(animatorHashWinPhase1);
        yield return new WaitForSeconds(timeBetweenBallsPhase1AnimBegin);

        ball = modelGrid.GetCellFromModel((int)pattern.cells[4].y, (int)pattern.cells[4].x).ball;
        ball.FixSortingLayer(true);
        ball.Animator.SetTrigger(animatorHashWinPhase1);
        yield return new WaitForSeconds(timeBeforePhase2AnimBegin);
        StartCoroutine(playVictoryAnimationPhase2(pattern));
    }

    IEnumerator playVictoryAnimationPhase2(WinningPattern pattern)
    {
        modelGrid.GetCellFromModel((int)pattern.cells[0].y, (int)pattern.cells[0].x).ball.Animator.SetTrigger(animatorHashWinPhase2);
        modelGrid.GetCellFromModel((int)pattern.cells[1].y, (int)pattern.cells[1].x).ball.Animator.SetTrigger(animatorHashWinPhase2);
        modelGrid.GetCellFromModel((int)pattern.cells[2].y, (int)pattern.cells[2].x).ball.Animator.SetTrigger(animatorHashWinPhase2);
        modelGrid.GetCellFromModel((int)pattern.cells[3].y, (int)pattern.cells[3].x).ball.Animator.SetTrigger(animatorHashWinPhase2);
        modelGrid.GetCellFromModel((int)pattern.cells[4].y, (int)pattern.cells[4].x).ball.Animator.SetTrigger(animatorHashWinPhase2);

        yield return new WaitForSeconds(timeFromPhase2AnimBeginToRoundResultPanel);
        StartCoroutine(addVictoryPoints(pattern));
    }

    IEnumerator addVictoryPoints(WinningPattern pattern)
    {
        Ball ball = modelGrid.GetCellFromModel((int)pattern.cells[0].y, (int)pattern.cells[0].x).ball;
        ball.Animator.SetTrigger(animatorHashScoreCounting);
        ball.FixSortingLayer(false);
        yield return new WaitForSeconds(1.0f);

        ball = modelGrid.GetCellFromModel((int)pattern.cells[1].y, (int)pattern.cells[1].x).ball;
        ball.Animator.SetTrigger(animatorHashScoreCounting);
        ball.FixSortingLayer(false);
        yield return new WaitForSeconds(1.0f);

        ball = modelGrid.GetCellFromModel((int)pattern.cells[2].y, (int)pattern.cells[2].x).ball;
        ball.Animator.SetTrigger(animatorHashScoreCounting);
        ball.FixSortingLayer(false);
        yield return new WaitForSeconds(1.0f);

        ball = modelGrid.GetCellFromModel((int)pattern.cells[3].y, (int)pattern.cells[3].x).ball;
        ball.Animator.SetTrigger(animatorHashScoreCounting);
        ball.FixSortingLayer(false);
        yield return new WaitForSeconds(1.0f);

        ball = modelGrid.GetCellFromModel((int)pattern.cells[4].y, (int)pattern.cells[4].x).ball;
        ball.Animator.SetTrigger(animatorHashScoreCounting);
        ball.FixSortingLayer(false);
        yield return new WaitForSeconds(3.0f);
        playVictoryAnimationEnd(pattern);
    }

    private void playVictoryAnimationEnd(WinningPattern pattern)
    {
        UIManager.Instance.DisableBackToMainMenuButton(true);

        if (IsEqualityTurn && PlayerManager.Instance.Player1.NbOfTurn != PlayerManager.Instance.Player2.NbOfTurn)
        {
            NextTurn();
        }
        else
        {
            DisplayRoundResult();
        }
    }

    public void HighlightAvailableMoveCells(Cell cell)
    {
        if (!Options.GetEnablePlacementHelp())
            return;

        modelGrid.ResetCellsColor();

        List<Move> moves = new List<Move>();
        if (PlayerManager.Instance.GetPlayer(GridManager.Instance.ActualTurn).HasAlreadyJumpedOnce)
            moves = optiGrid.GetAvailableMoves(cell, true);
        else
            moves = optiGrid.GetAvailableMoves(cell);

        foreach (Move move in moves)
        {
            Cell toCell = modelGrid.GetCellFromModel(move.toY, move.toX);
            toCell.SetHighlightedCell(true);
        }
    }

    private int GetWinningPatternScore(WinningPattern winningPattern)
    {
        int score = 0;
        if (winningPattern.cells.Length == 0)
        {
        }
        else
        {
            for (int i = 0; i < winningPattern.cells.Length; ++i)
            {
                Cell cell = modelGrid.GetCellFromModel(new Vector2(winningPattern.cells[i].y, winningPattern.cells[i].x));

                if (cell && cell.ball)
                    score += cell.ball.Score;
            }
        }

        return score;
    }

    public void BallAddScore(Ball ball)
    {
        UIManager.Instance.PopScoreParticle(ball);
    }

    public void AddPlayer1Score(int nb)
    {
        PlayerManager.Instance.Player1.roundScore = nb;
        PlayerManager.Instance.Player1.totalScore += nb;
        UIManager.Instance.player1.SetScoreCounter(PlayerManager.Instance.Player1.totalScore);
    }

    public void AddPlayer2Score(int nb)
    {
        PlayerManager.Instance.Player2.roundScore = nb;
        PlayerManager.Instance.Player2.totalScore += nb;
        UIManager.Instance.player2.SetScoreCounter(PlayerManager.Instance.Player2.totalScore);
    }

    public void SendLastTurnData()
    {
        if (AlreadySentLastTurnData)
        {
            return;
        }

        if (lastTurnMoves == null)
        {
            return;
        }

        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("SendLastTurnDataRPC", PhotonTargets.Others, lastTurnMoves.ToArray(), lastTurnBallId);
    }

    [PunRPC]
    void SendLastTurnDataRPC(Vector2[] movements, int ballId)
    {
        if (movements == null)
        {
            return;
        }

        Player player = PlayerManager.Instance.GetPlayer(ActualTurn);
        if (player == null)
        {
        }

        RemotePlayer remotePlayer = player as RemotePlayer;
        if (remotePlayer == null)
        {
            remotePlayer = PlayerManager.Instance.GetPlayer(NotActualTurn) as RemotePlayer;
            if (remotePlayer == null)
            {
            }
        }

        remotePlayer.SetLastMovements(movements, ballId);

        if (movements.Length > 1)
            StartCoroutine(MoveCoroutine(movements));
        else if (movements.Length > 0)
            Phase1Move(movements[0], ballId);
    }

    void Phase1Move(Vector2 pos, int ballIndex)
    {
        Cell cell = modelGrid.GetCellFromModel(pos);
        Ball ball;

        if (PlayerManager.Instance.GetPlayer(ActualTurn).Color == BallColor.Black)
        {
            ball = blackBalls.Find(x => x.ballId == ballIndex);
        }
        else
        {
            ball = whiteBalls.Find(x => x.ballId == ballIndex);
        }

        ball.DOPlace(cell);

        PlayerManager.Instance.GetPlayer(ActualTurn).EndTurn();
    }

    IEnumerator MoveCoroutine(Vector2[] movements)
    {
        for (int i = 0; i < movements.Length - 1; i++)
        {
            ChangeBallPosition(modelGrid.GetCellFromModel(movements[i]), modelGrid.GetCellFromModel(movements[i + 1]));
            yield return new WaitForSeconds(0.9f);
        }

        PlayerManager.Instance.GetPlayer(ActualTurn).EndTurn();
    }

    public void OnRestart()
    {
        GameManager.Instance.Disconnect();
    }

    public void OnNextRound()
    {
        if (playerGoesNextRound)
            return;

        playerGoesNextRound = true;

        PhotonView photonView = PhotonView.Get(this);
        if (PhotonNetwork.connectedAndReady)
            photonView.RPC("SendNextRound", PhotonTargets.Others);

        if (opponentGoesNextRound || GameManager.Instance.GameMode == GameMode.Local || GameManager.Instance.GameMode == GameMode.AI)
        {
            playerGoesNextRound = opponentGoesNextRound = false;
            GoToNextRound();
        }
    }

    public void OnFinishTurn()
    {
        //
    }

    [PunRPC]
    public void SendNextRound()
    {
        opponentGoesNextRound = true;

        if (playerGoesNextRound)
        {
            playerGoesNextRound = opponentGoesNextRound = false;
            GoToNextRound();
        }
    }

    public void OnInviteFriend()
    {
        FB.AppRequest("Viens jouer à Item");

        //fbManager.LoadInvitableFriends ();
    }

}