using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerID
{
    Player1,
    Player2
}

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager instance;
    public static PlayerManager Instance { get { return instance; } }

    private Player[] players = new Player[2];

    public Player Player1 { get { return players[0]; } }
    public Player Player2 { get { return players[1]; } }

    [Header("Colors")]
    [SerializeField]
    private Ball whiteBallPrefab;
    [SerializeField]
    private Ball blackBallPrefab;

    [Header("AI")]
    [SerializeField]
    private Sprite IASprite;

    [SerializeField]
    private AIEvaluationData aiEvaluationData;
    private AIBehaviour aiBehaviour;

    public AIBehaviour AIBehaviour { get { return aiBehaviour; } }

    [SerializeField]
    public bool disableAI = false;
    [SerializeField]
    public bool randomAI = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        aiBehaviour = new AIBehaviour(aiEvaluationData);
    }

    public void CreateLocalPlayer(BallColor color, PlayerID id)
    {
        LocalPlayer player = new LocalPlayer(color);
        if (id == PlayerID.Player1)
        {
            player.OnTurnFinished += GridManager.Instance.PlayerTurnEnded;
            players[0] = player;
            UIManager.Instance.InitPlayer1(color);
        }
        else
        {
            player.OnTurnFinished += GridManager.Instance.PlayerTurnEnded;
            players[1] = player;
            UIManager.Instance.InitPlayer2(color);
        }
    }

    public void CreateAIPlayer(BallColor color, PlayerID id)
    {
        AIPlayer player = new AIPlayer(color);

        UIManager.Instance.SetPlayer2Name(PlayerManager.Instance.GetIAName());
        UIManager.Instance.SetPlayer2Pic(PlayerManager.Instance.GetIASprite());

        if (id == PlayerID.Player1)
        {
            player.OnTurnFinished += GridManager.Instance.PlayerTurnEnded;
            players[0] = player;
            UIManager.Instance.InitPlayer1(color);
        }
        else
        {
            player.OnTurnFinished += GridManager.Instance.PlayerTurnEnded;
            players[1] = player;
            UIManager.Instance.InitPlayer2(color);
        }
    }

    public void CreateRemotePlayer(BallColor color, PlayerID id)
    {
        RemotePlayer player = new RemotePlayer(color);

        if (id == PlayerID.Player1)
        {
            player.OnTurnFinished += GridManager.Instance.PlayerTurnEnded;
            players[0] = player;
            UIManager.Instance.InitPlayer1(color);
        }
        else
        {
            player.OnTurnFinished += GridManager.Instance.PlayerTurnEnded;
            players[1] = player;
            UIManager.Instance.InitPlayer2(color);
        }
    }

    public void SetPlayerColor(BallColor color, PlayerID id)
    {
        if (id == PlayerID.Player1 && players[0] != null)
        {
            players[0].SetColor(color);
            UIManager.Instance.InitPlayer1(color);
        }
        else if (players[1] != null)
        {
            players[1].SetColor(color);
            UIManager.Instance.InitPlayer2(color);
        }
    }

    public Player GetPlayer(BallColor color)
    {
        if (Player1.Color == color)
        {
            return Player1;
        }
        else
        {
            return Player2;
        }
    }

    public void SwitchPlayers()
    {
        // switch players
        Player player1 = players[0];
        players[0] = players[1];
        players[1] = player1;
    }

    private string GetIAName()
    {
        if (Options.IsLanguageFr())
        {
            return "Charles (IA)";
        }

        return "Charles (AI)";
    }

    private Sprite GetIASprite()
    {
        return IASprite;
    }
}
