using System;
using System.Collections.Generic;
using UnityEngine;

public class mQuickPlay : MonoBehaviour
{
    public GameMode SelectMode;

    public UIPopupList SelectModePopupList;

    public UIPopupList SelectMapPopupList;

    public GameObject SelectedMaxPlayers;

    public UILabel[] MaxPlayersList;

    public UIGrid Grid;

    private int MaxPlayers;

    private RoomInfo selectRoom;

    private bool defaultMaxPlayers = true;

    public void Open()
	{
		this.SelectModePopupList.Clear();
		this.SelectModePopupList.AddItem("Any");
		GameMode[] gameMode = GameModeManager.gameMode;
		for (int i = 0; i < gameMode.Length; i++)
		{
			this.SelectModePopupList.AddItem(gameMode[i].ToString());
		}
		this.SelectModePopupList.value = this.SelectModePopupList.items[0];
		this.UpdateMaps();
	}

	private void UpdateMaps()
	{
		if (this.SelectModePopupList.value == "Any")
		{
			this.SelectMapPopupList.transform.parent.gameObject.SetActive(false);
		}
		else
		{
			this.SelectMapPopupList.transform.parent.gameObject.SetActive(true);
			List<string> gameModeScenes = LevelManager.GetGameModeScenes((GameMode)((int)Enum.Parse(typeof(GameMode), this.SelectModePopupList.value)));
			this.SelectMapPopupList.Clear();
			this.SelectMapPopupList.AddItem(Localization.Get("Any", true));
			for (int i = 0; i < gameModeScenes.Count; i++)
			{
				this.SelectMapPopupList.AddItem(gameModeScenes[i]);
			}
			this.SelectMapPopupList.value = this.SelectMapPopupList.items[0];
		}
		this.Grid.repositionNow = true;
	}

	public void OnSelectGameMode()
	{
		this.UpdateMaps();
		if (this.SelectModePopupList.value != "Any" && this.SelectMapPopupList.value != "Any")
		{
			this.SelectMode = (GameMode)((int)Enum.Parse(typeof(GameMode), this.SelectModePopupList.value));
			mServerSettings.Check(this.SelectMode, this.SelectMapPopupList.value);
		}
		else
		{
			this.SetDefaultMaxPlayers();
		}
	}

	public void OnSelectMap()
	{
		if (this.SelectModePopupList.value != "Any" && this.SelectMapPopupList.value != "Any")
		{
			mServerSettings.Check(this.SelectMode, this.SelectMapPopupList.value);
		}
		else
		{
			this.SetDefaultMaxPlayers();
		}
	}

	public void SetMaxPlayer(GameObject go)
	{
		if (go.name == "-")
		{
			this.MaxPlayers = 0;
		}
		else
		{
			this.MaxPlayers = int.Parse(go.name);
		}
		TweenPosition.Begin(this.SelectedMaxPlayers, 0.2f, go.transform.localPosition);
	}

	public void SetDefaultMaxPlayers()
	{
		if (this.defaultMaxPlayers)
		{
			return;
		}
		this.SetMaxPlayers(new int[]
		{
			4,
			6,
			8,
			10,
			12
		});
		this.defaultMaxPlayers = true;
	}

	public void SetMaxPlayers(int[] list)
	{
		if (list.Length > 5)
		{
			Debug.LogError("Max list <=5");
			return;
		}
		this.defaultMaxPlayers = false;
		for (int i = 0; i < this.MaxPlayersList.Length; i++)
		{
			MonoBehaviour.print(i);
			this.MaxPlayersList[i].cachedGameObject.SetActive(false);
		}
		for (int j = 0; j < list.Length; j++)
		{
			this.MaxPlayersList[j].cachedGameObject.SetActive(true);
			this.MaxPlayersList[j].cachedGameObject.name = list[j].ToString();
			this.MaxPlayersList[j].text = list[j].ToString();
		}
		this.SetMaxPlayer(this.MaxPlayersList[0].cachedGameObject);
	}

	public void QuickPlay()
	{
		mPopUp.ShowText(Localization.Get("Search Server", true) + "...");
		TimerManager.In(0.2f + UnityEngine.Random.value, delegate()
		{
			this.SelectServer(this.SelectModePopupList.value, this.SelectMapPopupList.value, this.MaxPlayers);
		});
	}

	private void SelectServer(string mode, string map, int maxPlayers)
	{
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		List<RoomInfo> list = new List<RoomInfo>();
		for (int i = 0; i < roomList.Length; i++)
		{
			if (string.IsNullOrEmpty(roomList[i].GetPassword()) && roomList[i].PlayerCount != (int)roomList[i].MaxPlayers && roomList[i].GetCustomMapHash() == 0 && (mode == "Any" || roomList[i].GetGameMode().ToString() == mode) && (map == Localization.Get("Any", true) || roomList[i].GetSceneName() == map || mode == "Any") && (maxPlayers == 0 || (int)roomList[i].MaxPlayers == maxPlayers))
			{
				list.Add(roomList[i]);
			}
		}
		if (list.Count == 0)
		{
			mPopUp.HideAll("Server");
			mPopUp.ShowPopup(Localization.Get("The server with the selected data was not found. You want to create your own server?", true), Localization.Get("Search Server", true), Localization.Get("Yes", true), delegate()
			{
				mCreateServer.OpenPanel();
				mPopUp.HideAll("CreateServer");
			}, Localization.Get("No", true), delegate()
			{
				mPopUp.HideAll("Server");
			});
			return;
		}
		if (maxPlayers == 0)
		{
			this.selectRoom = list[UnityEngine.Random.Range(0, list.Count)];
		}
		else
		{
			list.Sort(new Comparison<RoomInfo>(mQuickPlay.SortByPlayerCount));
			int playerCount = list[0].PlayerCount;
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j].PlayerCount != playerCount)
				{
					list.RemoveAt(j);
					j = 0;
				}
			}
			this.selectRoom = list[UnityEngine.Random.Range(0, list.Count)];
		}
		mPhotonSettings.JoinServer(this.selectRoom);
	}

	public static int SortByPlayerCount(RoomInfo a, RoomInfo b)
	{
		return b.PlayerCount.CompareTo(a.PlayerCount);
	}
}
