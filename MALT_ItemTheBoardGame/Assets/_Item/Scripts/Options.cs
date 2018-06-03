using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Options
{

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

}
