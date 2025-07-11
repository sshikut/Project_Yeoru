using UnityEngine;

public class FreeMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized; // 대각선 이동 속도 보정
    }

    void FixedUpdate()
    {
        rb.velocity = movement * moveSpeed;
    }
}
