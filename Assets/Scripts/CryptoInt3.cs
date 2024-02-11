using System;
using UnityEngine;

[Serializable]
public struct CryptoInt3 : IFormattable, IEquatable<CryptoInt3>
{
    [SerializeField]
    private int cryptoKey;

    [SerializeField]
    private int hiddenValue;

    [SerializeField]
    private int fakeValue;

    [SerializeField]
    private bool inited;

    private CryptoInt3(int value)
	{
		do
		{
			this.cryptoKey = CryptoManager.Next();
		}
		while (this.cryptoKey == 0);
		this.hiddenValue = value + this.cryptoKey;
		this.fakeValue = value - this.cryptoKey;
		this.inited = true;
	}

	public void SetValue(int value)
	{
		do
		{
			this.cryptoKey = CryptoManager.Next();
		}
		while (this.cryptoKey == 0);
		this.hiddenValue = value + this.cryptoKey;
		this.fakeValue = value - this.cryptoKey;
	}

	public void UpdateValue()
	{
		this.SetValue(this.GetValue());
	}

	private int GetValue()
	{
		if (!this.inited)
		{
			do
			{
				this.cryptoKey = CryptoManager.Next();
			}
			while (this.cryptoKey == 0);
			this.hiddenValue = this.cryptoKey;
			this.fakeValue = -this.cryptoKey;
			this.inited = true;
		}
		if (this.hiddenValue - this.cryptoKey != this.fakeValue + this.cryptoKey)
		{
			CryptoManager.CheatingDetected();
			return 0;
		}
		return this.hiddenValue - this.cryptoKey;
	}

	public override bool Equals(object obj)
	{
		return obj is CryptoInt3 && this.Equals((CryptoInt3)obj);
	}

	public bool Equals(CryptoInt3 obj)
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

	public static implicit operator CryptoInt3(int value)
	{
		CryptoInt3 result = new CryptoInt3(value);
		return result;
	}

	public static implicit operator CryptoInt3(CryptoInt value)
	{
		CryptoInt3 result = new CryptoInt3(value);
		return result;
	}

	public static implicit operator int(CryptoInt3 value)
	{
		return value.GetValue();
	}

	public static CryptoInt3 operator ++(CryptoInt3 value)
	{
		value.SetValue(value.GetValue() + 1);
		return value;
	}

	public static CryptoInt3 operator --(CryptoInt3 value)
	{
		value.SetValue(value.GetValue() - 1);
		return value;
	}
}
