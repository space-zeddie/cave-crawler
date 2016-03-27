using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager> 
{
    public GameObject loadingImage;

    private int currentScene;

    void Start()
    {
        currentScene = Application.loadedLevel;
    }

    public void LoadMainMenu()
    {
        LoadScene(0);
    }

    public void LoadEnterMenu()
    {
        LoadScene(1);
    }

    public void LoadSingleplayerCave()
    {
        LoadScene(2);
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
        currentScene = level;
        loadingImage.SetActive(false);
    }
}
