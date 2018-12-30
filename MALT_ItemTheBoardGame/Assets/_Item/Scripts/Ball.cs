using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine.Rendering;

public enum BallColor : byte
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

    [HideInInspector]
    public Vector3 startPosition;
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

    private SortingGroup sortingGroup;

    float highlightCurveTimer;

    private Animator _animator;
    public Animator Animator { get { return _animator; } }

    public bool isPickedUp;

    private bool isAboveUI;

    private bool noFX;

    public int AnimatorHashPlaceBall;
    public int AnimatorHashMove;
    private int animatorHashPutDown;
    private int animatorHashPickUp;

    void Awake()
    {
        startPosition = transform.localPosition;
        startScale = transform.localScale;
        _animator =  GetComponent<Animator>();
        sortingGroup = ballSprite.GetComponent<SortingGroup>();
        noFX = false;

        AnimatorHashPlaceBall = Animator.StringToHash("PlaceBall");
        AnimatorHashMove = Animator.StringToHash("Move");
        animatorHashPutDown = Animator.StringToHash("PutDown");
        animatorHashPickUp = Animator.StringToHash("PickUp");
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
        _animator.SetTrigger(animatorHashPickUp);
        AudioManager.Instance.PlayAudio(SoundID.PawnSelect);
        isPickedUp = true;
    }

    public void PutDownBall()
    {
        FixSortingLayer(false);
        HideHighlight();
        _animator.SetTrigger(animatorHashPutDown);
        isPickedUp = false;
    }

    public void ShowHighlight() {
        highlightCurveTimer = 0;
        highlight.gameObject.SetActive(true);

        if (owner)
            GridManager.Instance.HighlightAvailableMoveCells(owner);
    }

    public void HideHighlight() {
        highlight.gameObject.SetActive(false);
    }

    public void PassAboveUI(bool enabled)
    {
        isAboveUI = enabled;

        if (enabled)
        {
            sortingGroup.sortingLayerID = SortingLayer.NameToID("AboveUI");
        }
        else
        {
            sortingGroup.sortingLayerID = SortingLayer.NameToID("Ball");
        }
    }

    public void FixSortingLayer(bool enabled)
    {
        if (enabled)
        {
            sortingGroup.sortingOrder = 1;
            sortingGroup.sortingOrder += (int)Camera.main.WorldToScreenPoint(transform.position).y;
            ballShadow.sortingOrder = 1;
        }
        else
        {
            sortingGroup.sortingOrder = 0;
            ballShadow.sortingOrder = 0;
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

        AudioManager.Instance.PlayAudio(SoundID.PawnPlace);
    }

    private void OnBallTouchBoardVictory()
    {
        if (noFX)
        {
            noFX = false;
            return;
        }

        Camera.main.GetComponent<CameraShake>().Shake();
        AudioManager.Instance.PlayComboImpact();
        GameObject.Instantiate(FXTouchBoardVictory, transform.position, Quaternion.identity);
    }

    private void PopScoreParticle()
    {
        GridManager.Instance.BallAddScore(this);
    }

    static public bool resetPlaySound;

    public void MoveToResetPosition()
    {
        if (transform.localPosition == startPosition)
            return;

        transform.DOLocalMove(startPosition, 0.75f).OnComplete(() =>
        {
            if (resetPlaySound)
            {
                AudioManager.Instance.PlayAudio(SoundID.PawnPlace);
                resetPlaySound = false;
            }
        });

        _animator.SetTrigger(AnimatorHashMove);
        noFX = true;
    }

    public void PlayComboLiftUpSound()
    {
        AudioManager.Instance.PlayNextBallPopSound();
    }
}