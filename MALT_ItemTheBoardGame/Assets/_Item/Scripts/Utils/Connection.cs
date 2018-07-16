using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class Connection : PunBehaviour {

	public string UserId;
	public string previousRoom;
    public bool connected;

	public void ApplyUserIdAndConnect()
	{
		string nickName = "DemoNick";
		Debug.Log("Nickname: " + nickName + " userID: " + this.UserId,this);

        connected = true;

        if (PhotonNetwork.AuthValues == null)
		{
			PhotonNetwork.AuthValues = new AuthenticationValues();
		}
		//else
		//{
		//    Debug.Log("Re-using AuthValues. UserId: " + PhotonNetwork.AuthValues.UserId);
		//}

		PhotonNetwork.playerName = nickName;
		PhotonNetwork.ConnectUsingSettings("0.5");

		// this way we can force timeouts by pausing the client (in editor)
		PhotonHandler.StopFallbackSendAckThread();
	}


	public override void OnConnectedToMaster()
	{
		// after connect 
		this.UserId = PhotonNetwork.player.UserId;
		////Debug.Log("UserID " + this.UserId);

		// after timeout: re-join "old" room (if one is known)
		if (!string.IsNullOrEmpty(this.previousRoom))
		{
			Debug.Log("ReJoining previous room: " + this.previousRoom);
			PhotonNetwork.ReJoinRoom(this.previousRoom);
			this.previousRoom = null;       // we only will try to re-join once. if this fails, we will get into a random/new room
		}
		else
		{
            // else: join a random room
            //Debug.Log("joinrandomroom");
			//PhotonNetwork.JoinRandomRoom();
		}
	}

	public override void OnJoinedLobby()
	{
		OnConnectedToMaster(); // this way, it does not matter if we join a lobby or not
	}

	public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
	{
        Debug.Log("randomjoinfailed then create");
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 2, PlayerTtl = 5000 }, null);
    }

	public override void OnJoinedRoom()
	{
		Debug.Log("Joined room: " + PhotonNetwork.room.Name);
		this.previousRoom = PhotonNetwork.room.Name;

	}

	public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
	{
		this.previousRoom = null;
	}

	public override void OnConnectionFail(DisconnectCause cause)
	{
		Debug.Log("Disconnected due to: " + cause + ". this.previousRoom: " + this.previousRoom);
	}
}
