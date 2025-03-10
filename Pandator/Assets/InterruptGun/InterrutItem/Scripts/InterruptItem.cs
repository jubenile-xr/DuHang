using UnityEngine;

public class InterrupteItem : MonoBehaviour
{
    private float time = 0.0f;
    private void Update()
    {
        // 3秒後に消える
        time += Time.deltaTime;
        if(time > 3.0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject Player = collision.gameObject;
        if(Player.tag == "Player")
        {
            Player.GetComponent<StateManager>()?.SetInterrupted(true);
            Player.GetComponent<TestStateManager>()?.SetInterrupted(true);
            Player.GetComponent<PhotonStateManager>()?.SetInterrupted(true);
            // ここは視覚的にわかりやすいように色を変える処理を追加しているだけ
            Player.GetComponent<TestPlayerColorManager>().ChangeColorRed();
            Destroy(gameObject);
        }
    }
}
