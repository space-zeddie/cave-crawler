using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;

public class HumanPlayer : Player
{
    public UnitParentScript allUnits;
    public GameObject CarrierPrefab;
    public GameObject SentinelPrefab;

    PlayerState player_state;

    public GameObject[] gameUnits = new GameObject[0];
    public int Score;

   // void Start() { }

   // void OnLevelWasLoaded(int level)
    //void OnServerAddPlayer()
    void Start()
    {
        if (PlayersParent.Instance != null)
            this.gameObject.transform.parent = PlayersParent.Instance.gameObject.transform;
        if (UnitParentScript.Instance != null)
            allUnits = UnitParentScript.Instance;
        player_state = this.gameObject.GetComponent<PlayerState>();
        //Debug.Log("Started HumanPlayer " + PlayerNumber);
        this.gameObject.SetActive(true);
    }

    public void LoadFromGlobal()
    {
        if (player_state == null) player_state = this.gameObject.GetComponent<PlayerState>();
        if (!player_state.Loaded) player_state.LoadFromGlobal();
        if (player_state.LocalPlayerData.DeployedUnits == null || player_state.LocalPlayerData.DeployedUnits.GetLength(0) == 0)
        {
           // Debug.Log("Assigning Units");
            player_state.LocalPlayerData.DeployedUnits = new int[2];
            player_state.LocalPlayerData.DeployedUnits[0] = 1; // carrier
            player_state.LocalPlayerData.DeployedUnits[1] = 2; // sentinel
            player_state.LocalPlayerData.unitI = new int[2] { -1, -1 };
            player_state.LocalPlayerData.unitJ = new int[2] { -1, -1 };
          //  PlayerState.Instance.LocalPlayerData.DeployedUnitCell = new int[2]{-1, -1};
        }
        gameUnits = new GameObject[player_state.LocalPlayerData.DeployedUnits.GetLength(0)];
        //Debug.Log(PlayerState.Instance.LocalPlayerData.unitI);
        int i = 0;
        foreach (int unitTypeCode in player_state.LocalPlayerData.DeployedUnits)
        {
            if (unitTypeCode == 1) gameUnits[i] = CarrierPrefab;
            else if (unitTypeCode == 2) gameUnits[i] = SentinelPrefab;
//            Debug.Log(gameUnits[i]);
           // gameUnits[i].CellNumber = PlayerState.Instance.LocalPlayerData.DeployedUnitCell[i];
            if (!StatManager.Instance.IsNewCave)
            {
                gameUnits[i].GetComponent<GameUnit>().Cell = new FloorCell();
                (gameUnits[i].GetComponent<GameUnit>().Cell as Hexagon).i = player_state.LocalPlayerData.unitI[i];
                (gameUnits[i].GetComponent<GameUnit>().Cell as Hexagon).j = player_state.LocalPlayerData.unitJ[i];
            }
            ++i;
        }
        Score = player_state.LocalPlayerData.Score;
    }

    public override void Play(CellGrid cellGrid)
    {
        cellGrid.CellGridState = new CellGridStateWaitingForInput(cellGrid);
    }

    public void SaveStats()
    {
        player_state.LocalPlayerData.map = GameObject.FindObjectOfType<HexGridCellularAutomata>().GetMap();
        int i = 0;
        gameUnits = new GameObject[allUnits.transform.childCount];
        foreach (GameUnit gu in allUnits.GetComponentsInChildren<GameUnit>())
            gameUnits[i++] = gu.gameObject;
        player_state.LocalPlayerData.DeployedUnits = new int[gameUnits.GetLength(0)];
        player_state.LocalPlayerData.unitI = new int[gameUnits.GetLength(0)];
        player_state.LocalPlayerData.unitJ = new int[gameUnits.GetLength(0)];
        i = 0;
        foreach (GameObject gu in gameUnits)
        {
            if (gu.GetComponent<GameUnit>() is Carrier) player_state.LocalPlayerData.DeployedUnits[i] = 1;
            else if (gu.GetComponent<GameUnit>() is Sentinel) player_state.LocalPlayerData.DeployedUnits[i] = 2;
           // PlayerState.Instance.LocalPlayerData.DeployedUnitCell[i] = gu.CellNumber;
            player_state.LocalPlayerData.unitI[i] = (gu.GetComponent<GameUnit>().Cell as Hexagon).i;
            player_state.LocalPlayerData.unitJ[i] = (gu.GetComponent<GameUnit>().Cell as Hexagon).j;
            Debug.Log(player_state.LocalPlayerData.unitI[i] + ", " + player_state.LocalPlayerData.unitJ[i]);
            ++i;
        }
        player_state.LocalPlayerData.Score = Score;
    }
}