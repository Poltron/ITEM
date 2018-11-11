﻿using System;
using System.Collections.Generic;
using UnityEngine;

public enum CellColor : byte
{
    None = 0,
    White = 1,
    Black = 2,
    NOT_A_CELL = 3
}

public class OptiBall
{
    public CellColor color;
    public bool isPoint;

    public OptiBall()
    { }

    public OptiBall(bool _isPoint, CellColor _color)
    {
        isPoint = _isPoint;
        color = _color;
    }
}

public class OptimizedCellInternal
{
    public CellColor color;
    public bool isPoint;

    public OptimizedCellInternal()
    { }

    public OptimizedCellInternal(bool _isPoint, CellColor _color)
    {
        isPoint = _isPoint;
        color = _color;
    }
}

public class OptimizedCell
{
    public int x;
    public int y;
    public CellColor color;
    public bool isPoint;

    public OptimizedCell()
    { }

    public OptimizedCell(int _x, int _y, CellColor _color, bool _isPoint)
    {
        x = _x;
        y = _y;
        color = _color;
        isPoint = _isPoint;
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
            //Debug.LogError("IsSame with cell 0");
        }

        return true;
    }

    public int GetScore(ModelGrid grid)
    {
        int score = 0;

        score += grid.GetCellFromModel((int)cells[0].y, (int)cells[0].x).ball.Score;
        score += grid.GetCellFromModel((int)cells[1].y, (int)cells[1].x).ball.Score;
        score += grid.GetCellFromModel((int)cells[2].y, (int)cells[2].x).ball.Score;
        score += grid.GetCellFromModel((int)cells[3].y, (int)cells[3].x).ball.Score;
        score += grid.GetCellFromModel((int)cells[4].y, (int)cells[4].x).ball.Score;

        return score;
    }
}

public class OptimizedGrid
{
    private OptimizedCellInternal[][] cells;
    public OptimizedCellInternal[][] Cells { get { return cells; } }

    private List<OptiBall> blackBallsLeft;
    public List<OptiBall> BlackBallsLeft { get { return blackBallsLeft; } }

    private List<OptiBall> whiteBallsLeft;
    public List<OptiBall> WhiteBallsLeft { get { return whiteBallsLeft; } }

    private AIEvaluationData patternData;
    public AIEvaluationData PatternData { get { return patternData; } }

    private int width;
    private int height;

    public OptimizedGrid(int _width, int _height)
    {
        width = _width;
        height = _height;

        CreateBallsList();
        CreateEmptyOptimizedGrid();
    }

    private void CreateBallsList()
    {
        blackBallsLeft = new List<OptiBall>();
        for (int i = 0;i < 10;i++)
        {
            blackBallsLeft.Add(new OptiBall( ((i % 2) == 0) ? false : true, CellColor.Black));
        }

        whiteBallsLeft = new List<OptiBall>();
        for (int i = 0; i < 10; i++)
        {
            whiteBallsLeft.Add(new OptiBall( ((i % 2) == 0) ? false : true, CellColor.White));
        }
    }

    private void CreateEmptyOptimizedGrid()
    {
        cells = new OptimizedCellInternal[height][];
        bool isSmallRow = false;

        for (int i = 0; i < height; ++i)
        {
            int actualWidth = width;
            if (isSmallRow)
                actualWidth = width - 1;

            cells[i] = new OptimizedCellInternal[actualWidth];

            for (int j = 0; j < actualWidth; ++j)
            {
                if (i == 4 && j == 2)
                {
                    cells[i][j] = new OptimizedCellInternal(false, CellColor.NOT_A_CELL);
                    continue;
                }

                cells[i][j] = new OptimizedCellInternal(false, CellColor.None);
            }

            isSmallRow = !isSmallRow;
        }
    }

    public void Reset()
    {
        CreateBallsList();
        CreateEmptyOptimizedGrid();
    }

    public void SetPatternData(AIEvaluationData data)
    {
        patternData = data;
    }

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
        if (y < 0 || y > GetActualWidth(x) || x < 0 || x >= height || cells[x][y].color == CellColor.NOT_A_CELL)
            return null;

        return new OptimizedCell(x, y, cells[x][y].color, cells[x][y].isPoint);
    }

    public OptimizedCell GetCell(Vector2 pos)
    {
        return GetCell((int)pos.x, (int)pos.y);
    }

    public void DoMove(Move move)
    {
        // if it was a new ball, sub one from the list
        if (move.IsNewBall())
        {
            cells[move.toX][move.toY].color = move.color;
            // we do not consider points on new ball
            
            move.isPoint = SubBall(move.color).isPoint;
        }
        else // if it wasn't, remove it from its original cell
        {
            // add ball to "TO" cell
            cells[move.toX][move.toY].color = cells[move.fromX][move.fromY].color;
            cells[move.toX][move.toY].isPoint = cells[move.fromX][move.fromY].isPoint;
            // remove ball from "FROM" cell
            cells[move.fromX][move.fromY].color = CellColor.None;
            cells[move.fromX][move.fromY].isPoint = false;
        }
    }

    public void UndoMove(Move move)
    {
        // if it was a new ball, add it back to the list
        if (move.IsNewBall())
        {
            AddBall(move.color, move.isPoint);
        }
        // if it wasn't, put it back on its cell
        else
        {
            cells[move.fromX][move.fromY].color = cells[move.toX][move.toY].color;
            cells[move.fromX][move.fromY].isPoint = cells[move.toX][move.toY].isPoint;
        }

        cells[move.toX][move.toY].color = CellColor.None;
        cells[move.toX][move.toY].isPoint = false;

    }

    public bool HasColorWon(CellColor color)
    {
        List<WinningPattern> patterns = new List<WinningPattern>();
        GetWinningPatterns(out patterns);

        foreach (WinningPattern pattern in patterns)
        {
            if (pattern.color == color)
                return true;
        }

        return false;
    }

    public bool GetWinningPatterns(out List<WinningPattern> winningPatternsToExclude)
    {
        winningPatternsToExclude = new List<WinningPattern>();

        bool isWinning = false;
        WinningPattern winningPattern = new WinningPattern();
        while (FindWinningPattern(patternData.horizontalLinePatterns, out winningPattern, winningPatternsToExclude))
        {
            WinningPattern winny = new WinningPattern(winningPattern);
            winny.type = PatternType.HorizontalLine;
            winningPatternsToExclude.Add(winny);
            isWinning = true;
        }

        while (FindWinningPattern(patternData.verticalLinePatterns, out winningPattern, winningPatternsToExclude))
        {
            WinningPattern winny = new WinningPattern(winningPattern);
            winny.type = PatternType.VerticalLine;
            winningPatternsToExclude.Add(winny);
            isWinning = true;
        }

        while (FindWinningPattern(patternData.diagonalLinePatterns, out winningPattern, winningPatternsToExclude))
        {
            WinningPattern winny = new WinningPattern(winningPattern);
            winny.type = PatternType.AscDiagonalLine;
            winningPatternsToExclude.Add(winny);
            isWinning = true;
        }

        while (FindWinningPattern(patternData.otherDiagonalLinePatterns, out winningPattern, winningPatternsToExclude))
        {
            WinningPattern winny = new WinningPattern(winningPattern);
            winny.type = PatternType.DescDiagonalLine;
            winningPatternsToExclude.Add(winny);
            isWinning = true;
        }

        while (FindWinningPattern(patternData.horizontalCrossPatterns, out winningPattern, winningPatternsToExclude))
        {
            WinningPattern winny = new WinningPattern(winningPattern);
            winny.type = PatternType.HorizontalCross;
            winningPatternsToExclude.Add(winny);
            isWinning = true;
        }

        while (FindWinningPattern(patternData.diagonalCrossPatterns, out winningPattern, winningPatternsToExclude))
        {
            WinningPattern winny = new WinningPattern(winningPattern);
            winny.type = PatternType.DiagonalCross;
            winningPatternsToExclude.Add(winny);
            isWinning = true;
        }

        return isWinning;
    }

    private bool FindWinningPattern(EvaluationPattern[] patterns, out WinningPattern winningPattern, List<WinningPattern> toExclude = null)
    {
        int aiPawnCount = 0;
        int opponentPawnCount = 0;
        winningPattern = new WinningPattern();

        for (int i = 0; i < patterns.Length; ++i)
        {
            aiPawnCount = 0;
            opponentPawnCount = 0;

            for (int j = 0; j < patterns[i].positions.Length; ++j)
            {
                IntVec2 pos = patterns[i].positions[j];
                if (cells[pos.X][pos.Y].color == CellColor.Black)
                {
                    aiPawnCount++;
                }
                else if (cells[pos.X][pos.Y].color == CellColor.White)
                {
                    opponentPawnCount++;
                }
            }

            if (aiPawnCount == 5 || opponentPawnCount == 5)
            {
                Vector2[] cells = new Vector2[5];
                cells[0] = new Vector2(patterns[i].positions[0].X, patterns[i].positions[0].Y);
                cells[1] = new Vector2(patterns[i].positions[1].X, patterns[i].positions[1].Y);
                cells[2] = new Vector2(patterns[i].positions[2].X, patterns[i].positions[2].Y);
                cells[3] = new Vector2(patterns[i].positions[3].X, patterns[i].positions[3].Y);
                cells[4] = new Vector2(patterns[i].positions[4].X, patterns[i].positions[4].Y);

                bool alreadyFound = false;
                foreach (WinningPattern patternToExclude in toExclude)
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
                winningPattern.color = (aiPawnCount == 5) ? CellColor.Black : CellColor.White;
                return true;
            }
        }

        return false;
    }

    private bool FindCellPattern(EvaluationPattern[] patterns, out List<Vector2> cellsPattern, int blackNeeded, int whiteNeeded, Func<int, int, int, int, bool> condition)
    {
        int aiPawnCount = 0;
        int opponentPawnCount = 0;
        cellsPattern = new List<Vector2>();

        for (int i = 0; i < patterns.Length; ++i)
        {
            aiPawnCount = 0;
            opponentPawnCount = 0;

            for (int j = 0; j < patterns[i].positions.Length; ++j)
            {
                IntVec2 pos = patterns[i].positions[j];
                if (cells[pos.X][pos.Y].color == CellColor.Black)
                {
                    aiPawnCount++;
                }
                else if (cells[pos.X][pos.Y].color == CellColor.White)
                {
                    opponentPawnCount++;
                }
            }

            if (condition(aiPawnCount, blackNeeded, opponentPawnCount, whiteNeeded))
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
            //Debug.Log("horizontalline 4-0 pattern");
            return true;
        }

        if (FindCellPattern(patternData.verticalLinePatterns, out cells, blackNeeded, whiteNeeded, CompareBalls))
        {
            //Debug.Log("verticalLinePatterns 4-0 pattern");
            return true;
        }

        if (FindCellPattern(patternData.diagonalLinePatterns, out cells, blackNeeded, whiteNeeded, CompareBalls))
        {
            //Debug.Log("diagonalLinePatterns 4-0 pattern");
            return true;
        }

        if (FindCellPattern(patternData.otherDiagonalLinePatterns, out cells, blackNeeded, whiteNeeded, CompareBalls))
        {
            //Debug.Log("otherDiagonalLinePatterns 4-0 pattern");
            return true;
        }

        if (FindCellPattern(patternData.horizontalCrossPatterns, out cells, blackNeeded, whiteNeeded, CompareBalls))
        {
            //Debug.Log("horizontalCrossPatterns 4-0 pattern");
            return true;
        }

        if (FindCellPattern(patternData.diagonalCrossPatterns, out cells, blackNeeded, whiteNeeded, CompareBalls))
        {
            //Debug.Log("diagonalCrossPatterns 4-0 pattern");
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

        foreach (Vector2 cell in patternCells)
        {
            if (cells[(int)cell.x][(int)cell.y].color == CellColor.None)
                emptyCell = cell;
        }

        foreach (Move move in moves)
        {
            bool skipMove = false;
            foreach (Vector2 cell in patternCells)
            {
                if (!(move.toX == emptyCell.x && move.toY == emptyCell.y) || (move.fromX == cell.x && move.fromY == cell.y))
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

    private bool CompareBalls(int aiPawnCount, int blackNeeded, int opponentPawnCount, int whiteNeeded)
    {
        if (aiPawnCount == blackNeeded && opponentPawnCount == whiteNeeded)
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
            ballsLeft = blackBallsLeft.Count;
        else
            ballsLeft = whiteBallsLeft.Count;

        if (ballsLeft == 0)
        {
            for (i = 0; i < Cells.Length; ++i)
            {
                for (j = 0; j < Cells[i].Length; ++j)
                {
                    if (Cells[i][j].color == color)
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
                                move.isPoint = cell.isPoint;

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
                                    move.isPoint = actualCell.isPoint;

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
                    if (cells[i][j].color == CellColor.None)
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

    public void UpdateOptimizedGridPoints(ModelGrid modelGrid)
    {
        for (int i = 0; i < cells.Length; ++i)
        {
            for (int j = 0; j < cells[i].Length; ++j)
            {
                if (modelGrid.GetCellFromModel(i, j).ball.Score > 0)
                    cells[i][j].isPoint = true;
                else
                    cells[i][j].isPoint = false;
            }
        }
    }

    public void AddBall(CellColor color, bool isPoint)
    {
        if (color == CellColor.Black)
            blackBallsLeft.Add(new OptiBall(isPoint, color));
        else
            whiteBallsLeft.Add(new OptiBall(isPoint, color));
    }

    public OptiBall SubBall(CellColor color)
    {
        OptiBall ball;

        if (color == CellColor.Black)
        {
            int rdm = UnityEngine.Random.Range(0, blackBallsLeft.Count - 1);
            ball = blackBallsLeft[rdm];
            blackBallsLeft.RemoveAt(rdm);
        }
        else
        {
            int rdm = UnityEngine.Random.Range(0, whiteBallsLeft.Count - 1);
            ball = whiteBallsLeft[rdm];
            whiteBallsLeft.RemoveAt(rdm);
        }

        return ball;
    }
}