using System;
using ExitGames.Client.Photon;

// Token: 0x020002B6 RID: 694
public class RoomOptions
{
	// Token: 0x17000372 RID: 882
	// (get) Token: 0x060019F5 RID: 6645 RVA: 0x00012FFC File Offset: 0x000111FC
	// (set) Token: 0x060019F6 RID: 6646 RVA: 0x00013004 File Offset: 0x00011204
	public bool IsVisible
	{
		get
		{
			return this.isVisibleField;
		}
		set
		{
			this.isVisibleField = value;
		}
	}

	// Token: 0x17000373 RID: 883
	// (get) Token: 0x060019F7 RID: 6647 RVA: 0x0001300D File Offset: 0x0001120D
	// (set) Token: 0x060019F8 RID: 6648 RVA: 0x00013015 File Offset: 0x00011215
	public bool IsOpen
	{
		get
		{
			return this.isOpenField;
		}
		set
		{
			this.isOpenField = value;
		}
	}

	// Token: 0x17000374 RID: 884
	// (get) Token: 0x060019F9 RID: 6649 RVA: 0x0001301E File Offset: 0x0001121E
	// (set) Token: 0x060019FA RID: 6650 RVA: 0x00013026 File Offset: 0x00011226
	public bool CleanupCacheOnLeave
	{
		get
		{
			return this.cleanupCacheOnLeaveField;
		}
		set
		{
			this.cleanupCacheOnLeaveField = value;
		}
	}

	// Token: 0x17000375 RID: 885
	// (get) Token: 0x060019FB RID: 6651 RVA: 0x0001302F File Offset: 0x0001122F
	public bool SuppressRoomEvents
	{
		get
		{
			return this.suppressRoomEventsField;
		}
	}

	// Token: 0x17000376 RID: 886
	// (get) Token: 0x060019FC RID: 6652 RVA: 0x00013037 File Offset: 0x00011237
	// (set) Token: 0x060019FD RID: 6653 RVA: 0x0001303F File Offset: 0x0001123F
	public bool PublishUserId
	{
		get
		{
			return this.publishUserIdField;
		}
		set
		{
			this.publishUserIdField = value;
		}
	}

	// Token: 0x17000377 RID: 887
	// (get) Token: 0x060019FE RID: 6654 RVA: 0x00013048 File Offset: 0x00011248
	// (set) Token: 0x060019FF RID: 6655 RVA: 0x00013050 File Offset: 0x00011250
	public bool DeleteNullProperties
	{
		get
		{
			return this.deleteNullPropertiesField;
		}
		set
		{
			this.deleteNullPropertiesField = value;
		}
	}

	// Token: 0x17000378 RID: 888
	// (get) Token: 0x06001A00 RID: 6656 RVA: 0x00012FFC File Offset: 0x000111FC
	// (set) Token: 0x06001A01 RID: 6657 RVA: 0x00013004 File Offset: 0x00011204
	[Obsolete("Use property with uppercase naming instead.")]
	public bool isVisible
	{
		get
		{
			return this.isVisibleField;
		}
		set
		{
			this.isVisibleField = value;
		}
	}

	// Token: 0x17000379 RID: 889
	// (get) Token: 0x06001A02 RID: 6658 RVA: 0x0001300D File Offset: 0x0001120D
	// (set) Token: 0x06001A03 RID: 6659 RVA: 0x00013015 File Offset: 0x00011215
	[Obsolete("Use property with uppercase naming instead.")]
	public bool isOpen
	{
		get
		{
			return this.isOpenField;
		}
		set
		{
			this.isOpenField = value;
		}
	}

	// Token: 0x1700037A RID: 890
	// (get) Token: 0x06001A04 RID: 6660 RVA: 0x00013059 File Offset: 0x00011259
	// (set) Token: 0x06001A05 RID: 6661 RVA: 0x00013061 File Offset: 0x00011261
	[Obsolete("Use property with uppercase naming instead.")]
	public byte maxPlayers
	{
		get
		{
			return this.MaxPlayers;
		}
		set
		{
			this.MaxPlayers = value;
		}
	}

	// Token: 0x1700037B RID: 891
	// (get) Token: 0x06001A06 RID: 6662 RVA: 0x0001301E File Offset: 0x0001121E
	// (set) Token: 0x06001A07 RID: 6663 RVA: 0x00013026 File Offset: 0x00011226
	[Obsolete("Use property with uppercase naming instead.")]
	public bool cleanupCacheOnLeave
	{
		get
		{
			return this.cleanupCacheOnLeaveField;
		}
		set
		{
			this.cleanupCacheOnLeaveField = value;
		}
	}

	// Token: 0x1700037C RID: 892
	// (get) Token: 0x06001A08 RID: 6664 RVA: 0x0001306A File Offset: 0x0001126A
	// (set) Token: 0x06001A09 RID: 6665 RVA: 0x00013072 File Offset: 0x00011272
	[Obsolete("Use property with uppercase naming instead.")]
	public Hashtable customRoomProperties
	{
		get
		{
			return this.CustomRoomProperties;
		}
		set
		{
			this.CustomRoomProperties = value;
		}
	}

	// Token: 0x1700037D RID: 893
	// (get) Token: 0x06001A0A RID: 6666 RVA: 0x0001307B File Offset: 0x0001127B
	// (set) Token: 0x06001A0B RID: 6667 RVA: 0x00013083 File Offset: 0x00011283
	[Obsolete("Use property with uppercase naming instead.")]
	public string[] customRoomPropertiesForLobby
	{
		get
		{
			return this.CustomRoomPropertiesForLobby;
		}
		set
		{
			this.CustomRoomPropertiesForLobby = value;
		}
	}

	// Token: 0x1700037E RID: 894
	// (get) Token: 0x06001A0C RID: 6668 RVA: 0x0001308C File Offset: 0x0001128C
	// (set) Token: 0x06001A0D RID: 6669 RVA: 0x00013094 File Offset: 0x00011294
	[Obsolete("Use property with uppercase naming instead.")]
	public string[] plugins
	{
		get
		{
			return this.Plugins;
		}
		set
		{
			this.Plugins = value;
		}
	}

	// Token: 0x1700037F RID: 895
	// (get) Token: 0x06001A0E RID: 6670 RVA: 0x0001302F File Offset: 0x0001122F
	[Obsolete("Use property with uppercase naming instead.")]
	public bool suppressRoomEvents
	{
		get
		{
			return this.suppressRoomEventsField;
		}
	}

	// Token: 0x17000380 RID: 896
	// (get) Token: 0x06001A0F RID: 6671 RVA: 0x00013037 File Offset: 0x00011237
	// (set) Token: 0x06001A10 RID: 6672 RVA: 0x0001303F File Offset: 0x0001123F
	[Obsolete("Use property with uppercase naming instead.")]
	public bool publishUserId
	{
		get
		{
			return this.publishUserIdField;
		}
		set
		{
			this.publishUserIdField = value;
		}
	}

	// Token: 0x04000F01 RID: 3841
	private bool isVisibleField = true;

	// Token: 0x04000F02 RID: 3842
	private bool isOpenField = true;

	// Token: 0x04000F03 RID: 3843
	public byte MaxPlayers;

	// Token: 0x04000F04 RID: 3844
	public int PlayerTtl;

	// Token: 0x04000F05 RID: 3845
	public int EmptyRoomTtl;

	// Token: 0x04000F06 RID: 3846
	private bool cleanupCacheOnLeaveField = PhotonNetwork.autoCleanUpPlayerObjects;

	// Token: 0x04000F07 RID: 3847
	public Hashtable CustomRoomProperties;

	// Token: 0x04000F08 RID: 3848
	public string[] CustomRoomPropertiesForLobby = new string[0];

	// Token: 0x04000F09 RID: 3849
	public string[] Plugins;

	// Token: 0x04000F0A RID: 3850
	private bool suppressRoomEventsField;

	// Token: 0x04000F0B RID: 3851
	private bool publishUserIdField;

	// Token: 0x04000F0C RID: 3852
	private bool deleteNullPropertiesField;
}
