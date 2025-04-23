using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private float score;
    private float aliveTime;
    private int interruptedCount;

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
        ScoreField.SetDataAliveTime(aliveTime);
    }

    public void SetIncrementInterruptedCount()
    {
        interruptedCount++;
        ScoreField.SetIncrementInterruptedCount(interruptedCount);
    }

    public float GetScore()
    {
        CalculateScore();
        return score;
    }

    // スコア計算 10ポイント/秒 + 5ポイント/中断 これは適当
    private void CalculateScore()
    {
        score = aliveTime * 10 + interruptedCount * 5;
        ScoreField.SetDataScore(score);
    }
}
