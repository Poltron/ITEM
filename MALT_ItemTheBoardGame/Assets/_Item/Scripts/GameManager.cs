﻿using DG.Tweening; // DOTween & DOVirtual
using Facebook.Unity; // Facebook
using GS; // FacebookPro
using Photon; // Network

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    MainMenu,
    LookingForPlayer,
    Gameplay,
    GameResults
}

public enum GameMode
{
    Local,
    Remote,
    AI
}

public class GameManager : PunBehaviour
{
    static private GameManager instance;
    static public GameManager Instance { get { return instance; } }

    private GameMode gameMode;
    public GameMode GameMode { get { return gameMode; } }

    private GameState gameState;
    public GameState GameState { get { return gameState; } }

    [SerializeField]
    private FBManager fbManager;

    private Connection connection;

    [SerializeField]
    private float timeToLaunchGameVSIA = 4;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        connection = GetComponent<Connection>();

        gameState = GameState.MainMenu;

        Options.Init();
    }

    private void Start()
    {
        UIManager.Instance.EndGame += EndGame;
    }

    public void StartGame ()
    {
        // load fb data
        if (FB.IsLoggedIn)
        {
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
            GridManager.Instance.StartTurns();
            

        gameState = GameState.Gameplay;
    }
    
    public void EndGame()
    {
        GridManager.Instance.ResetGame();
        UIManager.Instance.ShowGameplayCanvas(false);
        UIManager.Instance.ShowMenuCanvas(true);
        GridManager.Instance.DisplayBoard(false);
    }

    private void Update()
    {
        if (GameState == GameState.LookingForPlayer && Input.GetKeyDown(KeyCode.Space))
        {
            Disconnect();
            StartGameVSIA();
        }
    }

    #region FACEBOOK
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


    public void StartLocalGame()
    {
        gameMode = GameMode.Local;

        GridManager.Instance.InitForGameStart();

        PlayerManager.Instance.CreateLocalPlayer(BallColor.White, PlayerID.Player1);
        PlayerManager.Instance.CreateLocalPlayer(BallColor.Black, PlayerID.Player2);

        StartGame();
    }

    public void StartGameVSIA()
    {
        if (GameState != GameState.MainMenu && GameState != GameState.LookingForPlayer)
            return;

        gameMode = GameMode.AI;

        GridManager.Instance.InitForGameStart();

        PlayerManager.Instance.CreateLocalPlayer(BallColor.White, PlayerID.Player1);
        PlayerManager.Instance.CreateAIPlayer(BallColor.Black, PlayerID.Player2);

        StartGame();
    }

    public void StartLookingForOpponent()
    {
        gameMode = GameMode.Remote;
        gameState = GameState.LookingForPlayer;

        connection.ApplyUserIdAndConnect();

        UIManager.Instance.mainMenuPanel.HideAll();
        UIManager.Instance.DisplayWaitingForPlayerPanel(true);
    }

    public void StopLookingForOpponent()
    {
        gameState = GameState.MainMenu;

        Disconnect();

        UIManager.Instance.mainMenuPanel.ShowMenu();
        UIManager.Instance.DisplayWaitingForPlayerPanel(false);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.room.PlayerCount == 2)
        {
            gameState = GameState.Gameplay;

            GridManager.Instance.InitForGameStart();

            PlayerManager.Instance.CreateLocalPlayer(BallColor.Black, PlayerID.Player1);
            PlayerManager.Instance.CreateRemotePlayer(BallColor.White, PlayerID.Player2);

            UIManager.Instance.SetPlayer2Turn();
            UIManager.Instance.SetPlayer1Name(PlayerManager.Instance.Player1.playerName);

            print("OnJoinedRoom : 2 players in the room, sending player name > " + PlayerManager.Instance.Player1.playerName);
            SendName(PlayerManager.Instance.Player1.playerName);
            SendPicURL(PlayerManager.Instance.Player1.picURL);

            StartGame();
        }
        else
        {
            print("OnJoinedRoom : alone in the room");
            //UIManager.Instance.InitPlayer1(BallColor.White);
            //UIManager.Instance.SetPlayer1Name(PlayerManager.Instance.Player1.playerName);
        }
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        if (PhotonNetwork.room.PlayerCount == 2)
        {
            print("OnPhotonPlayerConnected : New player arrived !");

            gameState = GameState.Gameplay;

            GridManager.Instance.InitForGameStart();

            PlayerManager.Instance.CreateLocalPlayer(BallColor.White, PlayerID.Player1);
            PlayerManager.Instance.CreateRemotePlayer(BallColor.Black, PlayerID.Player2);

            UIManager.Instance.SetPlayer1Turn();
            PlayerManager.Instance.Player1.StartTurn();

            SendName(PlayerManager.Instance.Player1.playerName);
            SendPicURL(PlayerManager.Instance.Player1.picURL);

            StartGame();
        }
        else
        {
            print("OnPhotonPlayerConnected : Alone in the room.");
        }
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        Debug.Log("Other player disconnected! isInactive: " + otherPlayer.IsInactive);

        if (GameState == GameState.GameResults)
            return;

        Disconnect();
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
        PlayerManager.Instance.Player1.EndTurn();
        Disconnect();
    }

    public void Disconnect()
    {
        connection.enabled = false;

        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
    }
}
