using Photon.Pun;
using UnityEngine;

public class MouseMove : MonoBehaviour
{
    private float moveSpeed; // 移動速度倍率
    private const float slowSpeed = 0.5f; // スローモーション時の移動速度倍率
    private const float normalSpeed = 0.8f; // 通常時の移動速度倍率
    private bool isCollisionWall = false;
    [Header("登る速度")]
    private const float climbSpeed = 2.0f;
    private Rigidbody rb;

    [Header("OVRカメラ")]
    [SerializeField] private GameObject mouseCamera;
    [Header("カメラオブジェクト")]
    private GameObject mouseOVRCameraRig;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveSpeed = normalSpeed; // 初期値を通常速度に設定
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
            float totalSpeed = (speedR + speedL) * moveSpeed;

            // 頭（カメラ）の向きを取得して移動方向を決定
            Transform headTransform = Camera.main.transform;
            Vector3 forwardDirection = headTransform.forward;
            forwardDirection.y = 0; // 水平移動のみ考慮
            forwardDirection.Normalize();

            // 移動処理
            if (isCollisionWall)
            {
                transform.position += transform.up * Time.deltaTime * climbSpeed;
            }
            else
            {
                transform.Translate(forwardDirection * totalSpeed * Time.deltaTime, Space.World);
            }

            // カメラの位置をねずみの位置に合わせる
            mouseOVRCameraRig.transform.position = transform.position;

            // カメラの向きをねずみの向きに合わせる
            Quaternion targetRotation = Quaternion.Euler(0, mouseCamera.transform.eulerAngles.y, 0);
            transform.rotation = targetRotation;
        }
    }

    public void SetMouseOVRCameraRig()
    {
        mouseOVRCameraRig = GameObject.Find("MouseCameraRig(Clone)");
    }

    public void SetMoveSpeedNormal()
    {
        moveSpeed = normalSpeed;
    }
    public void SetMoveSpeedSlow()
    {
        moveSpeed = slowSpeed;
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