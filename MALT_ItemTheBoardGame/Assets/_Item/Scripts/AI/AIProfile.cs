using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIProfile : ScriptableObject
{
    [SerializeField]
    private string aiName;
    public string Name { get { return aiName; } }

    [SerializeField]
    private string levelFR;
    public string LevelFR { get { return levelFR; } }

    [SerializeField]
    private string levelEN;
    public string LevelEN { get { return levelEN; } }

    [SerializeField]
    private int depth;
    public int Depth { get { return depth; } }

    [SerializeField]
    private Sprite image;
    public Sprite Image { get { return image; } }
}
