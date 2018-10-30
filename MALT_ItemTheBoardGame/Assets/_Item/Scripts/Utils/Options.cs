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
            SetMuteMusic(false);
            SetMuteSFX(false);

            PlayerPrefs.SetInt("prefInitialized", 1);
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

    // Mute Music
    public static void SetMuteMusic(bool isEnabled)
    {
        if (isEnabled)
            PlayerPrefs.SetInt("muteMusic", 1);
        else
            PlayerPrefs.SetInt("muteMusic", 0);
    }

    public static bool GetMuteMusic()
    {
        return (PlayerPrefs.GetInt("muteMusic", 0) == 1);
    }

    // Mute SFX
    public static void SetMuteSFX(bool isEnabled)
    {
        if (isEnabled)
            PlayerPrefs.SetInt("muteSFX", 1);
        else
            PlayerPrefs.SetInt("muteSFX", 0);
    }

    public static bool GetMuteSFX()
    {
        return (PlayerPrefs.GetInt("muteSFX", 0) == 1);
    }
}
