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

        public AIBehaviour(AIEvaluationData _evaluationData)
        {
            evaluationData = _evaluationData;
        }

        public void SetAIEvaluationData(AIEvaluationData _data)
        {
            evaluationData = _data;
        }

        public Move GetBestMove(OptimizedGrid optiGrid)
        {
            grid = optiGrid;

            positionCount = 0;
            timeSpent = 0;
            float actualTime = Time.realtimeSinceStartup;

            Move bestMove = new Move();

            List<Vector2> canWinCells = new List<Vector2>();
            // if AI has a 4-0 pattern
            if (grid.CanColorWin(BallColor.Black, out canWinCells))
            {
                Debug.Log("BlackColor can win");

                // if AI can move to win
                if (grid.CanColorMoveToWin(BallColor.Black, canWinCells, out bestMove))
                {
                    Debug.Log("BlackColor can move to win");
                    return bestMove;
                }

                Debug.Log("BlackColor can't move to win");
            }

            // if Player can win next turn
            if (grid.CanColorWin(BallColor.White, out canWinCells))
            {
                Debug.Log("Player can win next turn");

                // if player can move to win next turn
                if (grid.CanColorMoveToWin(BallColor.White, canWinCells, out bestMove))
                {
                    Debug.Log("Player can move to win next turn");

                    // can AI def it ???
                    if (grid.CanColorMoveToWin(BallColor.Black, canWinCells, out bestMove))
                    {
                        Debug.Log("AI can def next turn win");
                        return bestMove;
                    }
                }
            }

            bestMove = MiniMaxRoot(5, true);

            float newTime = Time.realtimeSinceStartup;
            timeSpent = newTime - actualTime;

            Debug.Log(positionCount + " in " + timeSpent);

            return bestMove;
        }

        public Move MiniMaxRoot(int depth, bool isMaximisingPlayer)
        {
            CellColor color = CellColor.Black;

            if (!isMaximisingPlayer)
                color = CellColor.White;

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


        public int MiniMax(int depth, int alpha, int beta, bool isMaximisingPlayer)
        {
            positionCount++;

            if (depth == 0)
            {
                return -EvaluateGrid(evaluationData, depth);
            }

            var newGameMoves = grid.GetAvailableMoves(CellColor.Black);

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
                    if (grid.Cells[pos.X][pos.Y] == CellColor.Black)
                    {
                        blackCount++;
                    }
                    else if (grid.Cells[pos.X][pos.Y] == CellColor.White)
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
                        value -= 750;
                    else if (blackCount == 3 && whiteCount == 0)
                        value -= 1500;
                    else if (blackCount == 4 && whiteCount == 0)
                        value -= 20000;

                    else if (blackCount == 4 && whiteCount == 1)
                        value += 75000;

                    else if (blackCount == 2 && whiteCount == 1)
                        value -= 500;
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

}