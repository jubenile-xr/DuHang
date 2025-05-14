using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PandaGaol : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI instructionUI;
    private HitCountGoal hitGoalCount;
    private void Start()
    {
        instructionUI.text = "小動物に向かって撃て！！！";
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
                HitCountGoal.Instance.SetHitCount(); // Singleton インスタンスを使用
                instructionUI.text = HitCountGoal.Instance.GetHitCount().ToString();
                if (HitCountGoal.Instance.GetHitCount() == 3)
                {
                    instructionUI.text = "自由に遊ぼう！";
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