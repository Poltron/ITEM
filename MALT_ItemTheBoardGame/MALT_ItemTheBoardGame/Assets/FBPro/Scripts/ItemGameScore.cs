﻿using UnityEngine;
using UnityEngine.UI;

public class ItemGameScore : MonoBehaviour {

    public Text txtName, txtScore;

    public void AssignValues(string gameName, string gameScore)
    {
        txtName.text = gameName;
        txtScore.text = gameScore;
    }
}
