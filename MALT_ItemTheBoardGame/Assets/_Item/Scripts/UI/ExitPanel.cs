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

    void Start ()
    {
		
	}
	
	void Update ()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GetComponent<OptionsPanel>() != null)
            uiManager.OnOptionsButton();
        else if (GetComponent<HelpPanel>() != null)
            uiManager.OnHelpButton();
        else
            uiManager.OnBackToMainMenuButton();
    }
}
