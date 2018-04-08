using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace AppAdvisory.Item {

	public class UIManager : MonoBehaviour {


		public event Action Restart;
		public event Action FinishTurn;
		public event Action InviteFriend;

		public GameObject phase1Text;
		public GameObject phase2Text;

		public GameObject phase1TextBis;
		public GameObject phase2TextBis;

		public PlayerPanel player1;
		public PlayerPanel player2;

		public GameObject waitingForPlayerPanel;

		public GameObject endGamePanel;
		public GameObject yourTurn;
		public GameObject youWon;
		public GameObject youLost;
		public GameObject byForfeit;
		public GameObject restartButton;

		public GameObject inviteFriendButton;



		void Start ()
        {
			
		}
		
		void Update ()
        {
            Debug.Log("player1 is shown : " + player1.gameObject.activeInHierarchy + " / player2 is shown : " + player2.gameObject.activeInHierarchy);
		}

		public void Init() {
			DisplayPlayer1(false);
			DisplayPlayer2(false);
			DisplayYourTurn (false);

			DisPlayWaitingForPlayerPanel (false);

			DisplayEndGamePanel (false);
			DisplayYouWon (false);
			DisplayByForfeit(false);
			DisplayYouLost (false);

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

		public void DisplayPhase1Text(bool isShown) {
			phase1Text.SetActive (isShown);
			phase1TextBis.SetActive (isShown);
		}

		public void DisplayPhase2Text(bool isShown) {
			phase2Text.SetActive (isShown);
			phase2TextBis.SetActive (isShown);
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

		public void DisplayYourTurn(bool isShown) {
			//yourTurn.SetActive (isShown);
		}

		public void DisplayYouWon(bool isShown) {
			youWon.SetActive (isShown);
		}

		public void DisplayYouLost(bool isSown) {
			youLost.SetActive (isSown);
		}

		public void DisplayByForfeit(bool isShown) {
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
			player1.SetName (name);
		}

		public void SetPlayer2Name(string name) {
			player2.SetName (name);
		}

		public void SetPlayer1Pic(Sprite sprite) {
			player1.SetPic (sprite);
		}

		public void SetPlayer2Pic(Sprite sprite) {
			player2.SetPic (sprite);
		}

		public void SetPlayer1Turn() {
			player1.DisplayArrow (true);
			player2.DisplayArrow (false);
		}

		public void SetPlayer2Turn() {
			player1.DisplayArrow (false);
			player2.DisplayArrow (true);
		}

		public void DisplayPlayer1Arrow(bool isShown) {
			player1.DisplayArrow (isShown);
		}

		public void DisplayPlayer2Arrow(bool isShown) {
			player2.DisplayArrow (isShown);
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
