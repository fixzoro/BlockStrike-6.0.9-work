using System;
using System.Collections;
using System.Diagnostics;
using ExitGames.Client.Photon;
using UnityEngine;

// Token: 0x020002D1 RID: 721
internal class PhotonHandler : MonoBehaviour
{
	// Token: 0x06001B0E RID: 6926 RVA: 0x000B0958 File Offset: 0x000AEB58
	protected void Awake()
	{
		if (PhotonHandler.SP != null && PhotonHandler.SP != this && PhotonHandler.SP.gameObject != null)
		{
			UnityEngine.Object.DestroyImmediate(PhotonHandler.SP.gameObject);
		}
		PhotonHandler.SP = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		this.updateInterval = 1000 / PhotonNetwork.sendRate;
		this.updateIntervalOnSerialize = 1000 / PhotonNetwork.sendRateOnSerialize;
		PhotonHandler.StartFallbackSendAckThread();
	}

	// Token: 0x06001B0F RID: 6927 RVA: 0x00013827 File Offset: 0x00011A27
	protected void OnLevelWasLoaded(int level)
	{
		PhotonNetwork.networkingPeer.NewSceneLoaded();
		PhotonNetwork.networkingPeer.SetLevelInPropsIfSynced(SceneManagerHelper.ActiveSceneName, false, false);
		PhotonNetwork.networkingPeer.IsReloadingLevel = false;
		PhotonNetwork.networkingPeer.AsynchLevelLoadCall = false;
	}

	// Token: 0x06001B10 RID: 6928 RVA: 0x0001385A File Offset: 0x00011A5A
	protected void OnApplicationQuit()
	{
		PhotonHandler.AppQuits = true;
		PhotonHandler.StopFallbackSendAckThread();
		PhotonNetwork.Disconnect();
	}

	// Token: 0x06001B11 RID: 6929 RVA: 0x000B09E4 File Offset: 0x000AEBE4
	protected void OnApplicationPause(bool pause)
	{
        if (PhotonNetwork.BackgroundTimeout > 0.1f)
        {
            if (timerToStopConnectionInBackground == null)
            {
                timerToStopConnectionInBackground = new Stopwatch();
            }
            timerToStopConnectionInBackground.Reset();

            if (pause)
            {
                timerToStopConnectionInBackground.Start();
            }
            else
            {
                timerToStopConnectionInBackground.Stop();
            }
        }
    }

	// Token: 0x06001B12 RID: 6930 RVA: 0x0001386C File Offset: 0x00011A6C
	protected void OnDestroy()
	{
		PhotonHandler.StopFallbackSendAckThread();
	}

	// Token: 0x06001B13 RID: 6931 RVA: 0x000B0A40 File Offset: 0x000AEC40
	protected void FixedUpdate()
	{
		if (PhotonNetwork.networkingPeer == null)
		{
			UnityEngine.Debug.LogError("NetworkPeer broke!");
			return;
		}
		bool flag = true;
		this.MaxDispathData = 1;
		while (PhotonNetwork.isMessageQueueRunning && flag && this.MaxDispathData <= 5)
		{
			flag = PhotonNetwork.networkingPeer.DispatchIncomingCommands();
			this.MaxDispathData += 1;
		}
	}

	// Token: 0x06001B14 RID: 6932 RVA: 0x000B0AA8 File Offset: 0x000AECA8
	protected void LateUpdate()
	{
        int currentMsSinceStart = (int)(Time.realtimeSinceStartup * 1000);  // avoiding Environment.TickCount, which could be negative on long-running platforms
        if (PhotonNetwork.isMessageQueueRunning && currentMsSinceStart > this.nextSendTickCountOnSerialize)
        {
            PhotonNetwork.networkingPeer.RunViewUpdate();
            this.nextSendTickCountOnSerialize = currentMsSinceStart + this.updateIntervalOnSerialize;
            this.nextSendTickCount = 0;     // immediately send when synchronization code was running
        }

        currentMsSinceStart = (int)(Time.realtimeSinceStartup * 1000);
        if (currentMsSinceStart > this.nextSendTickCount)
        {
            bool doSend = true;
            while (PhotonNetwork.isMessageQueueRunning && doSend)
            {
                // Send all outgoing commands
                nProfiler.BeginSample("SendOutgoingCommands");
                doSend = PhotonNetwork.networkingPeer.SendOutgoingCommands();
                nProfiler.EndSample();
            }

            this.nextSendTickCount = currentMsSinceStart + this.updateInterval;
        }
    }

	// Token: 0x06001B15 RID: 6933 RVA: 0x00013873 File Offset: 0x00011A73
	protected void OnJoinedRoom()
	{
		PhotonNetwork.networkingPeer.LoadLevelIfSynced();
	}

	// Token: 0x06001B16 RID: 6934 RVA: 0x0001387F File Offset: 0x00011A7F
	protected void OnCreatedRoom()
	{
		PhotonNetwork.networkingPeer.SetLevelInPropsIfSynced(SceneManagerHelper.ActiveSceneName, false, false);
	}

	// Token: 0x06001B17 RID: 6935 RVA: 0x00013892 File Offset: 0x00011A92
	public static void StartFallbackSendAckThread()
	{
		if (PhotonHandler.sendThreadShouldRun)
		{
			return;
		}
		PhotonHandler.sendThreadShouldRun = true;
		SupportClass.StartBackgroundCalls(PhotonHandler.FallbackSendAckThread);
	}

	// Token: 0x06001B18 RID: 6936 RVA: 0x000138BE File Offset: 0x00011ABE
	public static void StopFallbackSendAckThread()
	{
		PhotonHandler.sendThreadShouldRun = false;
	}

	// Token: 0x06001B19 RID: 6937 RVA: 0x000B0B64 File Offset: 0x000AED64
	public static bool FallbackSendAckThread()
	{
        if (sendThreadShouldRun && !PhotonNetwork.offlineMode && PhotonNetwork.networkingPeer != null)
        {
            // check if the client should disconnect after some seconds in background
            if (timerToStopConnectionInBackground != null && PhotonNetwork.BackgroundTimeout > 0.1f)
            {
                if (timerToStopConnectionInBackground.ElapsedMilliseconds > PhotonNetwork.BackgroundTimeout * 1000)
                {
                    if (PhotonNetwork.connected)
                    {
                        PhotonNetwork.Disconnect();
                    }
                    timerToStopConnectionInBackground.Stop();
                    timerToStopConnectionInBackground.Reset();
                    return sendThreadShouldRun;
                }
            }

            if (!PhotonNetwork.isMessageQueueRunning || PhotonNetwork.networkingPeer.ConnectionTime - PhotonNetwork.networkingPeer.LastSendOutgoingTime > 200)
            {
                PhotonNetwork.networkingPeer.SendAcksOnly();
            }
        }

        return sendThreadShouldRun;
    }

	// Token: 0x170003A5 RID: 933
	// (get) Token: 0x06001B1A RID: 6938 RVA: 0x000B0C28 File Offset: 0x000AEE28
	// (set) Token: 0x06001B1B RID: 6939 RVA: 0x000138C6 File Offset: 0x00011AC6
	internal static CloudRegionCode BestRegionCodeInPreferences
	{
		get
		{
			string @string = nPlayerPrefs.GetString("Region", string.Empty);
			if (!string.IsNullOrEmpty(@string))
			{
				return Region.Parse(@string);
			}
			return CloudRegionCode.none;
		}
		set
		{
			if (value == CloudRegionCode.none)
			{
				nPlayerPrefs.DeleteKey("Region");
			}
			else
			{
				nPlayerPrefs.SetString("Region", value.ToString());
			}
		}
	}

	// Token: 0x06001B1C RID: 6940 RVA: 0x000138F3 File Offset: 0x00011AF3
	protected internal static void PingAvailableRegionsAndConnectToBest()
	{
		PhotonHandler.SP.StartCoroutine(PhotonHandler.SP.PingAvailableRegionsCoroutine(true));
	}

	// Token: 0x06001B1D RID: 6941 RVA: 0x000B0C5C File Offset: 0x000AEE5C
	internal IEnumerator PingAvailableRegionsCoroutine(bool connectToBest)
	{
		while (PhotonNetwork.networkingPeer.AvailableRegions == null)
		{
			if (PhotonNetwork.connectionStateDetailed != ClientState.ConnectingToNameServer && PhotonNetwork.connectionStateDetailed != ClientState.ConnectedToNameServer)
			{
				UnityEngine.Debug.LogError("Call ConnectToNameServer to ping available regions.");
				yield break;
			}
			UnityEngine.Debug.Log(string.Concat(new object[]
			{
				"Waiting for AvailableRegions. State: ",
				PhotonNetwork.connectionStateDetailed,
				" Server: ",
				PhotonNetwork.Server,
				" PhotonNetwork.networkingPeer.AvailableRegions ",
				PhotonNetwork.networkingPeer.AvailableRegions != null
			}));
			yield return new WaitForSeconds(0.25f);
		}
		if (PhotonNetwork.networkingPeer.AvailableRegions == null || PhotonNetwork.networkingPeer.AvailableRegions.Count == 0)
		{
			UnityEngine.Debug.LogError("No regions available. Are you sure your appid is valid and setup?");
			yield break;
		}
		PhotonPingManager pingManager = new PhotonPingManager();
		foreach (Region region in PhotonNetwork.networkingPeer.AvailableRegions)
		{
			PhotonHandler.SP.StartCoroutine(pingManager.PingSocket(region));
		}
		while (!pingManager.Done)
		{
			yield return new WaitForSeconds(0.1f);
		}
		Region best = pingManager.BestRegion;
		PhotonHandler.BestRegionCodeInPreferences = best.Code;
		if (connectToBest)
		{
			PhotonNetwork.networkingPeer.ConnectToRegionMaster(best.Code);
		}
		yield break;
	}

	// Token: 0x04000FC6 RID: 4038
	private const int SerializeRateFrameCorrection = 8;

	// Token: 0x04000FC7 RID: 4039
	private const string PlayerPrefsKey = "Region";

	// Token: 0x04000FC8 RID: 4040
	public static PhotonHandler SP;

	// Token: 0x04000FC9 RID: 4041
	public int updateInterval;

	// Token: 0x04000FCA RID: 4042
	public int updateIntervalOnSerialize;

	// Token: 0x04000FCB RID: 4043
	private int nextSendTickCount;

	// Token: 0x04000FCC RID: 4044
	private int nextSendTickCountOnSerialize;

	// Token: 0x04000FCD RID: 4045
	private static bool sendThreadShouldRun;

	// Token: 0x04000FCE RID: 4046
	private static Stopwatch timerToStopConnectionInBackground;

	// Token: 0x04000FCF RID: 4047
	protected internal static bool AppQuits;

	// Token: 0x04000FD0 RID: 4048
	protected internal static Type PingImplementation;

	// Token: 0x04000FD1 RID: 4049
	private byte MaxDispathData = 1;

	// Token: 0x04000FD2 RID: 4050
	public static int MaxDatagrams = 10;

	// Token: 0x04000FD3 RID: 4051
	public static bool SendAsap;
}
