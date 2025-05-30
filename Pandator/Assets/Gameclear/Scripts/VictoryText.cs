using UnityEngine;
using TMPro;
using System.Collections;

public class VictoryTextEffect : MonoBehaviour
{
    public TextMeshProUGUI victoryText; // TMP テキストコンポーネント
    public float shakeIntensity = 0.2f;  // 揺れの強さ
    private Vector3 originalCamPos;
    private Camera mainCamera;

    void Start()
    {
        // ゲーム開始時に「優勝」の文字を非表示にする
        victoryText.gameObject.SetActive(false);
        mainCamera = Camera.main;
    }

    void Update(){
        // 2秒後に「優勝」の文字を表示する
        if (Time.time > 2.0f && !victoryText.gameObject.activeSelf)
        {
            ShowVictoryText();
        }
    }

    public void ShowVictoryText()
    {
        StartCoroutine(AnimateText());
        StartCoroutine(CameraShake());
    }

    IEnumerator AnimateText()
    {
        victoryText.gameObject.SetActive(true); // テキストを表示

        float duration = 0.3f;
        float elapsed = 0f;
        Vector3 originalScale = victoryText.transform.localScale;
        victoryText.transform.localScale = Vector3.zero; // 初期スケールを0に設定

        // 拡大アニメーション（文字が爆発的に出現）
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            victoryText.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale * 1.2f, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 少し縮小して戻す
        victoryText.transform.localScale = originalScale;
    }

    IEnumerator CameraShake()
    {
        originalCamPos = mainCamera.transform.position;
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-shakeIntensity, shakeIntensity);
            float y = Random.Range(-shakeIntensity, shakeIntensity);
            mainCamera.transform.position = originalCamPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = originalCamPos;
    }
}
