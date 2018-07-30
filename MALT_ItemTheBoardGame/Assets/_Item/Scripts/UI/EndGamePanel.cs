using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGamePanel : UIPanel
{
    [Header("FR Settings")]
    [SerializeField]
    private string youWonTextFR;

    [SerializeField]
    private string youLostTextFR;

    [SerializeField]
    private string drawTextFR;

    [SerializeField]
    private string byForfeitTextFR;

    [SerializeField]
    private string playAgainTextFR;

    [Header("EN Settings")]
    [SerializeField]
    private string youWonTextEN;

    [SerializeField]
    private string youLostTextEN;

    [SerializeField]
    private string drawTextEN;

    [SerializeField]
    private string byForfeitTextEN;

    [SerializeField]
    private string playAgainTextEN;

    [Header("Localized Objects")]
    [SerializeField]
    private TextMeshProUGUI youWonText;

    [SerializeField]
    private TextMeshProUGUI youLostText;

    [SerializeField]
    private TextMeshProUGUI drawText;

    [SerializeField]
    private TextMeshProUGUI byForfeitText;

    [SerializeField]
    private TextMeshProUGUI playAgainText;

    [Header("")]
    [SerializeField]
    private RectTransform youWon;

    [SerializeField]
    private RectTransform youLost;

    [SerializeField]
    private RectTransform draw;

    [SerializeField]
    private RectTransform byForfeit;

    [SerializeField]
    private Button playAgain;

    [SerializeField]
    private RectTransform points;

    [SerializeField]
    private TextMeshProUGUI pointsText;

    private AudioManager audioManager;

    protected override void SetLanguageEN()
    {
        youWonText.text = youWonTextEN;
        youLostText.text = youLostTextEN;
        drawText.text = drawTextEN;
        byForfeitText.text = byForfeitTextEN;
        playAgainText.text = playAgainTextEN;
    }

    protected override void SetLanguageFR()
    {
        youWonText.text = youWonTextFR;
        youLostText.text = youLostTextFR;
        drawText.text = drawTextFR;
        byForfeitText.text = byForfeitTextFR;
        playAgainText.text = playAgainTextFR;
    }

    public void HideAll()
    {
        gameObject.SetActive(false);
        youWon.gameObject.SetActive(false);
        youLost.gameObject.SetActive(false);
        draw.gameObject.SetActive(false);
        byForfeit.gameObject.SetActive(false);
        points.gameObject.SetActive(false);
    }

    public void DisplayWonScreen(bool isShown)
    {
        gameObject.SetActive(isShown);
        youWon.gameObject.SetActive(isShown);
        points.gameObject.SetActive(isShown);
        playAgain.gameObject.SetActive(isShown);

        if (audioManager == null)
        {
            audioManager = FindObjectOfType<AudioManager>();
        }

        audioManager.PlayJingleAboveMusic(SoundID.JingleWin);
    }

    public void DisplayLooseScreen(bool isShown)
    {
        gameObject.SetActive(isShown);
        youLost.gameObject.SetActive(isShown);
        points.gameObject.SetActive(isShown);
        playAgain.gameObject.SetActive(isShown);

        if (audioManager == null)
        {
            audioManager = FindObjectOfType<AudioManager>();
        }

        audioManager.PlayJingleAboveMusic(SoundID.JingleLoose);
    }

    public void DisplayDrawScreen(bool isShown)
    {
        gameObject.SetActive(isShown);
        draw.gameObject.SetActive(isShown);
        points.gameObject.SetActive(isShown);
        playAgain.gameObject.SetActive(isShown);

        if (audioManager == null)
        {
            audioManager = FindObjectOfType<AudioManager>();
        }

        audioManager.PlayJingleAboveMusic(SoundID.JingleDraw);
    }

    public void DisplayWonByForfeit(bool isShown)
    {
        gameObject.SetActive(isShown);
        youWon.gameObject.SetActive(isShown);
        points.gameObject.SetActive(false);
        byForfeit.gameObject.SetActive(isShown);
        playAgain.gameObject.SetActive(isShown);
    }

    public void DisplayResult(int isWon)
    {
        if (isWon == 1)
        {
            DisplayWonScreen(true);
        }
        else if (isWon == -1)
        {
            DisplayLooseScreen(true);
        }
        else
        {
            DisplayDrawScreen(true);
        }
    }

    public void SetScore(int yourPoints, int theirPoints)
    {
        pointsText.text = yourPoints + " - " + theirPoints;
    }
}
