using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mServerList : MonoBehaviour
{
    private int selectMode = -1;

    public UILabel infoLabel;

    public UIPopupList modePopupList;

    public GameObject element;

    public GameObject switchTop;

    public UILabel switchTopLabel;

    public GameObject switchBottom;

    public UILabel switchBottomLabel;

    public UIInput searchInput;

    public UIScrollView scrollView;

    public GameObject settings;

    private bool activeSettings;

    public UIToggle officialMapsToogle;

    public UIToggle customMapsToogle;

    public UIToggle fullToogle;

    public UIToggle passwordToogle;

    public UIToggle sortToogle;

    public bool showOfficialMaps;

    public bool showCustomMaps;

    public bool showFullServers = true;

    public bool showPasswordServers = true;

    public bool sortServers;

    private List<GameObject> serverList = new List<GameObject>();

    private List<GameObject> serverListPool = new List<GameObject>();

    private bool updateServerList;

    private int maxPlayers;

    private int maxServers;

    private int maxPlayersTotal;

    private int maxServersTotal;

    private RoomInfo[] roomList;

    private int selectAllModeList = 1;

    private int maxAllModeList;

    private void Start()
	{
		this.showOfficialMaps = nPlayerPrefs.GetBool("showOfficialMaps", this.showOfficialMaps);
		this.showCustomMaps = nPlayerPrefs.GetBool("showCustomMaps", this.showCustomMaps);
		this.showFullServers = nPlayerPrefs.GetBool("showFullServers", this.showFullServers);
		this.showPasswordServers = nPlayerPrefs.GetBool("showPasswordServers", this.showPasswordServers);
		this.sortServers = nPlayerPrefs.GetBool("sortServers", this.sortServers);
		this.officialMapsToogle.value = this.showOfficialMaps;
		this.customMapsToogle.value = this.showCustomMaps;
		this.fullToogle.value = this.showFullServers;
		this.passwordToogle.value = this.showPasswordServers;
		this.sortToogle.value = this.sortServers;
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
				mPopUp.HideAll("ServerList", false);
				this.OnStart();
			};
			mPhotonSettings.Connect();
			return;
		}
		mPanelManager.Show("ServerList", false);
		this.OnStart();
	}

	private void OnStart()
	{
		this.modePopupList.Clear();
		this.modePopupList.AddItem("All", -1, null);
		GameMode[] gameMode = GameModeManager.gameMode;
		for (int i = 0; i < gameMode.Length; i++)
		{
			this.modePopupList.AddItem(gameMode[i].ToString(), (int)gameMode[i], null);
		}
		this.modePopupList.value = "All";
		this.UpdateServerList();
	}

	public void UpdateServerList()
	{
		this.maxAllModeList = 0;
		this.selectAllModeList = 1;
		if (!this.updateServerList)
		{
			this.scrollView.ResetPosition();
			base.StartCoroutine(this.CreateServerList(true));
		}
	}

	private IEnumerator CreateServerList(bool updateList)
	{
		this.updateServerList = true;
		yield return new WaitForSeconds(0.01f);
		this.ClearServerList();
		this.maxPlayers = 0;
		this.maxServers = 0;
		this.maxPlayersTotal = 0;
		this.maxServersTotal = 0;
		if (updateList)
		{
			this.roomList = PhotonNetwork.GetRoomList();
		}
		RoomInfo[] list = this.GetRooms();
		int count = -1;
		for (int i = 0; i < list.Length; i++)
		{
			this.maxServers++;
			this.maxPlayers += list[i].PlayerCount;
		}
		this.switchBottom.SetActive(false);
		this.UpdateServerInfo();
		if (list.Length / 30 >= 1)
		{
			if (this.maxAllModeList == 0)
			{
				this.maxAllModeList = Mathf.CeilToInt((float)list.Length / 30f);
			}
			this.switchTop.SetActive(true);
			this.switchTop.transform.localPosition = Vector3.up * 150f;
			int startIndex = (this.selectAllModeList - 1) * 30;
			int maxLength = 30;
			if (this.selectAllModeList * 30 > list.Length)
			{
				maxLength = list.Length - (this.selectAllModeList - 1) * 30;
			}
			this.switchTopLabel.text = (startIndex + 1).ToString() + "-" + (startIndex + maxLength).ToString();
			this.switchBottomLabel.text = (startIndex + 1).ToString() + "-" + (startIndex + maxLength).ToString();
			for (int j = startIndex; j < startIndex + maxLength; j++)
			{
				Transform element = this.GetElement();
				count++;
				element.GetComponent<mServerInfo>().SetData(list[j]);
				element.localPosition = Vector3.up * (float)(95 - 55 * count);
				element.gameObject.SetActive(true);
				this.serverList.Add(element.gameObject);
				yield return new WaitForSeconds(0.01f);
			}
			this.switchBottom.SetActive(true);
			this.switchBottom.transform.localPosition = Vector3.up * (float)(95 - 55 * (count + 1));
		}
		else
		{
			this.switchTop.SetActive(false);
			for (int k = 0; k < list.Length; k++)
			{
				if (this.selectMode == -1 || this.selectMode == (int)list[k].GetGameMode())
				{
					Transform element2 = this.GetElement();
					count++;
					element2.GetComponent<mServerInfo>().SetData(list[k]);
					element2.localPosition = Vector3.up * (float)(150 - 55 * count);
					element2.gameObject.SetActive(true);
					this.serverList.Add(element2.gameObject);
					yield return new WaitForSeconds(0.01f);
				}
			}
		}
		this.updateServerList = false;
		yield break;
	}

	private RoomInfo[] GetRooms()
	{
		List<RoomInfo> list = this.FilterSearch(this.roomList);
		list = this.FilterSelectMode(list);
		list = this.FilterOfficialServer(list);
		list = this.FilterShowFullServers(list);
		list = this.FilterShowPasswordServers(list);
		list = this.FilterCustomMaps(list);
		list = this.SortPlayers(list);
		for (int i = 0; i < this.roomList.Length; i++)
		{
			if (!this.roomList[i].isOfficialServer())
			{
				this.maxServersTotal++;
				this.maxPlayersTotal += this.roomList[i].PlayerCount;
			}
		}
		return list.ToArray();
	}

	private List<RoomInfo> FilterSearch(RoomInfo[] array)
	{
		List<RoomInfo> list = new List<RoomInfo>();
		if (!string.IsNullOrEmpty(this.searchInput.value))
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Name.ToLower().Contains(this.searchInput.value.ToLower()))
				{
					list.Add(array[i]);
				}
			}
		}
		else
		{
			for (int j = 0; j < array.Length; j++)
			{
				list.Add(array[j]);
			}
		}
		return list;
	}

	private List<RoomInfo> FilterSelectMode(List<RoomInfo> list)
	{
		if (this.selectMode != -1)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (this.selectMode != (int)list[i].GetGameMode())
				{
					list.RemoveAt(i);
					i--;
				}
			}
		}
		return list;
	}

	private List<RoomInfo> FilterOfficialServer(List<RoomInfo> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].isOfficialServer())
			{
				list.RemoveAt(i);
				i--;
			}
		}
		return list;
	}

	private List<RoomInfo> FilterShowFullServers(List<RoomInfo> list)
	{
		if (!this.showFullServers)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if ((int)list[i].MaxPlayers == list[i].PlayerCount)
				{
					list.RemoveAt(i);
					i--;
				}
			}
		}
		return list;
	}

	private List<RoomInfo> FilterShowPasswordServers(List<RoomInfo> list)
	{
		if (!this.showPasswordServers)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].HasPassword())
				{
					list.RemoveAt(i);
					i--;
				}
			}
		}
		return list;
	}

	private List<RoomInfo> FilterCustomMaps(List<RoomInfo> list)
	{
		if (!this.showOfficialMaps)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (!list[i].isCustomMap() || string.IsNullOrEmpty(list[i].GetSceneName()) || !GameModeManager.ContainsCustomMode(list[i].GetGameMode()))
				{
					list.RemoveAt(i);
					i--;
				}
			}
		}
		if (!this.showCustomMaps)
		{
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j].isCustomMap())
				{
					list.RemoveAt(j);
					j--;
				}
			}
		}
		if (this.officialMapsToogle.value && this.customMapsToogle.value)
		{
			for (int k = 0; k < list.Count; k++)
			{
				if (list[k].isCustomMap() && !GameModeManager.ContainsCustomMode(list[k].GetGameMode()))
				{
					list.RemoveAt(k);
					k--;
				}
			}
		}
		return list;
	}

	private List<RoomInfo> SortPlayers(List<RoomInfo> list)
	{
		if (this.sortServers)
		{
			list.Sort((RoomInfo x, RoomInfo y) => y.PlayerCount.CompareTo(x.PlayerCount));
		}
		return list;
	}

	private Transform GetElement()
	{
		GameObject gameObject;
		if (this.serverListPool.Count != 0)
		{
			gameObject = this.serverListPool[0];
			this.serverListPool.RemoveAt(0);
		}
		else
		{
			gameObject = this.scrollView.gameObject.AddChild(this.element);
		}
		return gameObject.transform;
	}

	private void ClearServerList()
	{
		for (int i = 0; i < this.serverList.Count; i++)
		{
			this.serverList[i].SetActive(false);
			this.serverListPool.Add(this.serverList[i]);
		}
		this.serverList.Clear();
	}

	private void UpdateServerInfo()
	{
		string text = string.Empty;
		if (this.maxPlayers == this.maxPlayersTotal)
		{
			text = string.Concat(new object[]
			{
				Localization.Get("Players", true),
				": ",
				this.maxPlayers,
				"\n",
				Localization.Get("Servers", true),
				": ",
				this.maxServers,
				"\n",
				Localization.Get("Ping", true),
				": ",
				PhotonNetwork.GetPing()
			});
		}
		else
		{
			text = string.Concat(new object[]
			{
				Localization.Get("Players", true),
				": ",
				this.maxPlayers,
				" (",
				this.maxPlayersTotal,
				") \n",
				Localization.Get("Servers", true),
				": ",
				this.maxServers,
				" (",
				this.maxServersTotal,
				") \n",
				Localization.Get("Ping", true),
				": ",
				PhotonNetwork.GetPing()
			});
		}
		this.infoLabel.text = text;
	}

	public void SelectMode()
	{
		if (this.selectMode == (int)this.modePopupList.data)
		{
			return;
		}
		this.selectMode = (int)this.modePopupList.data;
		this.maxAllModeList = 0;
		this.selectAllModeList = 1;
		if (this.updateServerList)
		{
			this.updateServerList = false;
			base.StopCoroutine(this.CreateServerList(false));
		}
		this.scrollView.ResetPosition();
		base.StartCoroutine(this.CreateServerList(true));
	}

	public void RightSwitch()
	{
		this.selectAllModeList++;
		if (this.selectAllModeList > this.maxAllModeList)
		{
			this.selectAllModeList = 1;
		}
		if (this.updateServerList)
		{
			this.updateServerList = false;
			base.StopCoroutine(this.CreateServerList(false));
		}
		this.scrollView.ResetPosition();
		base.StartCoroutine(this.CreateServerList(false));
	}

	public void LeftSwitch()
	{
		this.selectAllModeList--;
		if (this.selectAllModeList <= 0)
		{
			this.selectAllModeList = this.maxAllModeList;
		}
		if (this.updateServerList)
		{
			this.updateServerList = false;
			base.StopCoroutine(this.CreateServerList(false));
		}
		this.scrollView.ResetPosition();
		base.StartCoroutine(this.CreateServerList(false));
	}

	public void SettingsClick()
	{
		this.activeSettings = !this.activeSettings;
		if (this.activeSettings)
		{
			mPanelManager.ShowTween(this.settings);
			this.officialMapsToogle.value = this.showOfficialMaps;
			this.customMapsToogle.value = this.showCustomMaps;
			this.fullToogle.value = this.showFullServers;
			this.passwordToogle.value = this.showPasswordServers;
			this.sortToogle.value = this.sortServers;
		}
		else
		{
			mPanelManager.HideTween(this.settings);
		}
	}

	public void SettingsToogle()
	{
		this.showOfficialMaps = this.officialMapsToogle.value;
		this.showCustomMaps = this.customMapsToogle.value;
		this.showFullServers = this.fullToogle.value;
		this.showPasswordServers = this.passwordToogle.value;
		this.sortServers = this.sortToogle.value;
		nPlayerPrefs.SetBool("showOfficialMaps", this.officialMapsToogle.value);
		nPlayerPrefs.SetBool("showCustomMaps", this.customMapsToogle.value);
		nPlayerPrefs.SetBool("showFullServers", this.fullToogle.value);
		nPlayerPrefs.SetBool("showPasswordServers", this.passwordToogle.value);
		nPlayerPrefs.SetBool("sortServers", this.sortToogle.value);
	}
}
