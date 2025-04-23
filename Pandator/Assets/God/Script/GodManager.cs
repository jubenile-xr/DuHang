using System;
using UnityEngine;

public class GodManager : MonoBehaviour
{
    [Tooltip("追従させるカメラオブジェクト")]
    public GameObject godCamera;

    [Tooltip("追従させるプレイヤーの種類，PlayerNameArrayの配列のインデックスに対応(PandaとGodは除く)")]
    public enum FollowTargetType
    {
        Zero,
        One,
        Two,
        Panda,
        God
    }

    [Tooltip("GameManagerから取得するPlayerNameの配列")]
    private string[] playerNameArray;


    [Header("追従対象の設定")]
    [Tooltip("追従させるプレイヤーの種類")]
    public FollowTargetType followTarget = FollowTargetType.God;

    [Tooltip("カメラに対して適用する位置オフセット")]
    public Vector3 followOffset = Vector3.zero;

    // カメラの初期位置と回転を保持するための変数
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    // GameManager
    private GameManager gameManager;

    // PlayerNameArrayが作成されたかどうかを示すフラグ
    private bool hasPlayerNameCreated;

    void Start()
    {
        if (godCamera != null)
        {
            initialPosition = godCamera.transform.position;
            initialRotation = godCamera.transform.rotation;
        }
    }

    void Update()
    {
        if (gameManager == null)
        {
            GameObject gmObj = GameObject.FindWithTag("GameManager");
            if (gmObj)
            {
                gameManager = gmObj.GetComponent<GameManager>();
            }
        }

        if (gameManager != null && gameManager.GetPlayerType() == GameManager.PlayerType.GOD
            && gameManager.GetGameState() == GameManager.GameState.PLAY
            && !hasPlayerNameCreated)
        {
            SetPlayerNameArray(gameManager.GetAllPlayerNames());
            hasPlayerNameCreated = true;
        }

        // ゲーム終了時は初期状態に戻す
        if (gameManager != null && gameManager.GetGameState() == GameManager.GameState.END)
        {
            if (godCamera != null)
            {
                godCamera.transform.position = initialPosition;
                godCamera.transform.rotation = initialRotation;
            }
        }
    }

    // Inspector上で値が変更されたときに呼ばれる
    void OnValidate()
    {
        // Playモードの場合のみ更新する
        if (Application.isPlaying)
        {
            UpdateTargetCamera();
        }
    }

    // 指定した親オブジェクトの子から指定タグを持つオブジェクトを探索するヘルパーメソッド
    GameObject FindChildWithTag(GameObject parent, string tag)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.CompareTag(tag))
                return child.gameObject;
        }
        return null;
    }

    // followTargetに基づいて対象のカメラを探索し、godCameraに反映する処理
    private void UpdateTargetCamera()
    {
        if (godCamera == null)
            return;

        GameObject targetCamera = null;
        GameObject[] masterPlayers = GameObject.FindGameObjectsWithTag("MasterPlayer");
        Debug.LogWarning("masterPlayers length = " + masterPlayers.Length);

        if (followTarget == FollowTargetType.Zero)
        {
            Debug.LogWarning("followTarget: " + followTarget);
            foreach (GameObject master in masterPlayers)
            {
                ScoreManager scoreManager = master.GetComponent<ScoreManager>();
                if (scoreManager != null &&
                    scoreManager.GetPlayerName() == GetPlayerNameArray()[0])
                {
                    Debug.LogWarning("scoreManager.GetPlayerName() = " + scoreManager.GetPlayerName());
                    Debug.LogWarning("GetPlayerNameArray()[0] = " + GetPlayerNameArray()[0]);
                    GameObject tc = FindChildWithTag(master, "TrackedCamera");
                    if (tc != null)
                    {
                        Debug.LogWarning("targetCamera = " + tc);
                        targetCamera = tc;
                        break;
                    }
                }
            }
        }
        else if (followTarget == FollowTargetType.One)
        {
            foreach (GameObject master in masterPlayers)
            {
                ScoreManager scoreManager = master.GetComponent<ScoreManager>();
                if (scoreManager != null &&
                    scoreManager.GetPlayerName() == GetPlayerNameArray()[1])
                {
                    GameObject tc = FindChildWithTag(master, "TrackedCamera");
                    if (tc != null)
                    {
                        targetCamera = tc;
                        break;
                    }
                }
            }
        }
        else if (followTarget == FollowTargetType.Two)
        {
            foreach (GameObject master in masterPlayers)
            {
                ScoreManager scoreManager = master.GetComponent<ScoreManager>();
                if (scoreManager != null &&
                    scoreManager.GetPlayerName() == GetPlayerNameArray()[2])
                {
                    GameObject tc = FindChildWithTag(master, "TrackedCamera");
                    if (tc != null)
                    {
                        targetCamera = tc;
                        break;
                    }
                }
            }
        }
        else if (followTarget == FollowTargetType.Panda)
        {
            // 全てのMasterPlayerから名前に「Panda」を含むオブジェクトを対象とする
            foreach (GameObject master in masterPlayers)
            {
                if (master.gameObject.name.Contains("Panda"))
                {
                    GameObject tc = FindChildWithTag(master, "TrackedCamera");
                    if (tc != null)
                    {
                        targetCamera = tc;
                        break;
                    }
                }
            }
        }

        if (targetCamera == null)
        {
            Debug.LogWarning("対象のタグに一致するプレイヤーが設定されていません。");
            return;
        }

        godCamera.transform.position = targetCamera.transform.position + followOffset;
        godCamera.transform.rotation = targetCamera.transform.rotation;
    }

    private void SetPlayerNameArray(string[] playerNames)
    {
        playerNameArray = playerNames;
    }

    private string[] GetPlayerNameArray()
    {
        return playerNameArray;
    }

    void LateUpdate()
    {
        if (followTarget == FollowTargetType.God)
        {
            if (godCamera != null)
            {
                godCamera.transform.position = initialPosition;
                godCamera.transform.rotation = initialRotation;
            }
            return;
        }

        // Playモード中はOnValidateとは別にLateUpdateでも更新
        if (Application.isPlaying)
        {
            UpdateTargetCamera();
        }
    }
}