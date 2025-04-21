using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction.Unity.Input;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;

public class PhotonInterruptGun : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject RightController;
    private float bulletSpeed = 100f;
    [SerializeField] private float spanTime = 5f;
    private float recastTime = 0f;
    private bool shotable = true;
    [SerializeField]private GameObject shootSE;
    [SerializeField]private GameObject reloadSE;
    public GameManager gameManager;
    // private Animator animator;

    private void Start()
    {
        // animator = GetComponent<Animator>();

        // gameManagerの取得
        if (gameManager == null)
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            if (gameManager == null)
            {
                Debug.LogError("GameManager not found");
            }
        }
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) || Input.GetKeyDown(KeyCode.Space))
        {
            if (shotable && gameManager.GetGameState() == GameManager.GameState.PLAY)
            {
                Shot();
                shotable = false;
            }
        }
        recastTime += Time.deltaTime;
        if (recastTime > 5.0f){
            if(!shotable)reloadSE?.GetComponent<SoundPlayer>().Play();
            shotable = true;
            recastTime = 0;
        }
    }
    private void Shot()
    {
        GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, RightController.transform.position, transform.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(RightController.transform.forward * Time.deltaTime * 100 * bulletSpeed);
        shootSE?.GetComponent<SoundPlayer>().Play();
    }
}