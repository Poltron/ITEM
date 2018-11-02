using UnityEngine;
using System.Collections;
using UnityEditor;

public class MakeScriptableObject
{
    [MenuItem("Assets/Create/Custom/AI Profile")]
    public static void CreateMyAIProfile()
    {
        AIProfile asset = ScriptableObject.CreateInstance<AIProfile>();

        AssetDatabase.CreateAsset(asset, "Assets/AIProfile.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }

    [MenuItem("Assets/Create/Custom/AI Evaluation Data")]
    public static void CreateMyAIEvaluationDataAsset()
    {
        AIEvaluationData asset = ScriptableObject.CreateInstance<AIEvaluationData>();

        AssetDatabase.CreateAsset(asset, "Assets/AIEvaluationData.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }

    [MenuItem("Assets/Create/Custom/Sound List")]
    public static void CreateMySoundListAsset()
    {
        SoundListSO asset = ScriptableObject.CreateInstance<SoundListSO>();

        AssetDatabase.CreateAsset(asset, "Assets/SoundList.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}