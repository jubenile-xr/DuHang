using UnityEngine;

public class SweepingLight : MonoBehaviour
{
    [Header("回転の中心")]
    public Transform pivotPoint;

    [Header("回転パラメータ")]
    public float baseRotationSpeed = 30f;  // 基本回転速度（度/秒）
    public float randomSwingAngle = 20f;   // 最大スイング角度

    [Header("ノイズパラメータ")]
    public float noiseSpeed = 0.5f;        // ノイズ変化速度
    public float noiseStrength = 0.5f;     // ノイズ強度（0〜1）

    [Header("ライト設定")]
    public Light targetLight;              // 操作対象のライト
    public Transform targetToFollow;       // 照らす対象オブジェクト

    private float noiseTime;

    void Update()
    {
        // ノイズ時間の更新
        noiseTime += Time.deltaTime * noiseSpeed * 0.01f;

        float offsetX = (Mathf.PerlinNoise(noiseTime, 0f) - 0.5f) * 2f * randomSwingAngle * noiseStrength;
        float offsetY = (Mathf.PerlinNoise(0f, noiseTime) - 0.5f) * 2f * randomSwingAngle * noiseStrength;

        // 自身を pivotPoint を中心に Y 軸回転させる
        if (pivotPoint != null)
        {
            transform.RotateAround(pivotPoint.position, Vector3.up, baseRotationSpeed * Time.deltaTime);
        }

        // ノイズを加えた回転を計算
        Quaternion noiseRotation = Quaternion.Euler(offsetX, offsetY, 0f);
        transform.rotation = noiseRotation * transform.rotation;

        // ライトの制御
        if (targetLight != null)
        {
            // ターゲットが指定されていれば、常にその方向を見る
            if (targetToFollow != null)
            {
                Vector3 dir = (targetToFollow.position - targetLight.transform.position).normalized;
                Quaternion lookRot = Quaternion.LookRotation(dir);
                targetLight.transform.rotation = Quaternion.Slerp(targetLight.transform.rotation, lookRot, Time.deltaTime * 5f);
            }
            else
            {
                // 指定がない場合は、自身の回転に追従
                targetLight.transform.rotation = transform.rotation;
            }
        }
    }
}
