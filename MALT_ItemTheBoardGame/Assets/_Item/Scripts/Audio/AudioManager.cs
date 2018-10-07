using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundID
{
    SwitchTurn,
    PortraitRoll,

    CloseWindowHelp,
    CloseWindowOptions,
    CloseWindowTuto,
    OpenWindowHelp,
    OpenWindowOptions,
    OpenWindowTuto,

    CheckboxOn,
    CheckboxOff,

    ClickUI,

    ComboRumble,
    ComboImpact,
    ComboBallOne,
    ComboBallTwo,
    ComboBallThree,
    ComboBallFour,
    ComboBallFive,

    ParticleZero,
    ParticleOne,
    ParticleMove,

    PawnSelect,
    PawnPlace,

    JingleWin,
    JingleLoose,
    JingleDraw
}

[System.Serializable]
public struct Sound
{
    public SoundID id;
    public GameObject prefab;
}

[System.Serializable]
public class AudioManager : MonoBehaviour
{
    [Header("Audio Prefabs")]
    [SerializeField]
    private SoundListSO sounds;

    private AudioSource music;

    private int ballNumber;
    private bool isComboImpactPlayed;

    private void Start()
    {
        if (music == null)
        {
            Debug.Log("AudioManager start");
            Music musicGO = FindObjectOfType<Music>();
            if (musicGO)
                music = musicGO.GetComponent<AudioSource>();
            if (music == null)
                Debug.LogError("pas d'élément MUSIC :(");
            Debug.Log("AudioManager start end");
        }
    }

    public void PlayAudio(SoundID id)
    {
        if (Options.GetMuteSFX())
            return;

        foreach (var sound in sounds.list)
        {
            if (sound.id == id)
            {
                if (sound.prefab == null)
                    continue;

                GameObject.Instantiate(sound.prefab, Vector3.zero, Quaternion.identity);
            }
        }
    }

    /*
     *      MUSIC JINGLES
     */

    public void PlayJingleAboveMusic(SoundID id)
    {
        if (Options.GetMuteSFX())
            return;

        GameObject jingle = null;

        foreach (var sound in sounds.list)
        {
            if (sound.id == id)
            {
                if (sound.prefab == null)
                    continue;

                jingle = sound.prefab;
            }
        }

        if (jingle == null)
            return;

        StartCoroutine(crossFade(jingle));
    }

    private IEnumerator crossFade(GameObject jinglePrefab)
    {
        float baseMusicVolume = music.volume;
        float reducedMusicVolume = baseMusicVolume * 0.25f;

        for (float i = 0; i < 1; i += Time.deltaTime)
        {
            music.volume -= (baseMusicVolume - reducedMusicVolume) * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //Debug.Log(music.volume);
        music.volume = reducedMusicVolume;

        GameObject jingle = GameObject.Instantiate(jinglePrefab, Vector3.zero, Quaternion.identity);
        float jingleDuration = jingle.GetComponent<AudioSource>().clip.length;

        yield return new WaitForSeconds(jingleDuration);
        
        for (float i = 0; i < 1; i += Time.deltaTime)
        {
            music.volume += (baseMusicVolume - reducedMusicVolume) * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        //Debug.Log(music.volume);

        music.volume = baseMusicVolume;
    }

    /*
     *      COMBO
     */

    public void PlayComboImpact()
    {
        if (isComboImpactPlayed)
            return;

        PlayAudio(SoundID.ComboImpact);
        isComboImpactPlayed = true;
    }

    public void PlayNextBallPopSound()
    {
        switch (ballNumber)
        {
            case 1:
                PlayAudio(SoundID.ComboBallOne);
                break;
            case 2:
                PlayAudio(SoundID.ComboBallTwo);
                break;
            case 3:
                PlayAudio(SoundID.ComboBallThree);
                break;
            case 4:
                PlayAudio(SoundID.ComboBallFour);
                break;
            case 5:
                PlayAudio(SoundID.ComboBallFive);
                break;
        }

        ballNumber++;
    }

    public void ResetVictoryAnimationSounds()
    {
        isComboImpactPlayed = false;
        ballNumber = 1;
    }
}
