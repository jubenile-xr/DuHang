using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ConnectionManager : MonoBehaviourPunCallbacks
{
    private string roomName = "Pandator";
    void Start()
    {
        //接続
        PhotonNetwork.ConnectUsingSettings();
    }
    
    //マスターサーバーへ接続できたら呼ばれる
    public override void OnConnectedToMaster()
    {
        //Pandatorルームへの入室
        PhotonNetwork.JoinRoom(roomName);
    }

    //ルームに入室されたら呼ばれる
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
    }
    
    // ルーム入室に失敗されたら呼び出される
    // ルーム入室に失敗したら部屋ができていない可能性が高いので部屋を作るコードを書いている。
    public override void OnJoinRoomFailed(short returnCode, string message) {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }
    
}
