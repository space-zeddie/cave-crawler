using UnityEngine;
using System.Collections;

public class StatManager : Singleton<StatManager>
{
    public int Score;
    public GameUnit[] Units;
    public GameObject[] DeployedUnits;

    public GameObject CarrierPrefab;
    public GameObject SentinelPrefab;

    void Awake()
    {
        // STUB
        Score = 1;
        DeployedUnits = new GameObject[2];
        DeployedUnits[0] = CarrierPrefab;
        DeployedUnits[1] = SentinelPrefab;
    }

}
