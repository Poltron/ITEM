using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EasyUIAnimator;

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
    private Animation leftMenuAnimation;
    [SerializeField]
    private Animation rightMenuAnimation;
    [SerializeField]
    private Animation waitingForPlayerAnimation;
    [SerializeField]
    private Animation aiPanelAnimation;
    [SerializeField]
    private GameObject waitingForPlayerPanel;
    [SerializeField]
    private AIPanel[] aiPanels;

    private bool waitingForPlayer;
    private bool ai;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        PopIn();
    }

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

    public void PopIn()
    {
        GetComponent<CanvasGroup>().alpha = 1;
        GetComponent<CanvasGroup>().interactable = true;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        leftMenuAnimation.Play("LeftMenuPopIn", PlayMode.StopAll);
        rightMenuAnimation.Play("RightMenuPopIn", PlayMode.StopAll);
    }

    public void PopOut()
    {
        GetComponent<CanvasGroup>().interactable = false;
        GetComponent<CanvasGroup>().blocksRaycasts = false;

        DOVirtual.DelayedCall(1.0f, () => {
            GetComponent<CanvasGroup>().alpha = 0;
        });

        leftMenuAnimation.Play("LeftMenuPopOut", PlayMode.StopAll);
        rightMenuAnimation.Play("RightMenuPopOut", PlayMode.StopAll);

        ShowWaitingForPlayer(false);
        ShowAIs(false);
    }

    public void ShowWaitingForPlayer(bool showed)
    {
        if (showed)
        {
            waitingForPlayerAnimation.GetComponent<CanvasGroup>().alpha = 1;
            waitingForPlayerAnimation.GetComponent<CanvasGroup>().interactable = true;
            waitingForPlayerAnimation.GetComponent<CanvasGroup>().blocksRaycasts = true;

            waitingForPlayerAnimation.Play("WaitingForPlayerPopIn", PlayMode.StopAll);
        }
        else if (waitingForPlayer)
        {
            waitingForPlayerAnimation.GetComponent<CanvasGroup>().interactable = false;
            waitingForPlayerAnimation.GetComponent<CanvasGroup>().blocksRaycasts = false;

            DOVirtual.DelayedCall(0.5f, () =>
            {
                waitingForPlayerAnimation.GetComponent<CanvasGroup>().alpha = 0;
            });

            waitingForPlayerAnimation.Play("WaitingForPlayerPopOut", PlayMode.StopAll);
        }

        waitingForPlayer = showed;
    }

    public void ShowAIs(bool showed)
    {
        if (showed)
        {
            aiPanelAnimation.GetComponent<CanvasGroup>().alpha = 1;
            aiPanelAnimation.GetComponent<CanvasGroup>().interactable = true;
            aiPanelAnimation.GetComponent<CanvasGroup>().blocksRaycasts = true;

            aiPanelAnimation.Play("AIMenuPopIn", PlayMode.StopAll);
        }
        else if (ai)
        {
            aiPanelAnimation.GetComponent<CanvasGroup>().interactable = false;
            aiPanelAnimation.GetComponent<CanvasGroup>().blocksRaycasts = false;

            DOVirtual.DelayedCall(0.5f, () =>
            {
                aiPanelAnimation.GetComponent<CanvasGroup>().alpha = 0;
            });

            aiPanelAnimation.Play("AIMenuPopOut", PlayMode.StopAll);
        }

        ai = showed;

        if (showed)
        {
            foreach (AIPanel aiPanel in aiPanels)
            {
                aiPanel.Refresh();
            }
        }
    }

    public void PlayLocalDuel()
    {
        Debug.Log("PlayLocalDuel");
        
        GameManager.Instance.StartLocalGame();
        AudioManager.Instance.PlayAudio(SoundID.ClickUI);
    }

    public void PlayRemoteDuel()
    {
        Debug.Log("PlayRemoteDuel");

        ShowAIs(false);
        ShowWaitingForPlayer(true);

        GameManager.Instance.StartLookingForOpponent();
        AudioManager.Instance.PlayAudio(SoundID.ClickUI);
    }

    public void PlayVSAI()
    {
        Debug.Log("PlayVSAI");

        if (GameManager.Instance.GameState == GameState.LookingForPlayer)
            GameManager.Instance.StopLookingForOpponent();

        ShowWaitingForPlayer(false);
        ShowAIs(true);

        AudioManager.Instance.PlayAudio(SoundID.ClickUI);
    }
}
