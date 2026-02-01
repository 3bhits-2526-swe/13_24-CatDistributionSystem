using UnityEngine;
using System.Collections.Generic;

public interface IBuildingInspectable
{
    BuildingType Type { get; }

    int Level { get; }
    float ProductionTime { get; }
    string StateText { get; }

    int ActiveRecipeIndex { get; }
    void SetRecipe(int index);
}
