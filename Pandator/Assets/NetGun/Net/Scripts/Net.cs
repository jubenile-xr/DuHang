using UnityEngine;

public class Net : MonoBehaviour
{
    private float time = 0.0f;
    [SerializeField]private float collisionDeleteTime = 0.1f;
    private float collisionTime = 0.0f;
    private Animator animator;

    //一度当たったらonにする
    private bool isCollision = false;
    [SerializeField]private string targetTag;

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
        // if(isCollision)
        // {
        //     collisionTime += Time.deltaTime;
        //     if(collisionTime > collisionDeleteTime)
        //     {
        //         Destroy(gameObject);
        //     }
        // }

    }

    private void OnTriggerEnter(Collider collision)
    {
        if(isCollision) return;
        GameObject Player = collision.gameObject;

        if(Player.tag == "Player")
        {
            isCollision = true;
            //Netの中に入れる
            Player.GetComponent<SphereCollider>().isTrigger = true;
            //速度を0に
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            transform.position = Player.transform.position;
            transform.position += new Vector3(0, 1, 0);
            Player.GetComponent<StateManager>()?.SetAlive(false);
            // ここは視覚的にわかりやすいように色を変える処理を追加しているだけ
            Player.GetComponent<PlayerColorManager>()?.ChangeColorBlack();
            Debug.Log("hit");
        }
    }
}