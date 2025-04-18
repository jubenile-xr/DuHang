using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Oculus.Platform.Models;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
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


    private GameObject canvasObject;

    [Header("ゲームの状態はこっちで完全管理")]
    private GameState state = GameState.START;
    private Dictionary<string, float> scoreList;
    private bool hasPlayerNameCreated = false;

    [SerializeField]
    private PlayerType playerType = PlayerType.MR;

    private int aliveCount;
    private enum Winner
    {
        NONE,
        SMALLANIMAL,
        PANDA,
    }
    [SerializeField]
    private Winner winner;
    private List<string> winnerAnimalNameList;

    private void Start()
    {
        state = GameState.START;
        scoreList = new Dictionary<string, float>();
        aliveCount = 0;
        winner = Winner.NONE;
        winnerAnimalNameList = new List<string>();

        if (GetPlayerType() == PlayerType.VR)
        {
            SetLocalPlayerName(PhotonNetwork.NickName);
        }
    }

    private void Update()
    {
        if (GetGameState() == GameState.START && GetPlayerType() == PlayerType.GOD && Input.GetKey(KeyCode.Space))
        {
            SetGameState(GameState.PLAY);
            Debug.Log("Game State PLAY");
        }

        if (GetPlayerType() != PlayerType.GOD && GetGameState() == GameState.PLAY)
        {
            SetGameState(GameState.PLAY);
            SetupUI();
        }
    }

    public GameState GetGameState()
    {
        if (PhotonNetwork.InRoom &&
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameState", out object gameStateValue))
        {
            return (GameState)(int)gameStateValue;
        }
        return state;
    }

    public PlayerType GetPlayerType()
    {
        return playerType;
    }

    public void SetGameState(GameState newState)
    {
        state = newState;
        UpdateGameStateProperty();
    }

    private void UpdateGameStateProperty()
    {
        if (PhotonNetwork.InRoom)
        {
            ExitGames.Client.Photon.Hashtable gameStateProps = new ExitGames.Client.Photon.Hashtable();
            gameStateProps["gameState"] = (int)state;
            PhotonNetwork.CurrentRoom.SetCustomProperties(gameStateProps);
        }
    }


    public void SetDecrementAliveCount()
    {
        aliveCount--;
        Debug.Log("aliveCountDecrement: " + aliveCount);

        UpdateAliveCountProperty();

        if (aliveCount <= 0)
        {
            SetGameState(GameState.END);
            winner = Winner.PANDA;
            Debug.Log("Panda Win");
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
    private void SetupUI()
    {
        canvasObject = GameObject.FindWithTag("Canvas");
        if (canvasObject == null)
        {
            Debug.LogError("canvas object not found!");
        }

        // カスタムプロパティから各プレイヤーの名前情報を取得
        string[] playerNames = GetAllPlayerNames();
        Debug.Log(playerNames);

        if (canvasObject != null)
        {
            MRKilledImagedAttach mrKilleImagedAttach = canvasObject.GetComponent<MRKilledImagedAttach>();
            if (mrKilleImagedAttach != null)
            {
                for (int i = 0; i < playerNames.Length; i++)
                {
                    if (playerNames[i] != null || playerNames[i] != "")
                    {
                        switch (i)
                        {
                            case 0:
                                MRKilledImagedAttach.SetFirstCharacter(playerNames[i]);
                                break;
                            case 1:
                                MRKilledImagedAttach.SetSecondCharacter(playerNames[i]);
                                break;
                            case 2:
                                MRKilledImagedAttach.SetThirdCharacter(playerNames[i]);
                                break;
                            default:
                                Debug.LogError("Invalid player index");
                                break;
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("Canvas に MRKilleImagedAttach コンポーネントが見つかりません！");
            }
        }
    }

    public void SetScoreList(string animalName, float score)
    {
        scoreList[animalName] = score;
    }

    // 勝利した動物の名前をリストに追加する
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

    public void SetLocalPlayerName(string playerName)
    {
        PhotonNetwork.NickName = playerName;

        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props["playerName"] = playerName;
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public string[] GetAllPlayerNames()
    {
        List<string> names = new List<string>();
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("playerName", out object name))
            {
                names.Add(name.ToString());
            }
            else
            {
                names.Add("Unknown");
            }
        }
        return names.ToArray();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("playerName"))
        {
            object newName = changedProps["playerName"];
            Debug.Log("Player " + targetPlayer.ActorNumber + " has updated their name to: " + newName);
        }
    }
}