using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] private int width = 100;
    [SerializeField] private int height = 100;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Vector2 worldOffset;

    private GridCell[,] grid;
    private Dictionary<int, GridObjectData> objects = new();

    private int nextId = 1;

    private void Awake()
    {
        Instance = this;
        grid = new GridCell[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                grid[x, y] = new GridCell();
    }

    public Vector2Int WorldToGrid(Vector3 world)
    {
        world -= (Vector3)worldOffset;
        return new Vector2Int(
            Mathf.FloorToInt(world.x / cellSize),
            Mathf.FloorToInt(world.y / cellSize)
        );
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(
            gridPos.x * cellSize,
            gridPos.y * cellSize,
            0
        ) + (Vector3)worldOffset;
    }

    public bool CanPlace(Vector2Int origin, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int p = origin + new Vector2Int(x, y);
                if (!IsInside(p) || grid[p.x, p.y].occupied)
                    return false;
            }

        return true;
    }

    public int Place(Vector2Int origin, Vector2Int size)
    {
        int id = nextId++;
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int p = origin + new Vector2Int(x, y);
                grid[p.x, p.y].occupied = true;
                grid[p.x, p.y].objectId = id;
            }

        objects.Add(id, new GridObjectData { origin = origin, size = size });
        return id;
    }

    public void Remove(int id)
    {
        if (!objects.TryGetValue(id, out GridObjectData data))
            return;

        for (int x = 0; x < data.size.x; x++)
            for (int y = 0; y < data.size.y; y++)
            {
                Vector2Int p = data.origin + new Vector2Int(x, y);
                grid[p.x, p.y].occupied = false;
                grid[p.x, p.y].objectId = 0;
            }

        objects.Remove(id);
    }

    private bool IsInside(Vector2Int p)
    {
        return p.x >= 0 && p.y >= 0 && p.x < width && p.y < height;
    }
}
