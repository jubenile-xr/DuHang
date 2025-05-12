using UnityEngine;

public class HitCountGoal : MonoBehaviour
{
    private int hitGoalCount;

    public void SetHitCount()
    {
        hitGoalCount++;
    }

    public int GetHitCount() // メソッド名を PascalCase に変更
    {
        return hitGoalCount;
    }
}