using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterAudio : MonoBehaviour
{
    private AudioSource source;
    private float timer;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (timer > source.clip.length)
        {
            Destroy(gameObject);
        }
        else
        {
            timer += Time.deltaTime;
        }
    }
}
