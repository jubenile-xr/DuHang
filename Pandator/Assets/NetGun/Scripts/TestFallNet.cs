using UnityEngine;

public class TestFallNet : MonoBehaviour
{
    [SerializeField] private GameObject netPrefab;
    [SerializeField] private float recastTime = 5.0f;
    private float timer = 0.0f;

    void Update()
    {
        timer += Time.deltaTime;
        if(timer > recastTime)
        {
            timer = 0.0f;
            GameObject net = Instantiate(netPrefab, transform.position, Quaternion.identity);
        }
    }

}
