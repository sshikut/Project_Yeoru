using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Item item;
    public int count;

    public Sprite image;

    public Image itemImage;
    public TextMeshProUGUI itemName;

    public bool IsEmpty => item == null || count <= 0;

    private void Update()
    {
        if (item == null)
        {
            itemImage.gameObject.SetActive(false);
            itemName.gameObject.SetActive(false);
        }
        else
        {
            itemImage.gameObject.SetActive(true);
            itemName.gameObject.SetActive(true);
        }
    }

}
