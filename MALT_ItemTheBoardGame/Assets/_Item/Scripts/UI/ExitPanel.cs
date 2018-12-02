using AppAdvisory.Item;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExitPanel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private UIManager uiManager;

    private OptionsPanel optionsPanel;
    private HelpPanel helpPanel;

    void Awake()
    {
        optionsPanel = GetComponent<OptionsPanel>();
        helpPanel = GetComponent<HelpPanel>();
    }

    void Start ()
    {
		
	}
	
	void Update ()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (optionsPanel != null)
            uiManager.OnOptionsButton();
        else if (helpPanel != null)
            uiManager.OnHelpButton();
        else
            uiManager.OnBackToMainMenuButton();
    }
}
