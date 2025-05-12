using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PandaGaol : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI instructionUI;
    private int hitGoalCount;
    private void Start()
    {
        instructionUI.text = "小動物に向かって撃て！";
        HideObject();
        hitGoalCount = 0;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (Character.GetSelectedAnimal() == Character.GameCharacters.PANDA)
        {
            if (other.CompareTag("tutorialNet"))
            {
                gameObject.SetActive(false);
                hitGoalCount++;
                if (hitGoalCount = 3)
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