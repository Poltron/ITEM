using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundID
{
    SwitchTurn,
    CloseWindowHelp,
    CloseWindowOptions,
    CloseWindowTuto,
    OpenWindowHelp,
    OpenWindowOptions,
    Combo,
    PawnSelect,
    PawnPlace,
    ClickUI
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

    public void PlayAudio(SoundID id)
    {
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
}
