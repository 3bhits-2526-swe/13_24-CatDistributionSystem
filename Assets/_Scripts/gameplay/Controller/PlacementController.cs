using UnityEngine;

public enum PlacementMode
{
    None,
    DragDrop,
    SelectPlace
}

public class PlacementController : MonoBehaviour
{
    public static PlacementController Instance { get; private set; }

    [SerializeField] private Material ghostMaterialValid;
    [SerializeField] private Material ghostMaterialInvalid;
    [SerializeField] private LayerMask uiLayer;

    private PlacementMode currentMode = PlacementMode.None;
    private BuildingData selectedBuildingData;
    private GameObject ghostObject;
    private SpriteRenderer ghostRenderer;
    private bool isDragging = false;
    private int currentRotation = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (currentMode == PlacementMode.None) return;

        HandleRotationInput();

        if (currentMode == PlacementMode.DragDrop)
            HandleDragDropMode();
        else if (currentMode == PlacementMode.SelectPlace)
            HandleSelectPlaceMode();
    }

    private void HandleRotationInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentRotation = (currentRotation + 90) % 360;
            if (ghostObject != null)
                ghostObject.transform.rotation = Quaternion.Euler(0, 0, currentRotation);
        }
    }

    private void HandleDragDropMode()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        GridPosition gridPos = GridManager.Instance.WorldToGrid(mouseWorldPos);

        UpdateGhostPosition(gridPos);

        if (Input.GetMouseButton(0) && !isDragging)
        {
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            TryPlaceBuilding(gridPos);
            CancelPlacement();
        }

        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }

    private void HandleSelectPlaceMode()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        GridPosition gridPos = GridManager.Instance.WorldToGrid(mouseWorldPos);

        UpdateGhostPosition(gridPos);

        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceBuilding(gridPos);
        }

        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }

    private void UpdateGhostPosition(GridPosition gridPos)
    {
        if (ghostObject == null || selectedBuildingData == null) return;

        Vector3 worldPos = GridManager.Instance.GridToWorld(gridPos);
        worldPos += new Vector3(selectedBuildingData.size.width * 0.5f,
                                selectedBuildingData.size.height * 0.5f, 0);
        ghostObject.transform.position = worldPos;

        bool isValid = GridManager.Instance.IsPositionValid(gridPos, selectedBuildingData.size);
        bool canAfford = ResourceManager.Instance != null &&
                        ResourceManager.Instance.CanAfford(selectedBuildingData.baseCost);

        ghostRenderer.color = (isValid && canAfford) ?
            new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);
    }

    private void TryPlaceBuilding(GridPosition gridPos)
    {
        if (selectedBuildingData == null) return;

        if (!GridManager.Instance.IsPositionValid(gridPos, selectedBuildingData.size))
            return;

        if (ResourceManager.Instance == null || !ResourceManager.Instance.CanAfford(selectedBuildingData.baseCost))
            return;

        BuildingBehaviour building = BuildingManager.Instance.PlaceBuilding(
            selectedBuildingData,
            gridPos
        );

        if (building != null)
        {
            building.buildingBase.rotation = currentRotation;
            building.transform.rotation = Quaternion.Euler(0, 0, currentRotation);
        }
    }

    public void StartPlacement(BuildingData data, PlacementMode mode)
    {
        selectedBuildingData = data;
        currentMode = mode;
        currentRotation = 0;
        isDragging = false;

        CreateGhostObject();
        GridManager.Instance.SetPlacementMode(true);
    }

    private void CreateGhostObject()
    {
        if (ghostObject != null)
            Destroy(ghostObject);

        ghostObject = new GameObject("Ghost");
        ghostRenderer = ghostObject.AddComponent<SpriteRenderer>();

        if (selectedBuildingData != null && selectedBuildingData.sprite != null)
            ghostRenderer.sprite = selectedBuildingData.sprite;

        ghostRenderer.color = new Color(0, 1, 0, 0.5f);
        ghostRenderer.sortingOrder = 100;
    }

    public void CancelPlacement()
    {
        currentMode = PlacementMode.None;
        selectedBuildingData = null;
        isDragging = false;
        currentRotation = 0;

        if (ghostObject != null)
            Destroy(ghostObject);

        GridManager.Instance.SetPlacementMode(false);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}