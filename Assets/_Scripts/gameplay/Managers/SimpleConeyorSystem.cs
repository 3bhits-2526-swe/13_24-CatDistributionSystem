using System.Collections.Generic;
using UnityEngine;

public class SimpleConveyorSystem : MonoBehaviour
{
    public static SimpleConveyorSystem Instance { get; private set; }

    [SerializeField] private float transferInterval = 0.3f;

    private List<ConveyorBeltBuilding> conveyorBelts = new List<ConveyorBeltBuilding>();
    private float transferTimer = 0f;

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
        transferTimer += Time.deltaTime;

        if (transferTimer >= transferInterval)
        {
            ProcessAllConveyors();
            transferTimer = 0f;
        }
    }

    private void ProcessAllConveyors()
    {
        foreach (ConveyorBeltBuilding conveyor in conveyorBelts)
        {
            ProcessConveyor(conveyor);
        }
    }

    private void ProcessConveyor(ConveyorBeltBuilding conveyor)
    {
        if (conveyor.inputConnections.Count == 0 || conveyor.outputConnections.Count == 0)
            return;

        BuildingBehaviour inputBuilding = conveyor.inputConnections[0];
        BuildingBehaviour outputBuilding = conveyor.outputConnections[0];

        if (inputBuilding == null || outputBuilding == null)
            return;

        BuildingBase inputBase = inputBuilding.buildingBase;
        BuildingBase outputBase = outputBuilding.buildingBase;

        if (inputBase.HasItemToSend() && outputBase.CanReceiveItem())
        {
            ItemInstance item = inputBase.TrySendItem();
            if (item != null)
            {
                outputBase.TryReceiveItem(item);
            }
        }
    }

    public void RegisterConveyor(ConveyorBeltBuilding conveyor)
    {
        if (!conveyorBelts.Contains(conveyor))
            conveyorBelts.Add(conveyor);
    }

    public void UnregisterConveyor(ConveyorBeltBuilding conveyor)
    {
        conveyorBelts.Remove(conveyor);
    }
}