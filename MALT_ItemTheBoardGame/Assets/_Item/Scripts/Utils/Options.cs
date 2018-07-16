using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Options
{
    public static void Init()
    {
        if (PlayerPrefs.GetInt("prefInitialized", 0) == 0)
        {
            SetEnableHelpPopup(true);
            SetEnablePlacementHelp(true);
            SetAskForTuto(true);

            PlayerPrefs.SetInt("prefInitialized", 1);

            Debug.Log("not initialized");
        }
        else
        {
            Debug.Log("already initialized");
        }
    }

    // Language
    public static void SetLanguage(string language)
    {
        if (language == "fr")
            PlayerPrefs.SetString("language", "fr");
        else if (language == "en")
            PlayerPrefs.SetString("language", "en");
    }

    public static string GetLanguage()
    {
        return PlayerPrefs.GetString("language", "");
    }

    public static bool IsLanguageFr()
    {
        return PlayerPrefs.GetString("language", "") == "fr";
    }

    public static bool IsLanguageEn()
    {
        return PlayerPrefs.GetString("language", "") == "en";
    }

    // Ball Placement Help"
    public static void SetEnablePlacementHelp(bool isEnabled)
    {
        if (isEnabled)
            PlayerPrefs.SetInt("placementHelp", 1);
        else
            PlayerPrefs.SetInt("placementHelp", 0);
    }

    public static bool GetEnablePlacementHelp()
    {
        return (PlayerPrefs.GetInt("placementHelp", 0) == 1);
    }

    // Phase Help Popup
    public static void SetEnableHelpPopup(bool isEnabled)
    {
        if (isEnabled)
            PlayerPrefs.SetInt("helpPopup", 1);
        else
            PlayerPrefs.SetInt("helpPopup", 0);
    }

    public static bool GetEnableHelpPopup()
    {
        return (PlayerPrefs.GetInt("helpPopup", 0) == 1);
    }

    // Ask For Tutorial
    public static void SetAskForTuto(bool isEnabled)
    {
        if (isEnabled)
            PlayerPrefs.SetInt("askForTuto", 1);
        else
            PlayerPrefs.SetInt("askForTuto", 0);
    }

    public static bool GetAskForTuto()
    {
        return (PlayerPrefs.GetInt("askForTuto", 0) == 1);
    }
}
