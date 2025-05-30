using UnityEngine;

public class UIFaceCamera : MonoBehaviour
{
    public float distanceFromCamera = 3f;
    public Vector3 offset = new Vector3(0f, -0.5f, 0f);  // 向下微调位置

    void LateUpdate()
    {
        if (Camera.main == null) return;

        Transform cam = Camera.main.transform;

        // 设置位置：相机前方一定距离 + 可选偏移
        transform.position = cam.position + cam.forward * distanceFromCamera + cam.TransformDirection(offset);

        // 设置旋转：始终面向相机
        transform.rotation = Quaternion.LookRotation(transform.position - cam.position);
    }
}
