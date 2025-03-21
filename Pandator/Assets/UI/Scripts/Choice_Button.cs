using UnityEngine;
using UnityEngine.UI;

public class UIButtonScript : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        Debug.Log("Button clicked");
    }
}
