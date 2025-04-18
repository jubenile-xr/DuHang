using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public enum GameState
    {
        START,
        PLAY,
        END
    }
    public enum PlayerType
    {
        MR,
        VR,
        GOD
    }

    private static string[] playerNameArray;

    private GameObject canvasObject;


    [Header("ゲームの状態はこっちで完全管理")]
    [SerializeField] private static GameState state = GameState.START;
    private Dictionary<string, float> scoreList;

    [SerializeField]
    private PlayerType playerType = PlayerType.MR;

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

        canvasObject = GameObject.FindWithTag("Canvas");
        if (canvasObject == null)
        {
            Debug.LogError("GameManager object not found!");
        }

    }

    private void Update()
    {
        if (GetGameState() == GameState.START && GetPlayerType() == PlayerType.GOD && ((OVRInput.Get(OVRInput.Button.One) && // Aボタン
                                                  OVRInput.Get(OVRInput.Button.Two) && // Bボタン
                                                  OVRInput.Get(OVRInput.Button.Three) && // Xボタン
                                                  OVRInput.Get(OVRInput.Button.Four))  || // Yボタン
            Input.GetKey(KeyCode.Space) // 実験用
                                         ))
        {
            SetGameState(GameState.PLAY);
            if (canvasObject != null)
            {
                MRKilleImagedAttach mrKilleImagedAttach = canvasObject.GetComponent<MRKilleImagedAttach>();
                if (mrKilleImagedAttach != null)
                {
                    for (int i = 0; i < playerNameArray.Length; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                MRKilleImagedAttach.SetFirstCharacter(playerNameArray[i]);
                                break;
                            case 1:
                                MRKilleImagedAttach.SetSecondCharacter(playerNameArray[i]);
                                break;
                            case 2:
                                MRKilleImagedAttach.SetThirdCharacter(playerNameArray[i]);
                                break;
                            default:
                                Debug.LogError("Invalid player index");
                                break;
                        }
                    }
                }
                else
                {
                    Debug.LogError("Canvas component is missing on the GameManager object!");
                }
            }
        }
        
    }

    public GameState GetGameState()
    {
        return state;
    }

    public PlayerType GetPlayerType()
    {
        return playerType;
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
                switch (playerType)
                {
                    case PlayerType.MR:
                        SceneManager.LoadScene("ResultClearMRScene");
                        break;
                    case PlayerType.VR:
                        SceneManager.LoadScene("ResultClearVRScene");
                        break;
                    default:
                        Debug.LogError("Unknown PlayerType");
                        break;
                }

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
    public static string[] GetPlayerNameArray()
    {
        return playerNameArray;
    }

    // プレイヤー名が配列の中に存在するか
    public static bool IsPlayerNameInArray(string playerName)
    {
        foreach (string name in playerNameArray)
        {
            if (name == playerName)
            {
                return true;
            }
        }
        return false;
    }

    public static void AddToPlayerNameArray(string playerName)
    {
        if (playerNameArray == null)
        {
            Debug.LogError("playerNameArray is null");
            return;
        }
        for (int i = 0; i < playerNameArray.Length; i++)
        {
            if (playerNameArray[i] == null)
            {
                playerNameArray[i] = playerName;
                break;
            }
        }
    }

    public static int GetPlayerNameArrayIndex(string playerName)
    {
        for (int i = 0; i < playerNameArray.Length; i++)
        {
            if (playerNameArray[i] == playerName)
            {
                return i;
            }
        }
        return -1; // 見つからなかった場合
    }
}