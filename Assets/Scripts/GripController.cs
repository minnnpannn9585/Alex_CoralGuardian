using UnityEngine;

public class GripController : MonoBehaviour
{
    public TurntableController[] turntables;
    [SerializeField] private float grabDistance = 0.8f;

    private bool isGrabbing = false;
    private float mindist = 5f;
    private int mindistIndex;

    Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // While not grabbing, keep finding the nearest turntable in range
        if (!isGrabbing)
        {
            mindist = 5f;

            for (int i = 0; i < turntables.Length; i++)
            {
                if (turntables[i] == null)
                    continue;

                float dist = Vector2.Distance(transform.position, turntables[i].GripPoint.position);
                if (dist < mindist)
                {
                    mindist = dist;
                    mindistIndex = i;
                }
            }
        }

        // Hold Space = Grab (start once)
        if (!isGrabbing && mindist <= grabDistance && Input.GetKey(KeyCode.Space))
        {
            TryGrab();
        }
        // Release Space = Release
        else if (isGrabbing && Input.GetKeyUp(KeyCode.Space))
        {
            TryRelease();
        }
    }

    private void FixedUpdate()
    {
        if (isGrabbing && turntables[mindistIndex] != null)
        {
            turntables[mindistIndex].UpdateRotation(transform.position);
        }
    }

    private void TryGrab()
    {
        isGrabbing = true;
        anim.SetBool("isGrabbing", true);
        turntables[mindistIndex].StartGrab(transform.position);
    }

    private void TryRelease()
    {
        isGrabbing = false;
        anim.SetBool("isGrabbing", false);
        turntables[mindistIndex].EndGrab();
    }
}