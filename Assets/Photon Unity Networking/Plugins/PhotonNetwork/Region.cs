using System;

// Token: 0x020002EA RID: 746
public class Region
{
	// Token: 0x06001C8B RID: 7307 RVA: 0x000148FC File Offset: 0x00012AFC
	public Region(CloudRegionCode code)
	{
		this.Code = code;
		this.Cluster = code.ToString();
	}

	// Token: 0x06001C8C RID: 7308 RVA: 0x0001491C File Offset: 0x00012B1C
	public Region(CloudRegionCode code, string regionCodeString, string address)
	{
		this.Code = code;
		this.Cluster = regionCodeString;
		this.HostAndPort = address;
	}

	// Token: 0x06001C8D RID: 7309 RVA: 0x000B42B4 File Offset: 0x000B24B4
	public static CloudRegionCode Parse(string codeAsString)
	{
		if (codeAsString == null)
		{
			return CloudRegionCode.none;
		}
		int num = codeAsString.IndexOf('/');
		if (num > 0)
		{
			codeAsString = codeAsString.Substring(0, num);
		}
		codeAsString = codeAsString.ToLower();
		if (Enum.IsDefined(typeof(CloudRegionCode), codeAsString))
		{
			return (CloudRegionCode)((int)Enum.Parse(typeof(CloudRegionCode), codeAsString));
		}
		return CloudRegionCode.none;
	}

	// Token: 0x06001C8E RID: 7310 RVA: 0x000B4318 File Offset: 0x000B2518
	internal static CloudRegionFlag ParseFlag(CloudRegionCode region)
	{
		if (Enum.IsDefined(typeof(CloudRegionFlag), region.ToString()))
		{
			return (CloudRegionFlag)((int)Enum.Parse(typeof(CloudRegionFlag), region.ToString()));
		}
		return (CloudRegionFlag)0;
	}

	// Token: 0x06001C8F RID: 7311 RVA: 0x000B4368 File Offset: 0x000B2568
	[Obsolete]
	internal static CloudRegionFlag ParseFlag(string codeAsString)
	{
		codeAsString = codeAsString.ToLower();
		CloudRegionFlag result = (CloudRegionFlag)0;
		if (Enum.IsDefined(typeof(CloudRegionFlag), codeAsString))
		{
			result = (CloudRegionFlag)((int)Enum.Parse(typeof(CloudRegionFlag), codeAsString));
		}
		return result;
	}

	// Token: 0x06001C90 RID: 7312 RVA: 0x00014939 File Offset: 0x00012B39
	public override string ToString()
	{
		return string.Format("'{0}' \t{1}ms \t{2}", this.Cluster, this.Ping, this.HostAndPort);
	}

	// Token: 0x0400107C RID: 4220
	public CloudRegionCode Code;

	// Token: 0x0400107D RID: 4221
	public string Cluster;

	// Token: 0x0400107E RID: 4222
	public string HostAndPort;

	// Token: 0x0400107F RID: 4223
	public int Ping;
}
