using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Sentinel : GameUnit
{

    void Awake()
    {
        TotalMovementPoints = 2;
        TotalActionPoints = 0;
        HitPoints = 5;
        AttackRange = 4;
        AttackFactor = 5;
        DefenceFactor = 3;
        MovementPoints = 2;
        MovementSpeed = 3;
        ActionPoints = 4;
        canShoot = true;
        isDeployed = true;
    }
}
