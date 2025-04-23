using UnityEngine;

public class FollowCameraUI : MonoBehaviour
{
    public Transform cameraTransform;

    void Update()
    {
        Vector3 pos = cameraTransform.position;
        pos.z += 100f;
        transform.position = pos;
        Vector3 rot = cameraTransform.rotation.eulerAngles;
        rot.x -= 90f;
        transform.rotation = Quaternion.Euler(rot);
    }
}
