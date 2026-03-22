using UnityEngine;

public class Lobstere : MonoBehaviour
{
    public bool isFree;
    public bool hasTarget;

    [Header("Settings")]
    public float moveSpeed = 1f;
    public float eatTime = 3f; // 吃/等待的时间

    [Header("Targeting")]
    // 固定的目标数组，请在 Inspector 中按顺序拖入海胆
    public Transform[] targetArray; 
    
    // 状态管理
    private int currentTargetIndex = 0; // 当前数组索引
    private Transform currentTargetTrans;
    private bool isEating;
    private float eatTimer;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 没自由就什么都不做
        if (!isFree) return;

        // 防止物理碰撞导致被弹飞：设为 Kinematic 并清空速度
        if (!rb.isKinematic) rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // 1. 如果没有目标，尝试从数组中获取下一个
        if (!hasTarget)
        {
            FindNextFixedTarget();
            return;
        }

        // 容错：如果目标在半路被意外销毁了（空），跳过它去找下一个
        if (currentTargetTrans == null)
        {
            currentTargetIndex++; // 索引后移
            ResetTargetState();
            return;
        }

        // 2. 移动或进食逻辑
        if (!isEating)
        {
            // --- 移动阶段 ---
            // 朝向目标
            Vector3 direction = currentTargetTrans.position - transform.position;
            if (direction != Vector3.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }

            // 移动
            transform.position = Vector3.MoveTowards(transform.position, currentTargetTrans.position, moveSpeed * Time.deltaTime);

            // 判断距离，非常接近时认为到达
            if (Vector3.Distance(transform.position, currentTargetTrans.position) < 0.05f)
            {
                // 到达目标，开始进食/等待
                isEating = true;
                eatTimer = eatTime;
            }
        }
        else
        {
            // --- 进食阶段 (等待3秒) ---
            eatTimer -= Time.deltaTime;

            if (eatTimer <= 0)
            {
                // 时间到，处理目标销毁
                HandleTargetProcess();
                
                // 准备找数组里的下一个
                currentTargetIndex++;
                ResetTargetState();
            }
        }
    }

    // 寻找数组中的下一个有效目标
    void FindNextFixedTarget()
    {
        // 只要索引在数组范围内
        while (currentTargetIndex < targetArray.Length)
        {
            // 如果这个位置有物体（没被别人吃掉）
            if (targetArray[currentTargetIndex] != null)
            {
                currentTargetTrans = targetArray[currentTargetIndex];
                hasTarget = true;
                isEating = false;
                return;
            }
            
            // 如果当前索引是空的（null），说明已经被销毁了，直接跳过看下一个
            currentTargetIndex++;
        }

        // 如果循环结束还没找到，说明数组走完了，或者都空了
        hasTarget = false;
    }

    void HandleTargetProcess()
    {
        if (currentTargetTrans != null)
        {
            // 尝试获取目标父级上的 SeaUrchin 脚本来正规销毁（维持计数器正确）
            SeaUrchin manager = currentTargetTrans.GetComponentInParent<SeaUrchin>();
            if (manager != null)
            {
                manager.EatTarget(currentTargetTrans);
            }
            else
            {
                // 如果没有管理器，直接销毁 GameObject
                Destroy(currentTargetTrans.gameObject);
            }
        }
    }

    void ResetTargetState()
    {
        hasTarget = false;
        isEating = false;
        currentTargetTrans = null;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 当离开笼子（FishingNet）时获得自由
        if (collision.gameObject.CompareTag("FishingNet"))
        {
            isFree = true;
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.isTrigger = true; // 变为 Trigger 避免后续物理推挤
        }
    }
}
