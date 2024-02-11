using System;

// Token: 0x020002A4 RID: 676
public class FriendInfo
{
	// Token: 0x1700036C RID: 876
	// (get) Token: 0x060019CA RID: 6602 RVA: 0x00012EE0 File Offset: 0x000110E0
	[Obsolete("Use UserId.")]
	public string Name
	{
		get
		{
			return this.UserId;
		}
	}

	// Token: 0x1700036D RID: 877
	// (get) Token: 0x060019CB RID: 6603 RVA: 0x00012EE8 File Offset: 0x000110E8
	// (set) Token: 0x060019CC RID: 6604 RVA: 0x00012EF0 File Offset: 0x000110F0
	public string UserId { get; protected internal set; }

	// Token: 0x1700036E RID: 878
	// (get) Token: 0x060019CD RID: 6605 RVA: 0x00012EF9 File Offset: 0x000110F9
	// (set) Token: 0x060019CE RID: 6606 RVA: 0x00012F01 File Offset: 0x00011101
	public bool IsOnline { get; protected internal set; }

	// Token: 0x1700036F RID: 879
	// (get) Token: 0x060019CF RID: 6607 RVA: 0x00012F0A File Offset: 0x0001110A
	// (set) Token: 0x060019D0 RID: 6608 RVA: 0x00012F12 File Offset: 0x00011112
	public string Room { get; protected internal set; }

	// Token: 0x17000370 RID: 880
	// (get) Token: 0x060019D1 RID: 6609 RVA: 0x00012F1B File Offset: 0x0001111B
	public bool IsInRoom
	{
		get
		{
			return this.IsOnline && !string.IsNullOrEmpty(this.Room);
		}
	}

	// Token: 0x060019D2 RID: 6610 RVA: 0x000A9548 File Offset: 0x000A7748
	public override string ToString()
	{
		return string.Format("{0}\t is: {1}", this.UserId, this.IsOnline ? ((!this.IsInRoom) ? "on master" : "playing") : "offline");
	}
}
