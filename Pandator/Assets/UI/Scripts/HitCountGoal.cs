using UnityEngine;
using TMPro;
public class HitCountGoal : MonoBehaviour
{
    private int _hitGoalCount;
    [SerializeField] private TextMeshProUGUI instructionUI;
    
    void Start()
    {
        _hitGoalCount = 0;
        instructionUI.text = "小動物に向かって撃て！！！";
    }
    public void SetHitCount()
    {
        _hitGoalCount++;
    }

    private int GetHitCount() // メソッド名を PascalCase に変更
    {
        return _hitGoalCount;
    }

    void Update()
    {
        if (GetHitCount() == 3)
        {
            instructionUI.text = "自由に動こう！";
        }
    }
}
