using System;
using UnityEngine;
using UnityEngine.SceneManagement; // シーン遷移に必要

public class tutorialSceneTransition : MonoBehaviour
{
    private string nextSceneName; // 遷移先のシーン名を格納する変数
    [SerializeField] private setChangeSceneLogo setChangeSceneLogoScript; // setChangeSceneLogoスクリプトの参照

    private void Start(){
        if(Character.GetSelectedAnimal() == Character.GameCharacters.GOD)
        {
            nextSceneName = "GodScene";
        }
        else if(Character.GetSelectedAnimal() == Character.GameCharacters.PANDA)
        {
            nextSceneName = "MRScene";
        }else{
            nextSceneName = "VRScene";
        }
    }
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

        // スペースキーが押された場合
        if (Input.GetKeyDown(KeyCode.A)) // スペースキー
        {
            Debug.Log(Character.GetSelectedAnimal());
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
        // 遷移先のシーンに遷移
        SceneManager.LoadScene(nextSceneName);
    }
}