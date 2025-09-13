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

    void Update()
    {
        if (!GameManager.Instance.IsUIOpen())
        {
            // 방향키 입력 감지
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (input != Vector2.zero)
                lookDirection = input.normalized;

            // 상호작용 키 입력
            if (Input.GetKeyDown(interactKey))
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, lookDirection, interactDistance, interactLayer);

                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("NPC"))
                    {
                        GameManager.Instance.OpenUI(GameUIState.Dialogue);
                        hit.collider.GetComponent<NPC>().StartDialogue();
                    }

                    if (hit.collider.CompareTag("Item"))
                    {
                        if (inventory.CheckInventory())
                        {
                            inventory.AddItem(hit.collider.GetComponent<ItemPickUp>().item);
                            Destroy(hit.collider.gameObject);
                        }
                        else
                        {
                            Debug.Log("Inventory Full");
                        }
                    }

                    if (hit.collider.CompareTag("Pipe"))
                    {

                    }
                }
                
            }

            // (선택) 디버그용 레이 시각화
            Debug.DrawRay(transform.position, lookDirection * interactDistance, Color.yellow);
        }
    }
}
