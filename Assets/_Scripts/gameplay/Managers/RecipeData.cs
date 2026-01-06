using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipeData", menuName = "Cat Distribution/Recipe Data")]
public class RecipeData : ScriptableObject
{
    public string recipeName;
    public List<ItemData> requiredInputs = new List<ItemData>();
    public ItemData outputItem;
    public float productionMultiplier = 1f;
    public int unlockLevel = 1;
}