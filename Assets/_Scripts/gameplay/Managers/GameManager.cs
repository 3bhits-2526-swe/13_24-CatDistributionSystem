using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GridManager gridManager;
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private PlacementController placementController;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private ItemTransferManager itemTransferManager;

    [SerializeField] private ItemData defaultRawMaterial;
    [SerializeField] private RecipeData defaultFactoryRecipe;
    [SerializeField] private RecipeData defaultPackagerRecipe;

    [SerializeField] private float gridCellSize = 1f;
    [SerializeField] private int gridWidth = 50;
    [SerializeField] private int gridHeight = 50;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializeReferences();
    }

    private void InitializeReferences()
    {
        if (gridManager == null) gridManager = GetComponent<GridManager>();
        if (buildingManager == null) buildingManager = GetComponent<BuildingManager>();
        if (resourceManager == null) resourceManager = GetComponent<ResourceManager>();
        if (placementController == null) placementController = GetComponent<PlacementController>();
        if (cameraController == null) cameraController = FindFirstObjectByType<CameraController>();
        if (uiManager == null) uiManager = FindFirstObjectByType<UIManager>();
        if (itemTransferManager == null) itemTransferManager = GetComponent<ItemTransferManager>();
    }

    public GridManager GetGridManager() => gridManager;
    public BuildingManager GetBuildingManager() => buildingManager;
    public ResourceManager GetResourceManager() => resourceManager;
    public UIManager GetUIManager() => uiManager;
}