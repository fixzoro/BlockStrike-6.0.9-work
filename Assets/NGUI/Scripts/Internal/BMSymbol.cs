using System;
using UnityEngine;

// Token: 0x02000092 RID: 146
[Serializable]
public class BMSymbol
{
	// Token: 0x17000055 RID: 85
	// (get) Token: 0x06000403 RID: 1027 RVA: 0x00007A9B File Offset: 0x00005C9B
	public int length
	{
		get
		{
			if (this.mLength == 0)
			{
				this.mLength = this.sequence.Length;
			}
			return this.mLength;
		}
	}

	// Token: 0x17000056 RID: 86
	// (get) Token: 0x06000404 RID: 1028 RVA: 0x00007ABF File Offset: 0x00005CBF
	public int offsetX
	{
		get
		{
			return this.mOffsetX;
		}
	}

	// Token: 0x17000057 RID: 87
	// (get) Token: 0x06000405 RID: 1029 RVA: 0x00007AC7 File Offset: 0x00005CC7
	public int offsetY
	{
		get
		{
			return this.mOffsetY;
		}
	}

	// Token: 0x17000058 RID: 88
	// (get) Token: 0x06000406 RID: 1030 RVA: 0x00007ACF File Offset: 0x00005CCF
	public int width
	{
		get
		{
			return this.mWidth;
		}
	}

	// Token: 0x17000059 RID: 89
	// (get) Token: 0x06000407 RID: 1031 RVA: 0x00007AD7 File Offset: 0x00005CD7
	public int height
	{
		get
		{
			return this.mHeight;
		}
	}

	// Token: 0x1700005A RID: 90
	// (get) Token: 0x06000408 RID: 1032 RVA: 0x00007ADF File Offset: 0x00005CDF
	public int advance
	{
		get
		{
			return this.mAdvance;
		}
	}

	// Token: 0x1700005B RID: 91
	// (get) Token: 0x06000409 RID: 1033 RVA: 0x00007AE7 File Offset: 0x00005CE7
	public Rect uvRect
	{
		get
		{
			return this.mUV;
		}
	}

	// Token: 0x0600040A RID: 1034 RVA: 0x00007AEF File Offset: 0x00005CEF
	public void MarkAsChanged()
	{
		this.mIsValid = false;
	}

	// Token: 0x0600040B RID: 1035 RVA: 0x00031844 File Offset: 0x0002FA44
	public bool Validate(INGUIAtlas atlas)
	{
		if (atlas == null)
		{
			return false;
		}
		if (!this.mIsValid)
		{
			if (string.IsNullOrEmpty(this.spriteName))
			{
				return false;
			}
			this.mSprite = atlas.GetSprite(this.spriteName);
			Texture texture = atlas.texture;
			if (this.mSprite != null)
			{
				if (texture == null)
				{
					this.mSprite = null;
				}
				else
				{
					this.mUV = new Rect((float)this.mSprite.x, (float)this.mSprite.y, (float)this.mSprite.width, (float)this.mSprite.height);
					this.mUV = NGUIMath.ConvertToTexCoords(this.mUV, texture.width, texture.height);
					this.mOffsetX = this.mSprite.paddingLeft;
					this.mOffsetY = this.mSprite.paddingTop;
					this.mWidth = this.mSprite.width;
					this.mHeight = this.mSprite.height;
					this.mAdvance = this.mSprite.width + (this.mSprite.paddingLeft + this.mSprite.paddingRight);
					this.mIsValid = true;
				}
			}
		}
		return this.mSprite != null;
	}

	// Token: 0x04000334 RID: 820
	public string sequence;

	// Token: 0x04000335 RID: 821
	public string spriteName;

	// Token: 0x04000336 RID: 822
	private UISpriteData mSprite;

	// Token: 0x04000337 RID: 823
	private bool mIsValid;

	// Token: 0x04000338 RID: 824
	private int mLength;

	// Token: 0x04000339 RID: 825
	private int mOffsetX;

	// Token: 0x0400033A RID: 826
	private int mOffsetY;

	// Token: 0x0400033B RID: 827
	private int mWidth;

	// Token: 0x0400033C RID: 828
	private int mHeight;

	// Token: 0x0400033D RID: 829
	private int mAdvance;

	// Token: 0x0400033E RID: 830
	private Rect mUV;
}
