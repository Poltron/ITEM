using DG.Tweening; // DOTween & DOVirtual
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
        UIManager.Instance.EndGame += EndGameplay;
    }

    public void StartGame ()
    {
        // load fb data
        if (FB.IsLoggedIn)
        {
            Debug.Log("Logged in Facebook");
            PlayerManager.Instance.Player1.playerName = fbManager.pName;
            PlayerManager.Instance.Player1.picURL = fbManager.pUrlPic;

            Debug.Log(fbManager.pName + " / " + fbManager.pUrlPic);

            StartCoroutine(Utils.LoadSpriteFromURL(PlayerManager.Instance.Player1.picURL, (sprite) =>
            {
                UIManager.Instance.SetPlayer1Pic(sprite);
            }));
            UIManager.Instance.SetPlayer1Name(PlayerManager.Instance.Player1.playerName);
            
            fbManager.NameLoaded += OnNameLoaded;
            fbManager.PicURLLoaded += OnPicURLLoaded;
            
            //
            SendName(PlayerManager.Instance.Player1.playerName);
            SendPicURL(PlayerManager.Instance.Player1.picURL);
            //
        }
        else
        {
            Debug.Log("Not logged in Facebook");
        }

        // init UI
        UIManager.Instance.Init();
        GridManager.Instance.ReplaceBalls();

        DOVirtual.DelayedCall(UIManager.Instance.timeBeforeAskForTutoPop, () => {
            if (gameState != GameState.Gameplay)
                return;

            // start with tutorial or not
            if (Options.GetAskForTuto() && GameMode != GameMode.Remote)
            {
                UIManager.Instance.PopTuto();
            }
            else
            {
                UIManager.Instance.backToMainMenuButton.enabled = true;
                GridManager.Instance.StartTurns();
            }
        });

        gameState = GameState.Gameplay;
    }
    
    public void GameEnded()
    {
        gameState = GameState.GameResults;

        if (gameMode == GameMode.Remote)
            Disconnect();
    }

    public void EndGameplay()
    {
        gameState = GameState.MainMenu;

        GridManager.Instance.ResetGame();
        UIManager.Instance.ShowGameplayCanvas(false);
        UIManager.Instance.ShowMenuCanvas(true);
        GridManager.Instance.DisplayBoard(false);
    }

    #region FACEBOOK
    private void OnNameLoaded(string name)
    {
        print("onnameloaded " + name);
        PlayerManager.Instance.Player1.playerName = name;
    }

    private void OnPicURLLoaded(string url)
    {
        print("onpicurlloaded" + url);

        PlayerManager.Instance.Player1.picURL = url;

        StartCoroutine(Utils.LoadSpriteFromURL(PlayerManager.Instance.Player1.picURL, (sprite) => {
            UIManager.Instance.SetPlayer1Pic(sprite);
        }));
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
        Debug.Log("received name " + name);
        UIManager.Instance.SetPlayer2Name(name);
        return;
    }

    [PunRPC]
    private void ReceivePicURL(string picURL)
    {
        Debug.Log("received pic url " + picURL);
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
        GridManager.Instance.SetPawnsStartPosition(BallColor.White, BallColor.White);
        GridManager.Instance.SetPawnsStartPosition(BallColor.Black, BallColor.Black);

        // setup les noms
        if (Options.GetLanguage() == "FR")
        {
            if (!FB.IsLoggedIn)
            {
                PlayerManager.Instance.Player1.playerName = "Joueur 1";
                UIManager.Instance.SetPlayer1Name(PlayerManager.Instance.Player1.playerName);
            }
            PlayerManager.Instance.Player2.playerName = "Joueur 2";
            UIManager.Instance.SetPlayer2Name(PlayerManager.Instance.Player2.playerName);
        }
        else
        {
            if (!FB.IsLoggedIn)
            {
                PlayerManager.Instance.Player1.playerName = "Player 1";
                UIManager.Instance.SetPlayer1Name(PlayerManager.Instance.Player1.playerName);
            }
            PlayerManager.Instance.Player2.playerName = "Player 2";
            UIManager.Instance.SetPlayer2Name(PlayerManager.Instance.Player2.playerName);
        }

        StartGame();
    }

    public void StartGameVSIA(AIProfile aiProfile)
    {
        if (GameState != GameState.MainMenu && GameState != GameState.LookingForPlayer)
            return;

        gameMode = GameMode.AI;

        GridManager.Instance.InitForGameStart();

        PlayerManager.Instance.CreateLocalPlayer(BallColor.White, PlayerID.Player1);
        PlayerManager.Instance.CreateAIPlayer(aiProfile, BallColor.Black, PlayerID.Player2);
        GridManager.Instance.SetPawnsStartPosition(BallColor.White, BallColor.White);
        GridManager.Instance.SetPawnsStartPosition(BallColor.Black, BallColor.Black);

        StartGame();
    }

    public void StartLookingForOpponent()
    {
        gameMode = GameMode.Remote;
        gameState = GameState.LookingForPlayer;

        connection.ApplyUserIdAndConnect();
        
        UIManager.Instance.DisplayWaitingForPlayerPanel(true);
    }

    public void StopLookingForOpponent()
    {
        gameState = GameState.MainMenu;

        Disconnect();
        
        UIManager.Instance.DisplayWaitingForPlayerPanel(false);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.room.PlayerCount == 2)
        {
            print("OnJoinedRoom : 2 players in the room");

            gameState = GameState.Gameplay;

            GridManager.Instance.InitForGameStart();

            PlayerManager.Instance.CreateLocalPlayer(BallColor.Black, PlayerID.Player1);
            PlayerManager.Instance.CreateRemotePlayer(BallColor.White, PlayerID.Player2);
            GridManager.Instance.SetPawnsStartPosition(BallColor.White, BallColor.Black);
            GridManager.Instance.SetPawnsStartPosition(BallColor.Black, BallColor.White);

            SendName(PlayerManager.Instance.Player1.playerName);
            SendPicURL(PlayerManager.Instance.Player1.picURL);

            StartGame();
        }
        else
        {
            print("OnJoinedRoom : alone in the room");
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
            GridManager.Instance.SetPawnsStartPosition(BallColor.White, BallColor.White);
            GridManager.Instance.SetPawnsStartPosition(BallColor.Black, BallColor.Black);

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
