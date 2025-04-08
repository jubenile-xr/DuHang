using UnityEngine;

public class SetInterruptGunRotation : MonoBehaviour
{
    [SerializeField] private GameObject RHandTargetAnchor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //右手の角度をこのオブジェクトに適用
        this.transform.rotation = RHandTargetAnchor.transform.rotation;
    }
}
