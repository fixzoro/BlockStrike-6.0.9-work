using System;
using UnityEngine;

[Serializable]
public struct CryptoBool : IEquatable<CryptoBool>
{
    [SerializeField]
    private byte cryptoKey;

    [SerializeField]
    private byte hiddenValue;

    [SerializeField]
    private bool fakeValue;

    [SerializeField]
    private bool fakeValueChanged;

    [SerializeField]
    private bool inited;

    private CryptoBool(bool value)
	{
		this.cryptoKey = (byte)CryptoManager.random.Next(1, 200);
		this.hiddenValue = (byte)(((!value) ? 32 : 18) ^ this.cryptoKey);
		this.fakeValue = (CryptoManager.fakeValue && value);
		this.fakeValueChanged = false;
		this.inited = true;
        #if UNITY_EDITOR
        cryptoKeyEditor = cryptoKey;
        #endif
    }

    public void UpdateValue()
	{
		this.SetValue(this.GetValue());
	}

    #if UNITY_EDITOR
    public static byte cryptoKeyEditor;
    #endif

    public static int Encrypt(bool value)
    {
        return CryptoBool.Encrypt(value, 0);
    }

    public static int Encrypt(bool value, byte key)
    {
        if (key == 0)
        {
            #if UNITY_EDITOR
            key = CryptoBool.cryptoKeyEditor;
            #endif
        }
        int num = (!value) ? 32 : 18;
        return num ^ (int)key;
    }

    public static bool Decrypt(int value)
    {
        return CryptoBool.Decrypt(value, 0);
    }

    public static bool Decrypt(int value, byte key)
    {
        if (key == 0)
        {
            #if UNITY_EDITOR
            key = CryptoBool.cryptoKeyEditor;
            #endif
        }
        value ^= (int)key;
        return value != 32;
    }

    public void SetValue(bool value)
	{
		this.cryptoKey = (byte)CryptoManager.random.Next(1, 200);
		this.hiddenValue = (byte)(((!value) ? 32 : 18) ^ this.cryptoKey);
		this.fakeValue = (CryptoManager.fakeValue && value);
		this.fakeValueChanged = true;
	}

	private bool GetValue()
	{
		if (!this.inited)
		{
			this.cryptoKey = (byte)CryptoManager.random.Next(1, 200);
			this.hiddenValue = (byte)(32 ^ this.cryptoKey);
			this.fakeValue = false;
			this.fakeValueChanged = false;
			this.inited = true;
		}
		bool flag = (this.hiddenValue ^ this.cryptoKey) != 32;
		if (CryptoManager.fakeValue && this.fakeValueChanged && this.fakeValue != flag)
		{
			CryptoManager.CheatingDetected();
		}
		return flag;
	}

	public override bool Equals(object obj)
	{
		return obj is CryptoBool && this.Equals((CryptoBool)obj);
	}

	public bool Equals(CryptoBool obj)
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

	public static implicit operator CryptoBool(bool value)
	{
		CryptoBool result = new CryptoBool(value);
		return result;
	}

	public static implicit operator bool(CryptoBool value)
	{
		return value.GetValue();
	}
}
