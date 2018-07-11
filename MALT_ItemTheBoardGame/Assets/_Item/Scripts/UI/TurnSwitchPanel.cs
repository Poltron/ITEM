using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AppAdvisory.Item
{
    public class TurnSwitchPanel : UIPanel
    {

        [Header("FR Settings")]
        [SerializeField]
        private string phase1TextFR;

        [SerializeField]
        private string phase2TextFR;

        [SerializeField]
        private string yourTurnTextFR;

        [SerializeField]
        private string opponentTurnTextFR;

        [Header("EN Settings")]
        [SerializeField]
        private string phase1TextEN;

        [SerializeField]
        private string phase2TextEN;

        [SerializeField]
        private string yourTurnTextEN;

        [SerializeField]
        private string opponentTurnTextEN;

        [Header("Localized Objects")]
        [SerializeField]
        private TextMeshProUGUI phase1Text;

        [SerializeField]
        private TextMeshProUGUI phase2Text;

        [SerializeField]
        private TextMeshProUGUI yourTurnText;

        [SerializeField]
        private TextMeshProUGUI opponentTurnText;

        [Header("")]
        [SerializeField]
        private Animator animator;

        [SerializeField]
        private RectTransform phase1;

        [SerializeField]
        private RectTransform phase2;

        [SerializeField]
        private RectTransform yourTurn;

        [SerializeField]
        private RectTransform opponentTurn;
        
        private UIManager uiManager;
        private Action callBack;

        public void SetUIManager(UIManager ui)
        {
            uiManager = ui;
        }

        protected override void SetLanguageEN()
        {
            phase1Text.text = phase1TextEN;
            phase2Text.text = phase2TextEN;
            yourTurnText.text = yourTurnTextEN;
            opponentTurnText.text = opponentTurnTextEN;
        }

        protected override void SetLanguageFR()
        {
            phase1Text.text = phase1TextFR;
            phase2Text.text = phase2TextFR;
            yourTurnText.text = yourTurnTextFR;
            opponentTurnText.text = opponentTurnTextFR;
        }

        public void HideAll()
        {
            SetPhase1(false);
            SetPhase2(false);

            SetYourTurn(false);
            SetOpponentsTurn(false);
        }

        public void SetPhase1(bool isShown)
        {
            phase1.gameObject.SetActive(isShown);
            phase2.gameObject.SetActive(!isShown);
        }

        public void SetPhase2(bool isShown)
        {
            phase2.gameObject.SetActive(isShown);
            phase1.gameObject.SetActive(!isShown);
        }

        public void SetYourTurn(bool isShown)
        {
            yourTurn.gameObject.SetActive(isShown);
            opponentTurn.gameObject.SetActive(!isShown);
        }

        public void SetOpponentsTurn(bool isShown)
        {
            opponentTurn.gameObject.SetActive(isShown);
            yourTurn.gameObject.SetActive(!isShown);
        }

        public void StartTurnSwitchAnimation()
        {
            gameObject.SetActive(true);
            animator.SetTrigger("popIn");
        }

        private void CallbackTurnSwitchAnimation()
        {
            uiManager.AnimateNextTurn();
            gameObject.SetActive(false);

            if (callBack != null)
                callBack();
        }

        public void SetCallbackAnimationEnd(Action _callBack)
        {
            callBack = _callBack;
        }
    }
}