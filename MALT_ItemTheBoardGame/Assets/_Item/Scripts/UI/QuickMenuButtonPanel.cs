using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickMenuButtonPanel : UIPanel
{
    private Animation _animation;
    private CanvasGroup canvasGroup;

    private bool isShown;
    public bool IsShown { get { return isShown; } }

    protected override void Awake()
    {
        base.Awake();

        isShown = false;
        _animation = GetComponent<Animation>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    protected override void SetLanguageEN()
    {
    }

    protected override void SetLanguageFR()
    {
    }

    public void PopIn()
    {
        if (isShown)
            return;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;

        _animation.Play("QuickMenuPopIn", PlayMode.StopAll);
        isShown = true;

        AudioManager.Instance.PlayAudio(SoundID.OpenWindowHelp);
    }

    public void PopOut()
    {
        if (!isShown)
            return;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        _animation.Play("QuickMenuPopOut", PlayMode.StopAll);
        isShown = false;

        AudioManager.Instance.PlayAudio(SoundID.CloseWindowHelp);
    }

    public void AnimEndCallback()
    {
        canvasGroup.alpha = 0;
    }
}
