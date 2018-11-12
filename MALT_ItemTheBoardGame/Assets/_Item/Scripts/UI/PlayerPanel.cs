using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        [Header("White/Blue Marble")]
        [SerializeField]
        private Image whiteMarble;

        [SerializeField]
        private Sprite blueBanner;
        [SerializeField]
        private Sprite blueScore;
        [SerializeField]
        private Sprite blueName;

        [Header("Black/Red Marble")]
        [SerializeField]
        private Image blackMarble;

        [SerializeField]
        private Sprite redBanner;
        [SerializeField]
        private Sprite redScore;
        [SerializeField]
        private Sprite redName;

        [Header("")]
        [SerializeField]
        private int playerId;
        
        [SerializeField]
        private Image bannerImage;

        [SerializeField]
        private Image scoreImage;

        [SerializeField]
        private Image nameImage;

        private Image panel;

        public Text textCounter;

		public Image image;

        private bool isCustomName;

        [SerializeField]
        private Animation portraitRotationAnimation;

        [SerializeField]
        private Animator portraitTurnAnimator;

        [SerializeField]
        private Animator portraitLightAnimator;

        [SerializeField]
        private Animator scoreCounterAnimator;

        private AudioManager audioManager;

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

        public bool IsDefaultName(string name)
        {
            if (name == playerNameEN ||name == playerNameFR)
            {
                return true;
            }

            return false;
        }

        public void SetName(string name)
        {
            if (IsDefaultName(name))
            {
                if (Options.IsLanguageEn())
                    SetLanguageEN();
                else if (Options.IsLanguageFr())
                    SetLanguageFR();
            }
            else
            {
                isCustomName = true;
                playerName.text = name;
            }
		}

		public void SetPic(Sprite sprite)
        {
            if (sprite.rect.width > sprite.rect.height)
            {
                image.rectTransform.localScale = new Vector3(sprite.rect.width / sprite.rect.height, 1, 1);
            }
            else
            {
                image.rectTransform.localScale = new Vector3(1, sprite.rect.height / sprite.rect.width, 1);
            }

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

                bannerImage.sprite = redBanner;
                scoreImage.sprite = redScore;
                nameImage.sprite = redName;
			} else {
				whiteMarble.enabled = true;
				blackMarble.enabled = false;

                bannerImage.sprite = blueBanner;
                scoreImage.sprite = blueScore;
                nameImage.sprite = blueName;
            }
		}

		public void SetColor(Color color)
        {
			panel.color = color;
		}

        public void PlayPortraitAnimation()
        {
            portraitRotationAnimation.Play();

            //portraitLightAnimator.gameObject.SetActive(true);
            //portraitLightAnimator.SetTrigger("PopIn");

            portraitTurnAnimator.gameObject.SetActive(true);
            portraitTurnAnimator.SetBool("bPopIn", true);
    }

        public void StopPortraitAnimation()
        {
            portraitTurnAnimator.SetBool("bPopIn", false);
        }

        public void PopIn(bool showed)
        {
            GetComponent<Animator>().SetBool("bPopIn", showed);
        }

        public void StartScoreAnim()
        {
            scoreCounterAnimator.SetTrigger("PointReceived");
        }

        public void SetScoreCounter(int nb)
        {
            textCounter.text = nb.ToString();
        }

        private void PlayPortraitRollSound()
        {
            if (audioManager == null)
            {
                audioManager = FindObjectOfType<AudioManager>();
            }

            audioManager.PlayAudio(SoundID.PortraitRoll);
        }
	}