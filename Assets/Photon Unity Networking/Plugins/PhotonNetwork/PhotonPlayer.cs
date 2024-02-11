using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

// Token: 0x020002DD RID: 733
public class PhotonPlayer : IComparable<PhotonPlayer>, IComparable<int>, IEquatable<PhotonPlayer>, IEquatable<int>
{
	// Token: 0x06001BDF RID: 7135 RVA: 0x00014270 File Offset: 0x00012470
	public PhotonPlayer(bool isLocal, int actorID, string name)
	{
		this.CustomProperties = new Hashtable();
		this.IsLocal = isLocal;
		this.actorID = actorID;
		this.nameField = name;
	}

	// Token: 0x06001BE0 RID: 7136 RVA: 0x000142AA File Offset: 0x000124AA
	protected internal PhotonPlayer(bool isLocal, int actorID, Hashtable properties)
	{
		this.CustomProperties = new Hashtable();
		this.IsLocal = isLocal;
		this.actorID = actorID;
		this.InternalCacheProperties(properties);
	}

	// Token: 0x170003D8 RID: 984
	// (get) Token: 0x06001BE1 RID: 7137 RVA: 0x000142E4 File Offset: 0x000124E4
	public int ID
	{
		get
		{
			return this.actorID;
		}
	}

	// Token: 0x170003D9 RID: 985
	// (get) Token: 0x06001BE2 RID: 7138 RVA: 0x000142EC File Offset: 0x000124EC
	// (set) Token: 0x06001BE3 RID: 7139 RVA: 0x000B2A14 File Offset: 0x000B0C14
	public string NickName
	{
		get
		{
			return this.nameField;
		}
		set
		{
			if (!this.IsLocal)
			{
				Debug.LogError("Error: Cannot change the name of a remote player!");
				return;
			}
			if (string.IsNullOrEmpty(value) || value.Equals(this.nameField))
			{
				return;
			}
			this.nameField = value;
			PhotonNetwork.playerName = value;
		}
	}

	// Token: 0x170003DA RID: 986
	// (get) Token: 0x06001BE4 RID: 7140 RVA: 0x000142F4 File Offset: 0x000124F4
	// (set) Token: 0x06001BE5 RID: 7141 RVA: 0x000142FC File Offset: 0x000124FC
	public string UserId { get; internal set; }

	// Token: 0x170003DB RID: 987
	// (get) Token: 0x06001BE6 RID: 7142 RVA: 0x00014305 File Offset: 0x00012505
	public bool IsMasterClient
	{
		get
		{
			return PhotonNetwork.networkingPeer.mMasterClientId == this.ID;
		}
	}

	// Token: 0x170003DC RID: 988
	// (get) Token: 0x06001BE7 RID: 7143 RVA: 0x00014319 File Offset: 0x00012519
	// (set) Token: 0x06001BE8 RID: 7144 RVA: 0x00014321 File Offset: 0x00012521
	public bool IsInactive { get; set; }

	// Token: 0x170003DD RID: 989
	// (get) Token: 0x06001BE9 RID: 7145 RVA: 0x0001432A File Offset: 0x0001252A
	// (set) Token: 0x06001BEA RID: 7146 RVA: 0x00014332 File Offset: 0x00012532
	public Hashtable CustomProperties { get; internal set; }

	// Token: 0x170003DE RID: 990
	// (get) Token: 0x06001BEB RID: 7147 RVA: 0x000B2A64 File Offset: 0x000B0C64
	public Hashtable AllProperties
	{
		get
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Merge(this.CustomProperties);
			hashtable[ActorProperties.PlayerName] = this.NickName;
			return hashtable;
		}
	}

	// Token: 0x06001BEC RID: 7148 RVA: 0x000B2A9C File Offset: 0x000B0C9C
	public override bool Equals(object p)
	{
		PhotonPlayer photonPlayer = p as PhotonPlayer;
		return photonPlayer != null && this.GetHashCode() == photonPlayer.GetHashCode();
	}

	// Token: 0x06001BED RID: 7149 RVA: 0x0001433B File Offset: 0x0001253B
	public override int GetHashCode()
	{
		return this.ID;
	}

	// Token: 0x06001BEE RID: 7150 RVA: 0x00014343 File Offset: 0x00012543
	internal void InternalChangeLocalID(int newID)
	{
		if (!this.IsLocal)
		{
			Debug.LogError("ERROR You should never change PhotonPlayer IDs!");
			return;
		}
		this.actorID = newID;
	}

	// Token: 0x06001BEF RID: 7151 RVA: 0x000B2AC8 File Offset: 0x000B0CC8
	internal void InternalCacheProperties(Hashtable properties)
	{
		if (properties == null || properties.Count == 0 || this.CustomProperties.Equals(properties))
		{
			return;
		}
		if (properties.ContainsKey(ActorProperties.PlayerName))
		{
			this.nameField = (string)properties[ActorProperties.PlayerName];
		}
		if (properties.ContainsKey(ActorProperties.UserId))
		{
			this.UserId = (string)properties[ActorProperties.UserId];
		}
		if (properties.ContainsKey(ActorProperties.IsInactive))
		{
			this.IsInactive = (bool)properties[ActorProperties.IsInactive];
		}
		this.CustomProperties.MergeStringKeys(properties);
		this.CustomProperties.StripKeysWithNullValues();
	}

	// Token: 0x06001BF0 RID: 7152 RVA: 0x000B2BA0 File Offset: 0x000B0DA0
	public void SetCustomProperties(Hashtable propertiesToSet, Hashtable expectedValues = null, bool webForward = false)
	{
		nProfiler.BeginSample("PhotonPlayer.SetCustomProperties");
		if (propertiesToSet == null)
		{
			return;
		}
		Hashtable hashtable = propertiesToSet.StripToStringKeys();
		Hashtable hashtable2 = expectedValues.StripToStringKeys();
		bool flag = hashtable2 == null || hashtable2.Count == 0;
		bool flag2 = this.actorID > 0 && !PhotonNetwork.offlineMode;
		if (flag)
		{
			this.CustomProperties.Merge(hashtable);
			this.CustomProperties.StripKeysWithNullValues();
		}
		if (flag2)
		{
			PhotonNetwork.networkingPeer.OpSetPropertiesOfActor(this.actorID, hashtable, hashtable2, webForward);
		}
		if (!flag2 || flag)
		{
			this.InternalCacheProperties(hashtable);
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, new object[]
			{
				this,
				hashtable
			});
		}
		nProfiler.EndSample();
	}

	// Token: 0x06001BF1 RID: 7153 RVA: 0x00014362 File Offset: 0x00012562
	public static PhotonPlayer Find(int ID)
	{
		if (PhotonNetwork.networkingPeer != null)
		{
			return PhotonNetwork.networkingPeer.GetPlayerWithId(ID);
		}
		return null;
	}

	// Token: 0x06001BF2 RID: 7154 RVA: 0x0001437B File Offset: 0x0001257B
	public PhotonPlayer Get(int id)
	{
		return PhotonPlayer.Find(id);
	}

	// Token: 0x06001BF3 RID: 7155 RVA: 0x00014383 File Offset: 0x00012583
	public PhotonPlayer GetNext()
	{
		return this.GetNextFor(this.ID);
	}

	// Token: 0x06001BF4 RID: 7156 RVA: 0x00014391 File Offset: 0x00012591
	public PhotonPlayer GetNextFor(PhotonPlayer currentPlayer)
	{
		if (currentPlayer == null)
		{
			return null;
		}
		return this.GetNextFor(currentPlayer.ID);
	}

	// Token: 0x06001BF5 RID: 7157 RVA: 0x000B2C5C File Offset: 0x000B0E5C
	public PhotonPlayer GetNextFor(int currentPlayerId)
	{
		if (PhotonNetwork.networkingPeer == null || PhotonNetwork.networkingPeer.mActors == null || PhotonNetwork.networkingPeer.mActors.Count < 2)
		{
			return null;
		}
		Dictionary<int, PhotonPlayer> mActors = PhotonNetwork.networkingPeer.mActors;
		int num = int.MaxValue;
		int num2 = currentPlayerId;
		foreach (int num3 in mActors.Keys)
		{
			if (num3 < num2)
			{
				num2 = num3;
			}
			else if (num3 > currentPlayerId && num3 < num)
			{
				num = num3;
			}
		}
		return (num == int.MaxValue) ? mActors[num2] : mActors[num];
	}

	// Token: 0x06001BF6 RID: 7158 RVA: 0x000B2D30 File Offset: 0x000B0F30
	public int CompareTo(PhotonPlayer other)
	{
		if (other == null)
		{
			return 0;
		}
		return this.GetHashCode().CompareTo(other.GetHashCode());
	}

	// Token: 0x06001BF7 RID: 7159 RVA: 0x000B2D5C File Offset: 0x000B0F5C
	public int CompareTo(int other)
	{
		return this.GetHashCode().CompareTo(other);
	}

	// Token: 0x06001BF8 RID: 7160 RVA: 0x000B2D78 File Offset: 0x000B0F78
	public bool Equals(PhotonPlayer other)
	{
		return other != null && this.GetHashCode().Equals(other.GetHashCode());
	}

	// Token: 0x06001BF9 RID: 7161 RVA: 0x000B2DA4 File Offset: 0x000B0FA4
	public bool Equals(int other)
	{
		return this.GetHashCode().Equals(other);
	}

	// Token: 0x06001BFA RID: 7162 RVA: 0x000B2DC0 File Offset: 0x000B0FC0
	public override string ToString()
	{
		if (string.IsNullOrEmpty(this.NickName))
		{
			return string.Format("#{0:00}{1}{2}", this.ID, (!this.IsInactive) ? " " : " (inactive)", (!this.IsMasterClient) ? string.Empty : "(master)");
		}
		return string.Format("'{0}'{1}{2}", this.NickName, (!this.IsInactive) ? " " : " (inactive)", (!this.IsMasterClient) ? string.Empty : "(master)");
	}

	// Token: 0x06001BFB RID: 7163 RVA: 0x000B2E6C File Offset: 0x000B106C
	public string ToStringFull()
	{
		return string.Format("#{0:00} '{1}'{2} {3}", new object[]
		{
			this.ID,
			this.NickName,
			(!this.IsInactive) ? string.Empty : " (inactive)",
			this.CustomProperties.ToStringFull()
		});
	}

	// Token: 0x170003DF RID: 991
	// (get) Token: 0x06001BFC RID: 7164 RVA: 0x000143A7 File Offset: 0x000125A7
	// (set) Token: 0x06001BFD RID: 7165 RVA: 0x000143AF File Offset: 0x000125AF
	[Obsolete("Please use NickName (updated case for naming).")]
	public string name
	{
		get
		{
			return this.NickName;
		}
		set
		{
			this.NickName = value;
		}
	}

	// Token: 0x170003E0 RID: 992
	// (get) Token: 0x06001BFE RID: 7166 RVA: 0x000143B8 File Offset: 0x000125B8
	// (set) Token: 0x06001BFF RID: 7167 RVA: 0x000143C0 File Offset: 0x000125C0
	[Obsolete("Please use UserId (updated case for naming).")]
	public string userId
	{
		get
		{
			return this.UserId;
		}
		internal set
		{
			this.UserId = value;
		}
	}

	// Token: 0x170003E1 RID: 993
	// (get) Token: 0x06001C00 RID: 7168 RVA: 0x000143C9 File Offset: 0x000125C9
	[Obsolete("Please use IsLocal (updated case for naming).")]
	public bool isLocal
	{
		get
		{
			return this.IsLocal;
		}
	}

	// Token: 0x170003E2 RID: 994
	// (get) Token: 0x06001C01 RID: 7169 RVA: 0x000143D1 File Offset: 0x000125D1
	[Obsolete("Please use IsMasterClient (updated case for naming).")]
	public bool isMasterClient
	{
		get
		{
			return this.IsMasterClient;
		}
	}

	// Token: 0x170003E3 RID: 995
	// (get) Token: 0x06001C02 RID: 7170 RVA: 0x000143D9 File Offset: 0x000125D9
	// (set) Token: 0x06001C03 RID: 7171 RVA: 0x000143E1 File Offset: 0x000125E1
	[Obsolete("Please use IsInactive (updated case for naming).")]
	public bool isInactive
	{
		get
		{
			return this.IsInactive;
		}
		set
		{
			this.IsInactive = value;
		}
	}

	// Token: 0x170003E4 RID: 996
	// (get) Token: 0x06001C04 RID: 7172 RVA: 0x000143EA File Offset: 0x000125EA
	// (set) Token: 0x06001C05 RID: 7173 RVA: 0x000143F2 File Offset: 0x000125F2
	[Obsolete("Please use CustomProperties (updated case for naming).")]
	public Hashtable customProperties
	{
		get
		{
			return this.CustomProperties;
		}
		internal set
		{
			this.CustomProperties = value;
		}
	}

	// Token: 0x170003E5 RID: 997
	// (get) Token: 0x06001C06 RID: 7174 RVA: 0x000143FB File Offset: 0x000125FB
	[Obsolete("Please use AllProperties (updated case for naming).")]
	public Hashtable allProperties
	{
		get
		{
			return this.AllProperties;
		}
	}

	// Token: 0x04001020 RID: 4128
	private int actorID = -1;

	// Token: 0x04001021 RID: 4129
	private string nameField = string.Empty;

	// Token: 0x04001022 RID: 4130
	public readonly bool IsLocal;

	// Token: 0x04001023 RID: 4131
	public object TagObject;
}
