using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 100f; 

    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
