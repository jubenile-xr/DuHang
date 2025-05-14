using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PandaGaol : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI instructionUI;
    [SerializeField] private HitCountGoal hitGoalCount; // 参照を追加

    private void Start()
    {
        instructionUI.text = "小動物に向かって撃て！！！";
        HideObject();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Character.GetSelectedAnimal() == Character.GameCharacters.PANDA)
        {
            if (other.CompareTag("tutorialNet"))
            {
                gameObject.SetActive(false);
                hitGoalCount.SetHitCount();
                instructionUI.text = hitGoalCount.GetHitCount().ToString();
                if (hitGoalCount.GetHitCount() == 3)
                {
                    instructionUI.text = "自由に遊！";
                }
            }
        }
    }

    private void HideObject()
    {
        if (Character.GetSelectedAnimal() != Character.GameCharacters.PANDA)
        {
            gameObject.SetActive(false);
        }
    }
}