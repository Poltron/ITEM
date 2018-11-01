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

public class RoundScore
{
    public int playerScore;
    public int otherPlayerScore;
    public int result;

    public RoundScore(int _playerScore, int _opponentScore, int _result)
    {
        playerScore = _playerScore;
        otherPlayerScore = _opponentScore;
        result = _result;
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

    private BallColor actualTurn;
    public BallColor ActualTurn { get { return actualTurn; } }

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
        board.gameObject.SetActive(isShown);
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

        PlayerManager.Instance.Player1.Reset();
        PlayerManager.Instance.Player2.Reset();

        UIManager.Instance.ResetGame();
    }

    public void GoToNextRound()
    {
        roundNumber++;

        CleanBoard();

        if (GameManager.Instance.GameMode != GameMode.AI)
            SwitchPlayersColor();

        // whites begin the game
        if (PlayerManager.Instance.Player1.Color == BallColor.White)
        {
            UIManager.Instance.SetPlayer1Turn();
            PlayerManager.Instance.Player1.StartTurn();
        }
        else
        {
            UIManager.Instance.SetPlayer2Turn();
            PlayerManager.Instance.Player2.StartTurn();
        }
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

    public void ChangeBallPosition(Cell firstCell, Cell secondCell)
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

        GridManager.Instance.OptiGrid.DoMove(move);

        if (ball.isPickedUp)
            ball.GetComponent<Animator>().SetTrigger("PlaceBall");
        else
            ball.GetComponent<Animator>().SetTrigger("Move");

        ball.isPickedUp = false;
        ball.FixSortingLayer(true);

        ball.transform.DOMove(secondCell.transform.position, 1f).OnComplete(() =>
        {
            ball.transform.position = secondCell.transform.position;
            ball.FixSortingLayer(false);
        });
    }

    public void PlaceBallIA(Cell cell)
    {
        Ball ball = blackBalls.First();
        blackBalls.RemoveAt(0);
        ball.DOPlace(cell);
    }

    IEnumerator waitFor(float t, System.Action func)
    {
        while ((t -= Time.deltaTime) > 0)
            yield return new WaitForEndOfFrame();

        func();
    }

    public void NextTurn()
    {
        if (ActualTurn == PlayerManager.Instance.Player1.Color)
        {
            UIManager.Instance.SetPlayer2Turn();
            PlayerManager.Instance.Player2.StartTurn();
            actualTurn = PlayerManager.Instance.Player2.Color;
        }
        else
        {
            UIManager.Instance.SetPlayer1Turn();
            PlayerManager.Instance.Player1.StartTurn();
            actualTurn = PlayerManager.Instance.Player1.Color;
        }
    }

    /*
    public void Player1TurnEnded(List<Vector2> movements)
    {
        Cell cell = modelGrid.GetCellFromModel((int)movements[movements.Count - 1].x, (int)movements[movements.Count - 1].y);

        bool justWon = Utils.CheckWin(modelGrid, cell, false);
        if (justWon || isEqualityTurn)
        {
            if (justWon)
            {
                DOVirtual.DelayedCall(timeBeforeVictoryAnimation, PlayVictoryAnimation, true);
                Debug.Log("win animation");
            }

            if (!isEqualityTurn && (PlayerManager.Instance.Player1.NbOfTurn != PlayerManager.Instance.Player2.NbOfTurn))
            {
                isEqualityTurn = true;
                Debug.Log("player1 equalityturn");
            }
            else
            {
                Debug.Log("player1 endgame");
                EndGame(justWon);
            }
        }
        else
        {
            NextTurn();
        }
    }
    */

    public void PlayerTurnEnded(List<Vector2> movements)
    {
        Cell cell = modelGrid.GetCellFromModel((int)movements[movements.Count - 1].x, (int)movements[movements.Count - 1].y);

        bool justWon = Utils.CheckWin(modelGrid, cell, false);
        if (justWon || isEqualityTurn)
        {
            if (justWon)
            {
                DOVirtual.DelayedCall(timeBeforeVictoryAnimation, PlayVictoryAnimation, true);
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

        if (PlayerManager.Instance.GetPlayer(BallColor.Black).ballCount == 0 && isPlayingTutorial)
        {
            isPlayingTutorial = false;
            StartCoroutine(waitFor(1.5f, UIManager.Instance.DisplayTutorialPhase2Movement));
            Debug.Log("player2 display tutorial");
        }

        if (!justWon && !isEqualityTurn)
        {
            Debug.Log("player end turn");
            NextTurn();
        }
    }

    /*
    bool alreadyPassed = false;
    public void EndAIPhase()
    {
        if (PlayerManager.Instance.Player1.ballCount == 0 && !alreadyPassed)
        {
            alreadyPassed = true;

            if (isPlayingTutorial)
                StartCoroutine(waitFor(1.5f, UIManager.Instance.DisplayTutorialPhase2Movement));
            else
            {
                UIManager.Instance.SetPlayer1Turn();
                PlayerManager.Instance.Player1.StartTurn();
            }
        }
        else
        {
            UIManager.Instance.SetPlayer1Turn();
            PlayerManager.Instance.Player1.StartTurn();
        }
    }
    
    public void Phase1TurnFinishedPlayer(Vector2 pos)
    {
        Cell cell = modelGrid.GetCellFromModel(pos);

        //Debug.Log("numberOfTurnsPlayer1 : " + numberOfTurnsPlayer1 + " / numberOfTurnsPlayer2 : " + numberOfTurnsPlayer2);

        if (GameManager.Instance.GameMode == GameMode.AI)
        {
            bool justWon = Utils.CheckWin(modelGrid, cell, false);
            if (justWon || isEqualityTurn)
            {
                if (justWon)
                {
                    DOVirtual.DelayedCall(timeBeforeVictoryAnimation, PlayVictoryAnimation, true);
                }

                if (!isEqualityTurn && (numberOfTurnsPlayer1 != numberOfTurnsPlayer2))
                {
                    nextTurnIsAI = true;
                    isEqualityTurn = true;
                }
                else
                {
                    nextTurnIsAI = true;
                    EndGame(justWon);
                }
            }
            else
            {

                if (PlayerManager.Instance.disableAI) // debug feature to test without AI
                {
                    UIManager.Instance.SetPlayer1Turn();
                    PlayerManager.Instance.Player1.StartTurn();
                }
                else
                {
                    UIManager.Instance.SetPlayer2Turn();
                    PlayIAPhase1();
                }
            }
        }
        else
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("ReceiveMovementsPhase1", PhotonTargets.Others, pos, cell.ball.ballId);

            bool justWon = Utils.CheckWin(modelGrid, cell, false);
            if (justWon || isEqualityTurn)
            {
                if (justWon)
                {
                    DOVirtual.DelayedCall(timeBeforeVictoryAnimation, PlayVictoryAnimation, true);
                }

                if (!isEqualityTurn && (numberOfTurnsPlayer1 != numberOfTurnsPlayer2))
                {
                    nextTurnIsAI = false;
                    isEqualityTurn = true;
                }
                else
                {
                    nextTurnIsAI = false;
                    EndGame(justWon);
                }
            }
            else
            {
                UIManager.Instance.SetPlayer2Turn();
            }
        }
    }

    public void Phase2TurnFinishedPlayer(List<Vector2> movements)
    {
        Vector2[] movementArray = movements.ToArray();
        Cell cell = modelGrid.GetCellFromModel(movementArray[movementArray.Length - 1]);

        //Debug.Log("numberOfTurnsPlayer1 : " + numberOfTurnsPlayer1 + " / numberOfTurnsPlayer2 : " + numberOfTurnsPlayer2);

        if (GameManager.Instance.GameMode == GameMode.AI)
        {
            bool justWon = Utils.CheckWin(modelGrid, cell, false);
            if (justWon || isEqualityTurn)
            {
                if (justWon)
                {
                    DOVirtual.DelayedCall(1.5f, PlayVictoryAnimation, true);
                }

                if (!isEqualityTurn && (numberOfTurnsPlayer1 != numberOfTurnsPlayer2))
                {
                    nextTurnIsAI = true;
                    isEqualityTurn = true;
                }
                else
                {
                    nextTurnIsAI = true;
                    EndGame(justWon);
                }
            }
            else
            {
                UIManager.Instance.SetPlayer2Turn();
                PlayIAPhase2();
            }
        }
        else
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("ReceiveMovementsPhase2", PhotonTargets.Others, movementArray);

            bool justWon = Utils.CheckWin(modelGrid, cell, false);
            if (justWon || isEqualityTurn)
            {
                if (justWon)
                {
                    DOVirtual.DelayedCall(timeBeforeVictoryAnimation, PlayVictoryAnimation, true);
                }

                if (!isEqualityTurn && (numberOfTurnsPlayer1 != numberOfTurnsPlayer2))
                {
                    nextTurnIsAI = false;
                    isEqualityTurn = true;
                }
                else
                {
                    nextTurnIsAI = false;
                    EndGame(justWon);
                }
            }
            else
            {
                UIManager.Instance.SetPlayer2Turn();
            }
        }
    }*/

    public void EndGame(bool justWon)
    {
        PlayerManager.Instance.Player1.EndTurn();
        
        if (!justWon)
            DOVirtual.DelayedCall(1.5f, DisplayRoundResult, true);
    }

    private void DisplayRoundResult()
    {
        UIManager.Instance.DisplayRoundResultPanel(true, roundNumber, PlayerManager.Instance.Player1.roundScore, PlayerManager.Instance.Player2.roundScore);

        if (roundNumber == numberOfRound)
        {
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
            if (PlayerManager.Instance.Player1.NbOfTurn < PlayerManager.Instance.Player2.NbOfTurn)
            {
                UIManager.Instance.SetPlayer1Turn();
                PlayerManager.Instance.Player1.StartTurn();
            }
            else
            {
                UIManager.Instance.SetPlayer2Turn();
                PlayerManager.Instance.Player2.StartTurn();
            }
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

    [PunRPC]
    void ReceiveMovementsPhase1(Vector2 pos, int ballIndex)
    {
        //numberOfTurnsPlayer2++;

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

        bool justWon = Utils.CheckWin(modelGrid, cell, false);
        if (justWon || isEqualityTurn)
        {
            if (justWon)
            {
                DOVirtual.DelayedCall(timeBeforeVictoryAnimation, PlayVictoryAnimation, true);
            }

            if (!isEqualityTurn && (PlayerManager.Instance.Player1.NbOfTurn != PlayerManager.Instance.Player2.NbOfTurn))
            {
                isEqualityTurn = true;
            }
            else
            {
                EndGame(justWon);
            }
        }
        else
        {
            UIManager.Instance.SetPlayer1Turn();
            PlayerManager.Instance.Player1.StartTurn();
        }
    }

    [PunRPC]
    void ReceiveMovementsPhase2(Vector2[] movements)
    {
        //numberOfTurnsPlayer2++;

        StartCoroutine(MoveCoroutine(movements));

        Cell cell = modelGrid.GetCellFromModel(movements[movements.Length - 1]);

        bool justWon = Utils.CheckWin(modelGrid, cell, false);
        if (justWon || isEqualityTurn)
        {
            if (justWon)
            {
                DOVirtual.DelayedCall(timeBeforeVictoryAnimation, PlayVictoryAnimation, true);
            }

            if (!isEqualityTurn && (PlayerManager.Instance.Player1.NbOfTurn != PlayerManager.Instance.Player2.NbOfTurn))
            {
                isEqualityTurn = true;
            }
            else
            {
                EndGame(justWon);
            }
        }
        else
        {
            StartCoroutine(waitFor((movements.Length * 1.0f) - 1.0f, UIManager.Instance.SetPlayer1Turn, PlayerManager.Instance.Player1.StartTurn));
        }
    }

    IEnumerator waitFor(float t, System.Action callback, System.Action callback2)
    {
        while ((t -= Time.deltaTime) > 0)
            yield return new WaitForEndOfFrame();

        callback();
        callback2();
    }


    //		void MovePhase2(Vector2[] movements) {
    //			Sequence sequence = DOTween.Sequence ();
    //			for (int i = 0; i < movements.Length-1; i++) {
    //				sequence.Append ();
    //				player.ChangeBallPosition (grid.GetCellFromModel (movements [i]), grid.GetCellFromModel (movements [i+1]));
    //			}
    //
    //		}

    IEnumerator MoveCoroutine(Vector2[] movements)
    {
        for (int i = 0; i < movements.Length - 1; i++)
        {

            ChangeBallPosition(modelGrid.GetCellFromModel(movements[i]), modelGrid.GetCellFromModel(movements[i + 1]));
            yield return new WaitForSeconds(1f);
        }
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