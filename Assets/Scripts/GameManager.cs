using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;

public class GameManager : Singleton<GameManager> 
{
    public GameObject loadingImage;
    public Texture2D cursorTexture;
    
    private int currentScene;
    private int connectedTo;

    const int MinOpponents = 1;
    const int MaxOpponents = 7;
    const int Variant = 0;  // default

    bool signedIn = false;
    TurnBasedMatch currentMatch;

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
    }

    public void LoadClientMultiplayer()
    {
        LoadScene(4);
        NetworkManager nm = GameObject.FindObjectOfType<NetworkManager>();
        nm.StartClient();
        nm.client.RegisterHandler(MsgType.Connect, OnConnected);
        PlayGamesPlatform.Instance.TurnBased.CreateQuickMatch(MinOpponents, MaxOpponents,
        Variant, OnMatchStarted);
       // nm.client.Connect("localhost", 7777);
       // Debug.Log(nm.client.isConnected);
       // Debug.Log(nm.numPlayers);
    }

    // Callback:
    void OnMatchStarted(bool success, TurnBasedMatch match)
    {
        if (success)
        {
            Debug.Log("Match started");
            currentMatch = match;
        }
        else
        {
            Debug.Log("failed to match");
        }
    }

    public void FinMatch()
    {
        TurnBasedMatch match = currentMatch;  // our current match
        byte[] finalData = match.Data; // match data representing the final state of the match
        
        // define the match's outcome
        MatchOutcome outcome = new MatchOutcome();
        uint players = 0;
        foreach (Participant p in match.Participants) {
            // decide if participant p has won, lost or tied, and
            // their ranking (1st, 2nd, 3rd, ...):
            MatchOutcome.ParticipantResult result = MatchOutcome.ParticipantResult.Unset;
            uint placement = ++players;
            outcome.SetParticipantResult(p.ParticipantId, result, placement);
        }

        // finish the match
        PlayGamesPlatform.Instance.TurnBased.Finish(match, finalData, outcome, (bool success) => {
            if (success) {
                Debug.Log("Match finished");
            } else {
                Debug.Log("Match failed to finish");
            }
     });
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
        if (Social.localUser.authenticated)
        {
            loadingImage.SetActive(true);
            NetworkManager nm = GameObject.FindObjectOfType<NetworkManager>();
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
