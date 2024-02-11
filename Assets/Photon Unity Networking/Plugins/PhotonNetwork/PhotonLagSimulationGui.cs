using System;
using ExitGames.Client.Photon;
using UnityEngine;

// Token: 0x020002D3 RID: 723
public class PhotonLagSimulationGui : MonoBehaviour
{
	// Token: 0x170003A8 RID: 936
	// (get) Token: 0x06001B25 RID: 6949 RVA: 0x00013952 File Offset: 0x00011B52
	// (set) Token: 0x06001B26 RID: 6950 RVA: 0x0001395A File Offset: 0x00011B5A
	public PhotonPeer Peer { get; set; }

	// Token: 0x06001B27 RID: 6951 RVA: 0x00013963 File Offset: 0x00011B63
	public void Start()
	{
		this.Peer = PhotonNetwork.networkingPeer;
	}

	// Token: 0x06001B28 RID: 6952 RVA: 0x000B0E9C File Offset: 0x000AF09C
	public void OnGUI()
	{
		if (!this.Visible)
		{
			return;
		}
		if (this.Peer == null)
		{
			this.WindowRect = GUILayout.Window(this.WindowId, this.WindowRect, new GUI.WindowFunction(this.NetSimHasNoPeerWindow), "Netw. Sim.", new GUILayoutOption[0]);
		}
		else
		{
			this.WindowRect = GUILayout.Window(this.WindowId, this.WindowRect, new GUI.WindowFunction(this.NetSimWindow), "Netw. Sim.", new GUILayoutOption[0]);
		}
	}

	// Token: 0x06001B29 RID: 6953 RVA: 0x00013970 File Offset: 0x00011B70
	private void NetSimHasNoPeerWindow(int windowId)
	{
		GUILayout.Label("No peer to communicate with. ", new GUILayoutOption[0]);
	}

	// Token: 0x06001B2A RID: 6954 RVA: 0x000B0F24 File Offset: 0x000AF124
	private void NetSimWindow(int windowId)
	{
		GUILayout.Label(string.Format("Rtt:{0,4} +/-{1,3}", this.Peer.RoundTripTime, this.Peer.RoundTripTimeVariance), new GUILayoutOption[0]);
		bool isSimulationEnabled = this.Peer.IsSimulationEnabled;
		bool flag = GUILayout.Toggle(isSimulationEnabled, "Simulate", new GUILayoutOption[0]);
		if (flag != isSimulationEnabled)
		{
			this.Peer.IsSimulationEnabled = flag;
		}
		float num = (float)this.Peer.NetworkSimulationSettings.IncomingLag;
		GUILayout.Label("Lag " + num, new GUILayoutOption[0]);
		num = GUILayout.HorizontalSlider(num, 0f, 500f, new GUILayoutOption[0]);
		this.Peer.NetworkSimulationSettings.IncomingLag = (int)num;
		this.Peer.NetworkSimulationSettings.OutgoingLag = (int)num;
		float num2 = (float)this.Peer.NetworkSimulationSettings.IncomingJitter;
		GUILayout.Label("Jit " + num2, new GUILayoutOption[0]);
		num2 = GUILayout.HorizontalSlider(num2, 0f, 100f, new GUILayoutOption[0]);
		this.Peer.NetworkSimulationSettings.IncomingJitter = (int)num2;
		this.Peer.NetworkSimulationSettings.OutgoingJitter = (int)num2;
		float num3 = (float)this.Peer.NetworkSimulationSettings.IncomingLossPercentage;
		GUILayout.Label("Loss " + num3, new GUILayoutOption[0]);
		num3 = GUILayout.HorizontalSlider(num3, 0f, 10f, new GUILayoutOption[0]);
		this.Peer.NetworkSimulationSettings.IncomingLossPercentage = (int)num3;
		this.Peer.NetworkSimulationSettings.OutgoingLossPercentage = (int)num3;
		if (GUI.changed)
		{
			this.WindowRect.height = 100f;
		}
		GUI.DragWindow();
	}

	// Token: 0x04000FDC RID: 4060
	public Rect WindowRect = new Rect(0f, 100f, 120f, 100f);

	// Token: 0x04000FDD RID: 4061
	public int WindowId = 101;

	// Token: 0x04000FDE RID: 4062
	public bool Visible = true;
}
