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

    public GameObject RuntimeInstantiated;

    public void LoadFromGlobal()
    {
        StatManager sm = RuntimeInstantiated.GetComponentInChildren<StatManager>();
        gameUnits = new GameObject[sm.DeployedUnits.GetLength(0)];
        sm.DeployedUnits.CopyTo(gameUnits, 0);
        Score = sm.Score;
    }

    public override void Play(CellGrid cellGrid)
    {
        cellGrid.CellGridState = new CellGridStateWaitingForInput(cellGrid);
    }

    public void SaveStats()
    {
        StatManager sm = RuntimeInstantiated.GetComponentInChildren<StatManager>();
        gameUnits = allUnits.GetComponentsInChildren<GameObject>();
        sm.Score = Score;
        sm.DeployedUnits = new GameObject[gameUnits.GetLength(0)];
        gameUnits.CopyTo(sm.DeployedUnits, 0);
    }
}