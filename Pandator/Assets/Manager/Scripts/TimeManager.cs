using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("ゲーム中の時間")]
    private float gameTime;

    [Header("ゲームの終了時間")]
    [SerializeField]private float gameEndTime = 5;

    [Header("gameState変更用のためのGameManagerオブジェクト")]
    [SerializeField]private GameObject gameManagerObject;
    private GameManager gameManager;

    void Start()
    {
        gameTime = 0;
        
        gameManager = gameManagerObject.GetComponent<GameManager>();
        gameManager.SetGameState(GameManager.gameState.START);
    }
    void Update()
    {
        gameTime += Time.deltaTime;
        SwitchGameState();
    }
    
    // getter
    public float GetGameTime()
    {
        return gameTime;
    }

    // formatter 00:00
    private string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;
        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    private void SwitchGameState()
    {
        if (gameTime >= gameEndTime)
        {
            gameManager.SetGameState(GameManager.gameState.END);
        }
    }
    

}