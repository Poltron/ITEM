using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoPanel2 : MonoBehaviour
{
    private void PopOutAnimationCallback()
    {
        transform.parent.GetComponent<TutorialPanel>().DisableTutoScreen2();
    }
}
