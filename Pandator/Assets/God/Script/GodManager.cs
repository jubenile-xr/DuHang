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

    private void LateUpdate()
    {
        if (followTarget == FollowTargetType.God || godCamera == null)
            return;

        GameObject[] players = GameObject.FindGameObjectsWithTag("MasterPlayer");
        GameObject targetPlayer = null;

        string targetKeyword = followTarget.ToString();

        foreach (GameObject player in players)
        {
            if (player.name.Contains(targetKeyword))
            {
                targetPlayer = player;
                break;
            }
        }

        if (targetPlayer != null)
        {
            godCamera.transform.position = targetPlayer.transform.position + followOffset;
            godCamera.transform.rotation = targetPlayer.transform.rotation;
        }
    }
}