using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class NetAnimation : MonoBehaviour
{
    private Rigidbody rb;
    private bool hasCollided = false;

    [Header("回転補正の設定")]
    [Tooltip("下側がターゲット方向（空中では世界の下，衝突時は衝突面）へ向く補正にかける時間（秒）")]
    public float alignDuration = 1.0f;

    [Tooltip("回転補正完了後，物理演算を無効にする場合はtrueにする（衝突時のみ有効）")]
    public bool setKinematicAfterAlignment = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 物理演算による自動回転を抑制する（コードで回転補正を行うため）
        rb.freezeRotation = true;
        Quaternion targetRotation = Quaternion.FromToRotation(transform.TransformDirection(Vector3.down), Vector3.up) * transform.rotation;
        transform.rotation = targetRotation;
    }

    private void Update()
    {
        // 発射後、かつまだ衝突していない間は、オブジェクトの下側が世界の下(Vector3.down)を向くように補正する
        if (!hasCollided)
        {
            // 現在の下方向 (ローカル Vector3.up をワールド空間に変換)と目標の下方向（Vector3.down）との差分を元に回転を求める
            // これどうしよう，初めから回転方向が決まってるのは大丈夫か？
            Quaternion targetRotation = Quaternion.FromToRotation(transform.TransformDirection(Vector3.up), Vector3.up) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime / alignDuration);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 複数回処理されないように
        if (hasCollided) return;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        hasCollided = true;

        // 衝突時、最初のコンタクト点から衝突面の法線を取得(法線ベクトル方向にオブジェクトを向かせれば壁についた形になる？)
        Vector3 contactNormal = collision.contacts[0].normal;

        Quaternion targetRotation = Quaternion.FromToRotation(transform.TransformDirection(Vector3.up), contactNormal) * transform.rotation;

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
        transform.rotation = targetRot;

        if (setKinematicAfterAlignment)
        {
            rb.isKinematic = true;
        }
    }
}