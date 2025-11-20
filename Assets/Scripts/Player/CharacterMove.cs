using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    public float speed;
    public int walkCount;
    private int currentWalkCount;

    private Vector3 vector;

    private BoxCollider2D boxCollider;
    private LayerMask layerMask;
    public Animator animator;

    public float runSpeed;
    private float applyRunSpeed;
    private bool applyRunFlag = false;
    private bool isMoving = false;

    void Update()
    {
        // if (!canMove) return;

        if (!GameManager.Instance.CanPlayerMove) return;

        if (!isMoving)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                isMoving = true;
                StartCoroutine(MoveCoroutine());
            }
        }
    }

    IEnumerator MoveCoroutine()
    {
        while (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
        {
            if (!GameManager.Instance.CanPlayerMove)
            {
                break;
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                applyRunSpeed = runSpeed;
                applyRunFlag = true;
            }
            else
            {
                applyRunSpeed = 0;
                applyRunFlag = false;
            }


            vector.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), transform.position.z);

            if (vector.x != 0)
                vector.y = 0;


            animator.SetFloat("DirX", vector.x);
            animator.SetFloat("DirY", vector.y);
            animator.SetBool("Walking", true);

            while (currentWalkCount < walkCount)
            {
                // UI가 열렸어도 한 칸은 마저 진행
                transform.Translate(vector.x * (speed + applyRunSpeed) * 0.01f,
                                    vector.y * (speed + applyRunSpeed) * 0.01f, 0);

                SnapToPixelGrid();

                if (applyRunFlag)
                    currentWalkCount++;
                currentWalkCount++;

                yield return new WaitForSeconds(0.01f);
            }

            currentWalkCount = 0;

            if (GameManager.Instance.IsUIOpen())
                break;

        }
        animator.SetBool("Walking", false);
        isMoving = false;

    }

    public void SnapToPixelGrid()
    {
        float ppu = 32f;
        Vector3 pos = transform.position;
        pos.x = Mathf.Round(pos.x * ppu) / ppu;
        pos.y = Mathf.Round(pos.y * ppu) / ppu;
        transform.position = pos;
    }
}