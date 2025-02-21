using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private float score;
    private float aliveTime;
    private int interruptedCount;

    void Start()
    {
        score = 0;
        aliveTime = 0;
        interruptedCount = 0;
    }

    // Update is called once per frame
    void Update()
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

    private void calculateScore()
    {
        score = aliveTime * 10 - interruptedCount * 5;
    }
}
