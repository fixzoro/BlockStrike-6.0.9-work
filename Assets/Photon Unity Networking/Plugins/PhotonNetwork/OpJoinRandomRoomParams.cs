using System;
using ExitGames.Client.Photon;

// Token: 0x020002A9 RID: 681
internal class OpJoinRandomRoomParams
{
	// Token: 0x04000E44 RID: 3652
	public Hashtable ExpectedCustomRoomProperties;

	// Token: 0x04000E45 RID: 3653
	public byte ExpectedMaxPlayers;

	// Token: 0x04000E46 RID: 3654
	public MatchmakingMode MatchingType;

	// Token: 0x04000E47 RID: 3655
	public TypedLobby TypedLobby;

	// Token: 0x04000E48 RID: 3656
	public string SqlLobbyFilter;

	// Token: 0x04000E49 RID: 3657
	public string[] ExpectedUsers;
}
