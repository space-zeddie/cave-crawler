using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

[ExecuteInEditMode()]
public class HexGridCellularAutomata : ICellGridGenerator
{
    public GameObject HexagonFreePrefab;
    public GameObject HexagonWallPrefab;
    public int height;
    public int width;
    public bool useRandomSeed;
    public bool adjustFillPercent = true;
    public string seed;
    [Range(0, 100)]
    public int randomFillPercent;

    int[,] map;

    void Start()
    {
        ClearGrid();
        GenerateGrid();
        StartCoroutine(this.gameObject.GetComponent<ObstacleGenerator>().SpawnObstacles());
        StartCoroutine(this.gameObject.GetComponent<UnitGenerator>().SpawnUnits());
    }

    void ClearGrid()
    {
        var children = new List<GameObject>();
        foreach (Transform cell in this.CellsParent)
        {
            children.Add(cell.gameObject);
        }
        children.ForEach(c => DestroyImmediate(c));
    }
    
    public override List<Cell> GenerateGrid()
    {
        HexGridType hexGridType = width % 2 == 0 ? HexGridType.even_q : HexGridType.odd_q;
        var hexagons = new List<Cell>();

        if (HexagonFreePrefab.GetComponent<Hexagon>() == null || HexagonWallPrefab.GetComponent<Hexagon>() == null)
        {
            Debug.LogError("Invalid hexagon prefabs provided");
            return hexagons;
        }
        int adjust = (int)((width * height) / 1000);
        Debug.Log("Grid Adjusted by " + adjust);
        if (adjustFillPercent) randomFillPercent += adjust;

        map = new int[width, height];
        GenerateMap();

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                GameObject hexagon;
                if (map[j, i] == 1)
                {
                    hexagon = Instantiate(HexagonWallPrefab);
                    hexagon.GetComponent<Hexagon>().IsTaken = true;
                }
                else hexagon = Instantiate(HexagonFreePrefab);

                var hexSize = hexagon.GetComponent<Cell>().GetCellDimensions();

                hexagon.transform.position = new Vector3((j * hexSize.x * 0.75f), (i * hexSize.y) + (j % 2 == 0 ? 0 : hexSize.y * 0.5f));
                hexagon.GetComponent<Hexagon>().OffsetCoord = new Vector2(width - j - 1, height - i - 1);
                hexagon.GetComponent<Hexagon>().HexGridType = hexGridType;
                hexagon.GetComponent<Hexagon>().MovementCost = 1;
                hexagon.GetComponent<Hexagon>().i = j;
                hexagon.GetComponent<Hexagon>().j = i;
                hexagons.Add(hexagon.GetComponent<Cell>());

                hexagon.transform.parent = CellsParent;
            }
        }
        return hexagons;
    }

    public int[,] GetMap()
    {
        return map;
    }

    static bool IsInMapRange(int x, int y, int width, int height)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    void GenerateMap()
    {
        RandomFillMap();

        SmoothMap();
        SmoothMap();
        EnsureConnectivity();
    }

    void RandomFillMap()
    {
        if (useRandomSeed)
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
