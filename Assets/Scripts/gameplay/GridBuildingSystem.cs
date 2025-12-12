using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{
    public static GridBuildingSystem Instance { get; private set; }

    public enum MouseAction
    {
        None,
        PickUp,
        Place,
        Delete,
        Rotate
    }

    [Header("Grid Settings")]
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private bool snapToGrid = true;

    [Header("Visuals")]
    [SerializeField] internal Color validPlacementColor = new Color(0, 1, 0, 0.5f);
    [SerializeField] internal Color invalidPlacementColor = new Color(1, 0, 0, 0.5f);

    internal MouseAction currentAction = MouseAction.None;
    internal GameObject selectedObject;
    internal GameObject placementPreview;

    private BuildingGhost ghost;
    private Vector2 lastMousePosition;

    private void Awake() => Instance = this;

    private void Update()
    {
        if (currentAction == MouseAction.Place && placementPreview != null)
            UpdatePlacementPreview();
    }

    internal void StartPlacing(GameObject buildingPrefab)
    {
        currentAction = MouseAction.Place;

        if (placementPreview != null)
            Destroy(placementPreview);

        placementPreview = Instantiate(buildingPrefab);
        ghost = placementPreview.GetComponent<BuildingGhost>();

        if (ghost == null)
            ghost = placementPreview.AddComponent<BuildingGhost>();

        ghost.Initialize(this);
    }

    internal void StartDragging(GameObject building)
    {
        currentAction = MouseAction.PickUp;
        selectedObject = building;

        if (selectedObject.TryGetComponent<Building>(out var buildingComp))
            buildingComp.OnPickedUp();
    }

    private void UpdatePlacementPreview()
    {
        Vector2 mousePos = GetMouseWorldPosition();
        Vector2 snappedPos = snapToGrid ? SnapToGrid(mousePos) : mousePos;

        placementPreview.transform.position = snappedPos;

        bool canPlace = CanPlaceAt(snappedPos);
        ghost?.UpdateVisual(canPlace);
    }

    public bool TryPlaceObject(Vector2 position)
    {
        if (currentAction != MouseAction.Place || placementPreview == null)
            return false;

        Vector2 snappedPos = snapToGrid ? SnapToGrid(position) : position;

        if (!CanPlaceAt(snappedPos))
            return false;

        // Place the object
        placementPreview.transform.position = snappedPos;

        if (placementPreview.TryGetComponent<Building>(out var building))
            building.OnPlaced();

        // Clear placement mode
        placementPreview = null;
        currentAction = MouseAction.None;

        return true;
    }

    private bool CanPlaceAt()
    {
        if (placementPreview == null) return false;

        // Check for collisions
        Collider2D[] colliders = placementPreview.GetComponentsInChildren<Collider2D>();
        foreach (var collider in colliders)
        {
            collider.enabled = true;
            Collider2D[] overlaps = Physics2D.OverlapBoxAll(
                (Vector2)collider.bounds.center,
                collider.bounds.size,
                0
            );
            collider.enabled = false;

            foreach (var overlap in overlaps)
            {
                if (overlap.gameObject == placementPreview ||
                    overlap.gameObject == selectedObject)
                    continue;

                if (overlap.CompareTag("Building"))
                    return false;
            }
        }

        return true;
    }

    private Vector2 SnapToGrid(Vector2 position)
    {
        return new Vector2(
            Mathf.Round(position.x / gridSize) * gridSize,
            Mathf.Round(position.y / gridSize) * gridSize
        );
    }

    internal static Vector2 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    internal void CancelCurrentAction()
    {
        if (placementPreview != null)
            Destroy(placementPreview);

        if (currentAction == MouseAction.PickUp && selectedObject != null)
        {
            if (selectedObject.TryGetComponent<Building>(out var building))
            {
                building.OnCancelled();
            }
        }

        currentAction = MouseAction.None;
        selectedObject = null;
        placementPreview = null;
    }
}   