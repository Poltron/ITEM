using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FBPanel : UIPanel
{
    [Header("FR Settings")]
    [SerializeField]
    private string inviteFriendLabelFR;
    [SerializeField]
    private string fbConnectFR;

    [Header("EN Settings")]
    [SerializeField]
    private string inviteFriendLabelEN;
    [SerializeField]
    private string fbConnectEN;

    [Header("Localized Objects")]
    [SerializeField]
    private Text inviteFriendText;
    [SerializeField]
    private Text fbConnectText;

    [Header("")]
    [SerializeField]
    private Animator inviteFriendAnimator;
    [SerializeField]
    private Animator fbConnectAnimator;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void SetLanguageEN()
    {
        inviteFriendText.text = inviteFriendLabelEN;
        fbConnectText.text = fbConnectEN;
    }

    protected override void SetLanguageFR()
    {
        inviteFriendText.text = inviteFriendLabelFR;
        fbConnectText.text = fbConnectFR;
    }

    public void Show(bool showed)
    {
        if (!FB.IsInitialized)
            return;

        if (FB.IsLoggedIn)
            ShowInviteFriend(showed);
        else
            ShowFBConnect(showed);
    }

    public void ShowFBConnect(bool showed)
    {
        if (!FB.IsInitialized)
            return;

        fbConnectAnimator.SetBool(animatorHashPopIn, showed);
    }

    public void ShowInviteFriend(bool showed)
    {
        if (!FB.IsInitialized)
            return;

        inviteFriendAnimator.SetBool(animatorHashPopIn, showed);
    }
}
