using UnityEngine;

public class TutorialPandaController : MonoBehaviour
{
    public Transform leftHandTarget;
    public Transform rightHandTarget;
    public Transform pointView;
    public Transform cameraRig;
    private Animator animator;
    private Quaternion edit_Rightcamera;
    private Quaternion edit_Leftcamera;
    private Vector3 previousPosition = Vector3.zero; // 前フレームの位置
    private Vector3 previousVelocity = Vector3.zero; // 前フレームの速度
    private Vector3 currentVelocity = Vector3.zero;  // 現在の速度
    [SerializeField] private float minSpeed; // 移動速度
    private const float heightDiffPandaCamera = 1.8f; // パンダとカメラの高さの差分
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        edit_Rightcamera = Quaternion.Euler(0f, 0f, -90f);
        edit_Leftcamera = Quaternion.Euler(0f, 0f, 90f);

        // 初期位置と速度を設定
        if (cameraRig != null)
        {
            previousPosition = cameraRig.position;
            previousVelocity = Vector3.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // パンダの位置をカメラに合わせる
        Vector3 cameraPosition = cameraRig.position;
        cameraPosition.y -= heightDiffPandaCamera; // パンダの高さを調整
        transform.position = cameraPosition;
        // パンダの向きをカメラの向きに合わせる
        Quaternion targetRotation = Quaternion.Euler(0, cameraRig.transform.eulerAngles.y, 0);
        transform.rotation = targetRotation;
        // 加速度を取得してアニメーションに反映
        Vector3 acceleration = GetCameraRigAcceleration();
        if (acceleration.magnitude > minSpeed)
        {
            animator.SetFloat("speed", acceleration.magnitude);
        }
        else
        {
            animator.SetFloat("speed", 0);
        }
        //Debug.Log("加速度: " + acceleration.magnitude);
    }

    void OnAnimatorIK()
    {
        if (pointView != null)
        {
            animator.SetLookAtWeight(1);
            animator.SetLookAtPosition(pointView.position);
        }
        else
        {
            animator.SetLookAtWeight(0);
        }

        // 右手
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation * edit_Rightcamera);

        // 左手
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation * edit_Leftcamera);
    }

    public Vector3 GetCameraRigAcceleration()
    {
        // 現在の速度を計算
        currentVelocity = (cameraRig.position - previousPosition) / Mathf.Max(Time.deltaTime, 0.0001f); // 0除算を防ぐ

        // 加速度を計算
        Vector3 acceleration = (currentVelocity - previousVelocity) / Mathf.Max(Time.deltaTime, 0.0001f); // 0除算を防ぐ

        // 現在の速度を次のフレームのために保存
        previousVelocity = currentVelocity;

        // 現在の位置を次のフレームのために保存
        previousPosition = cameraRig.position;

        // 加速度を返す
        return acceleration;
    }
}
