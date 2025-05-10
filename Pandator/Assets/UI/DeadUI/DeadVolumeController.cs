using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class DeadVolumeController : MonoBehaviour
{
    public Volume volume; // InspectorでVolumeをアサイン
    private ColorAdjustments colorAdjustments;

    private bool isRunning = false;
    private bool isDead = false;
    [SerializeField] private SoundPlayer deadSE;

    void Start()
    {
        // VolumeコンポーネントからColorAdjustmentsを取得
        if (volume.profile.TryGet(out colorAdjustments))
        {
            this.colorAdjustments.colorFilter.overrideState = false;
            colorAdjustments.saturation.overrideState = false;
        }
        else
        {
            Debug.LogError("ColorAdjustments not found in the volume profile.");
        }
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.D)  && !isRunning)
        // {
        //     StartCoroutine(HandleColorAdjustmentSequence());
        // }
    }

    private IEnumerator HandleColorAdjustmentSequence()
    {
        isRunning = true;
        deadSE?.Play();

        // ColorFilterをオンにする
        colorAdjustments.colorFilter.overrideState = true;
        colorAdjustments.saturation.overrideState = false;

        Color startColor = colorAdjustments.colorFilter.value;
        Color targetColor = Color.black;

        float duration = 2f;
        float elapsed = 0f;

        // 2秒かけて黒にフェード
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            colorAdjustments.colorFilter.value = Color.Lerp(startColor, targetColor, elapsed / duration);
            yield return null;
        }
        colorAdjustments.colorFilter.value = targetColor;

        // 3秒待機
        yield return new WaitForSeconds(1f);

        // ColorFilterをオフ、Saturationをオン
        colorAdjustments.colorFilter.overrideState = false;
        colorAdjustments.saturation.overrideState = true;

        isRunning = false;
    }
    public void RunDeadVolume()
    {
        StartCoroutine(HandleColorAdjustmentSequence());
    }
}
