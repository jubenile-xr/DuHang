using System.Globalization;
using UnityEngine;

public class ResultPandaWinManager : MonoBehaviour
{
    [SerializeField] private GameObject resultScene;
    [SerializeField] private GameObject pandaWinScene;
    [SerializeField] private GameObject smallAnimalWinScene;
    
    private GameObject currentScene;

    void Start()
    {
        // resultScene.SetActive(false);
       // switch (ShareData.GetWinner())
        //{
            // case "SMALLANIMAL": //TODO: enumとかの方がよき
                //currentScene =Instantiate(smallAnimalWinScene);
                //break;
            // case "PANDA":
        currentScene =Instantiate(pandaWinScene);
                //break;
        //}
        currentScene.SetActive(true);
    }

    public void SetDeActiveWinScene()
    {
        currentScene.SetActive(false);
        Debug.Log("SetDeActiveWinScene");
    }
    
    public void SetActiveResultScene()
    {
        resultScene.SetActive(true);
    }
    
}
