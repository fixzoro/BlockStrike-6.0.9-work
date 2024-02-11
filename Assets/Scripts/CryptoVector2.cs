using System;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public struct CryptoVector2 : IEquatable<CryptoVector2>
{
    [SerializeField]
    private int cryptoKey;

    [SerializeField]
    private CryptoVector2.EncryptVector2 hiddenValue;

    [SerializeField]
    private Vector2 fakeValue;

    [SerializeField]
    private bool inited;

    private CryptoVector2(Vector2 value)
	{
		this.cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
		CryptoVector2.FloatIntUnion floatIntUnion = default(CryptoVector2.FloatIntUnion);
		floatIntUnion.f = value.x;
		CryptoVector2.EncryptVector2 encryptVector;
		encryptVector.x = (floatIntUnion.i ^ this.cryptoKey);
		CryptoVector2.FloatIntUnion floatIntUnion2 = default(CryptoVector2.FloatIntUnion);
		floatIntUnion2.f = value.y;
		encryptVector.y = (floatIntUnion2.i ^ this.cryptoKey);
		this.hiddenValue = encryptVector;
		this.fakeValue = ((!CryptoManager.fakeValue) ? Vector2.zero : value);
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

	public void UpdateValue()
	{
		this.SetValue(this.GetValue());
	}

	public void SetValue(Vector2 value)
	{
		this.cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
		this.hiddenValue = this.Encrypt(value);
		if (CryptoManager.fakeValue)
		{
			this.fakeValue = value;
		}
	}

	private Vector2 GetValue()
	{
		if (!this.inited)
		{
			this.cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
			this.hiddenValue = this.Encrypt(Vector2.zero);
			this.fakeValue = Vector2.zero;
			this.inited = true;
		}
		Vector2 vector = this.Decrypt(this.hiddenValue);
		if (CryptoManager.fakeValue && this.fakeValue != vector)
		{
			CryptoManager.CheatingDetected();
		}
		return vector;
	}

	private CryptoVector2.EncryptVector2 Encrypt(Vector2 value)
	{
		CryptoVector2.EncryptVector2 result;
		result.x = this.EncryptFloat(value.x);
		result.y = this.EncryptFloat(value.y);
		return result;
	}

	private int EncryptFloat(float value)
	{
		CryptoVector2.FloatIntUnion floatIntUnion = default(CryptoVector2.FloatIntUnion);
		floatIntUnion.f = value;
		floatIntUnion.i ^= this.cryptoKey;
		return floatIntUnion.i;
	}

	private Vector2 Decrypt(CryptoVector2.EncryptVector2 value)
	{
		Vector2 result;
		result.x = this.DecryptFloat(value.x);
		result.y = this.DecryptFloat(value.y);
		return result;
	}

	private float DecryptFloat(int value)
	{
		CryptoVector2.FloatIntUnion floatIntUnion = default(CryptoVector2.FloatIntUnion);
		floatIntUnion.i = (value ^ this.cryptoKey);
		return floatIntUnion.f;
	}

	public override bool Equals(object obj)
	{
		return obj is CryptoVector2 && this.Equals((CryptoVector2)obj);
	}

	public bool Equals(CryptoVector2 obj)
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

	public static implicit operator CryptoVector2(Vector2 value)
	{
		return new CryptoVector2(value);
	}

	public static implicit operator Vector2(CryptoVector2 value)
	{
		return value.GetValue();
	}

	public static CryptoVector2 operator +(CryptoVector2 a, CryptoVector2 b)
	{
		return a.GetValue() + b.GetValue();
	}

	public static CryptoVector2 operator +(Vector2 a, CryptoVector2 b)
	{
		return a + b.GetValue();
	}

	public static CryptoVector2 operator +(CryptoVector2 a, Vector2 b)
	{
		return a.GetValue() + b;
	}

	public static CryptoVector2 operator -(CryptoVector2 a, CryptoVector2 b)
	{
		return a.GetValue() - b.GetValue();
	}

	public static CryptoVector2 operator -(Vector2 a, CryptoVector2 b)
	{
		return a - b.GetValue();
	}

	public static CryptoVector2 operator -(CryptoVector2 a, Vector2 b)
	{
		return a.GetValue() - b;
	}

	public static CryptoVector2 operator -(CryptoVector2 a)
	{
		return -a.GetValue();
	}

	public static CryptoVector2 operator *(CryptoVector2 a, float d)
	{
		return a.GetValue() * d;
	}

	public static CryptoVector2 operator *(float d, CryptoVector2 a)
	{
		return d * a.GetValue();
	}

	public static CryptoVector2 operator /(CryptoVector2 a, float d)
	{
		return a.GetValue() / d;
	}

	public static bool operator ==(CryptoVector2 lhs, CryptoVector2 rhs)
	{
		return lhs.GetValue() == rhs.GetValue();
	}

	public static bool operator ==(Vector2 lhs, CryptoVector2 rhs)
	{
		return lhs == rhs.GetValue();
	}

	public static bool operator ==(CryptoVector2 lhs, Vector2 rhs)
	{
		return lhs.GetValue() == rhs;
	}

	public static bool operator !=(CryptoVector2 lhs, CryptoVector2 rhs)
	{
		return lhs.GetValue() != rhs.GetValue();
	}

	public static bool operator !=(Vector2 lhs, CryptoVector2 rhs)
	{
		return lhs != rhs.GetValue();
	}

	public static bool operator !=(CryptoVector2 lhs, Vector2 rhs)
	{
		return lhs.GetValue() != rhs;
	}

	[Serializable]
	public struct EncryptVector2
	{
		public int x;

		public int y;
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
