using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

// Token: 0x02000096 RID: 150
public class ByteReader
{
	// Token: 0x06000427 RID: 1063 RVA: 0x00007B5A File Offset: 0x00005D5A
	public ByteReader(byte[] bytes)
	{
		this.mBuffer = bytes;
	}

	// Token: 0x06000428 RID: 1064 RVA: 0x00007B69 File Offset: 0x00005D69
	public ByteReader(TextAsset asset)
	{
		this.mBuffer = asset.bytes;
	}

	// Token: 0x0600042A RID: 1066 RVA: 0x00031F40 File Offset: 0x00030140
	public static ByteReader Open(string path)
	{
		FileStream fileStream = File.OpenRead(path);
		if (fileStream != null)
		{
			fileStream.Seek(0L, SeekOrigin.End);
			byte[] array = new byte[fileStream.Position];
			fileStream.Seek(0L, SeekOrigin.Begin);
			fileStream.Read(array, 0, array.Length);
			fileStream.Close();
			return new ByteReader(array);
		}
		return null;
	}

	// Token: 0x1700005F RID: 95
	// (get) Token: 0x0600042B RID: 1067 RVA: 0x00007B89 File Offset: 0x00005D89
	public bool canRead
	{
		get
		{
			return this.mBuffer != null && this.mOffset < this.mBuffer.Length;
		}
	}

	// Token: 0x0600042C RID: 1068 RVA: 0x00007BA9 File Offset: 0x00005DA9
	private static string ReadLine(byte[] buffer, int start, int count)
	{
		return Encoding.UTF8.GetString(buffer, start, count);
	}

	// Token: 0x0600042D RID: 1069 RVA: 0x00007BB8 File Offset: 0x00005DB8
	public string ReadLine()
	{
		return this.ReadLine(true);
	}

	// Token: 0x0600042E RID: 1070 RVA: 0x00031F98 File Offset: 0x00030198
	public string ReadLine(bool skipEmptyLines)
	{
		int num = this.mBuffer.Length;
		if (skipEmptyLines)
		{
			while (this.mOffset < num && this.mBuffer[this.mOffset] < 32)
			{
				this.mOffset++;
			}
		}
		int i = this.mOffset;
		if (i < num)
		{
			while (i < num)
			{
				int num2 = (int)this.mBuffer[i++];
				if (num2 == 10 || num2 == 13)
				{
					IL_87:
					string result = ByteReader.ReadLine(this.mBuffer, this.mOffset, i - this.mOffset - 1);
					this.mOffset = i;
					return result;
				}
			}
			i++;
		}
		this.mOffset = num;
		return null;
	}

	// Token: 0x0600042F RID: 1071 RVA: 0x00032060 File Offset: 0x00030260
	public Dictionary<string, string> ReadDictionary()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		char[] separator = new char[]
		{
			'='
		};
		while (this.canRead)
		{
			string text = this.ReadLine();
			if (text == null)
			{
				break;
			}
			if (!text.StartsWith("//"))
			{
				string[] array = text.Split(separator, 2, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length == 2)
				{
					string key = array[0].Trim();
					string value = array[1].Trim().Replace("\\n", "\n");
					dictionary[key] = value;
				}
			}
		}
		return dictionary;
	}

	// Token: 0x06000430 RID: 1072 RVA: 0x000320F8 File Offset: 0x000302F8
	public BetterList<string> ReadCSV()
	{
		ByteReader.mTemp.Clear();
		string text = string.Empty;
		bool flag = false;
		int num = 0;
		while (this.canRead)
		{
			if (flag)
			{
				string text2 = this.ReadLine(false);
				if (text2 == null)
				{
					return null;
				}
				text2 = text2.Replace("\\n", "\n");
				text = text + "\n" + text2;
			}
			else
			{
				text = this.ReadLine(true);
				if (text == null)
				{
					return null;
				}
				text = text.Replace("\\n", "\n");
				num = 0;
			}
			int i = num;
			int length = text.Length;
			while (i < length)
			{
				char c = text[i];
				if (c == ',')
				{
					if (!flag)
					{
						ByteReader.mTemp.Add(text.Substring(num, i - num));
						num = i + 1;
					}
				}
				else if (c == '"')
				{
					if (flag)
					{
						if (i + 1 >= length)
						{
							ByteReader.mTemp.Add(text.Substring(num, i - num).Replace("\"\"", "\""));
							return ByteReader.mTemp;
						}
						if (text[i + 1] != '"')
						{
							ByteReader.mTemp.Add(text.Substring(num, i - num).Replace("\"\"", "\""));
							flag = false;
							if (text[i + 1] == ',')
							{
								i++;
								num = i + 1;
							}
						}
						else
						{
							i++;
						}
					}
					else
					{
						num = i + 1;
						flag = true;
					}
				}
				i++;
			}
			if (num < text.Length)
			{
				if (flag)
				{
					continue;
				}
				ByteReader.mTemp.Add(text.Substring(num, text.Length - num));
			}
			else
			{
				ByteReader.mTemp.Add(string.Empty);
			}
			return ByteReader.mTemp;
		}
		return null;
	}

	// Token: 0x04000345 RID: 837
	private byte[] mBuffer;

	// Token: 0x04000346 RID: 838
	private int mOffset;

	// Token: 0x04000347 RID: 839
	private static BetterList<string> mTemp = new BetterList<string>();
}
