using UnityEngine;

public class GripController : MonoBehaviour
{
    [Header("抓握设置")]
    [SerializeField] private TurntableController turntable; // 拖入转盘
    [SerializeField] private float grabDistance = 0.8f; // 抓握判定距离（稍微调大一点方便操作）
    
    private bool isGrabbing = false;

    private void Update()
    {
        if (turntable == null || turntable.GripPoint == null) return;

        // 计算玩家与抓握点的距离
        float dist = Vector2.Distance(transform.position, turntable.GripPoint.position);

        // 空格键切换状态
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isGrabbing && dist <= grabDistance)
            {
                TryGrab();
            }
            else if (isGrabbing)
            {
                TryRelease();
            }
        }
    }

    private void FixedUpdate()
    {
        // 如果抓着，就持续告诉转盘：“看我看我看我”
        if (isGrabbing && turntable != null)
        {
            turntable.UpdateRotation(transform.position);
        }
    }

    private void TryGrab()
    {
        isGrabbing = true;
        turntable.StartGrab();
        Debug.Log("抓稳了，带着我跑！");
    }

    private void TryRelease()
    {
        isGrabbing = false;
        turntable.EndGrab();
        Debug.Log("松开咯！");
    }

    // 调试：在Scene视图里画个圈显示抓握范围
    private void OnDrawGizmosSelected()
    {
        if (turntable != null && turntable.GripPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(turntable.GripPoint.position, grabDistance);
        }
    }
}