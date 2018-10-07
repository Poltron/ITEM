using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    AudioSource _source;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();

        // fix sale ayy
        Options.Init();
    }

    void Update ()
    {
        _source.mute = Options.GetMuteMusic();
	}
}
