using System;
using System.Collections.Generic;
using UnityEngine;

namespace AppAdvisory.Item
{

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

    public enum PatternType : byte
    {
        AALine,
        DiagonalLine,
        AACross,
        DiagonalCross
    }

    public class OptimizedGrid
    {
        private CellColor[][] cells;
        public CellColor[][] Cells { get { return cells; } }

        private int blackBallsLeft;
        public int BlackBallsLeft { get { return blackBallsLeft; } }

        private int whiteBallsLeft;
        public int WhiteBallsLeft { get { return whiteBallsLeft; } }

        private AIEvaluationData patternData;
        public AIEvaluationData PatternData { get { return patternData; } }

        private int width;
        private int height;

        public OptimizedGrid(int _width, int _height)
        {
            width = _width;
            height = _height;

            blackBallsLeft = 10;
            whiteBallsLeft = 10;

            CreateEmptyOptimizedGrid();
        }

        private void CreateEmptyOptimizedGrid()
        {
            cells = new CellColor[height][];
            bool isSmallRow = false;
            
            for (int i = 0; i < height; ++i)
            {
                int actualWidth = width;
                if (isSmallRow)
                    actualWidth = width - 1;

                cells[i] = new CellColor[actualWidth];

                for (int j = 0; j < actualWidth; ++j)
                {
                    if (i == 4 && j == 2)
                    {
                        cells[i][j] = CellColor.NOT_A_CELL;
                        continue;
                    }

                    cells[i][j] = CellColor.None;
                }

                isSmallRow = !isSmallRow;
            }
        }

        public void SetPatternData(AIEvaluationData data)
        {
            patternData = data;
        }

        /*private void GetOptimizedGrid(ModelGrid model)
        {
            blackBallsLeft = manager.blackBalls.Count;
            whiteBallsLeft = manager.whiteBalls.Count;

            for (int i = 0; i < model.grid.Length; ++i)
            {
                for (int j = 0; j < model.grid[i].Length; ++j)
                {
                    if (!model.grid[i][j])
                    {
                        model.cells[i][j] = CellColor.NOT_A_CELL;
                        continue;
                    }

                    if (model.grid[i][j].HasBall())
                    {
                        model.cells[i][j] = (CellColor)model.grid[i][j].ball.color;
                    }
                    else
                    {
                        model.cells[i][j] = CellColor.None;
                    }
                }
            }
        }*/

        public List<OptimizedCell> GetCellNeighbours(OptimizedCell cell)
        {
            List<OptimizedCell> neighbours = new List<OptimizedCell>();

            OptimizedCell neighbour = new OptimizedCell();

            //middle up
            neighbour = GetCell(cell.x + 2, cell.y);

            if (neighbour != null)
                neighbours.Add(neighbour);

            //middle down
            neighbour = GetCell(cell.x - 2, cell.y);

            if (neighbour != null)
                neighbours.Add(neighbour);

            //middle left
            neighbour = GetCell(cell.x, cell.y - 1);

            if (neighbour != null)
                neighbours.Add(neighbour);

            //middle right
            neighbour = GetCell(cell.x, cell.y + 1);
            if (neighbour != null)
                neighbours.Add(neighbour);

            int offset = cell.x % 2;

            //top left
            neighbour = GetCell(cell.x + 1, cell.y - 1 + offset);
            if (neighbour != null)
                neighbours.Add(neighbour);

            //top right
            neighbour = GetCell(cell.x + 1, cell.y + offset);
            if (neighbour != null)
                neighbours.Add(neighbour);

            //bottom left
            neighbour = GetCell(cell.x - 1, cell.y - 1 + offset);
            if (neighbour != null)
                neighbours.Add(neighbour);

            //bottom right
            neighbour = GetCell(cell.x - 1, cell.y + offset);
            if (neighbour != null)
                neighbours.Add(neighbour);

            return neighbours;
        }

        public OptimizedCell GetCell(int x, int y)
        {
            if (y < 0 || y >= GetActualWidth(y) || x < 0 || x >= height || cells[x][y] == CellColor.NOT_A_CELL)
                return null;

            return new OptimizedCell(x, y, cells[x][y]);
        }

        public OptimizedCell GetCell(Vector2 pos)
        {
            return GetCell((int)pos.x, (int)pos.y);
        }

        // returns if this move was adding a new ball to the board
        public void DoMove(Move move)
        {
            // if it was a new ball, sub one from the list
            if (move.IsNewBall())
            {
                SubBall(move.color);
            }
            else // if it wasn't, remove it from its original cell
            {
                // remove ball from "FROM" cell
                cells[move.fromX][move.fromY] = CellColor.None;
            }

            // add ball to "TO" cell
            cells[move.toX][move.toY] = move.color;
        }

        public void UndoMove(Move move)
        {
            cells[move.toX][move.toY] = CellColor.None;

            // if it was a new ball, add it back to the list
            if (move.IsNewBall())
            {
                AddBall(move.color);
            }
            // if it wasn't, put it back on its cell
            else
            {
                cells[move.fromX][move.fromY] = move.color;
            }
        }

        public bool GetWinningPattern(out List<Vector2> winningCells)
        {
            winningCells = new List<Vector2>();

            bool isWinning = FindWinningPattern(patternData.horizontalLinePatterns, out winningCells);
            if (isWinning)
            {
                return true;
            }

            isWinning = FindWinningPattern(patternData.verticalLinePatterns, out winningCells);
            if (isWinning)
            {
                return true;
            }

            isWinning = FindWinningPattern(patternData.diagonalLinePatterns, out winningCells);
            if (isWinning)
            {
                return true;
            }

            isWinning = FindWinningPattern(patternData.otherDiagonalLinePatterns, out winningCells);
            if (isWinning)
            {
                return true;
            }

            isWinning = FindWinningPattern(patternData.horizontalCrossPatterns, out winningCells);
            if (isWinning)
            {
                return true;
            }

            isWinning = FindWinningPattern(patternData.diagonalCrossPatterns, out winningCells);
            if (isWinning)
            {
                return true;
            }

            return false;
        }

        public bool GetWinningPatternAndType(out List<Vector2> winningCells, out PatternType patternType)
        {
            winningCells = new List<Vector2>();
            patternType = PatternType.AALine;

            bool isWinning = FindWinningPattern(patternData.horizontalLinePatterns, out winningCells);
            if (isWinning)
            {
                patternType = PatternType.AALine;
                return true;
            }

            isWinning = FindWinningPattern(patternData.verticalLinePatterns, out winningCells);
            if (isWinning)
            {
                patternType = PatternType.AALine;
                return true;
            }

            isWinning = FindWinningPattern(patternData.diagonalLinePatterns, out winningCells);
            if (isWinning)
            {
                patternType = PatternType.DiagonalLine;
                return true;
            }

            isWinning = FindWinningPattern(patternData.otherDiagonalLinePatterns, out winningCells);
            if (isWinning)
            {
                patternType = PatternType.DiagonalLine;
                return true;
            }

            isWinning = FindWinningPattern(patternData.horizontalCrossPatterns, out winningCells);
            if (isWinning)
            {
                patternType = PatternType.AACross;
                return true;
            }

            isWinning = FindWinningPattern(patternData.diagonalCrossPatterns, out winningCells);
            if (isWinning)
            {
                patternType = PatternType.DiagonalCross;
                return true;
            }

            return false;
        }

        private bool FindWinningPattern(EvaluationPattern[] patterns, out List<Vector2> winningCells)
        {
            int blackCount = 0;
            int whiteCount = 0;
            winningCells = new List<Vector2>();

            for (int i = 0; i < patterns.Length; ++i)
            {
                blackCount = 0;
                whiteCount = 0;

                for (int j = 0; j < patterns[i].positions.Length; ++j)
                {
                    IntVec2 pos = patterns[i].positions[j];
                    if (cells[pos.X][pos.Y] == CellColor.Black)
                    {
                        blackCount++;
                    }
                    else if (cells[pos.X][pos.Y] == CellColor.White)
                    {
                        whiteCount++;
                    }
                }

                if (blackCount == 5 || whiteCount == 5)
                {
                    winningCells.Add(new Vector2(patterns[i].positions[0].X, patterns[i].positions[0].Y));
                    winningCells.Add(new Vector2(patterns[i].positions[1].X, patterns[i].positions[1].Y));
                    winningCells.Add(new Vector2(patterns[i].positions[2].X, patterns[i].positions[2].Y));
                    winningCells.Add(new Vector2(patterns[i].positions[3].X, patterns[i].positions[3].Y));
                    winningCells.Add(new Vector2(patterns[i].positions[4].X, patterns[i].positions[4].Y));

                    return true;
                }
            }

            return false;
        }

        private bool FindCellPattern(EvaluationPattern[] patterns, out List<Vector2> cellsPattern, int blackNeeded, int whiteNeeded, Func<int, int, int, int, bool> condition)
        {
            int blackCount = 0;
            int whiteCount = 0;
            cellsPattern = new List<Vector2>();

            for (int i = 0; i < patterns.Length; ++i)
            {
                blackCount = 0;
                whiteCount = 0;

                for (int j = 0; j < patterns[i].positions.Length; ++j)
                {
                    IntVec2 pos = patterns[i].positions[j];
                    if (cells[pos.X][pos.Y] == CellColor.Black)
                    {
                        blackCount++;
                    }
                    else if (cells[pos.X][pos.Y] == CellColor.White)
                    {
                        whiteCount++;
                    }
                }

                if (condition(blackCount, blackNeeded, whiteCount, whiteNeeded))
                {
                    cellsPattern.Add(new Vector2(patterns[i].positions[0].X, patterns[i].positions[0].Y));
                    cellsPattern.Add(new Vector2(patterns[i].positions[1].X, patterns[i].positions[1].Y));
                    cellsPattern.Add(new Vector2(patterns[i].positions[2].X, patterns[i].positions[2].Y));
                    cellsPattern.Add(new Vector2(patterns[i].positions[3].X, patterns[i].positions[3].Y));
                    cellsPattern.Add(new Vector2(patterns[i].positions[4].X, patterns[i].positions[4].Y));
                    
                    return true;
                }
            }

            return false;
        }

        public bool CanColorWin(BallColor color, out List<Vector2> cells)
        {
            if (FindCellPattern(patternData.horizontalLinePatterns, out cells, 4, 0, CompareBalls))
            {
                return true;
            }

            if (FindCellPattern(patternData.verticalLinePatterns, out cells, 4, 0, CompareBalls))
            {
                return true;
            }

            if (FindCellPattern(patternData.diagonalLinePatterns, out cells, 4, 0, CompareBalls))
            {
                return true;
            }

            if (FindCellPattern(patternData.otherDiagonalLinePatterns, out cells, 4, 0, CompareBalls))
            {
                return true;
            }

            if (FindCellPattern(patternData.horizontalCrossPatterns, out cells, 4, 0, CompareBalls))
            {
                return true;
            }

            if (FindCellPattern(patternData.diagonalCrossPatterns, out cells, 4, 0, CompareBalls))
            {
                return true;
            }

            return false;
        }

        public bool CanColorMoveToWin(BallColor color, List<Vector2> patternCells, out Move bestMove)
        {
            bestMove = new Move();

            List<Move> potentialMove = new List<Move>();
            List<Move> moves = GetAvailableMoves((CellColor)color);

            Vector2 emptyCell = new Vector2();

            foreach(Vector2 cell in patternCells)
            {
                if (cells[(int)cell.x][(int)cell.y] == CellColor.None)
                    emptyCell = cell;
            }

            foreach (Move move in moves)
            {
                bool skipMove = false;
                foreach(Vector2 cell in patternCells)
                {
                    if ( !(move.toX == emptyCell.x && move.toY == emptyCell.y) || (move.fromX == cell.x && move.fromY == cell.y) )
                    {
                        skipMove = true;
                        break;
                    }
                }

                if (skipMove)
                    continue;

                potentialMove.Add(move);
            }

            if (potentialMove.Count > 0)
            {
                bestMove = potentialMove[UnityEngine.Random.Range(0, potentialMove.Count - 1)];
                return true;
            }
            
            return false;
        }

        private bool CompareBalls(int blackCount, int blackNeeded, int whiteCount, int whiteNeeded)
        {
            if (blackCount == blackNeeded && whiteCount == whiteNeeded)
            {
                return true;
            }

            return false;
        }

        /*public BallColor IsSomeoneWinning()
        {
            Move move = new Move();
            return IsSomeoneWinning(out move);
        }

        public BallColor IsSomeoneWinning(out Move move)
        {
            move = new Move();
            
            return BallColor.Black;
        }*/

        private int GetActualWidth(int y)
        {
            return width - y % 2 - 1;
        }

        public List<Move> GetAvailableMoves(CellColor color)
        {
            List<Move> moves = new List<Move>();

            int ballsLeft, i, j = 0;

            if (color == CellColor.Black)
                ballsLeft = blackBallsLeft;
            else
                ballsLeft = whiteBallsLeft;

            if (ballsLeft == 0)
            {
                for (i = 0; i < Cells.Length; ++i)
                {
                    for (j = 0; j < Cells[i].Length; ++j)
                    {
                        if (Cells[i][j] == color)
                        {
                            OptimizedCell actualCell = new OptimizedCell();
                            actualCell.x = i;
                            actualCell.y = j;
                            actualCell.color = color;

                            List<OptimizedCell> cells = GetCellNeighbours(actualCell);

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
                for (i = 0; i < cells.Length; ++i)
                {
                    for (j = 0; j < cells[i].Length; ++j)
                    {
                        if (cells[i][j] == CellColor.None)
                        {
                            Move move = new Move();
                            move.fromX = -1;
                            move.fromY = -1;
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

        public void AddBall(CellColor color)
        {
            if (color == CellColor.Black)
                blackBallsLeft++;
            else
                whiteBallsLeft++;
        }

        public void SubBall(CellColor color)
        {
            if (color == CellColor.Black)
                blackBallsLeft--;
            else
                whiteBallsLeft--;
        }

        public int BallsLeft(CellColor color)
        {
            if (color == CellColor.Black)
                return BlackBallsLeft;
            else
                return WhiteBallsLeft;
        }

        public bool IsBallsLeft(CellColor color)
        {
            if ((color == CellColor.Black && BlackBallsLeft > 0) || (color == CellColor.White && WhiteBallsLeft > 0))
                return true;

            return false;
        }


    }
}