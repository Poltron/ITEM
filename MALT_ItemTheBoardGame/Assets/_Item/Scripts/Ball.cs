
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
        private GameObject FXTouchBoard;

        [SerializeField]
        private GameObject FXTouchBoardVictory;

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
                ballSprite.sortingLayerID = SortingLayer.NameToID("AboveUI");
                ballNumberText.sortingLayerID = SortingLayer.NameToID("AboveUI");
                ballShadow.sortingLayerID = SortingLayer.NameToID("AboveUI");
                highlight.sortingLayerID = SortingLayer.NameToID("AboveUI");
            }
            else
            {
                ballSprite.sortingLayerID = SortingLayer.NameToID("Ball");
                ballNumberText.sortingLayerID = SortingLayer.NameToID("BallNumber");
                ballShadow.sortingLayerID = SortingLayer.NameToID("Default");
                highlight.sortingLayerID = SortingLayer.NameToID("Highlight");
            }
        }

        public void Reset()
        {
            owner = null;
            transform.position = startPosition;
            transform.localScale = startScale;
        }

        private void OnBallTouchBoard()
        {
            Debug.Log("balltouchboard");
            //Camera.main.GetComponent<CameraShake>().Shake(0.1f, 0.5f, 0.3f);
            GameObject.Instantiate(FXTouchBoard, transform.position, Quaternion.identity);
        }

        private void OnBallTouchBoardVictory()
        {
            Debug.Log("balltouchboardvictory");
            Camera.main.GetComponent<CameraShake>().Shake();    
            GameObject.Instantiate(FXTouchBoardVictory, transform.position, Quaternion.identity);
        }
    }
}