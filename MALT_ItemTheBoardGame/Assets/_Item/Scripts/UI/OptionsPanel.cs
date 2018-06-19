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

    private bool isFadingIn;
    public bool IsFadingOut { get { return !isFadingIn; } }
    public bool IsFadingIn { get { return isFadingIn; } }

    void Awake()
    {
        Init();
    }

    void Init()
    {
        isFadingIn = false;
        animator = GetComponent<Animator>();
    }

    public void PopIn()
    {
        gameObject.SetActive(true);

        if (animator == null)
        {
            Init();
        }

        if (animator.gameObject.activeInHierarchy)
        {
            animator.SetTrigger("OptionsPanelPopIn");
            isFadingIn = true;
        }
    }

    public void PopOut()
    {
        if (animator == null)
        {
            Init();
        }

        if (animator.gameObject.activeInHierarchy)
        {
            animator.SetTrigger("OptionsPanelPopOut");
            isFadingIn = false;
        }
    }

    private void PopOutAnimationEndCallback()
    {
        gameObject.SetActive(false);
    }

    public void ToggleEnableBallPlacementHelp(bool notUseful)
    {
        Options.SetEnablePlacementHelp(ballPlacementHelp.isOn);
    }

    public void ToggleEnableHelpPopup(bool notUseful)
    {
        Options.SetEnableHelpPopup(roundHelpPopup.isOn);
    }

    public void SetLanguage(string language)
    {
        Options.SetLanguage(language);
    }
}
