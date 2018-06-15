using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AppAdvisory.Item {

	public class PlayerPanel : MonoBehaviour
    {
        private Image panel;

        public Text textCounter;

        public Text playerName;
		public Image image;

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

		public void SetName(string name)
        {
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