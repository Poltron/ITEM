using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AskForTutoPanel : MonoBehaviour
{
    private void PopOutAnimationCallback()
    {
        transform.parent.GetComponent<TutorialPanel>().DisableAskForTuto();
    }
}
