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

    public int[,] map;
   // public int[] i;
   // public int[] j;

    public int SceneID;
}