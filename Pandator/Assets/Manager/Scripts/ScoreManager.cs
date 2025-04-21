using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ScoreManager : MonoBehaviour
{
    private float score;
    private float aliveTime;
    private int interruptedCount;
    public GameManager gameManager;
    [Header("スコア計算用の定数")]
    [SerializeField] private float scoreMultiplier = 100f; // スコア計算のための定数
    [SerializeField] private float hitPoint = 50f; // 妨害のポイント
    [SerializeField] private float PandaMaxPoint = 1500; // パンダの最大ポイント
    private void Start()
    {
        score = 0;
        aliveTime = 0;
        interruptedCount = 0;
    }
    private void Update()
    {

    }

    public void SetAliveTime(float time)
    {
        aliveTime = time;
    }

    public void SetIncrementInterruptedCount()
    {
        interruptedCount++;
    }

    public float GetScore()
    {
        CalculateScore();
        return score;
    }

    // スコア計算 10ポイント/秒 + 5ポイント/中断 これは適当
    private void CalculateScore()
    {
        if (gameManager.GetPlayerType() == GameManager.PlayerType.MR)
        {
            // パンダのスコア計算
            score = PandaMaxPoint - aliveTime * 5;
        }
        else if (gameManager.GetPlayerType() == GameManager.PlayerType.VR)
        {
            score = Mathf.Pow(aliveTime, 2) / scoreMultiplier + interruptedCount * hitPoint;
        }
        else
        {
            score = -1; // 不明な動物の場合はスコアを-1に設定
        }
    }
}
