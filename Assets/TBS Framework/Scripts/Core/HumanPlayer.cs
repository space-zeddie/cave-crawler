using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class HumanPlayer : Player
{
    public UnitParentScript allUnits;
    public GameUnit CarrierPrefab;
    public GameUnit SentinelPrefab;

    public GameUnit[] gameUnits = new GameUnit[0];
    public int Score;

    void Start() { }

    void OnLevelWasLoaded(int level)
    {
        if (PlayersParent.Instance != null)
            this.gameObject.transform.parent = PlayersParent.Instance.gameObject.transform;
        if (UnitParentScript.Instance != null)
            allUnits = UnitParentScript.Instance;
        this.gameObject.SetActive(true);
    }

    public void LoadFromGlobal()
    {
        if (!PlayerState.Instance.Loaded) PlayerState.Instance.LoadFromGlobal();
        if (PlayerState.Instance.LocalPlayerData.DeployedUnits == null || PlayerState.Instance.LocalPlayerData.DeployedUnits.GetLength(0) == 0)
        {
            PlayerState.Instance.LocalPlayerData.DeployedUnits = new int[2];
            PlayerState.Instance.LocalPlayerData.DeployedUnits[0] = 1; // carrier
            PlayerState.Instance.LocalPlayerData.DeployedUnits[1] = 2; // sentinel
            PlayerState.Instance.LocalPlayerData.unitI = new int[2] { -1, -1 };
            PlayerState.Instance.LocalPlayerData.unitJ = new int[2] { -1, -1 };
          //  PlayerState.Instance.LocalPlayerData.DeployedUnitCell = new int[2]{-1, -1};
        }
        gameUnits = new GameUnit[PlayerState.Instance.LocalPlayerData.DeployedUnits.GetLength(0)];
        Debug.Log(PlayerState.Instance.LocalPlayerData.unitI);
        int i = 0;
        foreach (int unitTypeCode in PlayerState.Instance.LocalPlayerData.DeployedUnits)
        {
            if (unitTypeCode == 1) gameUnits[i] = CarrierPrefab;
            else if (unitTypeCode == 2) gameUnits[i] = SentinelPrefab;
           // gameUnits[i].CellNumber = PlayerState.Instance.LocalPlayerData.DeployedUnitCell[i];
            if (!StatManager.Instance.IsNewCave)
            {
                gameUnits[i].Cell = new FloorCell();
                (gameUnits[i].Cell as Hexagon).i = PlayerState.Instance.LocalPlayerData.unitI[i];
                (gameUnits[i].Cell as Hexagon).j = PlayerState.Instance.LocalPlayerData.unitJ[i];
            }
            ++i;
        }
        Score = PlayerState.Instance.LocalPlayerData.Score;
    }

    public override void Play(CellGrid cellGrid)
    {
        cellGrid.CellGridState = new CellGridStateWaitingForInput(cellGrid);
    }

    public void SaveStats()
    {
        PlayerState.Instance.LocalPlayerData.map = GameObject.FindObjectOfType<HexGridCellularAutomata>().GetMap();
        gameUnits = allUnits.GetComponentsInChildren<GameUnit>();
        PlayerState.Instance.LocalPlayerData.DeployedUnits = new int[gameUnits.GetLength(0)];
        PlayerState.Instance.LocalPlayerData.unitI = new int[gameUnits.GetLength(0)];
        PlayerState.Instance.LocalPlayerData.unitJ = new int[gameUnits.GetLength(0)];
        int i = 0;
        foreach (GameUnit gu in gameUnits)
        {
            if (gu is Carrier) PlayerState.Instance.LocalPlayerData.DeployedUnits[i] = 1;
            else if (gu is Sentinel) PlayerState.Instance.LocalPlayerData.DeployedUnits[i] = 2;
           // PlayerState.Instance.LocalPlayerData.DeployedUnitCell[i] = gu.CellNumber;
            PlayerState.Instance.LocalPlayerData.unitI[i] = (gu.Cell as Hexagon).i;
            PlayerState.Instance.LocalPlayerData.unitJ[i] = (gu.Cell as Hexagon).j;
            Debug.Log(PlayerState.Instance.LocalPlayerData.unitI[i] + ", " + PlayerState.Instance.LocalPlayerData.unitJ[i]);
            ++i;
        }
        PlayerState.Instance.LocalPlayerData.Score = Score;
    }
}