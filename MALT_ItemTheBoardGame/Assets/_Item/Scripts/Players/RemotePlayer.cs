﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemotePlayer : Player
{
    public RemotePlayer(BallColor color)
        : base (color)
    {
    }

    public override void StartTurn()
    {
        base.StartTurn();

        if (!(NbOfTurn == 1 && PlayerManager.Instance.GetPlayer(GridManager.Instance.NotActualTurn).NbOfTurn == 0))
        {
            GridManager.Instance.SendLastTurnData();
            Debug.Log("after send last turn data");
        }
    }

    public override void EndTurn()
    {
        base.EndTurn();
    }

    public void SetLastMovements(Vector2[] _movements, int ballId)
    {
        movements = new List<Vector2>(_movements);
        currentBall = GridManager.Instance.GetBall(Color, ballId);
    }
}
