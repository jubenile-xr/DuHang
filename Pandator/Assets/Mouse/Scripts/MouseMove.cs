using UnityEngine;

public class MouseMove : MonoBehaviour
{
    [SerializeField] private Transform moveObject = null; // 移動するオブジェクト
    [SerializeField] private float moveSpeedMultiplier = 1.0f; // 移動速度倍率

    void Update()
    {
        if (moveObject == null) return;

        // 右手と左手の速度を取得
        Vector3 velocityR = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
        Vector3 velocityL = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);

        // XZ平面上の速度の合計を計算
        float speedR = new Vector2(velocityR.x, velocityR.z).magnitude;
        float speedL = new Vector2(velocityL.x, velocityL.z).magnitude;
        float totalSpeed = (speedR + speedL) * moveSpeedMultiplier;

        // 頭（カメラ）の向きを取得して移動方向を決定
        Transform headTransform = Camera.main.transform;
        Vector3 forwardDirection = headTransform.forward;
        forwardDirection.y = 0; // 水平移動のみ考慮
        forwardDirection.Normalize();

        // 移動処理
        moveObject.Translate(forwardDirection * totalSpeed * Time.deltaTime, Space.World);
    }
}