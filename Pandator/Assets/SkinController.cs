using UnityEngine;

public class SkinController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject playerObject;
    [SerializeField] private GameObject playerSkin;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerSkin.transform.position = playerObject.transform.position;
    }
}
