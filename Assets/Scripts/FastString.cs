using System;
using System.Collections.Generic;

public class FastString
{
    private string m_stringGenerated = string.Empty;

    private bool m_isStringGenerated;

    private char[] m_buffer;

    private int m_bufferPos;

    private int m_charsCapacity;

    private List<char> m_replacement;

    private object m_valueControl;

    private int m_valueControlInt = int.MinValue;

    public FastString(int initialCapacity = 32)
	{
		this.m_charsCapacity = initialCapacity;
		this.m_buffer = new char[initialCapacity];
	}

	public bool IsEmpty()
	{
		return (!this.m_isStringGenerated) ? (this.m_bufferPos == 0) : (this.m_stringGenerated == null);
	}

	public override string ToString()
	{
		if (!this.m_isStringGenerated)
		{
			this.m_stringGenerated = new string(this.m_buffer, 0, this.m_bufferPos);
			this.m_isStringGenerated = true;
		}
		return this.m_stringGenerated;
	}

	public bool IsModified(int newControlValue)
	{
		bool flag = newControlValue != this.m_valueControlInt;
		if (flag)
		{
			this.m_valueControlInt = newControlValue;
		}
		return flag;
	}

	public bool IsModified(object newControlValue)
	{
		bool flag = !newControlValue.Equals(this.m_valueControl);
		if (flag)
		{
			this.m_valueControl = newControlValue;
		}
		return flag;
	}

	public void Set(string str)
	{
		this.Clear();
		this.Append(str);
		this.m_stringGenerated = str;
		this.m_isStringGenerated = true;
	}

	public void Set(object str)
	{
		this.Set(str.ToString());
	}

	public void Set<T1, T2>(T1 str1, T2 str2)
	{
		this.Clear();
		this.Append(str1);
		this.Append(str2);
	}

	public void Set<T1, T2, T3>(T1 str1, T2 str2, T3 str3)
	{
		this.Clear();
		this.Append(str1);
		this.Append(str2);
		this.Append(str3);
	}

	public void Set<T1, T2, T3, T4>(T1 str1, T2 str2, T3 str3, T4 str4)
	{
		this.Clear();
		this.Append(str1);
		this.Append(str2);
		this.Append(str3);
		this.Append(str4);
	}

	public void Set(params object[] str)
	{
		this.Clear();
		for (int i = 0; i < str.Length; i++)
		{
			this.Append(str[i]);
		}
	}

	public FastString Clear()
	{
		this.m_bufferPos = 0;
		this.m_isStringGenerated = false;
		return this;
	}

	public FastString Append(string value)
	{
		this.ReallocateIFN(value.Length);
		int length = value.Length;
		for (int i = 0; i < length; i++)
		{
			this.m_buffer[this.m_bufferPos + i] = value[i];
		}
		this.m_bufferPos += length;
		this.m_isStringGenerated = false;
		return this;
	}

	public FastString Append(object value)
	{
		this.Append(value.ToString());
		return this;
	}

	public FastString Append(int value)
	{
		this.ReallocateIFN(16);
		if (value < 0)
		{
			value = -value;
			this.m_buffer[this.m_bufferPos++] = '-';
		}
		int num = 0;
		do
		{
			this.m_buffer[this.m_bufferPos++] = (char)(48 + value % 10);
			value /= 10;
			num++;
		}
		while (value != 0);
		for (int i = num / 2 - 1; i >= 0; i--)
		{
			char c = this.m_buffer[this.m_bufferPos - i - 1];
			this.m_buffer[this.m_bufferPos - i - 1] = this.m_buffer[this.m_bufferPos - num + i];
			this.m_buffer[this.m_bufferPos - num + i] = c;
		}
		this.m_isStringGenerated = false;
		return this;
	}

	public FastString Append(float valueF)
	{
		double num = (double)valueF;
		this.m_isStringGenerated = false;
		this.ReallocateIFN(32);
		if (num == 0.0)
		{
			this.m_buffer[this.m_bufferPos++] = '0';
			return this;
		}
		if (num < 0.0)
		{
			num = -num;
			this.m_buffer[this.m_bufferPos++] = '-';
		}
		int num2 = 0;
		while (num < 1000000.0)
		{
			num *= 10.0;
			num2++;
		}
		long num3 = (long)Math.Round(num);
		int num4 = 0;
		bool flag = true;
		while (num3 != 0L || num2 >= 0)
		{
			if (num3 % 10L != 0L || num2 <= 0)
			{
				flag = false;
			}
			if (!flag)
			{
				this.m_buffer[this.m_bufferPos + num4++] = (char)(48L + num3 % 10L);
			}
			if (--num2 == 0 && !flag)
			{
				this.m_buffer[this.m_bufferPos + num4++] = '.';
			}
			num3 /= 10L;
		}
		this.m_bufferPos += num4;
		for (int i = num4 / 2 - 1; i >= 0; i--)
		{
			char c = this.m_buffer[this.m_bufferPos - i - 1];
			this.m_buffer[this.m_bufferPos - i - 1] = this.m_buffer[this.m_bufferPos - num4 + i];
			this.m_buffer[this.m_bufferPos - num4 + i] = c;
		}
		return this;
	}

	public FastString Replace(string oldStr, string newStr)
	{
		if (this.m_bufferPos == 0)
		{
			return this;
		}
		if (this.m_replacement == null)
		{
			this.m_replacement = new List<char>();
		}
		for (int i = 0; i < this.m_bufferPos; i++)
		{
			bool flag = false;
			if (this.m_buffer[i] == oldStr[0])
			{
				int num = 1;
				while (num < oldStr.Length && this.m_buffer[i + num] == oldStr[num])
				{
					num++;
				}
				flag = (num >= oldStr.Length);
			}
			if (flag)
			{
				i += oldStr.Length - 1;
				if (newStr != null)
				{
					for (int j = 0; j < newStr.Length; j++)
					{
						this.m_replacement.Add(newStr[j]);
					}
				}
			}
			else
			{
				this.m_replacement.Add(this.m_buffer[i]);
			}
		}
		this.ReallocateIFN(this.m_replacement.Count - this.m_bufferPos);
		for (int k = 0; k < this.m_replacement.Count; k++)
		{
			this.m_buffer[k] = this.m_replacement[k];
		}
		this.m_bufferPos = this.m_replacement.Count;
		this.m_replacement.Clear();
		this.m_isStringGenerated = false;
		return this;
	}

	private void ReallocateIFN(int nbCharsToAdd)
	{
		if (this.m_bufferPos + nbCharsToAdd > this.m_charsCapacity)
		{
			this.m_charsCapacity = Math.Max(this.m_charsCapacity + nbCharsToAdd, this.m_charsCapacity * 2);
			char[] array = new char[this.m_charsCapacity];
			this.m_buffer.CopyTo(array, 0);
			this.m_buffer = array;
		}
	}
}
