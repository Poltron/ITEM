using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsPanel : MonoBehaviour
{
    private Animator animator;

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

    public void ToggleCheckmark()
    {

    }
}
