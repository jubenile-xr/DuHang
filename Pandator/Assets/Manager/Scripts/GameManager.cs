using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        START,
        PLAY,
        END
    }

    [Header("ゲームの状態はこっちで完全管理")]
    [SerializeField] private GameState state;
    private Dictionary<string, float> scoreList;

    private int aliveCount;
    private enum Winner
    {
        NONE,
        SMALLANIMAL,
        PANDA,
    }
    [SerializeField] private Winner winner;
    private List<string> winnerAnimalNameList;

    private void Start()
    {
        state = GameState.START;
        scoreList = new Dictionary<string, float>();
        aliveCount = 0;
        winner = Winner.NONE;
    }

    private void Update()
    {
    }

    public GameState GetGameState()
    {
        return state;
    }

    public void SetGameState(GameState newState)
    {
        state = newState;
    }

    public void SetDecrementAliveCount()
    {
        aliveCount--;
        if (aliveCount == 0)
        {
            SetGameState(GameState.END);
            winner = Winner.PANDA;
            Debug.Log("Panda Win");
            // TODO　スコアを集める
        }
    }
    public void SetIncrementAliveCount()
    {
        aliveCount++;
        Debug.Log("aliveCount: " + aliveCount);
    }

    public void SetScoreList(string animalName, float score)
    {
        scoreList[animalName] = score;
    }
    // 勝利した動物の名前をリストに追加する関数
    public void appendWinnerAnimalNameList(string animalName)
    {
        winnerAnimalNameList.Add(animalName);
    }
}