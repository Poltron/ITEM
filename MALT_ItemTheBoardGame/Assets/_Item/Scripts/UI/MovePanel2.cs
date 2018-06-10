using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePanel2 : MonoBehaviour
{
    private void PopOutAnimationCallback()
    {
        transform.parent.GetComponent<TutorialPanel>().DisablePhase2MoveScreen();
    }
}
