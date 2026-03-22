using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SunHitReef : MonoBehaviour
{
    [Header("Ray Settings")]
    [Tooltip("Layer mask for obstacles (Kelp) and targets (Reef)")]
    public LayerMask hitLayers;
    public float rayDistance = 10f;
    [Tooltip("Angle in degrees (0 = Right, 90 = Up, -90 = Down)")]
    public float angleDegrees = -90f;
    public Vector3 rayOriginOffset = Vector3.zero;

    [Header("Target Reef")]
    [Tooltip("The specific reef this sun beam is targeting")]
    public WhiteReef targetReef;

    [Header("Blocking Settings")]
    [Tooltip("Tag of objects that should NOT block the sun (e.g., Water)")]
    public string transparentTag = "ColdWater";

    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        // Default line settings if not set
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        CastRay();
    }

    void CastRay()
    {
        // Calculate direction
        Vector2 direction = Quaternion.Euler(0, 0, angleDegrees) * Vector2.right;
        Vector2 origin = transform.position + rayOriginOffset;

        // Use RaycastAll to handle complex overlapping
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, rayDistance, hitLayers);
        
        // Sort by distance to process them in order from the sun
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        Vector3 endPos = origin + direction * rayDistance;
        bool isHittingTarget = false;
        
        foreach (var hit in hits)
        {
            if (hit.collider == null) continue;

            // 1. Check if it's the target (or part of it)
            if (targetReef != null && (hit.collider.gameObject == targetReef.gameObject || hit.transform.IsChildOf(targetReef.transform)))
            {
                isHittingTarget = true;
                endPos = hit.point;
                break; // Found the target, stop.
            }

            // 2. Check if it's a transparent object (like Water)
            // If it has the transparent tag (e.g. "ColdWater"), we ignore it and continue the ray.
            if (hit.collider.CompareTag(transparentTag))
            {
                continue;
            }

            // 3. Otherwise, it is an Obstacle (e.g. Kelp)
            // It blocks the ray.
            // Note: We removed the (!hit.collider.isTrigger) check. 
            // This means Triggers (like Kelp might be) WILL block the sun, unless they are tagged "ColdWater".
            endPos = hit.point;
            break; 
        }

        // Apply logic to target reef
        if (targetReef != null)
        {
            targetReef.SetHitBySun(isHittingTarget);
        }

        // Update visuals
        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, endPos);
    }
}
