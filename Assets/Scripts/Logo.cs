using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using FreeJSON;
using UnityEngine;

public class Logo : MonoBehaviour
{
    public UILabel percentLabel;

    public List<CryptoString> appList;

    public CryptoString[] Permissions;

    private void Start()
	{
        base.StartCoroutine(this.Check());
    }

    private IEnumerator Check()
	{
        #if UNITY_EDITOR || UNITY_STANDALONE_WIN
        Settings.Load();
        Screen.sleepTimeout = -1;
        yield return new WaitForSeconds(0.4f);
        this.percentLabel.text = "0%";
        yield return new WaitForSeconds(0.4f);
        this.percentLabel.text = "50%";
        yield return new WaitForSeconds(0.4f);
        this.percentLabel.text = "90%";
        yield return new WaitForSeconds(0.4f);
        this.percentLabel.text = "100%";
        GameConsole.actived = Settings.Console;
        LevelManager.LoadLevel("GDPR");
        yield break;
        #else
        Settings.Load();
		AndroidNativeFunctions.ImmersiveMode();
		Screen.sleepTimeout = -1;
		yield return new WaitForSeconds(0.3f);
		this.percentLabel.text = "0%";
        if (this.CheckPermissions())
        {
            base.StopAllCoroutines();
        }
        yield return new WaitForSeconds(0.3f);
		this.percentLabel.text = "50%";
        if (this.CheckGame())
        {
            base.StopAllCoroutines();
        }
        yield return new WaitForSeconds(0.3f);
		this.percentLabel.text = "90%";
        if (this.CheckGame2())
        {
            base.StopAllCoroutines();
        }
        yield return new WaitForSeconds(0.3f);
		this.percentLabel.text = "100%";
		GameConsole.actived = Settings.Console;
		LevelManager.LoadLevel("GDPR");
		yield break;
        #endif
	}
    
	private bool CheckPermissions()
	{
		for (int i = 0; i < this.Permissions.Length; i++)
		{
			if (!AndroidNativeFunctions.CheckPermission("android.permission." + this.Permissions[i]))
			{
				AndroidNativeFunctions.HideProgressDialog();
				AndroidNativeFunctions.ShowAlert("For the game to work need permission: " + this.Permissions[i], "Block Strike", "OK", string.Empty, string.Empty, delegate(DialogInterface r)
				{
					Application.Quit();
				});
				return true;
			}
		}
		return false;
	}
    
	private bool CheckGame()
	{
		if (File.Exists(AndroidNativeFunctions.GetAbsolutePath() + "/Android/data/com.android.browser/files/8gh47gj37gnn4xz"))
		{
			this.Detected("Device Error");
			return true;
		}
        if (!Directory.Exists("/system") || !Directory.Exists("/data"))
		{
			this.Detected("Device Read Error");
			return true;
		}
		this.percentLabel.text = "10%";
		string directoryName = Path.GetDirectoryName(Application.dataPath);
		if (Directory.Exists(directoryName + "/arm") || Directory.Exists(directoryName + "/x86") || File.Exists(directoryName + "/" + Path.GetFileNameWithoutExtension(Application.dataPath) + ".odex"))
		{
			AndroidNativeFunctions.ShowAlert("Files corrupted, please reinstall the game.", "Block Strike", "OK", string.Empty, string.Empty, delegate(DialogInterface r)
			{
				Application.Quit();
			});
			return true;
		}
		if (Process.GetProcessesByName("parallel.monitor").Length != 0 || Process.GetProcessesByName("libuninstmon.so").Length != 0)
		{
			this.Detected("Parallel detected");
			return true;
		}
		if (File.Exists("/system/lib/libxposed_art.so") || File.Exists("/system/bin/app_process32_xposed") || File.Exists("/system/framework/XposedBridge.jar") || File.Exists("/system/xposed.prop") || File.Exists("/data/dalvik-cache/xposed_XResourcesSuperClass.dex") || File.Exists("/data/dalvik-cache/xposed_XTypedArraySuperClass.dex") || File.Exists("data/lp/xposed") || File.Exists("/data/dalvik-cache/profiles/de.robv.android.xposed.installer") || Directory.Exists("data/data/de.robv.android.xposed.installer") || Directory.Exists("data/data/com.android.z"))
		{
			this.Detected("Xposed Detected");
			return true;
		}
		string text = File.ReadAllText("/proc/" + Process.GetCurrentProcess().Id + "/maps");
		if (text.Contains("xposed") || text.Contains("libxposed_art") || text.Contains("XposedBridge"))
		{
			this.Detected("Xposed_detected");
			return true;
		}
		this.percentLabel.text = "35%";
        string[] installedApps = AndroidNativeFunctions.GetInstalledApps3();
        JsonArray jsonArray = new JsonArray();
        JsonArray jsonArray2 = new JsonArray();
        if (CryptoPrefs.HasKey("CheckApps2"))
        {
            jsonArray2 = JsonArray.Parse(CryptoPrefs.GetString("CheckApps2"));
        }
        int num = int.Parse("25000000");
        for (int i = 0; i < installedApps.Length; i++)
        {
            if (File.Exists(installedApps[i]))
            {
                long length = new FileInfo(installedApps[i]).Length;
                if (jsonArray2.Contains(Utils.MD5(length.ToString() + installedApps[i])))
                {
                    jsonArray.Add(Utils.MD5(length.ToString() + installedApps[i]));
                }
                else if (length < (long)num)
                {
                    
                    jsonArray.Add(Utils.MD5(length.ToString() + installedApps[i]));
                }
            }
            else if (!installedApps[i].Contains("MMIGroup") && !installedApps[i].Contains("cust/hw/"))
            {
                this.Detected("No Find App: " + installedApps[i]);
                return true;
            }
        }
        if (jsonArray.Length != 0)
        {
            CryptoPrefs.SetString("CheckApps2", jsonArray.ToString());
        }
        string[] array = File.ReadAllLines("/proc/" + Process.GetCurrentProcess().Id + "/maps");
		bool flag = false;
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j].Contains("data") && (array[j].Contains("virtual") || array[j].Contains("parallel") || array[j].Contains("cloneapp") || array[j].Contains("clonemaster") || array[j].Contains("dualspace") || array[j].Contains("multiaccount")))
			{
				this.Detected("Virtual Detected", array[j]);
				return true;
			}
			if (!flag && array[j].Contains("clone"))
			{
				flag = true;
			}
		}
		return false;
	}
    
	private bool CheckApplicationInfo()
	{
		return true;
	}
    
	private bool CheckGame2()
	{
		string @string = CryptoPrefs.GetString("GG-Name");
		string[] array = new string[0];
		array = AndroidNativeFunctions.GetInstalledApps2();
		if (array.Length < 20)
		{
			this.Detected("Error Device App");
			return true;
		}
		if (AndroidNativeFunctions.GetFilesDir().Contains("999"))
		{
			this.Detected("Error Device App 2");
			return true;
		}
		if (!string.IsNullOrEmpty(@string))
		{
			this.Detected("Game Guardian detected");
			return true;
		}
		if (Process.GetProcessesByName("su").Length != 0 && Process.GetProcessesByName("kworker").Length != 0)
		{
			this.Detected("Game Guardian detected");
			return true;
		}
		this.percentLabel.text = "60%";
		try
		{
			Process[] processesByName = Process.GetProcessesByName("su");
			for (int i = 0; i < processesByName.Length; i++)
			{
				string text = File.ReadAllLines("proc/" + processesByName[i].Id + "/status")[4];
				text = text.Replace(" ", string.Empty);
				text = text.Replace("\t", string.Empty);
				text = text.ToLower().Replace("ppid:", string.Empty);
				string text2 = File.ReadAllLines("proc/" + text + "/cmdline")[0];
				if (text2 != "su")
				{
					string sourceDir = AndroidNativeFunctions.GetSourceDir(text2);
					if (!string.IsNullOrEmpty(sourceDir) && File.Exists(sourceDir))
					{
						base.StartCoroutine(this.Check(sourceDir + "!/res/raw/chunk8", text2));
					}
				}
			}
		}
		catch
		{

		}
		try
		{
			Process[] processes = Process.GetProcesses();
			for (int j = 0; j < processes.Length; j++)
			{
				if (processes[j].ProcessName.Contains("/files/eventserver"))
				{
					this.Detected("RepetiTouch Detected");
					return true;
				}
			}
		}
		catch
		{

		}
		try
		{
			Process[] processesByName2 = Process.GetProcessesByName("kworker/0:0");
			for (int k = 0; k < processesByName2.Length; k++)
			{
				string text3 = File.ReadAllText("proc/" + processesByName2[k].Id + "/cmdline");
				if (text3.Contains("data"))
				{
					string[] array2 = text3.Split(new char[]
					{
						"/"[0]
					});
					if (Process.GetProcessesByName(array2[4]).Length != 0)
					{
						CryptoPrefs.SetString("GG-Name", array2[4]);
						this.Detected("Game Guardian detected");
						return true;
					}
				}
			}
		}
		catch
		{
		}
		this.percentLabel.text = "70%";
		for (int l = 0; l < array.Length; l++)
		{
			if (this.appList.Contains(array[l]))
			{
				this.Detected(AndroidNativeFunctions.GetAppName(array[l]) + " detected", array[l]);
				return true;
			}
			if (File.Exists("data/data/" + array[l] + "/files/lib1.so") || File.Exists("data/data/" + array[l] + "/files/lib2.so") || File.Exists("data/data/" + array[l] + "/files/lib5.so") || File.Exists("data/data/" + array[l] + "/files/lib8.so"))
			{
				this.Detected("Game Guardian detected");
				return true;
			}
			if ((this.FileExists(array[l], "lib0.so") && this.FileExists(array[l], "lib1.so")) || (this.FileExists(array[l], "lib2.so") && this.FileExists(array[l], "lib3.so")) || this.FileExists(array[l], "lib4.so") || this.FileExists(array[l], "lib7.so") || this.FileExists(array[l], "lib8.so"))
			{
				this.Detected("Game Guardian detected");
				return true;
			}
			if (File.Exists("data/data/" + array[l] + "/lib/libxigncode.so"))
			{
				List<int> list = new List<int>();
				list.Add(67040);
				list.Add(91384);
				list.Add(95488);
				list.Add(107668);
				list.Add(75424);
				int item = (int)new FileInfo("data/data/" + array[l] + "/lib/libxigncode.so").Length;
				if (list.Contains(item))
				{
					this.Detected(Utils.XOR("Game Guardian detected"));
					return true;
				}
			}
			else
			{
				if (this.FileExists(array[l], "libgg_time_32.so") || this.FileExists(array[l], "libgg_tibe.so"))
				{
					this.Detected("Game Guardian detected");
					return true;
				}
				if (this.FileExists(array[l], "libgg_time.so"))
				{
					this.Detected("Game Guardian detected", array[l]);
					return true;
				}
				if (this.FileExists(array[l], "lib-bulldog-daemon-exe.so"))
				{
					this.Detected("Game Guardian detected", array[l]);
					return true;
				}
				if (File.Exists(Utils.XOR("S/AINP8GgZsuag==") + array[l] + "/files/version.gg"))
				{
					this.Detected("Game Guardian detected");
					return true;
				}
				if (this.FileExists(array[l], "libgltools.so") || this.FileExists(array[l], "libjustuseless.so"))
				{
					this.Detected(AndroidNativeFunctions.GetAppName(array[l]) + " detected", array[l]);
					return true;
				}
				if (this.FileExists(array[l], "liblbs.so") && this.FileExists(array[l], "libmarsxlog.so"))
				{
					this.Detected(AndroidNativeFunctions.GetAppName(array[l]) + " detected", array[l]);
					return true;
				}
				if (this.FileExists(array[l], "libexec.so"))
				{
					if (!this.FileExists(array[l], "libunrar.so") && !File.Exists("data/data/" + array[l] + "/proxy.sh") && !File.Exists("data/data/" + array[l] + "/iptables"))
					{
						this.Detected(AndroidNativeFunctions.GetAppName(array[l]) + " detected", array[l]);
						return true;
					}
				}
				else
				{
					if (this.FileExists(array[l], "libencode.so"))
					{
						this.Detected(AndroidNativeFunctions.GetAppName(array[l]) + " detected", array[l]);
						return true;
					}
					if (this.FileExists(array[l], "libgamekiller.so"))
					{
						this.Detected(AndroidNativeFunctions.GetAppName(array[l]) + " detected", array[l]);
						return true;
					}
					if (this.FileExists(array[l], "libNative.so"))
					{
						if (File.Exists("data/data/" + array[l] + "/files/libNative.so"))
						{
							this.Detected(AndroidNativeFunctions.GetAppName(array[l]) + " detected", array[l]);
							return true;
						}
					}
					else
					{
						if (this.FileExists(array[l], "libcecore.so"))
						{
							this.Detected(AndroidNativeFunctions.GetAppName(array[l]) + " detected", array[l]);
							return true;
						}
						if (this.FileExists(array[l], "libipadjust.so"))
						{
							this.Detected(AndroidNativeFunctions.GetAppName(array[l]) + " detected", array[l]);
							return true;
						}
						if (this.FileExists(array[l], "libMacro.so"))
						{
							this.Detected(AndroidNativeFunctions.GetAppName(array[l]) + " detected", array[l]);
							return true;
						}
						if (Directory.Exists("data/data/" + array[l] + "/parallel_intl") || Directory.Exists("data/data/" + array[l] + "/parallel_lite") || Directory.Exists("data/data/" + array[l] + "/cache/com.phantom"))
						{
							this.Detected(AndroidNativeFunctions.GetAppName(array[l]) + " detected", array[l]);
							return true;
						}
						if (this.FileExists(array[l], "libmhx.so") || this.FileExists(array[l], "libbig-stuff.so"))
						{
							this.Detected(AndroidNativeFunctions.GetAppName(array[l]) + " detected", array[l]);
							return true;
						}
						if (this.FileExists(array[l], "libdummysprite.so"))
						{
							this.Detected(AndroidNativeFunctions.GetAppName(array[l]) + " detected", array[l]);
							return true;
						}
						if (Directory.Exists("data/data/" + array[l] + "/virtual"))
						{
							this.Detected(AndroidNativeFunctions.GetAppName(array[l]) + " detected", array[l]);
						}
						else if (Directory.Exists("data/data/" + array[l] + "/parallel_intl"))
						{
							this.Detected(AndroidNativeFunctions.GetAppName(array[l]) + " detected", array[l]);
						}
					}
				}
			}
		}
		return false;
	}
    
	private void Detected(string text)
	{
		this.percentLabel.text = "error";
		CheckManager.Detected(text);
	}
    
	private void Detected(string text, string log)
	{
		this.percentLabel.text = "error";
		CheckManager.Detected(text, log);
	}
    
	private IEnumerator Check(string path, string id)
	{
		WWW www = new WWW("jar:file://" + path);
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			CryptoPrefs.SetString("GG-Name", id);
			this.Detected(Utils.XOR("Game Guardian detected"));
		}
		www.Dispose();
		yield break;
	}
    
	private bool FileExists(string t1, string t2)
	{
		return File.Exists("data/data/" + t1 + "/lib/" + t2) || File.Exists("system/lib/" + t2) || File.Exists("system/priv-app/" + t1 + "/lib/arm/" + t2) || File.Exists("system/priv-app/" + t1 + "/lib/x86/" + t2);
	}
    
	private bool CheckGame3()
	{
		string[] array = new string[]
		{
            "DOTween.dll",
            "DOTween43.dll",
            "DOTween46.dll",
            "KDTree.dll",
            "Mono.Security.dll",
            "mscorlib.dll",
            "P31RestKit.dll",
            "Photon3Unity3D.dll",
            "ProBuilderCore-Unity4.dll",
            "ProBuilderMeshOps-Unity4.dll",
            "System.Core.dll",
            "System.dll",
            "UnityEngine.dll",
            "UnityEngine.UI.dll"
        };
		string[] array2 = new string[]
		{
            "145408",
            "9728",
            "20480",
            "14336",
            "293376",
            "2496512",
            "58880",
            "159232",
            "176128",
            "94208",
            "268288",
            "1069568",
            "668672",
            "176128"
        };
		List<string> list = new List<string>
		{
            "Assembly-CSharp",
            "DOTween",
            "DOTween43",
            "DOTween46",
            "KDTree",
            "Mono.Security",
            "mscorlib",
            "P31RestKit",
            "Photon3Unity3D",
            "ProBuilderCore-Unity4",
            "ProBuilderMeshOps-Unity4",
            "System.Core",
            "System",
            "UnityEngine",
            "UnityEngine.UI"
        };
		
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		if (assemblies.Length.ToString() != "14")
		{
			this.Detected("Error 6322");
			return true;
		}
		for (int j = 0; j < assemblies.Length; j++)
		{
			if (!list.Contains(assemblies[j].GetName().Name))
			{
				this.Detected("Error 1912");
				return true;
			}
		}
		return false;
	}
}
