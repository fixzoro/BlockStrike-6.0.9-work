using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x02000093 RID: 147
public class BetterList<T>
{
	// Token: 0x0600040D RID: 1037 RVA: 0x0003198C File Offset: 0x0002FB8C
	public IEnumerator<T> GetEnumerator()
	{
		if (this.buffer != null)
		{
			for (int i = 0; i < this.size; i++)
			{
				yield return this.buffer[i];
			}
		}
		yield break;
	}

	// Token: 0x1700005C RID: 92
	[DebuggerHidden]
	public T this[int i]
	{
		get
		{
			return this.buffer[i];
		}
		set
		{
			this.buffer[i] = value;
		}
	}

	// Token: 0x06000410 RID: 1040 RVA: 0x000319A8 File Offset: 0x0002FBA8
	private void AllocateMore()
	{
		T[] array = (this.buffer == null) ? new T[32] : new T[Mathf.Max(this.buffer.Length << 1, 32)];
		if (this.buffer != null && this.size > 0)
		{
			this.buffer.CopyTo(array, 0);
		}
		this.buffer = array;
	}

	// Token: 0x06000411 RID: 1041 RVA: 0x00031A10 File Offset: 0x0002FC10
	private void Trim()
	{
		if (this.size > 0)
		{
			if (this.size < this.buffer.Length)
			{
				T[] array = new T[this.size];
				for (int i = 0; i < this.size; i++)
				{
					array[i] = this.buffer[i];
				}
				this.buffer = array;
			}
		}
		else
		{
			this.buffer = null;
		}
	}

	// Token: 0x06000412 RID: 1042 RVA: 0x00007B15 File Offset: 0x00005D15
	public void Clear()
	{
		this.size = 0;
	}

	// Token: 0x06000413 RID: 1043 RVA: 0x00007B1E File Offset: 0x00005D1E
	public void Release()
	{
		this.size = 0;
		this.buffer = null;
	}

	// Token: 0x06000414 RID: 1044 RVA: 0x00031A88 File Offset: 0x0002FC88
	public void Add(T item)
	{
		if (this.buffer == null || this.size == this.buffer.Length)
		{
			this.AllocateMore();
		}
		this.buffer[this.size++] = item;
	}

	// Token: 0x06000415 RID: 1045 RVA: 0x00031AD8 File Offset: 0x0002FCD8
	public void Insert(int index, T item)
	{
		if (this.buffer == null || this.size == this.buffer.Length)
		{
			this.AllocateMore();
		}
		if (index > -1 && index < this.size)
		{
			for (int i = this.size; i > index; i--)
			{
				this.buffer[i] = this.buffer[i - 1];
			}
			this.buffer[index] = item;
			this.size++;
		}
		else
		{
			this.Add(item);
		}
	}

	// Token: 0x06000416 RID: 1046 RVA: 0x00031B74 File Offset: 0x0002FD74
	public bool Contains(T item)
	{
		if (this.buffer == null)
		{
			return false;
		}
		for (int i = 0; i < this.size; i++)
		{
			if (this.buffer[i].Equals(item))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000417 RID: 1047 RVA: 0x00031BCC File Offset: 0x0002FDCC
	public int IndexOf(T item)
	{
		if (this.buffer == null)
		{
			return -1;
		}
		for (int i = 0; i < this.size; i++)
		{
			if (this.buffer[i].Equals(item))
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000418 RID: 1048 RVA: 0x00031C24 File Offset: 0x0002FE24
	public bool Remove(T item)
	{
		if (this.buffer != null)
		{
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			for (int i = 0; i < this.size; i++)
			{
				if (@default.Equals(this.buffer[i], item))
				{
					this.size--;
					this.buffer[i] = default(T);
					for (int j = i; j < this.size; j++)
					{
						this.buffer[j] = this.buffer[j + 1];
					}
					this.buffer[this.size] = default(T);
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000419 RID: 1049 RVA: 0x00031CE4 File Offset: 0x0002FEE4
	public void RemoveAt(int index)
	{
		if (this.buffer != null && index > -1 && index < this.size)
		{
			this.size--;
			this.buffer[index] = default(T);
			for (int i = index; i < this.size; i++)
			{
				this.buffer[i] = this.buffer[i + 1];
			}
			this.buffer[this.size] = default(T);
		}
	}

	// Token: 0x0600041A RID: 1050 RVA: 0x00031D80 File Offset: 0x0002FF80
	public T Pop()
	{
		if (this.buffer != null && this.size != 0)
		{
			T result = this.buffer[--this.size];
			this.buffer[this.size] = default(T);
			return result;
		}
		return default(T);
	}

	// Token: 0x0600041B RID: 1051 RVA: 0x00007B2E File Offset: 0x00005D2E
	public T[] ToArray()
	{
		this.Trim();
		return this.buffer;
	}

	// Token: 0x0600041C RID: 1052 RVA: 0x00031DE8 File Offset: 0x0002FFE8
	[DebuggerHidden]
	[DebuggerStepThrough]
	public void Sort(BetterList<T>.CompareFunc comparer)
	{
		int num = 0;
		int num2 = this.size - 1;
		bool flag = true;
		while (flag)
		{
			flag = false;
			for (int i = num; i < num2; i++)
			{
				if (comparer(this.buffer[i], this.buffer[i + 1]) > 0)
				{
					T t = this.buffer[i];
					this.buffer[i] = this.buffer[i + 1];
					this.buffer[i + 1] = t;
					flag = true;
				}
				else if (!flag)
				{
					num = ((i != 0) ? (i - 1) : 0);
				}
			}
		}
	}

	// Token: 0x0400033F RID: 831
	public T[] buffer;

	// Token: 0x04000340 RID: 832
	public int size;

	// Token: 0x02000094 RID: 148
	// (Invoke) Token: 0x0600041E RID: 1054
	public delegate int CompareFunc(T left, T right);
}
