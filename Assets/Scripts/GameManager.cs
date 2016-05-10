using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

public class GameManager : Singleton<GameManager> 
{
    public GameObject loadingImage;
    public Texture2D cursorTexture;
    
    private int currentScene;
    private int connectedTo;

    const int MinOpponents = 1;
    const int MaxOpponents = 1;
    bool signedIn = false;

    List<MatchDesc> matchList = new List<MatchDesc>();
    bool matchCreated;
    NetworkMatch networkMatch;

    void Start()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        connectedTo = -1; 
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        // enables saving game progress.
        .EnableSavedGames()
        .Build();

        PlayGamesPlatform.InitializeInstance(config);
        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;
        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();
        NetworkManager nm = GameObject.FindObjectOfType<NetworkManager>();
        if (SceneManager.GetActiveScene().buildIndex == 4) nm.GetComponent<NetworkManagerHUD>().showGUI = true;
        else nm.GetComponent<NetworkManagerHUD>().showGUI = false;
        
    }



    void Invitation()
    {
        Debug.Log("invitation");
    }

    void Matched()
    {
        Debug.Log("Matched");
    }
    void Update()
    {
        GameObject authCheck = GameObject.FindGameObjectWithTag("AuthCheck");
        if (authCheck != null)
        {
            if (Social.localUser.authenticated)
            {
                authCheck.GetComponent<Text>().color = Color.gray;
                authCheck.GetComponent<Text>().text = "Weclome back!";
            }
            else
            {
                // light red
                authCheck.GetComponent<Text>().color = Color.red;
                authCheck.GetComponent<Text>().text = "Please, log in";
            }
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                Application.Quit();
            }
        }
    }

    void Awake()
    {
        loadingImage.SetActive(false);
        networkMatch = gameObject.AddComponent<NetworkMatch>();
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

    public void SignIn()
    {
        if (!signedIn)
        {
            // authenticate user:
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    Debug.Log("Authenticated, checking achievements");
                    

                    // Request loaded achievements, and register a callback for processing them
                    //  Social.LoadAchievements(ProcessLoadedAchievements);
                    signedIn = true;
                    string userInfo = "Username: " + Social.localUser.userName +
                        "\nUser ID: " + Social.localUser.id +
                        "\nIsUnderage: " + Social.localUser.underage;
                    Debug.Log(userInfo);
                }
                else
                    Debug.Log("Failed to authenticate");
            });
        }
        else
        {
            PlayGamesPlatform.Instance.SignOut();
            signedIn = false;
        }
    }


    public void ViewLeaderboard()
    {
        //Social.ShowLeaderboardUI();
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.ShowLeaderboardUI(Constants.leaderboard_cave_crawler_leaderboard);
    }

    // This function gets called when the LoadAchievement call completes
    void ProcessLoadedAchievements(IAchievement[] achievements)
    {
        if (achievements != null && achievements.Length == 0)
            Debug.Log("Error: no achievements found");
        else
            Debug.Log("Got " + achievements.Length + " achievements");

        // You can also call into the functions like this
        Social.ReportProgress("Achievement01", 100.0, result =>
        {
            if (result)
                Debug.Log("Successfully reported achievement progress");
            else
                Debug.Log("Failed to report achievement");
        });
    }

    public void SaveGame()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            Debug.Log("Saving game");
            PlayerState.Instance.SaveToGlobal();

            StatManager.Instance.LocalCopyOfPlayerData.SceneID = SceneManager.GetActiveScene().buildIndex;
            StatManager.Instance.SaveData();
        }
    }

    public void LoadGame()
    {
        LoadSingleplayerCave();
    }

    public void LoadMultiplayerCave()
    {
        
        NetworkManager nm = GameObject.FindObjectOfType<NetworkManager>();
        //NetworkServer.serverHostId;
        nm.StartHost();
        nm.serverBindAddress = Network.player.ipAddress;
        nm.StartMatchMaker();
        nm.matchMaker.SetProgramAppID((UnityEngine.Networking.Types.AppID)1036802);

       /* CreateMatchRequest create = new CreateMatchRequest();
        create.name = "NewRoom";
        create.size = 2;
        create.advertise = true;
        create.password = "";*/

       // networkMatch.CreateMatch(create, OnMatchCreate);
        LoadScene(4);
     //   Debug.Log("address " + nm.serverBindAddress);
    }

   /* public void OnMatchJoined(JoinMatchResponse matchJoin)
    {
        if (matchJoin.success)
        {
            Debug.Log("Join match succeeded");
            if (matchCreated)
            {
                Debug.LogWarning("Match already set up, aborting...");
                return;
            }
            Utility.SetAccessTokenForNetwork(matchJoin.networkId, new NetworkAccessToken(matchJoin.accessTokenString));
            NetworkClient myClient = new NetworkClient();
            myClient.RegisterHandler(MsgType.Connect, OnClientConnect);
            myClient.Connect(new MatchInfo(matchJoin));
        }
        else
        {
            Debug.LogError("Join match failed");
        }
    }*/

    public void OnMatchCreate(CreateMatchResponse matchResponse)
    {
        if (matchResponse.success)
        {
            Debug.Log("Create match succeeded");
            matchCreated = true;
            Utility.SetAccessTokenForNetwork(matchResponse.networkId, new NetworkAccessToken(matchResponse.accessTokenString));
            NetworkServer.Listen(new MatchInfo(matchResponse), 9000);
        }
        else
        {
            Debug.LogError("Create match failed");
        }
    }

    public void LoadClientMultiplayer()
    {
        LoadScene(4);
        NetworkManager nm = GameObject.FindObjectOfType<NetworkManager>();
        nm.networkAddress = Network.player.ipAddress;
        
        nm.StartMatchMaker();
        
        nm.matchMaker.SetProgramAppID((UnityEngine.Networking.Types.AppID)1036802);
        nm.StartClient();
        nm.client.RegisterHandler(MsgType.Connect, OnClientConnect);
        //nm.client.
       // nm.matchMaker.
        
       /* PlayGamesPlatform.Instance.TurnBased.CreateQuickMatch(MinOpponents, MaxOpponents,
        Variant, OnMatchStarted);*/
       // nm.client.Connect("localhost", 7777);
       // Debug.Log(nm.client.isConnected);
        Debug.Log(nm.numPlayers);
    }


    

    public void OnClientConnect(NetworkMessage netMsg)
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
        NetworkManager nm = GameObject.FindObjectOfType<NetworkManager>();
        if (level == 4) nm.GetComponent<NetworkManagerHUD>().showGUI = true;
        else nm.GetComponent<NetworkManagerHUD>().showGUI = false;

      //  if (Social.localUser.authenticated)
        {
            loadingImage.SetActive(true);            
            if (nm.isNetworkActive)
            {
                if (NetworkServer.active) nm.StopHost();
                else nm.StopClient();
            }
            SceneManager.LoadScene(level, LoadSceneMode.Single);
            Debug.Log("loading " + level);
            currentScene = level;
        }
        
        
    }
}
