using UnityEngine;

public class Net : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        GameObject Player = collision.gameObject;
        if(Player.tag == "Player" && Player.GetComponent<StateManager>().GetAlive())
        {
            Player.GetComponent<StateManager>()?.SetAlive(false);
            Player.GetComponent<PhotonStateManager>()?.SetAlive(false);
            // ここは視覚的にわかりやすいように色を変える処理を追加しているだけ
            Player.GetComponent<TestPlayerColorManager>()?.ChangeColorBlack();
        }
    }
}
