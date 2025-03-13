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
    public float flapThreshold = 10f;  // ����������ֱ۰ڶ���ֵ
    public float rotationSpeed = 5f;


    public float gravityForce = 9.8f; // ģ������
    public float gravityForceInAir = -15f; // ����������
    public float liftForce = 5f; // ������
    public float verticalVelocity = 0f;       // ��ǰ��ֱ�����ٶȣ�����Ϊ����


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
            // ֻ�е������ڷ���״̬ʱ��������
            Vector2 input = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
            Vector3 forward = CenterEyeAnchor.forward;
            Vector3 right = CenterEyeAnchor.right;

            // ȥ��ͷ��������б
            forward.y = 0f;
            right.y = 0f;

            Vector3 move = (forward * input.y + right * input.x) * moveSpeed;
            CharacterController.Move(move * Time.deltaTime);
        }
    }

    void HandleFlight()
    {
        // ��� ���ոհ��� A��
        bool isAButtonDown = OVRInput.GetDown(OVRInput.Button.One);

        // ��� ���Ƿ������ס A��
        bool isAButtonPressed = OVRInput.Get(OVRInput.Button.One); 

        //��ȡ�ֲ��ٶȣ����ڼ���ֱ۰ڶ����ȣ�
        Vector3 leftHandVel = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
        Vector3 rightHandVel = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);

        float handFlapStrength = (leftHandVel.y + rightHandVel.y) / 2;  // ����ƽ���ֱ۰ڶ��ٶ�
                                                                        //bool isFlap= handFlapStrength > flapThreshold;     

        // ����ƽ���ڶ�ǿ�� (������ y ������Ҳ������ magnitude)
        float leftStrength = leftHandVel.magnitude;
        float rightStrength = rightHandVel.magnitude;
        float avgStrength = (leftStrength + rightStrength) * 0.5f;

        // �ж��Ƿ񳬹���ֵ
        bool isFlap = (avgStrength > flapThreshold);

        // ����⵽�ڶ����ҵ�ǰ���Ƿ��� -> �������
        if (isFlap && !isFlying)
        {
            isFlying = true;

            // ����������˲���һ�����ϳ���
            verticalVelocity = liftForce;
        }

        else
        {
            isFlying = false;
        }

        // 3) �����Ƿ�ס���а�ť��ʩ������
        if (isFlying)
        {
            verticalVelocity = liftForce;
            // �����м�����������
            verticalVelocity -=  gravityForceInAir * Time.deltaTime;
           
            // ����ҳ�ͷ�Է����ƶ��������� y����������/����
            Vector3 direction = CenterEyeAnchor.forward;
            direction.Normalize();

            Vector3 movement = direction * flightSpeed;
            movement.y += verticalVelocity;

            // CharacterController �ƶ�
            CharacterController.Move(movement * Time.deltaTime);
        }
        else
        {
            // �������а�ť��������������
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
