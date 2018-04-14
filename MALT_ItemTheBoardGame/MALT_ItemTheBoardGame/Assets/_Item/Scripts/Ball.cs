
/***********************************************************************************************************
 * Produced by App Advisory - http://app-advisory.com
 * Facebook: https://facebook.com/appadvisory
 * Contact us: https://appadvisory.zendesk.com/hc/en-us/requests/new
 * App Advisory Unity Asset Store catalog: http://u3d.as/9cs
 * Developed by Gilbert Anthony Barouch - https://www.linkedin.com/in/ganbarouch
 ***********************************************************************************************************/




using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace AppAdvisory.Item
{
	public class Ball : MonoBehaviour 
	{
        [SerializeField]
		private BallColor color;

        public BallColor Color { get { return color; } }

        [SerializeField]
        int score;

        public int Score { get { return score; } }
        
		public Cell owner;

		private Vector3 startPosition;

        [SerializeField]
		private SpriteRenderer highlight;

        [SerializeField]
        private AnimationCurve curve;

        float highlightCurveTimer;

		void Awake()
        {
			SetStartPosition();
		}

        private void Start()
        {
            HideHighlight();
        }

        private void Update()
        {
            if (highlight.gameObject.activeInHierarchy)
            {
                highlightCurveTimer += Time.deltaTime;
                float scale = curve.Evaluate(highlightCurveTimer);
                highlight.transform.localScale = new Vector3(scale, scale, 1);
            }
        }

        public void ShowHighlight () {
            highlightCurveTimer = 0;
            highlight.gameObject.SetActive(true);
		}

		public void HideHighlight() {
            highlight.gameObject.SetActive(false);
		}
		public void ResetPosition() {
			//transform.position = startPosition;
		}

		public void SetStartPosition() {
			startPosition = transform.position;
		}
	}
}