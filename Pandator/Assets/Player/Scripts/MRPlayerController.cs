using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MRPlayerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject cameraRig;
    [SerializeField] private GameObject player;
    [SerializeField] private float moveSpeed = 2.0f; // 移動速度
    [SerializeField] private float maxRotationLimit = 80.0f; // 上下回転の制限
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraForward = cameraRig.transform.forward;
        player.transform.Translate(cameraForward * moveSpeed * Time.deltaTime, Space.Self);
    }
}