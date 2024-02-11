using System;
using ExitGames.Client.Photon;

// Token: 0x02000332 RID: 818
internal static class PhotonCustomValue
{
	// Token: 0x06001DFB RID: 7675 RVA: 0x000BB6C0 File Offset: 0x000B98C0
	public static void SetTeam(this PhotonPlayer player, Team team)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[PhotonCustomValue.teamKey] = (byte)team;
		player.SetCustomProperties(hashtable, null, false);
	}

	// Token: 0x06001DFC RID: 7676 RVA: 0x000BB6F0 File Offset: 0x000B98F0
	public static Team GetTeam(this PhotonPlayer player)
	{
        object obj;
        try
        {
            if (player.customProperties.TryGetValue(PhotonCustomValue.teamKey, out obj))
            {
                return (Team)((byte)obj);
            }
        }
        catch (Exception ex)
        {
            return Team.None;
        }
        return Team.None;
    }

	// Token: 0x06001DFD RID: 7677 RVA: 0x000BB71C File Offset: 0x000B991C
	public static void SetLevel(this PhotonPlayer player, int level)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[PhotonCustomValue.levelKey] = (byte)level;
		player.SetCustomProperties(hashtable, null, false);
	}

	// Token: 0x06001DFE RID: 7678 RVA: 0x000BB74C File Offset: 0x000B994C
	public static int GetLevel(this PhotonPlayer player)
	{
		object obj;
		if (player.CustomProperties.TryGetValue(PhotonCustomValue.levelKey, out obj))
		{
			return (int)((byte)obj);
		}
		return 1;
	}

	// Token: 0x06001DFF RID: 7679 RVA: 0x000BB778 File Offset: 0x000B9978
	public static void SetClan(this PhotonPlayer player, string clan)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[PhotonCustomValue.clanKey] = clan;
		player.SetCustomProperties(hashtable, null, false);
	}

	// Token: 0x06001E00 RID: 7680 RVA: 0x000BB7A0 File Offset: 0x000B99A0
	public static string GetClan(this PhotonPlayer player)
	{
		object obj;
		if (player.CustomProperties.TryGetValue(PhotonCustomValue.clanKey, out obj))
		{
			return (string)obj;
		}
		return string.Empty;
	}

	// Token: 0x06001E01 RID: 7681 RVA: 0x000BB7D0 File Offset: 0x000B99D0
	public static void SetPlayerID(this PhotonPlayer player, int id)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[PhotonCustomValue.playerIDKey] = id;
		player.SetCustomProperties(hashtable, null, false);
	}

	// Token: 0x06001E02 RID: 7682 RVA: 0x000BB800 File Offset: 0x000B9A00
	public static int GetPlayerID(this PhotonPlayer player)
	{
		object obj;
		if (player.CustomProperties.TryGetValue(PhotonCustomValue.playerIDKey, out obj))
		{
			return (int)obj;
		}
		return 0;
	}

	// Token: 0x06001E03 RID: 7683 RVA: 0x000BB82C File Offset: 0x000B9A2C
	public static void SetAvatarUrl(this PhotonPlayer player, string url)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[PhotonCustomValue.avatarKey] = url;
		player.SetCustomProperties(hashtable, null, false);
	}

	// Token: 0x06001E04 RID: 7684 RVA: 0x000BB854 File Offset: 0x000B9A54
	public static string GetAvatarUrl(this PhotonPlayer player)
	{
		object obj;
		if (player.CustomProperties.TryGetValue(PhotonCustomValue.avatarKey, out obj))
		{
			return (string)obj;
		}
		return string.Empty;
	}

	// Token: 0x06001E05 RID: 7685 RVA: 0x000BB884 File Offset: 0x000B9A84
	public static void SetGameMode(this Room room, GameMode mode)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[PhotonCustomValue.gameModeKey] = (byte)mode;
		room.SetCustomProperties(hashtable, null, false);
	}

	// Token: 0x06001E06 RID: 7686 RVA: 0x000BB8B4 File Offset: 0x000B9AB4
	public static GameMode GetGameMode(this Room room)
	{
		object obj;
		if (room.CustomProperties.TryGetValue(PhotonCustomValue.gameModeKey, out obj))
		{
			return (GameMode)((byte)obj);
		}
		return GameMode.TeamDeathmatch;
	}

	// Token: 0x06001E07 RID: 7687 RVA: 0x000BB8E0 File Offset: 0x000B9AE0
	public static void SetOnlyWeapon(this Room room, int weaponID)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[PhotonCustomValue.onlyWeaponKey] = (byte)weaponID;
		room.SetCustomProperties(hashtable, null, false);
	}

	// Token: 0x06001E08 RID: 7688 RVA: 0x000BB910 File Offset: 0x000B9B10
	public static int GetOnlyWeapon(this Room room)
	{
		object obj;
		if (room.CustomProperties.TryGetValue(PhotonCustomValue.onlyWeaponKey, out obj))
		{
			return (int)((byte)obj);
		}
		return 1;
	}

	// Token: 0x06001E09 RID: 7689 RVA: 0x000BB93C File Offset: 0x000B9B3C
	public static void SetRoundState(this Room room, RoundState state)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[PhotonCustomValue.roundStateKey] = (byte)state;
		room.SetCustomProperties(hashtable, null, false);
	}

	// Token: 0x06001E0A RID: 7690 RVA: 0x000BB96C File Offset: 0x000B9B6C
	public static RoundState GetRoundState(this Room room)
	{
		object obj;
		if (room.CustomProperties.TryGetValue(PhotonCustomValue.roundStateKey, out obj))
		{
			return (RoundState)((byte)obj);
		}
		return RoundState.WaitPlayer;
	}

	// Token: 0x06001E0B RID: 7691 RVA: 0x000BB8B4 File Offset: 0x000B9AB4
	public static GameMode GetGameMode(this RoomInfo room)
	{
		object obj;
		if (room.CustomProperties.TryGetValue(PhotonCustomValue.gameModeKey, out obj))
		{
			return (GameMode)((byte)obj);
		}
		return GameMode.TeamDeathmatch;
	}

	// Token: 0x06001E0C RID: 7692 RVA: 0x000BB998 File Offset: 0x000B9B98
	public static string GetSceneName(this RoomInfo room)
	{
		object obj;
		if (room.CustomProperties.TryGetValue(PhotonCustomValue.sceneNameKey, out obj))
		{
			return (string)obj;
		}
		return string.Empty;
	}

	// Token: 0x06001E0D RID: 7693 RVA: 0x000BB910 File Offset: 0x000B9B10
	public static int GetOnlyWeapon(this RoomInfo room)
	{
		object obj;
		if (room.CustomProperties.TryGetValue(PhotonCustomValue.onlyWeaponKey, out obj))
		{
			return (int)((byte)obj);
		}
		return 1;
	}

	// Token: 0x06001E0E RID: 7694 RVA: 0x000BB9C8 File Offset: 0x000B9BC8
	public static Hashtable CreateRoomHashtable(this RoomInfo photonNetwork, string password, GameMode mode, bool officialServer)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[PhotonCustomValue.passwordKey] = password;
		hashtable[PhotonCustomValue.gameModeKey] = (byte)mode;
		hashtable[PhotonCustomValue.officialServerKey] = officialServer;
		return hashtable;
	}

	// Token: 0x06001E0F RID: 7695 RVA: 0x000BBA0C File Offset: 0x000B9C0C
	public static string GetPassword(this RoomInfo room)
	{
		object obj;
		if (room.CustomProperties.TryGetValue(PhotonCustomValue.passwordKey, out obj))
		{
			return (string)obj;
		}
		return string.Empty;
	}

#if UNITY_EDITOR
    public static void SetPassword(this Room room, string text)
    {
        Hashtable hashtable = new Hashtable();
        hashtable[PhotonCustomValue.passwordKey] = text;
        room.SetCustomProperties(hashtable, null, false);
    }
#endif

    // Token: 0x06001E10 RID: 7696 RVA: 0x000BBA3C File Offset: 0x000B9C3C
    public static bool HasPassword(this RoomInfo room)
	{
		object obj;
		return room.CustomProperties.TryGetValue(PhotonCustomValue.passwordKey, out obj) && !string.IsNullOrEmpty((string)obj);
	}

	// Token: 0x06001E11 RID: 7697 RVA: 0x000BBA70 File Offset: 0x000B9C70
	public static int GetCustomMapHash(this RoomInfo room)
	{
		object obj;
		if (room.CustomProperties.TryGetValue(PhotonCustomValue.customMapHash, out obj))
		{
			return (int)obj;
		}
		return 0;
	}

	// Token: 0x06001E12 RID: 7698 RVA: 0x000BBA9C File Offset: 0x000B9C9C
	public static string GetCustomMapUrl(this RoomInfo room)
	{
		object obj;
		if (room.CustomProperties.TryGetValue(PhotonCustomValue.customMapUrl, out obj))
		{
			return (string)obj;
		}
		return string.Empty;
	}

	// Token: 0x06001E13 RID: 7699 RVA: 0x000BBACC File Offset: 0x000B9CCC
	public static int[] GetCustomMapModes(this RoomInfo room)
	{
		object obj;
		if (room.CustomProperties.TryGetValue(PhotonCustomValue.customMapModes, out obj))
		{
			return (int[])obj;
		}
		return new int[0];
	}

	// Token: 0x06001E14 RID: 7700 RVA: 0x000BBB00 File Offset: 0x000B9D00
	public static bool isCustomMap(this RoomInfo room)
	{
		object obj;
		return room.CustomProperties.TryGetValue(PhotonCustomValue.customMapHash, out obj) && (int)obj != 0;
	}

	// Token: 0x06001E15 RID: 7701 RVA: 0x000BBB38 File Offset: 0x000B9D38
	public static bool isOfficialServer(this RoomInfo room)
	{
		object obj;
		return room != null && room.CustomProperties.TryGetValue(PhotonCustomValue.officialServerKey, out obj) && (bool)obj;
	}

	// Token: 0x06001E16 RID: 7702 RVA: 0x000BBB6C File Offset: 0x000B9D6C
	public static void SetMinLevel(this Room room, byte level)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[PhotonCustomValue.minLevelKey] = level;
		room.SetCustomProperties(hashtable, null, false);
	}

	// Token: 0x06001E17 RID: 7703 RVA: 0x000BBB9C File Offset: 0x000B9D9C
	public static byte GetMinLevel(this RoomInfo room)
	{
		object obj;
		if (room.CustomProperties.TryGetValue(PhotonCustomValue.minLevelKey, out obj))
		{
			return (byte)obj;
		}
		return 1;
	}

	// Token: 0x040011DF RID: 4575
	public static string teamKey = "t";

	// Token: 0x040011E0 RID: 4576
	public static string levelKey = "l";

	// Token: 0x040011E1 RID: 4577
	public static string clanKey = "c";

	// Token: 0x040011E2 RID: 4578
	public static string pingKey = "p";

	// Token: 0x040011E3 RID: 4579
	public static string deathsKey = "d";

	// Token: 0x040011E4 RID: 4580
	public static string killsKey = "k";

	// Token: 0x040011E5 RID: 4581
	public static string deadKey = "de";

	// Token: 0x040011E6 RID: 4582
	public static string playerIDKey = "pl";

	// Token: 0x040011E7 RID: 4583
	public static string avatarKey = "a";

	// Token: 0x040011E8 RID: 4584
	public static string gameModeKey = "g";

	// Token: 0x040011E9 RID: 4585
	public static string onlyWeaponKey = "o";

	// Token: 0x040011EA RID: 4586
	public static string roundStateKey = "r";

	// Token: 0x040011EB RID: 4587
	public static string sceneNameKey = "s";

	// Token: 0x040011EC RID: 4588
	public static string passwordKey = "p";

	// Token: 0x040011ED RID: 4589
	public static string officialServerKey = "of";

	// Token: 0x040011EE RID: 4590
	public static string minLevelKey = "ml";

	// Token: 0x040011EF RID: 4591
	public static string customMapModes = "c1";

	// Token: 0x040011F0 RID: 4592
	public static string customMapHash = "c2";

	// Token: 0x040011F1 RID: 4593
	public static string customMapUrl = "c3";
}
