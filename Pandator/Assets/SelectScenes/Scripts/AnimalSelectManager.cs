using UnityEngine;
using UnityEngine.SceneManagement; // シーン遷移に必要

public class AnimalSelectManager : MonoBehaviour
{
    private bool isAnimalSelected = false; // 動物が選択されたかどうかのフラグ

    // Update is called once per frame
    private void Update()
    {
        // Aボタンが押された場合
        if (OVRInput.Get(OVRInput.Button.One) || Input.GetKeyDown(KeyCode.R)) // Aボタン
        {
            SelectAnimal(Character.GameCharacters.RABBIT);
        }
        // Bボタンが押された場合
        else if (OVRInput.Get(OVRInput.Button.Two) || Input.GetKeyDown(KeyCode.B)) // Bボタン
        {
            SelectAnimal(Character.GameCharacters.BIRD);
        }
        // Xボタンが押された場合
        else if (OVRInput.Get(OVRInput.Button.Three) || Input.GetKeyDown(KeyCode.M)) // Xボタン
        {
            SelectAnimal(Character.GameCharacters.MOUSE);
        }
        // Yボタンが押された場合
        // TODO: PANDAは1人しか選べないようにしないといけない
        else if (OVRInput.Get(OVRInput.Button.Four) || Input.GetKeyDown(KeyCode.P)) // Yボタン
        {
            SelectAnimal(Character.GameCharacters.PANDA);
        }
        // スペースキーが押された場合
        else if (Input.GetKeyDown(KeyCode.G)) // スペースキー
        {
            SelectAnimal(Character.GameCharacters.GOD);
        }
        if (isAnimalSelected && (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) || Input.GetKeyDown(KeyCode.O)))
        {
            if (Character.GetSelectedAnimal() == Character.GameCharacters.GOD)
            {
                GoToGodScene();
            }
            else
            {
                GoToTutorialScene();
            }
        }
    }

    private void SelectAnimal(Character.GameCharacters animal)
    {
        Character.SetSelectedAnimal(animal);
        isAnimalSelected = true;
        Debug.Log(animal);
    }
    private void GoToTutorialScene()
    {
        // チュートリアルシーンに遷移する処理
        SceneManager.LoadScene("TutorialScene");
        Debug.Log("Go to Tutorial Scene");
    }
    private void GoToGodScene()
    {
        // ゲームシーンに遷移する処理
        SceneManager.LoadScene("GodScene");
        Debug.Log("Go to God Scene");
    }
}
