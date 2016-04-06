using System;
using UnityEngine;

[Serializable]
public class PlayerStatistics
{
    public int Score;
    // saved by type number
    public int[] Units;
    // saved by type number
    public int[] DeployedUnits;
    // saved by the number of the cell in the hierarchy
    public int[] DeployedUnitCell;
    // saved by the number of the cell in the hierarchy
    public int[] SpawnCells;

    public int[,] map;

    public int SceneID;
}