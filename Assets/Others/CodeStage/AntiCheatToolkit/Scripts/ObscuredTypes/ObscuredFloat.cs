using System;
using System.Runtime.InteropServices;
using CodeStage.AntiCheat.Common;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	// Token: 0x020001C3 RID: 451
	[Serializable]
	public struct ObscuredFloat : IFormattable, IEquatable<ObscuredFloat>
	{
		// Token: 0x060010DB RID: 4315 RVA: 0x0000FF9A File Offset: 0x0000E19A
		private ObscuredFloat(ACTkByte4 value)
		{
			this.currentCryptoKey = ObscuredFloat.cryptoKey;
			this.hiddenValue = value;
			this.hiddenValueOld = null;
			this.fakeValue = 0f;
			this.inited = true;
		}

#if UNITY_EDITOR
        // For internal Editor usage only (may be useful for drawers).
        public static int cryptoKeyEditor = cryptoKey;
#endif

        // Token: 0x060010DD RID: 4317 RVA: 0x0000FFD3 File Offset: 0x0000E1D3
        public static void SetNewCryptoKey(int newKey)
		{
			ObscuredFloat.cryptoKey = newKey;
		}

		// Token: 0x060010DE RID: 4318 RVA: 0x0000FFDB File Offset: 0x0000E1DB
		public static int Encrypt(float value)
		{
			return ObscuredFloat.Encrypt(value, ObscuredFloat.cryptoKey);
		}

		// Token: 0x060010DF RID: 4319 RVA: 0x00075090 File Offset: 0x00073290
		public static int Encrypt(float value, int key)
		{
			ObscuredFloat.FloatIntBytesUnion floatIntBytesUnion = default(ObscuredFloat.FloatIntBytesUnion);
			floatIntBytesUnion.f = value;
			floatIntBytesUnion.i ^= key;
			return floatIntBytesUnion.i;
		}

		// Token: 0x060010E0 RID: 4320 RVA: 0x0000FFE8 File Offset: 0x0000E1E8
		private static ACTkByte4 InternalEncrypt(float value)
		{
			return ObscuredFloat.InternalEncrypt(value, 0);
		}

		// Token: 0x060010E1 RID: 4321 RVA: 0x000750C4 File Offset: 0x000732C4
		private static ACTkByte4 InternalEncrypt(float value, int key)
		{
			int num = key;
			if (num == 0)
			{
				num = ObscuredFloat.cryptoKey;
			}
			ObscuredFloat.FloatIntBytesUnion floatIntBytesUnion = default(ObscuredFloat.FloatIntBytesUnion);
			floatIntBytesUnion.f = value;
			floatIntBytesUnion.i ^= num;
			return floatIntBytesUnion.b4;
		}

		// Token: 0x060010E2 RID: 4322 RVA: 0x0000FFF1 File Offset: 0x0000E1F1
		public static float Decrypt(int value)
		{
			return ObscuredFloat.Decrypt(value, ObscuredFloat.cryptoKey);
		}

		// Token: 0x060010E3 RID: 4323 RVA: 0x00075108 File Offset: 0x00073308
		public static float Decrypt(int value, int key)
		{
			ObscuredFloat.FloatIntBytesUnion floatIntBytesUnion = default(ObscuredFloat.FloatIntBytesUnion);
			floatIntBytesUnion.i = (value ^ key);
			return floatIntBytesUnion.f;
		}

		// Token: 0x060010E4 RID: 4324 RVA: 0x0000FFFE File Offset: 0x0000E1FE
		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredFloat.cryptoKey)
			{
				this.hiddenValue = ObscuredFloat.InternalEncrypt(this.InternalDecrypt(), ObscuredFloat.cryptoKey);
				this.currentCryptoKey = ObscuredFloat.cryptoKey;
			}
		}

		// Token: 0x060010E5 RID: 4325 RVA: 0x00075130 File Offset: 0x00073330
		public void RandomizeCryptoKey()
		{
			float value = this.InternalDecrypt();
			do
			{
				this.currentCryptoKey = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
			}
			while (this.currentCryptoKey == 0);
			this.hiddenValue = ObscuredFloat.InternalEncrypt(value, this.currentCryptoKey);
		}

		// Token: 0x060010E6 RID: 4326 RVA: 0x00075178 File Offset: 0x00073378
		public int GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			ObscuredFloat.FloatIntBytesUnion floatIntBytesUnion = default(ObscuredFloat.FloatIntBytesUnion);
			floatIntBytesUnion.b4 = this.hiddenValue;
			return floatIntBytesUnion.i;
		}

		// Token: 0x060010E7 RID: 4327 RVA: 0x000751A8 File Offset: 0x000733A8
		public void SetEncrypted(int encrypted)
		{
			this.inited = true;
			ObscuredFloat.FloatIntBytesUnion floatIntBytesUnion = default(ObscuredFloat.FloatIntBytesUnion);
			floatIntBytesUnion.i = encrypted;
			this.hiddenValue = floatIntBytesUnion.b4;
			if (ObscuredCheatingDetector.IsRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		// Token: 0x060010E8 RID: 4328 RVA: 0x000751F0 File Offset: 0x000733F0
		private float InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredFloat.cryptoKey;
				this.hiddenValue = ObscuredFloat.InternalEncrypt(0f);
				this.fakeValue = 0f;
				this.inited = true;
			}
			ObscuredFloat.FloatIntBytesUnion floatIntBytesUnion = default(ObscuredFloat.FloatIntBytesUnion);
			floatIntBytesUnion.b4 = this.hiddenValue;
			floatIntBytesUnion.i ^= this.currentCryptoKey;
			float f = floatIntBytesUnion.f;
			if (ObscuredCheatingDetector.IsRunning && this.fakeValue != 0f && Math.Abs(f - this.fakeValue) > ObscuredCheatingDetector.Instance.floatEpsilon)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return f;
		}

		// Token: 0x060010E9 RID: 4329 RVA: 0x00010031 File Offset: 0x0000E231
		public override bool Equals(object obj)
		{
			return obj is ObscuredFloat && this.Equals((ObscuredFloat)obj);
		}

		// Token: 0x060010EA RID: 4330 RVA: 0x000752A8 File Offset: 0x000734A8
		public bool Equals(ObscuredFloat obj)
		{
			double num = (double)obj.InternalDecrypt();
			double obj2 = (double)this.InternalDecrypt();
			return num.Equals(obj2);
		}

		// Token: 0x060010EB RID: 4331 RVA: 0x000752D0 File Offset: 0x000734D0
		public override int GetHashCode()
		{
			return this.InternalDecrypt().GetHashCode();
		}

		// Token: 0x060010EC RID: 4332 RVA: 0x000752EC File Offset: 0x000734EC
		public override string ToString()
		{
			return this.InternalDecrypt().ToString();
		}

		// Token: 0x060010ED RID: 4333 RVA: 0x00075308 File Offset: 0x00073508
		public string ToString(string format)
		{
			return this.InternalDecrypt().ToString(format);
		}

		// Token: 0x060010EE RID: 4334 RVA: 0x00075324 File Offset: 0x00073524
		public string ToString(IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(provider);
		}

		// Token: 0x060010EF RID: 4335 RVA: 0x00075340 File Offset: 0x00073540
		public string ToString(string format, IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(format, provider);
		}

		// Token: 0x060010F0 RID: 4336 RVA: 0x00075360 File Offset: 0x00073560
		public static implicit operator ObscuredFloat(float value)
		{
			ObscuredFloat result = new ObscuredFloat(ObscuredFloat.InternalEncrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		// Token: 0x060010F1 RID: 4337 RVA: 0x0001004C File Offset: 0x0000E24C
		public static implicit operator float(ObscuredFloat value)
		{
			return value.InternalDecrypt();
		}

		// Token: 0x060010F2 RID: 4338 RVA: 0x00075390 File Offset: 0x00073590
		public static ObscuredFloat operator ++(ObscuredFloat input)
		{
			float value = input.InternalDecrypt() + 1f;
			input.hiddenValue = ObscuredFloat.InternalEncrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		// Token: 0x060010F3 RID: 4339 RVA: 0x000753D4 File Offset: 0x000735D4
		public static ObscuredFloat operator --(ObscuredFloat input)
		{
			float value = input.InternalDecrypt() - 1f;
			input.hiddenValue = ObscuredFloat.InternalEncrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		// Token: 0x04000B7A RID: 2938
		private static int cryptoKey = 230887;

		// Token: 0x04000B7B RID: 2939
		[SerializeField]
		private int currentCryptoKey;

		// Token: 0x04000B7C RID: 2940
		[SerializeField]
		private ACTkByte4 hiddenValue;

		// Token: 0x04000B7D RID: 2941
		[FormerlySerializedAs("hiddenValue")]
		[SerializeField]
		private byte[] hiddenValueOld;

		// Token: 0x04000B7E RID: 2942
		[SerializeField]
		private float fakeValue;

		// Token: 0x04000B7F RID: 2943
		[SerializeField]
		private bool inited;

		// Token: 0x020001C4 RID: 452
		[StructLayout(LayoutKind.Explicit)]
		private struct FloatIntBytesUnion
		{
			// Token: 0x04000B80 RID: 2944
			[FieldOffset(0)]
			public float f;

			// Token: 0x04000B81 RID: 2945
			[FieldOffset(0)]
			public int i;

			// Token: 0x04000B82 RID: 2946
			[FieldOffset(0)]
			public ACTkByte4 b4;
		}
	}
}
