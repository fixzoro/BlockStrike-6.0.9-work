using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;

// Token: 0x020002C6 RID: 710
public interface IPunCallbacks
{
	// Token: 0x06001AB6 RID: 6838
	void OnConnectedToPhoton();

	// Token: 0x06001AB7 RID: 6839
	void OnLeftRoom();

	// Token: 0x06001AB8 RID: 6840
	void OnMasterClientSwitched(PhotonPlayer newMasterClient);

	// Token: 0x06001AB9 RID: 6841
	void OnPhotonCreateRoomFailed(object[] codeAndMsg);

	// Token: 0x06001ABA RID: 6842
	void OnPhotonJoinRoomFailed(object[] codeAndMsg);

	// Token: 0x06001ABB RID: 6843
	void OnCreatedRoom();

	// Token: 0x06001ABC RID: 6844
	void OnJoinedLobby();

	// Token: 0x06001ABD RID: 6845
	void OnLeftLobby();

	// Token: 0x06001ABE RID: 6846
	void OnFailedToConnectToPhoton(DisconnectCause cause);

	// Token: 0x06001ABF RID: 6847
	void OnConnectionFail(DisconnectCause cause);

	// Token: 0x06001AC0 RID: 6848
	void OnDisconnectedFromPhoton();

	// Token: 0x06001AC1 RID: 6849
	void OnPhotonInstantiate(PhotonMessageInfo info);

	// Token: 0x06001AC2 RID: 6850
	void OnReceivedRoomListUpdate();

	// Token: 0x06001AC3 RID: 6851
	void OnJoinedRoom();

	// Token: 0x06001AC4 RID: 6852
	void OnPhotonPlayerConnected(PhotonPlayer newPlayer);

	// Token: 0x06001AC5 RID: 6853
	void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer);

	// Token: 0x06001AC6 RID: 6854
	void OnPhotonRandomJoinFailed(object[] codeAndMsg);

	// Token: 0x06001AC7 RID: 6855
	void OnConnectedToMaster();

	// Token: 0x06001AC8 RID: 6856
	void OnPhotonMaxCccuReached();

	// Token: 0x06001AC9 RID: 6857
	void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged);

	// Token: 0x06001ACA RID: 6858
	void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps);

	// Token: 0x06001ACB RID: 6859
	void OnUpdatedFriendList();

	// Token: 0x06001ACC RID: 6860
	void OnCustomAuthenticationFailed(string debugMessage);

	// Token: 0x06001ACD RID: 6861
	void OnCustomAuthenticationResponse(Dictionary<string, object> data);

	// Token: 0x06001ACE RID: 6862
	void OnWebRpcResponse(OperationResponse response);

	// Token: 0x06001ACF RID: 6863
	void OnOwnershipRequest(object[] viewAndPlayer);

	// Token: 0x06001AD0 RID: 6864
	void OnLobbyStatisticsUpdate();

	// Token: 0x06001AD1 RID: 6865
	void OnPhotonPlayerActivityChanged(PhotonPlayer otherPlayer);

	// Token: 0x06001AD2 RID: 6866
	void OnOwnershipTransfered(object[] viewAndPlayers);
}
