using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : Singleton<GameManager> 
{
    public GameObject loadingImage;
    
    private int currentScene;
    private HostData[] hostList;
    private int totalHosts;
    private int connectedTo;

    void Start()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
        hostList = new HostData[10];
        totalHosts = 0;
        connectedTo = -1;
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
        if (SceneManager.GetActiveScene().buildIndex == 2)
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
        NetworkManager nm = GameObject.FindObjectOfType<NetworkManager>();
        //NetworkServer.serverHostId;
        nm.StartHost();
        hostList[totalHosts] = new HostData();
        hostList[totalHosts].port = NetworkServer.listenPort;
        hostList[totalHosts].ip = new string[1];
        hostList[totalHosts].ip[0] = Network.player.ipAddress;
        hostList[totalHosts].playerLimit = 2;
        totalHosts++;
    }

    public void LoadClientMultiplayer()
    {
        LoadScene(4);
       /* int server = -1;
        for (server = 0; server < totalHosts; ++server)
            if (hostList[server].connectedPlayers < hostList[server].playerLimit)
                break;*/
        NetworkClient client = new NetworkClient();
        client.RegisterHandler(MsgType.Connect, OnConnected);
        client.Connect("localhost", 7777);
        
       // client.Connect(hostList[server].ip[0], hostList[server].port);
        NetworkManager nm = GameObject.FindObjectOfType<NetworkManager>();
        nm.client = client;
       // nm.StartClient();
    }

    public void OnConnected(NetworkMessage netMsg)
    {
        Debug.Log("Connected to server");
        // Debug.Log(NetworkServer.active + ": " + NetworkServer.listenPort);
    }

    public void LoadHostChoice()
    {
        LoadScene(5);
    }

    public void LoadLobby()
    {
        LoadScene(6);
        
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
        SceneManager.LoadScene(level, LoadSceneMode.Single);
        Debug.Log("loading " + level);
        currentScene = level;
    }
}
