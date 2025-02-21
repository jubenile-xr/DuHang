using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class BirdController : MonoBehaviour
{
    public float speed = 1.0f;
    public float verticalSensitivity = 1.0f;
    public float minHandMovementThreshold = 0.1f; // 最小手部移动阈值
    private Vector3 moveDirection;
    private InputDevice rightHandDevice;
    private InputDevice leftHandDevice;
    private Transform vrHeadset;

    void Start()
    {
        var inputDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, inputDevices);
        if (inputDevices.Count > 0)
        {
            rightHandDevice = inputDevices[0];
        }

        inputDevices.Clear();
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, inputDevices);
        if (inputDevices.Count > 0)
        {
            leftHandDevice = inputDevices[0];
        }

        vrHeadset = Camera.main.transform;
    }

    void Update()
    {
        if (rightHandDevice.isValid && leftHandDevice.isValid)
        {
            Vector3 rightHandAcceleration, leftHandAcceleration;
            if (rightHandDevice.TryGetFeatureValue(CommonUsages.deviceAcceleration, out rightHandAcceleration) &&
                leftHandDevice.TryGetFeatureValue(CommonUsages.deviceAcceleration, out leftHandAcceleration))
            {
                float handMovementDifference = Mathf.Abs(rightHandAcceleration.y - leftHandAcceleration.y);

                if (handMovementDifference > minHandMovementThreshold)
                {
                    moveDirection.y = (rightHandAcceleration.y + leftHandAcceleration.y) / 2 * verticalSensitivity;
                }
                else
                {
                    moveDirection.y = 0; // 若双手幅度太小，则不继续飞翔
                }
            }
        }

        moveDirection.x = vrHeadset.forward.x * speed * Time.deltaTime;
        moveDirection.z = vrHeadset.forward.z * speed * Time.deltaTime;
        transform.position += moveDirection;
    }
}
