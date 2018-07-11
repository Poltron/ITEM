using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingForPlayerPanel : UIPanel
{
    [Header("FR Settings")]
    [SerializeField]
    private string labelFR;

    [Header("EN Settings")]
    [SerializeField]
    private string labelEN;

    [Header("")]
    [SerializeField]
    private Text label;

    void Start()
    {
		
	}

    protected override void SetLanguageEN()
    {
        label.text = labelEN;
    }

    protected override void SetLanguageFR()
    {
        label.text = labelFR;
    }

}
