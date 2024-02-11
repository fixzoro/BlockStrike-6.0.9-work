using System;
using UnityEngine;

[Serializable]
public struct CryptoString : IEquatable<CryptoString>
{
    [SerializeField]
    private byte[] cryptoKey;

    [SerializeField]
    private byte[] hiddenValue;

    [SerializeField]
    private string fakeValue;

    [SerializeField]
    private bool inited;

    private CryptoString(string value)
	{
		this.cryptoKey = new byte[CryptoManager.random.Next(10, 30)];
		CryptoManager.random.NextBytes(this.cryptoKey);
		this.hiddenValue = CryptoString.GetBytes(value);
		for (int i = 0; i < this.hiddenValue.Length; i++)
		{
			this.hiddenValue[i] = (byte)(this.hiddenValue[i] ^ this.cryptoKey[i % this.cryptoKey.Length]);
		}
		this.fakeValue = ((!CryptoManager.fakeValue) ? string.Empty : value);
		this.inited = true;
        #if UNITY_EDITOR
        cryptoKeyEditor = cryptoKey;
        #endif
    }

    #if UNITY_EDITOR
    public static byte[] cryptoKeyEditor;
    #endif

    public void SetValue(string value)
	{
		this.cryptoKey = new byte[CryptoManager.random.Next(10, 30)];
		CryptoManager.random.NextBytes(this.cryptoKey);
		this.hiddenValue = CryptoString.GetBytes(value);
		for (int i = 0; i < this.hiddenValue.Length; i++)
		{
			this.hiddenValue[i] = (byte)(this.hiddenValue[i] ^ this.cryptoKey[i % this.cryptoKey.Length]);
		}
		this.fakeValue = ((!CryptoManager.fakeValue) ? string.Empty : value);
	}

	private string GetValue()
	{
		if (!this.inited)
		{
			this.cryptoKey = new byte[CryptoManager.random.Next(10, 30)];
			CryptoManager.random.NextBytes(this.cryptoKey);
			this.hiddenValue = new byte[0];
			this.fakeValue = string.Empty;
			this.inited = true;
		}
		byte[] array = new byte[this.hiddenValue.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (byte)(this.hiddenValue[i] ^ this.cryptoKey[i % this.cryptoKey.Length]);
		}
		string @string = CryptoString.GetString(array);
		if (CryptoManager.fakeValue && !string.IsNullOrEmpty(this.fakeValue) && this.fakeValue != @string)
		{
			CryptoManager.CheatingDetected();
		}
		return @string;
	}

	public override bool Equals(object obj)
	{
		return obj is CryptoString && this.Equals((CryptoString)obj);
	}

	public bool Equals(CryptoString obj)
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

	public int Length
	{
		get
		{
			return this.GetValue().Length;
		}
	}

	public bool Contains(string value)
	{
		return this.GetValue().Contains(value);
	}

	public static byte[] GetBytes(string str)
	{
		byte[] array = new byte[str.Length * 2];
		Buffer.BlockCopy(str.ToCharArray(), 0, array, 0, array.Length);
		return array;
	}

	public static string GetString(byte[] bytes)
	{
		char[] array = new char[bytes.Length / 2];
		Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
		return new string(array);
	}

	public static implicit operator CryptoString(string value)
	{
		CryptoString result = new CryptoString(value);
		return result;
	}

	public static implicit operator string(CryptoString value)
	{
		return value.GetValue();
	}
}
