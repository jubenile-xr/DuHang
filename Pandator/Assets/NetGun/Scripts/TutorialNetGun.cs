using UnityEngine;
using System;
public class TutorialNetGun : MonoBehaviour
{
    [SerializeField] private float BulletSpeed = 0.5f;
    [SerializeField] private GameObject RightController;
    [SerializeField] private GameObject Tip;
    [SerializeField] private float spanTime = 0f;
    private bool shotable = true;
    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // 実験用
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) || Input.GetKeyDown(KeyCode.G))
        {
            if (shotable)
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
            if (spanTime > 5.0f)
            {
                shotable = true; // 5秒後に発射可能にする
            }
        }
    }

    public void Shot() //弾の発射
    {
        animator.SetTrigger("Fire");

        GameObject bulletInstance = Instantiate(Resources.Load<GameObject>("InterruptItem/Net"), Tip.transform.position, Quaternion.LookRotation(RightController.transform.forward));
        bulletInstance.GetComponent<Rigidbody>().AddForce(-RightController.transform.forward * 20 * Time.deltaTime * 1000 * BulletSpeed);
    }
}
