using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class CacheManager
{
    public static string cryptoKey = "1gf4f4hy";

    public static void Save<T>(string name, string path, T data)
	{
		CacheManager.Save<T>(name, path, data, false, false, string.Empty);
	}

	public static void Save<T>(string name, string path, T data, bool md5)
	{
		CacheManager.Save<T>(name, path, data, md5, false, string.Empty);
	}

	public static void Save<T>(string name, string path, T data, bool md5, bool crypto)
	{
		CacheManager.Save<T>(name, path, data, md5, crypto, string.Empty);
	}

	public static void Save<T>(string name, string path, T data, bool md5, bool crypto, string customCryptoKey)
	{
		if (Application.isEditor)
		{
			path = Directory.GetParent(Application.dataPath).FullName + "/Cache/" + path;
		}
		if (Application.platform == RuntimePlatform.Android)
		{
			string text = new AndroidJavaClass("android.os.Environment").CallStatic<AndroidJavaObject>("getExternalStorageDirectory", new object[0]).Call<string>("getAbsolutePath", new object[0]);
			text += "/Android/data/";
			text += new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Call<string>("getPackageName", new object[0]);
			if (Directory.Exists(text))
			{
				if (Directory.Exists(text + "/cache"))
				{
					Directory.CreateDirectory(text + "/cache");
				}
				path = text + "/cache/" + path;
			}
			else
			{
				path = Application.dataPath;
			}
		}
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		if (md5)
		{
			name = CacheManager.Md5(name);
		}
		Stream stream = File.Open(path + "/" + name, FileMode.Create);
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		if (crypto)
		{
			string text2 = CacheManager.cryptoKey;
			if (!string.IsNullOrEmpty(customCryptoKey))
			{
				text2 = customCryptoKey;
			}
			CryptoStream cryptoStream = new CryptoStream(stream, new DESCryptoServiceProvider
			{
				Key = Encoding.ASCII.GetBytes(text2.Substring(0, 8)),
				IV = Encoding.ASCII.GetBytes(text2.Substring(0, 8))
			}.CreateEncryptor(), CryptoStreamMode.Write);
			binaryFormatter.Serialize(cryptoStream, data);
			cryptoStream.Close();
		}
		else
		{
			binaryFormatter.Serialize(stream, data);
		}
		stream.Close();
	}

	public static void SaveAsync<T>(string name, string path, T data)
	{
		CacheManager.SaveAsync<T>(name, path, data, false, false, string.Empty);
	}

	public static void SaveAsync<T>(string name, string path, T data, bool md5)
	{
		CacheManager.SaveAsync<T>(name, path, data, md5, false, string.Empty);
	}

	public static void SaveAsync<T>(string name, string path, T data, bool md5, bool crypto)
	{
		CacheManager.SaveAsync<T>(name, path, data, md5, crypto, string.Empty);
	}

	public static void SaveAsync<T>(string name, string path, T data, bool md5, bool crypto, string customCryptoKey)
	{
		if (SystemInfo.processorCount <= 1)
		{
			CacheManager.Save<T>(name, path, data, md5, crypto, customCryptoKey);
			return;
		}
		if (Application.isEditor)
		{
			path = Directory.GetParent(Application.dataPath).FullName + "/Cache/" + path;
		}
		if (Application.platform == RuntimePlatform.Android)
		{
			string text = new AndroidJavaClass("android.os.Environment").CallStatic<AndroidJavaObject>("getExternalStorageDirectory", new object[0]).Call<string>("getAbsolutePath", new object[0]);
			text += "/Android/data/";
			text += new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Call<string>("getPackageName", new object[0]);
			if (Directory.Exists(text))
			{
				if (Directory.Exists(text + "/cache"))
				{
					Directory.CreateDirectory(text + "/cache");
				}
				path = text + "/cache/" + path;
			}
			else
			{
				path = Application.dataPath;
			}
		}
		Loom.RunAsync(delegate
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			if (md5)
			{
				name = CacheManager.Md5(name);
			}
			Stream stream = File.Open(path + "/" + name, FileMode.Create);
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			if (crypto)
			{
				string customCryptoKey2 = CacheManager.cryptoKey;
				if (!string.IsNullOrEmpty(customCryptoKey))
				{
					customCryptoKey2 = customCryptoKey;
				}
				CryptoStream cryptoStream = new CryptoStream(stream, new DESCryptoServiceProvider
				{
					Key = Encoding.ASCII.GetBytes(customCryptoKey2.Substring(0, 8)),
					IV = Encoding.ASCII.GetBytes(customCryptoKey2.Substring(0, 8))
				}.CreateEncryptor(), CryptoStreamMode.Write);
				binaryFormatter.Serialize(cryptoStream, data);
				cryptoStream.Close();
			}
			else
			{
				binaryFormatter.Serialize(stream, data);
			}
			stream.Close();
		});
	}

	public static T Load<T>(string name, string path)
	{
		return CacheManager.Load<T>(name, path, false, false, string.Empty);
	}

	public static T Load<T>(string name, string path, bool md5)
	{
		return CacheManager.Load<T>(name, path, md5, false, string.Empty);
	}

	public static T Load<T>(string name, string path, bool md5, bool crypto)
	{
		return CacheManager.Load<T>(name, path, md5, crypto, string.Empty);
	}

	public static T Load<T>(string name, string path, bool md5, bool crypto, string customCryptoKey)
	{
		if (Application.isEditor)
		{
			path = Directory.GetParent(Application.dataPath).FullName + "/Cache/" + path;
		}
		if (Application.platform == RuntimePlatform.Android)
		{
			string text = new AndroidJavaClass("android.os.Environment").CallStatic<AndroidJavaObject>("getExternalStorageDirectory", new object[0]).Call<string>("getAbsolutePath", new object[0]);
			text += "/Android/data/";
			text += new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Call<string>("getPackageName", new object[0]);
			if (Directory.Exists(text))
			{
				if (Directory.Exists(text + "/cache"))
				{
					Directory.CreateDirectory(text + "/cache");
				}
				path = text + "/cache/" + path;
			}
			else
			{
				path = Application.dataPath;
			}
		}
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		if (md5)
		{
			name = CacheManager.Md5(name);
		}
		Stream stream = File.Open(path + "/" + name, FileMode.Open);
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		object obj;
		if (crypto)
		{
			string text2 = CacheManager.cryptoKey;
			if (!string.IsNullOrEmpty(customCryptoKey))
			{
				text2 = customCryptoKey;
			}
			CryptoStream cryptoStream = new CryptoStream(stream, new DESCryptoServiceProvider
			{
				Key = Encoding.ASCII.GetBytes(text2.Substring(0, 8)),
				IV = Encoding.ASCII.GetBytes(text2.Substring(0, 8))
			}.CreateDecryptor(), CryptoStreamMode.Read);
			obj = binaryFormatter.Deserialize(cryptoStream);
			cryptoStream.Close();
		}
		else
		{
			obj = binaryFormatter.Deserialize(stream);
		}
		stream.Close();
		return (T)((object)obj);
	}

	public static void LoadAsync<T>(Action<T> callback, string name, string path)
	{
		CacheManager.LoadAsync<T>(callback, name, path, false, false, string.Empty);
	}

	public static void LoadAsync<T>(Action<T> callback, string name, string path, bool md5)
	{
		CacheManager.LoadAsync<T>(callback, name, path, md5, false, string.Empty);
	}

	public static void LoadAsync<T>(Action<T> callback, string name, string path, bool md5, bool crypto)
	{
		CacheManager.LoadAsync<T>(callback, name, path, md5, crypto, string.Empty);
	}

	public static void LoadAsync<T>(Action<T> callback, string name, string path, bool md5, bool crypto, string customCryptoKey)
	{
		if (SystemInfo.processorCount <= 1)
		{
			callback(CacheManager.Load<T>(name, path, md5, crypto, customCryptoKey));
		}
		if (Application.isEditor)
		{
			path = Directory.GetParent(Application.dataPath).FullName + "/Cache/" + path;
		}
		if (Application.platform == RuntimePlatform.Android)
		{
			string text = new AndroidJavaClass("android.os.Environment").CallStatic<AndroidJavaObject>("getExternalStorageDirectory", new object[0]).Call<string>("getAbsolutePath", new object[0]);
			text += "/Android/data/";
			text += new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Call<string>("getPackageName", new object[0]);
			if (Directory.Exists(text))
			{
				if (Directory.Exists(text + "/cache"))
				{
					Directory.CreateDirectory(text + "/cache");
				}
				path = text + "/cache/" + path;
			}
			else
			{
				path = Application.dataPath;
			}
		}
		Loom.RunAsync(delegate
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			if (md5)
			{
				name = CacheManager.Md5(name);
			}
			Stream stream = File.Open(path + "/" + name, FileMode.Open);
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			object value = null;
			if (crypto)
			{
				string customCryptoKey2 = CacheManager.cryptoKey;
				if (!string.IsNullOrEmpty(customCryptoKey))
				{
					customCryptoKey2 = customCryptoKey;
				}
				CryptoStream cryptoStream = new CryptoStream(stream, new DESCryptoServiceProvider
				{
					Key = Encoding.ASCII.GetBytes(customCryptoKey2.Substring(0, 8)),
					IV = Encoding.ASCII.GetBytes(customCryptoKey2.Substring(0, 8))
				}.CreateDecryptor(), CryptoStreamMode.Read);
				value = binaryFormatter.Deserialize(cryptoStream);
				cryptoStream.Close();
			}
			else
			{
				value = binaryFormatter.Deserialize(stream);
			}
			stream.Close();
			Loom.QueueOnMainThread(delegate()
			{
				callback((T)((object)value));
			});
		});
	}

	public static bool Exists(string name, string path, bool md5)
	{
		if (Application.isEditor)
		{
			path = Directory.GetParent(Application.dataPath).FullName + "/Cache/" + path;
		}
		if (Application.platform == RuntimePlatform.Android)
		{
			string text = new AndroidJavaClass("android.os.Environment").CallStatic<AndroidJavaObject>("getExternalStorageDirectory", new object[0]).Call<string>("getAbsolutePath", new object[0]);
			text += "/Android/data/";
			text += new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Call<string>("getPackageName", new object[0]);
			if (Directory.Exists(text))
			{
				if (Directory.Exists(text + "/cache"))
				{
					Directory.CreateDirectory(text + "/cache");
				}
				path = text + "/cache/" + path;
			}
			else
			{
				path = Application.dataPath;
			}
		}
		if (md5)
		{
			name = CacheManager.Md5(name);
		}
		return File.Exists(path + "/" + name);
	}

	private static string Md5(string text)
	{
		UTF8Encoding utf8Encoding = new UTF8Encoding();
		byte[] bytes = utf8Encoding.GetBytes(text);
		MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] array = md5CryptoServiceProvider.ComputeHash(bytes);
		string text2 = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			text2 += Convert.ToString(array[i], 16).PadLeft(2, '0');
		}
		return text2.PadLeft(32, '0');
	}
}
