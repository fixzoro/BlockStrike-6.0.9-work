using System;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public struct CryptoVector3 : IEquatable<CryptoVector3>
{
    [SerializeField]
    private int cryptoKey;

    [SerializeField]
    private CryptoVector3.EncryptVector3 hiddenValue;

    [SerializeField]
    private Vector3 fakeValue;

    [SerializeField]
    private bool inited;

    private CryptoVector3(Vector3 value)
	{
		this.cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
		CryptoVector3.FloatIntUnion floatIntUnion = default(CryptoVector3.FloatIntUnion);
		floatIntUnion.f = value.x;
		CryptoVector3.EncryptVector3 encryptVector;
		encryptVector.x = (floatIntUnion.i ^ this.cryptoKey);
		CryptoVector3.FloatIntUnion floatIntUnion2 = default(CryptoVector3.FloatIntUnion);
		floatIntUnion2.f = value.y;
		encryptVector.y = (floatIntUnion2.i ^ this.cryptoKey);
		CryptoVector3.FloatIntUnion floatIntUnion3 = default(CryptoVector3.FloatIntUnion);
		floatIntUnion3.f = value.z;
		encryptVector.z = (floatIntUnion3.i ^ this.cryptoKey);
		this.hiddenValue = encryptVector;
		this.fakeValue = ((!CryptoManager.fakeValue) ? Vector3.zero : value);
		this.inited = true;
	}

	public float x
	{
		get
		{
			float num = this.DecryptFloat(this.hiddenValue.x);
			if (CryptoManager.fakeValue && this.fakeValue.x != num)
			{
				CryptoManager.CheatingDetected();
			}
			return num;
		}
		set
		{
			this.hiddenValue.x = this.EncryptFloat(value);
			if (CryptoManager.fakeValue)
			{
				this.fakeValue.x = value;
			}
		}
	}

	public float y
	{
		get
		{
			float num = this.DecryptFloat(this.hiddenValue.y);
			if (CryptoManager.fakeValue && this.fakeValue.y != num)
			{
				CryptoManager.CheatingDetected();
			}
			return num;
		}
		set
		{
			this.hiddenValue.y = this.EncryptFloat(value);
			if (CryptoManager.fakeValue)
			{
				this.fakeValue.y = value;
			}
		}
	}

    public float z
	{
		get
		{
			float num = this.DecryptFloat(this.hiddenValue.z);
			if (CryptoManager.fakeValue && this.fakeValue.z != num)
			{
				CryptoManager.CheatingDetected();
			}
			return num;
		}
		set
		{
			this.hiddenValue.z = this.EncryptFloat(value);
			if (CryptoManager.fakeValue)
			{
				this.fakeValue.z = value;
			}
		}
	}

	public void UpdateValue()
	{
		this.SetValue(this.GetValue());
	}

	public void SetValue(Vector3 value)
	{
		this.cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
		this.hiddenValue = this.Encrypt(value);
		if (CryptoManager.fakeValue)
		{
			this.fakeValue = value;
		}
	}

	private Vector3 GetValue()
	{
		if (!this.inited)
		{
			this.cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
			this.hiddenValue = this.Encrypt(Vector3.zero);
			this.fakeValue = Vector3.zero;
			this.inited = true;
		}
		Vector3 vector = this.Decrypt(this.hiddenValue);
		if (CryptoManager.fakeValue && this.fakeValue != vector)
		{
			CryptoManager.CheatingDetected();
		}
		return vector;
	}

	private CryptoVector3.EncryptVector3 Encrypt(Vector3 value)
	{
		CryptoVector3.EncryptVector3 result;
		result.x = this.EncryptFloat(value.x);
		result.y = this.EncryptFloat(value.y);
		result.z = this.EncryptFloat(value.z);
		return result;
	}

	private int EncryptFloat(float value)
	{
		CryptoVector3.FloatIntUnion floatIntUnion = default(CryptoVector3.FloatIntUnion);
		floatIntUnion.f = value;
		floatIntUnion.i ^= this.cryptoKey;
		return floatIntUnion.i;
	}

	private Vector3 Decrypt(CryptoVector3.EncryptVector3 value)
	{
		Vector3 result;
		result.x = this.DecryptFloat(value.x);
		result.y = this.DecryptFloat(value.y);
		result.z = this.DecryptFloat(value.z);
		return result;
	}

	private float DecryptFloat(int value)
	{
		CryptoVector3.FloatIntUnion floatIntUnion = default(CryptoVector3.FloatIntUnion);
		floatIntUnion.i = (value ^ this.cryptoKey);
		return floatIntUnion.f;
	}

	public override bool Equals(object obj)
	{
		return obj is CryptoVector3 && this.Equals((CryptoVector3)obj);
	}

	public bool Equals(CryptoVector3 obj)
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

	public static implicit operator CryptoVector3(Vector3 value)
	{
		return new CryptoVector3(value);
	}

	public static implicit operator Vector3(CryptoVector3 value)
	{
		return value.GetValue();
	}

	public static CryptoVector3 operator +(CryptoVector3 a, CryptoVector3 b)
	{
		return a.GetValue() + b.GetValue();
	}

	public static CryptoVector3 operator +(Vector3 a, CryptoVector3 b)
	{
		return a + b.GetValue();
	}

	public static CryptoVector3 operator +(CryptoVector3 a, Vector3 b)
	{
		return a.GetValue() + b;
	}

	public static CryptoVector3 operator -(CryptoVector3 a, CryptoVector3 b)
	{
		return a.GetValue() - b.GetValue();
	}

	public static CryptoVector3 operator -(Vector3 a, CryptoVector3 b)
	{
		return a - b.GetValue();
	}

	public static CryptoVector3 operator -(CryptoVector3 a, Vector3 b)
	{
		return a.GetValue() - b;
	}

	public static CryptoVector3 operator -(CryptoVector3 a)
	{
		return -a.GetValue();
	}

	public static CryptoVector3 operator *(CryptoVector3 a, float d)
	{
		return a.GetValue() * d;
	}

	public static CryptoVector3 operator *(float d, CryptoVector3 a)
	{
		return d * a.GetValue();
	}

	public static CryptoVector3 operator /(CryptoVector3 a, float d)
	{
		return a.GetValue() / d;
	}

	public static bool operator ==(CryptoVector3 lhs, CryptoVector3 rhs)
	{
		return lhs.GetValue() == rhs.GetValue();
	}

	public static bool operator ==(Vector3 lhs, CryptoVector3 rhs)
	{
		return lhs == rhs.GetValue();
	}

	public static bool operator ==(CryptoVector3 lhs, Vector3 rhs)
	{
		return lhs.GetValue() == rhs;
	}

	public static bool operator !=(CryptoVector3 lhs, CryptoVector3 rhs)
	{
		return lhs.GetValue() != rhs.GetValue();
	}

	public static bool operator !=(Vector3 lhs, CryptoVector3 rhs)
	{
		return lhs != rhs.GetValue();
	}

	public static bool operator !=(CryptoVector3 lhs, Vector3 rhs)
	{
		return lhs.GetValue() != rhs;
	}

	[Serializable]
	public struct EncryptVector3
	{
		public int x;

		public int y;

		public int z;
	}

	[Serializable]
	private struct Byte4
	{
		public byte b1;

		public byte b2;

		public byte b3;

		public byte b4;
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
