using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AppAdvisory.Item {

	public class PlayerPanel : MonoBehaviour {

		public Text playerName;
		public Image image;

		public Image whiteMarble;
		public Image blackMarble;

		public Image whiteArrow;
		public Image blackArrow;

		public GameObject maskArrow;

		private Image panel;

		void Awake () {
			panel = GetComponent<Image> ();
		}

		public void DisplayArrow(bool isShown) {
			//maskArrow.SetActive(isShown);
		}
			
		public void SetName(string name) {
			this.playerName.text = name;
		}

		public void SetPic(Sprite sprite) {
			this.image.sprite = sprite;
		}

		public void SetColor(BallColor color) {
			if (color == BallColor.Black) {
				whiteMarble.enabled = false;
				//whiteArrow.enabled = false;
				blackMarble.enabled = true;
				//blackArrow.enabled = true;
			} else {
				whiteMarble.enabled = true;
				//whiteArrow.enabled = true;
				blackMarble.enabled = false;
				//blackArrow.enabled = false;
			}
		}

		public void SetColor(Color color) {
			panel.color = color;
		}
	}

}