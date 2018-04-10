
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
	public class GridManager : PunBehaviour {

		public int numberOfPlayToShowInterstitial = 1;
		private Connection connection;
		public FBManager fbManager;

		public Grid grid;
        public AIEvaluationData aiEvaluationData;
        public AIBehaviour aiBehaviour;

		public Player playerPrefab;
		public Ball whiteBallPrefab;
		public Ball blackBallPrefab;
		public Transform marbleContainer;


		private Player player;
		private string playerName;
		private string playerPicURL;

		public UIManager uiManager;

		public List<Ball> blackBalls;
		public List<Ball> whiteBalls;

		public float timeToLaunchGameVSIA = 4;

		//private bool isPlayer1 = false;
		private bool isGameFinished = false;
		private bool isGameStarted = false;
		private bool isPlayingVSIA = false;

		private string[] randomNames = new string[] {	"Nathan",
			"Lucas",
			"Enzo",
			"Léo",
			"Louis",
			"Hugo",
			"Gabriel",
			"Ethan",
			"Mathis",
			"Jules",
			"Raphaël",
			"Arthur",
			"Tom",
			"Théo",
			"Noah",
			"Timéo",
			"Matheo",
			"Clément",
			"Maxime",
			"Yanis",
			"Maël",
			"Adam",
			"Thomas",
			"Evan",
			"Paul",
			"Nolan",
			"Axel",
			"Antoine",
			"Alexandre",
			"Noé",
			"Sacha",
			"Noa",
			"Baptiste",
			"Maxence",
			"Mohamed",
			"Gabin",
			"Alexis",
			"Rayan",
			"Quentin",
			"Valentin",
			"Mathys",
			"Victor",
			"Samuel",
			"Esteban",
			"Kylian",
			"Martin",
			"Romain",
			"Simon",
			"Matteo",
			"Aaron",
			"Lorenzo",
			"Lenny",
			"Robin",
			"Benjamin",
			"Adrien",
			"Naël",
			"Liam",
			"Pierre",
			"Titouan",
			"Ilyes",
			"Amine",
			"Dylan",
			"Rafael",
			"Tristan",
			"Julien",
			"Gaspard",
			"Eliott",
			"Nicolas",
			"Mathieu",
			"Bastien",
			"Mathias",
			"Corentin",
			"Oscar",
			"Marius",
			"Thibault",
			"Diego",
			"Kenzo",
			"Noam",
			"Mehdi",
			"Ilan",
			"Antonin",
			"Mateo",
			"Nino",
			"Louka",
			"Erwan",
			"Lilian",
			"Tiago",
			"William",
			"Kais",
			"Loan",
			"Jean",
			"Augustin",
			"Loris",
			"Timothe",
			"Ismael",
			"Ayoub",
			"Amaury",
			"Florian",
			"Luca",
			"David"
		};

		void Start () {
            aiBehaviour = new AIBehaviour(aiEvaluationData);
            
			DisplayMarbleContainer (false);

            DOVirtual.DelayedCall(timeToLaunchGameVSIA, StartGameVSIA);

            connection = GetComponent<Connection> ();

			uiManager.Init();
			uiManager.Restart += OnRestart;
			uiManager.InviteFriend += OnInviteFriend;

			connection.ApplyUserIdAndConnect ();

			fbManager = FindObjectOfType<FBManager> ();
            if (fbManager)
            {
                playerName = fbManager.pName;
                playerPicURL = fbManager.pUrlPic;


                fbManager.NameLoaded += OnNameLoaded;
                fbManager.PicURLLoaded += OnPicURLLoaded;
                //			fbManager.FacebookConnect += OnFacebookConnect;
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
			isPlayingVSIA = true;


			uiManager.DisplayYourTurn (true);
			uiManager.DisplayPhase1Text (true);
			uiManager.SetPlayer1Turn();
			//isPlayer1 = true;

			player = CreatePlayer (BallColor.White);
			uiManager.InitPlayer2(BallColor.Black);

			uiManager.DisplayPlayer1Arrow (true);
			uiManager.SetPlayer2Name (GetRandomIAName());

			player.StartTurn ();
		}

		private string GetRandomIAName() {
            //return randomNames [Random.Range (0, randomNames.Length - 1)];
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
			print ("onnameloaded " + name);
		}

		private void OnPicURLLoaded(string url) {
			playerPicURL = url;
			print ("onpicurlloaded" + url);
		}

		private void CreateGrid() {
			grid = new Grid(5, 9, transform, FindObjectsOfType<Cell>().ToList());
		}

		private Player CreatePlayer(BallColor color) {
			Player player = Instantiate (playerPrefab);
			player.ballPrefab = color == BallColor.White ? whiteBallPrefab : blackBallPrefab;
			player.SetGrid (grid);
			player.OnPhase1TurnFinished += Phase1TurnFinishedPlayer;
			player.OnPhase2TurnFinished += Phase2TurnFinishedPlayer;
			return player;
		}

		public void PlaceBallIA(Cell cell) {
			Ball ball = blackBalls.First ();
			blackBalls.RemoveAt (0);
			ball.DOPlace (cell);
		}

        public void MoveBallIA()
        {

        }

		public void PlayIAPhase1(Cell lastMove)
        {
            float time;
            Move move = aiBehaviour.GetBestMove(this, out time);
            StartCoroutine(waitFor(1.0f - time, move, PlayAIMovePhase1));
        }

        public void PlayAIMovePhase1(Move move)
        {
            Cell cell = grid.GetCellFromModel(move.toY, move.toX);
            PlaceBallIA(cell);

            if (Utils.CheckWinIA(grid, cell))
            {
                Debug.Log("IA WIN");
                RestartGame(false);
            }

            if (player.ballCount == 0)
            {
                uiManager.DisplayPhase1Text(false);
                uiManager.DisplayPhase2Text(true);
            }

            EndAIPhase();
        }

        public void EndAIPhase()
        {
            player.StartTurn();

            uiManager.DisplayYourTurn(true);
            uiManager.SetPlayer1Turn();
        }


		public bool TryToWin() {
			List<Cell> colorCells = grid.GetAllCellsWithColor ();

			if (colorCells.Count == 0)
				return false;

			List<Cell> cellsToWin = new List<Cell>();

			Cell colorCell;

			int count = 0;
			int maxCount = -1;

			for (int i = 0; i < colorCells.Count; i++) {
				colorCell = colorCells [i];

				//diagonal bottom left to top right
				List<Cell> diagonalBottomLeftToTopRightCells = Utils.GetDiagonalBottomLeftToTopRightCells (grid, colorCell);
				count = Utils.GetCellToWinCount (grid, diagonalBottomLeftToTopRightCells);

				if (count == 4) {
					maxCount = count;
					cellsToWin = diagonalBottomLeftToTopRightCells;
					break;
				} else if (count > maxCount) {
					maxCount = count;
					cellsToWin = diagonalBottomLeftToTopRightCells;
				}

				//diagonal bottom right to top left
				List<Cell> diagonalBottomRightToTopLeftCells = Utils.GetDiagonalBottomRightToTopLeftCells (grid, colorCell);
				count = Utils.GetCellToWinCount (grid, diagonalBottomRightToTopLeftCells);

				if (count == 4) {
					maxCount = count;
					cellsToWin = diagonalBottomRightToTopLeftCells;
					break;
				} else if (count > maxCount) {
					maxCount = count;
					cellsToWin = diagonalBottomRightToTopLeftCells;
				}


				//vertical
				List<Cell> verticalCells = Utils.GetVerticalCells (grid, colorCell);
				count = Utils.GetCellToWinCount (grid, verticalCells);

				if (count == 4) {
					maxCount = count;
					cellsToWin = verticalCells;
					break;
				} else if (count > maxCount) {
					maxCount = count;
					cellsToWin = verticalCells;
				}

				//horizontal
				List<Cell> horizontalCells = Utils.GetHorizontalCells (grid, colorCell);
				count = Utils.GetCellToWinCount (grid, horizontalCells);

				if (count == 4) {
					cellsToWin = horizontalCells;
					break;
				} else if (count > maxCount) {
					maxCount = count;
					cellsToWin = horizontalCells;
				}

				//cross diagonal
				List<Cell> crossDiagonalCells;

				crossDiagonalCells = Utils.GetCrossDiagonalCells (grid, colorCell);
				count = Utils.GetCellToWinCount (grid, crossDiagonalCells);
				if (count == 4) {
					cellsToWin = crossDiagonalCells;
					break;
				} else if (count > maxCount) {
					maxCount = count;
					cellsToWin = crossDiagonalCells;
				}

				int offset = colorCell.y % 2; 

				crossDiagonalCells = Utils.GetCrossDiagonalCells (grid, grid.GetCellFromModel(colorCell.x-1 + offset, colorCell.y+1));
				count = Utils.GetCellToWinCount (grid, crossDiagonalCells);
				if (count == 4) {
					cellsToWin = crossDiagonalCells;
					break;
				} else if (count > maxCount) {
					maxCount = count;
					cellsToWin = crossDiagonalCells;
				}

				crossDiagonalCells = Utils.GetCrossDiagonalCells (grid, grid.GetCellFromModel(colorCell.x + offset, colorCell.y + 1));
				count = Utils.GetCellToWinCount (grid, crossDiagonalCells);
				if (count == 4) {
					cellsToWin = crossDiagonalCells;
					break;
				} else if (count > maxCount) {
					maxCount = count;
					cellsToWin = crossDiagonalCells;
				}

				crossDiagonalCells = Utils.GetCrossDiagonalCells (grid, grid.GetCellFromModel(colorCell.x - 1 + offset, colorCell.y - 1));
				count = Utils.GetCellToWinCount (grid, crossDiagonalCells);
				if (count == 4) {
					cellsToWin = crossDiagonalCells;
					break;
				} else if (count > maxCount) {
					maxCount = count;
					cellsToWin = crossDiagonalCells;
				}

				crossDiagonalCells = Utils.GetCrossDiagonalCells (grid, grid.GetCellFromModel(colorCell.x + offset, colorCell.y - 1));
				count = Utils.GetCellToWinCount (grid, crossDiagonalCells);
				if (count == 4) {
					cellsToWin = crossDiagonalCells;
					break;
				} else if (count > maxCount) {
					maxCount = count;
					cellsToWin = crossDiagonalCells;
				}
					
				//cross normal
				List<Cell> crossNormalCells;

				crossNormalCells = Utils.GetCrossNormalCells(grid, colorCell);
				count = Utils.GetCellToWinCount (grid, crossNormalCells);

				if (count == 4) {
					cellsToWin = crossNormalCells;
					break;
				} else if (count > maxCount) {
					maxCount = count;
					cellsToWin = crossNormalCells;
				}
					
				crossNormalCells = Utils.GetCrossNormalCells(grid, grid.GetCellFromModel (colorCell.x - 1, colorCell.y));
				count = Utils.GetCellToWinCount (grid, crossNormalCells);
				if (count == 4) {
					cellsToWin = crossNormalCells;
					break;
				} else if (count > maxCount) {
					maxCount = count;
					cellsToWin = crossNormalCells;
				}

				crossNormalCells = Utils.GetCrossNormalCells(grid, grid.GetCellFromModel (colorCell.x + 1, colorCell.y));
				count = Utils.GetCellToWinCount (grid, crossNormalCells);
				if (count == 4) {
					cellsToWin = crossNormalCells;
					break;
				} else if (count > maxCount) {
					maxCount = count;
					cellsToWin = crossNormalCells;
				}

				crossNormalCells = Utils.GetCrossNormalCells(grid, grid.GetCellFromModel (colorCell.x, colorCell.y + 2));
				count = Utils.GetCellToWinCount (grid, crossNormalCells);
				if (count == 4) {
					cellsToWin = crossNormalCells;
					break;
				} else if (count > maxCount) {
					maxCount = count;
					cellsToWin = crossNormalCells;
				}

				crossNormalCells = Utils.GetCrossNormalCells(grid, grid.GetCellFromModel (colorCell.x, colorCell.y - 2));
				count = Utils.GetCellToWinCount (grid, crossNormalCells);
				if (count == 4) {
					cellsToWin = crossNormalCells;
					break;
				} else if (count > maxCount) {
					maxCount = count;
					cellsToWin = crossNormalCells;
				}
			}

			if (maxCount == -1)
				return false;

			//Utils.ResetCellsColor (grid);
			//Utils.HighlighCells (cellsToWin, Color.green);

			cellsToWin.Shuffle ();

			foreach (Cell winingCell in cellsToWin) {
				if (!winingCell.HasBall ()) {
					PlaceBallIA(winingCell);
					WinIA (winingCell);
					print ("Try to win");
					return true;
				}

			}


			return false;

		}
			
        IEnumerator waitFor(float t, Move move, System.Action<Move> func)
        {
            while ((t -= Time.deltaTime) > 0)
                yield return new WaitForEndOfFrame();

            func(move);
        }

		public void PlayIAPhase2()
        {
            float time;
            Move move = aiBehaviour.GetBestMove(this, out time);
            StartCoroutine(waitFor(1.0f - time, move, PlayAIMovePhase2));
        }

        public void PlayAIMovePhase2(Move move)
        {
            Cell cellFrom = grid.GetCellFromModel(move.fromY, move.fromX);
            Cell cellTo = grid.GetCellFromModel(move.toY, move.toX);

            player.ChangeBallPosition(cellFrom, cellTo);
            if (Utils.CheckWinIA(grid, cellTo))
            {
                RestartGame(false);
            }

            EndAIPhase();
        }

		public bool PlaceNeighbour(Cell lastMove) {
			List<Cell> neighbours = grid.GetCellNeighbours (lastMove);
			neighbours.Shuffle ();

			foreach (Cell neighbour in neighbours) {
				if (!neighbour.HasBall ()) {
					PlaceBallIA (neighbour);
					WinIA (neighbour);
					return true;
				}
			}
			return false;
		}

		public bool PreventWin(Cell lastMove) {
			//List<Cell> lastMoveNeighbours = grid.GetCellNeighbours (lastMove);
			List<Cell> winingCells;

			int cellCountToPrevent = 4;
			float random = Random.value;
			if (random < 0.4f) {
				cellCountToPrevent = 3;
			} else if (random < 1f) {
				cellCountToPrevent = 4;
			}
				
			int count = -1;

			//diagonal bottom left to top right
			winingCells = Utils.GetDiagonalBottomLeftToTopRightCells(grid, lastMove);
			count = Utils.GetCellToWinCountDiagonal (grid, winingCells, BallColor.White);
			if (count >= 3) {
				foreach (Cell winingCell in winingCells) {
					if (!winingCell.HasBall ()) {
						PlaceBallIA(winingCell);
						WinIA (winingCell);
						print ("Prevent wining diagonal");
						return true;
					}
				}
			}

			//diagonal bottom right to top left
			winingCells = Utils.GetDiagonalBottomRightToTopLeftCells(grid, lastMove);
			count = Utils.GetCellToWinCountDiagonal (grid, winingCells, BallColor.White);
			if (count >= 3) {
				foreach (Cell winingCell in winingCells) {
					if (!winingCell.HasBall ()) {
						PlaceBallIA(winingCell);
						WinIA (winingCell);
						print ("Prevent wining diagonal");
						return true;
					}
				}
			}


			//vertical
			winingCells = Utils.GetVerticalCells (grid, lastMove);
			count = Utils.GetCellToWinCount (grid, winingCells, BallColor.White);

			if (count >= cellCountToPrevent) {
				foreach (Cell winingCell in winingCells) {
					if (!winingCell.HasBall ()) {
						PlaceBallIA(winingCell);
						WinIA (winingCell);
						print ("Prevent wining");
						return true;
					}
				}
			}

			//horizontal
			winingCells = Utils.GetHorizontalCells (grid, lastMove);
			count = Utils.GetCellToWinCount (grid, winingCells, BallColor.White);
			if (count >= cellCountToPrevent) {
				foreach (Cell winingCell in winingCells) {
					if (!winingCell.HasBall ()) {
						PlaceBallIA(winingCell);
						WinIA (winingCell);
						print ("Prevent wining");
						return true;
					}
				}
			}

			//cross normal
			winingCells = Utils.GetCrossNormalCells (grid, lastMove);
			count = Utils.GetCellToWinCount (grid, winingCells, BallColor.White);
			if (count >= cellCountToPrevent) {
				foreach (Cell winingCell in winingCells) {
					if (!winingCell.HasBall ()) {
						PlaceBallIA(winingCell);
						WinIA (winingCell);
						print ("Prevent wining");
						return true;
					}
				}
			}

			winingCells = Utils.GetCrossNormalCells (grid, grid.GetCellFromModel (lastMove.x - 1, lastMove.y));
			count = Utils.GetCellToWinCount (grid, winingCells, BallColor.White);
			if (count >= cellCountToPrevent) {
				foreach (Cell winingCell in winingCells) {
					if (!winingCell.HasBall ()) {
						PlaceBallIA(winingCell);
						WinIA (winingCell);
						print ("Prevent wining");
						return true;
					}
				}
			}

			winingCells = Utils.GetCrossNormalCells (grid, grid.GetCellFromModel (lastMove.x + 1, lastMove.y));
			count = Utils.GetCellToWinCount (grid, winingCells, BallColor.White);
			if (count >= cellCountToPrevent) {
				foreach (Cell winingCell in winingCells) {
					if (!winingCell.HasBall ()) {
						PlaceBallIA(winingCell);
						WinIA (winingCell);
						print ("Prevent wining");
						return true;
					}
				}
			}

			winingCells = Utils.GetCrossNormalCells (grid, grid.GetCellFromModel (lastMove.x, lastMove.y + 2));
			count = Utils.GetCellToWinCount (grid, winingCells, BallColor.White);
			if (count >= cellCountToPrevent) {
				foreach (Cell winingCell in winingCells) {
					if (!winingCell.HasBall ()) {
						PlaceBallIA(winingCell);
						WinIA (winingCell);
						print ("Prevent wining");
						return true;
					}
				}
			}

			winingCells = Utils.GetCrossNormalCells (grid, grid.GetCellFromModel (lastMove.x, lastMove.y - 2));
			count = Utils.GetCellToWinCount (grid, winingCells, BallColor.White);
			if (count >= cellCountToPrevent) {
				foreach (Cell winingCell in winingCells) {
					if (!winingCell.HasBall ()) {
						PlaceBallIA(winingCell);
						WinIA (winingCell);
						print ("Prevent wining");
						return true;
					}
				}
			}
				
			//cross diagonal
			winingCells = Utils.GetCrossDiagonalCells (grid, lastMove);
			count = Utils.GetCellToWinCount (grid, winingCells, BallColor.White);
			if (count >= cellCountToPrevent) {
				foreach (Cell winingCell in winingCells) {
					if (!winingCell.HasBall ()) {
						PlaceBallIA(winingCell);
						WinIA (winingCell);
						print ("Prevent wining");
						return true;
					}
				}
			}

			int offset = lastMove.y % 2;

			winingCells = Utils.GetCrossDiagonalCells (grid, grid.GetCellFromModel(lastMove.x-1 + offset, lastMove.y+1));
			count = Utils.GetCellToWinCount (grid, winingCells, BallColor.White);
			if (count >= cellCountToPrevent) {
				foreach (Cell winingCell in winingCells) {
					if (!winingCell.HasBall ()) {
						PlaceBallIA(winingCell);
						WinIA (winingCell);
						print ("Prevent wining");
						return true;
					}
				}
			}

			winingCells = Utils.GetCrossDiagonalCells (grid, grid.GetCellFromModel(lastMove.x + offset, lastMove.y + 1));
			count = Utils.GetCellToWinCount (grid, winingCells, BallColor.White);
			if (count >= cellCountToPrevent) {
				foreach (Cell winingCell in winingCells) {
					if (!winingCell.HasBall ()) {
						PlaceBallIA(winingCell);
						WinIA (winingCell);
						print ("Prevent wining");
						return true;
					}
				}
			}

			winingCells = Utils.GetCrossDiagonalCells (grid, grid.GetCellFromModel(lastMove.x - 1 + offset, lastMove.y - 1));
			count = Utils.GetCellToWinCount (grid, winingCells, BallColor.White);
			if (count >= cellCountToPrevent) {
				foreach (Cell winingCell in winingCells) {
					if (!winingCell.HasBall ()) {
						PlaceBallIA(winingCell);
						WinIA (winingCell);
						print ("Prevent wining");
						return true;
					}
				}
			}

			winingCells = Utils.GetCrossDiagonalCells (grid, grid.GetCellFromModel(lastMove.x + offset, lastMove.y - 1));
			count = Utils.GetCellToWinCount (grid, winingCells, BallColor.White);
			if (count >= cellCountToPrevent) {
				foreach (Cell winingCell in winingCells) {
					if (!winingCell.HasBall ()) {
						PlaceBallIA(winingCell);
						WinIA (winingCell);
						print ("Prevent wining");
						return true;
					}
				}
			}
			return false;
		}
			

		public void PlaceRandom() {
			print("place random");

			Cell cell = grid.GetRandomEmptyCell ();
			PlaceBallIA (cell);
			WinIA (cell);
		}

		public void MoveRandom() {
			print ("move random");
			List<Cell> colorCells = grid.GetAllCellsWithColor ();

			colorCells.Shuffle ();
			List<Cell> cellsToGo = new List<Cell> ();
		
			List<Cell> neighbours;

			Cell destinationCell;

			foreach (Cell colorCell in colorCells) {
				neighbours = grid.GetCellNeighbours (colorCell);
				foreach (Cell cell in neighbours) {
					if (!cell.HasBall()) {
						cellsToGo.Add (cell);
					} else {
						destinationCell = grid.GetCellFromDirection (colorCell, cell);
						if (destinationCell) {
							if (!destinationCell.HasBall ())
								cellsToGo.Add (destinationCell);
						}

					}

					if (cellsToGo.Count > 0) {
						player.ChangeBallPosition (colorCell, cellsToGo[Random.Range(0, cellsToGo.Count-1)]);
						WinIA (cellsToGo.First ());
						return;
					}


				}

			}

		}

		public void Phase1TurnFinishedPlayer(Vector2 pos) {
			uiManager.DisplayPhase1Text (true);

			Cell cell = grid.GetCellFromModel (pos);

			if (isPlayingVSIA) {
				if (Win (cell))
					return;

				uiManager.DisplayYourTurn (false);
				uiManager.SetPlayer2Turn ();

                DOVirtual.DelayedCall (1.0f, () => PlayIAPhase1 (cell));
                //PlayIAPhase1(cell);
            } else {
				

				PhotonView photonView = PhotonView.Get(this);
				photonView.RPC ("ReceiveMovementsPhase1", PhotonTargets.Others, pos);

				if (!Win (cell)) {
					SetPlayerTurnOnEnd ();
				}
			}


		}

		public void Phase2TurnFinishedPlayer(List<Vector2> movements)
		{
			uiManager.DisplayPhase1Text (false);
			uiManager.DisplayPhase2Text (true);
			Vector2[] movementArray = movements.ToArray ();
			Cell cell = grid.GetCellFromModel (movementArray [movementArray.Length - 1]);

			if (isPlayingVSIA) {
				if (Win (cell))
					return;

				uiManager.DisplayYourTurn (false);
				uiManager.SetPlayer2Turn ();
				PlayIAPhase2();
			} else {
				

				PhotonView photonView = PhotonView.Get(this);
				photonView.RPC("ReceiveMovementsPhase2", PhotonTargets.Others, movementArray);

				if (!Win (cell)) {
					SetPlayerTurnOnEnd ();
				}
			}
		}

		private bool Win(Cell cell) {
			if (Utils.CheckWin (grid, cell)) {

				if (!isPlayingVSIA) {
					PhotonView photonView = PhotonView.Get (this);
					photonView.RPC ("ReceiveWin", PhotonTargets.Others, new Vector2 (cell.x, cell.y));
					player.EndTurn ();
				}

				DOVirtual.DelayedCall (1, () => {
					RestartGame (true);
				});
				return true;
			}
			return false;

		}

		private void WinIA(Cell cell) {
			if (Utils.CheckWinIA (grid, cell)) {
				RestartGame (false);
			}
		}

		void RestartGame(bool isWin) {
			ShowAds();


			isGameFinished = true;
			player.EndTurn ();

			uiManager.DisplayEndGamePanel (true);
			RestartConnection ();

			if (isWin) {
				uiManager.DisplayYouWon (true);
			} else {
				uiManager.DisplayYouLost (true);
			}

			uiManager.DisplayRestartButton (true);

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

			Cell cell = grid.GetCellFromModel (pos);
			Ball ball;

			if (player.color == BallColor.White) {
				ball = blackBalls.First ();
				blackBalls.RemoveAt (0);
			} else {
				ball = whiteBalls.First ();
				whiteBalls.RemoveAt (0);
			}

			ball.DOPlace (cell);


			player.StartTurn ();

			SetPlayerTurnOnReceive();
		}
			
		[PunRPC]
		void ReceiveMovementsPhase2(Vector2[] movements)
		{
			uiManager.DisplayPhase1Text (false);
			uiManager.DisplayPhase2Text (true);

			StartCoroutine (MoveCoroutine (movements));
			uiManager.DisplayYourTurn (true);
			player.StartTurn ();

			SetPlayerTurnOnReceive ();
		}

		[PunRPC]
		void ReceiveWin(Vector2 position) {
			print ("receive win");
			uiManager.ResetPlayerTurn ();

			player.EndTurn ();
			uiManager.DisplayYourTurn (false);


			Cell cell = grid.GetCellFromModel (position);
			Utils.CheckWin (grid, cell);
			RestartGame (false);
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

				player.ChangeBallPosition (grid.GetCellFromModel (movements [i]), grid.GetCellFromModel (movements [i+1]));
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
			uiManager.DisplayYouWon(true);
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