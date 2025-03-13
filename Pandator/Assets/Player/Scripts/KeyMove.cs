using UnityEngine;

public class KeyMove : MonoBehaviour
{
    [Header("移動速度")]
    [SerializeField] private float speed = 10.0f;

    private void Update()
    {
        float vertical = Input.GetAxis("Vertical") * speed;
        float horizontal = Input.GetAxis("Horizontal") * speed;

        vertical *= Time.deltaTime;
        horizontal *= Time.deltaTime;
        transform.Translate(horizontal, 0, vertical);

        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.Rotate(0, 90, 0);
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.Rotate(0, -90, 0);
        }
    }
    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
}
