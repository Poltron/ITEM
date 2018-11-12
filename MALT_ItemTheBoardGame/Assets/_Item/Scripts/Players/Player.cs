using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HedgehogTeam.EasyTouch;
using System;
using DG.Tweening;

abstract public class Player
{
	public string playerName;
	public string picURL;

    public event Action<Ball> OnBallSelection;
    public event Action<Cell> OnCellSelection;

    protected BallColor color;
    public BallColor Color { get { return color; } }

	public uint ballCount = 10;

	public event Action<List<Vector2>, int> OnTurnFinished;

    protected bool hasAlreadyJumpedOnce = false;

    public bool HasAlreadyJumpedOnce { get { return hasAlreadyJumpedOnce; } }
	protected Cell currentCell;
    protected List<Cell> currentCellsToMove;
    protected List<Cell> currentCellsToJump;
    protected List<Vector2> movements;
    protected Ball currentBall;

    protected int nbOfTurn;
    public int NbOfTurn { get { return nbOfTurn; } }

    public int totalScore;
    public int roundScore;

    public Player(BallColor _color)
    {
        color = _color;
    }

    public void SetColor(BallColor _color)
    {
        color = _color;
    }

    public void Reset()
    {
        ballCount = 10;
        nbOfTurn = 0;
        hasAlreadyJumpedOnce = false;
        currentCell = null;
        currentBall = null;
        currentCellsToMove = new List<Cell>();
        currentCellsToJump = new List<Cell>();
        movements = new List<Vector2>();
    }

    public virtual void StartTurn()
    {
        nbOfTurn++;
        Debug.Log(Color + " turn started");
    }

    public virtual void EndTurn()
    {
        Debug.Log(Color + " turn ended");
        if (currentBall == null)
        {
            Debug.Log("currentBall is null");
        }

        if (currentBall != null && movements.Count > 0)
            CallOnTurnFinished(movements, currentBall.ballId);

        currentBall = null;

        Debug.Log("after call on turn finished");
    }

    protected void CallOnBallSelection(Ball ball)
    {
        if (OnBallSelection != null)
            OnBallSelection(ball);
    }

    protected void CallOnCellSelection(Cell cell)
    {
        if (OnCellSelection != null)
            OnCellSelection(cell);
    }
    
    protected void CallOnTurnFinished(List<Vector2> vecList, int ballId)
    {
        if (OnTurnFinished != null)
            OnTurnFinished(vecList, ballId);
    }
}