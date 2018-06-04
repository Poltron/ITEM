
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
using TMPro;

namespace AppAdvisory.Item
{
    public enum BallColor : int
    {
        White = 1,
        Black = 2
    };

    public class Ball : MonoBehaviour
    {
        public int ballId;

        [SerializeField]
        private BallColor color;

        public BallColor Color { get { return color; } }

        [SerializeField]
        int score;

        public int Score { get { return score; } }

        public Cell owner;

        private Vector3 startPosition;
        private Vector3 startScale;

        [SerializeField]
        private SpriteRenderer ballSprite;
        [SerializeField]
        private TextMeshPro ballNumberText;
        [SerializeField]
        private SpriteRenderer ballShadow;
        [SerializeField]
        private SpriteRenderer highlight;

        [SerializeField]
        private AnimationCurve highlightCurve;

        float highlightCurveTimer;

        void Awake()
        {
            startPosition = transform.position;
            startScale = transform.localScale;
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
                float scale = highlightCurve.Evaluate(highlightCurveTimer);
                highlight.transform.localScale = new Vector3(scale, scale, 1);
            }
        }

        public void ShowHighlight() {
            highlightCurveTimer = 0;
            highlight.gameObject.SetActive(true);

            if (owner)
                FindObjectOfType<GridManager>().HighlightAvailableMoveCells(owner);
        }

        public void HideHighlight() {
            highlight.gameObject.SetActive(false);
        }

        public void ResetPosition() {
            //transform.position = startPosition;
        }

        public void PassAboveUI(bool enabled)
        {
            if (enabled)
            {
                ballSprite.sortingLayerID = SortingLayer.GetLayerValueFromName("AboveUI");
                ballNumberText.sortingLayerID = SortingLayer.GetLayerValueFromName("AboveUI");
                ballShadow.sortingLayerID = SortingLayer.GetLayerValueFromName("AboveUI");
                highlight.sortingLayerID = SortingLayer.GetLayerValueFromName("AboveUI");
            }
            else
            {
                ballSprite.sortingLayerID = SortingLayer.GetLayerValueFromName("Ball");
                ballNumberText.sortingLayerID = SortingLayer.GetLayerValueFromName("BallNumber");
                ballShadow.sortingLayerID = SortingLayer.GetLayerValueFromName("Default");
                highlight.sortingLayerID = SortingLayer.GetLayerValueFromName("Highlight");
            }
        }

        public void Reset()
        {
            owner = null;
            transform.position = startPosition;
            transform.localScale = startScale;
        }
	}
}