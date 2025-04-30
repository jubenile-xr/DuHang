using UnityEngine;
using UnityEngine.UI;

public class MouseGaol : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (Character.GetSelectedAnimal() == Character.GameCharacters.MOUSE)
        {
            if (other.CompareTag("MasterPlayer"))
            {
                transform.position += new Vector3(0, 10f, 0);
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