using UnityEngine;
public class PandaController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform leftHandTarget;
    public Transform rightHandTarget;
    private Animator animator;
    private Quaternion edit_Rightcamera;
    private Quaternion edit_Leftcamera;
    //視線
    public Transform pointView;
    void Start()
    {
        animator = GetComponent<Animator>();
        //両手の角度を調整
        edit_Rightcamera = Quaternion.Euler(0f, 0f, -90f);
        edit_Leftcamera = Quaternion.Euler(0f, 0f, 90f);
    }
    // Update is called once per frame
    void OnAnimatorIK() {
        
        //首の動き
        if (pointView != null)
        {
            animator.SetLookAtWeight(1);
            animator.SetLookAtPosition(pointView.position);
        }
        else
        {
            animator.SetLookAtWeight(0);
        }

        //右手
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation * edit_Rightcamera);

        //左手
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation * edit_Leftcamera);
    }
}
