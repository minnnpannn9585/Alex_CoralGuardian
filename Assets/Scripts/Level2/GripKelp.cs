using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GripKelp : MonoBehaviour
{
    bool canGrip = false;
    public Transform targetKelp;
    Rigidbody2D targetRb;
    bool isGripping = false;

    void Update()
    {
        // Only attempt grip behavior when we have a nearby kelp
        if (canGrip && targetKelp != null)
        {
            // Hold space to grip and follow the player
            if (Input.GetKey(KeyCode.Space))
            {
                if (!isGripping)
                    StartGrip();

                // Make the kelp follow the player position (no gravity while gripping)
                targetKelp.position = transform.position;
            }
            // Release when space is not held
            else if (isGripping)
            {
                EndGrip();
            }
        }
    }

    void StartGrip()
    {
        if (targetRb != null)
        {
            targetRb.gravityScale = 0f;
            targetRb.velocity = Vector2.zero;
        }
        isGripping = true;
    }

    void EndGrip()
    {
        if (targetRb != null)
        {
            targetRb.gravityScale = 0.1f;
        }
        isGripping = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Kelp"))
        {
            canGrip = true;
            targetKelp = collision.transform;
            targetRb = collision.GetComponent<Rigidbody2D>();
            // Do NOT change gravity here; wait until player actually holds space
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Kelp"))
        {
            // Restore gravity on exit (guard against null)
            var rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.gravityScale = 0.1f;

            // Clear references and state
            canGrip = false;
            targetKelp = null;
            targetRb = null;
            isGripping = false;
        }
    }
}
