﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HelpPanel : UIPanel
{
    [Header("FR Settings")]
    [SerializeField]
    private string titleFR;

    [Header("EN Settings")]
    [SerializeField]
    private string titleEN;

    [Header("Localized Objects")]
    [SerializeField]
    private TextMeshProUGUI title;

    private Animator animator;

    private bool isFadingIn;
    public bool IsFadingOut { get { return !isFadingIn; } }
    public bool IsFadingIn { get { return isFadingIn; } }

    private ExitPanel exitPanel;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        isFadingIn = false;
        animator = GetComponent<Animator>();
        exitPanel = GetComponent<ExitPanel>();
    }

    protected override void SetLanguageEN()
    {
        title.text = titleEN;
    }

    protected override void SetLanguageFR()
    {
        title.text = titleFR;
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
            animator.SetBool("bPopIn", true);
            isFadingIn = true;
            exitPanel.enabled = true;
            GetComponent<Image>().enabled = true;

                AudioManager.Instance.PlayAudio(SoundID.OpenWindowHelp);
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
            animator.SetBool("bPopIn", false);
            isFadingIn = false;
            exitPanel.enabled = false;

            GetComponent<Image>().enabled = false;
            AudioManager.Instance.PlayAudio(SoundID.CloseWindowHelp);
        }
    }

    private void PopOutAnimationEndCallback()
    {
        if (IsFadingOut)
        {
            isFadingIn = false;
            //gameObject.SetActive(false);
        }
    }

}
