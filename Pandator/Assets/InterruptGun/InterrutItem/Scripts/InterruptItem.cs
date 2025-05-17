using System.Collections;
using Photon.Pun;
using UnityEngine;

public class InterrupteItem : MonoBehaviour
{
    private float time = 0.0f;
    [SerializeField] private float collisionDeleteTime = 0.1f;
    private float collisionTime = 0.0f;
    private bool isCollision = false;
    [SerializeField] private GameObject hitSE;

    private void Update()
    {
        // 3秒後に自身を消す
        time += Time.deltaTime;
        if (time > 3.0f)
        {
            Destroy(gameObject);
        }

        if (isCollision)
        {
            collisionTime += Time.deltaTime;
            if (collisionTime > collisionDeleteTime)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        GameObject player = collision.gameObject;
        if (player.CompareTag("Player"))
        {
            if (player.GetComponent<PhotonView>().IsMine != GetComponent<PhotonView>().IsMine)
            {
                isCollision = true;
                // プレイヤーの状態を Interrupted に設定する
                player.GetComponent<StateManager>()?.SetInterrupted(true);
                Debug.Log("hit!");
                hitSE?.GetComponent<SoundPlayer>().Play();
            }
        }
        // このオブジェクトを消す
        Destroy(gameObject);
    }
}