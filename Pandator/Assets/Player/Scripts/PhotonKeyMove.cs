using UnityEngine;
using Photon.Pun;

public class PhotonKeyMove : MonoBehaviourPun
{
    [Header("移動速度")]
    [SerializeField] private float speed = 10.0f;

    private void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        float vertical = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float horizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        transform.Translate(horizontal, 0, vertical);

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.Rotate(0, 90, 0);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.Rotate(0, -90, 0);
        }
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
}