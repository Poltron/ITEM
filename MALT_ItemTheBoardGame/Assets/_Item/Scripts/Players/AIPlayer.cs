﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Player
{
    private AIProfile aiProfile;
    public AIProfile AIProfile { get { return aiProfile; } }

    public AIPlayer(AIProfile _aiProfile, BallColor color)
        : base (color)
    {
        aiProfile = _aiProfile;
    }

    public override void StartTurn()
    {
        base.StartTurn();
        
        if (ballCount > 0)
            PlayIAPhase1();
        else
            PlayIAPhase2();
    }

    public override void EndTurn()
    {
        base.EndTurn();
    }

    public void PlayIAPhase1()
    {
        Debug.Log("PlayIAPhase1");

        PlayerManager.Instance.AIBehaviour.SetAIProfile(aiProfile);

        if (PlayerManager.Instance.randomAI)
            CoroutineRunner.Start(PlayerManager.Instance.AIBehaviour.GetRandomMove(GridManager.Instance.OptiGrid, PlayIAPhase1CalculusEnded));
        else
            CoroutineRunner.Start(PlayerManager.Instance.AIBehaviour.GetBestMove(GridManager.Instance.OptiGrid, PlayIAPhase1CalculusEnded));
    }

    public void PlayIAPhase1CalculusEnded(Move move)
    {
        CoroutineRunner.Start(waitFor(3f - PlayerManager.Instance.AIBehaviour.timeSpent, move, PlayAIMovePhase1));
    }

    public void PlayAIMovePhase1(Move move)
    {
        Cell cell = GridManager.Instance.ModelGrid.GetCellFromModel(move.toY, move.toX);

        //GridManager.Instance.OptiGrid.DoMove(move);
        currentBall = GridManager.Instance.PlaceBallIA(cell);
        ballCount--;

        movements = new List<Vector2>();
        movements.Add(new Vector2(move.toY, move.toX));

        EndTurn();
    }

    public void PlayIAPhase2()
    {
        Debug.Log("PlayIAPhase2");

        PlayerManager.Instance.AIBehaviour.SetAIProfile(aiProfile);

        if (PlayerManager.Instance.randomAI)
            CoroutineRunner.Start(PlayerManager.Instance.AIBehaviour.GetRandomMove(GridManager.Instance.OptiGrid, PlayIAPhase2CalculusEnded));
        else
            CoroutineRunner.Start(PlayerManager.Instance.AIBehaviour.GetBestMove(GridManager.Instance.OptiGrid, PlayIAPhase2CalculusEnded));
    }

    public void PlayIAPhase2CalculusEnded(Move move)
    {
        CoroutineRunner.Start(waitFor(3f - PlayerManager.Instance.AIBehaviour.timeSpent, move, PlayAIMovePhase2));
    }

    public void PlayAIMovePhase2(Move move)
    {
        Cell cellFrom = GridManager.Instance.ModelGrid.GetCellFromModel(move.fromY, move.fromX);
        Cell cellTo = GridManager.Instance.ModelGrid.GetCellFromModel(move.toY, move.toX);

        //GridManager.Instance.OptiGrid.DoMove(move);
        currentBall = GridManager.Instance.ChangeBallPosition(cellFrom, cellTo);

        movements = new List<Vector2>();
        movements.Add(new Vector2(move.toY, move.toX));

        EndTurn();
    }

    IEnumerator waitFor(float t, Move move, System.Action<Move> func)
    {
        while ((t -= Time.deltaTime) > 0)
            yield return new WaitForEndOfFrame();

        func(move);
    }

}
