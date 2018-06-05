using AppAdvisory.Item;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField]
    private GridManager gridManager;

    [SerializeField]
    private UIManager uiManager;

    [SerializeField]
    private RectTransform panel;

    [SerializeField]
    private RectTransform askForTuto;

    [SerializeField]
    private RectTransform tutoScreen1;

    [SerializeField]
    private RectTransform tutoScreen2;

    [SerializeField]
    private RectTransform phase1MovementsScreen;

    [SerializeField]
    private RectTransform phase2MovementsScreen;

    public void HideAll()
    {
        panel.gameObject.SetActive(false);

        askForTuto.gameObject.SetActive(false);

        tutoScreen1.gameObject.SetActive(false);
        tutoScreen2.gameObject.SetActive(false);

        phase1MovementsScreen.gameObject.SetActive(false);
        phase2MovementsScreen.gameObject.SetActive(false);
    }

    public void DisplayAskForTuto(bool isShown)
    {
        panel.gameObject.SetActive(isShown);
        askForTuto.gameObject.SetActive(isShown);
    }

    public void OnAskForTutoButtonPress(bool value)
    {
        if (value)
        {
            DisplayAskForTuto(false);
            DisplayTutoScreen1(true);
            gridManager.SetPlayingTuto(true);
        }
        else
        {
            gridManager.StartLookingForGame();
            DisplayAskForTuto(false);
        }
    }

    public void DisplayTutoScreen1(bool isShown)
    {
        panel.gameObject.SetActive(isShown);
        tutoScreen1.gameObject.SetActive(isShown);
    }

    public void OnTutoScreen1NextButton()
    {
        DisplayTutoScreen1(false);
        DisplayTutoScreen2(true);
    }

    public void DisplayTutoScreen2(bool isShown)
    {
        panel.gameObject.SetActive(isShown);
        tutoScreen2.gameObject.SetActive(isShown);
    }

    public void OnTutoScreen2NextButton()
    {
        DisplayTutoScreen2(false);
        DisplayPhase1MovementsScreen(true);
    }

    public void DisplayPhase1MovementsScreen(bool isShown)
    {
        panel.gameObject.SetActive(isShown);
        phase1MovementsScreen.gameObject.SetActive(isShown);
    }

    public void OnPhase1MovementsScreenNextButton()
    {
        DisplayPhase1MovementsScreen(false);
        gridManager.StartGameVSIA();
        uiManager.turnSwitchPanel.SetCallbackAnimationEnd(uiManager.Phase1Tuto_ShowBall);
    }

    public void DisplayPhase2MovementsScreen(bool isShown)
    {
        panel.gameObject.SetActive(isShown);
        phase2MovementsScreen.gameObject.SetActive(isShown);
    }

    public void OnPhase2MovementsScreenNextButton()
    {
        DisplayPhase2MovementsScreen(false);
        uiManager.SetPlayer1Turn(gridManager.player.StartTurn);
    }
}
