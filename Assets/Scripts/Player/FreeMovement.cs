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
        movement = movement.normalized; // �밢�� �̵� �ӵ� ����
    }

    void FixedUpdate()
    {
        rb.velocity = movement * moveSpeed;
    }
}
