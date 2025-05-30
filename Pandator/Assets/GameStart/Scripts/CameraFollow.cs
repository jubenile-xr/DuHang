using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CameraFollowHead : MonoBehaviour
{
    [Header("複数キャラの頭部（ターゲット）")]
    public Transform[] targets;  // 順番：鳥、ウサギ、ネズミ - パンダ

    [Header("各ターゲットの表示時間")]
    public float followDuration = 2f;

    [Header("カメラのオフセット")]
    public Vector3 offset = new Vector3(0, 0, 1);

    [Header("追従スムーズ速度")]
    public float followSpeed = 6f;

    [Header("固定視点への切り替え時間")]
    public float switchAfterSeconds = 8f;
    public float duration = 3f;

    [Header("固定視点の位置（空のオブジェクト）")]
    public Transform fixedViewPoint;

    [Header("ぼかし効果（PostProcessing Volume）")]
    public Volume blurVolume;　//いま使えない

    [Header("UIキャンバスグループ")]
    public CanvasGroup uiCanvasGroup; //***** For display ONLY *****

    [Header("切り替え時に再生するBGM")]
    public AudioSource bgmSource; //いま使えない

    private float totalTimer = 0f;
    private bool switched = false;

    private int currentTargetIndex = 0;
    private float localTimer = 0f;

    void Start()
    {
        if (bgmSource == null)
            bgmSource = GetComponent<AudioSource>();

        if (blurVolume != null)
            blurVolume.weight = 0f;

        if (uiCanvasGroup != null)
        {
            uiCanvasGroup.alpha = 0f;
            uiCanvasGroup.interactable = false;
            uiCanvasGroup.blocksRaycasts = false;
        }
    }

    void LateUpdate()
    {
        totalTimer += Time.deltaTime;

        if (!switched)
        {
            // 前半：キャラを順番に追従
            if (totalTimer >= switchAfterSeconds)
            {
                SwitchToFixedView();
                switched = true;
            }
            else
            {
                UpdateFollowSequence(); // 順番に追いかける処理
            }
        }
    }

    void UpdateFollowSequence()
    {
        if (targets == null || targets.Length == 0) return;

        localTimer += Time.deltaTime;

        if (localTimer >= followDuration)
        {
            currentTargetIndex = (currentTargetIndex + 1) % targets.Length;
            localTimer = 0f;
        }

        Transform target = targets[currentTargetIndex];
        if (target == null) return;

        Vector3 targetPosition = target.position + target.TransformDirection(offset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        transform.LookAt(target);
    }

    void SwitchToFixedView()
    {
        if (fixedViewPoint != null)
            StartCoroutine(SmoothSwitchToFixedView(fixedViewPoint.position, fixedViewPoint.rotation));

        if (blurVolume != null)
            StartCoroutine(FadeInBlur());

        if (uiCanvasGroup != null)
            StartCoroutine(FadeInUI());
    }

    System.Collections.IEnumerator SmoothSwitchToFixedView(Vector3 targetPos, Quaternion targetRot)
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        if (bgmSource != null && !bgmSource.isPlaying)
        {
            bgmSource.Play();
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
    }

    System.Collections.IEnumerator FadeInBlur()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            blurVolume.weight = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
    }

    System.Collections.IEnumerator FadeInUI()
    {
        float t = 0f;
        uiCanvasGroup.interactable = true;
        uiCanvasGroup.blocksRaycasts = true;

        while (t < 1f)
        {
            t += Time.deltaTime;
            uiCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
    }
}
