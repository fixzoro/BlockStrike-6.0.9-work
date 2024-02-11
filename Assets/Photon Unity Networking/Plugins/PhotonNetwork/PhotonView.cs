using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

// Token: 0x020002E3 RID: 739
[AddComponentMenu("Photon Networking/Photon View &v")]
public class PhotonView : Photon.MonoBehaviour
{
	// Token: 0x170003E6 RID: 998
	// (get) Token: 0x06001C0D RID: 7181 RVA: 0x000144A7 File Offset: 0x000126A7
	// (set) Token: 0x06001C0E RID: 7182 RVA: 0x000144D5 File Offset: 0x000126D5
	public int prefix
	{
		get
		{
			if (this.prefixBackup == -1 && PhotonNetwork.networkingPeer != null)
			{
				this.prefixBackup = (int)PhotonNetwork.networkingPeer.currentLevelPrefix;
			}
			return this.prefixBackup;
		}
		set
		{
			this.prefixBackup = value;
		}
	}

	// Token: 0x170003E7 RID: 999
	// (get) Token: 0x06001C0F RID: 7183 RVA: 0x000144DE File Offset: 0x000126DE
	// (set) Token: 0x06001C10 RID: 7184 RVA: 0x00014507 File Offset: 0x00012707
	public object[] instantiationData
	{
        get
        {
            if (!this.didAwake)
            {
                // even though viewID and instantiationID are setup before the GO goes live, this data can't be set. as workaround: fetch it if needed
                this.instantiationDataField = PhotonNetwork.networkingPeer.FetchInstantiationData(this.instantiationId);
            }
            return this.instantiationDataField;
        }
        set { this.instantiationDataField = value; }
    }

	// Token: 0x170003E8 RID: 1000
	// (get) Token: 0x06001C11 RID: 7185 RVA: 0x00014510 File Offset: 0x00012710
	// (set) Token: 0x06001C12 RID: 7186 RVA: 0x000B32DC File Offset: 0x000B14DC
	public int viewID
	{
        get { return this.viewIdField; }
        set
        {
            // if ID was 0 for an awakened PhotonView, the view should add itself into the networkingPeer.photonViewList after setup
            bool viewMustRegister = this.didAwake && this.viewIdField == 0;

            // TODO: decide if a viewID can be changed once it wasn't 0. most likely that is not a good idea
            // check if this view is in networkingPeer.photonViewList and UPDATE said list (so we don't keep the old viewID with a reference to this object)
            // PhotonNetwork.networkingPeer.RemovePhotonView(this, true);

            this.ownerId = value / PhotonNetwork.MAX_VIEW_IDS;

            this.viewIdField = value;

            if (viewMustRegister)
            {
                PhotonNetwork.networkingPeer.RegisterPhotonView(this);
            }
            //Debug.Log("Set viewID: " + value + " ->  owner: " + this.ownerId + " subId: " + this.subId);
        }
    }

	// Token: 0x170003E9 RID: 1001
	// (get) Token: 0x06001C13 RID: 7187 RVA: 0x00014518 File Offset: 0x00012718
	public bool isSceneView
	{
		get
		{
			return this.CreatorActorNr == 0;
		}
	}

	// Token: 0x170003EA RID: 1002
	// (get) Token: 0x06001C14 RID: 7188 RVA: 0x00014523 File Offset: 0x00012723
	public PhotonPlayer owner
	{
		get
		{
			return PhotonPlayer.Find(this.ownerId);
		}
	}

	// Token: 0x170003EB RID: 1003
	// (get) Token: 0x06001C15 RID: 7189 RVA: 0x00014530 File Offset: 0x00012730
	public int OwnerActorNr
	{
		get
		{
			return this.ownerId;
		}
	}

	// Token: 0x170003EC RID: 1004
	// (get) Token: 0x06001C16 RID: 7190 RVA: 0x00014538 File Offset: 0x00012738
	public bool isOwnerActive
	{
		get
		{
			return this.ownerId != 0 && PhotonNetwork.networkingPeer.mActors.ContainsKey(this.ownerId);
		}
	}

	// Token: 0x170003ED RID: 1005
	// (get) Token: 0x06001C17 RID: 7191 RVA: 0x0001455D File Offset: 0x0001275D
	public int CreatorActorNr
	{
		get
		{
			return this.viewIdField / PhotonNetwork.MAX_VIEW_IDS;
		}
	}

	// Token: 0x170003EE RID: 1006
	// (get) Token: 0x06001C18 RID: 7192 RVA: 0x0001456B File Offset: 0x0001276B
	public bool isMine
	{
		get
		{
			return this.ownerId == PhotonNetwork.player.ID || (!this.isOwnerActive && PhotonNetwork.isMasterClient);
		}
	}

	// Token: 0x06001C19 RID: 7193 RVA: 0x00014598 File Offset: 0x00012798
	protected internal void Awake()
	{
        if (this.viewID != 0)
        {
            // registration might be too late when some script (on this GO) searches this view BUT GetPhotonView() can search ALL in that case
            PhotonNetwork.networkingPeer.RegisterPhotonView(this);
            this.instantiationDataField = PhotonNetwork.networkingPeer.FetchInstantiationData(this.instantiationId);
        }

        this.didAwake = true;
    }

	// Token: 0x06001C1A RID: 7194 RVA: 0x000145CD File Offset: 0x000127CD
	public void RequestOwnership()
	{
		PhotonNetwork.networkingPeer.RequestOwnership(this.viewID, this.ownerId);
	}

	// Token: 0x06001C1B RID: 7195 RVA: 0x000145E5 File Offset: 0x000127E5
	public void TransferOwnership(PhotonPlayer newOwner)
	{
		this.TransferOwnership(newOwner.ID);
	}

	// Token: 0x06001C1C RID: 7196 RVA: 0x000145F3 File Offset: 0x000127F3
	public void TransferOwnership(int newOwnerId)
	{
		PhotonNetwork.networkingPeer.TransferOwnership(this.viewID, newOwnerId);
		this.ownerId = newOwnerId;
	}

	// Token: 0x06001C1D RID: 7197 RVA: 0x000B3328 File Offset: 0x000B1528
	public void OnMasterClientSwitched(PhotonPlayer newMasterClient)
	{
		if (this.CreatorActorNr == 0 && !this.OwnerShipWasTransfered && (this.currentMasterID == -1 || this.ownerId == this.currentMasterID))
		{
			this.ownerId = newMasterClient.ID;
		}
		this.currentMasterID = newMasterClient.ID;
	}

	// Token: 0x06001C1E RID: 7198 RVA: 0x000B3380 File Offset: 0x000B1580
	protected internal void OnDestroy()
	{
		if (!this.removedFromLocalViewList)
		{
			bool flag = PhotonNetwork.networkingPeer.LocalCleanPhotonView(this);
			bool isLoadingLevel = Application.isLoadingLevel;
			if (flag && !isLoadingLevel && this.instantiationId > 0 && !PhotonHandler.AppQuits && PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
			{
				Debug.Log("PUN-instantiated '" + base.gameObject.name + "' got destroyed by engine. This is OK when loading levels. Otherwise use: PhotonNetwork.Destroy().");
			}
		}
	}

	// Token: 0x06001C1F RID: 7199 RVA: 0x000B33F8 File Offset: 0x000B15F8
	public void SerializeView(PhotonStream stream)
	{
		for (int i = 0; i < this.punObservables.Length; i++)
		{
			this.punObservables[i].OnPhotonSerializeView(stream);
		}
	}

	// Token: 0x06001C20 RID: 7200 RVA: 0x000B33F8 File Offset: 0x000B15F8
	public void DeserializeView(PhotonStream stream)
	{
        for (int i = 0; i < this.punObservables.Length; i++)
		{
			this.punObservables[i].OnPhotonSerializeView(stream);
		}
	}

	// Token: 0x06001C21 RID: 7201 RVA: 0x0001460D File Offset: 0x0001280D
	public void RPC(string methodName, PhotonTargets target)
	{
		PhotonNetwork.RPC(this, methodName, target, false, null);
	}

	// Token: 0x06001C22 RID: 7202 RVA: 0x00014619 File Offset: 0x00012819
	public void RPC(string methodName, PhotonTargets target, byte[] data)
	{
		PhotonNetwork.RPC(this, methodName, target, false, data);
	}

	// Token: 0x06001C23 RID: 7203 RVA: 0x00014625 File Offset: 0x00012825
	public void RPC(string methodName, PhotonTargets target, PhotonDataWrite data)
	{
		PhotonNetwork.RPC(this, methodName, target, false, data.ToArray());
	}

	// Token: 0x06001C24 RID: 7204 RVA: 0x00014636 File Offset: 0x00012836
	public void RpcSecure(string methodName, PhotonTargets target, bool encrypt, byte[] data)
	{
		PhotonNetwork.RPC(this, methodName, target, encrypt, data);
	}

	// Token: 0x06001C25 RID: 7205 RVA: 0x00014643 File Offset: 0x00012843
	public void RpcSecure(string methodName, PhotonTargets target, bool encrypt, PhotonDataWrite data)
	{
		PhotonNetwork.RPC(this, methodName, target, encrypt, data.ToArray());
	}

	// Token: 0x06001C26 RID: 7206 RVA: 0x00014655 File Offset: 0x00012855
	public void RpcSecure(string methodName, PhotonTargets target, bool encrypt)
	{
		PhotonNetwork.RPC(this, methodName, target, encrypt, null);
	}

	// Token: 0x06001C27 RID: 7207 RVA: 0x00014661 File Offset: 0x00012861
	public void RPC(string methodName, PhotonPlayer targetPlayer, byte[] data)
	{
		PhotonNetwork.RPC(this, methodName, targetPlayer, false, data);
	}

	// Token: 0x06001C28 RID: 7208 RVA: 0x0001466D File Offset: 0x0001286D
	public void RPC(string methodName, PhotonPlayer targetPlayer, PhotonDataWrite data)
	{
		PhotonNetwork.RPC(this, methodName, targetPlayer, false, data.ToArray());
	}

	// Token: 0x06001C29 RID: 7209 RVA: 0x0001467E File Offset: 0x0001287E
	public void RPC(string methodName, PhotonPlayer targetPlayer)
	{
		PhotonNetwork.RPC(this, methodName, targetPlayer, false, null);
	}

	// Token: 0x06001C2A RID: 7210 RVA: 0x0001468A File Offset: 0x0001288A
	public void RpcSecure(string methodName, PhotonPlayer targetPlayer, bool encrypt, byte[] data)
	{
		PhotonNetwork.RPC(this, methodName, targetPlayer, encrypt, data);
	}

	// Token: 0x06001C2B RID: 7211 RVA: 0x00014697 File Offset: 0x00012897
	public void RpcSecure(string methodName, PhotonPlayer targetPlayer, bool encrypt)
	{
		PhotonNetwork.RPC(this, methodName, targetPlayer, encrypt, null);
	}

	// Token: 0x06001C2C RID: 7212 RVA: 0x000146A3 File Offset: 0x000128A3
	public static PhotonView Get(Component component)
	{
		return component.GetComponent<PhotonView>();
	}

	// Token: 0x06001C2D RID: 7213 RVA: 0x00012EAE File Offset: 0x000110AE
	public static PhotonView Get(GameObject gameObj)
	{
		return gameObj.GetComponent<PhotonView>();
	}

	// Token: 0x06001C2E RID: 7214 RVA: 0x000146AB File Offset: 0x000128AB
	public static PhotonView Find(int viewID)
	{
		return PhotonNetwork.networkingPeer.GetPhotonView(viewID);
	}

	// Token: 0x06001C2F RID: 7215 RVA: 0x000B342C File Offset: 0x000B162C
	public override string ToString()
	{
		return string.Format("View ({3}){0} on {1} {2}", new object[]
		{
			this.viewID,
			(!(base.gameObject != null)) ? "GO==null" : base.gameObject.name,
			(!this.isSceneView) ? string.Empty : "(scene)",
			this.prefix
		});
	}

	// Token: 0x06001C30 RID: 7216 RVA: 0x000B34AC File Offset: 0x000B16AC
	public void AddPunObservable(IPunObservable observable)
	{
		IPunObservable[] array = new IPunObservable[this.punObservables.Length + 1];
		for (int i = 0; i < this.punObservables.Length; i++)
		{
			array[i] = this.punObservables[i];
		}
		array[array.Length - 1] = observable;
		this.punObservables = array;
	}

	// Token: 0x06001C31 RID: 7217 RVA: 0x000146B8 File Offset: 0x000128B8
	public void AddMessage(string methodName, PhotonView.MessageDelegate callback)
	{
		this.messages.Add(methodName, callback);
	}

	// Token: 0x06001C32 RID: 7218 RVA: 0x000146C7 File Offset: 0x000128C7
	public PhotonDataWrite GetData()
	{
		this.data.Clear();
		return this.data;
	}

	// Token: 0x06001C33 RID: 7219 RVA: 0x000B34FC File Offset: 0x000B16FC
	public void InvokeMessage(string methodName, byte[] data, int senderID, int sendTime)
	{
		nProfiler.BeginSample("PhotonView.InvokeMessage");
		if (!this.messages.ContainsKey(methodName))
		{
			Debug.Log("No Find Method: " + methodName);
			return;
		}
		this.messageData.SetData(data, senderID, sendTime);
		this.messages[methodName](this.messageData);
		nProfiler.EndSample();
	}

	// Token: 0x04001041 RID: 4161
	public int ownerId;

	// Token: 0x04001042 RID: 4162
	public byte group;

	// Token: 0x04001043 RID: 4163
	protected internal bool mixedModeIsReliable;

	// Token: 0x04001044 RID: 4164
	public IPunObservable[] punObservables = new IPunObservable[0];

	// Token: 0x04001045 RID: 4165
	public Dictionary<string, PhotonView.MessageDelegate> messages = new Dictionary<string, PhotonView.MessageDelegate>();

	// Token: 0x04001046 RID: 4166
	private PhotonDataWrite data = new PhotonDataWrite();

	// Token: 0x04001047 RID: 4167
	private PhotonMessage messageData = new PhotonMessage();

	// Token: 0x04001048 RID: 4168
	public bool OwnerShipWasTransfered;

	// Token: 0x04001049 RID: 4169
	public int prefixBackup = -1;

	// Token: 0x0400104A RID: 4170
	internal object[] instantiationDataField;

	// Token: 0x0400104B RID: 4171
	protected internal BetterList<byte> lastOnSerializeDataSent = new BetterList<byte>();

	// Token: 0x0400104C RID: 4172
	protected internal object[] lastOnSerializeDataReceived;

	// Token: 0x0400104D RID: 4173
	public ViewSynchronization synchronization;

	// Token: 0x0400104E RID: 4174
	public OwnershipOption ownershipTransfer;

	// Token: 0x0400104F RID: 4175
	[SerializeField]
	private int viewIdField;

	// Token: 0x04001050 RID: 4176
	public int instantiationId;

	// Token: 0x04001051 RID: 4177
	public int currentMasterID = -1;

	// Token: 0x04001052 RID: 4178
	protected internal bool didAwake;

	// Token: 0x04001053 RID: 4179
	[SerializeField]
	protected internal bool isRuntimeInstantiated;

	// Token: 0x04001054 RID: 4180
	protected internal bool removedFromLocalViewList;

	// Token: 0x020002E4 RID: 740
	// (Invoke) Token: 0x06001C35 RID: 7221
	public delegate void MessageDelegate(PhotonMessage message);
}
