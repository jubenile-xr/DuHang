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

    // ローカルで管理するプレイヤー名の配列（ローカルのPlayerTypeがVRの場合のみ利用）
    private string[] localPlayerNames = new string[0];

    private void Start()
    {
        state = GameState.START;
        scoreList = new Dictionary<string, float>();
        aliveCount = 0;
        winner = Winner.NONE;
        winnerAnimalNameList = new List<string>();

        // VRの場合のみ、ローカルプレイヤーの名前を設定（それ以外では更新しない）
        if (GetPlayerType() == PlayerType.VR)
        {
            SetLocalPlayerName(PhotonNetwork.NickName);
        }

        // ルームに入室中のプレイヤー名をローカル変数に設定（初期化）
        UpdateLocalPlayerNames();

        // 定期的にローカルの配列の内容をカスタムプロパティへ同期するコルーチンを開始
        StartCoroutine(SyncCustomPropertiesCoroutine());
    }

    private void Update()
    {
        if (canvasObject == null)
        {
            canvasObject = GameObject.FindWithTag("Canvas");
        }
        if (GetGameState() == GameState.START && GetPlayerType() == PlayerType.GOD && Input.GetKey(KeyCode.Space) && !hasPlayerNameCreated)
        {
            SetGameState(GameState.PLAY);
            Debug.Log("Game State PLAY");

            // デバッグ用：プレイヤー名リストの出力（ローカルがVRの場合のみ）
            string[] playerNames_forDebug = GetAllPlayerNames();
            Debug.Log("Player Names: " + string.Join(", ", playerNames_forDebug));
        }

        if (GetPlayerType() != PlayerType.GOD && GetGameState() == GameState.PLAY && !hasPlayerNameCreated)
        {
            SetGameState(GameState.PLAY);
            SetupUI();
            InitializePlayerDeadStatusArray();
            hasPlayerNameCreated = true;
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

    public void SetPlayerType(PlayerType type)
    {
        playerType = type;
        Debug.Log("SetPlayerType: " + playerType);
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
        // ローカルがVRの場合のみ、カスタムプロパティから各プレイヤーの名前情報を取得
        string[] playerNames = GetAllPlayerNames();
        Debug.Log("SetupUI: Player Names: " + string.Join(", ", playerNames));

        if (canvasObject != null)
        {
            MRKilledImagedAttach mrKilleImagedAttach = canvasObject.GetComponent<MRKilledImagedAttach>();
            if (mrKilleImagedAttach != null)
            {
                // 例として、各プレイヤー名が "BIRD", "RABBIT", "MOUSE" を含む場合に処理を行う
                for (int i = 0; i < playerNames.Length; i++)
                {
                    if (playerNames[i].Contains("BIRD") ||
                        playerNames[i].Contains("RABBIT") ||
                        playerNames[i].Contains("MOUSE"))
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
                Debug.LogError("Canvas に MRKilledImagedAttach コンポーネントが見つかりません！");
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
        if (propertiesThatChanged.ContainsKey("aliveCount"))
        {
            aliveCount = (int)propertiesThatChanged["aliveCount"];
            Debug.Log("aliveCount: " + aliveCount);
        }
        if (propertiesThatChanged.ContainsKey("gameState"))
        {
            int newStateInt = (int)propertiesThatChanged["gameState"];
            state = (GameState)newStateInt;
            Debug.Log("GameState updated to: " + state);
        }
        // カスタムプロパティから同期された死亡状態の更新
        if (propertiesThatChanged.ContainsKey("playerDeadStatus"))
        {
            playerDeadStatus = (bool[])propertiesThatChanged["playerDeadStatus"];
            Debug.Log("playerDeadStatus updated from custom properties. Array size = " + playerDeadStatus.Length);
        }
        // カスタムプロパティから同期されたプレイヤー名配列の更新（必要に応じて）
        if (propertiesThatChanged.ContainsKey("playerNameList"))
        {
            string[] names = propertiesThatChanged["playerNameList"] as string[];
            if (names != null)
            {
                localPlayerNames = names;
                Debug.Log("playerNameList updated from custom properties: " + string.Join(", ", localPlayerNames));
            }
        }
    }

    public void SetLocalPlayerName(string playerName)
    {
        PhotonNetwork.NickName = playerName;

        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props["playerName"] = playerName;
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        // ローカルがVRの場合のみ、プレイヤー名の更新を行う
        UpdateLocalPlayerNames();
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

    private void UpdateLocalPlayerNames()
    {
        List<string> namesList = new List<string>();
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.CustomProperties.TryGetValue("playerName", out object name))
            {
                namesList.Add(name.ToString());
            }
            else
            {
                namesList.Add("Unknown");
            }
        }
        localPlayerNames = namesList.ToArray();
        Debug.Log("LocalPlayerNames updated: " + string.Join(", ", localPlayerNames));
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        UpdateLocalPlayerNames();
    }

    // プレイヤーのカスタムプロパティが更新されたときにもローカル配列を更新
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        if (changedProps.ContainsKey("playerName"))
        {
            object newName = changedProps["playerName"];
            Debug.Log("Player " + targetPlayer.ActorNumber + " has updated their name to: " + newName);
            UpdateLocalPlayerNames();
        }
    }

    private void InitializePlayerDeadStatusArray()
    {
        string[] names = GetAllPlayerNames();
        playerDeadStatus = new bool[names.Length];
        // C# の bool の初期値は false なのでループで明示的にfalseを設定
        for (int i = 0; i < playerDeadStatus.Length; i++)
        {
            playerDeadStatus[i] = false;
        }
        Debug.Log("playerDeadStatus 初期化完了: サイズ = " + playerDeadStatus.Length);
        UpdatePlayerDeadStatusProperty();
    }

    private void UpdatePlayerDeadStatusProperty()
    {
        if (PhotonNetwork.InRoom)
        {
            ExitGames.Client.Photon.Hashtable statusProps = new ExitGames.Client.Photon.Hashtable();
            statusProps["playerDeadStatus"] = playerDeadStatus;
            PhotonNetwork.CurrentRoom.SetCustomProperties(statusProps);
        }
    }

    public bool[] GetPlayerDeadStatus()
    {
        return playerDeadStatus;
    }

    public void SetPlayerDeadStatusTrue(int index)
    {
        if (playerDeadStatus != null && index >= 0 && index < playerDeadStatus.Length)
        {
            playerDeadStatus[index] = true;
            Debug.Log("SetPlayerDeadStatusTrue: index = " + index + " が true に更新されました。");
            UpdatePlayerDeadStatusProperty();
        }
        else
        {
            Debug.LogError("SetPlayerDeadStatusTrue: インデックスが範囲外か、playerDeadStatus が初期化されていません。index = " + index);
        }
    }

    private IEnumerator SyncCustomPropertiesCoroutine()
    {
        // 1秒ごとに同期（必要に応じてウェイトを変更）
        while (true)
        {
            SyncPlayerNameList();
            SyncPlayerDeadStatus();
            yield return new WaitForSeconds(1f);
        }
    }
    
    private void SyncPlayerNameList()
    {
        if (PhotonNetwork.InRoom)
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
            props["playerNameList"] = localPlayerNames;
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }
    
    private void SyncPlayerDeadStatus()
    {
        if (PhotonNetwork.InRoom)
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
            props["playerDeadStatus"] = playerDeadStatus;
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }
}