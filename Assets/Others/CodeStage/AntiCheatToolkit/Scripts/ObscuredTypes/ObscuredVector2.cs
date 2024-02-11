using System;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	// Token: 0x020001D2 RID: 466
	[Serializable]
	public struct ObscuredVector2
	{
		// Token: 0x0600121C RID: 4636 RVA: 0x00010BFB File Offset: 0x0000EDFB
		private ObscuredVector2(ObscuredVector2.RawEncryptedVector2 value)
		{
			this.currentCryptoKey = ObscuredVector2.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = ObscuredVector2.initialFakeValue;
			this.inited = true;
		}

		// Token: 0x17000313 RID: 787
		// (get) Token: 0x0600121E RID: 4638 RVA: 0x00077E40 File Offset: 0x00076040
		// (set) Token: 0x0600121F RID: 4639 RVA: 0x00010C37 File Offset: 0x0000EE37
		public float x
		{
			get
			{
				float num = this.InternalDecryptField(this.hiddenValue.x);
				if (ObscuredCheatingDetector.IsRunning && !this.fakeValue.Equals(ObscuredVector2.initialFakeValue) && Math.Abs(num - this.fakeValue.x) > ObscuredCheatingDetector.Instance.vector2Epsilon)
				{
					ObscuredCheatingDetector.Instance.OnCheatingDetected();
				}
				return num;
			}
			set
			{
				this.hiddenValue.x = this.InternalEncryptField(value);
				if (ObscuredCheatingDetector.IsRunning)
				{
					this.fakeValue.x = value;
				}
			}
		}

		// Token: 0x17000314 RID: 788
		// (get) Token: 0x06001220 RID: 4640 RVA: 0x00077EB0 File Offset: 0x000760B0
		// (set) Token: 0x06001221 RID: 4641 RVA: 0x00010C61 File Offset: 0x0000EE61
		public float y
		{
			get
			{
				float num = this.InternalDecryptField(this.hiddenValue.y);
				if (ObscuredCheatingDetector.IsRunning && !this.fakeValue.Equals(ObscuredVector2.initialFakeValue) && Math.Abs(num - this.fakeValue.y) > ObscuredCheatingDetector.Instance.vector2Epsilon)
				{
					ObscuredCheatingDetector.Instance.OnCheatingDetected();
				}
				return num;
			}
			set
			{
				this.hiddenValue.y = this.InternalEncryptField(value);
				if (ObscuredCheatingDetector.IsRunning)
				{
					this.fakeValue.y = value;
				}
			}
		}

		// Token: 0x17000315 RID: 789
		public float this[int index]
		{
			get
			{
				if (index == 0)
				{
					return this.x;
				}
				if (index != 1)
				{
					throw new IndexOutOfRangeException("Invalid ObscuredVector2 index!");
				}
				return this.y;
			}
			set
			{
				if (index != 0)
				{
					if (index != 1)
					{
						throw new IndexOutOfRangeException("Invalid ObscuredVector2 index!");
					}
					this.y = value;
				}
				else
				{
					this.x = value;
				}
			}
		}

		// Token: 0x06001224 RID: 4644 RVA: 0x00010C8B File Offset: 0x0000EE8B
		public static void SetNewCryptoKey(int newKey)
		{
			ObscuredVector2.cryptoKey = newKey;
		}

		// Token: 0x06001225 RID: 4645 RVA: 0x00010C93 File Offset: 0x0000EE93
		public static ObscuredVector2.RawEncryptedVector2 Encrypt(Vector2 value)
		{
			return ObscuredVector2.Encrypt(value, 0);
		}

		// Token: 0x06001226 RID: 4646 RVA: 0x00077FA0 File Offset: 0x000761A0
		public static ObscuredVector2.RawEncryptedVector2 Encrypt(Vector2 value, int key)
		{
			if (key == 0)
			{
				key = ObscuredVector2.cryptoKey;
			}
			ObscuredVector2.RawEncryptedVector2 result;
			result.x = ObscuredFloat.Encrypt(value.x, key);
			result.y = ObscuredFloat.Encrypt(value.y, key);
			return result;
		}

		// Token: 0x06001227 RID: 4647 RVA: 0x00010C9C File Offset: 0x0000EE9C
		public static Vector2 Decrypt(ObscuredVector2.RawEncryptedVector2 value)
		{
			return ObscuredVector2.Decrypt(value, 0);
		}

		// Token: 0x06001228 RID: 4648 RVA: 0x00077FE4 File Offset: 0x000761E4
		public static Vector2 Decrypt(ObscuredVector2.RawEncryptedVector2 value, int key)
		{
			if (key == 0)
			{
				key = ObscuredVector2.cryptoKey;
			}
			Vector2 result;
			result.x = ObscuredFloat.Decrypt(value.x, key);
			result.y = ObscuredFloat.Decrypt(value.y, key);
			return result;
		}

		// Token: 0x06001229 RID: 4649 RVA: 0x00010CA5 File Offset: 0x0000EEA5
		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredVector2.cryptoKey)
			{
				this.hiddenValue = ObscuredVector2.Encrypt(this.InternalDecrypt(), ObscuredVector2.cryptoKey);
				this.currentCryptoKey = ObscuredVector2.cryptoKey;
			}
		}

		// Token: 0x0600122A RID: 4650 RVA: 0x00078028 File Offset: 0x00076228
		public void RandomizeCryptoKey()
		{
			Vector2 value = this.InternalDecrypt();
			do
			{
				this.currentCryptoKey = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
			}
			while (this.currentCryptoKey == 0);
			this.hiddenValue = ObscuredVector2.Encrypt(value, this.currentCryptoKey);
		}

		// Token: 0x0600122B RID: 4651 RVA: 0x00010CD8 File Offset: 0x0000EED8
		public ObscuredVector2.RawEncryptedVector2 GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		// Token: 0x0600122C RID: 4652 RVA: 0x00010CE6 File Offset: 0x0000EEE6
		public void SetEncrypted(ObscuredVector2.RawEncryptedVector2 encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.IsRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		// Token: 0x0600122D RID: 4653 RVA: 0x00078070 File Offset: 0x00076270
		private Vector2 InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredVector2.cryptoKey;
				this.hiddenValue = ObscuredVector2.Encrypt(ObscuredVector2.initialFakeValue);
				this.fakeValue = ObscuredVector2.initialFakeValue;
				this.inited = true;
			}
			Vector2 vector;
			vector.x = ObscuredFloat.Decrypt(this.hiddenValue.x, this.currentCryptoKey);
			vector.y = ObscuredFloat.Decrypt(this.hiddenValue.y, this.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning && !this.fakeValue.Equals(ObscuredVector2.initialFakeValue) && !this.CompareVectorsWithTolerance(vector, this.fakeValue))
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return vector;
		}

		// Token: 0x0600122E RID: 4654 RVA: 0x00078130 File Offset: 0x00076330
		private bool CompareVectorsWithTolerance(Vector2 vector1, Vector2 vector2)
		{
			float vector2Epsilon = ObscuredCheatingDetector.Instance.vector2Epsilon;
			return Math.Abs(vector1.x - vector2.x) < vector2Epsilon && Math.Abs(vector1.y - vector2.y) < vector2Epsilon;
		}

		// Token: 0x0600122F RID: 4655 RVA: 0x0007817C File Offset: 0x0007637C
		private float InternalDecryptField(int encrypted)
		{
			int key = ObscuredVector2.cryptoKey;
			if (this.currentCryptoKey != ObscuredVector2.cryptoKey)
			{
				key = this.currentCryptoKey;
			}
			return ObscuredFloat.Decrypt(encrypted, key);
		}

		// Token: 0x06001230 RID: 4656 RVA: 0x000781B0 File Offset: 0x000763B0
		private int InternalEncryptField(float encrypted)
		{
			return ObscuredFloat.Encrypt(encrypted, ObscuredVector2.cryptoKey);
		}

		// Token: 0x06001231 RID: 4657 RVA: 0x000781CC File Offset: 0x000763CC
		public override int GetHashCode()
		{
			return this.InternalDecrypt().GetHashCode();
		}

		// Token: 0x06001232 RID: 4658 RVA: 0x000781E8 File Offset: 0x000763E8
		public override string ToString()
		{
			return this.InternalDecrypt().ToString();
		}

		// Token: 0x06001233 RID: 4659 RVA: 0x00078204 File Offset: 0x00076404
		public string ToString(string format)
		{
			return this.InternalDecrypt().ToString(format);
		}

		// Token: 0x06001234 RID: 4660 RVA: 0x00078220 File Offset: 0x00076420
		public static implicit operator ObscuredVector2(Vector2 value)
		{
			ObscuredVector2 result = new ObscuredVector2(ObscuredVector2.Encrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		// Token: 0x06001235 RID: 4661 RVA: 0x00010D0C File Offset: 0x0000EF0C
		public static implicit operator Vector2(ObscuredVector2 value)
		{
			return value.InternalDecrypt();
		}

		// Token: 0x06001236 RID: 4662 RVA: 0x00078250 File Offset: 0x00076450
		public static implicit operator Vector3(ObscuredVector2 value)
		{
			Vector2 vector = value.InternalDecrypt();
			return new Vector3(vector.x, vector.y, 0f);
		}

		// Token: 0x04000BD8 RID: 3032
		private static int cryptoKey = 120206;

		// Token: 0x04000BD9 RID: 3033
		private static readonly Vector2 initialFakeValue = Vector2.zero;

		// Token: 0x04000BDA RID: 3034
		[SerializeField]
		private int currentCryptoKey;

		// Token: 0x04000BDB RID: 3035
		[SerializeField]
		private ObscuredVector2.RawEncryptedVector2 hiddenValue;

		// Token: 0x04000BDC RID: 3036
		[SerializeField]
		private Vector2 fakeValue;

		// Token: 0x04000BDD RID: 3037
		[SerializeField]
		private bool inited;

		// Token: 0x020001D3 RID: 467
		[Serializable]
		public struct RawEncryptedVector2
		{
			// Token: 0x04000BDE RID: 3038
			public int x;

			// Token: 0x04000BDF RID: 3039
			public int y;
		}
	}
}
