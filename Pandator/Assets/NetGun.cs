using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NetGun : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("GunComponent")]
    [SerializeField] private GameObject BulletType;
    [SerializeField] private float BulletSpeed = 10f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //実験用
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            Shot();
        }
    }

    public void Shot() //弾の発射
    {
        //弾の発射位置(transform.position)は再考の余地あり
        GameObject bulletInstance = Instantiate(BulletType, transform.position, Quaternion.LookRotation(transform.forward));
        bulletInstance.GetComponent<Rigidbody>().AddForce(transform.forward * BulletSpeed);
    }
}
