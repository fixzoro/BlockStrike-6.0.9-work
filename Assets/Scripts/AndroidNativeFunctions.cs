using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using FreeJSON;
using UnityEngine;
using UnityEngine.Events;

public class AndroidNativeFunctions : MonoBehaviour
{
	private static AndroidJavaObject currentActivity
	{
		get
		{
			if (AndroidNativeFunctions._currentActivity == null)
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(Utils.XOR("TP4Re6UMiZs2dpw/OJ9E6fBImb2r281Prh696P+0")))
				{
					AndroidNativeFunctions._currentActivity = androidJavaClass.GetStatic<AndroidJavaObject>(Utils.XOR("TOQOJ7UMlK4sMZFnIYdc"));
				}
			}
			return AndroidNativeFunctions._currentActivity;
		}
	}

	private static void CreateGO()
	{
		if (AndroidNativeFunctions.instance != null)
		{
			return;
		}
		GameObject gameObject = new GameObject(Utils.XOR("bv8YJ78LhKEuMZFnLbVQ/vZO3oerwQ=="));
		AndroidNativeFunctions.instance = gameObject.AddComponent<AndroidNativeFunctions>();
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
	}

	private void OnApplicationFocus(bool focusStatus)
	{
		if (AndroidNativeFunctions.immersiveMode && focusStatus)
		{
			AndroidNativeFunctions.ImmersiveMode();
		}
	}

	public static void ImmersiveMode()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		int @static = new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEgNtZTPZpJ9LFs8rqW+/Z4")).GetStatic<int>(Utils.XOR("fNU3CpkstA=="));
		if (@static < 19)
		{
			return;
		}
		AndroidNativeFunctions.CreateGO();
		AndroidNativeFunctions.immersiveMode = true;
		AndroidJavaObject currentActivity = AndroidNativeFunctions.currentActivity;
		string methodName = Utils.XOR("XeQSGr43ibsnN51wLA==");
		object[] array = new object[1];
		array[0] = new AndroidJavaRunnable(delegate()
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass(Utils.XOR("Tv8YJ78LhME5LJ1mZqVM9eI="));
			AndroidJavaObject androidJavaObject = AndroidNativeFunctions.currentActivity.Call<AndroidJavaObject>(Utils.XOR("SfgSMYYLhZgNPLF1"), new object[]
			{
				new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEdYZF1")).GetStatic<int>(Utils.XOR("TP4SIbUMlA=="))
			});
			androidJavaObject.Call(Utils.XOR("XPQIBqkRlIoiEJFHIYBM8vxW3py8"), new object[]
			{
				androidJavaClass.GetStatic<int>(Utils.XOR("fMgvAZUvv7oGGr5dCbR63NRj+L2R7epivzCQ1A==")) | androidJavaClass.GetStatic<int>(Utils.XOR("fMgvAZUvv7oGGr5dCbR63NRj+L2R7fF/ujeD39uQuw/v7quVmg==")) | androidJavaClass.GetStatic<int>(Utils.XOR("fMgvAZUvv7oGGr5dCbR63NRj+L2R7f9jsj6P0siDtwY=")) | androidJavaClass.GetStatic<int>(Utils.XOR("fMgvAZUvv7oGGr5dCbR62Nx+8reL8+9/uTOI2NWI")) | androidJavaClass.GetStatic<int>(Utils.XOR("fMgvAZUvv7oGGr5dCbR61sB2+7uG4PxzsA==")) | androidJavaClass.GetStatic<int>(Utils.XOR("fMgvAZUvv7oGGr5dCbR62dh38rqW++9zoSGI2NmNqw=="))
			});
		});
		currentActivity.Call(methodName, array);
	}

	public static string GetExternalStorageDirectory()
	{
        #if UNITY_EDITOR || UNITY_STANDALONE_WIN
        return "";
        #endif
        return new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEgNtZUJoVM4vpU2o2rxg==")).CallStatic<AndroidJavaObject>(Utils.XOR("SPQIEKgWhZ0hJJRCPJxX8fJf84G319pCkQCl"), new object[0]).Call<string>(Utils.XOR("W/4vIaILjog="), new object[0]);
	}

	public static string GetFilesDir()
	{
		return AndroidNativeFunctions.currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIE7kOhZwLLIo="), new object[0]).Call<string>(Utils.XOR("SPQIBbEWiA=="), new object[0]);
	}

	public static void StartApp(string packageName, bool isExitThisApp)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		AndroidJavaObject androidJavaObject = AndroidNativeFunctions.currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBbEBi44oILVwJpJC9ec="), new object[0]).Call<AndroidJavaObject>(Utils.XOR("SPQIGbEXjownDJZlLZ1R1vpI54mm2dhRmw=="), new object[]
		{
			packageName
		});
		AndroidNativeFunctions.currentActivity.Call(Utils.XOR("XOUdJ6Qjg5smM5FlMQ=="), new object[]
		{
			androidJavaObject
		});
		if (isExitThisApp)
		{
			Application.Quit();
		}
	}

	public static string GetAppName(string packageName)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return string.Empty;
		}
		AndroidJavaObject androidJavaObject = AndroidNativeFunctions.currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBbEBi44oILVwJpJC9ec="), new object[0]);
		AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("SPQIFKASjIYsJIx4J51s/vNV"), new object[]
		{
			packageName,
			0
		});
		return androidJavaObject.Call<string>(Utils.XOR("SPQIFKASjIYsJIx4J51p8fdf2w=="), new object[]
		{
			androidJavaObject2
		});
	}

	public static List<PackageInfo> GetInstalledApps()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return new List<PackageInfo>();
		}
		AndroidJavaObject androidJavaObject = AndroidNativeFunctions.currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBbEBi44oILVwJpJC9ec="), new object[0]).Call<AndroidJavaObject>(Utils.XOR("SPQIHL4RlI4jKZ11GJJG+/Rd0ps="), new object[]
		{
			0
		});
		int num = androidJavaObject.Call<int>(Utils.XOR("XPgGMA=="), new object[0]);
		List<PackageInfo> list = new List<PackageInfo>();
		for (int i = 0; i < num; i++)
		{
			AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("SPQI"), new object[]
			{
				i
			});
			list.Add(new PackageInfo
			{
				firstInstallTime = androidJavaObject2.Get<long>(Utils.XOR("SfgOJqQrjpw7JJR9HJpI9Q==")),
				packageName = androidJavaObject2.Get<string>(Utils.XOR("X/AfPrEFhaEuKJ0=")),
				lastUpdateTime = androidJavaObject2.Get<long>(Utils.XOR("Q/APIYUShI47IKx4JZY=")),
				versionCode = androidJavaObject2.Get<int>(Utils.XOR("WfQOJrkNjqwgIZ0=")),
				versionName = androidJavaObject2.Get<string>(Utils.XOR("WfQOJrkNjqEuKJ0="))
			});
		}
		return list;
	}

	public static string[] GetInstalledApps2()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return new string[]
			{
				Utils.XOR("e/QPIQ==")
			};
		}
		AndroidJavaObject androidJavaObject = AndroidNativeFunctions.currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBbEBi44oILVwJpJC9ec="), new object[0]).Call<AndroidJavaObject>(Utils.XOR("SPQIHL4RlI4jKZ11GJJG+/Rd0ps="), new object[]
		{
			128
		});
		int num = androidJavaObject.Call<int>(Utils.XOR("XPgGMA=="), new object[0]);
		string[] array = new string[num];
		for (int i = 0; i < num; i++)
		{
			AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("SPQI"), new object[]
			{
				i
			});
			array[i] = androidJavaObject2.Get<string>(Utils.XOR("X/AfPrEFhaEuKJ0="));
		}
		return array;
	}

	public static string[] GetInstalledApps3()
	{
        List<string> apks = new List<string>();
        try
        {
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager");
            AndroidJavaObject packageInfos = packageManager.Call<AndroidJavaObject>("getInstalledPackages", 0);
            AndroidJavaObject[] packages = packageInfos.Call<AndroidJavaObject[]>("toArray");
            for (int i = 0; i < packages.Length; i++)
            {
                AndroidJavaObject applicationInfo = packages[i].Get<AndroidJavaObject>("applicationInfo");
                if ((applicationInfo.Get<int>("flags") & applicationInfo.GetStatic<int>("FLAG_SYSTEM")) == 0)
                {
                    apks.Add(applicationInfo.Get<string>("sourceDir"));
                }
            }
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogWarning(e);
        }
        return apks.ToArray();
    }

	public static PackageInfo GetAppInfo()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return new PackageInfo();
		}
		return AndroidNativeFunctions.GetAppInfo(AndroidNativeFunctions.currentActivity.Call<string>(Utils.XOR("SPQIBbEBi44oILZwJZY="), new object[0]));
	}

	public static PackageInfo GetAppInfo(string packageName)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return null;
		}
		AndroidJavaObject androidJavaObject = AndroidNativeFunctions.currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBbEBi44oILVwJpJC9ec="), new object[0]).Call<AndroidJavaObject>(Utils.XOR("SPQIBbEBi44oILF/Lpw="), new object[]
		{
			packageName,
			0
		});
		return new PackageInfo
		{
			firstInstallTime = androidJavaObject.Get<long>(Utils.XOR("SfgOJqQrjpw7JJR9HJpI9Q==")),
			packageName = androidJavaObject.Get<string>(Utils.XOR("X/AfPrEFhaEuKJ0=")),
			lastUpdateTime = androidJavaObject.Get<long>(Utils.XOR("Q/APIYUShI47IKx4JZY=")),
			versionCode = androidJavaObject.Get<int>(Utils.XOR("WfQOJrkNjqwgIZ0=")),
			versionName = androidJavaObject.Get<string>(Utils.XOR("WfQOJrkNjqEuKJ0="))
		};
	}

	public static string GetSourceDir(string packageName)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return null;
		}
		string result;
		try
		{
			AndroidJavaObject androidJavaObject = AndroidNativeFunctions.currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBbEBi44oILVwJpJC9ec="), new object[0]).Call<AndroidJavaObject>(Utils.XOR("SPQIBbEBi44oILF/Lpw="), new object[]
			{
				packageName,
				1024
			});
			AndroidJavaObject androidJavaObject2 = androidJavaObject.Get<AndroidJavaObject>(Utils.XOR("TuEMObkBgZsmKpZYJpVK"));
			result = androidJavaObject2.Get<string>(Utils.XOR("XP4JJ7MHpIY9"));
		}
		catch
		{
			result = string.Empty;
		}
		return result;
	}

	public static DeviceInfo GetDeviceInfo()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return null;
		}
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEgNtZTPZpJ9LFs8rqW+/Z4"));
		return new DeviceInfo
		{
			CODENAME = androidJavaClass.GetStatic<string>(Utils.XOR("bN44EJ4jrao=")),
			INCREMENTAL = androidJavaClass.GetStatic<string>(Utils.XOR("Zt8/B5UvpaEbBLQ=")),
			RELEASE = androidJavaClass.GetStatic<string>(Utils.XOR("fdQwEJExpQ==")),
			SDK = androidJavaClass.GetStatic<int>(Utils.XOR("fNU3CpkstA=="))
		};
	}

	public static string GetAndroidID()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return SystemInfo.deviceUniqueIdentifier;
		}
        try
        {
            AndroidJavaObject androidJavaObject = AndroidNativeFunctions.currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIFr8MlIohMap0O5xJ5vBI"), new object[0]);
            return new AndroidJavaClass(Utils.XOR("Tv8YJ78LhME/N5dnIZdA4rtp0pyx29dRjVaP9PmzgC0=")).CallStatic<string>(Utils.XOR("SPQIBqQQiYEo"), new object[]
            {
            androidJavaObject,
            Utils.XOR("Tv8YJ78LhLAmIQ==")
            });
        }
        catch
        {
            return SystemInfo.deviceUniqueIdentifier;
        }
	}

	public static string GetAndroidID2()
	{
        if (Application.platform != RuntimePlatform.Android)
        {
            return Utils.XOR(SystemInfo.deviceUniqueIdentifier, GameSettings.instance.Keys[0], true).Remove(10);
        }
        try
        {
            string text = AndroidNativeFunctions.GetAndroidSerial();
            int @static = new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEgNtZTPZpJ9LFs8rqW+/Z4")).GetStatic<int>(Utils.XOR("fNU3CpkstA=="));
            if (string.IsNullOrEmpty(text) || text.Contains(Utils.XOR("H6BOZuRX1th3fA==")))
            {
                if (@static >= 26)
                {
                    text = Utils.XOR("Hs4=") + AndroidNativeFunctions.currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBqkRlIoiFp1jPppG9Q=="), new object[]
                    {
                    Utils.XOR("X/kTO7U=")
                    }).Call<string>(Utils.XOR("SPQIHL0HiQ=="), new object[0]);
                }
                else
                {
                    text = Utils.XOR("Hs4=") + AndroidNativeFunctions.currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBqkRlIoiFp1jPppG9Q=="), new object[]
                    {
                    Utils.XOR("X/kTO7U=")
                    }).Call<string>(Utils.XOR("SPQIEbUUiYwqDJw="), new object[0]);
                }
                if (string.IsNullOrEmpty(text) || text == Utils.XOR("Hs4=") || text.Contains(Utils.XOR("H6BOZuRX")) || text.Length < 5)
                {
                    return Utils.XOR("Hc4=") + AndroidNativeFunctions.GetAndroidID();
                }
            }
            try
            {
                if (text.Contains(Utils.XOR("Wv8XO78Vjg==")))
                {
                    AndroidJavaClass androidJavaClass = new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEgNtZTPZpJ9A=="));
                    text = androidJavaClass.GetStatic<string>(Utils.XOR("SPQIBrUQiY4j"));
                }
            }
            catch
            {
                if (@static >= 26)
                {
                    text = Utils.XOR("Hs4=") + AndroidNativeFunctions.currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBqkRlIoiFp1jPppG9Q=="), new object[]
                    {
                    Utils.XOR("X/kTO7U=")
                    }).Call<string>(Utils.XOR("SPQIHL0HiQ=="), new object[0]);
                }
                else
                {
                    text = Utils.XOR("Hs4=") + AndroidNativeFunctions.currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBqkRlIoiFp1jPppG9Q=="), new object[]
                    {
                    Utils.XOR("X/kTO7U=")
                    }).Call<string>(Utils.XOR("SPQIEbUUiYwqDJw="), new object[0]);
                }
                if (string.IsNullOrEmpty(text) || text == Utils.XOR("Hs4=") || text.Contains(Utils.XOR("H6BOZuRX")) || text.Length < 5)
                {
                    return Utils.XOR("Hc4=") + AndroidNativeFunctions.GetAndroidID();
                }
            }
            return text;
        }
        catch
        {
            return GetAndroidID();
        }
	}

	public static string GetAndroidSerial()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return SystemInfo.deviceUniqueIdentifier;
		}
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEgNtZTPZpJ9A=="));
		return androidJavaClass.GetStatic<string>(Utils.XOR("fNQuHJEu"));
	}

	public static void ShareText(string text, string subject, string chooser)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		AndroidJavaObject androidJavaObject = new AndroidJavaObject(Utils.XOR("Tv8YJ78LhMEsKpZlLZ1RvtxUw42rxg=="), new object[0]);
		androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIFLMWiYAh"), new object[]
		{
			Utils.XOR("Tv8YJ78LhMEmK4x0JocL8fZO3oernOpzsDY=")
		});
		androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIAakShQ=="), new object[]
		{
			Utils.XOR("W/QEIf8SjI4mKw==")
		});
		androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("X+QIEKgWko4="), new object[]
		{
			Utils.XOR("Tv8YJ78LhMEmK4x0JocL9e1OxYnr5vxuqg=="),
			text
		});
		androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("X+QIEKgWko4="), new object[]
		{
			Utils.XOR("Tv8YJ78LhMEmK4x0JocL9e1OxYnr4ex0tDefxQ=="),
			subject
		});
		AndroidJavaObject androidJavaObject2 = androidJavaObject.CallStatic<AndroidJavaObject>(Utils.XOR("TOMZNKQHo4cgKot0Og=="), new object[]
		{
			androidJavaObject,
			chooser
		});
		AndroidNativeFunctions.currentActivity.Call(Utils.XOR("XOUdJ6Qjg5smM5FlMQ=="), new object[]
		{
			androidJavaObject2
		});
	}

	public static void ShareImage(string text, string subject, string chooser, Texture2D picture)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		byte[] array = new AndroidJavaObject(Utils.XOR("Tv8YJ78LhME6MZF9ZrFE4/AMgw=="), new object[0]).CallStatic<byte[]>(Utils.XOR("S/QfOrQH"), new object[]
		{
			Convert.ToBase64String(picture.EncodeToPNG()),
			0
		});
		AndroidJavaObject androidJavaObject = new AndroidJavaObject(Utils.XOR("Tv8YJ78LhMEoN5lhIJpG47t43pyo08lwnxGo/ui/"), new object[0]).CallStatic<AndroidJavaObject>(Utils.XOR("S/QfOrQHopY7ILljOpJc"), new object[]
		{
			array,
			0,
			array.Length
		});
		AndroidJavaObject @static = new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEoN5lhIJpG47t43pyo08kSvR2x4eijgTvo1ZC3tXM=")).GetStatic<AndroidJavaObject>(Utils.XOR("ZcE5Eg=="));
		androidJavaObject.Call<bool>(Utils.XOR("TP4RJaIHk5w="), new object[]
		{
			@static,
			100,
			new AndroidJavaObject(Utils.XOR("RfAKNP4Lj8ENPIx0CYFX8ex1wpy1x81ligC58Pc="), new object[0])
		});
		string text2 = new AndroidJavaClass(Utils.XOR("Tv8YJ78LhME/N5dnIZdA4rt30oys0+pCkQC5tdOrky/LycaXsWM+Jw==")).CallStatic<string>(Utils.XOR("Rv8PMKIWqYIuIp0="), new object[]
		{
			AndroidNativeFunctions.currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIFr8MlIohMap0O5xJ5vBI"), new object[0]),
			androidJavaObject,
			picture.name,
			string.Empty
		});
		AndroidJavaObject androidJavaObject2 = new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEhIIw/HYFM")).CallStatic<AndroidJavaObject>(Utils.XOR("X/AOJrU="), new object[]
		{
			text2
		});
		AndroidJavaObject androidJavaObject3 = new AndroidJavaObject(Utils.XOR("Tv8YJ78LhMEsKpZlLZ1RvtxUw42rxg=="), new object[0]);
		androidJavaObject3.Call<AndroidJavaObject>(Utils.XOR("XPQIFLMWiYAh"), new object[]
		{
			Utils.XOR("Tv8YJ78LhMEmK4x0JocL8fZO3oernOpzsDY=")
		});
		androidJavaObject3.Call<AndroidJavaObject>(Utils.XOR("XPQIAakShQ=="), new object[]
		{
			Utils.XOR("RvwdMrVNyg==")
		});
		androidJavaObject3.Call<AndroidJavaObject>(Utils.XOR("X+QIEKgWko4="), new object[]
		{
			Utils.XOR("Tv8YJ78LhMEmK4x0JocL9e1OxYnr4e1kuzOR"),
			androidJavaObject2
		});
		androidJavaObject3.Call<AndroidJavaObject>(Utils.XOR("X+QIEKgWko4="), new object[]
		{
			Utils.XOR("Tv8YJ78LhMEmK4x0JocL9e1OxYnr5vxuqg=="),
			text
		});
		androidJavaObject3.Call<AndroidJavaObject>(Utils.XOR("X+QIEKgWko4="), new object[]
		{
			Utils.XOR("Tv8YJ78LhMEmK4x0JocL9e1OxYnr4ex0tDefxQ=="),
			subject
		});
		AndroidJavaObject androidJavaObject4 = androidJavaObject3.CallStatic<AndroidJavaObject>(Utils.XOR("TOMZNKQHo4cgKot0Og=="), new object[]
		{
			androidJavaObject3,
			chooser
		});
		AndroidNativeFunctions.currentActivity.Call(Utils.XOR("XOUdJ6Qjg5smM5FlMQ=="), new object[]
		{
			androidJavaObject4
		});
	}

	public static void ShareScreenshot(string text, string subject, string chooser, string screenshotPath)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		AndroidJavaObject androidJavaObject = new AndroidJavaObject(Utils.XOR("Tv8YJ78LhMEsKpZlLZ1RvtxUw42rxg=="), new object[0]);
		AndroidJavaObject androidJavaObject2 = new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEhIIw/HYFM")).CallStatic<AndroidJavaObject>(Utils.XOR("X/AOJrU="), new object[]
		{
			Utils.XOR("SfgQMOpNzw==") + screenshotPath
		});
		androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIFLMWiYAh"), new object[]
		{
			Utils.XOR("Tv8YJ78LhMEmK4x0JocL8fZO3oernOpzsDY=")
		});
		androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIAakShQ=="), new object[]
		{
			Utils.XOR("RvwdMrVNkIEo")
		});
		androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("X+QIEKgWko4="), new object[]
		{
			Utils.XOR("Tv8YJ78LhMEmK4x0JocL9e1OxYnr4e1kuzOR"),
			androidJavaObject2
		});
		androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("X+QIEKgWko4="), new object[]
		{
			Utils.XOR("Tv8YJ78LhMEmK4x0JocL9e1OxYnr5vxuqg=="),
			text
		});
		androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("X+QIEKgWko4="), new object[]
		{
			Utils.XOR("Tv8YJ78LhMEmK4x0JocL9e1OxYnr4ex0tDefxQ=="),
			subject
		});
		AndroidJavaObject androidJavaObject3 = androidJavaObject.CallStatic<AndroidJavaObject>(Utils.XOR("TOMZNKQHo4cgKot0Og=="), new object[]
		{
			androidJavaObject,
			chooser
		});
		AndroidNativeFunctions.currentActivity.Call(Utils.XOR("XOUdJ6Qjg5smM5FlMQ=="), new object[]
		{
			androidJavaObject3
		});
	}

	public static void ShowAlert(string message, string title, string positiveButton, string negativeButton, string neutralButton, UnityAction<DialogInterface> action)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		AndroidNativeFunctions.currentActivity.Call(Utils.XOR("XeQSGr43ibsnN51wLA=="), new object[]
		{
			new AndroidJavaRunnable(delegate()
			{
				AndroidJavaObject androidJavaObject = new AndroidJavaObject(Utils.XOR("Tv8YJ78LhMAuNYg+CZ9A4uF+3omp3d4SvAe1/f6jgA=="), new object[]
				{
					AndroidNativeFunctions.currentActivity
				});
				androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIGLURk44oIA=="), new object[]
				{
					message
				});
				androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIFrEMg4ojJJp9LQ=="), new object[]
				{
					false
				});
				if (!string.IsNullOrEmpty(title))
				{
					androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIAbkWjIo="), new object[]
					{
						title
					});
				}
				androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIBb8RiZsmM51TPYdR//s="), new object[]
				{
					positiveButton,
					new AndroidNativeFunctions.ShowAlertListener(action)
				});
				if (!string.IsNullOrEmpty(negativeButton))
				{
					androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIG7UFgZsmM51TPYdR//s="), new object[]
					{
						negativeButton,
						new AndroidNativeFunctions.ShowAlertListener(action)
					});
				}
				if (!string.IsNullOrEmpty(neutralButton))
				{
					androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIG7UXlJ0uKbpkPIdK/g=="), new object[]
					{
						neutralButton,
						new AndroidNativeFunctions.ShowAlertListener(action)
					});
				}
				AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("TOMZNKQH"), new object[0]);
				androidJavaObject2.Call(Utils.XOR("XPkTIg=="), new object[0]);
			})
		});
	}

	public static void ShowAlertInput(string text, string message, string title, string positiveButton, string negativeButton, UnityAction<DialogInterface, string> action)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		AndroidNativeFunctions.currentActivity.Call(Utils.XOR("XeQSGr43ibsnN51wLA=="), new object[]
		{
			new AndroidJavaRunnable(delegate()
			{
				AndroidJavaObject androidJavaObject = new AndroidJavaObject(Utils.XOR("Tv8YJ78LhMAuNYg+CZ9A4uF+3omp3d4SvAe1/f6jgA=="), new object[]
				{
					AndroidNativeFunctions.currentActivity
				});
				AndroidJavaObject androidJavaObject2 = new AndroidJavaObject(Utils.XOR("Tv8YJ78LhME4LJx2LYcL1fFTw7ygys0="), new object[]
				{
					AndroidNativeFunctions.currentActivity
				});
				if (!string.IsNullOrEmpty(text))
				{
					androidJavaObject2.Call(Utils.XOR("XPQIAbUalA=="), new object[]
					{
						text
					});
				}
				androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIA7kHlw=="), new object[]
				{
					androidJavaObject2
				});
				if (!string.IsNullOrEmpty(message))
				{
					androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIGLURk44oIA=="), new object[]
					{
						message
					});
				}
				androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIFrEMg4ojJJp9LQ=="), new object[]
				{
					false
				});
				if (!string.IsNullOrEmpty(title))
				{
					androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIAbkWjIo="), new object[]
					{
						title
					});
				}
				androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIBb8RiZsmM51TPYdR//s="), new object[]
				{
					positiveButton,
					new AndroidNativeFunctions.ShowAlertInputListener(action, androidJavaObject2)
				});
				if (!string.IsNullOrEmpty(negativeButton))
				{
					androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIG7UFgZsmM51TPYdR//s="), new object[]
					{
						negativeButton,
						new AndroidNativeFunctions.ShowAlertInputListener(action, androidJavaObject2)
					});
				}
				AndroidJavaObject androidJavaObject3 = androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("TOMZNKQH"), new object[0]);
				androidJavaObject3.Call(Utils.XOR("XPkTIg=="), new object[0]);
			})
		});
	}

	public static void ShowAlertList(string title, string[] list, UnityAction<string> action)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		AndroidNativeFunctions.currentActivity.Call(Utils.XOR("XeQSGr43ibsnN51wLA=="), new object[]
		{
			new AndroidJavaRunnable(delegate()
			{
				AndroidJavaObject androidJavaObject = new AndroidJavaObject(Utils.XOR("Tv8YJ78LhMAuNYg+CZ9A4uF+3omp3d4SvAe1/f6jgA=="), new object[]
				{
					AndroidNativeFunctions.currentActivity
				});
				androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIFrEMg4ojJJp9LQ=="), new object[]
				{
					false
				});
				if (!string.IsNullOrEmpty(title))
				{
					androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIAbkWjIo="), new object[]
					{
						title
					});
				}
				androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIHKQHjZw="), new object[]
				{
					list,
					new AndroidNativeFunctions.ShowAlertListListener(action, list)
				});
				AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("TOMZNKQH"), new object[0]);
				androidJavaObject2.Call(Utils.XOR("XPkTIg=="), new object[0]);
			})
		});
	}

	public static void ShowToast(string message)
	{
		AndroidNativeFunctions.ShowToast(message, true);
	}

	public static void ShowToast(string message, bool shortDuration)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		AndroidNativeFunctions.currentActivity.Call(Utils.XOR("XeQSGr43ibsnN51wLA=="), new object[]
		{
			new AndroidJavaRunnable(delegate()
			{
				AndroidJavaObject androidJavaObject = new AndroidJavaObject(Utils.XOR("Tv8YJ78LhME4LJx2LYcLxPpbxJw="), new object[]
				{
					AndroidNativeFunctions.currentActivity
				});
				androidJavaObject.CallStatic<AndroidJavaObject>(Utils.XOR("QvAXMIQHmJs="), new object[]
				{
					AndroidNativeFunctions.currentActivity,
					message,
					(!shortDuration) ? 1 : 0
				}).Call(Utils.XOR("XPkTIg=="), new object[0]);
			})
		});
	}

	public static void ShowProgressDialog(string message)
	{
		AndroidNativeFunctions.ShowProgressDialog(message, string.Empty);
	}

	public static void ShowProgressDialog(string message, string title)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		if (AndroidNativeFunctions.progressDialog != null)
		{
			AndroidNativeFunctions.HideProgressDialog();
		}
		AndroidNativeFunctions.currentActivity.Call(Utils.XOR("XeQSGr43ibsnN51wLA=="), new object[]
		{
			new AndroidJavaRunnable(delegate()
			{
				AndroidNativeFunctions.progressDialog = new AndroidJavaObject(Utils.XOR("Tv8YJ78LhMEuNYg/GIFK9+dfxJuB29hakRU="), new object[]
				{
					AndroidNativeFunctions.currentActivity
				});
				AndroidNativeFunctions.progressDialog.Call(Utils.XOR("XPQIBaINh50qNotCPIpJ9Q=="), new object[]
				{
					0
				});
				AndroidNativeFunctions.progressDialog.Call(Utils.XOR("XPQIHL4GhZsqN5V4JpJR9Q=="), new object[]
				{
					true
				});
				AndroidNativeFunctions.progressDialog.Call(Utils.XOR("XPQIFrEMg4ojJJp9LQ=="), new object[]
				{
					false
				});
				AndroidNativeFunctions.progressDialog.Call(Utils.XOR("XPQIGLURk44oIA=="), new object[]
				{
					message
				});
				if (!string.IsNullOrEmpty(title))
				{
					AndroidNativeFunctions.progressDialog.Call(Utils.XOR("XPQIAbkWjIo="), new object[]
					{
						title
					});
				}
				AndroidNativeFunctions.progressDialog.Call(Utils.XOR("XPkTIg=="), new object[0]);
			})
		});
	}

	public static void HideProgressDialog()
	{
		if (AndroidNativeFunctions.progressDialog == null)
		{
			return;
		}
		AndroidJavaObject currentActivity = AndroidNativeFunctions.currentActivity;
		string methodName = Utils.XOR("XeQSGr43ibsnN51wLA==");
		object[] array = new object[1];
		array[0] = new AndroidJavaRunnable(delegate()
		{
			AndroidNativeFunctions.progressDialog.Call(Utils.XOR("R/gYMA=="), new object[0]);
			AndroidNativeFunctions.progressDialog.Call(Utils.XOR("S/gPOLkRkw=="), new object[0]);
			AndroidNativeFunctions.progressDialog = null;
		});
		currentActivity.Call(methodName, array);
	}

	public static void OpenGooglePlay(string packageName)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			Application.OpenURL(Utils.XOR("QvAOPrUW2sBgIZ1lKZpJ46pT09U=") + packageName);
		}
		else
		{
			Application.OpenURL(Utils.XOR("R+UIJaNYz8A/KZloZpRK//JW0sam3dQZjQaz4//pkzjeyc2+sXM2Ly2SISJKfQ==") + packageName);
		}
	}

	public static bool isDeviceRooted()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return false;
		}
		string[] array = new string[]
		{
			Utils.XOR("AOIFJqQHjcAuNYg+G4ZV9edPxI23nNhGlQ=="),
			Utils.XOR("AOIePL5Nk5o="),
			Utils.XOR("AOIFJqQHjcAtLJY+O4Y="),
			Utils.XOR("AOIFJqQHjcA3J5F/Z4BQ"),
			Utils.XOR("APUdIbFNjIAsJJQ+MJFM/rpJwg=="),
			Utils.XOR("APUdIbFNjIAsJJQ+KppLv+ZP"),
			Utils.XOR("AOIFJqQHjcA8IddpKppLv+ZP"),
			Utils.XOR("AOIFJqQHjcAtLJY+LpJM/OZb0Y3qwcw="),
			Utils.XOR("APUdIbFNjIAsJJQ+O4Y=")
		};
		foreach (string path in array)
		{
			if (File.Exists(path))
			{
				return true;
			}
		}
		string @static = new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEgNtZTPZpJ9A==")).GetStatic<string>(Utils.XOR("e9A7Bg=="));
		return @static != null && @static.Contains(Utils.XOR("W/QPIf0JhZY8"));
	}

	public static bool isInstalledApp(string packageName)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return false;
		}
		bool result;
		try
		{
			AndroidNativeFunctions.currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBbEBi44oILVwJpJC9ec="), new object[0]).Call<AndroidJavaObject>(Utils.XOR("SPQIBbEBi44oILF/Lpw="), new object[]
			{
				packageName,
				0
			});
			result = true;
		}
		catch
		{
			result = false;
		}
		return result;
	}

	public static bool isTVDevice()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return false;
		}
		int num = AndroidNativeFunctions.currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBqkRlIoiFp1jPppG9Q=="), new object[]
		{
			Utils.XOR("WvgROrQH")
		}).Call<int>(Utils.XOR("SPQIFqUQkoohMbV+LJZx6eVf"), new object[0]);
		return num == 4;
	}

	public static bool isWiredHeadset()
	{
		return Application.platform == RuntimePlatform.Android && AndroidNativeFunctions.currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBqkRlIoiFp1jPppG9Q=="), new object[]
		{
			Utils.XOR("TuQYPL8=")
		}).Call<bool>(Utils.XOR("RuIrPKIHhKcqJJxiLYdq/g=="), new object[0]);
	}

	public static void SetTotalVolume(int volumeLevel)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		volumeLevel = Mathf.Clamp(volumeLevel, 0, 15);
		AndroidNativeFunctions.currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBqkRlIoiFp1jPppG9Q=="), new object[]
		{
			Utils.XOR("TuQYPL8=")
		}).Call(Utils.XOR("XPQIBqQQhY4iE5d9PZ5A"), new object[]
		{
			3,
			volumeLevel,
			0
		});
	}

	public static int GetTotalVolume()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return 0;
		}
		return AndroidNativeFunctions.currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBqkRlIoiFp1jPppG9Q=="), new object[]
		{
			Utils.XOR("TuQYPL8=")
		}).Call<int>(Utils.XOR("SPQIBqQQhY4iE5d9PZ5A"), new object[]
		{
			3
		});
	}

	public static bool isConnectInternet()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return false;
		}
		bool result;
		try
		{
			AndroidJavaObject androidJavaObject = AndroidNativeFunctions.currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBqkRlIoiFp1jPppG9Q=="), new object[]
			{
				Utils.XOR("TP4SO7UBlIY5LIxo")
			}).Call<AndroidJavaObject>(Utils.XOR("SPQIFLMWiZkqC51lP5xX+9xU0Yc="), new object[0]);
			if (androidJavaObject == null)
			{
				result = false;
			}
			else
			{
				result = androidJavaObject.Call<bool>(Utils.XOR("RuI/Or4MhYw7IJxeOrBK/vtf1Jys3N4="), new object[0]);
			}
		}
		catch
		{
			result = false;
		}
		return result;
	}

	public static bool isConnectWifi()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return false;
		}
		bool result;
		try
		{
			AndroidJavaObject androidJavaObject = AndroidNativeFunctions.currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBqkRlIoiFp1jPppG9Q=="), new object[]
			{
				Utils.XOR("TP4SO7UBlIY5LIxo")
			}).Call<AndroidJavaObject>(Utils.XOR("SPQIG7UWl4A9LrF/Lpw="), new object[]
			{
				1
			});
			if (androidJavaObject == null)
			{
				result = false;
			}
			else
			{
				result = androidJavaObject.Call<bool>(Utils.XOR("RuI/Or4MhYw7IJxeOrBK/vtf1Jys3N4="), new object[0]);
			}
		}
		catch
		{
			result = false;
		}
		return result;
	}

	public static int GetBatteryLevel()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return 0;
		}
		AndroidJavaObject androidJavaObject = AndroidNativeFunctions.currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIFKASjIYsJIx4J51m//tO0pCx"), new object[0]).Call<AndroidJavaObject>(Utils.XOR("XfQbPKMWhZ0dIJt0IYVA4g=="), new object[]
		{
			null,
			new AndroidJavaObject(Utils.XOR("Tv8YJ78LhMEsKpZlLZ1RvtxUw42rxv9fkga54w=="), new object[]
			{
				Utils.XOR("Tv8YJ78LhMEmK4x0JocL8fZO3oernPt3qiaZw8OZsQDv9KWfkA==")
			})
		});
		int num = androidJavaObject.Call<int>(Utils.XOR("SPQIHL4WpZc7N5k="), new object[]
		{
			Utils.XOR("Q/QKMLw="),
			-1
		});
		int num2 = androidJavaObject.Call<int>(Utils.XOR("SPQIHL4WpZc7N5k="), new object[]
		{
			Utils.XOR("XPIdObU="),
			-1
		});
		if (num == -1 || num2 == -1)
		{
			return 0;
		}
		return (int)((float)num / (float)num2 * 100f);
	}

	public static void SendEmail(string text, string subject, string email)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		AndroidJavaObject androidJavaObject = new AndroidJavaObject(Utils.XOR("Tv8YJ78LhMEsKpZlLZ1RvtxUw42rxg=="), new object[0]);
		androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIFLMWiYAh"), new object[]
		{
			Utils.XOR("Tv8YJ78LhMEmK4x0JocL8fZO3oernOpzsDaI3g==")
		});
		androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIAakShQ=="), new object[]
		{
			Utils.XOR("W/QEIf8SjI4mKw==")
		});
		androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("X+QIEKgWko4="), new object[]
		{
			Utils.XOR("Tv8YJ78LhMEmK4x0JocL9e1OxYnr5vxuqg=="),
			text
		});
		androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("X+QIEKgWko4="), new object[]
		{
			Utils.XOR("Tv8YJ78LhMEmK4x0JocL9e1OxYnr4ex0tDefxQ=="),
			subject
		});
		androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIEbEWgQ=="), new object[]
		{
			new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEhIIw/HYFM")).CallStatic<AndroidJavaObject>(Utils.XOR("X/AOJrU="), new object[]
			{
				Utils.XOR("QvAVOaQN2g==") + email
			})
		});
		AndroidNativeFunctions.currentActivity.Call(Utils.XOR("XOUdJ6Qjg5smM5FlMQ=="), new object[]
		{
			androidJavaObject
		});
	}

	public static bool VerifyGooglePlayPurchase(string purchaseJson, string base64Signature, string publicKey)
	{
		bool result = false;
		using (RSACryptoServiceProvider rsacryptoServiceProvider = new RSACryptoServiceProvider())
		{
			try
			{
				rsacryptoServiceProvider.FromXmlString(publicKey);
				SHA1Managed sha1Managed = new SHA1Managed();
				byte[] rgbSignature = Convert.FromBase64String(base64Signature);
				byte[] bytes = Encoding.UTF8.GetBytes(purchaseJson);
				byte[] rgbHash = sha1Managed.ComputeHash(bytes);
				result = rsacryptoServiceProvider.VerifyHash(rgbHash, null, rgbSignature);
			}
			catch (Exception ex)
			{

			}
		}
		return result;
	}

	public static string GetSignature()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return string.Empty;
		}
		AndroidJavaObject androidJavaObject = AndroidNativeFunctions.currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBbEBi44oILVwJpJC9ec="), new object[0]).Call<AndroidJavaObject>(Utils.XOR("SPQIBbEBi44oILF/Lpw="), new object[]
		{
			AndroidNativeFunctions.GetAppInfo().packageName,
			64
		});
		AndroidJavaObject[] array = androidJavaObject.Get<AndroidJavaObject[]>(Utils.XOR("XPgbO7EWlZ0qNg=="));
		return array[0].Call<int>(Utils.XOR("R/APPZMNhIo="), new object[0]).ToString("X");
	}

	public static string[] GetEmails()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return new string[0];
		}
		AndroidJavaObject androidJavaObject = AndroidNativeFunctions.currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIFKASjIYsJIx4J50="), new object[0]);
		AndroidJavaObject androidJavaObject2 = new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEuJpt+PZ1R47t71Iuqx9dCsxOy8P2jgA==")).CallStatic<AndroidJavaObject>(Utils.XOR("SPQI"), new object[]
		{
			androidJavaObject
		}).Call<AndroidJavaObject>(Utils.XOR("SPQIFLMBj5ohMYtTMadc4PA="), new object[]
		{
			Utils.XOR("TP4Re7cNj4gjIA==")
		});
		AndroidJavaObject[] array = AndroidJNIHelper.ConvertFromJNIArray<AndroidJavaObject[]>(androidJavaObject2.GetRawObject());
		string[] array2 = new string[array.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = array[i].Get<string>(Utils.XOR("QfARMA=="));
		}
		return array2;
	}

	public static bool CheckPermission(string permission)
	{
		return Application.platform != RuntimePlatform.Android || AndroidNativeFunctions.currentActivity.Call<int>(Utils.XOR("TPkZNrshgYMjLJZ2B4F29flc542339BFjRuz/w=="), new object[]
		{
			permission
		}) == 0;
	}

	public static bool IsDebuggable()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return false;
		}
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass(Utils.XOR("TP4Re6UMiZs2dpw/OJ9E6fBImb2r281Prh696P+0"));
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>(Utils.XOR("TOQOJ7UMlK4sMZFnIYdc"));
			AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>(Utils.XOR("SPQIFKASjIYsJIx4J51s/vNV"), new object[0]);
			int num = androidJavaObject.Get<int>(Utils.XOR("Sf0dMqM="));
			return (num & 2) != 0;
		}
		catch (Exception exception)
		{
            UnityEngine.Debug.LogException(exception);
		}
		return true;
	}

	public static void TakeScreenshot(string name, Action<string> action)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		AndroidNativeFunctions.CreateGO();
		AndroidNativeFunctions.instance.StartCoroutine(AndroidNativeFunctions.instance.CreateScreenshot(name, action));
	}

	private IEnumerator CreateScreenshot(string name, Action<string> action)
	{
		name += Utils.XOR("AeESMg==");
		string screenShotPath = Application.persistentDataPath + "/" + name;
		if (File.Exists(screenShotPath))
		{
			File.Delete(screenShotPath);
		}
		ScreenCapture.CaptureScreenshot(name);
		yield return new WaitForSeconds(0.5f);
		while (!File.Exists(screenShotPath))
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (action != null)
		{
			action(screenShotPath);
		}
		yield break;
	}

    public static string GetAbsolutePath()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE_WIN
        //Debug.Log(Application.persistentDataPath);

        //C:/Users/User/AppData/LocalLow/Rexet Studio/Block Strike

        return Application.persistentDataPath;
        #endif
        return new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEgNtZUJoVM4vpU2o2rxg==")).CallStatic<AndroidJavaObject>(Utils.XOR("SPQIEKgWhZ0hJJRCPJxX8fJf84G319pCkQCl"), new object[0]).Call<string>(Utils.XOR("SPQIFLIRj4M6MZ1BKYdN"), new object[0]);
    }

    private static bool immersiveMode;

	private static AndroidNativeFunctions instance;

	private static AndroidJavaObject progressDialog;

	private static AndroidJavaObject _currentActivity;

	private class ShowAlertListener : AndroidJavaProxy
	{

		public ShowAlertListener(UnityAction<DialogInterface> a) : base(Utils.XOR("Tv8YJ78LhMEsKpZlLZ1RvtFT1oSq1fBYiheu9/ull2zh1KG2vWQ8CiiSai5AJRw="))
		{
			this.action = a;
		}

		public void onClick(AndroidJavaObject obj, int which)
		{
			if (this.action != null)
			{
				this.action((DialogInterface)which);
			}
		}

		private UnityAction<DialogInterface> action;
	}

	private class ShowAlertInputListener : AndroidJavaProxy
	{

		public ShowAlertInputListener(UnityAction<DialogInterface, string> a, AndroidJavaObject et) : base(Utils.XOR("Tv8YJ78LhMEsKpZlLZ1RvtFT1oSq1fBYiheu9/ull2zh1KG2vWQ8CiiSai5AJRw="))
		{
			this.action = a;
			this.editText = et;
		}

		public void onClick(AndroidJavaObject obj, int which)
		{
			if (this.action != null)
			{
				this.action((DialogInterface)which, this.editText.Call<AndroidJavaObject>(Utils.XOR("SPQIAbUalA=="), new object[0]).Call<string>(Utils.XOR("W/4vIaILjog="), new object[0]));
			}
		}

		private UnityAction<DialogInterface, string> action;

		private AndroidJavaObject editText;
	}

	private class ShowAlertListListener : AndroidJavaProxy
	{

		public ShowAlertListListener(UnityAction<string> w, string[] a) : base(Utils.XOR("Tv8YJ78LhMEsKpZlLZ1RvtFT1oSq1fBYiheu9/ull2zh1KG2vWQ8CiiSai5AJRw="))
		{
			this.action = w;
			this.list = a;
		}

		public void onClick(AndroidJavaObject obj, int which)
		{
			this.action(this.list[which]);
		}

		private string[] list;

		private UnityAction<string> action;
	}

	public delegate void Callback();
}

public class AndroidShell
{
    public static void RunCommand(string command, Action<string> complete, Action<string> error)
    {
        AndroidShell.RunCommand("sh", command, complete, error);
    }

    public static void RunCommand(string file, string command, Action<string> complete, Action<string> error)
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            return;
        }
        Process process = new Process();
        process.StartInfo.FileName = file;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        process.Start();
        process.StandardInput.WriteLine(command);
        process.StandardInput.Flush();
        process.StandardInput.Close();
        process.WaitForExit();
        string text = process.StandardOutput.ReadToEnd();
        if (!string.IsNullOrEmpty(text))
        {
            if (complete != null)
            {
                complete(text);
            }
            return;
        }
        text = string.Empty;
        text = process.StandardError.ReadToEnd();
        if (!string.IsNullOrEmpty(text) && error != null)
        {
            error(text);
        }
    }
}

public class DeviceInfo
{
    public string CODENAME;

    public string INCREMENTAL;

    public string RELEASE;

    public int SDK;
}

public class PackageInfo
{
    public long firstInstallTime;

    public long lastUpdateTime;

    public string packageName;

    public int versionCode;

    public string versionName;
}


public enum DialogInterface
{
    Positive = -1,
    Negative = -2,
    Neutral = -3
}
