using Photon.Realtime;
using System.Runtime.CompilerServices;
// using Unity.Android.Gradle.Manifest;
using UnityEngine;


public class BirdMoveController : MonoBehaviour
{
    private bool isKeybord = false;
    private bool isInitialized = false;
    public Transform OvrPlayer;
    public Transform CenterEyeAnchor;
    private CharacterController CharacterController;

    public float flightSpeed = 1.0f; //Flight speed // 飛行速度
    private const float moveSpeedSlow = 0.5f; // Slow flight speed // スローフライト速度
    private const float moveSpeedNormal = 1.0f; // Normal flight speed // 通常の飛行速度
    public float moveSpeed = 1.0f; //Walking speed // 歩行速度

    //threshold // 閾値

    public float flapStartThreshold = 2.3f;  // 超过这个值才认为开始摆动 // > this value means the bird flapping // > この値を超えると鳥が羽ばたき始めたと見なします
    public float flapStopThreshold = 0.8f;   // 低于这个值才认为停止摆动 // < this value means the bird stop flapping // < この値を下回ると鳥が羽ばたきを停止したと見なします

    public int framesToStart = 3;   // 连续多少帧超过阈值才认为启动  // how many frames to start flapping // 羽ばたき開始と見なす連続フレーム数
    public int framesToStop = 30;    // 连续多少帧低于阈值才认为停止 // how many frames to stop flapping // 羽ばたき停止と見なす連続フレーム数

    private int aboveCount = 0;     // 记录连续超过阈值的帧数 // how many frames above the threshold // 連続で閾値を超えたフレーム数
    private int belowCount = 0;     // 记录连续低于阈值的帧数 // how many frames below the threshold // 連続で閾値を下回ったフレーム数

    //force // 力
    public float gravityForce = 9.8f;
    public float gravityForceInAir = 0f; // 飞行中重力 // gravity in air // 飛行中の重力
    private float flyForce = 0.1f; // 飞行中上升力 // lift force in air // 飛行中の上昇力
    public float liftForce = 2f; // 上升力 // when the first time fly will give the bird a lift force // 初めて飛行モードに入った際に鳥に与える上昇力
    public float verticalVelocity = 0f;       // 当前竖直方向速度（向上为正） // current vertical velocity // 現在の垂直方向速度（上方向が正）

    public bool isFlying = false; //判断是否进入飞行模式 // 飛行モードに入っているかどうかを示す
    public bool isWalking = false; //判断是否进入行走模式 // 歩行モードに入っているかどうかを示す

    // サムスティックを無効にするフラグ
    [SerializeField] private bool disableThumbstickMovement = true;
    
    private float xAngle = 0f;
    private float yAngle = 0f;

    void Start()
    {
        CharacterController = GetComponent<CharacterController>();
    }


    void Update()

    {
        if (!isInitialized) return;

        if (Input.GetKeyDown(KeyCode.K))
        {
            SetIsKeybord(true);
        }
        
        TellBirdMode();//判断是否进入飞行模式//tell the bird to fly or not

        if (!isFlying)
        {
            HandleWalking();
        }
        else
        {
            HandleFlight();
        }

        // 左スティックの入力を0にする
        Vector2 leftStick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        leftStick = Vector2.zero; // 強引に0にする

        if (isKeybord)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            xAngle -= mouseY;
            xAngle = Mathf.Clamp(xAngle, -90f, 90f);
            yAngle += mouseX;
            // yAngle = Mathf.Clamp(yAngle, -90f, 90f);
            CenterEyeAnchor.localRotation = Quaternion.Euler(xAngle, yAngle, 0);
        }
    }
    void TellBirdMode()
    {
        //获取手部速度（用于检测手臂摆动幅度）//get the hand velocity to detect the flapping // 手の速度を取得する（腕の振り幅を検出するため）
        Vector3 leftHandVel = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
        Vector3 rightHandVel = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);

        // 计算平均摆动强度 // calculate the average flapping strength // 羽ばたきの平均強度を計算する
        float leftStrength = leftHandVel.magnitude;
        float rightStrength = rightHandVel.magnitude;
        float avgStrength = (leftStrength + rightStrength) * 0.5f;
        
        if (avgStrength > flapStartThreshold || Input.GetKey(KeyCode.Space))
        {
            aboveCount++;
            belowCount = 0; //重置低于阈值的帧数 //reset the below count // 閾値未満のフレーム数をリセットする
            
            if (!isFlying && aboveCount >= framesToStart)
            {
                verticalVelocity = liftForce;
                isFlying = true;
            }
        }else {
            belowCount++;
            aboveCount = 0;

            if (isFlying && belowCount >= framesToStop)
            {
                isFlying = false;
            }
        }

        //以下内容用于脱离VR环境使用

        //bool isAButtonPressed = OVRInput.Get(OVRInput.Button.One);
        //if (isAButtonPressed)
        //{
        //    isFlying = true;
        //    verticalVelocity = liftForce;
        //}
        //else
        //{
        //    isFlying = false;
        //}

    }
    void HandleWalking()
    {
        // サムスティックが無効化されている場合は入力をゼロにする
        Vector2 input = Vector2.zero;
        
        if (isKeybord)
        {
            input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
        else if(!disableThumbstickMovement)    // サムスティックが有効な場合のみ入力を取得
        {
            // 右摇杆输入 // get the right thumbstick input // 右スティックの入力を取得する
            input = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        }

        if(input.magnitude > 0)
        {
            isWalking = true;
        }
        else
        {
            
            isWalking = false;
        }

        // 水平朝向 //  get the forward and right direction // 水平な前方方向と右方向を取得する
        Vector3 forward = CenterEyeAnchor.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = CenterEyeAnchor.right;
        right.y = 0f;
        right.Normalize();

        Vector3 move = (forward * input.y + right * input.x) * moveSpeed;

        // 一旦进入Walking状态，就会受到正常重力 // once enter the Walking state, the bird will be affected by the gravity // ウォーキング状態に入ると通常の重力が適用される
        move.y = -gravityForce * Time.deltaTime; //TODO: 落下速度調整

        CharacterController.Move( move * Time.deltaTime);

        // 如果碰到地面，重置速度 // if the bird hit the ground, reset the velocity // 地面に触れたら速度をリセットする
        if (CharacterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = 0f;
        }

    }

    void HandleFlight()
    {
        //// 刚刚按下 A
        //bool isAButtonDown = OVRInput.GetDown(OVRInput.Button.One);

        ////是否持续按住 A
        //bool isAButtonPressed = OVRInput.Get(OVRInput.Button.One);
        //这部分为后续添加的飞行控制或者技能代码提供接口，暂时不需要
        //*this part is for the future flight control or skill code, not needed for now

        //根据是否按住飞行按钮来施加重力 //apply gravity based on the flight button // 飛行ボタンの押下状態に応じて重力を適用する
        if (!isFlying) return;

        // 飞行中不受重力 // no gravity in the air // 飛行中は重力の影響を受けない
        verticalVelocity -= gravityForceInAir * Time.deltaTime;

        // 让玩家朝头显方向移动，不限制 y，可以向上/向下 // プレイヤーをHMDの向いている方向に移動させ、Y軸方向には制限がなく上下に移動できます
        // Move the player towards the direction of the head display, no limit on y, can move up and down // プレイヤーをHMDの向きに移動させ、Y方向の移動制限がないため上下に移動できます
        Vector3 direction = CenterEyeAnchor.forward;
        direction.Normalize();

        Vector3 movement = direction * flightSpeed;
        movement.y += verticalVelocity;

        CharacterController.Move(flyForce *movement * Time.deltaTime);
        
    }

    public void SetCenterEyeAnchor(Transform centerEyeAnchor)
    {
        CenterEyeAnchor = centerEyeAnchor;
        isInitialized = true;
    }

    public void SetMoveSpeedNormal()
    {
        flightSpeed = moveSpeedNormal;
        moveSpeed = moveSpeedNormal;
    }
    public void SetMoveSpeedSlow()
    {
        flightSpeed = moveSpeedSlow;
        moveSpeed = moveSpeedSlow;
    }

    // サムスティック移動の有効/無効を切り替えるメソッド
    public void SetThumbstickMovementEnabled(bool enabled)
    {
        disableThumbstickMovement = !enabled;
    }
    
    private void SetIsKeybord(bool isKeybord)
    {
        this.isKeybord = isKeybord;
    }
}
