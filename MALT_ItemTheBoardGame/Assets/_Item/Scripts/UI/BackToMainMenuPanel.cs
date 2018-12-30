using System.Collections;
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

    private Animation _animation;
    private Image _image;

    private bool isShown;
    public bool IsShown { get { return isShown; } }

    private CanvasGroup canvasGroup;

    protected override void Awake()
    {
        base.Awake();

        isShown = false;
        _animation = GetComponent<Animation>();
        _image = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void PopIn()
    {
        if (isShown)
            return;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;

        _animation.Play("BackToMainMenuPopIn", PlayMode.StopAll);
        isShown = true;
        _image.raycastTarget = true;
        AudioManager.Instance.PlayAudio(SoundID.OpenWindowOptions);
    }

    public void PopOut()
    {
        if (!isShown)
            return;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        _animation.Play("BackToMainMenuPopOut", PlayMode.StopAll);
        isShown = false;
        _image.raycastTarget = false;
        
        AudioManager.Instance.PlayAudio(SoundID.CloseWindowOptions);
    }

    public void AnimEndCallback()
    {
        canvasGroup.alpha = 0;
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

        GameManager.Instance.GameEnded();
        GameManager.Instance.EndGameplay();
    }

    public void OnNoButton()
    {
        UIManager.Instance.OnBackToMainMenuButton();
    }
}
