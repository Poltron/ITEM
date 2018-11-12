using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Move
{
    public CellColor color;
    public bool isPoint;
    public int fromX;
    public int fromY;
    public int toX;
    public int toY;

    public Move(CellColor _color, bool _isPoint, int _fromX, int _fromY, int _toX, int _toY)
    {
        color = _color;
        isPoint = _isPoint;
        fromX = _fromX;
        fromY = _fromY;

        toX = _toX;
        toY = _toY;
    }

    public bool IsNewBall()
    {
        if (fromX == -1 && fromY == -1)
        {
            return true;
        }

        return false;
    }
}

public class AIBehaviour
{
    public int positionCount = 0;
    public float timeSpent;

    AIEvaluationData evaluationData;
    OptimizedGrid grid;

    public bool canOnlyJump;

    private BallColor opponentColor;
    private int opponentScore;
    private BallColor aiColor;
    private int aiScore;

    private AIProfile aiProfile;

    public AIBehaviour(AIEvaluationData _evaluationData)
    {
        evaluationData = _evaluationData;
    }

    public void SetAIEvaluationData(AIEvaluationData _data)
    {
        evaluationData = _data;
    }

    public void SetAIProfile(AIProfile _aiProfile)
    {
        aiProfile = _aiProfile;
    }

    public void SetPlayersData(BallColor _aiColor, int _aiScore, BallColor _playerColor, int _playerScore)
    {
        aiColor = _aiColor;
        aiScore = _aiScore;
        opponentColor = _playerColor;
        opponentScore = _playerScore;
    }

    public IEnumerator GetRandomMove(OptimizedGrid optiGrid, System.Action<Move> toDo)
    {
        yield return new WaitForEndOfFrame();
        grid = optiGrid;

        List<Move> newGameMoves = grid.GetAvailableMoves((CellColor)aiColor);
        toDo(newGameMoves[Random.Range(0, newGameMoves.Count - 1)]);
    }

    public IEnumerator GetBestMove(OptimizedGrid optiGrid, System.Action<Move> toDo)
    {
        grid = optiGrid;

        positionCount = 0;
        timeSpent = 0;
        float actualTime = Time.realtimeSinceStartup;

        Move bestMove = new Move();
        bool done = false;

        yield return new WaitForEndOfFrame();

        List<Vector2> canWinCells = new List<Vector2>();
        // if AI has a 4-0 pattern
        if (grid.CanColorWin(aiColor, out canWinCells))
        {
            // if AI can move to win
            if (grid.CanColorMoveToWin(aiColor, canWinCells, out bestMove))
            {
                grid.DoMove(bestMove);

                if (IsScoreEnoughToWin(aiColor, canWinCells))
                    done = true;

                grid.UndoMove(bestMove);
            }
        }

        // if Player can win next turn
        if (grid.CanColorWin(opponentColor, out canWinCells) && IsScoreEnoughToWin(opponentColor, canWinCells) && !done)
        {
            // if player can move to win next turn
            if (grid.CanColorMoveToWin(opponentColor, canWinCells, out bestMove))
            {
                grid.DoMove(bestMove);
                bool canPlayerWin = IsScoreEnoughToWin(opponentColor, canWinCells);
                grid.UndoMove(bestMove);

                // can AI def it ???
                if (canPlayerWin && grid.CanColorMoveToWin(aiColor, canWinCells, out bestMove))
                {
                    grid.DoMove(bestMove);

                    if (IsScoreEnoughToWin(aiColor, canWinCells))
                        done = true;

                    grid.UndoMove(bestMove);
                }

            }
        }

        if (!done)
        {
            int depth = 3;
            bestMove = MiniMaxRoot(depth, true);
        }

        float newTime = Time.realtimeSinceStartup;
        timeSpent = newTime - actualTime;

        Debug.Log(positionCount + " in " + timeSpent);

        toDo(bestMove);
    }

    public Move MiniMaxRoot(int depth, bool isMaximisingPlayer)
    {
        CellColor color = (CellColor)aiColor;

        if (!isMaximisingPlayer)
            color = (CellColor)opponentColor;

        List<Move> newGameMoves = grid.GetAvailableMoves(color);
        int bestMove = int.MinValue + 1;
        List<Move> bestMovesFound = new List<Move>();

        // loop on all moves available on this board
        for (int i = 0; i < newGameMoves.Count; i++)
        {
            Move move = newGameMoves[i];

            positionCount++;

            grid.DoMove(move);

            int value = -EvaluateGrid(evaluationData, depth);

            if (!IsGameEnded(value))
            {
                // get loop on all moves available on board with this move
                value = MiniMax(depth - 1, int.MinValue + 1, int.MaxValue - 1, !isMaximisingPlayer);
            }

            grid.UndoMove(move);

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


    private int MiniMax(int depth, int alpha, int beta, bool isMaximisingPlayer)
    {
        positionCount++;

        if (depth == 0)
        {
            return -EvaluateGrid(evaluationData, depth);
        }

        CellColor color = (CellColor)aiColor;
        if (!isMaximisingPlayer)
            color = (CellColor)opponentColor;

        var newGameMoves = grid.GetAvailableMoves(color);


        // IA
        if (isMaximisingPlayer)
        {
            var bestMove = alpha;
            for (var i = 0; i < newGameMoves.Count; i++)
            {
                Move move = newGameMoves[i];

                grid.DoMove(move);

                bestMove = -EvaluateGrid(evaluationData, depth);

                if (!IsGameEnded(bestMove))
                {
                    // get loop on all moves available on board with this move
                    bestMove = Mathf.Max(bestMove, MiniMax(depth - 1, bestMove, beta, !isMaximisingPlayer));
                }

                grid.UndoMove(move);

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

                grid.DoMove(move);

                bestMove = -EvaluateGrid(evaluationData, depth);

                if (!IsGameEnded(bestMove))
                {
                    // get loop on all moves available on board with this move
                    bestMove = Mathf.Min(bestMove, MiniMax(depth - 1, alpha, bestMove, !isMaximisingPlayer));
                }

                grid.UndoMove(move);

                if (bestMove <= alpha)
                {
                    return bestMove;
                }
            }
            return bestMove;
        }
    }

    private int EvaluatePattern(EvaluationPattern[] patterns, int depth)
    {
        int fitness = 0;
        int aiPawnCount = 0;
        int opponentPawnCount = 0;
        int aiScore = 0;
        int opponentScore = 0;

        for (int i = 0; i < patterns.Length; ++i)
        {
            aiPawnCount = 0;
            opponentPawnCount = 0;
            aiScore = 0;
            opponentScore = 0;

            for (int j = 0; j < patterns[i].positions.Length; ++j)
            {
                IntVec2 pos = patterns[i].positions[j];
                if (grid.Cells[pos.X][pos.Y].color == (CellColor)aiColor)
                {
                    if (grid.Cells[pos.X][pos.Y].isPoint)
                        aiScore++;

                    aiPawnCount++;
                }
                else if (grid.Cells[pos.X][pos.Y].color == (CellColor)opponentColor)
                {
                    if (grid.Cells[pos.X][pos.Y].isPoint)
                        opponentScore++;

                    opponentPawnCount++;
                }
            }

            if (aiPawnCount == 5 && IsScoreEnoughToWin(aiColor, aiScore))
            {
                return -9999999 + depth - aiScore;
            }
            else if (opponentPawnCount == 5 && IsScoreEnoughToWin(opponentColor, opponentScore))
            {
                return 9999999 - depth + opponentScore;
            }
            else if (aiProfile.Depth == 1)
            {
                if (aiPawnCount == 1 && opponentPawnCount == 0)
                    fitness -= 100;
                else if (aiPawnCount == 2 && opponentPawnCount == 0)
                    fitness -= 1000;
                else if (aiPawnCount == 3 && opponentPawnCount == 0)
                    fitness -= 4000;
                else if (aiPawnCount == 4 && opponentPawnCount == 0)
                    fitness -= 20000;
                else if (aiPawnCount == 5 && opponentPawnCount == 0) // if it reaches that point, ai playing that would make itself loose
                    fitness += 1000000;

                else if (aiPawnCount == 4 && opponentPawnCount == 1)
                    fitness += 75000;

                else if (aiPawnCount == 2 && opponentPawnCount == 1)
                    fitness -= 500;
                else if (aiPawnCount == 1 && opponentPawnCount == 2)
                    fitness += 500;

                else if (aiPawnCount == 3 && opponentPawnCount == 1)
                    fitness -= 2500;
                else if (aiPawnCount == 1 && opponentPawnCount == 3)
                    fitness += 3500;

                else if (aiPawnCount == 3 && opponentPawnCount == 2)
                    fitness -= 4000;
                else if (aiPawnCount == 2 && opponentPawnCount == 3)
                    fitness += 4000;

                else if (aiPawnCount == 1 && opponentPawnCount == 4)
                    fitness -= 75000;

                else if (aiPawnCount == 0 && opponentPawnCount == 1)
                    fitness += 200;
                else if (aiPawnCount == 0 && opponentPawnCount == 2)
                    fitness += 1500;
                else if (aiPawnCount == 0 && opponentPawnCount == 3)
                    fitness += 5000;
                else if (aiPawnCount == 0 && opponentPawnCount == 4)
                    fitness += 50000;
                else if (aiPawnCount == 0 && opponentPawnCount == 5) // if it reaches that point, opponent playing that would make itself loose
                    fitness -= 1000000;
            }
            else if (aiProfile.Depth == 3)
            {
                if (aiPawnCount == 1 && opponentPawnCount == 0)
                    fitness -= 100;
                else if (aiPawnCount == 2 && opponentPawnCount == 0)
                    fitness -= 750;
                else if (aiPawnCount == 3 && opponentPawnCount == 0)
                    fitness -= 1500;
                else if (aiPawnCount == 4 && opponentPawnCount == 0)
                    fitness -= 20000;
                else if (aiPawnCount == 5 && opponentPawnCount == 0) // if it reaches that point, ai playing that would make itself loose
                    fitness += 1000000;

                else if (aiPawnCount == 4 && opponentPawnCount == 1)
                    fitness += 75000;

                else if (aiPawnCount == 2 && opponentPawnCount == 1)
                    fitness -= 500;
                else if (aiPawnCount == 1 && opponentPawnCount == 2)
                    fitness += 1000;

                else if (aiPawnCount == 3 && opponentPawnCount == 1)
                    fitness -= 2500;
                else if (aiPawnCount == 1 && opponentPawnCount == 3)
                    fitness += 2500;

                else if (aiPawnCount == 3 && opponentPawnCount == 2)
                    fitness -= 4000;
                else if (aiPawnCount == 2 && opponentPawnCount == 3)
                    fitness += 4000;

                else if (aiPawnCount == 1 && opponentPawnCount == 4)
                    fitness -= 75000;

                else if (aiPawnCount == 0 && opponentPawnCount == 1)
                    fitness += 300;
                else if (aiPawnCount == 0 && opponentPawnCount == 2)
                    fitness += 4000;
                else if (aiPawnCount == 0 && opponentPawnCount == 3)
                    fitness += 15000;
                else if (aiPawnCount == 0 && opponentPawnCount == 4)
                    fitness += 50000;
                else if (aiPawnCount == 0 && opponentPawnCount == 5) // if it reaches that point, opponent playing that would make itself loose
                    fitness -= 1000000;
            }
        }

        return fitness;
    }

    public int EvaluateGrid(AIEvaluationData data, int depth)
    {

        int value = 0;
        int tmpValue = 0;

        tmpValue = EvaluatePattern(data.horizontalLinePatterns, depth);
        if (IsGameEnded(tmpValue))
        {
            return tmpValue;
        }
        value += tmpValue;

        tmpValue = EvaluatePattern(data.verticalLinePatterns, depth);
        if (IsGameEnded(tmpValue))
        {
            return tmpValue;
        }
        value += tmpValue;

        tmpValue = EvaluatePattern(data.diagonalLinePatterns, depth);
        if (IsGameEnded(tmpValue))
        {
            return tmpValue;
        }
        value += tmpValue;

        tmpValue = EvaluatePattern(data.otherDiagonalLinePatterns, depth);
        if (IsGameEnded(tmpValue))
        {
            return tmpValue;
        }
        value += tmpValue;

        tmpValue = EvaluatePattern(data.horizontalCrossPatterns, depth);
        if (IsGameEnded(tmpValue))
        {
            return tmpValue;
        }
        value += tmpValue;


        tmpValue = EvaluatePattern(data.diagonalCrossPatterns, depth);
        if (IsGameEnded(tmpValue))
        {
            return tmpValue;
        }
        value += tmpValue;


        return value;
    }

    public bool IsScoreEnoughToWin(BallColor color, List<Vector2> winningPattern)
    {
        int total = 0;
        foreach (Vector2 pawn in winningPattern)
        {
            if (grid.Cells[(int)pawn.x][(int)pawn.y].isPoint)
                total++;
        }

        return IsScoreEnoughToWin(color, total);
    }

    public bool IsScoreEnoughToWin(BallColor color, int score)
    {
        if (aiColor == color && aiScore + score > opponentScore)
        {
            return true;
        }
        else if (opponentColor == color && opponentScore + score > aiScore)
        {
            return true;
        }

        return false;
    }

    public bool IsGameEnded(int score)
    {
        if (score > 9000000 || score < -9000000)
            return true;

        return false;
    }

    public bool IsIAWin(int score)
    {
        if (score < -9000000)
            return true;

        return false;
    }

    public bool IsPlayerWin(int score)
    {
        if (score > 9000000)
            return true;

        return false;
    }
}