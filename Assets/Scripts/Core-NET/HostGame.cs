﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Collections.Generic;

public class HostGame : MonoBehaviour
{
    List<MatchDesc> matchList = new List<MatchDesc>();
    NetworkManager nm;
    bool matchCreated;
    NetworkMatch networkMatch;
    Spawner spawner;
    NetworkClient remoteClient;
    JoinMatchResponse matchJoin;

    public GameObject spawnerPrefab;

    void Awake()
    {
        networkMatch = gameObject.AddComponent<NetworkMatch>();
        nm = GameObject.FindObjectOfType<NetworkManager>();
    }

    void OnGUI()
    {
        // You would normally not join a match you created yourself but this is possible here for demonstration purposes.
        if (GUILayout.Button("Create Cave"))
        {
            CreateMatchRequest create = new CreateMatchRequest();
            create.name = "NewRoom";
            create.size = 2;
            create.advertise = true;
            create.password = "";

            networkMatch.CreateMatch(create, OnMatchCreate);
        }

        if (GUILayout.Button("List rooms"))
        {
            networkMatch.ListMatches(0, 20, "", OnMatchList);
        }

        if (matchList.Count > 0)
        {
            GUILayout.Label("Current rooms");
        }
        foreach (var match in matchList)
        {
            if (GUILayout.Button(match.name))
            {
                networkMatch.JoinMatch(match.networkId, "", OnMatchJoined);
            }
        }
    }

    public void OnMatchCreate(CreateMatchResponse matchResponse)
    {
        if (matchResponse.success)
        {
            Debug.Log("Create match succeeded");
            matchCreated = true;
            Utility.SetAccessTokenForNetwork(matchResponse.networkId, new NetworkAccessToken(matchResponse.accessTokenString));
            NetworkServer.Listen(new MatchInfo(matchResponse), 9000);
            GameObject sp = (GameObject)GameObject.Instantiate(spawnerPrefab);
            if (NetworkServer.active) NetworkServer.Spawn(sp);
            else ClientScene.RegisterPrefab(sp);
            spawner = GameObject.FindObjectOfType<Spawner>();
            if (spawner != null) spawner.Spawn();

        }
        else
        {
            Debug.LogError("Create match failed");
        }
    }

    public void OnMatchList(ListMatchResponse matchListResponse)
    {
        if (matchListResponse.success && matchListResponse.matches != null)
        {
            networkMatch.JoinMatch(matchListResponse.matches[0].networkId, "", OnMatchJoined);
        }
    }

    public void OnMatchJoined(JoinMatchResponse matchJoin)
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
            NetworkClient myClient = nm.client;
            this.matchJoin = matchJoin;
            myClient.RegisterHandler(MsgType.Connect, OnClientConnect);
            myClient.Connect(new MatchInfo(matchJoin));
            GameObject sp = (GameObject)GameObject.Instantiate(spawnerPrefab);
            if (NetworkServer.active) NetworkServer.Spawn(sp);
            else ClientScene.RegisterPrefab(sp);
            if (spawner != null)
                spawner.AddClientPlayer();
        }
        else
        {
            Debug.LogError("Join match failed");
        }
    }

    public void SetUpRemoteClient(NetworkClient client)
    {
     //   remoteClient = client;
  //      remoteClient.RegisterHandler(MsgType.Connect, OnClientConnect);
//        remoteClient.Connect(new MatchInfo(matchJoin));
    }

    public void OnClientConnect(NetworkMessage msg)
    {
        Debug.Log("Connected!"); 
        //if (NetworkServer.active) NetworkServer.Spawn(sp);
        //else ClientScene.RegisterPrefab(sp);
//        spawner.AddClientPlayer();
        spawner = GameObject.FindObjectOfType<Spawner>();
        Debug.Log("where: " + nm.matchName);
        spawner.AddClientPlayer();
        //remoteClient.RegisterHandler(MsgType.Connect, OnClientConnect);
        //remoteClient.Connect(new MatchInfo(matchJoin));

    }
}