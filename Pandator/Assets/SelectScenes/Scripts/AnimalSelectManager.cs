using UnityEngine;
using UnityEngine.SceneManagement; // シーン遷移に必要

public class AnimalSelectManager : MonoBehaviour
{
    private bool isAnimalSelected = false; // 動物が選択されたかどうかのフラグ
    [SerializeField] private bool isDebugFlag = true; // 動物選択UI
    [SerializeField] private SelectedAnimalUI selectedAnimalUI; // 動物選択UI

    // Update is called once per frame
    private void Update()
    {
        // Aボタンが押された場合
        if (OVRInput.Get(OVRInput.Button.One) || Input.GetKeyDown(KeyCode.R)) // Aボタン
        {
            SelectAnimal(Character.GameCharacters.RABBIT);
            selectedAnimalUI.RabbitedSelected();
        }
        // Bボタンが押された場合
        else if (OVRInput.Get(OVRInput.Button.Two) || Input.GetKeyDown(KeyCode.B)) // Bボタン
        {
            SelectAnimal(Character.GameCharacters.BIRD);
            selectedAnimalUI.BirdSelected();
        }
        // Xボタンが押された場合
        else if (OVRInput.Get(OVRInput.Button.Three) || Input.GetKeyDown(KeyCode.M)) // Xボタン
        {
            SelectAnimal(Character.GameCharacters.MOUSE);
            selectedAnimalUI.MouseSelected();
        }
        // Yボタンが押された場合
        else if (isDebugFlag && (OVRInput.Get(OVRInput.Button.Four) || Input.GetKeyDown(KeyCode.P))) // Yボタン
        {
            SelectAnimal(Character.GameCharacters.PANDA);
            selectedAnimalUI.ResetAnimalUI();
        }
        if (isAnimalSelected && (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) || Input.GetKeyDown(KeyCode.O)))
        {
            GoToTutorialScene();
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
}
