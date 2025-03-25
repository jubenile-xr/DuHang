using UnityEngine;

public class PandaController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform leftHandTarget;
    public Transform rightHandTarget;
    private Animator animator;
    private Quaternion edit_Rightcamera;
    private Quaternion edit_Leftcamera;
    void Start()
    {
        animator = GetComponent<Animator>();
        edit_Rightcamera = Quaternion.Euler(0f, 0f, -90f);
        edit_Leftcamera = Quaternion.Euler(0f, 0f, 90f);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void OnAnimatorIK() {

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
