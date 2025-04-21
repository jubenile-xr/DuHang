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
    private bool hasPlayerNameCreated = false;

    [SerializeField]
    private PlayerType playerType = PlayerType.MR;

    private int aliveCount;
    private bool hasSendToGAS = false;
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

    // ローカルで管理するスコア配列
    private float[] localPlayerScores = new float[0];

    private void Start()
    {
        state = GameState.START;
        aliveCount = -1;
        winner = Winner.NONE;
        winnerAnimalNameList = new List<string>();

        // Room に入室済みなら既存の playerNameList を取ってくる
        FetchPlayerNameListFromRoom();

        // Room に入室済みなら既存の playerScoreList を取ってくる
        FetchPlayerScoreListFromRoom();

        // 定期的にローカル → Room へ同期
        StartCoroutine(SyncCustomPropertiesCoroutine());
    }

    private void Update()
    {
        if (canvasObject == null)
        {
            canvasObject = GameObject.FindWithTag("Canvas");
        }

        if (GetGameState() == GameState.START
            && GetPlayerType() == PlayerType.GOD
            && Input.GetKey(KeyCode.Space)
            && !hasPlayerNameCreated)
        {
            SetGameState(GameState.PLAY);
        }

        if (GetPlayerType() != PlayerType.GOD
            && GetGameState() == GameState.PLAY
            && !hasPlayerNameCreated)
        {
            SetAliveCount(GetAllPlayerNames().Length);
            // localPlayerNamesに格納されている名前を出力する
            Debug.Log("Local Player Names: " + GetAllPlayerNames().Length);
            SetupUI();
            InitializePlayerDeadStatusArray();
            hasPlayerNameCreated = true;
        }

        if (GetGameState() == GameState.PLAY || GetGameState() == GameState.END)
        {
            SetupDeadUI();
        }

        if (aliveCount == 0 && GetGameState() == GameState.END && !hasSendToGAS)
        {
            PostToGAS();
            LoadResultScene();
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


    public void SetAliveCount(int count)
    {
        aliveCount = count;
        UpdateAliveCountProperty();
    }
    public void SetDecrementAliveCount()
    {
        aliveCount--;
        UpdateAliveCountProperty();
        if (aliveCount <= 0)
        {
            SetGameState(GameState.END);
            winner = Winner.PANDA;
        }
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
        var mrAttach = canvasObject.GetComponent<KilledImagedAttach>();
        if (mrAttach == null)
        {
            Debug.LogError("Canvas に KilledImagedAttach がない！");
            return;
        }

        for (int i = 0; i < names.Length; i++)
        {
            var nm = names[i];
            if (nm.Contains("BIRD") || nm.Contains("RABBIT") || nm.Contains("MOUSE"))
            {
                switch (i)
                {
                    case 0: KilledImagedAttach.SetFirstCharacter(nm); break;
                    case 1: KilledImagedAttach.SetSecondCharacter(nm); break;
                    case 2: KilledImagedAttach.SetThirdCharacter(nm); break;
                    default: Debug.LogError("Invalid player index"); break;
                }
            }
        }
    }

    private void SetupDeadUI()
    {
        if (canvasObject == null)
        {
            canvasObject = GameObject.FindWithTag("Canvas");
            if (canvasObject == null) return;
        }

        Debug.LogWarning("SetUpDeadUI");

        var mrAttach = canvasObject.GetComponent<KilledImagedAttach>();
        if (mrAttach == null)
        {
            Debug.LogError("Canvas に KilledImagedAttach がない！");
            return;
        }
        Debug.LogWarning("KilledImagedAttach found");
        // Photon のカスタムプロパティから名前に基づくインデックスを取得
        Debug.LogWarning("PlayerDeadStatus: " + string.Join(", ",GetPlayerDeadStatus()));

        for (int i = 0; i < GetPlayerDeadStatus().Length; i++)
        {
            if (GetPlayerDeadStatus()[i])
            {
                switch (i)
                {
                    case 0:
                        Debug.LogWarning("SetFirstPlayerDead");
                        mrAttach.SetFirstPlayerDead();
                        break;
                    case 1:
                        mrAttach.SetSecondPlayerDead();
                        break;
                    case 2:
                        mrAttach.SetThirdPlayerDead();
                        break;
                }
            }
        }
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
            && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("playerNameList", out object obj))
        {
            if (obj is string[] names)
            {
                localPlayerNames = names;
            }
            else if (obj is object[] objArray)
            {
                localPlayerNames = new string[objArray.Length];
                for (int i = 0; i < objArray.Length; i++)
                {
                    localPlayerNames[i] = objArray[i].ToString();
                }
            }
            Debug.Log("Fetched playerNameList from room: " + string.Join(", ", localPlayerNames));
        }
    }


    public string[] GetAllPlayerNames()
    {
        if (PhotonNetwork.InRoom
            && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("playerNameList", out object obj))
        {
            if (obj is string[] names)
            {
                return names;
            }
            else if (obj is object[] objArray)
            {
                string[] namesFromRoom = new string[objArray.Length];
                for (int i = 0; i < objArray.Length; i++)
                {
                    namesFromRoom[i] = objArray[i].ToString();
                }
                return namesFromRoom;
            }
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
            object propValue = props["playerNameList"];
            if (propValue is string[] names)
            {
                localPlayerNames = names;
            }
            else if (propValue is object[] objArray)
            {
                // object[] から string[] へ変換
                localPlayerNames = new string[objArray.Length];
                for (int i = 0; i < objArray.Length; i++)
                {
                    localPlayerNames[i] = objArray[i].ToString();
                }
            }
            Debug.Log("playerNameList updated from room: " + string.Join(", ", localPlayerNames));
        }
        if (props.ContainsKey("playerScoreList"))
        {
            localPlayerScores = props["playerScoreList"] as float[];
            Debug.Log("playerScoreList updated from room: " + string.Join(", ", localPlayerScores));
        }
    }

    private IEnumerator SyncCustomPropertiesCoroutine()
    {
        while (true)
        {
            UpdatePlayerNameListProperty();
            UpdatePlayerDeadStatusProperty();
            UpdatePlayerScoreListProperty();
            yield return new WaitForSeconds(1f);
        }
    }

    // Room から既存の playerScoreList を取得してローカルにセット
    private void FetchPlayerScoreListFromRoom()
    {
        if (PhotonNetwork.InRoom
            && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("playerScoreList", out object obj)
            && obj is float[] scores)
        {
            localPlayerScores = scores;
            Debug.Log("Fetched playerScoreList from room: " + string.Join(", ", localPlayerScores));
        }
    }

    // インデックス指定でセットして同期
    public void SetLocalPlayerScore(int index, float score)
    {
        // 必要ならサイズ拡張
        if (index >= localPlayerScores.Length)
        {
            Array.Resize(ref localPlayerScores, index + 1);
        }
        localPlayerScores[index] = score;
        UpdatePlayerScoreListProperty();
    }

    // カスタムプロパティに最新値を流し込む
    private void UpdatePlayerScoreListProperty()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(
                new ExitGames.Client.Photon.Hashtable { ["playerScoreList"] = localPlayerScores }
            );
        }
    }
    // TODO: このメソッドをGASに送信するAPIを叩く処理に変更して(dear MAOZ)
    public void PostToGAS()
    {
        // localPlayerScoresの内容をGASにPOSTする処理を実装
        Debug.Log("GASにPOSTしたよ！！！！");
        hasSendToGAS = true;
    }
}