
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


namespace AppAdvisory.Item
{
	public class Cell : MonoBehaviour 
	{

		public int x;
		public int y;
        
		public Ball ball;

        private SpriteRenderer spriteRenderer;
		private CircleCollider2D circleCollider;

        private Animator animator;

        void Awake()
        {
            animator = GetComponent<Animator>();
			circleCollider = GetComponent<CircleCollider2D> ();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetCollider(bool isActive)
        {
			circleCollider.enabled = isActive;
        }

        public bool HasBall()
        {
            return ball != null;
        }

        public bool HasBall(BallColor color)
        {
            if (!HasBall())
                return false;

            return ball.Color == color;
        }

        public void SetModelPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void SetHighlightedCell(bool isHighlighted)
        {
            animator.SetBool("Highlighted", isHighlighted);
        }

        public void PassAboveUI(bool isEnabled)
        {
            if (isEnabled)
                spriteRenderer.sortingLayerID = SortingLayer.NameToID("AboveUI");
            else
                spriteRenderer.sortingLayerID = SortingLayer.NameToID("Cell");
        }
    }
}