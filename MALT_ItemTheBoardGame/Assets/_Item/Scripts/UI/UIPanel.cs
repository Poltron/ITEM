using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIPanel : MonoBehaviour
{
    private void Start()
    {
        SetLanguage(Options.GetLanguage());
    }

    public void SetLanguage(string language)
    {
        if (language == "en")
        {
            SetLanguageEN();
        }
        else
        {
            SetLanguageFR();
        }
    }

    protected abstract void SetLanguageFR();
    protected abstract void SetLanguageEN();
}
