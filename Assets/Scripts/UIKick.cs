using System;
using System.Collections.Generic;
using FreeJSON;
using Photon;
using UnityEngine;

public class UIKick : Photon.MonoBehaviour
{
    private float lastKickTime;

    private Dictionary<string, List<string>> kickList = new Dictionary<string, List<string>>();

    private List<string> kicked = new List<string>();

    private void Start()
	{
		PhotonRPC.AddMessage("PhotonKickPlayer", new PhotonRPC.MessageDelegate(this.PhotonKickPlayer));
	}

	private void OnEnable()
	{
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(this.OnPhotonPlayerConnected));
	}

	private void OnDisable()
	{
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Remove(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(this.OnPhotonPlayerConnected));
	}

	private void OnPhotonPlayerConnected(PhotonPlayer playerConnect)
	{
		if (PhotonNetwork.isMasterClient)
		{
			if (this.kicked.Contains(playerConnect.GetPlayerID().ToString()) || string.IsNullOrEmpty(playerConnect.GetPlayerID().ToString()))
			{
				PhotonNetwork.CloseConnection(playerConnect);
			}
			else
			{
				TimerManager.In(1f, delegate()
				{
					JsonObject jsonObject = new JsonObject();
					if (this.kickList.Count > 0)
					{
						jsonObject.Add("1", this.kickList);
					}
					if (this.kicked.Count > 0)
					{
						jsonObject.Add("2", this.kicked);
					}
					if (jsonObject.Length > 0)
					{
						PhotonDataWrite data = PhotonRPC.GetData();
						data.Write(3);
						data.Write(jsonObject.ToString());
						PhotonRPC.RPC("PhotonKickPlayer", playerConnect, data);
					}
				});
			}
		}
	}

	public void Kick()
	{
		if (this.lastKickTime > Time.time)
		{
			UIToast.Show(UIKick.ConvertTime(this.lastKickTime - Time.time));
			return;
		}
		if (this.kickList.ContainsKey(UIPlayerStatistics.SelectPlayer.GetPlayerID().ToString()) && this.kickList[UIPlayerStatistics.SelectPlayer.GetPlayerID().ToString()].Contains(PhotonNetwork.player.GetPlayerID().ToString()))
		{
			UIToast.Show("Error");
			return;
		}
		PhotonDataWrite data = PhotonRPC.GetData();
		data.Write(1);
		data.Write(UIPlayerStatistics.SelectPlayer.ID);
		PhotonRPC.RPC("PhotonKickPlayer", PhotonTargets.All, data);
		this.lastKickTime = Time.time + 300f;
		UIToast.Show("Ok");
	}

	[PunRPC]
	private void PhotonKickPlayer(PhotonMessage message)
	{
		if (message.ReadInt() == 1)
		{
			PhotonPlayer player = PhotonPlayer.Find(message.ReadInt());
			if (player == null)
			{
				return;
			}
			if (this.kickList.ContainsKey(player.GetPlayerID().ToString()))
			{
				this.kickList[player.GetPlayerID().ToString()].Add(message.sender.GetPlayerID().ToString());
			}
			else
			{
				this.kickList[player.GetPlayerID().ToString()] = new List<string>
				{
					message.sender.GetPlayerID().ToString()
				};
			}
			if (PhotonNetwork.isMasterClient)
			{
				int num = this.kickList[player.GetPlayerID().ToString()].Count * 100;
				if (num / PhotonNetwork.room.PlayerCount >= 60)
				{
					PhotonDataWrite data = PhotonRPC.GetData();
					data.Write(2);
					data.Write(player.ID);
					PhotonRPC.RPC("PhotonKickPlayer", PhotonTargets.All, data);
					TimerManager.In(1.5f, delegate()
					{
						PhotonNetwork.CloseConnection(player);
					});
				}
			}
		}
		else if (message.ReadInt() == 2)
		{
			PhotonPlayer photonPlayer = PhotonPlayer.Find(message.ReadInt());
			if (photonPlayer == null)
			{
				return;
			}
			this.kicked.Add(photonPlayer.GetPlayerID().ToString());
			if (PhotonNetwork.player.ID == photonPlayer.ID)
			{
				GameManager.leaveRoomMessage = Localization.Get("You kicked from the server", true);
			}
		}
		else if (message.ReadInt() == 3)
		{
			JsonObject jsonObject = JsonObject.Parse(message.ReadString());
			if (jsonObject.ContainsKey("1"))
			{
				this.kickList = jsonObject.Get<Dictionary<string, List<string>>>("1");
			}
			if (jsonObject.ContainsKey("2"))
			{
				this.kicked = jsonObject.Get<List<string>>("2");
			}
		}
	}

	private static string ConvertTime(float time)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds((double)time);
		return string.Format("{0:0}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
	}
}
