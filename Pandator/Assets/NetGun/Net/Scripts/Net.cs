using UnityEngine;

public class Net : MonoBehaviour
{
    private float time = 0.0f;
    [SerializeField]private float collisionDeleteTime = 0.1f;
    private float collisionTime = 0.0f;

    private bool isCollision = false;
    private void Update()
    {
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
