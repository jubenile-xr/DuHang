using System.Numerics;
using Photon.Pun;
using UnityEngine;

public class PlayerGenerate : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject playerSkin;
    [SerializeField] private GameObject parent;
    [SerializeField] private bool connectable = true;
    private GameObject playerObject;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (connectable && playerObject == null)
        {
            GameObject playerObject = PhotonNetwork.Instantiate(playerSkin.name, parent.transform.position, UnityEngine.Quaternion.LookRotation(parent.transform.forward));
            playerObject.transform.SetParent(parent.transform);
            playerObject.transform.localPosition = parent.transform.position;
            playerObject.transform.localRotation = UnityEngine.Quaternion.LookRotation(parent.transform.forward);
            playerObject.transform.localScale = UnityEngine.Vector3.one;

            connectable = false;
        }
    }
}
