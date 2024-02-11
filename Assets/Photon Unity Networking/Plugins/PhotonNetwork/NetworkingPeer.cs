using System;
using System.Collections.Generic;
using System.Reflection;
using ExitGames.Client.Photon;
using UnityEngine;

// Token: 0x020002C2 RID: 706
internal class NetworkingPeer : LoadBalancingPeer, IPhotonPeerListener
{
	// Token: 0x06001A2C RID: 6700 RVA: 0x000AA614 File Offset: 0x000A8814
	public NetworkingPeer(string playername, ConnectionProtocol connectionProtocol) : base(connectionProtocol)
	{
		base.Listener = this;
		base.LimitOfUnreliableCommands = 40;
		this.lobby = TypedLobby.Default;
		this.PlayerName = playername;
		this.LocalPlayer = new PhotonPlayer(true, -1, this.playername);
		this.AddNewPlayer(this.LocalPlayer.ID, this.LocalPlayer);
		this.rpcShortcuts = new BetterList<string>();
		for (int i = 0; i < PhotonNetwork.PhotonServerSettings.RpcList.Count; i++)
		{
			this.rpcShortcuts.Add(PhotonNetwork.PhotonServerSettings.RpcList[i]);
		}
		this.State = ClientState.PeerCreated;
	}

	// Token: 0x17000387 RID: 903
	// (get) Token: 0x06001A2E RID: 6702 RVA: 0x000131F3 File Offset: 0x000113F3
	protected internal string AppVersion
	{
		get
		{
			return string.Format("{0}_{1}", PhotonNetwork.gameVersion, "1.92");
		}
	}

	// Token: 0x17000388 RID: 904
	// (get) Token: 0x06001A2F RID: 6703 RVA: 0x00013209 File Offset: 0x00011409
	// (set) Token: 0x06001A30 RID: 6704 RVA: 0x00013211 File Offset: 0x00011411
	public AuthenticationValues AuthValues { get; set; }

	// Token: 0x17000389 RID: 905
	// (get) Token: 0x06001A31 RID: 6705 RVA: 0x0001321A File Offset: 0x0001141A
	private string TokenForInit
	{
		get
		{
			if (this.AuthMode == AuthModeOption.Auth)
			{
				return null;
			}
			return (this.AuthValues == null) ? null : this.AuthValues.Token;
		}
	}

	// Token: 0x1700038A RID: 906
	// (get) Token: 0x06001A32 RID: 6706 RVA: 0x00013245 File Offset: 0x00011445
	// (set) Token: 0x06001A33 RID: 6707 RVA: 0x0001324D File Offset: 0x0001144D
	public bool IsUsingNameServer { get; protected internal set; }

	// Token: 0x1700038B RID: 907
	// (get) Token: 0x06001A34 RID: 6708 RVA: 0x00013256 File Offset: 0x00011456
	public string NameServerAddress
	{
		get
		{
			return this.GetNameServerAddress();
		}
	}

	// Token: 0x1700038C RID: 908
	// (get) Token: 0x06001A35 RID: 6709 RVA: 0x0001325E File Offset: 0x0001145E
	// (set) Token: 0x06001A36 RID: 6710 RVA: 0x00013266 File Offset: 0x00011466
	public string MasterServerAddress { get; protected internal set; }

	// Token: 0x1700038D RID: 909
	// (get) Token: 0x06001A37 RID: 6711 RVA: 0x0001326F File Offset: 0x0001146F
	// (set) Token: 0x06001A38 RID: 6712 RVA: 0x00013277 File Offset: 0x00011477
	public string GameServerAddress { get; protected internal set; }

	// Token: 0x1700038E RID: 910
	// (get) Token: 0x06001A39 RID: 6713 RVA: 0x00013280 File Offset: 0x00011480
	// (set) Token: 0x06001A3A RID: 6714 RVA: 0x00013288 File Offset: 0x00011488
	protected internal ServerConnection Server { get; private set; }

	// Token: 0x1700038F RID: 911
	// (get) Token: 0x06001A3B RID: 6715 RVA: 0x00013291 File Offset: 0x00011491
	// (set) Token: 0x06001A3C RID: 6716 RVA: 0x00013299 File Offset: 0x00011499
	public ClientState State { get; internal set; }

	// Token: 0x17000390 RID: 912
	// (get) Token: 0x06001A3D RID: 6717 RVA: 0x000132A2 File Offset: 0x000114A2
	// (set) Token: 0x06001A3E RID: 6718 RVA: 0x000132AA File Offset: 0x000114AA
	public TypedLobby lobby { get; set; }

	// Token: 0x17000391 RID: 913
	// (get) Token: 0x06001A3F RID: 6719 RVA: 0x000132B3 File Offset: 0x000114B3
	private bool requestLobbyStatistics
	{
		get
		{
			return PhotonNetwork.EnableLobbyStatistics && this.Server == ServerConnection.MasterServer;
		}
	}

	// Token: 0x17000392 RID: 914
	// (get) Token: 0x06001A40 RID: 6720 RVA: 0x000132CB File Offset: 0x000114CB
	// (set) Token: 0x06001A41 RID: 6721 RVA: 0x000AA89C File Offset: 0x000A8A9C
	public string PlayerName
	{
		get
		{
			return this.playername;
		}
		set
		{
			if (string.IsNullOrEmpty(value) || value.Equals(this.playername))
			{
				return;
			}
			if (this.LocalPlayer != null)
			{
				this.LocalPlayer.NickName = value;
			}
			this.playername = value;
			if (this.CurrentRoom != null)
			{
				this.SendPlayerName();
			}
		}
	}

	// Token: 0x17000393 RID: 915
	// (get) Token: 0x06001A42 RID: 6722 RVA: 0x000132D3 File Offset: 0x000114D3
	// (set) Token: 0x06001A43 RID: 6723 RVA: 0x000132F8 File Offset: 0x000114F8
	public Room CurrentRoom
	{
		get
		{
			if (this.currentRoom != null && this.currentRoom.IsLocalClientInside)
			{
				return this.currentRoom;
			}
			return null;
		}
		private set
		{
			this.currentRoom = value;
		}
	}

	// Token: 0x17000394 RID: 916
	// (get) Token: 0x06001A44 RID: 6724 RVA: 0x00013301 File Offset: 0x00011501
	// (set) Token: 0x06001A45 RID: 6725 RVA: 0x00013309 File Offset: 0x00011509
	public PhotonPlayer LocalPlayer { get; internal set; }

	// Token: 0x17000395 RID: 917
	// (get) Token: 0x06001A46 RID: 6726 RVA: 0x00013312 File Offset: 0x00011512
	// (set) Token: 0x06001A47 RID: 6727 RVA: 0x0001331A File Offset: 0x0001151A
	public int PlayersOnMasterCount { get; internal set; }

	// Token: 0x17000396 RID: 918
	// (get) Token: 0x06001A48 RID: 6728 RVA: 0x00013323 File Offset: 0x00011523
	// (set) Token: 0x06001A49 RID: 6729 RVA: 0x0001332B File Offset: 0x0001152B
	public int PlayersInRoomsCount { get; internal set; }

	// Token: 0x17000397 RID: 919
	// (get) Token: 0x06001A4A RID: 6730 RVA: 0x00013334 File Offset: 0x00011534
	// (set) Token: 0x06001A4B RID: 6731 RVA: 0x0001333C File Offset: 0x0001153C
	public int RoomsCount { get; internal set; }

	// Token: 0x17000398 RID: 920
	// (get) Token: 0x06001A4C RID: 6732 RVA: 0x00013345 File Offset: 0x00011545
	protected internal int FriendListAge
	{
		get
		{
			return (!this.isFetchingFriendList && this.friendListTimestamp != 0) ? (Environment.TickCount - this.friendListTimestamp) : 0;
		}
	}

	// Token: 0x17000399 RID: 921
	// (get) Token: 0x06001A4D RID: 6733 RVA: 0x0001336F File Offset: 0x0001156F
	public bool IsAuthorizeSecretAvailable
	{
		get
		{
			return this.AuthValues != null && !string.IsNullOrEmpty(this.AuthValues.Token);
		}
	}

	// Token: 0x1700039A RID: 922
	// (get) Token: 0x06001A4E RID: 6734 RVA: 0x00013392 File Offset: 0x00011592
	// (set) Token: 0x06001A4F RID: 6735 RVA: 0x0001339A File Offset: 0x0001159A
	public List<Region> AvailableRegions { get; protected internal set; }

	// Token: 0x1700039B RID: 923
	// (get) Token: 0x06001A50 RID: 6736 RVA: 0x000133A3 File Offset: 0x000115A3
	// (set) Token: 0x06001A51 RID: 6737 RVA: 0x000133AB File Offset: 0x000115AB
	public CloudRegionCode CloudRegion { get; protected internal set; }

	// Token: 0x1700039C RID: 924
	// (get) Token: 0x06001A52 RID: 6738 RVA: 0x000133B4 File Offset: 0x000115B4
	// (set) Token: 0x06001A53 RID: 6739 RVA: 0x000133E8 File Offset: 0x000115E8
	public int mMasterClientId
	{
		get
		{
			if (PhotonNetwork.offlineMode)
			{
				return this.LocalPlayer.ID;
			}
			return (this.CurrentRoom != null) ? this.CurrentRoom.MasterClientId : 0;
		}
		private set
		{
			if (this.CurrentRoom != null)
			{
				this.CurrentRoom.MasterClientId = value;
			}
		}
	}

	// Token: 0x06001A54 RID: 6740 RVA: 0x000AA8F8 File Offset: 0x000A8AF8
	private string GetNameServerAddress()
	{
		ConnectionProtocol transportProtocol = ConnectionProtocol.Udp;
		int num = 0;
		NetworkingPeer.ProtocolToNameServerPort.TryGetValue(transportProtocol, out num);
		string arg = string.Empty;
		if (transportProtocol == ConnectionProtocol.WebSocket)
		{
			arg = "ws://";
		}
		else if (transportProtocol == ConnectionProtocol.WebSocketSecure)
		{
			arg = "wss://";
		}
		if (PhotonNetwork.UseAlternativeUdpPorts && transportProtocol == ConnectionProtocol.Udp)
		{
			num = 27000;
		}
		return string.Format("{0}{1}:{2}", arg, "ns.exitgames.com", num);
	}

	// Token: 0x06001A55 RID: 6741 RVA: 0x00013401 File Offset: 0x00011601
	public override bool Connect(string serverAddress, string applicationName)
	{
		Debug.LogError("Avoid using this directly. Thanks.");
		return false;
	}

	// Token: 0x06001A56 RID: 6742 RVA: 0x0001340E File Offset: 0x0001160E
	public bool ReconnectToMaster()
	{
		if (this.AuthValues == null)
		{
			Debug.LogWarning("ReconnectToMaster() with AuthValues == null is not correct!");
			this.AuthValues = new AuthenticationValues();
		}
		this.AuthValues.Token = this.tokenCache;
		return this.Connect(this.MasterServerAddress, ServerConnection.MasterServer);
	}

	// Token: 0x06001A57 RID: 6743 RVA: 0x000AA974 File Offset: 0x000A8B74
	public bool ReconnectAndRejoin()
	{
		if (this.AuthValues == null)
		{
			Debug.LogWarning("ReconnectAndRejoin() with AuthValues == null is not correct!");
			this.AuthValues = new AuthenticationValues();
		}
		this.AuthValues.Token = this.tokenCache;
		if (!string.IsNullOrEmpty(this.GameServerAddress) && this.enterRoomParamsCache != null)
		{
			this.lastJoinType = JoinType.JoinRoom;
			this.enterRoomParamsCache.RejoinOnly = true;
			return this.Connect(this.GameServerAddress, ServerConnection.GameServer);
		}
		return false;
	}

	// Token: 0x06001A58 RID: 6744 RVA: 0x000AA9F0 File Offset: 0x000A8BF0
	public bool Connect(string serverAddress, ServerConnection type)
	{
		if (PhotonHandler.AppQuits)
		{
			Debug.LogWarning("Ignoring Connect() because app gets closed. If this is an error, check PhotonHandler.AppQuits.");
			return false;
		}
		if (this.State == ClientState.Disconnecting)
		{
			Debug.LogError("Connect() failed. Can't connect while disconnecting (still). Current state: " + PhotonNetwork.connectionStateDetailed);
			return false;
		}
		this.cachedServerType = type;
		this.cachedServerAddress = serverAddress;
		this.cachedApplicationName = string.Empty;
		this.SetupProtocol(type);
		bool flag = base.Connect(serverAddress, string.Empty);
		if (flag)
		{
			switch (type)
			{
			case ServerConnection.MasterServer:
				this.State = ClientState.ConnectingToMasterserver;
				break;
			case ServerConnection.GameServer:
				this.State = ClientState.ConnectingToGameserver;
				break;
			case ServerConnection.NameServer:
				this.State = ClientState.ConnectingToNameServer;
				break;
			}
		}
		return flag;
	}

	// Token: 0x06001A59 RID: 6745 RVA: 0x000AAAB4 File Offset: 0x000A8CB4
	private bool Reconnect()
	{
		this._isReconnecting = true;
		PhotonNetwork.SwitchToProtocol(PhotonNetwork.PhotonServerSettings.Protocol);
		this.SetupProtocol(this.cachedServerType);
		bool flag = base.Connect(this.cachedServerAddress, this.cachedApplicationName);
		if (flag)
		{
			switch (this.cachedServerType)
			{
			case ServerConnection.MasterServer:
				this.State = ClientState.ConnectingToMasterserver;
				break;
			case ServerConnection.GameServer:
				this.State = ClientState.ConnectingToGameserver;
				break;
			case ServerConnection.NameServer:
				this.State = ClientState.ConnectingToNameServer;
				break;
			}
		}
		return flag;
	}

	// Token: 0x06001A5A RID: 6746 RVA: 0x000AAB48 File Offset: 0x000A8D48
	public bool ConnectToNameServer()
	{
		if (PhotonHandler.AppQuits)
		{
			Debug.LogWarning("Ignoring Connect() because app gets closed. If this is an error, check PhotonHandler.AppQuits.");
			return false;
		}
		this.IsUsingNameServer = true;
		this.CloudRegion = CloudRegionCode.none;
		if (this.State == ClientState.ConnectedToNameServer)
		{
			return true;
		}
		this.SetupProtocol(ServerConnection.NameServer);
		this.cachedServerType = ServerConnection.NameServer;
		this.cachedServerAddress = this.NameServerAddress;
		this.cachedApplicationName = "ns";
		if (!base.Connect(this.NameServerAddress, "ns"))
		{
			return false;
		}
		this.State = ClientState.ConnectingToNameServer;
		return true;
	}

	// Token: 0x06001A5B RID: 6747 RVA: 0x000AABD4 File Offset: 0x000A8DD4
	public bool ConnectToRegionMaster(CloudRegionCode region)
	{
		if (PhotonHandler.AppQuits)
		{
			Debug.LogWarning("Ignoring Connect() because app gets closed. If this is an error, check PhotonHandler.AppQuits.");
			return false;
		}
		this.IsUsingNameServer = true;
		this.CloudRegion = region;
		if (this.State == ClientState.ConnectedToNameServer)
		{
			return this.CallAuthenticate();
		}
		this.cachedServerType = ServerConnection.NameServer;
		this.cachedServerAddress = this.NameServerAddress;
		this.cachedApplicationName = "ns";
		this.SetupProtocol(ServerConnection.NameServer);
		if (!base.Connect(this.NameServerAddress, "ns"))
		{
			return false;
		}
		this.State = ClientState.ConnectingToNameServer;
		return true;
	}

	// Token: 0x06001A5C RID: 6748 RVA: 0x000AAC68 File Offset: 0x000A8E68
	protected internal void SetupProtocol(ServerConnection serverType)
	{
		ConnectionProtocol connectionProtocol = ConnectionProtocol.Udp;
		if (this.AuthMode == AuthModeOption.AuthOnceWss)
		{
			if (serverType != ServerConnection.NameServer)
			{
				if (PhotonNetwork.logLevel >= PhotonLogLevel.ErrorsOnly)
				{
					Debug.LogWarning("Using PhotonServerSettings.Protocol when leaving the NameServer (AuthMode is AuthOnceWss): " + PhotonNetwork.PhotonServerSettings.Protocol);
				}
				connectionProtocol = PhotonNetwork.PhotonServerSettings.Protocol;
			}
			else
			{
				if (PhotonNetwork.logLevel >= PhotonLogLevel.ErrorsOnly)
				{
					Debug.LogWarning("Using WebSocket to connect NameServer (AuthMode is AuthOnceWss).");
				}
				connectionProtocol = ConnectionProtocol.WebSocketSecure;
			}
		}
		Type type = Type.GetType("ExitGames.Client.Photon.SocketWebTcp, Assembly-CSharp", false);
		if (type == null)
		{
			type = Type.GetType("ExitGames.Client.Photon.SocketWebTcp, Assembly-CSharp-firstpass", false);
		}
		if (type != null)
		{
			//this.SocketImplementationConfig[ConnectionProtocol.WebSocket] = type;
			//this.SocketImplementationConfig[ConnectionProtocol.WebSocketSecure] = type;
		}
		if (PhotonHandler.PingImplementation == null)
		{
			PhotonHandler.PingImplementation = typeof(PingMono);
		}
		if (ConnectionProtocol.Udp == connectionProtocol)
		{
			return;
		}
		if (PhotonNetwork.logLevel >= PhotonLogLevel.ErrorsOnly)
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				"Protocol switch from: ",
                ConnectionProtocol.Udp,
				" to: ",
				connectionProtocol,
				"."
			}));
		}
		
	}

	// Token: 0x06001A5D RID: 6749 RVA: 0x0001344E File Offset: 0x0001164E
	public override void Disconnect()
	{
		if (base.PeerState == PeerStateValue.Disconnected)
		{
			if (!PhotonHandler.AppQuits)
			{
				Debug.LogWarning(string.Format("Can't execute Disconnect() while not connected. Nothing changed. State: {0}", this.State));
			}
			return;
		}
		this.State = ClientState.Disconnecting;
		base.Disconnect();
	}

	// Token: 0x06001A5E RID: 6750 RVA: 0x000AAD98 File Offset: 0x000A8F98
	private bool CallAuthenticate()
	{
		AuthenticationValues authenticationValues;
		if ((authenticationValues = this.AuthValues) == null)
		{
			authenticationValues = new AuthenticationValues
			{
				UserId = this.PlayerName
			};
		}
		AuthenticationValues authValues = authenticationValues;
		if (this.AuthMode == AuthModeOption.Auth)
		{
			return this.OpAuthenticate(this.AppId, this.AppVersion, authValues, this.CloudRegion.ToString(), this.requestLobbyStatistics);
		}
		return this.OpAuthenticateOnce(this.AppId, this.AppVersion, authValues, this.CloudRegion.ToString(), this.EncryptionMode, PhotonNetwork.PhotonServerSettings.Protocol);
	}

	// Token: 0x06001A5F RID: 6751 RVA: 0x000AAE30 File Offset: 0x000A9030
	private void DisconnectToReconnect()
	{
		switch (this.Server)
		{
		case ServerConnection.MasterServer:
			this.State = ClientState.DisconnectingFromMasterserver;
			base.Disconnect();
			break;
		case ServerConnection.GameServer:
			this.State = ClientState.DisconnectingFromGameserver;
			base.Disconnect();
			break;
		case ServerConnection.NameServer:
			this.State = ClientState.DisconnectingFromNameServer;
			base.Disconnect();
			break;
		}
	}

	// Token: 0x06001A60 RID: 6752 RVA: 0x000AAE94 File Offset: 0x000A9094
	public bool GetRegions()
	{
		if (this.Server != ServerConnection.NameServer)
		{
			return false;
		}
		bool flag = this.OpGetRegions(this.AppId);
		if (flag)
		{
			this.AvailableRegions = null;
		}
		return flag;
	}

	// Token: 0x06001A61 RID: 6753 RVA: 0x0001348E File Offset: 0x0001168E
	public override bool OpFindFriends(string[] friendsToFind)
	{
		if (this.isFetchingFriendList)
		{
			return false;
		}
		this.friendListRequested = friendsToFind;
		this.isFetchingFriendList = true;
		return base.OpFindFriends(friendsToFind);
	}

	// Token: 0x06001A62 RID: 6754 RVA: 0x000AAECC File Offset: 0x000A90CC
	public bool OpCreateGame(EnterRoomParams enterRoomParams)
	{
		bool flag = this.Server == ServerConnection.GameServer;
		enterRoomParams.OnGameServer = flag;
		enterRoomParams.PlayerProperties = this.GetLocalActorProperties();
		if (!flag)
		{
			this.enterRoomParamsCache = enterRoomParams;
		}
		this.lastJoinType = JoinType.CreateRoom;
		return base.OpCreateRoom(enterRoomParams);
	}

	// Token: 0x06001A63 RID: 6755 RVA: 0x000AAF14 File Offset: 0x000A9114
	public override bool OpJoinRoom(EnterRoomParams opParams)
	{
		bool flag = this.Server == ServerConnection.GameServer;
		opParams.OnGameServer = flag;
		if (!flag)
		{
			this.enterRoomParamsCache = opParams;
		}
		this.lastJoinType = ((!opParams.CreateIfNotExists) ? JoinType.JoinRoom : JoinType.JoinOrCreateRoom);
		return base.OpJoinRoom(opParams);
	}

	// Token: 0x06001A64 RID: 6756 RVA: 0x000134B2 File Offset: 0x000116B2
	public override bool OpJoinRandomRoom(OpJoinRandomRoomParams opJoinRandomRoomParams)
	{
		this.enterRoomParamsCache = new EnterRoomParams();
		this.enterRoomParamsCache.Lobby = opJoinRandomRoomParams.TypedLobby;
		this.enterRoomParamsCache.ExpectedUsers = opJoinRandomRoomParams.ExpectedUsers;
		this.lastJoinType = JoinType.JoinRandomRoom;
		return base.OpJoinRandomRoom(opJoinRandomRoomParams);
	}

	// Token: 0x06001A65 RID: 6757 RVA: 0x000134EF File Offset: 0x000116EF
	public override bool OpRaiseEvent(byte eventCode, object customEventContent, bool sendReliable, RaiseEventOptions raiseEventOptions)
	{
		return !PhotonNetwork.offlineMode && base.OpRaiseEvent(eventCode, customEventContent, sendReliable, raiseEventOptions);
	}

	// Token: 0x06001A66 RID: 6758 RVA: 0x000AAF60 File Offset: 0x000A9160
	private void ReadoutProperties(Hashtable gameProperties, Hashtable pActorProperties, int targetActorNr)
	{
        // Debug.LogWarning("ReadoutProperties gameProperties: " + gameProperties.ToStringFull() + " pActorProperties: " + pActorProperties.ToStringFull() + " targetActorNr: " + targetActorNr);

        // read per-player properties (or those of one target player) and cache those locally
        if (pActorProperties != null && pActorProperties.Count > 0)
        {
            if (targetActorNr > 0)
            {
                // we have a single entry in the pActorProperties with one
                // user's name
                // targets MUST exist before you set properties
                PhotonPlayer target = this.GetPlayerWithId(targetActorNr);
                if (target != null)
                {
                    Hashtable props = this.ReadoutPropertiesForActorNr(pActorProperties, targetActorNr);
                    target.InternalCacheProperties(props);
                    SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, target, props);
                }
            }
            else
            {
                // in this case, we've got a key-value pair per actor (each
                // value is a hashtable with the actor's properties then)
                int actorNr;
                Hashtable props;
                string newName;
                PhotonPlayer target;

                foreach (object key in pActorProperties.Keys)
                {
                    actorNr = (int)key;
                    props = (Hashtable)pActorProperties[key];
                    newName = (string)props[ActorProperties.PlayerName];

                    target = this.GetPlayerWithId(actorNr);
                    if (target == null)
                    {
                        target = new PhotonPlayer(false, actorNr, newName);
                        this.AddNewPlayer(actorNr, target);
                    }

                    target.InternalCacheProperties(props);
                    SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, target, props);
                }
            }
        }

        // read game properties and cache them locally
        if (this.CurrentRoom != null && gameProperties != null)
        {
            this.CurrentRoom.InternalCacheProperties(gameProperties);
            SendMonoMessage(PhotonNetworkingMessage.OnPhotonCustomRoomPropertiesChanged, gameProperties);
            if (PhotonNetwork.automaticallySyncScene)
            {
                this.LoadLevelIfSynced();   // will load new scene if sceneName was changed
            }
        }
    }

	// Token: 0x06001A67 RID: 6759 RVA: 0x00013508 File Offset: 0x00011708
	private Hashtable ReadoutPropertiesForActorNr(Hashtable actorProperties, int actorNr)
	{
		if (actorProperties.ContainsKey(actorNr))
		{
			return (Hashtable)actorProperties[actorNr];
		}
		return actorProperties;
	}

	// Token: 0x06001A68 RID: 6760 RVA: 0x000AB0C4 File Offset: 0x000A92C4
	public void ChangeLocalID(int newID)
	{
        if (this.LocalPlayer == null)
        {
            Debug.LogWarning(string.Format("LocalPlayer is null or not in mActors! LocalPlayer: {0} mActors==null: {1} newID: {2}", this.LocalPlayer, this.mActors == null, newID));
        }

        if (this.mActors.ContainsKey(this.LocalPlayer.ID))
        {
            this.mActors.Remove(this.LocalPlayer.ID);
        }

        this.LocalPlayer.InternalChangeLocalID(newID);
        this.mActors[this.LocalPlayer.ID] = this.LocalPlayer;
        this.RebuildPlayerListCopies();
    }

	// Token: 0x06001A69 RID: 6761 RVA: 0x0001352E File Offset: 0x0001172E
	private void LeftLobbyCleanup()
	{
		this.mGameList = new Dictionary<string, RoomInfo>();
		this.mGameListCopy = new RoomInfo[0];
		if (this.insideLobby)
		{
			this.insideLobby = false;
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnLeftLobby, new object[0]);
		}
	}

	// Token: 0x06001A6A RID: 6762 RVA: 0x000AB168 File Offset: 0x000A9368
	private void LeftRoomCleanup()
	{
		bool flag = this.CurrentRoom != null;
		bool flag2 = (this.CurrentRoom == null) ? PhotonNetwork.autoCleanUpPlayerObjects : this.CurrentRoom.AutoCleanUp;
		this.hasSwitchedMC = false;
		this.CurrentRoom = null;
		this.mActors = new Dictionary<int, PhotonPlayer>();
		this.mPlayerListCopy = new PhotonPlayer[0];
		this.mOtherPlayerListCopy = new PhotonPlayer[0];
		this.allowedReceivingGroups = new HashSet<byte>();
		this.blockSendingGroups = new HashSet<byte>();
		this.mGameList = new Dictionary<string, RoomInfo>();
		this.mGameListCopy = new RoomInfo[0];
		this.isFetchingFriendList = false;
		this.ChangeLocalID(-1);
		if (flag2)
		{
			this.LocalCleanupAnythingInstantiated(true);
			PhotonNetwork.manuallyAllocatedViewIds = new List<int>();
		}
		if (flag)
		{
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnLeftRoom, new object[0]);
		}
	}

	// Token: 0x06001A6B RID: 6763 RVA: 0x000AB238 File Offset: 0x000A9438
	protected internal void LocalCleanupAnythingInstantiated(bool destroyInstantiatedGameObjects)
	{
		if (this.tempInstantiationData.Count > 0)
		{
			Debug.LogWarning("It seems some instantiation is not completed, as instantiation data is used. You should make sure instantiations are paused when calling this method. Cleaning now, despite this.");
		}
		if (destroyInstantiatedGameObjects)
		{
			HashSet<GameObject> hashSet = new HashSet<GameObject>();
			foreach (PhotonView photonView in this.photonViewList.Values)
			{
				if (photonView.isRuntimeInstantiated)
				{
					hashSet.Add(photonView.gameObject);
				}
			}
			foreach (GameObject go in hashSet)
			{
				this.RemoveInstantiatedGO(go, true);
			}
		}
		this.tempInstantiationData.Clear();
		PhotonNetwork.lastUsedViewSubId = 0;
		PhotonNetwork.lastUsedViewSubIdStatic = 0;
	}

	// Token: 0x06001A6C RID: 6764 RVA: 0x000AB330 File Offset: 0x000A9530
	private void GameEnteredOnGameServer(OperationResponse operationResponse)
	{
        if (operationResponse.ReturnCode != 0)
        {
            switch (operationResponse.OperationCode)
            {
                case OperationCode.CreateGame:
                    if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                    {
                        Debug.Log("Create failed on GameServer. Changing back to MasterServer. Msg: " + operationResponse.DebugMessage);
                    }
                    SendMonoMessage(PhotonNetworkingMessage.OnPhotonCreateRoomFailed, operationResponse.ReturnCode, operationResponse.DebugMessage);
                    break;
                case OperationCode.JoinGame:
                    if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                    {
                        Debug.Log("Join failed on GameServer. Changing back to MasterServer. Msg: " + operationResponse.DebugMessage);
                        if (operationResponse.ReturnCode == ErrorCode.GameDoesNotExist)
                        {
                            Debug.Log("Most likely the game became empty during the switch to GameServer.");
                        }
                    }
                    SendMonoMessage(PhotonNetworkingMessage.OnPhotonJoinRoomFailed, operationResponse.ReturnCode, operationResponse.DebugMessage);
                    break;
                case OperationCode.JoinRandomGame:
                    if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                    {
                        Debug.Log("Join failed on GameServer. Changing back to MasterServer. Msg: " + operationResponse.DebugMessage);
                        if (operationResponse.ReturnCode == ErrorCode.GameDoesNotExist)
                        {
                            Debug.Log("Most likely the game became empty during the switch to GameServer.");
                        }
                    }
                    SendMonoMessage(PhotonNetworkingMessage.OnPhotonRandomJoinFailed, operationResponse.ReturnCode, operationResponse.DebugMessage);
                    break;
            }

            this.DisconnectToReconnect();
            return;
        }

        Room current = new Room(this.enterRoomParamsCache.RoomName, this.enterRoomParamsCache.RoomOptions);
        current.IsLocalClientInside = true;
        this.CurrentRoom = current;

        this.State = ClientState.Joined;

        if (operationResponse.Parameters.ContainsKey(ParameterCode.ActorList))
        {
            int[] actorsInRoom = (int[])operationResponse.Parameters[ParameterCode.ActorList];
            this.UpdatedActorList(actorsInRoom);
        }

        // the local player's actor-properties are not returned in join-result. add this player to the list
        int localActorNr = (int)operationResponse[ParameterCode.ActorNr];
        this.ChangeLocalID(localActorNr);


        Hashtable actorProperties = (Hashtable)operationResponse[ParameterCode.PlayerProperties];
        Hashtable gameProperties = (Hashtable)operationResponse[ParameterCode.GameProperties];
        this.ReadoutProperties(gameProperties, actorProperties, 0);

        if (!this.CurrentRoom.serverSideMasterClient) this.CheckMasterClient(-1);

        if (this.mPlayernameHasToBeUpdated)
        {
            this.SendPlayerName();
        }

        switch (operationResponse.OperationCode)
        {
            case OperationCode.CreateGame:
                SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom);
                break;
            case OperationCode.JoinGame:
            case OperationCode.JoinRandomGame:
                // the mono message for this is sent at another place
                break;
        }
    }

	// Token: 0x06001A6D RID: 6765 RVA: 0x00013565 File Offset: 0x00011765
	private void AddNewPlayer(int ID, PhotonPlayer player)
	{
		if (!this.mActors.ContainsKey(ID))
		{
			this.mActors[ID] = player;
			this.RebuildPlayerListCopies();
		}
		else
		{
			Debug.LogError("Adding player twice: " + ID);
		}
	}

	// Token: 0x06001A6E RID: 6766 RVA: 0x000135A5 File Offset: 0x000117A5
	private void RemovePlayer(int ID, PhotonPlayer player)
	{
		this.mActors.Remove(ID);
		if (!player.IsLocal)
		{
			this.RebuildPlayerListCopies();
		}
	}

	// Token: 0x06001A6F RID: 6767 RVA: 0x000AB590 File Offset: 0x000A9790
	private void RebuildPlayerListCopies()
	{
        this.mPlayerListCopy = new PhotonPlayer[this.mActors.Count];
        this.mActors.Values.CopyTo(this.mPlayerListCopy, 0);

        List<PhotonPlayer> otherP = new List<PhotonPlayer>();
        for (int i = 0; i < this.mPlayerListCopy.Length; i++)
        {
            PhotonPlayer player = this.mPlayerListCopy[i];
            if (!player.IsLocal)
            {
                otherP.Add(player);
            }
        }

        this.mOtherPlayerListCopy = otherP.ToArray();
    }

	// Token: 0x06001A70 RID: 6768 RVA: 0x000AB610 File Offset: 0x000A9810
	private void ResetPhotonViewsOnSerialize()
	{
		foreach (PhotonView photonView in this.photonViewList.Values)
		{
			photonView.lastOnSerializeDataSent.Clear();
		}
	}

	// Token: 0x06001A71 RID: 6769 RVA: 0x000AB674 File Offset: 0x000A9874
	private void HandleEventLeave(int actorID, EventData evLeave)
	{
        if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
            Debug.Log("HandleEventLeave for player ID: " + actorID + " evLeave: " + evLeave.ToStringFull());


        // actorNr is fetched out of event
        PhotonPlayer player = this.GetPlayerWithId(actorID);
        if (player == null)
        {
            Debug.LogError(String.Format("Received event Leave for unknown player ID: {0}", actorID));
            return;
        }

        bool _isAlreadyInactive = player.IsInactive;

        if (evLeave.Parameters.ContainsKey(ParameterCode.IsInactive))
        {
            // player becomes inactive (but might return / is not gone for good)
            player.IsInactive = (bool)evLeave.Parameters[ParameterCode.IsInactive];


            if (player.IsInactive != _isAlreadyInactive)
            {
                SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerActivityChanged, player);
            }

            if (player.IsInactive && _isAlreadyInactive)
            {
                Debug.LogWarning("HandleEventLeave for player ID: " + actorID + " isInactive: " + player.IsInactive + ". Stopping handling if inactive.");
                return;
            }
        }

        // having a new master before calling destroy for the leaving player is important!
        // so we elect a new masterclient and ignore the leaving player (who is still in playerlists).
        // note: there is/was a server-side-error which sent 0 as new master instead of skipping the key/value. below is a check for 0 due to that
        if (evLeave.Parameters.ContainsKey(ParameterCode.MasterClientId))
        {
            int newMaster = (int)evLeave[ParameterCode.MasterClientId];
            if (newMaster != 0)
            {
                this.mMasterClientId = (int)evLeave[ParameterCode.MasterClientId];
                this.UpdateMasterClient();
            }
        }
        else if (!this.CurrentRoom.serverSideMasterClient)
        {
            this.CheckMasterClient(actorID);
        }


        // we let the player up if inactive but if we were already inactive, then we have to actually remove the player properly.
        if (player.IsInactive && !_isAlreadyInactive)
        {
            return;
        }

        // destroy objects & buffered messages
        if (this.CurrentRoom != null && this.CurrentRoom.AutoCleanUp)
        {
            this.DestroyPlayerObjects(actorID, true);
        }

        RemovePlayer(actorID, player);

        // finally, send notification (the playerList and masterclient are now updated)
        SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerDisconnected, player);
    }

	// Token: 0x06001A72 RID: 6770 RVA: 0x000AB83C File Offset: 0x000A9A3C
	private void CheckMasterClient(int leavingPlayerId)
	{
        bool currentMasterIsLeaving = this.mMasterClientId == leavingPlayerId;
        bool someoneIsLeaving = leavingPlayerId > 0;

        // return early if SOME player (leavingId > 0) is leaving AND it's NOT the current master
        if (someoneIsLeaving && !currentMasterIsLeaving)
        {
            return;
        }

        // picking the player with lowest ID (longest in game).
        int lowestActorNumber;
        if (this.mActors.Count <= 1)
        {
            lowestActorNumber = this.LocalPlayer.ID;
        }
        else
        {
            // keys in mActors are their actorNumbers
            lowestActorNumber = Int32.MaxValue;
            foreach (int key in this.mActors.Keys)
            {
                if (key < lowestActorNumber && key != leavingPlayerId)
                {
                    lowestActorNumber = key;
                }
            }
        }
        this.mMasterClientId = lowestActorNumber;

        // callback ONLY when the current master left
        if (someoneIsLeaving)
        {
            SendMonoMessage(PhotonNetworkingMessage.OnMasterClientSwitched, this.GetPlayerWithId(lowestActorNumber));
        }
    }

	// Token: 0x06001A73 RID: 6771 RVA: 0x000135C5 File Offset: 0x000117C5
	protected internal void UpdateMasterClient()
	{
		NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnMasterClientSwitched, new object[]
		{
			PhotonNetwork.masterClient
		});
	}

	// Token: 0x06001A74 RID: 6772 RVA: 0x000AB910 File Offset: 0x000A9B10
	private static int ReturnLowestPlayerId(PhotonPlayer[] players, int playerIdToIgnore)
	{
        if (players == null || players.Length == 0)
        {
            return -1;
        }

        int lowestActorNumber = Int32.MaxValue;
        for (int i = 0; i < players.Length; i++)
        {
            PhotonPlayer photonPlayer = players[i];
            if (photonPlayer.ID == playerIdToIgnore)
            {
                continue;
            }

            if (photonPlayer.ID < lowestActorNumber)
            {
                lowestActorNumber = photonPlayer.ID;
            }
        }

        return lowestActorNumber;
    }

	// Token: 0x06001A75 RID: 6773 RVA: 0x000AB970 File Offset: 0x000A9B70
	protected internal bool SetMasterClient(int playerId, bool sync)
	{
        bool masterReplaced = this.mMasterClientId != playerId;
        if (!masterReplaced || !this.mActors.ContainsKey(playerId))
        {
            return false;
        }

        if (sync)
        {
            bool sent = this.OpRaiseEvent(PunEvent.AssignMaster, new Hashtable() { { (byte)1, playerId } }, true, null);
            if (!sent)
            {
                return false;
            }
        }

        this.hasSwitchedMC = true;
        this.CurrentRoom.MasterClientId = playerId;
        SendMonoMessage(PhotonNetworkingMessage.OnMasterClientSwitched, this.GetPlayerWithId(playerId));    // we only callback when an actual change is done
        return true;
    }

	// Token: 0x06001A76 RID: 6774 RVA: 0x000ABA04 File Offset: 0x000A9C04
	public bool SetMasterClient(int nextMasterId)
	{
        Hashtable newProps = new Hashtable() { { GamePropertyKey.MasterClientId, nextMasterId } };
        Hashtable prevProps = new Hashtable() { { GamePropertyKey.MasterClientId, this.mMasterClientId } };
        return this.OpSetPropertiesOfRoom(newProps, expectedProperties: prevProps, webForward: false);
    }

	// Token: 0x06001A77 RID: 6775 RVA: 0x000ABA5C File Offset: 0x000A9C5C
	protected internal PhotonPlayer GetPlayerWithId(int number)
	{
        if (this.mActors == null) return null;

        PhotonPlayer player = null;
        this.mActors.TryGetValue(number, out player);
        return player;
    }

	// Token: 0x06001A78 RID: 6776 RVA: 0x000ABA88 File Offset: 0x000A9C88
	private void SendPlayerName()
	{
        if (this.State == ClientState.Joining)
        {
            // this means, the join on the gameServer is sent (with an outdated name). send the new when in game
            this.mPlayernameHasToBeUpdated = true;
            return;
        }

        if (this.LocalPlayer != null)
        {
            this.LocalPlayer.NickName = this.PlayerName;
            Hashtable properties = new Hashtable();
            properties[ActorProperties.PlayerName] = this.PlayerName;
            if (this.LocalPlayer.ID > 0)
            {
                this.OpSetPropertiesOfActor(this.LocalPlayer.ID, properties, null);
                this.mPlayernameHasToBeUpdated = false;
            }
        }
    }

	// Token: 0x06001A79 RID: 6777 RVA: 0x000ABB10 File Offset: 0x000A9D10
	private Hashtable GetLocalActorProperties()
	{
        if (PhotonNetwork.player != null)
        {
            return PhotonNetwork.player.AllProperties;
        }

        Hashtable actorProperties = new Hashtable();
        actorProperties[ActorProperties.PlayerName] = this.PlayerName;
        return actorProperties;
    }

	// Token: 0x06001A7A RID: 6778 RVA: 0x000ABB50 File Offset: 0x000A9D50
	public void DebugReturn(DebugLevel level, string message)
	{
		if (level == DebugLevel.ERROR)
		{
			Debug.LogError(message);
		}
		else if (level == DebugLevel.WARNING)
		{
			Debug.LogWarning(message);
		}
		else if (level == DebugLevel.INFO && PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
		{
			Debug.Log(message);
		}
		else if (level == DebugLevel.ALL && PhotonNetwork.logLevel == PhotonLogLevel.Full)
		{
			Debug.Log(message);
		}
	}

	// Token: 0x06001A7B RID: 6779 RVA: 0x000ABBB8 File Offset: 0x000A9DB8
	public void OnOperationResponse(OperationResponse operationResponse)
	{
        if (PhotonNetwork.networkingPeer.State == ClientState.Disconnecting)
        {
            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
            {
                Debug.Log("OperationResponse ignored while disconnecting. Code: " + operationResponse.OperationCode);
            }
            return;
        }

        // extra logging for error debugging (helping developers with a bit of automated analysis)
        if (operationResponse.ReturnCode == 0)
        {
            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                Debug.Log(operationResponse.ToString());
        }
        else
        {
            if (operationResponse.ReturnCode == ErrorCode.OperationNotAllowedInCurrentState)
            {
                Debug.LogError("Operation " + operationResponse.OperationCode + " could not be executed (yet). Wait for state JoinedLobby or ConnectedToMaster and their callbacks before calling operations. WebRPCs need a server-side configuration. Enum OperationCode helps identify the operation.");
            }
            else if (operationResponse.ReturnCode == ErrorCode.PluginReportedError)
            {
                Debug.LogError("Operation " + operationResponse.OperationCode + " failed in a server-side plugin. Check the configuration in the Dashboard. Message from server-plugin: " + operationResponse.DebugMessage);
            }
            else if (operationResponse.ReturnCode == ErrorCode.NoRandomMatchFound)
            {
                Debug.LogWarning("Operation failed: " + operationResponse.ToStringFull());
            }
            else
            {
                Debug.LogError("Operation failed: " + operationResponse.ToStringFull() + " Server: " + this.Server);
            }
        }

        // use the "secret" or "token" whenever we get it. doesn't really matter if it's in AuthResponse.
        if (operationResponse.Parameters.ContainsKey(ParameterCode.Secret))
        {
            if (this.AuthValues == null)
            {
                this.AuthValues = new AuthenticationValues();
                // this.DebugReturn(DebugLevel.ERROR, "Server returned secret. Created AuthValues.");
            }

            this.AuthValues.Token = operationResponse[ParameterCode.Secret] as string;
            this.tokenCache = this.AuthValues.Token;
        }

        switch (operationResponse.OperationCode)
        {
            case OperationCode.Authenticate:
            case OperationCode.AuthenticateOnce:
                {
                    // ClientState oldState = this.State;

                    if (operationResponse.ReturnCode != 0)
                    {
                        if (operationResponse.ReturnCode == ErrorCode.InvalidOperation)
                        {
                            Debug.LogError(string.Format("If you host Photon yourself, make sure to start the 'Instance LoadBalancing' " + this.ServerAddress));
                        }
                        else if (operationResponse.ReturnCode == ErrorCode.InvalidAuthentication)
                        {
                            Debug.LogError(string.Format("The appId this client sent is unknown on the server (Cloud). Check settings. If using the Cloud, check account."));
                            SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, DisconnectCause.InvalidAuthentication);
                        }
                        else if (operationResponse.ReturnCode == ErrorCode.CustomAuthenticationFailed)
                        {
                            Debug.LogError(string.Format("Custom Authentication failed (either due to user-input or configuration or AuthParameter string format). Calling: OnCustomAuthenticationFailed()"));
                            SendMonoMessage(PhotonNetworkingMessage.OnCustomAuthenticationFailed, operationResponse.DebugMessage);
                        }
                        else
                        {
                            Debug.LogError(string.Format("Authentication failed: '{0}' Code: {1}", operationResponse.DebugMessage, operationResponse.ReturnCode));
                        }

                        this.State = ClientState.Disconnecting;
                        this.Disconnect();

                        if (operationResponse.ReturnCode == ErrorCode.MaxCcuReached)
                        {
                            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                                Debug.LogWarning(string.Format("Currently, the limit of users is reached for this title. Try again later. Disconnecting"));
                            SendMonoMessage(PhotonNetworkingMessage.OnPhotonMaxCccuReached);
                            SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, DisconnectCause.MaxCcuReached);
                        }
                        else if (operationResponse.ReturnCode == ErrorCode.InvalidRegion)
                        {
                            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                                Debug.LogError(string.Format("The used master server address is not available with the subscription currently used. Got to Photon Cloud Dashboard or change URL. Disconnecting."));
                            SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, DisconnectCause.InvalidRegion);
                        }
                        else if (operationResponse.ReturnCode == ErrorCode.AuthenticationTicketExpired)
                        {
                            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                                Debug.LogError(string.Format("The authentication ticket expired. You need to connect (and authenticate) again. Disconnecting."));
                            SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, DisconnectCause.AuthenticationTicketExpired);
                        }
                        break;
                    }
                    else
                    {
                        // successful connect/auth. depending on the used server, do next steps:

                        if (this.Server == ServerConnection.NameServer || this.Server == ServerConnection.MasterServer)
                        {
                            if (operationResponse.Parameters.ContainsKey(ParameterCode.UserId))
                            {
                                string incomingId = (string)operationResponse.Parameters[ParameterCode.UserId];
                                if (!string.IsNullOrEmpty(incomingId))
                                {
                                    if (this.AuthValues == null)
                                    {
                                        this.AuthValues = new AuthenticationValues();
                                    }
                                    this.AuthValues.UserId = incomingId;
                                    PhotonNetwork.player.UserId = incomingId;

                                    if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                                    {
                                        this.DebugReturn(DebugLevel.INFO, string.Format("Received your UserID from server. Updating local value to: {0}", incomingId));
                                    }
                                }
                            }
                            if (operationResponse.Parameters.ContainsKey(ParameterCode.NickName))
                            {
                                this.PlayerName = (string)operationResponse.Parameters[ParameterCode.NickName];
                                if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                                {
                                    this.DebugReturn(DebugLevel.INFO, string.Format("Received your NickName from server. Updating local value to: {0}", this.playername));
                                }
                            }

                            if (operationResponse.Parameters.ContainsKey(ParameterCode.EncryptionData))
                            {
                                this.SetupEncryption((Dictionary<byte, object>)operationResponse.Parameters[ParameterCode.EncryptionData]);
                            }
                        }

                        if (this.Server == ServerConnection.NameServer)
                        {
                            // on the NameServer, authenticate returns the MasterServer address for a region and we hop off to there
                            this.MasterServerAddress = operationResponse[ParameterCode.Address] as string;
                            if (PhotonNetwork.UseAlternativeUdpPorts && this.UsedProtocol == ConnectionProtocol.Udp)
                            {
                                this.MasterServerAddress = this.MasterServerAddress.Replace("5058", "27000").Replace("5055", "27001").Replace("5056", "27002");
                            }
                            this.DisconnectToReconnect();
                        }
                        else if (this.Server == ServerConnection.MasterServer)
                        {
                            if (this.AuthMode != AuthModeOption.Auth)
                            {
                                this.OpSettings(this.requestLobbyStatistics);
                            }
                            if (PhotonNetwork.autoJoinLobby)
                            {
                                this.State = ClientState.Authenticated;
                                this.OpJoinLobby(this.lobby);
                            }
                            else
                            {
                                this.State = ClientState.ConnectedToMaster;
                                SendMonoMessage(PhotonNetworkingMessage.OnConnectedToMaster);
                            }
                        }
                        else if (this.Server == ServerConnection.GameServer)
                        {
                            this.State = ClientState.Joining;
                            this.enterRoomParamsCache.PlayerProperties = GetLocalActorProperties();
                            this.enterRoomParamsCache.OnGameServer = true;

                            if (this.lastJoinType == JoinType.JoinRoom || this.lastJoinType == JoinType.JoinRandomRoom || this.lastJoinType == JoinType.JoinOrCreateRoom)
                            {
                                // if we just "join" the game, do so. if we wanted to "create the room on demand", we have to send this to the game server as well.
                                this.OpJoinRoom(this.enterRoomParamsCache);
                            }
                            else if (this.lastJoinType == JoinType.CreateRoom)
                            {
                                this.OpCreateGame(this.enterRoomParamsCache);
                            }
                        }

                        if (operationResponse.Parameters.ContainsKey(ParameterCode.Data))
                        {
                            // optionally, OpAuth may return some data for the client to use. if it's available, call OnCustomAuthenticationResponse
                            Dictionary<string, object> data = (Dictionary<string, object>)operationResponse.Parameters[ParameterCode.Data];
                            if (data != null)
                            {
                                SendMonoMessage(PhotonNetworkingMessage.OnCustomAuthenticationResponse, data);
                            }
                        }
                    }
                    break;
                }

            case OperationCode.GetRegions:
                // Debug.Log("GetRegions returned: " + operationResponse.ToStringFull());

                if (operationResponse.ReturnCode == ErrorCode.InvalidAuthentication)
                {
                    Debug.LogError(string.Format("The appId this client sent is unknown on the server (Cloud). Check settings. If using the Cloud, check account."));
                    SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, DisconnectCause.InvalidAuthentication);

                    this.State = ClientState.Disconnecting;
                    this.Disconnect();
                    break;
                }
                if (operationResponse.ReturnCode != ErrorCode.Ok)
                {
                    Debug.LogError("GetRegions failed. Can't provide regions list. Error: " + operationResponse.ReturnCode + ": " + operationResponse.DebugMessage);
                    break;
                }

                string[] regions = operationResponse[ParameterCode.Region] as string[];
                string[] servers = operationResponse[ParameterCode.Address] as string[];
                if (regions == null || servers == null || regions.Length != servers.Length)
                {
                    Debug.LogError("The region arrays from Name Server are not ok. Must be non-null and same length. " + (regions == null) + " " + (servers == null) + "\n" + operationResponse.ToStringFull());
                    break;
                }

                this.AvailableRegions = new List<Region>(regions.Length);
                for (int i = 0; i < regions.Length; i++)
                {
                    string regionCodeString = regions[i];
                    if (string.IsNullOrEmpty(regionCodeString))
                    {
                        continue;
                    }
                    regionCodeString = regionCodeString.ToLower();
                    CloudRegionCode code = Region.Parse(regionCodeString);

                    // check if enabled (or ignored by PhotonServerSettings.EnabledRegions)
                    bool enabledRegion = true;
                    if (PhotonNetwork.PhotonServerSettings.HostType == ServerSettings.HostingOption.BestRegion && PhotonNetwork.PhotonServerSettings.EnabledRegions != 0)
                    {
                        CloudRegionFlag flag = Region.ParseFlag(code);
                        enabledRegion = ((PhotonNetwork.PhotonServerSettings.EnabledRegions & flag) != 0);
                        if (!enabledRegion && PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                        {
                            Debug.Log("Skipping region because it's not in PhotonServerSettings.EnabledRegions: " + code);
                        }
                    }
                    if (enabledRegion)
                    {
                        this.AvailableRegions.Add(new Region(code, regionCodeString, servers[i]));
                    }
                }

                // PUN assumes you fetch the name-server's list of regions to ping them
                if (PhotonNetwork.PhotonServerSettings.HostType == ServerSettings.HostingOption.BestRegion)
                {

                    CloudRegionCode bestFromPrefs = PhotonHandler.BestRegionCodeInPreferences;
                    if (bestFromPrefs != CloudRegionCode.none &&
                    this.AvailableRegions.Exists(x => x.Code == bestFromPrefs)
                    )
                    {
                        Debug.Log("Best region found in PlayerPrefs. Connecting to: " + bestFromPrefs);
                        if (!this.ConnectToRegionMaster(bestFromPrefs))
                        {
                            PhotonHandler.PingAvailableRegionsAndConnectToBest();
                        }
                    }
                    else
                    {

                        PhotonHandler.PingAvailableRegionsAndConnectToBest();
                    }
                }
                break;

            case OperationCode.CreateGame:
                {
                    if (this.Server == ServerConnection.GameServer)
                    {
                        this.GameEnteredOnGameServer(operationResponse);
                    }
                    else
                    {
                        if (operationResponse.ReturnCode != 0)
                        {
                            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                                Debug.LogWarning(string.Format("CreateRoom failed, client stays on masterserver: {0}.", operationResponse.ToStringFull()));

                            this.State = (this.insideLobby) ? ClientState.JoinedLobby : ClientState.ConnectedToMaster;
                            SendMonoMessage(PhotonNetworkingMessage.OnPhotonCreateRoomFailed, operationResponse.ReturnCode, operationResponse.DebugMessage);
                            break;
                        }

                        string gameID = (string)operationResponse[ParameterCode.RoomName];
                        if (!string.IsNullOrEmpty(gameID))
                        {
                            // is only sent by the server's response, if it has not been
                            // sent with the client's request before!
                            this.enterRoomParamsCache.RoomName = gameID;
                        }

                        this.GameServerAddress = (string)operationResponse[ParameterCode.Address];
                        if (PhotonNetwork.UseAlternativeUdpPorts && this.UsedProtocol == ConnectionProtocol.Udp)
                        {
                            this.GameServerAddress = this.GameServerAddress.Replace("5058", "27000").Replace("5055", "27001").Replace("5056", "27002");
                        }
                        this.DisconnectToReconnect();
                    }

                    break;
                }

            case OperationCode.JoinGame:
                {
                    if (this.Server != ServerConnection.GameServer)
                    {
                        if (operationResponse.ReturnCode != 0)
                        {
                            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                                Debug.Log(string.Format("JoinRoom failed (room maybe closed by now). Client stays on masterserver: {0}. State: {1}", operationResponse.ToStringFull(), this.State));

                            SendMonoMessage(PhotonNetworkingMessage.OnPhotonJoinRoomFailed, operationResponse.ReturnCode, operationResponse.DebugMessage);
                            break;
                        }

                        this.GameServerAddress = (string)operationResponse[ParameterCode.Address];
                        if (PhotonNetwork.UseAlternativeUdpPorts && this.UsedProtocol == ConnectionProtocol.Udp)
                        {
                            this.GameServerAddress = this.GameServerAddress.Replace("5058", "27000").Replace("5055", "27001").Replace("5056", "27002");
                        }
                        this.DisconnectToReconnect();
                    }
                    else
                    {
                        this.GameEnteredOnGameServer(operationResponse);
                    }

                    break;
                }

            case OperationCode.JoinRandomGame:
                {
                    // happens only on master. on gameserver, this is a regular join (we don't need to find a random game again)
                    // the operation OpJoinRandom either fails (with returncode 8) or returns game-to-join information
                    if (operationResponse.ReturnCode != 0)
                    {
                        if (operationResponse.ReturnCode == ErrorCode.NoRandomMatchFound)
                        {
                            if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
                                Debug.Log("JoinRandom failed: No open game. Calling: OnPhotonRandomJoinFailed() and staying on master server.");
                        }
                        else if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                        {
                            Debug.LogWarning(string.Format("JoinRandom failed: {0}.", operationResponse.ToStringFull()));
                        }

                        SendMonoMessage(PhotonNetworkingMessage.OnPhotonRandomJoinFailed, operationResponse.ReturnCode, operationResponse.DebugMessage);
                        break;
                    }

                    string roomName = (string)operationResponse[ParameterCode.RoomName];
                    this.enterRoomParamsCache.RoomName = roomName;
                    this.GameServerAddress = (string)operationResponse[ParameterCode.Address];
                    if (PhotonNetwork.UseAlternativeUdpPorts && this.UsedProtocol == ConnectionProtocol.Udp)
                    {
                        this.GameServerAddress = this.GameServerAddress.Replace("5058", "27000").Replace("5055", "27001").Replace("5056", "27002");
                    }
                    this.DisconnectToReconnect();
                    break;
                }

            case OperationCode.GetGameList:
                if (operationResponse.ReturnCode != 0)
                {
                    this.DebugReturn(DebugLevel.ERROR, "GetGameList failed: " + operationResponse.ToStringFull());
                    break;
                }

                this.mGameList = new Dictionary<string, RoomInfo>();
                Hashtable games = (Hashtable)operationResponse[ParameterCode.GameList];
                foreach (var gameKey in games.Keys)
                {
                    string gameName = (string)gameKey;
                    this.mGameList[gameName] = new RoomInfo(gameName, (Hashtable)games[gameKey]);
                }
                mGameListCopy = new RoomInfo[mGameList.Count];
                mGameList.Values.CopyTo(mGameListCopy, 0);
                SendMonoMessage(PhotonNetworkingMessage.OnReceivedRoomListUpdate);
                break;

            case OperationCode.JoinLobby:
                this.State = ClientState.JoinedLobby;
                this.insideLobby = true;
                SendMonoMessage(PhotonNetworkingMessage.OnJoinedLobby);

                // this.mListener.joinLobbyReturn();
                break;
            case OperationCode.LeaveLobby:
                this.State = ClientState.Authenticated;
                this.LeftLobbyCleanup();    // will set insideLobby = false
                break;

            case OperationCode.Leave:
                this.DisconnectToReconnect();
                break;

            case OperationCode.SetProperties:
                // this.mListener.setPropertiesReturn(returnCode, debugMsg);
                break;

            case OperationCode.GetProperties:
                {
                    Hashtable actorProperties = (Hashtable)operationResponse[ParameterCode.PlayerProperties];
                    Hashtable gameProperties = (Hashtable)operationResponse[ParameterCode.GameProperties];
                    this.ReadoutProperties(gameProperties, actorProperties, 0);

                    // RemoveByteTypedPropertyKeys(actorProperties, false);
                    // RemoveByteTypedPropertyKeys(gameProperties, false);
                    // this.mListener.getPropertiesReturn(gameProperties, actorProperties, returnCode, debugMsg);
                    break;
                }

            case OperationCode.RaiseEvent:
                // this usually doesn't give us a result. only if the caching is affected the server will send one.
                break;

            case OperationCode.FindFriends:
                bool[] onlineList = operationResponse[ParameterCode.FindFriendsResponseOnlineList] as bool[];
                string[] roomList = operationResponse[ParameterCode.FindFriendsResponseRoomIdList] as string[];

                if (onlineList != null && roomList != null && this.friendListRequested != null && onlineList.Length == this.friendListRequested.Length)
                {
                    List<FriendInfo> friendList = new List<FriendInfo>(this.friendListRequested.Length);
                    for (int index = 0; index < this.friendListRequested.Length; index++)
                    {
                        FriendInfo friend = new FriendInfo();
                        friend.UserId = this.friendListRequested[index];
                        friend.Room = roomList[index];
                        friend.IsOnline = onlineList[index];
                        friendList.Insert(index, friend);
                    }
                    PhotonNetwork.Friends = friendList;
                }
                else
                {
                    // any of the lists is null and shouldn't. print a error
                    Debug.LogError("FindFriends failed to apply the result, as a required value wasn't provided or the friend list length differed from result.");
                }

                this.friendListRequested = null;
                this.isFetchingFriendList = false;
                this.friendListTimestamp = Environment.TickCount;
                if (this.friendListTimestamp == 0)
                {
                    this.friendListTimestamp = 1;   // makes sure the timestamp is not accidentally 0
                }

                SendMonoMessage(PhotonNetworkingMessage.OnUpdatedFriendList);
                break;

            case OperationCode.WebRpc:
                SendMonoMessage(PhotonNetworkingMessage.OnWebRpcResponse, operationResponse);
                break;

            default:
                Debug.LogWarning(string.Format("OperationResponse unhandled: {0}", operationResponse.ToString()));
                break;
        }

        //this.externalListener.OnOperationResponse(operationResponse);
    }

    // Token: 0x06001A7C RID: 6780 RVA: 0x000AC8E0 File Offset: 0x000AAAE0
    public void OnStatusChanged(StatusCode statusCode)
	{
        if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
            Debug.Log(string.Format("OnStatusChanged: {0} current State: {1}", statusCode.ToString(), this.State));

        switch (statusCode)
        {
            case StatusCode.Connect:
                if (this.State == ClientState.ConnectingToNameServer)
                {
                    if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
                        Debug.Log("Connected to NameServer.");

                    this.Server = ServerConnection.NameServer;
                    if (this.AuthValues != null)
                    {
                        this.AuthValues.Token = null;     // when connecting to NameServer, invalidate any auth values
                    }
                }

                if (this.State == ClientState.ConnectingToGameserver)
                {
                    if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
                        Debug.Log("Connected to gameserver.");

                    this.Server = ServerConnection.GameServer;
                    this.State = ClientState.ConnectedToGameserver;
                }

                if (this.State == ClientState.ConnectingToMasterserver)
                {
                    if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
                        Debug.Log("Connected to masterserver.");

                    this.Server = ServerConnection.MasterServer;
                    this.State = ClientState.Authenticating;  // photon v4 always requires OpAuthenticate. even self-hosted Photon Server

                    if (this.IsInitialConnect)
                    {
                        this.IsInitialConnect = false;  // after handling potential initial-connect issues with special messages, we are now sure we can reach a server
                        SendMonoMessage(PhotonNetworkingMessage.OnConnectedToPhoton);
                    }
                }


                if (this.UsedProtocol != ConnectionProtocol.WebSocketSecure)
                {
                    if (this.Server == ServerConnection.NameServer || this.AuthMode == AuthModeOption.Auth)
                    {
                        if (!PhotonNetwork.offlineMode)
                            this.EstablishEncryption();
                    }
                }
                else
                {
                    if (DebugOut == DebugLevel.INFO)
                    {
                        Debug.Log("Skipping EstablishEncryption. Protocol is secure.");
                    }

                    goto case StatusCode.EncryptionEstablished;
                }
                break;

            case StatusCode.EncryptionEstablished:

                // reset flags
                _isReconnecting = false;

                // on nameserver, the "process" is stopped here, so the developer/game can either get regions or authenticate with a specific region
                if (this.Server == ServerConnection.NameServer)
                {
                    this.State = ClientState.ConnectedToNameServer;

                    if (!this.didAuthenticate && this.CloudRegion == CloudRegionCode.none)
                    {
                        // this client is not setup to connect to a default region. find out which regions there are!
                        this.OpGetRegions(this.AppId);
                    }
                }

                if (this.Server != ServerConnection.NameServer && (this.AuthMode == AuthModeOption.AuthOnce || this.AuthMode == AuthModeOption.AuthOnceWss))
                {
                    // AuthMode "Once" means we only authenticate on the NameServer
                    Debug.Log("didAuthenticate " + didAuthenticate + " AuthMode " + AuthMode);
                    break;
                }


                // we might need to authenticate automatically now, so the client can do anything at all
                if (!this.didAuthenticate && (!this.IsUsingNameServer || this.CloudRegion != CloudRegionCode.none))
                {
                    this.didAuthenticate = this.CallAuthenticate();

                    if (this.didAuthenticate)
                    {
                        this.State = ClientState.Authenticating;
                    }
                }
                break;

            case StatusCode.EncryptionFailedToEstablish:
                Debug.LogError("Encryption wasn't established: " + statusCode + ". Going to authenticate anyways.");
                AuthenticationValues authV = this.AuthValues ?? new AuthenticationValues() { UserId = this.PlayerName };
                this.OpAuthenticate(this.AppId, this.AppVersion, authV, this.CloudRegion.ToString(), this.requestLobbyStatistics);     // TODO: check if there are alternatives
                break;

            case StatusCode.Disconnect:
                this.didAuthenticate = false;
                this.isFetchingFriendList = false;
                if (this.Server == ServerConnection.GameServer) this.LeftRoomCleanup();
                if (this.Server == ServerConnection.MasterServer) this.LeftLobbyCleanup();

                if (this.State == ClientState.DisconnectingFromMasterserver)
                {
                    if (this.Connect(this.GameServerAddress, ServerConnection.GameServer))
                    {
                        this.State = ClientState.ConnectingToGameserver;
                    }
                }
                else if (this.State == ClientState.DisconnectingFromGameserver || this.State == ClientState.DisconnectingFromNameServer)
                {
                    this.SetupProtocol(ServerConnection.MasterServer);
                    if (this.Connect(this.MasterServerAddress, ServerConnection.MasterServer))
                    {
                        this.State = ClientState.ConnectingToMasterserver;
                    }
                }
                else
                {
                    if (_isReconnecting)
                    {
                        return;
                    }

                    if (this.AuthValues != null)
                    {
                        this.AuthValues.Token = null;       // invalidate any custom auth secrets
                    }

                    this.IsInitialConnect = false;          // not "connecting" anymore
                    this.State = ClientState.PeerCreated;   // if we set another state here, we could keep clients from connecting in OnDisconnectedFromPhoton right here.
                    SendMonoMessage(PhotonNetworkingMessage.OnDisconnectedFromPhoton);
                }
                break;

            case StatusCode.ExceptionOnConnect:
            case StatusCode.SecurityExceptionOnConnect:
                this.IsInitialConnect = false;

                this.State = ClientState.PeerCreated;
                if (this.AuthValues != null)
                {
                    this.AuthValues.Token = null;  // invalidate any custom auth secrets
                }

                DisconnectCause cause = (DisconnectCause)statusCode;
                SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, cause);
                break;

            case StatusCode.Exception:
                if (this.IsInitialConnect)
                {
                    Debug.LogError("Exception while connecting to: " + this.ServerAddress + ". Check if the server is available.");
                    if (this.ServerAddress == null || this.ServerAddress.StartsWith("127.0.0.1"))
                    {
                        Debug.LogWarning("The server address is 127.0.0.1 (localhost): Make sure the server is running on this machine. Android and iOS emulators have their own localhost.");
                        if (this.ServerAddress == this.GameServerAddress)
                        {
                            Debug.LogWarning("This might be a misconfiguration in the game server config. You need to edit it to a (public) address.");
                        }
                    }

                    this.State = ClientState.PeerCreated;
                    cause = (DisconnectCause)statusCode;
                    this.IsInitialConnect = false;
                    SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, cause);
                }
                else
                {
                    this.State = ClientState.PeerCreated;

                    cause = (DisconnectCause)statusCode;
                    SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, cause);
                }

                this.Disconnect();
                break;

            case StatusCode.TimeoutDisconnect:
                if (this.IsInitialConnect)
                {
                    // UNITY_IOS || UNITY_EDITOR
#if FALSE
                    if (this.UsedProtocol == ConnectionProtocol.Udp)
                    {

                        Debug.LogWarning("UDP Connection timed out, Reconnecting using TCP");

                        PhotonNetwork.PhotonServerSettings.Protocol = ConnectionProtocol.Tcp;

                        // replace port in cached address:
                        // This is limited... The Photon ServerSettings only has one field for serverPort,
                        // so we can't know if the TCP port would have had a different port on a custom server
                        if (this.cachedServerAddress.Contains(":"))
                        {
                            this.cachedServerAddress = this.cachedServerAddress.Split(':')[0];
                            this.cachedServerAddress += ":4530";
                        }

                        this.Reconnect();
                        return;
                    }
#endif

                    if (!_isReconnecting)
                    {
                        Debug.LogWarning(statusCode + " while connecting to: " + this.ServerAddress + ". Check if the server is available.");

                        this.IsInitialConnect = false;
                        cause = (DisconnectCause)statusCode;
                        SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, cause);
                    }
                }
                else
                {
                    cause = (DisconnectCause)statusCode;
                    SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, cause);
                }
                if (this.AuthValues != null)
                {
                    this.AuthValues.Token = null;  // invalidate any custom auth secrets
                }

                /* JF: we need this when reconnecting and joining.
                if (this.ServerAddress.Equals(this.GameServerAddress))
                {
                    this.GameServerAddress = null;
                }
                if (this.ServerAddress.Equals(this.MasterServerAddress))
                {
                    this.ServerAddress = null;
                }
                */

                this.Disconnect();
                break;

            case StatusCode.ExceptionOnReceive:
            case StatusCode.DisconnectByServer:
            case StatusCode.DisconnectByServerLogic:
            case StatusCode.DisconnectByServerUserLimit:
                if (this.IsInitialConnect)
                {
                    Debug.LogWarning(statusCode + " while connecting to: " + this.ServerAddress + ". Check if the server is available.");

                    this.IsInitialConnect = false;
                    cause = (DisconnectCause)statusCode;
                    SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, cause);
                }
                else
                {
                    cause = (DisconnectCause)statusCode;
                    SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, cause);
                }
                if (this.AuthValues != null)
                {
                    this.AuthValues.Token = null;  // invalidate any custom auth secrets
                }

                this.Disconnect();
                break;

            case StatusCode.SendError:
                // this.mListener.clientErrorReturn(statusCode);
                break;

            //case StatusCode.QueueOutgoingReliableWarning:
            //case StatusCode.QueueOutgoingUnreliableWarning:
            //case StatusCode.QueueOutgoingAcksWarning:
            //case StatusCode.QueueSentWarning:
            //    // this.mListener.warningReturn(statusCode);
            //    break;

            //case StatusCode.QueueIncomingReliableWarning:
            //case StatusCode.QueueIncomingUnreliableWarning:
            //    Debug.Log(statusCode + ". This client buffers many incoming messages. This is OK temporarily. With lots of these warnings, check if you send too much or execute messages too slow. " + (PhotonNetwork.isMessageQueueRunning? "":"Your isMessageQueueRunning is false. This can cause the issue temporarily.") );
            //    break;

            // // TCP "routing" is an option of Photon that's not currently needed (or supported) by PUN
            //case StatusCode.TcpRouterResponseOk:
            //    break;
            //case StatusCode.TcpRouterResponseEndpointUnknown:
            //case StatusCode.TcpRouterResponseNodeIdUnknown:
            //case StatusCode.TcpRouterResponseNodeNotReady:

            //    this.DebugReturn(DebugLevel.ERROR, "Unexpected router response: " + statusCode);
            //    break;

            default:

                // this.mListener.serverErrorReturn(statusCode.value());
                Debug.LogError("Received unknown status code: " + statusCode);
                break;
        }

        //this.externalListener.OnStatusChanged(statusCode);
    }

    // Token: 0x06001A7D RID: 6781 RVA: 0x000ACF3C File Offset: 0x000AB13C
    public void OnEvent(EventData photonEvent)
	{
        if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
            Debug.Log(string.Format("OnEvent: {0}", photonEvent.ToString()));

        int actorNr = -1;
        PhotonPlayer originatingPlayer = null;

        if (photonEvent.Parameters.ContainsKey(ParameterCode.ActorNr))
        {
            actorNr = (int)photonEvent[ParameterCode.ActorNr];
            originatingPlayer = this.GetPlayerWithId(actorNr);

            //else
            //{
            //    // the actor sending this event is not in actorlist. this is usually no problem
            //    if (photonEvent.Code != (byte)LiteOpCode.Join)
            //    {
            //        Debug.LogWarning("Received event, but we do not have this actor:  " + actorNr);
            //    }
            //}
        }

        switch (photonEvent.Code)
        {
            case PunEvent.OwnershipRequest:
                {
                    int[] requestValues = (int[])photonEvent.Parameters[ParameterCode.CustomEventContent];
                    int requestedViewId = requestValues[0];
                    int currentOwner = requestValues[1];


                    PhotonView requestedView = PhotonView.Find(requestedViewId);
                    if (requestedView == null)
                    {
                        Debug.LogWarning("Can't find PhotonView of incoming OwnershipRequest. ViewId not found: " + requestedViewId);
                        break;
                    }

                    if (PhotonNetwork.logLevel == PhotonLogLevel.Informational)
                        Debug.Log("Ev OwnershipRequest " + requestedView.ownershipTransfer + ". ActorNr: " + actorNr + " takes from: " + currentOwner + ". local RequestedView.ownerId: " + requestedView.ownerId + " isOwnerActive: " + requestedView.isOwnerActive + ". MasterClient: " + this.mMasterClientId + ". This client's player: " + PhotonNetwork.player.ToStringFull());

                    switch (requestedView.ownershipTransfer)
                    {
                        case OwnershipOption.Fixed:
                            Debug.LogWarning("Ownership mode == fixed. Ignoring request.");
                            break;
                        case OwnershipOption.Takeover:
                            if (currentOwner == requestedView.ownerId || (currentOwner == 0 && requestedView.ownerId == this.mMasterClientId) || requestedView.ownerId == 0)
                            {
                                // a takeover is successful automatically, if taken from current owner
                                requestedView.OwnerShipWasTransfered = true;
                                int _oldOwnerId = requestedView.ownerId;
                                PhotonPlayer _oldOwner = this.GetPlayerWithId(_oldOwnerId);

                                requestedView.ownerId = actorNr;


                                if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                                {
                                    Debug.LogWarning(requestedView + " ownership transfered to: " + actorNr);
                                }

                                SendMonoMessage(PhotonNetworkingMessage.OnOwnershipTransfered, new object[] { requestedView, originatingPlayer, _oldOwner });

                            }
                            break;
                        case OwnershipOption.Request:
                            if (currentOwner == PhotonNetwork.player.ID || PhotonNetwork.player.IsMasterClient)
                            {
                                if ((requestedView.ownerId == PhotonNetwork.player.ID) || (PhotonNetwork.player.IsMasterClient && !requestedView.isOwnerActive))
                                {
                                    SendMonoMessage(PhotonNetworkingMessage.OnOwnershipRequest, new object[] { requestedView, originatingPlayer });
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                break;

            case PunEvent.OwnershipTransfer:
                {
                    int[] transferViewToUserID = (int[])photonEvent.Parameters[ParameterCode.CustomEventContent];

                    if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                    {
                        Debug.Log("Ev OwnershipTransfer. ViewID " + transferViewToUserID[0] + " to: " + transferViewToUserID[1] + " Time: " + Environment.TickCount % 1000);
                    }



                    int requestedViewId = transferViewToUserID[0];
                    int newOwnerId = transferViewToUserID[1];

                    PhotonView pv = PhotonView.Find(requestedViewId);
                    if (pv != null)
                    {
                        int _oldOwnerID = pv.ownerId;
                        pv.OwnerShipWasTransfered = true;
                        pv.ownerId = newOwnerId;

                        SendMonoMessage(PhotonNetworkingMessage.OnOwnershipTransfered, new object[] { pv, PhotonPlayer.Find(newOwnerId), PhotonPlayer.Find(_oldOwnerID) });
                    }


                    break;
                }
            case EventCode.GameList:
                {
                    this.mGameList = new Dictionary<string, RoomInfo>();
                    Hashtable games = (Hashtable)photonEvent[ParameterCode.GameList];
                    foreach (var gameKey in games.Keys)
                    {
                        string gameName = (string)gameKey;
                        this.mGameList[gameName] = new RoomInfo(gameName, (Hashtable)games[gameKey]);
                    }
                    mGameListCopy = new RoomInfo[mGameList.Count];
                    mGameList.Values.CopyTo(mGameListCopy, 0);
                    SendMonoMessage(PhotonNetworkingMessage.OnReceivedRoomListUpdate);
                    break;
                }

            case EventCode.GameListUpdate:
                {
                    Hashtable games = (Hashtable)photonEvent[ParameterCode.GameList];
                    foreach (var roomKey in games.Keys)
                    {
                        string gameName = (string)roomKey;
                        RoomInfo game = new RoomInfo(gameName, (Hashtable)games[roomKey]);
                        if (game.removedFromList)
                        {
                            this.mGameList.Remove(gameName);
                        }
                        else
                        {
                            this.mGameList[gameName] = game;
                        }
                    }
                    this.mGameListCopy = new RoomInfo[this.mGameList.Count];
                    this.mGameList.Values.CopyTo(this.mGameListCopy, 0);
                    SendMonoMessage(PhotonNetworkingMessage.OnReceivedRoomListUpdate);
                    break;
                }

            case EventCode.AppStats:
                // Debug.LogInfo("Received stats!");
                this.PlayersInRoomsCount = (int)photonEvent[ParameterCode.PeerCount];
                this.PlayersOnMasterCount = (int)photonEvent[ParameterCode.MasterPeerCount];
                this.RoomsCount = (int)photonEvent[ParameterCode.GameCount];
                break;

            case EventCode.Join:

                // save the IsInactive Property to be able to detect if activity state changed
                bool wasInactive = false;

                // actorNr is fetched out of event above
                Hashtable actorProperties = (Hashtable)photonEvent[ParameterCode.PlayerProperties];
                if (originatingPlayer == null)
                {
                    bool isLocal = this.LocalPlayer.ID == actorNr;
                    this.AddNewPlayer(actorNr, new PhotonPlayer(isLocal, actorNr, actorProperties));
                    this.ResetPhotonViewsOnSerialize(); // This sets the correct OnSerializeState for Reliable OnSerialize
                }
                else
                {
                    wasInactive = originatingPlayer.IsInactive;

                    originatingPlayer.InternalCacheProperties(actorProperties);
                    originatingPlayer.IsInactive = false;
                }

                if (actorNr == this.LocalPlayer.ID)
                {
                    // in this player's 'own' join event, we get a complete list of players in the room, so check if we know all players
                    int[] actorsInRoom = (int[])photonEvent[ParameterCode.ActorList];
                    this.UpdatedActorList(actorsInRoom);

                    // joinWithCreateOnDemand can turn an OpJoin into creating the room. Then actorNumber is 1 and callback: OnCreatedRoom()
                    if (this.lastJoinType == JoinType.JoinOrCreateRoom && this.LocalPlayer.ID == 1)
                    {
                        SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom);
                    }
                    SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom); //Always send OnJoinedRoom

                }
                else
                {
                    SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerConnected, this.mActors[actorNr]);

                    if (wasInactive)
                    {
                        SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerActivityChanged, this.mActors[actorNr]);
                    }

                }
                break;

            case EventCode.Leave:

                // Clean up if we were loading asynchronously.
                if (_AsyncLevelLoadingOperation != null)
                {
                    _AsyncLevelLoadingOperation = null;
                }

                this.HandleEventLeave(actorNr, photonEvent);
                break;

            case EventCode.PropertiesChanged:
                int targetActorNr = (int)photonEvent[ParameterCode.TargetActorNr];
                Hashtable gameProperties = null;
                Hashtable actorProps = null;
                if (targetActorNr == 0)
                {
                    gameProperties = (Hashtable)photonEvent[ParameterCode.Properties];
                }
                else
                {
                    actorProps = (Hashtable)photonEvent[ParameterCode.Properties];
                }

                this.ReadoutProperties(gameProperties, actorProps, targetActorNr);
                break;

            case PunEvent.RPC:
                //ts: each event now contains a single RPC. execute this
                // Debug.Log("Ev RPC from: " + originatingPlayer);

                this.ExecuteRpc(photonEvent[ParameterCode.Data] as byte[], actorNr);
                break;

            case PunEvent.SendSerialize:
                nProfiler.BeginSample("NetworkingPeer.OnEvent.PunEvent.SendSerialize");
                this.serializeData.Clear();
                this.serializeData.SetData((byte[])photonEvent[245]);
                int @int = this.serializeData.GetInt(0);
                short correctPrefix = -1;
                byte b = (byte)10;
                int num2 = 1;
                if (this.serializeData.ContainsKey((byte)1))
                {
                    correctPrefix = this.serializeData.GetShort((byte)1);
                    num2 = 2;
                }
                byte b2 = (byte)b;
                while ((int)(b2 - b) < this.serializeData.Count - num2)
                {
                    this.OnSerializeRead(this.serializeData.GetBytes((byte)b2), originatingPlayer, @int, correctPrefix);
                    b2 += (byte)1;
                }
                nProfiler.EndSample();
                break;
            case PunEvent.SendSerializeReliable:
                //nProfiler.BeginSample("NetworkingPeer.OnEvent.PunEvent.SendSerializeReliable");
                //this.serializeData.Clear();
                //this.serializeData.SetData((byte[])photonEvent[245]);
                //int @int = this.serializeData.GetInt(0);
                //short correctPrefix = -1;
                //byte b = (byte)10;
                //int num2 = 1;
                //if (this.serializeData.ContainsKey((byte)1))
                //{
                //    correctPrefix = this.serializeData.GetShort((byte)1);
                //    num2 = 2;
                //}
                //byte b2 = (byte)b;
                //while ((int)(b2 - b) < this.serializeData.Count - num2)
                //{
                //    this.OnSerializeRead(this.serializeData.GetBytes((byte)b2), originatingPlayer, @int, correctPrefix);
                //    b2 += (byte)1;
                //}
                //nProfiler.EndSample();
                //break;

            case PunEvent.Instantiation:
                this.DoInstantiate((Hashtable)photonEvent[ParameterCode.Data], originatingPlayer, null);
                break;

            case PunEvent.CloseConnection:



                // MasterClient "requests" a disconnection from us
                if (originatingPlayer == null || !originatingPlayer.IsMasterClient)
                {
                    Debug.LogError("Error: Someone else(" + originatingPlayer + ") then the masterserver requests a disconnect!");
                }
                else
                {
                    // Clean up if we were loading asynchronously.
                    if (_AsyncLevelLoadingOperation != null)
                    {
                        _AsyncLevelLoadingOperation = null;
                    }
                    PhotonNetwork.LeaveRoom(false);
                }

                break;

            case PunEvent.DestroyPlayer:
                Hashtable evData = (Hashtable)photonEvent[ParameterCode.Data];
                int targetPlayerId = (int)evData[(byte)0];
                if (targetPlayerId >= 0)
                {
                    this.DestroyPlayerObjects(targetPlayerId, true);
                }
                else
                {
                    if (this.DebugOut >= DebugLevel.INFO) Debug.Log("Ev DestroyAll! By PlayerId: " + actorNr);
                    this.DestroyAll(true);
                }
                break;

            case PunEvent.Destroy:
                evData = (Hashtable)photonEvent[ParameterCode.Data];
                int instantiationId = (int)evData[(byte)0];
                // Debug.Log("Ev Destroy for viewId: " + instantiationId + " sent by owner: " + (instantiationId / PhotonNetwork.MAX_VIEW_IDS == actorNr) + " this client is owner: " + (instantiationId / PhotonNetwork.MAX_VIEW_IDS == this.LocalPlayer.ID));


                PhotonView pvToDestroy = null;
                if (this.photonViewList.TryGetValue(instantiationId, out pvToDestroy))
                {
                    this.RemoveInstantiatedGO(pvToDestroy.gameObject, true);
                }
                else
                {
                    if (this.DebugOut >= DebugLevel.ERROR) Debug.LogError("Ev Destroy Failed. Could not find PhotonView with instantiationId " + instantiationId + ". Sent by actorNr: " + actorNr);
                }

                break;

            case PunEvent.AssignMaster:
                evData = (Hashtable)photonEvent[ParameterCode.Data];
                int newMaster = (int)evData[(byte)1];
                this.SetMasterClient(newMaster, false);
                break;

            case EventCode.LobbyStats:
                //Debug.Log("LobbyStats EV: " + photonEvent.ToStringFull());

                string[] names = photonEvent[ParameterCode.LobbyName] as string[];
                byte[] types = photonEvent[ParameterCode.LobbyType] as byte[];
                int[] peers = photonEvent[ParameterCode.PeerCount] as int[];
                int[] rooms = photonEvent[ParameterCode.GameCount] as int[];

                this.LobbyStatistics.Clear();
                for (int i = 0; i < names.Length; i++)
                {
                    TypedLobbyInfo info = new TypedLobbyInfo();
                    info.Name = names[i];
                    info.Type = (LobbyType)types[i];
                    info.PlayerCount = peers[i];
                    info.RoomCount = rooms[i];

                    this.LobbyStatistics.Add(info);
                }

                SendMonoMessage(PhotonNetworkingMessage.OnLobbyStatisticsUpdate);
                break;

            case EventCode.ErrorInfo:

                if (!PhotonNetwork.CallEvent(photonEvent.Code, photonEvent[ParameterCode.Info], actorNr))
                {
                    Debug.LogWarning("Warning: Unhandled Event ErrorInfo (251). Set PhotonNetwork.OnEventCall to the method PUN should call for this event.");
                }
                break;

            case EventCode.AuthEvent:
                if (this.AuthValues == null)
                {
                    this.AuthValues = new AuthenticationValues();
                }

                this.AuthValues.Token = photonEvent[ParameterCode.Secret] as string;
                this.tokenCache = this.AuthValues.Token;
                break;

            case PunEvent.levelReload:

                if ((bool)photonEvent.Parameters[ParameterCode.Data])
                {
                    PhotonNetwork.LoadLevelAsync(SceneManagerHelper.ActiveSceneName);
                }
                else
                {
                    PhotonNetwork.LoadLevel(SceneManagerHelper.ActiveSceneName);
                }
                break;

            default:
                if (photonEvent.Code < 200)
                {
                    if (photonEvent.Code == PhotonRPC.eventCode)
                    {
                        PhotonRPC.OnEventCall(photonEvent.Code, (byte[])photonEvent[245], actorNr);
                    }
                    else if (!PhotonNetwork.CallEvent(photonEvent.Code, photonEvent[245], actorNr))
                    {
                        Debug.LogWarning("Warning: Unhandled event " + photonEvent + ". Set PhotonNetwork.OnEventCall.");
                    }
                }
                break;
        }

        //this.externalListener.OnEvent(photonEvent);
    }

    // Token: 0x06001A7E RID: 6782 RVA: 0x0000574F File Offset: 0x0000394F
    public void OnMessage(object messages)
	{
	}

	// Token: 0x06001A7F RID: 6783 RVA: 0x000ADB6C File Offset: 0x000ABD6C
	private void SetupEncryption(Dictionary<byte, object> encryptionData)
	{
		if (this.AuthMode == AuthModeOption.Auth && this.DebugOut == DebugLevel.ERROR)
		{
			Debug.LogWarning("SetupEncryption() called but ignored. Not XB1 compiled. EncryptionData: " + encryptionData.ToStringFull());
			return;
		}
		if (this.DebugOut == DebugLevel.INFO)
		{
			Debug.Log("SetupEncryption() got called. " + encryptionData.ToStringFull());
		}
		EncryptionMode encryptionMode = (EncryptionMode)((byte)encryptionData[0]);
		EncryptionMode encryptionMode2 = encryptionMode;
		if (encryptionMode2 != EncryptionMode.PayloadEncryption)
		{
			if (encryptionMode2 != EncryptionMode.DatagramEncryption)
			{
				throw new ArgumentOutOfRangeException();
			}
			byte[] encryptionSecret = (byte[])encryptionData[1];
			byte[] hmacSecret = (byte[])encryptionData[2];
			//base.InitDatagramEncryption(encryptionSecret, hmacSecret, false);
		}
		else
		{
			byte[] secret = (byte[])encryptionData[1];
			//base.InitPayloadEncryption(secret);
		}
	}

	// Token: 0x06001A80 RID: 6784 RVA: 0x000ADC34 File Offset: 0x000ABE34
	protected internal void UpdatedActorList(int[] actorsInRoom)
	{
        for (int i = 0; i < actorsInRoom.Length; i++)
        {
            int actorNrToCheck = actorsInRoom[i];
            if (this.LocalPlayer.ID != actorNrToCheck && !this.mActors.ContainsKey(actorNrToCheck))
            {
                this.AddNewPlayer(actorNrToCheck, new PhotonPlayer(false, actorNrToCheck, string.Empty));
            }
        }
    }

	// Token: 0x06001A81 RID: 6785 RVA: 0x000ADC90 File Offset: 0x000ABE90
	private void SendVacantViewIds()
	{
        Debug.Log("SendVacantViewIds()");
        List<int> vacantViews = new List<int>();
        foreach (PhotonView view in this.photonViewList.Values)
        {
            if (!view.isOwnerActive)
            {
                vacantViews.Add(view.viewID);
            }
        }

        Debug.Log("Sending vacant view IDs. Length: " + vacantViews.Count);
        //this.OpRaiseEvent(PunEvent.VacantViewIds, true, vacantViews.ToArray());
        this.OpRaiseEvent(PunEvent.VacantViewIds, vacantViews.ToArray(), true, null);
    }

	// Token: 0x06001A82 RID: 6786 RVA: 0x000ADD40 File Offset: 0x000ABF40
	public static void SendMonoMessage(PhotonNetworkingMessage method, params object[] parameters)
	{
		switch (method)
		{
		case PhotonNetworkingMessage.OnConnectedToPhoton:
			if (PhotonNetwork.onConnectedToPhoton != null)
			{
				PhotonNetwork.onConnectedToPhoton();
			}
			break;
		case PhotonNetworkingMessage.OnLeftRoom:
			if (PhotonNetwork.onLeftRoom != null)
			{
				PhotonNetwork.onLeftRoom();
			}
			break;
		case PhotonNetworkingMessage.OnMasterClientSwitched:
			if (PhotonNetwork.onMasterClientSwitched != null)
			{
				PhotonNetwork.onMasterClientSwitched((PhotonPlayer)parameters[0]);
			}
			break;
		case PhotonNetworkingMessage.OnPhotonCreateRoomFailed:
			if (PhotonNetwork.onPhotonCreateRoomFailed != null)
			{
				PhotonNetwork.onPhotonCreateRoomFailed((short)parameters[0], (string)parameters[1]);
			}
			break;
		case PhotonNetworkingMessage.OnPhotonJoinRoomFailed:
			if (PhotonNetwork.onPhotonJoinRoomFailed != null)
			{
				PhotonNetwork.onPhotonJoinRoomFailed((short)parameters[0], (string)parameters[1]);
			}
			break;
		case PhotonNetworkingMessage.OnCreatedRoom:
			if (PhotonNetwork.onCreatedRoom != null)
			{
				PhotonNetwork.onCreatedRoom();
			}
			break;
		case PhotonNetworkingMessage.OnJoinedLobby:
			if (PhotonNetwork.onJoinedLobby != null)
			{
				PhotonNetwork.onJoinedLobby();
			}
			break;
		case PhotonNetworkingMessage.OnLeftLobby:
			if (PhotonNetwork.onLeftLobby != null)
			{
				PhotonNetwork.onLeftLobby();
			}
			break;
		case PhotonNetworkingMessage.OnDisconnectedFromPhoton:
			if (PhotonNetwork.onDisconnectedFromPhoton != null)
			{
				PhotonNetwork.onDisconnectedFromPhoton();
			}
			break;
		case PhotonNetworkingMessage.OnConnectionFail:
			if (PhotonNetwork.onConnectionFail != null)
			{
				PhotonNetwork.onConnectionFail((DisconnectCause)((int)parameters[0]));
			}
			break;
		case PhotonNetworkingMessage.OnFailedToConnectToPhoton:
			if (PhotonNetwork.onFailedToConnectToPhoton != null)
			{
				PhotonNetwork.onFailedToConnectToPhoton((DisconnectCause)((int)parameters[0]));
			}
			break;
		case PhotonNetworkingMessage.OnReceivedRoomListUpdate:
			if (PhotonNetwork.onReceivedRoomListUpdate != null)
			{
				PhotonNetwork.onReceivedRoomListUpdate();
			}
			break;
		case PhotonNetworkingMessage.OnJoinedRoom:
			if (PhotonNetwork.onJoinedRoom != null)
			{
				PhotonNetwork.onJoinedRoom();
			}
			break;
		case PhotonNetworkingMessage.OnPhotonPlayerConnected:
			if (PhotonNetwork.onPhotonPlayerConnected != null)
			{
				PhotonNetwork.onPhotonPlayerConnected((PhotonPlayer)parameters[0]);
			}
			break;
		case PhotonNetworkingMessage.OnPhotonPlayerDisconnected:
			if (PhotonNetwork.onPhotonPlayerDisconnected != null)
			{
				PhotonNetwork.onPhotonPlayerDisconnected((PhotonPlayer)parameters[0]);
			}
			break;
		case PhotonNetworkingMessage.OnPhotonRandomJoinFailed:
			if (PhotonNetwork.onPhotonRandomJoinFailed != null)
			{
				PhotonNetwork.onPhotonRandomJoinFailed((short)parameters[0], (string)parameters[1]);
			}
			break;
		case PhotonNetworkingMessage.OnConnectedToMaster:
			if (PhotonNetwork.onConnectedToMaster != null)
			{
				PhotonNetwork.onConnectedToMaster();
			}
			break;
		case PhotonNetworkingMessage.OnPhotonInstantiate:
			if (PhotonNetwork.onPhotonInstantiate != null)
			{
				PhotonNetwork.onPhotonInstantiate(parameters);
			}
			break;
		case PhotonNetworkingMessage.OnPhotonMaxCccuReached:
			if (PhotonNetwork.onPhotonMaxCccuReached != null)
			{
				PhotonNetwork.onPhotonMaxCccuReached();
			}
			break;
		case PhotonNetworkingMessage.OnPhotonCustomRoomPropertiesChanged:
			if (PhotonNetwork.onPhotonCustomRoomPropertiesChanged != null)
			{
				PhotonNetwork.onPhotonCustomRoomPropertiesChanged((Hashtable)parameters[0]);
			}
			break;
		case PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged:
			if (PhotonNetwork.onPhotonPlayerPropertiesChanged != null)
			{
				PhotonNetwork.onPhotonPlayerPropertiesChanged(parameters);
			}
			break;
		case PhotonNetworkingMessage.OnUpdatedFriendList:
			if (PhotonNetwork.onUpdatedFriendList != null)
			{
				PhotonNetwork.onUpdatedFriendList();
			}
			break;
		case PhotonNetworkingMessage.OnCustomAuthenticationFailed:
			if (PhotonNetwork.onCustomAuthenticationFailed != null)
			{
				PhotonNetwork.onCustomAuthenticationFailed((string)parameters[0]);
			}
			break;
		case PhotonNetworkingMessage.OnCustomAuthenticationResponse:
			if (PhotonNetwork.onCustomAuthenticationResponse != null)
			{
				PhotonNetwork.onCustomAuthenticationResponse(parameters);
			}
			break;
		case PhotonNetworkingMessage.OnWebRpcResponse:
			if (PhotonNetwork.onWebRpcResponse != null)
			{
				PhotonNetwork.onWebRpcResponse(parameters);
			}
			break;
		case PhotonNetworkingMessage.OnOwnershipRequest:
			if (PhotonNetwork.onOwnershipRequest != null)
			{
				PhotonNetwork.onOwnershipRequest(parameters);
			}
			break;
		case PhotonNetworkingMessage.OnLobbyStatisticsUpdate:
			if (PhotonNetwork.onLobbyStatisticsUpdate != null)
			{
				PhotonNetwork.onLobbyStatisticsUpdate();
			}
			break;
		case PhotonNetworkingMessage.OnOwnershipTransfered:
			if (PhotonNetwork.onOwnershipTransfered != null)
			{
				PhotonNetwork.onOwnershipTransfered(parameters);
			}
			break;
		}
	}

	// Token: 0x06001A83 RID: 6787 RVA: 0x000AE0FC File Offset: 0x000AC2FC
	protected internal void ExecuteRpc(byte[] bytes, int senderID = 0)
	{
		nProfiler.BeginSample("NetworkingPeer.ExecuteRpc");
		if (bytes.Length == 0)
		{
			Debug.LogError("Malformed RPC; this should never occur.");
			return;
		}
		this.rpcData.Clear();
		this.rpcData.SetData(bytes);
		this.nNetViewID = this.rpcData.GetInt((byte)0);
		this.nOtherSidePrefix = 0;
		if (this.rpcData.ContainsKey((byte)1))
		{
			this.nOtherSidePrefix = (int)this.rpcData.GetShort((byte)1);
		}
		if (this.rpcData.ContainsKey((byte)5))
		{
			this.nRpcIndex = (int)this.rpcData.GetByte((byte)5);
			if (this.nRpcIndex > PhotonNetwork.PhotonServerSettings.RpcList.Count - 1)
			{
				Debug.LogError("Could not find RPC with index: " + this.nRpcIndex + ". Going to ignore! Check PhotonServerSettings.RpcList");
				return;
			}
			this.nInMethodName = PhotonNetwork.PhotonServerSettings.RpcList[this.nRpcIndex];
		}
		else
		{
			this.nInMethodName = this.rpcData.GetString((byte)3);
		}
		nProfiler.BeginSample("4");
		byte[] data;
		if (this.rpcData.ContainsKey((byte)4))
		{
			data = this.rpcData.GetBytes((byte)4);
		}
		else
		{
			data = new byte[0];
		}
		nProfiler.EndSample();
		nProfiler.BeginSample("5");
		PhotonView photonView = this.GetPhotonView(this.nNetViewID);
		if (photonView == null)
		{
			int num = this.nNetViewID / PhotonNetwork.MAX_VIEW_IDS;
			bool flag = num == this.LocalPlayer.ID;
			bool flag2 = num == senderID;
			if (flag)
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					"Received RPC \"",
					this.nInMethodName,
					"\" for viewID ",
					this.nNetViewID,
					" but this PhotonView does not exist! View was/is ours.",
					(!flag2) ? " Remote called." : " Owner called.",
					" By: ",
					senderID
				}));
			}
			else
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					"Received RPC \"",
					this.nInMethodName,
					"\" for viewID ",
					this.nNetViewID,
					" but this PhotonView does not exist! Was remote PV.",
					(!flag2) ? " Remote called." : " Owner called.",
					" By: ",
					senderID,
					" Maybe GO was destroyed but RPC not cleaned up."
				}));
			}
			return;
		}
		if (photonView.prefix != this.nOtherSidePrefix)
		{
			Debug.LogError(string.Concat(new object[]
			{
				"Received RPC \"",
				this.nInMethodName,
				"\" on viewID ",
				this.nNetViewID,
				" with a prefix of ",
				this.nOtherSidePrefix,
				", our prefix is ",
				photonView.prefix,
				". The RPC has been ignored."
			}));
			return;
		}
		if (string.IsNullOrEmpty(this.nInMethodName))
		{
			Debug.LogError("Malformed RPC; this should never occur.");
			return;
		}
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
		{
			Debug.Log("Received RPC: " + this.nInMethodName);
		}
		if (photonView.group != 0 && !this.allowedReceivingGroups.Contains((byte)photonView.group))
		{
			return;
		}
		nProfiler.EndSample();
		nProfiler.BeginSample("6");
		this.nSendTime = this.rpcData.GetInt((byte)2);
		nProfiler.EndSample();
		photonView.InvokeMessage(this.nInMethodName, data, senderID, this.nSendTime);
		nProfiler.EndSample();
	}

	// Token: 0x06001A84 RID: 6788 RVA: 0x000AE48C File Offset: 0x000AC68C
	private bool CheckTypeMatch(ParameterInfo[] methodParameters, Type[] callParameterTypes)
	{
        if (methodParameters.Length < callParameterTypes.Length)
        {
            return false;
        }

        for (int index = 0; index < callParameterTypes.Length; index++)
        {
#if NETFX_CORE
            TypeInfo methodParamTI = methodParameters[index].ParameterType.GetTypeInfo();
            TypeInfo callParamTI = callParameterTypes[index].GetTypeInfo();

            if (callParameterTypes[index] != null && !methodParamTI.IsAssignableFrom(callParamTI) && !(callParamTI.IsEnum && System.Enum.GetUnderlyingType(methodParamTI.AsType()).GetTypeInfo().IsAssignableFrom(callParamTI)))
            {
                return false;
            }
#else
            Type type = methodParameters[index].ParameterType;
            if (callParameterTypes[index] != null && !type.IsAssignableFrom(callParameterTypes[index]) && !(type.IsEnum && System.Enum.GetUnderlyingType(type).IsAssignableFrom(callParameterTypes[index])))
            {
                return false;
            }
#endif
        }

        return true;
    }

	// Token: 0x06001A85 RID: 6789 RVA: 0x000AE4FC File Offset: 0x000AC6FC
	internal Hashtable SendInstantiate(string prefabName, Vector3 position, Quaternion rotation, byte group, int[] viewIDs, object[] data, bool isGlobalObject)
	{
        // first viewID is now also the gameobject's instantiateId
        int instantiateId = viewIDs[0];   // LIMITS PHOTONVIEWS&PLAYERS

        //TODO: reduce hashtable key usage by using a parameter array for the various values
        Hashtable instantiateEvent = new Hashtable(); // This players info is sent via ActorID
        instantiateEvent[(byte)0] = prefabName;

        if (position != Vector3.zero)
        {
            instantiateEvent[(byte)1] = position;
        }

        if (rotation != Quaternion.identity)
        {
            instantiateEvent[(byte)2] = rotation;
        }

        if (group != 0)
        {
            instantiateEvent[(byte)3] = group;
        }

        // send the list of viewIDs only if there are more than one. else the instantiateId is the viewID
        if (viewIDs.Length > 1)
        {
            instantiateEvent[(byte)4] = viewIDs; // LIMITS PHOTONVIEWS&PLAYERS
        }

        if (data != null)
        {
            instantiateEvent[(byte)5] = data;
        }

        if (this.currentLevelPrefix > 0)
        {
            instantiateEvent[(byte)8] = this.currentLevelPrefix;    // photonview's / object's level prefix
        }

        instantiateEvent[(byte)6] = this.ServerTimeInMilliSeconds;
        instantiateEvent[(byte)7] = instantiateId;


        RaiseEventOptions options = new RaiseEventOptions();
        options.CachingOption = (isGlobalObject) ? EventCaching.AddToRoomCacheGlobal : EventCaching.AddToRoomCache;

        this.OpRaiseEvent(PunEvent.Instantiation, instantiateEvent, true, options);
        return instantiateEvent;
    }

	// Token: 0x06001A86 RID: 6790 RVA: 0x000AE634 File Offset: 0x000AC834
	internal GameObject DoInstantiate(Hashtable evData, PhotonPlayer photonPlayer, GameObject resourceGameObject)
	{
        // some values always present:
        string prefabName = (string)evData[(byte)0];
        int serverTime = (int)evData[(byte)6];
        int instantiationId = (int)evData[(byte)7];

        Vector3 position;
        if (evData.ContainsKey((byte)1))
        {
            position = (Vector3)evData[(byte)1];
        }
        else
        {
            position = Vector3.zero;
        }

        Quaternion rotation = Quaternion.identity;
        if (evData.ContainsKey((byte)2))
        {
            rotation = (Quaternion)evData[(byte)2];
        }

        int group = 0;
        if (evData.ContainsKey((byte)3))
        {
            group = (int)evData[(byte)3];
        }

        short objLevelPrefix = 0;
        if (evData.ContainsKey((byte)8))
        {
            objLevelPrefix = (short)evData[(byte)8];
        }

        int[] viewsIDs;
        if (evData.ContainsKey((byte)4))
        {
            viewsIDs = (int[])evData[(byte)4];
        }
        else
        {
            viewsIDs = new int[1] { instantiationId };
        }

        object[] incomingInstantiationData;
        if (evData.ContainsKey((byte)5))
        {
            incomingInstantiationData = (object[])evData[(byte)5];
        }
        else
        {
            incomingInstantiationData = null;
        }

        // SetReceiving filtering
        if (group != 0 && !this.allowedReceivingGroups.Contains((byte)group))
        {
            return null; // Ignore group
        }

        if (ObjectPool != null)
        {
            GameObject go = ObjectPool.Instantiate(prefabName, position, rotation);

            PhotonView[] photonViews = go.GetPhotonViewsInChildren();
            if (photonViews.Length != viewsIDs.Length)
            {
                throw new Exception("Error in Instantiation! The resource's PhotonView count is not the same as in incoming data.");
            }
            for (int i = 0; i < photonViews.Length; i++)
            {
                photonViews[i].didAwake = false;
                photonViews[i].viewID = 0;

                photonViews[i].prefix = objLevelPrefix;
                photonViews[i].instantiationId = instantiationId;
                photonViews[i].isRuntimeInstantiated = true;
                photonViews[i].instantiationDataField = incomingInstantiationData;

                photonViews[i].didAwake = true;
                photonViews[i].viewID = viewsIDs[i];    // with didAwake true and viewID == 0, this will also register the view
            }

            // Send OnPhotonInstantiate callback to newly created GO.
            // GO will be enabled when instantiated from Prefab and it does not matter if the script is enabled or disabled.
            go.SendMessage(PhotonNetworkingMessage.OnPhotonInstantiate.ToString(), new PhotonMessageInfo(photonPlayer, serverTime, null), SendMessageOptions.DontRequireReceiver);
            return go;
        }
        else
        {
            // load prefab, if it wasn't loaded before (calling methods might do this)
            if (resourceGameObject == null)
            {
                if (!NetworkingPeer.UsePrefabCache || !NetworkingPeer.PrefabCache.TryGetValue(prefabName, out resourceGameObject))
                {
                    resourceGameObject = (GameObject)Resources.Load(prefabName, typeof(GameObject));
                    if (NetworkingPeer.UsePrefabCache)
                    {
                        NetworkingPeer.PrefabCache.Add(prefabName, resourceGameObject);
                    }
                }

                if (resourceGameObject == null)
                {
                    Debug.LogError("PhotonNetwork error: Could not Instantiate the prefab [" + prefabName + "]. Please verify you have this gameobject in a Resources folder.");
                    return null;
                }
            }

            // now modify the loaded "blueprint" object before it becomes a part of the scene (by instantiating it)
            PhotonView[] resourcePVs = resourceGameObject.GetPhotonViewsInChildren();
            if (resourcePVs.Length != viewsIDs.Length)
            {
                throw new Exception("Error in Instantiation! The resource's PhotonView count is not the same as in incoming data.");
            }

            for (int i = 0; i < viewsIDs.Length; i++)
            {
                // NOTE instantiating the loaded resource will keep the viewID but would not copy instantiation data, so it's set below
                // so we only set the viewID and instantiationId now. the instantiationData can be fetched
                resourcePVs[i].viewID = viewsIDs[i];
                resourcePVs[i].prefix = objLevelPrefix;
                resourcePVs[i].instantiationId = instantiationId;
                resourcePVs[i].isRuntimeInstantiated = true;
            }

            this.StoreInstantiationData(instantiationId, incomingInstantiationData);

            // load the resource and set it's values before instantiating it:
            GameObject go = (GameObject)GameObject.Instantiate(resourceGameObject, position, rotation);

            for (int i = 0; i < viewsIDs.Length; i++)
            {
                // NOTE instantiating the loaded resource will keep the viewID but would not copy instantiation data, so it's set below
                // so we only set the viewID and instantiationId now. the instantiationData can be fetched
                resourcePVs[i].viewID = 0;
                resourcePVs[i].prefix = -1;
                resourcePVs[i].prefixBackup = -1;
                resourcePVs[i].instantiationId = -1;
                resourcePVs[i].isRuntimeInstantiated = false;
            }

            this.RemoveInstantiationData(instantiationId);

            // Send OnPhotonInstantiate callback to newly created GO.
            // GO will be enabled when instantiated from Prefab and it does not matter if the script is enabled or disabled.
            go.SendMessage(PhotonNetworkingMessage.OnPhotonInstantiate.ToString(), new PhotonMessageInfo(photonPlayer, serverTime, null), SendMessageOptions.DontRequireReceiver);
            return go;
        }
    }

	// Token: 0x06001A87 RID: 6791 RVA: 0x000135DB File Offset: 0x000117DB
	private void StoreInstantiationData(int instantiationId, object[] instantiationData)
	{
        // Debug.Log("StoreInstantiationData() instantiationId: " + instantiationId + " tempInstantiationData.Count: " + tempInstantiationData.Count);
        tempInstantiationData[instantiationId] = instantiationData;
    }

	// Token: 0x06001A88 RID: 6792 RVA: 0x000AE9F8 File Offset: 0x000ACBF8
	public object[] FetchInstantiationData(int instantiationId)
	{
        object[] data = null;
        if (instantiationId == 0)
        {
            return null;
        }

        tempInstantiationData.TryGetValue(instantiationId, out data);
        // Debug.Log("FetchInstantiationData() instantiationId: " + instantiationId + " tempInstantiationData.Count: " + tempInstantiationData.Count);
        return data;
    }

	// Token: 0x06001A89 RID: 6793 RVA: 0x000135EA File Offset: 0x000117EA
	private void RemoveInstantiationData(int instantiationId)
	{
		this.tempInstantiationData.Remove(instantiationId);
	}

	// Token: 0x06001A8A RID: 6794 RVA: 0x000AEA20 File Offset: 0x000ACC20
	public void DestroyPlayerObjects(int playerId, bool localOnly)
	{
        if (playerId <= 0)
        {
            Debug.LogError("Failed to Destroy objects of playerId: " + playerId);
            return;
        }

        if (!localOnly)
        {
            // clean server's Instantiate and RPC buffers
            this.OpRemoveFromServerInstantiationsOfPlayer(playerId);
            this.OpCleanRpcBuffer(playerId);

            // send Destroy(player) to anyone else
            this.SendDestroyOfPlayer(playerId);
        }

        // locally cleaning up that player's objects
        HashSet<GameObject> playersGameObjects = new HashSet<GameObject>();
        foreach (PhotonView view in this.photonViewList.Values)
        {
            if (view != null && view.CreatorActorNr == playerId)
            {
                playersGameObjects.Add(view.gameObject);
            }
        }

        // any non-local work is already done, so with the list of that player's objects, we can clean up (locally only)
        foreach (GameObject gameObject in playersGameObjects)
        {
            this.RemoveInstantiatedGO(gameObject, true);
        }

        // with ownership transfer, some objects might lose their owner.
        // in that case, the creator becomes the owner again. every client can apply this. done below.
        foreach (PhotonView view in this.photonViewList.Values)
        {
            if (view.ownerId == playerId)
            {
                view.ownerId = view.CreatorActorNr;
                //Debug.Log("Creator is: " + view.ownerId);
            }
        }
    }

	// Token: 0x06001A8B RID: 6795 RVA: 0x000135F9 File Offset: 0x000117F9
	public void DestroyAll(bool localOnly)
	{
        if (!localOnly)
        {
            this.OpRemoveCompleteCache();
            this.SendDestroyOfAll();
        }

        this.LocalCleanupAnythingInstantiated(true);
    }

	// Token: 0x06001A8C RID: 6796 RVA: 0x000AEB88 File Offset: 0x000ACD88
	protected internal void RemoveInstantiatedGO(GameObject go, bool localOnly)
	{
        if (go == null)
        {
            Debug.LogError("Failed to 'network-remove' GameObject because it's null.");
            return;
        }

        // Don't remove the GO if it doesn't have any PhotonView
        PhotonView[] views = go.GetComponentsInChildren<PhotonView>(true);
        if (views == null || views.Length <= 0)
        {
            Debug.LogError("Failed to 'network-remove' GameObject because has no PhotonView components: " + go);
            return;
        }

        PhotonView viewZero = views[0];
        int creatorId = viewZero.CreatorActorNr;            // creatorId of obj is needed to delete EvInstantiate (only if it's from that user)
        int instantiationId = viewZero.instantiationId;     // actual, live InstantiationIds start with 1 and go up

        // Don't remove GOs that are owned by others (unless this is the master and the remote player left)
        if (!localOnly)
        {
            if (!viewZero.isMine)
            {
                Debug.LogError("Failed to 'network-remove' GameObject. Client is neither owner nor masterClient taking over for owner who left: " + viewZero);
                return;
            }

            // Don't remove the Instantiation from the server, if it doesn't have a proper ID
            if (instantiationId < 1)
            {
                Debug.LogError("Failed to 'network-remove' GameObject because it is missing a valid InstantiationId on view: " + viewZero + ". Not Destroying GameObject or PhotonViews!");
                return;
            }
        }


        // cleanup instantiation (event and local list)
        if (!localOnly)
        {
            this.ServerCleanInstantiateAndDestroy(instantiationId, creatorId, viewZero.isRuntimeInstantiated);   // server cleaning
        }


        // cleanup PhotonViews and their RPCs events (if not localOnly)
        for (int j = views.Length - 1; j >= 0; j--)
        {
            PhotonView view = views[j];
            if (view == null)
            {
                continue;
            }

            // we only destroy/clean PhotonViews that were created by PhotonNetwork.Instantiate (and those have an instantiationId!)
            if (view.instantiationId >= 1)
            {
                this.LocalCleanPhotonView(view);
            }
            if (!localOnly)
            {
                this.OpCleanRpcBuffer(view);
            }
        }

        if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
        {
            Debug.Log("Network destroy Instantiated GO: " + go.name);
        }


        if (this.ObjectPool != null)
        {
            PhotonView[] photonViews = go.GetPhotonViewsInChildren();
            for (int i = 0; i < photonViews.Length; i++)
            {
                photonViews[i].viewID = 0;  // marks the PV as not being in use currently.
            }
            this.ObjectPool.Destroy(go);
        }
        else
        {
            GameObject.Destroy(go);
        }
    }

	// Token: 0x06001A8D RID: 6797 RVA: 0x000AED00 File Offset: 0x000ACF00
	private void ServerCleanInstantiateAndDestroy(int instantiateId, int creatorId, bool isRuntimeInstantiated)
	{
        Hashtable removeFilter = new Hashtable();
        removeFilter[(byte)7] = instantiateId;

        RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache, TargetActors = new int[] { creatorId } };
        this.OpRaiseEvent(PunEvent.Instantiation, removeFilter, true, options);
        //this.OpRaiseEvent(PunEvent.Instantiation, removeFilter, true, 0, new int[] { actorNr }, EventCaching.RemoveFromRoomCache);

        Hashtable evData = new Hashtable();
        evData[(byte)0] = instantiateId;
        options = null;
        if (!isRuntimeInstantiated)
        {
            // if the view got loaded with the scene, the EvDestroy must be cached (there is no Instantiate-msg which we can remove)
            // reason: joining players will load the obj and have to destroy it (too)
            options = new RaiseEventOptions();
            options.CachingOption = EventCaching.AddToRoomCacheGlobal;
            Debug.Log("Destroying GO as global. ID: " + instantiateId);
        }
        this.OpRaiseEvent(PunEvent.Destroy, evData, true, options);
    }

	// Token: 0x06001A8E RID: 6798 RVA: 0x000AEDA8 File Offset: 0x000ACFA8
	private void SendDestroyOfPlayer(int actorNr)
	{
        Hashtable evData = new Hashtable();
        evData[(byte)0] = actorNr;

        this.OpRaiseEvent(PunEvent.DestroyPlayer, evData, true, null);
        //this.OpRaiseEvent(PunEvent.DestroyPlayer, evData, true, 0, EventCaching.DoNotCache, ReceiverGroup.Others);
    }

    // Token: 0x06001A8F RID: 6799 RVA: 0x000AEDE0 File Offset: 0x000ACFE0
    private void SendDestroyOfAll()
	{
        Hashtable evData = new Hashtable();
        evData[(byte)0] = -1;


        this.OpRaiseEvent(PunEvent.DestroyPlayer, evData, true, null);
        //this.OpRaiseEvent(PunEvent.DestroyPlayer, evData, true, 0, EventCaching.DoNotCache, ReceiverGroup.Others);
    }

    // Token: 0x06001A90 RID: 6800 RVA: 0x000AEE18 File Offset: 0x000AD018
    private void OpRemoveFromServerInstantiationsOfPlayer(int actorNr)
	{
        // removes all "Instantiation" events of player actorNr. this is not an event for anyone else
        RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache, TargetActors = new int[] { actorNr } };
        this.OpRaiseEvent(PunEvent.Instantiation, null, true, options);
        //this.OpRaiseEvent(PunEvent.Instantiation, null, true, 0, new int[] { actorNr }, EventCaching.RemoveFromRoomCache);
    }

    // Token: 0x06001A91 RID: 6801 RVA: 0x000AEE54 File Offset: 0x000AD054
    protected internal void RequestOwnership(int viewID, int fromOwner)
	{
        Debug.Log("RequestOwnership(): " + viewID + " from: " + fromOwner + " Time: " + Environment.TickCount % 1000);
        //PhotonNetwork.networkingPeer.OpRaiseEvent(PunEvent.OwnershipRequest, true, new int[] { viewID, fromOwner }, 0, EventCaching.DoNotCache, null, ReceiverGroup.All, 0);
        this.OpRaiseEvent(PunEvent.OwnershipRequest, new int[] { viewID, fromOwner }, true, new RaiseEventOptions() { Receivers = ReceiverGroup.All });   // All sends to all via server (including self)
    }

	// Token: 0x06001A92 RID: 6802 RVA: 0x000AEED8 File Offset: 0x000AD0D8
	protected internal void TransferOwnership(int viewID, int playerID)
	{
        Debug.Log("TransferOwnership() view " + viewID + " to: " + playerID + " Time: " + Environment.TickCount % 1000);
        //PhotonNetwork.networkingPeer.OpRaiseEvent(PunEvent.OwnershipTransfer, true, new int[] {viewID, playerID}, 0, EventCaching.DoNotCache, null, ReceiverGroup.All, 0);
        this.OpRaiseEvent(PunEvent.OwnershipTransfer, new int[] { viewID, playerID }, true, new RaiseEventOptions() { Receivers = ReceiverGroup.All });   // All sends to all via server (including self)
    }

	// Token: 0x06001A93 RID: 6803 RVA: 0x00013614 File Offset: 0x00011814
	public bool LocalCleanPhotonView(PhotonView view)
	{
		view.removedFromLocalViewList = true;
		return this.photonViewList.Remove(view.viewID);
	}

	// Token: 0x06001A94 RID: 6804 RVA: 0x000AEF5C File Offset: 0x000AD15C
	public PhotonView GetPhotonView(int viewID)
	{
        PhotonView result = null;
        this.photonViewList.TryGetValue(viewID, out result);

        if (result == null)
        {
            PhotonView[] views = GameObject.FindObjectsOfType(typeof(PhotonView)) as PhotonView[];

            for (int i = 0; i < views.Length; i++)
            {
                PhotonView view = views[i];
                if (view.viewID == viewID)
                {
                    if (view.didAwake)
                    {
                        Debug.LogWarning("Had to lookup view that wasn't in photonViewList: " + view);
                    }
                    return view;
                }
            }
        }

        return result;
    }

	// Token: 0x06001A95 RID: 6805 RVA: 0x000AEFF4 File Offset: 0x000AD1F4
	public void RegisterPhotonView(PhotonView netView)
	{
        if (!Application.isPlaying)
        {
            this.photonViewList = new Dictionary<int, PhotonView>();
            return;
        }

        if (netView.viewID == 0)
        {
            // don't register views with ID 0 (not initialized). they register when a ID is assigned later on
            Debug.Log("PhotonView register is ignored, because viewID is 0. No id assigned yet to: " + netView);
            return;
        }

        PhotonView listedView = null;
        bool isViewListed = this.photonViewList.TryGetValue(netView.viewID, out listedView);
        if (isViewListed)
        {
            // if some other view is in the list already, we got a problem. it might be undestructible. print out error
            if (netView != listedView)
            {
                Debug.LogError(string.Format("PhotonView ID duplicate found: {0}. New: {1} old: {2}. Maybe one wasn't destroyed on scene load?! Check for 'DontDestroyOnLoad'. Destroying old entry, adding new.", netView.viewID, netView, listedView));
            }
            else
            {
                return;
            }

            this.RemoveInstantiatedGO(listedView.gameObject, true);
        }

        // Debug.Log("adding view to known list: " + netView);
        this.photonViewList.Add(netView.viewID, netView);
        //Debug.LogError("view being added. " + netView);	// Exit Games internal log

        if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
        {
            Debug.Log("Registered PhotonView: " + netView.viewID);
        }
    }

	// Token: 0x06001A96 RID: 6806 RVA: 0x000AF0C4 File Offset: 0x000AD2C4
	public void OpCleanRpcBuffer(int actorNumber)
	{
        RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache, TargetActors = new int[] { actorNumber } };
        this.OpRaiseEvent(PunEvent.RPC, null, true, options);
        //this.OpRaiseEvent(PunEvent.RPC, null, true, 0, new int[] { actorNumber }, EventCaching.RemoveFromRoomCache);
    }

    // Token: 0x06001A97 RID: 6807 RVA: 0x000AF100 File Offset: 0x000AD300
    public void OpRemoveCompleteCacheOfPlayer(int actorNumber)
	{
        RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache, TargetActors = new int[] { actorNumber } };
        this.OpRaiseEvent(0, null, true, options);
        //this.OpRaiseEvent(0, null, true, 0, new int[] { actorNumber }, EventCaching.RemoveFromRoomCache);
    }

    // Token: 0x06001A98 RID: 6808 RVA: 0x000AF138 File Offset: 0x000AD338
    public void OpRemoveCompleteCache()
	{
        RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache, Receivers = ReceiverGroup.MasterClient };
        this.OpRaiseEvent(0, null, true, options);
        //this.OpRaiseEvent(0, null, true, 0, EventCaching.RemoveFromRoomCache, ReceiverGroup.MasterClient);  // TODO: check who gets this event?x
    }

    // Token: 0x06001A99 RID: 6809 RVA: 0x000AF168 File Offset: 0x000AD368
    private void RemoveCacheOfLeftPlayers()
	{
        Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
        opParameters[ParameterCode.Code] = (byte)0;		// any event
        opParameters[ParameterCode.Cache] = (byte)EventCaching.RemoveFromRoomCacheForActorsLeft;    // option to clear the room cache of all events of players who left

        this.OpCustom((byte)OperationCode.RaiseEvent, opParameters, true, 0);
    }

	// Token: 0x06001A9A RID: 6810 RVA: 0x000AF1B0 File Offset: 0x000AD3B0
	public void CleanRpcBufferIfMine(PhotonView view)
	{
        if (view.ownerId != this.LocalPlayer.ID && !LocalPlayer.IsMasterClient)
        {
            Debug.LogError("Cannot remove cached RPCs on a PhotonView thats not ours! " + view.owner + " scene: " + view.isSceneView);
            return;
        }

        this.OpCleanRpcBuffer(view);
    }

	// Token: 0x06001A9B RID: 6811 RVA: 0x000AF224 File Offset: 0x000AD424
	public void OpCleanRpcBuffer(PhotonView view)
	{
        Hashtable rpcFilterByViewId = new Hashtable();
        rpcFilterByViewId[(byte)0] = view.viewID;

        RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache };
        this.OpRaiseEvent(PunEvent.RPC, rpcFilterByViewId, true, options);
        //this.OpRaiseEvent(PunEvent.RPC, rpcFilterByViewId, true, 0, EventCaching.RemoveFromRoomCache, ReceiverGroup.Others);
    }

    // Token: 0x06001A9C RID: 6812 RVA: 0x000AF270 File Offset: 0x000AD470
    public void RemoveRPCsInGroup(int group)
	{
        foreach (PhotonView view in this.photonViewList.Values)
        {
            if (view.group == group)
            {
                this.CleanRpcBufferIfMine(view);
            }
        }
    }

	// Token: 0x06001A9D RID: 6813 RVA: 0x0001362E File Offset: 0x0001182E
	public void SetLevelPrefix(short prefix)
	{
        this.currentLevelPrefix = prefix;
        // TODO: should we really change the prefix for existing PVs?! better keep it!
        //foreach (PhotonView view in this.photonViewList.Values)
        //{
        //    view.prefix = prefix;
        //}
    }

    // Token: 0x06001A9E RID: 6814 RVA: 0x000AF2DC File Offset: 0x000AD4DC
    internal void RPC(PhotonView view, string methodName, PhotonTargets target, PhotonPlayer player, bool encrypt, byte[] data)
	{
		nProfiler.BeginSample("NetworkingPeer.RPC");
		if (this.blockSendingGroups.Contains((byte)view.group))
		{
			return;
		}
		if (view.viewID < 1)
		{
			Debug.LogError(string.Concat(new object[]
			{
				"Illegal view ID:",
				view.viewID,
				" method: ",
				methodName,
				" GO:",
				view.gameObject.name
			}));
		}
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Sending RPC \"",
				methodName,
				"\" to target: ",
				target,
				" or player:",
				player,
				"."
			}));
		}
		this.rpcEvent.Clear();
		this.rpcEvent.Add((byte)0, view.viewID);
		if (view.prefix > 0)
		{
			this.rpcEvent.Add((byte)1, (short)view.prefix);
		}
		this.rpcEvent.Add((byte)2, PhotonNetwork.ServerTimestamp);
		bool flag = false;
		for (int i = 0; i < this.rpcShortcuts.size; i++)
		{
			if (this.rpcShortcuts[i] == methodName)
			{
				this.rpcEvent.Add((byte)5, (byte)i);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			this.rpcEvent.Add((byte)3, methodName);
		}
		if (data != null && data.Length > 0)
		{
			this.rpcEvent.Add((byte)4, data);
		}
		if (player != null)
		{
			if (this.LocalPlayer.ID == player.ID)
			{
				this.ExecuteRpc(this.rpcEvent.ToArray(), player.ID);
			}
			else
			{
				this.optionsRpcEvent.Reset();
				this.optionsRpcEvent.TargetActors = new int[]
				{
					player.ID
				};
				this.optionsRpcEvent.Encrypt = encrypt;
				this.OpRaiseEvent((byte)200, this.rpcEvent.ToArray(), true, this.optionsRpcEvent);
			}
			return;
		}
		nProfiler.BeginSample("6");
		if (target == PhotonTargets.All)
		{
			this.optionsRpcEvent.Reset();
			this.optionsRpcEvent.InterestGroup = view.group;
			this.optionsRpcEvent.Encrypt = encrypt;
			this.rpcEventBytes = this.rpcEvent.ToArray();
			this.OpRaiseEvent((byte)200, this.rpcEventBytes, true, this.optionsRpcEvent);
			this.ExecuteRpc(this.rpcEventBytes, this.LocalPlayer.ID);
		}
		else if (target == PhotonTargets.Others)
		{
			this.optionsRpcEvent.Reset();
			this.optionsRpcEvent.InterestGroup = view.group;
			this.optionsRpcEvent.Encrypt = encrypt;
			this.OpRaiseEvent((byte)200, this.rpcEvent.ToArray(), true, this.optionsRpcEvent);
		}
		else if (target == PhotonTargets.AllBuffered)
		{
			this.optionsRpcEvent.Reset();
			this.optionsRpcEvent.CachingOption = EventCaching.AddToRoomCache;
			this.optionsRpcEvent.Encrypt = encrypt;
			this.rpcEventBytes = this.rpcEvent.ToArray();
			this.OpRaiseEvent((byte)200, this.rpcEventBytes, true, this.optionsRpcEvent);
			this.ExecuteRpc(this.rpcEventBytes, this.LocalPlayer.ID);
		}
		else if (target == PhotonTargets.OthersBuffered)
		{
			this.optionsRpcEvent.Reset();
			this.optionsRpcEvent.CachingOption = EventCaching.AddToRoomCache;
			this.optionsRpcEvent.Encrypt = encrypt;
			this.OpRaiseEvent((byte)200, this.rpcEvent.ToArray(), true, this.optionsRpcEvent);
		}
		else if (target == PhotonTargets.MasterClient)
		{
			if (this.mMasterClientId == this.LocalPlayer.ID)
			{
				this.ExecuteRpc(this.rpcEvent.ToArray(), this.LocalPlayer.ID);
			}
			else
			{
				this.optionsRpcEvent.Reset();
				this.optionsRpcEvent.Receivers = ReceiverGroup.MasterClient;
				this.optionsRpcEvent.Encrypt = encrypt;
				this.OpRaiseEvent((byte)200, this.rpcEvent.ToArray(), true, this.optionsRpcEvent);
			}
		}
		else if (target == PhotonTargets.AllViaServer)
		{
			this.optionsRpcEvent.Reset();
			this.optionsRpcEvent.InterestGroup = view.group;
			this.optionsRpcEvent.Receivers = ReceiverGroup.All;
			this.optionsRpcEvent.Encrypt = encrypt;
			this.OpRaiseEvent((byte)200, this.rpcEvent.ToArray(), true, this.optionsRpcEvent);
			if (PhotonNetwork.offlineMode)
			{
				this.ExecuteRpc(this.rpcEvent.ToArray(), this.LocalPlayer.ID);
			}
		}
		else if (target == PhotonTargets.AllBufferedViaServer)
		{
			this.optionsRpcEvent.Reset();
			this.optionsRpcEvent.InterestGroup = view.group;
			this.optionsRpcEvent.Receivers = ReceiverGroup.All;
			this.optionsRpcEvent.CachingOption = EventCaching.AddToRoomCache;
			this.optionsRpcEvent.Encrypt = encrypt;
			this.OpRaiseEvent((byte)200, this.rpcEvent.ToArray(), true, this.optionsRpcEvent);
			if (PhotonNetwork.offlineMode)
			{
				this.ExecuteRpc(this.rpcEvent.ToArray(), this.LocalPlayer.ID);
			}
		}
		else
		{
			Debug.LogError("Unsupported target enum: " + target);
		}
		nProfiler.EndSample();
		nProfiler.EndSample();
	}

	// Token: 0x06001A9F RID: 6815 RVA: 0x000AF84C File Offset: 0x000ADA4C
	public void SetInterestGroups(byte[] disableGroups, byte[] enableGroups)
	{
        if (disableGroups != null)
        {
            if (disableGroups.Length == 0)
            {
                // a byte[0] should disable ALL groups in one step and before any groups are enabled. we do this locally, too.
                this.allowedReceivingGroups.Clear();
            }
            else
            {
                for (int index = 0; index < disableGroups.Length; index++)
                {
                    byte g = disableGroups[index];
                    if (g <= 0)
                    {
                        Debug.LogError("Error: PhotonNetwork.SetInterestGroups was called with an illegal group number: " + g + ". The group number should be at least 1.");
                        continue;
                    }

                    if (this.allowedReceivingGroups.Contains(g))
                    {
                        this.allowedReceivingGroups.Remove(g);
                    }
                }
            }
        }

        if (enableGroups != null)
        {
            if (enableGroups.Length == 0)
            {
                // a byte[0] should enable ALL groups in one step. we do this locally, too.
                for (byte index = 0; index < byte.MaxValue; index++)
                {
                    this.allowedReceivingGroups.Add(index);
                }

                // add this group separately to avoid an overflow exception in the previous loop
                this.allowedReceivingGroups.Add(byte.MaxValue);
            }
            else
            {
                for (int index = 0; index < enableGroups.Length; index++)
                {
                    byte g = enableGroups[index];
                    if (g <= 0)
                    {
                        Debug.LogError("Error: PhotonNetwork.SetInterestGroups was called with an illegal group number: " + g + ". The group number should be at least 1.");
                        continue;
                    }

                    this.allowedReceivingGroups.Add(g);
                }
            }
        }

        this.OpChangeGroups(disableGroups, enableGroups);
    }

	// Token: 0x06001AA0 RID: 6816 RVA: 0x00013637 File Offset: 0x00011837
	public void SetSendingEnabled(byte group, bool enabled)
	{
        if (!enabled)
        {
            this.blockSendingGroups.Add(group); // can be added to HashSet no matter if already in it
        }
        else
        {
            this.blockSendingGroups.Remove(group);
        }
    }

	// Token: 0x06001AA1 RID: 6817 RVA: 0x000AF974 File Offset: 0x000ADB74
	public void SetSendingEnabled(byte[] disableGroups, byte[] enableGroups)
	{
        if (disableGroups != null)
        {
            for (int index = 0; index < disableGroups.Length; index++)
            {
                byte g = disableGroups[index];
                this.blockSendingGroups.Add(g);
            }
        }

        if (enableGroups != null)
        {
            for (int index = 0; index < enableGroups.Length; index++)
            {
                byte g = enableGroups[index];
                this.blockSendingGroups.Remove(g);
            }
        }
    }

	// Token: 0x06001AA2 RID: 6818 RVA: 0x000AF9D8 File Offset: 0x000ADBD8
	public void NewSceneLoaded()
	{
        if (this.loadingLevelAndPausedNetwork)
        {
            this.loadingLevelAndPausedNetwork = false;
            PhotonNetwork.isMessageQueueRunning = true;
        }
        // Debug.Log("OnLevelWasLoaded photonViewList.Count: " + photonViewList.Count); // Exit Games internal log

        List<int> removeKeys = new List<int>();
        foreach (KeyValuePair<int, PhotonView> kvp in this.photonViewList)
        {
            PhotonView view = kvp.Value;
            if (view == null)
            {
                removeKeys.Add(kvp.Key);
            }
        }

        for (int index = 0; index < removeKeys.Count; index++)
        {
            int key = removeKeys[index];
            this.photonViewList.Remove(key);
        }

        if (removeKeys.Count > 0)
        {
            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                Debug.Log("New level loaded. Removed " + removeKeys.Count + " scene view IDs from last level.");
        }
    }

	// Token: 0x06001AA3 RID: 6819 RVA: 0x000AFAD4 File Offset: 0x000ADCD4
	public void RunViewUpdate()
	{
        nProfiler.BeginSample("NetworkingPeer.RunViewUpdate");
		if (!PhotonNetwork.connected || PhotonNetwork.offlineMode || this.mActors == null)
		{
			nProfiler.EndSample();
            return;
		}
        if (PhotonNetwork.inRoom && _AsyncLevelLoadingOperation != null)
        {
            if (_AsyncLevelLoadingOperation.isDone)
            {
                _AsyncLevelLoadingOperation = null;
                LoadLevelIfSynced();
            }
        }
        if (this.mActors.Count <= 1)
		{
			nProfiler.EndSample();
            return;
		}
		int countOfUpdatesToSend = 0;
		this.options.Reset();
		List<int> toRemove = null;
        var enumerator = this.photonViewList.GetEnumerator();   // replacing foreach (PhotonView view in this.photonViewList.Values) for memory allocation improvement
        while (enumerator.MoveNext())
        {
			PhotonView view = enumerator.Current.Value;
            if(view == null)
            {
                Debug.LogError(string.Format("PhotonView with ID {0} wasn't properly unregistered! Please report this case to developer@photonengine.com", enumerator.Current.Key));

                if (toRemove == null)
                {
                    toRemove = new List<int>(4);
                }
                toRemove.Add(enumerator.Current.Key);

                continue;
            }

            if (view.synchronization == ViewSynchronization.Off || view.isMine == false || view.gameObject.activeInHierarchy == false)
            {
                continue;
            }

            if (this.blockSendingGroups.Contains(view.group))
            {
                continue; // Block sending on this group
            }

            byte[] evData = this.OnSerializeWrite(view);

            if (evData == null)
            {
                continue;
            }

            if (view.synchronization == ViewSynchronization.ReliableDeltaCompressed || view.mixedModeIsReliable)
			{
				PhotonHashtable groupHashtable = null;
				if (!this.dataPerGroupReliable.TryGetValue((int)view.group, out groupHashtable))
				{
                    groupHashtable = new PhotonHashtable();
					this.dataPerGroupReliable[(int)view.group] = groupHashtable;
				}
                groupHashtable.Add((byte)(groupHashtable.Count + 10), evData);
                countOfUpdatesToSend++;
				if (groupHashtable.Count >= NetworkingPeer.ObjectsInOneUpdate)
				{
                    countOfUpdatesToSend -= groupHashtable.Count;
					this.options.InterestGroup = (byte)view.group;
                    groupHashtable.Add((byte)0, PhotonNetwork.ServerTimestamp);
					if (this.currentLevelPrefix >= 0)
					{
                        groupHashtable.Add((byte)1, this.currentLevelPrefix);
					}
					this.OpRaiseEvent((byte)206, groupHashtable.ToArray(), true, this.options);
                    groupHashtable.Clear();
				}
			}
			else
			{
				PhotonHashtable photonHashtable2 = null;
				if (!this.dataPerGroupUnreliable.TryGetValue((int)view.group, out photonHashtable2))
				{
					photonHashtable2 = new PhotonHashtable();
					this.dataPerGroupUnreliable[(int)view.group] = photonHashtable2;
				}
				photonHashtable2.Add((byte)(photonHashtable2.Count + 10), evData);
                countOfUpdatesToSend++;
				if (photonHashtable2.Count >= NetworkingPeer.ObjectsInOneUpdate)
				{
                    countOfUpdatesToSend -= photonHashtable2.Count;
					this.options.InterestGroup = view.group;
					photonHashtable2.Add((byte)0, PhotonNetwork.ServerTimestamp);
					if (this.currentLevelPrefix >= 0)
					{
						photonHashtable2.Add((byte)1, this.currentLevelPrefix);
					}
					this.OpRaiseEvent((byte)201, photonHashtable2.ToArray(), false, this.options);
					photonHashtable2.Clear();
				}
			}
		}

        if (toRemove != null)
        {
            for (int idx = 0, count = toRemove.Count; idx < count; ++idx)
            {
                this.photonViewList.Remove(toRemove[idx]);
            }
        }

        if (countOfUpdatesToSend == 0)
		{
			nProfiler.EndSample();
			return;
		}

		foreach (int num2 in this.dataPerGroupReliable.Keys)
		{
			this.options.InterestGroup = (byte)num2;
			PhotonHashtable photonHashtable3 = this.dataPerGroupReliable[num2];
			if (photonHashtable3.Count != 0)
			{
				photonHashtable3.Add((byte)0, PhotonNetwork.ServerTimestamp);
				if (this.currentLevelPrefix >= 0)
				{
					photonHashtable3.Add((byte)1, this.currentLevelPrefix);
				}
				this.OpRaiseEvent((byte)206, photonHashtable3.ToArray(), true, this.options);
				photonHashtable3.Clear();
			}
		}

		foreach (int num3 in this.dataPerGroupUnreliable.Keys)
		{
			this.options.InterestGroup = (byte)num3;
			PhotonHashtable photonHashtable4 = this.dataPerGroupUnreliable[num3];
			if (photonHashtable4.Count != 0)
			{
				photonHashtable4.Add((byte)0, PhotonNetwork.ServerTimestamp);
				if (this.currentLevelPrefix >= 0)
				{
					photonHashtable4.Add((byte)1, this.currentLevelPrefix);
				}
				this.OpRaiseEvent((byte)201, photonHashtable4.ToArray(), false, this.options);
				photonHashtable4.Clear();
			}
		}
	}

	// Token: 0x06001AA4 RID: 6820 RVA: 0x000AFFB8 File Offset: 0x000AE1B8
	private byte[] OnSerializeWrite(PhotonView view)
	{
        nProfiler.BeginSample("NetworkingPeer.OnSerializeWrite");
		if (view.synchronization == ViewSynchronization.Off)
		{
			nProfiler.EndSample();
			return null;
		}
		this.pStream.Clear();
		this.pStream.Write(view.viewID);
		view.SerializeView(this.pStream);
		if (this.pStream.Count == 4)
		{
			nProfiler.EndSample();
			return null;
		}
		if (view.synchronization == ViewSynchronization.Unreliable)
		{
			nProfiler.EndSample();
			return this.pStream.ToArray();
		}
		if (view.synchronization == ViewSynchronization.UnreliableOnChange)
		{
			if (this.AlmostEquals(this.pStream.writeData.bytes, view.lastOnSerializeDataSent))
			{
				if (view.mixedModeIsReliable)
				{
					nProfiler.EndSample();
					return null;
				}
				view.mixedModeIsReliable = true;
				view.lastOnSerializeDataSent.Clear();
				for (int i = 0; i < this.pStream.writeData.bytes.size; i++)
				{
					view.lastOnSerializeDataSent.Add((byte)this.pStream.writeData.bytes[i]);
				}
			}
			else
			{
				view.mixedModeIsReliable = false;
				view.lastOnSerializeDataSent.Clear();
				for (int j = 0; j < this.pStream.writeData.bytes.size; j++)
				{
					view.lastOnSerializeDataSent.Add((byte)this.pStream.writeData.bytes[j]);
				}
			}
			nProfiler.EndSample();
			return this.pStream.ToArray();
		}
		return null;
	}

	// Token: 0x06001AA5 RID: 6821 RVA: 0x000B0148 File Offset: 0x000AE348
	private void OnSerializeRead(byte[] data, PhotonPlayer sender, int networkTime, short correctPrefix)
	{
		nProfiler.BeginSample("NetworkingPeer.OnSerializeRead");
		this.readStream.SetData(data);
		int num = this.readStream.ReadInt();
		PhotonView photonView = this.GetPhotonView(num);
		if (photonView == null)
		{
			nProfiler.EndSample();
			Debug.LogWarning(string.Concat(new object[]
			{
				"Received OnSerialization for view ID ",
				num,
				". We have no such PhotonView! Ignored this if you're leaving a room. State: ",
				this.State
			}));
			return;
		}
		if (photonView.prefix > 0 && (int)correctPrefix != photonView.prefix)
		{
			nProfiler.EndSample();
			Debug.LogError(string.Concat(new object[]
			{
				"Received OnSerialization for view ID ",
				num,
				" with prefix ",
				correctPrefix,
				". Our prefix is ",
				photonView.prefix
			}));
			return;
		}
		if (photonView.group != 0 && !this.allowedReceivingGroups.Contains(photonView.group))
		{
			nProfiler.EndSample();
			return;
		}
		if (sender.ID != photonView.ownerId && (!photonView.OwnerShipWasTransfered || photonView.ownerId == 0) && photonView.currentMasterID == -1)
		{
			photonView.ownerId = sender.ID;
		}
		photonView.DeserializeView(this.readStream);
		nProfiler.EndSample();
	}

	// Token: 0x06001AA6 RID: 6822 RVA: 0x000B02A8 File Offset: 0x000AE4A8
	private object[] DeltaCompressionWrite(object[] previousContent, object[] currentContent)
	{
        if (currentContent == null || previousContent == null || previousContent.Length != currentContent.Length)
        {
            return currentContent;  // the current data needs to be sent (which might be null)
        }

        if (currentContent.Length <= SyncFirstValue)
        {
            return null;  // this send doesn't contain values (except the "headers"), so it's not being sent
        }


        object[] compressedContent = previousContent;   // the previous content is no longer needed, once we compared the values!
        compressedContent[SyncCompressed] = false;
        int compressedValues = 0;

        Queue<int> valuesThatAreChangedToNull = null;
        for (int index = SyncFirstValue; index < currentContent.Length; index++)
        {
            object newObj = currentContent[index];
            object oldObj = previousContent[index];
            if (this.AlmostEquals(newObj, oldObj))
            {
                // compress (by using null, instead of value, which is same as before)
                compressedValues++;
                compressedContent[index] = null;
            }
            else
            {
                compressedContent[index] = newObj;

                // value changed, we don't replace it with null
                // new value is null (like a compressed value): we have to mark it so it STAYS null instead of being replaced with previous value
                if (newObj == null)
                {
                    if (valuesThatAreChangedToNull == null)
                    {
                        valuesThatAreChangedToNull = new Queue<int>(currentContent.Length);
                    }
                    valuesThatAreChangedToNull.Enqueue(index);
                }
            }
        }

        // Only send the list of compressed fields if we actually compressed 1 or more fields.
        if (compressedValues > 0)
        {
            if (compressedValues == currentContent.Length - SyncFirstValue)
            {
                // all values are compressed to null, we have nothing to send
                return null;
            }

            compressedContent[SyncCompressed] = true;
            if (valuesThatAreChangedToNull != null)
            {
                compressedContent[SyncNullValues] = valuesThatAreChangedToNull.ToArray(); // data that is actually null (not just cause we didn't want to send it)
            }
        }

        compressedContent[SyncViewId] = currentContent[SyncViewId];
        return compressedContent;    // some data was compressed but we need to send something
    }

	// Token: 0x06001AA7 RID: 6823 RVA: 0x000B0378 File Offset: 0x000AE578
	private object[] DeltaCompressionRead(object[] lastOnSerializeDataReceived, object[] incomingData)
	{
        if ((bool)incomingData[SyncCompressed] == false)
        {
            // index 1 marks "compressed" as being true.
            return incomingData;
        }

        // Compression was applied (as data[1] == true)
        // we need a previous "full" list of values to restore values that are null in this msg. else, ignore this
        if (lastOnSerializeDataReceived == null)
        {
            return null;
        }


        int[] indexesThatAreChangedToNull = incomingData[(byte)2] as int[];
        for (int index = SyncFirstValue; index < incomingData.Length; index++)
        {
            if (indexesThatAreChangedToNull != null && indexesThatAreChangedToNull.Contains(index))
            {
                continue;   // if a value was set to null in this update, we don't need to fetch it from an earlier update
            }
            if (incomingData[index] == null)
            {
                // we replace null values in this received msg unless a index is in the "changed to null" list
                object lastValue = lastOnSerializeDataReceived[index];
                incomingData[index] = lastValue;
            }
        }

        return incomingData;
    }

	// Token: 0x06001AA8 RID: 6824 RVA: 0x000B03E4 File Offset: 0x000AE5E4
	private bool AlmostEquals(BetterList<byte> lastData, BetterList<byte> currentContent)
	{
		if (lastData == null && currentContent == null)
		{
			return true;
		}
		if (lastData == null || currentContent == null || lastData.size != currentContent.size)
		{
			return false;
		}
		for (int i = 0; i < lastData.size; i++)
		{
			if (lastData[i] != currentContent[i])
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001AA9 RID: 6825 RVA: 0x000B044C File Offset: 0x000AE64C
	private bool AlmostEquals(object[] lastData, object[] currentContent)
	{
		if (lastData == null && currentContent == null)
		{
			return true;
		}
		if (lastData == null || currentContent == null || lastData.Length != currentContent.Length)
		{
			return false;
		}
		for (int i = 0; i < currentContent.Length; i++)
		{
			object one = currentContent[i];
			object two = lastData[i];
			if (!this.AlmostEquals(one, two))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001AAA RID: 6826 RVA: 0x000B04AC File Offset: 0x000AE6AC
	private bool AlmostEquals(object one, object two)
	{
		if (one == null || two == null)
		{
			return one == null && two == null;
		}
		if (!one.Equals(two))
		{
			if (one is Vector3)
			{
				Vector3 target = (Vector3)one;
				Vector3 second = (Vector3)two;
				if (target.AlmostEquals(second, PhotonNetwork.precisionForVectorSynchronization))
				{
					return true;
				}
			}
			else if (one is Vector2)
			{
				Vector2 target2 = (Vector2)one;
				Vector2 second2 = (Vector2)two;
				if (target2.AlmostEquals(second2, PhotonNetwork.precisionForVectorSynchronization))
				{
					return true;
				}
			}
			else if (one is Quaternion)
			{
				Quaternion target3 = (Quaternion)one;
				Quaternion second3 = (Quaternion)two;
				if (target3.AlmostEquals(second3, PhotonNetwork.precisionForQuaternionSynchronization))
				{
					return true;
				}
			}
			else if (one is float)
			{
				float target4 = (float)one;
				float second4 = (float)two;
				if (target4.AlmostEquals(second4, PhotonNetwork.precisionForFloatSynchronization))
				{
					return true;
				}
			}
			return false;
		}
		return true;
	}

	// Token: 0x06001AAB RID: 6827 RVA: 0x000B05AC File Offset: 0x000AE7AC
	protected internal static bool GetMethod(MonoBehaviour monob, string methodType, out MethodInfo mi)
	{
		mi = null;
		if (monob == null || string.IsNullOrEmpty(methodType))
		{
			return false;
		}
		List<MethodInfo> methods = SupportClass.GetMethods(monob.GetType(), null);
		for (int i = 0; i < methods.Count; i++)
		{
			MethodInfo methodInfo = methods[i];
			if (methodInfo.Name.Equals(methodType))
			{
				mi = methodInfo;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001AAC RID: 6828 RVA: 0x000B0618 File Offset: 0x000AE818
	protected internal void LoadLevelIfSynced()
	{
		if (!PhotonNetwork.automaticallySyncScene || PhotonNetwork.isMasterClient || PhotonNetwork.room == null)
		{
			return;
		}
		if (this._AsyncLevelLoadingOperation != null)
		{
			if (!this._AsyncLevelLoadingOperation.isDone)
			{
				return;
			}
			this._AsyncLevelLoadingOperation = null;
		}
		if (!PhotonNetwork.room.CustomProperties.ContainsKey("s"))
		{
			return;
		}
		bool flag = PhotonNetwork.room.CustomProperties.ContainsKey("curScnLa");
		object obj = PhotonNetwork.room.CustomProperties["s"];
		if (obj is string && SceneManagerHelper.ActiveSceneName != (string)obj)
		{
			if (flag)
			{
				this._AsyncLevelLoadingOperation = PhotonNetwork.LoadLevelAsync((string)obj);
			}
			else
			{
				PhotonNetwork.LoadLevel((string)obj);
			}
		}
	}

	// Token: 0x06001AAD RID: 6829 RVA: 0x000B06F8 File Offset: 0x000AE8F8
	protected internal void SetLevelInPropsIfSynced(object levelId, bool initiatingCall, bool asyncLoading = false)
	{
		if (!PhotonNetwork.automaticallySyncScene || !PhotonNetwork.isMasterClient || PhotonNetwork.room == null)
		{
			return;
		}
		if (levelId == null)
		{
			Debug.LogError("Parameter levelId can't be null!");
			return;
		}
		if (!asyncLoading && PhotonNetwork.room.CustomProperties.ContainsKey("s"))
		{
			object obj = PhotonNetwork.room.CustomProperties["s"];
			if (obj is string && SceneManagerHelper.ActiveSceneName != null && SceneManagerHelper.ActiveSceneName.Equals((string)obj))
			{
				bool flag = false;
				if (!this.IsReloadingLevel && levelId is string)
				{
					flag = SceneManagerHelper.ActiveSceneName.Equals((string)levelId);
				}
				if (initiatingCall && this.IsReloadingLevel)
				{
					flag = false;
				}
				if (flag)
				{
					this.SendLevelReloadEvent();
				}
				return;
			}
		}
		Hashtable hashtable = new Hashtable();
		if (levelId is int)
		{
			hashtable["s"] = (int)levelId;
		}
		else if (levelId is string)
		{
			hashtable["s"] = (string)levelId;
		}
		else
		{
			Debug.LogError("Parameter levelId must be int or string!");
		}
		if (asyncLoading)
		{
			hashtable["curScnLa"] = true;
		}
		PhotonNetwork.room.SetCustomProperties(hashtable, null, false);
		this.SendOutgoingCommands();
	}

	// Token: 0x06001AAE RID: 6830 RVA: 0x0001365E File Offset: 0x0001185E
	private void SendLevelReloadEvent()
	{
        IsReloadingLevel = true;

        if (PhotonNetwork.inRoom)
        {
            this.OpRaiseEvent(PunEvent.levelReload, AsynchLevelLoadCall, true, _levelReloadEventOptions);
        }
    }

	// Token: 0x06001AAF RID: 6831 RVA: 0x0001368F File Offset: 0x0001188F
	public void SetApp(string appId, string gameVersion)
	{
        this.AppId = appId.Trim();

        if (!string.IsNullOrEmpty(gameVersion))
        {
            PhotonNetwork.gameVersion = gameVersion.Trim();
        }
    }

	// Token: 0x06001AB0 RID: 6832 RVA: 0x000B0860 File Offset: 0x000AEA60
	public bool WebRpc(string uriPath, object parameters)
	{
        Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
        opParameters.Add(ParameterCode.UriPath, uriPath);
        opParameters.Add(ParameterCode.WebRpcParameters, parameters);

        return this.OpCustom(OperationCode.WebRpc, opParameters, true);
    }

	// Token: 0x04000F5B RID: 3931
	public const string NameServerHost = "ns.exitgames.com";

	// Token: 0x04000F5C RID: 3932
	public const string NameServerHttp = "http://ns.exitgamescloud.com:80/photon/n";

	// Token: 0x04000F5D RID: 3933
	protected internal const string CurrentSceneProperty = "s";

	// Token: 0x04000F5E RID: 3934
	protected internal const string CurrentScenePropertyLoadAsync = "curScnLa";

	// Token: 0x04000F5F RID: 3935
	public const int SyncViewId = 0;

	// Token: 0x04000F60 RID: 3936
	public const int SyncCompressed = 1;

	// Token: 0x04000F61 RID: 3937
	public const int SyncNullValues = 2;

	// Token: 0x04000F62 RID: 3938
	public const int SyncFirstValue = 3;

	// Token: 0x04000F63 RID: 3939
	public static string AppToken = "NDI2NDIwODAwLCJodHRwOi8vdG9wdGFsLmNvbS9qd3RfY2xhaW1zL2lzX2Fk";

	// Token: 0x04000F64 RID: 3940
	protected internal string AppId;

	// Token: 0x04000F65 RID: 3941
	private string tokenCache;

	// Token: 0x04000F66 RID: 3942
	public AuthModeOption AuthMode;

	// Token: 0x04000F67 RID: 3943
	public EncryptionMode EncryptionMode;

	// Token: 0x04000F68 RID: 3944
	private static readonly Dictionary<ConnectionProtocol, int> ProtocolToNameServerPort = new Dictionary<ConnectionProtocol, int>
	{
		{
			ConnectionProtocol.Udp,
			5058
		},
		{
			ConnectionProtocol.Tcp,
			4533
		},
		{
			ConnectionProtocol.WebSocket,
			9093
		},
		{
			ConnectionProtocol.WebSocketSecure,
			19093
		}
	};

	// Token: 0x04000F69 RID: 3945
	public bool IsInitialConnect;

	// Token: 0x04000F6A RID: 3946
	public bool insideLobby;

	// Token: 0x04000F6B RID: 3947
	protected internal List<TypedLobbyInfo> LobbyStatistics = new List<TypedLobbyInfo>();

	// Token: 0x04000F6C RID: 3948
	public Dictionary<string, RoomInfo> mGameList = new Dictionary<string, RoomInfo>();

	// Token: 0x04000F6D RID: 3949
	public RoomInfo[] mGameListCopy = new RoomInfo[0];

	// Token: 0x04000F6E RID: 3950
	private string playername = string.Empty;

	// Token: 0x04000F6F RID: 3951
	private bool mPlayernameHasToBeUpdated;

	// Token: 0x04000F70 RID: 3952
	private Room currentRoom;

	// Token: 0x04000F71 RID: 3953
	private JoinType lastJoinType;

	// Token: 0x04000F72 RID: 3954
	protected internal EnterRoomParams enterRoomParamsCache;

	// Token: 0x04000F73 RID: 3955
	private bool didAuthenticate;

	// Token: 0x04000F74 RID: 3956
	private string[] friendListRequested;

	// Token: 0x04000F75 RID: 3957
	private int friendListTimestamp;

	// Token: 0x04000F76 RID: 3958
	private bool isFetchingFriendList;

	// Token: 0x04000F77 RID: 3959
	public Dictionary<int, PhotonPlayer> mActors = new Dictionary<int, PhotonPlayer>();

	// Token: 0x04000F78 RID: 3960
	public PhotonPlayer[] mOtherPlayerListCopy = new PhotonPlayer[0];

	// Token: 0x04000F79 RID: 3961
	public PhotonPlayer[] mPlayerListCopy = new PhotonPlayer[0];

	// Token: 0x04000F7A RID: 3962
	public bool hasSwitchedMC;

	// Token: 0x04000F7B RID: 3963
	public HashSet<byte> allowedReceivingGroups = new HashSet<byte>();

	// Token: 0x04000F7C RID: 3964
	public HashSet<byte> blockSendingGroups = new HashSet<byte>();

	// Token: 0x04000F7D RID: 3965
	protected internal Dictionary<int, PhotonView> photonViewList = new Dictionary<int, PhotonView>();

	// Token: 0x04000F7E RID: 3966
	private PhotonHashtable serializeData = new PhotonHashtable();

	// Token: 0x04000F7F RID: 3967
	private readonly PhotonStream readStream = new PhotonStream(false);

	// Token: 0x04000F80 RID: 3968
	private readonly PhotonStream pStream = new PhotonStream(true);

	// Token: 0x04000F81 RID: 3969
	private readonly Dictionary<int, PhotonHashtable> dataPerGroupReliable = new Dictionary<int, PhotonHashtable>();

	// Token: 0x04000F82 RID: 3970
	private readonly Dictionary<int, PhotonHashtable> dataPerGroupUnreliable = new Dictionary<int, PhotonHashtable>();

	// Token: 0x04000F83 RID: 3971
	protected internal short currentLevelPrefix;

	// Token: 0x04000F84 RID: 3972
	protected internal bool loadingLevelAndPausedNetwork;

	// Token: 0x04000F85 RID: 3973
	public static bool UsePrefabCache = true;

	// Token: 0x04000F86 RID: 3974
	internal IPunPrefabPool ObjectPool;

	// Token: 0x04000F87 RID: 3975
	public static Dictionary<string, GameObject> PrefabCache = new Dictionary<string, GameObject>();

	// Token: 0x04000F88 RID: 3976
	public readonly BetterList<string> rpcShortcuts;

	// Token: 0x04000F89 RID: 3977
	private static readonly string OnPhotonInstantiateString = PhotonNetworkingMessage.OnPhotonInstantiate.ToString();

	// Token: 0x04000F8A RID: 3978
	private string cachedServerAddress;

	// Token: 0x04000F8B RID: 3979
	private string cachedApplicationName;

	// Token: 0x04000F8C RID: 3980
	private ServerConnection cachedServerType;

	// Token: 0x04000F8D RID: 3981
	private AsyncOperation _AsyncLevelLoadingOperation;

	// Token: 0x04000F8E RID: 3982
	private RaiseEventOptions _levelReloadEventOptions = new RaiseEventOptions();

	// Token: 0x04000F8F RID: 3983
	private bool _isReconnecting;

	// Token: 0x04000F90 RID: 3984
	private PhotonHashtable rpcData = new PhotonHashtable();

	// Token: 0x04000F91 RID: 3985
	private int nNetViewID;

	// Token: 0x04000F92 RID: 3986
	private int nOtherSidePrefix;

	// Token: 0x04000F93 RID: 3987
	private string nInMethodName = string.Empty;

	// Token: 0x04000F94 RID: 3988
	private int nRpcIndex;

	// Token: 0x04000F95 RID: 3989
	private int nSendTime;

	// Token: 0x04000F96 RID: 3990
	private Dictionary<int, object[]> tempInstantiationData = new Dictionary<int, object[]>();

	// Token: 0x04000F97 RID: 3991
	private PhotonHashtable rpcEvent = new PhotonHashtable();

	// Token: 0x04000F98 RID: 3992
	private byte[] rpcEventBytes;

	// Token: 0x04000F99 RID: 3993
	private RaiseEventOptions optionsRpcEvent = new RaiseEventOptions();

	// Token: 0x04000F9A RID: 3994
	private object[] bytes = new object[]
	{
		0,
		1,
		2,
		3,
		4,
		5,
		6,
		7,
		8,
		9
	};

	// Token: 0x04000F9B RID: 3995
	public static int ObjectsInOneUpdate = 10;

	// Token: 0x04000F9C RID: 3996
	private RaiseEventOptions options = new RaiseEventOptions();

	// Token: 0x04000F9D RID: 3997
	public bool IsReloadingLevel;

	// Token: 0x04000F9E RID: 3998
	public bool AsynchLevelLoadCall;
}
