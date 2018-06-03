using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGamePanel : MonoBehaviour
{
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
    }

    public void DisplayLooseScreen(bool isShown)
    {
        gameObject.SetActive(isShown);
        youLost.gameObject.SetActive(isShown);
        points.gameObject.SetActive(isShown);
        playAgain.gameObject.SetActive(isShown);
    }

    public void DisplayDrawScreen(bool isShown)
    {
        gameObject.SetActive(isShown);
        draw.gameObject.SetActive(isShown);
        points.gameObject.SetActive(isShown);
        playAgain.gameObject.SetActive(isShown);
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
