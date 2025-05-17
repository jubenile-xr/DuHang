using UnityEngine;
using Photon.Pun;
public class InterruptGun : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 500;
    // private Animator animator;

    private void Start()
    {
        // animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(transform.forward * -bulletSpeed);
        }
    }
}