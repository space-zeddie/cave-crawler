﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Spawner : NetworkBehaviour
{
    NetworkManager nm;
    public GameObject hex;
   // SyncListBool m_bools;

    void Awake()
    {
        nm = GameObject.FindObjectOfType<NetworkManager>();
        StatManager.Instance.IsNewCave = true;
    }

	//void OnStartServer () 
    void OnLevelWasLoaded(int level)
    {
        if (level == 4)
        {
            //  m_bools = new SyncListBool();
            Spawn();
        }
	}

    public override void OnStartClient()
    {
        if (!NetworkServer.active)
        {
            Debug.Log("spawning");
            Cmd_AddPlayer();
            GUIControllerNet.Instance.Init();
        }
    }

    [Command]
    void Cmd_AddPlayer()
    {
        ClientScene.AddPlayer(0);
    }

    public void Spawn()
    {
        Debug.Log("spawning");
        ClientScene.AddPlayer(0);
        //nm.GetComponent<GridManager>().GenMap(hex.GetComponent<HexGCANetwork>());
        GameObject hexgrid = (GameObject)Instantiate(hex);
        // player.GetComponent<HumanPlayer>().PlayerNumber = GameObject.FindObjectOfType<PlayersParent>().gameObject.transform.childCount; 
        if (NetworkServer.active) NetworkServer.Spawn(hexgrid);
        else
        {
            ClientScene.RegisterPrefab(hexgrid);
            Debug.Log("registered hexgrid on client");
        }
        GUIControllerNet.Instance.Init();
    }

    public GameObject Spawn(GameObject prefab, Vector3 position)
    {
        if (prefab.GetComponent<NetworkIdentity>() == null) return null;
        var go = (GameObject)Instantiate(prefab, position, Quaternion.identity);
        Cmd_Spawn(go);
        Debug.Log("Spawned " + prefab + " on " + position + " with " + LocalPlayer() + " authority");
        return go;
    }

    [Command]
    public void Cmd_Spawn(GameObject go)
    {
        NetworkServer.SpawnWithClientAuthority(go, LocalPlayer());
    }

    public GameObject LocalPlayer()
    {
        var players = GameObject.FindObjectOfType<PlayersParent>().gameObject.transform;
        GameObject player = null;
        for (int i = 0; i < players.childCount; ++i)
            if (players.GetChild(i).GetComponent<NetworkIdentity>().isLocalPlayer)
                player = players.GetChild(i).gameObject;
        return player;

    }
	
}
