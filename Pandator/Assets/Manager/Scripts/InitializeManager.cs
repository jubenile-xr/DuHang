using System.Collections;
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
    [SerializeField]private GameObject VRModel;

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
                eventSystem.SetActive(true);
                if (gameManager.GetPlayerType() == GameManager.PlayerType.VR)
                {
                    VRModel.SetActive(true);
                }
                // プレイヤーキャラクターの生成およびカメラの生成
                    switch (character)
                    {
                        case GameCharacter.BIRD:
                            player = PhotonNetwork.Instantiate("Player/BirdPlayer", new Vector3(0.5f, 3.0f, 0f),
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
                            player = PhotonNetwork.Instantiate("Player/RabbitPlayer", new Vector3(0f, 3.0f, 0f),
                                Quaternion.identity);
                            stateManager = player.GetComponentInChildren<StateManager>();
                            scoreManager = player.GetComponentInChildren<ScoreManager>();
                            camera = Instantiate(Resources.Load<GameObject>("CameraRig/RabbitCameraRig"),
                                new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                            canvas.SetActive(true);
                            break;
                        case GameCharacter.MOUSE:
                            player = PhotonNetwork.Instantiate("Player/MousePlayer", new Vector3(-0.5f, 3.0f, 0f),
                                Quaternion.identity);
                            stateManager = player.GetComponentInChildren<StateManager>();
                            scoreManager = player.GetComponentInChildren<ScoreManager>();
                            camera = Instantiate(Resources.Load<GameObject>("CameraRig/MouseCameraRig"),
                                new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                            canvas.SetActive(true);
                            break;
                        case GameCharacter.PANDA:
                            player = PhotonNetwork.Instantiate("Player/PandaPlayer", new Vector3(0f, 1.0f, 0f),
                                Quaternion.identity);
                            scoreManager = player.GetComponentInChildren<ScoreManager>();
                            camera = Instantiate(Resources.Load<GameObject>("CameraRig/PandaCameraRig"),
                            new Vector3(0f, 1.0f, 0f), Quaternion.identity);
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
}