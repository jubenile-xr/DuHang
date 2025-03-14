using UnityEngine;

public class Camerafollow1 : MonoBehaviour
{
    public Transform ovrPlayer;
    public Transform centerEyeAnchor;
    public float CamerafollowSpeed = 20f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
        if (ovrPlayer == null ) return;

        //��С��ƽ������ OVRPlayerController
        transform.position = Vector3.Lerp(transform.position, ovrPlayer.position, CamerafollowSpeed * Time.deltaTime);

        //ʵ��С��������淽��ת��
        Vector3 forwardDirection = centerEyeAnchor.forward; // ��ȡͷ�Գ���

        //forwardDirection.y = 0;  // ���Ը����ǣ�����ˮƽ,����ʵ��С��������淽��ת��
        forwardDirection.y = 0;
        transform.forward = Vector3.Slerp(transform.forward, forwardDirection, Time.deltaTime * 5f);
    }
}
