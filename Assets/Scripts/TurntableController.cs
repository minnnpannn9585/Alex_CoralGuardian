using UnityEngine;

public class TurntableController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D turntableRb;

    public Transform GripPoint { get; private set; }
    private bool isBeingGrabbed = false;
    
    private float grabAngleOffset;

    private void Awake()
    {
        turntableRb = GetComponent<Rigidbody2D>();
        GripPoint = transform.Find("GripPoint");
    }

    private void Reset()
    {
        Awake();
        if (turntableRb != null)
        {
            turntableRb.constraints = RigidbodyConstraints2D.FreezePosition;
            turntableRb.gravityScale = 0;
        }
    }

    public void StartGrab(Vector2 playerPosition)
    {
        isBeingGrabbed = true;
        //turntableRb.isKinematic = true;
        
        Vector2 dirToPlayer = playerPosition - (Vector2)transform.position;
        float playerAngle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;

        grabAngleOffset = turntableRb.rotation - playerAngle;
    }

    public void EndGrab()
    {
        isBeingGrabbed = false;
        //turntableRb.isKinematic = false;
    }

    public void UpdateRotation(Vector2 playerPosition)
    {
        if (!isBeingGrabbed || turntableRb == null) return;

        Vector2 currentDirToPlayer = playerPosition - (Vector2)transform.position;
        float currentPlayerAngle = Mathf.Atan2(currentDirToPlayer.y, currentDirToPlayer.x) * Mathf.Rad2Deg;

        float targetAngle = currentPlayerAngle + grabAngleOffset;

        turntableRb.MoveRotation(targetAngle);
    }
}