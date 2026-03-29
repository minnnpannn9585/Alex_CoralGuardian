using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody2D playerRb;

    [Header("Wall Blocking (when player collider is Trigger)")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float castDistance = 0.05f;
    [SerializeField] private Collider2D playerCollider;

    private readonly RaycastHit2D[] hits = new RaycastHit2D[8];

    private void Reset()
    {
        playerRb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
    }

    private void Awake()
    {
        if (playerRb == null) playerRb = GetComponent<Rigidbody2D>();
        if (playerCollider == null) playerCollider = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector2 inputDir = new Vector2(horizontal, vertical).normalized;

        Vector2 desiredVelocity = inputDir * moveSpeed;

        if (playerCollider != null && playerCollider.isTrigger)
        {
            // X axis block only if pushing into a wall
            if (desiredVelocity.x != 0f)
            {
                Vector2 dirX = new Vector2(Mathf.Sign(desiredVelocity.x), 0f);
                if (IsBlockedByWallWhenMoving(dirX))
                    desiredVelocity.x = 0f;
            }

            // Y axis block only if pushing into a wall
            if (desiredVelocity.y != 0f)
            {
                Vector2 dirY = new Vector2(0f, Mathf.Sign(desiredVelocity.y));
                if (IsBlockedByWallWhenMoving(dirY))
                    desiredVelocity.y = 0f;
            }
        }

        playerRb.velocity = desiredVelocity;

        if (horizontal > 0)
        {
            transform.GetChild(0).localScale = new Vector3(-1, 1, 1) * 0.145f;
        }
        else if (horizontal < 0)
        {
            transform.GetChild(0).localScale = new Vector3(1, 1, 1) * 0.145f;
        }
    }

    private bool IsBlockedByWallWhenMoving(Vector2 moveDirNormalized)
    {
        var filter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = wallLayer,
            useTriggers = false
        };

        int hitCount = playerCollider.Cast(moveDirNormalized, filter, hits, castDistance);

        for (int i = 0; i < hitCount; i++)
        {
            // If we're moving into the surface (opposite the normal), then block.
            // Example: wall normal points right, moving left => dot < 0 => blocked.
            Vector2 n = hits[i].normal;
            if (Vector2.Dot(moveDirNormalized, n) < -0.01f)
                return true;
        }

        return false;
    }
}