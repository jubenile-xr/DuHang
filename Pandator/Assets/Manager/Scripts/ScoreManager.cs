using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private float score;
    private float aliveTime;
    private int interruptedCount;
    private string playerName;
    [Header("ゲームマネージャー")] private GameManager gameManager;

    private void Start()
    {
        score = 0;
        aliveTime = 0;
        interruptedCount = 0;
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
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
    }

    public void SetIncrementInterruptedCount()
    {
        interruptedCount++;
    }

    public float GetScore()
    {
        CalculateScore();
        return score;
    }

    // スコア計算 10ポイント/秒 + 5ポイント/中断 これは適当
    private void CalculateScore()
    {
        score = aliveTime * 10 + interruptedCount * 5;
    }

    private void SendScoreToGameManager()
    {
        string[] playerNames = gameManager.GetAllPlayerNames();
        Debug.Log("Score:DeadLogic: Player Names: " + string.Join(", ", playerNames));
        Debug.Log("Score:PlayerName" + playerName);

        for (int i = 0; i < playerNames.Length; i++)
        {
            Debug.Log("Score:PlayerNames TF" + playerNames[i].Contains(playerName));
            if (playerNames[i].Contains(playerName))
            {
                gameManager.SetLocalPlayerScore(i, GetScore());
            }
        }
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
    }
}
