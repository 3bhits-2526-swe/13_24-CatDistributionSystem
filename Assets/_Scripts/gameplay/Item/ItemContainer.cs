using UnityEngine;
using System;
using System.Collections.Generic;

public class ItemContainer : MonoBehaviour
{
    public int capacity = 10;
    public List<ItemData> items = new List<ItemData>();

    public bool CanAddItem() => items.Count < capacity;
    public bool HasItems() => items.Count > 0;

    public bool AddItem(ItemData item)
    {
        if (CanAddItem())
        {
            items.Add(item);
            return true;
        }
        return false;
    }

    public ItemData RemoveItem()
    {
        if (HasItems())
        {
            ItemData item = items[0];
            items.RemoveAt(0);
            return item;
        }
        return null;
    }

    public void Clear() => items.Clear();
}