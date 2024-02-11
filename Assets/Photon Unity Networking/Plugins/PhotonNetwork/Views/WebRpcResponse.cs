using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;

// Token: 0x020002D0 RID: 720
public class WebRpcResponse
{
	// Token: 0x06001B02 RID: 6914 RVA: 0x000B08BC File Offset: 0x000AEABC
	public WebRpcResponse(OperationResponse response)
	{
		object obj;
		response.Parameters.TryGetValue(209, out obj);
		this.Name = (obj as string);
		response.Parameters.TryGetValue(207, out obj);
		this.ReturnCode = ((obj == null) ? -1 : ((int)((byte)obj)));
		response.Parameters.TryGetValue(208, out obj);
		this.Parameters = (obj as Dictionary<string, object>);
		response.Parameters.TryGetValue(206, out obj);
		this.DebugMessage = (obj as string);
	}

	// Token: 0x170003A1 RID: 929
	// (get) Token: 0x06001B03 RID: 6915 RVA: 0x0001378B File Offset: 0x0001198B
	// (set) Token: 0x06001B04 RID: 6916 RVA: 0x00013793 File Offset: 0x00011993
	public string Name { get; private set; }

	// Token: 0x170003A2 RID: 930
	// (get) Token: 0x06001B05 RID: 6917 RVA: 0x0001379C File Offset: 0x0001199C
	// (set) Token: 0x06001B06 RID: 6918 RVA: 0x000137A4 File Offset: 0x000119A4
	public int ReturnCode { get; private set; }

	// Token: 0x170003A3 RID: 931
	// (get) Token: 0x06001B07 RID: 6919 RVA: 0x000137AD File Offset: 0x000119AD
	// (set) Token: 0x06001B08 RID: 6920 RVA: 0x000137B5 File Offset: 0x000119B5
	public string DebugMessage { get; private set; }

	// Token: 0x170003A4 RID: 932
	// (get) Token: 0x06001B09 RID: 6921 RVA: 0x000137BE File Offset: 0x000119BE
	// (set) Token: 0x06001B0A RID: 6922 RVA: 0x000137C6 File Offset: 0x000119C6
	public Dictionary<string, object> Parameters { get; private set; }

	// Token: 0x06001B0B RID: 6923 RVA: 0x000137CF File Offset: 0x000119CF
	public string ToStringFull()
	{
		return string.Format("{0}={2}: {1} \"{3}\"", new object[]
		{
			this.Name,
			SupportClass.DictionaryToString(this.Parameters),
			this.ReturnCode,
			this.DebugMessage
		});
	}
}
