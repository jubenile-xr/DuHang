using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private enum gameState
    {
        START,
        PLAY,
        END
    }
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

    private void setGameState(gameState newState)
    {
        state = newState;
    }

    public void setAliveCount()
    {
        aliveCount--;
        if (aliveCount == 0)
        {
            setGameState(gameState.END);
        }
    }

    public void setScoreList(string animalName, float score)
    {
        scoreList[animalName] = score;
    }
    // 勝利した動物の名前をリストに追加する関数
    public void appendWinnerAnimalNameList(string animalName)
    {
        winnerAnimalNameList.Add(animalName);
    }
}