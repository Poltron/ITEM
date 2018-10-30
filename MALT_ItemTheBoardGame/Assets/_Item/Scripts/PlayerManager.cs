using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public bool disableAI = false;
    [SerializeField]
    public bool randomAI = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
		
	}
	
    public void CreatePlayer(BallColor color, bool isPlayer1 = false)
    {
        Player player = new Player();
        player.ballPrefab = color == BallColor.White ? whiteBallPrefab : blackBallPrefab;
        player.OnPhase1TurnFinished += GridManager.Instance.Phase1TurnFinishedPlayer;
        player.OnPhase2TurnFinished += GridManager.Instance.Phase2TurnFinishedPlayer;

        if (isPlayer1)
        {
            players[0] = player;
            UIManager.Instance.InitPlayer1(color);
        }
        else
        {
            players[1] = player;
            UIManager.Instance.InitPlayer2(color);
        }
    }

    public void SetPlayerColor(BallColor color, bool isPlayer1 = false)
    {
        if (isPlayer1 && players[0] != null)
        {
            players[0].ballPrefab = color == BallColor.White ? whiteBallPrefab : blackBallPrefab;
            UIManager.Instance.InitPlayer1(color);
        }
        else if (players[1] != null)
        {
            players[1].ballPrefab = color == BallColor.White ? whiteBallPrefab : blackBallPrefab;
            UIManager.Instance.InitPlayer2(color);
        }
    }

    public string GetIAName()
    {
        if (Options.IsLanguageFr())
        {
            return "Charles (IA)";
        }

        return "Charles (AI)";
    }

    public Sprite GetIASprite()
    {
        return IASprite;
    }
}
