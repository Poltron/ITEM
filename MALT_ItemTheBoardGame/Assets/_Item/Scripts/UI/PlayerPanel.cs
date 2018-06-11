using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AppAdvisory.Item {

	public class PlayerPanel : MonoBehaviour
    {
        private Image panel;

        public Text playerName;
		public Image image;

		public Image whiteMarble;
		public Image blackMarble;

        [SerializeField]
        private Animation portraitRotationAnimation;

        [SerializeField]
        private Animator portraitTurnAnimation;

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
            portraitTurnAnimation.SetTrigger("PopOut");
        }
	}

}