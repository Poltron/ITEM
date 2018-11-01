using System.Collections;
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
    }

    public override void EndTurn()
    {
        base.EndTurn();
    }
}
