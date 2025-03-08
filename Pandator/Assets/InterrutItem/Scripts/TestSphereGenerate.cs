using UnityEngine;

public class TestSphereGenerate : MonoBehaviour
{
    [SerializeField] private GameObject spherePrefab;
    [SerializeField] private float recastTime = 5.0f;
    private float timer = 0.0f;
    [SerializeField] private float speed = 5.0f;

    void Update()
    {
        timer += Time.deltaTime;
        if(timer > recastTime)
        {
            timer = 0.0f;
            GameObject sphere = Instantiate(spherePrefab, transform.position, Quaternion.identity);
            sphere.GetComponent<Rigidbody>().linearVelocity = transform.forward * speed;
        }
    }

}
