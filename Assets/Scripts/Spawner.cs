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
        NetworkServer.Spawn(hexgrid);
        GUIControllerScript.Instance.Init();
    }
	
}
