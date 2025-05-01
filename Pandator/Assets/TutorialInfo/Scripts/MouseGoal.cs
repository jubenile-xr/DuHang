using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MouseGaol : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI instructionUI;
    private void Start()
    {
        instructionUI.text = "RED BALL";
        HideObject();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (Character.GetSelectedAnimal() == Character.GameCharacters.MOUSE)
        {
            if (other.CompareTag("Player"))
            {
                instructionUI.text = "PLAY FREE";
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