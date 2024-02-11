using System;
using System.Security.Cryptography;
using System.Text;

public static class CryptoManager
{
    public static string cryptoKey = "^8b1v]x4";

    public static byte[] cryptoKeyBytes = new byte[]
    {
        1,
        61,
        42,
        250,
        125
    };

    public static Random random = new Random();

    public static bool fakeValue = false;

    private static int seed;

    private static int randValue;

    private static int _staticValue = 0;

    private static Action DetectorAction;

    static CryptoManager()
	{
		CryptoManager.seed = DateTime.Now.Millisecond;
		CryptoManager.randValue = CryptoManager.seed;
	}

	public static int staticValue
	{
		get
		{
			if (CryptoManager._staticValue == 0)
			{
				CryptoManager._staticValue = CryptoManager.Next(1, 10);
			}
			return CryptoManager._staticValue;
		}
	}

	public static void StartDetection(Action callback)
	{
		CryptoManager.DetectorAction = callback;
	}

	public static void CheatingDetected()
	{
		if (CryptoManager.DetectorAction != null)
		{
			CryptoManager.DetectorAction();
		}
	}

	public static string MD5(int value)
	{
		return CryptoManager.MD5(BitConverter.GetBytes(value));
	}

	public static string MD5(string value)
	{
		UTF8Encoding utf8Encoding = new UTF8Encoding();
		return CryptoManager.MD5(utf8Encoding.GetBytes(value));
	}

	public static string MD5(byte[] bytes)
	{
		MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] array = md5CryptoServiceProvider.ComputeHash(bytes);
		string text = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			text += Convert.ToString(array[i], 16).PadLeft(2, '0');
		}
		return text.PadLeft(32, '0');
	}

	public static int Next()
	{
		CryptoManager.randValue ^= CryptoManager.randValue << 21;
		CryptoManager.randValue ^= CryptoManager.randValue >> 3;
		CryptoManager.randValue ^= CryptoManager.randValue << 4;
		return CryptoManager.randValue;
	}

	public static int Next(int min, int max)
	{
		CryptoManager.randValue ^= CryptoManager.randValue << 21;
		CryptoManager.randValue ^= CryptoManager.randValue >> 3;
		CryptoManager.randValue ^= CryptoManager.randValue << 4;
		return (int)(((float)CryptoManager.randValue / 2.14748365E+09f + 1f) / 2f * (float)(max - min) + (float)min);
	}
}
