using UnityEngine;

public class Bird : MonoBehaviour
{
    private string name;
    [SerializeField]
    private float moveSpeed;
    private GameObject model;

    private Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        animator.SetInteger("moving",0);
        if (Input.GetKey(KeyCode.W))
        {
            animator.SetInteger("moving",1);
        }else if (Input.GetKey(KeyCode.S))
        {
            animator.SetInteger("moving",2);
        }else if (Input.GetKey(KeyCode.A))
        {
            animator.SetInteger("moving",3);
        }
    }
}
