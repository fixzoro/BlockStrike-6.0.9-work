using System;
using BSCM;
using UnityEngine;

public class mCreateCustomServer : MonoBehaviour
{
    public GameMode selectMode;

    public UIPopupList modePopupList;

    public UIPopupList mapPopupList;

    public UIInput serverNameInput;

    public GameObject selectedMaxPlayers;

    public int selectMaxPlayers = 4;

    public UILabel[] maxPlayersList;

    public UIInput passwordInput;

    private static mCreateCustomServer instance;

    public static GameMode mode
	{
		get
		{
			return mCreateCustomServer.instance.selectMode;
		}
	}

	public static string map
	{
		get
		{
			return mCreateCustomServer.instance.mapPopupList.value;
		}
	}

	public static string serverName
	{
		get
		{
			return mCreateCustomServer.instance.serverNameInput.value;
		}
		set
		{
			mCreateCustomServer.instance.serverNameInput.value = value;
		}
	}

	public static int maxPlayers
	{
		get
		{
			return mCreateCustomServer.instance.selectMaxPlayers;
		}
	}

	public static string password
	{
		get
		{
			return mCreateCustomServer.instance.passwordInput.value;
		}
	}

	private void Start()
	{
		mCreateCustomServer.instance = this;
	}

	public void Open()
	{
		mPanelManager.Show("CreateCustomServer", true);
		mCreateCustomServer.serverName = "Room " + UnityEngine.Random.Range(0, 99999);
		this.modePopupList.Clear();
		GameMode[] customGameMode = GameModeManager.customGameMode;
		for (int i = 0; i < customGameMode.Length; i++)
		{
			this.modePopupList.AddItem(customGameMode[i].ToString());
		}
		this.modePopupList.value = this.modePopupList.items[0];
		this.UpdateMaps();
	}

	public static void OpenPanel()
	{
		mCreateCustomServer.instance.Open();
	}

	private void UpdateMaps()
	{
		string[] mapsList = Manager.GetMapsList(mCreateCustomServer.mode);
		this.mapPopupList.Clear();
		for (int i = 0; i < mapsList.Length; i++)
		{
			this.mapPopupList.AddItem(Manager.GetBundleName(mapsList[i]), mapsList[i], null);
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

	public void SelectGameMode()
	{
		this.selectMode = (GameMode)((int)Enum.Parse(typeof(GameMode), this.modePopupList.value));
		this.UpdateMaps();
		mServerSettings.Check(mCreateCustomServer.mode, mCreateCustomServer.map);
	}

	public void SelectMap()
	{
		mServerSettings.Check(mCreateCustomServer.mode, mCreateCustomServer.map);
	}

	public void CheckServerName()
	{
		if (mCreateCustomServer.serverName.Length < 4 || Utils.IsNullOrWhiteSpace(mCreateCustomServer.serverName) || BadWordsManager.Contains(mCreateCustomServer.serverName))
		{
			mCreateCustomServer.serverName = "Room " + UnityEngine.Random.Range(0, 99999);
		}
		mCreateCustomServer.serverName = NGUIText.StripSymbols(mCreateCustomServer.serverName);
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		for (int i = 0; i < roomList.Length; i++)
		{
			if (roomList[i].Name == mCreateCustomServer.serverName)
			{
				mCreateCustomServer.serverName = "Room " + UnityEngine.Random.Range(0, 99999);
				UIToast.Show(Localization.Get("Name already taken", true));
				break;
			}
		}
	}

	public void SetMaxPlayer(GameObject go)
	{
		this.selectMaxPlayers = int.Parse(go.name);
		TweenPosition.Begin(this.selectedMaxPlayers, 0.2f, go.transform.localPosition);
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
		for (int i = 0; i < this.maxPlayersList.Length; i++)
		{
			this.maxPlayersList[i].cachedGameObject.SetActive(false);
		}
		for (int j = 0; j < list.Length; j++)
		{
			this.maxPlayersList[j].cachedGameObject.SetActive(true);
			this.maxPlayersList[j].cachedGameObject.name = list[j].ToString();
			this.maxPlayersList[j].text = list[j].ToString();
		}
		this.SetMaxPlayer(this.maxPlayersList[0].cachedGameObject);
	}

	public void DeleteMap()
	{
		if (this.mapPopupList.items.Count <= 0)
		{
			return;
		}
		mPopUp.ShowPopup("Удалить карту: " + mCreateCustomServer.map, Localization.Get("Delete", true), Localization.Get("Yes", true), delegate()
		{
			Manager.DeleteBundle(mCreateCustomServer.map);
			this.Open();
			mPopUp.HideAll();
		}, Localization.Get("No", true), delegate()
		{
			mPopUp.HideAll();
		});
	}

	public void CreateServer()
	{
		if (this.mapPopupList.items.Count <= 0)
		{
			return;
		}
		Manager.LoadBundle((string)this.mapPopupList.data);
		mPhotonSettings.CreateServer(mCreateCustomServer.serverName, mCreateCustomServer.map, mCreateCustomServer.mode, mCreateCustomServer.password, mCreateCustomServer.maxPlayers, true);
	}

	public void ShowToast(string text)
	{
		UIToast.Show(Localization.Get(text, true));
	}
}
