using System;
using System.Collections.Generic;

// Token: 0x02000091 RID: 145
[Serializable]
public class BMGlyph
{
	// Token: 0x060003FF RID: 1023 RVA: 0x00031690 File Offset: 0x0002F890
	public int GetKerning(int previousChar)
	{
		if (this.kerning != null && previousChar != 0)
		{
			int i = 0;
			int count = this.kerning.Count;
			while (i < count)
			{
				if (this.kerning[i] == previousChar)
				{
					return this.kerning[i + 1];
				}
				i += 2;
			}
		}
		return 0;
	}

	// Token: 0x06000400 RID: 1024 RVA: 0x000316F0 File Offset: 0x0002F8F0
	public void SetKerning(int previousChar, int amount)
	{
		if (this.kerning == null)
		{
			this.kerning = new List<int>();
		}
		for (int i = 0; i < this.kerning.Count; i += 2)
		{
			if (this.kerning[i] == previousChar)
			{
				this.kerning[i + 1] = amount;
				return;
			}
		}
		this.kerning.Add(previousChar);
		this.kerning.Add(amount);
	}

	// Token: 0x06000401 RID: 1025 RVA: 0x0003176C File Offset: 0x0002F96C
	public void Trim(int xMin, int yMin, int xMax, int yMax)
	{
		int num = this.x + this.width;
		int num2 = this.y + this.height;
		if (this.x < xMin)
		{
			int num3 = xMin - this.x;
			this.x += num3;
			this.width -= num3;
			this.offsetX += num3;
		}
		if (this.y < yMin)
		{
			int num4 = yMin - this.y;
			this.y += num4;
			this.height -= num4;
			this.offsetY += num4;
		}
		if (num > xMax)
		{
			this.width -= num - xMax;
		}
		if (num2 > yMax)
		{
			this.height -= num2 - yMax;
		}
	}

	// Token: 0x0400032A RID: 810
	public int index;

	// Token: 0x0400032B RID: 811
	public int x;

	// Token: 0x0400032C RID: 812
	public int y;

	// Token: 0x0400032D RID: 813
	public int width;

	// Token: 0x0400032E RID: 814
	public int height;

	// Token: 0x0400032F RID: 815
	public int offsetX;

	// Token: 0x04000330 RID: 816
	public int offsetY;

	// Token: 0x04000331 RID: 817
	public int advance;

	// Token: 0x04000332 RID: 818
	public int channel;

	// Token: 0x04000333 RID: 819
	public List<int> kerning;
}
