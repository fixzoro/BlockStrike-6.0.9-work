using System;
using System.Collections.Generic;
using UnityEngine;

public class UIFriends : MonoBehaviour
{
    public const int maxFriends = 20;

    private static List<int> requestPlayers = new List<int>();

    private void Start()
	{
		PhotonRPC.AddMessage("PhotonAddFriend", new PhotonRPC.MessageDelegate(this.PhotonAddFriend));
	}

	public void AddFriend()
	{
		if (UIFriends.requestPlayers.Contains(UIPlayerStatistics.SelectPlayer.GetPlayerID()))
		{
			UIToast.Show(Localization.Get("Request has already been sent", true));
			return;
		}
		if (AccountManager.instance.Data.Friends.Contains(UIPlayerStatistics.SelectPlayer.GetPlayerID()))
		{
			UIToast.Show(Localization.Get("Player is already in your friends", true));
			return;
		}
		if (AccountManager.instance.Data.Friends.Count >= 20)
		{
			UIToast.Show(Localization.Get("Reached the maximum number of friends", true));
			return;
		}
		PhotonDataWrite data = PhotonRPC.GetData();
		data.Write((byte)1);
		PhotonRPC.RPC("PhotonAddFriend", UIPlayerStatistics.SelectPlayer, data);
		UIFriends.requestPlayers.Add(UIPlayerStatistics.SelectPlayer.GetPlayerID());
	}

	[PunRPC]
	private void PhotonAddFriend(PhotonMessage message)
	{
		byte b = message.ReadByte();
		if (b == 1)
		{
            PhotonPlayer player = message.sender;
			if (player == null || AccountManager.instance.Data.Friends.Contains(player.GetPlayerID()))
			{
                return;
			}
			if (AccountManager.instance.Data.Friends.Count >= 20)
			{
                return;
			}
			UIMessage.Add(Localization.Get("Add friend", true) + ": " + player.UserId, Localization.Get("Friends", true), player.ID, delegate(bool result, object obj)
			{
				if (AccountManager.instance.Data.Friends.Count >= 20)
				{
					UIToast.Show(Localization.Get("Reached the maximum number of friends", true));
					return;
				}
				player = PhotonPlayer.Find((int)obj);
				if (player == null)
				{
					return;
				}
				if (result)
				{
                    //CryptoPrefs.SetString("Friend_#" + (int)obj, player.UserId);
                    CryptoPrefs.SetString("Friend_#" + player.GetPlayerID(), player.UserId);
					AccountManager.AddFriend(player.GetPlayerID(), player.UserId, delegate
					{
						UIToast.Show(Localization.Get("Added friend", true));
					}, delegate(string error)
					{
						UIToast.Show(error);
					});
				}
				PhotonDataWrite data = PhotonRPC.GetData();
				data.Write((byte)2);
				data.Write(result);
				PhotonRPC.RPC("PhotonAddFriend", player, data);
				UIFriends.requestPlayers.Add(player.GetPlayerID());
			});
		}
		else if (b == 2)
		{
            bool flag = message.ReadBool();
			if (flag)
			{
				if (AccountManager.instance.Data.Friends.Count >= 20)
				{
					return;
				}
				PhotonPlayer sender = message.sender;
				CryptoPrefs.SetString("Friend_#" + sender.GetPlayerID(), sender.UserId);
                AccountManager.AddFriend(sender.GetPlayerID(), sender.UserId, delegate
				{
					UIToast.Show(Localization.Get("Added friend", true));
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
	}
}
