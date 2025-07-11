using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 1f;
    public LayerMask interactLayer;
    public KeyCode interactKey = KeyCode.E;

    private Vector2 lookDirection = Vector2.down; // 기본 방향

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
                // IInteractable 인터페이스로도 처리 가능
                hit.collider.GetComponent<NPC>().StartDialogue();
            }
        }

        // (선택) 디버그용 레이 시각화
        Debug.DrawRay(transform.position, lookDirection * interactDistance, Color.yellow);
    }
}
