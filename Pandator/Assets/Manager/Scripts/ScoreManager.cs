using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private float score;
    private float aliveTime;
    private int interruptedCount;
    [Header("スコア計算用")]
    [SerializeField] private float scoreMultiplier; // スコアの倍率
    [SerializeField] private float hitPoint; // 妨害のスコア

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
        // 非線形スコア計算: aliveTimeの2乗を使用
        score = Mathf.Pow(aliveTime, 2) / scoreMultiplier + interruptedCount * hitPoint;

        // 小数点以下を切り捨て
        score = Mathf.Floor(score);
    }
}
