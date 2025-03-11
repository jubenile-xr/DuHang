using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
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

    private float aliveCount; // IPunObservableの実装と一致させるためfloat型に変更
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
        winnerAnimalNameList = new List<string>(); // リストの初期化を追加
    }

    private void Update()
    {
        // 必要に応じて実装
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
        if (aliveCount <= 0) // 0以下の場合に変更
        {
            SetGameState(GameState.END);
            winner = Winner.PANDA;
            Debug.Log("GameEND・Winner: " + winner);
            // TODO　スコアを集める
        }
    }

    public void SetIncrementAliveCount()
    {
        aliveCount++;
        Debug.Log("aliveCount: "+aliveCount);
    }

    public void SetScoreList(string animalName, float score)
    {
        scoreList[animalName] = score;
    }

    // 勝利した動物の名前をリストに追加する関数
    public void AppendWinnerAnimalNameList(string animalName)
    {
        winnerAnimalNameList.Add(animalName);
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            // 自身のアバターのスタミナを送信する
            stream.SendNext(aliveCount);
        } else {
            // 他プレイヤーのアバターのスタミナを受信する
            aliveCount = (float)stream.ReceiveNext();
        }
    }
}