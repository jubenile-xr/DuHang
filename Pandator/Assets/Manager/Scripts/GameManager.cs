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
    private gameState state;
    private Dictionary<string, float> scoreList;

    private int aliveCount;
    private enum Winner
    {
        SMALLANIMAL,
        PANDA,
    }
    private Winner winner;
    private List<string> winnerAnimalNameList;

    void Start()
    {
        state = gameState.START;
        scoreList = new Dictionary<string, float>();
        aliveCount = 0;
    }

    void Update()
    {
        switch (state)
        {
            case gameState.START:
                break;
            case gameState.PLAY:
                if (aliveCount == 0)
                {
                    winner = Winner.PANDA;
                    setGameState(gameState.END);
                }
                // ここに時間による終了条件を追加 winnerはSMALLANIMAL 勝者小動物のリストにいれる
                // if ()
                // {
                //     winner = Winner.SMALLANIMAL;
                //     setGameState(gameState.END);
                // }
                break;
            case gameState.END:
                break;
        }
    }

    private void setGameState(gameState newState)
    {
        state = newState;
    }

    public void setAliveCount()
    {
        aliveCount--;
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