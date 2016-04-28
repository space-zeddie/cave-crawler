using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class HexGCANetwork : HexGridCellularAutomata 
{
    void Start() { }

    void OnLevelWasLoaded(int level)
    {
        //GameObject.FindObjectOfType<NetworkManager>().StartHost();
        if (GameObject.FindObjectOfType<NetworkManager>().isNetworkActive) Debug.Log("Is in Network");
        base.LoadGrid(false);
    }
}
