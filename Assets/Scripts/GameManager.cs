using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager> 
{
    public GameObject loadingImage;


    public void LoadEnterMenu()
    {
        Debug.Log("Enter Menu");
    }

    public void LoadSingleplayerCave()
    {
        Debug.Log("Enter Singleplayer Cave");
        LoadScene(1);
    }

    public void LoadMultiplayerCave()
    {
        Debug.Log("Enter Multiplayer Cave");
    }

    public void LoadProfile()
    {
        Debug.Log("Profile");
    }

    void LoadScene(int level)
    {
        loadingImage.SetActive(true);
        Application.LoadLevel(level);
    }

    public void CallMenu()
    {
        Debug.Log("Menu Called");
    }
}
