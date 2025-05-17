using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BirdGaol : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI instructionUI;
    private void Start()
    {
        if (Character.GetSelectedAnimal() == Character.GameCharacters.BIRD)
        {
            gameObject.SetActive(true);
            instructionUI.text = "パンダのボールに\nむかえ！";
        }
        else
        {
            gameObject.SetActive(false);
        }
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
}