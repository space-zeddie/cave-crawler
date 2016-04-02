using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class HumanPlayer : Player
{
    public UnitParentScript allUnits;

    public GameObject[] gameUnits;
    int numberOfDeployedUnits = 2; // STUB
    public int Score;

    public PlayerStatistics localPlayerData = new PlayerStatistics();

    public void LoadFromGlobal()
    {
        localPlayerData = StatManager.Instance.savedPlayerData;
        gameUnits = new GameObject[localPlayerData.DeployedUnits.GetLength(0)];
        localPlayerData.DeployedUnits.CopyTo(gameUnits, 0);
        Score = localPlayerData.Score;
    }

    public override void Play(CellGrid cellGrid)
    {
        cellGrid.CellGridState = new CellGridStateWaitingForInput(cellGrid);
    }

    public void SaveStats()
    {
        StatManager.Instance.savedPlayerData = localPlayerData;
    }
}