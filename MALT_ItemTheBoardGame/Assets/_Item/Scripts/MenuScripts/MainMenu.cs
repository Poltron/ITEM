using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : UIPanel
{
    [Header("FR Settings")]
    [SerializeField]
    private string localButtonTextFR;
    [SerializeField]
    private string remoteButtonTextFR;
    [SerializeField]
    private string aiButtonTextFR;
    [SerializeField]
    private string aiChoiceTextFR;

    [Header("EN Settings")]
    [SerializeField]
    private string localButtonTextEN;
    [SerializeField]
    private string remoteButtonTextEN;
    [SerializeField]
    private string aiButtonTextEN;
    [SerializeField]
    private string aiChoiceTextEN;

    [Header("")]
    [SerializeField]
    private Text localButtonText;
    [SerializeField]
    private Text remoteButtonText;
    [SerializeField]
    private Text aiButtonText;
    [SerializeField]
    private TextMeshProUGUI aiChoiceText;

    [SerializeField]
    private GameObject menuButtons;
    [SerializeField]
    private GameObject loginButtons;
    [SerializeField]
    private GameObject aiChoiceButtons;
    [SerializeField]
    private AIPanel[] aiPanels;

    protected override void SetLanguageFR()
    {
        localButtonText.text = localButtonTextFR;
        remoteButtonText.text = remoteButtonTextFR;
        aiButtonText.text = aiButtonTextFR;
        aiChoiceText.text = aiChoiceTextFR;
    }

    protected override void SetLanguageEN()
    {
        localButtonText.text = localButtonTextEN;
        remoteButtonText.text = remoteButtonTextEN;
        aiButtonText.text = aiButtonTextEN;
        aiChoiceText.text = aiChoiceTextEN;
    }

    public void ShowMenu()
    {
        menuButtons.SetActive(true);
        loginButtons.SetActive(false);
        aiChoiceButtons.SetActive(false);
    }

    public void ShowLogin()
    {
        menuButtons.SetActive(false);
        loginButtons.SetActive(true);
        aiChoiceButtons.SetActive(false);
    }

    public void ShowAIs()
    {
        menuButtons.SetActive(false);
        loginButtons.SetActive(false);
        aiChoiceButtons.SetActive(true);

        foreach(AIPanel aiPanel in aiPanels)
        {
            aiPanel.Refresh();
        }
    }

    public void HideAll()
    {
        menuButtons.SetActive(false);
        loginButtons.SetActive(false);
        aiChoiceButtons.SetActive(false);
    }

    public void PlayLocalDuel()
    {
        Debug.Log("PlayLocalDuel");
        GameManager.Instance.StartLocalGame();
    }

    public void PlayRemoteDuel()
    {
        Debug.Log("PlayRemoteDuel");
        GameManager.Instance.StartLookingForOpponent();
    }

    public void PlayVSAI()
    {
        Debug.Log("PlayVSAI");
        ShowAIs();
        //GameManager.Instance.StartGameVSIA();
    }
}
