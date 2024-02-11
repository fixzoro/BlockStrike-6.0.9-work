using System;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	// Token: 0x020001D4 RID: 468
	[Serializable]
	public struct ObscuredVector3
	{
		// Token: 0x06001237 RID: 4663 RVA: 0x00010D15 File Offset: 0x0000EF15
		private ObscuredVector3(ObscuredVector3.RawEncryptedVector3 encrypted)
		{
			this.currentCryptoKey = ObscuredVector3.cryptoKey;
			this.hiddenValue = encrypted;
			this.fakeValue = ObscuredVector3.initialFakeValue;
			this.inited = true;
		}

		// Token: 0x17000316 RID: 790
		// (get) Token: 0x06001239 RID: 4665 RVA: 0x00078280 File Offset: 0x00076480
		// (set) Token: 0x0600123A RID: 4666 RVA: 0x00010D51 File Offset: 0x0000EF51
		public float x
		{
			get
			{
				float num = this.InternalDecryptField(this.hiddenValue.x);
				if (ObscuredCheatingDetector.IsRunning && !this.fakeValue.Equals(ObscuredVector3.initialFakeValue) && Math.Abs(num - this.fakeValue.x) > ObscuredCheatingDetector.Instance.vector3Epsilon)
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

		// Token: 0x17000317 RID: 791
		// (get) Token: 0x0600123B RID: 4667 RVA: 0x000782F0 File Offset: 0x000764F0
		// (set) Token: 0x0600123C RID: 4668 RVA: 0x00010D7B File Offset: 0x0000EF7B
		public float y
		{
			get
			{
				float num = this.InternalDecryptField(this.hiddenValue.y);
				if (ObscuredCheatingDetector.IsRunning && !this.fakeValue.Equals(ObscuredVector3.initialFakeValue) && Math.Abs(num - this.fakeValue.y) > ObscuredCheatingDetector.Instance.vector3Epsilon)
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

		// Token: 0x17000318 RID: 792
		// (get) Token: 0x0600123D RID: 4669 RVA: 0x00078360 File Offset: 0x00076560
		// (set) Token: 0x0600123E RID: 4670 RVA: 0x00010DA5 File Offset: 0x0000EFA5
		public float z
		{
			get
			{
				float num = this.InternalDecryptField(this.hiddenValue.z);
				if (ObscuredCheatingDetector.IsRunning && !this.fakeValue.Equals(ObscuredVector3.initialFakeValue) && Math.Abs(num - this.fakeValue.z) > ObscuredCheatingDetector.Instance.vector3Epsilon)
				{
					ObscuredCheatingDetector.Instance.OnCheatingDetected();
				}
				return num;
			}
			set
			{
				this.hiddenValue.z = this.InternalEncryptField(value);
				if (ObscuredCheatingDetector.IsRunning)
				{
					this.fakeValue.z = value;
				}
			}
		}

		// Token: 0x17000319 RID: 793
		public float this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return this.x;
				case 1:
					return this.y;
				case 2:
					return this.z;
				default:
					throw new IndexOutOfRangeException("Invalid ObscuredVector3 index!");
				}
			}
			set
			{
				switch (index)
				{
				case 0:
					this.x = value;
					break;
				case 1:
					this.y = value;
					break;
				case 2:
					this.z = value;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid ObscuredVector3 index!");
				}
			}
		}

		// Token: 0x06001241 RID: 4673 RVA: 0x00010DCF File Offset: 0x0000EFCF
		public static void SetNewCryptoKey(int newKey)
		{
			ObscuredVector3.cryptoKey = newKey;
		}

		// Token: 0x06001242 RID: 4674 RVA: 0x00010DD7 File Offset: 0x0000EFD7
		public static ObscuredVector3.RawEncryptedVector3 Encrypt(Vector3 value)
		{
			return ObscuredVector3.Encrypt(value, 0);
		}

		// Token: 0x06001243 RID: 4675 RVA: 0x00078470 File Offset: 0x00076670
		public static ObscuredVector3.RawEncryptedVector3 Encrypt(Vector3 value, int key)
		{
			if (key == 0)
			{
				key = ObscuredVector3.cryptoKey;
			}
			ObscuredVector3.RawEncryptedVector3 result;
			result.x = ObscuredFloat.Encrypt(value.x, key);
			result.y = ObscuredFloat.Encrypt(value.y, key);
			result.z = ObscuredFloat.Encrypt(value.z, key);
			return result;
		}

		// Token: 0x06001244 RID: 4676 RVA: 0x00010DE0 File Offset: 0x0000EFE0
		public static Vector3 Decrypt(ObscuredVector3.RawEncryptedVector3 value)
		{
			return ObscuredVector3.Decrypt(value, 0);
		}

		// Token: 0x06001245 RID: 4677 RVA: 0x000784C8 File Offset: 0x000766C8
		public static Vector3 Decrypt(ObscuredVector3.RawEncryptedVector3 value, int key)
		{
			if (key == 0)
			{
				key = ObscuredVector3.cryptoKey;
			}
			Vector3 result;
			result.x = ObscuredFloat.Decrypt(value.x, key);
			result.y = ObscuredFloat.Decrypt(value.y, key);
			result.z = ObscuredFloat.Decrypt(value.z, key);
			return result;
		}

		// Token: 0x06001246 RID: 4678 RVA: 0x00010DE9 File Offset: 0x0000EFE9
		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredVector3.cryptoKey)
			{
				this.hiddenValue = ObscuredVector3.Encrypt(this.InternalDecrypt(), ObscuredVector3.cryptoKey);
				this.currentCryptoKey = ObscuredVector3.cryptoKey;
			}
		}

		// Token: 0x06001247 RID: 4679 RVA: 0x00078520 File Offset: 0x00076720
		public void RandomizeCryptoKey()
		{
			Vector3 value = this.InternalDecrypt();
			do
			{
				this.currentCryptoKey = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
			}
			while (this.currentCryptoKey == 0);
			this.hiddenValue = ObscuredVector3.Encrypt(value, this.currentCryptoKey);
		}

		// Token: 0x06001248 RID: 4680 RVA: 0x00010E1C File Offset: 0x0000F01C
		public ObscuredVector3.RawEncryptedVector3 GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		// Token: 0x06001249 RID: 4681 RVA: 0x00010E2A File Offset: 0x0000F02A
		public void SetEncrypted(ObscuredVector3.RawEncryptedVector3 encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.IsRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		// Token: 0x0600124A RID: 4682 RVA: 0x00078568 File Offset: 0x00076768
		private Vector3 InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredVector3.cryptoKey;
				this.hiddenValue = ObscuredVector3.Encrypt(ObscuredVector3.initialFakeValue, ObscuredVector3.cryptoKey);
				this.fakeValue = ObscuredVector3.initialFakeValue;
				this.inited = true;
			}
			Vector3 vector;
			vector.x = ObscuredFloat.Decrypt(this.hiddenValue.x, this.currentCryptoKey);
			vector.y = ObscuredFloat.Decrypt(this.hiddenValue.y, this.currentCryptoKey);
			vector.z = ObscuredFloat.Decrypt(this.hiddenValue.z, this.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning && !this.fakeValue.Equals(Vector3.zero) && !this.CompareVectorsWithTolerance(vector, this.fakeValue))
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return vector;
		}

		// Token: 0x0600124B RID: 4683 RVA: 0x0007864C File Offset: 0x0007684C
		private bool CompareVectorsWithTolerance(Vector3 vector1, Vector3 vector2)
		{
			float vector3Epsilon = ObscuredCheatingDetector.Instance.vector3Epsilon;
			return Math.Abs(vector1.x - vector2.x) < vector3Epsilon && Math.Abs(vector1.y - vector2.y) < vector3Epsilon && Math.Abs(vector1.z - vector2.z) < vector3Epsilon;
		}

		// Token: 0x0600124C RID: 4684 RVA: 0x000786B4 File Offset: 0x000768B4
		private float InternalDecryptField(int encrypted)
		{
			int key = ObscuredVector3.cryptoKey;
			if (this.currentCryptoKey != ObscuredVector3.cryptoKey)
			{
				key = this.currentCryptoKey;
			}
			return ObscuredFloat.Decrypt(encrypted, key);
		}

		// Token: 0x0600124D RID: 4685 RVA: 0x000786E8 File Offset: 0x000768E8
		private int InternalEncryptField(float encrypted)
		{
			return ObscuredFloat.Encrypt(encrypted, ObscuredVector3.cryptoKey);
		}

		// Token: 0x0600124E RID: 4686 RVA: 0x00078704 File Offset: 0x00076904
		public override bool Equals(object other)
		{
			return this.InternalDecrypt().Equals(other);
		}

		// Token: 0x0600124F RID: 4687 RVA: 0x00078720 File Offset: 0x00076920
		public override int GetHashCode()
		{
			return this.InternalDecrypt().GetHashCode();
		}

		// Token: 0x06001250 RID: 4688 RVA: 0x0007873C File Offset: 0x0007693C
		public override string ToString()
		{
			return this.InternalDecrypt().ToString();
		}

		// Token: 0x06001251 RID: 4689 RVA: 0x00078758 File Offset: 0x00076958
		public string ToString(string format)
		{
			return this.InternalDecrypt().ToString(format);
		}

		// Token: 0x06001252 RID: 4690 RVA: 0x00078774 File Offset: 0x00076974
		public static implicit operator ObscuredVector3(Vector3 value)
		{
			ObscuredVector3 result = new ObscuredVector3(ObscuredVector3.Encrypt(value, ObscuredVector3.cryptoKey));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		// Token: 0x06001253 RID: 4691 RVA: 0x00010E50 File Offset: 0x0000F050
		public static implicit operator Vector3(ObscuredVector3 value)
		{
			return value.InternalDecrypt();
		}

		// Token: 0x06001254 RID: 4692 RVA: 0x00010E59 File Offset: 0x0000F059
		public static ObscuredVector3 operator +(ObscuredVector3 a, ObscuredVector3 b)
		{
			return a.InternalDecrypt() + b.InternalDecrypt();
		}

		// Token: 0x06001255 RID: 4693 RVA: 0x00010E73 File Offset: 0x0000F073
		public static ObscuredVector3 operator +(Vector3 a, ObscuredVector3 b)
		{
			return a + b.InternalDecrypt();
		}

		// Token: 0x06001256 RID: 4694 RVA: 0x00010E87 File Offset: 0x0000F087
		public static ObscuredVector3 operator +(ObscuredVector3 a, Vector3 b)
		{
			return a.InternalDecrypt() + b;
		}

		// Token: 0x06001257 RID: 4695 RVA: 0x00010E9B File Offset: 0x0000F09B
		public static ObscuredVector3 operator -(ObscuredVector3 a, ObscuredVector3 b)
		{
			return a.InternalDecrypt() - b.InternalDecrypt();
		}

		// Token: 0x06001258 RID: 4696 RVA: 0x00010EB5 File Offset: 0x0000F0B5
		public static ObscuredVector3 operator -(Vector3 a, ObscuredVector3 b)
		{
			return a - b.InternalDecrypt();
		}

		// Token: 0x06001259 RID: 4697 RVA: 0x00010EC9 File Offset: 0x0000F0C9
		public static ObscuredVector3 operator -(ObscuredVector3 a, Vector3 b)
		{
			return a.InternalDecrypt() - b;
		}

		// Token: 0x0600125A RID: 4698 RVA: 0x00010EDD File Offset: 0x0000F0DD
		public static ObscuredVector3 operator -(ObscuredVector3 a)
		{
			return -a.InternalDecrypt();
		}

		// Token: 0x0600125B RID: 4699 RVA: 0x00010EF0 File Offset: 0x0000F0F0
		public static ObscuredVector3 operator *(ObscuredVector3 a, float d)
		{
			return a.InternalDecrypt() * d;
		}

		// Token: 0x0600125C RID: 4700 RVA: 0x00010F04 File Offset: 0x0000F104
		public static ObscuredVector3 operator *(float d, ObscuredVector3 a)
		{
			return d * a.InternalDecrypt();
		}

		// Token: 0x0600125D RID: 4701 RVA: 0x00010F18 File Offset: 0x0000F118
		public static ObscuredVector3 operator /(ObscuredVector3 a, float d)
		{
			return a.InternalDecrypt() / d;
		}

		// Token: 0x0600125E RID: 4702 RVA: 0x00010F2C File Offset: 0x0000F12C
		public static bool operator ==(ObscuredVector3 lhs, ObscuredVector3 rhs)
		{
			return lhs.InternalDecrypt() == rhs.InternalDecrypt();
		}

		// Token: 0x0600125F RID: 4703 RVA: 0x00010F41 File Offset: 0x0000F141
		public static bool operator ==(Vector3 lhs, ObscuredVector3 rhs)
		{
			return lhs == rhs.InternalDecrypt();
		}

		// Token: 0x06001260 RID: 4704 RVA: 0x00010F50 File Offset: 0x0000F150
		public static bool operator ==(ObscuredVector3 lhs, Vector3 rhs)
		{
			return lhs.InternalDecrypt() == rhs;
		}

		// Token: 0x06001261 RID: 4705 RVA: 0x00010F5F File Offset: 0x0000F15F
		public static bool operator !=(ObscuredVector3 lhs, ObscuredVector3 rhs)
		{
			return lhs.InternalDecrypt() != rhs.InternalDecrypt();
		}

		// Token: 0x06001262 RID: 4706 RVA: 0x00010F74 File Offset: 0x0000F174
		public static bool operator !=(Vector3 lhs, ObscuredVector3 rhs)
		{
			return lhs != rhs.InternalDecrypt();
		}

		// Token: 0x06001263 RID: 4707 RVA: 0x00010F83 File Offset: 0x0000F183
		public static bool operator !=(ObscuredVector3 lhs, Vector3 rhs)
		{
			return lhs.InternalDecrypt() != rhs;
		}

		// Token: 0x04000BE0 RID: 3040
		private static int cryptoKey = 542369;

		// Token: 0x04000BE1 RID: 3041
		private static readonly Vector3 initialFakeValue = Vector3.zero;

		// Token: 0x04000BE2 RID: 3042
		[SerializeField]
		private int currentCryptoKey;

		// Token: 0x04000BE3 RID: 3043
		[SerializeField]
		private ObscuredVector3.RawEncryptedVector3 hiddenValue;

		// Token: 0x04000BE4 RID: 3044
		[SerializeField]
		private Vector3 fakeValue;

		// Token: 0x04000BE5 RID: 3045
		[SerializeField]
		private bool inited;

		// Token: 0x020001D5 RID: 469
		[Serializable]
		public struct RawEncryptedVector3
		{
			// Token: 0x04000BE6 RID: 3046
			public int x;

			// Token: 0x04000BE7 RID: 3047
			public int y;

			// Token: 0x04000BE8 RID: 3048
			public int z;
		}
	}
}
