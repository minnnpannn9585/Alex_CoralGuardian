using UnityEngine;

public class Lobster : MonoBehaviour
{
    public bool isFree;
    public bool hasTarget;
    
    [Header("Settings")]
    public float moveSpeed = 1f;
    public float eatTime = 3f; // 吃海胆需要3秒

    // 状态管理
    private Transform currentTargetTrans;
    private SeaUrchin currentOwner; // 当前目标属于哪个海胆群(外层还是内层)
    private bool isEating;
    private float eatTimer;

    Rigidbody2D rb;
    public SeaUrchin outer;
    public SeaUrchin inner;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (outer == null && inner == null)
        {
            Debug.LogWarning($"[Lobster] '{name}' missing SeaUrchin references! Please assign 'Inner' and 'Outer' SeaUrchin groups in the Inspector.");
        }
    }

    void Update()
    {
        // 没自由就什么都不做
        if (!isFree) return;

        // 防止物理碰撞导致被弹飞：设为 Kinematic 并清空速度
        if (!rb.isKinematic) rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // 1. 如果没有目标，尝试寻找目标
        if (!hasTarget)
        {
            FindNextTarget();
            return; 
        }

        // 容错：如果目标在半路被意外销毁了（虽然有预定机制，但防患于未然）
        if (currentTargetTrans == null)
        {
            ResetTargetState();
            return;
        }

        // 2. 移动或进食逻辑
        if (!isEating)
        {
            // --- 移动阶段 ---
            // 可选：让龙虾朝向目标
            Vector3 direction = currentTargetTrans.position - transform.position;
            if (direction != Vector3.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; 
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }

            transform.position = Vector3.MoveTowards(transform.position, currentTargetTrans.position, moveSpeed * Time.deltaTime);

            // 判断距离，非常接近时认为到达
            if (Vector3.Distance(transform.position, currentTargetTrans.position) < 0.05f)
            {
                // 到达目标，开始进食
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
                // 时间到，通知海胆群销毁目标
                if (currentOwner != null)
                {
                    currentOwner.EatTarget(currentTargetTrans);
                }
                
                // 重置状态，准备找下一个
                ResetTargetState();
            }
        }
    }

    void FindNextTarget()
    {
        // 优先级逻辑：
        // 先尝试在外层找，TryGetAvailableTarget 会返回 false 如果全被吃完了或者全正在被别人吃
        if (outer != null && outer.TryGetAvailableTarget(this, out currentTargetTrans))
        {
            currentOwner = outer;
            hasTarget = true;
            isEating = false;
        }
        // 外层没得选，去内层找
        else if (inner != null && inner.TryGetAvailableTarget(this, out currentTargetTrans))
        {
            currentOwner = inner;
            hasTarget = true;
            isEating = false;
        }
        else
        {
            // 内外都没得吃，发呆
            hasTarget = false;
        }
    }

    void ResetTargetState()
    {
        hasTarget = false;
        isEating = false;
        currentTargetTrans = null;
        currentOwner = null;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("FishingNet"))
        {
            isFree = true;
            GetComponent<Collider2D>().isTrigger = true;
        }
    }
}
