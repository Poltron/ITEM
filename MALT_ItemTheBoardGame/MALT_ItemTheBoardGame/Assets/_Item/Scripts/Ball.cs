
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
		public BallColor color;

		public Cell owner;

		private Vector3 startPosition;

		public SpriteRenderer highlight;

		void Awake() {
			
			SetStartPosition ();
			Color c = highlight.color;
			c.a = 0;
			highlight.color = c;
		}

		public void ShowHighlight () {
			highlight.DOFade (1, 0.3f);

		}

		public void HideHighlight() {
			highlight.DOFade (0, 0.3f);
		}

		public void ResetPosition() {
			//transform.position = startPosition;
		}

		public void SetStartPosition() {
			startPosition = transform.position;
		}
	}
}