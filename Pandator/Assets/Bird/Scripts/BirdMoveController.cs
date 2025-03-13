using Photon.Realtime;
using System.Runtime.CompilerServices;
using Unity.Android.Gradle.Manifest;
using UnityEngine;


public class BirdMoveController : MonoBehaviour
{
    public Transform OvrPlayer;
    public Transform CenterEyeAnchor;

    private CharacterController CharacterController;

    public float flightSpeed = 15f;
    public float moveSpeed = 1.0f; //Walking speed
    public float flapThreshold = 10f;  // 触发飞翔的手臂摆动阈值
    public float rotationSpeed = 5f;


    public float gravityForce = 9.8f; // 模拟重力
    public float gravityForceInAir = -15f; // 飞行中重力
    public float liftForce = 5f; // 上升力
    public float verticalVelocity = 0f;       // 当前竖直方向速度（向上为正）


    bool isFlying = false;


    void Start()
    {
        CharacterController = GetComponent<CharacterController>();
    }


    void Update()
    {
        HandleWalking();

        HandleFlight();


    }

    void HandleWalking()
    {
        if (!isFlying)
        {
            // 只有当不处于飞行状态时才做行走
            Vector2 input = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
            Vector3 forward = CenterEyeAnchor.forward;
            Vector3 right = CenterEyeAnchor.right;

            // 去掉头显上下倾斜
            forward.y = 0f;
            right.y = 0f;

            Vector3 move = (forward * input.y + right * input.x) * moveSpeed;
            CharacterController.Move(move * Time.deltaTime);
        }
    }

    void HandleFlight()
    {
        // 检测 “刚刚按下 A”
        bool isAButtonDown = OVRInput.GetDown(OVRInput.Button.One);

        // 检测 “是否持续按住 A”
        bool isAButtonPressed = OVRInput.Get(OVRInput.Button.One); 

        //获取手部速度（用于检测手臂摆动幅度）
        Vector3 leftHandVel = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
        Vector3 rightHandVel = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);

        float handFlapStrength = (leftHandVel.y + rightHandVel.y) / 2;  // 计算平均手臂摆动速度
                                                                        //bool isFlap= handFlapStrength > flapThreshold;     

        // 计算平均摆动强度 (可以是 y 分量，也可以用 magnitude)
        float leftStrength = leftHandVel.magnitude;
        float rightStrength = rightHandVel.magnitude;
        float avgStrength = (leftStrength + rightStrength) * 0.5f;

        // 判断是否超过阈值
        bool isFlap = (avgStrength > flapThreshold);

        // 当检测到摆动并且当前不是飞行 -> 进入飞行
        if (isFlap && !isFlying)
        {
            isFlying = true;

            // 如果想在起飞瞬间给一次向上冲量
            verticalVelocity = liftForce;
        }

        else
        {
            isFlying = false;
        }

        // 3) 根据是否按住飞行按钮来施加重力
        if (isFlying)
        {
            verticalVelocity = liftForce;
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
