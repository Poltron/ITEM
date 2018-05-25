using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class IntVec2
{
    [SerializeField]
    public int X;
    [SerializeField]
    public int Y;
}

[System.Serializable]
public class EvaluationPattern
{
    [SerializeField]
    public string description;
    [SerializeField]
    public IntVec2[] positions;
}

public class AIEvaluationData : ScriptableObject
{
    [SerializeField]
    public EvaluationPattern[] horizontalLinePatterns;
    [SerializeField]
    public EvaluationPattern[] verticalLinePatterns;
    [SerializeField]
    public EvaluationPattern[] diagonalLinePatterns;
    [SerializeField]
    public EvaluationPattern[] otherDiagonalLinePatterns;
    [SerializeField]
    public EvaluationPattern[] horizontalCrossPatterns;
    [SerializeField]
    public EvaluationPattern[] diagonalCrossPatterns;
    [SerializeField]
    public int HowManyTurnsForRandom;
}
