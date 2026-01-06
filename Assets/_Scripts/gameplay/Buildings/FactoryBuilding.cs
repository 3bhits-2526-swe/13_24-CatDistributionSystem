using UnityEngine;

public class FactoryBuilding : BuildingBase
{
    private RecipeData currentRecipe;
    private int requiredInputCount = 1;

    public FactoryBuilding(BuildingData data, RecipeData recipe) : base(data)
    {
        currentRecipe = recipe;
        data.maxInputs = 3;
        data.maxOutputs = 1;
        UpdateRequiredInputs();
    }

    private void UpdateRequiredInputs()
    {
        requiredInputCount = Mathf.Min(currentLevel, 3);
    }

    public override void Process()
    {
        if (inputBuffer.ItemCount >= requiredInputCount && outputBuffer.CanAddItem())
        {
            var outputItem = ProcessRecipe(currentRecipe);
            if (outputItem != null)
            {
                outputBuffer.AddItem(outputItem);
            }
        }
    }

    public override bool CanAcceptInput() => inputBuffer.CanAddItem();
    public override bool HasOutput() => outputBuffer.CanAddItem();

    public override void OnLevelUp(int globalLevel)
    {
        base.OnLevelUp(globalLevel);
        UpdateRequiredInputs();
    }
}