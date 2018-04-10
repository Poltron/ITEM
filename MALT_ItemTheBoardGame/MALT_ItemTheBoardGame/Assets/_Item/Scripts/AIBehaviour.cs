using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AppAdvisory.Item
{

    public struct Move
    {
        public CellColor color;
        public int fromX;
        public int fromY;
        public int toX;
        public int toY;

        public Move(CellColor _color, int _fromX, int _fromY, int _toX, int _toY)
        {
            color = _color;

            fromX = _fromX;
            fromY = _fromY;

            toX = _toX;
            toY = _toY;
        }
    }

    public enum CellColor : byte
    {
        None = 0,
        White = 1,
        Black = 2,
        NOT_A_CELL = 3
    }

    public class OptimizedCell
    {
        public int x;
        public int y;
        public CellColor color;

        public OptimizedCell()
        { }

        public OptimizedCell(int _x, int _y, CellColor _color)
        {
            x = _x;
            y = _y;
            color = _color;
        }
    }

    public class OptimizedGrid
    {
        public CellColor[][] cells;
        public int blackBallsLeft;
        public int whiteBallsLeft;

        public int width = 5;
        public int height = 9;

        public OptimizedGrid()
        {
            cells = new CellColor[9][];
            cells[0] = new CellColor[5];
            cells[1] = new CellColor[4];
            cells[2] = new CellColor[5];
            cells[3] = new CellColor[4];
            cells[4] = new CellColor[5];
            cells[5] = new CellColor[4];
            cells[6] = new CellColor[5];
            cells[7] = new CellColor[4];
            cells[8] = new CellColor[5];
        }

        public List<OptimizedCell> GetCellNeighbours(OptimizedCell cell)
        {
            List<OptimizedCell> neighbours = new List<OptimizedCell>();

            OptimizedCell neighbour = new OptimizedCell();

            //middle up
            neighbour = GetCellFromModel(cell.x + 2, cell.y);

            if (neighbour != null)
                neighbours.Add(neighbour);

            //middle down
            neighbour = GetCellFromModel(cell.x - 2, cell.y);

            if (neighbour != null)
                neighbours.Add(neighbour);

            //middle left
            neighbour = GetCellFromModel(cell.x, cell.y - 1);

            if (neighbour != null)
                neighbours.Add(neighbour);

            //middle right
            neighbour = GetCellFromModel(cell.x, cell.y + 1);
            if (neighbour != null)
                neighbours.Add(neighbour);

            int offset = cell.x % 2;

            //top left
            neighbour = GetCellFromModel(cell.x + 1, cell.y - 1 + offset);
            if (neighbour != null)
                neighbours.Add(neighbour);

            //top right
            neighbour = GetCellFromModel(cell.x + 1, cell.y + offset);
            if (neighbour != null)
                neighbours.Add(neighbour);

            //bottom left
            neighbour = GetCellFromModel(cell.x - 1, cell.y - 1 + offset);
            if (neighbour != null)
                neighbours.Add(neighbour);

            //bottom right
            neighbour = GetCellFromModel(cell.x - 1, cell.y + offset);
            if (neighbour != null)
                neighbours.Add(neighbour);

            return neighbours;
        }

        /*public OptimizedCell GetCellFromDirection(OptimizedCell firstCell, OptimizedCell secondCell)
        {
            OptimizedCell destinationCell;

            //middle up
            if (firstCell.x == secondCell.x && (firstCell.y + 2) == secondCell.y)
            {
                destinationCell = GetCellFromModel(secondCell.x, secondCell.y + 2);
                if (destinationCell != null)
                    return destinationCell;
            }

            //middle down
            if (firstCell.x == secondCell.x && (firstCell.y - 2) == secondCell.y)
            {
                destinationCell = GetCellFromModel(secondCell.x, secondCell.y - 2);
                if (destinationCell != null)
                    return destinationCell;
            }

            //middle left
            if (firstCell.x - 1 == secondCell.x && firstCell.y == secondCell.y)
            {
                destinationCell = GetCellFromModel(secondCell.x - 1, secondCell.y);
                if (destinationCell != null)
                    return destinationCell;
            }

            //middle right
            if (firstCell.x + 1 == secondCell.x && firstCell.y == secondCell.y)
            {
                destinationCell = GetCellFromModel(secondCell.x + 1, secondCell.y);
                if (destinationCell != null)
                    return destinationCell;
            }


            int offsetFirstCell = firstCell.y % 2;
            int offsetSecondtCell = secondCell.y % 2;


            //top left
            if ((firstCell.x - 1 + offsetFirstCell) == secondCell.x && (firstCell.y + 1) == secondCell.y)
            {
                destinationCell = GetCellFromModel(secondCell.x - 1 + offsetSecondtCell, secondCell.y + 1);
                if (destinationCell != null)
                    return destinationCell;
            }

            //top right
            if ((firstCell.x + offsetFirstCell) == secondCell.x && (firstCell.y + 1) == secondCell.y)
            {
                destinationCell = GetCellFromModel(secondCell.x + offsetSecondtCell, secondCell.y + 1);
                if (destinationCell != null)
                    return destinationCell;
            }

            //bottom left
            if ((firstCell.x - 1 + offsetFirstCell) == secondCell.x && (firstCell.y - 1) == secondCell.y)
            {
                destinationCell = GetCellFromModel(secondCell.x - 1 + offsetSecondtCell, secondCell.y - 1);
                if (destinationCell != null)
                    return destinationCell;
            }

            //bottom right
            if ((firstCell.x + offsetFirstCell) == secondCell.x && (firstCell.y - 1) == secondCell.y)
            {
                destinationCell = GetCellFromModel(secondCell.x + offsetSecondtCell, secondCell.y - 1);
                if (destinationCell != null)
                    return destinationCell;
            }

            return null;
        }
        */

        public OptimizedCell GetCellFromModel(int x, int y)
        {
            if (y < 0 || y >= GetActualWidth(y) || x < 0 || x >= height || cells[x][y] == CellColor.NOT_A_CELL)
                return null;
            
            return new OptimizedCell(x, y, cells[x][y]);
        }

        public OptimizedCell GetCellFromModel(Vector2 pos)
        {
            return GetCellFromModel((int)pos.x, (int)pos.y);
        }

        private int GetActualWidth(int y)
        {
            return width - y % 2 - 1;
        }
    }

    public class AIBehaviour
    {
        int[][] positionValueGrid;
        int positionCount = 0;
        float timeSpent;

        OptimizedGrid grid;
        AIEvaluationData evaluationData;

        public AIBehaviour(AIEvaluationData _evaluationData)
        {
            positionValueGrid = new int[9][];

            positionValueGrid[0] = new int[] { 1, 5, 3, 5, 1 };
            positionValueGrid[1] = new int[] {  7, 9, 9, 7 };
            positionValueGrid[2] = new int[] { 5, 11, 11, 11, 5 };
            positionValueGrid[3] = new int[] {  9, 7, 7, 9 };
            positionValueGrid[4] = new int[] { 3, 11, 0, 11, 3 };
            positionValueGrid[5] = new int[] {  9, 7, 7, 9 };
            positionValueGrid[6] = new int[] { 5, 11, 11, 11, 5 };
            positionValueGrid[7] = new int[] {  7, 9, 9, 7 };
            positionValueGrid[8] = new int[] { 1, 5, 3, 5, 1 };

            grid = new OptimizedGrid();
            evaluationData = _evaluationData;
        }

        private void GetOptimizedGrid(GridManager gridManager)
        {
            grid.blackBallsLeft = gridManager.blackBalls.Count;
            grid.whiteBallsLeft = gridManager.whiteBalls.Count;

            for (int i = 0; i < gridManager.grid.grid.Length; ++i)
            {
                for (int j = 0; j < gridManager.grid.grid[i].Length; ++j)
                {
                    if (!gridManager.grid.grid[i][j])
                    {
                        grid.cells[i][j] = CellColor.NOT_A_CELL;
                        continue;
                    }
                    
                    if (gridManager.grid.grid[i][j].HasBall())
                    {
                        grid.cells[i][j] = (CellColor)gridManager.grid.grid[i][j].ball.color;
                    }
                    else
                    {
                        grid.cells[i][j] = CellColor.None;
                    }
                }
            }
        }

        public Move GetBestMove(GridManager gridManager, out float timeSpent)
        {
            positionCount = 0;
            timeSpent = 0;
            float actualTime = Time.realtimeSinceStartup;

            GetOptimizedGrid(gridManager);
            
            Move bestMove = MiniMaxRoot(5, true);

            float newTime = Time.realtimeSinceStartup;
            timeSpent = newTime - actualTime;

            Debug.Log(positionCount + " in " + timeSpent);

            return bestMove;
        }

        public List<Move> GetAvailableMoves(CellColor color)
        {
            List<Move> moves = new List<Move>();

            int ballsLeft, i, j = 0;

            if (color == CellColor.Black)
                ballsLeft = grid.blackBallsLeft;
            else
                ballsLeft = grid.whiteBallsLeft;

            if (ballsLeft == 0)
            {
                for (i = 0; i < grid.cells.Length; ++i)
                {
                    for (j = 0; j < grid.cells[i].Length; ++j)
                    {
                        if (grid.cells[i][j] == color)
                        {
                            OptimizedCell actualCell = new OptimizedCell();
                            actualCell.x = i;
                            actualCell.y = j;
                            actualCell.color = color;
                            
                            List<OptimizedCell> cells = grid.GetCellNeighbours(actualCell);

                            foreach (OptimizedCell cell in cells)
                            {
                                if (cell.color == CellColor.None)
                                {
                                    Move move = new Move();
                                    move.fromX = actualCell.x;
                                    move.fromY = actualCell.y;
                                    move.toX = cell.x;
                                    move.toY = cell.y;
                                    move.color = actualCell.color;

                                    moves.Add(move);
                                }
                                else if (cell.color != CellColor.NOT_A_CELL)
                                {

                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (i = 0; i < grid.cells.Length; ++i)
                {
                    for (j = 0; j < grid.cells[i].Length; ++j)
                    {
                        if (grid.cells[i][j] == CellColor.None)
                        {
                            Move move = new Move();
                            move.toX = i;
                            move.toY = j;
                            move.color = color;
                            moves.Add(move);
                        }
                    }
                }
            }

            return moves;
        }

        private int EvaluatePattern(EvaluationPattern[] patterns, int depth)
        {
            int value = 0;
            int blackCount = 0;
            int whiteCount = 0;

            for (int i = 0; i < patterns.Length; ++i)
            {
                blackCount = 0;
                whiteCount = 0;

                for (int j = 0; j < patterns[i].positions.Length; ++j)
                {
                    IntVec2 pos = patterns[i].positions[j];
                    if (grid.cells[pos.X][pos.Y] == CellColor.Black)
                    {
                        blackCount++;
                    }
                    else if (grid.cells[pos.X][pos.Y] == CellColor.White)
                    {
                        whiteCount++;
                    }
                }

                if (blackCount == 5)
                {
                    return -9999999 + depth;
                }
                else if (whiteCount == 5)
                {
                    return 9999999 - depth;
                }
                else
                {
                    if (blackCount == 1 && whiteCount == 0)
                        value -= 100;
                    else if (blackCount == 2 && whiteCount == 0)
                        value -= 400;
                    else if (blackCount == 3 && whiteCount == 0)
                        value -= 1500;
                    else if (blackCount == 4 && whiteCount == 0)
                        value -= 20000;

                    else if (blackCount == 4 && whiteCount == 1)
                        value += 75000;

                    else if (blackCount == 2 && whiteCount == 1)
                        value -= 1000;
                    else if (blackCount == 1 && whiteCount == 2)
                        value += 1000;

                    else if (blackCount == 3 && whiteCount == 1)
                        value -= 2500;
                    else if (blackCount == 1 && whiteCount == 3)
                        value += 2500;

                    else if (blackCount == 3 && whiteCount == 2)
                        value -= 4000;
                    else if (blackCount == 2 && whiteCount == 3)
                        value += 4000;

                    else if (blackCount == 1 && whiteCount == 4)
                        value -= 75000;

                    else if (blackCount == 0 && whiteCount == 1)
                        value += 300;
                    else if (blackCount == 0 && whiteCount == 2)
                        value += 4000;
                    else if (blackCount == 0 && whiteCount == 3)
                        value += 15000;
                    else if (blackCount == 0 && whiteCount == 4)
                        value += 50000;

                    /*if (blackCount == 1 && whiteCount == 0)
                        value -= 100;
                    else if (blackCount == 2 && whiteCount == 0)
                        value -= 400;
                    else if (blackCount == 3 && whiteCount == 0)
                        value -= 1500;
                    else if (blackCount == 4 && whiteCount == 0)
                        value -= 20000;

                    else if (blackCount == 1 && whiteCount == 1)
                        value -= 100;
                    else if (blackCount == 2 && whiteCount == 1)
                        value -= 300;
                    else if (blackCount == 3 && whiteCount == 1)
                        value -= 1000;
                    else if (blackCount == 4 && whiteCount == 1)
                        value -= 3000;

                    else if (blackCount == 1 && whiteCount == 2)
                        value -= 100;
                    else if (blackCount == 2 && whiteCount == 2)
                        value -= 300;
                    else if (blackCount == 3 && whiteCount == 2)
                        value -= 1000;

                    else if (blackCount == 1 && whiteCount == 3)
                        value -= 2000;
                    else if (blackCount == 2 && whiteCount == 3)
                        value -= 1000;

                    else if (blackCount == 1 && whiteCount == 4)
                        value -= 250000;
                    
                    else if (blackCount == 0 && whiteCount == 1)
                        value += 100;
                    else if (blackCount == 0 && whiteCount == 2)
                        value += 3000;
                    else if (blackCount == 0 && whiteCount == 3)
                        value += 75000;
                    else if (blackCount == 0 && whiteCount == 4)
                        value += 1000000;*/

                    /*if (blackCount == 1 && whiteCount == 0)
                        value -= 100;
                    else if (blackCount == 2 && whiteCount == 0)
                        value -= 400;
                    else if (blackCount == 3 && whiteCount == 0)
                        value -= 1500;
                    else if (blackCount == 4 && whiteCount == 0)
                        value -= 20000;
                    else if (blackCount == 4 && whiteCount == 1)
                        value += 30000;

                    else if (blackCount == 0 && whiteCount == 1)
                        value += 100;
                    else if (blackCount == 0 && whiteCount == 2)
                        value += 400;
                    else if (blackCount == 0 && whiteCount == 3)
                        value += 1500;
                    else if (blackCount == 0 && whiteCount == 4)
                        value += 20000;
                    else if (blackCount == 1 && whiteCount == 4)
                        value -= 30000;*/
                }
            }
            return value;
        }

        public int EvaluateGrid(int depth)
        {
            
            int value = 0;
            int tmpValue = 0;

            tmpValue = EvaluatePattern(evaluationData.horizontalLinePatterns, depth);
            if (IsGameEnded(tmpValue))
            {
                return tmpValue;
            }
            value += tmpValue;

            tmpValue = EvaluatePattern(evaluationData.verticalLinePatterns, depth);
            if (IsGameEnded(tmpValue))
            {
                return tmpValue;
            }
            value += tmpValue;

            tmpValue = EvaluatePattern(evaluationData.diagonalLinePatterns, depth);
            if (IsGameEnded(tmpValue))
            {
                return tmpValue;
            }
            value += tmpValue;

            tmpValue = EvaluatePattern(evaluationData.otherDiagonalLinePatterns, depth);
            if (IsGameEnded(tmpValue))
            {
                return tmpValue;
            }
            value += tmpValue;

            tmpValue = EvaluatePattern(evaluationData.horizontalCrossPatterns, depth);
            if (IsGameEnded(tmpValue))
            {
                return tmpValue;
            }
            value += tmpValue;


            tmpValue = EvaluatePattern(evaluationData.diagonalCrossPatterns, depth);
            if (IsGameEnded(tmpValue))
            {
                return tmpValue;
            }
            value += tmpValue;


            return value;
        }

        public Move MiniMaxRoot(int depth, bool isMaximisingPlayer)
        {
            CellColor color = CellColor.Black;

            if (!isMaximisingPlayer)
                color = CellColor.White;

            List<Move> newGameMoves = GetAvailableMoves(color);
            int bestMove = int.MinValue + 1;
            List<Move> bestMovesFound = new List<Move>();

            // loop on all moves available on this board
            for (int i = 0; i < newGameMoves.Count; i++)
            {
                Move move = newGameMoves[i];

                positionCount++;

                //  DO MOVE
                // if there isn't any balls left, a ball is moved
                bool newBall = false;
                if (!IsBallsLeft(color))
                {
                    // remove ball from "FROM" cell
                    grid.cells[move.fromX][move.fromY] = CellColor.None;
                }
                // if there are some balls left, a ball is added to the board
                else
                {
                    newBall = true;
                    SubBall(color);
                }   

                // add ball to "TO" cell
                grid.cells[move.toX][move.toY] = color;

                int value = -EvaluateGrid(depth);

                if (!IsGameEnded(value))
                {
                    // get loop on all moves available on board with this move
                    value = MiniMax(depth - 1, int.MinValue + 1, int.MaxValue - 1, !isMaximisingPlayer);
                }

                grid.cells[move.toX][move.toY] = CellColor.None;

                // UNDO MOVE
                // if there isn't any balls left, a ball is moved
                if (!newBall)
                {
                    // remove ball from "FROM" cell
                    grid.cells[move.fromX][move.fromY] = color;
                }
                // if there are some balls left, a ball is added to the board
                else
                {
                    AddBall(color);
                }

                //Debug.Log("move " + i + " : " + value + " ( " + move.toX + " ; " + move.toY + " )");

                // is this move better
                if (value > bestMove)
                {
                    bestMove = value;
                    bestMovesFound.Clear();
                    bestMovesFound.Add(move);
                }
                else if (value == bestMove)
                {
                    bestMovesFound.Add(move);
                }
            }

            return bestMovesFound[Random.Range(0, bestMovesFound.Count - 1)];
        }


        public int MiniMax(int depth, int alpha, int beta, bool isMaximisingPlayer)
        {
            positionCount++;

            if (depth == 0)
            {
                return -EvaluateGrid(depth);
            }

            var newGameMoves = GetAvailableMoves(CellColor.Black);

            // IA
            if (isMaximisingPlayer)
            {
                var bestMove = alpha;
                for (var i = 0; i < newGameMoves.Count; i++)
                {
                    Move move = newGameMoves[i];

                    //  DO MOVE
                    // if there isn't any balls left, a ball is moved
                    bool newBall = false;
                    if (!IsBallsLeft(CellColor.Black))
                    {
                        // remove ball from "FROM" cell
                        grid.cells[move.fromX][move.fromY] = CellColor.None;
                    }
                    // if there are some balls left, a ball is added to the board
                    else
                    {
                        newBall = true;
                        SubBall(CellColor.Black);
                    }

                    // add ball to "TO" cell
                    grid.cells[move.toX][move.toY] = CellColor.Black;


                    bestMove = -EvaluateGrid(depth);

                    if (!IsGameEnded(bestMove))
                    {
                        // get loop on all moves available on board with this move
                        bestMove = Mathf.Max(bestMove, MiniMax(depth - 1, bestMove, beta, !isMaximisingPlayer));
                    }

                    grid.cells[move.toX][move.toY] = CellColor.None;

                    // UNDO MOVE
                    // if there isn't any balls left, a ball is moved
                    if (!newBall)
                    {
                        // remove ball from "FROM" cell
                        grid.cells[move.fromX][move.fromY] = CellColor.Black;
                    }
                    // if there are some balls left, a ball is added to the board
                    else
                    {
                        AddBall(CellColor.Black);
                    }

                    if (beta <= bestMove)
                    {
                        return bestMove;
                    }
                }
                return bestMove;
            }
            // Joueur
            else
            {
                var bestMove = beta;
                for (var i = 0; i < newGameMoves.Count; i++)
                {
                    Move move = newGameMoves[i];

                    //  DO MOVE
                    // if there isn't any balls left, a ball is moved
                    bool newBall = false;
                    if (!IsBallsLeft(CellColor.Black))
                    {
                        // remove ball from "FROM" cell
                        grid.cells[move.fromX][move.fromY] = CellColor.None;
                    }
                    // if there are some balls left, a ball is added to the board
                    else
                    {
                        newBall = true;
                        SubBall(CellColor.Black);
                    }

                    // add ball to "TO" cell
                    grid.cells[move.toX][move.toY] = CellColor.Black;

                    bestMove = -EvaluateGrid(depth);

                    if (!IsGameEnded(bestMove))
                    {
                        // get loop on all moves available on board with this move
                        bestMove = Mathf.Min(bestMove, MiniMax(depth - 1, alpha, bestMove, !isMaximisingPlayer));
                    }

                    grid.cells[move.toX][move.toY] = CellColor.None;

                    // UNDO MOVE
                    // if there isn't any balls left, a ball is moved
                    if (!newBall)
                    {
                        // remove ball from "FROM" cell
                        grid.cells[move.fromX][move.fromY] = CellColor.Black;
                    }
                    // if there are some balls left, a ball is added to the board
                    else
                    {
                        AddBall(CellColor.Black);
                    }
                    
                    if (bestMove <= alpha)
                    {
                        return bestMove;
                    }
                }
                return bestMove;
            }
        }


        /*
        public int MiniMax(int depth, int alpha, int beta, bool isMaximisingPlayer)
        {
            positionCount++;

            if (depth == 0) {
                return -EvaluateGrid(depth);
            }
            
            var newGameMoves = GetAvailableMoves(CellColor.Black);

            // IA
            if (isMaximisingPlayer)
            {
                var bestMove = int.MinValue + 1;
                for (var i = 0; i < newGameMoves.Count; i++)
                {
                    Move move = newGameMoves[i];

                    //  DO MOVE
                    // if there isn't any balls left, a ball is moved
                    bool newBall = false;
                    if (!IsBallsLeft(CellColor.Black))
                    {
                        // remove ball from "FROM" cell
                        grid.cells[move.fromX][move.fromY] = CellColor.None;
                    }
                    // if there are some balls left, a ball is added to the board
                    else
                    {
                        newBall = true;
                        SubBall(CellColor.Black);
                    }

                    // add ball to "TO" cell
                    grid.cells[move.toX][move.toY] = CellColor.Black;


                    bestMove = -EvaluateGrid(depth);

                    if (!IsGameEnded(bestMove))
                    {
                        // get loop on all moves available on board with this move
                        bestMove = Mathf.Max(bestMove, MiniMax(depth - 1, alpha, beta, !isMaximisingPlayer));
                    }

                    grid.cells[move.toX][move.toY] = CellColor.None;

                    // UNDO MOVE
                    // if there isn't any balls left, a ball is moved
                    if (!newBall)
                    {
                        // remove ball from "FROM" cell
                        grid.cells[move.fromX][move.fromY] = CellColor.Black;
                    }
                    // if there are some balls left, a ball is added to the board
                    else
                    {
                        AddBall(CellColor.Black);
                    }

                    alpha = Mathf.Max(alpha, bestMove);

                    if (beta <= alpha) {
                        return bestMove;
                    }
                }
                return bestMove;
            }
            // Joueur
            else
            {
                var bestMove = int.MaxValue - 1;
                for (var i = 0; i < newGameMoves.Count; i++)
                {
                    Move move = newGameMoves[i];

                    //  DO MOVE
                    // if there isn't any balls left, a ball is moved
                    bool newBall = false;
                    if (!IsBallsLeft(CellColor.Black))
                    {
                        // remove ball from "FROM" cell
                        grid.cells[move.fromX][move.fromY] = CellColor.None;
                    }
                    // if there are some balls left, a ball is added to the board
                    else
                    {
                        newBall = true;
                        SubBall(CellColor.Black);
                    }

                    // add ball to "TO" cell
                    grid.cells[move.toX][move.toY] = CellColor.Black;

                    bestMove = -EvaluateGrid(depth);

                    if (!IsGameEnded(bestMove))
                    {
                        // get loop on all moves available on board with this move
                        bestMove = Mathf.Min(bestMove, MiniMax(depth - 1, alpha, beta, !isMaximisingPlayer));
                    }

                    grid.cells[move.toX][move.toY] = CellColor.None;

                    // UNDO MOVE
                    // if there isn't any balls left, a ball is moved
                    if (!newBall)
                    {
                        // remove ball from "FROM" cell
                        grid.cells[move.fromX][move.fromY] = CellColor.Black;
                    }
                    // if there are some balls left, a ball is added to the board
                    else
                    {
                        AddBall(CellColor.Black);
                    }

                    beta = Mathf.Min(beta, bestMove);

                    if (beta <= alpha) {
                        return bestMove;
                    }
                }
                return bestMove;
            }
        }
        */

        public int BallsLeft(CellColor color)
        {
            if (color == CellColor.Black)
                return grid.blackBallsLeft;
            else
                return grid.whiteBallsLeft;
        }

        public bool IsBallsLeft(CellColor color)
        {
            if ( ( color == CellColor.Black && grid.blackBallsLeft > 0 ) || ( color == CellColor.White && grid.whiteBallsLeft > 0 ) )
                return true;

            return false;
        }

        public void AddBall(CellColor color)
        {
            if (color == CellColor.Black)
                grid.blackBallsLeft++;
            else
                grid.whiteBallsLeft++;
        }

        public void SubBall(CellColor color)
        {
            if (color == CellColor.Black)
                grid.blackBallsLeft++;
            else
                grid.whiteBallsLeft++;
        }

        public bool IsGameEnded(int score)
        {
            if (score > 9000000 || score < -9000000)
                return true;

            return false;
        }
    }

}