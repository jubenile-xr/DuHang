using System.Collections;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.EventSystems;
using OVR;

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
    private bool localIsSpatialAnchorCreated = false; // ローカルでの状態管理用
    private Transform playerSpawnPoint;
    [SerializeField]private SpatialAnchorLoader spatialAnchorLoader;
    public GameObject roomWall1;
    public GameObject roomWall2;
    private Vector3 spawnPosition;
    private Vector3 cameraPositionAnimal;
    private Vector3 cameraPositionPanda;

    // VR空間のスケール（MR空間との比率）
    private const float VRWorldScale = 1.0f; // この値は実際の環境に合わせて調整する必要があります

    [SerializeField] private float anchorLoadRetryInterval = 1.0f; // リトライ間隔
    [SerializeField] private float anchorLoadTimeout = 30.0f; // タイムアウト時間
    private bool isAnchorLoadSuccessful = false;
    private bool isAnchorLoadAttempted = false; // アンカーのロード試行を追跡

    // ローカルでのspatialAnchorの座標管理用
    private Vector3 localAnchorPosition;
    private Quaternion localAnchorRotation;
    private bool hasLocalAnchorTransform = false;

    [SerializeField] private SoundPlayer bgm;

    private bool isPlayerRigidbodyDestoryed = false;
    void Start()
    {
        loadingTime = 0;

        // デバッグモードの場合
        if (DebugManager.GetDebugMode())
        {
            // SpatialAnchorLoaderを探して取得
            spatialAnchor = Instantiate(Resources.Load<GameObject>("SpatialAnchor/prefab/spatialAnchor"),
                new Vector3(0f, 0f, 0f), Quaternion.identity);
            // SpatialAnchorの子オブジェクトを検索してroom_completeの子供のroomとroom.001を非アクティブに設定
            Transform roomCompleteTransform = spatialAnchor.transform.Find("room_complete004");
            if (roomCompleteTransform != null)
            {
                Transform room = roomCompleteTransform.Find("room.002");
                if (room != null)
                {
                    room.gameObject.SetActive(false);
                }

                Transform room001 = roomCompleteTransform.Find("room.003");
                if (room001 != null)
                {
                    room001.gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.LogWarning("room_complete object not found under SpatialAnchor.");
            }

            //debugCanvasを表示
            Transform debugCanvasTransform = spatialAnchor.transform.Find("DebugCanvas");
            if (debugCanvasTransform != null)
            {
                debugCanvasTransform.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("debugCanvas object not found.");
            }

            if (spatialAnchorLoader)
            {
                SetupDebugEnvironment();
            }
            else
            {
                Debug.LogError("SpatialAnchorLoader componentis missing on the tagged object!");
            }

            //SetupDebugEnvironment();
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

    private void SetupDebugEnvironment()
    {
        loadingScene.SetActive(false);
        Instantiate(Resources.Load<GameObject>("CameraRig/debugCamera"));
        Transform canvasTransform = spatialAnchor.transform
            .GetComponentsInChildren<Transform>()
            .FirstOrDefault(t => t.CompareTag("DebugCanvas"));

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

        if (gameManager != null && gameManager.GetGameState() == GameManager.GameState.PLAY &&
            !isPlayerRigidbodyDestoryed)
        {
            GameObject[] masterPlayers = GameObject.FindGameObjectsWithTag("MasterPlayer");
            foreach (GameObject masterPlayer in masterPlayers)
            {
                Rigidbody[] childRigidbodies = masterPlayer.GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody rb in childRigidbodies)
                {
                    Destroy(rb);
                }
            }
            isPlayerRigidbodyDestoryed = true;
        }

        // PANDAプレイヤーの場合のアンカーロード処理
        if (character == GameCharacter.PANDA && spatialAnchor != null && !isAnchorLoadAttempted)
        {
            AnchorManager anchorManager = spatialAnchor.GetComponent<AnchorManager>();
            if (anchorManager != null)
            {
                LoadAnchorOnce(anchorManager);
            }
            else
            {
                // アンカーマネージャーが見つからない場合も続行
                isAnchorLoadAttempted = true;
                SetIsSpatialAnchorCreated(true);
            }
        }

        // VRプレイヤーとGODの場合のSpatialAnchor生成チェック
        if ((character == GameCharacter.BIRD || character == GameCharacter.RABBIT ||
             character == GameCharacter.MOUSE || character == GameCharacter.GOD) &&
            !spatialAnchor && PhotonNetwork.InRoom)
        {
            Vector3 anchorPosition;
            Quaternion anchorRotation;
            if (TryGetSpatialAnchorTransform(out anchorPosition, out anchorRotation))
            {
                CreateSpatialAnchor(anchorPosition, anchorRotation);
                SetIsSpatialAnchorCreated(true);
            }
        }


        // ゲームマネージャーが存在し、アニメーションが終了し、プレイヤーが生成されていない場合
        if (gameManager != null && (isAnimationFinished || character == GameCharacter.GOD) && !isPlayerCreated && isSpatialAnchorCreated)
        {
            if (gameManager.GetPlayerType() != GameManager.PlayerType.GOD)
            {
                loadingScene.SetActive(false);
                eventSystem.SetActive(true);
                bgm?.Play();

                // playerSpawnのtransform.positionを取得
                spawnPosition = Vector3.zero;
                cameraPositionAnimal = Vector3.zero;
                cameraPositionPanda = Vector3.zero;

                if (playerSpawnPoint != null)
                {
                    spawnPosition = playerSpawnPoint.position;
                    cameraPositionAnimal = playerSpawnPoint.position;
                    cameraPositionPanda = playerSpawnPoint.position;
                }
                else
                {
                    // デフォルトのスポーン位置
                    spawnPosition = new Vector3(localAnchorPosition.x, localAnchorPosition.y + 2, localAnchorPosition.z);
                    cameraPositionAnimal = new Vector3(0f, 1.0f, 0f);
                    cameraPositionPanda = new Vector3(0f, 0f, 0f);
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
                            cameraPositionAnimal, Quaternion.identity);
                        canvas.SetActive(true);
                        break;
                    case GameCharacter.MOUSE:
                        player = PhotonNetwork.Instantiate("Player/MousePlayer", spawnPosition, Quaternion.identity);
                        stateManager = player.GetComponentInChildren<StateManager>();
                        scoreManager = player.GetComponentInChildren<ScoreManager>();
                        camera = Instantiate(Resources.Load<GameObject>("CameraRig/MouseCameraRig"),
                            cameraPositionAnimal, Quaternion.identity);
                        canvas.SetActive(true);
                        break;
                    case GameCharacter.PANDA:
                        player = PhotonNetwork.Instantiate("Player/PandaPlayer", spawnPosition, Quaternion.identity);
                        scoreManager = player.GetComponentInChildren<ScoreManager>();
                        camera = Instantiate(Resources.Load<GameObject>("CameraRig/PandaCameraRig"),
                        cameraPositionPanda, Quaternion.identity);
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
            // PANDAの場合はisSpatialAnchorCreatedのチェックをスキップしてspatialAnchorを生成
            spatialAnchor = Instantiate(Resources.Load<GameObject>("SpatialAnchor/prefab/spatialAnchor"),
                new Vector3(0f, 0f, 0f), Quaternion.identity);
            SetIsSpatialAnchorCreated(true);
            // SpatialAnchorの子オブジェクトを検索してroom_completeの子供のroomとroom.001を非アクティブに設定
            Transform roomCompleteTransform = spatialAnchor.transform.Find("room_complete004");
            roomCompleteTransform.gameObject.SetActive(false);
        }


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
                Character.SetMyName(candidateName);
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
        localIsSpatialAnchorCreated = isCreated; // ローカルの状態も更新

        if (PhotonNetwork.InRoom && character == GameCharacter.PANDA)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                ["isSpatialAnchorCreated"] = isCreated
            });
        }
    }

    private void LoadAnchorOnce(AnchorManager anchorManager)
    {
        if (!isAnchorLoadAttempted)
        {
            isAnchorLoadAttempted = true;
            Debug.Log("Attempting to load anchor (First and only attempt)...");
            anchorManager.LoadAnchorFromExternal();
            StartCoroutine(WaitForAnchorLoad(anchorManager));
        }
    }

    private IEnumerator WaitForAnchorLoad(AnchorManager anchorManager)
    {
        float elapsedTime = 0f;
        float timeout = 5.0f; // 5秒のタイムアウト

        while (elapsedTime < timeout && !anchorManager.isCreated)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (anchorManager.isCreated)
        {
            Transform anchorTransform = anchorManager.GetAnchorTransform();
            if (anchorTransform != null)
            {
                Debug.Log($"Anchor loaded successfully! Position: {anchorTransform.position}");
                isAnchorLoadSuccessful = true;
                StartCoroutine(CheckAnchorLoadedAndSetProperty(anchorManager));
            }
        }
        else
        {
            Debug.LogWarning("Failed to load anchor within timeout period. Continuing without anchor.");
        }

        // ロードの成功/失敗に関わらず、次の処理に進むためのフラグを設定
        isAnchorLoadAttempted = true;
        SetIsSpatialAnchorCreated(true);
    }

    private IEnumerator CheckAnchorLoadedAndSetProperty(AnchorManager anchorManager)
    {
        Transform anchorTransform = anchorManager.GetAnchorTransform();
        if (anchorTransform != null)
        {
            Debug.Log("SpatialAnchor loaded successfully. Setting custom property.");

            // OVRSceneManagerから座標系の変換行列を取得
            OVRSceneManager sceneManager = FindObjectOfType<OVRSceneManager>();
            if (sceneManager != null)
            {
                // OVRSceneManagerの座標系でのSpatialAnchorの位置を取得
                Matrix4x4 sceneToWorldMatrix = sceneManager.transform.localToWorldMatrix;
                Vector3 sceneSpacePosition = sceneToWorldMatrix.MultiplyPoint3x4(anchorTransform.position);
                Quaternion sceneSpaceRotation = sceneManager.transform.rotation * anchorTransform.rotation;

                SetSpatialAnchorTransformProperty(sceneSpacePosition, sceneSpaceRotation);
                SetIsSpatialAnchorCreated(true);
            }
            else
            {
                Debug.LogError("OVRSceneManager not found in the scene!");
            }
        }
        else
        {
            Debug.LogWarning("SpatialAnchor failed to load or not created yet.");
        }
        yield return null;
    }

    // spatialAnchorを生成する共通メソッド
    private void CreateSpatialAnchor(Vector3 position, Quaternion rotation)
    {
        // isSpatialAnchorCreatedがtrueかつspatialAnchorが未生成の場合のみ実行
        if (!spatialAnchor && isSpatialAnchorCreated)
        {
            Debug.Log($"Creating SpatialAnchor at position: {position}, rotation: {rotation}");
            spatialAnchor = Instantiate(Resources.Load<GameObject>("SpatialAnchor/prefab/spatialAnchor"),
                position, rotation);

            // ローカルの座標を保存
            localAnchorPosition = position;
            localAnchorRotation = rotation;
            hasLocalAnchorTransform = true;

            playerSpawnPoint = spatialAnchor
                .GetComponentsInChildren<Transform>()
                .FirstOrDefault(t => t.CompareTag("playerSpawn"));

            if (character == GameCharacter.BIRD || character == GameCharacter.RABBIT ||
                character == GameCharacter.MOUSE)
            {
                localIsSpatialAnchorCreated = true;
            }

            Debug.Log($"SpatialAnchor created successfully at local position: {localAnchorPosition}");
        }
        else if (!isSpatialAnchorCreated)
        {
            Debug.Log("Waiting for isSpatialAnchorCreated to become true before creating SpatialAnchor");
        }
    }

    // SpatialAnchorの位置情報をカスタムプロパティとして設定
    private void SetSpatialAnchorTransformProperty(Vector3 position, Quaternion rotation)
    {
        if (PhotonNetwork.InRoom)
        {
            // ローカルの座標を保存
            localAnchorPosition = position;
            localAnchorRotation = rotation;
            hasLocalAnchorTransform = true;

            GameManager.SceneTransform anchorTransform = new GameManager.SceneTransform
            {
                position = position,
                rotation = rotation
            };

            string transformJson = JsonUtility.ToJson(anchorTransform);
            ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable
            {
                ["spatialAnchorTransform"] = transformJson,
                ["isSpatialAnchorCreated"] = true
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);

            Debug.Log($"Set SpatialAnchor transform to custom property: {transformJson}");
            Debug.Log($"Local anchor position: {localAnchorPosition}, rotation: {localAnchorRotation}");
            localIsSpatialAnchorCreated = true;
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

                    // VRプレイヤーの場合、座標系の変換を行う
                    if (GetGameCharacter() != GameCharacter.PANDA)
                    {
                        // VR空間での原点を基準とした位置に変換
                        position = anchorTransform.position;
                        rotation = anchorTransform.rotation;

                        // VRプレイヤーのスケールに合わせて調整（必要に応じて調整）
                        position *= VRWorldScale;
                    }
                    else
                    {
                        position = anchorTransform.position;
                        rotation = anchorTransform.rotation;
                    }
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

        // isSpatialAnchorCreatedの更新を確認
        if (propertiesThatChanged.ContainsKey("isSpatialAnchorCreated"))
        {
            bool newValue = (bool)propertiesThatChanged["isSpatialAnchorCreated"];
            isSpatialAnchorCreated = newValue;
            localIsSpatialAnchorCreated = newValue;
            Debug.Log($"isSpatialAnchorCreated updated to: {newValue}");
        }

        // spatialAnchorTransformプロパティが更新された場合
        if (propertiesThatChanged.ContainsKey("spatialAnchorTransform") &&
            GetGameCharacter() != GameCharacter.PANDA &&
            !spatialAnchor)
        {
            Vector3 anchorPosition;
            Quaternion anchorRotation;

            if (TryGetSpatialAnchorTransform(out anchorPosition, out anchorRotation))
            {
                CreateSpatialAnchor(anchorPosition, anchorRotation);
            }
        }
    }

    // デバッグ用のローカル座標取得メソッド
    public Vector3 GetLocalAnchorPosition()
    {
        return localAnchorPosition;
    }

    public Quaternion GetLocalAnchorRotation()
    {
        return localAnchorRotation;
    }

    public bool HasLocalAnchorTransform()
    {
        return hasLocalAnchorTransform;
    }
}
