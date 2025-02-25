using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ConnectionManager : MonoBehaviourPunCallbacks
{
    private string roomName = "Pandator";
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
    }

    public override void OnJoinRoomFailed(short returnCode, string message) {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }
    
}
