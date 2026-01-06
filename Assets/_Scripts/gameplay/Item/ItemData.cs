using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Cat Distribution/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int baseValue = 10;
    public ItemType itemType;
}

public enum ItemType
{
    RawMaterial,    // Cat powder
    Product,        // Processed cat
    Packaged,       // Cat in package
    Special         // Special combinations
}