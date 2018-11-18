using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

    public class RoundPanel : UIPanel
    {
        private RectTransform panel;

        [Header("FR Settings")]
        [SerializeField]
        private string roundLabelFR;
        [SerializeField]
        private string nextRoundLabelFR;
        [SerializeField]
        private string gameResultsLabelFR;

        [Header("EN Settings")]
        [SerializeField]
        private string roundLabelEN;
        [SerializeField]
        private string nextRoundLabelEN;
        [SerializeField]
        private string gameResultsLabelEN;

        [Header("Localized Objects")]
        [SerializeField]
        private TextMeshProUGUI roundText;
        [SerializeField]
        private TextMeshProUGUI nextRoundText;
        [SerializeField]
        private TextMeshProUGUI gameResultsText;

        [Header("")]
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private TextMeshProUGUI ptsText;
        [SerializeField]
        private Button nextRound;
        [SerializeField]
        private Button gameResults;

        private void Awake()
        {
            panel = GetComponent<RectTransform>();
        }

        protected override void SetLanguageEN()
        {
            roundText.text = roundLabelEN;
            //nextRoundText.text = nextRoundLabelEN;
            gameResultsText.text = gameResultsLabelEN;
        }

        protected override void SetLanguageFR()
        {
            roundText.text = roundLabelFR;
            //nextRoundText.text = nextRoundLabelFR;
            gameResultsText.text = gameResultsLabelFR;
        }

        public void DisplayRoundResult(bool isShown)
        {
            panel.gameObject.SetActive(true);
        }

        public void ActivateNextRoundButton(bool isShown)
        {
            nextRound.gameObject.SetActive(isShown);
            gameResults.gameObject.SetActive(!isShown);
        }

        public void ActivateGameResultsButton(bool isShown)
        {
            gameResults.gameObject.SetActive(isShown);
            nextRound.gameObject.SetActive(!isShown);
        }

        public void SetRoundNumber(int nb)
        {
            roundText.text = "ROUND " + nb;
        }

        public void SetScore(int yourPoints, int theirPoints)
        {
            ptsText.text = yourPoints + " - " + theirPoints;
        }

        public void HideAll()
        {
            if (panel == null)
            {
                panel = GetComponent<RectTransform>();
            }

            panel.gameObject.SetActive(false);
        }

        public void StartRoundPanelPopInAnimation()
        {
            gameObject.SetActive(true);
            animator.SetTrigger("popIn");
        }
    }