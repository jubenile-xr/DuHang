using UnityEngine;
using UnityEngine.SceneManagement; // シーン遷移に必要

public class AnimalSelectManager : MonoBehaviour
{
    private bool isAnimalSelected = false; // 動物が選択されたかどうかのフラグ
    [SerializeField] private bool isDebugFlag = true; // デバッグ用フラグ
    [SerializeField] private SelectedAnimalUI selectedAnimalUI; // 動物選択UI

    private int currentSelectionIndex = 0; // 現在選択されているキャラクターのインデックス
    private Character.GameCharacters[] animals = new Character.GameCharacters[]
    {
        Character.GameCharacters.BIRD,
        Character.GameCharacters.RABBIT,
        Character.GameCharacters.MOUSE
    };

    private void Start()
    {
        // 初期状態のUIを更新
        UpdateSelectionUI();
    }

    // Update is called once per frame
    private void Update()
    {
        // 左の中指ボタンで左に移動
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveSelectionLeft();
        }
        // 右の中指ボタンで右に移動
        else if (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveSelectionRight();
        }

        // A,B,X,Yボタンで選択を確定
        if ((OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger)) || Input.GetKeyDown(KeyCode.Return))
        {
            SelectAnimal(animals[currentSelectionIndex]);
        }  else if ((OVRInput.Get(OVRInput.Button.One) && OVRInput.Get(OVRInput.Button.Two)) || Input.GetKeyDown(KeyCode.P)) // Yボタン
        {
            SelectAnimal(Character.GameCharacters.PANDA);
        }

        // // チュートリアルシーンに遷移
        // if (isAnimalSelected && (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) || Input.GetKeyDown(KeyCode.O)))
        // {
        //     GoToTutorialScene();
        // }
    }

    private void MoveSelectionLeft()
    {
        currentSelectionIndex--;
        if (currentSelectionIndex < 0)
        {
            currentSelectionIndex = animals.Length - 1; // 一番右にループ
        }
        UpdateSelectionUI();
        Debug.Log($"Current Selection: {animals[currentSelectionIndex]}");
    }

    private void MoveSelectionRight()
    {
        currentSelectionIndex++;
        if (currentSelectionIndex >= animals.Length)
        {
            currentSelectionIndex = 0; // 一番左にループ
        }
        UpdateSelectionUI();
        Debug.Log($"Current Selection: {animals[currentSelectionIndex]}");
    }

    private void UpdateSelectionUI()
    {
        // 現在の選択に応じてUIを更新
        switch (animals[currentSelectionIndex])
        {
            case Character.GameCharacters.BIRD:
                selectedAnimalUI.BirdSelected();
                break;
            case Character.GameCharacters.RABBIT:
                selectedAnimalUI.RabbitedSelected();
                break;
            case Character.GameCharacters.MOUSE:
                selectedAnimalUI.MouseSelected();
                break;
        }
    }

    private void SelectAnimal(Character.GameCharacters animal)
    {
        Character.SetSelectedAnimal(animal);
        isAnimalSelected = true;
        Debug.Log($"Selected Animal: {animal}");

        // チュートリアルシーンに移行
        GoToTutorialScene();
    }

    private void GoToTutorialScene()
    {
        // チュートリアルシーンに遷移する処理
        SceneManager.LoadScene("TutorialScene");
        Debug.Log("Go to Tutorial Scene");
    }
}
