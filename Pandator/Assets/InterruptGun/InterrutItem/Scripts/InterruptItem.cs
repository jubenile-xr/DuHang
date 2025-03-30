using System.Collections;
using UnityEngine;

public class InterrupteItem : MonoBehaviour
{
    private float time = 0.0f;
    [SerializeField] private float collisionDeleteTime = 0.1f;
    private float collisionTime = 0.0f;
    private bool isCollision = false;

    private void Update()
    {
        // 3秒後に自身を消す
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
        GameObject player = collision.gameObject;
        if (player.CompareTag("Player"))
        {
            isCollision = true;

            // プレイヤーの状態を Interrupted に設定する
            player.GetComponent<StateManager>()?.SetInterrupted(true);

            // 子オブジェクト内のすべての Renderer を取得して処理する
            Renderer[] renderers = player.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in renderers)
            {
                // URP Litの場合、_Surfaceプロパティを1に設定するとTransparentモードになる
                rend.material.SetFloat("_Surface", 1);

                // まず色を赤にして不透明（alpha=1）に設定する
                Color newColor = Color.red;
                newColor.a = 1f;
                rend.material.color = newColor;
            }

            // 1秒後に透明化する処理を開始する
            StartCoroutine(DelayedTransparency(player, 1.0f));

            Debug.Log("hit!");
        }
    }

    private IEnumerator DelayedTransparency(GameObject player, float delay)
    {
        yield return new WaitForSeconds(delay);

        // 再度子オブジェクト内のすべての Renderer を取得して透明化する
        Renderer[] renderers = player.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            // alpha値を0に変更して透明にする
            Color newColor = rend.material.color;
            newColor.a = 0f;
            rend.material.color = newColor;
        }
    }
}