using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MouseGaol : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI instructionUI;
    private void Start()
    {
        instructionUI.text = "パンダのボールに\nむかえ！";
        HideObject();
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
    private void HideObject()
    {
        if (Character.GetSelectedAnimal() != Character.GameCharacters.MOUSE)
        {
            gameObject.SetActive(false);
        }
    }
}