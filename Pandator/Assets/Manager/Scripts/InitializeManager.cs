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
    private GameObject gameManagerObject;
    private bool hasPlayerNameCreated = false;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    void Update()
    {
        if (gameManager == null)
        {
            gameManagerObject = GameObject.FindWithTag("GameManager");
        }

        if (GetGameCharacter() == GameCharacter.GOD)
        {
            hasPlayerNameCreated = true;
        }

        if (!hasPlayerNameCreated && gameManagerObject != null)
        {
            CreatePlayerName();
            hasPlayerNameCreated = true;
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
                // TODO: GameManagerの生成を消して、GameManagerがカスタムプロパティを共有できるように
                PhotonNetwork.Instantiate("GameManager", new Vector3(0f, 0f, 0f), Quaternion.identity);
                break;
            case GameCharacter.GOD:
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
                    gameManager.SetIncrementAliveCount();
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
        if (gameManagerObject == null)
        {
            Debug.LogError("GameManager object not found!");
            return;
        }

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

        // ローカルプレイヤーの名前を Photon のカスタムプロパティと NickName に設定
        SetLocalPlayerName(playerName);

        // マスタープレイヤーの子オブジェクト「Player」を取得し、StateManager で管理する場合の処理
        GameObject masterPlayerObject = GameObject.FindWithTag("MasterPlayer");
        if (masterPlayerObject == null)
        {
            Debug.LogError("MasterPlayer object not found!");
            return;
        }

        Transform playerTransform = masterPlayerObject.transform.Find("Player");
        if (playerTransform == null || !playerTransform.CompareTag("Player"))
        {
            Debug.LogError("Player child object not found under MasterPlayer!");
            return;
        }

        StateManager stateManager = playerTransform.GetComponent<StateManager>();
        if (stateManager != null)
        {
            // StateManager にプレイヤー名を設定（static でもインスタンスメソッドでも、ここはプロジェクトに合わせて修正）
            StateManager.SetPlayerName(playerName);
        }
        else
        {
            Debug.LogError("StateManager component not found on Player object!");
        }
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

    private void SetLocalPlayerName(string name)
    {
        PhotonNetwork.NickName = name;

        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props.Add("playerName", name);
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public GameCharacter GetGameCharacter()
    {
        return character;
    }
}