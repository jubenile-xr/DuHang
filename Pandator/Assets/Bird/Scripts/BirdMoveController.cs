using Photon.Realtime;
using System.Runtime.CompilerServices;
using Unity.Android.Gradle.Manifest;
using UnityEngine;


public class BirdMoveController : MonoBehaviour
{
    public Transform OvrPlayer;
    public Transform CenterEyeAnchor;
    private CharacterController CharacterController;

    public float flightSpeed = 10f; //Flight speed // �w���ٶ�
    public float moveSpeed = 1.0f; //Walking speed // �i���ٶ�

    //threshold // 铂�

    public float flapStartThreshold = 2.3f;  // �������ֵ����Ϊ��ʼ�ڶ� // > this value means the bird flapping // > ���΂��򳬤�����B����Ф���ʼ�᤿��Ҋ�ʤ��ޤ�
    public float flapStopThreshold = 0.8f;   // �������ֵ����Ϊֹͣ�ڶ� // < this value means the bird stop flapping // < ���΂����»ؤ���B����Ф�����ֹͣ������Ҋ�ʤ��ޤ�

    public int framesToStart = 3;   // ��������֡������ֵ����Ϊ����  // how many frames to start flapping // ��Ф����_ʼ��Ҋ�ʤ��B�A�ե�`����
    public int framesToStop = 30;    // ��������֡������ֵ����Ϊֹͣ // how many frames to stop flapping // ��Ф���ֹͣ��Ҋ�ʤ��B�A�ե�`����

    private int aboveCount = 0;     // ��¼����������ֵ��֡�� // how many frames above the threshold // �B�A��铂��򳬤����ե�`����
    private int belowCount = 0;     // ��¼����������ֵ��֡�� // how many frames below the threshold // �B�A��铂����»ؤä��ե�`����

    //force // ��
    public float gravityForce = 9.8f;
    public float gravityForceInAir = 0f; // ���������� // gravity in air // �w���Ф�����
    public float liftForce = 4f; // ������ // when the first time fly will give the bird a lift force // ������w�Х�`�ɤ���ä��H���B���뤨���ϕN��
    public float verticalVelocity = 0f;       // ��ǰ��ֱ�����ٶȣ�����Ϊ���� // current vertical velocity // �F�ڤδ�ֱ�����ٶȣ��Ϸ�������

    bool isFlying = false; //�ж��Ƿ�������ģʽ // �w�Х�`�ɤ���äƤ��뤫�ɤ�����ʾ��


    void Start()
    {
        CharacterController = GetComponent<CharacterController>();
    }


    void Update()

    {
        TellBirdMode();//�ж��Ƿ�������ģʽ//tell the bird to fly or not

        if (!isFlying)
        { 
            HandleWalking(); 
        }
        else
        {
            HandleFlight();
        }

    }
    void TellBirdMode() 
    {
        //��ȡ�ֲ��ٶȣ����ڼ���ֱ۰ڶ����ȣ�//get the hand velocity to detect the flapping // �֤��ٶȤ�ȡ�ä��루���������ʳ����뤿�ᣩ
        Vector3 leftHandVel = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
        Vector3 rightHandVel = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);

        // ����ƽ���ڶ�ǿ�� // calculate the average flapping strength // ��Ф�����ƽ�����Ȥ�Ӌ�㤹��
        float leftStrength = leftHandVel.magnitude;
        float rightStrength = rightHandVel.magnitude;
        float avgStrength = (leftStrength + rightStrength) * 0.5f;

        if (avgStrength > flapStartThreshold)
        {
            aboveCount++;
            belowCount = 0; //���õ�����ֵ��֡�� //reset the below count // 铂�δ���Υե�`������ꥻ�åȤ���

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
        // ��ҡ������ // get the right thumbstick input // �ҥ��ƥ��å���������ȡ�ä���
        Vector2 input = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        // ˮƽ���� //  get the forward and right direction // ˮƽ��ǰ��������ҷ����ȡ�ä���
        Vector3 forward = CenterEyeAnchor.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = CenterEyeAnchor.right;
        right.y = 0f;
        right.Normalize();

        Vector3 move = (forward * input.y + right * input.x) * moveSpeed;

        // һ������Walking״̬���ͻ��ܵ��������� // once enter the Walking state, the bird will be affected by the gravity // �����`����״�B������ͨ�����������m�ä����
        verticalVelocity -= gravityForce * Time.deltaTime;
        move.y = verticalVelocity;

        CharacterController.Move(move * Time.deltaTime);

        // ����������棬�����ٶ� // if the bird hit the ground, reset the velocity // ����˴��줿���ٶȤ�ꥻ�åȤ���
        if (CharacterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = 0f;
        }

    }

    void HandleFlight()
    {
        //// �ոհ��� A 
        //bool isAButtonDown = OVRInput.GetDown(OVRInput.Button.One);

        ////�Ƿ������ס A
        //bool isAButtonPressed = OVRInput.Get(OVRInput.Button.One); 
        //�ⲿ��Ϊ������ӵķ��п��ƻ��߼��ܴ����ṩ�ӿڣ���ʱ����Ҫ
        //*this part is for the future flight control or skill code, not needed for now

        //�����Ƿ�ס���а�ť��ʩ������ //apply gravity based on the flight button // �w�Хܥ����Ѻ��״�B�ˏꤸ���������m�ä���
        if (isFlying)
        {
            // �����в������� // no gravity in the air // �w���Ф�������Ӱ푤��ܤ��ʤ�
            verticalVelocity -= gravityForceInAir * Time.deltaTime;

            // ����ҳ�ͷ�Է����ƶ��������� y����������/���� // �ץ쥤��`��HMD���򤤤Ƥ��뷽����ƄӤ�����Y�S����ˤ����ޤ��ʤ����¤��ƄӤǤ��ޤ�
            // Move the player towards the direction of the head display, no limit on y, can move up and down // �ץ쥤��`��HMD���򤭤��ƄӤ�����Y������Ƅ����ޤ��ʤ��������¤��ƄӤǤ��ޤ�
            Vector3 direction = CenterEyeAnchor.forward;
            direction.Normalize();

            Vector3 movement = direction * flightSpeed;
            movement.y += verticalVelocity;

            CharacterController.Move(movement * Time.deltaTime);
        }
        else
        {
            // ���ɾͻָ��������� // if not flying, apply normal gravity // �w�Ф��Ƥ��ʤ����Ϥ�ͨ�����������m�ä���
            verticalVelocity -= gravityForce * Time.deltaTime;
        }

    }
}
