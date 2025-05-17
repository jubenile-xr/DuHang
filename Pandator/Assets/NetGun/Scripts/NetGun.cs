using System;
using UnityEngine;
using Photon.Pun;

public class NetGun : MonoBehaviourPun
{
    [SerializeField] private float BulletSpeed = 0.5f;
    [SerializeField] private GameObject RightController;
    [SerializeField] private GameObject Tip;
    private float spanTime = 0f;
    [SerializeField] private SoundPlayer shootSE;
    [SerializeField] private SoundPlayer chargeSE;
    private bool shotable = true;
    private Animator animator;
    public GameManager gameManager;
    void Start()
    {
        animator = GetComponent<Animator>();
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

    void Update()
    {
        // 実験用
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) || Input.GetKeyDown(KeyCode.Space))
        {
            // リロードされてる && 自分のPhotonView && ゲーム状態がPLAY
            if (shotable && photonView.IsMine && gameManager.GetGameState() == GameManager.GameState.PLAY)
            {
                Shot();
                shotable = false;
                spanTime = 0f; // 発射時にカウントをリセット
            }
        }

        // 発射間隔を管理
        if (!shotable)
        {
            spanTime += Time.deltaTime;
            if (spanTime > Durations.NET_GUN_DURATION)
            {
                shotable = true;
                chargeSE.Play();
            }
        }
    }

    public void Shot() // 弾の発射
    {
        animator.SetTrigger("Fire");

        GameObject bulletInstance = PhotonNetwork.Instantiate("InterruptItem/Net", Tip.transform.position, Quaternion.LookRotation(RightController.transform.forward));
        bulletInstance.GetComponent<Rigidbody>().AddForce(-RightController.transform.forward * 20 * Time.deltaTime * 1000 * BulletSpeed);

        shootSE.Play();
    }
}