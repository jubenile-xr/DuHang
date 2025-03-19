using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class NetAnimation : MonoBehaviour
{
    private Rigidbody rb;
    private bool hasCollided = false;

    [Header("回転補正の設定")]
    [Tooltip("下側がターゲット方向（空中では世界の下、衝突時は衝突面）へ向く補正にかける時間（秒）")]
    public float alignDuration = 1.0f;

    [Tooltip("回転補正完了後、物理演算を無効にする場合はtrueにする（衝突時のみ有効です）")]
    public bool setKinematicAfterAlignment = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 物理演算による自動回転を抑制する（コードで回転補正を行うため）
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // 発射後、かつまだ衝突していない間は、オブジェクトの下側が世界の下(Vector3.down)を向くように補正する。
        if (!hasCollided)
        {
            // 現在の下方向 (ローカル Vector3.down をワールド空間に変換)
            // と目標の下方向（Vector3.down）との差分を元に回転を求める
            Quaternion targetRotation = Quaternion.FromToRotation(transform.TransformDirection(Vector3.up), Vector3.up) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime / alignDuration);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 複数回処理されないように
        if (hasCollided) return;

        // 速度と角速度をリセットして、物理運動を停止する
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        hasCollided = true;

        // 衝突時、最初のコンタクト点から衝突面の法線を取得
        Vector3 contactNormal = collision.contacts[0].normal;

        /*
            衝突時の目標回転は、オブジェクトの下側（ローカル Vector3.down ）が
            衝突面の法線に一致するように計算します。
            すなわち、Quaternion.FromToRotation(現在の下方向, 接触面の法線) を現在の回転に掛け合わせる
        */
        Quaternion targetRotation = Quaternion.FromToRotation(transform.TransformDirection(Vector3.up), contactNormal) * transform.rotation;

        // なめらかに回転を補正するコルーチンを開始
        StartCoroutine(SmoothAlign(targetRotation));
    }

    private IEnumerator SmoothAlign(Quaternion targetRot)
    {
        Quaternion startRot = transform.rotation;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / alignDuration;
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }
        transform.rotation = targetRot; // 補正完了

        // 必要に応じて、回転補正後は物理運動を完全に停止させる
        if (setKinematicAfterAlignment)
        {
            rb.isKinematic = true;
        }
    }
}