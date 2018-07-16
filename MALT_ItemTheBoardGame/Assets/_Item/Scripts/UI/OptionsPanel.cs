using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsPanel : UIPanel
{
    [Header("FR Settings")]
    [SerializeField]
    private string ballPlacementHelpLabelFR;
    [SerializeField]
    private string roundHelpPopupLabelFR;

    [Header("EN Settings")]
    [SerializeField]
    private string ballPlacementHelpLabelEN;
    [SerializeField]
    private string roundHelpPopupLabelEN;

    [Header("")]
    [SerializeField]
    private Toggle ballPlacementHelp;
    [SerializeField]
    private TextMeshProUGUI ballPlacementHelpLabel;
    [SerializeField]
    private Toggle roundHelpPopup;
    [SerializeField]
    private TextMeshProUGUI roundHelpPopupLabel;

    [Header("")]
    private Animator animator;

    private bool isFadingIn;
    public bool IsFadingOut { get { return !isFadingIn; } }
    public bool IsFadingIn { get { return isFadingIn; } }

    public event Action OnLanguageChange;

    private AudioManager audioManager;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        isFadingIn = false;
        animator = GetComponent<Animator>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    void RefreshValues()
    {
        if (Options.GetEnablePlacementHelp())
            ballPlacementHelp.isOn = true;
        else
            ballPlacementHelp.isOn = false;

        if (Options.GetEnableHelpPopup())
            roundHelpPopup.isOn = true;
        else
            roundHelpPopup.isOn = false;
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
            RefreshValues();
            animator.SetTrigger("OptionsPanelPopIn");
            isFadingIn = true;

            if (audioManager != null)
            {
                audioManager.PlayAudio(SoundID.OpenWindowOptions);
            }
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
            animator.SetTrigger("OptionsPanelPopOut");
            isFadingIn = false;

            if (audioManager != null)
            {
                audioManager.PlayAudio(SoundID.CloseWindowOptions);
            }
        }
    }

    private void PopOutAnimationEndCallback()
    {
        gameObject.SetActive(false);
    }

    public void ToggleEnableBallPlacementHelp(bool notUseful)
    {
        Options.SetEnablePlacementHelp(ballPlacementHelp.isOn);
    }

    public void ToggleEnableHelpPopup(bool notUseful)
    {
        Options.SetEnableHelpPopup(roundHelpPopup.isOn);
    }

    public void SetLocalization(string language)
    {
        Options.SetLanguage(language);

        if (OnLanguageChange != null)
            OnLanguageChange.Invoke();
    }

    protected override void SetLanguageFR()
    {
        roundHelpPopupLabel.text = roundHelpPopupLabelFR;
        ballPlacementHelpLabel.text = ballPlacementHelpLabelFR;
    }

    protected override void SetLanguageEN()
    {
        roundHelpPopupLabel.text = roundHelpPopupLabelEN;
        ballPlacementHelpLabel.text = ballPlacementHelpLabelEN;
    }
}
