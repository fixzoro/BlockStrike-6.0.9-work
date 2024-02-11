using System;
using System.Collections.Generic;
using UnityEngine;

public class mOfficialServer : MonoBehaviour
{
    public UIPopupList selectModePopupList;

    public UIPopupList selectMapPopupList;

    public static GameMode[] officialGameMode = new GameMode[]
    {
        GameMode.TeamDeathmatch,
        GameMode.Classic,
        GameMode.Bomb,
        GameMode.Bomb2,
        GameMode.GunGame,
        GameMode.Escort,
        GameMode.RandomMode
    };

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
				mPopUp.HideAll("OfficialServers");
				this.OnStart();
			};
			mPhotonSettings.Connect();
			return;
		}
		mPanelManager.Show("OfficialServers", true);
		this.OnStart();
	}

	private void OnStart()
	{
		this.selectModePopupList.Clear();
		this.selectModePopupList.AddItem("Any");
		GameMode[] array = mOfficialServer.officialGameMode;
		for (int i = 0; i < array.Length; i++)
		{
			this.selectModePopupList.AddItem(array[i].ToString());
		}
		this.selectModePopupList.value = this.selectModePopupList.items[0];
		this.UpdateMaps();
	}

	private void UpdateMaps()
	{
		if (this.selectModePopupList.value == "Any")
		{
			this.selectMapPopupList.GetComponent<Collider>().enabled = false;
			this.selectMapPopupList.Clear();
			this.selectMapPopupList.AddItem(Localization.Get("Any", true));
			this.selectMapPopupList.GetComponent<UIWidget>().alpha = 0.5f;
			this.selectMapPopupList.value = this.selectMapPopupList.items[0];
		}
		else
		{
			this.selectMapPopupList.GetComponent<Collider>().enabled = true;
			this.selectMapPopupList.GetComponent<UIWidget>().alpha = 1f;
			List<string> gameModeScenes = LevelManager.GetGameModeScenes((GameMode)((int)Enum.Parse(typeof(GameMode), this.selectModePopupList.value)));
			this.selectMapPopupList.Clear();
			this.selectMapPopupList.AddItem(Localization.Get("Any", true));
			for (int i = 0; i < gameModeScenes.Count; i++)
			{
				this.selectMapPopupList.AddItem(gameModeScenes[i]);
			}
			this.selectMapPopupList.value = this.selectMapPopupList.items[0];
		}
	}

	public void OnSelectGameMode()
	{
		this.UpdateMaps();
	}

	public void Join()
	{
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		List<RoomInfo> list = new List<RoomInfo>();
		for (int i = 0; i < roomList.Length; i++)
		{
			if (roomList[i].isOfficialServer())
			{
				list.Add(roomList[i]);
			}
		}
		if (this.selectModePopupList.value != "Any")
		{
			GameMode gameMode = (GameMode)((int)Enum.Parse(typeof(GameMode), this.selectModePopupList.value));
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j].GetGameMode() != gameMode)
				{
					list.RemoveAt(j);
					j--;
				}
			}
		}
		if (this.selectMapPopupList.value != Localization.Get("Any", true))
		{
			string value = this.selectMapPopupList.value;
			for (int k = 0; k < list.Count; k++)
			{
				if (list[k].GetSceneName() != value)
				{
					list.RemoveAt(k);
					k--;
				}
			}
		}
		for (int l = 0; l < list.Count; l++)
		{
			if (list[l].PlayerCount == (int)list[l].MaxPlayers)
			{
				list.RemoveAt(l);
				l--;
			}
		}
		byte b = (byte)AccountManager.GetLevel();
		for (int m = 0; m < list.Count; m++)
		{
			if (list[m].GetMinLevel() > b + 10)
			{
				list.RemoveAt(m);
				m--;
			}
		}
		list.Sort((RoomInfo x, RoomInfo y) => y.GetMinLevel().CompareTo(x.GetMinLevel()));
		if (list != null && list.Count != 0)
		{
			mPhotonSettings.JoinServer(list[0]);
		}
		else
		{
			string map = string.Empty;
			string name = "off_" + UnityEngine.Random.Range(1000000, 100000000);
			GameMode mode;
			if (this.selectModePopupList.value == "Any")
			{
				mode = mOfficialServer.officialGameMode[UnityEngine.Random.Range(0, mOfficialServer.officialGameMode.Length)];
			}
			else
			{
				mode = (GameMode)((int)Enum.Parse(typeof(GameMode), this.selectModePopupList.value));
			}
			if (this.selectMapPopupList.value == Localization.Get("Any", true))
			{
				List<string> gameModeScenes = LevelManager.GetGameModeScenes(mode);
				map = gameModeScenes[UnityEngine.Random.Range(0, gameModeScenes.Count)];
			}
			else
			{
				map = this.selectMapPopupList.value;
			}
			mPhotonSettings.CreateOfficialServer(name, map, mode);
		}
	}
}
