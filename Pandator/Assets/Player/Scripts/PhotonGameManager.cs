using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;

public class PhotonGameManager : MonoBehaviourPunCallbacks
{
    public GameObject PhotonFailureObject;
    private GameObject camera;
    [SerializeField] public bool IsRabbit;
    [SerializeField] public bool IsBird;
    [SerializeField] public bool IsMouse;
    [SerializeField] public bool IsPanda;
    private GameManager gameManager;

    private GameObject player;
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
        Debug.Log("Joined Room");
        {

            if (!IsPanda)
            {
                StartCoroutine(WaitForGameManager());
            }

            // ルームに入室できたら、PhotonObject(本記事ではSphere)を生成する
            if (IsBird)
            {
                player = PhotonNetwork.Instantiate("Player/BirdPlayer", new Vector3(0f, 0f, 0f), Quaternion.identity);
                // gameManager.SetIncrementAliveCount();
                camera = Instantiate(Resources.Load<GameObject>("CameraRig/BirdCameraRig"), new Vector3(0f, 0f, 0f),
                    Quaternion.identity);
            }
            else if (IsRabbit)
            {
                player = PhotonNetwork.Instantiate("Player/RabbitPlayer", new Vector3(0f, 0f, 0f), Quaternion.identity);
                // gameManager.SetIncrementAliveCount();
                camera = Instantiate(Resources.Load<GameObject>("CameraRig/RabbitCameraRig"), new Vector3(0f, 0f, 0f),
                    Quaternion.identity);
            }
            else if (IsMouse)
            {
                player = PhotonNetwork.Instantiate("Player/MousePlayer", new Vector3(0f, 0f, 0f), Quaternion.identity);
                // gameManager.SetIncrementAliveCount();
                camera = Instantiate(Resources.Load<GameObject>("CameraRig/MouseCameraRig"), new Vector3(0f, 0f, 0f),
                    Quaternion.identity);
            }
            else if (IsPanda)
            {
                player = PhotonNetwork.Instantiate("Player/PandaPlayer", new Vector3(0f, 0f, 0f), Quaternion.identity);
                PhotonNetwork.Instantiate("GameManager", new Vector3(0f, 0f, 0f), Quaternion.identity);
                camera = Instantiate(Resources.Load<GameObject>("CameraRig/PandaCameraRig"), new Vector3(0f, 0f, 0f),
                    Quaternion.identity);
            }

            camera.transform.SetParent(player.transform);
        }

        CreatePhotonAvatar avatarScript = player.GetComponent<CreatePhotonAvatar>();
        if (avatarScript == null)
        {
            Debug.LogError("CreatePhotonAvatar script is missing on the instantiated player object!");
            return;
        }

        avatarScript.ExecuteCreatePhotonAvatar();
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
                Debug.Log("GameManager object not found. Waiting...");
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