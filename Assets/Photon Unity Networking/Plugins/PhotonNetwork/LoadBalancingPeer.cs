// ----------------------------------------------------------------------------
// <copyright file="LoadBalancingPeer.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2016 Exit Games GmbH
// </copyright>
// <summary>
//   Provides operations to use the LoadBalancing and Cloud photon servers.
//   No logic is implemented here.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_5_0 || UNITY_5_1 || UNITY_5_3_OR_NEWER
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using SupportClassPun = ExitGames.Client.Photon.SupportClass;
#endif


/// <summary>
/// A LoadbalancingPeer provides the operations and enum definitions needed to use the loadbalancing server application which is also used in Photon Cloud.
/// </summary>
/// <remarks>
/// Internally used by PUN.
/// The LoadBalancingPeer does not keep a state, instead this is done by a LoadBalancingClient.
/// </remarks>
internal class LoadBalancingPeer : PhotonPeer
{

    internal bool IsProtocolSecure
    {
        get { return this.UsedProtocol == ConnectionProtocol.WebSocketSecure; }
    }

    private readonly Dictionary<byte, object> opParameters = new Dictionary<byte, object>(); // used in OpRaiseEvent() (avoids lots of new Dictionary() calls)


    /// <summary>
    /// Creates a Peer with specified connection protocol. You need to set the Listener before using the peer.
    /// </summary>
    /// <remarks>Each connection protocol has it's own default networking ports for Photon.</remarks>
    /// <param name="protocolType">The preferred option is UDP.</param>
    public LoadBalancingPeer(ConnectionProtocol protocolType) : base(protocolType)
    {
        // this does not require a Listener, so:
        // make sure to set this.Listener before using a peer!
    }

    /// <summary>
    /// Creates a Peer with specified connection protocol and a Listener for callbacks.
    /// </summary>
    public LoadBalancingPeer(IPhotonPeerListener listener, ConnectionProtocol protocolType) : this(protocolType)
    {
        this.Listener = listener;
    }

    public virtual bool OpGetRegions(string appId)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters[(byte)ParameterCode.ApplicationId] = appId;

        return this.OpCustom(OperationCode.GetRegions, parameters, true, 0, true);
    }

    /// <summary>
    /// Joins the lobby on the Master Server, where you get a list of RoomInfos of currently open rooms.
    /// This is an async request which triggers a OnOperationResponse() call.
    /// </summary>
    /// <param name="lobby">The lobby join to.</param>
    /// <returns>If the operation could be sent (has to be connected).</returns>
    public virtual bool OpJoinLobby(TypedLobby lobby = null)
    {
        if (this.DebugOut >= DebugLevel.INFO)
        {
            this.Listener.DebugReturn(DebugLevel.INFO, "OpJoinLobby()");
        }

        Dictionary<byte, object> parameters = null;
        if (lobby != null && !lobby.IsDefault)
        {
            parameters = new Dictionary<byte, object>();
            parameters[(byte)ParameterCode.LobbyName] = lobby.Name;
            parameters[(byte)ParameterCode.LobbyType] = (byte)lobby.Type;
        }

        return this.OpCustom(OperationCode.JoinLobby, parameters, true);
    }


    /// <summary>
    /// Leaves the lobby on the Master Server.
    /// This is an async request which triggers a OnOperationResponse() call.
    /// </summary>
    /// <returns>If the operation could be sent (requires connection).</returns>
    public virtual bool OpLeaveLobby()
    {
        if (this.DebugOut >= DebugLevel.INFO)
        {
            this.Listener.DebugReturn(DebugLevel.INFO, "OpLeaveLobby()");
        }

        return this.OpCustom(OperationCode.LeaveLobby, null, true);
    }


    /// <summary>Used in the RoomOptionFlags parameter, this bitmask toggles options in the room.</summary>
    enum RoomOptionBit : int
    {
        CheckUserOnJoin = 0x01,  // toggles a check of the UserId when joining (enabling returning to a game)
        DeleteCacheOnLeave = 0x02,  // deletes cache on leave
        SuppressRoomEvents = 0x04,  // suppresses all room events
        PublishUserId = 0x08,  // signals that we should publish userId
        DeleteNullProps = 0x10,  // signals that we should remove property if its value was set to null. see RoomOption to Delete Null Properties
        BroadcastPropsChangeToAll = 0x20,  // signals that we should send PropertyChanged event to all room players including initiator
    }

    private void RoomOptionsToOpParameters(Dictionary<byte, object> op, RoomOptions roomOptions)
    {
        if (roomOptions == null)
        {
            roomOptions = new RoomOptions();
        }

        Hashtable gameProperties = new Hashtable();
        gameProperties[GamePropertyKey.IsOpen] = roomOptions.IsOpen;
        gameProperties[GamePropertyKey.IsVisible] = roomOptions.IsVisible;
        gameProperties[GamePropertyKey.PropsListedInLobby] = (roomOptions.CustomRoomPropertiesForLobby == null) ? new string[0] : roomOptions.CustomRoomPropertiesForLobby;
        gameProperties.MergeStringKeys(roomOptions.CustomRoomProperties);
        if (roomOptions.MaxPlayers > 0)
        {
            gameProperties[GamePropertyKey.MaxPlayers] = roomOptions.MaxPlayers;
        }
        op[ParameterCode.GameProperties] = gameProperties;


        int flags = 0;  // a new way to send the room options as bitwise-flags
        op[ParameterCode.CleanupCacheOnLeave] = roomOptions.CleanupCacheOnLeave;    // this is actually setting the room's config
        if (roomOptions.CleanupCacheOnLeave)
        {
            flags = flags | (int)RoomOptionBit.DeleteCacheOnLeave;
            gameProperties[GamePropertyKey.CleanupCacheOnLeave] = true;             // this is only informational for the clients which join
        }

        // in PUN v1.88 and PUN 2, CheckUserOnJoin is set by default:
        flags = flags | (int)RoomOptionBit.CheckUserOnJoin;
        op[ParameterCode.CheckUserOnJoin] = true;

        if (roomOptions.PlayerTtl > 0 || roomOptions.PlayerTtl == -1)
        {
            op[ParameterCode.PlayerTTL] = roomOptions.PlayerTtl;    // TURNBASED
        }

        if (roomOptions.EmptyRoomTtl > 0)
        {
            op[ParameterCode.EmptyRoomTTL] = roomOptions.EmptyRoomTtl;   //TURNBASED
        }

        if (roomOptions.SuppressRoomEvents)
        {
            flags = flags | (int)RoomOptionBit.SuppressRoomEvents;
            op[ParameterCode.SuppressRoomEvents] = true;
        }
        if (roomOptions.Plugins != null)
        {
            op[ParameterCode.Plugins] = roomOptions.Plugins;
        }
        if (roomOptions.PublishUserId)
        {
            flags = flags | (int)RoomOptionBit.PublishUserId;
            op[ParameterCode.PublishUserId] = true;
        }
        if (roomOptions.DeleteNullProperties)
        {
            flags = flags | (int)RoomOptionBit.DeleteNullProps; // this is only settable as flag
        }

        op[ParameterCode.RoomOptionFlags] = flags;
    }


    /// <summary>
    /// Creates a room (on either Master or Game Server).
    /// The OperationResponse depends on the server the peer is connected to:
    /// Master will return a Game Server to connect to.
    /// Game Server will return the joined Room's data.
    /// This is an async request which triggers a OnOperationResponse() call.
    /// </summary>
    /// <remarks>
    /// If the room is already existing, the OperationResponse will have a returnCode of ErrorCode.GameAlreadyExists.
    /// </remarks>
    public virtual bool OpCreateRoom(EnterRoomParams opParams)
    {
        if (this.DebugOut >= DebugLevel.INFO)
        {
            this.Listener.DebugReturn(DebugLevel.INFO, "OpCreateRoom()");
        }

        Dictionary<byte, object> op = new Dictionary<byte, object>();

        if (!string.IsNullOrEmpty(opParams.RoomName))
        {
            op[ParameterCode.RoomName] = opParams.RoomName;
        }
        if (opParams.Lobby != null && !string.IsNullOrEmpty(opParams.Lobby.Name))
        {
            op[ParameterCode.LobbyName] = opParams.Lobby.Name;
            op[ParameterCode.LobbyType] = (byte)opParams.Lobby.Type;
        }

        if (opParams.ExpectedUsers != null && opParams.ExpectedUsers.Length > 0)
        {
            op[ParameterCode.Add] = opParams.ExpectedUsers;
        }
        if (opParams.OnGameServer)
        {
            if (opParams.PlayerProperties != null && opParams.PlayerProperties.Count > 0)
            {
                op[ParameterCode.PlayerProperties] = opParams.PlayerProperties;
                op[ParameterCode.Broadcast] = true; // TODO: check if this also makes sense when creating a room?! // broadcast actor properties
            }

            this.RoomOptionsToOpParameters(op, opParams.RoomOptions);
        }

        //UnityEngine.Debug.Log("CreateRoom: " + SupportClassPun.DictionaryToString(op));
        return this.OpCustom(OperationCode.CreateGame, op, true);
    }

    /// <summary>
    /// Joins a room by name or creates new room if room with given name not exists.
    /// The OperationResponse depends on the server the peer is connected to:
    /// Master will return a Game Server to connect to.
    /// Game Server will return the joined Room's data.
    /// This is an async request which triggers a OnOperationResponse() call.
    /// </summary>
    /// <remarks>
    /// If the room is not existing (anymore), the OperationResponse will have a returnCode of ErrorCode.GameDoesNotExist.
    /// Other possible ErrorCodes are: GameClosed, GameFull.
    /// </remarks>
    /// <returns>If the operation could be sent (requires connection).</returns>
    public virtual bool OpJoinRoom(EnterRoomParams opParams)
    {
        if (this.DebugOut >= DebugLevel.INFO)
        {
            this.Listener.DebugReturn(DebugLevel.INFO, "OpJoinRoom()");
        }
        Dictionary<byte, object> op = new Dictionary<byte, object>();

        if (!string.IsNullOrEmpty(opParams.RoomName))
        {
            op[ParameterCode.RoomName] = opParams.RoomName;
        }

        if (opParams.CreateIfNotExists)
        {
            op[ParameterCode.JoinMode] = (byte)JoinMode.CreateIfNotExists;
            if (opParams.Lobby != null)
            {
                op[ParameterCode.LobbyName] = opParams.Lobby.Name;
                op[ParameterCode.LobbyType] = (byte)opParams.Lobby.Type;
            }
        }

        if (opParams.RejoinOnly)
        {
            op[ParameterCode.JoinMode] = (byte)JoinMode.RejoinOnly; // changed from JoinMode.JoinOrRejoin
        }

        if (opParams.ExpectedUsers != null && opParams.ExpectedUsers.Length > 0)
        {
            op[ParameterCode.Add] = opParams.ExpectedUsers;
        }

        if (opParams.OnGameServer)
        {
            if (opParams.PlayerProperties != null && opParams.PlayerProperties.Count > 0)
            {
                op[ParameterCode.PlayerProperties] = opParams.PlayerProperties;
                op[ParameterCode.Broadcast] = true; // broadcast actor properties
            }

            if (opParams.CreateIfNotExists)
            {
                this.RoomOptionsToOpParameters(op, opParams.RoomOptions);
            }
        }

        // UnityEngine.Debug.Log("JoinRoom: " + SupportClassPun.DictionaryToString(op));
        return this.OpCustom(OperationCode.JoinGame, op, true);
    }


    /// <summary>
    /// Operation to join a random, available room. Overloads take additional player properties.
    /// This is an async request which triggers a OnOperationResponse() call.
    /// If all rooms are closed or full, the OperationResponse will have a returnCode of ErrorCode.NoRandomMatchFound.
    /// If successful, the OperationResponse contains a gameserver address and the name of some room.
    /// </summary>
    /// <returns>If the operation could be sent currently (requires connection).</returns>
    public virtual bool OpJoinRandomRoom(OpJoinRandomRoomParams opJoinRandomRoomParams)
    {
        if (this.DebugOut >= DebugLevel.INFO)
        {
            this.Listener.DebugReturn(DebugLevel.INFO, "OpJoinRandomRoom()");
        }

        Hashtable expectedRoomProperties = new Hashtable();
        expectedRoomProperties.MergeStringKeys(opJoinRandomRoomParams.ExpectedCustomRoomProperties);
        if (opJoinRandomRoomParams.ExpectedMaxPlayers > 0)
        {
            expectedRoomProperties[GamePropertyKey.MaxPlayers] = opJoinRandomRoomParams.ExpectedMaxPlayers;
        }

        Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
        if (expectedRoomProperties.Count > 0)
        {
            opParameters[ParameterCode.GameProperties] = expectedRoomProperties;
        }

        if (opJoinRandomRoomParams.MatchingType != MatchmakingMode.FillRoom)
        {
            opParameters[ParameterCode.MatchMakingType] = (byte)opJoinRandomRoomParams.MatchingType;
        }

        if (opJoinRandomRoomParams.TypedLobby != null && !string.IsNullOrEmpty(opJoinRandomRoomParams.TypedLobby.Name))
        {
            opParameters[ParameterCode.LobbyName] = opJoinRandomRoomParams.TypedLobby.Name;
            opParameters[ParameterCode.LobbyType] = (byte)opJoinRandomRoomParams.TypedLobby.Type;
        }

        if (!string.IsNullOrEmpty(opJoinRandomRoomParams.SqlLobbyFilter))
        {
            opParameters[ParameterCode.Data] = opJoinRandomRoomParams.SqlLobbyFilter;
        }

        if (opJoinRandomRoomParams.ExpectedUsers != null && opJoinRandomRoomParams.ExpectedUsers.Length > 0)
        {
            opParameters[ParameterCode.Add] = opJoinRandomRoomParams.ExpectedUsers;
        }

        // UnityEngine.Debug.LogWarning("OpJoinRandom: " + opParameters.ToStringFull());
        return this.OpCustom(OperationCode.JoinRandomGame, opParameters, true);
    }


    /// <summary>
    /// Leaves a room with option to come back later or "for good".
    /// </summary>
    /// <param name="becomeInactive">Async games can be re-joined (loaded) later on. Set to false, if you want to abandon a game entirely.</param>
    /// <returns>If the opteration can be send currently.</returns>
    public virtual bool OpLeaveRoom(bool becomeInactive)
    {
        Dictionary<byte, object> parameters = null;
        if (becomeInactive)
        {
            parameters = new Dictionary<byte, object>();
            parameters[ParameterCode.IsInactive] = becomeInactive;
        }
        return this.OpCustom(OperationCode.Leave, parameters, true);
    }

    /// <summary>Gets a list of games matching a SQL-like where clause.</summary>
    /// <remarks>
    /// Operation is only available in lobbies of type SqlLobby.
    /// This is an async request which triggers a OnOperationResponse() call.
    /// Returned game list is stored in RoomInfoList.
    /// </remarks>
    /// <see cref="https://doc.photonengine.com/en-us/pun/current/lobby-and-matchmaking/matchmaking-and-lobby#sql_lobby_type"/>
    /// <param name="lobby">The lobby to query. Has to be of type SqlLobby.</param>
    /// <param name="queryData">The sql query statement.</param>
    /// <returns>If the operation could be sent (has to be connected).</returns>
    public virtual bool OpGetGameList(TypedLobby lobby, string queryData)
    {
        if (this.DebugOut >= DebugLevel.INFO)
        {
            this.Listener.DebugReturn(DebugLevel.INFO, "OpGetGameList()");
        }

        if (lobby == null)
        {
            if (this.DebugOut >= DebugLevel.INFO)
            {
                this.Listener.DebugReturn(DebugLevel.INFO, "OpGetGameList not sent. Lobby cannot be null.");
            }
            return false;
        }

        if (lobby.Type != LobbyType.SqlLobby)
        {
            if (this.DebugOut >= DebugLevel.INFO)
            {
                this.Listener.DebugReturn(DebugLevel.INFO, "OpGetGameList not sent. LobbyType must be SqlLobby.");
            }
            return false;
        }

        Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
        opParameters[(byte)ParameterCode.LobbyName] = lobby.Name;
        opParameters[(byte)ParameterCode.LobbyType] = (byte)lobby.Type;
        opParameters[(byte)ParameterCode.Data] = queryData;

        return this.OpCustom(OperationCode.GetGameList, opParameters, true);
    }

    /// <summary>
    /// Request the rooms and online status for a list of friends (each client must set a unique username via OpAuthenticate).
    /// </summary>
    /// <remarks>
    /// Used on Master Server to find the rooms played by a selected list of users.
    /// Users identify themselves by using OpAuthenticate with a unique username.
    /// The list of usernames must be fetched from some other source (not provided by Photon).
    ///
    /// The server response includes 2 arrays of info (each index matching a friend from the request):
    /// ParameterCode.FindFriendsResponseOnlineList = bool[] of online states
    /// ParameterCode.FindFriendsResponseRoomIdList = string[] of room names (empty string if not in a room)
    /// </remarks>
    /// <param name="friendsToFind">Array of friend's names (make sure they are unique).</param>
    /// <returns>If the operation could be sent (requires connection).</returns>
    public virtual bool OpFindFriends(string[] friendsToFind)
    {
        Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
        if (friendsToFind != null && friendsToFind.Length > 0)
        {
            opParameters[ParameterCode.FindFriendsRequestList] = friendsToFind;
        }

        return this.OpCustom(OperationCode.FindFriends, opParameters, true);
    }

    public bool OpSetCustomPropertiesOfActor(int actorNr, Hashtable actorProperties)
    {
        return this.OpSetPropertiesOfActor(actorNr, actorProperties.StripToStringKeys(), null);
    }

    /// <summary>
    /// Sets properties of a player / actor.
    /// Internally this uses OpSetProperties, which can be used to either set room or player properties.
    /// </summary>
    /// <param name="actorNr">The payer ID (a.k.a. actorNumber) of the player to attach these properties to.</param>
    /// <param name="actorProperties">The properties to add or update.</param>
    /// <param name="expectedProperties">If set, these must be in the current properties-set (on the server) to set actorProperties: CAS.</param>
    /// <param name="webForward">Set to true, to forward the set properties to a WebHook, defined for this app (in Dashboard).</param>
    /// <returns>If the operation could be sent (requires connection).</returns>
    protected internal bool OpSetPropertiesOfActor(int actorNr, Hashtable actorProperties, Hashtable expectedProperties = null, bool webForward = false)
    {
        if (this.DebugOut >= DebugLevel.INFO)
        {
            this.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfActor()");
        }

        if (actorNr <= 0 || actorProperties == null)
        {
            if (this.DebugOut >= DebugLevel.INFO)
            {
                this.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfActor not sent. ActorNr must be > 0 and actorProperties != null.");
            }
            return false;
        }

        Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
        opParameters.Add(ParameterCode.Properties, actorProperties);
        opParameters.Add(ParameterCode.ActorNr, actorNr);
        opParameters.Add(ParameterCode.Broadcast, true);
        if (expectedProperties != null && expectedProperties.Count != 0)
        {
            opParameters.Add(ParameterCode.ExpectedValues, expectedProperties);
        }

        if (webForward)
        {
            opParameters[ParameterCode.EventForward] = true;
        }

        return this.OpCustom((byte)OperationCode.SetProperties, opParameters, true, 0, false);
    }


    protected internal void OpSetPropertyOfRoom(byte propCode, object value)
    {
        Hashtable properties = new Hashtable();
        properties[propCode] = value;
        this.OpSetPropertiesOfRoom(properties, expectedProperties: null, webForward: false);
    }

    public bool OpSetCustomPropertiesOfRoom(Hashtable gameProperties, bool broadcast, byte channelId)
    {
        return this.OpSetPropertiesOfRoom(gameProperties.StripToStringKeys(), expectedProperties: null, webForward: false);
    }

    /// <summary>
    /// Sets properties of a room.
    /// Internally this uses OpSetProperties, which can be used to either set room or player properties.
    /// </summary>
    /// <param name="gameProperties">The properties to add or update.</param>
    /// <param name="expectedProperties">The properties expected when update occurs. (CAS : "Check And Swap")</param>
    /// <param name="webForward">"WebFlag" to indicate if request should be forwarded as "PathProperties" webhook or not.</param>
    /// <returns>If the operation could be sent (has to be connected).</returns>
    protected internal bool OpSetPropertiesOfRoom(Hashtable gameProperties, Hashtable expectedProperties = null, bool webForward = false)
    {
        if (this.DebugOut >= DebugLevel.INFO)
        {
            this.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfRoom()");
        }

        Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
        opParameters.Add(ParameterCode.Properties, gameProperties);
        opParameters.Add(ParameterCode.Broadcast, true);
        if (expectedProperties != null && expectedProperties.Count != 0)
        {
            opParameters.Add(ParameterCode.ExpectedValues, expectedProperties);
        }

        if (webForward)
        {
            opParameters[ParameterCode.EventForward] = true;
        }

        return this.OpCustom((byte)OperationCode.SetProperties, opParameters, true, 0, false);
    }

    /// <summary>
    /// Sends this app's appId and appVersion to identify this application server side.
    /// This is an async request which triggers a OnOperationResponse() call.
    /// </summary>
    /// <remarks>
    /// This operation makes use of encryption, if that is established before.
    /// See: EstablishEncryption(). Check encryption with IsEncryptionAvailable.
    /// This operation is allowed only once per connection (multiple calls will have ErrorCode != Ok).
    /// </remarks>
    /// <param name="appId">Your application's name or ID to authenticate. This is assigned by Photon Cloud (webpage).</param>
    /// <param name="appVersion">The client's version (clients with differing client appVersions are separated and players don't meet).</param>
    /// <param name="authValues">Contains all values relevant for authentication. Even without account system (external Custom Auth), the clients are allowed to identify themselves.</param>
    /// <param name="regionCode">Optional region code, if the client should connect to a specific Photon Cloud Region.</param>
    /// <param name="getLobbyStatistics">Set to true on Master Server to receive "Lobby Statistics" events.</param>
    /// <returns>If the operation could be sent (has to be connected).</returns>
    public virtual bool OpAuthenticate(string appId, string appVersion, AuthenticationValues authValues, string regionCode, bool getLobbyStatistics)
    {
        if (this.DebugOut >= DebugLevel.INFO)
        {
            this.Listener.DebugReturn(DebugLevel.INFO, "OpAuthenticate()");
        }

        Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
        if (getLobbyStatistics)
        {
            // must be sent in operation, even if a Token is available
            opParameters[ParameterCode.LobbyStats] = true;
        }

        // shortcut, if we have a Token
        if (authValues != null && authValues.Token != null)
        {
            opParameters[ParameterCode.Secret] = authValues.Token;
            return this.OpCustom(OperationCode.Authenticate, opParameters, true, (byte)0, false);   // we don't have to encrypt, when we have a token (which is encrypted)
        }


        // without a token, we send a complete op auth

        opParameters[ParameterCode.AppVersion] = appVersion;
        opParameters[ParameterCode.ApplicationId] = appId;

        if (!string.IsNullOrEmpty(regionCode))
        {
            opParameters[ParameterCode.Region] = regionCode;
        }

        if (authValues != null)
        {

            if (!string.IsNullOrEmpty(authValues.UserId))
            {
                opParameters[ParameterCode.UserId] = authValues.UserId;
            }

            if (authValues.AuthType != CustomAuthenticationType.None)
            {
                if (!this.IsProtocolSecure && !this.IsEncryptionAvailable)
                {
                    this.Listener.DebugReturn(DebugLevel.ERROR, "OpAuthenticate() failed. When you want Custom Authentication encryption is mandatory.");
                    return false;
                }

                opParameters[ParameterCode.ClientAuthenticationType] = (byte)authValues.AuthType;
                if (!string.IsNullOrEmpty(authValues.Token))
                {
                    opParameters[ParameterCode.Secret] = authValues.Token;
                }
                else
                {
                    if (!string.IsNullOrEmpty(authValues.AuthGetParameters))
                    {
                        opParameters[ParameterCode.ClientAuthenticationParams] = authValues.AuthGetParameters;
                    }
                    if (authValues.AuthPostData != null)
                    {
                        opParameters[ParameterCode.ClientAuthenticationData] = authValues.AuthPostData;
                    }
                }
            }
        }

        bool sent = this.OpCustom(OperationCode.Authenticate, opParameters, true, (byte)0, this.IsEncryptionAvailable);
        if (!sent)
        {
            this.Listener.DebugReturn(DebugLevel.ERROR, "Error calling OpAuthenticate! Did not work. Check log output, AuthValues and if you're connected.");
        }
        return sent;
    }


    /// <summary>
    /// Sends this app's appId and appVersion to identify this application server side.
    /// This is an async request which triggers a OnOperationResponse() call.
    /// </summary>
    /// <remarks>
    /// This operation makes use of encryption, if that is established before.
    /// See: EstablishEncryption(). Check encryption with IsEncryptionAvailable.
    /// This operation is allowed only once per connection (multiple calls will have ErrorCode != Ok).
    /// </remarks>
    /// <param name="appId">Your application's name or ID to authenticate. This is assigned by Photon Cloud (webpage).</param>
    /// <param name="appVersion">The client's version (clients with differing client appVersions are separated and players don't meet).</param>
    /// <param name="authValues">Optional authentication values. The client can set no values or a UserId or some parameters for Custom Authentication by a server.</param>
    /// <param name="regionCode">Optional region code, if the client should connect to a specific Photon Cloud Region.</param>
    /// <param name="encryptionMode"></param>
    /// <param name="expectedProtocol"></param>
    /// <returns>If the operation could be sent (has to be connected).</returns>
    public virtual bool OpAuthenticateOnce(string appId, string appVersion, AuthenticationValues authValues, string regionCode, EncryptionMode encryptionMode, ConnectionProtocol expectedProtocol)
    {
        if (this.DebugOut >= DebugLevel.INFO)
        {
            this.Listener.DebugReturn(DebugLevel.INFO, "OpAuthenticate()");
        }


        var opParameters = new Dictionary<byte, object>();

        // shortcut, if we have a Token
        if (authValues != null && authValues.Token != null)
        {
            opParameters[ParameterCode.Secret] = authValues.Token;
            return this.OpCustom(OperationCode.AuthenticateOnce, opParameters, true, (byte)0, false);   // we don't have to encrypt, when we have a token (which is encrypted)
        }

        if (encryptionMode == EncryptionMode.DatagramEncryption && expectedProtocol != ConnectionProtocol.Udp)
        {
            Debug.LogWarning("Expected protocol set to UDP, due to encryption mode DatagramEncryption. Changing protocol in PhotonServerSettings from: " + PhotonNetwork.PhotonServerSettings.Protocol);
            PhotonNetwork.PhotonServerSettings.Protocol = ConnectionProtocol.Udp;
            expectedProtocol = ConnectionProtocol.Udp;
        }

        opParameters[ParameterCode.ExpectedProtocol] = (byte)expectedProtocol;
        opParameters[ParameterCode.EncryptionMode] = (byte)encryptionMode;

        opParameters[ParameterCode.AppVersion] = appVersion;
        opParameters[ParameterCode.ApplicationId] = appId;

        if (!string.IsNullOrEmpty(regionCode))
        {
            opParameters[ParameterCode.Region] = regionCode;
        }

        if (authValues != null)
        {
            if (!string.IsNullOrEmpty(authValues.UserId))
            {
                opParameters[ParameterCode.UserId] = authValues.UserId;
            }

            if (authValues.AuthType != CustomAuthenticationType.None)
            {
                opParameters[ParameterCode.ClientAuthenticationType] = (byte)authValues.AuthType;
                if (!string.IsNullOrEmpty(authValues.Token))
                {
                    opParameters[ParameterCode.Secret] = authValues.Token;
                }
                else
                {
                    if (!string.IsNullOrEmpty(authValues.AuthGetParameters))
                    {
                        opParameters[ParameterCode.ClientAuthenticationParams] = authValues.AuthGetParameters;
                    }
                    if (authValues.AuthPostData != null)
                    {
                        opParameters[ParameterCode.ClientAuthenticationData] = authValues.AuthPostData;
                    }
                }
            }
        }

        return this.OpCustom(OperationCode.AuthenticateOnce, opParameters, true, (byte)0, this.IsEncryptionAvailable);
    }

    /// <summary>
    /// Operation to handle this client's interest groups (for events in room).
    /// </summary>
    /// <remarks>
    /// Note the difference between passing null and byte[0]:
    ///   null won't add/remove any groups.
    ///   byte[0] will add/remove all (existing) groups.
    /// First, removing groups is executed. This way, you could leave all groups and join only the ones provided.
    ///
    /// Changes become active not immediately but when the server executes this operation (approximately RTT/2).
    /// </remarks>
    /// <param name="groupsToRemove">Groups to remove from interest. Null will not remove any. A byte[0] will remove all.</param>
    /// <param name="groupsToAdd">Groups to add to interest. Null will not add any. A byte[0] will add all current.</param>
    /// <returns>If operation could be enqueued for sending. Sent when calling: Service or SendOutgoingCommands.</returns>
    public virtual bool OpChangeGroups(byte[] groupsToRemove, byte[] groupsToAdd)
    {
        if (this.DebugOut >= DebugLevel.ALL)
        {
            this.Listener.DebugReturn(DebugLevel.ALL, "OpChangeGroups()");
        }

        Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
        if (groupsToRemove != null)
        {
            opParameters[(byte)ParameterCode.Remove] = groupsToRemove;
        }
        if (groupsToAdd != null)
        {
            opParameters[(byte)ParameterCode.Add] = groupsToAdd;
        }

        return this.OpCustom((byte)OperationCode.ChangeGroups, opParameters, true, 0);
    }


    /// <summary>
    /// Send an event with custom code/type and any content to the other players in the same room.
    /// </summary>
    /// <remarks>This override explicitly uses another parameter order to not mix it up with the implementation for Hashtable only.</remarks>
    /// <param name="eventCode">Identifies this type of event (and the content). Your game's event codes can start with 0.</param>
    /// <param name="customEventContent">Any serializable datatype (including Hashtable like the other OpRaiseEvent overloads).</param>
    /// <param name="sendReliable">If this event has to arrive reliably (potentially repeated if it's lost).</param>
    /// <param name="raiseEventOptions">Contains (slightly) less often used options. If you pass null, the default options will be used.</param>
    /// <returns>If operation could be enqueued for sending. Sent when calling: Service or SendOutgoingCommands.</returns>
    public virtual bool OpRaiseEvent(byte eventCode, object customEventContent, bool sendReliable, RaiseEventOptions raiseEventOptions)
    {
        this.opParameters.Clear(); // re-used private variable to avoid many new Dictionary() calls (garbage collection)
        this.opParameters[(byte)ParameterCode.Code] = (byte)eventCode;
        if (customEventContent != null)
        {
            this.opParameters[(byte)ParameterCode.Data] = customEventContent;
        }

        if (raiseEventOptions == null)
        {
            raiseEventOptions = RaiseEventOptions.Default;
        }
        else
        {
            if (raiseEventOptions.CachingOption != EventCaching.DoNotCache)
            {
                this.opParameters[(byte)ParameterCode.Cache] = (byte)raiseEventOptions.CachingOption;
            }
            if (raiseEventOptions.Receivers != ReceiverGroup.Others)
            {
                this.opParameters[(byte)ParameterCode.ReceiverGroup] = (byte)raiseEventOptions.Receivers;
            }
            if (raiseEventOptions.InterestGroup != 0)
            {
                this.opParameters[(byte)ParameterCode.Group] = (byte)raiseEventOptions.InterestGroup;
            }
            if (raiseEventOptions.TargetActors != null)
            {
                this.opParameters[(byte)ParameterCode.ActorList] = raiseEventOptions.TargetActors;
            }
            if (raiseEventOptions.ForwardToWebhook)
            {
                this.opParameters[(byte)ParameterCode.EventForward] = true; //TURNBASED
            }
        }

        return this.OpCustom((byte)OperationCode.RaiseEvent, this.opParameters, sendReliable, raiseEventOptions.SequenceChannel, raiseEventOptions.Encrypt);
    }


#if PHOTON_LIB_MIN_4_1_2
        /// <summary>
        /// Send an event with custom code/type and any content to the other players in the same room.
        /// </summary>
        /// <remarks>This override explicitly uses another parameter order to not mix it up with the implementation for Hashtable only.</remarks>
        /// <param name="eventCode">Identifies this type of event (and the content). Your game's event codes can start with 0.</param>
        /// <param name="customEventContent">Any serializable datatype (including Hashtable like the other OpRaiseEvent overloads).</param>
        /// <param name="raiseEventOptions">Contains (slightly) less often used options. If you pass null, the default options will be used.</param>
        /// <param name="sendOptions">Send options wrap up reliability, sequencing and channel.</param>
        /// <returns>If operation could be enqueued for sending. Sent when calling: Service or SendOutgoingCommands.</returns>
        public virtual bool OpRaiseEvent(byte eventCode, object customEventContent, RaiseEventOptions raiseEventOptions, SendOptions sendOptions)
        {
            this.opParameters.Clear(); // re-used private variable to avoid many new Dictionary() calls (garbage collection)
            this.opParameters[(byte)ParameterCode.Code] = (byte)eventCode;
            if (customEventContent != null)
            {
                this.opParameters[(byte)ParameterCode.Data] = customEventContent;
            }

            if (raiseEventOptions == null)
            {
                raiseEventOptions = RaiseEventOptions.Default;
            }
            else
            {
                if (sendOptions.Channel != raiseEventOptions.SequenceChannel || sendOptions.Encrypt != raiseEventOptions.Encrypt)
                {
                    // TODO: This should be a one-time warning. 
                    // NOTE: Later on, it will be impossible to mix up SendOptions and RaiseEventOptions, as they won't have overlapping settings.
                    this.Listener.DebugReturn(DebugLevel.WARNING, "You are using RaiseEventOptions and SendOptions with conflicting settings. Please check channel and encryption value.");
                }

                if (raiseEventOptions.CachingOption != EventCaching.DoNotCache)
                {
                    this.opParameters[(byte)ParameterCode.Cache] = (byte)raiseEventOptions.CachingOption;
                }
                if (raiseEventOptions.Receivers != ReceiverGroup.Others)
                {
                    this.opParameters[(byte)ParameterCode.ReceiverGroup] = (byte)raiseEventOptions.Receivers;
                }
                if (raiseEventOptions.InterestGroup != 0)
                {
                    this.opParameters[(byte)ParameterCode.Group] = (byte)raiseEventOptions.InterestGroup;
                }
                if (raiseEventOptions.TargetActors != null)
                {
                    this.opParameters[(byte)ParameterCode.ActorList] = raiseEventOptions.TargetActors;
                }
                //if (raiseEventOptions.Flags.HttpForward)
                //{
                //    this.opParameters[(byte)ParameterCode.EventForward] = raiseEventOptions.Flags.WebhookFlags; //TURNBASED
                //}
            }

            return this.SendOperation(OperationCode.RaiseEvent, this.opParameters, sendOptions);
        }
#endif

    /// <summary>
    /// Internally used operation to set some "per server" settings. This is for the Master Server.
    /// </summary>
    /// <param name="receiveLobbyStats">Set to true, to get Lobby Statistics (lists of existing lobbies).</param>
    /// <returns>False if the operation could not be sent.</returns>
    public virtual bool OpSettings(bool receiveLobbyStats)
    {
        if (this.DebugOut >= DebugLevel.ALL)
        {
            this.Listener.DebugReturn(DebugLevel.ALL, "OpSettings()");
        }

        // re-used private variable to avoid many new Dictionary() calls (garbage collection)
        this.opParameters.Clear();

        // implementation for Master Server:
        if (receiveLobbyStats)
        {
            this.opParameters[(byte)0] = receiveLobbyStats;
        }

        if (this.opParameters.Count == 0)
        {
            // no need to send op in case we set the default values
            return true;
        }
        return this.OpCustom((byte)OperationCode.ServerSettings, this.opParameters, true);
    }
}




/// <summary>
/// Container for user authentication in Photon. Set AuthValues before you connect - all else is handled.
/// </summary>
/// <remarks>
/// On Photon, user authentication is optional but can be useful in many cases.
/// If you want to FindFriends, a unique ID per user is very practical.
///
/// There are basically three options for user authentification: None at all, the client sets some UserId
/// or you can use some account web-service to authenticate a user (and set the UserId server-side).
///
/// Custom Authentication lets you verify end-users by some kind of login or token. It sends those
/// values to Photon which will verify them before granting access or disconnecting the client.
///
/// The AuthValues are sent in OpAuthenticate when you connect, so they must be set before you connect.
/// Should you not set any AuthValues, PUN will create them and set the playerName as userId in them.
/// If the AuthValues.userId is null or empty when it's sent to the server, then the Photon Server assigns a userId!
///
/// The Photon Cloud Dashboard will let you enable this feature and set important server values for it.
/// https://www.photonengine.com/dashboard
/// </remarks>

