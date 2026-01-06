using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] private float cellSize = 1f;
    [SerializeField] private int width = 50;
    [SerializeField] private int height = 50;

    [Header("Grid Visual Settings")]
    [SerializeField] private bool showGridOnStart = true;
    [SerializeField] private bool showGridDuringPlacement = true;
    [SerializeField] private Color gridColorNormal = new Color(1, 1, 1, 0.1f);
    [SerializeField] private Color gridColorPlacement = new Color(0, 1, 1, 0.3f);
    [SerializeField] private float gridLineWidth = 0.02f;
    [SerializeField] private Material gridMaterial;

    private Dictionary<GridPosition, BuildingBehaviour> grid = new Dictionary<GridPosition, BuildingBehaviour>();
    private GameObject gridContainer;
    private List<LineRenderer> gridLines = new List<LineRenderer>();
    private bool isGridVisible = false;

    private void Awake()
    {
        Instance = this;
        CreateVisualGrid();
        SetGridVisibility(showGridOnStart);
    }

    private void CreateVisualGrid()
    {
        gridContainer = new GameObject("GridVisual");
        gridContainer.transform.SetParent(transform);

        if (gridMaterial == null)
        {
            gridMaterial = new Material(Shader.Find("Sprites/Default"));
        }

        Vector3 gridCenter = new Vector3(width * cellSize * 0.5f, height * cellSize * 0.5f, 0);

        for (int x = 0; x <= width; x++)
        {
            GameObject lineObj = new GameObject($"GridLine_V_{x}");
            lineObj.transform.SetParent(gridContainer.transform);
            LineRenderer lr = lineObj.AddComponent<LineRenderer>();

            lr.startWidth = gridLineWidth;
            lr.endWidth = gridLineWidth;
            lr.material = gridMaterial;
            lr.startColor = gridColorNormal;
            lr.endColor = gridColorNormal;
            lr.sortingOrder = -1;
            lr.useWorldSpace = true;

            lr.positionCount = 2;
            float xPos = x * cellSize;
            lr.SetPosition(0, new Vector3(xPos, 0, 0));
            lr.SetPosition(1, new Vector3(xPos, height * cellSize, 0));

            gridLines.Add(lr);
        }

        for (int y = 0; y <= height; y++)
        {
            GameObject lineObj = new GameObject($"GridLine_H_{y}");
            lineObj.transform.SetParent(gridContainer.transform);
            LineRenderer lr = lineObj.AddComponent<LineRenderer>();

            lr.startWidth = gridLineWidth;
            lr.endWidth = gridLineWidth;
            lr.material = gridMaterial;
            lr.startColor = gridColorNormal;
            lr.endColor = gridColorNormal;
            lr.sortingOrder = -1;
            lr.useWorldSpace = true;

            lr.positionCount = 2;
            float yPos = y * cellSize;
            lr.SetPosition(0, new Vector3(0, yPos, 0));
            lr.SetPosition(1, new Vector3(width * cellSize, yPos, 0));

            gridLines.Add(lr);
        }
    }

    public void SetGridVisibility(bool visible)
    {
        isGridVisible = visible;
        if (gridContainer != null)
            gridContainer.SetActive(visible);
    }

    public void ToggleGrid()
    {
        SetGridVisibility(!isGridVisible);
    }

    public void SetPlacementMode(bool inPlacementMode)
    {
        if (inPlacementMode && showGridDuringPlacement)
        {
            SetGridVisibility(true);
            UpdateGridColor(gridColorPlacement);
        }
        else if (!inPlacementMode && !showGridOnStart)
        {
            SetGridVisibility(false);
            UpdateGridColor(gridColorNormal);
        }
    }

    private void UpdateGridColor(Color color)
    {
        foreach (var line in gridLines)
        {
            if (line != null)
            {
                line.startColor = color;
                line.endColor = color;
            }
        }
    }

    public bool IsPositionValid(GridPosition position, BuildingSize size)
    {
        for (int x = 0; x < size.width; x++)
        {
            for (int y = 0; y < size.height; y++)
            {
                GridPosition checkPos = new GridPosition(position.x + x, position.y + y);

                if (checkPos.x < 0 || checkPos.x >= width || checkPos.y < 0 || checkPos.y >= height)
                    return false;

                if (grid.ContainsKey(checkPos))
                    return false;
            }
        }
        return true;
    }

    public void OccupyPosition(GridPosition position, BuildingSize size, BuildingBehaviour building)
    {
        for (int x = 0; x < size.width; x++)
        {
            for (int y = 0; y < size.height; y++)
            {
                GridPosition occupyPos = new GridPosition(position.x + x, position.y + y);
                grid[occupyPos] = building;
            }
        }
    }

    public void ClearPosition(GridPosition position, BuildingSize size)
    {
        for (int x = 0; x < size.width; x++)
        {
            for (int y = 0; y < size.height; y++)
            {
                GridPosition clearPos = new GridPosition(position.x + x, position.y + y);
                grid.Remove(clearPos);
            }
        }
    }

    public Vector3 GridToWorld(GridPosition gridPos)
    {
        return new Vector3(gridPos.x * cellSize, gridPos.y * cellSize, 0);
    }

    public GridPosition WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / cellSize);
        int y = Mathf.FloorToInt(worldPos.y / cellSize);
        return new GridPosition(x, y);
    }

    public BuildingBehaviour GetBuildingAt(GridPosition position)
    {
        grid.TryGetValue(position, out BuildingBehaviour building);
        return building;
    }
}