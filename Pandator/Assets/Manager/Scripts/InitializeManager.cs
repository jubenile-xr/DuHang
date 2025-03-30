using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class InitializeManager : MonoBehaviourPunCallbacks
{
    public GameObject PhotonFailureObject;
    private enum GameCharacter
    {
        BIRD,
        RABBIT,
        MOUSE,
        PANDA
    }
    [SerializeField] private GameCharacter character;
    private GameManager gameManager;
    private GameObject player;
    private GameObject camera;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
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
                player = PhotonNetwork.Instantiate("Player/RabbitPlayer", new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                camera = Instantiate(Resources.Load<GameObject>("CameraRig/RabbitCameraRig"), new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                break;
            case GameCharacter.MOUSE:
                player = PhotonNetwork.Instantiate("Player/MousePlayer", new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                camera = Instantiate(Resources.Load<GameObject>("CameraRig/MouseCameraRig"), new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                break;
            case GameCharacter.PANDA:
                player = PhotonNetwork.Instantiate("Player/PandaPlayer", new Vector3(0f, 0.5f, 0f), Quaternion.identity);
                camera = Instantiate(Resources.Load<GameObject>("CameraRig/PandaCameraRig"), new Vector3(0f, 0.5f, 0f), Quaternion.identity);
                // TODO: GameManagerの生成を消して、GameManagerがカスタムプロパティを共有できるように
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
}