using UnityEngine;

public class WhiteReef : MonoBehaviour
{
    [Header("Sprites (override another object's SpriteRenderer)")]
    [Tooltip("Target SpriteRenderer to change. If null, uses this GameObject's SpriteRenderer.")]
    [SerializeField] private SpriteRenderer targetSpriteRenderer;

    [Tooltip("Sun hitting, No Water (Default/Bleached)")]
    public Sprite bleachedSprite;

    [Tooltip("Sun Blocked, No Water (Shadowed)")]
    public Sprite shadowedSprite;

    [Tooltip("Sun Blocked, Has Water (Alive)")]
    public Sprite aliveSprite;

    [Tooltip("Sun hitting, Has Water (Thermal Shock/Dead)")]
    public Sprite thermalShockSprite;

    [Tooltip("Sun hitting, Has Water, Acclimated (Solved)")]
    public Sprite solvedSprite;

    [Header("Detection")]
    public string coldWaterTag = "ColdWater";

    private bool isHitBySun = true; // Assume start in sun
    private bool isHitByColdWater;
    private bool isAcclimated = false;

    public bool IsSolved { get; private set; }

    private void Start()
    {
        if (targetSpriteRenderer == null)
            targetSpriteRenderer = GetComponent<SpriteRenderer>();

        UpdateVisuals();
    }

    public void SetHitBySun(bool isHit)
    {
        if (isHitBySun != isHit)
        {
            isHitBySun = isHit;
            UpdateVisuals();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(coldWaterTag))
        {
            isHitByColdWater = true;
            UpdateVisuals();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(coldWaterTag))
        {
            isHitByColdWater = false;
            UpdateVisuals();
        }
    }

    private void UpdateVisuals()
    {
        if (targetSpriteRenderer == null)
            return;

        // Logic for Acclimation: Only acclimate if in water AND shade
        if (!isHitBySun && isHitByColdWater)
        {
            isAcclimated = true;
        }
        // If water is lost, acclimation is lost
        else if (!isHitByColdWater)
        {
            isAcclimated = false;
        }

        IsSolved = false;

        Sprite nextSprite = null;

        if (isHitBySun)
        {
            if (isHitByColdWater)
            {
                if (isAcclimated)
                {
                    // Sun + Water + Acclimated = Solved (Thriving)
                    nextSprite = solvedSprite;
                    IsSolved = true;
                }
                else
                {
                    // Sun + Water + Not Acclimated = Thermal Shock
                    nextSprite = thermalShockSprite;
                }
            }
            else
            {
                // Sun + No Water = Bleached
                nextSprite = bleachedSprite;
            }
        }
        else
        {
            if (isHitByColdWater)
            {
                // No Sun + Water = Alive / Acclimating
                nextSprite = aliveSprite;
            }
            else
            {
                // No Sun + No Water = Shadowed
                nextSprite = shadowedSprite;
            }
        }

        if (nextSprite != null && targetSpriteRenderer.sprite != nextSprite)
            targetSpriteRenderer.sprite = nextSprite;
    }
}
