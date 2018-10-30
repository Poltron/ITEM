using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject menuButtons;

    [SerializeField]
    private GameObject loginButtons;

    void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    public void ShowMenu()
    {
        menuButtons.SetActive(true);
        loginButtons.SetActive(false);
    }

    public void PlayLocalDuel()
    {
        Debug.Log("PlayLocalDuel");
    }

    public void PlayRemoteDuel()
    {
        Debug.Log("PlayRemoteDuel");
    }

    public void PlayVSAI()
    {
        Debug.Log("PlayVSAI");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
