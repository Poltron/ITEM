using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingForPlayerPanel : UIPanel
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

    private float timer;
    [SerializeField]
    private float timeBeforeCancelButtonPops;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

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
}
