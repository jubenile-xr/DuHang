using UnityEngine;

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

    void MoveTowardsTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }
}