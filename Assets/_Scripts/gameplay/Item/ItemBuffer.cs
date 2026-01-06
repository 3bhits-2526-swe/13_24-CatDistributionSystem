using System.Collections.Generic;

public class ItemBuffer
{
    public int capacity = 10;
    public Queue<ItemInstance> items = new Queue<ItemInstance>();

    public bool IsFull => items.Count >= capacity;
    public bool HasItems => items.Count > 0;
    public int ItemCount => items.Count;

    public bool CanAddItem() => !IsFull;

    public bool AddItem(ItemInstance item)
    {
        if (!CanAddItem()) return false;
        items.Enqueue(item);
        return true;
    }

    public ItemInstance RemoveItem()
    {
        if (!HasItems) return null;
        return items.Dequeue();
    }

    public ItemInstance PeekItem()
    {
        if (!HasItems) return null;
        return items.Peek();
    }

    public void Clear() => items.Clear();
}