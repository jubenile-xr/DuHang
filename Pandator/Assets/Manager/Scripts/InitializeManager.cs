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
    private bool isAnimationFinished = false;
    private bool isPlayerCreated = false;
    [Header("ローディング中の時間")] private float loadingTime;
    [SerializeField] private GameObject loadingScene;
    [SerializeField] private GameObject canvas;
    public GameObject MRUI;
    void Start()
    {
        loadingTime = 0;
        if (character != GameCharacter.GOD)
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
                default:
                    Debug.LogError("Invalid character selected.");
                    return; // 不正なキャラクターが選択された場合は処理を中断
            }
        }

        PhotonNetwork.ConnectUsingSettings();
    }

    void Update()
    {
        if (!isAnimationFinished)
        {
            loadingTime += Time.deltaTime;
            if (loadingTime > 14f)
            {
                SetIsAnimationFinished(true);
            }
        }

        if (gameManager && isAnimationFinished && !isPlayerCreated)
        {
            if (gameManager.GetPlayerType() != GameManager.PlayerType.GOD)
            {
                loadingScene.SetActive(false);
                // プレイヤーキャラクターの生成およびカメラの生成
                    switch (character)
                    {
                        case GameCharacter.BIRD:
                            player = PhotonNetwork.Instantiate("Player/BirdPlayer", new Vector3(0f, 1.0f, 0f),
                                Quaternion.identity);
                            GameObject eyePos = player.transform.Find("eyePos").gameObject;
                            camera = Instantiate(Resources.Load<GameObject>("CameraRig/BirdCameraRig"),
                                eyePos.transform.position, Quaternion.identity);
                            player.GetComponent<BirdMoveController>()
                                .SetCenterEyeAnchor(
                                    camera.transform.Find("TrackingSpace/CenterEyeAnchor").transform);
                            canvas.SetActive(true);
                            break;
                        case GameCharacter.RABBIT:
                            player = PhotonNetwork.Instantiate("Player/RabbitPlayer", new Vector3(0f, 2.0f, 0f),
                                Quaternion.identity);
                            camera = Instantiate(Resources.Load<GameObject>("CameraRig/RabbitCameraRig"),
                                new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                            canvas.SetActive(true);
                            break;
                        case GameCharacter.MOUSE:
                            player = PhotonNetwork.Instantiate("Player/MousePlayer", new Vector3(0f, 1.0f, 0f),
                                Quaternion.identity);
                            camera = Instantiate(Resources.Load<GameObject>("CameraRig/MouseCameraRig"),
                                new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                            canvas.SetActive(true);
                            break;
                        case GameCharacter.PANDA:
                            player = PhotonNetwork.Instantiate("Player/PandaPlayer", new Vector3(0f, 1.566f, 1.7f),
                                Quaternion.identity);
                            camera = Instantiate(Resources.Load<GameObject>("CameraRig/PandaCameraRig"),
                                new Vector3(0f, 0.5f, 0f), Quaternion.identity);
                            canvas.SetActive(true);
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
                    }

                    avatarScript.ExecuteCreatePhotonAvatar();

                    switch (character)
                    {
                        case GameCharacter.PANDA:
                            CanvasCameraSetter.Instance.SetCanvasCamera();
                            CanvasCameraSetter.Instance.SetCanvasSortingLayer();
                            MRUI.SetActive(true);
                            break;
                        case GameCharacter.MOUSE:
                            MouseMove mouseMoveScript = player.GetComponentInChildren<MouseMove>();
                            if (mouseMoveScript == null)
                            {
                                Debug.LogError("MouseMove script is missing on the instantiated player object!");
                            }

                            mouseMoveScript.SetMouseOVRCameraRig();
                            CanvasCameraSetter.Instance.SetCanvasCamera();
                            CanvasCameraSetter.Instance.SetCanvasSortingLayer();
                            break;
                        case GameCharacter.RABBIT:
                            RabbitMove rabbitMoveScript = player.GetComponentInChildren<RabbitMove>();
                            if (rabbitMoveScript == null)
                            {
                                Debug.LogError("RabbitMove script is missing on the instantiated player object!");
                            }

                            rabbitMoveScript.SetRabbitOVRCameraRig();
                            CanvasCameraSetter.Instance.SetCanvasCamera();
                            CanvasCameraSetter.Instance.SetCanvasSortingLayer();
                            break;
                        case GameCharacter.BIRD:
                            // BIRD用の処理があれば追加
                            CanvasCameraSetter.Instance.SetCanvasCamera();
                            CanvasCameraSetter.Instance.SetCanvasSortingLayer();
                            break;
                        default:
                            Debug.LogWarning("未処理のキャラクタータイプです: " + character);
                            break;
                    }

            }
            SetPlayerCreated(true);
        }

        GameObject masterPlayer = GameObject.FindWithTag("MasterPlayer");
        if (masterPlayer == null)
        {
            Debug.LogError("MasterPlayer object not found! Check the tag and its active status in the scene.");
        }
        else
        {
            masterPlayer.SetActive(true);
        }

        string formattedGameChar = GetFormattedGameCharacter();
        if (!masterPlayer.name.Contains(formattedGameChar))
        {
            Debug.Log("MasterPlayer name does not match the character type.");
        }

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
        StartCoroutine(WaitForGameManager());
        if (GetGameCharacter() == GameCharacter.GOD)
        {
            PhotonNetwork.Instantiate("GameManager", new Vector3(0f, 0f, 0f), Quaternion.identity);
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
                    if (GetGameCharacter() == GameCharacter.BIRD || GetGameCharacter() == GameCharacter.MOUSE ||
                        GetGameCharacter() == GameCharacter.RABBIT)
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

                    if (!hasPlayerNameCreated && stateManager != null && scoreManager != null &&
                        gameManager.GetPlayerType() == GameManager.PlayerType.VR)
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
        Debug.LogWarning("CreatePlayerName");
        int i = 1;
        string candidateName = "";

        while (true)
        {
            candidateName = character.ToString() + i.ToString();

            bool exists = false;
            foreach (string existingName in gameManager.GetAllPlayerNames())
            {
                if (existingName.Equals(candidateName))
                {
                    exists = true;
                    break;
                }
            }

            // candidateName が存在しなければ設定処理を実行
            if (!exists)
            {
                Debug.LogWarning("PlayerName Created! " + candidateName);
                gameManager.AddLocalPlayerName(candidateName);
                stateManager.SetPlayerName(candidateName);
                scoreManager.SetPlayerName(candidateName);
                break;
            }
            i++;
        }
    }

    public GameCharacter GetGameCharacter()
    {
        return character;
    }

    private void SetIsAnimationFinished(bool isFinished)
    {
        isAnimationFinished = isFinished;
    }
    private void SetPlayerCreated(bool isCreated)
    {
        isPlayerCreated = isCreated;
    }
}