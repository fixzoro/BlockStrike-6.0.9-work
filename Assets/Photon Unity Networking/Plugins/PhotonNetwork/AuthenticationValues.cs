using System;
using System.Collections.Generic;

// Token: 0x020002BD RID: 701
public class AuthenticationValues
{
	// Token: 0x06001A1B RID: 6683 RVA: 0x00013125 File Offset: 0x00011325
	public AuthenticationValues()
	{
	}

	// Token: 0x06001A1C RID: 6684 RVA: 0x00013138 File Offset: 0x00011338
	public AuthenticationValues(string userId)
	{
		this.UserId = userId;
	}

	// Token: 0x17000382 RID: 898
	// (get) Token: 0x06001A1D RID: 6685 RVA: 0x00013152 File Offset: 0x00011352
	// (set) Token: 0x06001A1E RID: 6686 RVA: 0x0001315A File Offset: 0x0001135A
	public CustomAuthenticationType AuthType
	{
		get
		{
			return this.authType;
		}
		set
		{
			this.authType = value;
		}
	}

	// Token: 0x17000383 RID: 899
	// (get) Token: 0x06001A1F RID: 6687 RVA: 0x00013163 File Offset: 0x00011363
	// (set) Token: 0x06001A20 RID: 6688 RVA: 0x0001316B File Offset: 0x0001136B
	public string AuthGetParameters { get; set; }

	// Token: 0x17000384 RID: 900
	// (get) Token: 0x06001A21 RID: 6689 RVA: 0x00013174 File Offset: 0x00011374
	// (set) Token: 0x06001A22 RID: 6690 RVA: 0x0001317C File Offset: 0x0001137C
	public object AuthPostData { get; private set; }

	// Token: 0x17000385 RID: 901
	// (get) Token: 0x06001A23 RID: 6691 RVA: 0x00013185 File Offset: 0x00011385
	// (set) Token: 0x06001A24 RID: 6692 RVA: 0x0001318D File Offset: 0x0001138D
	public string Token { get; set; }

	// Token: 0x17000386 RID: 902
	// (get) Token: 0x06001A25 RID: 6693 RVA: 0x00013196 File Offset: 0x00011396
	// (set) Token: 0x06001A26 RID: 6694 RVA: 0x0001319E File Offset: 0x0001139E
	public string UserId { get; set; }

	// Token: 0x06001A27 RID: 6695 RVA: 0x000131A7 File Offset: 0x000113A7
	public virtual void SetAuthPostData(string stringData)
	{
		this.AuthPostData = ((!string.IsNullOrEmpty(stringData)) ? stringData : null);
	}

	// Token: 0x06001A28 RID: 6696 RVA: 0x000131C1 File Offset: 0x000113C1
	public virtual void SetAuthPostData(byte[] byteData)
	{
		this.AuthPostData = byteData;
	}

	// Token: 0x06001A29 RID: 6697 RVA: 0x000131C1 File Offset: 0x000113C1
	public virtual void SetAuthPostData(Dictionary<string, object> dictData)
	{
		this.AuthPostData = dictData;
	}

	// Token: 0x06001A2A RID: 6698 RVA: 0x000AA5B0 File Offset: 0x000A87B0
	public virtual void AddAuthParameter(string key, string value)
	{
		string text = (!string.IsNullOrEmpty(this.AuthGetParameters)) ? "&" : string.Empty;
		this.AuthGetParameters = string.Format("{0}{1}{2}={3}", new object[]
		{
			this.AuthGetParameters,
			text,
			Uri.EscapeDataString(key),
			Uri.EscapeDataString(value)
		});
	}

	// Token: 0x06001A2B RID: 6699 RVA: 0x000131CA File Offset: 0x000113CA
	public override string ToString()
	{
		return string.Format("AuthenticationValues UserId: {0}, GetParameters: {1} Token available: {2}", this.UserId, this.AuthGetParameters, this.Token != null);
	}

	// Token: 0x04000F2A RID: 3882
	private CustomAuthenticationType authType = CustomAuthenticationType.None;
}
