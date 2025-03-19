using UnityEngine;

public class PandaController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform leftHandTarget;
    public Transform rightHandTarget;
    public Transform waist;
    public Transform Panda;
    public Transform PandaCamera;
    public Transform Head;
    private Animator animator;
    private Quaternion edit_camera;
    void Start()
    {
        animator = GetComponent<Animator>();
        edit_camera = Quaternion.Euler(0f, 0f, -90f);
    }
    // Update is called once per frame
    void Update()
    {
        //パンダの移動(x,z軸のみ)
        Vector3 newPosition = Panda.position;
        newPosition.x = PandaCamera.position.x;
        newPosition.z = PandaCamera.position.z;
        Panda.position = newPosition;
        
        //パンダの向き
        Vector3 newRotation = Panda.eulerAngles;
        newRotation.y = PandaCamera.eulerAngles.y;
        Panda.eulerAngles = newRotation;
    }
    void OnAnimatorIK() {

        //右手
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation * edit_camera);

        //左手
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation * edit_camera);
    }
}
