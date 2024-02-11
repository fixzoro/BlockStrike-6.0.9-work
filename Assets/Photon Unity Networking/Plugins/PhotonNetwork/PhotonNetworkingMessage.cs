using System;

// Token: 0x0200029A RID: 666
public enum PhotonNetworkingMessage
{
	// Token: 0x04000DE1 RID: 3553
	OnConnectedToPhoton,
	// Token: 0x04000DE2 RID: 3554
	OnLeftRoom,
	// Token: 0x04000DE3 RID: 3555
	OnMasterClientSwitched,
	// Token: 0x04000DE4 RID: 3556
	OnPhotonCreateRoomFailed,
	// Token: 0x04000DE5 RID: 3557
	OnPhotonJoinRoomFailed,
	// Token: 0x04000DE6 RID: 3558
	OnCreatedRoom,
	// Token: 0x04000DE7 RID: 3559
	OnJoinedLobby,
	// Token: 0x04000DE8 RID: 3560
	OnLeftLobby,
	// Token: 0x04000DE9 RID: 3561
	OnDisconnectedFromPhoton,
	// Token: 0x04000DEA RID: 3562
	OnConnectionFail,
	// Token: 0x04000DEB RID: 3563
	OnFailedToConnectToPhoton,
	// Token: 0x04000DEC RID: 3564
	OnReceivedRoomListUpdate,
	// Token: 0x04000DED RID: 3565
	OnJoinedRoom,
	// Token: 0x04000DEE RID: 3566
	OnPhotonPlayerConnected,
	// Token: 0x04000DEF RID: 3567
	OnPhotonPlayerDisconnected,
	// Token: 0x04000DF0 RID: 3568
	OnPhotonRandomJoinFailed,
	// Token: 0x04000DF1 RID: 3569
	OnConnectedToMaster,
	// Token: 0x04000DF2 RID: 3570
	OnPhotonSerializeView,
	// Token: 0x04000DF3 RID: 3571
	OnPhotonInstantiate,
	// Token: 0x04000DF4 RID: 3572
	OnPhotonMaxCccuReached,
	// Token: 0x04000DF5 RID: 3573
	OnPhotonCustomRoomPropertiesChanged,
	// Token: 0x04000DF6 RID: 3574
	OnPhotonPlayerPropertiesChanged,
	// Token: 0x04000DF7 RID: 3575
	OnUpdatedFriendList,
	// Token: 0x04000DF8 RID: 3576
	OnCustomAuthenticationFailed,
	// Token: 0x04000DF9 RID: 3577
	OnCustomAuthenticationResponse,
	// Token: 0x04000DFA RID: 3578
	OnWebRpcResponse,
	// Token: 0x04000DFB RID: 3579
	OnOwnershipRequest,
	// Token: 0x04000DFC RID: 3580
	OnLobbyStatisticsUpdate,
	// Token: 0x04000DFD RID: 3581
	OnPhotonPlayerActivityChanged,
	// Token: 0x04000DFE RID: 3582
	OnOwnershipTransfered
}
