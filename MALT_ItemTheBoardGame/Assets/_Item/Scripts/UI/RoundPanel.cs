using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AppAdvisory.Item
{
    public class RoundPanel : MonoBehaviour
    {
        private RectTransform panel;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private TextMeshProUGUI roundText;

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
}