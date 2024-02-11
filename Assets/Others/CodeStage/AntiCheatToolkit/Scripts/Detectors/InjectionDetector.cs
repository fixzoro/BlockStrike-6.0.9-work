using System;
using System.IO;
using System.Reflection;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using UnityEngine.Events;

namespace CodeStage.AntiCheat.Detectors
{
	// Token: 0x020001B5 RID: 437
	[AddComponentMenu("Code Stage/Anti-Cheat Toolkit/Injection Detector")]
	public class InjectionDetector : ActDetectorBase
	{
		// Token: 0x06000FEA RID: 4074 RVA: 0x0000F3A4 File Offset: 0x0000D5A4
		private InjectionDetector()
		{
		}

		// Token: 0x06000FEB RID: 4075 RVA: 0x0000F3AC File Offset: 0x0000D5AC
		public static void StartDetection()
		{
			if (InjectionDetector.Instance != null)
			{
				InjectionDetector.Instance.StartDetectionInternal(null, null);
			}
			else
			{
				Debug.LogError("[ACTk] Injection Detector: can't be started since it doesn't exists in scene or not yet initialized!");
			}
		}

		// Token: 0x06000FEC RID: 4076 RVA: 0x0000F3D9 File Offset: 0x0000D5D9
		public static void StartDetection(UnityAction callback)
		{
			InjectionDetector.GetOrCreateInstance.StartDetectionInternal(callback, null);
		}

		// Token: 0x06000FED RID: 4077 RVA: 0x0000F3E7 File Offset: 0x0000D5E7
		public static void StartDetection(UnityAction<string> callback)
		{
			InjectionDetector.GetOrCreateInstance.StartDetectionInternal(null, callback);
		}

		// Token: 0x06000FEE RID: 4078 RVA: 0x0000F3F5 File Offset: 0x0000D5F5
		public static void StopDetection()
		{
			if (InjectionDetector.Instance != null)
			{
				InjectionDetector.Instance.StopDetectionInternal();
			}
		}

		// Token: 0x06000FEF RID: 4079 RVA: 0x0000F411 File Offset: 0x0000D611
		public static void Dispose()
		{
			if (InjectionDetector.Instance != null)
			{
				InjectionDetector.Instance.DisposeInternal();
			}
		}

		// Token: 0x170002FD RID: 765
		// (get) Token: 0x06000FF0 RID: 4080 RVA: 0x0000F42D File Offset: 0x0000D62D
		// (set) Token: 0x06000FF1 RID: 4081 RVA: 0x0000F434 File Offset: 0x0000D634
		public static InjectionDetector Instance { get; private set; }

		// Token: 0x170002FE RID: 766
		// (get) Token: 0x06000FF2 RID: 4082 RVA: 0x000723A0 File Offset: 0x000705A0
		private static InjectionDetector GetOrCreateInstance
		{
			get
			{
				if (InjectionDetector.Instance != null)
				{
					return InjectionDetector.Instance;
				}
				if (ActDetectorBase.detectorsContainer == null)
				{
					ActDetectorBase.detectorsContainer = new GameObject("Anti-Cheat Toolkit Detectors");
				}
				InjectionDetector.Instance = ActDetectorBase.detectorsContainer.AddComponent<InjectionDetector>();
				return InjectionDetector.Instance;
			}
		}

		// Token: 0x06000FF3 RID: 4083 RVA: 0x0000F43C File Offset: 0x0000D63C
		private void Awake()
		{
			InjectionDetector.instancesInScene++;
			if (this.Init(InjectionDetector.Instance, "Injection Detector"))
			{
				InjectionDetector.Instance = this;
			}
		}

		// Token: 0x06000FF4 RID: 4084 RVA: 0x0000F465 File Offset: 0x0000D665
		protected override void OnDestroy()
		{
			base.OnDestroy();
			InjectionDetector.instancesInScene--;
		}

		// Token: 0x06000FF5 RID: 4085 RVA: 0x0000F479 File Offset: 0x0000D679
		private void OnLevelWasLoaded()
		{
			this.OnLevelLoadedCallback();
		}

		// Token: 0x06000FF6 RID: 4086 RVA: 0x000723F8 File Offset: 0x000705F8
		private void OnLevelLoadedCallback()
		{
			if (InjectionDetector.instancesInScene < 2)
			{
				if (!this.keepAlive)
				{
					this.DisposeInternal();
				}
			}
			else if (!this.keepAlive && InjectionDetector.Instance != this)
			{
				this.DisposeInternal();
			}
		}

		// Token: 0x06000FF7 RID: 4087 RVA: 0x00072448 File Offset: 0x00070648
		private void StartDetectionInternal(UnityAction callback, UnityAction<string> callbackWithArgument)
		{
			if (this.isRunning)
			{
				Debug.LogWarning("[ACTk] Injection Detector: already running!", this);
				return;
			}
			if (!base.enabled)
			{
				Debug.LogWarning("[ACTk] Injection Detector: disabled but StartDetection still called from somewhere (see stack trace for this message)!", this);
				return;
			}
			if (callback != null && this.detectionEventHasListener)
			{
				Debug.LogWarning("[ACTk] Injection Detector: has properly configured Detection Event in the inspector, but still get started with Action callback. Both Action and Detection Event will be called on detection. Are you sure you wish to do this?", this);
			}
			if (callback == null && !this.detectionEventHasListener)
			{
				Debug.LogWarning("[ACTk] Injection Detector: was started without any callbacks. Please configure Detection Event in the inspector, or pass the callback Action to the StartDetection method.", this);
				base.enabled = false;
				return;
			}
			this.detectionAction = callback;
			this.detectionActionWithArgument = callbackWithArgument;
			this.started = true;
			this.isRunning = true;
			if (this.allowedAssemblies == null)
			{
				this.LoadAndParseAllowedAssemblies();
			}
			if (this.signaturesAreNotGenuine)
			{
				this.OnCheatingDetected("signatures");
				return;
			}
			string cause;
			if (!this.FindInjectionInCurrentAssemblies(out cause))
			{
				AppDomain.CurrentDomain.AssemblyLoad += this.OnNewAssemblyLoaded;
			}
			else
			{
				this.OnCheatingDetected(cause);
			}
		}

		// Token: 0x06000FF8 RID: 4088 RVA: 0x0000F481 File Offset: 0x0000D681
		protected override void StartDetectionAutomatically()
		{
			this.StartDetectionInternal(null, null);
		}

		// Token: 0x06000FF9 RID: 4089 RVA: 0x0000F48B File Offset: 0x0000D68B
		protected override void PauseDetector()
		{
			this.isRunning = false;
			AppDomain.CurrentDomain.AssemblyLoad -= this.OnNewAssemblyLoaded;
		}

		// Token: 0x06000FFA RID: 4090 RVA: 0x00072538 File Offset: 0x00070738
		protected override void ResumeDetector()
		{
			if (this.detectionAction == null && this.detectionActionWithArgument == null && !this.detectionEventHasListener)
			{
				return;
			}
			this.isRunning = true;
			AppDomain.CurrentDomain.AssemblyLoad += this.OnNewAssemblyLoaded;
		}

		// Token: 0x06000FFB RID: 4091 RVA: 0x0000F4AA File Offset: 0x0000D6AA
		protected override void StopDetectionInternal()
		{
			if (!this.started)
			{
				return;
			}
			AppDomain.CurrentDomain.AssemblyLoad -= this.OnNewAssemblyLoaded;
			this.detectionAction = null;
			this.detectionActionWithArgument = null;
			this.started = false;
			this.isRunning = false;
		}

		// Token: 0x06000FFC RID: 4092 RVA: 0x0000F4EA File Offset: 0x0000D6EA
		protected override void DisposeInternal()
		{
			base.DisposeInternal();
			if (InjectionDetector.Instance == this)
			{
				InjectionDetector.Instance = null;
			}
		}

		// Token: 0x06000FFD RID: 4093 RVA: 0x0000F508 File Offset: 0x0000D708
		private void OnCheatingDetected(string cause)
		{
			if (this.detectionActionWithArgument != null)
			{
				this.detectionActionWithArgument(cause);
			}
			base.OnCheatingDetected();
		}

		// Token: 0x06000FFE RID: 4094 RVA: 0x0000F527 File Offset: 0x0000D727
		private void OnNewAssemblyLoaded(object sender, AssemblyLoadEventArgs args)
		{
			if (!this.AssemblyAllowed(args.LoadedAssembly))
			{
				this.OnCheatingDetected(args.LoadedAssembly.FullName);
			}
		}

		// Token: 0x06000FFF RID: 4095 RVA: 0x00072584 File Offset: 0x00070784
		private bool FindInjectionInCurrentAssemblies(out string cause)
		{
			cause = null;
			bool result = false;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			if (assemblies.Length == 0)
			{
				cause = "no assemblies";
				result = true;
			}
			else
			{
				foreach (Assembly assembly in assemblies)
				{
					if (!this.AssemblyAllowed(assembly))
					{
						cause = assembly.FullName;
						result = true;
						break;
					}
				}
			}
			return result;
		}

		// Token: 0x06001000 RID: 4096 RVA: 0x000725F4 File Offset: 0x000707F4
		private bool AssemblyAllowed(Assembly ass)
		{
			string name = ass.GetName().Name;
			int assemblyHash = this.GetAssemblyHash(ass);
			bool result = false;
			for (int i = 0; i < this.allowedAssemblies.Length; i++)
			{
				InjectionDetector.AllowedAssembly allowedAssembly = this.allowedAssemblies[i];
				if (allowedAssembly.name == name && Array.IndexOf<int>(allowedAssembly.hashes, assemblyHash) != -1)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		// Token: 0x06001001 RID: 4097 RVA: 0x00072668 File Offset: 0x00070868
		private void LoadAndParseAllowedAssemblies()
		{
			TextAsset textAsset = (TextAsset)Resources.Load("fndid", typeof(TextAsset));
			if (textAsset == null)
			{
				this.signaturesAreNotGenuine = true;
				return;
			}
			string[] separator = new string[]
			{
				":"
			};
			MemoryStream memoryStream = new MemoryStream(textAsset.bytes);
			BinaryReader binaryReader = new BinaryReader(memoryStream);
			int num = binaryReader.ReadInt32();
			this.allowedAssemblies = new InjectionDetector.AllowedAssembly[num];
			for (int i = 0; i < num; i++)
			{
				string text = binaryReader.ReadString();
				text = ObscuredString.EncryptDecrypt(text, "Elina");
				string[] array = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				int num2 = array.Length;
				if (num2 <= 1)
				{
					this.signaturesAreNotGenuine = true;
					binaryReader.Close();
					memoryStream.Close();
					return;
				}
				string name = array[0];
				int[] array2 = new int[num2 - 1];
				for (int j = 1; j < num2; j++)
				{
					array2[j - 1] = int.Parse(array[j]);
				}
				this.allowedAssemblies[i] = new InjectionDetector.AllowedAssembly(name, array2);
			}
			binaryReader.Close();
			memoryStream.Close();
			Resources.UnloadAsset(textAsset);
			this.hexTable = new string[256];
			for (int k = 0; k < 256; k++)
			{
				this.hexTable[k] = k.ToString("x2");
			}
		}

		// Token: 0x06001002 RID: 4098 RVA: 0x000727D8 File Offset: 0x000709D8
		private int GetAssemblyHash(Assembly ass)
		{
			AssemblyName name = ass.GetName();
			byte[] publicKeyToken = name.GetPublicKeyToken();
			string text;
			if (publicKeyToken.Length >= 8)
			{
				text = name.Name + this.PublicKeyTokenToString(publicKeyToken);
			}
			else
			{
				text = name.Name;
			}
			int num = 0;
			int length = text.Length;
			for (int i = 0; i < length; i++)
			{
				num += (int)text[i];
				num += num << 10;
				num ^= num >> 6;
			}
			num += num << 3;
			num ^= num >> 11;
			return num + (num << 15);
		}

		// Token: 0x06001003 RID: 4099 RVA: 0x0007286C File Offset: 0x00070A6C
		private string PublicKeyTokenToString(byte[] bytes)
		{
			string text = string.Empty;
			for (int i = 0; i < 8; i++)
			{
				text += this.hexTable[(int)bytes[i]];
			}
			return text;
		}

		// Token: 0x04000B05 RID: 2821
		internal const string COMPONENT_NAME = "Injection Detector";

		// Token: 0x04000B06 RID: 2822
		internal const string FINAL_LOG_PREFIX = "[ACTk] Injection Detector: ";

		// Token: 0x04000B07 RID: 2823
		protected UnityAction<string> detectionActionWithArgument;

		// Token: 0x04000B08 RID: 2824
		private static int instancesInScene;

		// Token: 0x04000B09 RID: 2825
		private bool signaturesAreNotGenuine;

		// Token: 0x04000B0A RID: 2826
		private InjectionDetector.AllowedAssembly[] allowedAssemblies;

		// Token: 0x04000B0B RID: 2827
		private string[] hexTable;

		// Token: 0x020001B6 RID: 438
		private class AllowedAssembly
		{
			// Token: 0x06001004 RID: 4100 RVA: 0x0000F54B File Offset: 0x0000D74B
			public AllowedAssembly(string name, int[] hashes)
			{
				this.name = name;
				this.hashes = hashes;
			}

			// Token: 0x04000B0D RID: 2829
			public readonly string name;

			// Token: 0x04000B0E RID: 2830
			public readonly int[] hashes;
		}
	}
}
