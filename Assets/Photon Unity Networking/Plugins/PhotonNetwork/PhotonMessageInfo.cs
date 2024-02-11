using System;

// Token: 0x020002CA RID: 714
public struct PhotonMessageInfo
{
	// Token: 0x06001AF6 RID: 6902 RVA: 0x00013716 File Offset: 0x00011916
	public PhotonMessageInfo(PhotonPlayer player, int timestamp, PhotonView view)
	{
		this.sender = player;
		this.timeInt = timestamp;
		this.photonView = view;
	}

	// Token: 0x1700039F RID: 927
	// (get) Token: 0x06001AF7 RID: 6903 RVA: 0x000B0898 File Offset: 0x000AEA98
	public double timestamp
	{
		get
		{
			uint num = (uint)this.timeInt;
			double num2 = num;
			return num2 / 1000.0;
		}
	}

	// Token: 0x06001AF8 RID: 6904 RVA: 0x0001372D File Offset: 0x0001192D
	public override string ToString()
	{
		return string.Format("[PhotonMessageInfo: Sender='{1}' Senttime={0}]", this.timestamp, this.sender);
	}

	// Token: 0x04000FAF RID: 4015
	private readonly int timeInt;

	// Token: 0x04000FB0 RID: 4016
	public readonly PhotonPlayer sender;

	// Token: 0x04000FB1 RID: 4017
	public readonly PhotonView photonView;
}
