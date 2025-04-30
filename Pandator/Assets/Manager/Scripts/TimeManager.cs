using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("ゲーム中の時間")] private float gameTime;

    private float gameEndTime = 300f; // ゲーム終了時間
    private GameManager gameManager;
    private GameObject canvas;
    private CanvasDispTime canvasDispTime;

    private void Start()
    {
        gameTime = 0;

        GameObject gmObj = GameObject.FindWithTag("GameManager");
        if (gmObj)
        {
            gameManager = gmObj.GetComponent<GameManager>();
            if (gameManager)
            {
                Debug.Log("GameManager found.");
            }
        }
    }

    private void Update()
    {
        if (canvas == null)
        {
            canvas = GameObject.FindGameObjectWithTag("Canvas");
        }
        if (canvasDispTime == null && canvas != null)
        {
            canvasDispTime = canvas.GetComponentInChildren<CanvasDispTime>();
        }
        if (canvasDispTime != null)
        {
            canvasDispTime.SetTimeText(FormatTime(gameTime));
        }
        // SmallAnimalはまだ作成していない
        if (canvasDispTime != null && gameManager.GetGameState() == GameManager.GameState.PLAY)
        {
            gameTime += Time.deltaTime;
            SwitchGameState();
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
            gameManager.SetGameState(GameManager.GameState.END);
        }
    }
}