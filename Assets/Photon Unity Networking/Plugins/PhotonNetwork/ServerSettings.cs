using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

// Token: 0x020002EB RID: 747
[Serializable]
public class ServerSettings : ScriptableObject
{
	// Token: 0x06001C92 RID: 7314 RVA: 0x0001495C File Offset: 0x00012B5C
	public void UseCloudBestRegion(string cloudAppid)
	{
		this.HostType = ServerSettings.HostingOption.BestRegion;
		this.AppID = cloudAppid;
	}

	// Token: 0x06001C93 RID: 7315 RVA: 0x0001496C File Offset: 0x00012B6C
	public void UseCloud(string cloudAppid)
	{
		this.HostType = ServerSettings.HostingOption.PhotonCloud;
		this.AppID = cloudAppid;
	}

	// Token: 0x06001C94 RID: 7316 RVA: 0x0001497C File Offset: 0x00012B7C
	public void UseCloud(string cloudAppid, CloudRegionCode code)
	{
		this.HostType = ServerSettings.HostingOption.PhotonCloud;
		this.AppID = cloudAppid;
		this.PreferredRegion = code;
	}

	// Token: 0x06001C95 RID: 7317 RVA: 0x00014993 File Offset: 0x00012B93
	public void UseMyServer(string serverAddress, int serverPort, string application)
	{
		this.HostType = ServerSettings.HostingOption.SelfHosted;
		this.AppID = ((application == null) ? "master" : application);
		this.ServerAddress = serverAddress;
		this.ServerPort = serverPort;
	}

	// Token: 0x06001C96 RID: 7318 RVA: 0x000B4424 File Offset: 0x000B2624
	public static bool IsAppId(string val)
	{
		try
		{
			new Guid(val);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x17000417 RID: 1047
	// (get) Token: 0x06001C97 RID: 7319 RVA: 0x000149C1 File Offset: 0x00012BC1
	public static CloudRegionCode BestRegionCodeInPreferences
	{
		get
		{
			return PhotonHandler.BestRegionCodeInPreferences;
		}
	}

	// Token: 0x06001C98 RID: 7320 RVA: 0x000149C8 File Offset: 0x00012BC8
	public static void ResetBestRegionCodeInPreferences()
	{
		PhotonHandler.BestRegionCodeInPreferences = CloudRegionCode.none;
	}

	// Token: 0x06001C99 RID: 7321 RVA: 0x000149D0 File Offset: 0x00012BD0
	public override string ToString()
	{
		return string.Concat(new object[]
		{
			"ServerSettings: ",
			this.HostType,
			" ",
			this.ServerAddress
		});
	}

	// Token: 0x04001080 RID: 4224
	public string AppID = string.Empty;

	// Token: 0x04001081 RID: 4225
	public string VoiceAppID = string.Empty;

	// Token: 0x04001082 RID: 4226
	public string ChatAppID = string.Empty;

	// Token: 0x04001083 RID: 4227
	public ServerSettings.HostingOption HostType;

	// Token: 0x04001084 RID: 4228
	public CloudRegionCode PreferredRegion;

	// Token: 0x04001085 RID: 4229
	public CloudRegionFlag EnabledRegions = (CloudRegionFlag)(-1);

	// Token: 0x04001086 RID: 4230
	public ConnectionProtocol Protocol;

	// Token: 0x04001087 RID: 4231
	public string ServerAddress = string.Empty;

	// Token: 0x04001088 RID: 4232
	public int ServerPort = 5055;

	// Token: 0x04001089 RID: 4233
	public int VoiceServerPort = 5055;

	// Token: 0x0400108A RID: 4234
	public bool JoinLobby;

	// Token: 0x0400108B RID: 4235
	public bool EnableLobbyStatistics;

	// Token: 0x0400108C RID: 4236
	public PhotonLogLevel PunLogging;

	// Token: 0x0400108D RID: 4237
	public DebugLevel NetworkLogging = DebugLevel.ERROR;

	// Token: 0x0400108E RID: 4238
	public bool RunInBackground = true;

	// Token: 0x0400108F RID: 4239
	public List<string> RpcList = new List<string>();

	// Token: 0x04001090 RID: 4240
	[HideInInspector]
	public bool DisableAutoOpenWizard;

	// Token: 0x020002EC RID: 748
	public enum HostingOption
	{
		// Token: 0x04001092 RID: 4242
		NotSet,
		// Token: 0x04001093 RID: 4243
		PhotonCloud,
		// Token: 0x04001094 RID: 4244
		SelfHosted,
		// Token: 0x04001095 RID: 4245
		OfflineMode,
		// Token: 0x04001096 RID: 4246
		BestRegion
	}
}
