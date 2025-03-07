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

    public void setAliveTime(float time)
    {
        aliveTime = time;
    }

    public void setInterruptedCount()
    {
        interruptedCount++;
    }

    public float getScore()
    {
        calculateScore();
        return score;
    }

    // スコア計算 10ポイント/秒 + 5ポイント/中断 これは適当
    private void calculateScore()
    {
        score = aliveTime * 10 + interruptedCount * 5;
    }
}
