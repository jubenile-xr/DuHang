using Photon.Pun;
using UnityEngine;

public class RabbitMove : MonoBehaviour
{
    [SerializeField] private float moveSpeedMultiplier = 1.0f; // 移動速度倍率

    private Rigidbody rb;

    [Header("OVRカメラ")]
    [SerializeField] private GameObject rabbitCamera;
    [Header("カメラオブジェクト")]
    private GameObject rabbitOVRCameraRig;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //IsMineで自分のキャラクターかどうかを判定
        if (GetComponent<PhotonView>().IsMine)
        {
            // 右手と左手の速度を取得
            Vector3 velocityR = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
            Vector3 velocityL = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);

            // XZ平面上の速度の合計を計算
            float speedR = Mathf.Abs(velocityR.y);
            float speedL = Mathf.Abs(velocityL.y);
            float totalSpeed = (speedR + speedL) * moveSpeedMultiplier;

            // 頭（カメラ）の向きを取得して移動方向を決定
            Transform headTransform = Camera.main.transform;
            Vector3 forwardDirection = headTransform.forward;
            forwardDirection.y = 0; // 水平移動のみ考慮
            forwardDirection.Normalize();

            // 移動処理
            transform.Translate(forwardDirection * totalSpeed * Time.deltaTime, Space.World);

            // カメラの位置をうさぎの位置に合わせる
            rabbitOVRCameraRig.transform.position = transform.position;

            // カメラの向きをうさぎの向きに合わせる
            Quaternion targetRotation = Quaternion.Euler(0, rabbitCamera.transform.eulerAngles.y, 0);
            transform.rotation = targetRotation;
        }

    }

    public void SetRabbitOVRCameraRig()
    {
        rabbitOVRCameraRig = GameObject.Find("RabbitCameraRig(Clone)");
    }
}