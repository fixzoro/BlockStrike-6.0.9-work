using System;

// Token: 0x020002AB RID: 683
public class ErrorCode
{
	// Token: 0x04000E52 RID: 3666
	public const int Ok = 0;

	// Token: 0x04000E53 RID: 3667
	public const int OperationNotAllowedInCurrentState = -3;

	// Token: 0x04000E54 RID: 3668
	[Obsolete("Use InvalidOperation.")]
	public const int InvalidOperationCode = -2;

	// Token: 0x04000E55 RID: 3669
	public const int InvalidOperation = -2;

	// Token: 0x04000E56 RID: 3670
	public const int InternalServerError = -1;

	// Token: 0x04000E57 RID: 3671
	public const int InvalidAuthentication = 32767;

	// Token: 0x04000E58 RID: 3672
	public const int GameIdAlreadyExists = 32766;

	// Token: 0x04000E59 RID: 3673
	public const int GameFull = 32765;

	// Token: 0x04000E5A RID: 3674
	public const int GameClosed = 32764;

	// Token: 0x04000E5B RID: 3675
	[Obsolete("No longer used, cause random matchmaking is no longer a process.")]
	public const int AlreadyMatched = 32763;

	// Token: 0x04000E5C RID: 3676
	public const int ServerFull = 32762;

	// Token: 0x04000E5D RID: 3677
	public const int UserBlocked = 32761;

	// Token: 0x04000E5E RID: 3678
	public const int NoRandomMatchFound = 32760;

	// Token: 0x04000E5F RID: 3679
	public const int GameDoesNotExist = 32758;

	// Token: 0x04000E60 RID: 3680
	public const int MaxCcuReached = 32757;

	// Token: 0x04000E61 RID: 3681
	public const int InvalidRegion = 32756;

	// Token: 0x04000E62 RID: 3682
	public const int CustomAuthenticationFailed = 32755;

	// Token: 0x04000E63 RID: 3683
	public const int AuthenticationTicketExpired = 32753;

	// Token: 0x04000E64 RID: 3684
	public const int PluginReportedError = 32752;

	// Token: 0x04000E65 RID: 3685
	public const int PluginMismatch = 32751;

	// Token: 0x04000E66 RID: 3686
	public const int JoinFailedPeerAlreadyJoined = 32750;

	// Token: 0x04000E67 RID: 3687
	public const int JoinFailedFoundInactiveJoiner = 32749;

	// Token: 0x04000E68 RID: 3688
	public const int JoinFailedWithRejoinerNotFound = 32748;

	// Token: 0x04000E69 RID: 3689
	public const int JoinFailedFoundExcludedUserId = 32747;

	// Token: 0x04000E6A RID: 3690
	public const int JoinFailedFoundActiveJoiner = 32746;

	// Token: 0x04000E6B RID: 3691
	public const int HttpLimitReached = 32745;

	// Token: 0x04000E6C RID: 3692
	public const int ExternalHttpCallFailed = 32744;

	// Token: 0x04000E6D RID: 3693
	public const int SlotError = 32742;

	// Token: 0x04000E6E RID: 3694
	public const int InvalidEncryptionParameters = 32741;
}
