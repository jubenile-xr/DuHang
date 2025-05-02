using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RabbitGoal : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI instructionUI;
    private void Start()
    {
        instructionUI.text = "パンダのボールにむかえ！";
        HideObject();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (Character.GetSelectedAnimal() == Character.GameCharacters.RABBIT)
        {
            if (other.CompareTag("Player"))
            {
                gameObject.SetActive(false);
                instructionUI.text = "自由に動き回って！";
            }
        }
    }
    private void HideObject()
    {
        if (Character.GetSelectedAnimal() != Character.GameCharacters.RABBIT)
        {
            gameObject.SetActive(false);
        }
    }
}