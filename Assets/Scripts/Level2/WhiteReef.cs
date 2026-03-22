using UnityEngine;

public class WhiteReef : MonoBehaviour
{
    [Header("Colors")]
    [Tooltip("Sun hitting, No Water (Default/Bleached)")]
    public Color bleachedColor = Color.gray;
    
    [Tooltip("Use this to map the old deadColor to bleachedColor in the inspector if needed, or update manually")]
    public Color deadColor = Color.gray; // Keeping this temporarily to avoid losing inspector data or errors if referenced elsewhere, but logic will use bleachedColor. Wait, better to just use deadColor and rename the label or add new fields.

    [Tooltip("Sun Blocked, No Water (Shadowed)")]
    public Color shadowedColor = new Color(0.8f, 0.8f, 0.8f); // Light gray
    
    [Tooltip("Sun Blocked, Has Water (Alive)")]
    public Color aliveColor = Color.white;
    
    [Tooltip("Sun hitting, Has Water (Thermal Shock/Dead)")]
    public Color thermalShockColor = Color.red;

    [Tooltip("Sun hitting, Has Water, Acclimated (Solved)")]
    public Color solvedColor = Color.green;

    [Header("Detection")]
    public string coldWaterTag = "ColdWater";

    [Tooltip("SpriteRenderer to change color")]
    [HideInInspector]public SpriteRenderer spriteRenderer;

    private bool isHitBySun = true; // Assume start in sun
    private bool isHitByColdWater;
    private bool isAcclimated = false;

    public bool IsSolved { get; private set; }

    private void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
            
        // Sync deadColor to bleachedColor if user set it previously
        if (bleachedColor == Color.gray && deadColor != Color.gray)
            bleachedColor = deadColor;

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
        if (spriteRenderer == null) return;

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

        if (isHitBySun)
        {
            if (isHitByColdWater)
            {
                if (isAcclimated)
                {
                    // Sun + Water + Acclimated = Solved (Thriving)
                    spriteRenderer.color = solvedColor;
                    IsSolved = true;
                }
                else
                {
                    // Sun + Water + Not Acclimated = Thermal Shock
                    spriteRenderer.color = thermalShockColor;
                }
            }
            else
            {
                // Sun + No Water = Bleached
                spriteRenderer.color = bleachedColor;
            }
        }
        else
        {
            if (isHitByColdWater)
            {
                // No Sun + Water = Alive / Acclimating
                spriteRenderer.color = aliveColor;
            }
            else
            {
                // No Sun + No Water = Shadowed
                spriteRenderer.color = shadowedColor;
            }
        }
    }
}
