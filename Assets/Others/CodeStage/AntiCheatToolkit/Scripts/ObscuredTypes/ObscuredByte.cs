using System;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	// Token: 0x020001BD RID: 445
	[Serializable]
	public struct ObscuredByte : IFormattable, IEquatable<ObscuredByte>
	{
		// Token: 0x06001080 RID: 4224 RVA: 0x0000FC4B File Offset: 0x0000DE4B
		private ObscuredByte(byte value)
		{
			this.currentCryptoKey = ObscuredByte.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = 0;
			this.inited = true;
		}

		// Token: 0x06001082 RID: 4226 RVA: 0x0000FC79 File Offset: 0x0000DE79
		public static void SetNewCryptoKey(byte newKey)
		{
			ObscuredByte.cryptoKey = newKey;
		}

		// Token: 0x06001083 RID: 4227 RVA: 0x0000FC81 File Offset: 0x0000DE81
		public static byte EncryptDecrypt(byte value)
		{
			return ObscuredByte.EncryptDecrypt(value, 0);
		}

		// Token: 0x06001084 RID: 4228 RVA: 0x0000FC8A File Offset: 0x0000DE8A
		public static byte EncryptDecrypt(byte value, byte key)
		{
			if (key == 0)
			{
				return (byte)(value ^ ObscuredByte.cryptoKey);
			}
			return (byte)(value ^ key);
		}

		// Token: 0x06001085 RID: 4229 RVA: 0x0000FC9F File Offset: 0x0000DE9F
		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredByte.cryptoKey)
			{
				this.hiddenValue = ObscuredByte.EncryptDecrypt(this.InternalDecrypt(), ObscuredByte.cryptoKey);
				this.currentCryptoKey = ObscuredByte.cryptoKey;
			}
		}

		// Token: 0x06001086 RID: 4230 RVA: 0x00074504 File Offset: 0x00072704
		public void RandomizeCryptoKey()
		{
			byte value = this.InternalDecrypt();
			this.currentCryptoKey = (byte)UnityEngine.Random.Range(1, 255);
			this.hiddenValue = ObscuredByte.EncryptDecrypt(value, this.currentCryptoKey);
		}

		// Token: 0x06001087 RID: 4231 RVA: 0x0000FCD2 File Offset: 0x0000DED2
		public byte GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		// Token: 0x06001088 RID: 4232 RVA: 0x0000FCE0 File Offset: 0x0000DEE0
		public void SetEncrypted(byte encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.IsRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		// Token: 0x06001089 RID: 4233 RVA: 0x0007453C File Offset: 0x0007273C
		private byte InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredByte.cryptoKey;
				this.hiddenValue = ObscuredByte.EncryptDecrypt(0);
				this.fakeValue = 0;
				this.inited = true;
			}
			byte b = ObscuredByte.EncryptDecrypt(this.hiddenValue, this.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning && this.fakeValue != 0 && b != this.fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return b;
		}

		// Token: 0x0600108A RID: 4234 RVA: 0x0000FD06 File Offset: 0x0000DF06
		public override bool Equals(object obj)
		{
			return obj is ObscuredByte && this.Equals((ObscuredByte)obj);
		}

		// Token: 0x0600108B RID: 4235 RVA: 0x000745B8 File Offset: 0x000727B8
		public bool Equals(ObscuredByte obj)
		{
			if (this.currentCryptoKey == obj.currentCryptoKey)
			{
				return this.hiddenValue == obj.hiddenValue;
			}
			return ObscuredByte.EncryptDecrypt(this.hiddenValue, this.currentCryptoKey) == ObscuredByte.EncryptDecrypt(obj.hiddenValue, obj.currentCryptoKey);
		}

		// Token: 0x0600108C RID: 4236 RVA: 0x00074610 File Offset: 0x00072810
		public override string ToString()
		{
			return this.InternalDecrypt().ToString();
		}

		// Token: 0x0600108D RID: 4237 RVA: 0x0007462C File Offset: 0x0007282C
		public string ToString(string format)
		{
			return this.InternalDecrypt().ToString(format);
		}

		// Token: 0x0600108E RID: 4238 RVA: 0x00074648 File Offset: 0x00072848
		public override int GetHashCode()
		{
			return this.InternalDecrypt().GetHashCode();
		}

		// Token: 0x0600108F RID: 4239 RVA: 0x00074664 File Offset: 0x00072864
		public string ToString(IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(provider);
		}

		// Token: 0x06001090 RID: 4240 RVA: 0x00074680 File Offset: 0x00072880
		public string ToString(string format, IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(format, provider);
		}

		// Token: 0x06001091 RID: 4241 RVA: 0x000746A0 File Offset: 0x000728A0
		public static implicit operator ObscuredByte(byte value)
		{
			ObscuredByte result = new ObscuredByte(ObscuredByte.EncryptDecrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		// Token: 0x06001092 RID: 4242 RVA: 0x0000FD21 File Offset: 0x0000DF21
		public static implicit operator byte(ObscuredByte value)
		{
			return value.InternalDecrypt();
		}

		// Token: 0x06001093 RID: 4243 RVA: 0x000746D0 File Offset: 0x000728D0
		public static ObscuredByte operator ++(ObscuredByte input)
		{
			byte value = (byte)(input.InternalDecrypt() + 1);
			input.hiddenValue = ObscuredByte.EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		// Token: 0x06001094 RID: 4244 RVA: 0x00074710 File Offset: 0x00072910
		public static ObscuredByte operator --(ObscuredByte input)
		{
			byte value = (byte)(input.InternalDecrypt() - 1);
			input.hiddenValue = ObscuredByte.EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		// Token: 0x04000B5D RID: 2909
		private static byte cryptoKey = 244;

		// Token: 0x04000B5E RID: 2910
		private byte currentCryptoKey;

		// Token: 0x04000B5F RID: 2911
		private byte hiddenValue;

		// Token: 0x04000B60 RID: 2912
		private byte fakeValue;

		// Token: 0x04000B61 RID: 2913
		private bool inited;
	}
}
