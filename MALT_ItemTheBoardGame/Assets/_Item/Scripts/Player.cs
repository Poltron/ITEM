using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HedgehogTeam.EasyTouch;
using System;
using DG.Tweening;

public class Player
{
	public string playerName;
	public string picURL;

    public event Action<Ball> OnBallSelection;
    public event Action<Cell> OnCellSelection;

    public BallColor color { get { return ballPrefab.Color; } }
	public Ball ballPrefab;

	public uint ballCount = 10;
	private bool isTweening = false;

	public event Action<Vector2> OnPhase1TurnFinished;
	public event Action<List<Vector2>> OnPhase2TurnFinished;

	public bool hasAlreadyJumpedOnce = false;
	private Cell currentCell;
	private List<Cell> currentCellsToMove;
	private List<Cell> currentCellsToJump;
	private List<Vector2> movements;

	private Ball currentBall;

    private GameObject exclusivePickableObject;

	public bool showHelp = false;

    private float lastInput = 0;

    public void Reset()
    {
        ballCount = 10;
        isTweening = false;
        hasAlreadyJumpedOnce = false;
        currentCell = null;
        currentBall = null;
        currentCellsToMove = new List<Cell>();
        currentCellsToJump = new List<Cell>();
        movements = new List<Vector2>();
    }

	public void StartTurn()
    {
        //Debug.Log("playerstartturn");
           
		EasyTouch.On_TouchUp += OnTouchUp;
	}


    public void EndTurn()
    {
        //Debug.Log("playerendturn");

        GridManager.Instance.ModelGrid.ResetCellsColor();

        EasyTouch.On_TouchUp -= OnTouchUp;
	}
       
    public void SetExclusivePickableObject(GameObject go)
    {
        exclusivePickableObject = go;
    }

    public void OnTouchUpPublic(Gesture gesture)
    {
        //Debug.Log("on touch up public");
        OnTouchUp(gesture);
    }

	void OnTouchUp(Gesture gesture)
    {
        // mesure préventive pour le moment ou il y a un double input en pahse 1 qui bloque la bille sélectionnée ???
        if (lastInput + 0.1f > Time.realtimeSinceStartup)
            return;

        lastInput = Time.realtimeSinceStartup;

        //Debug.Log("on touch up");

        if (isTweening)
		    return;

        if (!gesture.pickedObject)
		    return;

        //Debug.Log(gesture.pickedObject.name);

        if (exclusivePickableObject != null && gesture.pickedObject != exclusivePickableObject)
            return;

		Cell pickedCell = gesture.pickedObject.GetComponent<Cell> (); 
		Ball pickedBall = gesture.pickedObject.GetComponent<Ball> ();

		if (pickedBall)
			if (pickedBall.owner)
				pickedCell = pickedBall.owner;

		if (ballCount > 0)
        {
			if (currentBall)
            {
				if (!pickedCell)
                   {
                       if (pickedBall == currentBall)
                       {
                           currentBall.PutDownBall();
                           currentBall = null;
                       }
					else if (pickedBall && pickedBall.Color == color) {
						currentBall.PutDownBall();
						currentBall = pickedBall;
						currentBall.PickUpBall();

                           if (OnBallSelection != null)
                               OnBallSelection(currentBall);
					}
					return;
				}

				if (pickedCell.HasBall ()) {
					currentBall.ResetPosition ();
					return;
				}


				EndTurn ();
				ballCount--;
				currentBall.DOPlace (pickedCell);
				currentBall.HideHighlight ();
				currentBall = null;

                   if (OnCellSelection != null)
                       OnCellSelection(pickedCell);

				SendTurnDataPhase1 (pickedCell);


			}
            else
            {
				if (!pickedBall)
					return;

				if (pickedBall.Color != color)
					return;

                   if (pickedBall.owner != null)
                       return;

                   RegisterBall(gesture, pickedBall);
				//On_DragStartPhase1 (gesture, pickedBall);
			}

			if (!pickedCell)
				return;

			if (pickedCell.HasBall ())
				return;

			EndTurn ();
			ballCount--;
			SendTurnDataPhase1 (pickedCell);

		}
        else
        {
			if (!pickedCell)
				return;

			if (!currentCell)
            {
				if(!pickedCell.HasBall(color))
					return;

				SelectBall (pickedCell);
			} 
			else
            {
				MoveBall (pickedCell);
			}
		}
	}

	public void SelectBall(Cell pickedCell)
    {
		Cell destinationCell;

		List<Cell> neighbours = GridManager.Instance.ModelGrid.GetCellNeighbours (pickedCell);
		currentCellsToMove = new List<Cell> ();
		currentCellsToJump = new List<Cell> ();
		currentCell = pickedCell;

		foreach (Cell cell in neighbours) {
			if (!cell.HasBall()) {
				if(!hasAlreadyJumpedOnce)
					currentCellsToMove.Add (cell);
			} else {
				destinationCell = GridManager.Instance.ModelGrid.GetCellFromDirection (pickedCell, cell);
				if (destinationCell) {
					if (!destinationCell.HasBall ())
						currentCellsToJump.Add (destinationCell);
				}

			}
		}

		currentBall = currentCell.ball;
		currentBall.PickUpBall();

           if (OnBallSelection != null)
               OnBallSelection(currentBall);
	}

	void ResetCurrentCells() {
		currentCell = null;
		currentCellsToJump = null;
		currentCellsToMove = null;
	}

	public void ChangeBallPosition(Cell firstCell, Cell secondCell) {

           Ball ball = firstCell.ball;

		secondCell.ball = ball;
		firstCell.ball = null;

		ball.owner = secondCell;
           
           Move move = new Move();
           move.fromX = firstCell.y;
           move.fromY = firstCell.x;
           move.toX = secondCell.y;
           move.toY = secondCell.x;
           move.color = (CellColor)ball.Color;

        GridManager.Instance.OptiGrid.DoMove(move);

		isTweening = true;

           if (ball.isPickedUp)
               ball.GetComponent<Animator>().SetTrigger("PlaceBall");
           else
               ball.GetComponent<Animator>().SetTrigger("Move");

           ball.isPickedUp = false;

           ball.FixSortingLayer(true);

           ball.transform.DOMove (secondCell.transform.position, 1f).OnComplete (() => {
			ball.transform.position = secondCell.transform.position;
			//ball.SetStartPosition ();
			isTweening = false;
               ball.FixSortingLayer(false);
           });
	}

	void SendTurnDataPhase1(Cell pickedCell) {
		List<Vector2> movements = new List<Vector2> ();
		movements.Add (new Vector2(pickedCell.x, pickedCell.y));

		if (OnPhase1TurnFinished != null)
			OnPhase1TurnFinished (new Vector2 (pickedCell.x, pickedCell.y));
	}

	void SendTurnDataPhase2() {
		hasAlreadyJumpedOnce = false;
		//print("player phase 2 turn over");

		if (OnPhase2TurnFinished != null)
			OnPhase2TurnFinished (movements);
	}

	public void MoveBall(Cell pickedCell) {
		if (pickedCell == currentCell) {
            GridManager.Instance.ModelGrid.ResetCellsColor();

			if (hasAlreadyJumpedOnce) {
				currentBall.HideHighlight ();
				EndTurn ();
				SendTurnDataPhase2 ();
			}			
               else
               {
                   currentBall.PutDownBall();
               }

			ResetCurrentCells ();
			return;
		}


		if (pickedCell.HasBall ()) {
			currentBall.ResetPosition ();
			return;
		}

		if (currentCellsToMove.Contains (pickedCell)) {
			movements = new List<Vector2> ();
			movements.Add (new Vector2 (currentCell.x, currentCell.y));
			movements.Add (new Vector2 (pickedCell.x, pickedCell.y));

            GridManager.Instance.ModelGrid.ResetCellsColor();
               ChangeBallPosition (currentCell, pickedCell);
			currentBall.HideHighlight ();
			ResetCurrentCells ();
			EndTurn ();
			SendTurnDataPhase2 ();

		} else if (currentCellsToJump.Contains (pickedCell)) {
			if (hasAlreadyJumpedOnce)
				movements.Add (new Vector2 (pickedCell.x, pickedCell.y));
			else {
				movements = new List<Vector2> ();
				movements.Add (new Vector2 (currentCell.x, currentCell.y));
				movements.Add (new Vector2 (pickedCell.x, pickedCell.y));
			}

            GridManager.Instance.ModelGrid.ResetCellsColor();
			ChangeBallPosition (currentCell, pickedCell);
			ResetCurrentCells ();
			hasAlreadyJumpedOnce = true;
			SelectBall (pickedCell);
		} else {
			currentBall.ResetPosition ();
			//Utils.ResetCellsColor (grid);
			return;
           }

           if (OnCellSelection != null)
               OnCellSelection(pickedCell);

           Utils.CheckWin (GridManager.Instance.ModelGrid, pickedCell);
	}

       public void PickBall(Ball ball)
       {
           currentBall = ball;
           currentBall.PickUpBall();

           if (OnBallSelection != null)
               OnBallSelection(ball);
       }

       public void PickCell(Cell cell)
       {
           currentCell = cell;

           if (OnCellSelection != null)
               OnCellSelection(cell);
       }

       void RegisterBall(Gesture gesture, Ball ball)  {
		currentBall = ball;
		currentBall.PickUpBall();

           if (OnBallSelection != null)
               OnBallSelection(ball);
	}

	void OnDestroy(){
		EndTurn ();
	}
}