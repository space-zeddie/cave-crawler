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
   // public int[] DeployedUnitCell;
    public int[] Spawns;

    public int[,] map;
    public int[] unitI;
    public int[] unitJ;

    public int[] spawnI;
    public int[] spawnJ;

    public int SceneID;
}