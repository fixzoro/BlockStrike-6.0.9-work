using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

// Token: 0x020004E3 RID: 1251
public class PhotonDataWrite
{
	// Token: 0x06002A98 RID: 10904 RVA: 0x000FDB10 File Offset: 0x000FBD10
	public PhotonDataWrite()
	{
		this.bytes = new BetterList<byte>();
	}

	// Token: 0x06002A99 RID: 10905 RVA: 0x000FDB40 File Offset: 0x000FBD40
	public PhotonDataWrite(int size)
	{
		this.bytes = new BetterList<byte>();
		this.bytes.size = size;
	}

	// Token: 0x06002A9A RID: 10906 RVA: 0x0001D8CE File Offset: 0x0001BACE
	public void Clear()
	{
		this.bytes.Clear();
	}

	// Token: 0x06002A9B RID: 10907 RVA: 0x0001D8DB File Offset: 0x0001BADB
	public byte[] ToArray()
	{
		return this.bytes.ToArray();
	}

	// Token: 0x06002A9C RID: 10908 RVA: 0x0001D8E8 File Offset: 0x0001BAE8
	public void Write(byte value)
	{
		this.bytes.Add((byte)value);
	}

	// Token: 0x06002A9D RID: 10909 RVA: 0x000FDB7C File Offset: 0x000FBD7C
	public void Write(byte[] value)
	{
		this.Write((short)value.Length);
		for (int i = 0; i < value.Length; i++)
		{
			this.bytes.Add((byte)value[i]);
		}
	}

	// Token: 0x06002A9E RID: 10910 RVA: 0x0001D8F6 File Offset: 0x0001BAF6
	public void Write(bool value)
	{
		this.bytes.Add((byte)((!value) ? 0 : 1));
	}

	// Token: 0x06002A9F RID: 10911 RVA: 0x0001D911 File Offset: 0x0001BB11
	public void Write(int value)
	{
		this.bytes.Add((byte)(value >> 24));
		this.bytes.Add((byte)(value >> 16));
		this.bytes.Add((byte)(value >> 8));
		this.bytes.Add((byte)value);
	}

	// Token: 0x06002AA0 RID: 10912 RVA: 0x000FDBB8 File Offset: 0x000FBDB8
	public void Write(int[] value)
	{
		this.Write((short)value.Length);
		for (int i = 0; i < value.Length; i++)
		{
			this.Write((byte)value[i]);
		}
	}

	// Token: 0x06002AA1 RID: 10913 RVA: 0x000FDBEC File Offset: 0x000FBDEC
	public void Write(float value)
	{
		this.floatConverter.f = value;
		this.bytes.Add((byte)this.floatConverter.b4.b1);
		this.bytes.Add((byte)this.floatConverter.b4.b2);
		this.bytes.Add((byte)this.floatConverter.b4.b3);
		this.bytes.Add((byte)this.floatConverter.b4.b4);
	}

	// Token: 0x06002AA2 RID: 10914 RVA: 0x0001D94F File Offset: 0x0001BB4F
	public void Write(short value)
	{
		this.bytes.Add((byte)(value >> 8));
		this.bytes.Add((byte)value);
	}

	// Token: 0x06002AA3 RID: 10915 RVA: 0x000FDC74 File Offset: 0x000FBE74
	public void Write(short[] value)
	{
		this.Write((short)value.Length);
		for (int i = 0; i < value.Length; i++)
		{
			this.Write((byte)value[i]);
		}
	}

	// Token: 0x06002AA4 RID: 10916 RVA: 0x000FDCA8 File Offset: 0x000FBEA8
	public void Write(string value)
	{
		byte[] value2 = Encoding.UTF8.GetBytes(value);
		this.Write(value2);
	}

	// Token: 0x06002AA5 RID: 10917 RVA: 0x0001D96D File Offset: 0x0001BB6D
	public void Write(Vector3 value)
	{
		this.Write(value.x);
		this.Write(value.y);
		this.Write(value.z);
	}

	// Token: 0x06002AA6 RID: 10918 RVA: 0x0001D996 File Offset: 0x0001BB96
	public void Write(Vector2 value)
	{
		this.Write(value.x);
		this.Write(value.y);
	}

	// Token: 0x06002AA7 RID: 10919 RVA: 0x0001D9B2 File Offset: 0x0001BBB2
	public void Write(Quaternion value)
	{
		this.Write(value.x);
		this.Write(value.y);
		this.Write(value.z);
		this.Write(value.w);
	}

	// Token: 0x06002AA8 RID: 10920 RVA: 0x0001D9E8 File Offset: 0x0001BBE8
	public void Write(PhotonPlayer value)
	{
		this.Write(value.ID);
	}

	// Token: 0x04001B7E RID: 7038
	public BetterList<byte> bytes;

	// Token: 0x04001B7F RID: 7039
	private PhotonDataWrite.FloatBytesUnion floatConverter = default(PhotonDataWrite.FloatBytesUnion);

	// Token: 0x020004E4 RID: 1252
	[Serializable]
	private struct Byte4
	{
		// Token: 0x04001B80 RID: 7040
		public byte b1;

		// Token: 0x04001B81 RID: 7041
		public byte b2;

		// Token: 0x04001B82 RID: 7042
		public byte b3;

		// Token: 0x04001B83 RID: 7043
		public byte b4;
	}

	// Token: 0x020004E5 RID: 1253
	[StructLayout(LayoutKind.Explicit)]
	private struct FloatBytesUnion
	{
		// Token: 0x04001B84 RID: 7044
		[FieldOffset(0)]
		public float f;

		// Token: 0x04001B85 RID: 7045
		[FieldOffset(0)]
		public PhotonDataWrite.Byte4 b4;
	}
}
