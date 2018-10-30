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
        private AIBehaviour aiBehaviour;

        public AIBehaviour AIBehaviour { get { return aiBehaviour; } }

        [SerializeField]
        private Transform marbleContainer;
        [SerializeField]
        public List<Ball> blackBalls;
        [SerializeField]
        public List<Ball> whiteBalls;

        [HideInInspector]
        public int playerScore = 0;
        [HideInInspector]
        public int otherPlayerScore = 0;

        [HideInInspector]
        public int totalPlayerScore = 0;
        [HideInInspector]
        public int totalOtherPlayerScore = 0;

        private bool isEqualityTurn = false;
        private bool nextTurnIsAI = false;
        public bool IsEqualityTurn { get { return isEqualityTurn; } }

        private bool isPlayingTutorial = false;

        private bool opponentGoesNextRound = false;
        private bool playerGoesNextRound = false;
        private int roundNumber;

        private int numberOfRound = 2;

        [HideInInspector]
        public int numberOfTurnsPlayer1 = 0;
        public int Player1NbOfTurn { get { return numberOfTurnsPlayer1; } }
        private int numberOfTurnsPlayer2 = 0;
        public int Player2NbOfTurn { get { return numberOfTurnsPlayer2; } }

        [Header("Victory Animation")]

        [SerializeField]
        private float timeBeforeVictoryAnimation;
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

        void Start ()
        {
            numberOfTurnsPlayer1 = 0;
            numberOfTurnsPlayer2 = 0;
            roundNumber = 1;

            aiBehaviour = new AIBehaviour(aiEvaluationData);

            UIManager.Instance.Restart += OnRestart;
            UIManager.Instance.NextRound += OnNextRound;
            UIManager.Instance.InviteFriend += OnInviteFriend;
            UIManager.Instance.FinishTurn += OnFinishTurn;
        }
    
        public void InitForGameStart()
        {
            DisplayMarbleContainer(true);
            CreateGrid();
            numberOfTurnsPlayer1 = 0;
            numberOfTurnsPlayer2 = 0;
        }

        public void SetPlayingTuto(bool isPlaying)
        {
            isPlayingTutorial = isPlaying;
        }

		public void DisplayMarbleContainer(bool isShown) {
			marbleContainer.gameObject.SetActive (isShown);
		}

		private void CreateGrid()
        {
			modelGrid = new ModelGrid(5, 9, FindObjectsOfType<Cell>().ToList());
            optiGrid = new OptimizedGrid(5, 9);
            optiGrid.SetPatternData(aiEvaluationData);
		}

        public void GoToNextRound()
        {
            roundNumber++;

            alreadyAnimatedPattern.Clear();

            optiGrid.Reset();
            modelGrid.Reset();

            blackBalls.Clear();
            whiteBalls.Clear();

            Ball[] balls = FindObjectsOfType<Ball>();

            Ball.resetPlaySound = true;

            foreach(Ball b in balls)
            {
                b.Reset();

                if (b.Color == BallColor.Black)
                    blackBalls.Add(b);
                else
                    whiteBalls.Add(b);

                b.MoveToResetPosition();
            }

            GameManager.Instance.isGameFinished = false;
            isEqualityTurn = false;
            GameManager.Instance.isGameStarted = true;
            
            PlayerManager.Instance.Player1.Reset();

            playerScore = 0;
            otherPlayerScore = 0;

            numberOfTurnsPlayer1 = 0;
            numberOfTurnsPlayer2 = 0;

            UIManager.Instance.ResetGame();

            SwitchPlayersColor();

            // whites begin the game
            if (PlayerManager.Instance.Player1.color == BallColor.White)
            {
                UIManager.Instance.SetPlayer1Turn();
                PlayerManager.Instance.Player1.StartTurn();
            }
            else
            {
                UIManager.Instance.SetPlayer2Turn();
            }
        }

        private void SwitchPlayersColor()
        {
            if (PlayerManager.Instance.Player1.color == BallColor.Black)
            {
                PlayerManager.Instance.SetPlayerColor(BallColor.White, true);
            }
            else
            {
                PlayerManager.Instance.SetPlayerColor(BallColor.Black);
            }
        }

		public void PlaceBallIA(Cell cell)
        {
			Ball ball = blackBalls.First ();
			blackBalls.RemoveAt (0);
			ball.DOPlace (cell);
		}

		public void PlayIAPhase1()
        {
            numberOfTurnsPlayer2++;
            if (PlayerManager.Instance.randomAI)
                StartCoroutine(aiBehaviour.GetRandomMove(optiGrid, PlayIAPhase1CalculusEnded));
            else
                StartCoroutine(aiBehaviour.GetBestMove(optiGrid, PlayIAPhase1CalculusEnded));
        }

        public void PlayIAPhase1CalculusEnded(Move move)
        {
            StartCoroutine(waitFor(3f - aiBehaviour.timeSpent, move, PlayAIMovePhase1));
        }

        public void PlayAIMovePhase1(Move move)
        {
            Cell cell = modelGrid.GetCellFromModel(move.toY, move.toX);
            PlaceBallIA(cell);

            bool justWon = Utils.CheckWinIA(modelGrid, cell);
            if (justWon || isEqualityTurn)
            {
                if (justWon)
                {
                    DOVirtual.DelayedCall(1.5f, PlayVictoryAnimation, true);
                }

                //Debug.Log("numberOfTurnsPlayer1 : " + numberOfTurnsPlayer1 + " / numberOfTurnsPlayer2 : " + numberOfTurnsPlayer2);
                nextTurnIsAI = false;
                EndGame(justWon);
            }
            else
            {
                EndAIPhase();
            }
        }

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

        IEnumerator waitFor(float t, System.Action func)
        {
            while ((t -= Time.deltaTime) > 0)
                yield return new WaitForEndOfFrame();

            func();
        }

        IEnumerator waitFor(float t, Move move, System.Action<Move> func)
        {
            while ((t -= Time.deltaTime) > 0)
                yield return new WaitForEndOfFrame();

            func(move);
        }

        public void PlayIAPhase2()
        {
            numberOfTurnsPlayer2++;

            if (PlayerManager.Instance.randomAI)
                StartCoroutine(aiBehaviour.GetRandomMove(optiGrid, PlayIAPhase2CalculusEnded));
            else
                StartCoroutine(aiBehaviour.GetBestMove(optiGrid, PlayIAPhase2CalculusEnded));
        }

        public void PlayIAPhase2CalculusEnded(Move move)
        {
            StartCoroutine(waitFor(3f - aiBehaviour.timeSpent, move, PlayAIMovePhase2));
        }

        public void PlayAIMovePhase2(Move move)
        {
            Cell cellFrom = modelGrid.GetCellFromModel(move.fromY, move.fromX);
            Cell cellTo = modelGrid.GetCellFromModel(move.toY, move.toX);

            OptiGrid.DoMove(move);
            PlayerManager.Instance.Player1.ChangeBallPosition(cellFrom, cellTo);

            bool justWon = Utils.CheckWinIA(modelGrid, cellTo);
            if (justWon || isEqualityTurn)
            {
                if (justWon)
                {
                    DOVirtual.DelayedCall(timeBeforeVictoryAnimation, PlayVictoryAnimation, true);
                }

                //Debug.Log("numberOfTurnsPlayer1 : " + numberOfTurnsPlayer1 + " / numberOfTurnsPlayer2 : " + numberOfTurnsPlayer2);
                nextTurnIsAI = false;
                EndGame(justWon);
            }
            else
            {
                EndAIPhase();
            }
        }
        
		public void Phase1TurnFinishedPlayer(Vector2 pos)
        {
            Cell cell = modelGrid.GetCellFromModel (pos);

            //Debug.Log("numberOfTurnsPlayer1 : " + numberOfTurnsPlayer1 + " / numberOfTurnsPlayer2 : " + numberOfTurnsPlayer2);

            if (GameManager.Instance.isPlayingVSIA)
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
				photonView.RPC ("ReceiveMovementsPhase1", PhotonTargets.Others, pos, cell.ball.ballId);

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
            Vector2[] movementArray = movements.ToArray ();
			Cell cell = modelGrid.GetCellFromModel (movementArray [movementArray.Length - 1]);

            //Debug.Log("numberOfTurnsPlayer1 : " + numberOfTurnsPlayer1 + " / numberOfTurnsPlayer2 : " + numberOfTurnsPlayer2);

            if (GameManager.Instance.isPlayingVSIA)
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
		}

		private void EndGame(bool justWon)
        {
            GameManager.Instance.isGameFinished = true;
            PlayerManager.Instance.Player1.EndTurn();

            if (!justWon)
                DOVirtual.DelayedCall(1.5f, DisplayRoundResult, true);
        }

        private void DisplayRoundResult()
        {
            UIManager.Instance.DisplayRoundResultPanel(true, roundNumber, playerScore, otherPlayerScore);

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
        private void PlayVictoryAnimation()
        {
            List<WinningPattern> winningPatterns = new List<WinningPattern>();
            optiGrid.GetWinningPatterns(out winningPatterns);

            WinningPattern toKeep = new WinningPattern();
            int bestScore = -1;

            foreach (WinningPattern pattern in winningPatterns)
            {
                bool alreadyDone = false;

                foreach(WinningPattern alreadyDonePattern in alreadyAnimatedPattern)
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
            ball.GetComponent<Animator>().SetTrigger("WinPhase1");
            yield return new WaitForSeconds(timeBetweenBallsPhase1AnimBegin);

            ball = modelGrid.GetCellFromModel((int)pattern.cells[1].y, (int)pattern.cells[1].x).ball;
            ball.FixSortingLayer(true);
            ball.GetComponent<Animator>().SetTrigger("WinPhase1");
            yield return new WaitForSeconds(timeBetweenBallsPhase1AnimBegin);

            ball = modelGrid.GetCellFromModel((int)pattern.cells[2].y, (int)pattern.cells[2].x).ball;
            ball.FixSortingLayer(true);
            ball.GetComponent<Animator>().SetTrigger("WinPhase1");
            yield return new WaitForSeconds(timeBetweenBallsPhase1AnimBegin);

            ball = modelGrid.GetCellFromModel((int)pattern.cells[3].y, (int)pattern.cells[3].x).ball;
            ball.FixSortingLayer(true);
            ball.GetComponent<Animator>().SetTrigger("WinPhase1");
            yield return new WaitForSeconds(timeBetweenBallsPhase1AnimBegin);

            ball = modelGrid.GetCellFromModel((int)pattern.cells[4].y, (int)pattern.cells[4].x).ball;
            ball.FixSortingLayer(true);
            ball.GetComponent<Animator>().SetTrigger("WinPhase1");
            yield return new WaitForSeconds(timeBeforePhase2AnimBegin);

            StartCoroutine(playVictoryAnimationPhase2(pattern));
        }

        IEnumerator playVictoryAnimationPhase2(WinningPattern pattern)
        {
            modelGrid.GetCellFromModel((int)pattern.cells[0].y, (int)pattern.cells[0].x).ball.GetComponent<Animator>().SetTrigger("WinPhase2");
            modelGrid.GetCellFromModel((int)pattern.cells[1].y, (int)pattern.cells[1].x).ball.GetComponent<Animator>().SetTrigger("WinPhase2");
            modelGrid.GetCellFromModel((int)pattern.cells[2].y, (int)pattern.cells[2].x).ball.GetComponent<Animator>().SetTrigger("WinPhase2");
            modelGrid.GetCellFromModel((int)pattern.cells[3].y, (int)pattern.cells[3].x).ball.GetComponent<Animator>().SetTrigger("WinPhase2");
            modelGrid.GetCellFromModel((int)pattern.cells[4].y, (int)pattern.cells[4].x).ball.GetComponent<Animator>().SetTrigger("WinPhase2");

            yield return new WaitForSeconds(timeFromPhase2AnimBeginToRoundResultPanel);

            StartCoroutine(addVictoryPoints(pattern));
        }

        IEnumerator addVictoryPoints(WinningPattern pattern)
        {
            Ball ball = modelGrid.GetCellFromModel((int)pattern.cells[0].y, (int)pattern.cells[0].x).ball;
            ball.GetComponent<Animator>().SetTrigger("ScoreCounting");
            ball.FixSortingLayer(false);
            yield return new WaitForSeconds(1.0f);

            ball = modelGrid.GetCellFromModel((int)pattern.cells[1].y, (int)pattern.cells[1].x).ball;
            ball.GetComponent<Animator>().SetTrigger("ScoreCounting");
            ball.FixSortingLayer(false);
            yield return new WaitForSeconds(1.0f);

            ball = modelGrid.GetCellFromModel((int)pattern.cells[2].y, (int)pattern.cells[2].x).ball;
            ball.GetComponent<Animator>().SetTrigger("ScoreCounting");
            ball.FixSortingLayer(false);
            yield return new WaitForSeconds(1.0f);

            ball = modelGrid.GetCellFromModel((int)pattern.cells[3].y, (int)pattern.cells[3].x).ball;
            ball.GetComponent<Animator>().SetTrigger("ScoreCounting");
            ball.FixSortingLayer(false);
            yield return new WaitForSeconds(1.0f);

            ball = modelGrid.GetCellFromModel((int)pattern.cells[4].y, (int)pattern.cells[4].x).ball;
            ball.GetComponent<Animator>().SetTrigger("ScoreCounting");
            ball.FixSortingLayer(false);
            yield return new WaitForSeconds(3.0f);

            playVictoryAnimationEnd(pattern);
        }

        public void playVictoryAnimationEnd(WinningPattern pattern)
        {
            if (IsEqualityTurn && numberOfTurnsPlayer1 != numberOfTurnsPlayer2)
            {
                if (GameManager.Instance.isPlayingVSIA)
                {
                    if (nextTurnIsAI)
                    {
                        UIManager.Instance.SetPlayer2Turn();
                        if (optiGrid.BlackBallsLeft != 0)
                            PlayIAPhase1();
                        else
                            PlayIAPhase2();
                    }
                    else
                    {
                        EndAIPhase();
                    }
                }
                else
                {
                    if (PlayerManager.Instance.Player1.color == BallColor.Black)
                    {
                        UIManager.Instance.SetPlayer1Turn();
                        PlayerManager.Instance.Player1.StartTurn();
                    }
                    else
                    {
                        UIManager.Instance.SetPlayer2Turn();
                    }
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
            if (PlayerManager.Instance.Player1.hasAlreadyJumpedOnce)
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
            playerScore += nb;
            totalPlayerScore += nb;
            UIManager.Instance.player1.SetScoreCounter(totalPlayerScore);
        }

        public void AddPlayer2Score(int nb)
        {
            otherPlayerScore += nb;
            totalOtherPlayerScore += nb;
            UIManager.Instance.player2.SetScoreCounter(totalOtherPlayerScore);
        }

		[PunRPC]
		void ReceiveMovementsPhase1(Vector2 pos, int ballIndex)
		{
            numberOfTurnsPlayer2++;

            Cell cell = modelGrid.GetCellFromModel (pos);
			Ball ball;

			if (PlayerManager.Instance.Player1.color == BallColor.White)
            {
                ball = blackBalls.Find(x => x.ballId == ballIndex);
                blackBalls.Remove(ball);
			}
            else
            {
                ball = whiteBalls.Find(x => x.ballId == ballIndex);
                whiteBalls.Remove(ball);
			}

			ball.DOPlace (cell);

            bool justWon = Utils.CheckWin(modelGrid, cell, false);
            if (justWon || isEqualityTurn)
            {
                if (justWon)
                {
                    DOVirtual.DelayedCall(timeBeforeVictoryAnimation, PlayVictoryAnimation, true);
                }

                if (!isEqualityTurn && (numberOfTurnsPlayer1 != numberOfTurnsPlayer2))
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
            numberOfTurnsPlayer2++;

            StartCoroutine(MoveCoroutine (movements));

            Cell cell = modelGrid.GetCellFromModel(movements[movements.Length - 1]);

            bool justWon = Utils.CheckWin(modelGrid, cell, false);
            if (justWon || isEqualityTurn)
            {
                if (justWon)
                {
                    DOVirtual.DelayedCall(timeBeforeVictoryAnimation, PlayVictoryAnimation, true);
                }

                if (!isEqualityTurn && (numberOfTurnsPlayer1 != numberOfTurnsPlayer2))
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
			for (int i = 0; i < movements.Length-1; i++) {

                PlayerManager.Instance.Player1.ChangeBallPosition (modelGrid.GetCellFromModel (movements [i]), modelGrid.GetCellFromModel (movements [i+1]));
				yield return new WaitForSeconds (1f);
			}
        }

		public void OnRestart()
        {
            GameManager.Instance.RestartConnection();
            SceneManager.LoadScene(1);
        }

        public void OnNextRound()
        {
            if (playerGoesNextRound)
                return;

            playerGoesNextRound = true;

            PhotonView photonView = PhotonView.Get(this);
            if (PhotonNetwork.connectedAndReady)
                photonView.RPC("SendNextRound", PhotonTargets.Others);

            if (opponentGoesNextRound || GameManager.Instance.isPlayingVSIA)
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

		public void OnInviteFriend() {
			FB.AppRequest ("Viens jouer à Item");

			//fbManager.LoadInvitableFriends ();
		}

	}