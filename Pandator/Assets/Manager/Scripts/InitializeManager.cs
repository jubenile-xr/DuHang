using System.Collections;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

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
    private bool hasSpatialAnchorInstantiated = false;

    void Start()
    {
        Debug.Log("debug mode" + DebugManager.GetDebugMode());

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

        // デバッグモードの場合のみ、ローカルでspatialAnchorを生成
        if (DebugManager.GetDebugMode())
        {
            GameObject spatialAnchorPrefab = Resources.Load<GameObject>("SpatialAnchor/Prefab/spatialAnchor");
            if (spatialAnchorPrefab != null)
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
                SetupPlayerSpawn();
            }
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
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

            if (gameManager && isAnimationFinished && !isPlayerCreated && spatialAnchor != null)
            {
                // プレイヤーとカメラを生成する準備ができているかチェック
                if (playerSpawn == null)
                {
                    SetupPlayerSpawn();
                }
                else if (isAnchorLoaded)
                {
                    // ロード完了したらUIを表示
                    loadingScene.SetActive(false);
                    eventSystem.SetActive(true);
                    CreatePlayerAndCamera();
                    SetPlayerCreated(true);
                }
            }
        }
    }

    // ルーム参加に成功した時の処理
    public override void OnJoinedRoom()
    {
        Debug.LogWarning("OnJoinedRoom");

        // GameManagerの生成を待つ
        StartCoroutine(WaitForGameManager());

        // GameManagerを生成（GODプレイヤーのみ）
        if (GetGameCharacter() == GameCharacter.GOD)
        {
            PhotonNetwork.Instantiate("GameManager", Vector3.zero, Quaternion.identity);
        }

        // パンダ(MR)の場合はspatialAnchorを生成
        if (GetGameCharacter() == GameCharacter.PANDA && !hasSpatialAnchorInstantiated)
        {
            Debug.Log("Panda: Instantiating spatialAnchor via PhotonNetwork");
            try
            {
                // spatialAnchorを生成
                spatialAnchor = PhotonNetwork.Instantiate("SpatialAnchor/Prefab/spatialAnchor", Vector3.zero, Quaternion.identity);
                if (spatialAnchor != null)
                {
                    hasSpatialAnchorInstantiated = true;

                    // *** PhotonNetwork.Instantiate後の処理 ***

                    // 1. 必要なコンポーネントの取得と設定
                    anchorManager = spatialAnchor.GetComponent<AnchorManager>();
                    if (anchorManager == null)
                    {
                        Debug.LogError("AnchorManager component not found on instantiated spatialAnchor");
                    }

                    // 2. 親子関係の設定（必要に応じて）
                    if (transform.parent != null)
                    {
                        spatialAnchor.transform.SetParent(transform.parent, false);
                    }

                    // 3. 位置と回転の調整（必要に応じて）
                    Vector3 cameraPosition = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
                    Vector3 forward = Camera.main != null ? Camera.main.transform.forward : Vector3.forward;
                    // カメラの前方1.5mにアンカーを配置
                    Vector3 position = cameraPosition + forward * 1.5f;
                    position.y = cameraPosition.y - 0.5f;  // カメラより少し下に配置
                    spatialAnchor.transform.position = position;

                    // 4. アンカーの有効化
                    spatialAnchor.SetActive(true);

                    // 5. PlayerSpawnポイントの初期化
                    SetupPlayerSpawn();

                    // 6. ロード画面を非表示
                    if (loadingScene != null)
                    {
                        loadingScene.SetActive(false);
                    }

                    // 7. その他の初期化（必要に応じて）
                    Debug.Log($"SpatialAnchor successfully instantiated: ID={spatialAnchor.GetPhotonView().ViewID}, Position={spatialAnchor.transform.position}");

                    // 8. 他のプレイヤーへの通知（オプション）
                    if (PhotonNetwork.InRoom)
                    {
                        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
                        props.Add("spatialAnchorCreated", true);
                        props.Add("spatialAnchorViewID", spatialAnchor.GetPhotonView().ViewID);
                        props.Add("spatialAnchorPosition", spatialAnchor.transform.position);
                        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
                    }
                }
                else
                {
                    Debug.LogError("Failed to instantiate spatialAnchor: result is null");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to instantiate spatialAnchor: " + e.Message);
            }
        }
    }

    //コルーチンでOnJoinedRoom内でリトライ機構ができるように
    //GameManagerの取得
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

        // VR以外のプレイヤー名を生成
        if (gameManager.GetPlayerType() != GameManager.PlayerType.VR)
        {
            CreatePlayerName();
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
