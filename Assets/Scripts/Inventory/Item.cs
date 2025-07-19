using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item  : ScriptableObject
{
    public string itemName;
    public Sprite icon;

    [TextArea]
    public string description;

    public ItemType type;
    public int value;
}

public enum ItemType
{
    Consumable,
    KeyItem
}
