using System;
using System.Text;
using CodeStage.AntiCheat.Utils;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	// Token: 0x020001C7 RID: 455
	public static class ObscuredPrefs
	{
		// Token: 0x1700030E RID: 782
		// (get) Token: 0x06001127 RID: 4391 RVA: 0x00010243 File Offset: 0x0000E443
		// (set) Token: 0x06001126 RID: 4390 RVA: 0x0001023B File Offset: 0x0000E43B
		public static string CryptoKey
		{
			get
			{
				return ObscuredPrefs.cryptoKey;
			}
			set
			{
				ObscuredPrefs.cryptoKey = value;
			}
		}

		// Token: 0x1700030F RID: 783
		// (get) Token: 0x06001128 RID: 4392 RVA: 0x0001024A File Offset: 0x0000E44A
		// (set) Token: 0x06001129 RID: 4393 RVA: 0x0001026A File Offset: 0x0000E46A
		public static string DeviceId
		{
			get
			{
				if (string.IsNullOrEmpty(ObscuredPrefs.deviceId))
				{
					ObscuredPrefs.deviceId = ObscuredPrefs.GetDeviceId();
				}
				return ObscuredPrefs.deviceId;
			}
			set
			{
				ObscuredPrefs.deviceId = value;
			}
		}

		// Token: 0x17000310 RID: 784
		// (get) Token: 0x0600112A RID: 4394 RVA: 0x00010272 File Offset: 0x0000E472
		// (set) Token: 0x0600112B RID: 4395 RVA: 0x00010279 File Offset: 0x0000E479
		[Obsolete("This property is obsolete, please use DeviceId instead.")]
		internal static string DeviceID
		{
			get
			{
				return ObscuredPrefs.DeviceId;
			}
			set
			{
				ObscuredPrefs.DeviceId = value;
			}
		}

		// Token: 0x17000311 RID: 785
		// (get) Token: 0x0600112C RID: 4396 RVA: 0x00010281 File Offset: 0x0000E481
		private static uint DeviceIdHash
		{
			get
			{
				if (ObscuredPrefs.deviceIdHash == 0U)
				{
					ObscuredPrefs.deviceIdHash = ObscuredPrefs.CalculateChecksum(ObscuredPrefs.DeviceId);
				}
				return ObscuredPrefs.deviceIdHash;
			}
		}

		// Token: 0x0600112D RID: 4397 RVA: 0x000102A1 File Offset: 0x0000E4A1
		public static void ForceLockToDeviceInit()
		{
			if (string.IsNullOrEmpty(ObscuredPrefs.deviceId))
			{
				ObscuredPrefs.deviceId = ObscuredPrefs.GetDeviceId();
				ObscuredPrefs.deviceIdHash = ObscuredPrefs.CalculateChecksum(ObscuredPrefs.deviceId);
			}
			else
			{
				Debug.LogWarning("[ACTk] ObscuredPrefs.ForceLockToDeviceInit() is called, but device ID is already obtained!");
			}
		}

		// Token: 0x0600112E RID: 4398 RVA: 0x000102DA File Offset: 0x0000E4DA
		[Obsolete("This method is obsolete, use property CryptoKey instead")]
		internal static void SetNewCryptoKey(string newKey)
		{
			ObscuredPrefs.CryptoKey = newKey;
		}

		// Token: 0x0600112F RID: 4399 RVA: 0x000102E2 File Offset: 0x0000E4E2
		public static void SetInt(string key, int value)
		{
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), ObscuredPrefs.EncryptIntValue(key, value));
		}

		// Token: 0x06001130 RID: 4400 RVA: 0x000102F6 File Offset: 0x0000E4F6
		public static int GetInt(string key)
		{
			return ObscuredPrefs.GetInt(key, 0);
		}

		// Token: 0x06001131 RID: 4401 RVA: 0x0007596C File Offset: 0x00073B6C
		public static int GetInt(string key, int defaultValue)
		{
			string text = ObscuredPrefs.EncryptKey(key);
			if (!PlayerPrefs.HasKey(text) && PlayerPrefs.HasKey(key))
			{
				int @int = PlayerPrefs.GetInt(key, defaultValue);
				if (!ObscuredPrefs.preservePlayerPrefs)
				{
					ObscuredPrefs.SetInt(key, @int);
					PlayerPrefs.DeleteKey(key);
				}
				return @int;
			}
			string encryptedPrefsString = ObscuredPrefs.GetEncryptedPrefsString(key, text);
			return (!(encryptedPrefsString == "{not_found}")) ? ObscuredPrefs.DecryptIntValue(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		// Token: 0x06001132 RID: 4402 RVA: 0x000759E0 File Offset: 0x00073BE0
		internal static string EncryptIntValue(string key, int value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return ObscuredPrefs.EncryptData(key, bytes, ObscuredPrefs.DataType.Int);
		}

		// Token: 0x06001133 RID: 4403 RVA: 0x000759FC File Offset: 0x00073BFC
		internal static int DecryptIntValue(string key, string encryptedInput, int defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = ObscuredPrefs.DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				int num;
				int.TryParse(text, out num);
				ObscuredPrefs.SetInt(key, num);
				return num;
			}
			else
			{
				byte[] array = ObscuredPrefs.DecryptData(key, encryptedInput);
				if (array == null)
				{
					return defaultValue;
				}
				return BitConverter.ToInt32(array, 0);
			}
		}

		// Token: 0x06001134 RID: 4404 RVA: 0x000102FF File Offset: 0x0000E4FF
		public static void SetUInt(string key, uint value)
		{
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), ObscuredPrefs.EncryptUIntValue(key, value));
		}

		// Token: 0x06001135 RID: 4405 RVA: 0x00010313 File Offset: 0x0000E513
		public static uint GetUInt(string key)
		{
			return ObscuredPrefs.GetUInt(key, 0U);
		}

		// Token: 0x06001136 RID: 4406 RVA: 0x00075A5C File Offset: 0x00073C5C
		public static uint GetUInt(string key, uint defaultValue)
		{
			string encryptedPrefsString = ObscuredPrefs.GetEncryptedPrefsString(key, ObscuredPrefs.EncryptKey(key));
			return (!(encryptedPrefsString == "{not_found}")) ? ObscuredPrefs.DecryptUIntValue(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		// Token: 0x06001137 RID: 4407 RVA: 0x00075A94 File Offset: 0x00073C94
		private static string EncryptUIntValue(string key, uint value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return ObscuredPrefs.EncryptData(key, bytes, ObscuredPrefs.DataType.UInt);
		}

		// Token: 0x06001138 RID: 4408 RVA: 0x00075AB4 File Offset: 0x00073CB4
		private static uint DecryptUIntValue(string key, string encryptedInput, uint defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = ObscuredPrefs.DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				uint num;
				uint.TryParse(text, out num);
				ObscuredPrefs.SetUInt(key, num);
				return num;
			}
			else
			{
				byte[] array = ObscuredPrefs.DecryptData(key, encryptedInput);
				if (array == null)
				{
					return defaultValue;
				}
				return BitConverter.ToUInt32(array, 0);
			}
		}

		// Token: 0x06001139 RID: 4409 RVA: 0x0001031C File Offset: 0x0000E51C
		public static void SetString(string key, string value)
		{
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), ObscuredPrefs.EncryptStringValue(key, value));
		}

		// Token: 0x0600113A RID: 4410 RVA: 0x00010330 File Offset: 0x0000E530
		public static string GetString(string key)
		{
			return ObscuredPrefs.GetString(key, string.Empty);
		}

		// Token: 0x0600113B RID: 4411 RVA: 0x00075B14 File Offset: 0x00073D14
		public static string GetString(string key, string defaultValue)
		{
			string text = ObscuredPrefs.EncryptKey(key);
			if (!PlayerPrefs.HasKey(text) && PlayerPrefs.HasKey(key))
			{
				string @string = PlayerPrefs.GetString(key, defaultValue);
				if (!ObscuredPrefs.preservePlayerPrefs)
				{
					ObscuredPrefs.SetString(key, @string);
					PlayerPrefs.DeleteKey(key);
				}
				return @string;
			}
			string encryptedPrefsString = ObscuredPrefs.GetEncryptedPrefsString(key, text);
			return (!(encryptedPrefsString == "{not_found}")) ? ObscuredPrefs.DecryptStringValue(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		// Token: 0x0600113C RID: 4412 RVA: 0x00075B88 File Offset: 0x00073D88
		internal static string EncryptStringValue(string key, string value)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(value);
			return ObscuredPrefs.EncryptData(key, bytes, ObscuredPrefs.DataType.String);
		}

		// Token: 0x0600113D RID: 4413 RVA: 0x00075BAC File Offset: 0x00073DAC
		internal static string DecryptStringValue(string key, string encryptedInput, string defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = ObscuredPrefs.DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				ObscuredPrefs.SetString(key, text);
				return text;
			}
			else
			{
				byte[] array = ObscuredPrefs.DecryptData(key, encryptedInput);
				if (array == null)
				{
					return defaultValue;
				}
				return Encoding.UTF8.GetString(array, 0, array.Length);
			}
		}

		// Token: 0x0600113E RID: 4414 RVA: 0x0001033D File Offset: 0x0000E53D
		public static void SetFloat(string key, float value)
		{
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), ObscuredPrefs.EncryptFloatValue(key, value));
		}

		// Token: 0x0600113F RID: 4415 RVA: 0x00010351 File Offset: 0x0000E551
		public static float GetFloat(string key)
		{
			return ObscuredPrefs.GetFloat(key, 0f);
		}

		// Token: 0x06001140 RID: 4416 RVA: 0x00075C0C File Offset: 0x00073E0C
		public static float GetFloat(string key, float defaultValue)
		{
			string text = ObscuredPrefs.EncryptKey(key);
			if (!PlayerPrefs.HasKey(text) && PlayerPrefs.HasKey(key))
			{
				float @float = PlayerPrefs.GetFloat(key, defaultValue);
				if (!ObscuredPrefs.preservePlayerPrefs)
				{
					ObscuredPrefs.SetFloat(key, @float);
					PlayerPrefs.DeleteKey(key);
				}
				return @float;
			}
			string encryptedPrefsString = ObscuredPrefs.GetEncryptedPrefsString(key, text);
			return (!(encryptedPrefsString == "{not_found}")) ? ObscuredPrefs.DecryptFloatValue(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		// Token: 0x06001141 RID: 4417 RVA: 0x00075C80 File Offset: 0x00073E80
		internal static string EncryptFloatValue(string key, float value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return ObscuredPrefs.EncryptData(key, bytes, ObscuredPrefs.DataType.Float);
		}

		// Token: 0x06001142 RID: 4418 RVA: 0x00075CA0 File Offset: 0x00073EA0
		internal static float DecryptFloatValue(string key, string encryptedInput, float defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = ObscuredPrefs.DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				float num;
				float.TryParse(text, out num);
				ObscuredPrefs.SetFloat(key, num);
				return num;
			}
			else
			{
				byte[] array = ObscuredPrefs.DecryptData(key, encryptedInput);
				if (array == null)
				{
					return defaultValue;
				}
				return BitConverter.ToSingle(array, 0);
			}
		}

		// Token: 0x06001143 RID: 4419 RVA: 0x0001035E File Offset: 0x0000E55E
		public static void SetDouble(string key, double value)
		{
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), ObscuredPrefs.EncryptDoubleValue(key, value));
		}

		// Token: 0x06001144 RID: 4420 RVA: 0x00010372 File Offset: 0x0000E572
		public static double GetDouble(string key)
		{
			return ObscuredPrefs.GetDouble(key, 0.0);
		}

		// Token: 0x06001145 RID: 4421 RVA: 0x00075D00 File Offset: 0x00073F00
		public static double GetDouble(string key, double defaultValue)
		{
			string encryptedPrefsString = ObscuredPrefs.GetEncryptedPrefsString(key, ObscuredPrefs.EncryptKey(key));
			return (!(encryptedPrefsString == "{not_found}")) ? ObscuredPrefs.DecryptDoubleValue(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		// Token: 0x06001146 RID: 4422 RVA: 0x00075D38 File Offset: 0x00073F38
		private static string EncryptDoubleValue(string key, double value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return ObscuredPrefs.EncryptData(key, bytes, ObscuredPrefs.DataType.Double);
		}

		// Token: 0x06001147 RID: 4423 RVA: 0x00075D58 File Offset: 0x00073F58
		private static double DecryptDoubleValue(string key, string encryptedInput, double defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = ObscuredPrefs.DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				double num;
				double.TryParse(text, out num);
				ObscuredPrefs.SetDouble(key, num);
				return num;
			}
			else
			{
				byte[] array = ObscuredPrefs.DecryptData(key, encryptedInput);
				if (array == null)
				{
					return defaultValue;
				}
				return BitConverter.ToDouble(array, 0);
			}
		}

		// Token: 0x06001148 RID: 4424 RVA: 0x00010383 File Offset: 0x0000E583
		public static void SetLong(string key, long value)
		{
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), ObscuredPrefs.EncryptLongValue(key, value));
		}

		// Token: 0x06001149 RID: 4425 RVA: 0x00010397 File Offset: 0x0000E597
		public static long GetLong(string key)
		{
			return ObscuredPrefs.GetLong(key, 0L);
		}

		// Token: 0x0600114A RID: 4426 RVA: 0x00075DB8 File Offset: 0x00073FB8
		public static long GetLong(string key, long defaultValue)
		{
			string encryptedPrefsString = ObscuredPrefs.GetEncryptedPrefsString(key, ObscuredPrefs.EncryptKey(key));
			return (!(encryptedPrefsString == "{not_found}")) ? ObscuredPrefs.DecryptLongValue(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		// Token: 0x0600114B RID: 4427 RVA: 0x00075DF0 File Offset: 0x00073FF0
		private static string EncryptLongValue(string key, long value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return ObscuredPrefs.EncryptData(key, bytes, ObscuredPrefs.DataType.Long);
		}

		// Token: 0x0600114C RID: 4428 RVA: 0x00075E10 File Offset: 0x00074010
		private static long DecryptLongValue(string key, string encryptedInput, long defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = ObscuredPrefs.DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				long num;
				long.TryParse(text, out num);
				ObscuredPrefs.SetLong(key, num);
				return num;
			}
			else
			{
				byte[] array = ObscuredPrefs.DecryptData(key, encryptedInput);
				if (array == null)
				{
					return defaultValue;
				}
				return BitConverter.ToInt64(array, 0);
			}
		}

		// Token: 0x0600114D RID: 4429 RVA: 0x000103A1 File Offset: 0x0000E5A1
		public static void SetBool(string key, bool value)
		{
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), ObscuredPrefs.EncryptBoolValue(key, value));
		}

		// Token: 0x0600114E RID: 4430 RVA: 0x000103B5 File Offset: 0x0000E5B5
		public static bool GetBool(string key)
		{
			return ObscuredPrefs.GetBool(key, false);
		}

		// Token: 0x0600114F RID: 4431 RVA: 0x00075E70 File Offset: 0x00074070
		public static bool GetBool(string key, bool defaultValue)
		{
			string encryptedPrefsString = ObscuredPrefs.GetEncryptedPrefsString(key, ObscuredPrefs.EncryptKey(key));
			return (!(encryptedPrefsString == "{not_found}")) ? ObscuredPrefs.DecryptBoolValue(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		// Token: 0x06001150 RID: 4432 RVA: 0x00075EA8 File Offset: 0x000740A8
		private static string EncryptBoolValue(string key, bool value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return ObscuredPrefs.EncryptData(key, bytes, ObscuredPrefs.DataType.Bool);
		}

		// Token: 0x06001151 RID: 4433 RVA: 0x00075EC8 File Offset: 0x000740C8
		private static bool DecryptBoolValue(string key, string encryptedInput, bool defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = ObscuredPrefs.DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				int num;
				int.TryParse(text, out num);
				ObscuredPrefs.SetBool(key, num == 1);
				return num == 1;
			}
			else
			{
				byte[] array = ObscuredPrefs.DecryptData(key, encryptedInput);
				if (array == null)
				{
					return defaultValue;
				}
				return BitConverter.ToBoolean(array, 0);
			}
		}

		// Token: 0x06001152 RID: 4434 RVA: 0x000103BE File Offset: 0x0000E5BE
		public static void SetByteArray(string key, byte[] value)
		{
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), ObscuredPrefs.EncryptByteArrayValue(key, value));
		}

		// Token: 0x06001153 RID: 4435 RVA: 0x000103D2 File Offset: 0x0000E5D2
		public static byte[] GetByteArray(string key)
		{
			return ObscuredPrefs.GetByteArray(key, 0, 0);
		}

		// Token: 0x06001154 RID: 4436 RVA: 0x00075F30 File Offset: 0x00074130
		public static byte[] GetByteArray(string key, byte defaultValue, int defaultLength)
		{
			string encryptedPrefsString = ObscuredPrefs.GetEncryptedPrefsString(key, ObscuredPrefs.EncryptKey(key));
			if (encryptedPrefsString == "{not_found}")
			{
				return ObscuredPrefs.ConstructByteArray(defaultValue, defaultLength);
			}
			return ObscuredPrefs.DecryptByteArrayValue(key, encryptedPrefsString, defaultValue, defaultLength);
		}

		// Token: 0x06001155 RID: 4437 RVA: 0x000103DC File Offset: 0x0000E5DC
		private static string EncryptByteArrayValue(string key, byte[] value)
		{
			return ObscuredPrefs.EncryptData(key, value, ObscuredPrefs.DataType.ByteArray);
		}

		// Token: 0x06001156 RID: 4438 RVA: 0x00075F6C File Offset: 0x0007416C
		private static byte[] DecryptByteArrayValue(string key, string encryptedInput, byte defaultValue, int defaultLength)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = ObscuredPrefs.DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return ObscuredPrefs.ConstructByteArray(defaultValue, defaultLength);
				}
				byte[] bytes = Encoding.UTF8.GetBytes(text);
				ObscuredPrefs.SetByteArray(key, bytes);
				return bytes;
			}
			else
			{
				byte[] array = ObscuredPrefs.DecryptData(key, encryptedInput);
				if (array == null)
				{
					return ObscuredPrefs.ConstructByteArray(defaultValue, defaultLength);
				}
				return array;
			}
		}

		// Token: 0x06001157 RID: 4439 RVA: 0x00075FD4 File Offset: 0x000741D4
		private static byte[] ConstructByteArray(byte value, int length)
		{
			byte[] array = new byte[length];
			for (int i = 0; i < length; i++)
			{
				array[i] = value;
			}
			return array;
		}

		// Token: 0x06001158 RID: 4440 RVA: 0x000103E7 File Offset: 0x0000E5E7
		public static void SetVector2(string key, Vector2 value)
		{
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), ObscuredPrefs.EncryptVector2Value(key, value));
		}

		// Token: 0x06001159 RID: 4441 RVA: 0x000103FB File Offset: 0x0000E5FB
		public static Vector2 GetVector2(string key)
		{
			return ObscuredPrefs.GetVector2(key, Vector2.zero);
		}

		// Token: 0x0600115A RID: 4442 RVA: 0x00076000 File Offset: 0x00074200
		public static Vector2 GetVector2(string key, Vector2 defaultValue)
		{
			string encryptedPrefsString = ObscuredPrefs.GetEncryptedPrefsString(key, ObscuredPrefs.EncryptKey(key));
			return (!(encryptedPrefsString == "{not_found}")) ? ObscuredPrefs.DecryptVector2Value(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		// Token: 0x0600115B RID: 4443 RVA: 0x00076038 File Offset: 0x00074238
		private static string EncryptVector2Value(string key, Vector2 value)
		{
			byte[] array = new byte[8];
			Buffer.BlockCopy(BitConverter.GetBytes(value.x), 0, array, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.y), 0, array, 4, 4);
			return ObscuredPrefs.EncryptData(key, array, ObscuredPrefs.DataType.Vector2);
		}

		// Token: 0x0600115C RID: 4444 RVA: 0x00076080 File Offset: 0x00074280
		private static Vector2 DecryptVector2Value(string key, string encryptedInput, Vector2 defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = ObscuredPrefs.DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				string[] array = text.Split(new char[]
				{
					"|"[0]
				});
				float x;
				float.TryParse(array[0], out x);
				float y;
				float.TryParse(array[1], out y);
				Vector2 vector = new Vector2(x, y);
				ObscuredPrefs.SetVector2(key, vector);
				return vector;
			}
			else
			{
				byte[] array2 = ObscuredPrefs.DecryptData(key, encryptedInput);
				if (array2 == null)
				{
					return defaultValue;
				}
				Vector2 result;
				result.x = BitConverter.ToSingle(array2, 0);
				result.y = BitConverter.ToSingle(array2, 4);
				return result;
			}
		}

		// Token: 0x0600115D RID: 4445 RVA: 0x00010408 File Offset: 0x0000E608
		public static void SetVector3(string key, Vector3 value)
		{
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), ObscuredPrefs.EncryptVector3Value(key, value));
		}

		// Token: 0x0600115E RID: 4446 RVA: 0x0001041C File Offset: 0x0000E61C
		public static Vector3 GetVector3(string key)
		{
			return ObscuredPrefs.GetVector3(key, Vector3.zero);
		}

		// Token: 0x0600115F RID: 4447 RVA: 0x0007612C File Offset: 0x0007432C
		public static Vector3 GetVector3(string key, Vector3 defaultValue)
		{
			string encryptedPrefsString = ObscuredPrefs.GetEncryptedPrefsString(key, ObscuredPrefs.EncryptKey(key));
			return (!(encryptedPrefsString == "{not_found}")) ? ObscuredPrefs.DecryptVector3Value(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		// Token: 0x06001160 RID: 4448 RVA: 0x00076164 File Offset: 0x00074364
		private static string EncryptVector3Value(string key, Vector3 value)
		{
			byte[] array = new byte[12];
			Buffer.BlockCopy(BitConverter.GetBytes(value.x), 0, array, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.y), 0, array, 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.z), 0, array, 8, 4);
			return ObscuredPrefs.EncryptData(key, array, ObscuredPrefs.DataType.Vector3);
		}

		// Token: 0x06001161 RID: 4449 RVA: 0x000761C4 File Offset: 0x000743C4
		private static Vector3 DecryptVector3Value(string key, string encryptedInput, Vector3 defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = ObscuredPrefs.DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				string[] array = text.Split(new char[]
				{
					"|"[0]
				});
				float x;
				float.TryParse(array[0], out x);
				float y;
				float.TryParse(array[1], out y);
				float z;
				float.TryParse(array[2], out z);
				Vector3 vector = new Vector3(x, y, z);
				ObscuredPrefs.SetVector3(key, vector);
				return vector;
			}
			else
			{
				byte[] array2 = ObscuredPrefs.DecryptData(key, encryptedInput);
				if (array2 == null)
				{
					return defaultValue;
				}
				Vector3 result;
				result.x = BitConverter.ToSingle(array2, 0);
				result.y = BitConverter.ToSingle(array2, 4);
				result.z = BitConverter.ToSingle(array2, 8);
				return result;
			}
		}

		// Token: 0x06001162 RID: 4450 RVA: 0x00010429 File Offset: 0x0000E629
		public static void SetQuaternion(string key, Quaternion value)
		{
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), ObscuredPrefs.EncryptQuaternionValue(key, value));
		}

		// Token: 0x06001163 RID: 4451 RVA: 0x0001043D File Offset: 0x0000E63D
		public static Quaternion GetQuaternion(string key)
		{
			return ObscuredPrefs.GetQuaternion(key, Quaternion.identity);
		}

		// Token: 0x06001164 RID: 4452 RVA: 0x0007628C File Offset: 0x0007448C
		public static Quaternion GetQuaternion(string key, Quaternion defaultValue)
		{
			string encryptedPrefsString = ObscuredPrefs.GetEncryptedPrefsString(key, ObscuredPrefs.EncryptKey(key));
			return (!(encryptedPrefsString == "{not_found}")) ? ObscuredPrefs.DecryptQuaternionValue(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		// Token: 0x06001165 RID: 4453 RVA: 0x000762C4 File Offset: 0x000744C4
		private static string EncryptQuaternionValue(string key, Quaternion value)
		{
			byte[] array = new byte[16];
			Buffer.BlockCopy(BitConverter.GetBytes(value.x), 0, array, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.y), 0, array, 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.z), 0, array, 8, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.w), 0, array, 12, 4);
			return ObscuredPrefs.EncryptData(key, array, ObscuredPrefs.DataType.Quaternion);
		}

		// Token: 0x06001166 RID: 4454 RVA: 0x00076338 File Offset: 0x00074538
		private static Quaternion DecryptQuaternionValue(string key, string encryptedInput, Quaternion defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = ObscuredPrefs.DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				string[] array = text.Split(new char[]
				{
					"|"[0]
				});
				float x;
				float.TryParse(array[0], out x);
				float y;
				float.TryParse(array[1], out y);
				float z;
				float.TryParse(array[2], out z);
				float w;
				float.TryParse(array[3], out w);
				Quaternion quaternion = new Quaternion(x, y, z, w);
				ObscuredPrefs.SetQuaternion(key, quaternion);
				return quaternion;
			}
			else
			{
				byte[] array2 = ObscuredPrefs.DecryptData(key, encryptedInput);
				if (array2 == null)
				{
					return defaultValue;
				}
				Quaternion result;
				result.x = BitConverter.ToSingle(array2, 0);
				result.y = BitConverter.ToSingle(array2, 4);
				result.z = BitConverter.ToSingle(array2, 8);
				result.w = BitConverter.ToSingle(array2, 12);
				return result;
			}
		}

		// Token: 0x06001167 RID: 4455 RVA: 0x0007641C File Offset: 0x0007461C
		public static void SetColor(string key, Color32 value)
		{
			uint value2 = (uint)((int)value.a << 24 | (int)value.r << 16 | (int)value.g << 8 | (int)value.b);
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), ObscuredPrefs.EncryptColorValue(key, value2));
		}

		// Token: 0x06001168 RID: 4456 RVA: 0x0001044A File Offset: 0x0000E64A
		public static Color32 GetColor(string key)
		{
			return ObscuredPrefs.GetColor(key, new Color32(0, 0, 0, 1));
		}

		// Token: 0x06001169 RID: 4457 RVA: 0x00076464 File Offset: 0x00074664
		public static Color32 GetColor(string key, Color32 defaultValue)
		{
			string encryptedPrefsString = ObscuredPrefs.GetEncryptedPrefsString(key, ObscuredPrefs.EncryptKey(key));
			if (encryptedPrefsString == "{not_found}")
			{
				return defaultValue;
			}
			uint num = ObscuredPrefs.DecryptUIntValue(key, encryptedPrefsString, 16777216U);
			byte a = (byte)(num >> 24);
			byte r = (byte)(num >> 16);
			byte g = (byte)(num >> 8);
			byte b = (byte)num;
			return new Color32(r, g, b, a);
		}

		// Token: 0x0600116A RID: 4458 RVA: 0x000764C0 File Offset: 0x000746C0
		private static string EncryptColorValue(string key, uint value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return ObscuredPrefs.EncryptData(key, bytes, ObscuredPrefs.DataType.Color);
		}

		// Token: 0x0600116B RID: 4459 RVA: 0x0001045B File Offset: 0x0000E65B
		public static void SetRect(string key, Rect value)
		{
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), ObscuredPrefs.EncryptRectValue(key, value));
		}

		// Token: 0x0600116C RID: 4460 RVA: 0x0001046F File Offset: 0x0000E66F
		public static Rect GetRect(string key)
		{
			return ObscuredPrefs.GetRect(key, new Rect(0f, 0f, 0f, 0f));
		}

		// Token: 0x0600116D RID: 4461 RVA: 0x000764E0 File Offset: 0x000746E0
		public static Rect GetRect(string key, Rect defaultValue)
		{
			string encryptedPrefsString = ObscuredPrefs.GetEncryptedPrefsString(key, ObscuredPrefs.EncryptKey(key));
			return (!(encryptedPrefsString == "{not_found}")) ? ObscuredPrefs.DecryptRectValue(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		// Token: 0x0600116E RID: 4462 RVA: 0x00076518 File Offset: 0x00074718
		private static string EncryptRectValue(string key, Rect value)
		{
			byte[] array = new byte[16];
			Buffer.BlockCopy(BitConverter.GetBytes(value.x), 0, array, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.y), 0, array, 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.width), 0, array, 8, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.height), 0, array, 12, 4);
			return ObscuredPrefs.EncryptData(key, array, ObscuredPrefs.DataType.Rect);
		}

		// Token: 0x0600116F RID: 4463 RVA: 0x0007658C File Offset: 0x0007478C
		private static Rect DecryptRectValue(string key, string encryptedInput, Rect defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = ObscuredPrefs.DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				string[] array = text.Split(new char[]
				{
					"|"[0]
				});
				float left;
				float.TryParse(array[0], out left);
				float top;
				float.TryParse(array[1], out top);
				float width;
				float.TryParse(array[2], out width);
				float height;
				float.TryParse(array[3], out height);
				Rect rect = new Rect(left, top, width, height);
				ObscuredPrefs.SetRect(key, rect);
				return rect;
			}
			else
			{
				byte[] array2 = ObscuredPrefs.DecryptData(key, encryptedInput);
				if (array2 == null)
				{
					return defaultValue;
				}
				return new Rect
				{
					x = BitConverter.ToSingle(array2, 0),
					y = BitConverter.ToSingle(array2, 4),
					width = BitConverter.ToSingle(array2, 8),
					height = BitConverter.ToSingle(array2, 12)
				};
			}
		}

		// Token: 0x06001170 RID: 4464 RVA: 0x00010490 File Offset: 0x0000E690
		public static void SetRawValue(string key, string encryptedValue)
		{
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), encryptedValue);
		}

		// Token: 0x06001171 RID: 4465 RVA: 0x00076678 File Offset: 0x00074878
		public static string GetRawValue(string key)
		{
			string key2 = ObscuredPrefs.EncryptKey(key);
			return PlayerPrefs.GetString(key2);
		}

		// Token: 0x06001172 RID: 4466 RVA: 0x00076694 File Offset: 0x00074894
		internal static ObscuredPrefs.DataType GetRawValueType(string value)
		{
			ObscuredPrefs.DataType result = ObscuredPrefs.DataType.Unknown;
			byte[] array;
			try
			{
				array = Convert.FromBase64String(value);
			}
			catch (Exception)
			{
				return result;
			}
			if (array.Length < 7)
			{
				return result;
			}
			int num = array.Length;
			result = (ObscuredPrefs.DataType)array[num - 7];
			byte b = array[num - 6];
			if (b > 10)
			{
				result = ObscuredPrefs.DataType.Unknown;
			}
			return result;
		}

		// Token: 0x06001173 RID: 4467 RVA: 0x0001049E File Offset: 0x0000E69E
		internal static string EncryptKey(string key)
		{
			key = ObscuredString.EncryptDecrypt(key, ObscuredPrefs.cryptoKey);
			key = Convert.ToBase64String(Encoding.UTF8.GetBytes(key));
			return key;
		}

		// Token: 0x06001174 RID: 4468 RVA: 0x000104C0 File Offset: 0x0000E6C0
		public static bool HasKey(string key)
		{
			return PlayerPrefs.HasKey(key) || PlayerPrefs.HasKey(ObscuredPrefs.EncryptKey(key));
		}

		// Token: 0x06001175 RID: 4469 RVA: 0x000104DB File Offset: 0x0000E6DB
		public static void DeleteKey(string key)
		{
			PlayerPrefs.DeleteKey(ObscuredPrefs.EncryptKey(key));
			if (!ObscuredPrefs.preservePlayerPrefs)
			{
				PlayerPrefs.DeleteKey(key);
			}
		}

		// Token: 0x06001176 RID: 4470 RVA: 0x000104F8 File Offset: 0x0000E6F8
		public static void DeleteAll()
		{
			PlayerPrefs.DeleteAll();
		}

		// Token: 0x06001177 RID: 4471 RVA: 0x000104FF File Offset: 0x0000E6FF
		public static void Save()
		{
			PlayerPrefs.Save();
		}

		// Token: 0x06001178 RID: 4472 RVA: 0x000766F8 File Offset: 0x000748F8
		private static string GetEncryptedPrefsString(string key, string encryptedKey)
		{
			string @string = PlayerPrefs.GetString(encryptedKey, "{not_found}");
			if (@string == "{not_found}" && PlayerPrefs.HasKey(key))
			{
				Debug.LogWarning("[ACTk] Are you trying to read regular PlayerPrefs data using ObscuredPrefs (key = " + key + ")?");
			}
			return @string;
		}

		// Token: 0x06001179 RID: 4473 RVA: 0x00076744 File Offset: 0x00074944
		private static string EncryptData(string key, byte[] cleanBytes, ObscuredPrefs.DataType type)
		{
			int num = cleanBytes.Length;
			byte[] src = ObscuredPrefs.EncryptDecryptBytes(cleanBytes, num, key + ObscuredPrefs.cryptoKey);
			uint num2 = xxHash.CalculateHash(cleanBytes, num, 0U);
			byte[] src2 = new byte[]
			{
				(byte)(num2 & 255U),
				(byte)(num2 >> 8 & 255U),
				(byte)(num2 >> 16 & 255U),
				(byte)(num2 >> 24 & 255U)
			};
			byte[] array = null;
			int num3;
			if (ObscuredPrefs.lockToDevice != ObscuredPrefs.DeviceLockLevel.None)
			{
				num3 = num + 11;
				uint num4 = ObscuredPrefs.DeviceIdHash;
				array = new byte[]
				{
					(byte)(num4 & 255U),
					(byte)(num4 >> 8 & 255U),
					(byte)(num4 >> 16 & 255U),
					(byte)(num4 >> 24 & 255U)
				};
			}
			else
			{
				num3 = num + 7;
			}
			byte[] array2 = new byte[num3];
			Buffer.BlockCopy(src, 0, array2, 0, num);
			if (array != null)
			{
				Buffer.BlockCopy(array, 0, array2, num, 4);
			}
			array2[num3 - 7] = (byte)type;
			array2[num3 - 6] = 2;
			array2[num3 - 5] = (byte)ObscuredPrefs.lockToDevice;
			Buffer.BlockCopy(src2, 0, array2, num3 - 4, 4);
			return Convert.ToBase64String(array2);
		}

		// Token: 0x0600117A RID: 4474 RVA: 0x0007686C File Offset: 0x00074A6C
		internal static byte[] DecryptData(string key, string encryptedInput)
		{
			byte[] array;
			try
			{
				array = Convert.FromBase64String(encryptedInput);
			}
			catch (Exception)
			{
				ObscuredPrefs.SavesTampered();
				return null;
			}
			if (array.Length <= 0)
			{
				ObscuredPrefs.SavesTampered();
				return null;
			}
			int num = array.Length;
			byte b = array[num - 6];
			if (b != 2)
			{
				ObscuredPrefs.SavesTampered();
				return null;
			}
			ObscuredPrefs.DeviceLockLevel deviceLockLevel = (ObscuredPrefs.DeviceLockLevel)array[num - 5];
			byte[] array2 = new byte[4];
			Buffer.BlockCopy(array, num - 4, array2, 0, 4);
			uint num2 = (uint)((int)array2[0] | (int)array2[1] << 8 | (int)array2[2] << 16 | (int)array2[3] << 24);
			uint num3 = 0U;
			int num4;
			if (deviceLockLevel != ObscuredPrefs.DeviceLockLevel.None)
			{
				num4 = num - 11;
				if (ObscuredPrefs.lockToDevice != ObscuredPrefs.DeviceLockLevel.None)
				{
					byte[] array3 = new byte[4];
					Buffer.BlockCopy(array, num4, array3, 0, 4);
					num3 = (uint)((int)array3[0] | (int)array3[1] << 8 | (int)array3[2] << 16 | (int)array3[3] << 24);
				}
			}
			else
			{
				num4 = num - 7;
			}
			byte[] array4 = new byte[num4];
			Buffer.BlockCopy(array, 0, array4, 0, num4);
			byte[] array5 = ObscuredPrefs.EncryptDecryptBytes(array4, num4, key + ObscuredPrefs.cryptoKey);
			uint num5 = xxHash.CalculateHash(array5, num4, 0U);
			if (num5 != num2)
			{
				ObscuredPrefs.SavesTampered();
				return null;
			}
			if (ObscuredPrefs.lockToDevice == ObscuredPrefs.DeviceLockLevel.Strict && num3 == 0U && !ObscuredPrefs.emergencyMode && !ObscuredPrefs.readForeignSaves)
			{
				return null;
			}
			if (num3 != 0U && !ObscuredPrefs.emergencyMode)
			{
				uint num6 = ObscuredPrefs.DeviceIdHash;
				if (num3 != num6)
				{
					ObscuredPrefs.PossibleForeignSavesDetected();
					if (!ObscuredPrefs.readForeignSaves)
					{
						return null;
					}
				}
			}
			return array5;
		}

		// Token: 0x0600117B RID: 4475 RVA: 0x00076A04 File Offset: 0x00074C04
		private static uint CalculateChecksum(string input)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(input + ObscuredPrefs.cryptoKey);
			return xxHash.CalculateHash(bytes, bytes.Length, 0U);
		}

		// Token: 0x0600117C RID: 4476 RVA: 0x00010506 File Offset: 0x0000E706
		private static void SavesTampered()
		{
			if (ObscuredPrefs.onAlterationDetected != null)
			{
				ObscuredPrefs.onAlterationDetected();
				ObscuredPrefs.onAlterationDetected = null;
			}
		}

		// Token: 0x0600117D RID: 4477 RVA: 0x00010522 File Offset: 0x0000E722
		private static void PossibleForeignSavesDetected()
		{
			if (ObscuredPrefs.onPossibleForeignSavesDetected != null && !ObscuredPrefs.foreignSavesReported)
			{
				ObscuredPrefs.foreignSavesReported = true;
				ObscuredPrefs.onPossibleForeignSavesDetected();
			}
		}

		// Token: 0x0600117E RID: 4478 RVA: 0x00076A34 File Offset: 0x00074C34
		private static string GetDeviceId()
		{
			string text = string.Empty;
			if (string.IsNullOrEmpty(text))
			{
				text = SystemInfo.deviceUniqueIdentifier;
			}
			return text;
		}

		// Token: 0x0600117F RID: 4479 RVA: 0x00076A5C File Offset: 0x00074C5C
		private static byte[] EncryptDecryptBytes(byte[] bytes, int dataLength, string key)
		{
			int length = key.Length;
			byte[] array = new byte[dataLength];
			for (int i = 0; i < dataLength; i++)
			{
				array[i] = (byte)((char)bytes[i] ^ key[i % length]);
			}
			return array;
		}

		// Token: 0x06001180 RID: 4480 RVA: 0x00076A9C File Offset: 0x00074C9C
		private static string DeprecatedDecryptValue(string value)
		{
			string[] array = value.Split(new char[]
			{
				':'
			});
			if (array.Length < 2)
			{
				ObscuredPrefs.SavesTampered();
				return string.Empty;
			}
			string text = array[0];
			string a = array[1];
			byte[] array2;
			try
			{
				array2 = Convert.FromBase64String(text);
			}
			catch
			{
				ObscuredPrefs.SavesTampered();
				return string.Empty;
			}
			string @string = Encoding.UTF8.GetString(array2, 0, array2.Length);
			string result = ObscuredString.EncryptDecrypt(@string, ObscuredPrefs.cryptoKey);
			if (array.Length == 3)
			{
				if (a != ObscuredPrefs.DeprecatedCalculateChecksum(text + ObscuredPrefs.DeprecatedDeviceId))
				{
					ObscuredPrefs.SavesTampered();
				}
			}
			else if (array.Length == 2)
			{
				if (a != ObscuredPrefs.DeprecatedCalculateChecksum(text))
				{
					ObscuredPrefs.SavesTampered();
				}
			}
			else
			{
				ObscuredPrefs.SavesTampered();
			}
			if (ObscuredPrefs.lockToDevice != ObscuredPrefs.DeviceLockLevel.None && !ObscuredPrefs.emergencyMode)
			{
				if (array.Length >= 3)
				{
					string a2 = array[2];
					if (a2 != ObscuredPrefs.DeprecatedDeviceId)
					{
						if (!ObscuredPrefs.readForeignSaves)
						{
							result = string.Empty;
						}
						ObscuredPrefs.PossibleForeignSavesDetected();
					}
				}
				else if (ObscuredPrefs.lockToDevice == ObscuredPrefs.DeviceLockLevel.Strict)
				{
					if (!ObscuredPrefs.readForeignSaves)
					{
						result = string.Empty;
					}
					ObscuredPrefs.PossibleForeignSavesDetected();
				}
				else if (a != ObscuredPrefs.DeprecatedCalculateChecksum(text))
				{
					if (!ObscuredPrefs.readForeignSaves)
					{
						result = string.Empty;
					}
					ObscuredPrefs.PossibleForeignSavesDetected();
				}
			}
			return result;
		}

		// Token: 0x06001181 RID: 4481 RVA: 0x00076C20 File Offset: 0x00074E20
		private static string DeprecatedCalculateChecksum(string input)
		{
			int num = 0;
			byte[] bytes = Encoding.UTF8.GetBytes(input + ObscuredPrefs.cryptoKey);
			int num2 = bytes.Length;
			int num3 = ObscuredPrefs.cryptoKey.Length ^ 64;
			for (int i = 0; i < num2; i++)
			{
				byte b = bytes[i];
				num += (int)b + (int)b * (i + num3) % 3;
			}
			return num.ToString("X2");
		}

		// Token: 0x17000312 RID: 786
		// (get) Token: 0x06001182 RID: 4482 RVA: 0x00010548 File Offset: 0x0000E748
		private static string DeprecatedDeviceId
		{
			get
			{
				if (string.IsNullOrEmpty(ObscuredPrefs.deprecatedDeviceId))
				{
					ObscuredPrefs.deprecatedDeviceId = ObscuredPrefs.DeprecatedCalculateChecksum(ObscuredPrefs.DeviceId);
				}
				return ObscuredPrefs.deprecatedDeviceId;
			}
		}

		// Token: 0x04000B8E RID: 2958
		private const byte VERSION = 2;

		// Token: 0x04000B8F RID: 2959
		private const string RAW_NOT_FOUND = "{not_found}";

		// Token: 0x04000B90 RID: 2960
		private const string DATA_SEPARATOR = "|";

		// Token: 0x04000B91 RID: 2961
		private const char DEPRECATED_RAW_SEPARATOR = ':';

		// Token: 0x04000B92 RID: 2962
		private static bool foreignSavesReported;

		// Token: 0x04000B93 RID: 2963
		private static string cryptoKey = "63k3f97xf";

		// Token: 0x04000B94 RID: 2964
		private static string deviceId;

		// Token: 0x04000B95 RID: 2965
		private static uint deviceIdHash;

		// Token: 0x04000B96 RID: 2966
		public static Action onAlterationDetected;

		// Token: 0x04000B97 RID: 2967
		public static bool preservePlayerPrefs;

		// Token: 0x04000B98 RID: 2968
		public static Action onPossibleForeignSavesDetected;

		// Token: 0x04000B99 RID: 2969
		public static ObscuredPrefs.DeviceLockLevel lockToDevice;

		// Token: 0x04000B9A RID: 2970
		public static bool readForeignSaves;

		// Token: 0x04000B9B RID: 2971
		public static bool emergencyMode;

		// Token: 0x04000B9C RID: 2972
		private static string deprecatedDeviceId;

		// Token: 0x020001C8 RID: 456
		internal enum DataType : byte
		{
			// Token: 0x04000B9E RID: 2974
			Unknown,
			// Token: 0x04000B9F RID: 2975
			Int = 5,
			// Token: 0x04000BA0 RID: 2976
			UInt = 10,
			// Token: 0x04000BA1 RID: 2977
			String = 15,
			// Token: 0x04000BA2 RID: 2978
			Float = 20,
			// Token: 0x04000BA3 RID: 2979
			Double = 25,
			// Token: 0x04000BA4 RID: 2980
			Long = 30,
			// Token: 0x04000BA5 RID: 2981
			Bool = 35,
			// Token: 0x04000BA6 RID: 2982
			ByteArray = 40,
			// Token: 0x04000BA7 RID: 2983
			Vector2 = 45,
			// Token: 0x04000BA8 RID: 2984
			Vector3 = 50,
			// Token: 0x04000BA9 RID: 2985
			Quaternion = 55,
			// Token: 0x04000BAA RID: 2986
			Color = 60,
			// Token: 0x04000BAB RID: 2987
			Rect = 65
		}

		// Token: 0x020001C9 RID: 457
		public enum DeviceLockLevel : byte
		{
			// Token: 0x04000BAD RID: 2989
			None,
			// Token: 0x04000BAE RID: 2990
			Soft,
			// Token: 0x04000BAF RID: 2991
			Strict
		}
	}
}
