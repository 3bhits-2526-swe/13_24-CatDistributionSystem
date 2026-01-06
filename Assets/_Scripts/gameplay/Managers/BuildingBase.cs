using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class BuildingBase
{
    public BuildingData data;
    public GridPosition gridPosition;
    public int currentLevel = 1;
    public int rotation = 0;

    public ItemBuffer inputBuffer = new ItemBuffer();
    public ItemBuffer outputBuffer = new ItemBuffer();
    public List<RecipeData> availableRecipes = new List<RecipeData>();

    protected ItemData currentOutputItem;
    protected float productionProgress = 0f;

    public List<BuildingBehaviour> inputConnections = new List<BuildingBehaviour>();
    public List<BuildingBehaviour> outputConnections = new List<BuildingBehaviour>();

    public BuildingBase(BuildingData buildingData)
    {
        data = buildingData;
        inputBuffer.capacity = data.maxInputs;
        outputBuffer.capacity = data.maxOutputs;
    }

    public abstract void Process();
    public abstract bool CanAcceptInput();
    public abstract bool HasOutput();

    public virtual void OnPlaced() { }
    public virtual void OnDestroyed() { }

    public virtual void OnLevelUp(int globalLevel)
    {
        currentLevel = Mathf.Min(currentLevel + 1, data.maxLevel);
        inputBuffer.capacity = Mathf.Min(data.maxInputs * currentLevel, 20);
        outputBuffer.capacity = Mathf.Min(data.maxOutputs * currentLevel, 20);
    }

    public bool CanReceiveItem() => inputBuffer.CanAddItem();
    public bool HasItemToSend() => outputBuffer.HasItems;

    public bool TryReceiveItem(ItemInstance item)
    {
        if (!CanReceiveItem()) return false;
        return inputBuffer.AddItem(item);
    }

    public ItemInstance TrySendItem()
    {
        if (!HasItemToSend()) return null;
        return outputBuffer.RemoveItem();
    }

    protected ItemInstance ProcessRecipe(RecipeData recipe)
    {
        if (inputBuffer.ItemCount < recipe.requiredInputs.Count)
            return null;

        foreach (var requiredItem in recipe.requiredInputs)
        {
            inputBuffer.RemoveItem();
        }

        return new ItemInstance(recipe.outputItem);
    }
}
