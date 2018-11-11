using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AppAdvisory.Item
{
    public enum DEBUG_VALUE
    {
        BLACKBALLSLEFT,
        WHITEBALLSLEFT,
        AIPOSITIONTESTNB,
        AIPOSITIONTESTTIME,
        OPTIGRID,
        PLAYER1NBTURN,
        PLAYER2NBTURN,
        PLAYER1SCORE,
        PLAYER2SCORE,
        PLAYER1TOTALSCORE,
        PLAYER2TOTALSCORE,
        WHOSTURN,
        EQUALITYTURN
    };

    public class UIDebugValue : MonoBehaviour {
        
        [SerializeField]
        private DEBUG_VALUE debugValue;

        private Text _text;

        private void Awake()
        {
            _text = GetComponent<Text>();
        }

        private void Update()
        {
            switch (debugValue)
            {
                case DEBUG_VALUE.BLACKBALLSLEFT:
                    if (GridManager.Instance.OptiGrid != null)
                        _text.text = GridManager.Instance.OptiGrid.BlackBallsLeft.ToString();
                    break;
                case DEBUG_VALUE.WHITEBALLSLEFT:
                    if (GridManager.Instance.OptiGrid != null)
                        _text.text = GridManager.Instance.OptiGrid.WhiteBallsLeft.ToString();
                    break;
                case DEBUG_VALUE.AIPOSITIONTESTNB:
                    if (PlayerManager.Instance.AIBehaviour != null)
                        _text.text = PlayerManager.Instance.AIBehaviour.positionCount.ToString();
                    break;
                case DEBUG_VALUE.AIPOSITIONTESTTIME:
                    if (PlayerManager.Instance.AIBehaviour != null)
                        _text.text = PlayerManager.Instance.AIBehaviour.timeSpent.ToString("0.0000") + "s";
                    break;
                case DEBUG_VALUE.OPTIGRID:
                    string str = GetLetterFromOptiGrid(8, 0) + "__" + GetLetterFromOptiGrid(8, 1) + "__" + GetLetterFromOptiGrid(8, 2) + "__" + GetLetterFromOptiGrid(8, 3) + "__" + GetLetterFromOptiGrid(8, 4)
                                + "\n__" + GetLetterFromOptiGrid(7, 0) + "__" + GetLetterFromOptiGrid(7, 1) + "__" + GetLetterFromOptiGrid(7, 2) + "__" + GetLetterFromOptiGrid(7, 3) + "__\n"
                                + GetLetterFromOptiGrid(6, 0) + "__" + GetLetterFromOptiGrid(6, 1) + "__" + GetLetterFromOptiGrid(6, 2) + "__" + GetLetterFromOptiGrid(6, 3) + "__" + GetLetterFromOptiGrid(6, 4)
                                + "\n__" + GetLetterFromOptiGrid(5, 0) + "__" + GetLetterFromOptiGrid(5, 1) + "__" + GetLetterFromOptiGrid(5, 2) + "__" + GetLetterFromOptiGrid(5, 3) + "__\n"
                                + GetLetterFromOptiGrid(4, 0) + "__" + GetLetterFromOptiGrid(4, 1) + "__" + GetLetterFromOptiGrid(4, 2) + "__" + GetLetterFromOptiGrid(4, 3) + "__" + GetLetterFromOptiGrid(4, 4)
                                + "\n__" + GetLetterFromOptiGrid(3, 0) + "__" + GetLetterFromOptiGrid(3, 1) + "__" + GetLetterFromOptiGrid(3, 2) + "__" + GetLetterFromOptiGrid(3, 3) + "__\n"
                                + GetLetterFromOptiGrid(2, 0) + "__" + GetLetterFromOptiGrid(2, 1) + "__" + GetLetterFromOptiGrid(2, 2) + "__" + GetLetterFromOptiGrid(2, 3) + "__" + GetLetterFromOptiGrid(2, 4)
                                + "\n__" + GetLetterFromOptiGrid(1, 0) + "__" + GetLetterFromOptiGrid(1, 1) + "__" + GetLetterFromOptiGrid(1, 2) + "__" + GetLetterFromOptiGrid(1, 3) + "__\n"
                                + GetLetterFromOptiGrid(0, 0) + "__" + GetLetterFromOptiGrid(0, 1) + "__" + GetLetterFromOptiGrid(0, 2) + "__" + GetLetterFromOptiGrid(0, 3) + "__" + GetLetterFromOptiGrid(0, 4);

                    _text.text = str;
                    break;
                case DEBUG_VALUE.PLAYER1NBTURN:
                    _text.text = PlayerManager.Instance.Player1.NbOfTurn.ToString();
                    break;
                case DEBUG_VALUE.PLAYER2NBTURN:
                    _text.text = PlayerManager.Instance.Player2.NbOfTurn.ToString();
                    break;
                case DEBUG_VALUE.PLAYER1SCORE:
                    _text.text = PlayerManager.Instance.Player1.roundScore.ToString();
                    break;
                case DEBUG_VALUE.PLAYER2SCORE:
                    _text.text = PlayerManager.Instance.Player2.roundScore.ToString();
                    break;
                case DEBUG_VALUE.PLAYER1TOTALSCORE:
                    _text.text = PlayerManager.Instance.Player1.totalScore.ToString();
                    break;
                case DEBUG_VALUE.PLAYER2TOTALSCORE:
                    _text.text = PlayerManager.Instance.Player2.totalScore.ToString();
                    break;
                case DEBUG_VALUE.WHOSTURN:
                    _text.text = FindObjectOfType<UIManager>().isPlayer1Turn ? "player1turn" : "player2turn";
                    break;
                case DEBUG_VALUE.EQUALITYTURN:
                    _text.text = (GridManager.Instance.IsEqualityTurn) ? "true" : "false";
                    break;
            }
        }

        private string GetLetterFromOptiGrid(int x, int y)
        {
            if (GridManager.Instance.OptiGrid != null)
            {
                switch (GridManager.Instance.OptiGrid.Cells[x][y].color)
                {
                    case CellColor.Black:
                        if (GridManager.Instance.OptiGrid.Cells[x][y].isPoint)
                            return "b";
                        else
                            return "B";
                    case CellColor.White:
                        if (GridManager.Instance.OptiGrid.Cells[x][y].isPoint)
                            return "w";
                        else
                            return "W";
                    case CellColor.None:
                        if (GridManager.Instance.OptiGrid.Cells[x][y].isPoint)
                            return "o";
                        else
                            return "0";
                    case CellColor.NOT_A_CELL:
                        return "X";
                }
            }

            return "";
        }

    }
}