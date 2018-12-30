using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using HedgehogTeam.EasyTouch;
using DG.Tweening;
using GS;
using Facebook.Unity;

public class UIManager : MonoBehaviour
{
    static private UIManager instance;
    static public UIManager Instance { get { return instance; } }

    public event Action EndGame;
    public event Action NextRound;
    public event Action FinishTurn;
    public event Action InviteFriend;

    [SerializeField]
    private CanvasGroup gameplayCanvas;
    [SerializeField]
    private MainMenu menuUI;
    [SerializeField]
    private GameObject boardContainer;
    [SerializeField]
    private Animation boardContainerAnimation;

    [SerializeField]
    private SpriteRenderer boardOverlay;
    [SerializeField]
    private Animation boardOverlayAnimation;

	public PlayerPanel player1;
    [SerializeField]
    private Animator player1Animator;

    public PlayerPanel player2;
    [SerializeField]
    private Animator player2Animator;

    public MainMenu mainMenuPanel;
    public WaitingForPlayerPanel waitingForPlayerPanel;
    public RoundPanel roundResultPanel;
    public EndGamePanel endGamePanel;
    public ForfeitPanel forfeitPanel;
    public BackToMainMenuPanel backToMainMenuPanel;
    public OptionsPanel optionsPanel;
    public HelpPanel helpPanel;
    public TutorialPanel tutoPanel;
    public FBPanel fbPanel;
    public QuickMenuButtonPanel quickMenu;
    public Button backToMainMenuButton;

    public bool isPlayer1Turn;

    [SerializeField]
    private Material toAnimate;

    [SerializeField]
    private GameObject scoreParticle;

    [Header("Animation Timing")]
    [SerializeField]
    public float timeBeforeAskForTutoPop = 2f;
    [SerializeField]
    private float timeBeforePlayerPanelPopIn;
    [SerializeField]
    private float timeBeforeQuickMenuPopIn;
    [SerializeField]
    private float timeBeforeBoardPopIn;

    [SerializeField]
    private float godrayTimeBeforeAnimation;
    [SerializeField]
    private float godrayAnimationTime;

    private int animatorHashWin;
    private int animatorHashLoose;

    void Awake()
    {
        animatorHashWin = Animator.StringToHash("Win");
        animatorHashLoose = Animator.StringToHash("Loose");

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        optionsPanel.OnLanguageChange += LanguageChanged;
    }

    IEnumerator waitFor(float t, Action toDo)
    {
        yield return new WaitForSeconds(t);
        toDo();
    }

    public void ResetGame()
    {
        endGamePanel.Display(false);
        roundResultPanel.Display(false);
    }

    public void ShowMenuCanvas(bool showed)
    {
        if (showed)
        {
            menuUI.PopIn();
        }
        else
        {
            menuUI.PopOut();
        }

        fbPanel.Show(showed);
    }

    public void ShowGameplayCanvas(bool showed)
    {
        if (showed)
        {
            gameplayCanvas.alpha = 1;
            gameplayCanvas.interactable = true;
            gameplayCanvas.blocksRaycasts = true;
        }
        else
        {
            DOVirtual.DelayedCall(1.0f, () => {
                gameplayCanvas.alpha = 0;
                gameplayCanvas.interactable = false;
                gameplayCanvas.blocksRaycasts = false;
            });
        }

        DOVirtual.DelayedCall(timeBeforePlayerPanelPopIn, () => { player1.PopIn(showed); });
        DOVirtual.DelayedCall(timeBeforePlayerPanelPopIn, () => { player2.PopIn(showed); });
        DOVirtual.DelayedCall(timeBeforeQuickMenuPopIn, () => { DisplayQuickMenuButtons(showed); });
    }

    public void Init()
    {
        ShowMenuCanvas(false);
        ShowGameplayCanvas(true);

        player1.SetScoreCounter(0);
        player2.SetScoreCounter(0);

        optionsPanel.OnLanguageChange += LanguageChanged;

        tutoPanel.HideAll();

        backToMainMenuButton.enabled = false;
        LanguageChanged();
    }

    public void LanguageChanged()
    {
        string language = Options.GetLanguage();
        fbPanel.SetLanguage(language);
        optionsPanel.SetLanguage(language);
        backToMainMenuPanel.SetLanguage(language);
        waitingForPlayerPanel.SetLanguage(language);
        roundResultPanel.SetLanguage(language);
        endGamePanel.SetLanguage(language);
        helpPanel.SetLanguage(language);
        tutoPanel.SetLanguage(language);
        quickMenu.SetLanguage(language);
        mainMenuPanel.SetLanguage(language);
        player1.SetLanguage(language);
        player2.SetLanguage(language);

        PlayerManager.Instance.Player1.playerName = player1.playerName.text;
    }

    public void PopTuto()
    {
        tutoPanel.gameObject.SetActive(true);
        tutoPanel.PopAskForTuto(true);
    }
	
	public void DisplayPlayer1(bool isShown)
    {
        player1.gameObject.SetActive(isShown);
    }

	public void DisplayPlayer2(bool isShown)
    {
        player2.gameObject.SetActive(isShown);
    }

    public void DisplayQuickMenuButtons(bool enabled)
    {
        if (enabled)
        {
            quickMenu.PopIn();
        }
        else
        {
            quickMenu.PopOut();
        }
    }

    public void DisableBackToMainMenuButton(bool isShown)
    {
        backToMainMenuButton.enabled = isShown;
    }

    IEnumerator PlayGodrayAnimation()
    {
        toAnimate.SetFloat("_OverallOpacity", 0);
        yield return new WaitForSeconds(godrayTimeBeforeAnimation);

        for (float i = 0; i < godrayAnimationTime; i += Time.deltaTime)
        {
            toAnimate.SetFloat("_OverallOpacity", i);
            yield return new WaitForEndOfFrame();
        }
    }

    public void DisplayYouWon() {
        player1.SetWinText();
        player1Animator.SetTrigger(animatorHashWin);
        StartCoroutine(PlayGodrayAnimation());

        player2.SetLooseText();
        player2Animator.SetTrigger(animatorHashLoose);
    }

    public void DisplayYouLost() {
        player1.SetLooseText();
        player1Animator.SetTrigger(animatorHashLoose);

        player2.SetWinText();
        player2Animator.SetTrigger(animatorHashWin);
        StartCoroutine(PlayGodrayAnimation());
    }

    public void DisplayDraw()
    {
        player1.SetDrawText();
        player1Animator.SetTrigger(animatorHashLoose);

        player2.SetDrawText();
        player2Animator.SetTrigger(animatorHashLoose);
    }

    public void DisplayForfeit(bool isShown)
    {
        forfeitPanel.Display(isShown);
    }

    public void InitPlayer1(BallColor color)
    {
        player1.SetColor(color);
    }

    public void InitPlayer2(BallColor color)
    {
        player2.SetColor(color);
    }

    public void SetPlayer1Name(string name)
    {
        if (player1)
            player1.SetName(name);
    }

    public void SetPlayer2Name(string name)
    {
        if (player2)
            player2.SetName(name);
    }

    public void SetPlayer1Pic(Sprite sprite)
    {
        player1.SetPic(sprite);
    }

    public void SetPlayer2Pic(Sprite sprite)
    {
        player2.SetPic(sprite);
    }

    public void DisplayFBPanel(bool showed)
    {
        fbPanel.Show(false);
    }

    public void DisplayConnectFBButton(bool showed)
    {
        fbPanel.ShowFBConnect(showed);
    }

    public void DisplayInviteFriendButton(bool showed)
    {
        fbPanel.ShowInviteFriend(showed);
    }

    public void StopPortraitAnimation()
    {
        player1.StopPortraitAnimation();
        player2.StopPortraitAnimation();
    }

    public void StopPlayerTurns()
    {
        player1.StopPortraitAnimation();

        if (PlayerManager.Instance.Player1 != null)
            PlayerManager.Instance.Player1.EndTurn();

        player2.StopPortraitAnimation();

        if (PlayerManager.Instance.Player2 != null)
            PlayerManager.Instance.Player2.EndTurn();
    }

    public void SetPlayer1Turn()
    {
        isPlayer1Turn = true;
        AnimateNextTurn();
    }

    public void DisplayTutorialPhase2Movement()
    {
        tutoPanel.PopPhase2MoveScreen(true);
    }

    public void SetPlayer2Turn()
    {
        isPlayer1Turn = false;
        AnimateNextTurn();
    }
        
    public void PopScoreParticle(Ball ball)
    {
        GameObject g = GameObject.Instantiate(scoreParticle, ball.transform.position, Quaternion.identity);
        AudioManager.Instance.PlayAudio(SoundID.ParticleMove);

        if (isPlayer1Turn)
        {
            g.transform.DOMove(player1.textCounter.rectTransform.position, 0.5f).OnComplete(() =>
            {
                if (ball.Score != 0)
                {
                    AudioManager.Instance.PlayAudio(SoundID.ParticleOne);
                    player1.StartScoreAnim();
                    GridManager.Instance.AddPlayer1Score(ball.Score);
                }
                else
                {
                    AudioManager.Instance.PlayAudio(SoundID.ParticleZero);
                }

                Destroy(g);
            });
        }
        else
        {
            g.transform.DOMove(player2.textCounter.rectTransform.position, 0.5f).OnComplete(() =>
            {
                if (ball.Score != 0)
                {
                    AudioManager.Instance.PlayAudio(SoundID.ParticleOne);
                    player2.StartScoreAnim();
                    GridManager.Instance.AddPlayer2Score(ball.Score);
                }
                else
                {
                    AudioManager.Instance.PlayAudio(SoundID.ParticleZero);
                }

                Destroy(g);
            });
        }
    }

    public void AnimateNextTurn()
    {
        boardOverlayAnimation.Play();

        if (isPlayer1Turn)
        {
            player1.PlayPortraitAnimation();
            player2.StopPortraitAnimation();
        }
        else
        {
            player1.StopPortraitAnimation();
            player2.PlayPortraitAnimation();
        }
    }

    public void ResetPlayerTurn()
    {
        player1.SetColor(Color.white);
        player2.SetColor(Color.white);
    }



    public void OnNextRoundButton()
    {
        AudioManager.Instance.PlayAudio(SoundID.ClickUI);

        if (NextRound != null)
        {
            NextRound();
            roundResultPanel.Display(false);
        }
    }

    public void OnForfeitEndGameButton()
    {
        AudioManager.Instance.PlayAudio(SoundID.ClickUI);
        forfeitPanel.Display(false);

        if (EndGame != null)
            EndGame();
    }

    public void OnEndGameButton()
    {
        AudioManager.Instance.PlayAudio(SoundID.ClickUI);
        endGamePanel.Display(false);

        if (EndGame != null)
            EndGame();
    }

	public void OnInviteFriendbutton()
    {
        AudioManager.Instance.PlayAudio(SoundID.ClickUI);

	    if (InviteFriend != null)
	        InviteFriend ();
    }

    public void OnHelpButton()
    {
        AudioManager.Instance.PlayAudio(SoundID.ClickUI);

        optionsPanel.PopOut();
        backToMainMenuPanel.PopOut();

        if (helpPanel.IsShown)
            helpPanel.PopOut();
        else
            helpPanel.PopIn();
    }

    public void OnOptionsButton()
    {
        AudioManager.Instance.PlayAudio(SoundID.ClickUI);

        helpPanel.PopOut();
        backToMainMenuPanel.PopOut();

        if (optionsPanel.IsShown)
            optionsPanel.PopOut();
        else
            optionsPanel.PopIn();
    }

    public void OnBackToMainMenuButton()
    {
        AudioManager.Instance.PlayAudio(SoundID.ClickUI);

        helpPanel.PopOut();
        optionsPanel.PopOut();

        if (backToMainMenuPanel.IsShown)
            backToMainMenuPanel.PopOut();
        else
            backToMainMenuPanel.PopIn();
    }

    public void DisplayEndGame()
    {
        int isWon = EvaluateWin(PlayerManager.Instance.Player1.totalScore, PlayerManager.Instance.Player2.totalScore);

        if (isWon == 1)
            DisplayYouWon();
        else if (isWon == 0)
            DisplayDraw();
        else
            DisplayYouLost();
    }

    private int EvaluateWin(int playerPoints, int opponentPoints)
    {
        int isWon = 0;
        if (playerPoints > opponentPoints)
        {
            isWon = 1;
        }
        else if (opponentPoints > playerPoints)
        {
            isWon = -1;
        }

        return isWon;
    }
}
