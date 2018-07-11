using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIPanel : MonoBehaviour
{
    public void SetLanguage(string language)
    {
        if (language == "fr")
        {
            SetLanguageFR();
        }
        else if (language == "en")
        {
            SetLanguageEN();
        }
    }

    protected abstract void SetLanguageFR();
    protected abstract void SetLanguageEN();
}
