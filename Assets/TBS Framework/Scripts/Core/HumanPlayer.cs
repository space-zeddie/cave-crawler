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

    public void LoadFromGlobal()
    {
        PlayerState.Instance.LoadFromGlobal();
        if (PlayerState.Instance.LocalPlayerData.DeployedUnits == null || PlayerState.Instance.LocalPlayerData.DeployedUnits.GetLength(0) == 0)
        {
            PlayerState.Instance.LocalPlayerData.DeployedUnits = new int[2];
            PlayerState.Instance.LocalPlayerData.DeployedUnits[0] = 1; // carrier
            PlayerState.Instance.LocalPlayerData.DeployedUnits[1] = 2; // sentinel
            PlayerState.Instance.LocalPlayerData.DeployedUnitCell = new int[2]{-1, -1};
        }
        gameUnits = new GameUnit[PlayerState.Instance.LocalPlayerData.DeployedUnits.GetLength(0)];
        int i = 0;
        foreach (int unitTypeCode in PlayerState.Instance.LocalPlayerData.DeployedUnits)
        {
            if (unitTypeCode == 1) gameUnits[i] = CarrierPrefab;
            else if (unitTypeCode == 2) gameUnits[i] = SentinelPrefab;
            gameUnits[i].CellNumber = PlayerState.Instance.LocalPlayerData.DeployedUnitCell[i];
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
        int i = 0;
        foreach (GameUnit gu in gameUnits)
        {
            if (gu is Carrier) PlayerState.Instance.LocalPlayerData.DeployedUnits[i] = 1;
            else if (gu is Sentinel) PlayerState.Instance.LocalPlayerData.DeployedUnits[i] = 2;
            PlayerState.Instance.LocalPlayerData.DeployedUnitCell[i] = gu.CellNumber;
            ++i;
        }
        PlayerState.Instance.LocalPlayerData.Score = Score;
    }
}