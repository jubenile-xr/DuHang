using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction.Unity.Input;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;


public class NetGun : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float BulletSpeed = 1f;
    [SerializeField] private GameObject RightController;
    [SerializeField] private GameObject Tip;
    [SerializeField] private float spanTime = 0f;
    private Boolean shotable = true;
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //実験用
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) || Input.GetKeyDown(KeyCode.Space))

        {
            if (shotable)
            {
                Shot();
                shotable = false;
            }

        }
        spanTime += Time.deltaTime;

        //銃を初期状態に戻す
        if (spanTime > 5.0f)
        {
            shotable = true;
            spanTime = 0;

        }
    }

    public void Shot() //弾の発射
    {
        animator.SetTrigger("Fire");

        //弾の発射位置(transform.position)は再考の余地あり
        GameObject bulletInstance = PhotonNetwork.Instantiate("InterruptItem/Net", -RightController.transform.position, Quaternion.LookRotation(RightController.transform.forward));
        bulletInstance.GetComponent<Rigidbody>().AddForce(RightController.transform.forward * 10 * Time.deltaTime * 1000 * BulletSpeed);

    }
}
