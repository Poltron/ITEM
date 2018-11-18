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
    private SpriteRenderer boardOverlay;

	public PlayerPanel player1;
    public PlayerPanel player2;

    public MainMenu mainMenuPanel;
    public WaitingForPlayerPanel waitingForPlayerPanel;
    public RoundPanel roundResultPanel;
    public EndGamePanel endGamePanel;
    public TurnSwitchPanel turnSwitchPanel;
    public BackToMainMenuPanel backToMainMenuPanel;
    public OptionsPanel optionsPanel;
    public HelpPanel helpPanel;
    public TutorialPanel tutoPanel;
    public InviteFriendPanel inviteFriendButton;
    public GameObject fbConnectButton;
    public Animator fbPanel;
    public RectTransform overlayPanel;
    public Animator quickMenu;
    public Button backToMainMenuButton;
    public GameObject arrowPrefab;
    private Transform arrowFocus;

    public bool isPlayer1Turn;

    public Ball Phase1Tuto_BallToMove;

    public Cell Phase1Tuto_CellToMoveTo;

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

    void Awake()
    {
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

        DisplayInviteFriendButton(showed);

        //menuCanvas.gameObject.SetActive(showed);
    }

    public void ShowGameplayCanvas(bool showed)
    {
        DOVirtual.DelayedCall(timeBeforePlayerPanelPopIn, () => { player1.PopIn(showed); });
        DOVirtual.DelayedCall(timeBeforePlayerPanelPopIn, () => { player2.PopIn(showed); });
        DOVirtual.DelayedCall(timeBeforeQuickMenuPopIn, () => { quickMenu.SetBool("bPopIn", showed); });
        DOVirtual.DelayedCall(timeBeforeBoardPopIn, () => { boardContainer.GetComponent<Animator>().SetBool("bPopIn", showed); });
    }

    public void Init()
    {
        ShowMenuCanvas(false);
        ShowGameplayCanvas(true);

        player1.SetScoreCounter(0);
        player2.SetScoreCounter(0);

        optionsPanel.OnLanguageChange += LanguageChanged;

        turnSwitchPanel.HideAll();
        tutoPanel.HideAll();

        backToMainMenuButton.enabled = false;
        LanguageChanged();
    }

    public void LanguageChanged()
    {
        string language = Options.GetLanguage();
        optionsPanel.SetLanguage(language);
        backToMainMenuPanel.SetLanguage(language);
        inviteFriendButton.SetLanguage(language);
        waitingForPlayerPanel.SetLanguage(language);
        roundResultPanel.SetLanguage(language);
        endGamePanel.SetLanguage(language);
        turnSwitchPanel.SetLanguage(language);
        helpPanel.SetLanguage(language);
        tutoPanel.SetLanguage(language);
        player1.SetLanguage(language);
        player2.SetLanguage(language);

        PlayerManager.Instance.Player1.playerName = player1.playerName.text;
    }

    public void PopTuto()
    {
        tutoPanel.gameObject.SetActive(true);
        tutoPanel.PopAskForTuto(true);
    }

    public void DisplayTurnSwitchPhase1(bool isShown)
    {
        turnSwitchPanel.SetPhase1(isShown);
    }

    public void DisplayTurnSwitchPhase2(bool isShown)
    {
        turnSwitchPanel.SetPhase2(isShown);
    }

    public void DisplayWaitingForPlayerPanel(bool isShown)
    {
        waitingForPlayerPanel.GetComponent<Animator>().SetBool("bPopIn", isShown);
    }
	
	public void DisplayPlayer1(bool isShown)
    {
        player1.gameObject.SetActive(isShown);
    }

	public void DisplayPlayer2(bool isShown)
    {
        player2.gameObject.SetActive(isShown);
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
            Debug.Log(toAnimate.GetFloat("_OverallOpacity"));
            toAnimate.SetFloat("_OverallOpacity", i);
            yield return new WaitForEndOfFrame();
        }
    }

    public void DisplayYouWon() {
        player1.SetWinText();
        player1.GetComponent<Animator>().SetTrigger("Win");
        StartCoroutine(PlayGodrayAnimation());

        player2.SetLooseText();
        player2.GetComponent<Animator>().SetTrigger("Loose");
    }

    public void DisplayYouLost() {
        player1.SetLooseText();
        player1.GetComponent<Animator>().SetTrigger("Loose");

        player2.SetWinText();
        player2.GetComponent<Animator>().SetTrigger("Win");
        StartCoroutine(PlayGodrayAnimation());
    }

    public void DisplayDraw()
    {
        player1.SetDrawText();
        player1.GetComponent<Animator>().SetTrigger("Loose");

        player2.SetDrawText();
        player2.GetComponent<Animator>().SetTrigger("Loose");
    }

    public void DisplayForfeit(bool isShown)
    {
        //endGamePanel.DisplayWonByForfeit(isShown);
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

    public void DisplayConnectFBButton(bool showed)
    {
        fbPanel.SetBool("bPopIn", showed);
        fbPanel.SetBool("bIsFBConnected", FB.IsLoggedIn);
    }

    public void DisplayInviteFriendButton(bool showed)
    {
        fbPanel.SetBool("bPopIn", showed);
        fbPanel.SetBool("bIsFBConnected", FB.IsLoggedIn);
    }

    public void StopPortraitAnimation()
    {
        player1.StopPortraitAnimation();
        player2.StopPortraitAnimation();
    }

    public void StopPlayerTurns()
    {
        player1.StopPortraitAnimation();
        PlayerManager.Instance.Player1.EndTurn();

        player2.StopPortraitAnimation();
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
        boardOverlay.GetComponent<Animation>().Play();

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

        if (helpPanel.IsFadingIn)
        {
            helpPanel.PopOut();
        }
        else
        {
            helpPanel.PopIn();
        }
    }

    public void OnOptionsButton()
    {
        AudioManager.Instance.PlayAudio(SoundID.ClickUI);

        helpPanel.PopOut();
        backToMainMenuPanel.PopOut();

        if (optionsPanel.IsFadingIn)
        {
            optionsPanel.PopOut();
        }
        else
        {
            optionsPanel.PopIn();
        }
    }

    public void OnBackToMainMenuButton()
    {
        AudioManager.Instance.PlayAudio(SoundID.ClickUI);

        helpPanel.PopOut();
        optionsPanel.PopOut();

        if (backToMainMenuPanel.IsFadingIn)
        {
            backToMainMenuPanel.PopOut();
        }
        else
        {
            backToMainMenuPanel.PopIn();
        }
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

    private void DisplayOverlay(bool isShown)
    {
        overlayPanel.gameObject.SetActive(isShown);
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
