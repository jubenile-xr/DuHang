public static class ScoreField
{
    private static float dataInterruptedCount;
    private static float dataScore;
    private static float dataAliveTime;

    public static void SetDataAliveTime(float time)
    {
        dataAliveTime = time;
    }

    public static void SetIncrementInterruptedCount(int interruptedCount)
    {
        dataInterruptedCount = interruptedCount;
    }

    // スコア計算 10ポイント/秒 + 5ポイント/中断
    public static void SetDataScore(float score)
    {
        dataScore = score;
    }

    public static float GetScore()
    {
        return dataScore;
    }
    public static int GetInterruptedCount()
    {
        return (int)dataInterruptedCount;
    }

    public static float GetAliveTime()
    {
        return dataAliveTime;
    }
}
