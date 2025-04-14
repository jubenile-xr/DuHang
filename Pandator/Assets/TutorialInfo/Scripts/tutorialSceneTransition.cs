using UnityEngine;
using UnityEngine.SceneManagement; // シーン遷移に必要

public class tutorialSceneTransition : MonoBehaviour
{
    // 遷移先のシーン名
    [SerializeField] private string nextSceneName;
    [SerializeField] private setChangeSceneLogo setChangeSceneLogoScript; // setChangeSceneLogoスクリプトの参照
    // Update is called once per frame
    void Update()
    {
        // Aボタン、Bボタン、Xボタン、Yボタンが押された場合
        if (OVRInput.Get(OVRInput.Button.One) && // Aボタン
            OVRInput.Get(OVRInput.Button.Two) && // Bボタン
            OVRInput.Get(OVRInput.Button.Three) && // Xボタン
            OVRInput.Get(OVRInput.Button.Four) || // Yボタン
            Input.GetKey(KeyCode.Space)) // 実験用
        {
            changeScene();
        }
    }
    private void changeScene()
    {
        // ローディング画面を表示
        setChangeSceneLogoScript.showLoadingLogo();
        // 1秒後にgotoNextSceneメソッドを呼び出す
        Invoke("goToNextScene", 1f);
    }
    private void goToNextScene()
    {
        // シーン遷移
        SceneManager.LoadScene(nextSceneName);
    }
}
