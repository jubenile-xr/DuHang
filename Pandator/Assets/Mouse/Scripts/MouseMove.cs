using Photon.Pun;
using Unity.VisualScripting;
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

    [Header("速度の閾値")]
    [SerializeField] private float speedThreshold = 0.1f; // これより遅かったら動かない
    private InitializeManager InitializeManager;
    private float floarValue;
    
    private bool isKeybord = false;
    private float xAngle = 0f;
    private float yAngle = 0f;
    
    private Transform spawnPoint; // スポーン地点のTransform
    [SerializeField] private float spawnPlusPointY = 0.5f;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveSpeed = normalSpeed; // 初期値を通常速度に設定
        InitializeManager = GameObject.FindWithTag("InitializeManager").GetComponent<InitializeManager>();
        floarValue = InitializeManager.GetLocalAnchorPosition().y;
        
        // "playerSpawn"タグを持つゲームオブジェクトを検索して登録
        GameObject spawnObject = GameObject.FindWithTag("playerSpawn");
        if (spawnObject != null)
        {
            spawnPoint = spawnObject.transform;
        }
        else
        {
            // 見つからなかった場合にエラーメッセージを表示
            Debug.LogError("Error: 'playerSpawn' tag not found in the scene.");
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            isKeybord = true;
        }
        
        // IsMineで自分のキャラクターかどうかを判定
        if (GetComponent<PhotonView>().IsMine)
        {
            // キーボードの'R'キーか、Meta Questの左コントローラーの'X,Y'ボタンが押された瞬間をチェック
            if (Input.GetKeyDown(KeyCode.R) || (OVRInput.Get(OVRInput.Button.Three) && OVRInput.Get(OVRInput.Button.Four)))
            {
                // spawnPointが設定されていれば、その位置に移動
                if (spawnPoint != null)
                {
                    transform.position = spawnPoint.position + new Vector3(0, spawnPlusPointY, 0);
                    // テレポート後の慣性をなくすため、Rigidbodyの速度をリセット
                    if (rb != null)
                    {
                        rb.linearVelocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                    }
                    return; // このフレームでは他の移動処理を行わない
                }
            }

            // 右手と左手の速度を取得
            Vector3 velocityR = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
            Vector3 velocityL = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);

            // Y軸方向の速度の絶対値を取得
            float speedR = Mathf.Abs(velocityR.y);
            float speedL = Mathf.Abs(velocityL.y);

            // カメラの位置をねずみの位置に合わせる
            Vector3 cameraPosition = transform.position;
            cameraPosition.y += 0.2f; // y軸を+0.2
            
            if (mouseOVRCameraRig != null) // Nullチェックを追加してエラーを回避
            {
                mouseOVRCameraRig.transform.position = cameraPosition;
            }

            // カメラの向きをねずみの向きに合わせる
            Quaternion targetRotation = Quaternion.Euler(0, mouseCamera.transform.eulerAngles.y, 0);
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
            if (isCollisionWall)
            {
                // 壁に接触している間は登る
                transform.position += transform.up * Time.deltaTime * climbSpeed;
            }
            else
            {
                // キーボード操作の場合、移動方向と速度を上書き
                if (isKeybord)
                {
                    float moveX = Input.GetAxis("Horizontal");
                    float moveZ = Input.GetAxis("Vertical");
                    forwardDirection = Camera.main.transform.right * moveX + Camera.main.transform.forward * moveZ;
                    totalSpeed = 2f;
                    forwardDirection.Normalize();
                }
                // 通常の移動
                transform.Translate(forwardDirection * totalSpeed * Time.deltaTime, Space.World);
            }
        }

        //落ちた時用
        if (transform.position.y < floarValue)
        {
            transform.position = new Vector3(transform.position.x, floarValue + 0.1f, transform.position.z);
        }
        
        // マウスでのカメラ操作
        if (isKeybord)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            xAngle -= mouseY;
            xAngle = Mathf.Clamp(xAngle, -90f, 90f);
            yAngle += mouseX;
            Camera.main.transform.localRotation = Quaternion.Euler(xAngle, yAngle, 0);
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
        if (collision.gameObject.CompareTag("Wall"))
        {
            isCollisionWall = true;
            rb.useGravity = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            rb.useGravity = true;
            isCollisionWall = false;
        }
    }
}