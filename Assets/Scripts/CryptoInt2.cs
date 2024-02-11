using System;
using UnityEngine;

[Serializable]
public struct CryptoInt2 : IFormattable, IEquatable<CryptoInt2>
{
    [SerializeField]
    private int cryptoKey;

    [SerializeField]
    private string hiddenValue;

    [SerializeField]
    private int fakeValue;

    [SerializeField]
    private bool inited;

    private CryptoInt2(int value)
	{
		do
		{
			this.cryptoKey = CryptoManager.random.Next(int.MinValue, int.MaxValue);
		}
		while (this.cryptoKey == 0);
		this.hiddenValue = CryptoManager.MD5(value ^ this.cryptoKey);
		this.fakeValue = value;
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
        return CryptoInt2.Encrypt(value, 0);
    }

    public static int Encrypt(int value, int key)
    {
        #if UNITY_EDITOR
        if (key == 0)
        {
            return value ^ CryptoInt2.cryptoKeyEditor;
        }
        #endif
        return value ^ key;
    }

    public static int Decrypt(int value)
    {
        return CryptoInt2.Decrypt(value, 0);
    }

    public static int Decrypt(int value, int key)
    {
        #if UNITY_EDITOR
        if (key == 0)
        {
            return value ^ CryptoInt2.cryptoKeyEditor;
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
		this.hiddenValue = CryptoManager.MD5(value ^ this.cryptoKey);
		this.fakeValue = value;
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
			this.hiddenValue = CryptoManager.MD5(this.cryptoKey);
			this.fakeValue = 0;
			this.inited = true;
		}
		if (CryptoManager.MD5(this.fakeValue ^ this.cryptoKey) != this.hiddenValue || this.cryptoKey == 0)
		{
			CryptoManager.CheatingDetected();
			return 0;
		}
		return this.fakeValue;
	}

	public override bool Equals(object obj)
	{
		return obj is CryptoInt2 && this.Equals((CryptoInt2)obj);
	}

	public bool Equals(CryptoInt2 obj)
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

	public static implicit operator CryptoInt2(int value)
	{
		CryptoInt2 result = new CryptoInt2(value);
		return result;
	}

	public static implicit operator int(CryptoInt2 value)
	{
		return value.GetValue();
	}

	public static CryptoInt2 operator ++(CryptoInt2 value)
	{
		value.SetValue(value.GetValue() + 1);
		return value;
	}

	public static CryptoInt2 operator --(CryptoInt2 value)
	{
		value.SetValue(value.GetValue() - 1);
		return value;
	}
}
