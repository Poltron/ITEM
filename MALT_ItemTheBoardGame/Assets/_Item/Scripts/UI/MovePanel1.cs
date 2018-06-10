using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePanel1 : MonoBehaviour
{
    private void PopOutAnimationCallback()
    {
        transform.parent.GetComponent<TutorialPanel>().DisablePhase1MoveScreen();
    }
}
