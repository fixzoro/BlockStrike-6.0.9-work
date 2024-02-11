using System;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	// Token: 0x020001CF RID: 463
	[Serializable]
	public struct ObscuredUInt : IFormattable, IEquatable<ObscuredUInt>
	{
		// Token: 0x060011D8 RID: 4568 RVA: 0x0001093E File Offset: 0x0000EB3E
		private ObscuredUInt(uint value)
		{
			this.currentCryptoKey = ObscuredUInt.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = 0U;
			this.inited = true;
		}

		// Token: 0x060011DA RID: 4570 RVA: 0x0001096C File Offset: 0x0000EB6C
		public static void SetNewCryptoKey(uint newKey)
		{
			ObscuredUInt.cryptoKey = newKey;
		}

		// Token: 0x060011DB RID: 4571 RVA: 0x00010974 File Offset: 0x0000EB74
		public static uint Encrypt(uint value)
		{
			return ObscuredUInt.Encrypt(value, 0U);
		}

		// Token: 0x060011DC RID: 4572 RVA: 0x0001097D File Offset: 0x0000EB7D
		public static uint Decrypt(uint value)
		{
			return ObscuredUInt.Decrypt(value, 0U);
		}

		// Token: 0x060011DD RID: 4573 RVA: 0x00010986 File Offset: 0x0000EB86
		public static uint Encrypt(uint value, uint key)
		{
			if (key == 0U)
			{
				return value ^ ObscuredUInt.cryptoKey;
			}
			return value ^ key;
		}

		// Token: 0x060011DE RID: 4574 RVA: 0x00010986 File Offset: 0x0000EB86
		public static uint Decrypt(uint value, uint key)
		{
			if (key == 0U)
			{
				return value ^ ObscuredUInt.cryptoKey;
			}
			return value ^ key;
		}

		// Token: 0x060011DF RID: 4575 RVA: 0x00010999 File Offset: 0x0000EB99
		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredUInt.cryptoKey)
			{
				this.hiddenValue = ObscuredUInt.Encrypt(this.InternalDecrypt(), ObscuredUInt.cryptoKey);
				this.currentCryptoKey = ObscuredUInt.cryptoKey;
			}
		}

		// Token: 0x060011E0 RID: 4576 RVA: 0x00077758 File Offset: 0x00075958
		public void RandomizeCryptoKey()
		{
			uint value = this.InternalDecrypt();
			this.currentCryptoKey = (uint)UnityEngine.Random.Range(1, int.MaxValue);
			this.hiddenValue = ObscuredUInt.Encrypt(value, this.currentCryptoKey);
		}

		// Token: 0x060011E1 RID: 4577 RVA: 0x000109CC File Offset: 0x0000EBCC
		public uint GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		// Token: 0x060011E2 RID: 4578 RVA: 0x000109DA File Offset: 0x0000EBDA
		public void SetEncrypted(uint encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.IsRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		// Token: 0x060011E3 RID: 4579 RVA: 0x00077790 File Offset: 0x00075990
		private uint InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredUInt.cryptoKey;
				this.hiddenValue = ObscuredUInt.Encrypt(0U);
				this.fakeValue = 0U;
				this.inited = true;
			}
			uint num = ObscuredUInt.Decrypt(this.hiddenValue, this.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning && this.fakeValue != 0U && num != this.fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return num;
		}

		// Token: 0x060011E4 RID: 4580 RVA: 0x00010A00 File Offset: 0x0000EC00
		public override bool Equals(object obj)
		{
			return obj is ObscuredUInt && this.Equals((ObscuredUInt)obj);
		}

		// Token: 0x060011E5 RID: 4581 RVA: 0x0007780C File Offset: 0x00075A0C
		public bool Equals(ObscuredUInt obj)
		{
			if (this.currentCryptoKey == obj.currentCryptoKey)
			{
				return this.hiddenValue == obj.hiddenValue;
			}
			return ObscuredUInt.Decrypt(this.hiddenValue, this.currentCryptoKey) == ObscuredUInt.Decrypt(obj.hiddenValue, obj.currentCryptoKey);
		}

		// Token: 0x060011E6 RID: 4582 RVA: 0x00077864 File Offset: 0x00075A64
		public override string ToString()
		{
			return this.InternalDecrypt().ToString();
		}

		// Token: 0x060011E7 RID: 4583 RVA: 0x00077880 File Offset: 0x00075A80
		public string ToString(string format)
		{
			return this.InternalDecrypt().ToString(format);
		}

		// Token: 0x060011E8 RID: 4584 RVA: 0x0007789C File Offset: 0x00075A9C
		public override int GetHashCode()
		{
			return this.InternalDecrypt().GetHashCode();
		}

		// Token: 0x060011E9 RID: 4585 RVA: 0x000778B8 File Offset: 0x00075AB8
		public string ToString(IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(provider);
		}

		// Token: 0x060011EA RID: 4586 RVA: 0x000778D4 File Offset: 0x00075AD4
		public string ToString(string format, IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(format, provider);
		}

		// Token: 0x060011EB RID: 4587 RVA: 0x000778F4 File Offset: 0x00075AF4
		public static implicit operator ObscuredUInt(uint value)
		{
			ObscuredUInt result = new ObscuredUInt(ObscuredUInt.Encrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		// Token: 0x060011EC RID: 4588 RVA: 0x00010A1B File Offset: 0x0000EC1B
		public static implicit operator uint(ObscuredUInt value)
		{
			return value.InternalDecrypt();
		}

		// Token: 0x060011ED RID: 4589 RVA: 0x00010A24 File Offset: 0x0000EC24
		public static explicit operator ObscuredInt(ObscuredUInt value)
		{
			return (int)value.InternalDecrypt();
		}

		// Token: 0x060011EE RID: 4590 RVA: 0x00077924 File Offset: 0x00075B24
		public static ObscuredUInt operator ++(ObscuredUInt input)
		{
			uint value = input.InternalDecrypt() + 1U;
			input.hiddenValue = ObscuredUInt.Encrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		// Token: 0x060011EF RID: 4591 RVA: 0x00077964 File Offset: 0x00075B64
		public static ObscuredUInt operator --(ObscuredUInt input)
		{
			uint value = input.InternalDecrypt() - 1U;
			input.hiddenValue = ObscuredUInt.Encrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		// Token: 0x04000BC9 RID: 3017
		private static uint cryptoKey = 240513U;

		// Token: 0x04000BCA RID: 3018
		private uint currentCryptoKey;

		// Token: 0x04000BCB RID: 3019
		private uint hiddenValue;

		// Token: 0x04000BCC RID: 3020
		private uint fakeValue;

		// Token: 0x04000BCD RID: 3021
		private bool inited;
	}
}
