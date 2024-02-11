using System;
using ExitGames.Client.Photon;

// Token: 0x020002E9 RID: 745
public class RoomInfo
{
	// Token: 0x06001C6F RID: 7279 RVA: 0x0001481F File Offset: 0x00012A1F
	protected internal RoomInfo(string roomName, Hashtable properties)
	{
		this.InternalCacheProperties(properties);
		this.nameField = roomName;
	}

	// Token: 0x17000407 RID: 1031
	// (get) Token: 0x06001C70 RID: 7280 RVA: 0x00014859 File Offset: 0x00012A59
	// (set) Token: 0x06001C71 RID: 7281 RVA: 0x00014861 File Offset: 0x00012A61
	public bool removedFromList { get; internal set; }

	// Token: 0x17000408 RID: 1032
	// (get) Token: 0x06001C72 RID: 7282 RVA: 0x0001486A File Offset: 0x00012A6A
	// (set) Token: 0x06001C73 RID: 7283 RVA: 0x00014872 File Offset: 0x00012A72
	protected internal bool serverSideMasterClient { get; private set; }

	// Token: 0x17000409 RID: 1033
	// (get) Token: 0x06001C74 RID: 7284 RVA: 0x0001487B File Offset: 0x00012A7B
	public Hashtable CustomProperties
	{
		get
		{
			return this.customPropertiesField;
		}
	}

	// Token: 0x1700040A RID: 1034
	// (get) Token: 0x06001C75 RID: 7285 RVA: 0x0001470E File Offset: 0x0001290E
	public string Name
	{
		get
		{
			return this.nameField;
		}
	}

	// Token: 0x1700040B RID: 1035
	// (get) Token: 0x06001C76 RID: 7286 RVA: 0x00014883 File Offset: 0x00012A83
	// (set) Token: 0x06001C77 RID: 7287 RVA: 0x0001488B File Offset: 0x00012A8B
	public int PlayerCount { get; private set; }

	// Token: 0x1700040C RID: 1036
	// (get) Token: 0x06001C78 RID: 7288 RVA: 0x00014894 File Offset: 0x00012A94
	// (set) Token: 0x06001C79 RID: 7289 RVA: 0x0001489C File Offset: 0x00012A9C
	public bool IsLocalClientInside { get; set; }

	// Token: 0x1700040D RID: 1037
	// (get) Token: 0x06001C7A RID: 7290 RVA: 0x00014748 File Offset: 0x00012948
	public byte MaxPlayers
	{
		get
		{
			return this.maxPlayersField;
		}
	}

	// Token: 0x1700040E RID: 1038
	// (get) Token: 0x06001C7B RID: 7291 RVA: 0x0001471F File Offset: 0x0001291F
	public bool IsOpen
	{
		get
		{
			return this.openField;
		}
	}

	// Token: 0x1700040F RID: 1039
	// (get) Token: 0x06001C7C RID: 7292 RVA: 0x00014727 File Offset: 0x00012927
	public bool IsVisible
	{
		get
		{
			return this.visibleField;
		}
	}

	// Token: 0x06001C7D RID: 7293 RVA: 0x000B3F24 File Offset: 0x000B2124
	public override bool Equals(object other)
	{
		RoomInfo roomInfo = other as RoomInfo;
		return roomInfo != null && this.Name.Equals(roomInfo.nameField);
	}

	// Token: 0x06001C7E RID: 7294 RVA: 0x000148A5 File Offset: 0x00012AA5
	public override int GetHashCode()
	{
		return this.nameField.GetHashCode();
	}

	// Token: 0x06001C7F RID: 7295 RVA: 0x000B3F54 File Offset: 0x000B2154
	public override string ToString()
	{
		return string.Format("Room: '{0}' {1},{2} {4}/{3} players.", new object[]
		{
			this.nameField,
			(!this.visibleField) ? "hidden" : "visible",
			(!this.openField) ? "closed" : "open",
			this.maxPlayersField,
			this.PlayerCount
		});
	}

	// Token: 0x06001C80 RID: 7296 RVA: 0x000B3FD0 File Offset: 0x000B21D0
	public string ToStringFull()
	{
		return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", new object[]
		{
			this.nameField,
			(!this.visibleField) ? "hidden" : "visible",
			(!this.openField) ? "closed" : "open",
			this.maxPlayersField,
			this.PlayerCount,
			this.customPropertiesField.ToStringFull()
		});
	}

	// Token: 0x06001C81 RID: 7297 RVA: 0x000B405C File Offset: 0x000B225C
	protected internal void InternalCacheProperties(Hashtable propertiesToCache)
	{
		if (propertiesToCache == null || propertiesToCache.Count == 0 || this.customPropertiesField.Equals(propertiesToCache))
		{
			return;
		}
		if (propertiesToCache.ContainsKey(GamePropertyKey.Removed))
		{
			this.removedFromList = (bool)propertiesToCache[GamePropertyKey.Removed];
			if (this.removedFromList)
			{
				return;
			}
		}
		if (propertiesToCache.ContainsKey(GamePropertyKey.MaxPlayers))
		{
			this.maxPlayersField = (byte)propertiesToCache[GamePropertyKey.MaxPlayers];
		}
		if (propertiesToCache.ContainsKey(GamePropertyKey.IsOpen))
		{
			this.openField = (bool)propertiesToCache[GamePropertyKey.IsOpen];
		}
		if (propertiesToCache.ContainsKey(GamePropertyKey.IsVisible))
		{
			this.visibleField = (bool)propertiesToCache[GamePropertyKey.IsVisible];
		}
		if (propertiesToCache.ContainsKey(GamePropertyKey.PlayerCount))
		{
			this.PlayerCount = (int)((byte)propertiesToCache[GamePropertyKey.PlayerCount]);
		}
		if (propertiesToCache.ContainsKey(GamePropertyKey.CleanupCacheOnLeave))
		{
			this.autoCleanUpField = (bool)propertiesToCache[GamePropertyKey.CleanupCacheOnLeave];
		}
		if (propertiesToCache.ContainsKey(GamePropertyKey.MasterClientId))
		{
			this.serverSideMasterClient = true;
			bool flag = this.masterClientIdField != 0;
			this.masterClientIdField = (int)propertiesToCache[GamePropertyKey.MasterClientId];
			if (flag)
			{
				PhotonNetwork.networkingPeer.UpdateMasterClient();
			}
		}
		if (propertiesToCache.ContainsKey(GamePropertyKey.ExpectedUsers))
		{
			this.expectedUsersField = (string[])propertiesToCache[GamePropertyKey.ExpectedUsers];
		}
		if (propertiesToCache.ContainsKey(GamePropertyKey.EmptyRoomTtl))
		{
			this.emptyRoomTtlField = (int)propertiesToCache[GamePropertyKey.EmptyRoomTtl];
		}
		if (propertiesToCache.ContainsKey(GamePropertyKey.PlayerTtl))
		{
			this.playerTtlField = (int)propertiesToCache[GamePropertyKey.PlayerTtl];
		}
		this.customPropertiesField.MergeStringKeys(propertiesToCache);
		this.customPropertiesField.StripKeysWithNullValues();
	}

	// Token: 0x17000410 RID: 1040
	// (get) Token: 0x06001C82 RID: 7298 RVA: 0x000148B2 File Offset: 0x00012AB2
	[Obsolete("Please use CustomProperties (updated case for naming).")]
	public Hashtable customProperties
	{
		get
		{
			return this.CustomProperties;
		}
	}

	// Token: 0x17000411 RID: 1041
	// (get) Token: 0x06001C83 RID: 7299 RVA: 0x000148BA File Offset: 0x00012ABA
	[Obsolete("Please use Name (updated case for naming).")]
	public string name
	{
		get
		{
			return this.Name;
		}
	}

	// Token: 0x17000412 RID: 1042
	// (get) Token: 0x06001C84 RID: 7300 RVA: 0x000148C2 File Offset: 0x00012AC2
	// (set) Token: 0x06001C85 RID: 7301 RVA: 0x000148CA File Offset: 0x00012ACA
	[Obsolete("Please use PlayerCount (updated case for naming).")]
	public int playerCount
	{
		get
		{
			return this.PlayerCount;
		}
		set
		{
			this.PlayerCount = value;
		}
	}

	// Token: 0x17000413 RID: 1043
	// (get) Token: 0x06001C86 RID: 7302 RVA: 0x000148D3 File Offset: 0x00012AD3
	// (set) Token: 0x06001C87 RID: 7303 RVA: 0x000148DB File Offset: 0x00012ADB
	[Obsolete("Please use IsLocalClientInside (updated case for naming).")]
	public bool isLocalClientInside
	{
		get
		{
			return this.IsLocalClientInside;
		}
		set
		{
			this.IsLocalClientInside = value;
		}
	}

	// Token: 0x17000414 RID: 1044
	// (get) Token: 0x06001C88 RID: 7304 RVA: 0x000148E4 File Offset: 0x00012AE4
	[Obsolete("Please use MaxPlayers (updated case for naming).")]
	public byte maxPlayers
	{
		get
		{
			return this.MaxPlayers;
		}
	}

	// Token: 0x17000415 RID: 1045
	// (get) Token: 0x06001C89 RID: 7305 RVA: 0x000148EC File Offset: 0x00012AEC
	[Obsolete("Please use IsOpen (updated case for naming).")]
	public bool open
	{
		get
		{
			return this.IsOpen;
		}
	}

	// Token: 0x17000416 RID: 1046
	// (get) Token: 0x06001C8A RID: 7306 RVA: 0x000148F4 File Offset: 0x00012AF4
	[Obsolete("Please use IsVisible (updated case for naming).")]
	public bool visible
	{
		get
		{
			return this.IsVisible;
		}
	}

	// Token: 0x0400106E RID: 4206
	private Hashtable customPropertiesField = new Hashtable();

	// Token: 0x0400106F RID: 4207
	protected byte maxPlayersField;

	// Token: 0x04001070 RID: 4208
	protected int emptyRoomTtlField;

	// Token: 0x04001071 RID: 4209
	protected int playerTtlField;

	// Token: 0x04001072 RID: 4210
	protected string[] expectedUsersField;

	// Token: 0x04001073 RID: 4211
	protected bool openField = true;

	// Token: 0x04001074 RID: 4212
	protected bool visibleField = true;

	// Token: 0x04001075 RID: 4213
	protected bool autoCleanUpField = PhotonNetwork.autoCleanUpPlayerObjects;

	// Token: 0x04001076 RID: 4214
	protected string nameField;

	// Token: 0x04001077 RID: 4215
	protected internal int masterClientIdField;
}
