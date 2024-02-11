using System;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	// Token: 0x020001D1 RID: 465
	[Serializable]
	public struct ObscuredUShort : IFormattable, IEquatable<ObscuredUShort>
	{
		// Token: 0x06001207 RID: 4615 RVA: 0x00010B1C File Offset: 0x0000ED1C
		private ObscuredUShort(ushort value)
		{
			this.currentCryptoKey = ObscuredUShort.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = 0;
			this.inited = true;
		}

		// Token: 0x06001209 RID: 4617 RVA: 0x00010B4A File Offset: 0x0000ED4A
		public static void SetNewCryptoKey(ushort newKey)
		{
			ObscuredUShort.cryptoKey = newKey;
		}

		// Token: 0x0600120A RID: 4618 RVA: 0x00010B52 File Offset: 0x0000ED52
		public static ushort EncryptDecrypt(ushort value)
		{
			return ObscuredUShort.EncryptDecrypt(value, 0);
		}

		// Token: 0x0600120B RID: 4619 RVA: 0x00010B5B File Offset: 0x0000ED5B
		public static ushort EncryptDecrypt(ushort value, ushort key)
		{
			if (key == 0)
			{
				return (ushort)(value ^ ObscuredUShort.cryptoKey);
			}
			return (ushort)(value ^ key);
		}

		// Token: 0x0600120C RID: 4620 RVA: 0x00010B70 File Offset: 0x0000ED70
		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredUShort.cryptoKey)
			{
				this.hiddenValue = ObscuredUShort.EncryptDecrypt(this.InternalDecrypt(), ObscuredUShort.cryptoKey);
				this.currentCryptoKey = ObscuredUShort.cryptoKey;
			}
		}

		// Token: 0x0600120D RID: 4621 RVA: 0x00077BF4 File Offset: 0x00075DF4
		public void RandomizeCryptoKey()
		{
			ushort value = this.InternalDecrypt();
			this.currentCryptoKey = (ushort)UnityEngine.Random.Range(1, 32767);
			this.hiddenValue = ObscuredUShort.EncryptDecrypt(value, this.currentCryptoKey);
		}

		// Token: 0x0600120E RID: 4622 RVA: 0x00010BA3 File Offset: 0x0000EDA3
		public ushort GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		// Token: 0x0600120F RID: 4623 RVA: 0x00010BB1 File Offset: 0x0000EDB1
		public void SetEncrypted(ushort encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.IsRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		// Token: 0x06001210 RID: 4624 RVA: 0x00077C2C File Offset: 0x00075E2C
		private ushort InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredUShort.cryptoKey;
				this.hiddenValue = ObscuredUShort.EncryptDecrypt(0);
				this.fakeValue = 0;
				this.inited = true;
			}
			ushort num = ObscuredUShort.EncryptDecrypt(this.hiddenValue, this.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning && this.fakeValue != 0 && num != this.fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return num;
		}

		// Token: 0x06001211 RID: 4625 RVA: 0x00010BD7 File Offset: 0x0000EDD7
		public override bool Equals(object obj)
		{
			return obj is ObscuredUShort && this.Equals((ObscuredUShort)obj);
		}

		// Token: 0x06001212 RID: 4626 RVA: 0x00077CA8 File Offset: 0x00075EA8
		public bool Equals(ObscuredUShort obj)
		{
			if (this.currentCryptoKey == obj.currentCryptoKey)
			{
				return this.hiddenValue == obj.hiddenValue;
			}
			return ObscuredUShort.EncryptDecrypt(this.hiddenValue, this.currentCryptoKey) == ObscuredUShort.EncryptDecrypt(obj.hiddenValue, obj.currentCryptoKey);
		}

		// Token: 0x06001213 RID: 4627 RVA: 0x00077D00 File Offset: 0x00075F00
		public override string ToString()
		{
			return this.InternalDecrypt().ToString();
		}

		// Token: 0x06001214 RID: 4628 RVA: 0x00077D1C File Offset: 0x00075F1C
		public string ToString(string format)
		{
			return this.InternalDecrypt().ToString(format);
		}

		// Token: 0x06001215 RID: 4629 RVA: 0x00077D38 File Offset: 0x00075F38
		public override int GetHashCode()
		{
			return this.InternalDecrypt().GetHashCode();
		}

		// Token: 0x06001216 RID: 4630 RVA: 0x00077D54 File Offset: 0x00075F54
		public string ToString(IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(provider);
		}

		// Token: 0x06001217 RID: 4631 RVA: 0x00077D70 File Offset: 0x00075F70
		public string ToString(string format, IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(format, provider);
		}

		// Token: 0x06001218 RID: 4632 RVA: 0x00077D90 File Offset: 0x00075F90
		public static implicit operator ObscuredUShort(ushort value)
		{
			ObscuredUShort result = new ObscuredUShort(ObscuredUShort.EncryptDecrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		// Token: 0x06001219 RID: 4633 RVA: 0x00010BF2 File Offset: 0x0000EDF2
		public static implicit operator ushort(ObscuredUShort value)
		{
			return value.InternalDecrypt();
		}

		// Token: 0x0600121A RID: 4634 RVA: 0x00077DC0 File Offset: 0x00075FC0
		public static ObscuredUShort operator ++(ObscuredUShort input)
		{
			ushort value = (ushort)(input.InternalDecrypt() + 1);
			input.hiddenValue = ObscuredUShort.EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		// Token: 0x0600121B RID: 4635 RVA: 0x00077E00 File Offset: 0x00076000
		public static ObscuredUShort operator --(ObscuredUShort input)
		{
			ushort value = (ushort)(input.InternalDecrypt() - 1);
			input.hiddenValue = ObscuredUShort.EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		// Token: 0x04000BD3 RID: 3027
		private static ushort cryptoKey = 224;

		// Token: 0x04000BD4 RID: 3028
		private ushort currentCryptoKey;

		// Token: 0x04000BD5 RID: 3029
		private ushort hiddenValue;

		// Token: 0x04000BD6 RID: 3030
		private ushort fakeValue;

		// Token: 0x04000BD7 RID: 3031
		private bool inited;
	}
}
