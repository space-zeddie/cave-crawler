﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Spawner : NetworkBehaviour
{
    NetworkManager nm;
   // SyncListBool m_bools;

    void Awake()
    {
        nm = GameObject.FindObjectOfType<NetworkManager>();
    }

	//void OnStartServer () 
    void OnLevelWasLoaded(int level)
    {
        if (level == 4)
        {
            //  m_bools = new SyncListBool();
            ClientScene.AddPlayer(0);
            Debug.Log("Server Started in " + Application.loadedLevel + ", about to spawn client player");
            //GameObject player = (GameObject)Instantiate(nm.playerPrefab);
            // player.GetComponent<HumanPlayer>().PlayerNumber = GameObject.FindObjectOfType<PlayersParent>().gameObject.transform.childCount;

            // NetworkServer.Spawn(player);
        }
	}

    public void Spawn()
    {

    }
	
}
