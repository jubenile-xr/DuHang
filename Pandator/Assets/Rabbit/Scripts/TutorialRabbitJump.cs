using UnityEngine;
using System.Collections.Generic;

public class TutorialRabbitJump : MonoBehaviour
{
    private float jumpForce; // ジャンプ力
    private const float jumpSlow = 3.0f;
    private const float jumpNormal = 6.0f;

    private const float handSpeedThreshold = 1.5f; // 手の振りの速度の閾値
    private const float speedSyncThreshold = 0.5f; // 両手の速度差の閾値（同時判定用）
    private float maxR, maxL, maxAbs;
    private Rigidbody rb;
    private bool isGrounded = true; // 地面にいるかの判定
    private Queue<(float timestamp, float value)> valueHistory = new Queue<(float, float)>();
    private const float timeToKeep = 0.2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        jumpForce = jumpNormal; // 初期値を通常ジャンプ力に設定
        if (rb == null)
        {
            Debug.LogError("Rigidbodyがアタッチされていません");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 右手のAボタンが押されたかチェック
        bool isAButtonPressed = OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch);

        // 右手と左手の速度を取得
        Vector3 velocityR = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
        Vector3 velocityL = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
        float R_y = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch).y;
        float L_y = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch).y;
        // 現在の値（例としてオブジェクトの位置）を取得
        float currentValue = (R_y + L_y) / 2;
        float currentTime = Time.time;

        // 値をキューに追加
        valueHistory.Enqueue((currentTime, currentValue));

        // 古い値を削除
        while (valueHistory.Count > 0 && currentTime - valueHistory.Peek().timestamp > timeToKeep)
        {
            valueHistory.Dequeue();
        }

        // 0.2秒前の値を取得（無ければ最新値）
        float previousValue = valueHistory.Count > 0 ? valueHistory.Peek().value : currentValue;

        // スイングアップの判定（現在の値が過去の値より大きいか）
        bool isSwingUp = currentValue > previousValue;

        // XZ平面上の速度を計算（上下の動きを除外）
        float speedR = new Vector2(velocityR.x, velocityR.z).magnitude;
        float speedL = new Vector2(velocityL.x, velocityL.z).magnitude;
        if (speedR > maxR)
        {
            maxR = speedR;
        }
        if (speedL > maxL)
        {
            maxL = speedL;
        }
        if (Mathf.Abs(speedR - speedL) > maxAbs)
        {
            maxAbs = Mathf.Abs(speedR - speedL);
        }
        // 速度の条件を満たしているか（両手の速度が一定値以上 & 速度差が小さい）
        bool isHandSwinging = (speedR > handSpeedThreshold && speedL > handSpeedThreshold) && (Mathf.Abs(speedR - speedL) < speedSyncThreshold);

        //bool TestKeySpace = Input.GetKeyDown(KeyCode.Space);
        // 条件を満たしたらジャンプ
        if ((isAButtonPressed && isGrounded && isHandSwinging && isSwingUp) || (Input.GetKeyDown(KeyCode.J) && isGrounded))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false; // 空中にいると判定
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
    public void SetJumpSpeedNormal()
    {
        jumpForce = jumpNormal;
    }
    public void SetJumpSpeedSlow()
    {
        jumpForce = jumpSlow;
    }
}
