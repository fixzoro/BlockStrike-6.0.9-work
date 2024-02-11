using System;
using ExitGames.Client.Photon;
using UnityEngine;

// Token: 0x020002E8 RID: 744
public class Room : RoomInfo
{
    // Token: 0x06001C45 RID: 7237 RVA: 0x000B3A4C File Offset: 0x000B1C4C
    internal Room(string roomName, RoomOptions options) : base(roomName, null)
    {
        if (options == null)
        {
            options = new RoomOptions();
        }
        this.visibleField = options.IsVisible;
        this.openField = options.IsOpen;
        this.maxPlayersField = options.MaxPlayers;
        this.autoCleanUpField = options.CleanupCacheOnLeave;
        base.InternalCacheProperties(options.CustomRoomProperties);
        this.PropertiesListedInLobby = options.CustomRoomPropertiesForLobby;
    }

    // Token: 0x170003F3 RID: 1011
    // (get) Token: 0x06001C46 RID: 7238 RVA: 0x0001470E File Offset: 0x0001290E
    // (set) Token: 0x06001C47 RID: 7239 RVA: 0x00014716 File Offset: 0x00012916
    public new string Name
    {
        get
        {
            return this.nameField;
        }
        internal set
        {
            this.nameField = value;
        }
    }

    // Token: 0x170003F4 RID: 1012
    // (get) Token: 0x06001C48 RID: 7240 RVA: 0x0001471F File Offset: 0x0001291F
    // (set) Token: 0x06001C49 RID: 7241 RVA: 0x000B3AB4 File Offset: 0x000B1CB4
    public new bool IsOpen
    {
        get
        {
            return this.openField;
        }
        set
        {
            if (!this.Equals(PhotonNetwork.room))
            {
                Debug.LogWarning("Can't set open when not in that room.");
            }
            if (value != this.openField && !PhotonNetwork.offlineMode)
            {
                PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(new Hashtable
                {
                    {
                        GamePropertyKey.IsOpen,
                        value
                    }
                }, null, false);
            }
            this.openField = value;
        }
    }

    // Token: 0x170003F5 RID: 1013
    // (get) Token: 0x06001C4A RID: 7242 RVA: 0x00014727 File Offset: 0x00012927
    // (set) Token: 0x06001C4B RID: 7243 RVA: 0x000B3B18 File Offset: 0x000B1D18
    public new bool IsVisible
    {
        get
        {
            return this.visibleField;
        }
        set
        {
            if (!this.Equals(PhotonNetwork.room))
            {
                Debug.LogWarning("Can't set visible when not in that room.");
            }
            if (value != this.visibleField && !PhotonNetwork.offlineMode)
            {
                PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(new Hashtable
                {
                    {
                        GamePropertyKey.IsVisible,
                        value
                    }
                }, null, false);
            }
            this.visibleField = value;
        }
    }

    // Token: 0x170003F6 RID: 1014
    // (get) Token: 0x06001C4C RID: 7244 RVA: 0x0001472F File Offset: 0x0001292F
    // (set) Token: 0x06001C4D RID: 7245 RVA: 0x00014737 File Offset: 0x00012937
    public string[] PropertiesListedInLobby { get; private set; }

    // Token: 0x170003F7 RID: 1015
    // (get) Token: 0x06001C4E RID: 7246 RVA: 0x00014740 File Offset: 0x00012940
    public bool AutoCleanUp
    {
        get
        {
            return this.autoCleanUpField;
        }
    }

    // Token: 0x170003F8 RID: 1016
    // (get) Token: 0x06001C4F RID: 7247 RVA: 0x00014748 File Offset: 0x00012948
    // (set) Token: 0x06001C50 RID: 7248 RVA: 0x000B3B7C File Offset: 0x000B1D7C
    public new int MaxPlayers
    {
        get
        {
            return (int)this.maxPlayersField;
        }

        set
        {
            if (!this.Equals(PhotonNetwork.room))
            {
                UnityEngine.Debug.LogWarning("Can't set maxPlayers when not in that room.");
            }

            if (value > 255)
            {
                UnityEngine.Debug.LogWarning("Can't set Room.maxPlayers to: " + value + ". Using max value: 255.");
                value = 255;
            }

            if (value != this.maxPlayersField && !PhotonNetwork.offlineMode)
            {
                PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(new Hashtable() { { GamePropertyKey.MaxPlayers, (byte)value } }, expectedProperties: null, webForward: false);
            }

            this.maxPlayersField = (byte)value;
        }
    }

    // Token: 0x170003F9 RID: 1017
    // (get) Token: 0x06001C51 RID: 7249 RVA: 0x00014750 File Offset: 0x00012950
    public new int PlayerCount
    {
        get
        {
            if (PhotonNetwork.playerList != null)
            {
                return PhotonNetwork.playerList.Length;
            }
            return 0;
        }
    }

    // Token: 0x170003FA RID: 1018
    // (get) Token: 0x06001C52 RID: 7250 RVA: 0x00014762 File Offset: 0x00012962
    public string[] ExpectedUsers
    {
        get
        {
            return this.expectedUsersField;
        }
    }

    // Token: 0x170003FB RID: 1019
    // (get) Token: 0x06001C53 RID: 7251 RVA: 0x0001476A File Offset: 0x0001296A
    // (set) Token: 0x06001C54 RID: 7252 RVA: 0x000B3C34 File Offset: 0x000B1E34
    public int PlayerTtl
    {
        get
        {
            return this.playerTtlField;
        }
        set
        {
            if (!this.Equals(PhotonNetwork.room))
            {
                Debug.LogWarning("Can't set PlayerTtl when not in a room.");
            }
            if (value != this.playerTtlField && !PhotonNetwork.offlineMode)
            {
                PhotonNetwork.networkingPeer.OpSetPropertyOfRoom(246, value);
            }
            this.playerTtlField = value;
        }
    }

    // Token: 0x170003FC RID: 1020
    // (get) Token: 0x06001C55 RID: 7253 RVA: 0x00014772 File Offset: 0x00012972
    // (set) Token: 0x06001C56 RID: 7254 RVA: 0x000B3C84 File Offset: 0x000B1E84
    public int EmptyRoomTtl
    {
        get
        {
            return this.emptyRoomTtlField;
        }
        set
        {
            if (!this.Equals(PhotonNetwork.room))
            {
                Debug.LogWarning("Can't set EmptyRoomTtl when not in a room.");
            }
            if (value != this.emptyRoomTtlField && !PhotonNetwork.offlineMode)
            {
                PhotonNetwork.networkingPeer.OpSetPropertyOfRoom(245, value);
            }
            this.emptyRoomTtlField = value;
        }
    }

    // Token: 0x170003FD RID: 1021
    // (get) Token: 0x06001C57 RID: 7255 RVA: 0x0001477A File Offset: 0x0001297A
    // (set) Token: 0x06001C58 RID: 7256 RVA: 0x00014782 File Offset: 0x00012982
    protected internal int MasterClientId
    {
        get
        {
            return this.masterClientIdField;
        }
        set
        {
            this.masterClientIdField = value;
        }
    }

    // Token: 0x06001C59 RID: 7257 RVA: 0x000B3CD4 File Offset: 0x000B1ED4
    public void SetCustomProperties(Hashtable propertiesToSet, Hashtable expectedValues = null, bool webForward = false)
    {
        if (propertiesToSet == null)
        {
            return;
        }
        Hashtable hashtable = propertiesToSet.StripToStringKeys();
        Hashtable hashtable2 = expectedValues.StripToStringKeys();
        bool flag = hashtable2 == null || hashtable2.Count == 0;
        if (PhotonNetwork.offlineMode || flag)
        {
            base.CustomProperties.Merge(hashtable);
            base.CustomProperties.StripKeysWithNullValues();
        }
        if (!PhotonNetwork.offlineMode)
        {
            PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(hashtable, hashtable2, webForward);
        }
        if (PhotonNetwork.offlineMode || flag)
        {
            NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonCustomRoomPropertiesChanged, new object[]
            {
                hashtable
            });
        }
    }

    // Token: 0x06001C5A RID: 7258 RVA: 0x000B3D54 File Offset: 0x000B1F54
    public void SetPropertiesListedInLobby(string[] propsListedInLobby)
    {
        Hashtable hashtable = new Hashtable();
        hashtable[GamePropertyKey.PropsListedInLobby] = propsListedInLobby;
        PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(hashtable, null, false);
        this.PropertiesListedInLobby = propsListedInLobby;
    }

    // Token: 0x06001C5B RID: 7259 RVA: 0x000B3D90 File Offset: 0x000B1F90
    public void ClearExpectedUsers()
    {
        Hashtable hashtable = new Hashtable();
        hashtable[247] = new string[0];
        Hashtable hashtable2 = new Hashtable();
        hashtable2[247] = this.ExpectedUsers;
        PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(hashtable, hashtable2, false);
    }

    // Token: 0x06001C5C RID: 7260 RVA: 0x000B3DE4 File Offset: 0x000B1FE4
    public void SetExpectedUsers(string[] expectedUsers)
    {
        Hashtable hashtable = new Hashtable();
        hashtable[247] = expectedUsers;
        Hashtable hashtable2 = new Hashtable();
        hashtable2[247] = this.ExpectedUsers;
        PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(hashtable, hashtable2, false);
    }

    // Token: 0x06001C5D RID: 7261 RVA: 0x000B3E34 File Offset: 0x000B2034
    public override string ToString()
    {
        return string.Format("Room: '{0}' {1},{2} {4}/{3} players.", new object[]
        {
            this.nameField,
            (!this.visibleField) ? "hidden" : "visible",
            (!this.openField) ? "closed" : "open",
            this.maxPlayersField,
            this.PlayerCount
        });
    }

    // Token: 0x06001C5E RID: 7262 RVA: 0x000B3EA4 File Offset: 0x000B20A4
    public new string ToStringFull()
    {
        return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", new object[]
        {
            this.nameField,
            (!this.visibleField) ? "hidden" : "visible",
            (!this.openField) ? "closed" : "open",
            this.maxPlayersField,
            this.PlayerCount,
            base.CustomProperties.ToStringFull()
        });
    }

    // Token: 0x170003FE RID: 1022
    // (get) Token: 0x06001C5F RID: 7263 RVA: 0x0001478B File Offset: 0x0001298B
    // (set) Token: 0x06001C60 RID: 7264 RVA: 0x00014793 File Offset: 0x00012993
    [Obsolete("Please use Name (updated case for naming).")]
    public new string name
    {
        get
        {
            return this.Name;
        }
        internal set
        {
            this.Name = value;
        }
    }

    // Token: 0x170003FF RID: 1023
    // (get) Token: 0x06001C61 RID: 7265 RVA: 0x0001479C File Offset: 0x0001299C
    // (set) Token: 0x06001C62 RID: 7266 RVA: 0x000147A4 File Offset: 0x000129A4
    [Obsolete("Please use IsOpen (updated case for naming).")]
    public new bool open
    {
        get
        {
            return this.IsOpen;
        }
        set
        {
            this.IsOpen = value;
        }
    }

    // Token: 0x17000400 RID: 1024
    // (get) Token: 0x06001C63 RID: 7267 RVA: 0x000147AD File Offset: 0x000129AD
    // (set) Token: 0x06001C64 RID: 7268 RVA: 0x000147B5 File Offset: 0x000129B5
    [Obsolete("Please use IsVisible (updated case for naming).")]
    public new bool visible
    {
        get
        {
            return this.IsVisible;
        }
        set
        {
            this.IsVisible = value;
        }
    }

    // Token: 0x17000401 RID: 1025
    // (get) Token: 0x06001C65 RID: 7269 RVA: 0x000147BE File Offset: 0x000129BE
    // (set) Token: 0x06001C66 RID: 7270 RVA: 0x000147C6 File Offset: 0x000129C6
    [Obsolete("Please use PropertiesListedInLobby (updated case for naming).")]
    public string[] propertiesListedInLobby
    {
        get
        {
            return this.PropertiesListedInLobby;
        }
        private set
        {
            this.PropertiesListedInLobby = value;
        }
    }

    // Token: 0x17000402 RID: 1026
    // (get) Token: 0x06001C67 RID: 7271 RVA: 0x000147CF File Offset: 0x000129CF
    [Obsolete("Please use AutoCleanUp (updated case for naming).")]
    public bool autoCleanUp
    {
        get
        {
            return this.AutoCleanUp;
        }
    }

    // Token: 0x17000403 RID: 1027
    // (get) Token: 0x06001C68 RID: 7272 RVA: 0x000147D7 File Offset: 0x000129D7
    // (set) Token: 0x06001C69 RID: 7273 RVA: 0x000147DF File Offset: 0x000129DF
    [Obsolete("Please use MaxPlayers (updated case for naming).")]
    public new int maxPlayers
    {
        get
        {
            return this.MaxPlayers;
        }
        set
        {
            this.MaxPlayers = value;
        }
    }

    // Token: 0x17000404 RID: 1028
    // (get) Token: 0x06001C6A RID: 7274 RVA: 0x000147E8 File Offset: 0x000129E8
    [Obsolete("Please use PlayerCount (updated case for naming).")]
    public new int playerCount
    {
        get
        {
            return this.PlayerCount;
        }
    }

    // Token: 0x17000405 RID: 1029
    // (get) Token: 0x06001C6B RID: 7275 RVA: 0x000147F0 File Offset: 0x000129F0
    [Obsolete("Please use ExpectedUsers (updated case for naming).")]
    public string[] expectedUsers
    {
        get
        {
            return this.ExpectedUsers;
        }
    }

    // Token: 0x17000406 RID: 1030
    // (get) Token: 0x06001C6C RID: 7276 RVA: 0x000147F8 File Offset: 0x000129F8
    // (set) Token: 0x06001C6D RID: 7277 RVA: 0x00014800 File Offset: 0x00012A00
    [Obsolete("Please use MasterClientId (updated case for naming).")]
    protected internal int masterClientId
    {
        get
        {
            return this.MasterClientId;
        }
        set
        {
            this.MasterClientId = value;
        }
    }
}
