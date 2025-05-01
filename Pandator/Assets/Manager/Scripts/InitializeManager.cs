using System.Collections;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting.FullSerializer;
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

    private static string playerName;
    private bool hasPlayerNameCreated = false;
    private StateManager stateManager;
    private ScoreManager scoreManager;
    [SerializeField]private GameObject eventSystem;
    private bool isAnimationFinished = false;
    private bool isPlayerCreated = false;
    [Header("ローディング中の時間")] private float loadingTime;
    [SerializeField] private GameObject loadingScene;
    [SerializeField] private GameObject canvas;
    public GameObject MRUI;
    private GameObject debugCanvas;
    private GameObject spatialAnchor;
    private bool isSpatialAnchorCreated = false;
    private Transform playerSpawnPoint;
    [SerializeField]private SpatialAnchorLoader spatialAnchorLoader;

    // SpatialAnchorのロード状態を管理
    private bool isSpatialAnchorLoaded = false;
    // キーボード入力検出用フラグ
    private bool yKeyPressed = false;

    void Start()
    {
        loadingTime = 0;

        // デバッグモードの場合
        if (DebugManager.GetDebugMode())
        {
            // SpatialAnchorLoaderを探して取得
            spatialAnchor = Instantiate(Resources.Load<GameObject>("SpatialAnchor/prefab/spatialAnchor"),
                new Vector3(0f, 0f, 0f), Quaternion.identity);
            if (spatialAnchorLoader)
            {
                SetupDebugEnvironment();
            }
            else
            {
                Debug.LogError("SpatialAnchorLoader component is missing on the tagged object!");
                SetupDebugEnvironment();
            }

            return;
        }

        // デバッグモードでない場合は通常処理
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

    // private IEnumerator WaitForAnchorLoadAndSetupDebugForPanda()
    // {
    //     // spatialAnchorLoaderのisLoadedがtrueになるまで待機
    //     while (!spatialAnchorLoader.isLoaded)
    //     {
    //         yield return null;
    //     }

    //     isAnchorLoaded = true;
    //     Debug.Log("Anchor loaded successfully in debug mode for PANDA");
    //     SetupDebugEnvironment();
    // }

    private void SetupDebugEnvironment()
    {
        loadingScene.SetActive(false);
        Instantiate(Resources.Load<GameObject>("CameraRig/debugCamera"));
        Transform canvasTransform = spatialAnchor.transform
            .GetComponentsInChildren<Transform>()
            .FirstOrDefault(t => t.CompareTag("Canvas"));

        if (canvasTransform != null)
        {
            debugCanvas = canvasTransform.gameObject;
            Debug.Log("Canvasタグのオブジェクトが見つかりました: " + debugCanvas.name);
        }
        else
        {
            Debug.LogError("Canvasタグのオブジェクトが見つかりません。");
        }
        CanvasCameraSetter.Instance.SetCanvasCamera();
        CanvasCameraSetter.Instance.SetCanvasSortingLayer();
        debugCanvas.gameObject.SetActive(true);
    }

    void Update()
    {
        // デバッグモードの場合は以降の処理をスキップ
        if (DebugManager.GetDebugMode())
        {
            return;
        }

        if (!isAnimationFinished)
        {
            loadingTime += Time.deltaTime;
            if (loadingTime > 14f)
            {
                SetIsAnimationFinished(true);
            }
        }

        // PANDAプレイヤーの場合、キーボードの "Y" キーを検出 (MetaQuestのYボタンのシミュレート)
        if (character == GameCharacter.PANDA && spatialAnchor != null && !yKeyPressed && (Input.GetKeyDown(KeyCode.Y) || OVRInput.GetDown(OVRInput.Button.Two)))
        {
            yKeyPressed = true;
            AnchorManager anchorManager = spatialAnchor.GetComponent<AnchorManager>();
            if (anchorManager != null)
            {
                anchorManager.LoadAnchorFromExternal();
                StartCoroutine(CheckAnchorLoadedAndSetProperty(anchorManager));
            }
        }

        // ゲームマネージャーが存在し、アニメーションが終了し、プレイヤーが生成されていない場合
        if (gameManager && (isAnimationFinished || character == GameCharacter.GOD) && !isPlayerCreated && (isSpatialAnchorCreated || character == GameCharacter.GOD))
        {
            if (gameManager.GetPlayerType() != GameManager.PlayerType.GOD)
            {
                loadingScene.SetActive(false);
                eventSystem.SetActive(true);

                // playerSpawnのtransform.positionを取得
                Vector3 spawnPosition = Vector3.zero;
                Vector3 cameraPosition = Vector3.zero;

                if (playerSpawnPoint != null)
                {
                    spawnPosition = playerSpawnPoint.position;
                    cameraPosition = playerSpawnPoint.position;
                }
                else
                {
                    // デフォルトのスポーン位置
                    spawnPosition = new Vector3(0f, 3.0f, 0f);
                    cameraPosition = new Vector3(0f, 1.0f, 0f);
                }

                // プレイヤーキャラクターの生成およびカメラの生成
                // プレイヤーキャラクターやカメラの位置は全てspatialAnchorの子オブジェクトの"playerSpawn"の位置に配置されるようにしました
                // IMO: spatialAnchorの位置がちゃんと合っていないとまずいですが，全てMR側に合うようになっているのでMRとVRとのズレは解消できると思う
                switch (character)
                {
                    case GameCharacter.BIRD:
                        player = PhotonNetwork.Instantiate("Player/BirdPlayer", spawnPosition, Quaternion.identity);
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
                        player = PhotonNetwork.Instantiate("Player/RabbitPlayer", spawnPosition, Quaternion.identity);
                        stateManager = player.GetComponentInChildren<StateManager>();
                        scoreManager = player.GetComponentInChildren<ScoreManager>();
                        camera = Instantiate(Resources.Load<GameObject>("CameraRig/RabbitCameraRig"),
                            cameraPosition, Quaternion.identity);
                        canvas.SetActive(true);
                        break;
                    case GameCharacter.MOUSE:
                        player = PhotonNetwork.Instantiate("Player/MousePlayer", spawnPosition, Quaternion.identity);
                        stateManager = player.GetComponentInChildren<StateManager>();
                        scoreManager = player.GetComponentInChildren<ScoreManager>();
                        camera = Instantiate(Resources.Load<GameObject>("CameraRig/MouseCameraRig"),
                            cameraPosition, Quaternion.identity);
                        canvas.SetActive(true);
                        break;
                    case GameCharacter.PANDA:
                        player = PhotonNetwork.Instantiate("Player/PandaPlayer", spawnPosition, Quaternion.identity);
                        scoreManager = player.GetComponentInChildren<ScoreManager>();
                        camera = Instantiate(Resources.Load<GameObject>("CameraRig/PandaCameraRig"),
                        cameraPosition, Quaternion.identity);
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
    }

    // ルームに参加する処理
    public override void OnConnectedToMaster()
    {
        // デバッグモードの場合は以降の処理をスキップ
        if (DebugManager.GetDebugMode())
        {
            return;
        }

        // 固定ルーム "SampleRoomName" に参加
        PhotonNetwork.JoinRoom("SampleRoomName");
    }

    // ルーム参加に失敗した場合(通常，指定したルーム名が存在しなかった場合)の処理
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        // デバッグモードの場合は以降の処理をスキップ
        if (DebugManager.GetDebugMode())
        {
            return;
        }

        // ルーム参加に失敗した場合はルームを新規作成（最大8人まで）
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 8;
        PhotonNetwork.CreateRoom("SampleRoomName", roomOptions);
    }

    // ルーム参加に成功した時の処理
    public override void OnJoinedRoom()
    {
        // デバッグモードの場合は以降の処理をスキップ
        if (DebugManager.GetDebugMode())
        {
            return;
        }

        Debug.LogWarning("OnJoinedRoom");
        StartCoroutine(WaitForGameManager());
        if (GetGameCharacter() == GameCharacter.GOD)
        {
            PhotonNetwork.Instantiate("GameManager", new Vector3(0f, 0f, 0f), Quaternion.identity);
        }

        // PANDAの場合、spatialAnchorを生成
        if (GetGameCharacter() == GameCharacter.PANDA)
        {
            spatialAnchor = Instantiate(Resources.Load<GameObject>("SpatialAnchor/prefab/spatialAnchor"),
                new Vector3(0f, 0f, 0f), Quaternion.identity);
            SetIsSpatialAnchorCreated(true);
        }
        // PANDAでもGODでもない場合（小動物）、カスタムプロパティからSpatialAnchorの位置情報を取得
        else if (GetGameCharacter() != GameCharacter.GOD)
        {
            Vector3 anchorPosition;
            Quaternion anchorRotation;

            if (TryGetSpatialAnchorTransform(out anchorPosition, out anchorRotation))
            {
                Debug.Log("Using SpatialAnchor position from custom property: " + anchorPosition);
                spatialAnchor = Instantiate(Resources.Load<GameObject>("SpatialAnchor/prefab/spatialAnchor"),
                    anchorPosition, anchorRotation);
            }
            else
            {
                Debug.Log("No stored SpatialAnchor position found. Using default position.");
                spatialAnchor = Instantiate(Resources.Load<GameObject>("SpatialAnchor/prefab/spatialAnchor"),
                    new Vector3(0f, 0f, 0f), Quaternion.identity);
            }

            playerSpawnPoint = spatialAnchor
                .GetComponentsInChildren<Transform>()
                .FirstOrDefault(t => t.CompareTag("playerSpawn"));

            SetIsSpatialAnchorCreated(true);
            Debug.Log("SpatialAnchor and PlayerSpawn found.");
        }
    }

    private IEnumerator WaitForAnchorLoadAndCreateSpatialAnchorForPanda()
    {
        // spatialAnchorLoaderのisLoadedがtrueになるまで待機
        //while (!spatialAnchorLoader.isLoaded)
        //{
        //    yield return null;
        //}

        Debug.Log("Anchor loaded successfully for PANDA. Creating SpatialAnchor via PhotonNetwork.");
        spatialAnchor = PhotonNetwork.Instantiate("SpatialAnchor/prefab/spatialAnchor", new Vector3(0f, 0f, 0f), Quaternion.identity);
        SetIsSpatialAnchorCreated(true);
        yield return null;

    }

    private Transform FindPlayerSpawnPointInAnchor(GameObject anchor)
    {
        // 子オブジェクトから"playerSpawn"タグを持つオブジェクトを検索
        Transform[] allChildren = anchor.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.CompareTag("playerSpawn"))
            {
                return child;
            }
        }
        return null;
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

                // player が生成され、stateManager と scoreManager が取得できるまで待機
                if (GetGameCharacter() != GameCharacter.GOD && GetGameCharacter() != GameCharacter.PANDA)
                {
                    while (stateManager == null || scoreManager == null)
                    {
                        Debug.Log("Waiting for player instantiation...");
                        yield return null;
                    }
                }

                Debug.Log("状態確認: hasPlayerNameCreated=" + hasPlayerNameCreated +
                          ", stateManager=" + stateManager +
                          ", scoreManager=" + scoreManager +
                          ", GameManager.PlayerType=" + gameManager.GetPlayerType());

                if (!hasPlayerNameCreated && stateManager != null && scoreManager != null &&
                    GetGameCharacter() != GameCharacter.GOD && GetGameCharacter() != GameCharacter.PANDA)
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
        yield return null;
    }
}

    // OnDisconnectedという名前だがルーム切断時のみではなく接続失敗時にも実行する処理
    public override void OnDisconnected(DisconnectCause cause)
    {
        // デバッグモードの場合は以降の処理をスキップ
        if (DebugManager.GetDebugMode())
        {
            return;
        }

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
    private void SetIsSpatialAnchorCreated(bool isCreated)
    {
        isSpatialAnchorCreated = isCreated;
    }

    // アンカーがロードされたかチェックし、ロードされたらカスタムプロパティに位置情報を設定
    private IEnumerator CheckAnchorLoadedAndSetProperty(AnchorManager anchorManager)
    {
        // アンカーのロードを待つ（EventベースなのでちょっとしたDelay）
        yield return new WaitForSeconds(1.0f);

        Transform anchorTransform = anchorManager.GetAnchorTransform();
        if (anchorTransform != null)
        {
            Debug.Log("SpatialAnchor loaded successfully. Setting custom property.");
            SetSpatialAnchorTransformProperty(anchorTransform.position, anchorTransform.rotation);
            isSpatialAnchorLoaded = true;
        }
        else
        {
            Debug.LogWarning("SpatialAnchor failed to load or not created yet.");
        }
    }

    // SpatialAnchorの位置情報をカスタムプロパティとして設定
    private void SetSpatialAnchorTransformProperty(Vector3 position, Quaternion rotation)
    {
        if (PhotonNetwork.InRoom)
        {
            GameManager.SceneTransform anchorTransform = new GameManager.SceneTransform
            {
                position = position,
                rotation = rotation
            };

            string transformJson = JsonUtility.ToJson(anchorTransform);
            PhotonNetwork.CurrentRoom.SetCustomProperties(
                new ExitGames.Client.Photon.Hashtable { ["spatialAnchorTransform"] = transformJson }
            );

            Debug.Log("Set SpatialAnchor transform to custom property: " + transformJson);
        }
    }

    // カスタムプロパティからSpatialAnchorの位置情報を取得
    private bool TryGetSpatialAnchorTransform(out Vector3 position, out Quaternion rotation)
    {
        position = Vector3.zero;
        rotation = Quaternion.identity;

        if (PhotonNetwork.InRoom &&
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("spatialAnchorTransform", out object transformObj))
        {
            if (transformObj is string transformJson)
            {
                try
                {
                    GameManager.SceneTransform anchorTransform = JsonUtility.FromJson<GameManager.SceneTransform>(transformJson);
                    position = anchorTransform.position;
                    rotation = anchorTransform.rotation;
                    return true;
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Failed to parse spatialAnchorTransform: " + e.Message);
                }
            }
        }

        return false;
    }

    // カスタムプロパティの更新通知を受け取る
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        // spatialAnchorTransformプロパティが更新された場合
        if (propertiesThatChanged.ContainsKey("spatialAnchorTransform") &&
            GetGameCharacter() != GameCharacter.PANDA && GetGameCharacter() != GameCharacter.GOD &&
            !isSpatialAnchorCreated)
        {
            Vector3 anchorPosition;
            Quaternion anchorRotation;

            if (TryGetSpatialAnchorTransform(out anchorPosition, out anchorRotation) && !spatialAnchor)
            {
                Debug.Log("SpatialAnchor transform updated from room. Creating new anchor.");
                spatialAnchor = Instantiate(Resources.Load<GameObject>("SpatialAnchor/prefab/spatialAnchor"),
                    anchorPosition, anchorRotation);

                playerSpawnPoint = spatialAnchor
                    .GetComponentsInChildren<Transform>()
                    .FirstOrDefault(t => t.CompareTag("playerSpawn"));

                SetIsSpatialAnchorCreated(true);
            }
        }
    }
}
