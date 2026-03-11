using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lobster : MonoBehaviour
{
    public bool isFree;
    public bool isOuter;
    public bool hasTarget;
    Vector3 targetPos;
    Rigidbody2D rb;
    public SeaUrchin outer;
    public SeaUrchin inner;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFree && !hasTarget)
        {
            rb.gravityScale = 0;
            SelectTarget();
            
        }
        if (hasTarget)
        {
            rb.MovePosition(Vector2.MoveTowards(transform.position, targetPos, 1f * Time.deltaTime));
        }

    }

    void SelectTarget()
    {
        if(isOuter)
        {
            int rand = Random.Range(0, outer.curchinTrans.Length);
            targetPos = outer.curchinTrans[rand].position;
            hasTarget = true;
        }
        else
        {
            int rand = Random.Range(0, inner.curchinTrans.Length);
            targetPos = inner.curchinTrans[rand].position;
            hasTarget = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "FishingNet")
        {
            isFree = true;
        }
    }
}
