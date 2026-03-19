using System.Collections.Generic;
using UnityEngine;

public class SeaUrchin : MonoBehaviour
{
    // 在编辑器里把海胆子物体拖进去，或者代码自动获取
    public Transform[] curchinTrans;

    // 记录已经被预定的海胆ID (InstanceID -> 预定它的龙虾)
    private Dictionary<int, Lobster> reservations = new Dictionary<int, Lobster>();

    private void Awake()
    {
        // 如果没有手动赋值，自动获取所有子物体作为海胆集合
        if (curchinTrans == null || curchinTrans.Length == 0)
        {
            List<Transform> children = new List<Transform>();
            foreach (Transform child in transform)
            {
                children.Add(child);
            }
            curchinTrans = children.ToArray();
        }
    }

    /// <summary>
    /// 尝试申请一个没人选的目标
    /// </summary>
    public bool TryGetAvailableTarget(Lobster asker, out Transform target)
    {
        target = null;
        List<Transform> available = new List<Transform>();

        foreach (var trans in curchinTrans)
        {
            // 过滤掉空的（已经被吃掉销毁的）
            if (trans == null) continue;
            
            // 过滤掉不激活的
            if (!trans.gameObject.activeSelf) continue;

            // 过滤掉已经被别人预定的
            int id = trans.GetInstanceID();
            if (reservations.ContainsKey(id)) continue;

            available.Add(trans);
        }

        // 如果没有可用目标
        if (available.Count == 0) return false;

        // 随机取一个
        target = available[Random.Range(0, available.Count)];
        
        // 登记预定
        reservations.Add(target.GetInstanceID(), asker);
        return true;
    }

    /// <summary>
    /// 吃完销毁
    /// </summary>
    public void EatTarget(Transform target)
    {
        if (target == null) return;
        
        int id = target.GetInstanceID();
        if (reservations.ContainsKey(id))
        {
            reservations.Remove(id);
        }
        
        Destroy(target.gameObject);
    }
}