using System;
using System.Collections.Generic;
using BSCM;
using ExitGames.Client.Photon;
using FreeJSON;
using UnityEngine;

public class mPhotonSettings : MonoBehaviour
{
    private static string selectMap;

    private static bool newRegion;

    private static RoomInfo queueRoom;

    public static Action connectedCallback;

    #if UNITY_EDITOR
    public static bool isFake;
    #endif

    public static string region
	{
		get
		{
			return nPlayerPrefs.GetString("Region", "Best");
		}
		set
		{
			nPlayerPrefs.SetString("Region", value);
		}
	}

	private void Start()
	{
		PhotonNetwork.automaticallySyncScene = true;
		PhotonNetwork.offlineMode = false;
		PhotonNetwork.UseRpcMonoBehaviourCache = true;
	}

	private void OnEnable()
	{
		PhotonNetwork.onConnectedToPhoton = (PhotonNetwork.VoidDelegate)Delegate.Combine(PhotonNetwork.onConnectedToPhoton, new PhotonNetwork.VoidDelegate(this.OnConnectedToPhoton));
		PhotonNetwork.onDisconnectedFromPhoton = (PhotonNetwork.VoidDelegate)Delegate.Combine(PhotonNetwork.onDisconnectedFromPhoton, new PhotonNetwork.VoidDelegate(this.OnDisconnectedFromPhoton));
		PhotonNetwork.onConnectionFail = (PhotonNetwork.DisconnectCauseDelegate)Delegate.Combine(PhotonNetwork.onConnectionFail, new PhotonNetwork.DisconnectCauseDelegate(this.OnConnectionFail));
		PhotonNetwork.onJoinedRoom = (PhotonNetwork.VoidDelegate)Delegate.Combine(PhotonNetwork.onJoinedRoom, new PhotonNetwork.VoidDelegate(this.OnJoinedRoom));
		PhotonNetwork.onPhotonJoinRoomFailed = (PhotonNetwork.ResponseDelegate)Delegate.Combine(PhotonNetwork.onPhotonJoinRoomFailed, new PhotonNetwork.ResponseDelegate(this.OnPhotonJoinRoomFailed));
		PhotonNetwork.onPhotonCreateRoomFailed = (PhotonNetwork.ResponseDelegate)Delegate.Combine(PhotonNetwork.onPhotonCreateRoomFailed, new PhotonNetwork.ResponseDelegate(this.OnPhotonCreateRoomFailed));
		PhotonNetwork.onCustomAuthenticationFailed = (PhotonNetwork.StringDelegate)Delegate.Combine(PhotonNetwork.onCustomAuthenticationFailed, new PhotonNetwork.StringDelegate(this.OnCustomAuthenticationFailed));
		PhotonNetwork.onCustomAuthenticationResponse = (PhotonNetwork.ObjectsDelegate)Delegate.Combine(PhotonNetwork.onCustomAuthenticationResponse, new PhotonNetwork.ObjectsDelegate(this.OnCustomAuthenticationResponse));
	}

	private void OnDisable()
	{
		PhotonNetwork.onConnectedToPhoton = (PhotonNetwork.VoidDelegate)Delegate.Remove(PhotonNetwork.onConnectedToPhoton, new PhotonNetwork.VoidDelegate(this.OnConnectedToPhoton));
		PhotonNetwork.onDisconnectedFromPhoton = (PhotonNetwork.VoidDelegate)Delegate.Remove(PhotonNetwork.onDisconnectedFromPhoton, new PhotonNetwork.VoidDelegate(this.OnDisconnectedFromPhoton));
		PhotonNetwork.onConnectionFail = (PhotonNetwork.DisconnectCauseDelegate)Delegate.Remove(PhotonNetwork.onConnectionFail, new PhotonNetwork.DisconnectCauseDelegate(this.OnConnectionFail));
		PhotonNetwork.onJoinedRoom = (PhotonNetwork.VoidDelegate)Delegate.Remove(PhotonNetwork.onJoinedRoom, new PhotonNetwork.VoidDelegate(this.OnJoinedRoom));
		PhotonNetwork.onPhotonJoinRoomFailed = (PhotonNetwork.ResponseDelegate)Delegate.Remove(PhotonNetwork.onPhotonJoinRoomFailed, new PhotonNetwork.ResponseDelegate(this.OnPhotonJoinRoomFailed));
		PhotonNetwork.onPhotonCreateRoomFailed = (PhotonNetwork.ResponseDelegate)Delegate.Remove(PhotonNetwork.onPhotonCreateRoomFailed, new PhotonNetwork.ResponseDelegate(this.OnPhotonCreateRoomFailed));
		PhotonNetwork.onCustomAuthenticationFailed = (PhotonNetwork.StringDelegate)Delegate.Remove(PhotonNetwork.onCustomAuthenticationFailed, new PhotonNetwork.StringDelegate(this.OnCustomAuthenticationFailed));
		PhotonNetwork.onCustomAuthenticationResponse = (PhotonNetwork.ObjectsDelegate)Delegate.Remove(PhotonNetwork.onCustomAuthenticationResponse, new PhotonNetwork.ObjectsDelegate(this.OnCustomAuthenticationResponse));
	}

    #if UNITY_EDITOR
    private static string name;
    #endif

    public static void Connect()
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account", true));
			return;
		}
		if (PhotonNetwork.connected && !mPhotonSettings.newRegion)
		{
			mPhotonSettings.connectedCallback();
			return;
		}
		if (PhotonNetwork.connected)
		{
			PhotonNetwork.Disconnect();
		}
		mPopUp.ShowText(Localization.Get("By DRRIXS", true) + "...");
		string bundleVersion = VersionManager.bundleVersion;
		string appid = GameSettings.instance.PhotonID;
		PhotonNetwork.AuthValues = new AuthenticationValues();
		PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.Custom;
        #if UNITY_EDITOR
        if (isFake)
        {
            System.Text.StringBuilder str_build = new System.Text.StringBuilder();
            System.Random random = new System.Random();
            char letter;
            for (int i = 0; i < 5; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }
            name = str_build.ToString();
            PhotonNetwork.AuthValues.UserId = name[0].ToString().ToUpper() + name.ToLower().Substring(1);
        }
        else
        {
            PhotonNetwork.AuthValues.UserId = AccountManager.instance.Data.AccountName;
        }
        #else
        PhotonNetwork.AuthValues.UserId = AccountManager.instance.Data.AccountName;
        #endif
        PhotonNetwork.AuthValues.AddAuthParameter("f", "7");
		JsonObject jsonObject = new JsonObject();
		jsonObject.Add("v", bundleVersion);
		jsonObject.Add("m", NetworkingPeer.AppToken);
		jsonObject.Add("e", AccountManager.AccountID);
		jsonObject.Add("s", AccountManager.instance.Data.Session);
		PhotonNetwork.AuthValues.AddAuthParameter("v", Utils.XOR(jsonObject.ToString(), true));
		if (nPlayerPrefs.HasKey("Region"))
		{
			PhotonNetwork.ConnectToRegion(Region.Parse(mPhotonSettings.region), bundleVersion, appid);
		}
		else
		{
			PhotonNetwork.ConnectToBestCloudServer(bundleVersion, appid);
		}
		mPhotonSettings.newRegion = false;
	}

	public void SelectRegion(string region)
	{
		if (mPhotonSettings.region == region)
		{
			return;
		}
		mPhotonSettings.region = region;
		mPhotonSettings.newRegion = true;
		mVersionManager.UpdateRegion();
		if (PhotonNetwork.connected)
		{
			PhotonNetwork.Disconnect();
		}
	}

	private void OnConnectedToPhoton()
	{
		if (!PhotonNetwork.connected)
		{
			return;
		}
		mVersionManager.UpdateRegion();
        #if UNITY_EDITOR
        if (isFake)
        {
            PhotonNetwork.player.SetLevel(UnityEngine.Random.RandomRange(1, 10));
            PhotonNetwork.player.SetClan("");
        }
        else
        {
            PhotonNetwork.player.SetLevel(AccountManager.GetLevel());
            PhotonNetwork.player.SetClan(AccountManager.GetClan());
            PhotonNetwork.player.SetAvatarUrl(AccountManager.instance.Data.AvatarUrl);
        }
        #else
        PhotonNetwork.player.SetLevel(AccountManager.GetLevel());
        PhotonNetwork.player.SetClan(AccountManager.GetClan());
        PhotonNetwork.player.SetAvatarUrl(AccountManager.instance.Data.AvatarUrl);
        #endif
		TimerManager.In("PhotonConnected", 1.5f, delegate()
		{
			mPhotonSettings.connectedCallback();
		});
	}

	private void OnDisconnectedFromPhoton()
	{
        mPopUp.HideAll("Menu");
	}

	private void OnFailedToConnectToPhoton(DisconnectCause cause)
	{
		TimerManager.Cancel("PhotonConnected");
		UIToast.Show("Failed: " + cause.ToString());
	}

	private void OnConnectionFail(DisconnectCause cause)
	{
		TimerManager.Cancel("PhotonConnected");
		UIToast.Show("Fail: " + cause.ToString());
	}

	private void OnCustomAuthenticationFailed(string error)
	{
        
    }

	private void OnCustomAuthenticationResponse(object[] obj)
	{
		TimerManager.In(0.1f, delegate()
		{
			MonoBehaviour.print(obj.Length);
			object[] obj2 = obj;
			for (int i = 0; i < obj2.Length; i++)
			{
				MonoBehaviour.print(((Dictionary<string, object>)obj2[i])["Test"]);
			}
		});
	}

	private void OnJoinedRoom()
	{
		PlayerRoundManager.Clear();
		PlayerRoundManager.SetMode(PhotonNetwork.room.GetGameMode());
		if (PhotonNetwork.offlineMode)
		{
			LevelManager.LoadLevel(mPhotonSettings.selectMap);
			return;
		}
		PhotonNetwork.isMessageQueueRunning = false;
		PhotonNetwork.LoadLevel(mPhotonSettings.selectMap);
	}

	private void OnPhotonJoinRoomFailed(short code, string message)
	{
		mJoinServer.onBack();
		if (message == "Game full")
		{
			UIToast.Show(Localization.Get("The server is full", true));
			return;
		}
		UIToast.Show(string.Concat(new object[]
		{
			"Error Code: ",
			code,
			" Message: ",
			message
		}));
	}

	private void OnPhotonCreateRoomFailed(short code, string message)
	{
		mPopUp.HideAll("CreateServer");
		if (code == 32766)
		{
			UIToast.Show(Localization.Get("Server with this name already exists", true));
			return;
		}
		UIToast.Show(string.Concat(new object[]
		{
			"Error Code: ",
			code,
			" Message: ",
			message
		}));
	}

	public static void CreateOfficialServer(string name, string map, GameMode mode)
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account", true));
			return;
		}
		mPopUp.ShowText(Localization.Get("Connecting", true) + "...");
		PhotonNetwork.player.SetPlayerID(AccountManager.instance.Data.ID);
		PhotonNetwork.player.ClearProperties();
		mPhotonSettings.selectMap = map;
		Hashtable hashtable = PhotonNetwork.room.CreateRoomHashtable("off", mode, true);
		hashtable[PhotonCustomValue.minLevelKey] = (byte)AccountManager.GetLevel();
		PhotonNetwork.CreateRoom(name, new RoomOptions
		{
			MaxPlayers = 12,
			IsOpen = true,
			IsVisible = true,
			PublishUserId = true,
			CustomRoomProperties = hashtable,
			CustomRoomPropertiesForLobby = new string[]
			{
				PhotonCustomValue.sceneNameKey,
				PhotonCustomValue.passwordKey,
				PhotonCustomValue.gameModeKey,
				PhotonCustomValue.officialServerKey,
				PhotonCustomValue.minLevelKey
			}
		}, null);
	}

	public static void CreateServer(string name, string map, GameMode mode, string password, int maxPlayers, bool custom)
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account", true));
			return;
		}
		mPopUp.ShowText(Localization.Get("Creating Server", true) + "...");
		PhotonNetwork.player.SetPlayerID(AccountManager.instance.Data.ID);
		PhotonNetwork.player.ClearProperties();
        #if !UNITY_EDITOR
        if (map == "50Traps")
		{
			maxPlayers = Mathf.Clamp(maxPlayers, 4, 32);
		}
		else
		{
			maxPlayers = Mathf.Clamp(maxPlayers, 4, 12);
		}
        #endif
		mPhotonSettings.selectMap = map;
		Hashtable hashtable = PhotonNetwork.room.CreateRoomHashtable(password, mode, false);
		if (mode == GameMode.Only)
		{
			hashtable[PhotonCustomValue.onlyWeaponKey] = (byte)mServerSettings.GetOnlyWeapon();
		}
		if (custom)
		{
			hashtable[PhotonCustomValue.customMapHash] = Manager.hash;
			hashtable[PhotonCustomValue.customMapUrl] = Manager.bundleUrl;
			hashtable[PhotonCustomValue.customMapModes] = Manager.modes;
		}
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = (byte)maxPlayers;
		roomOptions.IsOpen = true;
		roomOptions.IsVisible = true;
		roomOptions.PublishUserId = true;
		roomOptions.CustomRoomProperties = hashtable;
		if (custom)
		{
			roomOptions.CustomRoomPropertiesForLobby = new string[]
			{
				PhotonCustomValue.sceneNameKey,
				PhotonCustomValue.passwordKey,
				PhotonCustomValue.gameModeKey,
				PhotonCustomValue.customMapHash,
				PhotonCustomValue.customMapUrl,
				PhotonCustomValue.customMapModes
			};
		}
		else if (mode == GameMode.Only)
		{
			roomOptions.CustomRoomPropertiesForLobby = new string[]
			{
				PhotonCustomValue.sceneNameKey,
				PhotonCustomValue.passwordKey,
				PhotonCustomValue.gameModeKey,
				PhotonCustomValue.onlyWeaponKey
			};
		}
		else
		{
			roomOptions.CustomRoomPropertiesForLobby = new string[]
			{
				PhotonCustomValue.sceneNameKey,
				PhotonCustomValue.passwordKey,
				PhotonCustomValue.gameModeKey
			};
		}
		LevelManager.customScene = custom;
		PhotonNetwork.CreateRoom(name, roomOptions, null);
	}

	public static void QueueServer(RoomInfo room)
	{
		mPhotonSettings.queueRoom = room;
		mPopUp.ShowPopup(Localization.Get("Please wait", true) + "...", Localization.Get("Queue", true), Localization.Get("Exit", true), delegate()
		{
			mPhotonSettings.queueRoom = null;
			TimerManager.Cancel("QueueTimer");
			mJoinServer.onBack();
		});
		TimerManager.In("QueueTimer", 1f, -1, 1f, delegate()
		{
			if (mPhotonSettings.queueRoom == null)
			{
				TimerManager.Cancel("QueueTimer");
				return;
			}
			RoomInfo[] roomList = PhotonNetwork.GetRoomList();
			int i = 0;
			while (i < roomList.Length)
			{
				if (roomList[i].Name == mPhotonSettings.queueRoom.Name)
				{
					if (roomList[i].PlayerCount != (int)roomList[i].MaxPlayers)
					{
						TimerManager.Cancel("QueueTimer");
						mPhotonSettings.JoinServer(mPhotonSettings.queueRoom);
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		});
	}

	public void CreateServerOffline(string map)
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account", true));
			return;
		}
		if (PhotonNetwork.connected)
		{
			PhotonNetwork.Disconnect();
		}
		mPopUp.ShowText(Localization.Get("Loading", true) + "...");
		TimerManager.In(0.2f, delegate()
		{
			mPopUp.ShowText(Localization.Get("Loading", true) + "...");
			mPhotonSettings.selectMap = map;
			PhotonNetwork.offlineMode = true;
			LevelManager.customScene = false;
			PhotonNetwork.CreateRoom(map);
		});
	}

	public static void CreateCustomServerOffline(string map, GameMode mode)
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account", true));
			return;
		}
		if (PhotonNetwork.connected)
		{
			PhotonNetwork.Disconnect();
		}
		mPopUp.ShowText(Localization.Get("Loading", true) + "...");
		Hashtable customRoomProperties = PhotonNetwork.room.CreateRoomHashtable(string.Empty, mode, false);
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = 1;
		roomOptions.IsOpen = false;
		roomOptions.IsVisible = false;
		roomOptions.CustomRoomProperties = customRoomProperties;
		mPhotonSettings.selectMap = map;
		PhotonNetwork.offlineMode = true;
		LevelManager.customScene = true;
		PhotonNetwork.CreateRoom(map, roomOptions, null);
	}

	public static void JoinServer(RoomInfo room)
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account", true));
			return;
		}
		if (mPhotonSettings.queueRoom == null)
		{
			mPopUp.ShowText(Localization.Get("Connecting", true) + "...");
		}
		PhotonNetwork.player.SetPlayerID(AccountManager.instance.Data.ID);
		PhotonNetwork.player.ClearProperties();
		mPhotonSettings.selectMap = room.GetSceneName();
		PhotonNetwork.JoinRoom(room.Name);
	}
}
