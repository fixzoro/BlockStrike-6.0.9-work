using System;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	// Token: 0x020001BE RID: 446
	[Serializable]
	public struct ObscuredChar : IEquatable<ObscuredChar>
	{
		// Token: 0x06001095 RID: 4245 RVA: 0x0000FD2A File Offset: 0x0000DF2A
		private ObscuredChar(char value)
		{
			this.currentCryptoKey = ObscuredChar.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = '\0';
			this.inited = true;
		}

		// Token: 0x06001097 RID: 4247 RVA: 0x0000FD58 File Offset: 0x0000DF58
		public static void SetNewCryptoKey(char newKey)
		{
			ObscuredChar.cryptoKey = newKey;
		}

		// Token: 0x06001098 RID: 4248 RVA: 0x0000FD60 File Offset: 0x0000DF60
		public static char EncryptDecrypt(char value)
		{
			return ObscuredChar.EncryptDecrypt(value, '\0');
		}

		// Token: 0x06001099 RID: 4249 RVA: 0x0000FD69 File Offset: 0x0000DF69
		public static char EncryptDecrypt(char value, char key)
		{
			if (key == '\0')
			{
				return (char)(value ^ ObscuredChar.cryptoKey);
			}
			return (char)(value ^ key);
		}

		// Token: 0x0600109A RID: 4250 RVA: 0x0000FD7E File Offset: 0x0000DF7E
		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredChar.cryptoKey)
			{
				this.hiddenValue = ObscuredChar.EncryptDecrypt(this.InternalDecrypt(), ObscuredChar.cryptoKey);
				this.currentCryptoKey = ObscuredChar.cryptoKey;
			}
		}

		// Token: 0x0600109B RID: 4251 RVA: 0x00074750 File Offset: 0x00072950
		public void RandomizeCryptoKey()
		{
			char value = this.InternalDecrypt();
			this.currentCryptoKey = (char)UnityEngine.Random.Range(1, 65535);
			this.hiddenValue = ObscuredChar.EncryptDecrypt(value, this.currentCryptoKey);
		}

		// Token: 0x0600109C RID: 4252 RVA: 0x0000FDB1 File Offset: 0x0000DFB1
		public char GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		// Token: 0x0600109D RID: 4253 RVA: 0x0000FDBF File Offset: 0x0000DFBF
		public void SetEncrypted(char encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.IsRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		// Token: 0x0600109E RID: 4254 RVA: 0x00074788 File Offset: 0x00072988
		private char InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredChar.cryptoKey;
				this.hiddenValue = ObscuredChar.EncryptDecrypt('\0');
				this.fakeValue = '\0';
				this.inited = true;
			}
			char c = ObscuredChar.EncryptDecrypt(this.hiddenValue, this.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning && this.fakeValue != '\0' && c != this.fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return c;
		}

		// Token: 0x0600109F RID: 4255 RVA: 0x0000FDE5 File Offset: 0x0000DFE5
		public override bool Equals(object obj)
		{
			return obj is ObscuredChar && this.Equals((ObscuredChar)obj);
		}

		// Token: 0x060010A0 RID: 4256 RVA: 0x00074804 File Offset: 0x00072A04
		public bool Equals(ObscuredChar obj)
		{
			if (this.currentCryptoKey == obj.currentCryptoKey)
			{
				return this.hiddenValue == obj.hiddenValue;
			}
			return ObscuredChar.EncryptDecrypt(this.hiddenValue, this.currentCryptoKey) == ObscuredChar.EncryptDecrypt(obj.hiddenValue, obj.currentCryptoKey);
		}

		// Token: 0x060010A1 RID: 4257 RVA: 0x0007485C File Offset: 0x00072A5C
		public override string ToString()
		{
			return this.InternalDecrypt().ToString();
		}

		// Token: 0x060010A2 RID: 4258 RVA: 0x00074878 File Offset: 0x00072A78
		public string ToString(IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(provider);
		}

		// Token: 0x060010A3 RID: 4259 RVA: 0x00074894 File Offset: 0x00072A94
		public override int GetHashCode()
		{
			return this.InternalDecrypt().GetHashCode();
		}

		// Token: 0x060010A4 RID: 4260 RVA: 0x000748B0 File Offset: 0x00072AB0
		public static implicit operator ObscuredChar(char value)
		{
			ObscuredChar result = new ObscuredChar(ObscuredChar.EncryptDecrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		// Token: 0x060010A5 RID: 4261 RVA: 0x0000FE00 File Offset: 0x0000E000
		public static implicit operator char(ObscuredChar value)
		{
			return value.InternalDecrypt();
		}

		// Token: 0x060010A6 RID: 4262 RVA: 0x000748E0 File Offset: 0x00072AE0
		public static ObscuredChar operator ++(ObscuredChar input)
		{
			char value = (char)(input.InternalDecrypt() + '\u0001');
			input.hiddenValue = ObscuredChar.EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		// Token: 0x060010A7 RID: 4263 RVA: 0x00074920 File Offset: 0x00072B20
		public static ObscuredChar operator --(ObscuredChar input)
		{
			char value = (char)(input.InternalDecrypt() - '\u0001');
			input.hiddenValue = ObscuredChar.EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		// Token: 0x04000B62 RID: 2914
		private static char cryptoKey = '—';

		// Token: 0x04000B63 RID: 2915
		private char currentCryptoKey;

		// Token: 0x04000B64 RID: 2916
		private char hiddenValue;

		// Token: 0x04000B65 RID: 2917
		private char fakeValue;

		// Token: 0x04000B66 RID: 2918
		private bool inited;
	}
}
