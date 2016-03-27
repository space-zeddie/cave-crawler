using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager> 
{
    public GameObject loadingImage;

    public void LoadMainMenu()
    {
        LoadScene(0);
    }

    public void LoadEnterMenu()
    {
    }

    public void LoadSingleplayerCave()
    {
        LoadScene(1);
    }

    public void LoadMultiplayerCave()
    {
    }

    public void LoadProfile()
    {
    }

    public void Quit()
    {
        // ??
        Application.Quit();
    }

    void LoadScene(int level)
    {
        loadingImage.SetActive(true);
        Application.LoadLevel(level);
    }
}
