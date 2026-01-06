using UnityEngine;

[System.Serializable]
public class ItemInstance
{
    public ItemData data;
    public float valueMultiplier = 1f;

    public int GetTotalValue() => Mathf.RoundToInt(data.baseValue * valueMultiplier);

    public ItemInstance(ItemData itemData)
    {
        data = itemData;
    }
}