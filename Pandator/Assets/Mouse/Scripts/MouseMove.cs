using UnityEngine;

public class MouseMove : MonoBehaviour
{
    [SerializeField] private float moveSpeedMultiplier = 1.0f; // 移動速度倍率
    private bool isCollisionWall = false;
    [Header("登る速度")]
    [SerializeField] private float climbSpeed = 2.0f;
    private Rigidbody rb;
    [Header("ねずみのモデル")]
    [SerializeField] private GameObject mouseModel;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
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

        // カメラの向きに合わせてオブジェクトを回転
        mouseModel.transform.rotation = Quaternion.LookRotation(forwardDirection);

        // 移動処理
        if (isCollisionWall)
        {
            transform.position += transform.up * Time.deltaTime * climbSpeed;
        }
        else
        {
            transform.Translate(forwardDirection * totalSpeed * Time.deltaTime, Space.World);
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            isCollisionWall = true;
            // x,z, rotait固定
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            isCollisionWall = false;
        }
    }
}