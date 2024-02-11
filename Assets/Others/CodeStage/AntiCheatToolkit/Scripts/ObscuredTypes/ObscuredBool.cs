using System;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	// Token: 0x020001BC RID: 444
	[Serializable]
	public struct ObscuredBool : IEquatable<ObscuredBool>
	{
		// Token: 0x0600106E RID: 4206 RVA: 0x0000FB4B File Offset: 0x0000DD4B
		private ObscuredBool(int value)
		{
			this.currentCryptoKey = ObscuredBool.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = false;
			this.fakeValueChanged = false;
			this.inited = true;
		}

#if UNITY_EDITOR
        // For internal Editor usage only (may be useful for drawers).
        public static byte cryptoKeyEditor = cryptoKey;
#endif

        // Token: 0x06001070 RID: 4208 RVA: 0x0000FB80 File Offset: 0x0000DD80
        public static void SetNewCryptoKey(byte newKey)
		{
			ObscuredBool.cryptoKey = newKey;
		}

		// Token: 0x06001071 RID: 4209 RVA: 0x0000FB88 File Offset: 0x0000DD88
		public static int Encrypt(bool value)
		{
			return ObscuredBool.Encrypt(value, 0);
		}

		// Token: 0x06001072 RID: 4210 RVA: 0x00074340 File Offset: 0x00072540
		public static int Encrypt(bool value, byte key)
		{
			if (key == 0)
			{
				key = ObscuredBool.cryptoKey;
			}
			int num = (!value) ? 181 : 213;
			return num ^ (int)key;
		}

		// Token: 0x06001073 RID: 4211 RVA: 0x0000FB91 File Offset: 0x0000DD91
		public static bool Decrypt(int value)
		{
			return ObscuredBool.Decrypt(value, 0);
		}

		// Token: 0x06001074 RID: 4212 RVA: 0x0000FB9A File Offset: 0x0000DD9A
		public static bool Decrypt(int value, byte key)
		{
			if (key == 0)
			{
				key = ObscuredBool.cryptoKey;
			}
			value ^= (int)key;
			return value != 181;
		}

		// Token: 0x06001075 RID: 4213 RVA: 0x0000FBB9 File Offset: 0x0000DDB9
		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredBool.cryptoKey)
			{
				this.hiddenValue = ObscuredBool.Encrypt(this.InternalDecrypt(), ObscuredBool.cryptoKey);
				this.currentCryptoKey = ObscuredBool.cryptoKey;
			}
		}

		// Token: 0x06001076 RID: 4214 RVA: 0x00074378 File Offset: 0x00072578
		public void RandomizeCryptoKey()
		{
			bool value = this.InternalDecrypt();
			this.currentCryptoKey = (byte)UnityEngine.Random.Range(1, 150);
			this.hiddenValue = ObscuredBool.Encrypt(value, this.currentCryptoKey);
		}

		// Token: 0x06001077 RID: 4215 RVA: 0x0000FBEC File Offset: 0x0000DDEC
		public int GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		// Token: 0x06001078 RID: 4216 RVA: 0x0000FBFA File Offset: 0x0000DDFA
		public void SetEncrypted(int encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.IsRunning)
			{
				this.fakeValue = this.InternalDecrypt();
				this.fakeValueChanged = true;
			}
		}

		// Token: 0x06001079 RID: 4217 RVA: 0x000743B0 File Offset: 0x000725B0
		private bool InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredBool.cryptoKey;
				this.hiddenValue = ObscuredBool.Encrypt(false);
				this.fakeValue = false;
				this.fakeValueChanged = true;
				this.inited = true;
			}
			int num = this.hiddenValue;
			num ^= (int)this.currentCryptoKey;
			bool flag = num != 181;
			if (ObscuredCheatingDetector.IsRunning && this.fakeValueChanged && flag != this.fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return flag;
		}

		// Token: 0x0600107A RID: 4218 RVA: 0x0000FC27 File Offset: 0x0000DE27
		public override bool Equals(object obj)
		{
			return obj is ObscuredBool && this.Equals((ObscuredBool)obj);
		}

		// Token: 0x0600107B RID: 4219 RVA: 0x0007443C File Offset: 0x0007263C
		public bool Equals(ObscuredBool obj)
		{
			if (this.currentCryptoKey == obj.currentCryptoKey)
			{
				return this.hiddenValue == obj.hiddenValue;
			}
			return ObscuredBool.Decrypt(this.hiddenValue, this.currentCryptoKey) == ObscuredBool.Decrypt(obj.hiddenValue, obj.currentCryptoKey);
		}

		// Token: 0x0600107C RID: 4220 RVA: 0x00074494 File Offset: 0x00072694
		public override int GetHashCode()
		{
			return this.InternalDecrypt().GetHashCode();
		}

		// Token: 0x0600107D RID: 4221 RVA: 0x000744B0 File Offset: 0x000726B0
		public override string ToString()
		{
			return this.InternalDecrypt().ToString();
		}

		// Token: 0x0600107E RID: 4222 RVA: 0x000744CC File Offset: 0x000726CC
		public static implicit operator ObscuredBool(bool value)
		{
			ObscuredBool result = new ObscuredBool(ObscuredBool.Encrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
				result.fakeValueChanged = true;
			}
			return result;
		}

		// Token: 0x0600107F RID: 4223 RVA: 0x0000FC42 File Offset: 0x0000DE42
		public static implicit operator bool(ObscuredBool value)
		{
			return value.InternalDecrypt();
		}

		// Token: 0x04000B57 RID: 2903
		private static byte cryptoKey = 215;

		// Token: 0x04000B58 RID: 2904
		[SerializeField]
		private byte currentCryptoKey;

		// Token: 0x04000B59 RID: 2905
		[SerializeField]
		private int hiddenValue;

		// Token: 0x04000B5A RID: 2906
		[SerializeField]
		private bool fakeValue;

		// Token: 0x04000B5B RID: 2907
		[SerializeField]
		private bool fakeValueChanged;

		// Token: 0x04000B5C RID: 2908
		[SerializeField]
		private bool inited;
	}
}
