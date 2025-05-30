using UnityEngine;
using UnityEngine.UI;

public class tutorialChargeTime : MonoBehaviour
{
    public Slider slider;
    public RawImage rawImage; // RawImageを追加
    public float duration = 5f; // 3秒でMaxに到達

    private float currentTime = 0f;
    private bool isRunning = true;
    private bool canReset = true; // スペースでリセット可能かどうか

    void Start()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }
        slider.value = 1; // 初期状態をマックスに設定

        if (Character.GetSelectedAnimal() == Character.GameCharacters.PANDA)
        {
            duration = Durations.NET_GUN_DURATION;
        }
        else
        {
            duration = Durations.INTERRUPT_GUN_DURATION;
        }
    }

    void Update()
    {
        if (isRunning)
        {
            currentTime += Time.deltaTime;
            float t = Mathf.Clamp01(currentTime / duration);
            slider.value = Mathf.Lerp(0, 1, t);


            // 3秒経過したらリセット可能にする
            if (currentTime >= duration)
            {
                canReset = true; // 3秒経過後はリセット可能
                rawImage.color = Color.white;
            }
        }

        if ((Input.GetKeyDown(KeyCode.Space) || OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)) && canReset)
        {
            ResetSlider();
            rawImage.color = new Color(0f, 246f / 255f, 1f, 1f); // R=0, G=246, B=255
        }
    }

    void ResetSlider()
    {
        currentTime = 0f;
        slider.value = 0; // スライダーを最小値に戻す
        isRunning = true; // リセット後に実行を開始
        canReset = false; // リセット後はスペースでの再実行を無効にする
    }
}