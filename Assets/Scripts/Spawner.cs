using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Spawner : NetworkBehaviour
{
    NetworkManager nm;
    public GameObject hex;
    public GameObject player;
    public static bool updated = false;
   // SyncListBool m_bools;

    void Awake()
    {
        nm = GameObject.FindObjectOfType<NetworkManager>();
        StatManager.Instance.IsNewCave = true;
    }

    void Start()
    {
        if (!NetworkServer.active) 
        { 
           // Debug.Log("added player on client"); 
            //ClientScene.AddPlayer(1);  
             Debug.Log("connecting client to local server"); 
            ClientScene.ConnectLocalServer(); 
        }
     //   player = LocalPlayer();
     //   Debug.Log("Active local player:" + GameObject.FindObjectOfType<Spawner>().LocalPlayer());
     //   Debug.Log(nm.numPlayers);
       // Debug.Log("Local players: " + ClientScene.localPlayers.Count);
    }


    public override void OnStartClient()
    {
        if (!NetworkServer.active)
        {
            ClientScene.AddPlayer(1);
        }
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

    /*public override void OnStartClient()
    {
        if (!NetworkServer.active)
        {
            Debug.Log("spawning client");
            ClientScene.AddPlayer(nm.client.connection, 1);
            GUIControllerNet.Instance.Init();
        }
    }*/

    /*public override void OnClientReady()
    {
        if (!NetworkServer.active)
        {
            Debug.Log("spawning client");
            ClientScene.AddPlayer(nm.client.connection, 0);
            GUIControllerNet.Instance.Init();
        }
    }*/

    public void Spawn()
    {
        Debug.Log("spawning");
        ClientScene.ConnectLocalServer();
        nm.client = NetworkClient.allClients[0];
        nm.client.Connect("localhost", 7777);
      //  Debug.Log(NetworkClient.allClients[0].connection.address);
        ClientScene.AddPlayer(NetworkServer.localConnections[0], 0);
        GameObject player = (GameObject)Instantiate(nm.playerPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(NetworkServer.localConnections[0], player, 0);
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
        player = LocalPlayer();
        Debug.Log(player);
        player.GetComponent<HumanPlayerNet>().Cmd_Spawn(go);
        Debug.Log("LocalPlayer " + LocalPlayer());
        Debug.Log("Spawned " + prefab); //+ " on " + position + " with " + LocalPlayer() + " authority");
        return go;
    }

    public GameObject LocalPlayer()
    {
        var players = GameObject.FindObjectOfType<PlayersParent>().gameObject.transform;
        GameObject player = null;
        for (int i = 0; i < players.childCount; ++i)
            if (!NetworkServer.active && players.GetChild(i).GetComponent<HumanPlayerNet>().PlayerNumber == 1)
                player = players.GetChild(i).gameObject;
            else if (NetworkServer.active && players.GetChild(i).GetComponent<HumanPlayerNet>().PlayerNumber == 0)
                player = players.GetChild(i).gameObject;
        Debug.Log(player);
        return player;

    }
	
}
