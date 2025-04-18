using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("ゲーム中の時間")]
    private float gameTime;

    [Header("ゲームの終了時間")]
    [SerializeField]private float gameEndTime = 5;
    private GameManager gameManager;
    private CanvasDispTime canvasDispTime;

    private void Start()
    {
        gameTime = 0;
        gameManager = this.GetComponent<GameManager>();
        if (gameManager == null)
        {
            Debug.Log("GameManagerがアタッチされていません");
        }
        gameManager.SetGameState(GameManager.GameState.START);

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
        gameTime += Time.deltaTime;
        SwitchGameState();
        // SmallAnimalはまだ作成していない
        if(canvasDispTime != null){
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