using Photon.Pun;
using UnityEngine;

public class Net : MonoBehaviour
{
    private float time = 0.0f;
    [SerializeField]private float collisionDeleteTime = 0.1f;
    private float collisionTime = 0.0f;
    private Animator animator;

    private bool isCollision = false;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        
        animator.SetTrigger("Capture");
        
    }
    private void Update()
    {
        animator.SetTrigger("Idle");
        
        // 3秒後に消える
        time += Time.deltaTime;
        if(time > 3.0f)
        {
            Destroy(gameObject);
        }
        if(isCollision)
        {
            collisionTime += Time.deltaTime;
            if(collisionTime > collisionDeleteTime)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        GameObject Player = collision.gameObject;
        if(Player.tag == "Player")
        {
            isCollision = true;
            Player.GetComponent<StateManager>()?.SetAlive(false);
            Player.GetComponent<PhotonStateManager>()?.SetAlive(false);
            // ここは視覚的にわかりやすいように色を変える処理を追加しているだけ
            Player.GetComponent<TestPlayerColorManager>()?.ChangeColorBlack();
        }
    }
}
