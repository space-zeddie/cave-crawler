using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using System;

[ExecuteInEditMode()]
public class HexGCANetwork : ICellGridGeneratorNet
{
    public GameObject HexagonFreePrefab;
    public GameObject HexagonWallPrefab;
    [SyncVar]
    public int height;
    [SyncVar]
    public int width;
    public bool useRandomSeed;
    public bool adjustFillPercent = true;
    [SyncVar]
    public string seed;
    [Range(0, 100)]
    public int randomFillPercent;
    int[,] map;
    public CellNet[,] cells;
    HexGridType hexGridType;

    [SyncVar]
    bool mapped = false;
   // SyncListInt _syncMap = new SyncListInt();

    void Awake()
    {
        NetworkManager nm = GameObject.FindObjectOfType<NetworkManager>();
        if (PlayersParent.Instance != null)
            this.gameObject.GetComponent<CellGridNet>().PlayersParent = PlayersParent.Instance.gameObject.transform;
        if (UnitParentScript.Instance != null)
            this.gameObject.GetComponent<UnitGeneratorNet>().UnitsParent = UnitParentScript.Instance.gameObject.transform;
        HumanPlayerNet thisPlayer = null;
        for (int i = 0; i < PlayersParent.Instance.gameObject.transform.childCount; ++i)
        {
            GameObject p = PlayersParent.Instance.gameObject.transform.GetChild(i).gameObject;
            if (p.GetComponent<NetworkIdentity>().isLocalPlayer || p.GetComponent<HumanPlayerNet>().PlayerNumber == 0)
                thisPlayer = p.GetComponent<HumanPlayerNet>();
        }
        if (thisPlayer != null)
            this.gameObject.GetComponent<UnitGeneratorNet>().player = thisPlayer;
        GameObject obst = GameObject.FindGameObjectWithTag("ObstacleParent");
        GameObject maincam = GameObject.FindGameObjectWithTag("MainCamera");
        GameObject maincam1 = GameObject.FindGameObjectWithTag("MainCamera1");
        if (obst != null)
            this.gameObject.GetComponent<ObstacleGeneratorNet>().ObstaclesParent = obst.transform;
        if (maincam != null)
            this.gameObject.GetComponent<UnitGeneratorNet>().CarrierCamera = maincam.GetComponent<Camera>();
        if (maincam1 != null)
            this.gameObject.GetComponent<UnitGeneratorNet>().CarrierCamera1 = maincam1.GetComponent<Camera>();
        if (nm.isNetworkActive) Debug.Log("Is in Network");
        if (NetworkServer.active)
        {
            if (!mapped) GenerateMap();
            //UpdateSyncedMap();
            Debug.Log("generated map");
            LoadGrid();
        }
        else
        {
           // GetMapFromSyncMap();
            LoadGridForClient();
        }
        CellGridNet cgrid = this.gameObject.GetComponent<CellGridNet>();
        GUIControllerNet.Instance.CellGrid = cgrid;
       // this.gameObject.GetComponent<CellGridNet>().CellGridState = new CellGridStateWaitingForInput(cgrid);
       // this.gameObject.GetComponent<CellGridNet>().CreateUnits();
    }

    protected void LoadGrid()
    {
        ClearGrid();
        Debug.Log("New map");
        GenerateMap();
            GenerateGrid();
        StartCoroutine(this.gameObject.GetComponent<ObstacleGeneratorNet>().SpawnObstacles());
        StartCoroutine(this.gameObject.GetComponent<UnitGeneratorNet>().SpawnUnits());
    }

    void LoadGridForClient()
    {
        StatManager.Instance.LoadData();
        if (PlayerState.Instance != null && !PlayerState.Instance.Loaded) PlayerState.Instance.LoadFromGlobal();
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log("Local player started");
        if (!NetworkServer.active)
        {
            HumanPlayerNet thisPlayer = null;
            for (int i = 0; i < PlayersParent.Instance.gameObject.transform.childCount; ++i)
            {
                GameObject p = PlayersParent.Instance.gameObject.transform.GetChild(i).gameObject;
                if (p.GetComponent<NetworkIdentity>().isLocalPlayer || p.GetComponent<HumanPlayerNet>().PlayerNumber == 0)
                    thisPlayer = p.GetComponent<HumanPlayerNet>();
            }
            if (thisPlayer != null)
                this.gameObject.GetComponent<UnitGeneratorNet>().player = thisPlayer;
            Debug.Log("This player : " + thisPlayer.PlayerNumber);
        }
    }

    /*public void UpdateSyncedMap()
    {
        _syncMap = new SyncListInt();
        for (int j = 0; j < width; ++j)
        {
            for (int i = 0; i < height; ++i)
            {
                _syncMap.Add(map[i, j]);
            }
        }
    }

    public void GetMapFromSyncMap()
    {
        map = new int[width, height];
        for (int x = 0; x < _syncMap.Count; ++x)
        {
            int i = x % width;
            int j = x / width;
            map[i, j] = _syncMap[x];
        }
    }*/

    

    public void ClearGrid()
    {
        var children = new List<GameObject>();
        foreach (Transform cell in this.CellsParent)
        {
            children.Add(cell.gameObject);
        }
        children.ForEach(c => DestroyImmediate(c));
    }

    public override List<CellNet> GenerateGrid()
    {
        hexGridType = width % 2 == 0 ? HexGridType.even_q : HexGridType.odd_q;
        var hexagons = new List<CellNet>();
        cells = new CellNet[width, height];

        if (HexagonFreePrefab.GetComponent<HexagonNet>() == null || HexagonWallPrefab.GetComponent<HexagonNet>() == null)
        {
            Debug.LogError("Invalid hexagon prefabs provided");
            return hexagons;
        }
        int adjust = (int)((width * height) / 1000);
        Debug.Log("Grid Adjusted by " + adjust);
        if (adjustFillPercent) randomFillPercent += adjust;

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                GameObject hexagon = InstantiateHexagon(j, i);
                hexagons.Add(hexagon.GetComponent<CellNet>());
                cells[j, i] = hexagon.GetComponent<CellNet>();
            }
        }
        return hexagons;
    }

    GameObject InstantiateHexagon(int i, int j)
    {
        GameObject hexagon;
        if (IsInMapRange(i, j, width, height) && map[i, j] == 1)
        {
            hexagon = Instantiate(HexagonWallPrefab);
            hexagon.GetComponent<HexagonNet>().IsTaken = true;
        }
        else hexagon = Instantiate(HexagonFreePrefab);

        var hexSize = hexagon.GetComponent<CellNet>().GetCellDimensions();

        hexagon.transform.position = new Vector3((i * hexSize.x * 0.75f), (j * hexSize.y) + (i % 2 == 0 ? 0 : hexSize.y * 0.5f));
        hexagon.GetComponent<HexagonNet>().OffsetCoord = new Vector2(width - i - 1, height - j - 1);
        hexagon.GetComponent<HexagonNet>().HexGridType = hexGridType;
        hexagon.GetComponent<HexagonNet>().MovementCost = 1;
        hexagon.GetComponent<HexagonNet>().i = i;
        hexagon.GetComponent<HexagonNet>().j = j;
        hexagon.transform.parent = CellsParent;
        //if (NetworkServer.active)
       // {
           // hexagon.GetComponent<CellNet>().parentNetId = this.netId;
            NetworkServer.Spawn(hexagon);
       // }
       // else ClientScene.RegisterPrefab(hexagon);

        return hexagon;
    }

    public void DemolishWallOnCell(WallCellNet wallCell)
    {
        int i = (wallCell as HexagonNet).i;
        int j = (wallCell as HexagonNet).j;

        map[i, j] = 0;
        Destroy(wallCell);
        GameObject hex = InstantiateHexagon(i, j);
    }

    public int[,] GetMap()
    {
        return map;
    }

    static bool IsInMapRange(int x, int y, int width, int height)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public void GenerateMap()
    {
        map = new int[width, height];
        RandomFillMap();

        SmoothMap();
        SmoothMap();
        EnsureConnectivity();
    }

    void RandomFillMap()
    {
        if (useRandomSeed && NetworkServer.active)
            seed = Time.time.ToString();

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    map[x, y] = 1;
                else
                    map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > 4)
                    map[x, y] = 1;
                else if (neighbourWallTiles < 3)
                    map[x, y] = 0;

            }
        }

    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX == gridX && neighbourY == gridY) continue;
                if (IsInMapRange(neighbourX, neighbourY, width, height))
                    wallCount += map[neighbourX, neighbourY];
                else
                    wallCount++;
            }
        }
        if (IsInMapRange(gridX + 1, gridY, width, height)) wallCount += map[gridX + 1, gridY];

        return wallCount;
    }

    struct Coord
    {
        public int tileX;
        public int tileY;

        public Coord(int x, int y)
        {
            tileX = x;
            tileY = y;
        }
    }


    class Room : IComparable<Room>
    {
        public List<Coord> tiles;
        public List<Coord> edgeTiles;
        public List<Room> connectedRooms;
        public int roomSize;
        public bool isAccessibleFromMainRoom;
        public bool isMainRoom;

        public Room()
        {
        }

        public Room(List<Coord> roomTiles, int[,] map, int width, int height)
        {
            tiles = roomTiles;
            roomSize = tiles.Count;
            connectedRooms = new List<Room>();

            edgeTiles = new List<Coord>();
            foreach (Coord tile in tiles)
            {
                for (int x = tile.tileX - 1; x <= tile.tileX; x++)
                {
                    for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                    {
                        if ((x != tile.tileX || y != tile.tileY) && map[x, y] == 1)
                            edgeTiles.Add(tile);
                    }
                }
                if (IsInMapRange(tile.tileX + 1, tile.tileY, width, height) && map[tile.tileX + 1, tile.tileY] == 1) edgeTiles.Add(tile);
            }
        }

        public void SetAccessibleFromMainRoom()
        {
            if (!isAccessibleFromMainRoom)
            {
                isAccessibleFromMainRoom = true;
                foreach (Room connectedRoom in connectedRooms)
                {
                    connectedRoom.SetAccessibleFromMainRoom();
                }
            }
        }

        public static void ConnectRooms(Room roomA, Room roomB)
        {
            if (roomA.isAccessibleFromMainRoom)
            {
                roomB.SetAccessibleFromMainRoom();
            }
            else if (roomB.isAccessibleFromMainRoom)
            {
                roomA.SetAccessibleFromMainRoom();
            }
            roomA.connectedRooms.Add(roomB);
            roomB.connectedRooms.Add(roomA);
        }

        public bool IsConnected(Room otherRoom)
        {
            return connectedRooms.Contains(otherRoom);
        }

        public int CompareTo(Room otherRoom)
        {
            return otherRoom.roomSize.CompareTo(roomSize);
        }
    }

    List<Coord> GetRegionTiles(int startX, int startY)
    {
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[width, height];
        int tileType = map[startX, startY];

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);
            int x = 0, y = 0;
            for (x = tile.tileX - 1; x <= tile.tileX; x++)
            {
                for (y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if (IsInMapRange(x, y, width, height) && (y != tile.tileY || x != tile.tileX))
                    {
                        if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                        {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
            x = tile.tileX + 1; y = tile.tileY;
            if (IsInMapRange(x, y, width, height) && mapFlags[x, y] == 0 && map[x, y] == tileType)
            {
                mapFlags[x, y] = 1;
                queue.Enqueue(new Coord(x, y));
            }
        }
        return tiles;
    }

    List<List<Coord>> GetRegions(int tileType)
    {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                {
                    List<Coord> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);

                    foreach (Coord tile in newRegion)
                    {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }

        return regions;
    }

    List<Coord> GetLine(Coord from, Coord to)
    {
        List<Coord> line = new List<Coord>();

        int x = from.tileX;
        int y = from.tileY;

        int dx = to.tileX - from.tileX;
        int dy = to.tileY - from.tileY;

        bool inverted = false;
        int step = Math.Sign(dx);
        int gradientStep = Math.Sign(dy);

        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if (longest < shortest)
        {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);

            step = Math.Sign(dy);
            gradientStep = Math.Sign(dx);
        }

        int gradientAccumulation = longest / 2;
        for (int i = 0; i < longest; i++)
        {
            line.Add(new Coord(x, y));

            if (inverted)
                y += step;
            else
                x += step;

            gradientAccumulation += shortest;
            if (gradientAccumulation >= longest)
            {
                if (inverted)
                    x += gradientStep;
                else
                    y += gradientStep;
                gradientAccumulation -= longest;
            }
        }

        return line;
    }

    void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB)
    {
        Room.ConnectRooms(roomA, roomB);
        //Debug.DrawLine (CoordToWorldPoint (tileA), CoordToWorldPoint (tileB), Color.green, 100);

        List<Coord> line = GetLine(tileA, tileB);
        foreach (Coord c in line)
        {
            map[c.tileX, c.tileY] = 0;
        }
    }

    void ConnectClosestRooms(List<Room> allRooms, bool forceAccessibilityFromMainRoom = false)
    {

        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();

        if (forceAccessibilityFromMainRoom)
        {
            foreach (Room room in allRooms)
            {
                if (room.isAccessibleFromMainRoom)
                    roomListB.Add(room);
                else
                    roomListA.Add(room);
            }
        }
        else
        {
            roomListA = allRooms;
            roomListB = allRooms;
        }

        int bestDistance = 0;
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();
        bool possibleConnectionFound = false;

        foreach (Room roomA in roomListA)
        {
            if (!forceAccessibilityFromMainRoom)
            {
                possibleConnectionFound = false;
                if (roomA.connectedRooms.Count > 0) continue;
            }

            foreach (Room roomB in roomListB)
            {
                if (roomA == roomB || roomA.IsConnected(roomB)) continue;

                for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++)
                {
                    for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++)
                    {
                        Coord tileA = roomA.edgeTiles[tileIndexA];
                        Coord tileB = roomB.edgeTiles[tileIndexB];
                        int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

                        if (distanceBetweenRooms < bestDistance || !possibleConnectionFound)
                        {
                            bestDistance = distanceBetweenRooms;
                            possibleConnectionFound = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
                }
            }
            if (possibleConnectionFound && !forceAccessibilityFromMainRoom)
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
        }

        if (possibleConnectionFound && forceAccessibilityFromMainRoom)
        {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(allRooms, true);
        }

        if (!forceAccessibilityFromMainRoom)
            ConnectClosestRooms(allRooms, true);
    }

    void EnsureConnectivity()
    {
        List<List<Coord>> roomRegions = GetRegions(0);
        // int roomThresholdSize = 50;
        List<Room> survivingRooms = new List<Room>();

        foreach (List<Coord> roomRegion in roomRegions)
        {
            // if (roomRegion.Count < roomThresholdSize)
            // {
            //   foreach (Coord tile in roomRegion)
            //     map[tile.tileX, tile.tileY] = 1;
            // }
            // else
            survivingRooms.Add(new Room(roomRegion, map, width, height));
        }
        survivingRooms.Sort();
        survivingRooms[0].isMainRoom = true;
        survivingRooms[0].isAccessibleFromMainRoom = true;

        ConnectClosestRooms(survivingRooms);
    }

}
