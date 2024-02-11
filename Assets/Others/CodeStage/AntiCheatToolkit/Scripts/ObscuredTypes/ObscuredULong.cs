using System;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	// Token: 0x020001D0 RID: 464
	[Serializable]
	public struct ObscuredULong : IFormattable, IEquatable<ObscuredULong>
	{
		// Token: 0x060011F0 RID: 4592 RVA: 0x00010A32 File Offset: 0x0000EC32
		private ObscuredULong(ulong value)
		{
			this.currentCryptoKey = ObscuredULong.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = 0UL;
			this.inited = true;
		}

		// Token: 0x060011F2 RID: 4594 RVA: 0x00010A62 File Offset: 0x0000EC62
		public static void SetNewCryptoKey(ulong newKey)
		{
			ObscuredULong.cryptoKey = newKey;
		}

		// Token: 0x060011F3 RID: 4595 RVA: 0x00010A6A File Offset: 0x0000EC6A
		public static ulong Encrypt(ulong value)
		{
			return ObscuredULong.Encrypt(value, 0UL);
		}

		// Token: 0x060011F4 RID: 4596 RVA: 0x00010A74 File Offset: 0x0000EC74
		public static ulong Decrypt(ulong value)
		{
			return ObscuredULong.Decrypt(value, 0UL);
		}

		// Token: 0x060011F5 RID: 4597 RVA: 0x00010A7E File Offset: 0x0000EC7E
		public static ulong Encrypt(ulong value, ulong key)
		{
			if (key == 0UL)
			{
				return value ^ ObscuredULong.cryptoKey;
			}
			return value ^ key;
		}

		// Token: 0x060011F6 RID: 4598 RVA: 0x00010A7E File Offset: 0x0000EC7E
		public static ulong Decrypt(ulong value, ulong key)
		{
			if (key == 0UL)
			{
				return value ^ ObscuredULong.cryptoKey;
			}
			return value ^ key;
		}

		// Token: 0x060011F7 RID: 4599 RVA: 0x00010A91 File Offset: 0x0000EC91
		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredULong.cryptoKey)
			{
				this.hiddenValue = ObscuredULong.Encrypt(this.InternalDecrypt(), ObscuredULong.cryptoKey);
				this.currentCryptoKey = ObscuredULong.cryptoKey;
			}
		}

		// Token: 0x060011F8 RID: 4600 RVA: 0x000779A4 File Offset: 0x00075BA4
		public void RandomizeCryptoKey()
		{
			ulong value = this.InternalDecrypt();
			this.currentCryptoKey = (ulong)((long)UnityEngine.Random.Range(1, int.MaxValue));
			this.hiddenValue = ObscuredULong.Encrypt(value, this.currentCryptoKey);
		}

		// Token: 0x060011F9 RID: 4601 RVA: 0x00010AC4 File Offset: 0x0000ECC4
		public ulong GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		// Token: 0x060011FA RID: 4602 RVA: 0x00010AD2 File Offset: 0x0000ECD2
		public void SetEncrypted(ulong encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.IsRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		// Token: 0x060011FB RID: 4603 RVA: 0x000779DC File Offset: 0x00075BDC
		private ulong InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredULong.cryptoKey;
				this.hiddenValue = ObscuredULong.Encrypt(0UL);
				this.fakeValue = 0UL;
				this.inited = true;
			}
			ulong num = ObscuredULong.Decrypt(this.hiddenValue, this.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning && this.fakeValue != 0UL && num != this.fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return num;
		}

		// Token: 0x060011FC RID: 4604 RVA: 0x00010AF8 File Offset: 0x0000ECF8
		public override bool Equals(object obj)
		{
			return obj is ObscuredULong && this.Equals((ObscuredULong)obj);
		}

		// Token: 0x060011FD RID: 4605 RVA: 0x00077A5C File Offset: 0x00075C5C
		public bool Equals(ObscuredULong obj)
		{
			if (this.currentCryptoKey == obj.currentCryptoKey)
			{
				return this.hiddenValue == obj.hiddenValue;
			}
			return ObscuredULong.Decrypt(this.hiddenValue, this.currentCryptoKey) == ObscuredULong.Decrypt(obj.hiddenValue, obj.currentCryptoKey);
		}

		// Token: 0x060011FE RID: 4606 RVA: 0x00077AB4 File Offset: 0x00075CB4
		public override int GetHashCode()
		{
			return this.InternalDecrypt().GetHashCode();
		}

		// Token: 0x060011FF RID: 4607 RVA: 0x00077AD0 File Offset: 0x00075CD0
		public override string ToString()
		{
			return this.InternalDecrypt().ToString();
		}

		// Token: 0x06001200 RID: 4608 RVA: 0x00077AEC File Offset: 0x00075CEC
		public string ToString(string format)
		{
			return this.InternalDecrypt().ToString(format);
		}

		// Token: 0x06001201 RID: 4609 RVA: 0x00077B08 File Offset: 0x00075D08
		public string ToString(IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(provider);
		}

		// Token: 0x06001202 RID: 4610 RVA: 0x00077B24 File Offset: 0x00075D24
		public string ToString(string format, IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(format, provider);
		}

		// Token: 0x06001203 RID: 4611 RVA: 0x00077B44 File Offset: 0x00075D44
		public static implicit operator ObscuredULong(ulong value)
		{
			ObscuredULong result = new ObscuredULong(ObscuredULong.Encrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		// Token: 0x06001204 RID: 4612 RVA: 0x00010B13 File Offset: 0x0000ED13
		public static implicit operator ulong(ObscuredULong value)
		{
			return value.InternalDecrypt();
		}

		// Token: 0x06001205 RID: 4613 RVA: 0x00077B74 File Offset: 0x00075D74
		public static ObscuredULong operator ++(ObscuredULong input)
		{
			ulong value = input.InternalDecrypt() + 1UL;
			input.hiddenValue = ObscuredULong.Encrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		// Token: 0x06001206 RID: 4614 RVA: 0x00077BB4 File Offset: 0x00075DB4
		public static ObscuredULong operator --(ObscuredULong input)
		{
			ulong value = input.InternalDecrypt() - 1UL;
			input.hiddenValue = ObscuredULong.Encrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		// Token: 0x04000BCE RID: 3022
		private static ulong cryptoKey = 444443UL;

		// Token: 0x04000BCF RID: 3023
		private ulong currentCryptoKey;

		// Token: 0x04000BD0 RID: 3024
		private ulong hiddenValue;

		// Token: 0x04000BD1 RID: 3025
		private ulong fakeValue;

		// Token: 0x04000BD2 RID: 3026
		private bool inited;
	}
}
