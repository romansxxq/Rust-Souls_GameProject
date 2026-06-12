using System;
using System.Collections.Generic;
using UnityEngine;

public class BSPDungeonGenerator : MonoBehaviour
{
    [Header("Map")]
    public int mapWidth = 50;
    public int mapHeight = 50;

    [Header("Leaf / Room sizes")]
    public int minLeafSize = 6;
    public int maxLeafSize = 20;
    public int minRoomSize = 4;
    public int maxRoomSize = 12;

    [Header("Seed")]
    public int seed = 12345;
    public bool useRandomSeed = true;

    [Header("Prefabs")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public Transform parent;

    [Header("Options")]
    public bool generateOnStart = false;
    public bool debugDraw = false;

    int[,] map;
    System.Random rng;

    [ContextMenu("Generate Dungeon")]
    public void GenerateDungeon()
    {
        if (mapWidth < 10 || mapHeight < 10)
        {
            Debug.LogWarning("Map size too small");
            return;
        }

        if (useRandomSeed) seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        rng = new System.Random(seed);

        map = new int[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++)
            for (int y = 0; y < mapHeight; y++)
                map[x, y] = 0; // wall

        Leaf root = new Leaf(0, 0, mapWidth, mapHeight);
        List<Leaf> leafs = new List<Leaf> { root };

        bool didSplit = true;
        int safety = 0;
        while (didSplit && safety < 1000)
        {
            didSplit = false;
            List<Leaf> newLeaves = new List<Leaf>();
            foreach (var l in leafs)
            {
                if (l.left == null && l.right == null)
                {
                    if (l.rect.width > maxLeafSize || l.rect.height > maxLeafSize || rng.NextDouble() > 0.75)
                    {
                        if (l.Split(rng, minLeafSize))
                        {
                            newLeaves.Add(l.left);
                            newLeaves.Add(l.right);
                            didSplit = true;
                        }
                        else newLeaves.Add(l);
                    }
                    else newLeaves.Add(l);
                }
                else newLeaves.Add(l);
            }
            leafs = newLeaves;
            safety++;
        }

        List<RectInt> rooms = new List<RectInt>();
        List<Vector2Int[]> halls = new List<Vector2Int[]>();
        root.CreateRooms(rng, minRoomSize, maxRoomSize, rooms, halls);

        // carve rooms
        foreach (var r in rooms)
        {
            for (int x = r.x; x < r.x + r.width; x++)
                for (int y = r.y; y < r.y + r.height; y++)
                    if (InMap(x, y)) map[x, y] = 1;
        }

        // carve halls
        foreach (var pair in halls)
        {
            CarveCorridor(pair[0], pair[1]);
        }

        SpawnPrefabs();
        Debug.Log($"Dungeon generated (seed={seed}). Rooms: {rooms.Count}");
    }

    bool InMap(int x, int y) => x >= 0 && y >= 0 && x < mapWidth && y < mapHeight;

    void CarveCorridor(Vector2Int a, Vector2Int b)
    {
        int x = a.x;
        int y = a.y;
        if (rng.Next(0, 2) == 0)
        {
            while (x != b.x)
            {
                if (InMap(x, y)) map[x, y] = 1;
                x += (b.x > x) ? 1 : -1;
            }
            while (y != b.y)
            {
                if (InMap(x, y)) map[x, y] = 1;
                y += (b.y > y) ? 1 : -1;
            }
        }
        else
        {
            while (y != b.y)
            {
                if (InMap(x, y)) map[x, y] = 1;
                y += (b.y > y) ? 1 : -1;
            }
            while (x != b.x)
            {
                if (InMap(x, y)) map[x, y] = 1;
                x += (b.x > x) ? 1 : -1;
            }
        }
    }

    void SpawnPrefabs()
    {
        if (parent != null)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
#if UNITY_EDITOR
                DestroyImmediate(parent.GetChild(i).gameObject);
#else
                Destroy(parent.GetChild(i).gameObject);
#endif
            }
        }

        for (int x = 0; x < mapWidth; x++)
        for (int y = 0; y < mapHeight; y++)
        {
            Vector3 pos = new Vector3(x, 0, y);
            if (map[x, y] == 1)
            {
                if (floorPrefab != null) Instantiate(floorPrefab, pos, Quaternion.identity, parent);
            }
            else
            {
                if (wallPrefab != null) Instantiate(wallPrefab, pos, Quaternion.identity, parent);
            }
        }
    }

    [ContextMenu("Clear Dungeon")]
    public void ClearDungeon()
    {
        if (parent == null) return;
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            DestroyImmediate(parent.GetChild(i).gameObject);
#else
            Destroy(parent.GetChild(i).gameObject);
#endif
        }
    }

    void Start()
    {
        if (generateOnStart) GenerateDungeon();
    }

    void OnDrawGizmosSelected()
    {
        if (!debugDraw || map == null) return;
        for (int x = 0; x < mapWidth; x++)
            for (int y = 0; y < mapHeight; y++)
            {
                Gizmos.color = (map[x, y] == 1) ? Color.white : Color.black;
                Gizmos.DrawCube(new Vector3(x, 0, y), Vector3.one * 0.9f);
            }
    }

    class Leaf
    {
        public RectInt rect;
        public Leaf left, right;
        public RectInt room;
        public bool hasRoom = false;

        public Leaf(int x, int y, int w, int h)
        {
            rect = new RectInt(x, y, w, h);
        }

        public bool Split(System.Random rand, int minLeafSize)
        {
            bool splitH = rand.Next(0, 2) == 0;
            if (rect.width > rect.height && (float)rect.width / rect.height >= 1.25f) splitH = false;
            else if (rect.height > rect.width && (float)rect.height / rect.width >= 1.25f) splitH = true;

            int max = (splitH ? rect.height : rect.width) - minLeafSize;
            if (max <= minLeafSize) return false;
            int split = rand.Next(minLeafSize, max);

            if (splitH)
            {
                left = new Leaf(rect.x, rect.y, rect.width, split);
                right = new Leaf(rect.x, rect.y + split, rect.width, rect.height - split);
            }
            else
            {
                left = new Leaf(rect.x, rect.y, split, rect.height);
                right = new Leaf(rect.x + split, rect.y, rect.width - split, rect.height);
            }

            return true;
        }

        public void CreateRooms(System.Random rand, int minRoomSize, int maxRoomSize, List<RectInt> rooms, List<Vector2Int[]> halls)
        {
            if (left != null || right != null)
            {
                if (left != null) left.CreateRooms(rand, minRoomSize, maxRoomSize, rooms, halls);
                if (right != null) right.CreateRooms(rand, minRoomSize, maxRoomSize, rooms, halls);
                if (left != null && right != null)
                {
                    Vector2Int a = left.GetRoomCenter();
                    Vector2Int b = right.GetRoomCenter();
                    halls.Add(new Vector2Int[] { a, b });
                }
            }
            else
            {
                int maxW = Mathf.Min(rect.width - 2, maxRoomSize);
                int maxH = Mathf.Min(rect.height - 2, maxRoomSize);
                if (maxW < minRoomSize) maxW = minRoomSize;
                if (maxH < minRoomSize) maxH = minRoomSize;

                int w = rand.Next(minRoomSize, maxW + 1);
                int h = rand.Next(minRoomSize, maxH + 1);
                int x = rand.Next(rect.x + 1, rect.x + rect.width - w);
                int y = rand.Next(rect.y + 1, rect.y + rect.height - h);
                room = new RectInt(x, y, w, h);
                hasRoom = true;
                rooms.Add(room);
            }
        }

        public Vector2Int GetRoomCenter()
        {
            if (hasRoom) return new Vector2Int(room.x + room.width / 2, room.y + room.height / 2);
            Vector2Int a = left != null ? left.GetRoomCenter() : Vector2Int.zero;
            Vector2Int b = right != null ? right.GetRoomCenter() : a;
            return new Vector2Int((a.x + b.x) / 2, (a.y + b.y) / 2);
        }
    }
}
