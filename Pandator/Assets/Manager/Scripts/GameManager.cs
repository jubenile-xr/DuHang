using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;

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

    [Header("ゲームの状態はこっちで完全管理")] private GameState state = GameState.START;
    private bool hasPlayerNameCreated = false;

    [SerializeField] private PlayerType playerType = PlayerType.MR;

    private int aliveCount;
    private bool hasSendToGAS = false;
    private int previousLocalPlayerNameCount = 0;

    private enum Winner
    {
        NONE,
        SMALLANIMAL,
        PANDA,
    }

    [SerializeField] private Winner winner;
    private List<string> winnerAnimalNameList;

    // プレイヤー死亡状態を管理するローカル配列
    private bool[] playerDeadStatus;

    // ローカルで管理するプレイヤー名の配列
    private string[] localPlayerNames = new string[0];

    // ローカルで管理するスコア配列
    private float[] localPlayerScores = new float[0];

    // startSE
    [SerializeField] private SoundPlayer startSE;

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

        switch (Character.GetSelectedAnimal())
        {
            case Character.GameCharacters.BIRD:
                playerType = PlayerType.VR;
                break;
            case Character.GameCharacters.RABBIT:
                playerType = PlayerType.VR;
                break;
            case Character.GameCharacters.MOUSE:
                playerType = PlayerType.VR;
                break;
            case Character.GameCharacters.PANDA:
                playerType = PlayerType.MR;
                break;
            default:
                Debug.LogWarning("GOD PlayerType");
                playerType = PlayerType.GOD;
                break;
        }
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
            startSE?.Play();
        }

        if (GetGameState() == GameState.PLAY
            && !hasPlayerNameCreated)
        {
            // Initialize player dead status array first
            InitializePlayerDeadStatusArray();
            // Set alive count based on playerDeadStatus instead of player names length
            UpdateAliveCountFromDeadStatus();
            // localPlayerNamesに格納されている名前を出力する
            Debug.Log("Local Player Names: " + GetAllPlayerNames().Length);
            SetupUI();
            hasPlayerNameCreated = true;
        }

        if (GetGameState() == GameState.PLAY || GetGameState() == GameState.END)
        {
            SetupDeadUI();
        }

        HandleEndGame();

        // ここほんまにむずかった
        // GodScene内での処理，GodSceneに(clone)で出てくるGameObjectのScoreManagerのSetNameをする
        if (GetPlayerType() == PlayerType.GOD)
        {
            if (localPlayerNames.Length > previousLocalPlayerNameCount)
            {
                for (int i = previousLocalPlayerNameCount; i < localPlayerNames.Length; i++)
                {
                    string addedName = localPlayerNames[i];
                    GameObject[] masterPlayers = GameObject.FindGameObjectsWithTag("MasterPlayer");
                    if (masterPlayers != null && masterPlayers.Length > 0)
                    {
                        foreach (GameObject masterPlayer in masterPlayers)
                        {
                            bool match = false;
                            if (addedName.Contains("RABBIT") && masterPlayer.name.Contains("Rabbit"))
                            {
                                match = true;
                            }
                            else if (addedName.Contains("BIRD") && masterPlayer.name.Contains("Bird"))
                            {
                                match = true;
                            }
                            else if (addedName.Contains("MOUSE") && masterPlayer.name.Contains("Mouse"))
                            {
                                match = true;
                            }

                            if (match)
                            {
                                ScoreManager scoreManager = masterPlayer.GetComponent<ScoreManager>();
                                if (scoreManager != null)
                                {
                                    if (scoreManager.GetPlayerName() == null || !scoreManager.GetPlayerName().Equals(addedName))
                                    {
                                        scoreManager.SetPlayerName(addedName);
                                    }
                                }
                                else
                                {
                                    Debug.LogError("MasterPlayer に ScoreManager コンポーネントが存在しません。");
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("タグ 'MasterPlayer' の GameObject がシーンに存在しません。");
                    }
                }
                previousLocalPlayerNameCount = localPlayerNames.Length;
            }
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


    // New method to update aliveCount based on playerDeadStatus array
    public void UpdateAliveCountFromDeadStatus()
    {
        if (playerDeadStatus == null || playerDeadStatus.Length == 0)
            return;

        int alivePlayerCount = 0;

        // Count the number of players that are still alive (false in playerDeadStatus)
        for (int i = 0; i < playerDeadStatus.Length; i++)
        {
            if (!playerDeadStatus[i])
            {
                alivePlayerCount++;
            }
        }

        // Update the aliveCount with the new value
        aliveCount = alivePlayerCount;
        UpdateAliveCountProperty();

        // Check if all VR players are dead
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

    private void HandleEndGame()
    {
        if (GetGameState() == GameState.END && !hasSendToGAS )
        {
            if (playerType == PlayerType.GOD) //神が一気にデータ送る
            {
                SaveRankingData();
            }
            LoadResultScene();
        }
    }

    private void LoadResultScene()
    {
        if (aliveCount == 0)
        {
            SceneManager.LoadScene("ResultClearMRScene");
            // ShareData.SetWinner(winner.ToString());
        }
        else
        {
            SceneManager.LoadScene("ResultClearVRScene");
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
            bool isMine = Character.GetMyName().Equals(nm);
            Debug.Log($"SetupUI: Player {i}: {nm}, isMine: {isMine}");
            if (nm.Contains("BIRD") || nm.Contains("RABBIT") || nm.Contains("MOUSE"))
            {
                switch (i)
                {
                    case 0:
                        KilledImagedAttach.SetFirstCharacter(nm, isMine);
                        break;
                    case 1:
                        KilledImagedAttach.SetSecondCharacter(nm, isMine);
                        break;
                    case 2:
                        KilledImagedAttach.SetThirdCharacter(nm, isMine);
                        break;
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
        // Debug.LogWarning("PlayerDeadStatus: " + string.Join(", ", GetPlayerDeadStatus()));

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
            // プレイヤーの死亡状態を更新した後に生存者数を更新する
            UpdateAliveCountFromDeadStatus();
        }
        else
        {
            Debug.LogError($"Invalid index or not initialized: {index}");
        }
    }

    // プレイヤー自身が自分の死亡状態を設定するメソッド
    public void SetOwnPlayerDeadStatusTrue()
    {
        // 自分の名前を取得
        string myName = Character.GetMyName();
        // 全プレイヤー名を取得
        string[] allNames = GetAllPlayerNames();

        // 自分の名前に対応するインデックスを検索
        int myIndex = -1;
        for (int i = 0; i < allNames.Length; i++)
        {
            if (allNames[i] == myName)
            {
                myIndex = i;
                break;
            }
        }

        // 有効なインデックスが見つかった場合のみ死亡状態を変更
        if (myIndex >= 0 && playerDeadStatus != null && myIndex < playerDeadStatus.Length)
        {
            // すでに死亡状態の場合は何もしない
            if (playerDeadStatus[myIndex])
                return;

            playerDeadStatus[myIndex] = true;
            Debug.Log($"プレイヤー {myName} (インデックス: {myIndex}) が自身の死亡状態をtrueに設定しました");

            // 死亡状態をPhotonプロパティに同期
            UpdatePlayerDeadStatusProperty();

            // 生存者数を更新
            UpdateAliveCountFromDeadStatus();
        }
        else
        {
            Debug.LogError($"自身の死亡状態の設定に失敗: プレイヤー {myName} が見つからないか、インデックス {myIndex} が無効です");
        }
    }

    private void UpdatePlayerDeadStatusProperty()
    {
        if (PhotonNetwork.InRoom)
        {
            // 競合を防ぐため、一度現在のプロパティを取得してから設定する
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
            props["playerDeadStatus"] = playerDeadStatus;
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            Debug.Log("playerDeadStatus更新: " + string.Join(", ", playerDeadStatus.Select(b => b.ToString()).ToArray()));
        }
    }

    public void AddLocalPlayerName(string name)
    {
        var list = new List<string>(localPlayerNames);
        list.Add(name);
        localPlayerNames = list.ToArray();
        UpdatePlayerNameListProperty();

        if (GetPlayerType() == PlayerType.GOD)
        {
            // タグ「MasterPlayer」のオブジェクトを取得
            GameObject masterPlayer = GameObject.FindWithTag("MasterPlayer");
            if (masterPlayer != null && masterPlayer.name.Contains("Rabbit") ||
                masterPlayer.name.Contains("Bird") || masterPlayer.name.Contains("Mouse"))
            {
                ScoreManager scoreManager = masterPlayer.GetComponent<ScoreManager>();
                if (scoreManager != null)
                {
                    // 追加された名前を ScoreManager に設定
                    scoreManager.SetPlayerName(name);
                }
                else
                {
                    Debug.LogError("MasterPlayer オブジェクトに ScoreManager コンポーネントが存在しません。");
                }
            }
            else
            {
                Debug.LogError("タグ 'MasterPlayer' の GameObject がシーンに存在しません。");
            }
        }
    }

    void UpdatePlayerNameListProperty()
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
        // index が -1 なら、配列の末尾に追加するため、index を現在の長さに設定
        if (index == -1)
        {
            index = localPlayerScores.Length;
        }

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

    public void SaveRankingData()
    {
        // 送信するデータの数が一致しているか確認
        if (localPlayerNames.Length+1 != localPlayerScores.Length) return;
        string now = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
       for(var i = 0; i < localPlayerNames.Length; i++)
       {
            StartCoroutine(PostToGAS(localPlayerNames[i], (int)localPlayerScores[i],now));

            if (i == localPlayerNames.Length - 1)
            {
                StartCoroutine(PostToGAS("PANDA", (int)localPlayerScores[localPlayerNames.Length - 1],now));
            }
       }
       hasSendToGAS = true;
    }

    private string JudgeAnimal(string playerName)
    {
        if (playerName.Contains("BIRD"))
        {
            return "bird";
        }
        else if (playerName.Contains("RABBIT"))
        {
            return "rabbit";
        }
        else if (playerName.Contains("MOUSE"))
        {
            return "mouse";
        }
        else if(playerName.Contains("PANDA"))
        {
            return "panda";
        }
        else
        {
            return null;
        }
    }

    private IEnumerator PostToGAS(string name, int score,string dateTime)
    {
        string url = "https://script.google.com/macros/s/AKfycbyY_owGTG88vJx_2hieTMpRGVW6EkQVyT1qdTwGKu66OlLiFxzl3MSsaIjXgx-UUpE/exec";

        JsonData data = new JsonData
        {
            name = name,
            score = score,
            animal = JudgeAnimal(name),
            dateTime = dateTime,
            winner = winnerAnimalNameList.Contains("name")
        };

        if (data.animal == null)
        {
            Debug.LogError("Invalid animal type");
            yield break;
        }

        string jsonString = JsonUtility.ToJson(data);

        Debug.Log("jsonString: " + jsonString);

        UnityWebRequest webRequest = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonString);
        webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            if (webRequest.downloadHandler != null)
            {
                string text = webRequest.downloadHandler.text;
                Debug.Log(text);
            }
        }

        Debug.Log("SendToGAS: " + name + " " + score);
    }

    [System.Serializable]
    private class JsonData
    {
        public string name;
        public int score;
        public string animal;
        public string dateTime;
        public bool winner;
    }

    [System.Serializable]
    public class SceneTransform
    {
        public Vector3 position;
        public Quaternion rotation;
    }
}
