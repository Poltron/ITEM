using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsPanel : MonoBehaviour
{
    private Animator animator;

    [SerializeField]
    private Toggle ballPlacementHelp;

    [SerializeField]
    private Toggle roundHelpPopup;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
		
	}

    public void ToggleShowPanel()
    {
        bool toggle = !gameObject.activeInHierarchy;
        gameObject.SetActive(toggle);
    }

    public void ToggleShowPanel(bool isShown)
    {
        gameObject.SetActive(isShown);
    }

    public void PopIn()
    {
        gameObject.SetActive(true);

        if (animator == null)
        {
            Init();
        }

        animator.SetTrigger("popIn");
    }

    public void PopOut()
    {
        gameObject.SetActive(false);

        if (animator == null)
        {
            Init();
        }

        animator.SetTrigger("popOut");
    }

    public void ToggleEnableBallPlacementHelp(bool notUseful)
    {
        Options.SetEnablePlacementHelp(ballPlacementHelp.isOn);
    }

    public void ToggleEnableHelpPopup(bool notUseful)
    {
        Options.SetEnableHelpPopup(roundHelpPopup.isOn);
    }
}
