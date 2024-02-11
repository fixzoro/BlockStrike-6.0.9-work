using System;
using System.Collections.Generic;
using UnityEngine;

public class UIClan : MonoBehaviour
{
    public const int maxPlayers = 20;

    private static List<int> requestPlayers = new List<int>();

    private void Start()
	{
		PhotonRPC.AddMessage("PhotonAddClan", new PhotonRPC.MessageDelegate(this.PhotonAddClan));
	}

	public void AddPlayer()
	{
		if (string.IsNullOrEmpty(AccountManager.instance.Data.Clan))
		{
			MonoBehaviour.print(1);
			return;
		}
		if (AccountManager.instance.Data.ID != ClanManager.admin)
		{
			MonoBehaviour.print(2);
			return;
		}
		if (UIClan.requestPlayers.Contains(UIPlayerStatistics.SelectPlayer.GetPlayerID()))
		{
			MonoBehaviour.print(3);
			UIToast.Show(Localization.Get("Request has already been sent", true));
			return;
		}
		if (ClanManager.players.Contains(UIPlayerStatistics.SelectPlayer.GetPlayerID()) || !string.IsNullOrEmpty(UIPlayerStatistics.SelectPlayer.GetClan()))
		{
			MonoBehaviour.print(4);
			UIToast.Show(Localization.Get("Player is already in clan", true));
			return;
		}
		if (ClanManager.players.Length >= 20)
		{
			MonoBehaviour.print(5);
			UIToast.Show(Localization.Get("The maximum number of players in the clan", true));
			return;
		}
		PhotonDataWrite data = PhotonRPC.GetData();
		data.Write((byte)1);
		data.Write(AccountManager.instance.Data.Clan.ToString());
		PhotonRPC.RPC("PhotonAddClan", UIPlayerStatistics.SelectPlayer, data);
		UIClan.requestPlayers.Add(UIPlayerStatistics.SelectPlayer.GetPlayerID());
	}

	[PunRPC]
	private void PhotonAddClan(PhotonMessage message)
	{
		byte b = message.ReadByte();
		if (b == 1)
		{
			string arg = message.ReadString();
			PhotonPlayer player = message.sender;
			if (player == null || string.IsNullOrEmpty(player.GetClan()))
			{
				return;
			}
			if (!string.IsNullOrEmpty(AccountManager.instance.Data.Clan))
			{
				return;
			}
			UIMessage.Add(string.Format(Localization.Get("Your invite {0} to clan {1}. Want to join?", true), player.UserId, arg), Localization.Get("Clan", true), player.ID, delegate(bool result, object obj)
			{
				player = PhotonPlayer.Find((int)obj);
				if (player == null)
				{
					return;
				}
				PhotonDataWrite data = PhotonRPC.GetData();
				data.Write((byte)2);
				data.Write(result);
				PhotonRPC.RPC("PhotonAddClan", player, data);
				UIClan.requestPlayers.Add(player.GetPlayerID());
			});
		}
		else if (b == 2)
		{
			bool flag = message.ReadBool();
			if (flag)
			{
				PhotonPlayer player = message.sender;
				CryptoPrefs.SetString("Friend_#" + player.GetPlayerID(), player.UserId);
				AccountManager.Clan.AddPlayer(player.GetPlayerID(), delegate
				{
					UIToast.Show(player.UserId + " " + Localization.Get("joined the clan", true));
					PhotonDataWrite data = PhotonRPC.GetData();
					data.Write((byte)3);
					data.Write(AccountManager.instance.Data.Clan.ToString());
					PhotonRPC.RPC("PhotonAddClan", player, data);
				}, delegate(string error)
				{
					UIToast.Show(error);
				});
			}
			else
			{
				UIToast.Show("declined the request");
			}
		}
		else if (b == 3)
		{
			string value = message.ReadString();
			AccountManager.instance.Data.Clan = value;
			UIToast.Show(PhotonNetwork.player.UserId + " " + Localization.Get("joined the clan", true));
		}
	}
}
