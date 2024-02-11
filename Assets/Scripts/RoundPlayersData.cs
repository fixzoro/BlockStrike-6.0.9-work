using System;
using ExitGames.Client.Photon;

public static class RoundPlayersData
{
    private static readonly byte pingKey = 4;

    private static readonly byte updateDataKey = 97;

    private static readonly byte clearKey = 98;

    private static readonly byte allDataKey = 99;

    private static RoundPlayersData.PlayerData localData = new RoundPlayersData.PlayerData();

    private static BetterList<RoundPlayersData.PlayerData> playerList = new BetterList<RoundPlayersData.PlayerData>();

    private static bool init = false;

    private static PhotonDataWrite photonData = new PhotonDataWrite();

    public static void Init()
	{
		if (RoundPlayersData.init)
		{
			return;
		}
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(RoundPlayersData.OnPhotonPlayerConnected));
		PhotonNetwork.onPhotonPlayerDisconnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerDisconnected, new PhotonNetwork.PhotonPlayerDelegate(RoundPlayersData.OnPhotonPlayerDisconnected));
		RoundPlayersData.init = true;
	}

	public static void ClearProperties(this PhotonPlayer player)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[PhotonCustomValue.teamKey] = 0;
		player.SetCustomProperties(hashtable, null, false);
		if (!player.IsLocal)
		{
			return;
		}
		RoundPlayersData.localData.death = 0;
		RoundPlayersData.localData.kills = 0;
		RoundPlayersData.localData.dead = true;
		if (PhotonNetwork.room != null)
		{
			RoundPlayersData.photonData.Clear();
			RoundPlayersData.photonData.Write(RoundPlayersData.clearKey);
			PhotonRPC.RPC("UpdateRoundInfo", PhotonTargets.Others, RoundPlayersData.photonData);
		}
	}

	private static void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		RoundPlayersData.PlayerData playerData = new RoundPlayersData.PlayerData();
		playerData.player = player;
		RoundPlayersData.playerList.Add(playerData);
		RoundPlayersData.photonData.Clear();
		RoundPlayersData.photonData.Write(RoundPlayersData.allDataKey);
		RoundPlayersData.photonData.Write((short)RoundPlayersData.localData.death);
		RoundPlayersData.photonData.Write((short)RoundPlayersData.localData.kills);
		RoundPlayersData.photonData.Write((short)RoundPlayersData.localData.ping);
		RoundPlayersData.photonData.Write(RoundPlayersData.localData.dead);
		TimerManager.In(0.5f, delegate()
		{
			PhotonRPC.RPC("UpdateRoundInfo", player, RoundPlayersData.photonData);
		});
	}

	private static void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		for (int i = 0; i < RoundPlayersData.playerList.size; i++)
		{
			if (RoundPlayersData.playerList[i].player.ID == player.ID)
			{
				RoundPlayersData.playerList.RemoveAt(i);
				break;
			}
		}
	}

	private static RoundPlayersData.PlayerData GetData(PhotonPlayer player)
	{
		for (int i = 0; i < RoundPlayersData.playerList.size; i++)
		{
			if (RoundPlayersData.playerList[i].player.ID == player.ID)
			{
				return RoundPlayersData.playerList[i];
			}
		}
		return null;
	}

	public static void SetDeaths(this PhotonPlayer player, int deaths)
	{
		if (!player.IsLocal)
		{
			return;
		}
		RoundPlayersData.localData.death = deaths;
		RoundPlayersData.photonData.Clear();
		RoundPlayersData.photonData.Write(RoundPlayersData.updateDataKey);
		RoundPlayersData.photonData.Write((short)RoundPlayersData.localData.death);
		RoundPlayersData.photonData.Write((short)RoundPlayersData.localData.kills);
		RoundPlayersData.photonData.Write(RoundPlayersData.localData.dead);
		PhotonRPC.RPC("UpdateRoundInfo", PhotonTargets.Others, RoundPlayersData.photonData);
	}

	public static void SetDeaths1(this PhotonPlayer player)
	{
		if (!player.IsLocal)
		{
			return;
		}
		int num = player.GetDeaths();
		num++;
		player.SetDeaths(num);
	}

	public static int GetDeaths(this PhotonPlayer player)
	{
		if (player.IsLocal)
		{
			return RoundPlayersData.localData.death;
		}
		for (int i = 0; i < RoundPlayersData.playerList.size; i++)
		{
			if (RoundPlayersData.playerList[i].player.ID == player.ID)
			{
				return RoundPlayersData.playerList[i].death;
			}
		}
		return 0;
	}

	public static void SetKills(this PhotonPlayer player, int kills)
	{
		if (!player.IsLocal)
		{
			return;
		}
		RoundPlayersData.localData.kills = kills;
		RoundPlayersData.photonData.Clear();
		RoundPlayersData.photonData.Write(RoundPlayersData.updateDataKey);
		RoundPlayersData.photonData.Write((short)RoundPlayersData.localData.death);
		RoundPlayersData.photonData.Write((short)RoundPlayersData.localData.kills);
		RoundPlayersData.photonData.Write(RoundPlayersData.localData.dead);
		PhotonRPC.RPC("UpdateRoundInfo", PhotonTargets.Others, RoundPlayersData.photonData);
	}

	public static void SetKills1(this PhotonPlayer player)
	{
		if (!player.IsLocal)
		{
			return;
		}
		int num = player.GetKills();
		num++;
		player.SetKills(num);
	}

	public static int GetKills(this PhotonPlayer player)
	{
		if (player.IsLocal)
		{
			return RoundPlayersData.localData.kills;
		}
		for (int i = 0; i < RoundPlayersData.playerList.size; i++)
		{
			if (RoundPlayersData.playerList[i].player.ID == player.ID)
			{
				return RoundPlayersData.playerList[i].kills;
			}
		}
		return 0;
	}

	public static void SetDead(this PhotonPlayer player, bool dead)
	{
		if (!player.IsLocal)
		{
			return;
		}
		RoundPlayersData.localData.dead = dead;
		RoundPlayersData.photonData.Clear();
		RoundPlayersData.photonData.Write(RoundPlayersData.updateDataKey);
		RoundPlayersData.photonData.Write((short)RoundPlayersData.localData.death);
		RoundPlayersData.photonData.Write((short)RoundPlayersData.localData.kills);
		RoundPlayersData.photonData.Write(RoundPlayersData.localData.dead);
		PhotonRPC.RPC("UpdateRoundInfo", PhotonTargets.Others, RoundPlayersData.photonData);
	}

	public static bool GetDead(this PhotonPlayer player)
	{
		if (player.IsLocal)
		{
			return RoundPlayersData.localData.dead;
		}
		for (int i = 0; i < RoundPlayersData.playerList.size; i++)
		{
			if (RoundPlayersData.playerList[i].player.ID == player.ID)
			{
				return RoundPlayersData.playerList[i].dead;
			}
		}
		return false;
	}

	public static void UpdatePing(this PhotonPlayer player)
	{
		if (!player.IsLocal)
		{
			return;
		}
		RoundPlayersData.localData.ping = PhotonNetwork.GetPing();
		RoundPlayersData.photonData.Clear();
		RoundPlayersData.photonData.Write(RoundPlayersData.pingKey);
		RoundPlayersData.photonData.Write((short)RoundPlayersData.localData.ping);
		PhotonRPC.RPC("UpdateRoundInfo", PhotonTargets.Others, RoundPlayersData.photonData);
	}

	public static int GetPing(this PhotonPlayer player)
	{
		if (player.IsLocal)
		{
			return RoundPlayersData.localData.ping;
		}
		for (int i = 0; i < RoundPlayersData.playerList.size; i++)
		{
			if (RoundPlayersData.playerList[i].player.ID == player.ID)
			{
				return RoundPlayersData.playerList[i].ping;
			}
		}
		return 200;
	}

	[PunRPC]
	public static void UpdateRoundInfo(PhotonMessage message)
	{
		byte b = message.ReadByte();
		if (b == 99)
		{
			RoundPlayersData.PlayerData playerData = new RoundPlayersData.PlayerData();
			playerData.player = message.sender;
			playerData.death = (int)message.ReadShort();
			playerData.kills = (int)message.ReadShort();
			playerData.ping = (int)message.ReadShort();
			playerData.dead = message.ReadBool();
			RoundPlayersData.playerList.Add(playerData);
			return;
		}
		RoundPlayersData.PlayerData playerData2 = RoundPlayersData.GetData(message.sender);
		if (playerData2 == null)
		{
			playerData2 = new RoundPlayersData.PlayerData();
			playerData2.player = message.sender;
			playerData2.death = 0;
			playerData2.kills = 0;
			playerData2.dead = true;
			RoundPlayersData.playerList.Add(playerData2);
			return;
		}
		byte b2 = b;
		switch (b2)
		{
		case 2:
			playerData2.death = (int)message.ReadShort();
			break;
		case 3:
			playerData2.kills = (int)message.ReadShort();
			break;
		case 4:
			playerData2.ping = (int)message.ReadShort();
			break;
		case 5:
			playerData2.dead = message.ReadBool();
			break;
		default:
			if (b2 != 97)
			{
				if (b2 == 98)
				{
					playerData2.death = 0;
					playerData2.kills = 0;
					playerData2.dead = true;
				}
			}
			else
			{
				playerData2.death = (int)message.ReadShort();
				playerData2.kills = (int)message.ReadShort();
				playerData2.dead = message.ReadBool();
			}
			break;
		}
	}

	public class PlayerData
	{
		public PhotonPlayer player;

		public int death;

		public int kills;

		public int ping = 200;

		public bool dead = true;
	}
}
