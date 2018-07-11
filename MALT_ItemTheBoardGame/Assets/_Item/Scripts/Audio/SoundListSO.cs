using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundListSO : ScriptableObject
{
    [SerializeField]
    public Sound[] list;
}