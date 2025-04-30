using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("ゲーム中の時間")] private float gameTime;

    private float gameEndTime = 300f; // ゲーム終了時間
    private GameManager gameManager;
    private GameObject canvas;
    private CanvasDispTime canvasDispTime;
    private SoundPlayer soundPlayer;
    private const float LAST_SPURT_TIME = 30f; // 音楽の切り替え時間
    private const float LAST_SPURT_PITCH = 1.5f; // 音楽のピッチ
    private bool isChangeSound = false; // 音楽の切り替えフラグ

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

        soundPlayer = GameObject.FindWithTag("BGM").GetComponent<SoundPlayer>();
        if (soundPlayer)
        {
            Debug.Log("SoundPlayer found.");
        }
        else
        {
            Debug.Log("SoundPlayer not found.");
        }
    }

    private void Update()
    {
        if (canvas == null)
        {
            canvas = GameObject.FindGameObjectWithTag("Canvas");
        }
        if (canvasDispTime == null)
        {
            canvasDispTime = canvas.GetComponentInChildren<CanvasDispTime>();
        }
        if (canvasDispTime != null)
        {

            canvasDispTime.SetTimeText(FormatTime(gameEndTime - gameTime));
        }
        // SmallAnimalはまだ作成していない
        if (canvasDispTime != null && gameManager.GetGameState() == GameManager.GameState.PLAY)
        {
            gameTime += Time.deltaTime;
            SwitchGameState();
        }
        if(gameTime >= gameEndTime - LAST_SPURT_TIME && !isChangeSound)
        {
            soundPlayer?.SetPitch(LAST_SPURT_PITCH);
            isChangeSound = true;
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