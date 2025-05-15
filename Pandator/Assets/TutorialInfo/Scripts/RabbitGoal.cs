using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RabbitGoal : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI instructionUI;
    private void Start()
    {
        if (Character.GetSelectedAnimal() != Character.GameCharacters.RABBIT)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            instructionUI.text = "パンダのボールに\nむかえ！";
        }
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
}