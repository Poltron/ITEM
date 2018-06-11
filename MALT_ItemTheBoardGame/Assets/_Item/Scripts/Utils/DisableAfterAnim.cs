using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterAnim : MonoBehaviour
{
    private void Disable()
    {
        gameObject.SetActive(false);
    }
}
