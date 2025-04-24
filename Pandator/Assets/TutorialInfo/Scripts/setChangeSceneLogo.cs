using UnityEngine;

public class setChangeSceneLogo : MonoBehaviour
{
    [SerializeField] private tutorialSceneTransition tutorialSceneTransitionScript; // tutorialSceneTransitionスクリプトの参照

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 初期状態でUI画像を非表示に設定
        gameObject.SetActive(false);
    }
    public void showLoadingLogo()
    {
        // UI画像を表示
        gameObject.SetActive(true);
    }
}