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
            isCollision = true;

            // プレイヤーの状態を Interrupted に設定する
            player.GetComponent<StateManager>()?.SetInterrupted(true);

            // 子オブジェクト内のすべての Renderer を取得する
            Renderer[] renderers = player.GetComponentsInChildren<Renderer>();
            // 各Rendererの元の色を保存するための配列
            Color[] originalColors = new Color[renderers.Length];

            for (int i = 0; i < renderers.Length; i++)
            {
                // 元の色を保存する
                originalColors[i] = renderers[i].material.color;

                // URP Litの場合、_Surfaceプロパティを1に設定するとTransparentモードになる
                renderers[i].material.SetFloat("_Surface", 1);

                // まず色を赤にして不透明（alpha=1）に設定する
                Color newColor = Color.red;
                newColor.a = 1f;
                renderers[i].material.color = newColor;
            }

            // 1秒後に元の色に戻す処理を開始する
            StartCoroutine(ResetColorAfterDelay(renderers, originalColors, 1.0f));

            Debug.Log("hit!");
        }
    }

    private IEnumerator ResetColorAfterDelay(Renderer[] renderers, Color[] originalColors, float delay)
    {
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                // 元の色に戻す
                renderers[i].material.color = originalColors[i];
            }
        }
    }
}