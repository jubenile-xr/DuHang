using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private enum gameState
    {
        Tutorial,
        Playing,
        Finished
    }
    private gameState state;
    private Dictionary<string, float> scoreList; // key: animal name, value: score

    private int aliveCount;
    private float gameTime;

    void Start()
    {
        state = gameState.Tutorial;
        gameTime = 0;
        scoreList = new Dictionary<string, float>(); // 修正
        aliveCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case gameState.Tutorial:
                break;
            case gameState.Playing:
                gameTime += Time.deltaTime;
                break;
            case gameState.Finished:
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
}