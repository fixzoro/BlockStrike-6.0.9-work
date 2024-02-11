using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000555 RID: 1365
public static class PhotonRPC
{
	// Token: 0x06002E28 RID: 11816 RVA: 0x000201D7 File Offset: 0x0001E3D7
	public static void AddMessage(string methodName, PhotonRPC.MessageDelegate callback)
	{
		PhotonRPC.messages.Add(methodName, callback);
	}

	// Token: 0x06002E29 RID: 11817 RVA: 0x000201E5 File Offset: 0x0001E3E5
	public static bool Contains(string methodName)
	{
		return PhotonRPC.messages.ContainsKey(methodName);
	}

	// Token: 0x06002E2A RID: 11818 RVA: 0x000201F2 File Offset: 0x0001E3F2
	public static PhotonDataWrite GetData()
	{
		PhotonRPC.data.Clear();
		return PhotonRPC.data;
	}

	// Token: 0x06002E2B RID: 11819 RVA: 0x00020203 File Offset: 0x0001E403
	public static void Clear()
	{
		PhotonRPC.messages.Clear();
	}

	// Token: 0x06002E2C RID: 11820 RVA: 0x0002020F File Offset: 0x0001E40F
	public static void OnEventCall(byte code, byte[] content, int senderID)
	{
		if (PhotonRPC.eventCode != code)
		{
			return;
		}
		PhotonRPC.ExecuteRpc(content, senderID);
	}

	// Token: 0x06002E2D RID: 11821 RVA: 0x0010BCFC File Offset: 0x00109EFC
	private static void InvokeMessage(string methodName, byte[] data, int senderID, int sendTime)
	{
		nProfiler.BeginSample("PhotonRPC.InvokeMessage");
		nProfiler.BeginSample("1");
		if (!PhotonRPC.messages.ContainsKey(methodName))
		{
			Debug.Log("No Find Method: " + methodName);
			return;
		}
		nProfiler.EndSample();
		nProfiler.BeginSample("2");
		PhotonRPC.messageData.SetData(data, senderID, sendTime);
		nProfiler.EndSample();
		nProfiler.BeginSample("3");
		PhotonRPC.messages[methodName](PhotonRPC.messageData);
		nProfiler.EndSample();
		nProfiler.EndSample();
	}

	// Token: 0x06002E2E RID: 11822 RVA: 0x0010BD88 File Offset: 0x00109F88
	private static void ExecuteRpc(byte[] bytes, int senderID = 0)
	{
		nProfiler.BeginSample("PhotonRPC.ExecuteRpc");
		if (bytes.Length == 0)
		{
			Debug.LogError("Malformed RPC; this should never occur.");
			return;
		}
		PhotonRPC.rpcData.Clear();
		PhotonRPC.rpcData.SetData(bytes);
		string text = string.Empty;
		if (PhotonRPC.rpcData.ContainsKey((byte)5))
		{
			byte @byte = PhotonRPC.rpcData.GetByte((byte)5);
			if ((int)@byte > PhotonNetwork.PhotonServerSettings.RpcList.Count - 1)
			{
				Debug.LogError("Could not find RPC with index: " + @byte + ". Going to ignore! Check PhotonServerSettings.RpcList");
				return;
			}
			text = PhotonNetwork.PhotonServerSettings.RpcList[(int)@byte];
		}
		else
		{
			text = PhotonRPC.rpcData.GetString((byte)3);
		}
		nProfiler.BeginSample("4");
		byte[] array;
		if (PhotonRPC.rpcData.ContainsKey((byte)4))
		{
			array = PhotonRPC.rpcData.GetBytes((byte)4);
		}
		else
		{
			array = new byte[0];
		}
		nProfiler.EndSample();
		if (string.IsNullOrEmpty(text))
		{
			Debug.LogError("Malformed RPC; this should never occur.");
			return;
		}
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
		{
			Debug.Log("Received RPC: " + text);
		}
		nProfiler.BeginSample("6");
		int @int = PhotonRPC.rpcData.GetInt((byte)2);
		nProfiler.EndSample();
		PhotonRPC.InvokeMessage(text, array, senderID, @int);
		nProfiler.EndSample();
	}

	// Token: 0x06002E2F RID: 11823 RVA: 0x00020224 File Offset: 0x0001E424
	public static void RPC(string methodName, PhotonTargets target)
	{
		PhotonRPC.RPC(methodName, target, null, null);
	}

	// Token: 0x06002E30 RID: 11824 RVA: 0x0002022F File Offset: 0x0001E42F
	public static void RPC(string methodName, PhotonTargets target, byte[] data)
	{
		PhotonRPC.RPC(methodName, target, null, data);
	}

	// Token: 0x06002E31 RID: 11825 RVA: 0x0010BED0 File Offset: 0x0010A0D0
	public static void RPC(string methodName, PhotonTargets target, int value)
	{
		PhotonDataWrite photonDataWrite = PhotonRPC.GetData();
		photonDataWrite.Write(value);
		PhotonRPC.RPC(methodName, target, null, photonDataWrite.ToArray());
	}

	// Token: 0x06002E32 RID: 11826 RVA: 0x0002023A File Offset: 0x0001E43A
	public static void RPC(string methodName, PhotonTargets target, PhotonDataWrite data)
	{
		PhotonRPC.RPC(methodName, target, null, data.ToArray());
	}

	// Token: 0x06002E33 RID: 11827 RVA: 0x0002024A File Offset: 0x0001E44A
	public static void RPC(string methodName, PhotonPlayer targetPlayer, byte[] data)
	{
		PhotonRPC.RPC(methodName, PhotonTargets.Others, targetPlayer, data);
	}

	// Token: 0x06002E34 RID: 11828 RVA: 0x00020255 File Offset: 0x0001E455
	public static void RPC(string methodName, PhotonPlayer targetPlayer, PhotonDataWrite data)
	{
		PhotonRPC.RPC(methodName, PhotonTargets.Others, targetPlayer, data.ToArray());
	}

	// Token: 0x06002E35 RID: 11829 RVA: 0x00020265 File Offset: 0x0001E465
	public static void RPC(string methodName, PhotonPlayer targetPlayer)
	{
		PhotonRPC.RPC(methodName, PhotonTargets.Others, targetPlayer, null);
	}

	// Token: 0x06002E36 RID: 11830 RVA: 0x0010BEF8 File Offset: 0x0010A0F8
	private static void RPC(string methodName, PhotonTargets target, PhotonPlayer player, byte[] data)
	{
		nProfiler.BeginSample("PhotonPRC.RPC");
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
		PhotonRPC.rpcEvent.Clear();
		PhotonRPC.rpcEvent.Add((byte)2, PhotonNetwork.ServerTimestamp);
		bool flag = false;
		for (int i = 0; i < PhotonNetwork.networkingPeer.rpcShortcuts.size; i++)
		{
			if (PhotonNetwork.networkingPeer.rpcShortcuts[i] == methodName)
			{
				PhotonRPC.rpcEvent.Add((byte)5, (byte)i);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			PhotonRPC.rpcEvent.Add((byte)3, methodName);
		}
		if (data != null && data.Length > 0)
		{
			PhotonRPC.rpcEvent.Add((byte)4, data);
		}
		if (player != null)
		{
			if (PhotonNetwork.networkingPeer.LocalPlayer.ID == player.ID)
			{
				PhotonRPC.ExecuteRpc(PhotonRPC.rpcEvent.ToArray(), player.ID);
			}
			else
			{
				PhotonRPC.optionsRpcEvent.Reset();
				PhotonRPC.optionsRpcEvent.TargetActors = new int[]
				{
					player.ID
				};
				PhotonNetwork.networkingPeer.OpRaiseEvent(PhotonRPC.eventCode, PhotonRPC.rpcEvent.ToArray(), true, PhotonRPC.optionsRpcEvent);
			}
			return;
		}
		nProfiler.BeginSample("6");
		if (target == PhotonTargets.All)
		{
			PhotonRPC.optionsRpcEvent.Reset();
			PhotonRPC.rpcEventBytes = PhotonRPC.rpcEvent.ToArray();
			PhotonNetwork.networkingPeer.OpRaiseEvent(PhotonRPC.eventCode, PhotonRPC.rpcEventBytes, true, PhotonRPC.optionsRpcEvent);
			PhotonRPC.ExecuteRpc(PhotonRPC.rpcEventBytes, PhotonNetwork.networkingPeer.LocalPlayer.ID);
		}
		else if (target == PhotonTargets.Others)
		{
			PhotonRPC.optionsRpcEvent.Reset();
			PhotonNetwork.networkingPeer.OpRaiseEvent(PhotonRPC.eventCode, PhotonRPC.rpcEvent.ToArray(), true, PhotonRPC.optionsRpcEvent);
		}
		else if (target == PhotonTargets.AllBuffered)
		{
			PhotonRPC.optionsRpcEvent.Reset();
			PhotonRPC.optionsRpcEvent.CachingOption = EventCaching.AddToRoomCache;
			PhotonRPC.rpcEventBytes = PhotonRPC.rpcEvent.ToArray();
			PhotonNetwork.networkingPeer.OpRaiseEvent(PhotonRPC.eventCode, PhotonRPC.rpcEventBytes, true, PhotonRPC.optionsRpcEvent);
			PhotonRPC.ExecuteRpc(PhotonRPC.rpcEventBytes, PhotonNetwork.networkingPeer.LocalPlayer.ID);
		}
		else if (target == PhotonTargets.OthersBuffered)
		{
			PhotonRPC.optionsRpcEvent.Reset();
			PhotonRPC.optionsRpcEvent.CachingOption = EventCaching.AddToRoomCache;
			PhotonNetwork.networkingPeer.OpRaiseEvent(PhotonRPC.eventCode, PhotonRPC.rpcEvent.ToArray(), true, PhotonRPC.optionsRpcEvent);
		}
		else if (target == PhotonTargets.MasterClient)
		{
			if (PhotonNetwork.networkingPeer.mMasterClientId == PhotonNetwork.networkingPeer.LocalPlayer.ID)
			{
				PhotonRPC.ExecuteRpc(PhotonRPC.rpcEvent.ToArray(), PhotonNetwork.networkingPeer.LocalPlayer.ID);
			}
			else
			{
				PhotonRPC.optionsRpcEvent.Reset();
				PhotonRPC.optionsRpcEvent.Receivers = ReceiverGroup.MasterClient;
				PhotonNetwork.networkingPeer.OpRaiseEvent(PhotonRPC.eventCode, PhotonRPC.rpcEvent.ToArray(), true, PhotonRPC.optionsRpcEvent);
			}
		}
		else if (target == PhotonTargets.AllViaServer)
		{
			PhotonRPC.optionsRpcEvent.Reset();
			PhotonRPC.optionsRpcEvent.Receivers = ReceiverGroup.All;
			PhotonNetwork.networkingPeer.OpRaiseEvent(PhotonRPC.eventCode, PhotonRPC.rpcEvent.ToArray(), true, PhotonRPC.optionsRpcEvent);
			if (PhotonNetwork.offlineMode)
			{
				PhotonRPC.ExecuteRpc(PhotonRPC.rpcEvent.ToArray(), PhotonNetwork.networkingPeer.LocalPlayer.ID);
			}
		}
		else if (target == PhotonTargets.AllBufferedViaServer)
		{
			PhotonRPC.optionsRpcEvent.Reset();
			PhotonRPC.optionsRpcEvent.Receivers = ReceiverGroup.All;
			PhotonRPC.optionsRpcEvent.CachingOption = EventCaching.AddToRoomCache;
			PhotonNetwork.networkingPeer.OpRaiseEvent(PhotonRPC.eventCode, PhotonRPC.rpcEvent.ToArray(), true, PhotonRPC.optionsRpcEvent);
			if (PhotonNetwork.offlineMode)
			{
				PhotonRPC.ExecuteRpc(PhotonRPC.rpcEvent.ToArray(), PhotonNetwork.networkingPeer.LocalPlayer.ID);
			}
		}
		else
		{
			Debug.LogError("Unsupported target enum: " + target);
		}
		nProfiler.EndSample();
		nProfiler.EndSample();
	}

	// Token: 0x04001DA3 RID: 7587
	public static readonly byte eventCode = 42;

	// Token: 0x04001DA4 RID: 7588
	private static PhotonHashtable rpcEvent = new PhotonHashtable();

	// Token: 0x04001DA5 RID: 7589
	private static PhotonHashtable rpcData = new PhotonHashtable();

	// Token: 0x04001DA6 RID: 7590
	private static byte[] rpcEventBytes;

	// Token: 0x04001DA7 RID: 7591
	private static RaiseEventOptions optionsRpcEvent = new RaiseEventOptions();

	// Token: 0x04001DA8 RID: 7592
	private static Dictionary<string, PhotonRPC.MessageDelegate> messages = new Dictionary<string, PhotonRPC.MessageDelegate>();

	// Token: 0x04001DA9 RID: 7593
	private static PhotonDataWrite data = new PhotonDataWrite();

	// Token: 0x04001DAA RID: 7594
	private static PhotonMessage messageData = new PhotonMessage();

	// Token: 0x02000556 RID: 1366
	// (Invoke) Token: 0x06002E38 RID: 11832
	public delegate void MessageDelegate(PhotonMessage message);
}
