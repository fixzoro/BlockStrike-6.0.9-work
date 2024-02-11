using System;
using System.Diagnostics;
using System.Text;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace CodeStage.AntiCheat.Examples
{
	// Token: 0x020001AF RID: 431
	[AddComponentMenu("")]
	public class ObscuredPerformanceTests : MonoBehaviour
	{
		// Token: 0x06000FCD RID: 4045 RVA: 0x0000F31C File Offset: 0x0000D51C
		private void Start()
		{
			base.Invoke("StartTests", 1f);
		}

		// Token: 0x06000FCE RID: 4046 RVA: 0x00070FF4 File Offset: 0x0006F1F4
		private void StartTests()
		{
			this.logBuilder.Length = 0;
			this.logBuilder.AppendLine("[ACTk] <b>[ Performance tests ]</b>");
			if (this.boolTest)
			{
				this.TestBool();
			}
			if (this.byteTest)
			{
				this.TestByte();
			}
			if (this.shortTest)
			{
				this.TestShort();
			}
			if (this.ushortTest)
			{
				this.TestUShort();
			}
			if (this.intTest)
			{
				this.TestInt();
			}
			if (this.uintTest)
			{
				this.TestUInt();
			}
			if (this.longTest)
			{
				this.TestLong();
			}
			if (this.floatTest)
			{
				this.TestFloat();
			}
			if (this.doubleTest)
			{
				this.TestDouble();
			}
			if (this.stringTest)
			{
				this.TestString();
			}
			if (this.vector3Test)
			{
				this.TestVector3();
			}
			if (this.prefsTest)
			{
				this.TestPrefs();
			}
			UnityEngine.Debug.Log(this.logBuilder);
		}

		// Token: 0x06000FCF RID: 4047 RVA: 0x000710F8 File Offset: 0x0006F2F8
		private void TestBool()
		{
			this.logBuilder.AppendLine("ObscuredBool vs bool, " + this.boolIterations + " iterations for read and write");
			ObscuredBool value = true;
			bool flag = value;
			bool flag2 = false;
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < this.boolIterations; i++)
			{
				flag2 = value;
			}
			for (int j = 0; j < this.boolIterations; j++)
			{
				value = flag2;
			}
			stopwatch.Stop();
			this.logBuilder.AppendLine("ObscuredBool:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			stopwatch.Reset();
			stopwatch.Start();
			for (int k = 0; k < this.boolIterations; k++)
			{
				flag2 = flag;
			}
			for (int l = 0; l < this.boolIterations; l++)
			{
				flag = flag2;
			}
			stopwatch.Stop();
			this.logBuilder.AppendLine("bool:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			if (flag2)
			{
			}
			if (value)
			{
			}
			if (flag)
			{
			}
		}

		// Token: 0x06000FD0 RID: 4048 RVA: 0x00071244 File Offset: 0x0006F444
		private void TestByte()
		{
			this.logBuilder.AppendLine("ObscuredByte vs byte, " + this.byteIterations + " iterations for read and write");
			ObscuredByte value = 100;
			byte b = value;
			byte b2 = 0;
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < this.byteIterations; i++)
			{
				b2 = value;
			}
			for (int j = 0; j < this.byteIterations; j++)
			{
				value = b2;
			}
			stopwatch.Stop();
			this.logBuilder.AppendLine("ObscuredByte:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			stopwatch.Reset();
			stopwatch.Start();
			for (int k = 0; k < this.byteIterations; k++)
			{
				b2 = b;
			}
			for (int l = 0; l < this.byteIterations; l++)
			{
				b = b2;
			}
			stopwatch.Stop();
			this.logBuilder.AppendLine("byte:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			if (b2 != 0)
			{
			}
			if (value != 0)
			{
			}
			if (b != 0)
			{
			}
		}

		// Token: 0x06000FD1 RID: 4049 RVA: 0x00071394 File Offset: 0x0006F594
		private void TestShort()
		{
			this.logBuilder.AppendLine("ObscuredShort vs short, " + this.shortIterations + " iterations for read and write");
			ObscuredShort value = 100;
			short num = value;
			short num2 = 0;
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < this.shortIterations; i++)
			{
				num2 = value;
			}
			for (int j = 0; j < this.shortIterations; j++)
			{
				value = num2;
			}
			stopwatch.Stop();
			this.logBuilder.AppendLine("ObscuredShort:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			stopwatch.Reset();
			stopwatch.Start();
			for (int k = 0; k < this.shortIterations; k++)
			{
				num2 = num;
			}
			for (int l = 0; l < this.shortIterations; l++)
			{
				num = num2;
			}
			stopwatch.Stop();
			this.logBuilder.AppendLine("short:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			if (num2 != 0)
			{
			}
			if (value != 0)
			{
			}
			if (num != 0)
			{
			}
		}

		// Token: 0x06000FD2 RID: 4050 RVA: 0x000714E4 File Offset: 0x0006F6E4
		private void TestUShort()
		{
			this.logBuilder.AppendLine("ObscuredUShort vs ushort, " + this.ushortIterations + " iterations for read and write");
			ObscuredUShort value = 100;
			ushort num = value;
			ushort num2 = 0;
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < this.ushortIterations; i++)
			{
				num2 = value;
			}
			for (int j = 0; j < this.ushortIterations; j++)
			{
				value = num2;
			}
			stopwatch.Stop();
			this.logBuilder.AppendLine("ObscuredUShort:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			stopwatch.Reset();
			stopwatch.Start();
			for (int k = 0; k < this.ushortIterations; k++)
			{
				num2 = num;
			}
			for (int l = 0; l < this.ushortIterations; l++)
			{
				num = num2;
			}
			stopwatch.Stop();
			this.logBuilder.AppendLine("ushort:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			if (num2 != 0)
			{
			}
			if (value != 0)
			{
			}
			if (num != 0)
			{
			}
		}

		// Token: 0x06000FD3 RID: 4051 RVA: 0x00071634 File Offset: 0x0006F834
		private void TestDouble()
		{
			this.logBuilder.AppendLine("ObscuredDouble vs double, " + this.doubleIterations + " iterations for read and write");
			ObscuredDouble value = 100.0;
			double num = value;
			double num2 = 0.0;
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < this.doubleIterations; i++)
			{
				num2 = value;
			}
			for (int j = 0; j < this.doubleIterations; j++)
			{
				value = num2;
			}
			stopwatch.Stop();
			this.logBuilder.AppendLine("ObscuredDouble:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			stopwatch.Reset();
			stopwatch.Start();
			for (int k = 0; k < this.doubleIterations; k++)
			{
				num2 = num;
			}
			for (int l = 0; l < this.doubleIterations; l++)
			{
				num = num2;
			}
			stopwatch.Stop();
			this.logBuilder.AppendLine("double:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			if (num2 != 0.0)
			{
			}
			if (value != 0.0)
			{
			}
			if (num != 0.0)
			{
			}
		}

		// Token: 0x06000FD4 RID: 4052 RVA: 0x000717AC File Offset: 0x0006F9AC
		private void TestFloat()
		{
			this.logBuilder.AppendLine("ObscuredFloat vs float, " + this.floatIterations + " iterations for read and write");
			ObscuredFloat value = 100f;
			CryptoFloat value2 = 100f;
			float num = 0f;
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < this.floatIterations; i++)
			{
				num = value;
			}
			for (int j = 0; j < this.floatIterations; j++)
			{
				value = num;
			}
			stopwatch.Stop();
			this.logBuilder.AppendLine("ObscuredFloat:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			stopwatch.Reset();
			stopwatch.Start();
			for (int k = 0; k < this.floatIterations; k++)
			{
				num = value2;
			}
			for (int l = 0; l < this.floatIterations; l++)
			{
				value2 = num;
			}
			stopwatch.Stop();
			this.logBuilder.AppendLine("float:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			if (num != 0f)
			{
			}
			if (value != 0f)
			{
			}
			if (value2 != 0f)
			{
			}
		}

		// Token: 0x06000FD5 RID: 4053 RVA: 0x00071924 File Offset: 0x0006FB24
		private void TestInt()
		{
			this.logBuilder.AppendLine("ObscuredInt vs int, " + this.intIterations + " iterations for read and write");
			ObscuredInt value = 100;
			CryptoInt value2 = 100;
			int num = 0;
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < this.intIterations; i++)
			{
				num = value;
			}
			for (int j = 0; j < this.intIterations; j++)
			{
				value = num;
			}
			stopwatch.Stop();
			this.logBuilder.AppendLine("ObscuredInt:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			stopwatch.Reset();
			stopwatch.Start();
			for (int k = 0; k < this.intIterations; k++)
			{
				num = value2;
			}
			for (int l = 0; l < this.intIterations; l++)
			{
				value2 = num;
			}
			stopwatch.Stop();
			this.logBuilder.AppendLine("int:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			if (num != 0)
			{
			}
			if (value != 0)
			{
			}
			if (value2 != 0)
			{
			}
		}

		// Token: 0x06000FD6 RID: 4054 RVA: 0x00071A84 File Offset: 0x0006FC84
		private void TestLong()
		{
			this.logBuilder.AppendLine("ObscuredLong vs long, " + this.longIterations + " iterations for read and write");
			ObscuredLong value = 100L;
			long num = value;
			long num2 = 0L;
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < this.longIterations; i++)
			{
				num2 = value;
			}
			for (int j = 0; j < this.longIterations; j++)
			{
				value = num2;
			}
			stopwatch.Stop();
			this.logBuilder.AppendLine("ObscuredLong:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			stopwatch.Reset();
			stopwatch.Start();
			for (int k = 0; k < this.longIterations; k++)
			{
				num2 = num;
			}
			for (int l = 0; l < this.longIterations; l++)
			{
				num = num2;
			}
			stopwatch.Stop();
			this.logBuilder.AppendLine("long:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			if (num2 != 0L)
			{
			}
			if (value != 0L)
			{
			}
			if (num != 0L)
			{
			}
		}

		// Token: 0x06000FD7 RID: 4055 RVA: 0x00071BD4 File Offset: 0x0006FDD4
		private void TestString()
		{
			this.logBuilder.AppendLine("ObscuredString vs string, " + this.stringIterations + " iterations for read and write");
			ObscuredString obscuredString = "abcd";
			string text = obscuredString;
			string text2 = string.Empty;
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < this.stringIterations; i++)
			{
				text2 = obscuredString;
			}
			for (int j = 0; j < this.stringIterations; j++)
			{
				obscuredString = text2;
			}
			stopwatch.Stop();
			this.logBuilder.AppendLine("ObscuredString:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			stopwatch.Reset();
			stopwatch.Start();
			for (int k = 0; k < this.stringIterations; k++)
			{
				text2 = text;
			}
			for (int l = 0; l < this.stringIterations; l++)
			{
				text = text2;
			}
			stopwatch.Stop();
			this.logBuilder.AppendLine("string:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			if (text2 != string.Empty)
			{
			}
			if (obscuredString != string.Empty)
			{
			}
			if (text != string.Empty)
			{
			}
		}

		// Token: 0x06000FD8 RID: 4056 RVA: 0x00071D48 File Offset: 0x0006FF48
		private void TestUInt()
		{
			this.logBuilder.AppendLine("ObscuredUInt vs uint, " + this.uintIterations + " iterations for read and write");
			ObscuredUInt value = 100U;
			uint num = value;
			uint num2 = 0U;
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < this.uintIterations; i++)
			{
				num2 = value;
			}
			for (int j = 0; j < this.uintIterations; j++)
			{
				value = num2;
			}
			stopwatch.Stop();
			this.logBuilder.AppendLine("ObscuredUInt:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			stopwatch.Reset();
			stopwatch.Start();
			for (int k = 0; k < this.uintIterations; k++)
			{
				num2 = num;
			}
			for (int l = 0; l < this.uintIterations; l++)
			{
				num = num2;
			}
			stopwatch.Stop();
			this.logBuilder.AppendLine("uint:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			if (num2 != 0U)
			{
			}
			if (value != 0U)
			{
			}
			if (num != 0U)
			{
			}
		}

		// Token: 0x06000FD9 RID: 4057 RVA: 0x00071E98 File Offset: 0x00070098
		private void TestVector3()
		{
			this.logBuilder.AppendLine("ObscuredVector3 vs Vector3, " + this.vector3Iterations + " iterations for read and write");
			ObscuredVector3 obscuredVector = new Vector3(1f, 2f, 3f);
			Vector3 vector = obscuredVector;
			Vector3 vector2 = new Vector3(0f, 0f, 0f);
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < this.vector3Iterations; i++)
			{
				vector2 = obscuredVector;
			}
			for (int j = 0; j < this.vector3Iterations; j++)
			{
				obscuredVector = vector2;
			}
			stopwatch.Stop();
			this.logBuilder.AppendLine("ObscuredVector3:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			stopwatch.Reset();
			stopwatch.Start();
			for (int k = 0; k < this.vector3Iterations; k++)
			{
				vector2 = vector;
			}
			for (int l = 0; l < this.vector3Iterations; l++)
			{
				vector = vector2;
			}
			stopwatch.Stop();
			this.logBuilder.AppendLine("Vector3:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			if (vector2 != Vector3.zero)
			{
			}
			if (obscuredVector != Vector3.zero)
			{
			}
			if (vector != Vector3.zero)
			{
			}
		}

		// Token: 0x06000FDA RID: 4058 RVA: 0x00072024 File Offset: 0x00070224
		private void TestPrefs()
		{
			this.logBuilder.AppendLine("ObscuredPrefs vs PlayerPrefs, " + this.prefsIterations + " iterations for read and write");
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < this.prefsIterations; i++)
			{
				ObscuredPrefs.SetInt("__a", 1);
				ObscuredPrefs.SetFloat("__b", 2f);
				ObscuredPrefs.SetString("__c", "3");
			}
			for (int j = 0; j < this.prefsIterations; j++)
			{
				ObscuredPrefs.GetInt("__a", 1);
				ObscuredPrefs.GetFloat("__b", 2f);
				ObscuredPrefs.GetString("__c", "3");
			}
			stopwatch.Stop();
			this.logBuilder.AppendLine("ObscuredPrefs:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			ObscuredPrefs.DeleteKey("__a");
			ObscuredPrefs.DeleteKey("__b");
			ObscuredPrefs.DeleteKey("__c");
			stopwatch.Reset();
			stopwatch.Start();
			for (int k = 0; k < this.prefsIterations; k++)
			{
				PlayerPrefs.SetInt("__a", 1);
				PlayerPrefs.SetFloat("__b", 2f);
				PlayerPrefs.SetString("__c", "3");
			}
			for (int l = 0; l < this.prefsIterations; l++)
			{
				PlayerPrefs.GetInt("__a", 1);
				PlayerPrefs.GetFloat("__b", 2f);
				PlayerPrefs.GetString("__c", "3");
			}
			stopwatch.Stop();
			this.logBuilder.AppendLine("PlayerPrefs:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			PlayerPrefs.DeleteKey("__a");
			PlayerPrefs.DeleteKey("__b");
			PlayerPrefs.DeleteKey("__c");
		}

		// Token: 0x04000AC3 RID: 2755
		public bool boolTest = true;

		// Token: 0x04000AC4 RID: 2756
		public int boolIterations = 2500000;

		// Token: 0x04000AC5 RID: 2757
		public bool byteTest = true;

		// Token: 0x04000AC6 RID: 2758
		public int byteIterations = 2500000;

		// Token: 0x04000AC7 RID: 2759
		public bool shortTest = true;

		// Token: 0x04000AC8 RID: 2760
		public int shortIterations = 2500000;

		// Token: 0x04000AC9 RID: 2761
		public bool ushortTest = true;

		// Token: 0x04000ACA RID: 2762
		public int ushortIterations = 2500000;

		// Token: 0x04000ACB RID: 2763
		public bool intTest = true;

		// Token: 0x04000ACC RID: 2764
		public int intIterations = 2500000;

		// Token: 0x04000ACD RID: 2765
		public bool uintTest = true;

		// Token: 0x04000ACE RID: 2766
		public int uintIterations = 2500000;

		// Token: 0x04000ACF RID: 2767
		public bool longTest = true;

		// Token: 0x04000AD0 RID: 2768
		public int longIterations = 2500000;

		// Token: 0x04000AD1 RID: 2769
		public bool floatTest = true;

		// Token: 0x04000AD2 RID: 2770
		public int floatIterations = 2500000;

		// Token: 0x04000AD3 RID: 2771
		public bool doubleTest = true;

		// Token: 0x04000AD4 RID: 2772
		public int doubleIterations = 2500000;

		// Token: 0x04000AD5 RID: 2773
		public bool stringTest = true;

		// Token: 0x04000AD6 RID: 2774
		public int stringIterations = 250000;

		// Token: 0x04000AD7 RID: 2775
		public bool vector3Test = true;

		// Token: 0x04000AD8 RID: 2776
		public int vector3Iterations = 2500000;

		// Token: 0x04000AD9 RID: 2777
		public bool prefsTest = true;

		// Token: 0x04000ADA RID: 2778
		public int prefsIterations = 2500;

		// Token: 0x04000ADB RID: 2779
		private readonly StringBuilder logBuilder = new StringBuilder();
	}
}
