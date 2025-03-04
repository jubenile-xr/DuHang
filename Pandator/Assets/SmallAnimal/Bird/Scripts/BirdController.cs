using UnityEngine;
using UnityEngine.XR;

public class BirdController : MonoBehaviour
{
    public float walkSpeed = 1.5f;
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
        hmd = Camera.main.transform;
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

        InputDevice rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (!rightHandDevice.isValid) return;

        Vector2 moveInput;
        if (rightHandDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out moveInput))
        {
            transform.position += forwardDirection * moveInput.y * walkSpeed * Time.deltaTime;
        }
    }

    void HandleFlying()
    {
        InputDevice rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (!rightHandDevice.isValid) return;

        Vector3 velocity;
        if (rightHandDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out velocity))
        {
            float acceleration = (velocity - lastHandVelocity).magnitude / Time.deltaTime;
            lastHandVelocity = velocity;

            if (!isFlying && acceleration > flapThreshold)
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
