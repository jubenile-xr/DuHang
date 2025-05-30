using UnityEngine;

public class Camerafollow1 : MonoBehaviour
{
    public Transform ovrPlayer;
    public Transform centerEyeAnchor;
    public float CamerafollowSpeed = 20f;//镜头跟随速度//camera follow speed

    void Start()
    {
        
    }

    void Update()
    {
        if (ovrPlayer == null ) return;

        //让小鸟平滑跟随 OVRPlayerController
        //make the bird follow the camera smothly
        transform.position = Vector3.Lerp(transform.position, ovrPlayer.position, CamerafollowSpeed * Time.deltaTime);

        //实现小鸟的身体随方向转动
        //make the bird body rotate with the camera direction
        Vector3 forwardDirection = centerEyeAnchor.forward; // 获取头显朝向//get the camera direction

        // 忽略俯仰角，保持水平,用于实现小鸟的身体随方向转动
        // ignore the pitch angle, keep horizontal, used to make the bird body rotate with the camera direction
        forwardDirection.y = 0;
        transform.forward = Vector3.Slerp(transform.forward, forwardDirection, Time.deltaTime * 5f);
    }
}
