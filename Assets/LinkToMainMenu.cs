using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LinkToMainMenu : Singleton<LinkToMainMenu>
{
    private GameObject InstatiatedObjects;

    void Awake()
    {
        ///if
    }


    public void LoadMainMenu()
    {
        Application.LoadLevel(0);
    }
}
