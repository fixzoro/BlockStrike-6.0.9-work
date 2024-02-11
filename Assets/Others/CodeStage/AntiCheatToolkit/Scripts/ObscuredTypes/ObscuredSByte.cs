using System;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	// Token: 0x020001CC RID: 460
	[Serializable]
	public struct ObscuredSByte : IFormattable, IEquatable<ObscuredSByte>
	{
		// Token: 0x06001195 RID: 4501 RVA: 0x00010633 File Offset: 0x0000E833
		private ObscuredSByte(sbyte value)
		{
			this.currentCryptoKey = ObscuredSByte.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = 0;
			this.inited = true;
		}

		// Token: 0x06001197 RID: 4503 RVA: 0x0001065E File Offset: 0x0000E85E
		public static void SetNewCryptoKey(sbyte newKey)
		{
			ObscuredSByte.cryptoKey = newKey;
		}

		// Token: 0x06001198 RID: 4504 RVA: 0x00010666 File Offset: 0x0000E866
		public static sbyte EncryptDecrypt(sbyte value)
		{
			return ObscuredSByte.EncryptDecrypt(value, 0);
		}

		// Token: 0x06001199 RID: 4505 RVA: 0x0001066F File Offset: 0x0000E86F
		public static sbyte EncryptDecrypt(sbyte value, sbyte key)
		{
			if ((int)key == 0)
			{
				return (sbyte)((int)value ^ (int)ObscuredSByte.cryptoKey);
			}
			return (sbyte)((int)value ^ (int)key);
		}

		// Token: 0x0600119A RID: 4506 RVA: 0x00010689 File Offset: 0x0000E889
		public void ApplyNewCryptoKey()
		{
			if ((int)this.currentCryptoKey != (int)ObscuredSByte.cryptoKey)
			{
				this.hiddenValue = ObscuredSByte.EncryptDecrypt(this.InternalDecrypt(), ObscuredSByte.cryptoKey);
				this.currentCryptoKey = ObscuredSByte.cryptoKey;
			}
		}

		// Token: 0x0600119B RID: 4507 RVA: 0x00076FB0 File Offset: 0x000751B0
		public void RandomizeCryptoKey()
		{
			sbyte value = this.InternalDecrypt();
			do
			{
				this.currentCryptoKey = (sbyte)UnityEngine.Random.Range(-128, 127);
			}
			while ((int)this.currentCryptoKey == 0);
			this.hiddenValue = ObscuredSByte.EncryptDecrypt(value, this.currentCryptoKey);
		}

		// Token: 0x0600119C RID: 4508 RVA: 0x000106BE File Offset: 0x0000E8BE
		public sbyte GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		// Token: 0x0600119D RID: 4509 RVA: 0x000106CC File Offset: 0x0000E8CC
		public void SetEncrypted(sbyte encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.IsRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		// Token: 0x0600119E RID: 4510 RVA: 0x00076FF4 File Offset: 0x000751F4
		private sbyte InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredSByte.cryptoKey;
				this.hiddenValue = ObscuredSByte.EncryptDecrypt(0);
				this.fakeValue = 0;
				this.inited = true;
			}
			sbyte b = ObscuredSByte.EncryptDecrypt(this.hiddenValue, this.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning && (int)this.fakeValue != 0 && (int)b != (int)this.fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return b;
		}

		// Token: 0x0600119F RID: 4511 RVA: 0x000106F2 File Offset: 0x0000E8F2
		public override bool Equals(object obj)
		{
			return obj is ObscuredSByte && this.Equals((ObscuredSByte)obj);
		}

		// Token: 0x060011A0 RID: 4512 RVA: 0x00077074 File Offset: 0x00075274
		public bool Equals(ObscuredSByte obj)
		{
			if ((int)this.currentCryptoKey == (int)obj.currentCryptoKey)
			{
				return (int)this.hiddenValue == (int)obj.hiddenValue;
			}
			return (int)ObscuredSByte.EncryptDecrypt(this.hiddenValue, this.currentCryptoKey) == (int)ObscuredSByte.EncryptDecrypt(obj.hiddenValue, obj.currentCryptoKey);
		}

		// Token: 0x060011A1 RID: 4513 RVA: 0x000770D0 File Offset: 0x000752D0
		public override string ToString()
		{
			return this.InternalDecrypt().ToString();
		}

		// Token: 0x060011A2 RID: 4514 RVA: 0x000770EC File Offset: 0x000752EC
		public string ToString(string format)
		{
			return this.InternalDecrypt().ToString(format);
		}

		// Token: 0x060011A3 RID: 4515 RVA: 0x00077108 File Offset: 0x00075308
		public override int GetHashCode()
		{
			return this.InternalDecrypt().GetHashCode();
		}

		// Token: 0x060011A4 RID: 4516 RVA: 0x00077124 File Offset: 0x00075324
		public string ToString(IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(provider);
		}

		// Token: 0x060011A5 RID: 4517 RVA: 0x00077140 File Offset: 0x00075340
		public string ToString(string format, IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(format, provider);
		}

		// Token: 0x060011A6 RID: 4518 RVA: 0x00077160 File Offset: 0x00075360
		public static implicit operator ObscuredSByte(sbyte value)
		{
			ObscuredSByte result = new ObscuredSByte(ObscuredSByte.EncryptDecrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		// Token: 0x060011A7 RID: 4519 RVA: 0x0001070D File Offset: 0x0000E90D
		public static implicit operator sbyte(ObscuredSByte value)
		{
			return value.InternalDecrypt();
		}

		// Token: 0x060011A8 RID: 4520 RVA: 0x00077190 File Offset: 0x00075390
		public static ObscuredSByte operator ++(ObscuredSByte input)
		{
			sbyte value = (sbyte)((int)input.InternalDecrypt() + 1);
			input.hiddenValue = ObscuredSByte.EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		// Token: 0x060011A9 RID: 4521 RVA: 0x000771D0 File Offset: 0x000753D0
		public static ObscuredSByte operator --(ObscuredSByte input)
		{
			sbyte value = (sbyte)((int)input.InternalDecrypt() - 1);
			input.hiddenValue = ObscuredSByte.EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		// Token: 0x04000BBA RID: 3002
		private static sbyte cryptoKey = 112;

		// Token: 0x04000BBB RID: 3003
		private sbyte currentCryptoKey;

		// Token: 0x04000BBC RID: 3004
		private sbyte hiddenValue;

		// Token: 0x04000BBD RID: 3005
		private sbyte fakeValue;

		// Token: 0x04000BBE RID: 3006
		private bool inited;
	}
}
