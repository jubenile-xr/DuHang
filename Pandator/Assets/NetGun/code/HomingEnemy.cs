using UnityEngine;

// 特定の範囲内のターゲット（タグが "Enemy" のオブジェクト）を検出し，そのターゲットに向かってホーミングするモジュール
public class HomingEnemy : MonoBehaviour
{
    public float speed = 5f;
    public float detectionRadius = 3f;

    private Transform target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        FindTarget();
        if (target != null)
        {
            MoveTowardsTarget();
        }
    }

    void FindTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
// "Enemy"タグをつけたオブジェクトをホーミングするターゲットとして座標を保持する
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                target = hitCollider.transform;
                return;
            }
        }
        target = null;
    }

// 指定されたターゲットに向かって移動するメソッド
void MoveTowardsTarget()
{
    Vector3 direction = (target.position - transform.position).normalized;
    
    // direction(ターゲットへの方向の単位ベクトル)に speed(5f) で移動する（時間経過を考慮するため Time.deltaTime を掛ける）
    transform.position += direction * speed * Time.deltaTime;
}
}