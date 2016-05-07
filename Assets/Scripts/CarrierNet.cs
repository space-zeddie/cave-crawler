using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

[Serializable]
public class CarrierNet : GameUnitNet 
{  
    void Awake()
    {
        TotalMovementPoints = 2;
        TotalActionPoints = 0;
        AttackRange = 0;
        AttackFactor = 0;
        DefenceFactor = 5;
        MovementPoints = 2;
        MovementSpeed = 3;
        ActionPoints = 3;
        canShoot = false;
        isDeployed = true;
    }

}
