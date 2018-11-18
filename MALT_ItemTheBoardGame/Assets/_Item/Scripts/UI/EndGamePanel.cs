using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGamePanel : UIPanel
{
    [SerializeField]
    private Animator playAgain;
    
    protected override void SetLanguageEN()
    {
    }

    protected override void SetLanguageFR()
    {
    }

    public void Activate(bool isShown)
    {
        playAgain.gameObject.SetActive(isShown);
    }

    public void Display(bool isshown)
    {
        if (isshown)
            Activate(true);

        playAgain.SetBool("bPopIn", isshown);
    }
}
