using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;

public class GridManager : NetworkBehaviour
{
    bool mapped = false;

    public void GenMap(HexGCANetwork hex)
    {
        Debug.Log("Method called");
        if (!mapped)
        {
            Debug.Log("Mapping happened");
            hex.GenerateMap();
        }
    }
}
