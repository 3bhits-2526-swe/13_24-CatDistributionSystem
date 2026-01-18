using UnityEngine;

public class ProductionLineSetup : MonoBehaviour
{
    [Header("Building Data References")]
    [SerializeField] private BuildingData materializerData;
    [SerializeField] private BuildingData factoryData;
    [SerializeField] private BuildingData packagerData;
    [SerializeField] private BuildingData distributionData;
    [SerializeField] private BuildingData conveyorBeltData;

    [Header("Item & Recipe References")]
    [SerializeField] private ItemData catPowder;
    [SerializeField] private ItemData catProduct;
    [SerializeField] private ItemData packagedCat;
    [SerializeField] private RecipeData processRecipe;
    [SerializeField] private RecipeData packageRecipe;

    [Header("Production Line Layout")]
    [SerializeField] private bool autoSetupOnStart = true;
    [SerializeField] private GridPosition startPosition = new GridPosition(10, 10);

    [Header("Spacing (in grid cells)")]
    [SerializeField] private int buildingSpacing = 2; // Space between buildings
    [SerializeField] private int conveyorCount = 2;   // Conveyors between each building

    private void Start()
    {
        if (autoSetupOnStart)
        {
            SetupProductionLine();
        }
    }

    public void SetupProductionLine()
    {
        if (!ValidateReferences())
        {
            Debug.LogError("Missing references for production line setup!");
            return;
        }

        // Calculate positions with correct spacing
        GridPosition materializerPos = startPosition;
        GridPosition factoryPos = new GridPosition(
            startPosition.x + materializerData.size.width + buildingSpacing + conveyorCount,
            startPosition.y
        );
        GridPosition packagerPos = new GridPosition(
            factoryPos.x + factoryData.size.width + buildingSpacing + conveyorCount,
            startPosition.y
        );
        GridPosition distributionPos = new GridPosition(
            packagerPos.x + packagerData.size.width + buildingSpacing + conveyorCount,
            startPosition.y
        );

        // Clear any existing buildings in the area first
        ClearAreaForSetup();

        // Place main buildings
        BuildingBehaviour materializer = PlaceBuilding(materializerData, materializerPos, catPowder, null);
        BuildingBehaviour factory = PlaceBuilding(factoryData, factoryPos, null, processRecipe);
        BuildingBehaviour packager = PlaceBuilding(packagerData, packagerPos, null, packageRecipe);
        BuildingBehaviour distribution = PlaceBuilding(distributionData, distributionPos, null, null);

        if (materializer == null || factory == null || packager == null || distribution == null)
        {
            Debug.LogError("Failed to place one or more buildings!");
            return;
        }

        // Place conveyor belts between buildings
        SetupConveyors(materializerPos, materializerData.size.width, factoryPos, "Materializer→Factory");
        SetupConveyors(factoryPos, factoryData.size.width, packagerPos, "Factory→Packager");
        SetupConveyors(packagerPos, packagerData.size.width, distributionPos, "Packager→Distribution");

        Debug.Log("✅ Production line setup complete!");
        Debug.Log($"Materializer at ({materializerPos.x}, {materializerPos.y})");
        Debug.Log($"Factory at ({factoryPos.x}, {factoryPos.y})");
        Debug.Log($"Packager at ({packagerPos.x}, {packagerPos.y})");
        Debug.Log($"Distribution at ({distributionPos.x}, {distributionPos.y})");
    }

    private void SetupConveyors(GridPosition fromPos, int fromWidth, GridPosition toPos, string label)
    {
        int startX = fromPos.x + fromWidth;
        int endX = toPos.x - 1; // Leave 1 cell space before next building

        for (int x = startX; x < endX; x++)
        {
            GridPosition conveyorPos = new GridPosition(x, fromPos.y);
            BuildingBehaviour conveyor = PlaceBuilding(conveyorBeltData, conveyorPos, null, null);

            if (conveyor != null && conveyor.buildingBase is ConveyorBeltBuilding conveyorBelt)
            {
                // Set rotation to face right (east)
                conveyorBelt.rotation = 0;
                conveyor.transform.rotation = Quaternion.Euler(0, 0, 0);
                conveyorBelt.UpdateConnections();

                Debug.Log($"Placed conveyor at ({x}, {fromPos.y}) for {label}");
            }
        }
    }

    private BuildingBehaviour PlaceBuilding(BuildingData data, GridPosition position, ItemData item, RecipeData recipe)
    {
        if (BuildingManager.Instance == null)
        {
            Debug.LogError("BuildingManager.Instance is null!");
            return null;
        }

        return BuildingManager.Instance.PlaceBuilding(data, position, item, recipe);
    }

    private void ClearAreaForSetup()
    {
        // Optional: Clear any existing buildings in the setup area
        BuildingBehaviour[] existingBuildings = FindObjectsOfType<BuildingBehaviour>();

        foreach (BuildingBehaviour building in existingBuildings)
        {
            if (building.buildingBase != null)
            {
                GridPosition pos = building.buildingBase.gridPosition;
                if (pos.x >= startPosition.x - 5 && pos.x <= startPosition.x + 30 &&
                    pos.y >= startPosition.y - 5 && pos.y <= startPosition.y + 5)
                {
                    BuildingManager.Instance.RemoveBuilding(building);
                }
            }
        }
    }

    private bool ValidateReferences()
    {
        bool allValid = true;

        if (materializerData == null) { Debug.LogError("MaterializerData is missing!"); allValid = false; }
        if (factoryData == null) { Debug.LogError("FactoryData is missing!"); allValid = false; }
        if (packagerData == null) { Debug.LogError("PackagerData is missing!"); allValid = false; }
        if (distributionData == null) { Debug.LogError("DistributionData is missing!"); allValid = false; }
        if (conveyorBeltData == null) { Debug.LogError("ConveyorBeltData is missing!"); allValid = false; }

        if (catPowder == null) { Debug.LogError("CatPowder ItemData is missing!"); allValid = false; }
        if (catProduct == null) { Debug.LogError("CatProduct ItemData is missing!"); allValid = false; }
        if (packagedCat == null) { Debug.LogError("PackagedCat ItemData is missing!"); allValid = false; }

        if (processRecipe == null) { Debug.LogError("ProcessRecipe is missing!"); allValid = false; }
        if (packageRecipe == null) { Debug.LogError("PackageRecipe is missing!"); allValid = false; }

        return allValid;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SetupProductionLine();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            ClearAreaForSetup();
            Debug.Log("Cleared setup area");
        }
    }
}