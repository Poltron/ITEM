using UnityEngine;
using System.Collections;
using UnityEditor;

public class MakeScriptableObject
{
    [MenuItem("Assets/Create/AI Evaluation Data")]
    public static void CreateMyAsset()
    {
        AIEvaluationData asset = ScriptableObject.CreateInstance<AIEvaluationData>();

        AssetDatabase.CreateAsset(asset, "Assets/AIEvaluationData.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}