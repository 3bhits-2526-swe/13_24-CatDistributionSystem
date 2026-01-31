using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Factory : MonoBehaviour
{
    [SerializeField] private Recipe recipe;
    [SerializeField] private BuildingInput input;
    [SerializeField] private BuildingOutput output;

    private Dictionary<ItemType, int> inputBuffer = new();
    private float processTimer;
    private bool isProcessing;

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

            if (processTimer >= recipe.BaseProcessTime)
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

        for (int i = 0; i < recipe.Outputs.Length; i++)
        {
            ItemInstance item = Instantiate(
                recipe.Outputs[i],
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
        for (int i = 0; i < recipe.Inputs.Length; i++)
            if (recipe.Inputs[i] == type)
                return true;

        return false;
    }

    private bool HasAllInputs()
    {
        for (int i = 0; i < recipe.Inputs.Length; i++)
        {
            ItemType type = recipe.Inputs[i];
            int required = recipe.InputCounts[i];

            if (!inputBuffer.ContainsKey(type))
                return false;

            if (inputBuffer[type] < required)
                return false;
        }

        return true;
    }

    private void ConsumeRecipeInputs()
    {
        for (int i = 0; i < recipe.Inputs.Length; i++)
        {
            ItemType type = recipe.Inputs[i];
            int required = recipe.InputCounts[i];

            inputBuffer[type] -= required;
        }
    }
}
