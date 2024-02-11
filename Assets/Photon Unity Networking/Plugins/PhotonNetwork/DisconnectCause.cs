using System;

// Token: 0x020002C0 RID: 704
public enum DisconnectCause
{
	// Token: 0x04000F4B RID: 3915
	DisconnectByServerUserLimit = 1042,
	// Token: 0x04000F4C RID: 3916
	ExceptionOnConnect = 1023,
	// Token: 0x04000F4D RID: 3917
	DisconnectByServerTimeout = 1041,
	// Token: 0x04000F4E RID: 3918
	DisconnectByServerLogic = 1043,
	// Token: 0x04000F4F RID: 3919
	Exception = 1026,
	// Token: 0x04000F50 RID: 3920
	InvalidAuthentication = 32767,
	// Token: 0x04000F51 RID: 3921
	MaxCcuReached = 32757,
	// Token: 0x04000F52 RID: 3922
	InvalidRegion = 32756,
	// Token: 0x04000F53 RID: 3923
	SecurityExceptionOnConnect = 1022,
	// Token: 0x04000F54 RID: 3924
	DisconnectByClientTimeout = 1040,
	// Token: 0x04000F55 RID: 3925
	InternalReceiveException = 1039,
	// Token: 0x04000F56 RID: 3926
	AuthenticationTicketExpired = 32753
}
