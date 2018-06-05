using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Options
{
    public static void Init()
    {
        if (PlayerPrefs.GetInt("prefInitialized", 0) == 0)
        {

        }
        else
        {
            SetEnableHelpPopup(true);
            SetEnablePlacementHelp(true);
            SetAskForTuto(true);
        }
    }

    // Ball Placement Help
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
