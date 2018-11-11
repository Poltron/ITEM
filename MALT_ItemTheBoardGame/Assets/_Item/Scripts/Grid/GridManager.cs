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

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
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
        PlayerManager.Instance.GetPlayer(BallColor.White).StartTurn();
        actualTurn = BallColor.White;

        if (PlayerManager.Instance.Player1.Color == BallColor.White)
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
        board.GetComponent<Animator>().SetBool("bPopIn", isShown);
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
        CleanBoard();
    }

    public void CleanBoard()
    {
        alreadyAnimatedPattern.Clear();

        optiGrid.Reset();
        modelGrid.Reset();

        blackBalls.Clear();
        whiteBalls.Clear();

        Ball[] balls = FindObjectsOfType<Ball>();

        Ball.resetPlaySound = true;

        foreach (Ball b in balls)
        {
            b.Reset();

            if (b.Color == BallColor.Black)
                blackBalls.Add(b);
            else
                whiteBalls.Add(b);

            b.MoveToResetPosition();
        }

        isEqualityTurn = false;
        AlreadySentLastTurnData = false;
        lastTurnBallId = 0;
        lastTurnMoves = null;

        PlayerManager.Instance.Player1.Reset();
        PlayerManager.Instance.Player2.Reset();

        UIManager.Instance.ResetGame();
    }

    public void GoToNextRound()
    {
        roundNumber++;

        CleanBoard();
        
        SwitchPlayersColor();

        StartTurns();
    }

    private void SwitchPlayersColor()
    {
        if (PlayerManager.Instance.Player1.Color == BallColor.Black)
        {
            PlayerManager.Instance.SetPlayerColor(BallColor.White, PlayerID.Player1);
            PlayerManager.Instance.SetPlayerColor(BallColor.Black, PlayerID.Player2);
        }
        else
        {
            PlayerManager.Instance.SetPlayerColor(BallColor.Black, PlayerID.Player1);
            PlayerManager.Instance.SetPlayerColor(BallColor.White, PlayerID.Player2);
        }
    }

    public Ball GetBall(BallColor color, int id)
    {
        if (color == BallColor.White)
        {
            foreach(Ball ball in whiteBalls)
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
        Debug.Log("change ball position");
        Ball ball = firstCell.ball;

        secondCell.ball = ball;
        firstCell.ball = null;

        ball.owner = secondCell;
        Debug.Log("ball changed");
        Move move = new Move();
        move.fromX = firstCell.y;
        move.fromY = firstCell.x;
        move.toX = secondCell.y;
        move.toY = secondCell.x;
        move.color = (CellColor)ball.Color;
        move.isPoint = ball.Score > 0 ? true : false;

        Debug.Log("move set");
        GridManager.Instance.OptiGrid.DoMove(move);
        Debug.Log("opti move done");
        if (ball.isPickedUp)
            ball.GetComponent<Animator>().SetTrigger("PlaceBall");
        else
            ball.GetComponent<Animator>().SetTrigger("Move");

        ball.isPickedUp = false;
        ball.FixSortingLayer(true);
        Debug.Log("doing model move");
        ball.transform.DOMove(secondCell.transform.position, 0.75f).OnComplete(() =>
        {
            ball.transform.position = secondCell.transform.position;
            ball.FixSortingLayer(false);
            Debug.Log("model move done");
        });

        return ball;
    }

    public Ball PlaceBall(BallColor color, bool isPoint, Cell cell)
    {
        Ball ball;
        int score = (isPoint == true) ? 1 : 0;

        if (color == BallColor.Black)
        {
            int index = blackBalls.FindIndex(x => x.Score == score);
            ball = blackBalls[index];
            blackBalls.RemoveAt(index);
        }
        else
        {
            int index = whiteBalls.FindIndex(x => x.Score == score);
            ball = whiteBalls[index];
            whiteBalls.RemoveAt(index);
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

        Debug.Log(cell.name + " : (" + cell.x + " ; " + cell.y + ")");

        bool justWon = Utils.CheckWin(modelGrid, cell, false);
        if (justWon || isEqualityTurn)
        {
            if (justWon)
            {
                DOVirtual.DelayedCall(timeBeforeVictoryAnimation, PlayVictoryAnimation);
                //SendLastTurnData();
                //AlreadySentLastTurnData = true;
                Debug.Log("win animation");
            }

            if (!isEqualityTurn && (PlayerManager.Instance.Player1.NbOfTurn != PlayerManager.Instance.Player2.NbOfTurn))
            {
                isEqualityTurn = true;
                Debug.Log("player2 equalityturn");
            }
            else
            {
                Debug.Log("player2 endgame");
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
            Debug.Log("player end turn");
            NextTurn();
        }
    }

    private void OnPhase2Begin()
    {
        optiGrid.UpdateOptimizedGridPoints(modelGrid);
        DOVirtual.DelayedCall(1.5f, UIManager.Instance.DisplayTutorialPhase2Movement);
        Debug.Log("player2 display tutorial");
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
        UIManager.Instance.DisplayRoundResultPanel(true, roundNumber, PlayerManager.Instance.Player1.roundScore, PlayerManager.Instance.Player2.roundScore);

        if (roundNumber == numberOfRound)
        {
            GameManager.Instance.GameEnded();
            UIManager.Instance.roundResultPanel.ActivateGameResultsButton(true);
        }
        else
        {
            UIManager.Instance.roundResultPanel.ActivateNextRoundButton(true);
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
        Debug.Log("play victory animation4");
        Ball ball = modelGrid.GetCellFromModel((int)pattern.cells[0].y, (int)pattern.cells[0].x).ball;
        ball.FixSortingLayer(true);
        ball.Animator.SetTrigger("WinPhase1");
        yield return new WaitForSeconds(timeBetweenBallsPhase1AnimBegin);

        ball = modelGrid.GetCellFromModel((int)pattern.cells[1].y, (int)pattern.cells[1].x).ball;
        ball.FixSortingLayer(true);
        ball.Animator.SetTrigger("WinPhase1");
        yield return new WaitForSeconds(timeBetweenBallsPhase1AnimBegin);

        ball = modelGrid.GetCellFromModel((int)pattern.cells[2].y, (int)pattern.cells[2].x).ball;
        ball.FixSortingLayer(true);
        ball.Animator.SetTrigger("WinPhase1");
        yield return new WaitForSeconds(timeBetweenBallsPhase1AnimBegin);

        ball = modelGrid.GetCellFromModel((int)pattern.cells[3].y, (int)pattern.cells[3].x).ball;
        ball.FixSortingLayer(true);
        ball.Animator.SetTrigger("WinPhase1");
        yield return new WaitForSeconds(timeBetweenBallsPhase1AnimBegin);

        ball = modelGrid.GetCellFromModel((int)pattern.cells[4].y, (int)pattern.cells[4].x).ball;
        ball.FixSortingLayer(true);
        ball.Animator.SetTrigger("WinPhase1");
        yield return new WaitForSeconds(timeBeforePhase2AnimBegin);
        StartCoroutine(playVictoryAnimationPhase2(pattern));
    }

    IEnumerator playVictoryAnimationPhase2(WinningPattern pattern)
    {
        modelGrid.GetCellFromModel((int)pattern.cells[0].y, (int)pattern.cells[0].x).ball.Animator.SetTrigger("WinPhase2");
        modelGrid.GetCellFromModel((int)pattern.cells[1].y, (int)pattern.cells[1].x).ball.Animator.SetTrigger("WinPhase2");
        modelGrid.GetCellFromModel((int)pattern.cells[2].y, (int)pattern.cells[2].x).ball.Animator.SetTrigger("WinPhase2");
        modelGrid.GetCellFromModel((int)pattern.cells[3].y, (int)pattern.cells[3].x).ball.Animator.SetTrigger("WinPhase2");
        modelGrid.GetCellFromModel((int)pattern.cells[4].y, (int)pattern.cells[4].x).ball.Animator.SetTrigger("WinPhase2");

        yield return new WaitForSeconds(timeFromPhase2AnimBeginToRoundResultPanel);
        StartCoroutine(addVictoryPoints(pattern));
    }

    IEnumerator addVictoryPoints(WinningPattern pattern)
    {
        Ball ball = modelGrid.GetCellFromModel((int)pattern.cells[0].y, (int)pattern.cells[0].x).ball;
        ball.Animator.SetTrigger("ScoreCounting");
        ball.FixSortingLayer(false);
        yield return new WaitForSeconds(1.0f);

        ball = modelGrid.GetCellFromModel((int)pattern.cells[1].y, (int)pattern.cells[1].x).ball;
        ball.Animator.SetTrigger("ScoreCounting");
        ball.FixSortingLayer(false);
        yield return new WaitForSeconds(1.0f);

        ball = modelGrid.GetCellFromModel((int)pattern.cells[2].y, (int)pattern.cells[2].x).ball;
        ball.Animator.SetTrigger("ScoreCounting");
        ball.FixSortingLayer(false);
        yield return new WaitForSeconds(1.0f);

        ball = modelGrid.GetCellFromModel((int)pattern.cells[3].y, (int)pattern.cells[3].x).ball;
        ball.Animator.SetTrigger("ScoreCounting");
        ball.FixSortingLayer(false);
        yield return new WaitForSeconds(1.0f);

        ball = modelGrid.GetCellFromModel((int)pattern.cells[4].y, (int)pattern.cells[4].x).ball;
        ball.Animator.SetTrigger("ScoreCounting");
        ball.FixSortingLayer(false);
        yield return new WaitForSeconds(3.0f);
        playVictoryAnimationEnd(pattern);
    }

    private void playVictoryAnimationEnd(WinningPattern pattern)
    {
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
        if (PlayerManager.Instance.Player1.HasAlreadyJumpedOnce)
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
            //Debug.Log("no winning pattern");
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
        Debug.Log("send last turn data");
        if (AlreadySentLastTurnData)
        {
            Debug.Log("Already sent data");
            return;
        }

        if (lastTurnMoves == null)
        {
            Debug.Log("last turn moves are null, not sending");
            return;
        }

        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("SendLastTurnDataRPC", PhotonTargets.Others, lastTurnMoves.ToArray(), lastTurnBallId);
        Debug.Log("send last turn data after rpc");
    }

    [PunRPC]
    void SendLastTurnDataRPC(Vector2[] movements, int ballId)
    {
        if (movements == null)
        {
            Debug.Log("movements == null");
            return;
        }

        Player player = PlayerManager.Instance.GetPlayer(ActualTurn);
        if (player == null)
        {
            Debug.Log("player is null");
        }

        RemotePlayer remotePlayer = player as RemotePlayer;
        if (remotePlayer == null)
        {
            Debug.Log("remoteplayer is null");

            remotePlayer = PlayerManager.Instance.GetPlayer(NotActualTurn) as RemotePlayer;
            if (remotePlayer == null)
            {
                Debug.Log("remoteplayer is still null");
            }
        }

        remotePlayer.SetLastMovements(movements, ballId);

        Debug.Log("send last turn data RPC received");
        if (movements.Length > 1)
            StartCoroutine(MoveCoroutine(movements));
        else if (movements.Length > 0)
            Phase1Move(movements[0], ballId);
        else
            Debug.Log("empty last turn data");
    }

    void Phase1Move(Vector2 pos, int ballIndex)
    {
        Cell cell = modelGrid.GetCellFromModel(pos);
        Ball ball;

        if (PlayerManager.Instance.Player1.Color == BallColor.White)
        {
            ball = blackBalls.Find(x => x.ballId == ballIndex);
            blackBalls.Remove(ball);
        }
        else
        {
            ball = whiteBalls.Find(x => x.ballId == ballIndex);
            whiteBalls.Remove(ball);
        }

        ball.DOPlace(cell);

        Debug.Log("phase1move");
        PlayerManager.Instance.GetPlayer(ActualTurn).EndTurn();
    }
    
    IEnumerator MoveCoroutine(Vector2[] movements)
    {
        Debug.Log("movecoroutine");
        for (int i = 0; i < movements.Length - 1; i++)
        {
            ChangeBallPosition(modelGrid.GetCellFromModel(movements[i]), modelGrid.GetCellFromModel(movements[i + 1]));
            yield return new WaitForSeconds(0.9f);
        }
        Debug.Log("end movecoroutine");

        PlayerManager.Instance.GetPlayer(ActualTurn).EndTurn();
    }

    public void OnRestart()
    {
        Debug.Log("RESTART THE GAME");
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