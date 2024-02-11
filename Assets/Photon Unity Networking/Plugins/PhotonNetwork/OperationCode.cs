using System;

// Token: 0x020002B0 RID: 688
public class OperationCode
{
	// Token: 0x04000ECE RID: 3790
	[Obsolete("Exchanging encrpytion keys is done internally in the lib now. Don't expect this operation-result.")]
	public const byte ExchangeKeysForEncryption = 250;

	// Token: 0x04000ECF RID: 3791
	[Obsolete]
	public const byte Join = 255;

	// Token: 0x04000ED0 RID: 3792
	public const byte AuthenticateOnce = 231;

	// Token: 0x04000ED1 RID: 3793
	public const byte Authenticate = 230;

	// Token: 0x04000ED2 RID: 3794
	public const byte JoinLobby = 229;

	// Token: 0x04000ED3 RID: 3795
	public const byte LeaveLobby = 228;

	// Token: 0x04000ED4 RID: 3796
	public const byte CreateGame = 227;

	// Token: 0x04000ED5 RID: 3797
	public const byte JoinGame = 226;

	// Token: 0x04000ED6 RID: 3798
	public const byte JoinRandomGame = 225;

	// Token: 0x04000ED7 RID: 3799
	public const byte Leave = 254;

	// Token: 0x04000ED8 RID: 3800
	public const byte RaiseEvent = 253;

	// Token: 0x04000ED9 RID: 3801
	public const byte SetProperties = 252;

	// Token: 0x04000EDA RID: 3802
	public const byte GetProperties = 251;

	// Token: 0x04000EDB RID: 3803
	public const byte ChangeGroups = 248;

	// Token: 0x04000EDC RID: 3804
	public const byte FindFriends = 222;

	// Token: 0x04000EDD RID: 3805
	public const byte GetLobbyStats = 221;

	// Token: 0x04000EDE RID: 3806
	public const byte GetRegions = 220;

	// Token: 0x04000EDF RID: 3807
	public const byte WebRpc = 219;

	// Token: 0x04000EE0 RID: 3808
	public const byte ServerSettings = 218;

	// Token: 0x04000EE1 RID: 3809
	public const byte GetGameList = 217;
}
