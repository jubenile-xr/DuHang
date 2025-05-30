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
            gameManager = GameObject.Find("GameManager(Clone)").GetComponent<GameManager>();
            if (gameManager == null)
            {
                Debug.LogError("GameManager not found");
            }
        }
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) || Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (shotable && gameManager.GetGameState() == GameManager.GameState.PLAY)
            {
                Shot();
            }
        }
        if(!shotable)
        {
            recastTime += Time.deltaTime;
            if (recastTime > Durations.INTERRUPT_GUN_DURATION && !shotable)
            {
                reloadSE?.GetComponent<SoundPlayer>().Play();
                shotable = true;
            }
        }
    }
    private void Shot()
    {
        GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, RightController.transform.position, transform.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(RightController.transform.forward * Time.deltaTime * 100 * bulletSpeed);
        shootSE?.GetComponent<SoundPlayer>().Play();
        recastTime = 0f;
        shotable = false;
    }
}