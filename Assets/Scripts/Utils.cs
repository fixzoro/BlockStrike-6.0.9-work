using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class Utils
{
    public static string test = string.Empty;

    public static GameObject AddChild(GameObject inst, GameObject parent, [Optional] Vector3 position, [Optional] Quaternion rotation)
	{
		return Utils.AddChild(inst, parent.transform, position, rotation);
	}

	public static GameObject AddChild(GameObject inst, Transform parent, [Optional] Vector3 position, [Optional] Quaternion rotation)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(inst, Vector3.zero, Quaternion.identity) as GameObject;
		gameObject.transform.SetParent(parent);
		gameObject.transform.localPosition = position;
		gameObject.transform.localRotation = rotation;
		gameObject.transform.localScale = Vector3.one;
		gameObject.name = gameObject.name.Replace("(Clone)", string.Empty);
		return gameObject;
	}

	public static Vector3 GetRagdollForce(Vector3 playerPosition, Vector3 position)
	{
		Vector3 result = playerPosition - position;
		result.x = Mathf.Clamp(result.x, -1f, 1f);
		result.y = Mathf.Clamp(result.y, -1f, 1f);
		result.z = Mathf.Clamp(result.z, -1f, 1f);
		return result;
	}

	public static Color GetColor(int color)
	{
		switch (color)
		{
		case 0:
			return Color.white;
		case 1:
			return Color.red;
		case 2:
			return Color.yellow;
		case 3:
			return Color.green;
		case 4:
			return Color.cyan;
		case 5:
			return Color.blue;
		case 6:
			return Color.magenta;
		case 7:
			return Color.gray;
		case 8:
			return Color.black;
		case 9:
			return Color.clear;
		default:
			return Color.white;
		}
	}

	public static string ColorToHex(Color32 color)
	{
		return string.Concat(new string[]
		{
			"[",
			color.r.ToString("X2"),
			color.g.ToString("X2"),
			color.b.ToString("X2"),
			"]"
		});
	}

	public static string ColorToHex(Color32 color, string text)
	{
		return string.Concat(new string[]
		{
			"[",
			color.r.ToString("X2"),
			color.g.ToString("X2"),
			color.b.ToString("X2"),
			"]",
			text,
			"[-]"
		});
	}

	public static List<Vector3> GenerateBlockRoad(int maxBlocks)
	{
		List<Vector3> list = new List<Vector3>();
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < maxBlocks; i++)
		{
			int num = UnityEngine.Random.Range(1, 5);
			int num2 = UnityEngine.Random.Range(1, 5);
			int num3 = UnityEngine.Random.Range(1, 20);
			if (num >= 4)
			{
				zero.x += 1f;
			}
			if (num <= 3)
			{
				zero.x -= 1f;
			}
			if (num2 >= 4)
			{
				zero.z += 1f;
			}
			if (num2 <= 3)
			{
				zero.z -= 1f;
			}
			if (num3 == 1)
			{
				zero.y += 1f;
			}
			if (num3 == 19)
			{
				zero.y -= 1f;
			}
			if (num3 >= 6)
			{
				list.Add(zero);
			}
		}
		return list;
	}

	public static string NameGenerator(int line)
	{
		string text = string.Empty;
		string[] array = new string[]
		{
			"a",
			"e",
			"i",
			"o",
			"u",
			"y"
		};
		string[] array2 = new string[]
		{
			"b",
			"c",
			"d",
			"f",
			"g",
			"h",
			"j",
			"k",
			"l",
			"m",
			"n",
			"p",
			"q",
			"r",
			"s",
			"t",
			"v",
			"w",
			"x",
			"z"
		};
		bool flag = false;
		if (UnityEngine.Random.value > 0.5f)
		{
			text = array2[UnityEngine.Random.Range(0, array2.Length)];
		}
		else
		{
			text = array[UnityEngine.Random.Range(0, array.Length)];
			flag = true;
		}
		text = text.ToUpper();
		for (int i = 0; i < line - 1; i++)
		{
			if (flag)
			{
				if (UnityEngine.Random.value < 0.2f)
				{
					text += array[UnityEngine.Random.Range(0, array.Length)];
				}
				else
				{
					text += array2[UnityEngine.Random.Range(0, array2.Length)];
					flag = false;
				}
			}
			else if (UnityEngine.Random.value < 0.2f)
			{
				text += array2[UnityEngine.Random.Range(0, array2.Length)];
			}
			else
			{
				text += array[UnityEngine.Random.Range(0, array.Length)];
				flag = true;
			}
		}
		return text;
	}

	public static string MD5(string text)
	{
		return Utils.MD5(new UTF8Encoding().GetBytes(text));
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

	public static string MD5File(string path)
	{
		string result;
		using (MD5 md = System.Security.Cryptography.MD5.Create())
		{
			using (FileStream fileStream = File.OpenRead(path))
			{
				byte[] array = md.ComputeHash(fileStream);
				string text = string.Empty;
				for (int i = 0; i < array.Length; i++)
				{
					text += Convert.ToString(array[i], 16).PadLeft(2, '0');
				}
				result = text.PadLeft(32, '0');
			}
		}
		return result;
	}

	public static bool IsNullOrWhiteSpace(string value)
	{
		if (value == null)
		{
			return true;
		}
		if (value.Length == 0)
		{
			return true;
		}
		for (int i = 0; i < value.Length; i++)
		{
			if (value[i].ToString() == " ")
			{
				return true;
			}
		}
		return false;
	}

	public static string XOR(string text)
	{
		return Utils.XOR(text, GameSettings.instance.Keys[0], false);
	}

	public static string XOR(string text, bool encrypt)
	{
		return Utils.XOR(text, GameSettings.instance.Keys[0], encrypt);
	}

	public static string XOR(string text, string key, bool encrypt)
	{
		if (encrypt)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			byte[] bytes2 = Encoding.UTF8.GetBytes(key);
			return Convert.ToBase64String(Utils.EncryptDecrypt(bytes, bytes2));
		}
		byte[] data = Convert.FromBase64String(text);
		byte[] bytes3 = Encoding.UTF8.GetBytes(key);
		return Encoding.UTF8.GetString(Utils.EncryptDecrypt(data, bytes3));
	}

	public static byte[] XOR(byte[] bytes)
	{
		byte[] bytes2 = Encoding.UTF8.GetBytes(GameSettings.instance.Keys[0]);
		return Utils.EncryptDecrypt(bytes, bytes2);
	}

	public static byte[] XOR(byte[] bytes, byte[] key)
	{
		return Utils.EncryptDecrypt(bytes, key);
	}

	private static byte[] EncryptDecrypt(byte[] data, byte[] key)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		byte[] array = new byte[256];
		for (int i = 0; i < 256; i++)
		{
			array[i] = (byte)i;
		}
		for (int j = 0; j < 256; j++)
		{
			num3 = (byte)((num3 + (int)array[j] + (int)key[j % key.Length]) % 256);
			byte b = array[j];
			array[j] = array[num3];
			array[num3] = b;
		}
		byte[] array2 = new byte[data.Length];
		for (int k = 0; k < data.Length; k++)
		{
			num = (num + 1) % 256;
			num2 = (num2 + (int)array[num]) % 256;
			byte b = array[num];
			array[num] = array[num2];
			array[num2] = b;
			array2[k] = (byte)(data[k] ^ array[(int)(array[num] + array[num2]) % 256]);
		}
		return array2;
	}

	public static bool CompareVersion(string a, string b)
	{
		string[] array = a.Split(new char[]
		{
			"."[0]
		});
		string[] array2 = b.Split(new char[]
		{
			"."[0]
		});
		for (int i = 0; i < array.Length; i++)
		{
			if (Utils.StringToInt(array[i]) != Utils.StringToInt(array2[i]))
			{
				return Utils.StringToInt(array[i]) < Utils.StringToInt(array2[i]);
			}
		}
		return false;
	}

	public static int StringToInt(string text)
	{
		string text2 = string.Empty;
		for (int i = 0; i < text.Length; i++)
		{
			if (char.IsDigit(text[i]))
			{
				text2 += text[i];
			}
		}
		return int.Parse(text2);
	}

	public static string StripRich(string text)
	{
		int num = text.IndexOf("<");
		if (num == -1)
		{
			return text;
		}
		int num2 = text.IndexOf(">");
		if (num2 == -1)
		{
			return text;
		}
		return Utils.StripRich(text.Remove(num, num2 - num + 1));
	}
}
