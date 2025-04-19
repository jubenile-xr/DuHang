using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("ゲーム中の時間")]
    private float gameTime;

    [Header("ゲームの終了時間")]
    [SerializeField]private float gameEndTime = 5000;
    private GameManager gameManager;
    private CanvasDispTime canvasDispTime;

    private void Start()
    {
        gameTime = 0;
        if (canvasDispTime != null)
        {
            canvasDispTime.SetTimeText(FormatTime(gameTime));
        }

        GameObject gmObj = GameObject.FindWithTag("GameManager");
        if (gmObj)
        {
            gameManager = gmObj.GetComponent<GameManager>();
            if (gameManager)
            {
                Debug.Log("GameManager found.");
            }
        }
        // CanvasDispTimeのインスタンスを取得
        // PandaCanvasの中にある子オブジェクトのTimeからCanvasDispTimeを取得
        GameObject canvasObj = GameObject.Find("PandaCanvas");
        if (canvasObj == null)
        {
            Debug.Log("PandaCanvasが見つかりません");
        }else{
            canvasDispTime = canvasObj.GetComponentInChildren<CanvasDispTime>();
        }
    }
    private void Update()
    {
        // SmallAnimalはまだ作成していない
        if(canvasDispTime != null && gameManager.GetGameState() == GameManager.GameState.PLAY){
            gameTime += Time.deltaTime;
            SwitchGameState();
            canvasDispTime.SetTimeText(FormatTime(gameTime));
        }
    }

    private string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;
        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    // getter
    public float GetGameTime()
    {
        return gameTime;
    }

    private void SwitchGameState()
    {
        if (gameTime >= gameEndTime)
        {
            // gameManager.SetGameState(GameManager.GameState.END);
        }
    }


}