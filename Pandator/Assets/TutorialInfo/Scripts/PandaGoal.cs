using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PandaGaol : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI instructionUI;
    private HitCountGoal hitGoalCount;
    private void Start()
    {
        instructionUI.text = "小動物に向かって撃て！";
        HideObject();
        hitGoalCount = new HitCountGoal();

    }
    private void OnTriggerEnter(Collider other)
    {
        if (Character.GetSelectedAnimal() == Character.GameCharacters.PANDA)
        {
            if (other.CompareTag("tutorialNet"))
            {
                gameObject.SetActive(false);
                hitGoalCount.SetHitCount(); // インスタンスを使用
                if (hitGoalCount.GetHitCount() == 3)
                {
                    instructionUI.text = "自由に遊んで！";
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