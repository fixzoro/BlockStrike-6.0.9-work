using System;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	// Token: 0x020001C5 RID: 453
	[Serializable]
	public struct ObscuredInt : IFormattable, IEquatable<ObscuredInt>
	{
		// Token: 0x060010F4 RID: 4340 RVA: 0x00075418 File Offset: 0x00073618
		private ObscuredInt(int value)
		{
			if (ObscuredInt.randomCryptoKey)
			{
				int num = ObscuredInt.cryptoKey + value;
				num ^= num << 21;
				num ^= num >> 3;
				num ^= num << 4;
				this.currentCryptoKey = num;
			}
			else
			{
				this.currentCryptoKey = ObscuredInt.cryptoKey;
			}
			this.hiddenValue = value;
			this.fakeValue = 0;
			this.inited = true;
		}

#if UNITY_EDITOR
        // For internal Editor usage only (may be useful for drawers).
        public static int cryptoKeyEditor = cryptoKey;
#endif

        // Token: 0x060010F6 RID: 4342 RVA: 0x00010061 File Offset: 0x0000E261
        public static void SetNewCryptoKey(int newKey)
		{
			ObscuredInt.cryptoKey = newKey;
		}

		// Token: 0x060010F7 RID: 4343 RVA: 0x00010069 File Offset: 0x0000E269
		public static int Encrypt(int value)
		{
			return ObscuredInt.Encrypt(value, 0);
		}

		// Token: 0x060010F8 RID: 4344 RVA: 0x00010072 File Offset: 0x0000E272
		public static int Encrypt(int value, int key)
		{
			if (key == 0)
			{
				return value ^ ObscuredInt.cryptoKey;
			}
			return value ^ key;
		}

		// Token: 0x060010F9 RID: 4345 RVA: 0x00010085 File Offset: 0x0000E285
		public static int Decrypt(int value)
		{
			return ObscuredInt.Decrypt(value, 0);
		}

		// Token: 0x060010FA RID: 4346 RVA: 0x00010072 File Offset: 0x0000E272
		public static int Decrypt(int value, int key)
		{
			if (key == 0)
			{
				return value ^ ObscuredInt.cryptoKey;
			}
			return value ^ key;
		}

		// Token: 0x060010FB RID: 4347 RVA: 0x0001008E File Offset: 0x0000E28E
		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredInt.cryptoKey)
			{
				this.hiddenValue = ObscuredInt.Encrypt(this.InternalDecrypt(), ObscuredInt.cryptoKey);
				this.currentCryptoKey = ObscuredInt.cryptoKey;
			}
		}

		// Token: 0x060010FC RID: 4348 RVA: 0x00075478 File Offset: 0x00073678
		public void RandomizeCryptoKey()
		{
			this.hiddenValue = this.InternalDecrypt();
			do
			{
				this.currentCryptoKey = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
			}
			while (this.currentCryptoKey == 0);
			this.hiddenValue = ObscuredInt.Encrypt(this.hiddenValue, this.currentCryptoKey);
		}

		// Token: 0x060010FD RID: 4349 RVA: 0x000100C1 File Offset: 0x0000E2C1
		public int GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		// Token: 0x060010FE RID: 4350 RVA: 0x000100CF File Offset: 0x0000E2CF
		public void SetEncrypted(int encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.IsRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		// Token: 0x060010FF RID: 4351 RVA: 0x000754C8 File Offset: 0x000736C8
		private int InternalDecrypt()
		{
			if (!this.inited)
			{
				if (ObscuredInt.randomCryptoKey)
				{
					int num = ObscuredInt.cryptoKey;
					num ^= num << 21;
					num ^= num >> 3;
					num ^= num << 4;
					this.currentCryptoKey = num;
				}
				else
				{
					this.currentCryptoKey = ObscuredInt.cryptoKey;
				}
				this.hiddenValue = ObscuredInt.Encrypt(0);
				this.fakeValue = 0;
				this.inited = true;
			}
			int num2 = ObscuredInt.Decrypt(this.hiddenValue, this.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning && this.fakeValue != 0 && num2 != this.fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return num2;
		}

		// Token: 0x06001100 RID: 4352 RVA: 0x000100F5 File Offset: 0x0000E2F5
		public override bool Equals(object obj)
		{
			return obj is ObscuredInt && this.Equals((ObscuredInt)obj);
		}

		// Token: 0x06001101 RID: 4353 RVA: 0x00075574 File Offset: 0x00073774
		public bool Equals(ObscuredInt obj)
		{
			if (this.currentCryptoKey == obj.currentCryptoKey)
			{
				return this.hiddenValue == obj.hiddenValue;
			}
			return ObscuredInt.Decrypt(this.hiddenValue, this.currentCryptoKey) == ObscuredInt.Decrypt(obj.hiddenValue, obj.currentCryptoKey);
		}

		// Token: 0x06001102 RID: 4354 RVA: 0x000755CC File Offset: 0x000737CC
		public override int GetHashCode()
		{
			return this.InternalDecrypt().GetHashCode();
		}

		// Token: 0x06001103 RID: 4355 RVA: 0x000755E8 File Offset: 0x000737E8
		public override string ToString()
		{
			return this.InternalDecrypt().ToString();
		}

		// Token: 0x06001104 RID: 4356 RVA: 0x00075604 File Offset: 0x00073804
		public string ToString(string format)
		{
			return this.InternalDecrypt().ToString(format);
		}

		// Token: 0x06001105 RID: 4357 RVA: 0x00075620 File Offset: 0x00073820
		public string ToString(IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(provider);
		}

		// Token: 0x06001106 RID: 4358 RVA: 0x0007563C File Offset: 0x0007383C
		public string ToString(string format, IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(format, provider);
		}

		// Token: 0x06001107 RID: 4359 RVA: 0x0007565C File Offset: 0x0007385C
		public static implicit operator ObscuredInt(int value)
		{
			ObscuredInt result = new ObscuredInt(ObscuredInt.Encrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		// Token: 0x06001108 RID: 4360 RVA: 0x00010110 File Offset: 0x0000E310
		public static implicit operator int(ObscuredInt value)
		{
			return value.InternalDecrypt();
		}

		// Token: 0x06001109 RID: 4361 RVA: 0x00010119 File Offset: 0x0000E319
		public static implicit operator ObscuredFloat(ObscuredInt value)
		{
			return (float)value.InternalDecrypt();
		}

		// Token: 0x0600110A RID: 4362 RVA: 0x00010128 File Offset: 0x0000E328
		public static implicit operator ObscuredDouble(ObscuredInt value)
		{
			return (double)value.InternalDecrypt();
		}

		// Token: 0x0600110B RID: 4363 RVA: 0x00010137 File Offset: 0x0000E337
		public static explicit operator ObscuredUInt(ObscuredInt value)
		{
			return (uint)value.InternalDecrypt();
		}

		// Token: 0x0600110C RID: 4364 RVA: 0x0007568C File Offset: 0x0007388C
		public static ObscuredInt operator ++(ObscuredInt input)
		{
			int value = input.InternalDecrypt() + 1;
			input.hiddenValue = ObscuredInt.Encrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		// Token: 0x0600110D RID: 4365 RVA: 0x000756CC File Offset: 0x000738CC
		public static ObscuredInt operator --(ObscuredInt input)
		{
			int value = input.InternalDecrypt() - 1;
			input.hiddenValue = ObscuredInt.Encrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		// Token: 0x04000B83 RID: 2947
		private static int cryptoKey = 244444;

		// Token: 0x04000B84 RID: 2948
		public static bool randomCryptoKey;

		// Token: 0x04000B85 RID: 2949
		[SerializeField]
		private int currentCryptoKey;

		// Token: 0x04000B86 RID: 2950
		[SerializeField]
		private int hiddenValue;

		// Token: 0x04000B87 RID: 2951
		[SerializeField]
		private int fakeValue;

		// Token: 0x04000B88 RID: 2952
		[SerializeField]
		private bool inited;
	}
}
