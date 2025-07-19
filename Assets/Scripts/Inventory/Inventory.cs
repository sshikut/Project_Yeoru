using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public InventorySlot[] inventorySlots = new InventorySlot[10];

    private void Start()
    {
        inventorySlots = GetComponentsInChildren<InventorySlot>();
    }

    public bool AddItem(Item item, int amount = 1)
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.IsEmpty)
            {
                slot.item = item;
                slot.count++;
                return true;
            }
        }

        return false;
    }

    public bool CheckInventory()
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.IsEmpty)
            {
                return true;
            }
        }

        return false;
    }
}
