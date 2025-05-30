using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public Transform pathParent;        // パスの親オブジェクト（すべての経路ポイントを含む）
    public float moveSpeed = 1f;        // 移動速度
    public bool loop = true;            // ループするかどうか

    private Transform[] points;
    private int currentIndex = 0;

    void Start()
    {
        int count = pathParent.childCount;
        points = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            points[i] = pathParent.GetChild(i);
        }
    }

    void Update()
    {
        if (points == null || points.Length == 0) return;

        Transform target = points[currentIndex];
        Vector3 direction = target.position - transform.position;

        // ターゲット方向を向く
        if (direction != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 5f);
        }

        // 移動処理
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        // 現在のポイントに到達したら → 次のポイントへ
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentIndex++;
            if (currentIndex >= points.Length)
            {
                currentIndex = loop ? 0 : points.Length - 1;
            }
        }
    }
}
