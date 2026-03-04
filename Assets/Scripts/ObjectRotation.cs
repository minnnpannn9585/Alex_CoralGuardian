using UnityEngine;

public class ObjectRotation : MonoBehaviour
{
    public float rotationSpeed = 90f;
    
    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }
}