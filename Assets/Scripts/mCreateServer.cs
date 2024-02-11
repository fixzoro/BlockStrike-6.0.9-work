using System;
using System.Collections.Generic;
using BSCM;
using UnityEngine;

public class mCreateServer : MonoBehaviour
{
    public GameMode selectMode;

    public UIPopupList modePopupList;

    public UIPopupList mapPopupList;

    public UIInput serverNameInput;

    public UISprite[] maxPlayersSprite;

    public int selectMaxPlayers = 4;

    public UILabel[] maxPlayersLabel;

    public UIInput passwordInput;

    public GameObject customMaps;

    public UIToggle customMapsToggle;

    private static mCreateServer instance;

    public static GameMode mode
	{
		get
		{
			return mCreateServer.instance.selectMode;
		}
	}

	public static string map
	{
		get
		{
			return mCreateServer.instance.mapPopupList.value;
		}
	}

	public static string serverName
	{
		get
		{
			return mCreateServer.instance.serverNameInput.value;
		}
		set
		{
			mCreateServer.instance.serverNameInput.value = value;
		}
	}

	public static int maxPlayers
	{
		get
		{
			return mCreateServer.instance.selectMaxPlayers;
		}
	}

	public static string password
	{
		get
		{
			return mCreateServer.instance.passwordInput.value;
		}
	}

	public static bool custom
	{
		get
		{
			return mCreateServer.instance.customMapsToggle.value;
		}
	}

	private void Start()
	{
		mCreateServer.instance = this;
	}

	public void Open()
	{
		if (mCreateServer.custom)
		{
			Manager.Start();
		}
		this.customMaps.SetActive(Manager.enabled && AccountManager.GetLevel() >= 10);
		mCreateServer.serverName = Localization.Get("Room", true) + " " + UnityEngine.Random.Range(0, 99999);
		this.modePopupList.Clear();
		GameMode[] array = (!mCreateServer.custom) ? GameModeManager.gameMode : GameModeManager.customGameMode;
		for (int i = 0; i < array.Length; i++)
		{
			this.modePopupList.AddItem(array[i].ToString());
		}
		this.modePopupList.value = this.modePopupList.items[0];
		this.UpdateMaps();
	}

	public static void OpenPanel()
	{
		mCreateServer.instance.Open();
	}

	private void UpdateMaps()
	{
		if (!mCreateServer.custom)
		{
			List<string> gameModeScenes = LevelManager.GetGameModeScenes(this.selectMode);
			this.mapPopupList.Clear();
			for (int i = 0; i < gameModeScenes.Count; i++)
			{
				this.mapPopupList.AddItem(gameModeScenes[i]);
			}
			this.mapPopupList.value = this.mapPopupList.items[0];
		}
		else
		{
			string[] mapsList = Manager.GetMapsList(mCreateServer.mode);
			this.mapPopupList.Clear();
			for (int j = 0; j < mapsList.Length; j++)
			{
				this.mapPopupList.AddItem(Manager.GetBundleName(mapsList[j]), mapsList[j], null);
			}
			if (mapsList.Length > 0)
			{
				this.mapPopupList.value = this.mapPopupList.items[0];
			}
			else
			{
				this.mapPopupList.AddItem("-----");
				this.mapPopupList.value = this.mapPopupList.items[0];
				this.mapPopupList.Clear();
			}
		}
	}

	public void SelectGameMode()
	{
		this.selectMode = (GameMode)((int)Enum.Parse(typeof(GameMode), this.modePopupList.value));
		this.UpdateMaps();
		mServerSettings.Check(this.selectMode, mCreateServer.map);
	}

	public void SelectMap()
	{
		mServerSettings.Check(this.selectMode, mCreateServer.map);
	}

	public void CheckServerName()
	{
		if (mCreateServer.serverName.Length < 4 || Utils.IsNullOrWhiteSpace(mCreateServer.serverName) || mCreateServer.serverName.Contains("\n") || BadWordsManager.Contains(mCreateServer.serverName))
		{
			mCreateServer.serverName = Localization.Get("Room", true) + " " + UnityEngine.Random.Range(0, 99999);
		}
		mCreateServer.serverName = NGUIText.StripSymbols(mCreateServer.serverName);
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		for (int i = 0; i < roomList.Length; i++)
		{
			if (roomList[i].Name == mCreateServer.serverName)
			{
				mCreateServer.serverName = Localization.Get("Room", true) + " " + UnityEngine.Random.Range(0, 99999);
				UIToast.Show(Localization.Get("Name already taken", true));
				break;
			}
		}
	}

	public void SetMaxPlayer(GameObject go)
	{
		this.selectMaxPlayers = int.Parse(go.name);
		for (int i = 0; i < this.maxPlayersSprite.Length; i++)
		{
			if (this.maxPlayersLabel[i].text != go.name)
			{
				this.maxPlayersSprite[i].color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 40);
			}
			else
			{
				this.maxPlayersSprite[i].color = new Color32(235, 242, byte.MaxValue, byte.MaxValue);
			}
		}
	}

	public void SetDefaultMaxPlayers()
	{
		this.SetMaxPlayers(new int[]
		{
			4,
			6,
			8,
			10,
			12
		});
	}

	public void SetMaxPlayers(int[] list)
	{
		if (list.Length > 5)
		{
			Debug.LogError("Max list  <=5");
			return;
		}
		for (int i = 0; i < this.maxPlayersSprite.Length; i++)
		{
			this.maxPlayersSprite[i].cachedGameObject.SetActive(false);
		}
		for (int j = 0; j < list.Length; j++)
		{
			this.maxPlayersSprite[j].cachedGameObject.SetActive(true);
			this.maxPlayersSprite[j].cachedGameObject.name = list[j].ToString();
			this.maxPlayersLabel[j].text = list[j].ToString();
		}
		this.SetMaxPlayer(this.maxPlayersSprite[0].cachedGameObject);
	}

	public void CreateServer()
	{
		if (!mCreateServer.custom)
		{
			mPhotonSettings.CreateServer(mCreateServer.serverName, mCreateServer.map, mCreateServer.mode, mCreateServer.password, mCreateServer.maxPlayers, false);
		}
		else
		{
			if (this.mapPopupList.items.Count <= 0)
			{
				return;
			}
			Manager.LoadBundle((string)this.mapPopupList.data);
			mPhotonSettings.CreateServer(mCreateServer.serverName, mCreateServer.map, mCreateServer.mode, mCreateServer.password, mCreateServer.maxPlayers, true);
		}
	}
}
