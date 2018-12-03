using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoundPanel : UIPanel
{
    [SerializeField]
    private Animator animator;

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

    public void Activate(bool isShown)
    {
        animator.gameObject.SetActive(isShown);
    }

    public void Display(bool isShown)
    {
        if (isShown)
            Activate(true);

        animator.SetBool(animatorHashPopIn, isShown);
    }
}