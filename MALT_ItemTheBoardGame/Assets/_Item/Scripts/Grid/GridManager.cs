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
using AppAdvisory.Ads;

namespace AppAdvisory.Item {

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
        private Connection connection;
        public FBManager fbManager;

        private ModelGrid modelGrid;
        public ModelGrid ModelGrid { get { return modelGrid; } }

        private OptimizedGrid optiGrid;
        public OptimizedGrid OptiGrid { get { return optiGrid; } }

        [SerializeField]
        private AIEvaluationData aiEvaluationData;
        private AIBehaviour aiBehaviour;

        public AIBehaviour AIBehaviour { get { return aiBehaviour; } }

        [SerializeField]
        public Player playerPrefab;
        [SerializeField]
        public Ball whiteBallPrefab;
        [SerializeField]
        public Ball blackBallPrefab;

        [SerializeField]
        private Transform marbleContainer;
        [SerializeField]
        public List<Ball> blackBalls;
        [SerializeField]
        public List<Ball> whiteBalls;

        [SerializeField]
        private UIManager uiManager;

        [HideInInspector]
        public Player player;
        private string playerName;
        private string playerPicURL;

        public float timeToLaunchGameVSIA = 4;

        private bool isGameFinished = false;

        private int playerScore = 0;
        private int otherPlayerScore = 0;

        private int totalPlayerScore = 0;
        private int totalOtherPlayerScore = 0;

        private bool isEqualityTurn = false;
        private bool nextTurnIsAI = false;
        public bool IsEqualityTurn { get { return isEqualityTurn; } }
        private bool isGameStarted = false;
        private bool isPlayingVSIA = false;

        private bool isPlayingTutorial = false;

        private bool opponentGoesNextRound = false;
        private bool playerGoesNextRound = false;
        private int roundNumber;

        [SerializeField]
        private Sprite IASprite;

        private bool lookingForGame = false;

        [SerializeField]
        private bool disableAI = false;

        [SerializeField]
        private int numberOfRound;

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

        void Start ()
        {
            Options.Init();

            numberOfTurnsPlayer1 = 0;
            numberOfTurnsPlayer2 = 0;
            roundNumber = 1;

            aiBehaviour = new AIBehaviour(aiEvaluationData);

			//DisplayMarbleContainer (false);

            connection = GetComponent<Connection> ();

            uiManager.Init();
            uiManager.NextRound += OnNextRound;
			uiManager.Restart += OnRestart;
			uiManager.InviteFriend += OnInviteFriend;
            uiManager.FinishTurn += OnFinishTurn;

			connection.ApplyUserIdAndConnect ();

			fbManager = FindObjectOfType<FBManager> ();
            if (fbManager)
            {
                Debug.Log("fbManager != null");

                playerName = fbManager.pName;
                playerPicURL = fbManager.pUrlPic;

                fbManager.NameLoaded += OnNameLoaded;
                fbManager.PicURLLoaded += OnPicURLLoaded;

                //fbManager.FacebookConnect += OnFacebookConnect;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartGameVSIA();
            }
        }

        public void StartLookingForGame()
        {
            PhotonNetwork.JoinRandomRoom();
            lookingForGame = true;
            DOVirtual.DelayedCall(timeToLaunchGameVSIA, StartGameVSIA, true);
        }

        public void StartGameVSIA() {
			if (isGameStarted)
				return;

			connection.enabled = false;
			PhotonNetwork.LeaveRoom ();
			PhotonNetwork.Disconnect ();

			uiManager.DisplayWaitingForPlayerPanel (false);
			DisplayMarbleContainer (true);
			CreateGrid ();

			isGameStarted = true;
            numberOfTurnsPlayer1 = 0;
            numberOfTurnsPlayer2 = 0;
            isPlayingVSIA = true;

            player = CreatePlayer(BallColor.White);
			uiManager.InitPlayer2(BallColor.Black);

            uiManager.SetPlayer2Name(GetIAName());
            uiManager.SetPlayer2Pic(GetIASprite());
            uiManager.DisplayTurnSwitchPhase1(true);
            uiManager.DisplayYourTurn(true);
            uiManager.SetPlayer1Turn(player.StartTurn);
		}

        public void SetPlayingTuto(bool isPlaying)
        {
            isPlayingTutorial = isPlaying;
        }

        private string GetIAName() {
            return "IA";
		}

        private Sprite GetIASprite() {
            return IASprite;
        }

		private void DisplayMarbleContainer(bool isShown) {
			marbleContainer.gameObject.SetActive (isShown);
		}

		private void OnFacebookConnect() {
			connection.ApplyUserIdAndConnect ();
		}

		private void OnNameLoaded(string name) {
			playerName = name;
            uiManager.DisplayPlayer1(true);
			print ("onnameloaded " + name);
		}

		private void OnPicURLLoaded(string url) {
			playerPicURL = url;
            uiManager.DisplayPlayer1(true);
            print ("onpicurlloaded" + url);
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

            foreach(Ball b in balls)
            {
                b.Reset();

                if (b.Color == BallColor.Black)
                    blackBalls.Add(b);
                else
                    whiteBalls.Add(b);

                b.MoveToResetPosition();
            }
            
            isGameFinished = false;
            isEqualityTurn = false;
            isGameStarted = true;
            
            player.Reset();

            playerScore = 0;
            otherPlayerScore = 0;

            numberOfTurnsPlayer1 = 0;
            numberOfTurnsPlayer2 = 0;

            uiManager.ResetGame();

            SwitchPlayersColor();

            // whites begin the game
            if (player.color == BallColor.White)
            {
                uiManager.SetPlayer1Turn(player.StartTurn);
                uiManager.DisplayYourTurn(true);
            }
            else
            {
                uiManager.SetPlayer2Turn(null);
                uiManager.DisplayOpponentTurn(true);
            }
        }

		private Player CreatePlayer(BallColor color)
        {
			Player player = Instantiate (playerPrefab);
			player.ballPrefab = color == BallColor.White ? whiteBallPrefab : blackBallPrefab;
			player.SetGrid (modelGrid, optiGrid);
			player.OnPhase1TurnFinished += Phase1TurnFinishedPlayer;
			player.OnPhase2TurnFinished += Phase2TurnFinishedPlayer;
			return player;
		}

        private void SwitchPlayersColor()
        {
            if (player.color == BallColor.Black)
            {
                player.ballPrefab = blackBallPrefab;
                uiManager.InitPlayer2(BallColor.White);
            }
            else
            {
                player.ballPrefab = whiteBallPrefab;
                uiManager.InitPlayer2(BallColor.Black);
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

                if (!isEqualityTurn && (numberOfTurnsPlayer1 != numberOfTurnsPlayer2))
                {
                    nextTurnIsAI = false;
                    isEqualityTurn = true;
                }
                else
                {
                    nextTurnIsAI = false;
                    Win(cell, justWon);
                }
            }
            else
            {
                EndAIPhase();
            }
        }

        bool alreadyPassed = false;
        public void EndAIPhase()
        {
            if (player.ballCount == 0 && !alreadyPassed)
            {
                uiManager.DisplayTurnSwitchPhase1(false);
                uiManager.DisplayTurnSwitchPhase2(true);
                alreadyPassed = true;

                if (isPlayingTutorial)
                    uiManager.SetPlayer1Turn(uiManager.DisplayTutorialPhase2Movement);
                else
                    uiManager.SetPlayer1Turn(player.StartTurn);
            }
            else
            {
                uiManager.SetPlayer1Turn(player.StartTurn);
                
            }
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
            player.ChangeBallPosition(cellFrom, cellTo);

            bool justWon = Utils.CheckWinIA(modelGrid, cellTo);
            if (justWon || isEqualityTurn)
            {
                DOVirtual.DelayedCall(timeBeforeVictoryAnimation, PlayVictoryAnimation, true);
                if (!isEqualityTurn && (numberOfTurnsPlayer1 != numberOfTurnsPlayer2))
                {
                    nextTurnIsAI = false;
                    isEqualityTurn = true;
                }
                else
                {
                    nextTurnIsAI = false;
                    Win(cellTo, justWon);
                }
            }
            else
            {
                EndAIPhase();
            }
        }
        
		public void Phase1TurnFinishedPlayer(Vector2 pos)
        {
            uiManager.DisplayTurnSwitchPhase1(true);
            Cell cell = modelGrid.GetCellFromModel (pos);

			if (isPlayingVSIA)
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
                        Win(cell, justWon);
                    }
                }
                else
                {
                    
                    if (disableAI) // debug feature to test without AI
                    {
                        uiManager.SetPlayer1Turn(player.StartTurn);
                    }
                    else
                    {
                        uiManager.SetPlayer2Turn(null);
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
                    DOVirtual.DelayedCall(timeBeforeVictoryAnimation, PlayVictoryAnimation, true);
                    if (!isEqualityTurn && (numberOfTurnsPlayer1 != numberOfTurnsPlayer2))
                    {
                        nextTurnIsAI = false;
                        isEqualityTurn = true;
                    }
                    else
                    {
                        nextTurnIsAI = false;
                        Win(cell, justWon);
                    }
                }
                else
                {
                    uiManager.SetPlayer2Turn(null);
				}
            }
        }

		public void Phase2TurnFinishedPlayer(List<Vector2> movements)
        {
            uiManager.DisplayTurnSwitchPhase1(false);
            uiManager.DisplayTurnSwitchPhase2(true);
            Vector2[] movementArray = movements.ToArray ();
			Cell cell = modelGrid.GetCellFromModel (movementArray [movementArray.Length - 1]);

			if (isPlayingVSIA)
            {
                bool justWon = Utils.CheckWin(modelGrid, cell, false);
                if (justWon || isEqualityTurn)
                {
                    DOVirtual.DelayedCall(1.5f, PlayVictoryAnimation, true);
                    if (!isEqualityTurn && (numberOfTurnsPlayer1 != numberOfTurnsPlayer2))
                    {
                        nextTurnIsAI = true;
                        isEqualityTurn = true;
                    }
                    else
                    {
                        nextTurnIsAI = true;
                        Win(cell, justWon);
                    }
                }
                else
                {
                    uiManager.SetPlayer2Turn(null);
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
                        Win(cell, justWon);
                    }
                }
                else
                {
                    uiManager.SetPlayer2Turn(null);
				}
			}
		}

		private void Win(Cell cell, bool justWon)
        {
            /*List<WinningPattern> winningPatterns;
            OptiGrid.GetWinningPatterns(out winningPatterns);

            playerScore = 0;
            otherPlayerScore = 0;

            // if we have a winningPattern for the color, notify it
            foreach(WinningPattern pattern in winningPatterns)
            {
                if (pattern.color == (CellColor)player.color)
                {
                    playerScore = GetWinningPatternScore(pattern);
                }
                else
                {
                    otherPlayerScore = GetWinningPatternScore(pattern);
                }
            }

            totalPlayerScore += playerScore;
            totalOtherPlayerScore += otherPlayerScore;
            */
            isGameFinished = true;
            player.EndTurn();

            if (!justWon)
                DOVirtual.DelayedCall(1.5f, DisplayRoundResult, true);
        }

        private void DisplayRoundResult()
        {
            uiManager.DisplayRoundResultPanel(true, roundNumber, playerScore, otherPlayerScore);

            if (roundNumber == numberOfRound)
            {
                uiManager.roundResultPanel.ActivateGameResultsButton(true);
            }
            else
            {
                uiManager.roundResultPanel.ActivateNextRoundButton(true);
            }
        }

        List<WinningPattern> alreadyAnimatedPattern = new List<WinningPattern>();
        private void PlayVictoryAnimation()
        {
            List<WinningPattern> winningPatterns = new List<WinningPattern>();
            optiGrid.GetWinningPatterns(out winningPatterns);

            foreach(WinningPattern pattern in winningPatterns)
            {
                bool alreadyDone = false;

                foreach(WinningPattern alreadyDonePattern in alreadyAnimatedPattern)
                {
                    if (pattern.IsSame(pattern))
                    {
                        alreadyDone = true;
                        break;
                    }
                }

                if (alreadyDone)
                    continue;

                alreadyAnimatedPattern.Add(pattern);

                StartCoroutine(playVictoryAnimationPhase1(pattern));
            }
        }

        IEnumerator playVictoryAnimationPhase1(WinningPattern pattern)
        {
            modelGrid.GetCellFromModel((int)pattern.cells[0].y, (int)pattern.cells[0].x).ball.GetComponent<Animator>().SetTrigger("WinPhase1");
            yield return new WaitForSeconds(timeBetweenBallsPhase1AnimBegin);
            modelGrid.GetCellFromModel((int)pattern.cells[1].y, (int)pattern.cells[1].x).ball.GetComponent<Animator>().SetTrigger("WinPhase1");
            yield return new WaitForSeconds(timeBetweenBallsPhase1AnimBegin);
            modelGrid.GetCellFromModel((int)pattern.cells[2].y, (int)pattern.cells[2].x).ball.GetComponent<Animator>().SetTrigger("WinPhase1");
            yield return new WaitForSeconds(timeBetweenBallsPhase1AnimBegin);
            modelGrid.GetCellFromModel((int)pattern.cells[3].y, (int)pattern.cells[3].x).ball.GetComponent<Animator>().SetTrigger("WinPhase1");
            yield return new WaitForSeconds(timeBetweenBallsPhase1AnimBegin);
            modelGrid.GetCellFromModel((int)pattern.cells[4].y, (int)pattern.cells[4].x).ball.GetComponent<Animator>().SetTrigger("WinPhase1");

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
            modelGrid.GetCellFromModel((int)pattern.cells[0].y, (int)pattern.cells[0].x).ball.GetComponent<Animator>().SetTrigger("ScoreCounting");
            yield return new WaitForSeconds(1.0f);
            modelGrid.GetCellFromModel((int)pattern.cells[1].y, (int)pattern.cells[1].x).ball.GetComponent<Animator>().SetTrigger("ScoreCounting");
            yield return new WaitForSeconds(1.0f);
            modelGrid.GetCellFromModel((int)pattern.cells[2].y, (int)pattern.cells[2].x).ball.GetComponent<Animator>().SetTrigger("ScoreCounting");
            yield return new WaitForSeconds(1.0f);
            modelGrid.GetCellFromModel((int)pattern.cells[3].y, (int)pattern.cells[3].x).ball.GetComponent<Animator>().SetTrigger("ScoreCounting");
            yield return new WaitForSeconds(1.0f);
            modelGrid.GetCellFromModel((int)pattern.cells[4].y, (int)pattern.cells[4].x).ball.GetComponent<Animator>().SetTrigger("ScoreCounting");

            yield return new WaitForSeconds(3.0f);

            playVictoryAnimationEnd(pattern);
        }

        public void playVictoryAnimationEnd(WinningPattern pattern)
        {
            if (IsEqualityTurn)
            {
                if (isPlayingVSIA)
                {
                    if (nextTurnIsAI)
                    {
                        uiManager.SetPlayer2Turn(null);
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
                    if (player.color == BallColor.Black)
                    {
                        uiManager.SetPlayer1Turn(player.StartTurn);
                    }
                    else
                    {
                        uiManager.SetPlayer2Turn(null);
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

            List<Move> moves = optiGrid.GetAvailableMoves(cell);

            foreach(Move move in moves)
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
                Debug.Log("no winning pattern");
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
            uiManager.PopScoreParticle(ball);
        }

        public void AddPlayer1Score(int nb)
        {
            playerScore += nb;
            totalPlayerScore += nb;
            uiManager.player1.SetScoreCounter(totalPlayerScore);
        }

        public void AddPlayer2Score(int nb)
        {
            otherPlayerScore += nb;
            totalOtherPlayerScore += nb;
            uiManager.player2.SetScoreCounter(totalOtherPlayerScore);
        }

        public void ShowAds()
		{
			int count = PlayerPrefs.GetInt("GAMEOVER_COUNT",0);
			count++;

			#if APPADVISORY_ADS
			/*if(count > numberOfPlayToShowInterstitial)
			{

				if(AdsManager.instance.IsReadyInterstitial())
				{
					PlayerPrefs.SetInt("GAMEOVER_COUNT",0);
					AdsManager.instance.ShowInterstitial();
				}
			}
			else
			{
				PlayerPrefs.SetInt("GAMEOVER_COUNT", count);
			}
			PlayerPrefs.Save();*/
			#else
			if(count >= numberOfPlayToShowInterstitial)
			{
			Debug.LogWarning("To show ads, please have a look at Very Simple Ad on the Asset Store, or go to this link: " + VerySimpleAdsURL);
			PlayerPrefs.SetInt("GAMEOVER_COUNT",0);
			}
			else
			{
			PlayerPrefs.SetInt("GAMEOVER_COUNT", count);
			}
			PlayerPrefs.Save();
			#endif
		}

		void RestartConnection() {
			connection.enabled = false;
			player.EndTurn ();

			PhotonNetwork.LeaveRoom ();
			PhotonNetwork.Disconnect ();
		}

		[PunRPC]
		void ReceiveMovementsPhase1(Vector2 pos, int ballIndex)
		{
			if (player.ballCount > 0)
            {
                uiManager.DisplayTurnSwitchPhase1(true);
            }
            else
            {
                uiManager.DisplayTurnSwitchPhase1(false);
                uiManager.DisplayTurnSwitchPhase2(true);
            }

            numberOfTurnsPlayer2++;

            Cell cell = modelGrid.GetCellFromModel (pos);
			Ball ball;

			if (player.color == BallColor.White)
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
                if (!isEqualityTurn && (numberOfTurnsPlayer1 != numberOfTurnsPlayer2))
                {
                    isEqualityTurn = true;
                }
                else
                {
                    Win(cell, justWon);
                }
            }
            else
            {
                uiManager.SetPlayer1Turn(player.StartTurn);
            }
		}
			
		[PunRPC]
		void ReceiveMovementsPhase2(Vector2[] movements)
        {
            uiManager.DisplayTurnSwitchPhase1(false);
            uiManager.DisplayTurnSwitchPhase2(true);

            numberOfTurnsPlayer2++;

            StartCoroutine(MoveCoroutine (movements));

            Cell cell = modelGrid.GetCellFromModel(movements[movements.Length - 1]);

            bool justWon = Utils.CheckWin(modelGrid, cell, false);
            if (justWon || isEqualityTurn)
            {
                if (!isEqualityTurn && (numberOfTurnsPlayer1 != numberOfTurnsPlayer2))
                {
                    isEqualityTurn = true;
                }
                else
                {
                    Win(cell, justWon);
                }
            }
            else
            {
                uiManager.SetPlayer1Turn(player.StartTurn);
            }
		}

		private void SendName(string name) {
			PhotonView photonView = PhotonView.Get(this);
			photonView.RPC("ReceiveName", PhotonTargets.Others, name);
		}

		private void SendPicURL(string picURL) {
			PhotonView photonView = PhotonView.Get(this);
			photonView.RPC("ReceivePicURL", PhotonTargets.Others, picURL);
		}

		[PunRPC]
		private void ReceiveName(string name) {
			uiManager.SetPlayer2Name (name);
			return;
		}

		[PunRPC] 
		private void ReceivePicURL(string picURL) {
			StartCoroutine (Utils.LoadSpriteFromURL (picURL, (sprite) => {
				uiManager.SetPlayer2Pic(sprite);
			}));
		}

		override public void OnLeftRoom()
		{
			print ("leftRoom");
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

				player.ChangeBallPosition (modelGrid.GetCellFromModel (movements [i]), modelGrid.GetCellFromModel (movements [i+1]));
				yield return new WaitForSeconds (1f);
			}
        }
			
		public override void OnJoinedRoom()
		{

			if (PhotonNetwork.room.PlayerCount == 2)
			{
				uiManager.DisplayWaitingForPlayerPanel (false);
				DisplayMarbleContainer (true);
				CreateGrid ();
				isGameStarted = true;
                numberOfTurnsPlayer1 = 0;
                numberOfTurnsPlayer2 = 0;

                player = CreatePlayer (BallColor.Black);

				uiManager.InitPlayer1(BallColor.Black);
				uiManager.InitPlayer2(BallColor.White);
				uiManager.SetPlayer2Turn(null);

                uiManager.DisplayTurnSwitchPhase1(true);

                uiManager.SetPlayer1Name (playerName);
				StartCoroutine (Utils.LoadSpriteFromURL (playerPicURL, (sprite) => {
					uiManager.SetPlayer1Pic(sprite);
				}));


				print ("2 players in the room, sending player name : " + playerName);
				SendName (playerName);
				SendPicURL (playerPicURL);
			}
			else
			{
				print ("alone in the room, name is : " + playerName);
				uiManager.DisplayWaitingForPlayerPanel (true);
				uiManager.InitPlayer1(BallColor.White);
				uiManager.SetPlayer1Name (playerName);
				StartCoroutine (Utils.LoadSpriteFromURL (playerPicURL, (sprite) => {
					uiManager.SetPlayer1Pic(sprite);
				}));

				uiManager.DisplayYourTurn (false);
			}
		}

		public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
		{
			if (PhotonNetwork.room.PlayerCount == 2)
			{
				uiManager.DisplayWaitingForPlayerPanel (false);
				DisplayMarbleContainer (true);
				Debug.Log("Other player arrived");
				CreateGrid ();
				isGameStarted = true;
                numberOfTurnsPlayer1 = 0;
                numberOfTurnsPlayer2 = 0;

                uiManager.DisplayTurnSwitchPhase1(true);
                uiManager.DisplayYourTurn(true);
                uiManager.SetPlayer1Turn(player.StartTurn);

				player = CreatePlayer (BallColor.White);
				uiManager.InitPlayer2(BallColor.Black);

				SendName (playerName);
				SendPicURL (playerPicURL);

			} else 
			{
				print("alone");
				uiManager.DisplayWaitingForPlayerPanel (true);
			}
		}
			
		public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
		{
			Debug.Log ("Other player disconnected! isInactive: " + otherPlayer.IsInactive);
			if (isGameFinished)
				return;

			connection.enabled = false;
			PhotonNetwork.LeaveRoom ();
			PhotonNetwork.Disconnect ();
            
			uiManager.DisplayForfeit(true);
		}

		public void OnRestart()
        {
            PhotonNetwork.LoadLevel (1);
		}

        public void OnNextRound()
        {
            if (playerGoesNextRound)
                return;

            playerGoesNextRound = true;

            PhotonView photonView = PhotonView.Get(this);
            if (PhotonNetwork.connectedAndReady)
                photonView.RPC("SendNextRound", PhotonTargets.Others);

            if (opponentGoesNextRound || isPlayingVSIA)
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

}