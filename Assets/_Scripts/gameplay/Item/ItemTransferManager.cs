using System.Collections.Generic;
using UnityEngine;

public class ItemTransferManager : MonoBehaviour
{
    public static ItemTransferManager Instance { get; private set; }

    public float transferInterval = 0.5f;
    public bool showTransferEffects = true;

    private List<BuildingBehaviour> buildings = new List<BuildingBehaviour>();
    private float transferTimer = 0f;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        transferTimer += Time.deltaTime;

        if (transferTimer >= transferInterval)
        {
            ProcessItemTransfers();
            transferTimer = 0f;
        }
    }

    private void ProcessItemTransfers()
    {
        foreach (var building in buildings)
        {
            var buildingBase = building.buildingBase;

            if (buildingBase.HasItemToSend() && buildingBase.outputConnections.Count > 0)
            {
                foreach (var outputBuilding in buildingBase.outputConnections)
                {
                    var targetBase = outputBuilding.buildingBase;
                    if (targetBase.CanReceiveItem())
                    {
                        var item = buildingBase.TrySendItem();
                        if (item != null)
                        {
                            bool success = targetBase.TryReceiveItem(item);
                            if (success && showTransferEffects)
                            {
                                ShowTransferEffect(building.transform, outputBuilding.transform);
                            }
                            break;
                        }
                    }
                }
            }
        }
    }

    private void ShowTransferEffect(Transform from, Transform to)
    {
        Debug.DrawLine(from.position, to.position, Color.green, 0.5f);
    }

    public void RegisterBuilding(BuildingBehaviour building)
    {
        if (!buildings.Contains(building))
            buildings.Add(building);
    }

    public void UnregisterBuilding(BuildingBehaviour building)
    {
        buildings.Remove(building);
    }
}