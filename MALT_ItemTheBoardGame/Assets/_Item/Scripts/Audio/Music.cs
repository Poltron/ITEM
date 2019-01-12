using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    [SerializeField]
    AudioSource _menuSource;
    [SerializeField]
    AudioSource _gameSource;

    public AudioSource GameSource { get { return _gameSource; } }

    private Coroutine lerp;
    private float volume;

    private void Start()
    {
        volume = _menuSource.volume;
    }

    private void SetMuteMusic(bool mute)
    {
        _menuSource.mute = mute;
        _gameSource.mute = mute;
    }

    public void GoToMenuMusic()
    {
        if (lerp != null)
            StopCoroutine(lerp);

        lerp = StartCoroutine(CrossFade(_gameSource, _menuSource));
    }

    public void GoToGameMusic()
    {
        if (lerp != null)
            StopCoroutine(lerp);

        lerp = StartCoroutine(CrossFade(_menuSource, _gameSource));
    }

    private IEnumerator CrossFade(AudioSource toEnd, AudioSource toPlay)
    {
        toPlay.Play();

        for (float f = 0.0f; f < 3.0f; f += Time.deltaTime)
        {
            if (toEnd.isPlaying)
            toEnd.volume = (1.0f - (f / 3.0f)) * volume;
            toPlay.volume = (f / 3.0f) * volume;
            yield return new WaitForEndOfFrame();
        }

        toEnd.volume = 0;
        toEnd.Stop();
        toPlay.volume = volume;
    }
}
