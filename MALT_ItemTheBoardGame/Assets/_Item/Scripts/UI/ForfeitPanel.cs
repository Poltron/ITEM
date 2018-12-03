using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForfeitPanel : UIPanel
{
    [Header("FR Settings")]
    [SerializeField]
    private string labelFR;
    [SerializeField]
    private string cancelButtonTextFR;

    [Header("EN Settings")]
    [SerializeField]
    private string labelEN;
    [SerializeField]
    private string cancelButtonTextEN;

    [Header("")]
    [SerializeField]
    private Text label;
    [SerializeField]
    private Text cancelButtonText;
    [SerializeField]
    private Button cancelButton;
    private Animator animator;

    protected override void Awake()
    {
        base.Awake();

        animator = GetComponent<Animator>();
    }

    void Start()
    {
        SetLanguage(Options.GetLanguage());
    }

    protected override void SetLanguageEN()
    {
        label.text = labelEN;
        cancelButtonText.text = cancelButtonTextEN;
    }

    protected override void SetLanguageFR()
    {
        label.text = labelFR;
        cancelButtonText.text = cancelButtonTextFR;
    }

    public void Display(bool showed)
    {
        animator.SetBool(animatorHashPopIn, showed);
    }
}
