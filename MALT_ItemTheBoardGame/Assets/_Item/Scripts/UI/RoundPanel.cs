using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoundPanel : UIPanel
{
    private Animation roundAnimation;

    private bool isShown;

    protected override void Awake()
    {
        base.Awake();

        roundAnimation = GetComponent<Animation>();
    }

    protected override void SetLanguageEN()
    {
    }

    protected override void SetLanguageFR()
    {
    }

    public void Display(bool enabled)
    {
        if (enabled && !isShown)
        {
            roundAnimation.Play("RoundPanelPopIn", PlayMode.StopAll);
        }
        else if (!enabled && isShown)
        {
            roundAnimation.Play("RoundPanelPopOut", PlayMode.StopAll);
        }

        isShown = enabled;
    }
}