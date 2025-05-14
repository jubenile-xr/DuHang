using UnityEngine;

public class HitCountGoal : MonoBehaviour
{
    private static HitCountGoal instance;
    private int hitGoalCount;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // シーンを跨いでも消えない
        }
        else
        {
            Destroy(gameObject); // 既に存在する場合は新しいインスタンスを破棄
        }
    }

    public void SetHitCount()
    {
        hitGoalCount++;
    }

    public int GetHitCount()
    {
        return hitGoalCount;
    }

    public static HitCountGoal Instance
    {
        get { return instance; }
    }
}