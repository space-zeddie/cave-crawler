using UnityEngine;
using System.Collections;

public class HexGCANetwork : HexGridCellularAutomata 
{
    void Awake()
    {
        Debug.Log("Awaking Hex GCA");
        base.LoadGrid();
    }
}
