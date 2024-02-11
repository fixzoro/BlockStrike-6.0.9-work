using System;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	// Token: 0x020001CD RID: 461
	[Serializable]
	public struct ObscuredShort : IFormattable, IEquatable<ObscuredShort>
	{
		// Token: 0x060011AA RID: 4522 RVA: 0x00010716 File Offset: 0x0000E916
		private ObscuredShort(short value)
		{
			this.currentCryptoKey = ObscuredShort.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = 0;
			this.inited = true;
		}

		// Token: 0x060011AC RID: 4524 RVA: 0x00010744 File Offset: 0x0000E944
		public static void SetNewCryptoKey(short newKey)
		{
			ObscuredShort.cryptoKey = newKey;
		}

		// Token: 0x060011AD RID: 4525 RVA: 0x0001074C File Offset: 0x0000E94C
		public static short EncryptDecrypt(short value)
		{
			return ObscuredShort.EncryptDecrypt(value, 0);
		}

		// Token: 0x060011AE RID: 4526 RVA: 0x00010755 File Offset: 0x0000E955
		public static short EncryptDecrypt(short value, short key)
		{
			if (key == 0)
			{
				return (short)(value ^ ObscuredShort.cryptoKey);
			}
			return (short)(value ^ key);
		}

		// Token: 0x060011AF RID: 4527 RVA: 0x0001076A File Offset: 0x0000E96A
		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredShort.cryptoKey)
			{
				this.hiddenValue = ObscuredShort.EncryptDecrypt(this.InternalDecrypt(), ObscuredShort.cryptoKey);
				this.currentCryptoKey = ObscuredShort.cryptoKey;
			}
		}

		// Token: 0x060011B0 RID: 4528 RVA: 0x00077210 File Offset: 0x00075410
		public void RandomizeCryptoKey()
		{
			short value = this.InternalDecrypt();
			do
			{
				this.currentCryptoKey = (short)UnityEngine.Random.Range(-32768, 32767);
			}
			while (this.currentCryptoKey == 0);
			this.hiddenValue = ObscuredShort.EncryptDecrypt(value, this.currentCryptoKey);
		}

		// Token: 0x060011B1 RID: 4529 RVA: 0x0001079D File Offset: 0x0000E99D
		public short GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		// Token: 0x060011B2 RID: 4530 RVA: 0x000107AB File Offset: 0x0000E9AB
		public void SetEncrypted(short encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.IsRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		// Token: 0x060011B3 RID: 4531 RVA: 0x00077258 File Offset: 0x00075458
		private short InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredShort.cryptoKey;
				this.hiddenValue = ObscuredShort.EncryptDecrypt(0);
				this.fakeValue = 0;
				this.inited = true;
			}
			short num = ObscuredShort.EncryptDecrypt(this.hiddenValue, this.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning && this.fakeValue != 0 && num != this.fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return num;
		}

		// Token: 0x060011B4 RID: 4532 RVA: 0x000107D1 File Offset: 0x0000E9D1
		public override bool Equals(object obj)
		{
			return obj is ObscuredShort && this.Equals((ObscuredShort)obj);
		}

		// Token: 0x060011B5 RID: 4533 RVA: 0x000772D4 File Offset: 0x000754D4
		public bool Equals(ObscuredShort obj)
		{
			if (this.currentCryptoKey == obj.currentCryptoKey)
			{
				return this.hiddenValue == obj.hiddenValue;
			}
			return ObscuredShort.EncryptDecrypt(this.hiddenValue, this.currentCryptoKey) == ObscuredShort.EncryptDecrypt(obj.hiddenValue, obj.currentCryptoKey);
		}

		// Token: 0x060011B6 RID: 4534 RVA: 0x0007732C File Offset: 0x0007552C
		public override string ToString()
		{
			return this.InternalDecrypt().ToString();
		}

		// Token: 0x060011B7 RID: 4535 RVA: 0x00077348 File Offset: 0x00075548
		public string ToString(string format)
		{
			return this.InternalDecrypt().ToString(format);
		}

		// Token: 0x060011B8 RID: 4536 RVA: 0x00077364 File Offset: 0x00075564
		public override int GetHashCode()
		{
			return this.InternalDecrypt().GetHashCode();
		}

		// Token: 0x060011B9 RID: 4537 RVA: 0x00077380 File Offset: 0x00075580
		public string ToString(IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(provider);
		}

		// Token: 0x060011BA RID: 4538 RVA: 0x0007739C File Offset: 0x0007559C
		public string ToString(string format, IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(format, provider);
		}

		// Token: 0x060011BB RID: 4539 RVA: 0x000773BC File Offset: 0x000755BC
		public static implicit operator ObscuredShort(short value)
		{
			ObscuredShort result = new ObscuredShort(ObscuredShort.EncryptDecrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		// Token: 0x060011BC RID: 4540 RVA: 0x000107EC File Offset: 0x0000E9EC
		public static implicit operator short(ObscuredShort value)
		{
			return value.InternalDecrypt();
		}

		// Token: 0x060011BD RID: 4541 RVA: 0x000773EC File Offset: 0x000755EC
		public static ObscuredShort operator ++(ObscuredShort input)
		{
			short value = (short)(input.InternalDecrypt() + 1);
			input.hiddenValue = ObscuredShort.EncryptDecrypt(value);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		// Token: 0x060011BE RID: 4542 RVA: 0x00077424 File Offset: 0x00075624
		public static ObscuredShort operator --(ObscuredShort input)
		{
			short value = (short)(input.InternalDecrypt() - 1);
			input.hiddenValue = ObscuredShort.EncryptDecrypt(value);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		// Token: 0x04000BBF RID: 3007
		private static short cryptoKey = 214;

		// Token: 0x04000BC0 RID: 3008
		private short currentCryptoKey;

		// Token: 0x04000BC1 RID: 3009
		private short hiddenValue;

		// Token: 0x04000BC2 RID: 3010
		private short fakeValue;

		// Token: 0x04000BC3 RID: 3011
		private bool inited;
	}
}
