using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayWithoutFacebook : MonoBehaviour
{
    [SerializeField]
    private MainMenu menu;

    [SerializeField]
    private DontDestroyOnLoad dontDestroy;

    public void OnButtonPressed()
    {
        Destroy(dontDestroy.gameObject);
        menu.ShowMenu();
    }
}
