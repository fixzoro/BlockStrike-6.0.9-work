using System;
using System.Runtime.InteropServices;
using CodeStage.AntiCheat.Common;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	// Token: 0x020001C1 RID: 449
	[Serializable]
	public struct ObscuredDouble : IFormattable, IEquatable<ObscuredDouble>
	{
		// Token: 0x060010C2 RID: 4290 RVA: 0x0000FED9 File Offset: 0x0000E0D9
		private ObscuredDouble(ACTkByte8 value)
		{
			this.currentCryptoKey = ObscuredDouble.cryptoKey;
			this.hiddenValue = value;
			this.hiddenValueOld = null;
			this.fakeValue = 0.0;
			this.inited = true;
		}

		// Token: 0x060010C4 RID: 4292 RVA: 0x0000FF17 File Offset: 0x0000E117
		public static void SetNewCryptoKey(long newKey)
		{
			ObscuredDouble.cryptoKey = newKey;
		}

		// Token: 0x060010C5 RID: 4293 RVA: 0x0000FF1F File Offset: 0x0000E11F
		public static long Encrypt(double value)
		{
			return ObscuredDouble.Encrypt(value, ObscuredDouble.cryptoKey);
		}

		// Token: 0x060010C6 RID: 4294 RVA: 0x00074CF8 File Offset: 0x00072EF8
		public static long Encrypt(double value, long key)
		{
			ObscuredDouble.DoubleLongBytesUnion doubleLongBytesUnion = default(ObscuredDouble.DoubleLongBytesUnion);
			doubleLongBytesUnion.d = value;
			doubleLongBytesUnion.l ^= key;
			return doubleLongBytesUnion.l;
		}

		// Token: 0x060010C7 RID: 4295 RVA: 0x0000FF2C File Offset: 0x0000E12C
		private static ACTkByte8 InternalEncrypt(double value)
		{
			return ObscuredDouble.InternalEncrypt(value, 0L);
		}

		// Token: 0x060010C8 RID: 4296 RVA: 0x00074D2C File Offset: 0x00072F2C
		private static ACTkByte8 InternalEncrypt(double value, long key)
		{
			long num = key;
			if (num == 0L)
			{
				num = ObscuredDouble.cryptoKey;
			}
			ObscuredDouble.DoubleLongBytesUnion doubleLongBytesUnion = default(ObscuredDouble.DoubleLongBytesUnion);
			doubleLongBytesUnion.d = value;
			doubleLongBytesUnion.l ^= num;
			return doubleLongBytesUnion.b8;
		}

		// Token: 0x060010C9 RID: 4297 RVA: 0x0000FF36 File Offset: 0x0000E136
		public static double Decrypt(long value)
		{
			return ObscuredDouble.Decrypt(value, ObscuredDouble.cryptoKey);
		}

		// Token: 0x060010CA RID: 4298 RVA: 0x00074D70 File Offset: 0x00072F70
		public static double Decrypt(long value, long key)
		{
			ObscuredDouble.DoubleLongBytesUnion doubleLongBytesUnion = default(ObscuredDouble.DoubleLongBytesUnion);
			doubleLongBytesUnion.l = (value ^ key);
			return doubleLongBytesUnion.d;
		}

		// Token: 0x060010CB RID: 4299 RVA: 0x0000FF43 File Offset: 0x0000E143
		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredDouble.cryptoKey)
			{
				this.hiddenValue = ObscuredDouble.InternalEncrypt(this.InternalDecrypt(), ObscuredDouble.cryptoKey);
				this.currentCryptoKey = ObscuredDouble.cryptoKey;
			}
		}

		// Token: 0x060010CC RID: 4300 RVA: 0x00074D98 File Offset: 0x00072F98
		public void RandomizeCryptoKey()
		{
			double value = this.InternalDecrypt();
			do
			{
				this.currentCryptoKey = (long)UnityEngine.Random.Range(int.MinValue, int.MaxValue);
			}
			while (this.currentCryptoKey == 0L);
			this.hiddenValue = ObscuredDouble.InternalEncrypt(value, this.currentCryptoKey);
		}

		// Token: 0x060010CD RID: 4301 RVA: 0x00074DE0 File Offset: 0x00072FE0
		public long GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			ObscuredDouble.DoubleLongBytesUnion doubleLongBytesUnion = default(ObscuredDouble.DoubleLongBytesUnion);
			doubleLongBytesUnion.b8 = this.hiddenValue;
			return doubleLongBytesUnion.l;
		}

		// Token: 0x060010CE RID: 4302 RVA: 0x00074E10 File Offset: 0x00073010
		public void SetEncrypted(long encrypted)
		{
			this.inited = true;
			ObscuredDouble.DoubleLongBytesUnion doubleLongBytesUnion = default(ObscuredDouble.DoubleLongBytesUnion);
			doubleLongBytesUnion.l = encrypted;
			this.hiddenValue = doubleLongBytesUnion.b8;
			if (ObscuredCheatingDetector.IsRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		// Token: 0x060010CF RID: 4303 RVA: 0x00074E58 File Offset: 0x00073058
		private double InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredDouble.cryptoKey;
				this.hiddenValue = ObscuredDouble.InternalEncrypt(0.0);
				this.fakeValue = 0.0;
				this.inited = true;
			}
			ObscuredDouble.DoubleLongBytesUnion doubleLongBytesUnion = default(ObscuredDouble.DoubleLongBytesUnion);
			doubleLongBytesUnion.b8 = this.hiddenValue;
			doubleLongBytesUnion.l ^= this.currentCryptoKey;
			double d = doubleLongBytesUnion.d;
			if (ObscuredCheatingDetector.IsRunning && this.fakeValue != 0.0 && Math.Abs(d - this.fakeValue) > 1E-06)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return d;
		}

		// Token: 0x060010D0 RID: 4304 RVA: 0x00074F1C File Offset: 0x0007311C
		public override string ToString()
		{
			return this.InternalDecrypt().ToString();
		}

		// Token: 0x060010D1 RID: 4305 RVA: 0x00074F38 File Offset: 0x00073138
		public string ToString(string format)
		{
			return this.InternalDecrypt().ToString(format);
		}

		// Token: 0x060010D2 RID: 4306 RVA: 0x00074F54 File Offset: 0x00073154
		public string ToString(IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(provider);
		}

		// Token: 0x060010D3 RID: 4307 RVA: 0x00074F70 File Offset: 0x00073170
		public string ToString(string format, IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(format, provider);
		}

		// Token: 0x060010D4 RID: 4308 RVA: 0x0000FF76 File Offset: 0x0000E176
		public override bool Equals(object obj)
		{
			return obj is ObscuredDouble && this.Equals((ObscuredDouble)obj);
		}

		// Token: 0x060010D5 RID: 4309 RVA: 0x00074F90 File Offset: 0x00073190
		public bool Equals(ObscuredDouble obj)
		{
			return obj.InternalDecrypt().Equals(this.InternalDecrypt());
		}

		// Token: 0x060010D6 RID: 4310 RVA: 0x00074FB4 File Offset: 0x000731B4
		public override int GetHashCode()
		{
			return this.InternalDecrypt().GetHashCode();
		}

		// Token: 0x060010D7 RID: 4311 RVA: 0x00074FD0 File Offset: 0x000731D0
		public static implicit operator ObscuredDouble(double value)
		{
			ObscuredDouble result = new ObscuredDouble(ObscuredDouble.InternalEncrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		// Token: 0x060010D8 RID: 4312 RVA: 0x0000FF91 File Offset: 0x0000E191
		public static implicit operator double(ObscuredDouble value)
		{
			return value.InternalDecrypt();
		}

		// Token: 0x060010D9 RID: 4313 RVA: 0x00075000 File Offset: 0x00073200
		public static ObscuredDouble operator ++(ObscuredDouble input)
		{
			double value = input.InternalDecrypt() + 1.0;
			input.hiddenValue = ObscuredDouble.InternalEncrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		// Token: 0x060010DA RID: 4314 RVA: 0x00075048 File Offset: 0x00073248
		public static ObscuredDouble operator --(ObscuredDouble input)
		{
			double value = input.InternalDecrypt() - 1.0;
			input.hiddenValue = ObscuredDouble.InternalEncrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		// Token: 0x04000B71 RID: 2929
		private static long cryptoKey = 210987L;

		// Token: 0x04000B72 RID: 2930
		[SerializeField]
		private long currentCryptoKey;

		// Token: 0x04000B73 RID: 2931
		[FormerlySerializedAs("hiddenValue")]
		[SerializeField]
		private byte[] hiddenValueOld;

		// Token: 0x04000B74 RID: 2932
		[SerializeField]
		private ACTkByte8 hiddenValue;

		// Token: 0x04000B75 RID: 2933
		[SerializeField]
		private double fakeValue;

		// Token: 0x04000B76 RID: 2934
		[SerializeField]
		private bool inited;

		// Token: 0x020001C2 RID: 450
		[StructLayout(LayoutKind.Explicit)]
		private struct DoubleLongBytesUnion
		{
			// Token: 0x04000B77 RID: 2935
			[FieldOffset(0)]
			public double d;

			// Token: 0x04000B78 RID: 2936
			[FieldOffset(0)]
			public long l;

			// Token: 0x04000B79 RID: 2937
			[FieldOffset(0)]
			public ACTkByte8 b8;
		}
	}
}
