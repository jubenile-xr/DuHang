using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RandomMatchMaker : MonoBehaviourPunCallbacks
{
    public GameObject PhotonObject;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 8;
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonObject == null)
        {
            Debug.LogError("PhotonObject is not set in the inspector.");
            return;
        }

        PhotonNetwork.Instantiate(PhotonObject.name, new Vector3(0f, 1f, 0f), Quaternion.identity, 0);

        GameObject mainCamera = GameObject.FindWithTag("MainCamera");
        if (mainCamera != null)
        {
            mainCamera.GetComponent<UnityChan.ThirdPersonCamera>().enabled = true;
        }
        else
        {
            Debug.LogError("Main Camera not found.");
        }
    }
}
