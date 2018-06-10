using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpPanel : MonoBehaviour
{
    private Animator animator;

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
            animator.SetTrigger("HelpPanelPopIn");
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
            animator.SetTrigger("HelpPanelPopOut");
            isFadingIn = false;
        }
    }

    private void PopOutAnimationEndCallback()
    {
        if (IsFadingOut)
        {
            gameObject.SetActive(false);
        }
    }

}
