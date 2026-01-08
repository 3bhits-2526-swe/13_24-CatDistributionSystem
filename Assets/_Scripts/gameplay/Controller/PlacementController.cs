using UnityEngine;
using UnityEngine.EventSystems;

public enum PlacementMode
{
    None,
    DragDrop,
    SelectPlace
}

public class PlacementController : MonoBehaviour
{
    public static PlacementController Instance { get; private set; }

    [SerializeField] private Color ghostValidColor = new Color(0, 1, 0, 0.5f);
    [SerializeField] private Color ghostInvalidColor = new Color(1, 0, 0, 0.5f);
    [SerializeField] private BuildingPaletteUI buildingPalette;

    private PlacementMode currentMode = PlacementMode.None;
    private BuildingData selectedBuildingData;
    private GameObject ghostObject;
    private SpriteRenderer ghostRenderer;
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
        if (selectedBuildingData == null) return;

        HandleRotationInput();
        HandlePlacementMode();
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

    private void HandlePlacementMode()
    {
        if (IsPointerOverUI())
        {
            if (ghostObject != null)
                ghostObject.SetActive(false);
            return;
        }

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        GridPosition gridPos = GridManager.Instance.WorldToGrid(mouseWorldPos);

        UpdateGhostPosition(gridPos);

        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceBuilding(gridPos);
        }

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
        }
    }

    private void UpdateGhostPosition(GridPosition gridPos)
    {
        if (ghostObject == null || selectedBuildingData == null) return;

        ghostObject.SetActive(true);

        Vector3 worldPos = GridManager.Instance.GridToWorld(gridPos);
        worldPos += new Vector3(selectedBuildingData.size.width * 0.5f,
                                selectedBuildingData.size.height * 0.5f, 0);
        ghostObject.transform.position = worldPos;

        bool isValid = GridManager.Instance.IsPositionValid(gridPos, selectedBuildingData.size);
        bool canAfford = ResourceManager.Instance != null &&
                        ResourceManager.Instance.CanAfford(selectedBuildingData.baseCost);

        ghostRenderer.color = (isValid && canAfford) ? ghostValidColor : ghostInvalidColor;
    }

    public void StartPlacement(BuildingData data, PlacementMode mode)
    {
        if (data == null) return;

        CancelPlacement();

        selectedBuildingData = data;
        currentMode = mode;
        currentRotation = 0;

        CreateGhostObject();

        if (GridManager.Instance != null)
            GridManager.Instance.SetGridVisibility(true);
    }

    private void CreateGhostObject()
    {
        if (ghostObject != null)
            Destroy(ghostObject);

        ghostObject = new GameObject("PlacementGhost");
        ghostRenderer = ghostObject.AddComponent<SpriteRenderer>();

        if (selectedBuildingData != null && selectedBuildingData.sprite != null)
            ghostRenderer.sprite = selectedBuildingData.sprite;

        ghostRenderer.sortingOrder = 100;
        ghostRenderer.color = ghostValidColor;
    }

    private void TryPlaceBuilding(GridPosition gridPos)
    {
        if (selectedBuildingData == null) return;
        if (IsPointerOverUI()) return;

        if (!GridManager.Instance.IsPositionValid(gridPos, selectedBuildingData.size))
            return;

        if (ResourceManager.Instance == null || !ResourceManager.Instance.CanAfford(selectedBuildingData.baseCost))
            return;

        BuildingBehaviour building = BuildingManager.Instance.PlaceBuilding(selectedBuildingData, gridPos);

        if (building != null)
        {
            building.buildingBase.rotation = currentRotation;
            building.transform.rotation = Quaternion.Euler(0, 0, currentRotation);

            CancelPlacement();
        }
    }

    public void CancelPlacement()
    {
        currentMode = PlacementMode.None;
        selectedBuildingData = null;
        currentRotation = 0;

        if (ghostObject != null)
            Destroy(ghostObject);

        if (GridManager.Instance != null)
            GridManager.Instance.SetGridVisibility(false);
    }

    private bool IsPointerOverUI()
    {
        if (buildingPalette != null && buildingPalette.IsMouseOverPalette())
            return true;

        return EventSystem.current.IsPointerOverGameObject();
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}