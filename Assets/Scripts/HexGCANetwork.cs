using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class HexGCANetwork : HexGridCellularAutomata 
{
    void Start() { }

    void OnLevelWasLoaded(int level)
    {
        NetworkManager nm = GameObject.FindObjectOfType<NetworkManager>();
        //GameObject.FindObjectOfType<NetworkManager>().StartHost();
        if (nm.isNetworkActive) Debug.Log("Is in Network");
        base.LoadGrid(false);
    }
}
