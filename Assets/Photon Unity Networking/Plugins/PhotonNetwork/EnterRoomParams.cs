using System;
using ExitGames.Client.Photon;

// Token: 0x020002AA RID: 682
internal class EnterRoomParams
{
	// Token: 0x04000E4A RID: 3658
	public string RoomName;

	// Token: 0x04000E4B RID: 3659
	public RoomOptions RoomOptions;

	// Token: 0x04000E4C RID: 3660
	public TypedLobby Lobby;

	// Token: 0x04000E4D RID: 3661
	public Hashtable PlayerProperties;

	// Token: 0x04000E4E RID: 3662
	public bool OnGameServer = true;

	// Token: 0x04000E4F RID: 3663
	public bool CreateIfNotExists;

	// Token: 0x04000E50 RID: 3664
	public bool RejoinOnly;

	// Token: 0x04000E51 RID: 3665
	public string[] ExpectedUsers;
}
