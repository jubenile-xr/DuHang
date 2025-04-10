using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
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
        Debug.Log("aliveCount: " + aliveCount);
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
        // Debug.Log("SetDecrementAliveCount called");
        aliveCount--;
        Debug.Log("aliveCountDecrement: " + aliveCount);

        UpdateAliveCountProperty();

        //0以下じゃないと動かない
        if (aliveCount <= 0) 
        {
            SetGameState(GameState.END);
            winner = Winner.PANDA;
            Debug.Log("Panda Win");
            // TODO　スコアを集める
            if (aliveCount == 0)
            {
                PhotonNetwork.Instantiate("GameOverCube",new Vector3(0f, 0f, 0f), Quaternion.identity, 0);　//TODO パンダが優勝した時の遷移にする
            }
        }
    }
    public void SetIncrementAliveCount()
    {
        aliveCount++;
        Debug.Log("aliveCountIncrement: " + aliveCount);

        UpdateAliveCountProperty();
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

    private void UpdateAliveCountProperty()
    {
        if (PhotonNetwork.InRoom)
        {
            ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
            
            properties.Add("aliveCount", aliveCount);
            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if(propertiesThatChanged.ContainsKey("aliveCount"))
        {
            aliveCount = (int)propertiesThatChanged["aliveCount"];
            Debug.Log("aliveCount: " + aliveCount);
        }
    }
}