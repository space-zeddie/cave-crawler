using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode()]
public class ObstacleGenerator : Singleton<ObstacleGenerator> {

    public GameObject ExitPrefab;
  //  public GameObject HexagonWallPrefab;

    public Transform ObstaclesParent;
    public CellGrid CellGrid;
    public HexGridCellularAutomata Hex;

    bool instantiated = false;

    public IEnumerator SpawnObstacles()
    {
        while (CellGrid.Cells == null)
            yield return 0;
        if (!PlayerState.Instance.Loaded) PlayerState.Instance.LoadFromGlobal();
        if (!instantiated && StatManager.Instance.IsNewCave)
        {
            InstantiateObstacle(ExitPrefab);
        }
        else
        {
            foreach (int type in PlayerState.instance.LocalPlayerData.Spawns)
            {
                if (type == 0)
                    InstantiateObstacle(ExitPrefab);
            }
        }
    }

    public void SaveSpawns()
    {
        GameObject parent = GameObject.FindGameObjectWithTag("ObstacleParent");
        PlayerState.Instance.LocalPlayerData.Spawns = new int[parent.transform.GetChildCount()];
        PlayerState.Instance.LocalPlayerData.spawnI = new int[parent.transform.GetChildCount()];
        PlayerState.Instance.LocalPlayerData.spawnJ = new int[parent.transform.GetChildCount()];
        int i = 0;
        foreach (Collectable coll in parent.transform.GetComponentsInChildren<Collectable>())
        {
            Debug.Log(i);
            if (coll is Exit)
            {
                PlayerState.Instance.LocalPlayerData.Spawns[i] = 0;
                PlayerState.Instance.LocalPlayerData.spawnI[i] = coll.i;
                PlayerState.Instance.LocalPlayerData.spawnJ[i] = coll.j;
            }
            
            ++i;
        }
    }

    void InstantiateObstacle(GameObject prefab)
    {
        if (instantiated) return;
        instantiated = true;
        var cells = CellGrid.Cells;
        int[,] map = Hex.GetMap();
        System.Random rnd = new System.Random();
        int i = rnd.Next(Hex.width * Hex.height);
        var cell = CellGrid.gameObject.transform.GetChild(i).GetComponent<Hexagon>();

        while (cell == null || cell.IsTaken)
        {
            i = rnd.Next(Hex.width * Hex.height);
            cell = CellGrid.gameObject.transform.GetChild(i).GetComponent<Hexagon>();
        }

        GameObject obstacle = Instantiate(prefab);
        obstacle.AddComponent<Collectable>();
        if (cell is FloorCell) (cell as FloorCell).spawn = obstacle.GetComponent<Collectable>();
        cell.IsTaken = true;
        obstacle.GetComponent<Collectable>().i = cell.i;
        obstacle.GetComponent<Collectable>().j = cell.j;
        Vector3 offset = new Vector3(0, 0, cell.GetCellDimensions().z);
        obstacle.transform.position = cell.transform.position - offset;
        obstacle.transform.parent = ObstaclesParent;
    }

    void InstantiateObstacle(GameObject prefab, int i, int j)
    {
        if (instantiated) return;
        instantiated = true;
        var cell = Hex.cells[i, j] as Hexagon;
        GameObject obstacle = Instantiate(prefab);
        obstacle.AddComponent<Collectable>();
        if (cell is FloorCell) (cell as FloorCell).spawn = obstacle.GetComponent<Collectable>();
        cell.IsTaken = true;
        obstacle.GetComponent<Collectable>().i = cell.i;
        obstacle.GetComponent<Collectable>().j = cell.j;
        Vector3 offset = new Vector3(0, 0, cell.GetCellDimensions().z);
        obstacle.transform.position = cell.transform.position - offset;
        obstacle.transform.parent = ObstaclesParent;
    }

    void ManualSpawn()
    {
        var cells = CellGrid.Cells;

        for (int i = 0; i < ObstaclesParent.childCount; i++)
        {
            var obstacle = ObstaclesParent.GetChild(i);

            var cell = cells.OrderBy(h => Math.Abs((h.transform.position - obstacle.transform.position).magnitude)).First();
            if (!cell.IsTaken)
            {
                cell.IsTaken = true;
                Vector3 offset = new Vector3(0, 0, cell.GetCellDimensions().z);
                obstacle.position = cell.transform.position - offset;
            }
            else
            {
                Destroy(obstacle.gameObject);
            }
        }
    }

    public void SnapToGrid()
    {
        List<Transform> cells = new List<Transform>();

        foreach (Transform cell in CellGrid.transform)
        {
            cells.Add(cell);
        }

        foreach (Transform obstacle in ObstaclesParent)
        {
            var closestCell = cells.OrderBy(h => Math.Abs((h.transform.position - obstacle.transform.position).magnitude)).First();
            if (!closestCell.GetComponent<Cell>().IsTaken)
            {
                Vector3 offset = new Vector3(0, 0, closestCell.GetComponent<Cell>().GetCellDimensions().z);
                obstacle.position = closestCell.transform.position - offset;
            }
        }
    }
}
