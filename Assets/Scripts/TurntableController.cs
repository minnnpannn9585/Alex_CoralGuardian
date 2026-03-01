using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TurntableController : MonoBehaviour
{
    [Header("转盘物理设置")]
    [SerializeField] private Rigidbody2D turntableRb;
    [SerializeField] private float angularDamping = 0.5f;

    public Transform GripPoint { get; private set; } // 外部读取抓握点
    private bool isBeingGrabbed = false; // 标记当前是否被抓握

    private void Reset()
    {
        turntableRb = GetComponent<Rigidbody2D>();
        GripPoint = transform.Find("GripPoint"); // 自动查找子物体

        if (turntableRb != null)
        {
            // 冻结位置，只允许旋转
            turntableRb.constraints = RigidbodyConstraints2D.FreezePosition;
            turntableRb.angularDrag = angularDamping;
            turntableRb.gravityScale = 0;
        }
    }

    /// <summary>
    /// 外部调用：开始抓握
    /// </summary>
    public void StartGrab()
    {
        isBeingGrabbed = true;
        // 抓握时切换为动力学运动，让旋转更跟手
        turntableRb.isKinematic = true; 
    }

    /// <summary>
    /// 外部调用：结束抓握
    /// </summary>
    public void EndGrab()
    {
        isBeingGrabbed = false;
        // 松开时恢复物理，让转盘靠惯性转一会
        turntableRb.isKinematic = false;
    }

    /// <summary>
    /// 每帧调用：让转盘对准玩家
    /// </summary>
    public void UpdateRotation(Vector2 playerPosition)
    {
        if (!isBeingGrabbed || turntableRb == null) return;

        // 核心数学逻辑：
        // 1. 计算从转盘圆心 指向 玩家 的方向
        Vector2 directionToPlayer = playerPosition - (Vector2)transform.position;
        
        // 2. 将方向转换为角度
        float targetAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        // 3. 直接设置角度（因为isKinematic=true，所以瞬间对齐，保证三点一线）
        // 如果你想要一点点延迟感，可以把这里改成 Lerp 插值
        turntableRb.MoveRotation(targetAngle);
    }
}