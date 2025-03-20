using Photon.Realtime;
using System.Runtime.CompilerServices;
using Unity.Android.Gradle.Manifest;
using UnityEngine;


public class BirdMoveController : MonoBehaviour
{
    public Transform OvrPlayer;
    public Transform CenterEyeAnchor;

    private CharacterController CharacterController;

    public float flightSpeed = 10f;
    public float moveSpeed = 1.0f; //Walking speed

    //threshold
    public float flapThreshold = 5f;  // 触发飞翔的手臂摆动阈值
    public float flapStartThreshold = 2.3f;  // 超过这个值才认为开始摆动
    public float flapStopThreshold = 0.8f;   // 低于这个值才认为停止摆动
                                             
    public int framesToStart = 3;   // 连续多少帧超过阈值才认为启动
    public int framesToStop = 5;    // 连续多少帧低于阈值才认为停止

    private int aboveCount = 0;     // 记录连续超过阈值的帧数
    private int belowCount = 0;     // 记录连续低于阈值的帧数
  
    //force
    public float gravityForce = 9.8f; // 模拟重力
    public float gravityForceInAir = 0f; // 飞行中重力
    public float liftForce = 4f; // 上升力
    public float verticalVelocity = 0f;       // 当前竖直方向速度（向上为正）


    bool isFlying = false;

    //animation
    private Animator animator;

    void Start()
    {
        CharacterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();  // 获取 Animator 组件
    }


    void Update()

    {
        TellBirdMode();
        if (!isFlying)
        { 
            HandleWalking(); 
        }
        else
        {
            HandleFlight();
        }
        animator.SetBool("isFlying", isFlying);  // 控制飞行动画
        animator.SetFloat("Speed", CharacterController.velocity.magnitude); // 控制速度动画

    }
    void TellBirdMode() 
    {
        //获取手部速度（用于检测手臂摆动幅度）
        Vector3 leftHandVel = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
        Vector3 rightHandVel = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);

        
                                                                        //bool isFlap= handFlapStrength > flapThreshold;     

        // 计算平均摆动强度 (可以是 y 分量，也可以用 magnitude)
        float leftStrength = leftHandVel.magnitude;
        float rightStrength = rightHandVel.magnitude;
        float avgStrength = (leftStrength + rightStrength) * 0.5f;

        // 判断是否超过阈值
        //bool isFlap = (avgStrength > flapThreshold);
        ////bool isFlap = OVRInput.Get(OVRInput.Button.One);
        //// 当检测到摆动并且当前不是飞行 -> 进入飞行
        //if (isFlap)
        //{
        //    // 刚从不飞行 -> 飞行，可以给一次 liftForce
        //    //if (!isFlying)
        //    //{
        //    //    verticalVelocity = liftForce;
        //    //}
        //    isFlying = true;
        //}
        //else
        //{
        //    isFlying = false;
        //}
        if (avgStrength > flapStartThreshold)
        { 
            aboveCount++;
            belowCount = 0;

            if (!isFlying && aboveCount >= framesToStart)
            {
                verticalVelocity = liftForce;
                isFlying = true;
            }
        }
        else
        {
            belowCount++;
            aboveCount = 0;

            if (isFlying && belowCount >= framesToStop)
            {
                isFlying = false;
            }
        }

    }
    void HandleWalking()
    {
        // 右摇杆输入
        Vector2 input = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        // 水平朝向
        Vector3 forward = CenterEyeAnchor.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = CenterEyeAnchor.right;
        right.y = 0f;
        right.Normalize();

        Vector3 move = (forward * input.y + right * input.x) * moveSpeed;

        // 正常重力
        verticalVelocity -= gravityForce * Time.deltaTime;
        move.y = verticalVelocity;

        CharacterController.Move(move * Time.deltaTime);

        // 如果碰到地面，重置速度
        if (CharacterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = 0f;
        }

    }

    void HandleFlight()
    {
        //// 检测 “刚刚按下 A”
        //bool isAButtonDown = OVRInput.GetDown(OVRInput.Button.One);

        //// 检测 “是否持续按住 A”
        //bool isAButtonPressed = OVRInput.Get(OVRInput.Button.One); 

        //if(!isFlap)
        //{
        //    isFlying = false;
        //}

        // 3) 根据是否按住飞行按钮来施加重力
        if (isFlying)
        {
            
            // 飞行中减弱或不受重力
            verticalVelocity -=  gravityForceInAir * Time.deltaTime;
           
            // 让玩家朝头显方向移动，不限制 y，可以向上/向下
            Vector3 direction = CenterEyeAnchor.forward;
            direction.Normalize();

            Vector3 movement = direction * flightSpeed;
            movement.y += verticalVelocity;

            // CharacterController 移动
            CharacterController.Move(movement * Time.deltaTime);
        }
        else
        {
            // 不按飞行按钮则正常重力下落
            verticalVelocity -= gravityForce * Time.deltaTime;
        }





    }
    void Fly()
    {
        if (isFlying)
        {

        }
    }

}
