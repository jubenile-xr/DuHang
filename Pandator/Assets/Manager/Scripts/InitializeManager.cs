using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class InitializeManager : MonoBehaviourPunCallbacks
{
    public GameObject PhotonFailureObject;

    public enum GameCharacter
    {
        BIRD,
        RABBIT,
        MOUSE,
        PANDA,
        GOD
    }

    [SerializeField] private GameCharacter character;
    private GameManager gameManager;
    private GameObject player;
    private GameObject camera;

    private static string playerName;
    private bool hasPlayerNameCreated = false;
    private StateManager stateManager;
    private ScoreManager scoreManager;
    private GameObject playerPrefab;
    private string gameCharString;

    void Start()
    {
        switch (Character.GetSelectedAnimal())
        {
            case Character.GameCharacters.BIRD:
                character = GameCharacter.BIRD;
                break;
            case Character.GameCharacters.RABBIT:
                character = GameCharacter.RABBIT;
                break;
            case Character.GameCharacters.MOUSE:
                character = GameCharacter.MOUSE;
                break;
            case Character.GameCharacters.PANDA:
                character = GameCharacter.PANDA;
                break;
            case Character.GameCharacters.GOD:
                character = GameCharacter.GOD;
                break;
            default:
                Debug.LogError("Invalid character selected.");
                return; // 不正なキャラクターが選択された場合は処理を中断
        }
        PhotonNetwork.ConnectUsingSettings();
    }

    void Update()
    {
        GameObject masterPlayer = GameObject.FindWithTag("MasterPlayer");
        if (masterPlayer == null) return;

        string formattedGameChar = GetFormattedGameCharacter();
        if (!masterPlayer.name.Contains(formattedGameChar)) return;

        if (stateManager == null)
        {
            stateManager = masterPlayer.GetComponentInChildren<StateManager>();
        }

        if (scoreManager == null)
        {
            scoreManager = masterPlayer.GetComponentInChildren<ScoreManager>();
        }
    }

    private string GetFormattedGameCharacter()
    {
        string gameCharString = GetGameCharacter().ToString();
        if (!string.IsNullOrEmpty(gameCharString))
        {
            // 1文字目を大文字、2文字目以降を小文字に変換
            return gameCharString.Substring(0, 1).ToUpper() + gameCharString.Substring(1).ToLower();
        }
        return gameCharString;
    }


    // ルームに参加する処理
    public override void OnConnectedToMaster()
    {
        // 固定ルーム "SampleRoomName" に参加
        PhotonNetwork.JoinRoom("SampleRoomName");
    }

    // ルーム参加に失敗した場合(通常，指定したルーム名が存在しなかった場合)の処理
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        // ルーム参加に失敗した場合はルームを新規作成（最大8人まで）
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 8;
        PhotonNetwork.CreateRoom("SampleRoomName", roomOptions);
    }

    // ルーム参加に成功した時の処理
    public override void OnJoinedRoom()
    {
        if (character != GameCharacter.PANDA)
        {
            StartCoroutine(WaitForGameManager());
        }

        // プレイヤーキャラクターの生成およびカメラの生成
        switch (character)
        {
            case GameCharacter.BIRD:
                player = PhotonNetwork.Instantiate("Player/BirdPlayer", new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                GameObject eyePos = player.transform.Find("eyePos").gameObject;
                camera = Instantiate(Resources.Load<GameObject>("CameraRig/BirdCameraRig"), eyePos.transform.position, Quaternion.identity);
                player.GetComponent<BirdMoveController>().SetCenterEyeAnchor(camera.transform.Find("TrackingSpace/CenterEyeAnchor").transform);
                Debug.Log("BirdJoin");
                break;
            case GameCharacter.RABBIT:
                player = PhotonNetwork.Instantiate("Player/RabbitPlayer", new Vector3(0f, 2.0f, 0f), Quaternion.identity);
                camera = Instantiate(Resources.Load<GameObject>("CameraRig/RabbitCameraRig"), new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                break;
            case GameCharacter.MOUSE:
                player = PhotonNetwork.Instantiate("Player/MousePlayer", new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                camera = Instantiate(Resources.Load<GameObject>("CameraRig/MouseCameraRig"), new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                break;
            case GameCharacter.PANDA:
                player = PhotonNetwork.Instantiate("Player/PandaPlayer", new Vector3(0f, 1.566f, 1.7f), Quaternion.identity);
                camera = Instantiate(Resources.Load<GameObject>("CameraRig/PandaCameraRig"), new Vector3(0f, 0.5f, 0f), Quaternion.identity);
                break;
            case GameCharacter.GOD:
                PhotonNetwork.Instantiate("GameManager", new Vector3(0f, 0f, 0f), Quaternion.identity);
                break;
        }

        //カメラ生成の確認
        if (camera == null)
        {
            Debug.LogError("CameraRig is missing in the inspector.");
        }
        //カメラの親子関係を設定
        camera.transform.SetParent(player.transform);

        //CreatePhotonAvatarのOnCreate()を実行
        CreatePhotonAvatar avatarScript = player.GetComponent<CreatePhotonAvatar>();
        if (avatarScript == null)
        {
            Debug.LogError("CreatePhotonAvatar script is missing on the instantiated player object!");
            return;
        }
        avatarScript.ExecuteCreatePhotonAvatar();

        switch (character)
        {
            case GameCharacter.PANDA:
                CanvasCameraSetter.Instance.SetCanvasCamera();
                CanvasCameraSetter.Instance.SetCanvasSortingLayer();
                break;
            case GameCharacter.MOUSE:
                MouseMove mouseMoveScript = player.GetComponentInChildren<MouseMove>();
                if (mouseMoveScript == null)
                {
                    Debug.LogError("MouseMove script is missing on the instantiated player object!");
                    return;
                }
                mouseMoveScript.SetMouseOVRCameraRig();
                break;
            case GameCharacter.RABBIT:
                RabbitMove rabbitMoveScript = player.GetComponentInChildren<RabbitMove>();
                if (rabbitMoveScript == null)
                {
                    Debug.LogError("RabbitMove script is missing on the instantiated player object!");
                    return;
                }
                rabbitMoveScript.SetRabbitOVRCameraRig();
                break;
            case GameCharacter.BIRD:
                // BIRD用の処理があれば追加
                break;
            default:
                Debug.LogWarning("未処理のキャラクタータイプです: " + character);
                break;
        }
    }

    //コルーチンでOnJoinedRoom内でリトライ機構ができるように
    //GameManagerの取得とaliveCountのインクリメントを行う
    private IEnumerator WaitForGameManager()
    {
        while (!gameManager)
        {
            GameObject gmObj = GameObject.FindWithTag("GameManager");
            if (gmObj)
            {
                gameManager = gmObj.GetComponent<GameManager>();
                if (gameManager)
                {
                    Debug.Log("GameManager found.");
                    if (GetGameCharacter() == GameCharacter.BIRD || GetGameCharacter() == GameCharacter.MOUSE || GetGameCharacter() == GameCharacter.RABBIT)
                    {
                        gameManager.SetPlayerType(GameManager.PlayerType.VR);
                    }
                    else if (GetGameCharacter() == GameCharacter.PANDA)
                    {
                        gameManager.SetPlayerType(GameManager.PlayerType.MR);
                    }
                    else if (GetGameCharacter() == GameCharacter.GOD)
                    {
                        gameManager.SetPlayerType(GameManager.PlayerType.GOD);
                    }
                    else
                    {
                        Debug.LogError("Unknown player type");
                    }

                    if (gameManager.GetPlayerType() != GameManager.PlayerType.VR)
                    {
                        hasPlayerNameCreated = true;
                    }

                    if (!hasPlayerNameCreated && stateManager != null && scoreManager != null && gameManager.GetPlayerType() == GameManager.PlayerType.VR)
                    {
                        CreatePlayerName();
                        hasPlayerNameCreated = true;
                    }
                    yield break;
                }
                else
                {
                    Debug.LogError("GameManager object found, but component is missing.");
                }
            }
            else
            {
                // Debug.Log("GameManager object not found. Waiting...");
            }
            yield return null; // 1フレーム待機
        }
    }

    // OnDisconnectedという名前だがルーム切断時のみではなく接続失敗時にも実行する処理
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.LogError("Disconnected from Photon: " + cause.ToString());

        // 接続に失敗した場合、PhotonFailureObject(本記事ではCube) を表示する
        if (PhotonFailureObject != null)
        {
            // ローカルにオブジェクトを Instantiate する例（PhotonNetwork.Instantiate は使用できないため）
            Instantiate(PhotonFailureObject, new Vector3(0f, 0f, 0f), Quaternion.identity);
        }
        else
        {
            Debug.LogError("PhotonFailureObject is not set in the inspector.");
        }
    }

 private void CreatePlayerName()
    {
        int i = 1;
        string candidateName = character.ToString() + i.ToString();

        // PhotonNetwork.PlayerList を参照して、すでに使われている名前がないかチェック
        while (IsPlayerNameTaken(candidateName))
        {
            i++;
            candidateName = character.ToString() + i.ToString();
        }

        // ユニークなプレイヤー名が決定
        playerName = candidateName;

        // ローカルプレイヤーの名前を Photon のカスタムプロパティに設定
        gameManager.AddLocalPlayerName(playerName);

        stateManager.SetPlayerName(playerName);
        scoreManager.SetPlayerName(playerName);
    }

    private bool IsPlayerNameTaken(string candidateName)
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("playerName", out object existingName))
            {
                if (candidateName.Equals(existingName.ToString()))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public GameCharacter GetGameCharacter()
    {
        return character;
    }
}