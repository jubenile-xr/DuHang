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
    [Header("GunComponent")]
    [SerializeField] private GameObject BulletType;
    [SerializeField] private float BulletSpeed = 1f;
    [SerializeField] private GameObject RightController;
    [SerializeField] private float spanTime = 0f;
    private Boolean shotable = true;
    void Start()
    {
        //Debug.Log("start");
    }

    // Update is called once per frame
    void Update()
    {
        //実験用
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) || Input.GetKeyDown(KeyCode.A))

        {
            if (shotable)
            {
                Shot();
                shotable = false;
            }
            
        }
        spanTime += Time.deltaTime;
        if (spanTime > 5.0f)
        {
            shotable = true;
            spanTime = 0;

        }
    }

    public void Shot() //弾の発射
    {
        //弾の発射位置(transform.position)は再考の余地あり
        GameObject bulletInstance = PhotonNetwork.Instantiate(BulletType.name, RightController.transform.position, Quaternion.LookRotation(RightController.transform.forward));

        bulletInstance.GetComponent<Rigidbody>().AddForce(RightController.transform.forward * 10 * Time.deltaTime * 1000 * BulletSpeed);
        //Debug.Log("shot!");
    }
}
