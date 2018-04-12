using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AppAdvisory.Item
{
    public enum DEBUG_VALUE
    {
        BLACKBALLSLEFT,
        WHITEBALLSLEFT
    };

    public class DebugGUI : MonoBehaviour {

        [SerializeField]
        private DEBUG_VALUE debugValue;
        
        void Update()
        {
            switch (debugValue)
            {
                case DEBUG_VALUE.BLACKBALLSLEFT:
                    GetComponent<Text>().text = GameObject.Find("GridManager").GetComponent<GridManager>().OptiGrid.BlackBallsLeft.ToString();
                    break;
                case DEBUG_VALUE.WHITEBALLSLEFT:
                    GetComponent<Text>().text = GameObject.Find("GridManager").GetComponent<GridManager>().OptiGrid.WhiteBallsLeft.ToString();
                    break;
            }
        }



    }
}