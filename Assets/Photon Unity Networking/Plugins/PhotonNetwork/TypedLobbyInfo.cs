using System;

// Token: 0x020002BA RID: 698
public class TypedLobbyInfo : TypedLobby
{
	// Token: 0x06001A1A RID: 6682 RVA: 0x000AA560 File Offset: 0x000A8760
	public override string ToString()
	{
		return string.Format("TypedLobbyInfo '{0}'[{1}] rooms: {2} players: {3}", new object[]
		{
			this.Name,
			this.Type,
			this.RoomCount,
			this.PlayerCount
		});
	}

	// Token: 0x04000F1C RID: 3868
	public int PlayerCount;

	// Token: 0x04000F1D RID: 3869
	public int RoomCount;
}
