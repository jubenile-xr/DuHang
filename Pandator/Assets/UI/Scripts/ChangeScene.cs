using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ChangeScene : MonoBehaviour
{
    void Start()
    {
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button btn in buttons)
        {
            btn.onClick.AddListener(() => OnButtonClicked(btn));
        }
    }



    void OnButtonClicked(Button btn)
    {
        string sceneName = btn.gameObject.tag;
        SceneManager.LoadScene(sceneName);
    }

}
