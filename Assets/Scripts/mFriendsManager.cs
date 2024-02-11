using System;
using System.Collections.Generic;
using FreeJSON;
using Photon;
using UnityEngine;

public class mFriendsManager : Photon.MonoBehaviour
{
    private static Dictionary<int, string> cacheFriendInfo = new Dictionary<int, string>();

    public GameObject panel;

    public UIGrid grid;

    public UIGrid friendStatsGrid;

    public UIPanel friendStatsPanel;

    public GameObject playerStatsPanel;

    public UISprite playerStatsInRoomSprite;

    public UILabel playerStatsNameLabel;

    public UILabel playerStatsIDLabel;

    public UILabel playerStatsLevelLabel;

    public UILabel playerStatsXPLabel;

    public UILabel playerStatsDeathsLabel;

    public UILabel playerStatsKillsLabel;

    public UILabel playerStatsHeadshotLabel;

    public UILabel playerStatsTimeLabel;

    public UILabel playerStatsLastLoginLabel;

    public GameObject roomStatsPanel;

    public UILabel roomStatsRoomLabel;

    public UILabel roomStatsModeLabel;

    public UILabel roomStatsMapLabel;

    public UILabel roomStatsMaxPlayersLabel;

    public UILabel roomStatsPasswordLabel;

    public UILabel roomStatsCustomMapsLabel;

    public UILabel infoLabel;

    public mFriendsElement container;

    public Color onlineColor;

    public Color offlineColor;

    private mFriendsElement selectFriend;

    private List<mFriendsElement> activeContainers = new List<mFriendsElement>();

    private List<mFriendsElement> deactiveContainers = new List<mFriendsElement>();

    private void OnEnable()
	{
		PhotonNetwork.onUpdatedFriendList = (PhotonNetwork.VoidDelegate)Delegate.Combine(PhotonNetwork.onUpdatedFriendList, new PhotonNetwork.VoidDelegate(this.OnUpdatedFriendList));
	}

	private void OnDisable()
	{
		PhotonNetwork.onUpdatedFriendList = (PhotonNetwork.VoidDelegate)Delegate.Remove(PhotonNetwork.onUpdatedFriendList, new PhotonNetwork.VoidDelegate(this.OnUpdatedFriendList));
	}

	private void OnUpdatedFriendList()
	{
		if (PhotonNetwork.Friends == null)
		{
			return;
		}
		this.UpdateList();
	}

	public void Open()
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account", true));
			return;
		}
		if (!PhotonNetwork.connected)
		{
			mPhotonSettings.connectedCallback = delegate()
			{
				mPopUp.HideAll("Friends", true);
				this.UpdateList();
			};
			mPhotonSettings.Connect();
			return;
		}
		mPanelManager.Show("Friends", true);
		this.UpdateList();
	}

	public void Connect()
	{
		if (!this.selectFriend.info.IsInRoom)
		{
			return;
		}
		RoomInfo room = this.GetRoom(this.selectFriend.info.Room);
		if (room == null)
		{
			return;
		}
		if (room.isOfficialServer())
		{
			int num = (int)room.GetMinLevel() - AccountManager.GetLevel();
			if (num > 5)
			{
				UIToast.Show(string.Concat(new object[]
				{
					Localization.Get("Requires Level", true),
					" ",
					room.GetMinLevel(),
					" <"
				}));
				return;
			}
			if (num < -5)
			{
				UIToast.Show(string.Concat(new object[]
				{
					Localization.Get("Requires Level", true),
					" ",
					room.GetMinLevel(),
					" >"
				}));
				return;
			}
		}
		this.friendStatsPanel.cachedGameObject.SetActive(false);
		mJoinServer.room = room;
		mJoinServer.onBack = new Action(this.OnBack);
		mJoinServer.Join();
	}

	private void OnBack()
	{
		mPopUp.HideAll("Friends");
	}

	private RoomInfo GetRoom(string name)
	{
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		for (int i = 0; i < roomList.Length; i++)
		{
			if (roomList[i].Name == name)
			{
				return roomList[i];
			}
		}
		return null;
	}

	public void DeletePlayer()
	{
		mPopUp.ShowPopup(Localization.Get("Do you want to remove", true) + ": " + this.selectFriend.Name.text, Localization.Get("Delete", true), Localization.Get("Yes", true), delegate()
		{
			mPopUp.HideAll();
			AccountManager.instance.Data.Friends.Remove(this.selectFriend.ID);
			AccountManager.DeleteFriend(this.selectFriend.ID, delegate
			{
				UIToast.Show("Delete Friend Complete");
			}, delegate(string error)
			{
				UIToast.Show("Delete Friend Error: " + error);
			});
			this.friendStatsPanel.cachedGameObject.SetActive(false);
			this.UpdateList();
		}, Localization.Get("No", true), delegate()
		{
			mPopUp.HideAll();
		});
	}

	public void SelectPlayer(mFriendsElement element)
	{
		if (element == null)
		{
			return;
		}
		if (element.info == null)
		{
			return;
		}
		this.selectFriend = element;
		mPanelManager.ShowTween(this.friendStatsPanel.cachedGameObject);
		if (PhotonNetwork.Friends != null && PhotonNetwork.connected)
		{
			this.roomStatsPanel.SetActive(element.info.IsInRoom);
			if (element.info.IsInRoom)
			{
				RoomInfo room = this.GetRoom(element.info.Room);
				if (room != null)
				{
					this.roomStatsRoomLabel.text = ((!room.isOfficialServer()) ? room.Name : room.Name.Replace("off", Localization.Get("Official Servers", true)));
					this.roomStatsModeLabel.text = Localization.Get(room.GetGameMode().ToString(), true);
					this.roomStatsMapLabel.text = room.GetSceneName();
					this.roomStatsMaxPlayersLabel.text = room.PlayerCount + "/" + room.MaxPlayers;
					this.roomStatsPasswordLabel.text = ((!room.isOfficialServer()) ? ((!room.HasPassword()) ? Localization.Get("No", true) : Localization.Get("Yes", true)) : Localization.Get("No", true));
					this.roomStatsCustomMapsLabel.text = ((!room.isCustomMap()) ? Localization.Get("No", true) : Localization.Get("Yes", true));
				}
			}
			this.playerStatsInRoomSprite.color = ((!element.info.IsOnline) ? this.offlineColor : this.onlineColor);
		}
		else
		{
			this.roomStatsPanel.SetActive(false);
			this.playerStatsInRoomSprite.color = this.offlineColor;
		}
		this.playerStatsNameLabel.text = element.Name.text;
		this.playerStatsIDLabel.text = element.ID.ToString();
		this.playerStatsLevelLabel.text = string.Empty;
		this.playerStatsXPLabel.text = string.Empty;
		this.playerStatsDeathsLabel.text = string.Empty;
		this.playerStatsKillsLabel.text = string.Empty;
		this.playerStatsHeadshotLabel.text = string.Empty;
		this.playerStatsTimeLabel.text = string.Empty;
		this.playerStatsLastLoginLabel.text = string.Empty;
		if (mFriendsManager.cacheFriendInfo.ContainsKey(element.ID))
		{
			this.UpdateFriendInfo(mFriendsManager.cacheFriendInfo[element.ID]);
		}
		else
		{
			AccountManager.GetFriendsInfo(element.ID, delegate(string result)
			{
				mFriendsManager.cacheFriendInfo[element.ID] = result;
				this.UpdateFriendInfo(result);
			}, delegate(string error)
			{
				UIToast.Show("Get Friend Info Error: " + error);
			});
		}
		this.friendStatsGrid.repositionNow = true;
	}

	private void UpdateFriendInfo(string result)
	{
        JsonObject jsonObject = JsonObject.Parse(result);
        JsonObject jsonObjectRound = jsonObject.Get<JsonObject>("Round");
        this.playerStatsNameLabel.text = jsonObject.Get<string>("AccountName");
        this.playerStatsIDLabel.text = jsonObject.Get<string>("ID");
        this.playerStatsLevelLabel.text = jsonObjectRound.Get<string>("Level", "1");
        this.playerStatsXPLabel.text = jsonObjectRound.Get<string>("XP", "0");
        this.playerStatsDeathsLabel.text = jsonObjectRound.Get<string>("Deaths", "0");
        this.playerStatsKillsLabel.text = jsonObjectRound.Get<string>("Kills", "0");
        this.playerStatsHeadshotLabel.text = jsonObjectRound.Get<string>("Head", "0");
        this.playerStatsTimeLabel.text = this.ConvertTime(jsonObjectRound.Get<long>("Time", 0L));
        this.playerStatsLastLoginLabel.text = this.ConvertLastLoginTime(jsonObject.Get<long>("LastLogin"));
        CryptoPrefs.SetString("Friend_#" + this.playerStatsIDLabel.text, this.playerStatsNameLabel.text);
    }

	public void Refresh()
	{
		if (PhotonNetwork.connected)
		{
			string[] array = mFriendsManager.GetFriends().ToArray();
			if (array.Length > 0)
			{
				PhotonNetwork.FindFriends(array);
			}
		}
		this.UpdateList();
	}

	public void UpdateList()
	{
		if (PhotonNetwork.Friends == null && PhotonNetwork.connected)
		{
			string[] array = mFriendsManager.GetFriends().ToArray();
			if (array.Length > 0)
			{
				PhotonNetwork.FindFriends(array);
			}
		}
		this.DeactiveAll();
		for (int i = 0; i < AccountManager.instance.Data.Friends.Count; i++)
		{
			this.GetContainer().SetData(AccountManager.instance.Data.Friends[i]);
		}
		this.grid.repositionNow = true;
		if (!PhotonNetwork.connected)
		{
			this.infoLabel.text = Localization.Get("Connect to the region", true);
		}
		else
		{
			byte b = 0;
			byte b2 = 0;
			if (PhotonNetwork.Friends != null)
			{
				for (int j = 0; j < PhotonNetwork.Friends.Count; j++)
				{
					if (PhotonNetwork.Friends[j].IsOnline)
					{
						b += 1;
					}
					if (PhotonNetwork.Friends[j].IsInRoom)
					{
						b2 += 1;
					}
				}
			}
			string text = string.Concat(new object[]
			{
				Localization.Get("Friends", true),
				": ",
				AccountManager.instance.Data.Friends.Count,
				"/",
				20,
				"\n",
				Localization.Get("Online", true),
				": ",
				b,
				"/",
				AccountManager.instance.Data.Friends.Count,
				"\n",
				Localization.Get("Play in server", true),
				": ",
				b2,
				"/",
				b
			});
			this.infoLabel.text = text;
		}
	}

	private mFriendsElement GetContainer()
	{
		if (this.deactiveContainers.Count != 0)
		{
			mFriendsElement mFriendsElement = this.deactiveContainers[0];
			this.deactiveContainers.RemoveAt(0);
			mFriendsElement.Widget.cachedGameObject.SetActive(true);
			this.activeContainers.Add(mFriendsElement);
			return mFriendsElement;
		}
		GameObject gameObject = this.grid.gameObject.AddChild(this.container.Widget.cachedGameObject);
		gameObject.SetActive(true);
		mFriendsElement component = gameObject.GetComponent<mFriendsElement>();
		this.activeContainers.Add(component);
		return component;
	}

	private void DeactiveAll()
	{
		for (int i = 0; i < this.activeContainers.Count; i++)
		{
			this.activeContainers[i].Widget.cachedGameObject.SetActive(false);
			this.deactiveContainers.Add(this.activeContainers[i]);
		}
		this.activeContainers.Clear();
	}

	public static List<string> GetFriends()
	{
		List<string> list = new List<string>();
		List<int> list2 = new List<int>();
		string text = string.Empty;
		for (int i = 0; i < AccountManager.instance.Data.Friends.Count; i++)
		{
			text = CryptoPrefs.GetString("Friend_#" + AccountManager.instance.Data.Friends[i].ToString(), "null");
			if (text == "null")
			{
				list2.Add(AccountManager.instance.Data.Friends[i]);
			}
			else
			{
				list.Add(text);
			}
		}
		if (list2.Count != 0)
		{
			mFriendsManager.GetFriendsNames(list2.ToArray());
		}
		return list;
	}

	public static void GetFriendsNames(int[] ids)
	{
		AccountManager.GetFriendsName(ids, delegate
		{
			PhotonNetwork.FindFriends(mFriendsManager.GetFriends().ToArray());
		}, delegate(string error)
		{
			UIToast.Show("Get Friends Name Error: " + error);
		});
	}

	private string ConvertTime(long time)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds((double)time);
		if (timeSpan.Days * 24 + timeSpan.Hours > 0)
		{
			return string.Concat(new object[]
			{
				timeSpan.Days * 24 + timeSpan.Hours,
				" ",
				Localization.Get("h", true),
				"."
			});
		}
		if (timeSpan.Minutes > 0)
		{
			return string.Concat(new object[]
			{
				timeSpan.Minutes,
				" ",
				Localization.Get("m", true),
				"."
			});
		}
		return string.Concat(new object[]
		{
			timeSpan.Seconds,
			" ",
			Localization.Get("s", true),
			"."
		});
	}

	private string ConvertLastLoginTime(long time)
	{
		time += NTPManager.GetMilliSeconds(DateTime.Now) - NTPManager.GetMilliSeconds(DateTime.UtcNow);
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		dateTime = dateTime.AddMilliseconds((double)time);
		dateTime = dateTime.ToLocalTime();
		return string.Concat(new object[]
		{
			dateTime.Day.ToString("D2"),
			"/",
			dateTime.Month.ToString("D2"),
			"/",
			dateTime.Year,
			" ",
			dateTime.Hour.ToString("D2"),
			":",
			dateTime.Minute.ToString("D2"),
			":",
			dateTime.Second.ToString("D2")
		});
	}
}
