using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AIPanel : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private AIProfile profile;

    [SerializeField]
    private Image image;

    [SerializeField]
    private TextMeshProUGUI aiName;

    [SerializeField]
    private Animator animator;

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.Instance.StartGameVSIA(profile);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.gameObject.SetActive(true);
        animator.SetTrigger("PopIn");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (animator.gameObject.activeInHierarchy)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (!stateInfo.IsName("Base.Empty"))
                animator.SetTrigger("PopOut");
        }
    }

    public void Refresh()
    {
        image.sprite = profile.Image;

        string res = profile.Name;
        if (Options.IsLanguageEn())
            res += " - " + profile.LevelEN;
        else
            res += " - " + profile.LevelFR;

        aiName.text = res;
    }
}