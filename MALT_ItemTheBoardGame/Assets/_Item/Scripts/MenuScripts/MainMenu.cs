using System.Collections;
using System.Collections.Generic;
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

    [Header("EN Settings")]
    [SerializeField]
    private string localButtonTextEN;
    [SerializeField]
    private string remoteButtonTextEN;
    [SerializeField]
    private string aiButtonTextEN;

    [Header("")]
    [SerializeField]
    private Text localButtonText;
    [SerializeField]
    private Text remoteButtonText;
    [SerializeField]
    private Text aiButtonText;

    [SerializeField]
    private GameObject menuButtons;
    [SerializeField]
    private GameObject loginButtons;
    [SerializeField]
    private GameObject aiChoiceButtons;

    protected override void SetLanguageFR()
    {
        localButtonText.text = localButtonTextFR;
        remoteButtonText.text = remoteButtonTextFR;
        aiButtonText.text = aiButtonTextFR;
    }

    protected override void SetLanguageEN()
    {
        localButtonText.text = localButtonTextEN;
        remoteButtonText.text = remoteButtonTextEN;
        aiButtonText.text = aiButtonTextEN;
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
        GameManager.Instance.StartGameVSIA();
    }
}
