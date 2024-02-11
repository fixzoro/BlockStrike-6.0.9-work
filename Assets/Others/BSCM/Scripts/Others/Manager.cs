using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace BSCM
{
	public class Manager
	{
        public static bool enabled = true;

        public static GameMode[] modes;

        public static int hash;

        public static string bundleUrl;

        private static AssetBundle bundle;

        private static Dictionary<GameMode, List<string>> scenesGameMode = new Dictionary<GameMode, List<string>>();

        private static string directoryPath = AndroidNativeFunctions.GetAbsolutePath() + "/Block Strike/Custom Maps";

        public static void Start()
		{
			if (!Manager.enabled)
			{
				return;
			}
			Manager.CreateDirectory();
			Manager.scenesGameMode = new Dictionary<GameMode, List<string>>();
			string[] files = Directory.GetFiles(Manager.directoryPath, "*.txt");
			string text = string.Empty;
			for (int i = 0; i < files.Length; i++)
			{
				if (Manager.CheckFileName(files[i]))
				{
					text = Manager.GetAssetBundlePath(files[i]);
					if (!string.IsNullOrEmpty(text))
					{
						if (new FileInfo(text).Length <= 6000000L)
						{
							GameMode[] bundleModesPath = Manager.GetBundleModesPath(files[i]);
							if (bundleModesPath.Length != 0)
							{
								for (int j = 0; j < bundleModesPath.Length; j++)
								{
									if (!Manager.scenesGameMode.ContainsKey(bundleModesPath[j]))
									{
										Manager.scenesGameMode.Add(bundleModesPath[j], new List<string>());
									}
									Manager.scenesGameMode[bundleModesPath[j]].Add(text);
								}
							}
						}
					}
				}
			}
		}
        
		private static bool CheckFileName(string path)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
			return !string.IsNullOrEmpty(fileNameWithoutExtension) && !LevelManager.HasScene(fileNameWithoutExtension);
		}
        
		private static string GetAssetBundlePath(string fileInfoPath)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfoPath);
			if (File.Exists(Manager.directoryPath + "/" + fileNameWithoutExtension + ".bscm"))
			{
				return Manager.directoryPath + "/" + fileNameWithoutExtension + ".bscm";
			}
			return string.Empty;
		}

		public static bool HasMaps()
		{
			return Manager.scenesGameMode.Count != 0;
		}

		public static string[] GetMapsList(GameMode mode)
		{
			if (!Manager.scenesGameMode.ContainsKey(mode))
			{
				return new string[0];
			}
			return Manager.scenesGameMode[mode].ToArray();
		}

		public static void LoadBundle(string path)
		{
			byte[] binary = File.ReadAllBytes(path);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
			Manager.modes = Manager.GetBundleModes(fileNameWithoutExtension);
			Manager.hash = Manager.GetBundleHash(fileNameWithoutExtension);
			Manager.bundleUrl = Manager.GetBundleUrl(fileNameWithoutExtension);
			Manager.UnloadBundle();
			Manager.bundle = AssetBundle.LoadFromMemory(binary);
		}

		public static void UnloadBundle()
		{
			if (Manager.bundle != null)
			{
				Manager.bundle.Unload(true);
			}
		}

		public static string GetBundleName(string path)
		{
			return Path.GetFileNameWithoutExtension(path);
		}

		public static string GetBundlePath(string bundleName)
		{
			if (File.Exists(Manager.directoryPath + "/" + bundleName + ".bscm"))
			{
				return Manager.directoryPath + "/" + bundleName + ".bscm";
			}
			return string.Empty;
		}

		public static string SaveBundle(string name, int[] modes, int hash, string url, byte[] map)
		{
			Manager.CreateDirectory();
			File.WriteAllBytes(Manager.directoryPath + "/" + name + ".bscm", map);
			StringBuilder stringBuilder = new StringBuilder();
			string text = string.Empty;
			for (int i = 0; i < modes.Length; i++)
			{
				if (i < modes.Length - 1)
				{
					text = text + modes[i] + ",";
				}
				else
				{
					text += modes[i].ToString();
				}
			}
			stringBuilder.AppendLine("mode=" + text);
			stringBuilder.AppendLine("hash=" + hash);
			stringBuilder.AppendLine("id=" + url);
			File.WriteAllText(Manager.directoryPath + "/" + name + ".txt", stringBuilder.ToString());
			return Manager.directoryPath + "/" + name + ".bscm";
		}

		public static bool DeleteBundle(string name)
		{
			if (File.Exists(Manager.directoryPath + "/" + name + ".bscm"))
			{
				File.Delete(Manager.directoryPath + "/" + name + ".bscm");
				File.Delete(Manager.directoryPath + "/" + name + ".txt");
				Manager.Start();
				return true;
			}
			return false;
		}

		private static void CreateDirectory()
		{
			if (!Directory.Exists(Manager.directoryPath))
			{
				Directory.CreateDirectory(Manager.directoryPath);
			}
		}

		public static int GetBundleHash(string bundleName)
		{
			string path = Manager.directoryPath + "/" + bundleName + ".txt";
			if (File.Exists(path))
			{
				try
				{
					string[] array = File.ReadAllText(path).Split(new char[]
					{
						"\n"[0]
					});
					array[1] = array[1].Replace("hash=", string.Empty);
					int result = 0;
					int.TryParse(array[1], out result);
					return result;
				}
				catch
				{
					return 0;
				}
				return 0;
			}
			return 0;
		}

		private static string GetBundleUrl(string bundleName)
		{
			string path = Manager.directoryPath + "/" + bundleName + ".txt";
			if (File.Exists(path))
			{
				try
				{
					string[] array = File.ReadAllText(path).Split(new char[]
					{
						"\n"[0]
					});
					return array[2].Replace("id=", string.Empty);
				}
				catch
				{
					return string.Empty;
				}
			}
			return string.Empty;
		}

		private static GameMode[] GetBundleModesPath(string bundlePath)
		{
			return Manager.GetBundleModes(Path.GetFileNameWithoutExtension(bundlePath));
		}

		private static GameMode[] GetBundleModes(string bundleName)
		{
			string path = Manager.directoryPath + "/" + bundleName + ".txt";
			List<GameMode> list = new List<GameMode>();
			try
			{
				string[] array = File.ReadAllText(path).Split(new char[]
				{
					"\n"[0]
				});
				array[0] = array[0].Replace("mode=", string.Empty);
				array = array[0].Split(new char[]
				{
					","[0]
				});
				int item = 0;
				for (int i = 0; i < array.Length; i++)
				{
					if (int.TryParse(array[i], out item))
					{
						list.Add((GameMode)item);
					}
				}
			}
			catch
			{
			}
			return list.ToArray();
		}
	}
}
