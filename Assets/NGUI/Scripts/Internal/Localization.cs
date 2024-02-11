using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200009A RID: 154
public static class Localization
{
	// Token: 0x17000067 RID: 103
	// (get) Token: 0x0600045B RID: 1115 RVA: 0x00007DCD File Offset: 0x00005FCD
	// (set) Token: 0x0600045C RID: 1116 RVA: 0x00007DF4 File Offset: 0x00005FF4
	public static Dictionary<string, string[]> dictionary
	{
		get
		{
			if (!Localization.localizationHasBeenSet)
			{
				Localization.LoadDictionary(PlayerPrefs.GetString("Language", "English"), false);
			}
			return Localization.mDictionary;
		}
		set
		{
			Localization.localizationHasBeenSet = (value != null);
			Localization.mDictionary = value;
		}
	}

	// Token: 0x17000068 RID: 104
	// (get) Token: 0x0600045D RID: 1117 RVA: 0x00007E08 File Offset: 0x00006008
	public static string[] knownLanguages
	{
		get
		{
			if (!Localization.localizationHasBeenSet)
			{
				Localization.LoadDictionary(PlayerPrefs.GetString("Language", "English"), false);
			}
			return Localization.mLanguages;
		}
	}

	// Token: 0x17000069 RID: 105
	// (get) Token: 0x0600045E RID: 1118 RVA: 0x00007E2F File Offset: 0x0000602F
	// (set) Token: 0x0600045F RID: 1119 RVA: 0x00007E64 File Offset: 0x00006064
	public static string language
	{
		get
		{
            if (string.IsNullOrEmpty(Localization.mLanguage))
			{
				Localization.mLanguage = PlayerPrefs.GetString("Language", "English");
				Localization.LoadAndSelect(Localization.mLanguage);
			}
			return Localization.mLanguage;
		}
		set
		{
			if (Localization.mLanguage != value)
			{
				Localization.mLanguage = value;
				Localization.LoadAndSelect(value);
			}
		}
	}

	// Token: 0x06000460 RID: 1120 RVA: 0x00007E83 File Offset: 0x00006083
	public static bool Reload()
	{
		Localization.localizationHasBeenSet = false;
		if (!Localization.LoadDictionary(Localization.mLanguage, true))
		{
			return false;
		}
		if (Localization.onLocalize != null)
		{
			Localization.onLocalize();
		}
		return true;
	}

	// Token: 0x06000461 RID: 1121 RVA: 0x00032F38 File Offset: 0x00031138
	private static bool LoadDictionary(string value, bool merge = false)
	{
		byte[] array = null;
		if (!Localization.localizationHasBeenSet)
		{
			if (Localization.loadFunction == null)
			{
				TextAsset textAsset = Resources.Load<TextAsset>("Localization/Localization");
				if (textAsset != null)
				{
					array = textAsset.bytes;
				}
			}
			else
			{
				array = Localization.loadFunction("Localization");
			}
			Localization.localizationHasBeenSet = true;
		}
		if (Localization.LoadCSV(array, merge))
		{
			return true;
		}
		if (string.IsNullOrEmpty(value))
		{
			value = Localization.mLanguage;
		}
		if (string.IsNullOrEmpty(value))
		{
			return false;
		}
		if (Localization.loadFunction == null)
		{
			TextAsset textAsset2 = Resources.Load<TextAsset>(value);
			if (textAsset2 != null)
			{
				array = textAsset2.bytes;
			}
		}
		else
		{
			array = Localization.loadFunction(value);
		}
		if (array != null)
		{
			Localization.Set(value, array);
			return true;
		}
		return false;
	}

	// Token: 0x06000462 RID: 1122 RVA: 0x00033008 File Offset: 0x00031208
	private static bool LoadAndSelect(string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			if (Localization.mDictionary.Count == 0 && !Localization.LoadDictionary(value, false))
			{
				return false;
			}
			if (Localization.SelectLanguage(value))
			{
				return true;
			}
		}
		if (Localization.mOldDictionary.Count > 0)
		{
			return true;
		}
		Localization.mOldDictionary.Clear();
		Localization.mDictionary.Clear();
		if (string.IsNullOrEmpty(value))
		{
			PlayerPrefs.DeleteKey("Language");
		}
		return false;
	}

	// Token: 0x06000463 RID: 1123 RVA: 0x00033088 File Offset: 0x00031288
	public static void Load(TextAsset asset)
	{
		ByteReader byteReader = new ByteReader(asset);
		Localization.Set(asset.name, byteReader.ReadDictionary());
	}

	// Token: 0x06000464 RID: 1124 RVA: 0x000330B0 File Offset: 0x000312B0
	public static void Set(string languageName, byte[] bytes)
	{
		ByteReader byteReader = new ByteReader(bytes);
		Localization.Set(languageName, byteReader.ReadDictionary());
	}

	// Token: 0x06000465 RID: 1125 RVA: 0x00007EB2 File Offset: 0x000060B2
	public static void ReplaceKey(string key, string val)
	{
		if (!string.IsNullOrEmpty(val))
		{
			Localization.mReplacement[key] = val;
		}
		else
		{
			Localization.mReplacement.Remove(key);
		}
	}

	// Token: 0x06000466 RID: 1126 RVA: 0x00007EDC File Offset: 0x000060DC
	public static void ClearReplacements()
	{
		Localization.mReplacement.Clear();
	}

	// Token: 0x06000467 RID: 1127 RVA: 0x00007EE8 File Offset: 0x000060E8
	public static bool LoadCSV(TextAsset asset, bool merge = false)
	{
		return Localization.LoadCSV(asset.bytes, asset, merge);
	}

	// Token: 0x06000468 RID: 1128 RVA: 0x00007EF7 File Offset: 0x000060F7
	public static bool LoadCSV(byte[] bytes, bool merge = false)
	{
		return Localization.LoadCSV(bytes, null, merge);
	}

	// Token: 0x06000469 RID: 1129 RVA: 0x000330D0 File Offset: 0x000312D0
	private static bool HasLanguage(string languageName)
	{
		int i = 0;
		int num = Localization.mLanguages.Length;
		while (i < num)
		{
			if (Localization.mLanguages[i] == languageName)
			{
				return true;
			}
			i++;
		}
		return false;
	}

	// Token: 0x0600046A RID: 1130 RVA: 0x0003310C File Offset: 0x0003130C
	private static bool LoadCSV(byte[] bytes, TextAsset asset, bool merge = false)
	{
		if (bytes == null)
		{
			return false;
		}
		ByteReader byteReader = new ByteReader(bytes);
		BetterList<string> betterList = byteReader.ReadCSV();
		if (betterList.size < 2)
		{
			return false;
		}
		betterList.RemoveAt(0);
		string[] array = null;
		if (string.IsNullOrEmpty(Localization.mLanguage))
		{
			Localization.localizationHasBeenSet = false;
		}
		if (!Localization.localizationHasBeenSet || (!merge && !Localization.mMerging) || Localization.mLanguages == null || Localization.mLanguages.Length == 0)
		{
			Localization.mDictionary.Clear();
			Localization.mLanguages = new string[betterList.size];
			if (!Localization.localizationHasBeenSet)
			{
				Localization.mLanguage = PlayerPrefs.GetString("Language", betterList[0]);
				Localization.localizationHasBeenSet = true;
			}
			for (int i = 0; i < betterList.size; i++)
			{
				Localization.mLanguages[i] = betterList[i];
				if (Localization.mLanguages[i] == Localization.mLanguage)
				{
					Localization.mLanguageIndex = i;
				}
			}
		}
		else
		{
			array = new string[betterList.size];
			for (int j = 0; j < betterList.size; j++)
			{
				array[j] = betterList[j];
			}
			for (int k = 0; k < betterList.size; k++)
			{
				if (!Localization.HasLanguage(betterList[k]))
				{
					int num = Localization.mLanguages.Length + 1;
					Array.Resize<string>(ref Localization.mLanguages, num);
					Localization.mLanguages[num - 1] = betterList[k];
					Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
					foreach (KeyValuePair<string, string[]> keyValuePair in Localization.mDictionary)
					{
						string[] value = keyValuePair.Value;
						Array.Resize<string>(ref value, num);
						value[num - 1] = value[0];
						dictionary.Add(keyValuePair.Key, value);
					}
					Localization.mDictionary = dictionary;
				}
			}
		}
		Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
		for (int l = 0; l < Localization.mLanguages.Length; l++)
		{
			dictionary2.Add(Localization.mLanguages[l], l);
		}
		for (;;)
		{
			BetterList<string> betterList2 = byteReader.ReadCSV();
			if (betterList2 == null || betterList2.size == 0)
			{
				break;
			}
			if (!string.IsNullOrEmpty(betterList2[0]))
			{
				Localization.AddCSV(betterList2, array, dictionary2);
			}
		}
		if (!Localization.mMerging && Localization.onLocalize != null)
		{
			Localization.mMerging = true;
			Localization.OnLocalizeNotification onLocalizeNotification = Localization.onLocalize;
			Localization.onLocalize = null;
			onLocalizeNotification();
			Localization.onLocalize = onLocalizeNotification;
			Localization.mMerging = false;
		}
		if (merge && Localization.onLocalize != null)
		{
			Localization.onLocalize();
		}
		return true;
	}

	// Token: 0x0600046B RID: 1131 RVA: 0x000333F4 File Offset: 0x000315F4
	private static void AddCSV(BetterList<string> newValues, string[] newLanguages, Dictionary<string, int> languageIndices)
	{
		if (newValues.size < 2)
		{
			return;
		}
		string text = newValues[0];
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		string[] value = Localization.ExtractStrings(newValues, newLanguages, languageIndices);
		if (Localization.mDictionary.ContainsKey(text))
		{
			Localization.mDictionary[text] = value;
			if (newLanguages == null)
			{
				Debug.LogWarning("Localization key '" + text + "' is already present");
			}
		}
		else
		{
			try
			{
				Localization.mDictionary.Add(text, value);
			}
			catch (Exception ex)
			{
				Debug.LogError("Unable to add '" + text + "' to the Localization dictionary.\n" + ex.Message);
			}
		}
	}

	// Token: 0x0600046C RID: 1132 RVA: 0x000334AC File Offset: 0x000316AC
	private static string[] ExtractStrings(BetterList<string> added, string[] newLanguages, Dictionary<string, int> languageIndices)
	{
		if (newLanguages == null)
		{
			string[] array = new string[Localization.mLanguages.Length];
			int i = 1;
			int num = Mathf.Min(added.size, array.Length + 1);
			while (i < num)
			{
				array[i - 1] = added[i];
				i++;
			}
			return array;
		}
		string key = added[0];
		string[] array2;
		if (!Localization.mDictionary.TryGetValue(key, out array2))
		{
			array2 = new string[Localization.mLanguages.Length];
		}
		int j = 0;
		int num2 = newLanguages.Length;
		while (j < num2)
		{
			string key2 = newLanguages[j];
			int num3 = languageIndices[key2];
			array2[num3] = added[j + 1];
			j++;
		}
		return array2;
	}

	// Token: 0x0600046D RID: 1133 RVA: 0x00033564 File Offset: 0x00031764
	private static bool SelectLanguage(string language)
	{
		Localization.mLanguageIndex = -1;
		if (Localization.mDictionary.Count == 0)
		{
			return false;
		}
		int i = 0;
		int num = Localization.mLanguages.Length;
		while (i < num)
		{
			if (Localization.mLanguages[i] == language)
			{
				Localization.mOldDictionary.Clear();
				Localization.mLanguageIndex = i;
				Localization.mLanguage = language;
				PlayerPrefs.SetString("Language", Localization.mLanguage);
				if (Localization.onLocalize != null)
				{
					Localization.onLocalize();
				}
				return true;
			}
			i++;
		}
		return false;
	}

	// Token: 0x0600046E RID: 1134 RVA: 0x000335F0 File Offset: 0x000317F0
	public static void Set(string languageName, Dictionary<string, string> dictionary)
	{
		Localization.mLanguage = languageName;
		PlayerPrefs.SetString("Language", Localization.mLanguage);
		Localization.mOldDictionary = dictionary;
		Localization.localizationHasBeenSet = true;
		Localization.mLanguageIndex = -1;
		Localization.mLanguages = new string[]
		{
			languageName
		};
		if (Localization.onLocalize != null)
		{
			Localization.onLocalize();
		}
	}

	// Token: 0x0600046F RID: 1135 RVA: 0x00007F01 File Offset: 0x00006101
	public static void Set(string key, string value)
	{
		if (Localization.mOldDictionary.ContainsKey(key))
		{
			Localization.mOldDictionary[key] = value;
		}
		else
		{
			Localization.mOldDictionary.Add(key, value);
		}
	}

	// Token: 0x06000470 RID: 1136 RVA: 0x00033648 File Offset: 0x00031848
	public static bool Has(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return false;
		}
		if (!Localization.localizationHasBeenSet)
		{
			Localization.LoadDictionary(PlayerPrefs.GetString("Language", "English"), false);
		}
		if (Localization.mLanguages == null)
		{
			return false;
		}
		string language = Localization.language;
		if (Localization.mLanguageIndex == -1)
		{
			for (int i = 0; i < Localization.mLanguages.Length; i++)
			{
				if (Localization.mLanguages[i] == language)
				{
					Localization.mLanguageIndex = i;
					break;
				}
			}
		}
		if (Localization.mLanguageIndex == -1)
		{
			Localization.mLanguageIndex = 0;
			Localization.mLanguage = Localization.mLanguages[0];
		}
		UICamera.ControlScheme currentScheme = UICamera.currentScheme;
		if (currentScheme == UICamera.ControlScheme.Touch)
		{
			string key2 = key + " Mobile";
			if (Localization.mReplacement.ContainsKey(key2))
			{
				return true;
			}
			if (Localization.mLanguageIndex != -1 && Localization.mDictionary.ContainsKey(key2))
			{
				return true;
			}
			if (Localization.mOldDictionary.ContainsKey(key2))
			{
				return true;
			}
		}
		else if (currentScheme == UICamera.ControlScheme.Controller)
		{
			string key3 = key + " Controller";
			if (Localization.mReplacement.ContainsKey(key3))
			{
				return true;
			}
			if (Localization.mLanguageIndex != -1 && Localization.mDictionary.ContainsKey(key3))
			{
				return true;
			}
			if (Localization.mOldDictionary.ContainsKey(key3))
			{
				return true;
			}
		}
		return Localization.mReplacement.ContainsKey(key) || (Localization.mLanguageIndex != -1 && Localization.mDictionary.ContainsKey(key)) || Localization.mOldDictionary.ContainsKey(key);
	}

	// Token: 0x06000471 RID: 1137 RVA: 0x000337EC File Offset: 0x000319EC
	public static string Get(string key, bool warnIfMissing = true)
	{
		if (string.IsNullOrEmpty(key))
		{
			return null;
		}
		if (!Localization.localizationHasBeenSet)
		{
            Localization.LoadDictionary(PlayerPrefs.GetString("Language", "English"), false);
		}
		if (Localization.mLanguages == null)
		{
			Debug.LogError("No localization data present");
			return null;
		}
		string language = Localization.language;
		if (Localization.mLanguageIndex == -1)
		{
			for (int i = 0; i < Localization.mLanguages.Length; i++)
			{
				if (Localization.mLanguages[i] == language)
				{
					Localization.mLanguageIndex = i;
					break;
				}
			}
		}
		if (Localization.mLanguageIndex == -1)
		{
			Localization.mLanguageIndex = 0;
			Localization.mLanguage = Localization.mLanguages[0];
			Debug.LogWarning("Language not found: " + language);
		}
        UICamera.ControlScheme currentScheme = UICamera.currentScheme;
		string result;
		string[] array;
		if (currentScheme == UICamera.ControlScheme.Touch)
		{
			string key2 = key + " Mobile";
			if (Localization.mReplacement.TryGetValue(key2, out result))
			{
				return result;
			}
			if (Localization.mLanguageIndex != -1 && Localization.mDictionary.TryGetValue(key2, out array) && Localization.mLanguageIndex < array.Length)
			{
				return array[Localization.mLanguageIndex];
			}
			if (Localization.mOldDictionary.TryGetValue(key2, out result))
			{
				return result;
			}
		}
		else if (currentScheme == UICamera.ControlScheme.Controller)
		{
			string key3 = key + " Controller";
			if (Localization.mReplacement.TryGetValue(key3, out result))
			{
				return result;
			}
			if (Localization.mLanguageIndex != -1 && Localization.mDictionary.TryGetValue(key3, out array) && Localization.mLanguageIndex < array.Length)
			{
				return array[Localization.mLanguageIndex];
			}
			if (Localization.mOldDictionary.TryGetValue(key3, out result))
			{
				return result;
			}
		}
		if (Localization.mReplacement.TryGetValue(key, out result))
		{
			return result;
		}
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            mLanguageIndex = 0;
            mLanguage = mLanguages[0];
        }
        else
        {
            for (int i = 0; i < Localization.mLanguages.Length; i++)
            {
                if (Localization.mLanguages[i] == PlayerPrefs.GetString("Language", "English"))
                {
                    Localization.mLanguageIndex = i;
                }
            }
        }
#endif
        if (Localization.mLanguageIndex != -1 && Localization.mDictionary.TryGetValue(key, out array))
		{
			if (Localization.mLanguageIndex < array.Length)
			{
				string text = array[Localization.mLanguageIndex];
				if (string.IsNullOrEmpty(text))
				{
					text = array[0];
				}
				return text;
			}
			return array[0];
		}
		else
		{
			if (Localization.mOldDictionary.TryGetValue(key, out result))
			{
				return result;
			}
			return key;
		}
	}

	// Token: 0x06000472 RID: 1138 RVA: 0x00033A14 File Offset: 0x00031C14
	public static string Format(string key, object parameter)
	{
		string result;
		try
		{
			result = string.Format(Localization.Get(key, true), parameter);
		}
		catch (Exception)
		{
			Debug.LogError("string.Format(1): " + key);
			result = key;
		}
		return result;
	}

	// Token: 0x06000473 RID: 1139 RVA: 0x00033A68 File Offset: 0x00031C68
	public static string Format(string key, object arg0, object arg1)
	{
		string result;
		try
		{
			result = string.Format(Localization.Get(key, true), arg0, arg1);
		}
		catch (Exception)
		{
			Debug.LogError("string.Format(2): " + key);
			result = key;
		}
		return result;
	}

	// Token: 0x06000474 RID: 1140 RVA: 0x00033ABC File Offset: 0x00031CBC
	public static string Format(string key, object arg0, object arg1, object arg2)
	{
		string result;
		try
		{
			result = string.Format(Localization.Get(key, true), arg0, arg1, arg2);
		}
		catch (Exception)
		{
			Debug.LogError("string.Format(3): " + key);
			result = key;
		}
		return result;
	}

	// Token: 0x06000475 RID: 1141 RVA: 0x00033B14 File Offset: 0x00031D14
	public static string Format(string key, params object[] parameters)
	{
		string result;
		try
		{
			result = string.Format(Localization.Get(key, true), parameters);
		}
		catch (Exception)
		{
			Debug.LogError(string.Concat(new object[]
			{
				"string.Format(",
				parameters.Length,
				"): ",
				key
			}));
			result = key;
		}
		return result;
	}

	// Token: 0x1700006A RID: 106
	// (get) Token: 0x06000476 RID: 1142 RVA: 0x00007F30 File Offset: 0x00006130
	[Obsolete("Localization is now always active. You no longer need to check this property.")]
	public static bool isActive
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000477 RID: 1143 RVA: 0x00007F33 File Offset: 0x00006133
	[Obsolete("Use Localization.Get instead")]
	public static string Localize(string key)
	{
		return Localization.Get(key, true);
	}

	// Token: 0x06000478 RID: 1144 RVA: 0x00033B88 File Offset: 0x00031D88
	public static bool Exists(string key)
	{
		if (!Localization.localizationHasBeenSet)
		{
			Localization.language = PlayerPrefs.GetString("Language", "English");
		}
		string key2 = key + " Mobile";
		return Localization.mDictionary.ContainsKey(key2) || Localization.mOldDictionary.ContainsKey(key2) || Localization.mDictionary.ContainsKey(key) || Localization.mOldDictionary.ContainsKey(key);
	}

	// Token: 0x06000479 RID: 1145 RVA: 0x00033C04 File Offset: 0x00031E04
	public static void Set(string language, string key, string text)
	{
		string[] knownLanguages = Localization.knownLanguages;
		if (knownLanguages == null)
		{
			Localization.mLanguages = new string[]
			{
				language
			};
			knownLanguages = Localization.mLanguages;
		}
		int i = 0;
		int num = knownLanguages.Length;
		while (i < num)
		{
			if (knownLanguages[i] == language)
			{
				string[] array;
				if (!Localization.mDictionary.TryGetValue(key, out array))
				{
					array = new string[knownLanguages.Length];
					Localization.mDictionary[key] = array;
					array[0] = text;
				}
				array[i] = text;
				return;
			}
			i++;
		}
		int num2 = Localization.mLanguages.Length + 1;
		Array.Resize<string>(ref Localization.mLanguages, num2);
		Localization.mLanguages[num2 - 1] = language;
		Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
		foreach (KeyValuePair<string, string[]> keyValuePair in Localization.mDictionary)
		{
			string[] value = keyValuePair.Value;
			Array.Resize<string>(ref value, num2);
			value[num2 - 1] = value[0];
			dictionary.Add(keyValuePair.Key, value);
		}
		Localization.mDictionary = dictionary;
		string[] array2;
		if (!Localization.mDictionary.TryGetValue(key, out array2))
		{
			array2 = new string[knownLanguages.Length];
			Localization.mDictionary[key] = array2;
			array2[0] = text;
		}
		array2[num2 - 1] = text;
	}

	// Token: 0x0400035A RID: 858
	public static Localization.LoadFunction loadFunction;

	// Token: 0x0400035B RID: 859
	public static Localization.OnLocalizeNotification onLocalize;

	// Token: 0x0400035C RID: 860
	public static bool localizationHasBeenSet = false;

	// Token: 0x0400035D RID: 861
	private static string[] mLanguages = null;

	// Token: 0x0400035E RID: 862
	private static Dictionary<string, string> mOldDictionary = new Dictionary<string, string>();

	// Token: 0x0400035F RID: 863
	private static Dictionary<string, string[]> mDictionary = new Dictionary<string, string[]>();

	// Token: 0x04000360 RID: 864
	private static Dictionary<string, string> mReplacement = new Dictionary<string, string>();

	// Token: 0x04000361 RID: 865
	private static int mLanguageIndex = -1;

	// Token: 0x04000362 RID: 866
	private static string mLanguage;

	// Token: 0x04000363 RID: 867
	private static bool mMerging = false;

	// Token: 0x0200009B RID: 155
	// (Invoke) Token: 0x0600047B RID: 1147
	public delegate byte[] LoadFunction(string path);

	// Token: 0x0200009C RID: 156
	// (Invoke) Token: 0x0600047F RID: 1151
	public delegate void OnLocalizeNotification();
}
