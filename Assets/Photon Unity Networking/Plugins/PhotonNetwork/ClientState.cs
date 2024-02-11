using System;

// Token: 0x020002BE RID: 702
public enum ClientState
{
	// Token: 0x04000F30 RID: 3888
	Uninitialized,
	// Token: 0x04000F31 RID: 3889
	PeerCreated,
	// Token: 0x04000F32 RID: 3890
	Queued,
	// Token: 0x04000F33 RID: 3891
	Authenticated,
	// Token: 0x04000F34 RID: 3892
	JoinedLobby,
	// Token: 0x04000F35 RID: 3893
	DisconnectingFromMasterserver,
	// Token: 0x04000F36 RID: 3894
	ConnectingToGameserver,
	// Token: 0x04000F37 RID: 3895
	ConnectedToGameserver,
	// Token: 0x04000F38 RID: 3896
	Joining,
	// Token: 0x04000F39 RID: 3897
	Joined,
	// Token: 0x04000F3A RID: 3898
	Leaving,
	// Token: 0x04000F3B RID: 3899
	DisconnectingFromGameserver,
	// Token: 0x04000F3C RID: 3900
	ConnectingToMasterserver,
	// Token: 0x04000F3D RID: 3901
	QueuedComingFromGameserver,
	// Token: 0x04000F3E RID: 3902
	Disconnecting,
	// Token: 0x04000F3F RID: 3903
	Disconnected,
	// Token: 0x04000F40 RID: 3904
	ConnectedToMaster,
	// Token: 0x04000F41 RID: 3905
	ConnectingToNameServer,
	// Token: 0x04000F42 RID: 3906
	ConnectedToNameServer,
	// Token: 0x04000F43 RID: 3907
	DisconnectingFromNameServer,
	// Token: 0x04000F44 RID: 3908
	Authenticating
}
