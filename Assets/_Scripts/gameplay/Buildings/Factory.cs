using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Factory : BuildingBase
{
    [Header("Factory")]
    [SerializeField] private BuildingInput input;
    [SerializeField] private BuildingOutput output;

    private Dictionary<ItemType, int> inputBuffer = new();
    private float processTimer;
    private bool isProcessing;
    private int activeRecipe;

    public override float ProductionTime => CurrentRecipe.BaseProcessTime;
    public override string StateText => isProcessing ? "Working" : "Waiting";
    public override int ActiveRecipeIndex => activeRecipe;

    private Recipe CurrentRecipe => Type.recipes[activeRecipe];

    public override void SetRecipe(int index)
    {
        if (index < 0 || index >= Type.recipes.Length)
            return;

        activeRecipe = index;
    }

    private void Update()
    {
        TryConsumeInput();
        ProcessRecipe();
    }

    private void TryConsumeInput()
    {
        if (isProcessing)
            return;

        if (!input.TryConsume(out ItemInstance item))
            return;

        ItemType type = item.ItemType;

        if (!RecipeNeeds(type))
            return;

        Destroy(item.gameObject);

        if (!inputBuffer.ContainsKey(type))
            inputBuffer[type] = 0;

        inputBuffer[type]++;
    }

    private void ProcessRecipe()
    {
        if (isProcessing)
        {
            processTimer += Time.deltaTime;

            if (processTimer >= CurrentRecipe.BaseProcessTime)
                FinishRecipe();

            return;
        }

        if (!HasAllInputs())
            return;

        ConsumeRecipeInputs();
        processTimer = 0f;
        isProcessing = true;
    }

    private void FinishRecipe()
    {
        isProcessing = false;
        for (int i = 0; i < CurrentRecipe.Outputs.Length; i++)
        {
            ItemInstance item = Instantiate(
                CurrentRecipe.Outputs[i],
                output.transform.position,
                Quaternion.identity
            );
            item.gameObject.SetActive(false);

            if (!output.TryOutput(item))
                Destroy(item.gameObject);
            else
                Debug.Log("Crafting Item");
        }
    }

    private bool RecipeNeeds(ItemType type)
    {
        for (int i = 0; i < CurrentRecipe.Inputs.Length; i++)
            if (CurrentRecipe.Inputs[i] == type)
                return true;

        return false;
    }

    private bool HasAllInputs()
    {
        for (int i = 0; i < CurrentRecipe.Inputs.Length; i++)
        {
            ItemType type = CurrentRecipe.Inputs[i];
            int required = CurrentRecipe.InputCounts[i];

            if (!inputBuffer.ContainsKey(type))
                return false;

            if (inputBuffer[type] < required)
                return false;
        }

        return true;
    }

    private void ConsumeRecipeInputs()
    {
        for (int i = 0; i < CurrentRecipe.Inputs.Length; i++)
        {
            ItemType type = CurrentRecipe.Inputs[i];
            int required = CurrentRecipe.InputCounts[i];

            inputBuffer[type] -= required;
        }
    }
}
