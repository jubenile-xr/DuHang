using UnityEngine;

public class BirdController : MonoBehaviour
{
    public float walkSpeed = 2.0f;
    public float flightSpeed = 5.0f;
    public float liftSpeed = 2.0f;
    public float turnSpeed = 3.0f;
    public float gravity = 9.8f;
    public float flapThreshold = 1.5f;

    private bool isFlying = false;
    private float verticalVelocity = 0.0f;
    private Transform hmd;
    private Vector3 lastHandVelocity;

    void Start()
    {
        hmd = GameObject.Find("OVRCameraRig").transform;
        lastHandVelocity = Vector3.zero;
    }

    void Update()
    {
        HandleWalking();
        HandleFlying();
        ApplyGravity();
    }

    void HandleWalking()
    {
        if (isFlying) return;

        Vector3 forwardDirection = new Vector3(hmd.forward.x, 0, hmd.forward.z).normalized;

        Vector2 moveInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
        transform.position += forwardDirection * moveInput.y * walkSpeed * Time.deltaTime;
    }

    void HandleFlying()
    {
        bool jumpPressed = OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch); // A °´Å¥

        Vector3 velocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
        float acceleration = (velocity - lastHandVelocity).magnitude / Time.deltaTime;
        lastHandVelocity = velocity;

        if (!isFlying && (jumpPressed || acceleration > flapThreshold))
        {
            isFlying = true;
            verticalVelocity = liftSpeed;
        }
        else if (isFlying && acceleration > flapThreshold)
        {
            verticalVelocity += liftSpeed * Time.deltaTime;
            transform.position += hmd.forward * flightSpeed * Time.deltaTime;
        }
    }

    void ApplyGravity()
    {
        if (isFlying)
        {
            verticalVelocity -= gravity * Time.deltaTime;
            transform.position += new Vector3(0, verticalVelocity * Time.deltaTime, 0);

            if (transform.position.y <= 0.5f)
            {
                isFlying = false;
                verticalVelocity = 0;
                transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
            }
        }
    }
}
