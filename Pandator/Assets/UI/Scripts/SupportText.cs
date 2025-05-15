using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SupportText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI supportText;

    void Start()
    {
        supportText.gameObject.SetActive(false);
    }

    // 3秒間かけて左から右に移動
    public void MoveLeftToRight()
    {
        StartCoroutine(MoveText());
    }

    private IEnumerator MoveText()
    {
        supportText.gameObject.SetActive(true); // テキストを表示

        float duration = 3.0f; // 移動にかける時間
        float elapsed = 0f;
        Vector3 startPos = new Vector3(-Screen.width / 2, 0, 0); // 左端
        Vector3 endPos = new Vector3(Screen.width / 2, 0, 0); // 右端

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            supportText.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        supportText.gameObject.SetActive(false); // テキストを非表示
    }

    public void SetSupportText(string text)
    {
        supportText.text = text;
    }
}