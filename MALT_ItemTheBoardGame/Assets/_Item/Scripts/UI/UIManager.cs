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
    private CanvasGroup menuCanvas;
    [SerializeField]
    private GameObject boardContainer;

    [SerializeField]
    private float timeBeforeNextTurn;

    [SerializeField]
    private SpriteRenderer boardOverlay;

	public PlayerPanel player1;
    public PlayerPanel player2;

    public MainMenu mainMenuPanel;
    public WaitingForPlayerPanel waitingForPlayerPanel;
    public RoundPanel roundResultPanel;
    public EndGamePanel endGamePanel;
    public TurnSwitchPanel turnSwitchPanel;
    public OptionsPanel optionsPanel;
    public HelpPanel helpPanel;
    public TutorialPanel tutoPanel;
    public InviteFriendPanel inviteFriendButton;
    public RectTransform overlayPanel;
    
    public GameObject arrowPrefab;
    private Transform arrowFocus;

    public bool isPlayer1Turn;

    public Ball Phase1Tuto_BallToMove;

    public Cell Phase1Tuto_CellToMoveTo;

    [SerializeField]
    private GameObject scoreParticle;

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
        endGamePanel.HideAll();
    }

    public void ShowMenuCanvas(bool showed)
    {
        menuCanvas.gameObject.SetActive(showed);
    }

    public void ShowGameplayCanvas(bool showed)
    {
        gameplayCanvas.gameObject.SetActive(showed);
    }

    public void Init()
    {
        DisplayWaitingForPlayerPanel(false);
        menuCanvas.gameObject.SetActive(false);
        gameplayCanvas.gameObject.SetActive(true);

        player1.SetScoreCounter(0);
        player2.SetScoreCounter(0);

        optionsPanel.OnLanguageChange += LanguageChanged;

        endGamePanel.HideAll();
        roundResultPanel.HideAll();
        turnSwitchPanel.HideAll();
        tutoPanel.HideAll();

        if (!FB.IsLoggedIn)
        {
            inviteFriendButton.gameObject.SetActive(false);
        }

        LanguageChanged();
    }

    public void LanguageChanged()
    {
        string language = Options.GetLanguage();
        optionsPanel.SetLanguage(language);
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

        //overlayPanel.SetLanguage(language);
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
        waitingForPlayerPanel.gameObject.SetActive(isShown);
    }
	
	public void DisplayPlayer1(bool isShown)
    {
        player1.gameObject.SetActive(isShown);
    }

	public void DisplayPlayer2(bool isShown)
    {
        player2.gameObject.SetActive(isShown);
    }

    public void DisplayRoundResultPanel(bool isShown, int roundNumber, int yourPoints, int theirPoints)
    {
        int isWon = 0;
        if (yourPoints > theirPoints)
        {
            isWon = 1;
        }
        else if (theirPoints > yourPoints)
        {
            isWon = -1;
        }
        
        player1.StopPortraitAnimation();
        player2.StopPortraitAnimation();

        roundResultPanel.DisplayRoundResult(isShown);
        roundResultPanel.SetScore(yourPoints, theirPoints);
        roundResultPanel.SetRoundNumber(roundNumber);
    }

    public void DisplayYouWon(bool isShown, int yourPoints, int theirPoints) {
        endGamePanel.DisplayWonScreen(isShown);
        endGamePanel.SetScore(yourPoints, theirPoints);
    }

    public void DisplayYouLost(bool isShown, int yourPoints, int theirPoints) {
        endGamePanel.DisplayLooseScreen(isShown);
        endGamePanel.SetScore(yourPoints, theirPoints);
    }

    public void DisplayDraw(bool isShown, int yourPoints, int theirPoints)
    {
        endGamePanel.DisplayDrawScreen(isShown);
        endGamePanel.SetScore(yourPoints, theirPoints);
    }

    public void DisplayForfeit(bool isShown)
    {
        endGamePanel.DisplayWonByForfeit(isShown);
    }

    public void DisplayInviteFriendButton(bool isShown)
    {
        inviteFriendButton.gameObject.SetActive(isShown);
    }

    public void InitPlayer1(BallColor color)
    {
        DisplayPlayer1(true);
        player1.SetColor(color);
    }

    public void InitPlayer2(BallColor color)
    {
        DisplayPlayer2(true);
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

    /*
    public void DisplayYourTurn(bool isShown)
    {
        turnSwitchPanel.SetYourTurn(isShown);
    }

    public void DisplayOpponentTurn(bool isShown)
    {
        turnSwitchPanel.SetOpponentsTurn(isShown);
    }
    */

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
            roundResultPanel.HideAll();
        }
    }

    public void OnEndGameButton()
    {
        AudioManager.Instance.PlayAudio(SoundID.ClickUI);

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

        if (optionsPanel.IsFadingIn)
        {
            optionsPanel.PopOut();
        }
        else
        {
            optionsPanel.PopIn();
        }
    }

    public void OnGoToGameResults()
    {
        AudioManager.Instance.PlayAudio(SoundID.ClickUI);

        int isWon = EvaluateWin(PlayerManager.Instance.Player1.totalScore, PlayerManager.Instance.Player2.totalScore);

        endGamePanel.DisplayResult(isWon);
        endGamePanel.SetScore(PlayerManager.Instance.Player1.totalScore, PlayerManager.Instance.Player2.totalScore);
        roundResultPanel.HideAll();
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

    /*public void Phase1Tuto_ShowBall()
    {
        overlayPanel.gameObject.SetActive(true);
        Phase1Tuto_BallToMove.PassAboveUI(true);

        EasyTouch.SetEnableUIDetection(false);
        PlayerManager.Instance.Player1.SetExclusivePickableObject(Phase1Tuto_BallToMove.gameObject);
        PlayerManager.Instance.Player1.OnBallSelection += pickBallHack;

        EasyTouch.On_TouchUp += PlayerManager.Instance.Player1.OnTouchUpPublic;
        arrowFocus = Instantiate(arrowPrefab).transform;
        arrowFocus.position = Phase1Tuto_BallToMove.transform.position + new Vector3(0, 0.5f, 0);
    }

    private void pickBallHack(Ball ball)
    {
        Phase1Tuto_BallSelected(Phase1Tuto_BallToMove);
        PlayerManager.Instance.Player1.OnBallSelection -= pickBallHack;
    }
    
    public void Phase1Tuto_BallSelected(Ball ball)
    {
        Phase1Tuto_CellToMoveTo.PassAboveUI(true);

        PlayerManager.Instance.Player1.SetExclusivePickableObject(Phase1Tuto_CellToMoveTo.gameObject);
        PlayerManager.Instance.Player1.OnCellSelection += pickCellHack;

        arrowFocus.DOMove(Phase1Tuto_CellToMoveTo.transform.position + new Vector3(0, 1, 0), 1.0f);
    }

    private void pickCellHack(Cell cell)
    {
        Phase1Tuto_BackToNormal(Phase1Tuto_CellToMoveTo);
        BackToNormal();
        PlayerManager.Instance.Player1.OnCellSelection -= pickCellHack;

        EasyTouch.On_TouchUp -= PlayerManager.Instance.Player1.OnTouchUpPublic;
    }

    private void BackToNormal()
    {
        Phase1Tuto_BallToMove.PassAboveUI(false);
        Phase1Tuto_CellToMoveTo.PassAboveUI(false);
    }

    public void Phase1Tuto_BackToNormal(Cell cell)
    {
        // fade out overlay panel
        Image overlayImg = overlayPanel.GetComponent<Image>();
        Color overlayColor = overlayImg.color;
        overlayColor.a = 0f;
        overlayImg.DOColor(overlayColor, 1.0f).OnComplete(() => {
            overlayPanel.gameObject.SetActive(false);
        });
        
        // fade out arrow
        SpriteRenderer spriteRenderer = arrowFocus.GetComponentInChildren<SpriteRenderer>();
        Color newcol = spriteRenderer.color;
        newcol.a = 0f;
        spriteRenderer.DOColor(newcol, 1.0f).OnComplete(() => { arrowFocus.gameObject.SetActive(false); });

        // setup input detection back to normal
        PlayerManager.Instance.Player1.SetExclusivePickableObject(null);
        EasyTouch.SetEnableUIDetection(true);
    }*/
}
