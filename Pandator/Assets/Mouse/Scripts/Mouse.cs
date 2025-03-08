using UnityEngine;

public class Mouse : MonoBehaviour
{
    private string name;
    [SerializeField]
    private float moveSpeed;
    private GameObject model;
    
    private Animator animator;
    private void Start()
    {
        animator = this.GetComponent<Animator>();
    }
    private void Update()
    {
        animator.SetInteger("motion", 0);
        if (Input.GetKey(KeyCode.W))
        {
            animator.SetInteger("motion", 1);
        }
    }
}
