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
        HorizontalLine,
        VerticalLine,
        DescDiagonalLine,
        AscDiagonalLine,
        HorizontalCross,
        DiagonalCross
    }

    public class WinningPattern
    {
        public Vector2[] cells;
        public PatternType type;
        public CellColor color;

        public WinningPattern() { }

        public WinningPattern(WinningPattern toCopy)
        {
            cells = new Vector2[5];

            for (int i = 0; i < cells.Length; ++i)
            {
                cells[i] = toCopy.cells[i];
            }

            type = toCopy.type;
            color = toCopy.color;
        }

        public WinningPattern(Vector2[] _cells, PatternType _type, CellColor _color)
        {
            cells = _cells;
            type = _type;
            color = _color;
        }

        public bool IsSame(WinningPattern pattern)
        {
            if (pattern.color != color)
                return false;

            for (int i = 0; i < cells.Length; ++i)
            {
                if (cells[i].x != pattern.cells[i].x || cells[i].y != pattern.cells[i].y)
                {
                    return false;
                }
            }
            
            if (cells.Length == 0)
            {
                Debug.LogError("IsSame with cell 0");
            }

            return true;
        }
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

        public void Reset()
        {
            blackBallsLeft = 10;
            whiteBallsLeft = 10;

            CreateEmptyOptimizedGrid();
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
            if (y < 0 || y > GetActualWidth(x) || x < 0 || x >= height || cells[x][y] == CellColor.NOT_A_CELL)
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

        public bool HasColorWon(CellColor color)
        {
            List<WinningPattern> patterns = new List<WinningPattern>();
            GetWinningPatterns(out patterns);

            foreach(WinningPattern pattern in patterns)
            {
                if (pattern.color == color)
                    return true;
            }

            return false;
        }

        public bool GetWinningPatterns(out List<WinningPattern> winningPatterns)
        {
            winningPatterns = new List<WinningPattern>();

            bool isWinning = false;
            WinningPattern winningPattern = new WinningPattern();
            while (FindWinningPattern(patternData.horizontalLinePatterns, out winningPattern, winningPatterns))
            {
                WinningPattern winny = new WinningPattern(winningPattern);
                winny.type = PatternType.HorizontalLine;
                winningPatterns.Add(winny);
                isWinning = true;
            }

            while (FindWinningPattern(patternData.verticalLinePatterns, out winningPattern, winningPatterns))
            {
                WinningPattern winny = new WinningPattern(winningPattern);
                winny.type = PatternType.VerticalLine;
                winningPatterns.Add(winny);
                isWinning = true;
            }

            while (FindWinningPattern(patternData.diagonalLinePatterns, out winningPattern, winningPatterns))
            {
                WinningPattern winny = new WinningPattern(winningPattern);
                winny.type = PatternType.AscDiagonalLine;
                winningPatterns.Add(winny);
                isWinning = true;
            }

            while (FindWinningPattern(patternData.otherDiagonalLinePatterns, out winningPattern, winningPatterns))
            {
                WinningPattern winny = new WinningPattern(winningPattern);
                winny.type = PatternType.DescDiagonalLine;
                winningPatterns.Add(winny);
                isWinning = true;
            }

            while (FindWinningPattern(patternData.horizontalCrossPatterns, out winningPattern, winningPatterns))
            {
                WinningPattern winny = new WinningPattern(winningPattern);
                winny.type = PatternType.HorizontalCross;
                winningPatterns.Add(winny);
                isWinning = true;
            }

            while (FindWinningPattern(patternData.diagonalCrossPatterns, out winningPattern, winningPatterns))
            {
                WinningPattern winny = new WinningPattern(winningPattern);
                winny.type = PatternType.DiagonalCross;
                winningPatterns.Add(winny);
                isWinning = true;
            }

            return isWinning ;
        }

        private bool FindWinningPattern(EvaluationPattern[] patterns, out WinningPattern winningPattern, List<WinningPattern> toExclude = null)
        {
            int blackCount = 0;
            int whiteCount = 0;
            winningPattern = new WinningPattern();

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
                    Vector2[] cells = new Vector2[5];
                    cells[0] = new Vector2(patterns[i].positions[0].X, patterns[i].positions[0].Y);
                    cells[1] = new Vector2(patterns[i].positions[1].X, patterns[i].positions[1].Y);
                    cells[2] = new Vector2(patterns[i].positions[2].X, patterns[i].positions[2].Y);
                    cells[3] = new Vector2(patterns[i].positions[3].X, patterns[i].positions[3].Y);
                    cells[4] = new Vector2(patterns[i].positions[4].X, patterns[i].positions[4].Y);

                    bool alreadyFound = false;
                    foreach(WinningPattern patternToExclude in toExclude)
                    {
                        if (cells[0] == patternToExclude.cells[0] &&
                            cells[1] == patternToExclude.cells[1] &&
                            cells[2] == patternToExclude.cells[2] &&
                            cells[3] == patternToExclude.cells[3] &&
                            cells[4] == patternToExclude.cells[4])
                        {
                            alreadyFound = true;
                            break;
                        }
                    }

                    if (alreadyFound)
                        continue;

                    winningPattern.cells = cells;
                    winningPattern.color = (blackCount == 5) ? CellColor.Black : CellColor.White;
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
            int whiteNeeded = 0;
            int blackNeeded = 0;
            if (color == BallColor.Black)
                blackNeeded = 4;
            else
                whiteNeeded = 4;

            if (FindCellPattern(patternData.horizontalLinePatterns, out cells, blackNeeded, whiteNeeded, CompareBalls))
            {
                Debug.Log("horizontalline 4-0 pattern");
                return true;
            }

            if (FindCellPattern(patternData.verticalLinePatterns, out cells, blackNeeded, whiteNeeded, CompareBalls))
            {
                Debug.Log("verticalLinePatterns 4-0 pattern");
                return true;
            }

            if (FindCellPattern(patternData.diagonalLinePatterns, out cells, blackNeeded, whiteNeeded, CompareBalls))
            {
                Debug.Log("diagonalLinePatterns 4-0 pattern");
                return true;
            }

            if (FindCellPattern(patternData.otherDiagonalLinePatterns, out cells, blackNeeded, whiteNeeded, CompareBalls))
            {
                Debug.Log("otherDiagonalLinePatterns 4-0 pattern");
                return true;
            }

            if (FindCellPattern(patternData.horizontalCrossPatterns, out cells, blackNeeded, whiteNeeded, CompareBalls))
            {
                Debug.Log("horizontalCrossPatterns 4-0 pattern");
                return true;
            }

            if (FindCellPattern(patternData.diagonalCrossPatterns, out cells, blackNeeded, whiteNeeded, CompareBalls))
            {
                Debug.Log("diagonalCrossPatterns 4-0 pattern");
                return true;
            }

            Debug.Log("No 4-0 pattern");

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
        
        private int GetActualWidth(int x)
        {
            return width - x % 2 - 1;
        }
        public OptimizedCell GetCellFromDirection(OptimizedCell firstCell, OptimizedCell secondCell)
        {
            OptimizedCell destinationCell;

            //middle up
            if (firstCell.y == secondCell.y && (firstCell.x + 2) == secondCell.x)
            {
                destinationCell = GetCell(secondCell.x + 2, secondCell.y);
                if (destinationCell != null)
                    return destinationCell;
            }

            //middle down
            if (firstCell.y == secondCell.y && (firstCell.x - 2) == secondCell.x)
            {
                destinationCell = GetCell(secondCell.x - 2, secondCell.y);
                if (destinationCell != null)
                    return destinationCell;
            }

            //middle left
            if (firstCell.y - 1 == secondCell.y && firstCell.x == secondCell.x)
            {
                destinationCell = GetCell(secondCell.x, secondCell.y - 1);
                if (destinationCell != null)
                    return destinationCell;
            }

            //middle right
            if (firstCell.y + 1 == secondCell.y && firstCell.x == secondCell.x)
            {
                destinationCell = GetCell(secondCell.x, secondCell.y + 1);
                if (destinationCell != null)
                    return destinationCell;
            }


            int offsetFirstCell = firstCell.x % 2;
            int offsetSecondCell = secondCell.x % 2;


            //top left
            if ((firstCell.y - 1 + offsetFirstCell) == secondCell.y && (firstCell.x + 1) == secondCell.x)
            {
                destinationCell = GetCell(secondCell.x + 1, secondCell.y - 1 + offsetSecondCell);
                if (destinationCell != null)
                    return destinationCell;
            }

            //top right
            if ((firstCell.y + offsetFirstCell) == secondCell.y && (firstCell.x + 1) == secondCell.x)
            {
                destinationCell = GetCell(secondCell.x + 1, secondCell.y + offsetSecondCell);
                if (destinationCell != null)
                    return destinationCell;
            }

            //bottom left
            if ((firstCell.y - 1 + offsetFirstCell) == secondCell.y && (firstCell.x - 1) == secondCell.x)
            {
                destinationCell = GetCell(secondCell.x - 1, secondCell.y - 1 + offsetSecondCell);
                if (destinationCell != null)
                    return destinationCell;
            }

            //bottom right
            if ((firstCell.y + offsetFirstCell) == secondCell.y && (firstCell.x - 1) == secondCell.x)
            {
                destinationCell = GetCell(secondCell.x - 1, secondCell.y + offsetSecondCell);
                if (destinationCell != null)
                    return destinationCell;
            }

            return null;
        }


        public List<Move> GetAvailableMoves(Cell cell, bool canOnlyJump = false)
        {
            List<Move> moves = new List<Move>();

            List<Move> potentialMoves = new List<Move>();
            if (canOnlyJump)
                potentialMoves = GetAvailableMoves((CellColor)cell.ball.Color, true, true);
            else
                potentialMoves = GetAvailableMoves((CellColor)cell.ball.Color);

            foreach (Move move in potentialMoves)
            {
                if (move.fromX == cell.y && move.fromY == cell.x)
                {
                    moves.Add(move);
                }
            }

            return moves;
        }

        public List<Move> GetAvailableMoves(CellColor color, bool enableJump = true, bool canOnlyJump = false)
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
                                    if (canOnlyJump)
                                        continue;

                                    Move move = new Move();
                                    move.fromX = actualCell.x;
                                    move.fromY = actualCell.y;
                                    move.toX = cell.x;
                                    move.toY = cell.y;
                                    move.color = actualCell.color;

                                    moves.Add(move);
                                }
                                else if (cell.color != CellColor.NOT_A_CELL && enableJump)
                                {
                                    OptimizedCell potentialCell = GetCellFromDirection(actualCell, cell);

                                    if (potentialCell != null && potentialCell.color == CellColor.None)
                                    {
                                        Move move = new Move();
                                        move.fromX = actualCell.x;
                                        move.fromY = actualCell.y;
                                        move.toX = potentialCell.x;
                                        move.toY = potentialCell.y;
                                        move.color = actualCell.color;

                                        moves.Add(move);
                                    }
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