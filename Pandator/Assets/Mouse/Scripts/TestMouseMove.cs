using UnityEngine;

public class TestMouseMove : MonoBehaviour
{
    private bool isCollisionWall = false;
    [Header("歩く速度")]
    [SerializeField] private float walkSpeed = 5.0f;
    [Header("登る速度")]
    [SerializeField] private float climbSpeed = 2.0f;
    private Rigidbody rb;
    [Header("ねずみのオブジェクト")]
    [SerializeField] private GameObject camera;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            if (isCollisionWall)
            {
                transform.position += transform.up * Time.deltaTime * climbSpeed;
            }
            else
            {
                transform.position += transform.forward * Time.deltaTime * walkSpeed;
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * Time.deltaTime * walkSpeed;
        }
        camera.transform.position = transform.position;
        transform.rotation = camera.transform.rotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            isCollisionWall = true;
            // x,z, rotait固定
            rb.useGravity = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            rb.useGravity = true;
            isCollisionWall = false;
        }
    }
}