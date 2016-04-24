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

    void Awake()
    {
        loadingImage.SetActive(false);
    }

    public void LoadGameEndedScreen()
    {
        LoadScene(3);
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

    public void SaveGame()
    {
        if (Application.loadedLevel == 2)
        {
            Debug.Log("Saving game");
            PlayerState.Instance.SaveToGlobal();

            StatManager.Instance.LocalCopyOfPlayerData.SceneID = Application.loadedLevel;
            StatManager.Instance.SaveData();
        }
    }

    public void LoadGame()
    {
        LoadSingleplayerCave();
    }

    public void LoadMultiplayerCave()
    {
        LoadScene(4);
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
    }
}
