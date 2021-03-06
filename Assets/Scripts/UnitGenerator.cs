﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class UnitGenerator : Singleton<UnitGenerator>, IUnitGenerator
{
    public Transform UnitsParent;
    public Transform CellsParent;
    public CellGrid CellGrid;
    public HexGridCellularAutomata Hex;
    bool instantiated = false;

    public HumanPlayer player;

    public GameObject CarrierPrefab;
    public GameObject SentinelPrefab;

    public Camera CarrierCamera;

    public IEnumerator SpawnUnits()
    {
        while (CellGrid.Cells == null)
            yield return 0;
        if (!instantiated)
        { 
            SpawnUnits(new List<Cell>(CellGrid.gameObject.transform.GetComponentsInChildren<Cell>()));
        }
        
    }

    /// <summary>
    /// Returns units that are already children of UnitsParent object.
    /// </summary>
    public List<Unit> SpawnUnits(List<Cell> cells)
    {
        if (instantiated) return null;
        instantiated = true;
        List<Unit> ret = new List<Unit>();
        player.LoadFromGlobal();

        if (StatManager.Instance.IsNewCave)
        {
            ret.Add(InstantiateUnit(player.gameUnits[0]));

            for (int i = 1; i < player.gameUnits.GetLength(0); ++i)
            {
                ret.Add(InstantiateUnit(player.gameUnits[i].gameObject, ret[i - 1].gameObject.GetComponent<GameUnit>().Cell.GetNeighbours(CellGrid.Cells)));
            }
        }
        
        else
        {
            foreach (GameObject gu in player.gameUnits)
                ret.Add(InstantiateUnit(gu, (gu.GetComponent<GameUnit>().Cell as Hexagon).i, (gu.GetComponent<GameUnit>().Cell as Hexagon).j));
        }
        
        CarrierCamera.gameObject.GetComponent<CameraController>().RelocateToPlayer();
        return ret;
    }

    Unit InstantiateUnit(GameObject prefab)
    {
        var cells = CellGrid.Cells;
        int[,] map = Hex.GetMap();
        System.Random rnd = new System.Random();
        int i = rnd.Next(Hex.width * Hex.height);
        var cell = CellGrid.gameObject.transform.GetChild(i).GetComponent<Cell>();

        while (cell == null || cell.IsTaken)
        {
            i = rnd.Next(Hex.width * Hex.height);
            cell = CellGrid.gameObject.transform.GetChild(i).GetComponent<Cell>();
        }

        return InitUnit(prefab, cell, i);
    }

    Unit InstantiateUnit(GameObject prefab, int i, int j)
    {
        var cell = Hex.cells[i, j];
        if (cell == null || cell.IsTaken)
            return null;

        return InitUnit(prefab, cell);
    }

    Unit InstantiateUnit(GameObject prefab, List<Cell> cells)
    {
        var c = GetRandomCloseFreeCell(cells);
        // random if the search for a cell close by failed to complete in time
        if (c == null) return InstantiateUnit(prefab);
        return InitUnit(prefab, c);
    }

    Unit InitUnit(GameObject prefab, Cell cell, int i = -1)
    {
        GameObject unit = Instantiate(prefab);
        cell.IsTaken = true;
        unit.GetComponent<GameUnit>().Cell = cell;
        unit.transform.position = cell.transform.position;
        unit.transform.parent = UnitsParent;
        if (i != -1) unit.GetComponent<GameUnit>().CellNumber = i;
        unit.GetComponent<GameUnit>().Initialize();
        return unit.GetComponent<GameUnit>();
    }

    Cell GetRandomCloseFreeCell(List<Cell> cells, int iteration = 0)
    {
        System.Random rnd = new System.Random();
        int i = rnd.Next(cells.Count);
        Cell c = null;
        foreach (Cell cell in cells)
        {
            if (cell != null && !cell.IsTaken)
            {
                c = cell;
                break;
            }
        }
        ++iteration;
        if (iteration == 10) return null;
        if (c == null)
            return GetRandomCloseFreeCell(cells[0].GetNeighbours(CellGrid.Cells));
        return c;
    }

    List<Unit> ManualSpawn(List<Cell> cells)
    {
        List<Unit> ret = new List<Unit>();
        for (int i = 0; i < UnitsParent.childCount; i++)
        {
            var unit = UnitsParent.GetChild(i).GetComponent<Unit>();
            if (unit != null)
            {
                var cell = cells.OrderBy(h => Math.Abs((h.transform.position - unit.transform.position).magnitude)).First();
                if (!cell.IsTaken)
                {
                    cell.IsTaken = true;
                    unit.Cell = cell;
                    unit.transform.position = cell.transform.position;
                    unit.Initialize();
                    ret.Add(unit);
                }//Unit gets snapped to the nearest cell
                else
                {
                    Destroy(unit.gameObject);
                }//If the nearest cell is taken, the unit gets destroyed.
            }
            else
            {
                Debug.LogError("Invalid object in Units Parent game object");
            }

        }
        return ret;
    }

    public void SnapToGrid()
    {
        List<Transform> cells = new List<Transform>();

        foreach (Transform cell in CellsParent)
        {
            cells.Add(cell);
        }

        foreach (Transform unit in UnitsParent)
        {
            var closestCell = cells.OrderBy(h => Math.Abs((h.transform.position - unit.transform.position).magnitude)).First();
            if (!closestCell.GetComponent<Cell>().IsTaken)
            {
                Vector3 offset = new Vector3(0, 0, closestCell.GetComponent<Cell>().GetCellDimensions().z);
                unit.position = closestCell.transform.position - offset;
            }//Unit gets snapped to the nearest cell
        }
    }
}
