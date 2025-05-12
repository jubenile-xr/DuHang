using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BirdGaol : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI instructionUI;
    private void Start()
    {
        instructionUI.text = "パンダのボールに\nむかえ！";
        HideObject();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (Character.GetSelectedAnimal() == Character.GameCharacters.BIRD)
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
        if (Character.GetSelectedAnimal() != Character.GameCharacters.BIRD)
        {
            gameObject.SetActive(false);
        }
    }
}