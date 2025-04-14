using UnityEngine;

public class GodManager : MonoBehaviour
{
    [Tooltip("追従させるカメラオブジェクト")]
    public GameObject godCamera;

    public enum FollowTargetType
    {
        Rabbit,
        Bird,
        Mouse,
        Panda,
        God
    }

    [Header("追従対象の設定")]
    [Tooltip("追従させるプレイヤーの種類")]
    public FollowTargetType followTarget = FollowTargetType.God;

    [Tooltip("カメラに対して適用する位置オフセット")]
    public Vector3 followOffset = Vector3.zero;

    // カメラの初期位置と回転を保持するための変数
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    // シーン開始時にカメラの初期位置と回転を記録
    private void Start()
    {
        if (godCamera != null)
        {
            initialPosition = godCamera.transform.position;
            initialRotation = godCamera.transform.rotation;
        }
    }

    private void LateUpdate()
    {
        // followTargetがGodの場合は、カメラを初期位置と回転に戻す
        if (followTarget == FollowTargetType.God)
        {
            if (godCamera != null)
            {
                godCamera.transform.position = initialPosition;
                godCamera.transform.rotation = initialRotation;
            }
            return;
        }

        // godCameraが登録されていなければ処理を中断
        if (godCamera == null)
            return;

        GameObject[] players = null; // 初期化
        GameObject targetPlayer = null;

        // followTargetに応じて異なるタグから対象のプレイヤーを取得する
        if (followTarget == FollowTargetType.Mouse || followTarget == FollowTargetType.Rabbit)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
        }
        else if (followTarget == FollowTargetType.Bird)
        {
            players = GameObject.FindGameObjectsWithTag("MasterPlayer");
        }
        else if (followTarget == FollowTargetType.Panda)
        {
            players = GameObject.FindGameObjectsWithTag("PandaPlayer");
        }

        // もしplayersが初期化されていなければ、警告を表示して中断
        if (players == null)
        {
            Debug.LogWarning("対象のタグに一致するプレイヤーが設定されていません。");
            return;
        }

        string targetKeyword = followTarget.ToString();

        foreach (GameObject player in players)
        {
            if (player.name.Contains(targetKeyword))
            {
                targetPlayer = player;
                Debug.Log("target Player Name: " + targetPlayer.name);
                break;
            }
        }

        if (targetPlayer != null)
        {
            godCamera.transform.position = targetPlayer.transform.position + followOffset;
            godCamera.transform.rotation = targetPlayer.transform.rotation;
        }
        else
        {
            Debug.LogWarning("対象のプレイヤーが見つかりませんでした。");
        }
    }
}