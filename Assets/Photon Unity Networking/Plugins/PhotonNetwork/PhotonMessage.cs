using System;
using UnityEngine;

// Token: 0x020004E9 RID: 1257
public class PhotonMessage
{
	// Token: 0x1700047C RID: 1148
	// (get) Token: 0x06002AC3 RID: 10947 RVA: 0x000FECC8 File Offset: 0x000FCEC8
	public double timestamp
	{
		get
		{
			uint num = (uint)this.timeInt;
			double num2 = num;
			return num2 / 1000.0;
		}
	}

	// Token: 0x06002AC4 RID: 10948 RVA: 0x0001DA67 File Offset: 0x0001BC67
	public void SetData(byte[] bytes, int senderID, int sendTime)
	{
		nProfiler.BeginSample("PhotonMessage.SetData");
		this.readData.Clear();
		this.readData.bytes = bytes;
		this.sender = PhotonPlayer.Find(senderID);
		this.timeInt = sendTime;
		nProfiler.EndSample();
	}

	// Token: 0x06002AC5 RID: 10949 RVA: 0x0001DAA2 File Offset: 0x0001BCA2
	public void Clear()
	{
		this.readData.Clear();
	}

	// Token: 0x06002AC6 RID: 10950 RVA: 0x0001DAAF File Offset: 0x0001BCAF
	public byte ReadByte()
	{
		return this.readData.ReadByte();
	}

	// Token: 0x06002AC7 RID: 10951 RVA: 0x0001DABC File Offset: 0x0001BCBC
	public bool ReadBool()
	{
		return this.readData.ReadBool();
	}

	// Token: 0x06002AC8 RID: 10952 RVA: 0x0001DAC9 File Offset: 0x0001BCC9
	public byte[] ReadBytes()
	{
		return this.readData.ReadBytes();
	}

	// Token: 0x06002AC9 RID: 10953 RVA: 0x0001DAD6 File Offset: 0x0001BCD6
	public int ReadInt()
	{
		return this.readData.ReadInt();
	}

	// Token: 0x06002ACA RID: 10954 RVA: 0x0001DAE3 File Offset: 0x0001BCE3
	public short ReadShort()
	{
		return this.readData.ReadShort();
	}

	// Token: 0x06002ACB RID: 10955 RVA: 0x0001DAF0 File Offset: 0x0001BCF0
	public short[] ReadShorts()
	{
		return this.readData.ReadShorts();
	}

	// Token: 0x06002ACC RID: 10956 RVA: 0x0001DAFD File Offset: 0x0001BCFD
	public float ReadFloat()
	{
		return this.readData.ReadFloat();
	}

	// Token: 0x06002ACD RID: 10957 RVA: 0x0001DB0A File Offset: 0x0001BD0A
	public string ReadString()
	{
		return this.readData.ReadString();
	}

	// Token: 0x06002ACE RID: 10958 RVA: 0x0001DB17 File Offset: 0x0001BD17
	public Vector3 ReadVector3()
	{
		return this.readData.ReadVector3();
	}

	// Token: 0x06002ACF RID: 10959 RVA: 0x0001DB24 File Offset: 0x0001BD24
	public Quaternion ReadQuaternion()
	{
		return this.readData.ReadQuaternion();
	}

	// Token: 0x04001B91 RID: 7057
	public PhotonDataRead readData = new PhotonDataRead();

	// Token: 0x04001B92 RID: 7058
	private int timeInt;

	// Token: 0x04001B93 RID: 7059
	public PhotonPlayer sender;
}
