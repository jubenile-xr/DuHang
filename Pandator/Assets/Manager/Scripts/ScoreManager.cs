using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    private float score;
    private float aliveTime;
    private int interruptedCount;
    private string playerName;
    [Header("ゲームマネージャー")]
    private GameManager gameManager;
    private TimeManager timeManager;
    [Header("スコア計算用の定数")]
    [SerializeField] private float scoreMultiplier = 100f; // スコア計算のための定数
    [SerializeField] private float hitPoint = 50f; // 妨害のポイント
    [SerializeField] private float PandaMaxPoint = 1500; // パンダの最大ポイント

    private void Start()
    {
        score = 0;
        aliveTime = 0;
        interruptedCount = 0;
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        // TimeManager の参照を取得（TimeManager クラスに GetGameTime メソッドを実装する必要があります）
        timeManager = GameObject.FindObjectOfType<TimeManager>();
    }

    private void Update()
    {
        if (gameManager.GetGameState() == GameManager.GameState.END)
        {
            SendScoreToGameManager();
        }
    }

    public void SetAliveTime(float time)
    {
        aliveTime = time;
        ScoreField.SetDataAliveTime(aliveTime);
    }

    public void SetIncrementInterruptedCount()
    {
        interruptedCount++;
        ScoreField.SetIncrementInterruptedCount(interruptedCount);
    }

    public float GetScore()
    {
        CalculateScore();
        return score;
    }

    // スコア計算 10ポイント/秒 + 5ポイント/中断（例）
    private void CalculateScore()
    {
        if (gameManager.GetPlayerType() == GameManager.PlayerType.MR)
        {
            // Panda のスコア計算
            // timeManager.GetGameTime() は TimeManager に実装する必要があります。
            score = PandaMaxPoint - (int)timeManager.GetGameTime() * 5;
        }
        else if (gameManager.GetPlayerType() == GameManager.PlayerType.VR)
        {
            score = Mathf.Pow(aliveTime, 2) / scoreMultiplier + interruptedCount * hitPoint;
        }
        else
        {
            score = -1;
        }
        ScoreField.SetDataScore(score);
    }

    private void SendScoreToGameManager()
    {
        string[] playerNames = gameManager.GetAllPlayerNames();
        Debug.Log("Score:DeadLogic: Player Names: " + string.Join(", ", playerNames));
        Debug.Log("Score:PlayerName: " + playerName);

        if (gameManager.GetPlayerType() == GameManager.PlayerType.VR)
        {
            for (int i = 0; i < playerNames.Length; i++)
            {
                if (playerNames[i].Equals(playerName))
                {
                    gameManager.SetLocalPlayerScore(i, GetScore());
                }
            }
        }
        else if (gameManager.GetPlayerType() == GameManager.PlayerType.MR)
        {
            gameManager.SetLocalPlayerScore(-1, GetScore());
        }
    }

    public void SetPlayerName(string name) => playerName = name;
    public string GetPlayerName() => playerName;
}
