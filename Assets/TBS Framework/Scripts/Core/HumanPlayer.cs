using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

class HumanPlayer : Player
{
    public UnitParentScript allUnits;
    int numberOfDeployedUnits = 2; // STUB

    public bool MovedAllUnits()
    {
        GameUnit[] gameUnits = allUnits.GetComponentsInChildren<GameUnit>();
        foreach (GameUnit gu in gameUnits)
        {
            if (gu.PlayerNumber == this.PlayerNumber && !gu.HasMoved())
                return false;
        }
        return true;
    }

    public override void Play(CellGrid cellGrid)
    {
        cellGrid.CellGridState = new CellGridStateWaitingForInput(cellGrid);
    }
}