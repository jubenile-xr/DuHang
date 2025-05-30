using UnityEngine;

public class LightningLightEffect : MonoBehaviour
{
    [Header("雷光（ライト）")]
    public Light lightningLight;

    [Header("雷光の設定")]
    public float flashDuration = 0.1f;      // ライトの持続時間
    public float fadeDuration = 0.3f;       // フェードアウト時間
    public float maxIntensity = 6f;         // 最大の明るさ

    [Header("雷の音")]
    public AudioSource thunderSound;

    [Header("ランダム雷発生設定")]
    public bool autoLightning = true;
    public float minInterval = 3f;
    public float maxInterval = 8f;

    private bool isFlashing = false;

    void Start()
    {
        if (lightningLight != null)
        {
            lightningLight.enabled = false;
        }

        if (autoLightning)
        {
            StartCoroutine(RandomLightningLoop());
        }
    }

    public void TriggerLightning()
    {
        if (!isFlashing)
        {
            StartCoroutine(FlashLightning());
        }
    }

    private System.Collections.IEnumerator FlashLightning()
    {
        isFlashing = true;

        // 雷音を再生
        if (thunderSound != null)
        {
            thunderSound.Play();
        }

        // ライトを点灯
        lightningLight.enabled = true;
        lightningLight.intensity = maxIntensity;
        yield return new WaitForSeconds(flashDuration);

        // ライトを徐々に暗くする
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            lightningLight.intensity = Mathf.Lerp(maxIntensity, 0, t / fadeDuration);
            yield return null;
        }

        lightningLight.enabled = false;
        isFlashing = false;
    }

    private System.Collections.IEnumerator RandomLightningLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);
            TriggerLightning();
        }
    }
}
