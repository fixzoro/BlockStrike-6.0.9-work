using System;

// Token: 0x020002B7 RID: 695
public class RaiseEventOptions
{
	// Token: 0x06001A13 RID: 6675 RVA: 0x000AA4E0 File Offset: 0x000A86E0
	public void Reset()
	{
		this.CachingOption = RaiseEventOptions.Default.CachingOption;
		this.InterestGroup = RaiseEventOptions.Default.InterestGroup;
		this.TargetActors = RaiseEventOptions.Default.TargetActors;
		this.Receivers = RaiseEventOptions.Default.Receivers;
		this.SequenceChannel = RaiseEventOptions.Default.SequenceChannel;
		this.ForwardToWebhook = RaiseEventOptions.Default.ForwardToWebhook;
		this.Encrypt = RaiseEventOptions.Default.Encrypt;
	}

	// Token: 0x04000F0D RID: 3853
	public static readonly RaiseEventOptions Default = new RaiseEventOptions();

	// Token: 0x04000F0E RID: 3854
	public EventCaching CachingOption;

	// Token: 0x04000F0F RID: 3855
	public byte InterestGroup;

	// Token: 0x04000F10 RID: 3856
	public int[] TargetActors;

	// Token: 0x04000F11 RID: 3857
	public ReceiverGroup Receivers;

	// Token: 0x04000F12 RID: 3858
	public byte SequenceChannel;

	// Token: 0x04000F13 RID: 3859
	public bool ForwardToWebhook;

	// Token: 0x04000F14 RID: 3860
	public bool Encrypt;
}
