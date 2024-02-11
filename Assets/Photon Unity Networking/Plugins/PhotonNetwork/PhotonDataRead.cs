using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

// Token: 0x020004E0 RID: 1248
public class PhotonDataRead
{
	// Token: 0x06002A8B RID: 10891 RVA: 0x000FD968 File Offset: 0x000FBB68
	public PhotonDataRead()
	{
	}

	// Token: 0x06002A8C RID: 10892 RVA: 0x000FD99C File Offset: 0x000FBB9C
	public PhotonDataRead(byte[] value)
	{
		this.bytes = value;
	}

	// Token: 0x06002A8D RID: 10893 RVA: 0x0001D848 File Offset: 0x0001BA48
	public void Clear()
	{
		this.index = 0;
	}

	// Token: 0x06002A8E RID: 10894 RVA: 0x0001D851 File Offset: 0x0001BA51
	public bool ReadBool()
	{
		return this.ReadByte() != 0;
	}

	// Token: 0x06002A8F RID: 10895 RVA: 0x000FD9D4 File Offset: 0x000FBBD4
	public byte ReadByte()
	{
		byte result = this.bytes[this.index];
		this.index++;
		return result;
	}

	// Token: 0x06002A90 RID: 10896 RVA: 0x000FDA00 File Offset: 0x000FBC00
	public byte[] ReadBytes()
	{
		nProfiler.BeginSample("PhotonDataRead.ReadBytes");
		int num = (int)this.ReadShort();
		byte[] array = new byte[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = this.ReadByte();
		}
		nProfiler.EndSample();
		return array;
	}

	// Token: 0x06002A91 RID: 10897 RVA: 0x0001D85F File Offset: 0x0001BA5F
	public int ReadInt()
	{
		return (int)this.ReadByte() << 24 | (int)this.ReadByte() << 16 | (int)this.ReadByte() << 8 | (int)this.ReadByte();
	}

	// Token: 0x06002A92 RID: 10898 RVA: 0x0001D884 File Offset: 0x0001BA84
	public short ReadShort()
	{
		return (short)((int)this.ReadByte() << 8 | (int)this.ReadByte());
	}

	// Token: 0x06002A93 RID: 10899 RVA: 0x000FDA48 File Offset: 0x000FBC48
	public short[] ReadShorts()
	{
		int num = (int)this.ReadShort();
		short[] array = new short[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = this.ReadShort();
		}
		return array;
	}

	// Token: 0x06002A94 RID: 10900 RVA: 0x000FDA80 File Offset: 0x000FBC80
	public float ReadFloat()
	{
		this.floatBytes.b1 = this.ReadByte();
		this.floatBytes.b2 = this.ReadByte();
		this.floatBytes.b3 = this.ReadByte();
		this.floatBytes.b4 = this.ReadByte();
		this.floatConverter.b = this.floatBytes;
		return this.floatConverter.f;
	}

	// Token: 0x06002A95 RID: 10901 RVA: 0x000FDAF0 File Offset: 0x000FBCF0
	public string ReadString()
	{
		byte[] array = this.ReadBytes();
		return Encoding.UTF8.GetString(array);
	}

	// Token: 0x06002A96 RID: 10902 RVA: 0x0001D896 File Offset: 0x0001BA96
	public Vector3 ReadVector3()
	{
		return new Vector3(this.ReadFloat(), this.ReadFloat(), this.ReadFloat());
	}

	// Token: 0x06002A97 RID: 10903 RVA: 0x0001D8AF File Offset: 0x0001BAAF
	public Quaternion ReadQuaternion()
	{
		return new Quaternion(this.ReadFloat(), this.ReadFloat(), this.ReadFloat(), this.ReadFloat());
	}

	// Token: 0x04001B74 RID: 7028
	public byte[] bytes;

	// Token: 0x04001B75 RID: 7029
	public int index;

	// Token: 0x04001B76 RID: 7030
	private PhotonDataRead.FloatBytesUnion floatConverter = default(PhotonDataRead.FloatBytesUnion);

	// Token: 0x04001B77 RID: 7031
	private PhotonDataRead.Byte4 floatBytes = default(PhotonDataRead.Byte4);

	// Token: 0x020004E1 RID: 1249
	private struct Byte4
	{
		// Token: 0x04001B78 RID: 7032
		public byte b1;

		// Token: 0x04001B79 RID: 7033
		public byte b2;

		// Token: 0x04001B7A RID: 7034
		public byte b3;

		// Token: 0x04001B7B RID: 7035
		public byte b4;
	}

	// Token: 0x020004E2 RID: 1250
	[StructLayout(LayoutKind.Explicit)]
	private struct FloatBytesUnion
	{
		// Token: 0x04001B7C RID: 7036
		[FieldOffset(0)]
		public float f;

		// Token: 0x04001B7D RID: 7037
		[FieldOffset(0)]
		public PhotonDataRead.Byte4 b;
	}
}
