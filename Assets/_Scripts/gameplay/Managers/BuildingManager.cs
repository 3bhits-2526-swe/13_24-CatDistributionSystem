using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }

    [SerializeField] private List<ItemData> availableItems = new List<ItemData>();
    [SerializeField] private List<RecipeData> availableRecipes = new List<RecipeData>();

    private List<BuildingBehaviour> allBuildings = new List<BuildingBehaviour>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public BuildingBehaviour PlaceBuilding(BuildingData data, GridPosition gridPosition, ItemData outputItem = null, RecipeData recipe = null)
    {
        if (GridManager.Instance == null)
        {
            Debug.LogError("GridManager.Instance is null");
            return null;
        }

        if (ResourceManager.Instance == null)
        {
            Debug.LogError("ResourceManager.Instance is null");
            return null;
        }

        if (!GridManager.Instance.IsPositionValid(gridPosition, data.size))
            return null;

        if (!ResourceManager.Instance.CanAfford(data.baseCost))
            return null;

        GameObject buildingObj = new GameObject(data.buildingName);
        BuildingBehaviour behaviour = buildingObj.AddComponent<BuildingBehaviour>();
        buildingObj.AddComponent<BoxCollider2D>();

        BuildingBase buildingBase = CreateBuildingInstance(data, outputItem, recipe);
        if (buildingBase == null)
        {
            Destroy(buildingObj);
            Debug.LogError("BuildingBase was null, couldnt create a instance");
            return null;
        }

        buildingBase.gridPosition = gridPosition;
        behaviour.Initialize(buildingBase);

        Vector3 worldPos = GridManager.Instance.GridToWorld(gridPosition);
        worldPos += new Vector3(data.size.width * 0.5f, data.size.height * 0.5f, 0);
        buildingObj.transform.position = worldPos;

        GridManager.Instance.OccupyPosition(gridPosition, data.size, behaviour);
        allBuildings.Add(behaviour);

        ResourceManager.Instance.SpendMoney(data.baseCost);
        buildingBase.OnPlaced();

        if (ItemTransferManager.Instance != null)
            ItemTransferManager.Instance.RegisterBuilding(behaviour);

        // Handle conveyor belt specific setup
        if (buildingBase is ConveyorBeltBuilding conveyorBelt)
        {
            conveyorBelt.UpdateConnections();
            SimpleConveyorSystem.Instance?.RegisterConveyor(conveyorBelt);
        }

        return behaviour;
    }

    private BuildingBase CreateBuildingInstance(BuildingData data, ItemData outputItem, RecipeData recipe)
    {
        if (data == null) return null;

        Debug.Log("Create Building Instance: " + data.buildingName);

        switch (data.buildingType)
        {
            case BuildingType.Materializer:
                if (outputItem == null && availableItems.Count > 0)
                    outputItem = availableItems.Find(i => i != null && i.itemType == ItemType.RawMaterial);
                if (outputItem == null) return null;
                return new MaterializerBuilding(data, outputItem);

            case BuildingType.Factory:
                if (recipe == null && availableRecipes.Count > 0)
                    recipe = availableRecipes.Find(r => r != null && r.unlockLevel <= 1);
                if (recipe == null) return null;
                return new FactoryBuilding(data, recipe);

            case BuildingType.Packager:
                if (recipe == null && availableRecipes.Count > 0)
                    recipe = availableRecipes.Find(r => r != null && r.outputItem != null && r.outputItem.itemType == ItemType.Packaged);
                if (recipe == null) return null;
                return new PackagerBuilding(data, recipe);

            case BuildingType.Distribution:
                return new DistributionBuilding(data);

            case BuildingType.ConveyorBelt:
                return new ConveyorBeltBuilding(data);

            default:
                Debug.LogError("This shouldnt be happening. Unknown building type: " + data.buildingType);
                return null;
        }
    }

    public void RemoveBuilding(BuildingBehaviour building)
    {
        if (building == null) return;

        if (GridManager.Instance != null)
            GridManager.Instance.ClearPosition(building.buildingBase.gridPosition, building.buildingBase.data.size);

        building.buildingBase.OnDestroyed();

        if (ItemTransferManager.Instance != null)
            ItemTransferManager.Instance.UnregisterBuilding(building);

        // Handle conveyor belt specific cleanup
        if (building.buildingBase is ConveyorBeltBuilding conveyorBelt)
        {
            SimpleConveyorSystem.Instance?.UnregisterConveyor(conveyorBelt);
        }

        allBuildings.Remove(building);
        Destroy(building.gameObject);
    }

    public void ConnectBuildings(BuildingBehaviour from, BuildingBehaviour to)
    {
        if (from == null || to == null) return;

        var fromBase = from.buildingBase;
        var toBase = to.buildingBase;

        if (fromBase != null && toBase != null)
        {
            fromBase.outputConnections.Add(to);
            toBase.inputConnections.Add(from);
        }
    }

    public void OnGlobalLevelUp(int newLevel)
    {
        foreach (var building in allBuildings)
        {
            building.buildingBase.OnLevelUp(newLevel);
        }
    }
}