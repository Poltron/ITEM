using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterAnim : MonoBehaviour
{
	void SelfDestroy()
    {
        Destroy(gameObject);
	}
}
