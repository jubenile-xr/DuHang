using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public enum gameState
    {
        START,
        PLAY,
        END
    }
    
    [Header("ゲームの状態はこっちで完全管理")]
    [SerializeField] private gameState state;
    private Dictionary<string, float> scoreList;

    private int aliveCount;
    private enum Winner
    {
        SMALLANIMAL,
        PANDA,
    }
    [SerializeField] private Winner winner;
    private List<string> winnerAnimalNameList;

    private void Start()
    {
        state = gameState.START;
        scoreList = new Dictionary<string, float>();
        aliveCount = 0;
    }

    private void Update()
    {
    }

    public gameState GetGameState()
    {
        return state;
    }

    public void SetGameState(gameState newState)
    {
        state = newState;
    }

    public void SetDecrementAliveCount()
    {
        aliveCount--;
        if (aliveCount == 0)
        {
            SetGameState(gameState.END);
        }
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