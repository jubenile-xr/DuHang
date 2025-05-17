using UnityEngine;
using TMPro;

public class PandaGaol : MonoBehaviour
{
    private HitCountGoal hitGoalCount; // 参照を追加

    void Start()
    {
        // HitCountGoalコンポーネントをシーン内から検索
        hitGoalCount = FindObjectOfType<HitCountGoal>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (Character.GetSelectedAnimal() == Character.GameCharacters.PANDA)
        {
            if (other.CompareTag("Net"))
            {
                gameObject.SetActive(false);
                hitGoalCount.SetHitCount();
            }
        }
    }
}