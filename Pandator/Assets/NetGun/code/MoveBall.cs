using UnityEngine;

public class MoveBall : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(Vector3.forward * speed, ForceMode.VelocityChange);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // No need to move forward in Update anymore
    }
}