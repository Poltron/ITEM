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
    [SerializeField]
    private string askForTutoLabelFR;
    [SerializeField]
    private string muteMusicLabelFR;
    [SerializeField]
    private string muteSFXFR;

    [Header("EN Settings")]
    [SerializeField]
    private string ballPlacementHelpLabelEN;
    [SerializeField]
    private string roundHelpPopupLabelEN;
    [SerializeField]
    private string askForTutoLabelEN;
    [SerializeField]
    private string muteMusicLabelEN;
    [SerializeField]
    private string muteSFXEN;

    [Header("")]
    [SerializeField]
    private Toggle ballPlacementHelp;
    [SerializeField]
    private TextMeshProUGUI ballPlacementHelpLabel;
    [SerializeField]
    private Toggle roundHelpPopup;
    [SerializeField]
    private TextMeshProUGUI roundHelpPopupLabel;
    [SerializeField]
    private Toggle askForTutoPopup;
    [SerializeField]
    private TextMeshProUGUI askForTutoLabel;
    [SerializeField]
    private Toggle muteMusicToggle;
    [SerializeField]
    private TextMeshProUGUI muteMusicLabel;
    [SerializeField]
    private Toggle muteSFXToggle;
    [SerializeField]
    private TextMeshProUGUI muteSFXLabel;

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

        if (Options.GetAskForTuto())
            askForTutoPopup.isOn = true;
        else
            askForTutoPopup.isOn = false;

        if (Options.GetMuteMusic())
            muteMusicToggle.isOn = true;
        else
            muteMusicToggle.isOn = false;

        if (Options.GetMuteSFX())
            muteSFXToggle.isOn = true;
        else
            muteSFXToggle.isOn = false;
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
            animator.SetBool("bPopIn", true);
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
            animator.SetBool("bPopIn", false);
            isFadingIn = false;

            if (audioManager != null)
            {
                audioManager.PlayAudio(SoundID.CloseWindowOptions);
            }
        }
    }

    private void PopOutAnimationEndCallback()
    {
        isFadingIn = false;
        gameObject.SetActive(false);
    }

    public void ToggleEnableBallPlacementHelp(bool notUseful)
    {
        if (ballPlacementHelp.isOn)
            audioManager.PlayAudio(SoundID.CheckboxOn);
        else
            audioManager.PlayAudio(SoundID.CheckboxOff);

        Options.SetEnablePlacementHelp(ballPlacementHelp.isOn);
    }

    public void ToggleEnableHelpPopup(bool notUseful)
    {
        if (roundHelpPopup.isOn)
            audioManager.PlayAudio(SoundID.CheckboxOn);
        else
            audioManager.PlayAudio(SoundID.CheckboxOff);

        Options.SetEnableHelpPopup(roundHelpPopup.isOn);
    }

    public void ToggleEnableAskForTuto(bool notUseful)
    {
        if (askForTutoPopup.isOn)
            audioManager.PlayAudio(SoundID.CheckboxOn);
        else
            audioManager.PlayAudio(SoundID.CheckboxOff);

        Options.SetAskForTuto(askForTutoPopup.isOn);
    }

    public void ToggleEnableMuteMusic(bool notUseful)
    {
        if (muteMusicToggle.isOn)
            audioManager.PlayAudio(SoundID.CheckboxOn);
        else
            audioManager.PlayAudio(SoundID.CheckboxOff);

        Options.SetMuteMusic(muteMusicToggle.isOn);
    }

    public void ToggleEnableMuteSFX(bool notUseful)
    {
        if (muteSFXToggle.isOn)
            audioManager.PlayAudio(SoundID.CheckboxOn);
        else
            audioManager.PlayAudio(SoundID.CheckboxOff);

        Options.SetMuteSFX(muteSFXToggle.isOn);
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
        askForTutoLabel.text = askForTutoLabelFR;
        muteMusicLabel.text = muteMusicLabelFR;
        muteSFXLabel.text = muteSFXFR;
    }

    protected override void SetLanguageEN()
    {
        roundHelpPopupLabel.text = roundHelpPopupLabelEN;
        ballPlacementHelpLabel.text = ballPlacementHelpLabelEN;
        askForTutoLabel.text = askForTutoLabelEN;
        muteMusicLabel.text = muteMusicLabelEN;
        muteSFXLabel.text = muteSFXEN;
    }
}
