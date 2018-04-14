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
        OPTIGRID
    };

    public class DebugGUI : MonoBehaviour {

        [SerializeField]
        private GridManager gridManager;

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
                    if (gridManager.OptiGrid != null)
                        _text.text = gridManager.OptiGrid.BlackBallsLeft.ToString();
                    break;
                case DEBUG_VALUE.WHITEBALLSLEFT:
                    if (gridManager.OptiGrid != null)
                        _text.text = gridManager.OptiGrid.WhiteBallsLeft.ToString();
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

                    Debug.Log(str);
                    _text.text = str;
                    break;
            }
        }

        private string GetLetterFromOptiGrid(int x, int y)
        {
            if (gridManager.OptiGrid != null)
            {
                switch (gridManager.OptiGrid.Cells[x][y])
                {
                    case CellColor.Black:
                        return "B";
                    case CellColor.White:
                        return "W";
                    case CellColor.None:
                        return "0";
                    case CellColor.NOT_A_CELL:
                        return "X";
                }
            }

            return "";
        }

    }
}