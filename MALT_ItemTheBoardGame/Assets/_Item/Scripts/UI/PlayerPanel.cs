using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AppAdvisory.Item {

	public class PlayerPanel : UIPanel
    {

        [Header("FR Settings")]
        [SerializeField]
        private string playerNameFR;
        [SerializeField]
        private string opponentNameFR;

        [Header("EN Settings")]
        [SerializeField]
        private string playerNameEN;
        [SerializeField]
        private string opponentNameEN;

        [Header("Localized Objects")]
        [SerializeField]
        public Text playerName;

        [Header("")]
        [SerializeField]
        private int playerId;

        private Image panel;

        public Text textCounter;

		public Image image;

        private bool isCustomName;

		public Image whiteMarble;
		public Image blackMarble;

        [SerializeField]
        private Animation portraitRotationAnimation;

        [SerializeField]
        private Animator portraitTurnAnimation;

        [SerializeField]
        private Animator scoreCounterAnimator;

        void Awake ()
        {
			panel = GetComponent<Image> ();
		}

        public void HideAll()
        {
            panel.gameObject.SetActive(false);
        }

        protected override void SetLanguageEN()
        {
            if (playerId == 1 && !isCustomName)
            {
                playerName.text = playerNameEN;
            }
            else if (playerId == 2 && !isCustomName)
            {
                playerName.text = opponentNameEN;
            }
        }

        protected override void SetLanguageFR()
        {
            if (playerId == 1 && !isCustomName)
            {
                playerName.text = playerNameFR;
            }
            else if (playerId == 2 && !isCustomName)
            {
                playerName.text = opponentNameFR;
            }
        }

        public void SetName(string name)
        {
            isCustomName = true;
			this.playerName.text = name;
		}

		public void SetPic(Sprite sprite)
        {
			this.image.sprite = sprite;
		}

        public BallColor GetColor()
        {
            if (whiteMarble.enabled)
            {
                return BallColor.White;
            }
            else
            {
                return BallColor.Black;
            }
        }

		public void SetColor(BallColor color)
        {
			if (color == BallColor.Black) {
				whiteMarble.enabled = false;
				blackMarble.enabled = true;
			} else {
				whiteMarble.enabled = true;
				blackMarble.enabled = false;
			}
		}

		public void SetColor(Color color)
        {
			panel.color = color;
		}

        public void PlayPortraitAnimation()
        {
            portraitRotationAnimation.Play();

            portraitTurnAnimation.gameObject.SetActive(true);
            portraitTurnAnimation.SetTrigger("PopIn");
        }

        public void StopPortraitAnimation()
        {
            AnimatorStateInfo stateInfo = portraitTurnAnimation.GetCurrentAnimatorStateInfo(0);
            if (!stateInfo.IsName("Base.Empty"))
                portraitTurnAnimation.SetTrigger("PopOut");
        }

        public void StartScoreAnim()
        {
            scoreCounterAnimator.SetTrigger("PointReceived");
        }

        public void SetScoreCounter(int nb)
        {
            textCounter.text = nb.ToString();
        }
	}

}