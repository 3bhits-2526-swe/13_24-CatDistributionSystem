using UnityEngine;

public class PackagerBuilding : BuildingBase
{
    private RecipeData packagingRecipe;
    private float valueMultiplier = 1f;

    public PackagerBuilding(BuildingData data, RecipeData recipe) : base(data)
    {
        packagingRecipe = recipe;
        data.maxInputs = 1;
        data.maxOutputs = 1;
    }

    public override void Process()
    {
        if (inputBuffer.HasItems && outputBuffer.CanAddItem())
        {
            var inputItem = inputBuffer.RemoveItem();
            var packagedItem = new ItemInstance(packagingRecipe.outputItem);
            packagedItem.valueMultiplier = inputItem.valueMultiplier * valueMultiplier;
            outputBuffer.AddItem(packagedItem);
        }
    }

    public override bool CanAcceptInput() => inputBuffer.CanAddItem();
    public override bool HasOutput() => outputBuffer.CanAddItem();

    public override void OnLevelUp(int globalLevel)
    {
        base.OnLevelUp(globalLevel);
        valueMultiplier = 1f + (currentLevel * 0.5f);
    }
}