using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AppAdvisory.Item
{
    public class TurnSwitchPanel : MonoBehaviour
    {
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

        public void SetUIManager(UIManager ui)
        {
            uiManager = ui;
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
        }
    }
}