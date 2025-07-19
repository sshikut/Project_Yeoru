using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 1f;
    public LayerMask interactLayer;
    public KeyCode interactKey = KeyCode.E;
    public Inventory inventory;

    private Vector2 lookDirection = Vector2.down; // 기본 방향

    void Start()
    {
        inventory = GetComponentInChildren<Inventory>();
    }

    void Update()
    {
        // 방향키 입력 감지
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input != Vector2.zero)
            lookDirection = input.normalized;

        // 상호작용 키 입력
        if (Input.GetKeyDown(interactKey))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, lookDirection, interactDistance, interactLayer);

            if (hit.collider != null && hit.collider.CompareTag("NPC"))
            {
                hit.collider.GetComponent<NPC>().StartDialogue();
            }

            if (hit.collider.CompareTag("Item"))
            {
                Debug.Log("체크1");
                if (inventory.CheckInventory())
                {
                    Debug.Log("체크2");
                    inventory.AddItem(hit.collider.GetComponent<ItemPickUp>().item);
                }
                else
                {
                    Debug.Log("Inventory Full");
                }
            }
        }

        // (선택) 디버그용 레이 시각화
        Debug.DrawRay(transform.position, lookDirection * interactDistance, Color.yellow);
    }
}
