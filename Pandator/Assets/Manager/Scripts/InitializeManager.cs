using System.Collections;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.EventSystems;

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
    private GameObject spatialAnchor;
    private GameObject playerSpawn;
    [SerializeField]private Canvas debugCanvas;

    private static string playerName;
    private bool hasPlayerNameCreated = false;
    private StateManager stateManager;
    private ScoreManager scoreManager;
    [SerializeField]private GameObject eventSystem;
    private bool isAnimationFinished = false;
    private bool isPlayerCreated = false;
    private bool isAnchorLoaded = false;
    [Header("ローディング中の時間")] private float loadingTime;
    [SerializeField] private GameObject loadingScene;
    [SerializeField] private GameObject canvas;
    public GameObject MRUI;
    private AnchorManager anchorManager;

    void Start()
    {
        Debug.Log("debug mode" + DebugManager.GetDebugMode());

        // Resourcesからspatialアンカーのprefabを読み込む
        LoadSpatialAnchorFromResources();

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

        if (!DebugManager.GetDebugMode())
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // Resourcesからspatialアンカーのprefabを読み込み、インスタンス化する
    private void LoadSpatialAnchorFromResources()
    {
        GameObject spatialAnchorPrefab = Resources.Load<GameObject>("SpatialAnchor/Prefab/spatialAnchor");
        if (spatialAnchorPrefab != null)
        {
            // デバッグモードの場合のみ直接Instantiate
            if (DebugManager.GetDebugMode())
            {
                spatialAnchor = Instantiate(spatialAnchorPrefab, Vector3.zero, Quaternion.identity);
                spatialAnchor.transform.parent = transform.parent;
                anchorManager = spatialAnchor.GetComponent<AnchorManager>();

                loadingScene.SetActive(false);
                spatialAnchor.SetActive(true);
                Instantiate(Resources.Load<GameObject>("CameraRig/debugCamera"));
                CanvasCameraSetter.Instance.SetCanvasCamera();
                CanvasCameraSetter.Instance.SetCanvasSortingLayer();
                debugCanvas.gameObject.SetActive(true);

                // PlayerSpawnポイントを取得
                SetupPlayerSpawn();
            }
            else if (PhotonNetwork.IsConnected)
            {
                // パンダ以外のキャラクターは、spatialAnchorが見つかるまで待機
                if (character != GameCharacter.PANDA)
                {
                    StartCoroutine(WaitForSpatialAnchor());
                }
            }
        }
        else
        {
            Debug.LogError("Resources/SpatialAnchor/Prefab/spatialAnchor.prefabが見つかりません。");
        }
    }

    // spatialAnchorが出現するまで待機するコルーチン
    private IEnumerator WaitForSpatialAnchor()
    {

        while (spatialAnchor == null)
        {
            // シーン内のspatialAnchorタグを持つオブジェクトを検索
            GameObject[] anchors = GameObject.FindGameObjectsWithTag("spatialAnchor");
            if (anchors.Length > 0)
            {
                spatialAnchor = anchors[0];
                anchorManager = spatialAnchor.GetComponent<AnchorManager>();
                Debug.Log("Found spatialAnchor in scene");

                // PlayerSpawnポイントを取得
                SetupPlayerSpawn();
                break;
            }

            yield return new WaitForSeconds(0.5f);
        }

        if (spatialAnchor == null)
        {
            Debug.LogError("spatialAnchorが見つかりませんでした。タイムアウト。");
        }
    }

    // PlayerSpawnの設定
    private void SetupPlayerSpawn()
    {
        if (spatialAnchor != null)
        {
            Transform spawnTransform = spatialAnchor.GetComponentsInChildren<Transform>()
                .FirstOrDefault(child => child.CompareTag("playerSpawn"));
            if (spawnTransform != null)
            {
                playerSpawn = spawnTransform.gameObject;
                Debug.Log("playerSpawn found: " + playerSpawn.name);
                isAnchorLoaded = true;
            }
            else
            {
                Debug.LogError("playerSpawnがspatialAnchorの子オブジェクト内に見つかりません。");
            }
        }
    }

    void Update()
    {
        if (!DebugManager.GetDebugMode())
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
                if (spatialAnchor == null)
                {
                    // アンカーがなければ再度検索
                    GameObject[] anchors = GameObject.FindGameObjectsWithTag("spatialAnchor");
                    if (anchors.Length > 0)
                    {
                        spatialAnchor = anchors[0];
                        anchorManager = spatialAnchor.GetComponent<AnchorManager>();
                        SetupPlayerSpawn();
                    }
                    else if (character == GameCharacter.PANDA && PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
                    {
                        // ルーム参加済みでパンダの場合、何らかの理由でアンカーが見つからなければ再作成
                        Debug.LogWarning("Panda: spatialAnchor not found, recreating");
                        spatialAnchor = PhotonNetwork.Instantiate("SpatialAnchor/Prefab/spatialAnchor", Vector3.zero, Quaternion.identity);
                        spatialAnchor.transform.parent = transform.parent;
                        anchorManager = spatialAnchor.GetComponent<AnchorManager>();
                        SetupPlayerSpawn();
                    }
                }
                else
                {
                    if (playerSpawn == null)
                    {
                        SetupPlayerSpawn();
                    }
                    else
                    {
                        if (gameManager.GetPlayerType() != GameManager.PlayerType.GOD)
                        {
                            // MRプレイヤー(パンダ)の場合は、アンカーを作成・保存
                            if (gameManager.GetPlayerType() == GameManager.PlayerType.MR)
                            {
                                // アンカーがまだ作られていない場合は作成
                                if (anchorManager != null && !anchorManager.isCreated)
                                {
                                    Debug.Log("MR Player: Creating new anchor");
                                    anchorManager.CreateAnchor();

                                    // 作成完了を少し待つ
                                    StartCoroutine(SaveAnchorAfterCreation(anchorManager));
                                }
                                else if (anchorManager != null && anchorManager.isCreated)
                                {
                                    Debug.Log("MR Player: Anchor already created, saving");
                                    // すでに作成されているが念のため保存
                                    anchorManager.OnSaveLocalButtonPressed();
                                    isAnchorLoaded = true;
                                }
                            }

                            // VRプレイヤーの場合はローカルに保存されているアンカーを読み込む
                            if (gameManager.GetPlayerType() == GameManager.PlayerType.VR)
                            {
                                if (anchorManager != null && !isAnchorLoaded)
                                {
                                    Debug.Log("VR Player: Trying to load local anchor");
                                    // アンカーをロード
                                    anchorManager.OnLoadLocalButtonPressed();
                                    spatialAnchor.SetActive(true);

                                    // ここでアンカーの読み込み完了を待つ
                                    StartCoroutine(CheckAnchorLoaded());
                                }
                            }

                            // アンカーの読み込みが完了してからプレイヤーとカメラを生成
                            if (isAnchorLoaded)
                            {
                                CreatePlayerAndCamera();

                                // ロード完了したらUIを表示
                                loadingScene.SetActive(false);
                                eventSystem.SetActive(true);

                                SetPlayerCreated(true);
                            }
                        }
                    }
                }
            }
        }
    }

    // アンカーの読み込み完了を確認するコルーチン
    private IEnumerator CheckAnchorLoaded()
    {
        float timeout = 30f; // 最大30秒待つ
        float elapsedTime = 0f;

        // タイムアウトまで待機
        while (!isAnchorLoaded && elapsedTime < timeout)
        {
            yield return new WaitForSeconds(0.5f);
            elapsedTime += 0.5f;
            Debug.Log($"Waiting for anchor to be loaded... {elapsedTime}s");


            if (!isAnchorLoaded)
            {
                Debug.LogError("Failed to load anchor within timeout period");
                // エラー処理 - 再試行するか、ユーザーに通知するなど
            }
        }
    }

    // プレイヤーとカメラを生成するメソッド
    private void CreatePlayerAndCamera()
    {
        // プレイヤーキャラクターの生成およびカメラの生成
        switch (character)
        {
            case GameCharacter.BIRD:
                player = PhotonNetwork.Instantiate("Player/BirdPlayer",
                    playerSpawn.transform.position,
                    Quaternion.identity);
                stateManager = player.GetComponentInChildren<StateManager>();
                scoreManager = player.GetComponentInChildren<ScoreManager>();
                GameObject eyePos = player.transform.Find("eyePos").gameObject;
                camera = Instantiate(Resources.Load<GameObject>("CameraRig/BirdCameraRig"),
                    eyePos.transform.position, Quaternion.identity);
                player.GetComponent<BirdMoveController>()
                    .SetCenterEyeAnchor(
                        camera.transform.Find("TrackingSpace/CenterEyeAnchor").transform);
                canvas.SetActive(true);
                break;
            case GameCharacter.RABBIT:
                player = PhotonNetwork.Instantiate("Player/RabbitPlayer",
                    playerSpawn.transform.position,
                    Quaternion.identity);
                stateManager = player.GetComponentInChildren<StateManager>();
                scoreManager = player.GetComponentInChildren<ScoreManager>();
                camera = Instantiate(Resources.Load<GameObject>("CameraRig/RabbitCameraRig"),
                    playerSpawn.transform.position, Quaternion.identity);
                canvas.SetActive(true);
                break;
            case GameCharacter.MOUSE:
                player = PhotonNetwork.Instantiate("Player/MousePlayer",
                    playerSpawn.transform.position,
                    Quaternion.identity);
                stateManager = player.GetComponentInChildren<StateManager>();
                scoreManager = player.GetComponentInChildren<ScoreManager>();
                camera = Instantiate(Resources.Load<GameObject>("CameraRig/MouseCameraRig"),
                    new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                canvas.SetActive(true);
                break;
            case GameCharacter.PANDA:
                player = PhotonNetwork.Instantiate("Player/PandaPlayer",
                    playerSpawn.transform.position,
                    Quaternion.identity);
                scoreManager = player.GetComponentInChildren<ScoreManager>();
                camera = Instantiate(Resources.Load<GameObject>("CameraRig/PandaCameraRig"),
                    playerSpawn.transform.position, Quaternion.identity);
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
            Debug.LogError(
                "CreatePhotonAvatar script is missing on the instantiated player object!");
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
                    Debug.LogError(
                        "MouseMove script is missing on the instantiated player object!");
                }

                mouseMoveScript.SetMouseOVRCameraRig();
                CanvasCameraSetter.Instance.SetCanvasCamera();
                CanvasCameraSetter.Instance.SetCanvasSortingLayer();
                break;
            case GameCharacter.RABBIT:
                RabbitMove rabbitMoveScript = player.GetComponentInChildren<RabbitMove>();
                if (rabbitMoveScript == null)
                {
                    Debug.LogError(
                        "RabbitMove script is missing on the instantiated player object!");
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
        Debug.LogWarning("OnJoinedRoom");
        StartCoroutine(WaitForGameManager());
        if (GetGameCharacter() == GameCharacter.GOD)
        {
            PhotonNetwork.Instantiate("GameManager", new Vector3(0f, 0f, 0f), Quaternion.identity);
        }

        // パンダ(MR)の場合はspatialAnchorをルーム参加後にインスタンス化
        if (GetGameCharacter() == GameCharacter.PANDA && !DebugManager.GetDebugMode())
        {
            Debug.Log("Panda: Instantiating spatialAnchor via PhotonNetwork");
            spatialAnchor = PhotonNetwork.Instantiate("SpatialAnchor/Prefab/spatialAnchor", Vector3.zero, Quaternion.identity);
            spatialAnchor.transform.parent = transform.parent;
            anchorManager = spatialAnchor.GetComponent<AnchorManager>();

            // パンダの場合はロード画面を非表示にしてアンカーを表示
            loadingScene.SetActive(false);
            spatialAnchor.SetActive(true);

            // PlayerSpawnポイントを取得
            SetupPlayerSpawn();
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

                // アンカーが読み込まれるまでは待機
                yield break;
            }
            else
            {
                Debug.LogError("GameManager object found, but component is missing.");
            }
        }
        yield return null;
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

    // MRプレイヤーがアンカーを作成した後、保存するコルーチン
    private IEnumerator SaveAnchorAfterCreation(AnchorManager anchorManager)
    {
        // アンカー作成完了を待つ
        yield return new WaitForSeconds(2.0f);

        if (anchorManager.isCreated)
        {
            Debug.Log("MR Player: Anchor creation successful, saving");
            anchorManager.OnSaveLocalButtonPressed();
            isAnchorLoaded = true;
        }
        else
        {
            Debug.LogError("MR Player: Failed to create anchor within timeout");
        }
    }
}
