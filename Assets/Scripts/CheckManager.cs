using System;
using System.IO;
using FreeJSON;
using UnityEngine;

public class CheckManager : MonoBehaviour
{
    private static bool detected;

    private static bool quit;

    private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		CryptoManager.StartDetection(delegate
		{
			CheckManager.Detected("Crypto Detected");
		});
		this.CheckAll();
		//this.CheckNewApps();
		//TimerManager.In(5f, false, -1, 5f, delegate()
		//{
            //#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            //return;
            //#endif
            //if (new AndroidJavaClass("com.rexetstudio.Functions").GetStatic<bool>("isDetected"))
			//{
			//	string @static = new AndroidJavaClass("com.rexetstudio.Functions").GetStatic<string>("detectedText");
			//	if (string.IsNullOrEmpty(@static))
			//	{
			//		CheckManager.Detected("Error 345");
			//	}
			//	else
			//	{
			//		CheckManager.Detected(@static);
			//	}
			//}
		//});
	}

	public static void Detected()
	{
		CheckManager.Detected("Detected");
	}

	public static void Detected(string text)
	{
		CheckManager.Detected(text, string.Empty);
	}

	public static void Detected(string text, string log)
	{
		if (CheckManager.quit)
		{
			return;
		}
		if (CheckManager.detected)
		{
			return;
		}
		CheckManager.detected = true;
		GameSettings.instance.PhotonID = string.Empty;
		if (PhotonNetwork.inRoom)
		{
			PhotonNetwork.LeaveRoom(true);
		}
		AndroidNativeFunctions.ShowAlert(text, "Detected", "Ok", string.Empty, string.Empty, delegate(DialogInterface arg0)
		{
			Application.Quit();
		});
	}

	public static void Quit()
	{
		if (CheckManager.quit)
		{
			return;
		}
		CheckManager.quit = true;
		if (PhotonNetwork.inRoom)
		{
			PhotonNetwork.LeaveRoom(true);
		}
		TimerManager.In((float)nValue.int1, false, delegate()
		{
			Application.Quit();
		});
	}

	private void OnApplicationPause(bool pause)
	{
		//if (!pause)
		//{
		//	this.CheckNewApps();
		//	this.CheckAll();
		//}
	}

	private void CheckAll()
	{
		string externalStorageDirectory = AndroidNativeFunctions.GetExternalStorageDirectory();
		if (externalStorageDirectory.Contains("storage/emulated/") && !externalStorageDirectory.Contains("storage/emulated/0"))
		{
			byte b = 0;
			for (int i = 0; i < 100; i++)
			{
				if (Directory.Exists("/storage/emulated/" + i.ToString()))
				{
					b += 1;
				}
			}
			if (b >= 2)
			{
				CheckManager.Detected("Error 41513");
			}
		}
	}

	private void CheckNewApps()
	{
        #if UNITY_EDITOR || UNITY_STANDALONE_WIN
        return;
        #endif
        string text = new AndroidJavaClass("com.rexetstudio.checkapps").CallStatic<string>("GetNewApps", new object[0]);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		JsonArray jsonArray;
		try
		{
			jsonArray = JsonArray.Parse(text);
		}
		catch
		{
			return;
		}
		string text2 = string.Empty;
		int num = int.Parse("25000000");
		for (int i = 0; i < jsonArray.Length; i++)
		{
			text2 = jsonArray.Get<string>(i);
			text2 = text2.Replace("\\", string.Empty);
			text2 = text2.Replace("\"", string.Empty);
			if (!File.Exists(text2))
			{
				CheckManager.Detected("Apps Error");
				break;
			}
			long length = new FileInfo(text2).Length;
			//if (length < (long)num && (lzip.entryExists(text2, "res/raw/chunk8", null) || lzip.entryExists(text2, "lib/armeabi-v7a/libgg_tibe.so", null)))
			//{
			//	CheckManager.Detected("Game Guardian detected");
			//	break;
			//}
		}
	}
}
