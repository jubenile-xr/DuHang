using UnityEngine;

public class TutorialInterruptGun : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject RightController;
    private float bulletSpeed = 100f;
    [SerializeField] private float spanTime = 5f;
    private float recastTime = 0f;
    private bool shotable = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) || Input.GetKeyDown(KeyCode.Space))
        {
            if (shotable)
            {
                Shot();
                shotable = false;
            }
        }
        recastTime += Time.deltaTime;
        if (recastTime > 5.0f){
            shotable = true;
            recastTime = 0;
        }
    }

    private void Shot()
    {
        GameObject bullet = Instantiate(bulletPrefab, RightController.transform.position, transform.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(RightController.transform.forward * Time.deltaTime * 100 * bulletSpeed);
    }
}
