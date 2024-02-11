using System;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	// Token: 0x020001C6 RID: 454
	[Serializable]
	public struct ObscuredLong : IFormattable, IEquatable<ObscuredLong>
	{
		// Token: 0x0600110E RID: 4366 RVA: 0x00010145 File Offset: 0x0000E345
		private ObscuredLong(long value)
		{
			this.currentCryptoKey = ObscuredLong.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = 0L;
			this.inited = true;
		}

		// Token: 0x06001110 RID: 4368 RVA: 0x00010175 File Offset: 0x0000E375
		public static void SetNewCryptoKey(long newKey)
		{
			ObscuredLong.cryptoKey = newKey;
		}

		// Token: 0x06001111 RID: 4369 RVA: 0x0001017D File Offset: 0x0000E37D
		public static long Encrypt(long value)
		{
			return ObscuredLong.Encrypt(value, 0L);
		}

		// Token: 0x06001112 RID: 4370 RVA: 0x00010187 File Offset: 0x0000E387
		public static long Decrypt(long value)
		{
			return ObscuredLong.Decrypt(value, 0L);
		}

		// Token: 0x06001113 RID: 4371 RVA: 0x00010191 File Offset: 0x0000E391
		public static long Encrypt(long value, long key)
		{
			if (key == 0L)
			{
				return value ^ ObscuredLong.cryptoKey;
			}
			return value ^ key;
		}

		// Token: 0x06001114 RID: 4372 RVA: 0x00010191 File Offset: 0x0000E391
		public static long Decrypt(long value, long key)
		{
			if (key == 0L)
			{
				return value ^ ObscuredLong.cryptoKey;
			}
			return value ^ key;
		}

		// Token: 0x06001115 RID: 4373 RVA: 0x000101A4 File Offset: 0x0000E3A4
		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredLong.cryptoKey)
			{
				this.hiddenValue = ObscuredLong.Encrypt(this.InternalDecrypt(), ObscuredLong.cryptoKey);
				this.currentCryptoKey = ObscuredLong.cryptoKey;
			}
		}

		// Token: 0x06001116 RID: 4374 RVA: 0x0007570C File Offset: 0x0007390C
		public void RandomizeCryptoKey()
		{
			long value = this.InternalDecrypt();
			do
			{
				this.currentCryptoKey = (long)UnityEngine.Random.Range(int.MinValue, int.MaxValue);
			}
			while (this.currentCryptoKey == 0L);
			this.hiddenValue = ObscuredLong.Encrypt(value, this.currentCryptoKey);
		}

		// Token: 0x06001117 RID: 4375 RVA: 0x000101D7 File Offset: 0x0000E3D7
		public long GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		// Token: 0x06001118 RID: 4376 RVA: 0x000101E5 File Offset: 0x0000E3E5
		public void SetEncrypted(long encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.IsRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		// Token: 0x06001119 RID: 4377 RVA: 0x00075754 File Offset: 0x00073954
		private long InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredLong.cryptoKey;
				this.hiddenValue = ObscuredLong.Encrypt(0L);
				this.fakeValue = 0L;
				this.inited = true;
			}
			long num = ObscuredLong.Decrypt(this.hiddenValue, this.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning && this.fakeValue != 0L && num != this.fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return num;
		}

		// Token: 0x0600111A RID: 4378 RVA: 0x0001020B File Offset: 0x0000E40B
		public override bool Equals(object obj)
		{
			return obj is ObscuredLong && this.Equals((ObscuredLong)obj);
		}

		// Token: 0x0600111B RID: 4379 RVA: 0x000757D4 File Offset: 0x000739D4
		public bool Equals(ObscuredLong obj)
		{
			if (this.currentCryptoKey == obj.currentCryptoKey)
			{
				return this.hiddenValue == obj.hiddenValue;
			}
			return ObscuredLong.Decrypt(this.hiddenValue, this.currentCryptoKey) == ObscuredLong.Decrypt(obj.hiddenValue, obj.currentCryptoKey);
		}

		// Token: 0x0600111C RID: 4380 RVA: 0x0007582C File Offset: 0x00073A2C
		public override int GetHashCode()
		{
			return this.InternalDecrypt().GetHashCode();
		}

		// Token: 0x0600111D RID: 4381 RVA: 0x00075848 File Offset: 0x00073A48
		public override string ToString()
		{
			return this.InternalDecrypt().ToString();
		}

		// Token: 0x0600111E RID: 4382 RVA: 0x00075864 File Offset: 0x00073A64
		public string ToString(string format)
		{
			return this.InternalDecrypt().ToString(format);
		}

		// Token: 0x0600111F RID: 4383 RVA: 0x00075880 File Offset: 0x00073A80
		public string ToString(IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(provider);
		}

		// Token: 0x06001120 RID: 4384 RVA: 0x0007589C File Offset: 0x00073A9C
		public string ToString(string format, IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(format, provider);
		}

		// Token: 0x06001121 RID: 4385 RVA: 0x000758BC File Offset: 0x00073ABC
		public static implicit operator ObscuredLong(long value)
		{
			ObscuredLong result = new ObscuredLong(ObscuredLong.Encrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		// Token: 0x06001122 RID: 4386 RVA: 0x00010226 File Offset: 0x0000E426
		public static implicit operator long(ObscuredLong value)
		{
			return value.InternalDecrypt();
		}

		// Token: 0x06001123 RID: 4387 RVA: 0x000758EC File Offset: 0x00073AEC
		public static ObscuredLong operator ++(ObscuredLong input)
		{
			long value = input.InternalDecrypt() + 1L;
			input.hiddenValue = ObscuredLong.Encrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		// Token: 0x06001124 RID: 4388 RVA: 0x0007592C File Offset: 0x00073B2C
		public static ObscuredLong operator --(ObscuredLong input)
		{
			long value = input.InternalDecrypt() - 1L;
			input.hiddenValue = ObscuredLong.Encrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		// Token: 0x04000B89 RID: 2953
		private static long cryptoKey = 444442L;

		// Token: 0x04000B8A RID: 2954
		[SerializeField]
		private long currentCryptoKey;

		// Token: 0x04000B8B RID: 2955
		[SerializeField]
		private long hiddenValue;

		// Token: 0x04000B8C RID: 2956
		[SerializeField]
		private long fakeValue;

		// Token: 0x04000B8D RID: 2957
		[SerializeField]
		private bool inited;
	}
}
