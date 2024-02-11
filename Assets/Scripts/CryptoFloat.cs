using System;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public struct CryptoFloat : IFormattable, IEquatable<CryptoFloat>
{
    [SerializeField]
    public int cryptoKey;

    [SerializeField]
    public CryptoFloat.Byte4 hiddenValue;

    [SerializeField]
    private float fakeValue;

    [SerializeField]
    private bool inited;

    private CryptoFloat(float value)
	{
		this.cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
		CryptoFloat.FloatIntBytesUnion floatIntBytesUnion = default(CryptoFloat.FloatIntBytesUnion);
		floatIntBytesUnion.f = value;
		floatIntBytesUnion.i ^= this.cryptoKey;
		this.hiddenValue = floatIntBytesUnion.b4;
		this.fakeValue = ((!CryptoManager.fakeValue) ? 0f : value);
		this.inited = true;
	}

	public void UpdateValue()
	{
		this.SetValue(this.GetValue());
	}

	public void SetValue(float value)
	{
		this.cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
		this.hiddenValue = this.Encrypt(value);
		this.fakeValue = ((!CryptoManager.fakeValue) ? 0f : value);
	}

	private float GetValue()
	{
		if (!this.inited)
		{
			this.cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
			this.hiddenValue = this.Encrypt(0f);
			this.fakeValue = 0f;
			this.inited = true;
		}
		float num = this.Decrypt(this.hiddenValue);
		if (CryptoManager.fakeValue && this.fakeValue != num)
		{
			CryptoManager.CheatingDetected();
		}
		return num;
	}

	private CryptoFloat.Byte4 Encrypt(float value)
	{
		CryptoFloat.FloatIntBytesUnion floatIntBytesUnion = default(CryptoFloat.FloatIntBytesUnion);
		floatIntBytesUnion.f = value;
		floatIntBytesUnion.i ^= this.cryptoKey;
		return floatIntBytesUnion.b4;
	}

	private float Decrypt(CryptoFloat.Byte4 bytes)
	{
		CryptoFloat.FloatIntBytesUnion floatIntBytesUnion = default(CryptoFloat.FloatIntBytesUnion);
		floatIntBytesUnion.b4 = this.hiddenValue;
		floatIntBytesUnion.i ^= this.cryptoKey;
		return floatIntBytesUnion.f;
	}

	public override bool Equals(object obj)
	{
		return obj is CryptoFloat && this.Equals((CryptoFloat)obj);
	}

	public bool Equals(CryptoFloat obj)
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

	public string ToString(string format)
	{
		return this.GetValue().ToString(format);
	}

	public string ToString(IFormatProvider provider)
	{
		return this.GetValue().ToString(provider);
	}

	public string ToString(string format, IFormatProvider provider)
	{
		return this.GetValue().ToString(format, provider);
	}

	public static implicit operator CryptoFloat(float value)
	{
		CryptoFloat result = new CryptoFloat(value);
		return result;
	}

	public static implicit operator float(CryptoFloat value)
	{
		return value.GetValue();
	}

	public static CryptoFloat operator ++(CryptoFloat value)
	{
		value.SetValue(value.GetValue() + 1f);
		return value;
	}

	public static CryptoFloat operator --(CryptoFloat value)
	{
		value.SetValue(value.GetValue() - 1f);
		return value;
	}

	[Serializable]
	public struct Byte4
	{
		public byte b1;

		public byte b2;

		public byte b3;

		public byte b4;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct FloatIntBytesUnion
	{
		[FieldOffset(0)]
		public float f;

		[FieldOffset(0)]
		public int i;

		[FieldOffset(0)]
		public CryptoFloat.Byte4 b4;
	}
}
