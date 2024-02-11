using System;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	// Token: 0x020001CA RID: 458
	[Serializable]
	public struct ObscuredQuaternion
	{
		// Token: 0x06001183 RID: 4483 RVA: 0x0001056D File Offset: 0x0000E76D
		private ObscuredQuaternion(ObscuredQuaternion.RawEncryptedQuaternion value)
		{
			this.currentCryptoKey = ObscuredQuaternion.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = ObscuredQuaternion.initialFakeValue;
			this.inited = true;
		}

		// Token: 0x06001185 RID: 4485 RVA: 0x000105A9 File Offset: 0x0000E7A9
		public static void SetNewCryptoKey(int newKey)
		{
			ObscuredQuaternion.cryptoKey = newKey;
		}

		// Token: 0x06001186 RID: 4486 RVA: 0x000105B1 File Offset: 0x0000E7B1
		public static ObscuredQuaternion.RawEncryptedQuaternion Encrypt(Quaternion value)
		{
			return ObscuredQuaternion.Encrypt(value, 0);
		}

		// Token: 0x06001187 RID: 4487 RVA: 0x00076C90 File Offset: 0x00074E90
		public static ObscuredQuaternion.RawEncryptedQuaternion Encrypt(Quaternion value, int key)
		{
			if (key == 0)
			{
				key = ObscuredQuaternion.cryptoKey;
			}
			ObscuredQuaternion.RawEncryptedQuaternion result;
			result.x = ObscuredFloat.Encrypt(value.x, key);
			result.y = ObscuredFloat.Encrypt(value.y, key);
			result.z = ObscuredFloat.Encrypt(value.z, key);
			result.w = ObscuredFloat.Encrypt(value.w, key);
			return result;
		}

		// Token: 0x06001188 RID: 4488 RVA: 0x000105BA File Offset: 0x0000E7BA
		public static Quaternion Decrypt(ObscuredQuaternion.RawEncryptedQuaternion value)
		{
			return ObscuredQuaternion.Decrypt(value, 0);
		}

		// Token: 0x06001189 RID: 4489 RVA: 0x00076CFC File Offset: 0x00074EFC
		public static Quaternion Decrypt(ObscuredQuaternion.RawEncryptedQuaternion value, int key)
		{
			if (key == 0)
			{
				key = ObscuredQuaternion.cryptoKey;
			}
			Quaternion result;
			result.x = ObscuredFloat.Decrypt(value.x, key);
			result.y = ObscuredFloat.Decrypt(value.y, key);
			result.z = ObscuredFloat.Decrypt(value.z, key);
			result.w = ObscuredFloat.Decrypt(value.w, key);
			return result;
		}

		// Token: 0x0600118A RID: 4490 RVA: 0x000105C3 File Offset: 0x0000E7C3
		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredQuaternion.cryptoKey)
			{
				this.hiddenValue = ObscuredQuaternion.Encrypt(this.InternalDecrypt(), ObscuredQuaternion.cryptoKey);
				this.currentCryptoKey = ObscuredQuaternion.cryptoKey;
			}
		}

		// Token: 0x0600118B RID: 4491 RVA: 0x00076D68 File Offset: 0x00074F68
		public void RandomizeCryptoKey()
		{
			Quaternion value = this.InternalDecrypt();
			do
			{
				this.currentCryptoKey = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
			}
			while (this.currentCryptoKey == 0);
			this.hiddenValue = ObscuredQuaternion.Encrypt(value, this.currentCryptoKey);
		}

		// Token: 0x0600118C RID: 4492 RVA: 0x000105F6 File Offset: 0x0000E7F6
		public ObscuredQuaternion.RawEncryptedQuaternion GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		// Token: 0x0600118D RID: 4493 RVA: 0x00010604 File Offset: 0x0000E804
		public void SetEncrypted(ObscuredQuaternion.RawEncryptedQuaternion encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.IsRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		// Token: 0x0600118E RID: 4494 RVA: 0x00076DB0 File Offset: 0x00074FB0
		private Quaternion InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredQuaternion.cryptoKey;
				this.hiddenValue = ObscuredQuaternion.Encrypt(ObscuredQuaternion.initialFakeValue);
				this.fakeValue = ObscuredQuaternion.initialFakeValue;
				this.inited = true;
			}
			Quaternion quaternion;
			quaternion.x = ObscuredFloat.Decrypt(this.hiddenValue.x, this.currentCryptoKey);
			quaternion.y = ObscuredFloat.Decrypt(this.hiddenValue.y, this.currentCryptoKey);
			quaternion.z = ObscuredFloat.Decrypt(this.hiddenValue.z, this.currentCryptoKey);
			quaternion.w = ObscuredFloat.Decrypt(this.hiddenValue.w, this.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning && !this.fakeValue.Equals(ObscuredQuaternion.initialFakeValue) && !this.CompareQuaternionsWithTolerance(quaternion, this.fakeValue))
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return quaternion;
		}

		// Token: 0x0600118F RID: 4495 RVA: 0x00076EAC File Offset: 0x000750AC
		private bool CompareQuaternionsWithTolerance(Quaternion q1, Quaternion q2)
		{
			float quaternionEpsilon = ObscuredCheatingDetector.Instance.quaternionEpsilon;
			return Math.Abs(q1.x - q2.x) < quaternionEpsilon && Math.Abs(q1.y - q2.y) < quaternionEpsilon && Math.Abs(q1.z - q2.z) < quaternionEpsilon && Math.Abs(q1.w - q2.w) < quaternionEpsilon;
		}

		// Token: 0x06001190 RID: 4496 RVA: 0x00076F2C File Offset: 0x0007512C
		public override int GetHashCode()
		{
			return this.InternalDecrypt().GetHashCode();
		}

		// Token: 0x06001191 RID: 4497 RVA: 0x00076F48 File Offset: 0x00075148
		public override string ToString()
		{
			return this.InternalDecrypt().ToString();
		}

		// Token: 0x06001192 RID: 4498 RVA: 0x00076F64 File Offset: 0x00075164
		public string ToString(string format)
		{
			return this.InternalDecrypt().ToString(format);
		}

		// Token: 0x06001193 RID: 4499 RVA: 0x00076F80 File Offset: 0x00075180
		public static implicit operator ObscuredQuaternion(Quaternion value)
		{
			ObscuredQuaternion result = new ObscuredQuaternion(ObscuredQuaternion.Encrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		// Token: 0x06001194 RID: 4500 RVA: 0x0001062A File Offset: 0x0000E82A
		public static implicit operator Quaternion(ObscuredQuaternion value)
		{
			return value.InternalDecrypt();
		}

		// Token: 0x04000BB0 RID: 2992
		private static int cryptoKey = 120205;

		// Token: 0x04000BB1 RID: 2993
		private static readonly Quaternion initialFakeValue = Quaternion.identity;

		// Token: 0x04000BB2 RID: 2994
		[SerializeField]
		private int currentCryptoKey;

		// Token: 0x04000BB3 RID: 2995
		[SerializeField]
		private ObscuredQuaternion.RawEncryptedQuaternion hiddenValue;

		// Token: 0x04000BB4 RID: 2996
		[SerializeField]
		private Quaternion fakeValue;

		// Token: 0x04000BB5 RID: 2997
		[SerializeField]
		private bool inited;

		// Token: 0x020001CB RID: 459
		[Serializable]
		public struct RawEncryptedQuaternion
		{
			// Token: 0x04000BB6 RID: 2998
			public int x;

			// Token: 0x04000BB7 RID: 2999
			public int y;

			// Token: 0x04000BB8 RID: 3000
			public int z;

			// Token: 0x04000BB9 RID: 3001
			public int w;
		}
	}
}
