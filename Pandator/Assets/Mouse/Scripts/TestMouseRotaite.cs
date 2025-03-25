using UnityEngine;

public class TestMouseRotaite : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(0, 1.0f, 0);
        }
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(0, -1.0f, 0);
        }
    }
}

