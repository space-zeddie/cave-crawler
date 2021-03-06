﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;

public class HumanPlayerNet : PlayerNet
{
    public UnitParentScript allUnits;
    public GameObject CarrierPrefab;
    public GameObject SentinelPrefab;

    PlayerState player_state;

    public GameObject[] gameUnits = new GameObject[0];
    public int Score;

    // void Start() { }

    // void OnLevelWasLoaded(int level)
    //void OnServerAddPlayer()
    void Start()
    {
        if (PlayersParent.Instance != null)
            this.gameObject.transform.parent = PlayersParent.Instance.gameObject.transform;
        if (UnitParentScript.Instance != null)
            allUnits = UnitParentScript.Instance;
        this.PlayerNumber = PlayersParent.Instance.gameObject.transform.childCount - 2;
        if (PlayerNumber == 2 && !NetworkServer.active) ClientScene.SetLocalObject(this.GetComponent<NetworkIdentity>().netId, this.gameObject);
        player_state = this.gameObject.GetComponent<PlayerState>();
        //Debug.Log("Started HumanPlayer " + PlayerNumber);
        this.gameObject.SetActive(true);
        Debug.Log("started Human Player " + PlayerNumber);
    }

    void FixedUpdate()
    {
        if (PlayersParent.Instance.gameObject.transform.childCount >= 2 && !Spawner.updated && !NetworkServer.active)
        {
            if (GameObject.FindGameObjectWithTag("Grid") != null)
            {
                var go = GameObject.FindGameObjectWithTag("Grid").GetComponent<CellGridNet>().CalculatePlayers();
                if (go != null)
                {
                    Debug.Log("FixedUpdate");
                    GameObject.FindGameObjectWithTag("Grid").GetComponent<CellGridNet>().CreateUnits();
                    Spawner.updated = true;
                    GameObject.FindGameObjectWithTag("Grid").GetComponent<CellGridNet>().StartGame();
                }
            }
           
        }
    }

    [Command]
    public void Cmd_Spawn(GameObject go)
    {
        Debug.Log("Spawning for player number: " + PlayerNumber + " an object " + go);
        if (go.GetComponent<GameUnitNet>() != null)
            go.GetComponent<GameUnitNet>().PlayerNumber = PlayerNumber;
        NetworkServer.Spawn(go);
    }

    [Command]
    public void Cmd_SpawnUnits()
    {
        GameObject.FindObjectOfType<UnitGeneratorNet>().SpawnUnitsForClient(this);
    }

    [Command]
    public void Cmd_HighlightAvailableCells(bool isCarrier)
    {
        GameUnitNet unit = null;
        foreach (GameUnitNet gunit in allUnits.GetComponentsInChildren<GameUnitNet>())
        {
            if (isCarrier && gunit is CarrierNet) unit = gunit;
            else if (!isCarrier && gunit is SentinelNet) unit = gunit;
        }
        if (unit == null) return;
        unit.available = null;
        unit.PopulateAvailableCells();
        //Debug.Log(available.Count);
        foreach (CellNet cell in unit.available)
        {
            (cell as HexagonNet).MarkAsReachable();
            if (cell is FloorCellNet)
                (cell as FloorCellNet).moveable = unit;
            else if (cell is WallCellNet && unit.canShoot)
                (cell as WallCellNet).moveable = unit;
        }
    }

    void OnServerAddPlayer()
    {
        Debug.Log("server added local player");
    }


    public void LoadFromGlobal()
    {
        if (player_state == null) player_state = this.gameObject.GetComponent<PlayerState>();
        if (!player_state.Loaded) player_state.LoadFromGlobal();
        if (player_state.LocalPlayerData.DeployedUnits == null || player_state.LocalPlayerData.DeployedUnits.GetLength(0) == 0)
        {
            // Debug.Log("Assigning Units");
            player_state.LocalPlayerData.DeployedUnits = new int[2];
            player_state.LocalPlayerData.DeployedUnits[0] = 1; // carrier
            player_state.LocalPlayerData.DeployedUnits[1] = 2; // sentinel
            player_state.LocalPlayerData.unitI = new int[2] { -1, -1 };
            player_state.LocalPlayerData.unitJ = new int[2] { -1, -1 };
            //  PlayerState.Instance.LocalPlayerData.DeployedUnitCell = new int[2]{-1, -1};
        }
        gameUnits = new GameObject[player_state.LocalPlayerData.DeployedUnits.GetLength(0)];
        //Debug.Log(PlayerState.Instance.LocalPlayerData.unitI);
        int i = 0;
        foreach (int unitTypeCode in player_state.LocalPlayerData.DeployedUnits)
        {
            if (unitTypeCode == 1) gameUnits[i] = CarrierPrefab;
            else if (unitTypeCode == 2) gameUnits[i] = SentinelPrefab;
            //            Debug.Log(gameUnits[i]);
            // gameUnits[i].CellNumber = PlayerState.Instance.LocalPlayerData.DeployedUnitCell[i];
            if (!StatManager.Instance.IsNewCave)
            {
                gameUnits[i].GetComponent<GameUnitNet>().Cell = new FloorCellNet();
                (gameUnits[i].GetComponent<GameUnitNet>().Cell as HexagonNet).i = player_state.LocalPlayerData.unitI[i];
                (gameUnits[i].GetComponent<GameUnitNet>().Cell as HexagonNet).j = player_state.LocalPlayerData.unitJ[i];
            }
            ++i;
        }
        Score = player_state.LocalPlayerData.Score;
    }

    public override void Play(CellGridNet cellGrid)
    {
        Debug.Log("play called");
        cellGrid.CellGridState = new CellGridStateWaitingForInput(cellGrid);
    }

    public void SaveStats()
    {
        player_state.LocalPlayerData.Score = Score;
    }
}