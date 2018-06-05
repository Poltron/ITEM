using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using HedgehogTeam.EasyTouch;

namespace AppAdvisory.Item {

	public class UIManager : MonoBehaviour {

        [SerializeField]
        private GridManager gridManager;

        public event Action Restart;
        public event Action NextRound;
        public event Action FinishTurn;
		public event Action InviteFriend;

        [SerializeField]
        private float timeBeforeNextTurn;

        [SerializeField]
        private SpriteRenderer boardOverlay;

		public PlayerPanel player1;
		public PlayerPanel player2;

		public GameObject waitingForPlayerPanel;

        public RoundPanel roundResultPanel;
        public EndGamePanel endGamePanel;
        public TurnSwitchPanel turnSwitchPanel;
        public OptionsPanel optionsPanel;
        public RectTransform helpPanel;
        public TutorialPanel tutoPanel;
        public RectTransform overlayPanel;
        public followworldelement followWorldElement;

        public GameObject inviteFriendButton;

        private bool isPlayer1Turn;

        private List<RoundScore> roundScores;

        [SerializeField]
        private Ball Phase1Tuto_BallToMove;

        [SerializeField]
        private Cell Phase1Tuto_CellToMoveTo;

        IEnumerator waitFor(float t, Action toDo)
        {
            yield return new WaitForSeconds(t);
            toDo();
        }

		void Start ()
        {
            roundScores = new List<RoundScore>();
            turnSwitchPanel.SetUIManager(this);
		}

        public void ResetGame()
        {
            endGamePanel.HideAll();
            turnSwitchPanel.SetPhase1(true);
        }

		public void Init() {
            player1.HideAll();
            player2.HideAll();

			DisplayWaitingForPlayerPanel (false);
            
            endGamePanel.HideAll();
            roundResultPanel.HideAll();
            turnSwitchPanel.HideAll();
            tutoPanel.HideAll();

            tutoPanel.DisplayAskForTuto(true);
		}

        public void DisplayTurnSwitchPhase1(bool isShown)
        {
            turnSwitchPanel.SetPhase1(isShown);
        }

        public void DisplayTurnSwitchPhase2(bool isShown)
        {
            turnSwitchPanel.SetPhase2(isShown);
        }

        public void DisplayWaitingForPlayerPanel(bool isShown) {
			waitingForPlayerPanel.SetActive (isShown);
		}
			
		public void DisplayPlayer1(bool isShown) {
			player1.gameObject.SetActive (isShown);
		}

		public void DisplayPlayer2(bool isShown) {
			player2.gameObject.SetActive (isShown);
		}

        public void DisplayRoundResultPanel(bool isShown, int roundNumber, int yourPoints, int theirPoints)
        {
            int isWon = 0;
            if (yourPoints > theirPoints)
            {
                isWon = 1;
            }
            else if (theirPoints > yourPoints)
            {
                isWon = -1;
            }

            roundScores.Add(new RoundScore(yourPoints, theirPoints, isWon));

            roundResultPanel.DisplayRoundResult(isShown);
            roundResultPanel.SetScore(yourPoints, theirPoints);
            roundResultPanel.SetRoundNumber(roundNumber);
        }

        public void DisplayYouWon(bool isShown, int yourPoints, int theirPoints) {
            endGamePanel.DisplayWonScreen(isShown);
            endGamePanel.SetScore(yourPoints, theirPoints);
        }

		public void DisplayYouLost(bool isShown, int yourPoints, int theirPoints) {
            endGamePanel.DisplayLooseScreen(isShown);
            endGamePanel.SetScore(yourPoints, theirPoints);
        }

        public void DisplayDraw(bool isShown, int yourPoints, int theirPoints)
        {
            endGamePanel.DisplayDrawScreen(isShown);
            endGamePanel.SetScore(yourPoints, theirPoints);
        }

        public void DisplayForfeit(bool isShown) {
            endGamePanel.DisplayWonByForfeit(isShown);
		}

		public void DisplayInviteFriendButton(bool isShown) {
			inviteFriendButton.SetActive (isShown);
		}

		public void InitPlayer1(BallColor color) {
			DisplayPlayer1(true);
			player1.SetColor (color);
		}

		public void InitPlayer2(BallColor color) {
			DisplayPlayer2(true);
			player2.SetColor (color);
		}

		public void SetPlayer1Name(string name) {
            if (player1)
			    player1.SetName (name);
		}

		public void SetPlayer2Name(string name) {
            if (player2)
			    player2.SetName (name);
		}

		public void SetPlayer1Pic(Sprite sprite) {
			player1.SetPic (sprite);
		}

		public void SetPlayer2Pic(Sprite sprite) {
			player2.SetPic (sprite);
		}

		public void SetPlayer1Turn(Action callBack)
        {
            turnSwitchPanel.SetCallbackAnimationEnd(callBack);
            StartCoroutine(waitFor(timeBeforeNextTurn, SetPlayer1TurnReal));
		}

        public void DisplayTutorialPhase2Movement()
        {
            tutoPanel.DisplayPhase2MovementsScreen(true);
        }

        private void SetPlayer1TurnReal()
        {
            DisplayYourTurn(true);
            isPlayer1Turn = true;
            turnSwitchPanel.StartTurnSwitchAnimation();
        }

        public void SetPlayer2Turn(Action callBack)
        {
            turnSwitchPanel.SetCallbackAnimationEnd(callBack);
            StartCoroutine(waitFor(timeBeforeNextTurn, SetPlayer2TurnReal));
        }

        private void SetPlayer2TurnReal()
        {
            DisplayOpponentTurn(true);
            isPlayer1Turn = false;
            turnSwitchPanel.StartTurnSwitchAnimation();
        }
            
        void Update()
        {
            //Debug.Log(timer);
            //Debug.Log("player1 is shown : " + player1.gameObject.activeInHierarchy + " / player2 is shown : " + player2.gameObject.activeInHierarchy);
        }

        public void AnimateNextTurn()
        {
            boardOverlay.GetComponent<Animation>().Play();

            if (isPlayer1Turn)
                player1.GetComponent<Animation>().Play();
            else
                player2.GetComponent<Animation>().Play();
        }

        public void DisplayYourTurn(bool isShown)
        {
            turnSwitchPanel.SetYourTurn(isShown);
		}

		public void DisplayOpponentTurn(bool isShown)
        {
            turnSwitchPanel.SetOpponentsTurn(isShown);
        }

		public void ResetPlayerTurn()
        {
			player1.SetColor (Color.white);
			player2.SetColor (Color.white);
		}

		public void OnNextRoundButton()
        {
            if (NextRound != null)
            {
                NextRound();
                roundResultPanel.HideAll();
            }
		}

        public void OnRestartButton()
        {
            if (Restart != null)
                Restart();
        }

		public void OnInviteFriendbutton()
        {
			Debug.Log("invite friend");

			if (InviteFriend != null)
				InviteFriend ();
        }

        public void OnHelpButton()
        {
            helpPanel.gameObject.SetActive(!helpPanel.gameObject.activeInHierarchy);
            optionsPanel.ToggleShowPanel(false);
        }

        public void OnOptionsButton()
        {
            optionsPanel.ToggleShowPanel();
            helpPanel.gameObject.SetActive(false);
        }

        public void OnGoToGameResults()
        {
            int playerScore = 0;
            int opponentScore = 0;

            foreach (RoundScore score in roundScores)
            {
                playerScore += score.playerScore;
                opponentScore += score.otherPlayerScore;
            }

            int isWon = EvaluateWin(playerScore, opponentScore);

            endGamePanel.DisplayResult(isWon);
            endGamePanel.SetScore(playerScore, opponentScore);
            roundResultPanel.HideAll();
        }

        private void DisplayOverlay(bool isShown)
        {
            overlayPanel.gameObject.SetActive(isShown);
        }

        private int EvaluateWin(int playerPoints, int opponentPoints)
        {
            int isWon = 0;
            if (playerPoints > opponentPoints)
            {
                isWon = 1;
            }
            else if (opponentPoints > playerPoints)
            {
                isWon = -1;
            }

            return isWon;
        }

        public void Phase1Tuto_ShowBall()
        {
            overlayPanel.gameObject.SetActive(true);
            Phase1Tuto_BallToMove.PassAboveUI(true);

            followWorldElement.gameObject.SetActive(true);
            followWorldElement.target = Phase1Tuto_BallToMove.transform;
            followWorldElement.GetComponent<Button>().onClick.AddListener(pickBallHack);
        }

        private void pickBallHack()
        {
            Gesture gesture = new Gesture();
            gesture.pickedObject = Phase1Tuto_BallToMove.gameObject;
            gridManager.player.OnTouchUpPublic(gesture);

            Phase1Tuto_BallSelected(Phase1Tuto_BallToMove);
        }
        
        public void Phase1Tuto_BallSelected(Ball ball)
        {
            Phase1Tuto_CellToMoveTo.PassAboveUI(true);

            followWorldElement.target = Phase1Tuto_CellToMoveTo.transform;
            followWorldElement.GetComponent<Button>().onClick.AddListener(pickCellHack);
        }

        private void pickCellHack()
        {
            Gesture gesture = new Gesture();
            gesture.pickedObject = Phase1Tuto_CellToMoveTo.gameObject;
            gridManager.player.OnTouchUpPublic(gesture);

            Phase1Tuto_BackToNormal(Phase1Tuto_CellToMoveTo);

            followWorldElement.gameObject.SetActive(false);
            followWorldElement.target = null;
        }

        public void Phase1Tuto_BackToNormal(Cell cell)
        {
            Debug.Log("backtonormal");
            overlayPanel.gameObject.SetActive(false);
            Phase1Tuto_BallToMove.PassAboveUI(false);
            Phase1Tuto_CellToMoveTo.PassAboveUI(false);
        }
    }
}
