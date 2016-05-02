using UnityEngine;
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

    public void Spawn()
    {
        ClientScene.AddPlayer(0);
        //nm.GetComponent<GridManager>().GenMap(hex.GetComponent<HexGCANetwork>());
        GameObject hexgrid = (GameObject)Instantiate(hex);
        // player.GetComponent<HumanPlayer>().PlayerNumber = GameObject.FindObjectOfType<PlayersParent>().gameObject.transform.childCount; 
        if (NetworkServer.active) NetworkServer.Spawn(hexgrid);
        else ClientScene.RegisterPrefab(hexgrid);
        GUIControllerNet.Instance.Init();
    }

    public GameObject Spawn(GameObject prefab, Vector3 position)
    {
        if (prefab.GetComponent<NetworkIdentity>() == null) return null;
        var go = (GameObject)Instantiate(prefab, position, Quaternion.identity);
        //if (go.GetComponent<GameUnit>() != null) go.GetComponent<GameUnit>().Initialize();
        NetworkServer.SpawnWithClientAuthority(go, LocalPlayer());
        Debug.Log("Spawned " + prefab + " on " + position + " with " + LocalPlayer() + " authority");
        return go;
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
