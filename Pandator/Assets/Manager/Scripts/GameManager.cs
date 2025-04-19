using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    // プレイヤー死亡状態を管理するローカル配列
    private bool[] playerDeadStatus;

    // ローカルで管理するプレイヤー名の配列
    private string[] localPlayerNames = new string[0];

    private void Start()
    {
        state = GameState.START;
        scoreList = new Dictionary<string, float>();
        aliveCount = 0;
        winner = Winner.NONE;
        winnerAnimalNameList = new List<string>();

        // Room に入室済みなら既存の playerNameList を取ってくる
        FetchPlayerNameListFromRoom();

        // 定期的にローカル → Room へ同期
        StartCoroutine(SyncCustomPropertiesCoroutine());
    }

    private void Update()
    {
        if (canvasObject == null)
            canvasObject = GameObject.FindWithTag("Canvas");

        if (GetGameState() == GameState.START
            && GetPlayerType() == PlayerType.GOD
            && Input.GetKey(KeyCode.Space)
            && !hasPlayerNameCreated)
        {
            SetGameState(GameState.PLAY);
            Debug.Log("Game State PLAY");

            Debug.Log("Player Names: " + string.Join(", ", GetAllPlayerNames()));
        }

        if (GetPlayerType() != PlayerType.GOD
            && GetGameState() == GameState.PLAY
            && !hasPlayerNameCreated)
        {
            SetupUI();
            InitializePlayerDeadStatusArray();
            hasPlayerNameCreated = true;
        }
    }


    public GameState GetGameState()
    {
        if (PhotonNetwork.InRoom
            && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameState", out object gs))
        {
            return (GameState)(int)gs;
        }
        return state;
    }

    public void SetGameState(GameState newState)
    {
        state = newState;
        if (PhotonNetwork.InRoom)
        {
            var props = new ExitGames.Client.Photon.Hashtable { ["gameState"] = (int)state };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }

    public PlayerType GetPlayerType() => playerType;
    public void SetPlayerType(PlayerType type) => playerType = type;


    public void SetDecrementAliveCount()
    {
        aliveCount--;
        UpdateAliveCountProperty();
        if (aliveCount <= 0)
        {
            SetGameState(GameState.END);
            winner = Winner.PANDA;
            if (aliveCount == 0)
                LoadResultScene();
        }
    }

    public void SetIncrementAliveCount()
    {
        aliveCount++;
        UpdateAliveCountProperty();
    }

    private void UpdateAliveCountProperty()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(
                new ExitGames.Client.Photon.Hashtable { ["aliveCount"] = aliveCount }
            );
        }
    }

    private void LoadResultScene()
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

    private void SetupUI()
    {
        var names = GetAllPlayerNames();
        Debug.Log("SetupUI: Player Names: " + string.Join(", ", names));

        if (canvasObject == null) return;
        var mrAttach = canvasObject.GetComponent<MRKilledImagedAttach>();
        if (mrAttach == null)
        {
            Debug.LogError("Canvas に MRKilledImagedAttach がない！");
            return;
        }

        for (int i = 0; i < names.Length; i++)
        {
            var nm = names[i];
            if (nm.Contains("BIRD") || nm.Contains("RABBIT") || nm.Contains("MOUSE"))
            {
                switch (i)
                {
                    case 0: MRKilledImagedAttach.SetFirstCharacter(nm); break;
                    case 1: MRKilledImagedAttach.SetSecondCharacter(nm); break;
                    case 2: MRKilledImagedAttach.SetThirdCharacter(nm); break;
                    default: Debug.LogError("Invalid player index"); break;
                }
            }
        }
    }


    public void SetScoreList(string animalName, float score)
    {
        scoreList[animalName] = score;
    }

    public void appendWinnerAnimalNameList(string animalName)
    {
        winnerAnimalNameList.Add(animalName);
    }


    private void InitializePlayerDeadStatusArray()
    {
        var names = GetAllPlayerNames();
        playerDeadStatus = new bool[names.Length];
        for (int i = 0; i < playerDeadStatus.Length; i++)
            playerDeadStatus[i] = false;
        UpdatePlayerDeadStatusProperty();
    }

    public bool[] GetPlayerDeadStatus() => playerDeadStatus;

    public void SetPlayerDeadStatusTrue(int index)
    {
        if (playerDeadStatus != null
            && index >= 0
            && index < playerDeadStatus.Length)
        {
            playerDeadStatus[index] = true;
            UpdatePlayerDeadStatusProperty();
        }
        else
        {
            Debug.LogError($"Invalid index or not initialized: {index}");
        }
    }

    private void UpdatePlayerDeadStatusProperty()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(
                new ExitGames.Client.Photon.Hashtable { ["playerDeadStatus"] = playerDeadStatus }
            );
        }
    }


    public void SetLocalPlayerNames(string[] names)
    {
        localPlayerNames = names;
        UpdatePlayerNameListProperty();
    }

    public void AddLocalPlayerName(string name)
    {
        var list = new List<string>(localPlayerNames);
        if (!list.Contains(name))
        {
            list.Add(name);
            localPlayerNames = list.ToArray();
            UpdatePlayerNameListProperty();
        }
    }

    private void UpdatePlayerNameListProperty()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(
                new ExitGames.Client.Photon.Hashtable { ["playerNameList"] = localPlayerNames }
            );
        }
    }

    private void FetchPlayerNameListFromRoom()
    {
        if (PhotonNetwork.InRoom
            && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("playerNameList", out object obj)
            && obj is string[] names)
        {
            localPlayerNames = names;
            Debug.Log("Fetched playerNameList from room: " + string.Join(", ", localPlayerNames));
        }
    }


    public string[] GetAllPlayerNames()
    {
        if (PhotonNetwork.InRoom
            && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("playerNameList", out object obj)
            && obj is string[] namesFromRoom)
        {
            return namesFromRoom;
        }
        return localPlayerNames;
    }
    

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable props)
    {
        if (props.ContainsKey("aliveCount"))
        {
            aliveCount = (int)props["aliveCount"];
            Debug.Log("aliveCount updated: " + aliveCount);
        }
        if (props.ContainsKey("gameState"))
        {
            state = (GameState)(int)props["gameState"];
            Debug.Log("GameState updated: " + state);
        }
        if (props.ContainsKey("playerDeadStatus"))
        {
            playerDeadStatus = props["playerDeadStatus"] as bool[];
            Debug.Log("playerDeadStatus updated, len=" + (playerDeadStatus?.Length ?? 0));
        }
        if (props.ContainsKey("playerNameList"))
        {
            localPlayerNames = props["playerNameList"] as string[];
            Debug.Log("playerNameList updated from room: " + string.Join(", ", localPlayerNames));
        }
    }

    private IEnumerator SyncCustomPropertiesCoroutine()
    {
        while (true)
        {
            // Fallback in case an immediate SetCustomProperties was missed
            UpdatePlayerNameListProperty();
            UpdatePlayerDeadStatusProperty();
            yield return new WaitForSeconds(1f);
        }
    }
}