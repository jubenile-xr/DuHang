using UnityEngine;

public class Camerafollow1 : MonoBehaviour
{
    public Transform ovrPlayer;
    public Transform centerEyeAnchor;
    public float CamerafollowSpeed = 20f;//��ͷ�����ٶ�

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

        // ���Ը����ǣ�����ˮƽ,����ʵ��С��������淽��ת��
        forwardDirection.y = 0;
        transform.forward = Vector3.Slerp(transform.forward, forwardDirection, Time.deltaTime * 5f);
    }
}
