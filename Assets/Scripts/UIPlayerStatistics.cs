using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Photon;
using UnityEngine;

public class UIPlayerStatistics : Photon.MonoBehaviour
{
    [Header("Panel")]
    public GameObject RedBluePanel;

    public GameObject OnlyBluePanel;

    [Header("Scrolls")]
    public UIScrollView RedScroll;

    public UIScrollView BlueScroll;

    public UIScrollView BlueScroll2;

    [Header("Parents")]
    public Transform BlueTeamParent;

    public Transform RedTeamParent;

    public Transform BlueTeamParent2;

    [Header("Panel Labels")]
    public UILabel SpectatorsLabel;

    public UILabel SpectatorsLabel2;

    [Header("Server")]
    public UILabel ServerNameLabel;

    public UILabel MapNameLabel;

    public UILabel ServerNameLabel2;

    public UILabel MapNameLabel2;

    [Header("Score")]
    public UILabel BlueScore;

    public UILabel RedScore;

    [Header("Deaths")]
    public UILabel BlueDeath;

    public UILabel RedDeath;

    [Header("Player Info")]
    public GameObject PlayerInfo;

    public UILabel PlayerInfoName;

    public UILabel PlayerInfoID;

    public UILabel PlayerInfoLevel;

    public UILabel PlayerInfoXP;

    public UILabel PlayerInfoKD;

    public UILabel PlayerInfoHK;

    public UITexture PlayerInfoAvatar;

    private Dictionary<string, UIPlayerStatistics.PlayerInfoClass> PlayerInfoList = new Dictionary<string, UIPlayerStatistics.PlayerInfoClass>();

    [Header("Others")]
    public GameObject Container;

    public float ContainerGrid = 25f;

    public GameObject Root;

    public GameObject Root2;

    public static PhotonPlayer SelectPlayer;

    public static bool isOnlyBluePanel;

    private bool isShow;

    private List<UIPlayerStatisticsElement> PlayerList = new List<UIPlayerStatisticsElement>();

    private List<UIPlayerStatisticsElement> PlayerListPool = new List<UIPlayerStatisticsElement>();

    private void Start()
	{
		PhotonRPC.AddMessage("PhotonGetSelectPlayerInfo", new PhotonRPC.MessageDelegate(this.PhotonGetSelectPlayerInfo));
		PhotonRPC.AddMessage("PhotonSetSelectPlayerInfo", new PhotonRPC.MessageDelegate(this.PhotonSetSelectPlayerInfo));
        this.Container.SetActive(false);
	}

	private void OnEnable()
	{
		UIPlayerStatistics.isOnlyBluePanel = false;
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.GetButtonDown));
	}

	private void OnDisable()
	{
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.GetButtonDown));
	}

	private void GetButtonDown(string name)
	{
		if (name == "Statistics")
		{
			this.Show();
		}
	}

	private void Show()
	{
		if (PhotonNetwork.offlineMode)
		{
			return;
		}
		if (GameManager.loadingLevel)
		{
			return;
		}
		UIPanelManager.ShowPanel("Statistics");
		if (UIPlayerStatistics.isOnlyBluePanel)
		{
			this.ShowOnlyBluePanel();
		}
		else
		{
			base.StartCoroutine(this.ShowRedBluePanel());
		}
	}

	private IEnumerator ShowRedBluePanel()
	{
		this.RedBluePanel.SetActive(true);
		StringBuilder builderServerName = new StringBuilder();
		StringBuilder builderMapName = new StringBuilder();
		if (PhotonNetwork.room.isOfficialServer())
		{
			builderServerName.Append(PhotonNetwork.room.Name.Replace("off", Localization.Get("Official Servers", true)));
		}
		else
		{
			builderServerName.Append(PhotonNetwork.room.Name);
		}
		builderServerName.Append(" | ");
		if (PhotonNetwork.room.GetGameMode() == GameMode.Only)
		{
			builderServerName.Append(Localization.Get(PhotonNetwork.room.GetGameMode().ToString(), true) + " (" + WeaponManager.GetWeaponName(PhotonNetwork.room.GetOnlyWeapon()) + ")");
		}
		else
		{
			builderServerName.Append(Localization.Get(PhotonNetwork.room.GetGameMode().ToString(), true));
		}
		builderMapName.Append(PhotonNetwork.room.GetSceneName());
		builderMapName.Append(" | ");
		builderMapName.Append(StringCache.Get(PhotonNetwork.room.PlayerCount) + "/" + StringCache.Get(PhotonNetwork.room.MaxPlayers));
		this.ServerNameLabel.text = builderServerName.ToString();
		this.MapNameLabel.text = builderMapName.ToString();
		this.BlueScore.text = StringCache.Get(GameManager.blueScore);
		this.RedScore.text = StringCache.Get(GameManager.redScore);
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		List<PhotonPlayer> bluePlayers = new List<PhotonPlayer>();
		List<PhotonPlayer> redPlayers = new List<PhotonPlayer>();
		List<string> spectatorPlayers = new List<string>();
		byte blueDeath = 0;
		byte redDeath = 0;
		for (int i = 0; i < playerList.Length; i++)
		{
			if (playerList[i].GetTeam() == Team.Blue)
			{
				bluePlayers.Add(playerList[i]);
				if (!playerList[i].GetDead())
				{
					blueDeath += 1;
				}
			}
			else if (playerList[i].GetTeam() == Team.Red)
			{
				redPlayers.Add(playerList[i]);
				if (!playerList[i].GetDead())
				{
					redDeath += 1;
				}
			}
			else if (playerList[i].GetTeam() == Team.None)
			{
				spectatorPlayers.Add(playerList[i].UserId);
			}
		}
		this.BlueDeath.text = StringCache.Get((int)blueDeath) + "/" + StringCache.Get(bluePlayers.Count);
		this.RedDeath.text = StringCache.Get((int)redDeath) + "/" + StringCache.Get(redPlayers.Count);
		bluePlayers.Sort(new Comparison<PhotonPlayer>(UIPlayerStatistics.SortByKills));
		redPlayers.Sort(new Comparison<PhotonPlayer>(UIPlayerStatistics.SortByKills));
		if (spectatorPlayers.Count == 0)
		{
			this.SpectatorsLabel.text = string.Empty;
		}
		else
		{
			this.SpectatorsLabel.text = Localization.Get("Spectators", true) + ": " + string.Join(",", spectatorPlayers.ToArray());
		}
		for (int j = 0; j < bluePlayers.Count; j++)
		{
			yield return new WaitForSeconds(0.005f);
			UIPlayerStatisticsElement container = this.GetPlayerContainer(bluePlayers[j].UserId);
			container.SetData(bluePlayers[j]);
			container.DragScroll.scrollView = this.BlueScroll;
			container.cachedTransform.SetParent(this.BlueTeamParent);
			if (j == 0)
			{
				container.cachedTransform.localPosition = Vector3.zero;
			}
			else
			{
				container.cachedTransform.localPosition = Vector3.down * this.ContainerGrid * (float)j;
			}
			container.cachedTransform.localPosition = new Vector3(container.cachedTransform.localPosition.x, container.cachedTransform.localPosition.y, 0f);
			this.PlayerList.Add(container);
		}
		for (int k = 0; k < redPlayers.Count; k++)
		{
			yield return new WaitForSeconds(0.005f);
			UIPlayerStatisticsElement container2 = this.GetPlayerContainer(redPlayers[k].UserId);
			container2.SetData(redPlayers[k]);
			container2.DragScroll.scrollView = this.RedScroll;
			container2.cachedTransform.SetParent(this.RedTeamParent);
			if (k == 0)
			{
				container2.cachedTransform.localPosition = Vector3.zero;
			}
			else
			{
				container2.cachedTransform.localPosition = Vector3.down * this.ContainerGrid * (float)k;
			}
			container2.cachedTransform.localPosition = new Vector3(container2.cachedTransform.localPosition.x, container2.cachedTransform.localPosition.y, 0f);
			this.PlayerList.Add(container2);
		}
		yield break;
	}

	private void ShowOnlyBluePanel()
	{
		this.OnlyBluePanel.SetActive(true);
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder.Append(PhotonNetwork.room.Name);
		stringBuilder.Append(" | ");
		stringBuilder.Append(Localization.Get(PhotonNetwork.room.GetGameMode().ToString(), true));
		stringBuilder2.Append(PhotonNetwork.room.GetSceneName());
		stringBuilder2.Append(" | ");
		stringBuilder2.Append(StringCache.Get(PhotonNetwork.room.PlayerCount) + "/" + StringCache.Get(PhotonNetwork.room.MaxPlayers));
		this.ServerNameLabel2.text = stringBuilder.ToString();
		this.MapNameLabel2.text = stringBuilder2.ToString();
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		List<PhotonPlayer> list = new List<PhotonPlayer>();
		List<string> list2 = new List<string>();
		for (int i = 0; i < playerList.Length; i++)
		{
			if (playerList[i].GetTeam() == Team.Blue)
			{
				list.Add(playerList[i]);
			}
			else if (playerList[i].GetTeam() == Team.None)
			{
				list2.Add(playerList[i].UserId);
			}
		}
		list.Sort(new Comparison<PhotonPlayer>(UIPlayerStatistics.SortByKills));
		if (list2.Count == 0)
		{
			this.SpectatorsLabel2.text = string.Empty;
		}
		else
		{
			this.SpectatorsLabel2.text = Localization.Get("Spectators", true) + ": " + string.Join(",", list2.ToArray());
		}
		for (int j = 0; j < list.Count; j++)
		{
			UIPlayerStatisticsElement playerContainer = this.GetPlayerContainer(list[j].UserId);
			playerContainer.SetData(list[j]);
			playerContainer.DragScroll.scrollView = this.BlueScroll2;
			playerContainer.cachedTransform.SetParent(this.BlueTeamParent2);
			if (j == 0)
			{
				playerContainer.cachedTransform.localPosition = new Vector3(0f, 50f, 0f);
			}
			else
			{
				playerContainer.cachedTransform.localPosition = new Vector3(0f, 50f - this.ContainerGrid * (float)j, 0f);
			}
			this.PlayerList.Add(playerContainer);
		}
	}

	public void Close()
	{
		base.StopAllCoroutines();
		UIPanelManager.ShowPanel("Display");
		this.ClearList();
	}

	private UIPlayerStatisticsElement GetPlayerContainer(string name)
	{
		if (this.PlayerListPool.Count > 1)
		{
			for (int i = 0; i < this.PlayerListPool.Count; i++)
			{
				if (this.PlayerListPool[i].PlayerNameLabel.text == name)
				{
					UIPlayerStatisticsElement result = this.PlayerListPool[i];
					this.PlayerListPool.RemoveAt(i);
					return result;
				}
			}
			GameObject gameObject = ((!UIPlayerStatistics.isOnlyBluePanel) ? this.Root : this.Root2).AddChild(this.Container);
			return gameObject.GetComponent<UIPlayerStatisticsElement>();
		}
		GameObject gameObject2 = ((!UIPlayerStatistics.isOnlyBluePanel) ? this.Root : this.Root2).AddChild(this.Container);
		return gameObject2.GetComponent<UIPlayerStatisticsElement>();
	}

	private void ClearList()
	{
		if (this.PlayerList.Count != 0)
		{
			for (int i = 0; i < this.PlayerList.Count; i++)
			{
				this.PlayerListPool.Add(this.PlayerList[i]);
			}
			this.PlayerList.Clear();
			for (int j = 0; j < this.PlayerListPool.Count; j++)
			{
				this.PlayerListPool[j].Widget.alpha = 0f;
			}
		}
	}

	public static int SortByKills(PhotonPlayer a, PhotonPlayer b)
	{
		if (a.GetKills() != b.GetKills())
		{
			return b.GetKills().CompareTo(a.GetKills());
		}
		if (a.GetDeaths() != b.GetDeaths())
		{
			return a.GetDeaths().CompareTo(b.GetDeaths());
		}
		if (a.GetLevel() == b.GetLevel())
		{
			return b.UserId.CompareTo(a.UserId);
		}
		return b.GetLevel().CompareTo(a.GetLevel());
	}

	public static int GetPlayerStatsPosition(PhotonPlayer player)
	{
		List<PhotonPlayer> list = new List<PhotonPlayer>();
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
		{
			if (PhotonNetwork.playerList[i].GetTeam() == player.GetTeam())
			{
				list.Add(PhotonNetwork.playerList[i]);
			}
		}
		list.Sort(new Comparison<PhotonPlayer>(UIPlayerStatistics.SortByKills));
		for (int j = 0; j < list.Count; j++)
		{
			if (list[j] == player)
			{
				return j + 1;
			}
		}
		return 1;
	}

	public void OnSelectPlayer(UIPlayerStatisticsElement player)
	{
        if (player.PlayerInfo == null || player.PlayerInfo.IsLocal)
		{
			return;
		}
		this.PlayerInfoAvatar.cachedGameObject.SetActive(false);
		UIPlayerStatistics.SelectPlayer = player.PlayerInfo;
		this.PlayerInfo.SetActive(true);
		this.PlayerInfoName.text = player.PlayerInfo.UserId;
		this.PlayerInfoID.text = "ID: " + player.PlayerInfo.GetPlayerID();
		this.PlayerInfoLevel.text = StringCache.Get(player.PlayerInfo.GetLevel());
		if (this.PlayerInfoList.ContainsKey(player.PlayerInfo.UserId))
		{
			UIPlayerStatistics.PlayerInfoClass playerInfoClass = this.PlayerInfoList[player.PlayerInfo.UserId];
			this.PhotonSetSelectPlayerInfo(playerInfoClass.xp, playerInfoClass.deaths, playerInfoClass.kills, playerInfoClass.headshot);
		}
		else
		{
			this.PlayerInfoXP.text = "-";
			this.PlayerInfoKD.text = "-";
			this.PlayerInfoHK.text = "-";
			PhotonRPC.RPC("PhotonGetSelectPlayerInfo", player.PlayerInfo);
		}
		if (Settings.ShowAvatars)
		{
			AvatarManager.Get(player.PlayerInfo.GetAvatarUrl(), delegate(Texture2D r)
			{
				this.PlayerInfoAvatar.cachedGameObject.SetActive(true);
				this.PlayerInfoAvatar.mainTexture = r;
			});
		}
		else
		{
			this.PlayerInfoAvatar.cachedGameObject.SetActive(true);
			this.PlayerInfoAvatar.mainTexture = GameSettings.instance.NoAvatarTexture;
		}
	}

	[PunRPC]
	private void PhotonGetSelectPlayerInfo(PhotonMessage message)
	{
		int xp = AccountManager.GetXP();
		int deaths = AccountManager.GetDeaths();
		int kills = AccountManager.GetKills();
		int headshot = AccountManager.GetHeadshot();
		PhotonDataWrite data = PhotonRPC.GetData();
		data.Write(xp);
		data.Write(deaths);
		data.Write(kills);
		data.Write(headshot);
		PhotonRPC.RPC("PhotonSetSelectPlayerInfo", message.sender, data);
	}

	[PunRPC]
	private void PhotonSetSelectPlayerInfo(PhotonMessage message)
	{
		int xp = message.ReadInt();
		int deaths = message.ReadInt();
		int kills = message.ReadInt();
		int headshot = message.ReadInt();
		if (!this.PlayerInfoList.ContainsKey(message.sender.UserId))
		{
			UIPlayerStatistics.PlayerInfoClass playerInfoClass = new UIPlayerStatistics.PlayerInfoClass();
			playerInfoClass.xp = xp;
			playerInfoClass.deaths = deaths;
			playerInfoClass.kills = kills;
			playerInfoClass.headshot = headshot;
			this.PlayerInfoList.Add(message.sender.UserId, playerInfoClass);
		}
		this.PhotonSetSelectPlayerInfo(xp, deaths, kills, headshot);
	}

	private void PhotonSetSelectPlayerInfo(int xp, int deaths, int kills, int headshot)
	{
		this.PlayerInfoXP.text = StringCache.Get(xp);
		this.PlayerInfoKD.text = ((float)kills / (float)deaths).ToString("f2");
		this.PlayerInfoHK.text = ((float)headshot / (float)kills).ToString("f2");
	}

	public class PlayerInfoClass
	{

		public int xp;

		public int deaths;

		public int kills;

		public int headshot;
	}
}
