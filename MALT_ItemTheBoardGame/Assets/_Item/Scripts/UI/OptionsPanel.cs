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
    private Animation _animation;
    private Image _image;

    private bool isShown;
    public bool IsShown { get { return isShown; } }

    private CanvasGroup canvasGroup;

    public event Action OnLanguageChange;

    protected override void Awake()
    {
        base.Awake();

        isShown = false;
        _animation = GetComponent<Animation>();
        _image = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
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
        if (isShown)
            return;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;

        RefreshValues();
        _animation.Play("OptionsPanelPopIn", PlayMode.StopAll);
        isShown = true;
        _image.enabled = true;

        AudioManager.Instance.PlayAudio(SoundID.OpenWindowOptions);
    }

    public void PopOut()
    {
        if (!isShown)
            return;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        _animation.Play("OptionsPanelPopOut", PlayMode.StopAll);
        _image.enabled = false;
        isShown = false;

        AudioManager.Instance.PlayAudio(SoundID.CloseWindowOptions);
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

    public void AnimEndCallback()
    {
        canvasGroup.alpha = 0;
    }
}
