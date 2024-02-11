using System;
using System.Collections.Generic;
using System.Diagnostics;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020002D4 RID: 724
public static class PhotonNetwork
{
	// Token: 0x06001B2B RID: 6955 RVA: 0x000B10F4 File Offset: 0x000AF2F4
	static PhotonNetwork()
	{
		if (PhotonNetwork.PhotonServerSettings != null)
		{
			Application.runInBackground = PhotonNetwork.PhotonServerSettings.RunInBackground;
		}
		GameObject gameObject = new GameObject();
		PhotonNetwork.photonMono = gameObject.AddComponent<PhotonHandler>();
		gameObject.name = "PhotonMono";
		gameObject.hideFlags = HideFlags.HideInHierarchy;
		ConnectionProtocol protocol = PhotonNetwork.PhotonServerSettings.Protocol;
		PhotonNetwork.networkingPeer = new NetworkingPeer(string.Empty, protocol);
		PhotonNetwork.networkingPeer.QuickResendAttempts = 2;
		PhotonNetwork.networkingPeer.SentCountAllowance = 7;
		PhotonNetwork.startupStopwatch = new Stopwatch();
		PhotonNetwork.startupStopwatch.Start();
		PhotonNetwork.networkingPeer.LocalMsTimestampDelegate = (() => (int)PhotonNetwork.startupStopwatch.ElapsedMilliseconds);
		CustomTypes.Register();
        //logLevel = PhotonLogLevel.Full;
	}

	// Token: 0x14000019 RID: 25
	// (add) Token: 0x06001B2C RID: 6956 RVA: 0x00013982 File Offset: 0x00011B82
	// (remove) Token: 0x06001B2D RID: 6957 RVA: 0x00013999 File Offset: 0x00011B99
	public static event PhotonNetwork.EventCallback OnEventCall;

	// Token: 0x170003A9 RID: 937
	// (get) Token: 0x06001B2E RID: 6958 RVA: 0x000139B0 File Offset: 0x00011BB0
	// (set) Token: 0x06001B2F RID: 6959 RVA: 0x000139B7 File Offset: 0x00011BB7
	public static string gameVersion { get; set; }

	// Token: 0x170003AA RID: 938
	// (get) Token: 0x06001B30 RID: 6960 RVA: 0x000139BF File Offset: 0x00011BBF
	public static string ServerAddress
	{
		get
		{
			return (PhotonNetwork.networkingPeer == null) ? "<not connected>" : PhotonNetwork.networkingPeer.ServerAddress;
		}
	}

	// Token: 0x170003AB RID: 939
	// (get) Token: 0x06001B31 RID: 6961 RVA: 0x000139DF File Offset: 0x00011BDF
	public static CloudRegionCode CloudRegion
	{
		get
		{
			return (PhotonNetwork.networkingPeer == null || !PhotonNetwork.connected || PhotonNetwork.Server == ServerConnection.NameServer) ? CloudRegionCode.none : PhotonNetwork.networkingPeer.CloudRegion;
		}
	}

	// Token: 0x170003AC RID: 940
	// (get) Token: 0x06001B32 RID: 6962 RVA: 0x000B127C File Offset: 0x000AF47C
	public static bool connected
	{
		get
		{
			return PhotonNetwork.offlineMode || (PhotonNetwork.networkingPeer != null && (!PhotonNetwork.networkingPeer.IsInitialConnect && PhotonNetwork.networkingPeer.State != ClientState.PeerCreated && PhotonNetwork.networkingPeer.State != ClientState.Disconnected && PhotonNetwork.networkingPeer.State != ClientState.Disconnecting) && PhotonNetwork.networkingPeer.State != ClientState.ConnectingToNameServer);
		}
	}

	// Token: 0x170003AD RID: 941
	// (get) Token: 0x06001B33 RID: 6963 RVA: 0x00013A10 File Offset: 0x00011C10
	public static bool connecting
	{
		get
		{
			return PhotonNetwork.networkingPeer.IsInitialConnect && !PhotonNetwork.offlineMode;
		}
	}

	// Token: 0x170003AE RID: 942
	// (get) Token: 0x06001B34 RID: 6964 RVA: 0x000B12F8 File Offset: 0x000AF4F8
	public static bool connectedAndReady
	{
		get
		{
			if (!PhotonNetwork.connected)
			{
				return false;
			}
			if (PhotonNetwork.offlineMode)
			{
				return true;
			}
			ClientState connectionStateDetailed = PhotonNetwork.connectionStateDetailed;
			switch (connectionStateDetailed)
			{
			case ClientState.ConnectingToMasterserver:
			case ClientState.Disconnecting:
			case ClientState.Disconnected:
			case ClientState.ConnectingToNameServer:
			case ClientState.Authenticating:
				break;
			default:
				switch (connectionStateDetailed)
				{
				case ClientState.ConnectingToGameserver:
				case ClientState.Joining:
					break;
				default:
					if (connectionStateDetailed != ClientState.PeerCreated)
					{
						return true;
					}
					break;
				}
				break;
			}
			return false;
		}
	}

	// Token: 0x170003AF RID: 943
	// (get) Token: 0x06001B35 RID: 6965 RVA: 0x000B1374 File Offset: 0x000AF574
	public static ConnectionState connectionState
	{
		get
		{
			if (PhotonNetwork.offlineMode)
			{
				return ConnectionState.Connected;
			}
			if (PhotonNetwork.networkingPeer == null)
			{
				return ConnectionState.Disconnected;
			}
			PeerStateValue peerState = PhotonNetwork.networkingPeer.PeerState;
			switch (peerState)
			{
			case PeerStateValue.Disconnected:
				return ConnectionState.Disconnected;
			case PeerStateValue.Connecting:
				return ConnectionState.Connecting;
			default:
				if (peerState != PeerStateValue.InitializingApplication)
				{
					return ConnectionState.Disconnected;
				}
				return ConnectionState.InitializingApplication;
			case PeerStateValue.Connected:
				return ConnectionState.Connected;
			case PeerStateValue.Disconnecting:
				return ConnectionState.Disconnecting;
			}
		}
	}

	// Token: 0x170003B0 RID: 944
	// (get) Token: 0x06001B36 RID: 6966 RVA: 0x00013A2C File Offset: 0x00011C2C
	public static ClientState connectionStateDetailed
	{
		get
		{
			if (PhotonNetwork.offlineMode)
			{
				return (PhotonNetwork.offlineModeRoom == null) ? ClientState.ConnectedToMaster : ClientState.Joined;
			}
			if (PhotonNetwork.networkingPeer == null)
			{
				return ClientState.Disconnected;
			}
			return PhotonNetwork.networkingPeer.State;
		}
	}

	// Token: 0x170003B1 RID: 945
	// (get) Token: 0x06001B37 RID: 6967 RVA: 0x00013A63 File Offset: 0x00011C63
	public static ServerConnection Server
	{
		get
		{
			return (PhotonNetwork.networkingPeer == null) ? ServerConnection.NameServer : PhotonNetwork.networkingPeer.Server;
		}
	}

	// Token: 0x170003B2 RID: 946
	// (get) Token: 0x06001B38 RID: 6968 RVA: 0x00013A7F File Offset: 0x00011C7F
	// (set) Token: 0x06001B39 RID: 6969 RVA: 0x00013A9B File Offset: 0x00011C9B
	public static AuthenticationValues AuthValues
	{
		get
		{
			return (PhotonNetwork.networkingPeer == null) ? null : PhotonNetwork.networkingPeer.AuthValues;
		}
		set
		{
			if (PhotonNetwork.networkingPeer != null)
			{
				PhotonNetwork.networkingPeer.AuthValues = value;
			}
		}
	}

	// Token: 0x170003B3 RID: 947
	// (get) Token: 0x06001B3A RID: 6970 RVA: 0x00013AB2 File Offset: 0x00011CB2
	public static Room room
	{
		get
		{
			if (PhotonNetwork.isOfflineMode)
			{
				return PhotonNetwork.offlineModeRoom;
			}
			return PhotonNetwork.networkingPeer.CurrentRoom;
		}
	}

	// Token: 0x170003B4 RID: 948
	// (get) Token: 0x06001B3B RID: 6971 RVA: 0x00013ACE File Offset: 0x00011CCE
	public static PhotonPlayer player
	{
		get
		{
			if (PhotonNetwork.networkingPeer == null)
			{
				return null;
			}
			return PhotonNetwork.networkingPeer.LocalPlayer;
		}
	}

	// Token: 0x170003B5 RID: 949
	// (get) Token: 0x06001B3C RID: 6972 RVA: 0x00013AE6 File Offset: 0x00011CE6
	public static PhotonPlayer masterClient
	{
		get
		{
			if (PhotonNetwork.offlineMode)
			{
				return PhotonNetwork.player;
			}
			if (PhotonNetwork.networkingPeer == null)
			{
				return null;
			}
			return PhotonNetwork.networkingPeer.GetPlayerWithId(PhotonNetwork.networkingPeer.mMasterClientId);
		}
	}

	// Token: 0x170003B6 RID: 950
	// (get) Token: 0x06001B3D RID: 6973 RVA: 0x00013B18 File Offset: 0x00011D18
	// (set) Token: 0x06001B3E RID: 6974 RVA: 0x00013B24 File Offset: 0x00011D24
	public static string playerName
	{
		get
		{
			return PhotonNetwork.networkingPeer.PlayerName;
		}
		set
		{
			PhotonNetwork.networkingPeer.PlayerName = value;
		}
	}

	// Token: 0x170003B7 RID: 951
	// (get) Token: 0x06001B3F RID: 6975 RVA: 0x00013B31 File Offset: 0x00011D31
	public static PhotonPlayer[] playerList
	{
		get
		{
			if (PhotonNetwork.networkingPeer == null)
			{
				return new PhotonPlayer[0];
			}
			return PhotonNetwork.networkingPeer.mPlayerListCopy;
		}
	}

	// Token: 0x170003B8 RID: 952
	// (get) Token: 0x06001B40 RID: 6976 RVA: 0x00013B4E File Offset: 0x00011D4E
	public static PhotonPlayer[] otherPlayers
	{
		get
		{
			if (PhotonNetwork.networkingPeer == null)
			{
				return new PhotonPlayer[0];
			}
			return PhotonNetwork.networkingPeer.mOtherPlayerListCopy;
		}
	}

	// Token: 0x170003B9 RID: 953
	// (get) Token: 0x06001B41 RID: 6977 RVA: 0x00013B6B File Offset: 0x00011D6B
	// (set) Token: 0x06001B42 RID: 6978 RVA: 0x00013B72 File Offset: 0x00011D72
	public static List<FriendInfo> Friends { get; internal set; }

	// Token: 0x170003BA RID: 954
	// (get) Token: 0x06001B43 RID: 6979 RVA: 0x00013B7A File Offset: 0x00011D7A
	public static int FriendsListAge
	{
		get
		{
			return (PhotonNetwork.networkingPeer == null) ? 0 : PhotonNetwork.networkingPeer.FriendListAge;
		}
	}

	// Token: 0x170003BB RID: 955
	// (get) Token: 0x06001B44 RID: 6980 RVA: 0x00013B96 File Offset: 0x00011D96
	// (set) Token: 0x06001B45 RID: 6981 RVA: 0x00013BA2 File Offset: 0x00011DA2
	public static IPunPrefabPool PrefabPool
	{
		get
		{
			return PhotonNetwork.networkingPeer.ObjectPool;
		}
		set
		{
			PhotonNetwork.networkingPeer.ObjectPool = value;
		}
	}

	// Token: 0x170003BC RID: 956
	// (get) Token: 0x06001B46 RID: 6982 RVA: 0x00013BAF File Offset: 0x00011DAF
	// (set) Token: 0x06001B47 RID: 6983 RVA: 0x000B13D8 File Offset: 0x000AF5D8
	public static bool offlineMode
	{
		get
		{
			return PhotonNetwork.isOfflineMode;
		}
		set
		{
			if (value == PhotonNetwork.isOfflineMode)
			{
				return;
			}
			if (value && PhotonNetwork.connected)
			{
				UnityEngine.Debug.LogError("Can't start OFFLINE mode while connected!");
				return;
			}
			if (PhotonNetwork.networkingPeer.PeerState != PeerStateValue.Disconnected)
			{
				PhotonNetwork.networkingPeer.Disconnect();
			}
			PhotonNetwork.isOfflineMode = value;
			if (PhotonNetwork.isOfflineMode)
			{
				PhotonNetwork.networkingPeer.ChangeLocalID(-1);
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnConnectedToMaster, new object[0]);
			}
			else
			{
				PhotonNetwork.offlineModeRoom = null;
				PhotonNetwork.networkingPeer.ChangeLocalID(-1);
			}
		}
	}

	// Token: 0x170003BD RID: 957
	// (get) Token: 0x06001B48 RID: 6984 RVA: 0x00013BB6 File Offset: 0x00011DB6
	// (set) Token: 0x06001B49 RID: 6985 RVA: 0x00013BBD File Offset: 0x00011DBD
	public static bool automaticallySyncScene
	{
		get
		{
			return PhotonNetwork._mAutomaticallySyncScene;
		}
		set
		{
			PhotonNetwork._mAutomaticallySyncScene = value;
			if (PhotonNetwork._mAutomaticallySyncScene && PhotonNetwork.room != null)
			{
				PhotonNetwork.networkingPeer.LoadLevelIfSynced();
			}
		}
	}

	// Token: 0x170003BE RID: 958
	// (get) Token: 0x06001B4A RID: 6986 RVA: 0x00013BE3 File Offset: 0x00011DE3
	// (set) Token: 0x06001B4B RID: 6987 RVA: 0x00013BEA File Offset: 0x00011DEA
	public static bool autoCleanUpPlayerObjects
	{
		get
		{
			return PhotonNetwork.m_autoCleanUpPlayerObjects;
		}
		set
		{
			if (PhotonNetwork.room != null)
			{
				UnityEngine.Debug.LogError("Setting autoCleanUpPlayerObjects while in a room is not supported.");
			}
			else
			{
				PhotonNetwork.m_autoCleanUpPlayerObjects = value;
			}
		}
	}

	// Token: 0x170003BF RID: 959
	// (get) Token: 0x06001B4C RID: 6988 RVA: 0x00013C0B File Offset: 0x00011E0B
	// (set) Token: 0x06001B4D RID: 6989 RVA: 0x00013C17 File Offset: 0x00011E17
	public static bool autoJoinLobby
	{
		get
		{
			return PhotonNetwork.PhotonServerSettings.JoinLobby;
		}
		set
		{
			PhotonNetwork.PhotonServerSettings.JoinLobby = value;
		}
	}

	// Token: 0x170003C0 RID: 960
	// (get) Token: 0x06001B4E RID: 6990 RVA: 0x00013C24 File Offset: 0x00011E24
	// (set) Token: 0x06001B4F RID: 6991 RVA: 0x00013C30 File Offset: 0x00011E30
	public static bool EnableLobbyStatistics
	{
		get
		{
			return PhotonNetwork.PhotonServerSettings.EnableLobbyStatistics;
		}
		set
		{
			PhotonNetwork.PhotonServerSettings.EnableLobbyStatistics = value;
		}
	}

	// Token: 0x170003C1 RID: 961
	// (get) Token: 0x06001B50 RID: 6992 RVA: 0x00013C3D File Offset: 0x00011E3D
	// (set) Token: 0x06001B51 RID: 6993 RVA: 0x00013C49 File Offset: 0x00011E49
	public static List<TypedLobbyInfo> LobbyStatistics
	{
		get
		{
			return PhotonNetwork.networkingPeer.LobbyStatistics;
		}
		private set
		{
			PhotonNetwork.networkingPeer.LobbyStatistics = value;
		}
	}

	// Token: 0x170003C2 RID: 962
	// (get) Token: 0x06001B52 RID: 6994 RVA: 0x00013C56 File Offset: 0x00011E56
	public static bool insideLobby
	{
		get
		{
			return PhotonNetwork.networkingPeer.insideLobby;
		}
	}

	// Token: 0x170003C3 RID: 963
	// (get) Token: 0x06001B53 RID: 6995 RVA: 0x00013C62 File Offset: 0x00011E62
	// (set) Token: 0x06001B54 RID: 6996 RVA: 0x00013C6E File Offset: 0x00011E6E
	public static TypedLobby lobby
	{
		get
		{
			return PhotonNetwork.networkingPeer.lobby;
		}
		set
		{
			PhotonNetwork.networkingPeer.lobby = value;
		}
	}

	// Token: 0x170003C4 RID: 964
	// (get) Token: 0x06001B55 RID: 6997 RVA: 0x00013C7B File Offset: 0x00011E7B
	// (set) Token: 0x06001B56 RID: 6998 RVA: 0x00013C88 File Offset: 0x00011E88
	public static int sendRate
	{
		get
		{
			return 1000 / PhotonNetwork.sendInterval;
		}
		set
		{
			PhotonNetwork.sendInterval = 1000 / value;
			if (PhotonNetwork.photonMono != null)
			{
				PhotonNetwork.photonMono.updateInterval = PhotonNetwork.sendInterval;
			}
			if (value < PhotonNetwork.sendRateOnSerialize)
			{
				PhotonNetwork.sendRateOnSerialize = value;
			}
		}
	}

	// Token: 0x170003C5 RID: 965
	// (get) Token: 0x06001B57 RID: 6999 RVA: 0x00013CC6 File Offset: 0x00011EC6
	// (set) Token: 0x06001B58 RID: 7000 RVA: 0x000B1464 File Offset: 0x000AF664
	public static int sendRateOnSerialize
	{
		get
		{
			return 1000 / PhotonNetwork.sendIntervalOnSerialize;
		}
		set
		{
			if (value > PhotonNetwork.sendRate)
			{
				UnityEngine.Debug.LogError("Error: Can not set the OnSerialize rate higher than the overall SendRate.");
				value = PhotonNetwork.sendRate;
			}
			PhotonNetwork.sendIntervalOnSerialize = 1000 / value;
			if (PhotonNetwork.photonMono != null)
			{
				PhotonNetwork.photonMono.updateIntervalOnSerialize = PhotonNetwork.sendIntervalOnSerialize;
			}
		}
	}

	// Token: 0x170003C6 RID: 966
	// (get) Token: 0x06001B59 RID: 7001 RVA: 0x00013CD3 File Offset: 0x00011ED3
	// (set) Token: 0x06001B5A RID: 7002 RVA: 0x00013CDA File Offset: 0x00011EDA
	public static bool isMessageQueueRunning
	{
		get
		{
			return PhotonNetwork.m_isMessageQueueRunning;
		}
		set
		{
			if (value)
			{
				PhotonHandler.StartFallbackSendAckThread();
			}
			PhotonNetwork.networkingPeer.IsSendingOnlyAcks = !value;
			PhotonNetwork.m_isMessageQueueRunning = value;
		}
	}

	// Token: 0x170003C7 RID: 967
	// (get) Token: 0x06001B5B RID: 7003 RVA: 0x00013CFB File Offset: 0x00011EFB
	// (set) Token: 0x06001B5C RID: 7004 RVA: 0x00013D07 File Offset: 0x00011F07
	public static int unreliableCommandsLimit
	{
		get
		{
			return PhotonNetwork.networkingPeer.LimitOfUnreliableCommands;
		}
		set
		{
			PhotonNetwork.networkingPeer.LimitOfUnreliableCommands = value;
		}
	}

	// Token: 0x170003C8 RID: 968
	// (get) Token: 0x06001B5D RID: 7005 RVA: 0x000B14B8 File Offset: 0x000AF6B8
	public static double time
	{
		get
		{
			uint serverTimestamp = (uint)PhotonNetwork.ServerTimestamp;
			double num = serverTimestamp;
			return num / 1000.0;
		}
	}

	// Token: 0x170003C9 RID: 969
	// (get) Token: 0x06001B5E RID: 7006 RVA: 0x00013D14 File Offset: 0x00011F14
	public static int ServerTimestamp
	{
		get
		{
			if (PhotonNetwork.offlineMode)
			{
				return (int)PhotonNetwork.startupStopwatch.ElapsedMilliseconds;
			}
			return PhotonNetwork.networkingPeer.ServerTimeInMilliSeconds;
		}
	}

	// Token: 0x170003CA RID: 970
	// (get) Token: 0x06001B5F RID: 7007 RVA: 0x00013D36 File Offset: 0x00011F36
	public static bool isMasterClient
	{
		get
		{
			return PhotonNetwork.offlineMode || PhotonNetwork.networkingPeer.mMasterClientId == PhotonNetwork.player.ID;
		}
	}

	// Token: 0x170003CB RID: 971
	// (get) Token: 0x06001B60 RID: 7008 RVA: 0x00013D5A File Offset: 0x00011F5A
	public static bool inRoom
	{
		get
		{
			return PhotonNetwork.connectionStateDetailed == ClientState.Joined;
		}
	}

	// Token: 0x170003CC RID: 972
	// (get) Token: 0x06001B61 RID: 7009 RVA: 0x00013D65 File Offset: 0x00011F65
	public static bool isNonMasterClientInRoom
	{
		get
		{
			return !PhotonNetwork.isMasterClient && PhotonNetwork.room != null;
		}
	}

	// Token: 0x170003CD RID: 973
	// (get) Token: 0x06001B62 RID: 7010 RVA: 0x00013D7F File Offset: 0x00011F7F
	public static int countOfPlayersOnMaster
	{
		get
		{
			return PhotonNetwork.networkingPeer.PlayersOnMasterCount;
		}
	}

	// Token: 0x170003CE RID: 974
	// (get) Token: 0x06001B63 RID: 7011 RVA: 0x00013D8B File Offset: 0x00011F8B
	public static int countOfPlayersInRooms
	{
		get
		{
			return PhotonNetwork.networkingPeer.PlayersInRoomsCount;
		}
	}

	// Token: 0x170003CF RID: 975
	// (get) Token: 0x06001B64 RID: 7012 RVA: 0x00013D97 File Offset: 0x00011F97
	public static int countOfPlayers
	{
		get
		{
			return PhotonNetwork.networkingPeer.PlayersInRoomsCount + PhotonNetwork.networkingPeer.PlayersOnMasterCount;
		}
	}

	// Token: 0x170003D0 RID: 976
	// (get) Token: 0x06001B65 RID: 7013 RVA: 0x00013DAE File Offset: 0x00011FAE
	public static int countOfRooms
	{
		get
		{
			return PhotonNetwork.networkingPeer.RoomsCount;
		}
	}

	// Token: 0x170003D1 RID: 977
	// (get) Token: 0x06001B66 RID: 7014 RVA: 0x00013DBA File Offset: 0x00011FBA
	// (set) Token: 0x06001B67 RID: 7015 RVA: 0x00013DC6 File Offset: 0x00011FC6
	public static bool NetworkStatisticsEnabled
	{
		get
		{
			return PhotonNetwork.networkingPeer.TrafficStatsEnabled;
		}
		set
		{
			PhotonNetwork.networkingPeer.TrafficStatsEnabled = value;
		}
	}

	// Token: 0x170003D2 RID: 978
	// (get) Token: 0x06001B68 RID: 7016 RVA: 0x00013DD3 File Offset: 0x00011FD3
	public static int ResentReliableCommands
	{
		get
		{
			return PhotonNetwork.networkingPeer.ResentReliableCommands;
		}
	}

	// Token: 0x170003D3 RID: 979
	// (get) Token: 0x06001B69 RID: 7017 RVA: 0x00013DDF File Offset: 0x00011FDF
	// (set) Token: 0x06001B6A RID: 7018 RVA: 0x000B14DC File Offset: 0x000AF6DC
	public static bool CrcCheckEnabled
	{
		get
		{
			return PhotonNetwork.networkingPeer.CrcEnabled;
		}
		set
		{
			if (!PhotonNetwork.connected && !PhotonNetwork.connecting)
			{
				PhotonNetwork.networkingPeer.CrcEnabled = value;
			}
			else
			{
				UnityEngine.Debug.Log("Can't change CrcCheckEnabled while being connected. CrcCheckEnabled stays " + PhotonNetwork.networkingPeer.CrcEnabled);
			}
		}
	}

	// Token: 0x170003D4 RID: 980
	// (get) Token: 0x06001B6B RID: 7019 RVA: 0x00013DEB File Offset: 0x00011FEB
	public static int PacketLossByCrcCheck
	{
		get
		{
			return PhotonNetwork.networkingPeer.PacketLossByCrc;
		}
	}

	// Token: 0x170003D5 RID: 981
	// (get) Token: 0x06001B6C RID: 7020 RVA: 0x00013DF7 File Offset: 0x00011FF7
	// (set) Token: 0x06001B6D RID: 7021 RVA: 0x00013E03 File Offset: 0x00012003
	public static int MaxResendsBeforeDisconnect
	{
		get
		{
			return PhotonNetwork.networkingPeer.SentCountAllowance;
		}
		set
		{
			if (value < 3)
			{
				value = 3;
			}
			if (value > 10)
			{
				value = 10;
			}
			PhotonNetwork.networkingPeer.SentCountAllowance = value;
		}
	}

	// Token: 0x170003D6 RID: 982
	// (get) Token: 0x06001B6E RID: 7022 RVA: 0x00013E26 File Offset: 0x00012026
	// (set) Token: 0x06001B6F RID: 7023 RVA: 0x00013E32 File Offset: 0x00012032
	public static int QuickResends
	{
		get
		{
			return (int)PhotonNetwork.networkingPeer.QuickResendAttempts;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			if (value > 3)
			{
				value = 3;
			}
			PhotonNetwork.networkingPeer.QuickResendAttempts = (byte)value;
		}
	}

	// Token: 0x170003D7 RID: 983
	// (get) Token: 0x06001B70 RID: 7024 RVA: 0x00013E54 File Offset: 0x00012054
	// (set) Token: 0x06001B71 RID: 7025 RVA: 0x00013E5B File Offset: 0x0001205B
	public static bool UseAlternativeUdpPorts { get; set; }

	// Token: 0x06001B72 RID: 7026 RVA: 0x00013E63 File Offset: 0x00012063
	public static void SwitchToProtocol(ConnectionProtocol cp)
	{
		//PhotonNetwork.networkingPeer.TransportProtocol = cp;
	}

	// Token: 0x06001B73 RID: 7027 RVA: 0x000B152C File Offset: 0x000AF72C
	public static bool ConnectUsingSettings(string gameVersion)
	{
		if (PhotonNetwork.networkingPeer.PeerState != PeerStateValue.Disconnected)
		{
			UnityEngine.Debug.LogWarning("ConnectUsingSettings() failed. Can only connect while in state 'Disconnected'. Current state: " + PhotonNetwork.networkingPeer.PeerState);
			return false;
		}
		if (PhotonNetwork.PhotonServerSettings == null)
		{
			UnityEngine.Debug.LogError("Can't connect: Loading settings failed. ServerSettings asset must be in any 'Resources' folder as: PhotonServerSettings");
			return false;
		}
		if (PhotonNetwork.PhotonServerSettings.HostType == ServerSettings.HostingOption.NotSet)
		{
			UnityEngine.Debug.LogError("You did not select a Hosting Type in your PhotonServerSettings. Please set it up or don't use ConnectUsingSettings().");
			return false;
		}
		if (PhotonNetwork.logLevel == PhotonLogLevel.ErrorsOnly)
		{
			PhotonNetwork.logLevel = PhotonNetwork.PhotonServerSettings.PunLogging;
		}
		if (PhotonNetwork.networkingPeer.DebugOut == DebugLevel.ERROR)
		{
			PhotonNetwork.networkingPeer.DebugOut = PhotonNetwork.PhotonServerSettings.NetworkLogging;
		}
		PhotonNetwork.SwitchToProtocol(PhotonNetwork.PhotonServerSettings.Protocol);
		PhotonNetwork.networkingPeer.SetApp(PhotonNetwork.PhotonServerSettings.AppID, gameVersion);
		if (PhotonNetwork.PhotonServerSettings.HostType == ServerSettings.HostingOption.OfflineMode)
		{
			PhotonNetwork.offlineMode = true;
			return true;
		}
		if (PhotonNetwork.offlineMode)
		{
			UnityEngine.Debug.LogWarning("ConnectUsingSettings() disabled the offline mode. No longer offline.");
		}
		PhotonNetwork.offlineMode = false;
		PhotonNetwork.isMessageQueueRunning = true;
		PhotonNetwork.networkingPeer.IsInitialConnect = true;
		if (PhotonNetwork.PhotonServerSettings.HostType == ServerSettings.HostingOption.SelfHosted)
		{
			PhotonNetwork.networkingPeer.IsUsingNameServer = false;
			PhotonNetwork.networkingPeer.MasterServerAddress = ((PhotonNetwork.PhotonServerSettings.ServerPort != 0) ? (PhotonNetwork.PhotonServerSettings.ServerAddress + ":" + PhotonNetwork.PhotonServerSettings.ServerPort) : PhotonNetwork.PhotonServerSettings.ServerAddress);
			return PhotonNetwork.networkingPeer.Connect(PhotonNetwork.networkingPeer.MasterServerAddress, ServerConnection.MasterServer);
		}
		if (PhotonNetwork.PhotonServerSettings.HostType == ServerSettings.HostingOption.BestRegion)
		{
			return PhotonNetwork.ConnectToBestCloudServer(gameVersion);
		}
		return PhotonNetwork.networkingPeer.ConnectToRegionMaster(PhotonNetwork.PhotonServerSettings.PreferredRegion);
	}

	// Token: 0x06001B74 RID: 7028 RVA: 0x000B16EC File Offset: 0x000AF8EC
	public static bool ConnectToMaster(string masterServerAddress, int port, string appID, string gameVersion)
	{
		if (PhotonNetwork.networkingPeer.PeerState != PeerStateValue.Disconnected)
		{
			UnityEngine.Debug.LogWarning("ConnectToMaster() failed. Can only connect while in state 'Disconnected'. Current state: " + PhotonNetwork.networkingPeer.PeerState);
			return false;
		}
		if (PhotonNetwork.offlineMode)
		{
			PhotonNetwork.offlineMode = false;
			UnityEngine.Debug.LogWarning("ConnectToMaster() disabled the offline mode. No longer offline.");
		}
		if (!PhotonNetwork.isMessageQueueRunning)
		{
			PhotonNetwork.isMessageQueueRunning = true;
			UnityEngine.Debug.LogWarning("ConnectToMaster() enabled isMessageQueueRunning. Needs to be able to dispatch incoming messages.");
		}
		PhotonNetwork.networkingPeer.SetApp(appID, gameVersion);
		PhotonNetwork.networkingPeer.IsUsingNameServer = false;
		PhotonNetwork.networkingPeer.IsInitialConnect = true;
		PhotonNetwork.networkingPeer.MasterServerAddress = ((port != 0) ? (masterServerAddress + ":" + port) : masterServerAddress);
		return PhotonNetwork.networkingPeer.Connect(PhotonNetwork.networkingPeer.MasterServerAddress, ServerConnection.MasterServer);
	}

	// Token: 0x06001B75 RID: 7029 RVA: 0x000B17BC File Offset: 0x000AF9BC
	public static bool Reconnect()
	{
		if (string.IsNullOrEmpty(PhotonNetwork.networkingPeer.MasterServerAddress))
		{
			UnityEngine.Debug.LogWarning("Reconnect() failed. It seems the client wasn't connected before?! Current state: " + PhotonNetwork.networkingPeer.PeerState);
			return false;
		}
		if (PhotonNetwork.networkingPeer.PeerState != PeerStateValue.Disconnected)
		{
			UnityEngine.Debug.LogWarning("Reconnect() failed. Can only connect while in state 'Disconnected'. Current state: " + PhotonNetwork.networkingPeer.PeerState);
			return false;
		}
		if (PhotonNetwork.offlineMode)
		{
			PhotonNetwork.offlineMode = false;
			UnityEngine.Debug.LogWarning("Reconnect() disabled the offline mode. No longer offline.");
		}
		if (!PhotonNetwork.isMessageQueueRunning)
		{
			PhotonNetwork.isMessageQueueRunning = true;
			UnityEngine.Debug.LogWarning("Reconnect() enabled isMessageQueueRunning. Needs to be able to dispatch incoming messages.");
		}
		PhotonNetwork.networkingPeer.IsUsingNameServer = false;
		PhotonNetwork.networkingPeer.IsInitialConnect = false;
		return PhotonNetwork.networkingPeer.ReconnectToMaster();
	}

	// Token: 0x06001B76 RID: 7030 RVA: 0x000B1880 File Offset: 0x000AFA80
	public static bool ReconnectAndRejoin()
	{
		if (PhotonNetwork.networkingPeer.PeerState != PeerStateValue.Disconnected)
		{
			UnityEngine.Debug.LogWarning("ReconnectAndRejoin() failed. Can only connect while in state 'Disconnected'. Current state: " + PhotonNetwork.networkingPeer.PeerState);
			return false;
		}
		if (PhotonNetwork.offlineMode)
		{
			PhotonNetwork.offlineMode = false;
			UnityEngine.Debug.LogWarning("ReconnectAndRejoin() disabled the offline mode. No longer offline.");
		}
		if (string.IsNullOrEmpty(PhotonNetwork.networkingPeer.GameServerAddress))
		{
			UnityEngine.Debug.LogWarning("ReconnectAndRejoin() failed. It seems the client wasn't connected to a game server before (no address).");
			return false;
		}
		if (PhotonNetwork.networkingPeer.enterRoomParamsCache == null)
		{
			UnityEngine.Debug.LogWarning("ReconnectAndRejoin() failed. It seems the client doesn't have any previous room to re-join.");
			return false;
		}
		if (!PhotonNetwork.isMessageQueueRunning)
		{
			PhotonNetwork.isMessageQueueRunning = true;
			UnityEngine.Debug.LogWarning("ReconnectAndRejoin() enabled isMessageQueueRunning. Needs to be able to dispatch incoming messages.");
		}
		PhotonNetwork.networkingPeer.IsUsingNameServer = false;
		PhotonNetwork.networkingPeer.IsInitialConnect = false;
		return PhotonNetwork.networkingPeer.ReconnectAndRejoin();
	}

	// Token: 0x06001B77 RID: 7031 RVA: 0x00013E70 File Offset: 0x00012070
	public static bool ConnectToBestCloudServer(string gameVersion)
	{
		return PhotonNetwork.ConnectToBestCloudServer(gameVersion, PhotonNetwork.PhotonServerSettings.AppID);
	}

	// Token: 0x06001B78 RID: 7032 RVA: 0x000B194C File Offset: 0x000AFB4C
	public static bool ConnectToBestCloudServer(string gameVersion, string appID)
	{
		GUI.enabled = false;
		if (PhotonNetwork.networkingPeer.PeerState != PeerStateValue.Disconnected)
		{
			UnityEngine.Debug.LogWarning("ConnectToBestCloudServer() failed. Can only connect while in state 'Disconnected'. Current state: " + PhotonNetwork.networkingPeer.PeerState);
			return false;
		}
		if (PhotonNetwork.PhotonServerSettings == null)
		{
			UnityEngine.Debug.LogError("Can't connect: Loading settings failed. ServerSettings asset must be in any 'Resources' folder as: PhotonServerSettings");
			return false;
		}
		if (PhotonNetwork.PhotonServerSettings.HostType == ServerSettings.HostingOption.OfflineMode)
		{
			return PhotonNetwork.ConnectUsingSettings(gameVersion);
		}
		PhotonNetwork.networkingPeer.IsInitialConnect = true;
		PhotonNetwork.networkingPeer.SetApp(appID, gameVersion);
		return PhotonNetwork.networkingPeer.ConnectToNameServer();
	}

	// Token: 0x06001B79 RID: 7033 RVA: 0x000B19DC File Offset: 0x000AFBDC
	public static bool ConnectToRegion(CloudRegionCode region, string gameVersion, string appID)
	{
		GUI.enabled = false;
		if (PhotonNetwork.networkingPeer.PeerState != PeerStateValue.Disconnected)
		{
			UnityEngine.Debug.LogWarning("ConnectToRegion() failed. Can only connect while in state 'Disconnected'. Current state: " + PhotonNetwork.networkingPeer.PeerState);
			return false;
		}
		if (PhotonNetwork.PhotonServerSettings == null)
		{
			UnityEngine.Debug.LogError("Can't connect: ServerSettings asset must be in any 'Resources' folder as: PhotonServerSettings");
			return false;
		}
		if (PhotonNetwork.PhotonServerSettings.HostType == ServerSettings.HostingOption.OfflineMode)
		{
			return PhotonNetwork.ConnectUsingSettings(gameVersion);
		}
		PhotonNetwork.networkingPeer.IsInitialConnect = true;
		PhotonNetwork.networkingPeer.SetApp(appID, gameVersion);
		return region != CloudRegionCode.none && PhotonNetwork.networkingPeer.ConnectToRegionMaster(region);
	}

	// Token: 0x06001B7A RID: 7034 RVA: 0x00013E82 File Offset: 0x00012082
	public static void OverrideBestCloudServer(CloudRegionCode region)
	{
		PhotonHandler.BestRegionCodeInPreferences = region;
	}

	// Token: 0x06001B7B RID: 7035 RVA: 0x00013E8A File Offset: 0x0001208A
	public static void RefreshCloudServerRating()
	{
		throw new NotImplementedException("not available at the moment");
	}

	// Token: 0x06001B7C RID: 7036 RVA: 0x00013E96 File Offset: 0x00012096
	public static void NetworkStatisticsReset()
	{
		PhotonNetwork.networkingPeer.TrafficStatsReset();
	}

	// Token: 0x06001B7D RID: 7037 RVA: 0x00013EA2 File Offset: 0x000120A2
	public static string NetworkStatisticsToString()
	{
		if (PhotonNetwork.networkingPeer == null || PhotonNetwork.offlineMode)
		{
			return "Offline or in OfflineMode. No VitalStats available.";
		}
		return PhotonNetwork.networkingPeer.VitalStatsToString(false);
	}

	// Token: 0x06001B7E RID: 7038 RVA: 0x0000574F File Offset: 0x0000394F
	[Obsolete("Used for compatibility with Unity networking only. Encryption is automatically initialized while connecting.")]
	public static void InitializeSecurity()
	{
	}

	// Token: 0x06001B7F RID: 7039 RVA: 0x00013EC9 File Offset: 0x000120C9
	private static bool VerifyCanUseNetwork()
	{
		if (PhotonNetwork.connected)
		{
			return true;
		}
		UnityEngine.Debug.LogError("Cannot send messages when not connected. Either connect to Photon OR use offline mode!");
		return false;
	}

	// Token: 0x06001B80 RID: 7040 RVA: 0x000B1A70 File Offset: 0x000AFC70
	public static void Disconnect()
	{
		if (PhotonNetwork.offlineMode)
		{
			PhotonNetwork.offlineMode = false;
			PhotonNetwork.offlineModeRoom = null;
			PhotonNetwork.networkingPeer.State = ClientState.Disconnecting;
			PhotonNetwork.networkingPeer.OnStatusChanged(StatusCode.Disconnect);
			return;
		}
		if (PhotonNetwork.networkingPeer == null)
		{
			return;
		}
		PhotonNetwork.networkingPeer.Disconnect();
	}

	// Token: 0x06001B81 RID: 7041 RVA: 0x00013EE2 File Offset: 0x000120E2
	public static bool FindFriends(string[] friendsToFind)
	{
		return PhotonNetwork.networkingPeer != null && !PhotonNetwork.isOfflineMode && PhotonNetwork.networkingPeer.OpFindFriends(friendsToFind);
	}

	// Token: 0x06001B82 RID: 7042 RVA: 0x00013F05 File Offset: 0x00012105
	public static bool CreateRoom(string roomName)
	{
		return PhotonNetwork.CreateRoom(roomName, null, null, null);
	}

	// Token: 0x06001B83 RID: 7043 RVA: 0x00013F10 File Offset: 0x00012110
	public static bool CreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby)
	{
		return PhotonNetwork.CreateRoom(roomName, roomOptions, typedLobby, null);
	}

	// Token: 0x06001B84 RID: 7044 RVA: 0x000B1AC4 File Offset: 0x000AFCC4
	public static bool CreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby, string[] expectedUsers)
	{
		if (PhotonNetwork.offlineMode)
		{
			if (PhotonNetwork.offlineModeRoom != null)
			{
				UnityEngine.Debug.LogError("CreateRoom failed. In offline mode you still have to leave a room to enter another.");
				return false;
			}
			PhotonNetwork.EnterOfflineRoom(roomName, roomOptions, true);
			return true;
		}
		else
		{
			if (PhotonNetwork.networkingPeer.Server != ServerConnection.MasterServer || !PhotonNetwork.connectedAndReady)
			{
				UnityEngine.Debug.LogError("CreateRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
				return false;
			}
			typedLobby = (typedLobby ?? ((!PhotonNetwork.networkingPeer.insideLobby) ? null : PhotonNetwork.networkingPeer.lobby));
			EnterRoomParams enterRoomParams = new EnterRoomParams();
			enterRoomParams.RoomName = roomName;
			enterRoomParams.RoomOptions = roomOptions;
			enterRoomParams.Lobby = typedLobby;
			enterRoomParams.ExpectedUsers = expectedUsers;
			return PhotonNetwork.networkingPeer.OpCreateGame(enterRoomParams);
		}
	}

	// Token: 0x06001B85 RID: 7045 RVA: 0x00013F1B File Offset: 0x0001211B
	public static bool JoinRoom(string roomName)
	{
		return PhotonNetwork.JoinRoom(roomName, null);
	}

	// Token: 0x06001B86 RID: 7046 RVA: 0x000B1B78 File Offset: 0x000AFD78
	public static bool JoinRoom(string roomName, string[] expectedUsers)
	{
		if (PhotonNetwork.offlineMode)
		{
			if (PhotonNetwork.offlineModeRoom != null)
			{
				UnityEngine.Debug.LogError("JoinRoom failed. In offline mode you still have to leave a room to enter another.");
				return false;
			}
			PhotonNetwork.EnterOfflineRoom(roomName, null, true);
			return true;
		}
		else
		{
			if (PhotonNetwork.networkingPeer.Server != ServerConnection.MasterServer || !PhotonNetwork.connectedAndReady)
			{
				UnityEngine.Debug.LogError("JoinRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
				return false;
			}
			if (string.IsNullOrEmpty(roomName))
			{
				UnityEngine.Debug.LogError("JoinRoom failed. A roomname is required. If you don't know one, how will you join?");
				return false;
			}
			EnterRoomParams enterRoomParams = new EnterRoomParams();
			enterRoomParams.RoomName = roomName;
			enterRoomParams.ExpectedUsers = expectedUsers;
			return PhotonNetwork.networkingPeer.OpJoinRoom(enterRoomParams);
		}
	}

	// Token: 0x06001B87 RID: 7047 RVA: 0x00013F24 File Offset: 0x00012124
	public static bool JoinOrCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby)
	{
		return PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, typedLobby, null);
	}

	// Token: 0x06001B88 RID: 7048 RVA: 0x000B1C0C File Offset: 0x000AFE0C
	public static bool JoinOrCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby, string[] expectedUsers)
	{
		if (PhotonNetwork.offlineMode)
		{
			if (PhotonNetwork.offlineModeRoom != null)
			{
				UnityEngine.Debug.LogError("JoinOrCreateRoom failed. In offline mode you still have to leave a room to enter another.");
				return false;
			}
			PhotonNetwork.EnterOfflineRoom(roomName, roomOptions, true);
			return true;
		}
		else
		{
			if (PhotonNetwork.networkingPeer.Server != ServerConnection.MasterServer || !PhotonNetwork.connectedAndReady)
			{
				UnityEngine.Debug.LogError("JoinOrCreateRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
				return false;
			}
			if (string.IsNullOrEmpty(roomName))
			{
				UnityEngine.Debug.LogError("JoinOrCreateRoom failed. A roomname is required. If you don't know one, how will you join?");
				return false;
			}
			typedLobby = (typedLobby ?? ((!PhotonNetwork.networkingPeer.insideLobby) ? null : PhotonNetwork.networkingPeer.lobby));
			EnterRoomParams enterRoomParams = new EnterRoomParams();
			enterRoomParams.RoomName = roomName;
			enterRoomParams.RoomOptions = roomOptions;
			enterRoomParams.Lobby = typedLobby;
			enterRoomParams.CreateIfNotExists = true;
			enterRoomParams.PlayerProperties = PhotonNetwork.player.CustomProperties;
			enterRoomParams.ExpectedUsers = expectedUsers;
			return PhotonNetwork.networkingPeer.OpJoinRoom(enterRoomParams);
		}
	}

	// Token: 0x06001B89 RID: 7049 RVA: 0x00013F2F File Offset: 0x0001212F
	public static bool JoinRandomRoom()
	{
		return PhotonNetwork.JoinRandomRoom(null, 0, MatchmakingMode.FillRoom, null, null, null);
	}

	// Token: 0x06001B8A RID: 7050 RVA: 0x00013F3C File Offset: 0x0001213C
	public static bool JoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers)
	{
		return PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, MatchmakingMode.FillRoom, null, null, null);
	}

	// Token: 0x06001B8B RID: 7051 RVA: 0x000B1CEC File Offset: 0x000AFEEC
	public static bool JoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, MatchmakingMode matchingType, TypedLobby typedLobby, string sqlLobbyFilter, string[] expectedUsers = null)
	{
		if (PhotonNetwork.offlineMode)
		{
			if (PhotonNetwork.offlineModeRoom != null)
			{
				UnityEngine.Debug.LogError("JoinRandomRoom failed. In offline mode you still have to leave a room to enter another.");
				return false;
			}
			PhotonNetwork.EnterOfflineRoom("offline room", null, true);
			return true;
		}
		else
		{
			if (PhotonNetwork.networkingPeer.Server != ServerConnection.MasterServer || !PhotonNetwork.connectedAndReady)
			{
				UnityEngine.Debug.LogError("JoinRandomRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
				return false;
			}
			typedLobby = (typedLobby ?? ((!PhotonNetwork.networkingPeer.insideLobby) ? null : PhotonNetwork.networkingPeer.lobby));
			OpJoinRandomRoomParams opJoinRandomRoomParams = new OpJoinRandomRoomParams();
			opJoinRandomRoomParams.ExpectedCustomRoomProperties = expectedCustomRoomProperties;
			opJoinRandomRoomParams.ExpectedMaxPlayers = expectedMaxPlayers;
			opJoinRandomRoomParams.MatchingType = matchingType;
			opJoinRandomRoomParams.TypedLobby = typedLobby;
			opJoinRandomRoomParams.SqlLobbyFilter = sqlLobbyFilter;
			opJoinRandomRoomParams.ExpectedUsers = expectedUsers;
			return PhotonNetwork.networkingPeer.OpJoinRandomRoom(opJoinRandomRoomParams);
		}
	}

	// Token: 0x06001B8C RID: 7052 RVA: 0x000B1DB4 File Offset: 0x000AFFB4
	public static bool ReJoinRoom(string roomName)
	{
		if (PhotonNetwork.offlineMode)
		{
			UnityEngine.Debug.LogError("ReJoinRoom failed due to offline mode.");
			return false;
		}
		if (PhotonNetwork.networkingPeer.Server != ServerConnection.MasterServer || !PhotonNetwork.connectedAndReady)
		{
			UnityEngine.Debug.LogError("ReJoinRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
			return false;
		}
		if (string.IsNullOrEmpty(roomName))
		{
			UnityEngine.Debug.LogError("ReJoinRoom failed. A roomname is required. If you don't know one, how will you join?");
			return false;
		}
		EnterRoomParams enterRoomParams = new EnterRoomParams();
		enterRoomParams.RoomName = roomName;
		enterRoomParams.RejoinOnly = true;
		enterRoomParams.PlayerProperties = PhotonNetwork.player.CustomProperties;
		return PhotonNetwork.networkingPeer.OpJoinRoom(enterRoomParams);
	}

	// Token: 0x06001B8D RID: 7053 RVA: 0x000B1E44 File Offset: 0x000B0044
	private static void EnterOfflineRoom(string roomName, RoomOptions roomOptions, bool createdRoom)
	{
		PhotonNetwork.offlineModeRoom = new Room(roomName, roomOptions);
		PhotonNetwork.networkingPeer.ChangeLocalID(1);
		PhotonNetwork.networkingPeer.State = ClientState.ConnectingToGameserver;
		PhotonNetwork.networkingPeer.OnStatusChanged(StatusCode.Connect);
		PhotonNetwork.offlineModeRoom.MasterClientId = 1;
		if (createdRoom)
		{
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom, new object[0]);
		}
		NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom, new object[0]);
	}

	// Token: 0x06001B8E RID: 7054 RVA: 0x00013F49 File Offset: 0x00012149
	public static bool JoinLobby()
	{
		return PhotonNetwork.JoinLobby(null);
	}

	// Token: 0x06001B8F RID: 7055 RVA: 0x000B1EAC File Offset: 0x000B00AC
	public static bool JoinLobby(TypedLobby typedLobby)
	{
		if (PhotonNetwork.connected && PhotonNetwork.Server == ServerConnection.MasterServer)
		{
			if (typedLobby == null)
			{
				typedLobby = TypedLobby.Default;
			}
			bool flag = PhotonNetwork.networkingPeer.OpJoinLobby(typedLobby);
			if (flag)
			{
				PhotonNetwork.networkingPeer.lobby = typedLobby;
			}
			return flag;
		}
		return false;
	}

	// Token: 0x06001B90 RID: 7056 RVA: 0x00013F51 File Offset: 0x00012151
	public static bool LeaveLobby()
	{
		return PhotonNetwork.connected && PhotonNetwork.Server == ServerConnection.MasterServer && PhotonNetwork.networkingPeer.OpLeaveLobby();
	}

	// Token: 0x06001B91 RID: 7057 RVA: 0x000B1EFC File Offset: 0x000B00FC
	public static bool LeaveRoom(bool becomeInactive = true)
	{
		PhotonNetwork.leavingRoom = true;
		if (PhotonNetwork.offlineMode)
		{
			PhotonNetwork.offlineModeRoom = null;
			PhotonNetwork.networkingPeer.State = ClientState.PeerCreated;
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnLeftRoom, new object[0]);
			return true;
		}
		if (PhotonNetwork.room == null)
		{
			UnityEngine.Debug.LogWarning("PhotonNetwork.room is null. You don't have to call LeaveRoom() when you're not in one. State: " + PhotonNetwork.connectionStateDetailed);
		}
		else
		{
			becomeInactive = (becomeInactive && PhotonNetwork.room.PlayerTtl != 0);
		}
		return PhotonNetwork.networkingPeer.OpLeaveRoom(becomeInactive);
	}

	// Token: 0x06001B92 RID: 7058 RVA: 0x00013F73 File Offset: 0x00012173
	public static bool GetCustomRoomList(TypedLobby typedLobby, string sqlLobbyFilter)
	{
		return PhotonNetwork.networkingPeer.OpGetGameList(typedLobby, sqlLobbyFilter);
	}

	// Token: 0x06001B93 RID: 7059 RVA: 0x00013F81 File Offset: 0x00012181
	public static RoomInfo[] GetRoomList()
	{
		if (PhotonNetwork.offlineMode || PhotonNetwork.networkingPeer == null)
		{
			return new RoomInfo[0];
		}
		return PhotonNetwork.networkingPeer.mGameListCopy;
	}

	// Token: 0x06001B94 RID: 7060 RVA: 0x000B1F8C File Offset: 0x000B018C
	public static void SetPlayerCustomProperties(Hashtable customProperties)
	{
		if (customProperties == null)
		{
			customProperties = new Hashtable();
			foreach (object obj in PhotonNetwork.player.CustomProperties.Keys)
			{
				customProperties[(string)obj] = null;
			}
		}
		if (PhotonNetwork.room != null && PhotonNetwork.room.IsLocalClientInside)
		{
			PhotonNetwork.player.SetCustomProperties(customProperties, null, false);
		}
		else
		{
			PhotonNetwork.player.InternalCacheProperties(customProperties);
		}
	}

	// Token: 0x06001B95 RID: 7061 RVA: 0x000B2038 File Offset: 0x000B0238
	public static void RemovePlayerCustomProperties(string[] customPropertiesToDelete)
	{
		if (customPropertiesToDelete == null || customPropertiesToDelete.Length == 0 || PhotonNetwork.player.CustomProperties == null)
		{
			PhotonNetwork.player.CustomProperties = new Hashtable();
			return;
		}
		foreach (string key in customPropertiesToDelete)
		{
			if (PhotonNetwork.player.CustomProperties.ContainsKey(key))
			{
				PhotonNetwork.player.CustomProperties.Remove(key);
			}
		}
	}

	// Token: 0x06001B96 RID: 7062 RVA: 0x00013FA8 File Offset: 0x000121A8
	public static bool RaiseEvent(byte eventCode, object eventContent, bool sendReliable, RaiseEventOptions options)
	{
		if (!PhotonNetwork.inRoom || eventCode >= 200)
		{
			UnityEngine.Debug.LogWarning("RaiseEvent() failed. Your event is not being sent! Check if your are in a Room and the eventCode must be less than 200 (0..199).");
			return false;
		}
		return PhotonNetwork.networkingPeer.OpRaiseEvent(eventCode, eventContent, sendReliable, options);
	}

	// Token: 0x06001B97 RID: 7063 RVA: 0x000B20B0 File Offset: 0x000B02B0
	public static int AllocateViewID()
	{
		int num = PhotonNetwork.AllocateViewID(PhotonNetwork.player.ID);
		PhotonNetwork.manuallyAllocatedViewIds.Add(num);
		return num;
	}

	// Token: 0x06001B98 RID: 7064 RVA: 0x000B20DC File Offset: 0x000B02DC
	public static int AllocateSceneViewID()
	{
		if (!PhotonNetwork.isMasterClient)
		{
			UnityEngine.Debug.LogError("Only the Master Client can AllocateSceneViewID(). Check PhotonNetwork.isMasterClient!");
			return -1;
		}
		int num = PhotonNetwork.AllocateViewID(0);
		PhotonNetwork.manuallyAllocatedViewIds.Add(num);
		return num;
	}

	// Token: 0x06001B99 RID: 7065 RVA: 0x000B2114 File Offset: 0x000B0314
	private static int AllocateViewID(int ownerId)
	{
		if (ownerId == 0)
		{
			int num = PhotonNetwork.lastUsedViewSubIdStatic;
			int num2 = ownerId * PhotonNetwork.MAX_VIEW_IDS;
			for (int i = 1; i < PhotonNetwork.MAX_VIEW_IDS; i++)
			{
				num = (num + 1) % PhotonNetwork.MAX_VIEW_IDS;
				if (num != 0)
				{
					int num3 = num + num2;
					if (!PhotonNetwork.networkingPeer.photonViewList.ContainsKey(num3))
					{
						PhotonNetwork.lastUsedViewSubIdStatic = num;
						return num3;
					}
				}
			}
			throw new Exception(string.Format("AllocateViewID() failed. Room (user {0}) is out of 'scene' viewIDs. It seems all available are in use.", ownerId));
		}
		int num4 = PhotonNetwork.lastUsedViewSubId;
		int num5 = ownerId * PhotonNetwork.MAX_VIEW_IDS;
		for (int j = 1; j < PhotonNetwork.MAX_VIEW_IDS; j++)
		{
			num4 = (num4 + 1) % PhotonNetwork.MAX_VIEW_IDS;
			if (num4 != 0)
			{
				int num6 = num4 + num5;
				if (!PhotonNetwork.networkingPeer.photonViewList.ContainsKey(num6) && !PhotonNetwork.manuallyAllocatedViewIds.Contains(num6))
				{
					PhotonNetwork.lastUsedViewSubId = num4;
					return num6;
				}
			}
		}
		throw new Exception(string.Format("AllocateViewID() failed. User {0} is out of subIds, as all viewIDs are used.", ownerId));
	}

	// Token: 0x06001B9A RID: 7066 RVA: 0x000B2228 File Offset: 0x000B0428
	private static int[] AllocateSceneViewIDs(int countOfNewViews)
	{
		int[] array = new int[countOfNewViews];
		for (int i = 0; i < countOfNewViews; i++)
		{
			array[i] = PhotonNetwork.AllocateViewID(0);
		}
		return array;
	}

	// Token: 0x06001B9B RID: 7067 RVA: 0x000B2258 File Offset: 0x000B0458
	public static void UnAllocateViewID(int viewID)
	{
		PhotonNetwork.manuallyAllocatedViewIds.Remove(viewID);
		if (PhotonNetwork.networkingPeer.photonViewList.ContainsKey(viewID))
		{
			UnityEngine.Debug.LogWarning(string.Format("UnAllocateViewID() should be called after the PhotonView was destroyed (GameObject.Destroy()). ViewID: {0} still found in: {1}", viewID, PhotonNetwork.networkingPeer.photonViewList[viewID]));
		}
	}

	// Token: 0x06001B9C RID: 7068 RVA: 0x00013FD9 File Offset: 0x000121D9
	public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, byte group)
	{
		return PhotonNetwork.Instantiate(prefabName, position, rotation, group, null);
	}

	// Token: 0x06001B9D RID: 7069 RVA: 0x000B22AC File Offset: 0x000B04AC
	public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, byte group, object[] data)
	{
        if (!connected || (InstantiateInRoomOnly && !inRoom))
        {
            UnityEngine.Debug.LogError("Failed to Instantiate prefab: " + prefabName + ". Client should be in a room. Current connectionStateDetailed: " + PhotonNetwork.connectionStateDetailed);
            return null;
        }

        GameObject prefabGo;
        if (!UsePrefabCache || !PrefabCache.TryGetValue(prefabName, out prefabGo))
        {
            prefabGo = (GameObject)Resources.Load(prefabName, typeof(GameObject));
            if (UsePrefabCache)
            {
                PrefabCache.Add(prefabName, prefabGo);
            }
        }

        if (prefabGo == null)
        {
            UnityEngine.Debug.LogError("Failed to Instantiate prefab: " + prefabName + ". Verify the Prefab is in a Resources folder (and not in a subfolder)");
            return null;
        }

        // a scene object instantiated with network visibility has to contain a PhotonView
        if (prefabGo.GetComponent<PhotonView>() == null)
        {
            UnityEngine.Debug.LogError("Failed to Instantiate prefab:" + prefabName + ". Prefab must have a PhotonView component.");
            return null;
        }

        Component[] views = (Component[])prefabGo.GetPhotonViewsInChildren();
        int[] viewIDs = new int[views.Length];
        for (int i = 0; i < viewIDs.Length; i++)
        {
            //Debug.Log("Instantiate prefabName: " + prefabName + " player.ID: " + player.ID);
            viewIDs[i] = AllocateViewID(player.ID);
        }

        // Send to others, create info
        Hashtable instantiateEvent = networkingPeer.SendInstantiate(prefabName, position, rotation, group, viewIDs, data, false);

        // Instantiate the GO locally (but the same way as if it was done via event). This will also cache the instantiationId
        return networkingPeer.DoInstantiate(instantiateEvent, networkingPeer.LocalPlayer, prefabGo);
    }

	// Token: 0x06001B9E RID: 7070 RVA: 0x000B2400 File Offset: 0x000B0600
	public static GameObject InstantiateSceneObject(string prefabName, Vector3 position, Quaternion rotation, byte group, object[] data)
	{
		if (!PhotonNetwork.connected || (PhotonNetwork.InstantiateInRoomOnly && !PhotonNetwork.inRoom))
		{
			UnityEngine.Debug.LogError(string.Concat(new object[]
			{
				"Failed to InstantiateSceneObject prefab: ",
				prefabName,
				". Client should be in a room. Current connectionStateDetailed: ",
				PhotonNetwork.connectionStateDetailed
			}));
			return null;
		}
		if (!PhotonNetwork.isMasterClient)
		{
			UnityEngine.Debug.LogError("Failed to InstantiateSceneObject prefab: " + prefabName + ". Client is not the MasterClient in this room.");
			return null;
		}
		GameObject gameObject;
		if (!PhotonNetwork.UsePrefabCache || !PhotonNetwork.PrefabCache.TryGetValue(prefabName, out gameObject))
		{
			gameObject = (GameObject)Resources.Load(prefabName, typeof(GameObject));
			if (PhotonNetwork.UsePrefabCache)
			{
				PhotonNetwork.PrefabCache.Add(prefabName, gameObject);
			}
		}
		if (gameObject == null)
		{
			UnityEngine.Debug.LogError("Failed to InstantiateSceneObject prefab: " + prefabName + ". Verify the Prefab is in a Resources folder (and not in a subfolder)");
			return null;
		}
		if (gameObject.GetComponent<PhotonView>() == null)
		{
			UnityEngine.Debug.LogError("Failed to InstantiateSceneObject prefab:" + prefabName + ". Prefab must have a PhotonView component.");
			return null;
		}
		Component[] photonViewsInChildren = gameObject.GetPhotonViewsInChildren();
		int[] array = PhotonNetwork.AllocateSceneViewIDs(photonViewsInChildren.Length);
		if (array == null)
		{
			UnityEngine.Debug.LogError(string.Concat(new object[]
			{
				"Failed to InstantiateSceneObject prefab: ",
				prefabName,
				". No ViewIDs are free to use. Max is: ",
				PhotonNetwork.MAX_VIEW_IDS
			}));
			return null;
		}
		Hashtable evData = PhotonNetwork.networkingPeer.SendInstantiate(prefabName, position, rotation, group, array, data, true);
		return PhotonNetwork.networkingPeer.DoInstantiate(evData, PhotonNetwork.networkingPeer.LocalPlayer, gameObject);
	}

	// Token: 0x06001B9F RID: 7071 RVA: 0x00013FE5 File Offset: 0x000121E5
	public static int GetPing()
	{
		return PhotonNetwork.networkingPeer.RoundTripTime;
	}

	// Token: 0x06001BA0 RID: 7072 RVA: 0x00013FF1 File Offset: 0x000121F1
	public static void FetchServerTimestamp()
	{
		if (PhotonNetwork.networkingPeer != null)
		{
			PhotonNetwork.networkingPeer.FetchServerTimestamp();
		}
	}

	// Token: 0x06001BA1 RID: 7073 RVA: 0x00014007 File Offset: 0x00012207
	public static void SendOutgoingCommands()
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		while (PhotonNetwork.networkingPeer.SendOutgoingCommands())
		{
		}
	}

	// Token: 0x06001BA2 RID: 7074 RVA: 0x000B2584 File Offset: 0x000B0784
	public static bool CloseConnection(PhotonPlayer kickPlayer)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return false;
		}
		if (!PhotonNetwork.player.IsMasterClient)
		{
			UnityEngine.Debug.LogError("CloseConnection: Only the masterclient can kick another player.");
			return false;
		}
		if (kickPlayer == null)
		{
			UnityEngine.Debug.LogError("CloseConnection: No such player connected!");
			return false;
		}
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions
		{
			TargetActors = new int[]
			{
				kickPlayer.ID
			}
		};
		return PhotonNetwork.networkingPeer.OpRaiseEvent(203, null, true, raiseEventOptions);
	}

	// Token: 0x06001BA3 RID: 7075 RVA: 0x000B25FC File Offset: 0x000B07FC
	public static bool SetMasterClient(PhotonPlayer masterClientPlayer)
	{
		if (!PhotonNetwork.inRoom || !PhotonNetwork.VerifyCanUseNetwork() || PhotonNetwork.offlineMode)
		{
			if (PhotonNetwork.logLevel == PhotonLogLevel.Informational)
			{
				UnityEngine.Debug.Log("Can not SetMasterClient(). Not in room or in offlineMode.");
			}
			return false;
		}
		if (PhotonNetwork.room.serverSideMasterClient)
		{
			Hashtable gameProperties = new Hashtable
			{
				{
                    GamePropertyKey.MasterClientId,
					masterClientPlayer.ID
				}
			};
			Hashtable expectedProperties = new Hashtable
			{
				{
                    GamePropertyKey.MasterClientId,
					PhotonNetwork.networkingPeer.mMasterClientId
				}
			};
			return PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(gameProperties, expectedProperties, false);
		}
		return PhotonNetwork.isMasterClient && PhotonNetwork.networkingPeer.SetMasterClient(masterClientPlayer.ID, true);
	}

	// Token: 0x06001BA4 RID: 7076 RVA: 0x00014028 File Offset: 0x00012228
	public static void Destroy(PhotonView targetView)
	{
		if (targetView != null)
		{
			PhotonNetwork.networkingPeer.RemoveInstantiatedGO(targetView.gameObject, !PhotonNetwork.inRoom);
		}
		else
		{
			UnityEngine.Debug.LogError("Destroy(targetPhotonView) failed, cause targetPhotonView is null.");
		}
	}

	// Token: 0x06001BA5 RID: 7077 RVA: 0x0001405D File Offset: 0x0001225D
	public static void Destroy(GameObject targetGo)
	{
		PhotonNetwork.networkingPeer.RemoveInstantiatedGO(targetGo, !PhotonNetwork.inRoom);
	}

	// Token: 0x06001BA6 RID: 7078 RVA: 0x00014072 File Offset: 0x00012272
	public static void DestroyPlayerObjects(PhotonPlayer targetPlayer)
	{
		if (PhotonNetwork.player == null)
		{
			UnityEngine.Debug.LogError("DestroyPlayerObjects() failed, cause parameter 'targetPlayer' was null.");
		}
		PhotonNetwork.DestroyPlayerObjects(targetPlayer.ID);
	}

	// Token: 0x06001BA7 RID: 7079 RVA: 0x000B26C4 File Offset: 0x000B08C4
	public static void DestroyPlayerObjects(int targetPlayerId)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		if (PhotonNetwork.player.IsMasterClient || targetPlayerId == PhotonNetwork.player.ID)
		{
			PhotonNetwork.networkingPeer.DestroyPlayerObjects(targetPlayerId, false);
		}
		else
		{
			UnityEngine.Debug.LogError("DestroyPlayerObjects() failed, cause players can only destroy their own GameObjects. A Master Client can destroy anyone's. This is master: " + PhotonNetwork.isMasterClient);
		}
	}

	// Token: 0x06001BA8 RID: 7080 RVA: 0x00014093 File Offset: 0x00012293
	public static void DestroyAll()
	{
		if (PhotonNetwork.isMasterClient)
		{
			PhotonNetwork.networkingPeer.DestroyAll(false);
		}
		else
		{
			UnityEngine.Debug.LogError("Couldn't call DestroyAll() as only the master client is allowed to call this.");
		}
	}

	// Token: 0x06001BA9 RID: 7081 RVA: 0x000140B9 File Offset: 0x000122B9
	public static void RemoveRPCs(PhotonPlayer targetPlayer)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		if (!targetPlayer.IsLocal && !PhotonNetwork.isMasterClient)
		{
			UnityEngine.Debug.LogError("Error; Only the MasterClient can call RemoveRPCs for other players.");
			return;
		}
		PhotonNetwork.networkingPeer.OpCleanRpcBuffer(targetPlayer.ID);
	}

	// Token: 0x06001BAA RID: 7082 RVA: 0x000140F6 File Offset: 0x000122F6
	public static void RemoveRPCs(PhotonView targetPhotonView)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		PhotonNetwork.networkingPeer.CleanRpcBufferIfMine(targetPhotonView);
	}

	// Token: 0x06001BAB RID: 7083 RVA: 0x0001410E File Offset: 0x0001230E
	public static void RemoveRPCsInGroup(int targetGroup)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		PhotonNetwork.networkingPeer.RemoveRPCsInGroup(targetGroup);
	}

	// Token: 0x06001BAC RID: 7084 RVA: 0x000B2728 File Offset: 0x000B0928
	internal static void RPC(PhotonView view, string methodName, PhotonTargets target, bool encrypt, byte[] data)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		if (PhotonNetwork.room == null)
		{
			UnityEngine.Debug.LogWarning("RPCs can only be sent in rooms. Call of \"" + methodName + "\" gets executed locally only, if at all.");
			return;
		}
		if (PhotonNetwork.networkingPeer != null)
		{
			if (PhotonNetwork.room.serverSideMasterClient)
			{
				PhotonNetwork.networkingPeer.RPC(view, methodName, target, null, encrypt, data);
			}
			else if (PhotonNetwork.networkingPeer.hasSwitchedMC && target == PhotonTargets.MasterClient)
			{
				PhotonNetwork.networkingPeer.RPC(view, methodName, PhotonTargets.Others, PhotonNetwork.masterClient, encrypt, data);
			}
			else
			{
				PhotonNetwork.networkingPeer.RPC(view, methodName, target, null, encrypt, data);
			}
		}
		else
		{
			UnityEngine.Debug.LogWarning("Could not execute RPC " + methodName + ". Possible scene loading in progress?");
		}
	}

	// Token: 0x06001BAD RID: 7085 RVA: 0x000B27EC File Offset: 0x000B09EC
	internal static void RPC(PhotonView view, string methodName, PhotonPlayer targetPlayer, bool encrpyt, byte[] data)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		if (PhotonNetwork.room == null)
		{
			UnityEngine.Debug.LogWarning("RPCs can only be sent in rooms. Call of \"" + methodName + "\" gets executed locally only, if at all.");
			return;
		}
		if (PhotonNetwork.player == null)
		{
			UnityEngine.Debug.LogError("RPC can't be sent to target PhotonPlayer being null! Did not send \"" + methodName + "\" call.");
		}
		if (PhotonNetwork.networkingPeer != null)
		{
			PhotonNetwork.networkingPeer.RPC(view, methodName, PhotonTargets.Others, targetPlayer, encrpyt, data);
		}
		else
		{
			UnityEngine.Debug.LogWarning("Could not execute RPC " + methodName + ". Possible scene loading in progress?");
		}
	}

	// Token: 0x06001BAE RID: 7086 RVA: 0x00014126 File Offset: 0x00012326
	public static void CacheSendMonoMessageTargets(Type type)
	{
		if (type == null)
		{
			type = PhotonNetwork.SendMonoMessageTargetType;
		}
		PhotonNetwork.SendMonoMessageTargets = PhotonNetwork.FindGameObjectsWithComponent(type);
	}

	// Token: 0x06001BAF RID: 7087 RVA: 0x000B2878 File Offset: 0x000B0A78
	public static HashSet<GameObject> FindGameObjectsWithComponent(Type type)
	{
		HashSet<GameObject> hashSet = new HashSet<GameObject>();
		Component[] array = (Component[])UnityEngine.Object.FindObjectsOfType(type);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				hashSet.Add(array[i].gameObject);
			}
		}
		return hashSet;
	}

	// Token: 0x06001BB0 RID: 7088 RVA: 0x00014140 File Offset: 0x00012340
	[Obsolete("Use SetInterestGroups(byte group, bool enabled) instead.")]
	public static void SetReceivingEnabled(int group, bool enabled)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		PhotonNetwork.SetInterestGroups((byte)group, enabled);
	}

	// Token: 0x06001BB1 RID: 7089 RVA: 0x000B28CC File Offset: 0x000B0ACC
	public static void SetInterestGroups(byte group, bool enabled)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		if (enabled)
		{
			byte[] enableGroups = new byte[]
			{
				group
			};
			PhotonNetwork.networkingPeer.SetInterestGroups(null, enableGroups);
		}
		else
		{
			byte[] disableGroups = new byte[]
			{
				group
			};
			PhotonNetwork.networkingPeer.SetInterestGroups(disableGroups, null);
		}
	}

	// Token: 0x06001BB2 RID: 7090 RVA: 0x000B2920 File Offset: 0x000B0B20
	[Obsolete("Use SetInterestGroups(byte[] disableGroups, byte[] enableGroups) instead. Mind the parameter order!")]
	public static void SetReceivingEnabled(int[] enableGroups, int[] disableGroups)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		byte[] array = null;
		byte[] array2 = null;
		if (enableGroups != null)
		{
			array2 = new byte[enableGroups.Length];
			Array.Copy(enableGroups, array2, enableGroups.Length);
		}
		if (disableGroups != null)
		{
			array = new byte[disableGroups.Length];
			Array.Copy(disableGroups, array, disableGroups.Length);
		}
		PhotonNetwork.networkingPeer.SetInterestGroups(array, array2);
	}

	// Token: 0x06001BB3 RID: 7091 RVA: 0x00014155 File Offset: 0x00012355
	public static void SetInterestGroups(byte[] disableGroups, byte[] enableGroups)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		PhotonNetwork.networkingPeer.SetInterestGroups(disableGroups, enableGroups);
	}

	// Token: 0x06001BB4 RID: 7092 RVA: 0x0001416E File Offset: 0x0001236E
	[Obsolete("Use SetSendingEnabled(byte group, bool enabled). Interest Groups have a byte-typed ID. Mind the parameter order.")]
	public static void SetSendingEnabled(int group, bool enabled)
	{
		PhotonNetwork.SetSendingEnabled((byte)group, enabled);
	}

	// Token: 0x06001BB5 RID: 7093 RVA: 0x00014178 File Offset: 0x00012378
	public static void SetSendingEnabled(byte group, bool enabled)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		PhotonNetwork.networkingPeer.SetSendingEnabled(group, enabled);
	}

	// Token: 0x06001BB6 RID: 7094 RVA: 0x000B297C File Offset: 0x000B0B7C
	[Obsolete("Use SetSendingEnabled(byte group, bool enabled). Interest Groups have a byte-typed ID. Mind the parameter order.")]
	public static void SetSendingEnabled(int[] enableGroups, int[] disableGroups)
	{
		byte[] array = null;
		byte[] array2 = null;
		if (enableGroups != null)
		{
			array2 = new byte[enableGroups.Length];
			Array.Copy(enableGroups, array2, enableGroups.Length);
		}
		if (disableGroups != null)
		{
			array = new byte[disableGroups.Length];
			Array.Copy(disableGroups, array, disableGroups.Length);
		}
		PhotonNetwork.SetSendingEnabled(array, array2);
	}

	// Token: 0x06001BB7 RID: 7095 RVA: 0x00014191 File Offset: 0x00012391
	public static void SetSendingEnabled(byte[] disableGroups, byte[] enableGroups)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		PhotonNetwork.networkingPeer.SetSendingEnabled(disableGroups, enableGroups);
	}

	// Token: 0x06001BB8 RID: 7096 RVA: 0x000141AA File Offset: 0x000123AA
	public static void SetLevelPrefix(short prefix)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		PhotonNetwork.networkingPeer.SetLevelPrefix(prefix);
	}

	// Token: 0x06001BB9 RID: 7097 RVA: 0x000B29C8 File Offset: 0x000B0BC8
	public static AsyncOperation LoadLevelAsync(int levelNumber)
	{
		PhotonNetwork.networkingPeer.AsynchLevelLoadCall = true;
		if (PhotonNetwork.automaticallySyncScene)
		{
			PhotonNetwork.networkingPeer.SetLevelInPropsIfSynced(levelNumber, true, false);
		}
		PhotonNetwork.isMessageQueueRunning = false;
		PhotonNetwork.networkingPeer.loadingLevelAndPausedNetwork = true;
		return SceneManager.LoadSceneAsync(levelNumber, LoadSceneMode.Single);
	}

	// Token: 0x06001BBA RID: 7098 RVA: 0x000141C2 File Offset: 0x000123C2
	public static void LoadLevel(string levelName)
	{
		PhotonNetwork.networkingPeer.AsynchLevelLoadCall = false;
		if (PhotonNetwork.automaticallySyncScene)
		{
			PhotonNetwork.networkingPeer.SetLevelInPropsIfSynced(levelName, true, false);
		}
		PhotonNetwork.isMessageQueueRunning = false;
		PhotonNetwork.networkingPeer.loadingLevelAndPausedNetwork = true;
		LevelManager.LoadLevel(levelName);
	}

	// Token: 0x06001BBB RID: 7099 RVA: 0x000141FD File Offset: 0x000123FD
	public static AsyncOperation LoadLevelAsync(string levelName)
	{
		PhotonNetwork.networkingPeer.AsynchLevelLoadCall = true;
		if (PhotonNetwork.automaticallySyncScene)
		{
			PhotonNetwork.networkingPeer.SetLevelInPropsIfSynced(levelName, true, false);
		}
		PhotonNetwork.isMessageQueueRunning = false;
		PhotonNetwork.networkingPeer.loadingLevelAndPausedNetwork = true;
		return SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
	}

	// Token: 0x06001BBC RID: 7100 RVA: 0x00014239 File Offset: 0x00012439
	public static bool WebRpc(string name, object parameters)
	{
		return PhotonNetwork.networkingPeer.WebRpc(name, parameters);
	}

	// Token: 0x06001BBD RID: 7101 RVA: 0x00014247 File Offset: 0x00012447
	public static bool CallEvent(byte eventCode, object content, int senderId)
	{
		if (PhotonNetwork.OnEventCall != null)
		{
			PhotonNetwork.OnEventCall(eventCode, content, senderId);
			return true;
		}
		return false;
	}

	// Token: 0x04000FE0 RID: 4064
	public const string versionPUN = "1.92";

	// Token: 0x04000FE1 RID: 4065
	internal const string serverSettingsAssetFile = "PhotonServerSettings";

	// Token: 0x04000FE2 RID: 4066
	internal static readonly PhotonHandler photonMono;

	// Token: 0x04000FE3 RID: 4067
	internal static NetworkingPeer networkingPeer;

	// Token: 0x04000FE4 RID: 4068
	public static readonly int MAX_VIEW_IDS = 100;

	// Token: 0x04000FE5 RID: 4069
	public static ServerSettings PhotonServerSettings = (ServerSettings)Resources.Load("PhotonServerSettings", typeof(ServerSettings));

	// Token: 0x04000FE6 RID: 4070
	public static PhotonNetwork.VoidDelegate onConnectedToPhoton;

	// Token: 0x04000FE7 RID: 4071
	public static PhotonNetwork.VoidDelegate onLeftRoom;

	// Token: 0x04000FE8 RID: 4072
	public static PhotonNetwork.PhotonPlayerDelegate onMasterClientSwitched;

	// Token: 0x04000FE9 RID: 4073
	public static PhotonNetwork.ResponseDelegate onPhotonCreateRoomFailed;

	// Token: 0x04000FEA RID: 4074
	public static PhotonNetwork.ResponseDelegate onPhotonJoinRoomFailed;

	// Token: 0x04000FEB RID: 4075
	public static PhotonNetwork.VoidDelegate onCreatedRoom;

	// Token: 0x04000FEC RID: 4076
	public static PhotonNetwork.VoidDelegate onJoinedLobby;

	// Token: 0x04000FED RID: 4077
	public static PhotonNetwork.VoidDelegate onLeftLobby;

	// Token: 0x04000FEE RID: 4078
	public static PhotonNetwork.DisconnectCauseDelegate onFailedToConnectToPhoton;

	// Token: 0x04000FEF RID: 4079
	public static PhotonNetwork.VoidDelegate onDisconnectedFromPhoton;

	// Token: 0x04000FF0 RID: 4080
	public static PhotonNetwork.DisconnectCauseDelegate onConnectionFail;

	// Token: 0x04000FF1 RID: 4081
	public static PhotonNetwork.ObjectsDelegate onPhotonInstantiate;

	// Token: 0x04000FF2 RID: 4082
	public static PhotonNetwork.VoidDelegate onReceivedRoomListUpdate;

	// Token: 0x04000FF3 RID: 4083
	public static PhotonNetwork.VoidDelegate onJoinedRoom;

	// Token: 0x04000FF4 RID: 4084
	public static PhotonNetwork.PhotonPlayerDelegate onPhotonPlayerConnected;

	// Token: 0x04000FF5 RID: 4085
	public static PhotonNetwork.PhotonPlayerDelegate onPhotonPlayerDisconnected;

	// Token: 0x04000FF6 RID: 4086
	public static PhotonNetwork.ResponseDelegate onPhotonRandomJoinFailed;

	// Token: 0x04000FF7 RID: 4087
	public static PhotonNetwork.VoidDelegate onConnectedToMaster;

	// Token: 0x04000FF8 RID: 4088
	public static PhotonNetwork.VoidDelegate onPhotonMaxCccuReached;

	// Token: 0x04000FF9 RID: 4089
	public static PhotonNetwork.HashtableDelegate onPhotonCustomRoomPropertiesChanged;

	// Token: 0x04000FFA RID: 4090
	public static PhotonNetwork.ObjectsDelegate onPhotonPlayerPropertiesChanged;

	// Token: 0x04000FFB RID: 4091
	public static PhotonNetwork.VoidDelegate onUpdatedFriendList;

	// Token: 0x04000FFC RID: 4092
	public static PhotonNetwork.StringDelegate onCustomAuthenticationFailed;

	// Token: 0x04000FFD RID: 4093
	public static PhotonNetwork.ObjectsDelegate onCustomAuthenticationResponse;

	// Token: 0x04000FFE RID: 4094
	public static PhotonNetwork.ObjectsDelegate onWebRpcResponse;

	// Token: 0x04000FFF RID: 4095
	public static PhotonNetwork.ObjectsDelegate onOwnershipRequest;

	// Token: 0x04001000 RID: 4096
	public static PhotonNetwork.ObjectsDelegate onOwnershipTransfered;

	// Token: 0x04001001 RID: 4097
	public static PhotonNetwork.VoidDelegate onLobbyStatisticsUpdate;

	// Token: 0x04001002 RID: 4098
	public static bool InstantiateInRoomOnly = true;

	// Token: 0x04001003 RID: 4099
	public static PhotonLogLevel logLevel = PhotonLogLevel.ErrorsOnly;

	// Token: 0x04001004 RID: 4100
	public static float precisionForVectorSynchronization = 9.9E-05f;

	// Token: 0x04001005 RID: 4101
	public static float precisionForQuaternionSynchronization = 1f;

	// Token: 0x04001006 RID: 4102
	public static float precisionForFloatSynchronization = 0.01f;

	// Token: 0x04001007 RID: 4103
	public static bool UseRpcMonoBehaviourCache;

	// Token: 0x04001008 RID: 4104
	public static bool UsePrefabCache = true;

	// Token: 0x04001009 RID: 4105
	public static Dictionary<string, GameObject> PrefabCache = new Dictionary<string, GameObject>();

	// Token: 0x0400100A RID: 4106
	public static HashSet<GameObject> SendMonoMessageTargets;

	// Token: 0x0400100B RID: 4107
	public static Type SendMonoMessageTargetType = typeof(MonoBehaviour);

	// Token: 0x0400100C RID: 4108
	public static bool StartRpcsAsCoroutine = true;

	// Token: 0x0400100D RID: 4109
	private static bool isOfflineMode = false;

	// Token: 0x0400100E RID: 4110
	private static Room offlineModeRoom = null;

	// Token: 0x0400100F RID: 4111
	[Obsolete("Used for compatibility with Unity networking only.")]
	public static int maxConnections;

	// Token: 0x04001010 RID: 4112
	private static bool _mAutomaticallySyncScene = false;

	// Token: 0x04001011 RID: 4113
	private static bool m_autoCleanUpPlayerObjects = true;

	// Token: 0x04001012 RID: 4114
	private static int sendInterval = 50;

	// Token: 0x04001013 RID: 4115
	private static int sendIntervalOnSerialize = 100;

	// Token: 0x04001014 RID: 4116
	private static bool m_isMessageQueueRunning = true;

	// Token: 0x04001015 RID: 4117
	private static Stopwatch startupStopwatch;

	// Token: 0x04001016 RID: 4118
	public static float BackgroundTimeout = 60f;

	// Token: 0x04001017 RID: 4119
	internal static int lastUsedViewSubId = 0;

	// Token: 0x04001018 RID: 4120
	internal static int lastUsedViewSubIdStatic = 0;

	// Token: 0x04001019 RID: 4121
	internal static List<int> manuallyAllocatedViewIds = new List<int>();

	// Token: 0x0400101A RID: 4122
	public static bool leavingRoom = false;

	// Token: 0x020002D5 RID: 725
	// (Invoke) Token: 0x06001BC0 RID: 7104
	public delegate void VoidDelegate();

	// Token: 0x020002D6 RID: 726
	// (Invoke) Token: 0x06001BC4 RID: 7108
	public delegate void PhotonPlayerDelegate(PhotonPlayer player);

	// Token: 0x020002D7 RID: 727
	// (Invoke) Token: 0x06001BC8 RID: 7112
	public delegate void ObjectsDelegate(object[] obj);

	// Token: 0x020002D8 RID: 728
	// (Invoke) Token: 0x06001BCC RID: 7116
	public delegate void ResponseDelegate(short code, string message);

	// Token: 0x020002D9 RID: 729
	// (Invoke) Token: 0x06001BD0 RID: 7120
	public delegate void HashtableDelegate(Hashtable hashtable);

	// Token: 0x020002DA RID: 730
	// (Invoke) Token: 0x06001BD4 RID: 7124
	public delegate void StringDelegate(string text);

	// Token: 0x020002DB RID: 731
	// (Invoke) Token: 0x06001BD8 RID: 7128
	public delegate void DisconnectCauseDelegate(DisconnectCause cause);

	// Token: 0x020002DC RID: 732
	// (Invoke) Token: 0x06001BDC RID: 7132
	public delegate void EventCallback(byte eventCode, object content, int senderId);
}
