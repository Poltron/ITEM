using DG.Tweening; // DOTween & DOVirtual
using GS; // FacebookPro
using Photon; // Network

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : PunBehaviour
{
    static private GameManager instance;
    static public GameManager Instance { get { return instance; } }

    [SerializeField]
    private FBManager fbManager;
    private bool isFBConnected = false;

    private Connection connection;

    [SerializeField]
    private float timeToLaunchGameVSIA = 4;

    public bool isGameFinished = false;
    public bool isGameStarted = false;
    public bool isPlayingVSIA = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start ()
    {
        Options.Init();

        // create local player
        PlayerManager.Instance.CreatePlayer(BallColor.White, true);

        // connection to photon network
        connection = GetComponent<Connection>();
        connection.ApplyUserIdAndConnect();

        // load fb data
        if (fbManager)
        {
            isFBConnected = true;

            PlayerManager.Instance.Player1.playerName = fbManager.pName;
            PlayerManager.Instance.Player1.picURL = fbManager.pUrlPic;

            StartCoroutine(Utils.LoadSpriteFromURL(PlayerManager.Instance.Player1.picURL, (sprite) =>
            {
                UIManager.Instance.SetPlayer1Pic(sprite);
            }));

            fbManager.NameLoaded += OnNameLoaded;
            fbManager.PicURLLoaded += OnPicURLLoaded;

            UIManager.Instance.SetPlayer1Name(PlayerManager.Instance.Player1.playerName);
        }

        // init UI
        UIManager.Instance.Init();

        // start with tutorial or not
        if (Options.GetAskForTuto())
            UIManager.Instance.PopTuto();
        else
            StartLookingForGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGameVSIA();
        }
    }

    #region FACEBOOK
    private void OnFacebookConnect()
    {
        connection.ApplyUserIdAndConnect();
    }

    private void OnNameLoaded(string name)
    {
        PlayerManager.Instance.Player1.playerName = name;
        UIManager.Instance.DisplayPlayer1(true);
        print("onnameloaded " + name);
    }

    private void OnPicURLLoaded(string url)
    {
        PlayerManager.Instance.Player1.picURL = url;

        StartCoroutine(Utils.LoadSpriteFromURL(PlayerManager.Instance.Player1.picURL, (sprite) => {
            UIManager.Instance.SetPlayer1Pic(sprite);
        }));

        UIManager.Instance.DisplayPlayer1(true);

        print("onpicurlloaded" + url);
    }

    private void SendName(string name)
    {
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("ReceiveName", PhotonTargets.Others, name);
    }

    private void SendPicURL(string picURL)
    {
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("ReceivePicURL", PhotonTargets.Others, picURL);
    }

    [PunRPC]
    private void ReceiveName(string name)
    {
        UIManager.Instance.SetPlayer2Name(name);
        return;
    }

    [PunRPC]
    private void ReceivePicURL(string picURL)
    {
        StartCoroutine(Utils.LoadSpriteFromURL(picURL, (sprite) => {
            UIManager.Instance.SetPlayer2Pic(sprite);
        }));
    }
    #endregion FACEBOOK

    public void StartLookingForGame()
    {
        PhotonNetwork.JoinRandomRoom();
        DOVirtual.DelayedCall(timeToLaunchGameVSIA, StartGameVSIA, true);
    }

    public void StartGameVSIA()
    {
        if (isGameStarted)
            return;

        connection.enabled = false;
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();

        UIManager.Instance.DisplayWaitingForPlayerPanel(false);
        GridManager.Instance.InitForGameStart();
        isGameStarted = true;
        isPlayingVSIA = true;

        PlayerManager.Instance.SetPlayerColor(BallColor.White, true);
        PlayerManager.Instance.CreatePlayer(BallColor.Black);
        UIManager.Instance.InitPlayer2(BallColor.Black);

        UIManager.Instance.SetPlayer2Name(PlayerManager.Instance.GetIAName());
        UIManager.Instance.SetPlayer2Pic(PlayerManager.Instance.GetIASprite());

        UIManager.Instance.SetPlayer1Turn();
        PlayerManager.Instance.Player1.StartTurn();
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.room.PlayerCount == 2)
        {
            UIManager.Instance.DisplayWaitingForPlayerPanel(false);
            GridManager.Instance.InitForGameStart();
            isGameStarted = true;

            PlayerManager.Instance.SetPlayerColor(BallColor.Black, true);
            PlayerManager.Instance.CreatePlayer(BallColor.White);

            UIManager.Instance.SetPlayer2Turn();
            UIManager.Instance.SetPlayer1Name(PlayerManager.Instance.Player1.playerName);

            print("2 players in the room, sending player name : " + PlayerManager.Instance.Player1.playerName);
            SendName(PlayerManager.Instance.Player1.playerName);
            SendPicURL(PlayerManager.Instance.Player1.picURL);
        }
        else
        {
            print("alone in the room, name is : " + PlayerManager.Instance.Player1.playerName);

            UIManager.Instance.DisplayWaitingForPlayerPanel(true);
            UIManager.Instance.InitPlayer1(BallColor.White);
            UIManager.Instance.SetPlayer1Name(PlayerManager.Instance.Player1.playerName);

            UIManager.Instance.DisplayYourTurn(false);
        }
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        if (PhotonNetwork.room.PlayerCount == 2)
        {
            print("OnPhotonPlayerConnected : New player arrived !");

            UIManager.Instance.DisplayWaitingForPlayerPanel(false);
            GridManager.Instance.InitForGameStart();
            isGameStarted = true;

            PlayerManager.Instance.SetPlayerColor(BallColor.White, true);
            PlayerManager.Instance.CreatePlayer(BallColor.Black);

            UIManager.Instance.SetPlayer1Turn();
            PlayerManager.Instance.Player1.StartTurn();

            SendName(PlayerManager.Instance.Player1.playerName);
            SendPicURL(PlayerManager.Instance.Player1.picURL);
        }
        else
        {
            print("OnPhotonPlayerConnected : Alone in the room.");

            UIManager.Instance.DisplayWaitingForPlayerPanel(true);
        }
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        Debug.Log("Other player disconnected! isInactive: " + otherPlayer.IsInactive);
        if (isGameFinished)
            return;

        connection.enabled = false;
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();

        UIManager.Instance.DisplayForfeit(true);
    }

    public override void OnLeftRoom()
    {
        print("leftRoom");
    }

    public void OnRestart()
    {
        if (fbManager != null)
        {
            fbManager.NameLoaded -= OnNameLoaded;
            fbManager.PicURLLoaded -= OnPicURLLoaded;
        }
    }

    public void RestartConnection()
    {
        connection.enabled = false;
        PlayerManager.Instance.Player1.EndTurn();

        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
    }
}
