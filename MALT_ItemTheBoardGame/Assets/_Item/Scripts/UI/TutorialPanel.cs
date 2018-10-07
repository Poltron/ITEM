using AppAdvisory.Item;
using HedgehogTeam.EasyTouch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialPanel : UIPanel
{
    [Header("FR Settings")]
    // ask for tuto
    [SerializeField]
    private string askForTutoTextFR;
    [SerializeField]
    private string yesButtonLabelFR;
    [SerializeField]
    private string noButtonLabelFR;

    // tuto screen 1
    [SerializeField]
    private string tutoScreen1TitleFR;
    [SerializeField]
    [TextArea]
    private string tutoScreen1TextFR;

    // tuto screen 2
    [SerializeField]
    private string tutoScreen2TitleFR;
    [SerializeField]
    private string tutoScreen2Text1FR;
    [SerializeField]
    private string tutoScreen2TextLigneFR;
    [SerializeField]
    private string tutoScreen2TextOuFR;
    [SerializeField]
    private string tutoScreen2TextCroixFR;
    [SerializeField]
    private string tutoScreen2Text2FR;

    // phase1 movement help
    [SerializeField]
    private string phase1MovementHelpFR;
    [SerializeField]
    private string phase1MovementHelpTitleFR;
    [SerializeField]
    [TextArea]
    private string phase1MovementHelpTextFR;

    // phase2 movement help
    [SerializeField]
    private string phase2MovementHelpFR;
    [SerializeField]
    private string phase2MovementHelpTitleFR;
    [SerializeField]
    [TextArea]
    private string phase2MovementHelpTextFR;

    [Header("EN Settings")]
    // ask for tuto
    [SerializeField]
    private string askForTutoTextEN;
    [SerializeField]
    private string yesButtonLabelEN;
    [SerializeField]
    private string noButtonLabelEN;

    // tuto screen 1
    [SerializeField]
    private string tutoScreen1TitleEN;
    [SerializeField]
    [TextArea]
    private string tutoScreen1TextEN;

    // tuto screen 2
    [SerializeField]
    private string tutoScreen2TitleEN;
    [SerializeField]
    private string tutoScreen2Text1EN;
    [SerializeField]
    private string tutoScreen2TextLigneEN;
    [SerializeField]
    private string tutoScreen2TextOuEN;
    [SerializeField]
    private string tutoScreen2TextCroixEN;
    [SerializeField]
    private string tutoScreen2Text2EN;

    // phase1 movement help
    [SerializeField]
    private string phase1MovementHelpEN;
    [SerializeField]
    private string phase1MovementHelpTitleEN;
    [SerializeField]
    [TextArea]
    private string phase1MovementHelpTextEN;

    // phase2 movement help
    [SerializeField]
    private string phase2MovementHelpEN;
    [SerializeField]
    private string phase2MovementHelpTitleEN;
    [SerializeField]
    [TextArea]
    private string phase2MovementHelpTextEN;

    [Header("Localized Objects")]
    // ask for tuto
    [SerializeField]
    private TextMeshProUGUI askForTutoText;
    [SerializeField]
    private TextMeshProUGUI yesButtonLabel;
    [SerializeField]
    private TextMeshProUGUI noButtonLabel;

    // tuto screen 1
    [SerializeField]
    private TextMeshProUGUI tutoScreen1Title;
    [SerializeField]
    private TextMeshProUGUI tutoScreen1Text;

    // tuto screen 2
    [SerializeField]
    private TextMeshProUGUI tutoScreen2Title;
    [SerializeField]
    private TextMeshProUGUI tutoScreen2Text1;
    [SerializeField]
    private TextMeshProUGUI tutoScreen2TextLigne;
    [SerializeField]
    private TextMeshProUGUI tutoScreen2TextOu;
    [SerializeField]
    private TextMeshProUGUI tutoScreen2TextCroix;
    [SerializeField]
    private TextMeshProUGUI tutoScreen2Text2;

    // phase1 movement help
    [SerializeField]
    private TextMeshProUGUI phase1MovementHelpPhase1Text;
    [SerializeField]
    private TextMeshProUGUI phase1MovementHelpTitle;
    [SerializeField]
    private TextMeshProUGUI phase1MovementHelpText;

    // phase2 movement help
    [SerializeField]
    private TextMeshProUGUI phase2MovementHelpPhase2Text;
    [SerializeField]
    private TextMeshProUGUI phase2MovementHelpTitle;
    [SerializeField]
    private TextMeshProUGUI phase2MovementHelpText;

    [Header("")]
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

    private AudioManager audioManager;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void HideAll()
    {
        animator.gameObject.SetActive(false);
        askForTuto.gameObject.SetActive(false);

        tutoScreen1.gameObject.SetActive(false);
        tutoScreen2.gameObject.SetActive(false);

        phase1MovementsScreen.gameObject.SetActive(false);
        phase2MovementsScreen.gameObject.SetActive(false);
    }

    protected override void SetLanguageEN()
    {
        askForTutoText.text = askForTutoTextEN;
        yesButtonLabel.text = yesButtonLabelEN;
        noButtonLabel.text = noButtonLabelEN;
        
        tutoScreen1Title.text = tutoScreen1TitleEN;
        tutoScreen1Text.text = tutoScreen1TextEN;
        
        tutoScreen2Title.text = tutoScreen2TitleEN;
        tutoScreen2Text1.text = tutoScreen2Text1EN;
        tutoScreen2TextCroix.text = tutoScreen2TextCroixEN;
        tutoScreen2TextOu.text = tutoScreen2TextOuEN;
        tutoScreen2TextLigne.text = tutoScreen2TextLigneEN;
        tutoScreen2Text2.text = tutoScreen2Text2EN;

        phase1MovementHelpPhase1Text.text = phase1MovementHelpEN;
        phase1MovementHelpTitle.text = phase1MovementHelpTitleEN;
        phase1MovementHelpText.text = phase1MovementHelpTextEN;
        
        phase2MovementHelpPhase2Text.text = phase2MovementHelpEN;
        phase2MovementHelpTitle.text = phase2MovementHelpTitleEN;
        phase2MovementHelpText.text = phase2MovementHelpTextEN;
    }

    protected override void SetLanguageFR()
    {
        askForTutoText.text = askForTutoTextFR;
        yesButtonLabel.text = yesButtonLabelFR;
        noButtonLabel.text = noButtonLabelFR;

        tutoScreen1Title.text = tutoScreen1TitleFR;
        tutoScreen1Text.text = tutoScreen1TextFR;

        tutoScreen2Title.text = tutoScreen2TitleFR;
        tutoScreen2Text1.text = tutoScreen2Text1FR;
        tutoScreen2TextCroix.text = tutoScreen2TextCroixFR;
        tutoScreen2TextOu.text = tutoScreen2TextOuFR;
        tutoScreen2TextLigne.text = tutoScreen2TextLigneFR;
        tutoScreen2Text2.text = tutoScreen2Text2FR;

        phase1MovementHelpPhase1Text.text = phase1MovementHelpFR;
        phase1MovementHelpTitle.text = phase1MovementHelpTitleFR;
        phase1MovementHelpText.text = phase1MovementHelpTextFR;

        phase2MovementHelpPhase2Text.text = phase2MovementHelpFR;
        phase2MovementHelpTitle.text = phase2MovementHelpTitleFR;
        phase2MovementHelpText.text = phase2MovementHelpTextFR;
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

            if (audioManager == null)
            {
                audioManager = FindObjectOfType<AudioManager>();
            }

            audioManager.PlayAudio(SoundID.OpenWindowTuto);
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
            uiManager.DisplayWaitingForPlayerPanel(true);
        }

        if (audioManager != null)
        {
            audioManager.PlayAudio(SoundID.CloseWindowTuto);
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

            if (audioManager != null)
            {
                audioManager.PlayAudio(SoundID.CloseWindowTuto);
            }
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

            if (audioManager != null)
            {
                audioManager.PlayAudio(SoundID.CloseWindowTuto);
            }
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

            if (audioManager != null)
            {
                audioManager.PlayAudio(SoundID.CloseWindowTuto);
            }
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
        uiManager.Phase1Tuto_ShowBall();
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
            audioManager.PlayAudio(SoundID.OpenWindowTuto);
        }
        else
        {
            phase2MovementsScreen.SetTrigger("PopOut");

            if (audioManager != null)
            {
                audioManager.PlayAudio(SoundID.CloseWindowTuto);
            }
        }
    }

    public void DisablePhase2MoveScreen()
    {
        phase2MovementsScreen.gameObject.SetActive(false);
    }

    public void OnPhase2MoveScreenNextButton()
    {
        PopPhase2MoveScreen(false);
        uiManager.SetPlayer1Turn();
        gridManager.player.StartTurn();
        animator.SetTrigger("PopOut");
    }
}
