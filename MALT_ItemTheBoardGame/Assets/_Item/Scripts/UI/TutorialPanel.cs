using AppAdvisory.Item;
using HedgehogTeam.EasyTouch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private GridManager gridManager;

    [SerializeField]
    private UIManager uiManager;

    [SerializeField]
    private Animator askForTuto;

    [SerializeField]
    private Animator tutoScreen1;

    [SerializeField]
    private Animator tutoScreen2;

    [SerializeField]
    private Animator phase1MovementsScreen;

    [SerializeField]
    private Animator phase2MovementsScreen;

    private bool toClose;
    
    public void HideAll()
    {
        animator.gameObject.SetActive(false);
        askForTuto.gameObject.SetActive(false);

        tutoScreen1.gameObject.SetActive(false);
        tutoScreen2.gameObject.SetActive(false);

        phase1MovementsScreen.gameObject.SetActive(false);
        phase2MovementsScreen.gameObject.SetActive(false);
    }

    /*
     *  ASK FOR TUTO
     */

    public void PopAskForTuto(bool isShown)
    {
        if (isShown)
        {
            animator.SetTrigger("PopIn");
            askForTuto.gameObject.SetActive(isShown);
            askForTuto.SetTrigger("PopIn");
        }
        else
        {
            askForTuto.SetTrigger("PopOut");
        }
    }

    public void DisableAskForTuto()
    {
        askForTuto.gameObject.SetActive(false);
    }

    public void OnAskForTutoButtonPress(bool value)
    {
        if (value)
        {
            PopAskForTuto(false);
            PopTutoScreen1(true);
            gridManager.SetPlayingTuto(true);
        }
        else
        {
            animator.SetTrigger("PopOut");
            gridManager.StartLookingForGame();
            PopAskForTuto(false);
        }
    }

    /*
     *  TUTO SCREEN 1
     */

    public void PopTutoScreen1(bool isShown)
    {
        if (isShown)
        {
            tutoScreen1.gameObject.SetActive(isShown);
            tutoScreen1.SetTrigger("PopIn");
        }
        else
        {
            tutoScreen1.SetTrigger("PopOut");
        }
    }

    public void DisableTutoScreen1()
    {
        tutoScreen1.gameObject.SetActive(false);
    }

    public void OnTutoScreen1NextButton()
    {
        PopTutoScreen1(false);
        PopTutoScreen2(true);
    }

    /*
     *  TUTO SCREEN 2
     */

    public void PopTutoScreen2(bool isShown)
    {
        if (isShown)
        {
            tutoScreen2.gameObject.SetActive(isShown);
            tutoScreen2.SetTrigger("PopIn");
        }
        else
        {
            tutoScreen2.SetTrigger("PopOut");
        }
    }

    public void DisableTutoScreen2()
    {
        tutoScreen2.gameObject.SetActive(false);
    }

    public void OnTutoScreen2NextButton()
    {
        PopTutoScreen2(false);
        PopPhase1MoveScreen(true);
    }

    /*
     *  TUTO MOVE 1 SCREEN
     */

    public void PopPhase1MoveScreen(bool isShown)
    {
        if (isShown)
        {
            phase1MovementsScreen.gameObject.SetActive(isShown);
            phase1MovementsScreen.SetTrigger("PopIn");
        }
        else
        {
            phase1MovementsScreen.SetTrigger("PopOut");
        }
    }

    public void DisablePhase1MoveScreen()
    {
        phase1MovementsScreen.gameObject.SetActive(false);
    }

    public void OnPhase1MoveScreenNextButton()
    {
        PopPhase1MoveScreen(false);
        animator.SetTrigger("PopOut");
        gridManager.StartGameVSIA();
        uiManager.turnSwitchPanel.SetCallbackAnimationEnd(uiManager.Phase1Tuto_ShowBall);
    }

    /*
     *  TUTO MOVE 2 SCREEN
     */

    public void PopPhase2MoveScreen(bool isShown)
    {
        if (isShown)
        {
            animator.gameObject.SetActive(true);
            animator.SetTrigger("PopIn");
            phase2MovementsScreen.gameObject.SetActive(true);
            phase2MovementsScreen.SetTrigger("PopIn");
        }
        else
        {
            phase2MovementsScreen.SetTrigger("PopOut");
        }
    }

    public void DisablePhase2MoveScreen()
    {
        phase2MovementsScreen.gameObject.SetActive(false);
    }

    public void OnPhase2MoveScreenNextButton()
    {
        PopPhase2MoveScreen(false);
        uiManager.SetPlayer1Turn(gridManager.player.StartTurn);
        animator.SetTrigger("PopOut");
    }
}
