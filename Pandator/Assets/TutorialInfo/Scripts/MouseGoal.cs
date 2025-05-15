using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MouseGaol : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI instructionUI;
    private void Start()
    {
        if (Character.GetSelectedAnimal() != Character.GameCharacters.MOUSE)
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
        if (Character.GetSelectedAnimal() == Character.GameCharacters.MOUSE)
        {
            if (other.CompareTag("Player"))
            {
                gameObject.SetActive(false);
                instructionUI.text = "動物を打ってみよう！";
            }
        }
    }
}