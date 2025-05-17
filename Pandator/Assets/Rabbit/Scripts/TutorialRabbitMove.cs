using UnityEngine;

public class TutorialRabbitMove : MonoBehaviour
{
    private float moveSpeed; // 移動速度倍率
    private const float slowSpeed = 0.5f; // スローモーション時の移動速度倍率
    private const float normalSpeed = 0.8f; // 通常時の移動速度倍率

    private Rigidbody rb;

    [Header("OVRカメラ")]
    [SerializeField] private GameObject rabbitCamera;
    [Header("カメラオブジェクト")]
    private GameObject rabbitOVRCameraRig;
    [Header("速度の閾値")]
    [SerializeField] private float speedThreshold = 0.1f; // 移動のための速度の閾値
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    private bool isKeybord = false;
    private float xAngle = 0f;
    private float yAngle = 0f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveSpeed = normalSpeed; // 初期値を通常速度に設定
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            isKeybord = true;
        }
        
        // 右手と左手の速度を取得
        Vector3 velocityR = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
        Vector3 velocityL = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);

        // 右手のAボタンが押されたかチェック
        bool isAButtonPressed = OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch);

        // XZ平面上の速度の合計を計算
        float speedR = Mathf.Abs(velocityR.y);
        float speedL = Mathf.Abs(velocityL.y);

        // カメラの位置をうさぎの位置に合わせる
        Vector3 cameraPosition = transform.position;
        cameraPosition.y += 0.2f; // y軸を+0.2
        rabbitOVRCameraRig.transform.position = cameraPosition;

        // カメラの向きをうさぎの向きに合わせる
        Quaternion targetRotation = Quaternion.Euler(0, rabbitCamera.transform.eulerAngles.y, 0);
        transform.rotation = targetRotation;

        // 左スティックの入力を0にする
        Vector2 leftStick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        leftStick = Vector2.zero; // 強引に0にする

        // 速度が閾値以下の場合は移動しない
        if (speedR < speedThreshold && speedL < speedThreshold && !isKeybord)
        {
            return;
        }
        
        float totalSpeed = (speedR + speedL) * moveSpeed;

        // 頭（カメラ）の向きを取得して移動方向を決定
        Transform headTransform = Camera.main.transform;
        Vector3 forwardDirection = headTransform.forward;
        forwardDirection.y = 0; // 水平移動のみ考慮
        forwardDirection.Normalize();

        // 移動処理
        if (!isAButtonPressed && !isKeybord)
        {
            transform.Translate(forwardDirection * totalSpeed * Time.deltaTime, Space.World);
        }else if (isKeybord)
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");
            

            forwardDirection =  Camera.main.transform.right* moveX + Camera.main.transform.forward * moveZ;
            totalSpeed = 2f;
            transform.Translate(forwardDirection.normalized * totalSpeed * Time.deltaTime, Space.World);
        }

        //マウスでのカメラ操作
        if (isKeybord)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            xAngle -= mouseY;
            xAngle = Mathf.Clamp(xAngle, -90f, 90f);
            yAngle += mouseX;
            // yAngle = Mathf.Clamp(yAngle, -90f, 90f);
            Camera.main.transform.localRotation = Quaternion.Euler(xAngle, yAngle, 0);
        }


    }
    public void SetRabbitOVRCameraRig()
    {
        rabbitOVRCameraRig = GameObject.Find("TutorialCameraRig(Clone)");
    }
    // public void SetMoveSpeedNormal()
    // {
    //     moveSpeed = normalSpeed;
    // }
    // public void SetMoveSpeedSlow()
    // {
    //     moveSpeed = slowSpeed;
    // }
}
