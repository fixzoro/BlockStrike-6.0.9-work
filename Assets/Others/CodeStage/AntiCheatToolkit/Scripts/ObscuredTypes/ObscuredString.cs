using System;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	// Token: 0x020001CE RID: 462
	[Serializable]
	public sealed class ObscuredString
	{
		// Token: 0x060011BF RID: 4543 RVA: 0x00004734 File Offset: 0x00002934
		private ObscuredString()
		{
		}

		// Token: 0x060011C0 RID: 4544 RVA: 0x000107F5 File Offset: 0x0000E9F5
		private ObscuredString(byte[] value)
		{
			this.currentCryptoKey = ObscuredString.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = null;
			this.inited = true;
		}

#if UNITY_EDITOR
        // For internal Editor usage only (may be useful for drawers).
        public static string cryptoKeyEditor = cryptoKey;
#endif

        // Token: 0x060011C2 RID: 4546 RVA: 0x00010829 File Offset: 0x0000EA29
        public static void SetNewCryptoKey(string newKey)
		{
			ObscuredString.cryptoKey = newKey;
		}

		// Token: 0x060011C3 RID: 4547 RVA: 0x00010831 File Offset: 0x0000EA31
		public static string EncryptDecrypt(string value)
		{
			return ObscuredString.EncryptDecrypt(value, string.Empty);
		}

		// Token: 0x060011C4 RID: 4548 RVA: 0x0007745C File Offset: 0x0007565C
		public static string EncryptDecrypt(string value, string key)
		{
			if (string.IsNullOrEmpty(value))
			{
				return string.Empty;
			}
			if (string.IsNullOrEmpty(key))
			{
				key = ObscuredString.cryptoKey;
			}
			int length = key.Length;
			int length2 = value.Length;
			char[] array = new char[length2];
			for (int i = 0; i < length2; i++)
			{
				array[i] = (char)(value[i] ^ key[i % length]);
			}
			return new string(array);
		}

		// Token: 0x060011C5 RID: 4549 RVA: 0x0001083E File Offset: 0x0000EA3E
		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredString.cryptoKey)
			{
				this.hiddenValue = ObscuredString.InternalEncrypt(this.InternalDecrypt());
				this.currentCryptoKey = ObscuredString.cryptoKey;
			}
		}

		// Token: 0x060011C6 RID: 4550 RVA: 0x000774D0 File Offset: 0x000756D0
		public void RandomizeCryptoKey()
		{
			string value = this.InternalDecrypt();
			this.currentCryptoKey = UnityEngine.Random.Range(int.MinValue, int.MaxValue).ToString();
			this.hiddenValue = ObscuredString.InternalEncrypt(value, this.currentCryptoKey);
		}

		// Token: 0x060011C7 RID: 4551 RVA: 0x00010871 File Offset: 0x0000EA71
		public string GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return ObscuredString.GetString(this.hiddenValue);
		}

		// Token: 0x060011C8 RID: 4552 RVA: 0x00010884 File Offset: 0x0000EA84
		public void SetEncrypted(string encrypted)
		{
			this.inited = true;
			this.hiddenValue = ObscuredString.GetBytes(encrypted);
			if (ObscuredCheatingDetector.IsRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		// Token: 0x060011C9 RID: 4553 RVA: 0x000108AF File Offset: 0x0000EAAF
		private static byte[] InternalEncrypt(string value)
		{
			return ObscuredString.InternalEncrypt(value, ObscuredString.cryptoKey);
		}

		// Token: 0x060011CA RID: 4554 RVA: 0x000108BC File Offset: 0x0000EABC
		private static byte[] InternalEncrypt(string value, string key)
		{
			return ObscuredString.GetBytes(ObscuredString.EncryptDecrypt(value, key));
		}

		// Token: 0x060011CB RID: 4555 RVA: 0x00077514 File Offset: 0x00075714
		private string InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredString.cryptoKey;
				this.hiddenValue = ObscuredString.InternalEncrypt(string.Empty);
				this.fakeValue = string.Empty;
				this.inited = true;
			}
			string text = this.currentCryptoKey;
			if (string.IsNullOrEmpty(text))
			{
				text = ObscuredString.cryptoKey;
			}
			string text2 = ObscuredString.EncryptDecrypt(ObscuredString.GetString(this.hiddenValue), text);
			if (ObscuredCheatingDetector.IsRunning && !string.IsNullOrEmpty(this.fakeValue) && text2 != this.fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return text2;
		}

		// Token: 0x060011CC RID: 4556 RVA: 0x000108CA File Offset: 0x0000EACA
		public override string ToString()
		{
			return this.InternalDecrypt();
		}

		// Token: 0x060011CD RID: 4557 RVA: 0x000108D2 File Offset: 0x0000EAD2
		public override bool Equals(object obj)
		{
			return obj is ObscuredString && this.Equals((ObscuredString)obj);
		}

		// Token: 0x060011CE RID: 4558 RVA: 0x000775BC File Offset: 0x000757BC
		public bool Equals(ObscuredString value)
		{
			if (value == null)
			{
				return false;
			}
			if (this.currentCryptoKey == value.currentCryptoKey)
			{
				return ObscuredString.ArraysEquals(this.hiddenValue, value.hiddenValue);
			}
			return string.Equals(this.InternalDecrypt(), value.InternalDecrypt());
		}

		// Token: 0x060011CF RID: 4559 RVA: 0x000108ED File Offset: 0x0000EAED
		public bool Equals(ObscuredString value, StringComparison comparisonType)
		{
			return !(value == null) && string.Equals(this.InternalDecrypt(), value.InternalDecrypt(), comparisonType);
		}

		// Token: 0x060011D0 RID: 4560 RVA: 0x0001090F File Offset: 0x0000EB0F
		public override int GetHashCode()
		{
			return this.InternalDecrypt().GetHashCode();
		}

		// Token: 0x060011D1 RID: 4561 RVA: 0x00077610 File Offset: 0x00075810
		private static byte[] GetBytes(string str)
		{
			byte[] array = new byte[str.Length * 2];
			Buffer.BlockCopy(str.ToCharArray(), 0, array, 0, array.Length);
			return array;
		}

		// Token: 0x060011D2 RID: 4562 RVA: 0x00077640 File Offset: 0x00075840
		private static string GetString(byte[] bytes)
		{
			char[] array = new char[bytes.Length / 2];
			Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
			return new string(array);
		}

		// Token: 0x060011D3 RID: 4563 RVA: 0x0007766C File Offset: 0x0007586C
		private static bool ArraysEquals(byte[] a1, byte[] a2)
		{
			if (a1 == a2)
			{
				return true;
			}
			if (a1 == null || a2 == null)
			{
				return false;
			}
			if (a1.Length != a2.Length)
			{
				return false;
			}
			for (int i = 0; i < a1.Length; i++)
			{
				if (a1[i] != a2[i])
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060011D4 RID: 4564 RVA: 0x000776C0 File Offset: 0x000758C0
		public static implicit operator ObscuredString(string value)
		{
			if (value == null)
			{
				return null;
			}
			ObscuredString obscuredString = new ObscuredString(ObscuredString.InternalEncrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				obscuredString.fakeValue = value;
			}
			return obscuredString;
		}

		// Token: 0x060011D5 RID: 4565 RVA: 0x0001091C File Offset: 0x0000EB1C
		public static implicit operator string(ObscuredString value)
		{
			if (value == null)
			{
				return null;
			}
			return value.InternalDecrypt();
		}

		// Token: 0x060011D6 RID: 4566 RVA: 0x000776F4 File Offset: 0x000758F4
		public static bool operator ==(ObscuredString a, ObscuredString b)
		{
			if (object.ReferenceEquals(a, b))
			{
				return true;
			}
			if (a == null || b == null)
			{
				return false;
			}
			if (a.currentCryptoKey == b.currentCryptoKey)
			{
				return ObscuredString.ArraysEquals(a.hiddenValue, b.hiddenValue);
			}
			return string.Equals(a.InternalDecrypt(), b.InternalDecrypt());
		}

		// Token: 0x060011D7 RID: 4567 RVA: 0x00010932 File Offset: 0x0000EB32
		public static bool operator !=(ObscuredString a, ObscuredString b)
		{
			return !(a == b);
		}

		// Token: 0x04000BC4 RID: 3012
		private static string cryptoKey = "4441";

		// Token: 0x04000BC5 RID: 3013
		[SerializeField]
		private string currentCryptoKey;

		// Token: 0x04000BC6 RID: 3014
		[SerializeField]
		private byte[] hiddenValue;

		// Token: 0x04000BC7 RID: 3015
		[SerializeField]
		private string fakeValue;

		// Token: 0x04000BC8 RID: 3016
		[SerializeField]
		private bool inited;
	}
}
