using UnityEngine;

public class TestOtherPlayerMove : MonoBehaviour
{
    [Header("移動速度")]
    [SerializeField] private float speed = 10.0f;

    private void Update()
    {
        // jkliで移動
        float vertical = 0;
        float horizontal = 0;
        float walk = 0.01f;
        if (Input.GetKey(KeyCode.J))
        {
            horizontal = -1 * walk;
        }
        if (Input.GetKey(KeyCode.L))
        {
            horizontal = 1 * walk;
        }
        if (Input.GetKey(KeyCode.I))
        {
            vertical = walk;
        }
        if (Input.GetKey(KeyCode.K))
        {
            vertical = -1 * walk;
        }

        vertical *= speed;
        horizontal *= speed;
        transform.Translate(horizontal, 0, vertical);
    }
    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
}
