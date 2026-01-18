using UnityEngine;

public class ConveyorBeltBuilding : BuildingBase
{
    private float speedMultiplier = 1f;
    private Vector2Int inputDirection;
    private Vector2Int outputDirection;

    public ConveyorBeltBuilding(BuildingData data) : base(data)
    {
        data.maxInputs = 1;
        data.maxOutputs = 1;
        UpdateDirectionVectors();
    }

    public override void Process()
    {
        if (inputBuffer.HasItems && outputBuffer.CanAddItem())
        {
            ItemInstance item = inputBuffer.RemoveItem();
            outputBuffer.AddItem(item);
        }
    }

    public override bool CanAcceptInput() => inputBuffer.CanAddItem();
    public override bool HasOutput() => outputBuffer.CanAddItem();

    public GridPosition GetInputPosition()
    {
        return new GridPosition(
            gridPosition.x + inputDirection.x,
            gridPosition.y + inputDirection.y
        );
    }

    public GridPosition GetOutputPosition()
    {
        return new GridPosition(
            gridPosition.x + outputDirection.x,
            gridPosition.y + outputDirection.y
        );
    }

    public void UpdateConnections()
    {
        if (GridManager.Instance == null) return;

        BuildingBehaviour inputBuilding = GridManager.Instance.GetBuildingAt(GetInputPosition());
        BuildingBehaviour outputBuilding = GridManager.Instance.GetBuildingAt(GetOutputPosition());

        if (inputBuilding != null && inputBuilding.buildingBase.HasOutput())
        {
            if (!inputConnections.Contains(inputBuilding))
                inputConnections.Add(inputBuilding);
        }

        if (outputBuilding != null && outputBuilding.buildingBase.CanAcceptInput())
        {
            if (!outputConnections.Contains(outputBuilding))
                outputConnections.Add(outputBuilding);
        }
    }

    private void UpdateDirectionVectors()
    {
        switch (rotation)
        {
            case 0: // Right
                inputDirection = Vector2Int.left;
                outputDirection = Vector2Int.right;
                break;
            case 90: // Up
                inputDirection = Vector2Int.down;
                outputDirection = Vector2Int.up;
                break;
            case 180: // Left
                inputDirection = Vector2Int.right;
                outputDirection = Vector2Int.left;
                break;
            case 270: // Down
                inputDirection = Vector2Int.up;
                outputDirection = Vector2Int.down;
                break;
        }
    }

    public override void OnPlaced()
    {
        base.OnPlaced();
        UpdateDirectionVectors();
        UpdateConnections();
    }

    public override void OnLevelUp(int globalLevel)
    {
        base.OnLevelUp(globalLevel);
        speedMultiplier = 1f + (currentLevel * 0.25f);
    }
}