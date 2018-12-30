using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSDisplay : MonoBehaviour
{
    float deltaTime = 0.0f;

    private TextMeshPro textMeshPro;

    private void Start()
    {
        textMeshPro = GetComponent<TextMeshPro>();
    }

    void Update()
    {
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        textMeshPro.text = text;
    }
}