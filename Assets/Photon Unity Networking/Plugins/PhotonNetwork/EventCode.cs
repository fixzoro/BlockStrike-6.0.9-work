using System;

// Token: 0x020002AE RID: 686
public class EventCode
{
	// Token: 0x04000E7D RID: 3709
	public const byte GameList = 230;

	// Token: 0x04000E7E RID: 3710
	public const byte GameListUpdate = 229;

	// Token: 0x04000E7F RID: 3711
	public const byte QueueState = 228;

	// Token: 0x04000E80 RID: 3712
	public const byte Match = 227;

	// Token: 0x04000E81 RID: 3713
	public const byte AppStats = 226;

	// Token: 0x04000E82 RID: 3714
	public const byte LobbyStats = 224;

	// Token: 0x04000E83 RID: 3715
	[Obsolete("TCP routing was removed after becoming obsolete.")]
	public const byte AzureNodeInfo = 210;

	// Token: 0x04000E84 RID: 3716
	public const byte Join = 255;

	// Token: 0x04000E85 RID: 3717
	public const byte Leave = 254;

	// Token: 0x04000E86 RID: 3718
	public const byte PropertiesChanged = 253;

	// Token: 0x04000E87 RID: 3719
	[Obsolete("Use PropertiesChanged now.")]
	public const byte SetProperties = 253;

	// Token: 0x04000E88 RID: 3720
	public const byte ErrorInfo = 251;

	// Token: 0x04000E89 RID: 3721
	public const byte CacheSliceChanged = 250;

	// Token: 0x04000E8A RID: 3722
	public const byte AuthEvent = 223;
}
