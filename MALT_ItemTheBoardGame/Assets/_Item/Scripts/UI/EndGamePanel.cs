using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGamePanel : UIPanel
{
    [SerializeField]
    private Animation playAgain;

    private bool isShown;

    protected override void Awake()
    {
        base.Awake();
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
            playAgain.Play("EndGamePanelPopIn", PlayMode.StopAll);
        }
        else if (!enabled && isShown)
        {
            playAgain.Play("EndGamePanelPopOut", PlayMode.StopAll);
        }

        isShown = enabled;
    }
}
