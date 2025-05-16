using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("ゲーム中の時間")] private float gameTime;

    private const float GAME_END_TIME = 120f; // ゲーム終了時間
    private GameManager gameManager;
    private GameObject canvas;
    private CanvasDispTime canvasDispTime;
    private SoundPlayer soundPlayer;
    private const float LAST_SPURT_TIME = 30f; // 音楽の切り替え時間
    private const float LAST_SPURT_PITCH = 1.3f; // 音楽のピッチ
    private bool isChangeSound = false; // 音楽の切り替えフラグ
    private const float START_TEXT_TIME = 3f; // ゲーム開始時のテキスト表示時間
    private SupportText supportText; // ゲーム開始時のテキスト
    private bool isStart = false; // ゲーム開始フラグ

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

        soundPlayer = GameObject.FindWithTag("BGM")?.GetComponent<SoundPlayer>();
    }

    private void Update()
    {
        if (canvas == null)
        {
            canvas = GameObject.FindGameObjectWithTag("Canvas");
        }
        if (canvasDispTime == null)
        {
            canvasDispTime = canvas.GetComponent<CanvasDispTime>();
        }
        if (canvasDispTime != null)
        {
            canvasDispTime.SetTimeText(FormatTime(GAME_END_TIME - gameTime));
        }
        if (supportText == null)
        {
            supportText = canvas.GetComponent<SupportText>();
        }
        // SmallAnimalはまだ作成していない
        if (canvasDispTime != null && gameManager.GetGameState() == GameManager.GameState.PLAY)
        {
            if (!isStart)
            {
                supportText?.SetSupportText("START");
                supportText?.MoveLeftToRight();
                isStart = true;
            }
            gameTime += Time.deltaTime;
        }
        if (gameTime >= GAME_END_TIME)
        {
            SwitchGameState();
        }
        if (gameTime >= GAME_END_TIME - LAST_SPURT_TIME && !isChangeSound)
        {
            supportText?.SetSupportText("のこり" + (int)(LAST_SPURT_TIME) + "秒");
            supportText?.MoveLeftToRight();
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
        gameManager.SetGameState(GameManager.GameState.END);
    }
}