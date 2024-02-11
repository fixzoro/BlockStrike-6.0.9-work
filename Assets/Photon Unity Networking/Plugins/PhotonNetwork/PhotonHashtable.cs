using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

// Token: 0x020004E6 RID: 1254
public class PhotonHashtable
{
	// Token: 0x1700047B RID: 1147
	// (get) Token: 0x06002AAA RID: 10922 RVA: 0x0001D9F6 File Offset: 0x0001BBF6
	public int Count
	{
		get
		{
			return this.keys.size;
		}
	}

	// Token: 0x06002AAB RID: 10923 RVA: 0x000FDD1C File Offset: 0x000FBF1C
	public bool ContainsKey(byte key)
	{
		for (int i = 0; i < this.keys.size; i++)
		{
			if ((byte)this.keys[i] == (byte)key)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002AAC RID: 10924 RVA: 0x000FDD5C File Offset: 0x000FBF5C
	public byte[] ToArray()
	{
		byte[] array = new byte[this.keys.size + this.nums.size * 2 + this.values.size + 2];
		array[0] = (byte)this.keys.size;
		int num = 1;
		for (int i = 0; i < this.keys.size; i++)
		{
			array[num] = (byte)this.keys[i];
			num++;
		}
		for (int j = 0; j < this.nums.size; j++)
		{
			short num2 = (byte)this.nums[j];
			array[num] = (byte)(num2 >> 8);
			array[num + 1] = (byte)num2;
			num += 2;
		}
		for (int k = 0; k < this.values.size; k++)
		{
			array[num] = (byte)this.values[k];
			num++;
		}
		return array;
	}

	// Token: 0x06002AAD RID: 10925 RVA: 0x000FDE50 File Offset: 0x000FC050
	public void SetData(byte[] bytes)
	{
		this.Clear();
		int num = (int)bytes[0];
		int num2 = 1;
		this.keys.Clear();
		for (int i = 0; i < num; i++)
		{
			this.keys.Add(bytes[num2]);
			num2++;
		}
		this.nums.Clear();
		for (int j = 0; j < num; j++)
		{
			this.nums.Add((short)((int)bytes[num2] << 8 | (int)bytes[num2 + 1]));
			num2 += 2;
		}
		num = bytes.Length - num2;
		this.values.Clear();
		for (int k = 0; k < num; k++)
		{
			this.values.Add(bytes[num2]);
			num2++;
		}
	}

	// Token: 0x06002AAE RID: 10926 RVA: 0x0001DA03 File Offset: 0x0001BC03
	public void Clear()
	{
		this.keys.Clear();
		this.nums.Clear();
		this.values.Clear();
	}

	// Token: 0x06002AAF RID: 10927 RVA: 0x000FDF0C File Offset: 0x000FC10C
	public void Add(byte key, byte value)
	{
		if (this.ContainsKey((byte)key))
		{
			Debug.Log("Exists");
			return;
		}
		this.keys.Add((byte)key);
		this.nums.Add((short)this.values.size);
		this.values.Add((byte)value);
	}

	// Token: 0x06002AB0 RID: 10928 RVA: 0x000FDF60 File Offset: 0x000FC160
	public void Add(byte key, byte[] value)
	{
		if (this.ContainsKey((byte)key))
		{
			Debug.Log("Exists");
			return;
		}
		this.keys.Add((byte)key);
		this.nums.Add((short)this.values.size);
		short num = (short)value.Length;
		this.values.Add((byte)(num << 8));
		this.values.Add((byte)num);
		for (int i = 0; i < value.Length; i++)
		{
			this.values.Add((byte)value[i]);
		}
	}

	// Token: 0x06002AB1 RID: 10929 RVA: 0x000FDFEC File Offset: 0x000FC1EC
	public void Add(byte key, bool value)
	{
		if (this.ContainsKey((byte)key))
		{
			Debug.Log("Exists");
			return;
		}
		this.keys.Add((byte)key);
		this.nums.Add((short)this.values.size);
		this.values.Add((byte)((!value) ? 0 : 1));
	}

	// Token: 0x06002AB2 RID: 10930 RVA: 0x000FE04C File Offset: 0x000FC24C
	public void Add(byte key, int value)
	{
		if (this.ContainsKey(key))
		{
			Debug.Log("Exists");
			return;
		}
		this.keys.Add(key);
		this.nums.Add((short)this.values.size);
		this.values.Add((byte)(value >> 24));
		this.values.Add((byte)(value >> 16));
		this.values.Add((byte)(value >> 8));
		this.values.Add((byte)value);
	}

	// Token: 0x06002AB3 RID: 10931 RVA: 0x000FE0D0 File Offset: 0x000FC2D0
	public void Add(byte key, float value)
	{
		if (this.ContainsKey((byte)key))
		{
			Debug.Log("Exists");
			return;
		}
		this.keys.Add((byte)key);
		this.nums.Add((short)this.values.size);
		this.floatConverter.f = value;
		this.values.Add((byte)this.floatConverter.b4.b1);
		this.values.Add((byte)this.floatConverter.b4.b2);
		this.values.Add((byte)this.floatConverter.b4.b3);
		this.values.Add((byte)this.floatConverter.b4.b4);
	}

	// Token: 0x06002AB4 RID: 10932 RVA: 0x000FE190 File Offset: 0x000FC390
	public void Add(byte key, short value)
	{
		if (this.ContainsKey((byte)key))
		{
			Debug.Log("Exists");
			return;
		}
		this.keys.Add((byte)key);
		this.nums.Add((short)this.values.size);
		this.values.Add((byte)(value >> 8));
		this.values.Add((byte)value);
	}

	// Token: 0x06002AB5 RID: 10933 RVA: 0x000FE1F4 File Offset: 0x000FC3F4
	public void Add(byte key, string value)
	{
		if (this.ContainsKey((byte)key))
		{
			Debug.Log("Exists");
			return;
		}
		this.keys.Add((byte)key);
		this.nums.Add((short)this.values.size);
		byte[] bytes = Encoding.UTF8.GetBytes(value);
		short num = (short)bytes.Length;
		this.values.Add((byte)(num << 8));
		this.values.Add((byte)num);
		for (int i = 0; i < bytes.Length; i++)
		{
			this.values.Add((byte)bytes[i]);
		}
	}

	// Token: 0x06002AB6 RID: 10934 RVA: 0x000FE28C File Offset: 0x000FC48C
	public void Add(byte key, Vector3 value)
	{
		if (this.ContainsKey((byte)key))
		{
			Debug.Log("Exists");
			return;
		}
		this.keys.Add((byte)key);
		this.nums.Add((short)this.values.size);
		this.floatConverter.f = value.x;
		this.values.Add((byte)this.floatConverter.b4.b1);
		this.values.Add((byte)this.floatConverter.b4.b2);
		this.values.Add((byte)this.floatConverter.b4.b3);
		this.values.Add((byte)this.floatConverter.b4.b4);
		this.floatConverter.f = value.y;
		this.values.Add((byte)this.floatConverter.b4.b1);
		this.values.Add((byte)this.floatConverter.b4.b2);
		this.values.Add((byte)this.floatConverter.b4.b3);
		this.values.Add((byte)this.floatConverter.b4.b4);
		this.floatConverter.f = value.z;
		this.values.Add((byte)this.floatConverter.b4.b1);
		this.values.Add((byte)this.floatConverter.b4.b2);
		this.values.Add((byte)this.floatConverter.b4.b3);
		this.values.Add((byte)this.floatConverter.b4.b4);
	}

	// Token: 0x06002AB7 RID: 10935 RVA: 0x000FE450 File Offset: 0x000FC650
	public void Add(byte key, Quaternion value)
	{
		if (this.ContainsKey((byte)key))
		{
			Debug.Log("Exists");
			return;
		}
		this.keys.Add((byte)key);
		this.nums.Add((short)this.values.size);
		this.floatConverter.f = value.x;
		this.values.Add((byte)this.floatConverter.b4.b1);
		this.values.Add((byte)this.floatConverter.b4.b2);
		this.values.Add((byte)this.floatConverter.b4.b3);
		this.values.Add((byte)this.floatConverter.b4.b4);
		this.floatConverter.f = value.y;
		this.values.Add((byte)this.floatConverter.b4.b1);
		this.values.Add((byte)this.floatConverter.b4.b2);
		this.values.Add((byte)this.floatConverter.b4.b3);
		this.values.Add((byte)this.floatConverter.b4.b4);
		this.floatConverter.f = value.z;
		this.values.Add((byte)this.floatConverter.b4.b1);
		this.values.Add((byte)this.floatConverter.b4.b2);
		this.values.Add((byte)this.floatConverter.b4.b3);
		this.values.Add((byte)this.floatConverter.b4.b4);
		this.floatConverter.f = value.w;
		this.values.Add((byte)this.floatConverter.b4.b1);
		this.values.Add((byte)this.floatConverter.b4.b2);
		this.values.Add((byte)this.floatConverter.b4.b3);
		this.values.Add((byte)this.floatConverter.b4.b4);
	}

	// Token: 0x06002AB8 RID: 10936 RVA: 0x0001DA26 File Offset: 0x0001BC26
	public byte GetByte(byte key)
	{
		return (byte)this.values[this.GetNum((byte)key)];
	}

	// Token: 0x06002AB9 RID: 10937 RVA: 0x000FE690 File Offset: 0x000FC890
	public byte[] GetBytes(byte key)
	{
		int num = this.GetNum(key);
		short num2 = (short)((int)this.values[num] << 8 | (int)this.values[num + 1]);
		byte[] array = new byte[(int)num2];
		num += 2;
		for (int i = 0; i < (int)num2; i++)
		{
			array[i] = this.values[num];
			num++;
		}
		return array;
	}

	// Token: 0x06002ABA RID: 10938 RVA: 0x0001DA3A File Offset: 0x0001BC3A
	public bool GetBool(byte key)
	{
		return (byte)this.values[this.GetNum(key)] != (byte)0;
	}

	// Token: 0x06002ABB RID: 10939 RVA: 0x000FE6F8 File Offset: 0x000FC8F8
	public int GetInt(byte key)
	{
		int num = this.GetNum(key);
		return (int)this.values[num] << 24 | (int)this.values[num + 1] << 16 | (int)this.values[num + 2] << 8 | (int)this.values[num + 3];
	}

	// Token: 0x06002ABC RID: 10940 RVA: 0x000FE750 File Offset: 0x000FC950
	public float GetFloat(byte key)
	{
		int num = this.GetNum(key);
		this.floatBytes.b1 = (byte)this.values[num];
		this.floatBytes.b2 = (byte)this.values[num + 1];
		this.floatBytes.b3 = (byte)this.values[num + 2];
		this.floatBytes.b4 = (byte)this.values[num + 3];
		this.floatConverter.b4 = this.floatBytes;
		return this.floatConverter.f;
	}

	// Token: 0x06002ABD RID: 10941 RVA: 0x000FE7E4 File Offset: 0x000FC9E4
	public short GetShort(byte key)
	{
		int num = this.GetNum(key);
		return (short)((int)this.values[num] << 8 | (int)this.values[num + 1]);
	}

	// Token: 0x06002ABE RID: 10942 RVA: 0x000FE818 File Offset: 0x000FCA18
	public string GetString(byte key)
	{
		int num = this.GetNum(key);
		short num2 = (short)((int)this.values[num] << 8 | (int)this.values[num + 1]);
		byte[] array = new byte[(int)num2];
		num += 2;
		for (int i = 0; i < (int)num2; i++)
		{
			array[i] = this.values[num];
			num++;
		}
		return Encoding.UTF8.GetString(array);
	}

	// Token: 0x06002ABF RID: 10943 RVA: 0x000FE888 File Offset: 0x000FCA88
	public Vector3 GetVector3(byte key)
	{
		int num = this.GetNum(key);
		Vector3 result = default(Vector3);
		this.floatBytes.b1 = this.values[num];
		this.floatBytes.b2 = this.values[num + 1];
		this.floatBytes.b3 = this.values[num + 2];
		this.floatBytes.b4 = this.values[num + 3];
		this.floatConverter.b4 = this.floatBytes;
		result.x = this.floatConverter.f;
		this.floatBytes.b1 = this.values[num + 4];
		this.floatBytes.b2 = this.values[num + 5];
		this.floatBytes.b3 = this.values[num + 6];
		this.floatBytes.b4 = this.values[num + 7];
		this.floatConverter.b4 = this.floatBytes;
		result.y = this.floatConverter.f;
		this.floatBytes.b1 = this.values[num + 8];
		this.floatBytes.b2 = this.values[num + 9];
		this.floatBytes.b3 = this.values[num + 10];
		this.floatBytes.b4 = this.values[num + 11];
		this.floatConverter.b4 = this.floatBytes;
		result.z = this.floatConverter.f;
		return result;
	}

	// Token: 0x06002AC0 RID: 10944 RVA: 0x000FEA3C File Offset: 0x000FCC3C
	public Quaternion GetQuaternion(byte key)
	{
		int num = this.GetNum(key);
		Quaternion result = default(Quaternion);
		this.floatBytes.b1 = this.values[num];
		this.floatBytes.b2 = this.values[num + 1];
		this.floatBytes.b3 = this.values[num + 2];
		this.floatBytes.b4 = this.values[num + 3];
		this.floatConverter.b4 = this.floatBytes;
		result.x = this.floatConverter.f;
		this.floatBytes.b1 = this.values[num + 4];
		this.floatBytes.b2 = this.values[num + 5];
		this.floatBytes.b3 = this.values[num + 6];
		this.floatBytes.b4 = this.values[num + 7];
		this.floatConverter.b4 = this.floatBytes;
		result.y = this.floatConverter.f;
		this.floatBytes.b1 = this.values[num + 8];
		this.floatBytes.b2 = this.values[num + 9];
		this.floatBytes.b3 = this.values[num + 10];
		this.floatBytes.b4 = this.values[num + 11];
		this.floatConverter.b4 = this.floatBytes;
		result.z = this.floatConverter.f;
		this.floatBytes.b1 = this.values[num + 12];
		this.floatBytes.b2 = this.values[num + 13];
		this.floatBytes.b3 = this.values[num + 14];
		this.floatBytes.b4 = this.values[num + 15];
		this.floatConverter.b4 = this.floatBytes;
		result.w = this.floatConverter.f;
		return result;
	}

	// Token: 0x06002AC1 RID: 10945 RVA: 0x000FEC7C File Offset: 0x000FCE7C
	private int GetNum(byte key)
	{
		for (int i = 0; i < this.keys.size; i++)
		{
			if (this.keys[i] == key)
			{
				return (int)this.nums[i];
			}
		}
		return 0;
	}

	// Token: 0x04001B86 RID: 7046
	private BetterList<byte> keys = new BetterList<byte>();

	// Token: 0x04001B87 RID: 7047
	private BetterList<short> nums = new BetterList<short>();

	// Token: 0x04001B88 RID: 7048
	private BetterList<byte> values = new BetterList<byte>();

	// Token: 0x04001B89 RID: 7049
	private PhotonHashtable.FloatBytesUnion floatConverter = default(PhotonHashtable.FloatBytesUnion);

	// Token: 0x04001B8A RID: 7050
	private PhotonHashtable.Byte4 floatBytes = default(PhotonHashtable.Byte4);

	// Token: 0x020004E7 RID: 1255
	private struct Byte4
	{
		// Token: 0x04001B8B RID: 7051
		public byte b1;

		// Token: 0x04001B8C RID: 7052
		public byte b2;

		// Token: 0x04001B8D RID: 7053
		public byte b3;

		// Token: 0x04001B8E RID: 7054
		public byte b4;
	}

	// Token: 0x020004E8 RID: 1256
	[StructLayout(LayoutKind.Explicit)]
	private struct FloatBytesUnion
	{
		// Token: 0x04001B8F RID: 7055
		[FieldOffset(0)]
		public float f;

		// Token: 0x04001B90 RID: 7056
		[FieldOffset(0)]
		public PhotonHashtable.Byte4 b4;
	}
}
