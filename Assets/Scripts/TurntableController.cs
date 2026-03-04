using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TurntableController : MonoBehaviour
{
    [Header("转盘物理设置")]
    [SerializeField] private Rigidbody2D turntableRb;
    [SerializeField] private float angularDamping = 0.5f;

    public Transform GripPoint { get; private set; }
    private bool isBeingGrabbed = false;

    // 【新增】关键变量：记录抓握瞬间的角度偏移量
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
            turntableRb.angularDrag = angularDamping;
            turntableRb.gravityScale = 0;
        }
    }

    public void StartGrab(Vector2 playerPosition)
    {
        isBeingGrabbed = true;
        turntableRb.isKinematic = true;

        // 【核心修改 1/2】记录抓握瞬间的偏移量
        // 1. 计算抓握瞬间，玩家相对于转盘的角度
        Vector2 dirToPlayer = playerPosition - (Vector2)transform.position;
        float playerAngle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;
        
        // 2. 计算偏移量 = 转盘当前角度 - 玩家当前角度
        // 这样就能把玩家的位置“映射”到转盘当前的状态上
        grabAngleOffset = turntableRb.rotation - playerAngle;
    }

    public void EndGrab()
    {
        isBeingGrabbed = false;
        turntableRb.isKinematic = false;
    }

    public void UpdateRotation(Vector2 playerPosition)
    {
        if (!isBeingGrabbed || turntableRb == null) return;

        // 【核心修改 2/2】应用偏移量，保持相对方向
        // 1. 计算玩家现在相对于转盘的角度
        Vector2 currentDirToPlayer = playerPosition - (Vector2)transform.position;
        float currentPlayerAngle = Mathf.Atan2(currentDirToPlayer.y, currentDirToPlayer.x) * Mathf.Rad2Deg;

        // 2. 目标角度 = 玩家现在的角度 + 一开始记录的偏移量
        // 这样无论玩家怎么动，转盘都会保持抓握瞬间的那个“姿势”跟着转
        float targetAngle = currentPlayerAngle + grabAngleOffset;

        turntableRb.MoveRotation(targetAngle);
    }
}