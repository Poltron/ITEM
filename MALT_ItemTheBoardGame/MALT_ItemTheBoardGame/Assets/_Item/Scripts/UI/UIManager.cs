using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

namespace AppAdvisory.Item {

	public class UIManager : MonoBehaviour {


		public event Action Restart;
		public event Action FinishTurn;
		public event Action InviteFriend;

        [SerializeField]
        private AnimationCurve turnOpacity;
        [SerializeField]
        private AnimationCurve turnPosition;
        [SerializeField]
        private AnimationCurve phasePosition;
        [SerializeField]
        private float turnAnimationDuration;
        [SerializeField]
        private float timeBeforeNextTurn;
        private float timer;

        [SerializeField]
        private SpriteRenderer boardOverlay;

        [SerializeField]
        private GameObject overlay;
        [SerializeField]
        private GameObject yourTurn;
        [SerializeField]
        private GameObject opponentsTurn;

        public GameObject phase1Text;
		public GameObject phase2Text;

		public PlayerPanel player1;
		public PlayerPanel player2;

		public GameObject waitingForPlayerPanel;

		public GameObject endGamePanel;
		public GameObject youWon;
		public GameObject youLost;
		public GameObject byForfeit;
		public GameObject restartButton;
        public GameObject withPoints;

		public GameObject inviteFriendButton;

        private bool isPlayer1Turn;

        IEnumerator waitFor(float t, Action toDo)
        {
            yield return new WaitForSeconds(t);
            toDo();
        }

		void Start ()
        {
			
		}

		public void Init() {
			DisplayPlayer1(false);
			DisplayPlayer2(false);
			DisplayYourTurn (false);

			DisPlayWaitingForPlayerPanel (false);

			DisplayEndGamePanel (false);
			DisplayYouWon (false, 0);
			DisplayByForfeit(false);
			DisplayYouLost (false, 0);

			DisplayPhase1Text (false);
			DisplayPhase2Text (false);

			DisplayRestartButton (false);
		}

		public void DisPlayWaitingForPlayerPanel(bool isShown) {
			waitingForPlayerPanel.SetActive (isShown);
		}

		public void DisplayEndGamePanel(bool isShown) {
			endGamePanel.SetActive (isShown);
			DisplayPhase1Text (false);
			DisplayPhase2Text (false);
			DisplayYourTurn (false);
		}

        public void DisplayYourTurn(bool isShown)
        {

        }

        public void DisplayPhase1Text(bool isShown) {
			phase1Text.SetActive (isShown);
		}

		public void DisplayPhase2Text(bool isShown) {
			phase2Text.SetActive (isShown);
		}

		public void DisplayRestartButton(bool isShown) {
			restartButton.SetActive (isShown);
		}
			
		public void DisplayPlayer1(bool isShown) {
			player1.gameObject.SetActive (isShown);
		}

		public void DisplayPlayer2(bool isShown) {
			player2.gameObject.SetActive (isShown);
		}

        public void DisplayYouWon(bool isShown, int points) {
			youWon.SetActive (isShown);
            withPoints.SetActive(true);
            withPoints.GetComponentInChildren<TextMeshProUGUI>().text = "With " + points + " pts";
        }

		public void DisplayYouLost(bool isShown, int points) {
			youLost.SetActive (isShown);
            withPoints.SetActive(true);
            withPoints.GetComponentInChildren<TextMeshProUGUI>().text = "With " + points + " pts";
        }

		public void DisplayByForfeit(bool isShown) {
            withPoints.SetActive(false);
			byForfeit.SetActive(isShown);
		}

		public void DisplayInviteFriendButton(bool isShown) {
			inviteFriendButton.SetActive (isShown);
		}

		public void InitPlayer1(BallColor color) {
			DisplayPlayer1(true);
			player1.DisplayArrow (false);
			player1.SetColor (color);
		}

		public void InitPlayer2(BallColor color) {
			DisplayPlayer2(true);
			player2.DisplayArrow (false);
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

		public void SetPlayer1Turn()
        {
            StartCoroutine(waitFor(timeBeforeNextTurn, SetPlayer1TurnReal));
		}

        private void SetPlayer1TurnReal()
        {
            DisplayPlayer1Arrow(true);
            timer = 0;
            isPlayer1Turn = true;
        }

        public void SetPlayer2Turn()
        {
            StartCoroutine(waitFor(timeBeforeNextTurn, SetPlayer2TurnReal));
        }

        private void SetPlayer2TurnReal()
        {
            DisplayPlayer2Arrow(true);
            isPlayer1Turn = false;
            timer = 0;
        }

        void Update()
        {
            if (timer < turnAnimationDuration)
            {
                timer += Time.deltaTime;
                float opacity = turnOpacity.Evaluate(timer);

                Image spriteR = overlay.GetComponent<Image>();
                spriteR.color = new Color(spriteR.color.r, spriteR.color.g, spriteR.color.b, opacity);

                if (timer >= turnAnimationDuration)
                {
                    boardOverlay.GetComponent<Animation>().Play();

                    if (isPlayer1Turn)
                        player1.GetComponent<Animation>().Play();
                    else
                        player2.GetComponent<Animation>().Play();
                }
            }
            else
            {
                overlay.SetActive(false);
                yourTurn.SetActive(false);
                opponentsTurn.SetActive(false);
            }
            //Debug.Log(timer);
            //Debug.Log("player1 is shown : " + player1.gameObject.activeInHierarchy + " / player2 is shown : " + player2.gameObject.activeInHierarchy);
        }

        public void DisplayPlayer1Arrow(bool isShown) {
            overlay.SetActive(isShown);
            yourTurn.SetActive(isShown);
            opponentsTurn.SetActive(!isShown);
		}

		public void DisplayPlayer2Arrow(bool isShown) {
            overlay.SetActive(isShown);
            opponentsTurn.SetActive(isShown);
            yourTurn.SetActive(!isShown);
        }

		public void ResetPlayerTurn() {
			player1.SetColor (Color.white);
			player2.SetColor (Color.white);
		}

		public void OnRestartButton() {
			if(Restart != null)
				Restart ();
		}

		public void OnInviteFriendbutton() {
			Debug.Log("invite friend");

			if (InviteFriend != null)
				InviteFriend ();
		}
	}

}
