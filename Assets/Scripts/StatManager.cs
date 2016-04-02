using UnityEngine;
using System.Collections;

public class StatManager : Singleton<StatManager>
{
    public PlayerStatistics savedPlayerData = new PlayerStatistics();

    public GameObject CarrierPrefab;
    public GameObject SentinelPrefab;

    void Awake()
    {
        // STUB
        savedPlayerData.Score = 1;
        savedPlayerData.DeployedUnits = new GameObject[2];
        savedPlayerData.DeployedUnits[0] = CarrierPrefab;
        savedPlayerData.DeployedUnits[1] = SentinelPrefab;
    }

}
