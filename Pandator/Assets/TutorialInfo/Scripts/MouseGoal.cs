using UnityEngine;
using UnityEngine.UI;

public class MouseGaol : MonoBehaviour
{
    private void Start()
    {
        HideObject(); // 毎フレーム呼び出す
    }
    private void OnTriggerEnter(Collider other)
    {
        if (Character.GetSelectedAnimal() == Character.GameCharacters.MOUSE)
        {
            if (other.CompareTag("Player"))
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