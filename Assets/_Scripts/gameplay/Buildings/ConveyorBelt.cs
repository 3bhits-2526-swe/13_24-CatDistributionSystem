using UnityEngine;
using System;
using System.Collections.Generic;


public class ConveyorBelt : BuildingBase
{
    private float speedMultiplier = 1f;
    private BuildingBehaviour inputBuilding;
    private BuildingBehaviour outputBuilding;

    public ConveyorBelt(BuildingData data) : base(data) { }

    public override void Process()
    {
        // Conveyor belts don't produce, they transport
        // Logic will be handled in a separate ConveyorSystem
    }

    public void SetConnection(BuildingBehaviour input, BuildingBehaviour output)
    {
        inputBuilding = input;
        outputBuilding = output;

        if (input != null && !inputConnections.Contains(input))
            inputConnections.Add(input);

        if (output != null && !outputConnections.Contains(output))
            outputConnections.Add(output);
    }

    public override bool CanAcceptInput() => true;
    public override bool HasOutput() => true;

    public override void OnLevelUp(int globalLevel)
    {
        base.OnLevelUp(globalLevel);
        speedMultiplier = 1f + (currentLevel * 0.25f);
    }
}