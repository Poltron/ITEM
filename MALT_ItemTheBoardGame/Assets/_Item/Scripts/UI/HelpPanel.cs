using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class HelpPanel : UIPanel
{
    [Header("FR Settings")]
    [SerializeField]
    private string titleFR;
    [SerializeField]
    private string firstLineFR;
    [SerializeField]
    private string secondLineFR;
    [SerializeField]
    private string crossFR;
    [SerializeField]
    private string orFR;
    [SerializeField]
    private string lineFR;

    [Header("EN Settings")]
    [SerializeField]
    private string titleEN;
    [SerializeField]
    private string firstLineEN;
    [SerializeField]
    private string secondLineEN;
    [SerializeField]
    private string crossEN;
    [SerializeField]
    private string orEN;
    [SerializeField]
    private string lineEN;

    [Header("Localized Objects")]
    [SerializeField]
    private TextMeshProUGUI title;
    [SerializeField]
    private TextMeshProUGUI firstLine;
    [SerializeField]
    private TextMeshProUGUI secondLine;
    [SerializeField]
    private TextMeshProUGUI cross;
    [SerializeField]
    private TextMeshProUGUI or;
    [SerializeField]
    private TextMeshProUGUI line;

    private Animation _animation;

    private bool isShown;
    public bool IsShown { get { return isShown; } }
    private ExitPanel exitPanel;
    private Image _image;
    private CanvasGroup canvasGroup;

    protected override void Awake()
    {
        base.Awake();

        isShown = false;
        _animation = GetComponent<Animation>();
        exitPanel = GetComponent<ExitPanel>();
        _image = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    protected override void SetLanguageEN()
    {
        title.text = titleEN;
        firstLine.text = firstLineEN;
        secondLine.text = secondLineEN;
        cross.text = crossEN;
        or.text = orEN;
        line.text = lineEN;
    }

    protected override void SetLanguageFR()
    {
        title.text = titleFR;
        firstLine.text = firstLineFR;
        secondLine.text = secondLineFR;
        cross.text = crossFR;
        or.text = orFR;
        line.text = lineFR;
    }

    public void PopIn()
    {
        if (isShown)
            return;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;

        _animation.Play("HelpPanelPopIn", PlayMode.StopAll);
        isShown = true;
        exitPanel.enabled = true;
        _image.enabled = true;

        AudioManager.Instance.PlayAudio(SoundID.OpenWindowHelp);
    }

    public void PopOut()
    {
        if (!isShown)
            return;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        _animation.Play("HelpPanelPopOut", PlayMode.StopAll);
        isShown = false;
        exitPanel.enabled = false;

        _image.enabled = false;
        AudioManager.Instance.PlayAudio(SoundID.CloseWindowHelp);
    }

    public void AnimEndCallback()
    {
        canvasGroup.alpha = 0;
    }
}
