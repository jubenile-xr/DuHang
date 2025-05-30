using UnityEngine;

public class BirdAnimation : MonoBehaviour
{
    public GameObject OVRPlayer; // OVRPlayerController
    private Animator animator; // Bird's Animator
    private BirdMoveController birdMoveController; // get BirdMoveController

    void Start()
    {
        animator = GetComponent<Animator>(); 
        if (OVRPlayer != null)
        {
            birdMoveController = OVRPlayer.GetComponent<BirdMoveController>(); 
        }
    }

    void Update()
    {
        if (birdMoveController == null) return;

        
        bool isFlying = birdMoveController.isFlying;
        bool isWalking = birdMoveController.isWalking;


        
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isFlying", isFlying);
    }
}