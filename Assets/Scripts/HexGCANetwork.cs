using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class HexGCANetwork : HexGridCellularAutomata 
{
    public bool mapped = false;

    void Awake()
    {
        NetworkManager nm = GameObject.FindObjectOfType<NetworkManager>();
        if (PlayersParent.Instance != null)
            this.gameObject.GetComponent<CellGrid>().PlayersParent = PlayersParent.Instance.gameObject.transform;
        if (UnitParentScript.Instance != null)
            this.gameObject.GetComponent<UnitGenerator>().UnitsParent = UnitParentScript.Instance.gameObject.transform;
        HumanPlayer thisPlayer = null;
        for (int i = 0; i < PlayersParent.Instance.gameObject.transform.childCount; ++i )
        {
            GameObject p = PlayersParent.Instance.gameObject.transform.GetChild(i).gameObject;
            if (p.GetComponent<NetworkIdentity>().isLocalPlayer || p.GetComponent<HumanPlayer>().PlayerNumber == 0)
                thisPlayer = p.GetComponent<HumanPlayer>();
        }
        if (thisPlayer != null)
            this.gameObject.GetComponent<UnitGenerator>().player = thisPlayer;
        GameObject obst = GameObject.FindGameObjectWithTag("ObstacleParent");
        GameObject maincam = GameObject.FindGameObjectWithTag("MainCamera");
        if (obst != null)
            this.gameObject.GetComponent<ObstacleGenerator>().ObstaclesParent = obst.transform;
        if (maincam != null)
            this.gameObject.GetComponent<UnitGenerator>().CarrierCamera = maincam.GetComponent<Camera>();
        //GameObject.FindObjectOfType<NetworkManager>().StartHost();
        if (nm.isNetworkActive) Debug.Log("Is in Network");
        if (!mapped) base.GenerateMap();
        base.LoadGrid(false);
        GUIControllerScript.Instance.CellGrid = this.gameObject.GetComponent<CellGrid>();
        UnitParentScript.Instance.hexGrid = this.gameObject;
    }
}
