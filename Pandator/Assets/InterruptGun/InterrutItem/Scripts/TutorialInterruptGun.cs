using UnityEngine;

public class TutorialInterruptGun : MonoBehaviour
{
    private bool isKeybord = false;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject RightController;
    private float bulletSpeed = 100f;
    [SerializeField] private float spanTime = 5f;
    private float recastTime = 0f;
    private bool shotable = true;
    [SerializeField] private GameObject shootSE;
    [SerializeField] private GameObject reloadSE;
    private GameObject CenterEyeAnchor;

    void Start()
    {
        CenterEyeAnchor = GameObject.FindWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SetIsKeybord(true);
        }
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (shotable)
            {
                Shot();
            }
        }
        recastTime += Time.deltaTime;
        if (recastTime > Durations.INTERRUPT_GUN_DURATION && !shotable){
            reloadSE?.GetComponent<SoundPlayer>().Play();
            shotable = true;
        }
    }

    private void Shot()
    {
        
        if (isKeybord)
        {
            GameObject bullet = Instantiate(bulletPrefab, CenterEyeAnchor.transform.position, transform.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(CenterEyeAnchor.transform.forward * Time.deltaTime * 100 * bulletSpeed);
        }
        else
        {
            GameObject bullet = Instantiate(bulletPrefab, RightController.transform.position, transform.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(RightController.transform.forward * Time.deltaTime * 100 * bulletSpeed);
        }
        // GameObject bullet = Instantiate(bulletPrefab, RightController.transform.position, transform.rotation);
        // bullet.GetComponent<Rigidbody>().AddForce(RightController.transform.forward * Time.deltaTime * 100 * bulletSpeed);
        shootSE?.GetComponent<SoundPlayer>().Play();
        recastTime = 0f;
        shotable = false;
    }
    
    private void SetIsKeybord(bool isKeybord)
    {
        this.isKeybord = isKeybord;
    }
}
