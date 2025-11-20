using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    public InventorySlot[] inventorySlots = new InventorySlot[10];
    public GameObject inventoryObject;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환에도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        inventorySlots = inventoryObject.GetComponentsInChildren<InventorySlot>();
    }

    public bool AddItem(Item item, int amount = 1)
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.IsEmpty)
            {
                slot.item = item;
                slot.count++;
                slot.itemImage.sprite = item.icon;
                slot.itemName.text = item.itemName;

                return true;
            }
        }

        return false;
    }

    public void UseItem(string itemName)
    {
        foreach (var slot in inventorySlots)
        {
            if (!slot.IsEmpty && slot.itemName.text.Equals(itemName))
            {
                slot.item = null;
                slot.count--;
                slot.itemImage.sprite = null;
                slot.itemName.text = null;

                // 아이템 효과

                Debug.Log($"아이템 사용 : {itemName}");
            }
        }
    }

    public bool CheckInventory() // 인벤토리에 빈 칸이 있는지 확인하는 메소드
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

    public bool CheckItem(string itemName) // 해당 아이템이 있는지 확인하는 메소드
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.itemName.text.Equals(itemName))
            {
                return true;
            }
        }

        return false;
    }
}
