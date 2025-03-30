using UnityEngine;

public class InterrupteItem : MonoBehaviour
{
    private float time = 0.0f;
    [SerializeField] private float collisionDeleteTime = 0.1f;
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

                // 色を赤に変更し、alpha値を0に設定して完全な透明状態にする
                Color newColor = Color.red;
                newColor.a = 0f; // 必要に応じて透明度を調整してください

                rend.material.color = newColor;
            }

            Debug.Log("hit!");
        }
    }
}