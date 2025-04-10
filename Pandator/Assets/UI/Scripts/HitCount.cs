using UnityEngine;
using TMPro; // TextMeshProを使うのに必要

public class HitCount : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hitText; // 命中回数を表示するText
    private int hitCount = 0; // 命中回数

    void Start()
    {
        UpdateHitText(); // 初期表示
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))//stateManeger次第で変えて
        {
            hitCount++;
            UpdateHitText();
        }
    }

    // 表示用の関数
    void UpdateHitText()
    {
        hitText.text = $"HitCount {hitCount} times";
    }
}
