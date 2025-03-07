using UnityEngine;

public class InterrupteItem : MonoBehaviour
{

    void OnCollisionEnter(Collision collision)
    {
        GameObject Player = collision.gameObject;
        if(Player.tag == "Player")
        {
            Player.GetComponent<StateManager>().SetInterrupted(true);
            // ここは視覚的にわかりやすいように色を変える処理を追加しているだけ
            Player.GetComponent<TestPlayerColorManager>().ChangeColorRed();
            Destroy(gameObject);
        }
    }
}
