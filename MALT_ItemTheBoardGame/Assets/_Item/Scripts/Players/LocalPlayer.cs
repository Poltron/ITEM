using DG.Tweening; // dotween animations
using HedgehogTeam.EasyTouch; // input system
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayer : Player
{
    private GameObject exclusivePickableObject;
    private float lastInput = 0;

    public LocalPlayer(BallColor color)
        : base (color)
    {
    }

    public override void StartTurn()
    {
        base.StartTurn();

        Debug.Log("StartLocalPlayerTurn : " + Color);

        EasyTouch.On_TouchUp += OnTouchUp;
	}


    public override void EndTurn()
    {
        base.EndTurn();

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

        if (!gesture.pickedObject)
            return;

        //Debug.Log(gesture.pickedObject.name);

        if (exclusivePickableObject != null && gesture.pickedObject != exclusivePickableObject)
            return;

        Cell pickedCell = gesture.pickedObject.GetComponent<Cell>();
        Ball pickedBall = gesture.pickedObject.GetComponent<Ball>();

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
                    else if (pickedBall && pickedBall.Color == color)
                    {
                        currentBall.PutDownBall();
                        currentBall = pickedBall;
                        currentBall.PickUpBall();

                        CallOnBallSelection(currentBall);
                    }
                    return;
                }

                if (pickedCell.HasBall())
                {
                    currentBall.ResetPosition();
                    return;
                }


                EndTurn();
                ballCount--;
                currentBall.DOPlace(pickedCell);
                currentBall.HideHighlight();
                currentBall = null;

                CallOnCellSelection(pickedCell);

                SendTurnDataPhase1(pickedCell);


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

            if (pickedCell.HasBall())
                return;

            EndTurn();
            ballCount--;
            SendTurnDataPhase1(pickedCell);

        }
        else
        {
            if (!pickedCell)
                return;

            if (!currentCell)
            {
                if (!pickedCell.HasBall(color))
                    return;

                SelectBall(pickedCell);
            }
            else
            {
                MoveBall(pickedCell);
            }
        }
    }

    public void SelectBall(Cell pickedCell)
    {
        Cell destinationCell;

        List<Cell> neighbours = GridManager.Instance.ModelGrid.GetCellNeighbours(pickedCell);
        currentCellsToMove = new List<Cell>();
        currentCellsToJump = new List<Cell>();
        currentCell = pickedCell;

        foreach (Cell cell in neighbours)
        {
            if (!cell.HasBall())
            {
                if (!hasAlreadyJumpedOnce)
                    currentCellsToMove.Add(cell);
            }
            else
            {
                destinationCell = GridManager.Instance.ModelGrid.GetCellFromDirection(pickedCell, cell);
                if (destinationCell)
                {
                    if (!destinationCell.HasBall())
                        currentCellsToJump.Add(destinationCell);
                }

            }
        }

        currentBall = currentCell.ball;
        currentBall.PickUpBall();

        CallOnBallSelection(currentBall);
    }

    void ResetCurrentCells()
    {
        currentCell = null;
        currentCellsToJump = null;
        currentCellsToMove = null;
    }

    void SendTurnDataPhase1(Cell pickedCell)
    {
        List<Vector2> movements = new List<Vector2>();
        movements.Add(new Vector2(pickedCell.x, pickedCell.y));

        CallOnTurnFinished(movements);
    }

    void SendTurnDataPhase2()
    {
        hasAlreadyJumpedOnce = false;

        CallOnTurnFinished(movements);
    }

    public void MoveBall(Cell pickedCell)
    {
        if (pickedCell == currentCell)
        {
            GridManager.Instance.ModelGrid.ResetCellsColor();

            if (hasAlreadyJumpedOnce)
            {
                currentBall.HideHighlight();
                EndTurn();
                SendTurnDataPhase2();
            }
            else
            {
                currentBall.PutDownBall();
            }

            ResetCurrentCells();
            return;
        }


        if (pickedCell.HasBall())
        {
            currentBall.ResetPosition();
            return;
        }

        if (currentCellsToMove.Contains(pickedCell))
        {
            movements = new List<Vector2>();
            movements.Add(new Vector2(currentCell.x, currentCell.y));
            movements.Add(new Vector2(pickedCell.x, pickedCell.y));

            GridManager.Instance.ModelGrid.ResetCellsColor();
            GridManager.Instance.ChangeBallPosition(currentCell, pickedCell);
            currentBall.HideHighlight();
            ResetCurrentCells();
            EndTurn();
            SendTurnDataPhase2();

        }
        else if (currentCellsToJump.Contains(pickedCell))
        {
            if (hasAlreadyJumpedOnce)
                movements.Add(new Vector2(pickedCell.x, pickedCell.y));
            else
            {
                movements = new List<Vector2>();
                movements.Add(new Vector2(currentCell.x, currentCell.y));
                movements.Add(new Vector2(pickedCell.x, pickedCell.y));
            }

            GridManager.Instance.ModelGrid.ResetCellsColor();
            
            EasyTouch.On_TouchUp -= OnTouchUp;
            GridManager.Instance.ChangeBallPosition(currentCell, pickedCell);
            DOVirtual.DelayedCall(1.0f, () => { EasyTouch.On_TouchUp += OnTouchUp; });

            ResetCurrentCells();
            hasAlreadyJumpedOnce = true;
            SelectBall(pickedCell);
        }
        else
        {
            currentBall.ResetPosition();
            //Utils.ResetCellsColor (grid);
            return;
        }

        CallOnCellSelection(pickedCell);

        Utils.CheckWin(GridManager.Instance.ModelGrid, pickedCell);
    }

    public void PickBall(Ball ball)
    {
        currentBall = ball;
        currentBall.PickUpBall();

        CallOnBallSelection(ball);
    }

    public void PickCell(Cell cell)
    {
        currentCell = cell;

        CallOnCellSelection(cell);
    }

    void RegisterBall(Gesture gesture, Ball ball)
    {
        currentBall = ball;
        currentBall.PickUpBall();

        CallOnBallSelection(ball);
    }
}
