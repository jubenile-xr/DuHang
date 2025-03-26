using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject cameraRig;
    [SerializeField] private GameObject player;
    [SerializeField] private float moveSpeed = 2.0f; // 移動速度
    [SerializeField] private float maxRotationLimit = 80.0f; // 上下回転の制限
    public bool IsBird;
    public bool IsRabbit;
    public bool IsPanda;
    public bool IsMouse;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //キャラ移動処理
        Vector2 leftInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        CharaTranslate(leftInput);

        // カメラ回転処理
        Vector2 rightInput = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        CameraRotate(rightInput);
    }

    void CharaTranslate(Vector2 leftInput)
    {

        Vector3 forward = cameraRig.transform.forward;
        Vector3 right = cameraRig.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * leftInput.x + right * leftInput.y).normalized;
        moveDirection.y = 0f;
        player.transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.Self);
    }

    void CameraRotate(Vector2 rightInput)
    {
        //X軸回転角度の計算
        var angle = cameraRig.transform.eulerAngles;
        float minRotationLimit = 360f - maxRotationLimit;
        angle.x -= rightInput.y;
        if (angle.x > maxRotationLimit && angle.x < 180f)
        {
            angle.x = maxRotationLimit;
        } else if (angle.x < minRotationLimit && angle.x > 180f)
        {
            angle.x = minRotationLimit;
        }

        //Y軸回転角度の計算
        angle.y += rightInput.x;

        //カメラの回転
        cameraRig.transform.localEulerAngles = angle;
    }

    public void setCamera()
    {
        if (IsBird)
        {
            cameraRig = GameObject.Find("BirdPlayer(Clone)");
        } else if (IsRabbit)
        {
            cameraRig = GameObject.Find("RabbitCameraRig(Clone)");
        } else if (IsMouse)
        {
            cameraRig = GameObject.Find("MouseCameraRig(Clone)");
        } else if (IsPanda)
        {
            cameraRig = GameObject.Find("PandaCameraRig(Clone)");
        }
    }
}