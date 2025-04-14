using UnityEngine;

public class BirdRotationController : MonoBehaviour
{
    [SerializeField] private GameObject cameraRig;

    void Start()
    {
        
    }

    void Update()
    {
        // cameraRigのy軸の回転を取得
        float cameraRigYRotation = cameraRig.transform.eulerAngles.y;

        // 現在のオブジェクトの回転を取得
        Vector3 currentRotation = transform.eulerAngles;

        // y軸の回転をcameraRigのy軸回転に設定
        currentRotation.y = cameraRigYRotation;

        // 回転を反映
        transform.eulerAngles = currentRotation;
    }
}
