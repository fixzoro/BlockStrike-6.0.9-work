using System;

// Token: 0x02000129 RID: 297
[Serializable]
public class UISpriteData
{
	// Token: 0x170001CF RID: 463
	// (get) Token: 0x06000A79 RID: 2681 RVA: 0x0000B901 File Offset: 0x00009B01
	public bool hasBorder
	{
		get
		{
			return (this.borderLeft | this.borderRight | this.borderTop | this.borderBottom) != 0;
		}
	}

	// Token: 0x170001D0 RID: 464
	// (get) Token: 0x06000A7A RID: 2682 RVA: 0x0000B924 File Offset: 0x00009B24
	public bool hasPadding
	{
		get
		{
			return (this.paddingLeft | this.paddingRight | this.paddingTop | this.paddingBottom) != 0;
		}
	}

	// Token: 0x06000A7B RID: 2683 RVA: 0x0000B947 File Offset: 0x00009B47
	public void SetRect(int x, int y, int width, int height)
	{
		this.x = x;
		this.y = y;
		this.width = width;
		this.height = height;
	}

	// Token: 0x06000A7C RID: 2684 RVA: 0x0000B966 File Offset: 0x00009B66
	public void SetPadding(int left, int bottom, int right, int top)
	{
		this.paddingLeft = left;
		this.paddingBottom = bottom;
		this.paddingRight = right;
		this.paddingTop = top;
	}

	// Token: 0x06000A7D RID: 2685 RVA: 0x0000B985 File Offset: 0x00009B85
	public void SetBorder(int left, int bottom, int right, int top)
	{
		this.borderLeft = left;
		this.borderBottom = bottom;
		this.borderRight = right;
		this.borderTop = top;
	}

	// Token: 0x06000A7E RID: 2686 RVA: 0x00059964 File Offset: 0x00057B64
	public void CopyFrom(UISpriteData sd)
	{
		this.name = sd.name;
		this.x = sd.x;
		this.y = sd.y;
		this.width = sd.width;
		this.height = sd.height;
		this.borderLeft = sd.borderLeft;
		this.borderRight = sd.borderRight;
		this.borderTop = sd.borderTop;
		this.borderBottom = sd.borderBottom;
		this.paddingLeft = sd.paddingLeft;
		this.paddingRight = sd.paddingRight;
		this.paddingTop = sd.paddingTop;
		this.paddingBottom = sd.paddingBottom;
	}

	// Token: 0x06000A7F RID: 2687 RVA: 0x0000B9A4 File Offset: 0x00009BA4
	public void CopyBorderFrom(UISpriteData sd)
	{
		this.borderLeft = sd.borderLeft;
		this.borderRight = sd.borderRight;
		this.borderTop = sd.borderTop;
		this.borderBottom = sd.borderBottom;
	}

	// Token: 0x0400071B RID: 1819
	public string name = "Sprite";

	// Token: 0x0400071C RID: 1820
	public int x;

	// Token: 0x0400071D RID: 1821
	public int y;

	// Token: 0x0400071E RID: 1822
	public int width;

	// Token: 0x0400071F RID: 1823
	public int height;

	// Token: 0x04000720 RID: 1824
	public int borderLeft;

	// Token: 0x04000721 RID: 1825
	public int borderRight;

	// Token: 0x04000722 RID: 1826
	public int borderTop;

	// Token: 0x04000723 RID: 1827
	public int borderBottom;

	// Token: 0x04000724 RID: 1828
	public int paddingLeft;

	// Token: 0x04000725 RID: 1829
	public int paddingRight;

	// Token: 0x04000726 RID: 1830
	public int paddingTop;

	// Token: 0x04000727 RID: 1831
	public int paddingBottom;
}
