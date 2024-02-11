using System;
using System.Runtime.InteropServices;
using CodeStage.AntiCheat.Common;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	// Token: 0x020001BF RID: 447
	[Serializable]
	public struct ObscuredDecimal : IFormattable, IEquatable<ObscuredDecimal>
	{
		// Token: 0x060010A8 RID: 4264 RVA: 0x0000FE09 File Offset: 0x0000E009
		private ObscuredDecimal(ACTkByte16 value)
		{
			this.currentCryptoKey = ObscuredDecimal.cryptoKey;
			this.hiddenValue = value;
			this.hiddenValueOld = null;
			this.fakeValue = 0m;
			this.inited = true;
		}

		// Token: 0x060010AA RID: 4266 RVA: 0x0000FE44 File Offset: 0x0000E044
		public static void SetNewCryptoKey(long newKey)
		{
			ObscuredDecimal.cryptoKey = newKey;
		}

		// Token: 0x060010AB RID: 4267 RVA: 0x0000FE4C File Offset: 0x0000E04C
		public static decimal Encrypt(decimal value)
		{
			return ObscuredDecimal.Encrypt(value, ObscuredDecimal.cryptoKey);
		}

		// Token: 0x060010AC RID: 4268 RVA: 0x00074960 File Offset: 0x00072B60
		public static decimal Encrypt(decimal value, long key)
		{
			ObscuredDecimal.DecimalLongBytesUnion decimalLongBytesUnion = default(ObscuredDecimal.DecimalLongBytesUnion);
			decimalLongBytesUnion.d = value;
			decimalLongBytesUnion.l1 ^= key;
			decimalLongBytesUnion.l2 ^= key;
			return decimalLongBytesUnion.d;
		}

		// Token: 0x060010AD RID: 4269 RVA: 0x0000FE59 File Offset: 0x0000E059
		private static ACTkByte16 InternalEncrypt(decimal value)
		{
			return ObscuredDecimal.InternalEncrypt(value, 0L);
		}

		// Token: 0x060010AE RID: 4270 RVA: 0x000749A4 File Offset: 0x00072BA4
		private static ACTkByte16 InternalEncrypt(decimal value, long key)
		{
			long num = key;
			if (num == 0L)
			{
				num = ObscuredDecimal.cryptoKey;
			}
			ObscuredDecimal.DecimalLongBytesUnion decimalLongBytesUnion = default(ObscuredDecimal.DecimalLongBytesUnion);
			decimalLongBytesUnion.d = value;
			decimalLongBytesUnion.l1 ^= num;
			decimalLongBytesUnion.l2 ^= num;
			return decimalLongBytesUnion.b16;
		}

		// Token: 0x060010AF RID: 4271 RVA: 0x0000FE63 File Offset: 0x0000E063
		public static decimal Decrypt(decimal value)
		{
			return ObscuredDecimal.Decrypt(value, ObscuredDecimal.cryptoKey);
		}

		// Token: 0x060010B0 RID: 4272 RVA: 0x00074960 File Offset: 0x00072B60
		public static decimal Decrypt(decimal value, long key)
		{
			ObscuredDecimal.DecimalLongBytesUnion decimalLongBytesUnion = default(ObscuredDecimal.DecimalLongBytesUnion);
			decimalLongBytesUnion.d = value;
			decimalLongBytesUnion.l1 ^= key;
			decimalLongBytesUnion.l2 ^= key;
			return decimalLongBytesUnion.d;
		}

		// Token: 0x060010B1 RID: 4273 RVA: 0x0000FE70 File Offset: 0x0000E070
		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredDecimal.cryptoKey)
			{
				this.hiddenValue = ObscuredDecimal.InternalEncrypt(this.InternalDecrypt(), ObscuredDecimal.cryptoKey);
				this.currentCryptoKey = ObscuredDecimal.cryptoKey;
			}
		}

		// Token: 0x060010B2 RID: 4274 RVA: 0x000749F8 File Offset: 0x00072BF8
		public void RandomizeCryptoKey()
		{
			decimal value = this.InternalDecrypt();
			do
			{
				this.currentCryptoKey = (long)UnityEngine.Random.Range(int.MinValue, int.MaxValue);
			}
			while (this.currentCryptoKey == 0L);
			this.hiddenValue = ObscuredDecimal.InternalEncrypt(value, this.currentCryptoKey);
		}

		// Token: 0x060010B3 RID: 4275 RVA: 0x00074A40 File Offset: 0x00072C40
		public decimal GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			ObscuredDecimal.DecimalLongBytesUnion decimalLongBytesUnion = default(ObscuredDecimal.DecimalLongBytesUnion);
			decimalLongBytesUnion.b16 = this.hiddenValue;
			return decimalLongBytesUnion.d;
		}

		// Token: 0x060010B4 RID: 4276 RVA: 0x00074A70 File Offset: 0x00072C70
		public void SetEncrypted(decimal encrypted)
		{
			this.inited = true;
			ObscuredDecimal.DecimalLongBytesUnion decimalLongBytesUnion = default(ObscuredDecimal.DecimalLongBytesUnion);
			decimalLongBytesUnion.d = encrypted;
			this.hiddenValue = decimalLongBytesUnion.b16;
			if (ObscuredCheatingDetector.IsRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		// Token: 0x060010B5 RID: 4277 RVA: 0x00074AB8 File Offset: 0x00072CB8
		private decimal InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredDecimal.cryptoKey;
				this.hiddenValue = ObscuredDecimal.InternalEncrypt(0m);
				this.fakeValue = 0m;
				this.inited = true;
			}
			ObscuredDecimal.DecimalLongBytesUnion decimalLongBytesUnion = default(ObscuredDecimal.DecimalLongBytesUnion);
			decimalLongBytesUnion.b16 = this.hiddenValue;
			decimalLongBytesUnion.l1 ^= this.currentCryptoKey;
			decimalLongBytesUnion.l2 ^= this.currentCryptoKey;
			decimal d = decimalLongBytesUnion.d;
			if (ObscuredCheatingDetector.IsRunning && this.fakeValue != 0m && d != this.fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return d;
		}

		// Token: 0x060010B6 RID: 4278 RVA: 0x00074B84 File Offset: 0x00072D84
		public override string ToString()
		{
			return this.InternalDecrypt().ToString();
		}

		// Token: 0x060010B7 RID: 4279 RVA: 0x00074BA0 File Offset: 0x00072DA0
		public string ToString(string format)
		{
			return this.InternalDecrypt().ToString(format);
		}

		// Token: 0x060010B8 RID: 4280 RVA: 0x00074BBC File Offset: 0x00072DBC
		public string ToString(IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(provider);
		}

		// Token: 0x060010B9 RID: 4281 RVA: 0x00074BD8 File Offset: 0x00072DD8
		public string ToString(string format, IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(format, provider);
		}

		// Token: 0x060010BA RID: 4282 RVA: 0x0000FEA3 File Offset: 0x0000E0A3
		public override bool Equals(object obj)
		{
			return obj is ObscuredDecimal && this.Equals((ObscuredDecimal)obj);
		}

		// Token: 0x060010BB RID: 4283 RVA: 0x00074BF8 File Offset: 0x00072DF8
		public bool Equals(ObscuredDecimal obj)
		{
			return obj.InternalDecrypt().Equals(this.InternalDecrypt());
		}

		// Token: 0x060010BC RID: 4284 RVA: 0x00074C1C File Offset: 0x00072E1C
		public override int GetHashCode()
		{
			return this.InternalDecrypt().GetHashCode();
		}

		// Token: 0x060010BD RID: 4285 RVA: 0x00074C38 File Offset: 0x00072E38
		public static implicit operator ObscuredDecimal(decimal value)
		{
			ObscuredDecimal result = new ObscuredDecimal(ObscuredDecimal.InternalEncrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		// Token: 0x060010BE RID: 4286 RVA: 0x0000FEBE File Offset: 0x0000E0BE
		public static implicit operator decimal(ObscuredDecimal value)
		{
			return value.InternalDecrypt();
		}

		// Token: 0x060010BF RID: 4287 RVA: 0x0000FEC7 File Offset: 0x0000E0C7
		public static explicit operator ObscuredDecimal(ObscuredFloat f)
		{
			return (decimal)(float)f;
		}

		// Token: 0x060010C0 RID: 4288 RVA: 0x00074C68 File Offset: 0x00072E68
		public static ObscuredDecimal operator ++(ObscuredDecimal input)
		{
			decimal value = input.InternalDecrypt() + 1m;
			input.hiddenValue = ObscuredDecimal.InternalEncrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		// Token: 0x060010C1 RID: 4289 RVA: 0x00074CB0 File Offset: 0x00072EB0
		public static ObscuredDecimal operator --(ObscuredDecimal input)
		{
			decimal value = input.InternalDecrypt() - 1m;
			input.hiddenValue = ObscuredDecimal.InternalEncrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		// Token: 0x04000B67 RID: 2919
		private static long cryptoKey = 209208L;

		// Token: 0x04000B68 RID: 2920
		private long currentCryptoKey;

		// Token: 0x04000B69 RID: 2921
		[FormerlySerializedAs("hiddenValue")]
		private byte[] hiddenValueOld;

		// Token: 0x04000B6A RID: 2922
		private ACTkByte16 hiddenValue;

		// Token: 0x04000B6B RID: 2923
		private decimal fakeValue;

		// Token: 0x04000B6C RID: 2924
		private bool inited;

		// Token: 0x020001C0 RID: 448
		[StructLayout(LayoutKind.Explicit)]
		private struct DecimalLongBytesUnion
		{
			// Token: 0x04000B6D RID: 2925
			[FieldOffset(0)]
			public decimal d;

			// Token: 0x04000B6E RID: 2926
			[FieldOffset(0)]
			public long l1;

			// Token: 0x04000B6F RID: 2927
			[FieldOffset(8)]
			public long l2;

			// Token: 0x04000B70 RID: 2928
			[FieldOffset(0)]
			public ACTkByte16 b16;
		}
	}
}
