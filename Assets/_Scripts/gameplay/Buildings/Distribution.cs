using UnityEngine;

public class Distributor : MonoBehaviour
{
    [SerializeField] private BuildingInput input;

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
