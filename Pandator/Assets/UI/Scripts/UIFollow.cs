using UnityEngine;

public class FollowCameraUI : MonoBehaviour
{
    public Transform cameraTransform;

    void Update()
    {
        transform.position = cameraTransform.position;
        transform.rotation = cameraTransform.rotation;
    }
}
