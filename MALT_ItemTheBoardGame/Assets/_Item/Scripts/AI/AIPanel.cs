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
    private Color color;

    private void Awake()
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.Instance.StartGameVSIA(profile);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = color;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = Color.white;
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