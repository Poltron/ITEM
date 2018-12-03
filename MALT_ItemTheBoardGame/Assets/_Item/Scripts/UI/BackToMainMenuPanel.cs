﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackToMainMenuPanel : UIPanel
{

    [Header("FR Settings")]
    [SerializeField]
    private string yesLabelFR;
    [SerializeField]
    private string noLabelFR;
    [SerializeField]
    private string confirmLabelFR;
    [SerializeField]
    private string gameLabelFR;

    [Header("EN Settings")]
    [SerializeField]
    private string yesLabelEN;
    [SerializeField]
    private string noLabelEN;
    [SerializeField]
    private string confirmLabelEN;
    [SerializeField]
    private string gameLabelEN;

    [Header("")]
    [SerializeField]
    private TextMeshProUGUI noButtonText;
    [SerializeField]
    private TextMeshProUGUI yesButtonText;
    [SerializeField]
    private TextMeshProUGUI confirmText;
    [SerializeField]
    private TextMeshProUGUI gameText;

    private Animator animator;
    private Image _image;

    private bool isFadingIn;
    public bool IsFadingOut { get { return !isFadingIn; } }
    public bool IsFadingIn { get { return isFadingIn; } }

    protected override void Awake()
    {
        base.Awake();

        Init();
    }

    void Init()
    {
        isFadingIn = false;
        animator = GetComponent<Animator>();
        _image = GetComponent<Image>();
    }

    public void PopIn()
    {
        gameObject.SetActive(true);

        if (animator == null)
        {
            Init();
        }

        if (animator.gameObject.activeInHierarchy)
        {
            animator.SetBool(animatorHashPopIn, true);
            isFadingIn = true;
            _image.raycastTarget = true;
            AudioManager.Instance.PlayAudio(SoundID.OpenWindowOptions);
        }
    }

    public void PopOut()
    {
        if (animator == null)
        {
            Init();
        }

        if (animator.gameObject.activeInHierarchy)
        {
            animator.SetBool(animatorHashPopIn, false);
            isFadingIn = false;
            _image.raycastTarget = false;

            AudioManager.Instance.PlayAudio(SoundID.CloseWindowOptions);
        }
    }

    private void PopOutAnimationEndCallback()
    {
        isFadingIn = false;
        //gameObject.SetActive(false);
    }

    protected override void SetLanguageEN()
    {
        noButtonText.text = noLabelEN;
        yesButtonText.text = yesLabelEN;
        confirmText.text = confirmLabelEN;
        gameText.text = gameLabelEN;
    }

    protected override void SetLanguageFR()
    {
        noButtonText.text = noLabelFR;
        yesButtonText.text = yesLabelFR;
        confirmText.text = confirmLabelFR;
        gameText.text = gameLabelFR;
    }

    public void OnYesButton()
    {
        PopOut();
        gameObject.SetActive(false);

        GameManager.Instance.GameEnded();
        GameManager.Instance.EndGameplay();
    }

    public void OnNoButton()
    {
        UIManager.Instance.OnBackToMainMenuButton();
    }
}
