using UnityEngine;

public class TestMouseMove : MonoBehaviour
{
    private bool isCollisionWall = false;
    [SerializeField] private float speed = 5.0f;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Move the object
        if (Input.GetKey(KeyCode.W))
        {
            if (isCollisionWall)
            {
                transform.position += transform.up * Time.deltaTime * speed;
            }
            else
            {
                transform.position += transform.forward * Time.deltaTime * speed;
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * Time.deltaTime * speed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            isCollisionWall = true;
            // x,z, rotait固定
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            isCollisionWall = false;
        }
    }
}