using System;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public struct CryptoVector4 : IEquatable<CryptoVector4>
{
    [SerializeField]
    private int cryptoKey;

    [SerializeField]
    private CryptoVector4.EncryptVector4 hiddenValue;

    [SerializeField]
    private Vector4 fakeValue;

    [SerializeField]
    private bool inited;

    private CryptoVector4(Vector4 value)
	{
		this.cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
		CryptoVector4.FloatIntUnion floatIntUnion = default(CryptoVector4.FloatIntUnion);
		floatIntUnion.f = value.x;
		CryptoVector4.EncryptVector4 encryptVector;
		encryptVector.x = (floatIntUnion.i ^ this.cryptoKey);
		CryptoVector4.FloatIntUnion floatIntUnion2 = default(CryptoVector4.FloatIntUnion);
		floatIntUnion2.f = value.y;
		encryptVector.y = (floatIntUnion2.i ^ this.cryptoKey);
		CryptoVector4.FloatIntUnion floatIntUnion3 = default(CryptoVector4.FloatIntUnion);
		floatIntUnion3.f = value.z;
		encryptVector.z = (floatIntUnion3.i ^ this.cryptoKey);
		CryptoVector4.FloatIntUnion floatIntUnion4 = default(CryptoVector4.FloatIntUnion);
		floatIntUnion4.f = value.w;
		encryptVector.w = (floatIntUnion4.i ^ this.cryptoKey);
		this.hiddenValue = encryptVector;
		this.fakeValue = value;
		this.inited = true;
	}

	public float x
	{
		get
		{
			float num = this.DecryptFloat(this.hiddenValue.x);
			if (this.fakeValue.x != num)
			{
				CryptoManager.CheatingDetected();
			}
			return num;
		}
	}

	public float y
	{
		get
		{
			float num = this.DecryptFloat(this.hiddenValue.y);
			if (this.fakeValue.y != num)
			{
				CryptoManager.CheatingDetected();
			}
			return num;
		}
	}

	public float z
	{
		get
		{
			float num = this.DecryptFloat(this.hiddenValue.z);
			if (this.fakeValue.z != num)
			{
				CryptoManager.CheatingDetected();
			}
			return num;
		}
	}

	public float w
	{
		get
		{
			float num = this.DecryptFloat(this.hiddenValue.w);
			if (this.fakeValue.z != num)
			{
				CryptoManager.CheatingDetected();
			}
			return num;
		}
	}

	public void SetValue(Vector4 value)
	{
		this.cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
		this.hiddenValue = this.Encrypt(value);
		this.fakeValue = value;
	}

	private Vector4 GetValue()
	{
		if (!this.inited)
		{
			this.cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
			this.hiddenValue = this.Encrypt(Vector4.zero);
			this.fakeValue = Vector4.zero;
			this.inited = true;
		}
		Vector4 vector = this.Decrypt(this.hiddenValue);
		if (this.fakeValue != vector)
		{
			CryptoManager.CheatingDetected();
		}
		return vector;
	}

	private CryptoVector4.EncryptVector4 Encrypt(Vector4 value)
	{
		CryptoVector4.EncryptVector4 result;
		result.x = this.EncryptFloat(value.x);
		result.y = this.EncryptFloat(value.y);
		result.z = this.EncryptFloat(value.z);
		result.w = this.EncryptFloat(value.w);
		return result;
	}

	private int EncryptFloat(float value)
	{
		CryptoVector4.FloatIntUnion floatIntUnion = default(CryptoVector4.FloatIntUnion);
		floatIntUnion.f = value;
		floatIntUnion.i ^= this.cryptoKey;
		return floatIntUnion.i;
	}

	private Vector4 Decrypt(CryptoVector4.EncryptVector4 value)
	{
		Vector4 result;
		result.x = this.DecryptFloat(value.x);
		result.y = this.DecryptFloat(value.y);
		result.z = this.DecryptFloat(value.z);
		result.w = this.DecryptFloat(value.w);
		return result;
	}

	private float DecryptFloat(int value)
	{
		CryptoVector4.FloatIntUnion floatIntUnion = default(CryptoVector4.FloatIntUnion);
		floatIntUnion.i = (value ^ this.cryptoKey);
		return floatIntUnion.f;
	}

	public override bool Equals(object obj)
	{
		return obj is CryptoVector4 && this.Equals((CryptoVector4)obj);
	}

	public bool Equals(CryptoVector4 obj)
	{
		return this.GetValue() == obj.GetValue();
	}

	public override int GetHashCode()
	{
		return this.GetValue().GetHashCode();
	}

	public override string ToString()
	{
		return this.GetValue().ToString();
	}

	public static implicit operator CryptoVector4(Vector4 value)
	{
		return new CryptoVector4(value);
	}

	public static implicit operator Vector4(CryptoVector4 value)
	{
		return value.GetValue();
	}

	public static Vector4 operator +(CryptoVector4 a, CryptoVector4 b)
	{
		return a.GetValue() + b.GetValue();
	}

	public static Vector4 operator +(Vector4 a, CryptoVector4 b)
	{
		return a + b.GetValue();
	}

	public static Vector4 operator +(CryptoVector4 a, Vector4 b)
	{
		return a.GetValue() + b;
	}

	public static Vector4 operator -(CryptoVector4 a, CryptoVector4 b)
	{
		return a.GetValue() - b.GetValue();
	}

	public static Vector4 operator -(Vector4 a, CryptoVector4 b)
	{
		return a - b.GetValue();
	}

	public static Vector4 operator -(CryptoVector4 a, Vector4 b)
	{
		return a.GetValue() - b;
	}

	public static Vector4 operator -(CryptoVector4 a)
	{
		return -a.GetValue();
	}

	public static Vector4 operator *(CryptoVector4 a, float d)
	{
		return a.GetValue() * d;
	}

	public static Vector4 operator *(float d, CryptoVector4 a)
	{
		return d * a.GetValue();
	}

	public static Vector4 operator /(CryptoVector4 a, float d)
	{
		return a.GetValue() / d;
	}

	public static bool operator ==(CryptoVector4 lhs, CryptoVector4 rhs)
	{
		return lhs.GetValue() == rhs.GetValue();
	}

	public static bool operator ==(Vector4 lhs, CryptoVector4 rhs)
	{
		return lhs == rhs.GetValue();
	}

	public static bool operator ==(CryptoVector4 lhs, Vector4 rhs)
	{
		return lhs.GetValue() == rhs;
	}

	public static bool operator !=(CryptoVector4 lhs, CryptoVector4 rhs)
	{
		return lhs.GetValue() != rhs.GetValue();
	}

	public static bool operator !=(Vector4 lhs, CryptoVector4 rhs)
	{
		return lhs != rhs.GetValue();
	}

	public static bool operator !=(CryptoVector4 lhs, Vector4 rhs)
	{
		return lhs.GetValue() != rhs;
	}

	[Serializable]
	public struct EncryptVector4
	{
		public int x;

		public int y;

		public int z;

		public int w;
	}

	[StructLayout(LayoutKind.Explicit)]
	private struct FloatIntUnion
	{
		[FieldOffset(0)]
		public float f;

		[FieldOffset(0)]
		public int i;
	}
}
