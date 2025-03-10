using UnityEngine;

public class TestMouseMove : MonoBehaviour
{
    private bool isCollisionWall = false;
    [SerializeField] private float speed = 5.0f;
    private Rigidbody rb;
    private float timer = 0.0f;
    private float waitTime = 1.0f;
    private bool isWait = false;

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
                Debug.Log("Collision with wall");
                transform.position += transform.up * Time.deltaTime * speed;
                isWait = false;
                timer = 0.0f;
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
        if(isWait)
        {
            timer += Time.deltaTime;
            if(timer > waitTime)
            {
                isWait = false;
                timer = 0.0f;
                isCollisionWall = false;
                rb.constraints = RigidbodyConstraints.None;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            isCollisionWall = true;
            // x,z固定
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            Debug.Log("Collision Enter with wall");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            isWait = true;
            Debug.Log("Exit collision with wall");
        }
    }
}