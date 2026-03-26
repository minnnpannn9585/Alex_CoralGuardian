using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody2D playerRb;

    private void Reset()
    {
        playerRb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector2 moveDirection = new Vector2(horizontal, vertical).normalized;
        playerRb.velocity = moveDirection * moveSpeed;

        if(horizontal > 0)
        {
            transform.GetChild(0).localScale = new Vector3(-1, 1, 1) * 0.145f;
        }
        else if(horizontal < 0)
        {
            transform.GetChild(0).localScale = new Vector3(1, 1, 1) * 0.145f;
        }
    }
}