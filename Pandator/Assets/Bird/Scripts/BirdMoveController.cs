using UnityEngine;


public class BirdMoveController : MonoBehaviour
{
    public Transform CenterEyeAnchor;

    private CharacterController CharacterController;

    public float flightSpeed = 10f;
    public float moveSpeed = 1.0f; //Walking speed

    //threshold
    public float flapThreshold = 5f;  // ����������ֱ۰ڶ���ֵ
    public float flapStartThreshold = 2.3f;  // �������ֵ����Ϊ��ʼ�ڶ�
    public float flapStopThreshold = 0.8f;   // �������ֵ����Ϊֹͣ�ڶ�
                                             
    public int framesToStart = 3;   // ��������֡������ֵ����Ϊ����
    public int framesToStop = 5;    // ��������֡������ֵ����Ϊֹͣ

    private int aboveCount = 0;     // ��¼����������ֵ��֡��
    private int belowCount = 0;     // ��¼����������ֵ��֡��
  
    //force
    public float gravityForce = 9.8f; // ģ������
    public float gravityForceInAir = 0f; // ����������
    public float liftForce = 4f; // ������
    public float verticalVelocity = 0f;       // ��ǰ��ֱ�����ٶȣ�����Ϊ����


    bool isFlying = false;



    void Start()
    {
        CharacterController = GetComponent<CharacterController>();
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

    }
    void TellBirdMode() 
    {
// 获取手部速度（用于检测手臂摆动幅度）  // 手の速度を取得（腕の振れ幅の検出に利用）
Vector3 leftHandVel = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
        Vector3 rightHandVel = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);

        
                                                                        //bool isFlap= handFlapStrength > flapThreshold;     

        // ����ƽ���ڶ�ǿ�� (������ y ������Ҳ������ magnitude)
        float leftStrength = leftHandVel.magnitude;
        float rightStrength = rightHandVel.magnitude;
        float avgStrength = (leftStrength + rightStrength) * 0.5f;

        // �ж��Ƿ񳬹���ֵ
        //bool isFlap = (avgStrength > flapThreshold);
        ////bool isFlap = OVRInput.Get(OVRInput.Button.One);
        //// ����⵽�ڶ����ҵ�ǰ���Ƿ��� -> �������
        //if (isFlap)
        //{
        //    // �մӲ����� -> ���У����Ը�һ�� liftForce
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
        // ��ҡ������
        Vector2 input = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        // ˮƽ����
        Vector3 forward = CenterEyeAnchor.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = CenterEyeAnchor.right;
        right.y = 0f;
        right.Normalize();

        Vector3 move = (forward * input.y + right * input.x) * moveSpeed;

        // ��������
        verticalVelocity -= gravityForce * Time.deltaTime;
        move.y = verticalVelocity;

        CharacterController.Move(move * Time.deltaTime);

        // ����������棬�����ٶ�
        if (CharacterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = 0f;
        }

    }

    void HandleFlight()
    {
        //// ��� ���ոհ��� A��
        //bool isAButtonDown = OVRInput.GetDown(OVRInput.Button.One);

        //// ��� ���Ƿ������ס A��
        //bool isAButtonPressed = OVRInput.Get(OVRInput.Button.One); 

        //if(!isFlap)
        //{
        //    isFlying = false;
        //}

        // 3) �����Ƿ�ס���а�ť��ʩ������
        if (isFlying)
        {
            
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
