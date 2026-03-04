using UnityEngine;

public class GripController : MonoBehaviour
{
    public TurntableController[] turntables;
    [SerializeField] private float grabDistance = 0.8f;
    
    private bool isGrabbing = false;
    private float mindist = 5f;
    private int mindistIndex;

    private void Update()
    {
        for (int i = 0; i < turntables.Length; i++)
        {
            float dist = Vector2.Distance(transform.position, turntables[i].GripPoint.position);
            if (dist < mindist)
            {
                mindist = dist;
                mindistIndex = i;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isGrabbing && mindist <= grabDistance)
            {
                TryGrab();
            }
            else if (isGrabbing)
            {
                TryRelease();
            }
        }

        mindist = 5f;
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
        turntables[mindistIndex].StartGrab(transform.position); 
    }

    private void TryRelease()
    {
        isGrabbing = false;
        turntables[mindistIndex].EndGrab();
    }
}