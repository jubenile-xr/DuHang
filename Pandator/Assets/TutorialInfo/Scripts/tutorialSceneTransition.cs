using System;
using UnityEngine;
using UnityEngine.SceneManagement; // シーン遷移に必要

// Enum declaration for scene types
public enum SceneType
{
    MR, // MRシーン
    VR  // VRシーン
}

public class tutorialSceneTransition : MonoBehaviour
{
    // 遷移先のシーン名
    [SerializeField] private SceneType nextScene = SceneType.MR;
    private string nextSceneName; // 遷移先のシーン名を格納する変数
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
        Invoke(nameof(goToNextScene), 1f);
    }

    private void goToNextScene()
    {
        if (nextScene == SceneType.MR)
        {
            nextSceneName = "MRScene";
        }
        else if (nextScene == SceneType.VR)
        {
            nextSceneName = "VRScene";
        }
        else
        {
            Debug.LogError("Invalid scene type selected.");
            return;
        }
        // シーン遷移
        SceneManager.LoadScene(nextSceneName);
    }
}