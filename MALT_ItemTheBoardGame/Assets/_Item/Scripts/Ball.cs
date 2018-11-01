using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine.Rendering;

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

    private Animator _animator;
    public Animator Animator { get { return _animator; } }

    public bool isPickedUp;

    private bool isAboveUI;

    private bool noFX;
    private AudioManager audioManager;

    void Awake()
    {
        startPosition = transform.position;
        startScale = transform.localScale;
        _animator =  GetComponent<Animator>();
        audioManager = FindObjectOfType<AudioManager>();
        noFX = false;
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

    public void PickUpBall()
    {
        if (!isAboveUI)
            FixSortingLayer(true);

        ShowHighlight();
        _animator.SetTrigger("PickUp");
        audioManager.PlayAudio(SoundID.PawnSelect);
        isPickedUp = true;
    }

    public void PutDownBall()
    {
        FixSortingLayer(false);
        HideHighlight();
        _animator.SetTrigger("PutDown");
        isPickedUp = false;
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
        isAboveUI = enabled;

        if (enabled)
        {
            ballSprite.GetComponent<SortingGroup>().sortingLayerID = SortingLayer.NameToID("AboveUI");
        }
        else
        {
            ballSprite.GetComponent<SortingGroup>().sortingLayerID = SortingLayer.NameToID("Ball");
        }
    }

    public void FixSortingLayer(bool enabled)
    {
        if (enabled)
        {
            ballSprite.GetComponent<SortingGroup>().sortingOrder = 1;
            ballSprite.GetComponent<SortingGroup>().sortingOrder += (int)Camera.main.WorldToScreenPoint(transform.position).y;
            ballShadow.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
        else
        {
            ballSprite.GetComponent<SortingGroup>().sortingOrder = 0;
            ballShadow.GetComponent<SpriteRenderer>().sortingOrder = 0;
        }
    }

    public void Reset()
    {
        owner = null;
        transform.localScale = startScale;
        isPickedUp = false;
    }

    private void OnBallTouchBoard()
    {
        if (noFX)
        {
            noFX = false;
            return;
        }

        if (FXTouchBoard)
            GameObject.Instantiate(FXTouchBoard, transform.position, Quaternion.identity);

        audioManager.PlayAudio(SoundID.PawnPlace);
    }

    private void OnBallTouchBoardVictory()
    {
        if (noFX)
        {
            noFX = false;
            return;
        }

        Camera.main.GetComponent<CameraShake>().Shake();
        audioManager.PlayComboImpact();
        GameObject.Instantiate(FXTouchBoardVictory, transform.position, Quaternion.identity);
    }

    private void PopScoreParticle()
    {
        FindObjectOfType<GridManager>().BallAddScore(this);
    }

    static public bool resetPlaySound;

    public void MoveToResetPosition()
    {
        if (transform.position == startPosition)
            return;

        transform.DOMove(startPosition, 0.75f).OnComplete(() =>
        {
            if (resetPlaySound)
            {
                audioManager.PlayAudio(SoundID.PawnPlace);
                resetPlaySound = false;
            }
        });

        _animator.SetTrigger("Move");
        noFX = true;
    }

    public void PlayComboLiftUpSound()
    {
        audioManager.PlayNextBallPopSound();
    }
}