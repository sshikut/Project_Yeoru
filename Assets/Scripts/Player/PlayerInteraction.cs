using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 1f;
    public LayerMask interactLayer;
    public KeyCode interactKey = KeyCode.E;

    private Vector2 lookDirection = Vector2.down; // �⺻ ����

    void Update()
    {
        // ����Ű �Է� ����
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input != Vector2.zero)
            lookDirection = input.normalized;

        // ��ȣ�ۿ� Ű �Է�
        if (Input.GetKeyDown(interactKey))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, lookDirection, interactDistance, interactLayer);

            if (hit.collider != null && hit.collider.CompareTag("NPC"))
            {
                // IInteractable �������̽��ε� ó�� ����
                hit.collider.GetComponent<NPC>().StartDialogue();
            }
        }

        // (����) ����׿� ���� �ð�ȭ
        Debug.DrawRay(transform.position, lookDirection * interactDistance, Color.yellow);
    }
}
