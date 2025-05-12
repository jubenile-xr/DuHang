using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PandaGaol : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI instructionUI;
    private void Start()
    {
        instructionUI.text = "パンダの的に向かって撃て！";
        HideObject();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (Character.GetSelectedAnimal() == Character.GameCharacters.PANDA)
        {
            if (other.CompareTag("tutorialNet"))
            {
                gameObject.SetActive(false);
                instructionUI.text = "自由に遊んで！";
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