using System;
using System.Collections.Generic;
using FreeJSON;
using UnityEngine;

[Serializable]
public class mClanPlayers
{
    private static Dictionary<int, string> cachePlayerInfo = new Dictionary<int, string>();

    public GameObject panel;

    public UIGrid grid;

    public UIPanel statsPanel;

    public UILabel playerStatsNameLabel;

    public UILabel playerStatsIDLabel;

    public UILabel playerStatsLevelLabel;

    public UILabel playerStatsXPLabel;

    public UILabel playerStatsDeathsLabel;

    public UILabel playerStatsKillsLabel;

    public UILabel playerStatsHeadshotLabel;

    public UILabel playerStatsTimeLabel;

    public UILabel playerStatsLastLoginLabel;

    public mClanPlayerElement container;

    private mClanPlayerElement selectPlayer;

    private List<mClanPlayerElement> activeContainers = new List<mClanPlayerElement>();

    private List<mClanPlayerElement> deactiveContainers = new List<mClanPlayerElement>();

    public void Open()
	{
		this.UpdateList();
	}

	public void DeletePlayer()
	{
		mPopUp.ShowPopup(Localization.Get("Do you want to remove", true) + ": " + this.selectPlayer.Name.text, Localization.Get("Delete", true), Localization.Get("Yes", true), delegate()
		{
		}, Localization.Get("No", true), delegate()
		{
			mPopUp.HideAll();
		});
	}

	public void SelectPlayer(mClanPlayerElement element)
	{
		if (ClanManager.admin != AccountManager.instance.Data.ID)
		{
			return;
		}
		if (element == null)
		{
			return;
		}
		if (element.ID == AccountManager.instance.Data.ID)
		{
			return;
		}
		this.selectPlayer = element;
		mPanelManager.ShowTween(this.statsPanel.cachedGameObject);
		this.playerStatsNameLabel.text = element.Name.text;
		this.playerStatsIDLabel.text = element.ID.ToString();
		this.playerStatsLevelLabel.text = string.Empty;
		this.playerStatsXPLabel.text = string.Empty;
		this.playerStatsDeathsLabel.text = string.Empty;
		this.playerStatsKillsLabel.text = string.Empty;
		this.playerStatsHeadshotLabel.text = string.Empty;
		this.playerStatsTimeLabel.text = string.Empty;
		this.playerStatsLastLoginLabel.text = string.Empty;
		if (mClanPlayers.cachePlayerInfo.ContainsKey(element.ID))
		{
			this.UpdatePlayerInfo(mClanPlayers.cachePlayerInfo[element.ID]);
		}
		else
		{
			AccountManager.GetFriendsInfo(element.ID, delegate(string result)
			{
				mClanPlayers.cachePlayerInfo[element.ID] = result;
				this.UpdatePlayerInfo(result);
			}, delegate(string error)
			{
				UIToast.Show("Get Friend Info Error: " + error);
			});
		}
	}

	private void UpdatePlayerInfo(string result)
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

	public void UpdateList()
	{
		this.DeactiveAll();
		for (int i = 0; i < ClanManager.players.Length; i++)
		{
			this.GetContainer().SetData(ClanManager.players[i]);
		}
		this.grid.repositionNow = true;
		this.UpdatePlayers();
	}

	private mClanPlayerElement GetContainer()
	{
		if (this.deactiveContainers.Count != 0)
		{
			mClanPlayerElement mClanPlayerElement = this.deactiveContainers[0];
			this.deactiveContainers.RemoveAt(0);
			mClanPlayerElement.Widget.cachedGameObject.SetActive(true);
			this.activeContainers.Add(mClanPlayerElement);
			return mClanPlayerElement;
		}
		GameObject gameObject = this.grid.gameObject.AddChild(this.container.Widget.cachedGameObject);
		gameObject.SetActive(true);
		mClanPlayerElement component = gameObject.GetComponent<mClanPlayerElement>();
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

	private void UpdatePlayers()
	{
		List<int> list = new List<int>();
		string a = string.Empty;
		for (int i = 0; i < ClanManager.players.Length; i++)
		{
			a = CryptoPrefs.GetString("Friend_#" + ClanManager.players[i].ToString(), "null");
			if (a == "null")
			{
				list.Add(ClanManager.players[i]);
			}
		}
		if (list.Count != 0)
		{
			this.GetPlayersNames(list.ToArray());
		}
	}

	public void GetPlayersNames(int[] ids)
	{
		AccountManager.GetFriendsName(ids, delegate
		{
			this.UpdateList();
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
