using System;
using System.Collections;
using System.Diagnostics;
using System.Net;
using ExitGames.Client.Photon;
using UnityEngine;

// Token: 0x020002E5 RID: 741
public class PhotonPingManager
{
	// Token: 0x170003EF RID: 1007
	// (get) Token: 0x06001C3A RID: 7226 RVA: 0x000B3560 File Offset: 0x000B1760
	public Region BestRegion
	{
		get
		{
			Region result = null;
			int num = int.MaxValue;
			foreach (Region region in PhotonNetwork.networkingPeer.AvailableRegions)
			{
				if (region.Ping != 0 && region.Ping < num)
				{
					num = region.Ping;
					result = region;
				}
			}
			return result;
		}
	}

	// Token: 0x170003F0 RID: 1008
	// (get) Token: 0x06001C3B RID: 7227 RVA: 0x000146F2 File Offset: 0x000128F2
	public bool Done
	{
		get
		{
			return this.PingsRunning == 0;
		}
	}

	// Token: 0x06001C3C RID: 7228 RVA: 0x000B35E0 File Offset: 0x000B17E0
	public IEnumerator PingSocket(Region region)
	{
		region.Ping = PhotonPingManager.Attempts * PhotonPingManager.MaxMilliseconsPerPing;
		this.PingsRunning++;
		PhotonPing ping;
		if (PhotonHandler.PingImplementation == typeof(PingNativeDynamic))
		{
			UnityEngine.Debug.Log("Using constructor for new PingNativeDynamic()");
			ping = new PingNativeDynamic();
		}
		else if (PhotonHandler.PingImplementation == typeof(PingNativeStatic))
		{
			UnityEngine.Debug.Log("Using constructor for new PingNativeStatic()");
			ping = new PingNativeStatic();
		}
		else if (PhotonHandler.PingImplementation == typeof(PingMono))
		{
			ping = new PingMono();
		}
		else
		{
			ping = (PhotonPing)Activator.CreateInstance(PhotonHandler.PingImplementation);
		}
		float rttSum = 0f;
		int replyCount = 0;
		string regionAddress = region.HostAndPort;
		int indexOfColon = regionAddress.LastIndexOf(':');
		if (indexOfColon > 1)
		{
			regionAddress = regionAddress.Substring(0, indexOfColon);
		}
		int indexOfProtocol = regionAddress.IndexOf("wss://");
		if (indexOfProtocol > -1)
		{
			regionAddress = regionAddress.Substring(indexOfProtocol + "wss://".Length);
		}
		regionAddress = PhotonPingManager.ResolveHost(regionAddress);
		for (int i = 0; i < PhotonPingManager.Attempts; i++)
		{
			bool overtime = false;
			Stopwatch sw = new Stopwatch();
			sw.Start();
			try
			{
				ping.StartPing(regionAddress);
			}
			catch (Exception ex)
			{
				Exception e = ex;
				UnityEngine.Debug.Log("catched: " + e);
				this.PingsRunning--;
				break;
			}
			while (!ping.Done())
			{
				if (sw.ElapsedMilliseconds >= (long)PhotonPingManager.MaxMilliseconsPerPing)
				{
					overtime = true;
					break;
				}
				yield return 0;
			}
			int rtt = (int)sw.ElapsedMilliseconds;
			if (!PhotonPingManager.IgnoreInitialAttempt || i != 0)
			{
				if (ping.Successful && !overtime)
				{
					rttSum += (float)rtt;
					replyCount++;
					region.Ping = (int)(rttSum / (float)replyCount);
				}
			}
			yield return new WaitForSeconds(0.1f);
		}
		ping.Dispose();
		this.PingsRunning--;
		yield return null;
		yield break;
	}

	// Token: 0x06001C3D RID: 7229 RVA: 0x000B360C File Offset: 0x000B180C
	public static string ResolveHost(string hostName)
	{
		string text = string.Empty;
		try
		{
			IPAddress[] hostAddresses = Dns.GetHostAddresses(hostName);
			if (hostAddresses.Length == 1)
			{
				return hostAddresses[0].ToString();
			}
			foreach (IPAddress ipaddress in hostAddresses)
			{
				if (ipaddress != null)
				{
					if (ipaddress.ToString().Contains(":"))
					{
						return ipaddress.ToString();
					}
					if (string.IsNullOrEmpty(text))
					{
						text = hostAddresses.ToString();
					}
				}
			}
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.Log("Exception caught! " + ex.Source + " Message: " + ex.Message);
		}
		return text;
	}

	// Token: 0x04001055 RID: 4181
	private const string wssProtocolString = "wss://";

	// Token: 0x04001056 RID: 4182
	public bool UseNative;

	// Token: 0x04001057 RID: 4183
	public static int Attempts = 5;

	// Token: 0x04001058 RID: 4184
	public static bool IgnoreInitialAttempt = true;

	// Token: 0x04001059 RID: 4185
	public static int MaxMilliseconsPerPing = 800;

	// Token: 0x0400105A RID: 4186
	private int PingsRunning;
}
