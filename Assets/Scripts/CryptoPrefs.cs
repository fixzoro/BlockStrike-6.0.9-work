using System;
using System.Linq;
using System.Text;
using UnityEngine;

public static class CryptoPrefs
{
    private static byte[] defaultKey = new byte[]
    {
        97,
        50,
        51,
        106,
        54,
        103,
        57,
        52,
        104
    };

    private static byte[] S;

    private static int x = 0;

    private static int y = 0;

    static CryptoPrefs()
	{
		string text = SystemInfo.deviceUniqueIdentifier;
		if (string.IsNullOrEmpty(text))
		{
			text = "3h56gk58n3g5f0d2";
		}
		else
		{
			if (text.Length < 16)
			{
				text += "3h56gk58n3g5f0d2";
			}
			text = text.Remove(16);
		}
		CryptoPrefs.defaultKey = Encoding.UTF8.GetBytes(text);
		CryptoPrefs.SetCryptoKey();
	}

	public static bool HasKey(string key)
	{
		return PlayerPrefs.HasKey(Convert.ToBase64String(CryptoPrefs.EncryptDencrypt(Encoding.UTF8.GetBytes(key))));
	}

	public static void DeleteKey(string key)
	{
		PlayerPrefs.DeleteKey(Convert.ToBase64String(CryptoPrefs.EncryptDencrypt(Encoding.UTF8.GetBytes(key))));
	}

	public static void SetInt(string key, int value)
	{
		key = Convert.ToBase64String(CryptoPrefs.EncryptDencrypt(Encoding.UTF8.GetBytes(key)));
		string value2 = Convert.ToBase64String(CryptoPrefs.EncryptDencrypt(Encoding.UTF8.GetBytes(key + "|" + value)));
		PlayerPrefs.SetString(key, value2);
	}

	public static int GetInt(string key)
	{
		return CryptoPrefs.GetInt(key, 0);
	}

	public static int GetInt(string key, int defaultValue)
	{
		int result;
		try
		{
			key = Convert.ToBase64String(CryptoPrefs.EncryptDencrypt(Encoding.UTF8.GetBytes(key)));
			string @string = PlayerPrefs.GetString(key, defaultValue.ToString());
			if (@string == defaultValue.ToString())
			{
				result = defaultValue;
			}
			else
			{
				@string = Encoding.UTF8.GetString(CryptoPrefs.EncryptDencrypt(Convert.FromBase64String(@string)));
				result = int.Parse(@string.Split(new char[]
				{
					"|"[0]
				})[1]);
			}
		}
		catch
		{
			result = defaultValue;
		}
		return result;
	}

	public static void SetFloat(string key, float value)
	{
		key = Convert.ToBase64String(CryptoPrefs.EncryptDencrypt(Encoding.UTF8.GetBytes(key)));
		string value2 = Convert.ToBase64String(CryptoPrefs.EncryptDencrypt(Encoding.UTF8.GetBytes(key + "|" + value)));
		PlayerPrefs.SetString(key, value2);
	}

	public static float GetFloat(string key)
	{
		return CryptoPrefs.GetFloat(key, 0f);
	}

	public static float GetFloat(string key, float defaultValue)
	{
		float result;
		try
		{
			key = Convert.ToBase64String(CryptoPrefs.EncryptDencrypt(Encoding.UTF8.GetBytes(key)));
			string @string = PlayerPrefs.GetString(key, defaultValue.ToString());
			if (@string == defaultValue.ToString())
			{
				result = defaultValue;
			}
			else
			{
				@string = Encoding.UTF8.GetString(CryptoPrefs.EncryptDencrypt(Convert.FromBase64String(@string)));
				result = float.Parse(@string.Split(new char[]
				{
					"|"[0]
				})[1]);
			}
		}
		catch
		{
			result = defaultValue;
		}
		return result;
	}

	public static void SetBool(string key, bool value)
	{
		int num = UnityEngine.Random.Range(0, 1000);
		if (value && num % 2 == 0)
		{
			num++;
		}
		else if (!value && num % 2 == 1)
		{
			num++;
		}
		CryptoPrefs.SetInt(key, num);
	}

	public static bool GetBool(string key)
	{
		return CryptoPrefs.GetBool(key, false);
	}

	public static bool GetBool(string key, bool defaultValue)
	{
		int @int = CryptoPrefs.GetInt(key, (!defaultValue) ? 0 : 1);
		return @int % 2 == 1;
	}

	public static void SetString(string key, string value)
	{
		key = Convert.ToBase64String(CryptoPrefs.EncryptDencrypt(Encoding.UTF8.GetBytes(key)));
		string value2 = Convert.ToBase64String(CryptoPrefs.EncryptDencrypt(Encoding.UTF8.GetBytes(key + "|" + value)));
		PlayerPrefs.SetString(key, value2);
	}

	public static string GetString(string key)
	{
		return CryptoPrefs.GetString(key, string.Empty);
	}

	public static string GetString(string key, string defaultValue)
	{
		string result;
		try
		{
			key = Convert.ToBase64String(CryptoPrefs.EncryptDencrypt(Encoding.UTF8.GetBytes(key)));
			string @string = PlayerPrefs.GetString(key, defaultValue.ToString());
			if (@string == defaultValue.ToString())
			{
				result = defaultValue;
			}
			else
			{
				@string = Encoding.UTF8.GetString(CryptoPrefs.EncryptDencrypt(Convert.FromBase64String(@string)));
				result = @string.Split(new char[]
				{
					"|"[0]
				})[1];
			}
		}
		catch
		{
			result = defaultValue;
		}
		return result;
	}

	private static void SetCryptoKey()
	{
		CryptoPrefs.S = new byte[256];
		for (int i = 0; i < 256; i++)
		{
			CryptoPrefs.S[i] = (byte)i;
		}
		int num = 0;
		for (int j = 0; j < 256; j++)
		{
			num = (num + (int)CryptoPrefs.S[j] + (int)CryptoPrefs.defaultKey[j % CryptoPrefs.defaultKey.Length]) % 256;
			CryptoPrefs.Swap(CryptoPrefs.S, j, num);
		}
	}

	public static byte[] EncryptDencrypt(byte[] data)
	{
		CryptoPrefs.x = 0;
		CryptoPrefs.y = 0;
		CryptoPrefs.SetCryptoKey();
		data = data.Take(data.Length).ToArray<byte>();
		byte[] array = new byte[data.Length];
		for (int i = 0; i < data.Length; i++)
		{
			array[i] = (byte)(data[i] ^ CryptoPrefs.Kword());
		}
		return array;
	}

	private static void Swap(byte[] array, int ind1, int ind2)
	{
		byte b = array[ind1];
		array[ind1] = array[ind2];
		array[ind2] = b;
	}

	private static byte Kword()
	{
		CryptoPrefs.x = (CryptoPrefs.x + 1) % 256;
		CryptoPrefs.y = (CryptoPrefs.y + (int)CryptoPrefs.S[CryptoPrefs.x]) % 256;
		CryptoPrefs.Swap(CryptoPrefs.S, CryptoPrefs.x, CryptoPrefs.y);
		return CryptoPrefs.S[(int)(CryptoPrefs.S[CryptoPrefs.x] + CryptoPrefs.S[CryptoPrefs.y]) % 256];
	}
}
