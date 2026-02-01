using UnityEngine;

public class Distributor : BuildingBase
{
    [SerializeField] private BuildingInput input;

    public override float ProductionTime => 0f;
    public override string StateText => "Selling";
    public override int ActiveRecipeIndex => -1;
    public override void SetRecipe(int index) { }

    private void Update()
    {
        TrySellItem();
    }

    private void TrySellItem()
    {
        if (!input.TryConsume(out ItemInstance item))
            return;

        int value = CalculateValue(item);
        MoneyManager.Instance.Add(value);

        Destroy(item.gameObject);
    }

    private int CalculateValue(ItemInstance item)
    {
        float baseValue = item.ItemType.Value;
        float multiplier = item.valueMultiplier;

        return Mathf.RoundToInt(baseValue * multiplier);
    }
}
