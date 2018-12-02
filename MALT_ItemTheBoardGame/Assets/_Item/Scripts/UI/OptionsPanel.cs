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
    private Image imageAnimator;

    private bool isFadingIn;
    public bool IsFadingOut { get { return !isFadingIn; } }
    public bool IsFadingIn { get { return isFadingIn; } }

    public event Action OnLanguageChange;

   

    void Awake()
    {
        Init();
    }

    void Init()
    {
        isFadingIn = false;
        animator = GetComponent<Animator>();
        imageAnimator = GetComponent<Image>();
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
            animator.SetBool(animatorHashPopIn, true);
            isFadingIn = true;
            imageAnimator.enabled = true;

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

            imageAnimator.enabled = false;
            AudioManager.Instance.PlayAudio(SoundID.CloseWindowOptions);
        }
    }

    private void PopOutAnimationEndCallback()
    {
        isFadingIn = false;
        //gameObject.SetActive(false);
    }

    public void ToggleEnableBallPlacementHelp(bool notUseful)
    {
        if (ballPlacementHelp.isOn)
            AudioManager.Instance.PlayAudio(SoundID.CheckboxOn);
        else
            AudioManager.Instance.PlayAudio(SoundID.CheckboxOff);

        Options.SetEnablePlacementHelp(ballPlacementHelp.isOn);
    }

    public void ToggleEnableHelpPopup(bool notUseful)
    {
        if (roundHelpPopup.isOn)
            AudioManager.Instance.PlayAudio(SoundID.CheckboxOn);
        else
            AudioManager.Instance.PlayAudio(SoundID.CheckboxOff);

        Options.SetEnableHelpPopup(roundHelpPopup.isOn);
    }

    public void ToggleEnableAskForTuto(bool notUseful)
    {
        if (askForTutoPopup.isOn)
            AudioManager.Instance.PlayAudio(SoundID.CheckboxOn);
        else
            AudioManager.Instance.PlayAudio(SoundID.CheckboxOff);

        Options.SetAskForTuto(askForTutoPopup.isOn);
    }

    public void ToggleEnableMuteMusic(bool notUseful)
    {
        if (muteMusicToggle.isOn)
            AudioManager.Instance.PlayAudio(SoundID.CheckboxOn);
        else
            AudioManager.Instance.PlayAudio(SoundID.CheckboxOff);

        Options.SetMuteMusic(muteMusicToggle.isOn);
    }

    public void ToggleEnableMuteSFX(bool notUseful)
    {
        if (muteSFXToggle.isOn)
            AudioManager.Instance.PlayAudio(SoundID.CheckboxOn);
        else
            AudioManager.Instance.PlayAudio(SoundID.CheckboxOff);

        Options.SetMuteSFX(muteSFXToggle.isOn);
    }

    public void SetLocalization(string language)
    {
        Options.SetLanguage(language);

        if (OnLanguageChange != null)
            OnLanguageChange.Invoke();

        AudioManager.Instance.PlayAudio(SoundID.ClickUI);
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
