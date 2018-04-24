﻿using System.Collections;
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
    public class GridManager : PunBehaviour {

        public int numberOfPlayToShowInterstitial = 1;
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
        private GameObject horizontalLine;
        [SerializeField]
        private GameObject diagonalLine;

        [SerializeField]
        private Transform marbleContainer;
        [SerializeField]
        public List<Ball> blackBalls;
        [SerializeField]
        public List<Ball> whiteBalls;
        [SerializeField]
        private UIManager uiManager;

        private Player player;
        private string playerName;
        private string playerPicURL;

        public float timeToLaunchGameVSIA = 4;

        private bool isGameFinished = false;
        private int isWon = 0;
        private int playerScore = 0;
        private int otherPlayerScore = 0;
        private bool isEqualityTurn = false;
        public bool IsEqualityTurn { get { return isEqualityTurn; } }
        private bool isGameStarted = false;
        private bool isPlayingVSIA = false;

        [SerializeField]
        private bool disableAI = false;

        public int numberOfTurnsPlayer1 = 0;
        public int Player1NbOfTurn { get { return numberOfTurnsPlayer1; } }
        private int numberOfTurnsPlayer2 = 0;
        public int Player2NbOfTurn { get { return numberOfTurnsPlayer2; } }

        void Start ()
        {
            numberOfTurnsPlayer1 = 0;
            numberOfTurnsPlayer2 = 0;

            aiBehaviour = new AIBehaviour(aiEvaluationData);
            
			DisplayMarbleContainer (false);

            //DOVirtual.DelayedCall(timeToLaunchGameVSIA, StartGameVSIA);

            connection = GetComponent<Connection> ();

			uiManager.Init();
			uiManager.Restart += OnRestart;
			uiManager.InviteFriend += OnInviteFriend;

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

        public void StartGameVSIA() {
			if (isGameStarted)
				return;

			connection.enabled = false;
			PhotonNetwork.LeaveRoom ();
			PhotonNetwork.Disconnect ();


			uiManager.DisPlayWaitingForPlayerPanel (false);
			DisplayMarbleContainer (true);
			CreateGrid ();

			isGameStarted = true;
            numberOfTurnsPlayer1 = 0;
            numberOfTurnsPlayer2 = 0;
            isPlayingVSIA = true;

			uiManager.DisplayYourTurn (true);
			uiManager.DisplayPhase1Text (true);
			uiManager.SetPlayer1Turn();
			//isPlayer1 = true;

			player = CreatePlayer (BallColor.White);
			uiManager.InitPlayer2(BallColor.Black);

			uiManager.DisplayPlayer1Arrow (true);
			uiManager.SetPlayer2Name (GetIAName());

			player.StartTurn ();
		}

		private string GetIAName() {
            return "IA";
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

		private Player CreatePlayer(BallColor color)
        {
			Player player = Instantiate (playerPrefab);
			player.ballPrefab = color == BallColor.White ? whiteBallPrefab : blackBallPrefab;
			player.SetGrid (modelGrid, optiGrid);
			player.OnPhase1TurnFinished += Phase1TurnFinishedPlayer;
			player.OnPhase2TurnFinished += Phase2TurnFinishedPlayer;
			return player;
		}

		public void PlaceBallIA(Cell cell)
        {
			Ball ball = blackBalls.First ();
			blackBalls.RemoveAt (0);
			ball.DOPlace (cell);
		}

		public void PlayIAPhase1(Cell lastMove)
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
            bool end = false;
            if (Utils.CheckWinIA(modelGrid, cell) || isEqualityTurn)
            {
                if (!isEqualityTurn && (numberOfTurnsPlayer1 != numberOfTurnsPlayer2))
                {
                    isEqualityTurn = true;
                }
                else
                {
                    Win(cell);
                    end = true;
                }
            }
            
            if (!end)
            {
                EndAIPhase();
            }
        }

        public void EndAIPhase()
        {
            if (player.ballCount == 0)
            {
                uiManager.DisplayPhase1Text(false);
                uiManager.DisplayPhase2Text(true);
            }

            player.StartTurn();

            uiManager.DisplayYourTurn(true);
            uiManager.SetPlayer1Turn();
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

            bool end = false;
            if (Utils.CheckWinIA(modelGrid, cellTo) || isEqualityTurn)
            {
                if (!isEqualityTurn && (numberOfTurnsPlayer1 != numberOfTurnsPlayer2))
                {
                    isEqualityTurn = true;
                }
                else
                {
                    Win(cellTo);
                    end = true;
                }
            }
            
            if (!end)
            {
                EndAIPhase();
            }
        }
        
		public void Phase1TurnFinishedPlayer(Vector2 pos) {
			uiManager.DisplayPhase1Text (true);
			Cell cell = modelGrid.GetCellFromModel (pos);

            bool end = false;
			if (isPlayingVSIA)
            {
                if (Utils.CheckWin(modelGrid, cell, false) || isEqualityTurn)
                {
                    if (!isEqualityTurn && (numberOfTurnsPlayer1 != numberOfTurnsPlayer2))
                    {
                        isEqualityTurn = true;
                    }
                    else
                    {
                        Win(cell);
                        end = true;
                    }
                }
                
                if (!end)
                {
                    uiManager.DisplayYourTurn(false);
                    uiManager.SetPlayer2Turn();
                    
                    if (disableAI) // debug feature to test without AI
                    {
                        player.StartTurn();

                        uiManager.DisplayYourTurn(true);
                        uiManager.SetPlayer1Turn();
                    }
                    else
                    {
                        PlayIAPhase1(cell);
                    }
                }
            }
            else
            {
				PhotonView photonView = PhotonView.Get(this);
				photonView.RPC ("ReceiveMovementsPhase1", PhotonTargets.Others, pos);
                
                if (Utils.CheckWin(modelGrid, cell, false) || isEqualityTurn)
                {
                    if (!isEqualityTurn && (numberOfTurnsPlayer1 != numberOfTurnsPlayer2))
                    {
                        isEqualityTurn = true;
                    }
                    else
                    {
                        Win(cell);
                        end = true;
                    }
                }

                if (!end)
				{
					SetPlayerTurnOnEnd ();
				}
			}


		}

		public void Phase2TurnFinishedPlayer(List<Vector2> movements)
		{
			uiManager.DisplayPhase1Text (false);
			uiManager.DisplayPhase2Text (true);
            Vector2[] movementArray = movements.ToArray ();
			Cell cell = modelGrid.GetCellFromModel (movementArray [movementArray.Length - 1]);
            bool end = false;

			if (isPlayingVSIA)
            {
                if (Utils.CheckWin(modelGrid, cell, false) || isEqualityTurn)
                {
                    if (!isEqualityTurn && (numberOfTurnsPlayer1 != numberOfTurnsPlayer2))
                    {
                        isEqualityTurn = true;
                    }
                    else
                    {
                        Win(cell);
                        end = true;
                    }
                }
                if (!end)
                {
                    uiManager.DisplayYourTurn(false);
                    uiManager.SetPlayer2Turn();
                    PlayIAPhase2();
                }
			}
            else
            {
				PhotonView photonView = PhotonView.Get(this);
				photonView.RPC("ReceiveMovementsPhase2", PhotonTargets.Others, movementArray);

                if (Utils.CheckWin(modelGrid, cell, false) || isEqualityTurn)
                {
                    if (!isEqualityTurn && (numberOfTurnsPlayer1 != numberOfTurnsPlayer2))
                    {
                        isEqualityTurn = true;
                    }
                    else
                    {
                        Win(cell);
                        end = true;
                    }
                }

                if (!end)
                {
                    SetPlayerTurnOnEnd ();
				}
			}
		}

		private void Win(Cell cell)
        {
            bool playerWin = false, otherPlayerWin = false;

            List<WinningPattern> winningPatterns;
            OptiGrid.GetWinningPatterns(out winningPatterns);

            // if we have a winningPattern for the color, notify it
            foreach(WinningPattern pattern in winningPatterns)
            {
                if (pattern.color == (CellColor)player.color)
                {
                    playerWin = true;
                    playerScore = GetWinningPatternScore(pattern);
                }
                else
                {
                    otherPlayerWin = true;
                    otherPlayerScore = GetWinningPatternScore(pattern);
                }
            }
            
            // adjust win/loose if with score
            if (playerScore > otherPlayerScore)
            {
                otherPlayerWin = false;
            }
            else if (otherPlayerScore > playerScore)
            {
                playerWin = false;
            }

            if (playerWin && otherPlayerWin)
            {
                isWon = 0;
            }
            else if (playerWin)
            {
                isWon = 1;

            }
            else if (otherPlayerWin)
            {
                isWon = -1;
            }

            isGameFinished = true;
            player.EndTurn();

            PlayVictoryAnimation();

            /*if (!isPlayingVSIA) {
		    	PhotonView photonView = PhotonView.Get (this);
		    	photonView.RPC ("ReceiveWin", PhotonTargets.Others, new Vector2 (cell.x, cell.y));
		    	player.EndTurn ();
		    }*/
        }

        private void DisplayEndPanel()
        {
            uiManager.DisplayEndGamePanel(true);
            RestartConnection();

            if (isWon == 1)
            {
                uiManager.DisplayYouWon(true, playerScore, otherPlayerScore);
            }
            else if (isWon == -1)
            {
                uiManager.DisplayYouLost(true, playerScore, otherPlayerScore);
            }
            else
            {
                uiManager.DisplayDraw(true, playerScore, otherPlayerScore);
            }

            uiManager.DisplayRestartButton(true);
        }

        private void PlayVictoryAnimation()
        {
            List<WinningPattern> winningPatterns = new List<WinningPattern>();
            optiGrid.GetWinningPatterns(out winningPatterns);

            foreach(WinningPattern pattern in winningPatterns)
            {
                modelGrid.GetCellFromModel((int)pattern.cells[0].y, (int)pattern.cells[0].x).ball.transform.DOScale(1.1f, 1f);
                modelGrid.GetCellFromModel((int)pattern.cells[1].y, (int)pattern.cells[1].x).ball.transform.DOScale(1.1f, 1f);
                modelGrid.GetCellFromModel((int)pattern.cells[2].y, (int)pattern.cells[2].x).ball.transform.DOScale(1.1f, 1f);
                modelGrid.GetCellFromModel((int)pattern.cells[3].y, (int)pattern.cells[3].x).ball.transform.DOScale(1.1f, 1f);
                modelGrid.GetCellFromModel((int)pattern.cells[4].y, (int)pattern.cells[4].x).ball.transform.DOScale(1.1f, 1f);
            }

            DOVirtual.DelayedCall(1.2f, DisplayEndPanel, true);
        }

        public void HighlightAvailableMoveCells(Cell cell)
        {
            modelGrid.ResetCellsColor();

            List<Move> moves = optiGrid.GetAvailableMoves(cell);

            foreach(Move move in moves)
            {
                Cell toCell = modelGrid.GetCellFromModel(move.toY, move.toX);
                toCell.GetComponent<SpriteRenderer>().color = Color.red;
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

		public void ShowAds()
		{
			int count = PlayerPrefs.GetInt("GAMEOVER_COUNT",0);
			count++;

			#if APPADVISORY_ADS
			if(count > numberOfPlayToShowInterstitial)
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
			PlayerPrefs.Save();
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
		void ReceiveMovementsPhase1(Vector2 pos)
		{

			if (player.ballCount > 0) {
				uiManager.DisplayPhase1Text (true);
			} else {
				uiManager.DisplayPhase1Text (false);
				uiManager.DisplayPhase2Text (true);
			}

            numberOfTurnsPlayer2++;
            Cell cell = modelGrid.GetCellFromModel (pos);
			Ball ball;

			if (player.color == BallColor.White) {
				ball = blackBalls.First ();
				blackBalls.RemoveAt (0);
			} else {
				ball = whiteBalls.First ();
				whiteBalls.RemoveAt (0);
			}

			ball.DOPlace (cell);


			player.StartTurn();

			SetPlayerTurnOnReceive();
		}
			
		[PunRPC]
		void ReceiveMovementsPhase2(Vector2[] movements)
		{
			uiManager.DisplayPhase1Text(false);
			uiManager.DisplayPhase2Text(true);

            numberOfTurnsPlayer2++;

            StartCoroutine(MoveCoroutine (movements));
			uiManager.DisplayYourTurn(true);
			player.StartTurn();

            SetPlayerTurnOnReceive();
		}

		[PunRPC]
		void ReceiveWin(Vector2 position) {
			print ("receive win");
			uiManager.ResetPlayerTurn ();

			//player.EndTurn ();
			//uiManager.DisplayYourTurn (false);

			Cell cell = modelGrid.GetCellFromModel (position);
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

			/*if (isPlayer1)
				uiManager.SetPlayer2Name (name);
			else
				uiManager.SetPlayer1Name (name);*/
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

		IEnumerator MoveCoroutine(Vector2[] movements) {

			for (int i = 0; i < movements.Length-1; i++) {

				player.ChangeBallPosition (modelGrid.GetCellFromModel (movements [i]), modelGrid.GetCellFromModel (movements [i+1]));
				yield return new WaitForSeconds (1f);
			}

		}
			
		void SetPlayerTurnOnReceive() {
			uiManager.DisplayYourTurn (true);
			uiManager.SetPlayer1Turn ();

			return;
			/*
            if(isPlayer1) {
				uiManager.SetPlayer1Turn();
			} else {
				uiManager.SetPlayer2Turn();
			}
            */
		}

		void SetPlayerTurnOnEnd() {
			uiManager.DisplayYourTurn (false);
			uiManager.SetPlayer2Turn ();

			return;
			/*if(isPlayer1) {
				uiManager.SetPlayer2Turn();
			} else {
				uiManager.SetPlayer1Turn();
			}*/
		}



		public override void OnJoinedRoom()
		{

			if (PhotonNetwork.room.PlayerCount == 2)
			{
				uiManager.DisPlayWaitingForPlayerPanel (false);
				DisplayMarbleContainer (true);
				CreateGrid ();
				isGameStarted = true;
                numberOfTurnsPlayer1 = 0;
                numberOfTurnsPlayer2 = 0;

                player = CreatePlayer (BallColor.Black);

				uiManager.InitPlayer1(BallColor.Black);
				uiManager.InitPlayer2(BallColor.White);
				uiManager.SetPlayer2Turn();

				uiManager.DisplayYourTurn (false);

				uiManager.DisplayPhase1Text (true);


				uiManager.SetPlayer1Name (playerName);
				StartCoroutine (Utils.LoadSpriteFromURL (playerPicURL, (sprite) => {
					uiManager.SetPlayer1Pic(sprite);
				}));


				print ("2 players in the room, sending player name : " + playerName);
				SendName (playerName);
				SendPicURL (playerPicURL);

				//isPlayer1 = false;

			}
			else
			{
				print ("alone in the room, name is : " + playerName);
				uiManager.DisPlayWaitingForPlayerPanel (true);
				uiManager.InitPlayer1(BallColor.White);
				uiManager.SetPlayer1Name (playerName);
				StartCoroutine (Utils.LoadSpriteFromURL (playerPicURL, (sprite) => {
					uiManager.SetPlayer1Pic(sprite);
				}));

				uiManager.DisplayPlayer1Arrow (false);
			}
		}

		public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
		{
			if (PhotonNetwork.room.PlayerCount == 2)
			{
				uiManager.DisPlayWaitingForPlayerPanel (false);
				DisplayMarbleContainer (true);
				Debug.Log("Other player arrived");
				CreateGrid ();
				isGameStarted = true;
                numberOfTurnsPlayer1 = 0;
                numberOfTurnsPlayer2 = 0;

                uiManager.DisplayYourTurn (true);
				uiManager.DisplayPhase1Text (true);
				uiManager.SetPlayer1Turn();
				//isPlayer1 = true;

				player = CreatePlayer (BallColor.White);
				uiManager.InitPlayer2(BallColor.Black);

				SendName (playerName);
				SendPicURL (playerPicURL);

				uiManager.DisplayPlayer1Arrow (true);

				player.StartTurn ();
			} else 
			{
				print("alone");
				uiManager.DisPlayWaitingForPlayerPanel (true);
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

			uiManager.DisplayEndGamePanel (true);
			uiManager.DisplayYouWon(true, 0, 0);
			uiManager.DisplayByForfeit(true);

			uiManager.DisplayRestartButton (true);
		}

		public void OnRestart() 
		{
			PhotonNetwork.LoadLevel (1);
		}

		public void OnInviteFriend() {
			FB.AppRequest ("Viens jouer à Item");

			//fbManager.LoadInvitableFriends ();
		}

	}

}