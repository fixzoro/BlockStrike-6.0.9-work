using System;
using UnityEngine;

[Serializable]
public struct CryptoInt : IFormattable, IEquatable<CryptoInt>
{
    [SerializeField]
    private int cryptoKey;

    [SerializeField]
    private int hiddenValue;

    [SerializeField]
    private int fakeValue;

    [SerializeField]
    private bool inited;

    private CryptoInt(int value)
	{
		do
		{
			this.cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
		}
		while (this.cryptoKey == 0);
		this.hiddenValue = (value ^ this.cryptoKey);
		this.fakeValue = ((!CryptoManager.fakeValue) ? 0 : value);
		this.inited = true;
        #if UNITY_EDITOR
        cryptoKeyEditor = cryptoKey;
        #endif
    }

    #if UNITY_EDITOR
    public static int cryptoKeyEditor;
    #endif

    public static int Encrypt(int value)
    {
        return CryptoInt.Encrypt(value, 0);
    }

    public static int Encrypt(int value, int key)
    {
        #if UNITY_EDITOR
        if (key == 0)
        {
            return value ^ CryptoInt.cryptoKeyEditor;
        }
        #endif
        return value ^ key;
    }

    public static int Decrypt(int value)
    {
        return CryptoInt.Decrypt(value, 0);
    }

    public static int Decrypt(int value, int key)
    {
        #if UNITY_EDITOR
        if (key == 0)
        {
            return value ^ CryptoInt.cryptoKeyEditor;
        }
        #endif
        return value ^ key;
    }

    public void SetValue(int value)
	{
		do
		{
			this.cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
		}
		while (this.cryptoKey == 0);
		this.fakeValue = ((!CryptoManager.fakeValue) ? 0 : value);
		this.hiddenValue = (value ^ this.cryptoKey);
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
				this.cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
			}
			while (this.cryptoKey == 0);
			this.hiddenValue = this.cryptoKey;
			this.fakeValue = 0;
			this.inited = true;
		}
		int num = this.hiddenValue ^ this.cryptoKey;
		if ((CryptoManager.fakeValue && this.fakeValue != num) || this.cryptoKey == 0)
		{
			CryptoManager.CheatingDetected();
		}
		return num;
	}

	public override bool Equals(object obj)
	{
		return obj is CryptoInt && this.Equals((CryptoInt)obj);
	}

	public bool Equals(CryptoInt obj)
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

	public static implicit operator CryptoInt(int value)
	{
		CryptoInt result = new CryptoInt(value);
		return result;
	}

	public static implicit operator int(CryptoInt value)
	{
		return value.GetValue();
	}

	public static CryptoInt operator ++(CryptoInt value)
	{
		value.SetValue(value.GetValue() + 1);
		return value;
	}

	public static CryptoInt operator --(CryptoInt value)
	{
		value.SetValue(value.GetValue() - 1);
		return value;
	}
}
