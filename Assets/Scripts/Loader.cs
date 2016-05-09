using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;

public class Loader : Singleton<Loader>
{
    public GameObject InstatiatedObjects;
    public GameManager GMPrefab;
    public GameObject spawnerPrefab;
    public Canvas canvas;

    void Awake()
    {
        if (InstatiatedObjects != null)
        {
            if (GameManager.instance == null)
            {
                GameManager gm = Instantiate(GMPrefab);
                gm.transform.parent = InstatiatedObjects.transform;
                AssignButtonHandlers();
            }
        }
        else Debug.LogError("The scene must contain one designated game object for storing runtime-generated objects");

        if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            GameObject spawner = (GameObject)GameObject.Instantiate(spawnerPrefab);
            if (NetworkServer.active) NetworkServer.Spawn(spawner);
            else ClientScene.RegisterPrefab(spawner);
        }
    }

    void AssignButtonHandlers()
    {
        SingleplayerButton sb = canvas.gameObject.transform.GetComponentInChildren<SingleplayerButton>();
        MultiplayerButton mb = canvas.gameObject.transform.GetComponentInChildren<MultiplayerButton>();
        ReturnToMenuButton rmb = canvas.gameObject.transform.GetComponentInChildren<ReturnToMenuButton>();
        JoinHostButton jhb = canvas.gameObject.transform.GetComponentInChildren<JoinHostButton>();
        CreateHostButton chb = canvas.gameObject.transform.GetComponentInChildren<CreateHostButton>();
        if (sb != null)
            (sb as Button).onClick.AddListener(delegate { GameManager.Instance.LoadGame(); });
        if (mb != null)
            (mb as Button).onClick.AddListener(delegate { GameManager.Instance.LoadHostChoice(); });
        if (rmb != null)
            (rmb as Button).onClick.AddListener(delegate { GameManager.Instance.LoadMainMenu(); });
        if (chb != null)
            (chb as Button).onClick.AddListener(delegate { GameManager.Instance.LoadMultiplayerCave(); });
        if (jhb != null)
            (jhb as Button).onClick.AddListener(delegate { GameManager.Instance.LoadClientMultiplayer(); });
    }


    public void LoadMainMenu()
    {
        HexGridCellularAutomata hex = GameObject.FindObjectOfType<HexGridCellularAutomata>();
        if (hex != null)
            hex.ClearGrid();
        GameManager.Instance.LoadMainMenu();
    }

    public void LoadSingplayerScene()
    {
        GameManager.Instance.LoadGame();
        
    }
    public void LoadMultiplayerScene()
    {
        //Application.LoadLevel();
    }
}
