using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000E7 RID: 231
[AddComponentMenu("NGUI/UI/Unity2D Sprite")]
[ExecuteInEditMode]
public class UI2DSprite : UIBasicSprite
{
	// Token: 0x17000112 RID: 274
	// (get) Token: 0x060007D6 RID: 2006 RVA: 0x0000A077 File Offset: 0x00008277
	// (set) Token: 0x060007D7 RID: 2007 RVA: 0x0000A07F File Offset: 0x0000827F
	public Sprite sprite2D
	{
		get
		{
			return this.mSprite;
		}
		set
		{
			if (this.mSprite != value)
			{
				base.RemoveFromPanel();
				this.mSprite = value;
				this.nextSprite = null;
				base.CreatePanel();
			}
		}
	}

	// Token: 0x17000113 RID: 275
	// (get) Token: 0x060007D8 RID: 2008 RVA: 0x000092F4 File Offset: 0x000074F4
	// (set) Token: 0x060007D9 RID: 2009 RVA: 0x0000A0AD File Offset: 0x000082AD
	public override Material material
	{
		get
		{
			return this.mMat;
		}
		set
		{
			if (this.mMat != value)
			{
				base.RemoveFromPanel();
				this.mMat = value;
				this.mPMA = -1;
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x17000114 RID: 276
	// (get) Token: 0x060007DA RID: 2010 RVA: 0x00049AD8 File Offset: 0x00047CD8
	// (set) Token: 0x060007DB RID: 2011 RVA: 0x0000A0DA File Offset: 0x000082DA
	public override Shader shader
	{
		get
		{
			if (this.mMat != null)
			{
				return this.mMat.shader;
			}
			if (this.mShader == null)
			{
				this.mShader = Shader.Find("Unlit/Transparent Colored");
			}
			return this.mShader;
		}
		set
		{
			if (this.mShader != value)
			{
				base.RemoveFromPanel();
				this.mShader = value;
				if (this.mMat == null)
				{
					this.mPMA = -1;
					this.MarkAsChanged();
				}
			}
		}
	}

	// Token: 0x17000115 RID: 277
	// (get) Token: 0x060007DC RID: 2012 RVA: 0x0000A118 File Offset: 0x00008318
	public override Texture mainTexture
	{
		get
		{
			if (this.mSprite != null)
			{
				return this.mSprite.texture;
			}
			if (this.mMat != null)
			{
				return this.mMat.mainTexture;
			}
			return null;
		}
	}

	// Token: 0x17000116 RID: 278
	// (get) Token: 0x060007DD RID: 2013 RVA: 0x0000A155 File Offset: 0x00008355
	// (set) Token: 0x060007DE RID: 2014 RVA: 0x0000A15D File Offset: 0x0000835D
	public bool fixedAspect
	{
		get
		{
			return this.mFixedAspect;
		}
		set
		{
			if (this.mFixedAspect != value)
			{
				this.mFixedAspect = value;
				this.mDrawRegion = new Vector4(0f, 0f, 1f, 1f);
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x17000117 RID: 279
	// (get) Token: 0x060007DF RID: 2015 RVA: 0x00049B2C File Offset: 0x00047D2C
	public override bool premultipliedAlpha
	{
		get
		{
			if (this.mPMA == -1)
			{
				Shader shader = this.shader;
				this.mPMA = ((!(shader != null) || !shader.name.Contains("Premultiplied")) ? 0 : 1);
			}
			return this.mPMA == 1;
		}
	}

	// Token: 0x17000118 RID: 280
	// (get) Token: 0x060007E0 RID: 2016 RVA: 0x0000A197 File Offset: 0x00008397
	public override float pixelSize
	{
		get
		{
			return this.mPixelSize;
		}
	}

	// Token: 0x17000119 RID: 281
	// (get) Token: 0x060007E1 RID: 2017 RVA: 0x00049B84 File Offset: 0x00047D84
	public override Vector4 drawingDimensions
	{
		get
		{
			Vector2 pivotOffset = base.pivotOffset;
			float num = -pivotOffset.x * (float)this.mWidth;
			float num2 = -pivotOffset.y * (float)this.mHeight;
			float num3 = num + (float)this.mWidth;
			float num4 = num2 + (float)this.mHeight;
			if (this.mSprite != null && this.mType != UIBasicSprite.Type.Tiled)
			{
				int num5 = Mathf.RoundToInt(this.mSprite.rect.width);
				int num6 = Mathf.RoundToInt(this.mSprite.rect.height);
				int num7 = Mathf.RoundToInt(this.mSprite.textureRectOffset.x);
				int num8 = Mathf.RoundToInt(this.mSprite.textureRectOffset.y);
				int num9 = Mathf.RoundToInt(this.mSprite.rect.width - this.mSprite.textureRect.width - this.mSprite.textureRectOffset.x);
				int num10 = Mathf.RoundToInt(this.mSprite.rect.height - this.mSprite.textureRect.height - this.mSprite.textureRectOffset.y);
				float num11 = 1f;
				float num12 = 1f;
				if (num5 > 0 && num6 > 0 && (this.mType == UIBasicSprite.Type.Simple || this.mType == UIBasicSprite.Type.Filled))
				{
					if ((num5 & 1) != 0)
					{
						num9++;
					}
					if ((num6 & 1) != 0)
					{
						num10++;
					}
					num11 = 1f / (float)num5 * (float)this.mWidth;
					num12 = 1f / (float)num6 * (float)this.mHeight;
				}
				if (this.mFlip == UIBasicSprite.Flip.Horizontally || this.mFlip == UIBasicSprite.Flip.Both)
				{
					num += (float)num9 * num11;
					num3 -= (float)num7 * num11;
				}
				else
				{
					num += (float)num7 * num11;
					num3 -= (float)num9 * num11;
				}
				if (this.mFlip == UIBasicSprite.Flip.Vertically || this.mFlip == UIBasicSprite.Flip.Both)
				{
					num2 += (float)num10 * num12;
					num4 -= (float)num8 * num12;
				}
				else
				{
					num2 += (float)num8 * num12;
					num4 -= (float)num10 * num12;
				}
			}
			float num13;
			float num14;
			if (this.mFixedAspect)
			{
				num13 = 0f;
				num14 = 0f;
			}
			else
			{
				Vector4 vector = this.border * this.pixelSize;
				num13 = vector.x + vector.z;
				num14 = vector.y + vector.w;
			}
			float x = Mathf.Lerp(num, num3 - num13, this.mDrawRegion.x);
			float y = Mathf.Lerp(num2, num4 - num14, this.mDrawRegion.y);
			float z = Mathf.Lerp(num + num13, num3, this.mDrawRegion.z);
			float w = Mathf.Lerp(num2 + num14, num4, this.mDrawRegion.w);
			return new Vector4(x, y, z, w);
		}
	}

	// Token: 0x1700011A RID: 282
	// (get) Token: 0x060007E2 RID: 2018 RVA: 0x0000A19F File Offset: 0x0000839F
	// (set) Token: 0x060007E3 RID: 2019 RVA: 0x0000A1A7 File Offset: 0x000083A7
	public override Vector4 border
	{
		get
		{
			return this.mBorder;
		}
		set
		{
			if (this.mBorder != value)
			{
				this.mBorder = value;
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x060007E4 RID: 2020 RVA: 0x00049EA0 File Offset: 0x000480A0
	protected override void OnUpdate()
	{
		if (this.nextSprite != null)
		{
			if (this.nextSprite != this.mSprite)
			{
				this.sprite2D = this.nextSprite;
			}
			this.nextSprite = null;
		}
		base.OnUpdate();
		if (this.mFixedAspect)
		{
			Texture mainTexture = this.mainTexture;
			if (mainTexture != null)
			{
				int num = Mathf.RoundToInt(this.mSprite.rect.width);
				int num2 = Mathf.RoundToInt(this.mSprite.rect.height);
				int num3 = Mathf.RoundToInt(this.mSprite.textureRectOffset.x);
				int num4 = Mathf.RoundToInt(this.mSprite.textureRectOffset.y);
				int num5 = Mathf.RoundToInt(this.mSprite.rect.width - this.mSprite.textureRect.width - this.mSprite.textureRectOffset.x);
				int num6 = Mathf.RoundToInt(this.mSprite.rect.height - this.mSprite.textureRect.height - this.mSprite.textureRectOffset.y);
				num += num3 + num5;
				num2 += num6 + num4;
				float num7 = (float)this.mWidth;
				float num8 = (float)this.mHeight;
				float num9 = num7 / num8;
				float num10 = (float)num / (float)num2;
				if (num10 < num9)
				{
					float num11 = (num7 - num8 * num10) / num7 * 0.5f;
					base.drawRegion = new Vector4(num11, 0f, 1f - num11, 1f);
				}
				else
				{
					float num12 = (num8 - num7 / num10) / num8 * 0.5f;
					base.drawRegion = new Vector4(0f, num12, 1f, 1f - num12);
				}
			}
		}
	}

	// Token: 0x060007E5 RID: 2021 RVA: 0x0004A0A0 File Offset: 0x000482A0
	public override void MakePixelPerfect()
	{
		base.MakePixelPerfect();
		if (this.mType == UIBasicSprite.Type.Tiled)
		{
			return;
		}
		Texture mainTexture = this.mainTexture;
		if (mainTexture == null)
		{
			return;
		}
		if ((this.mType == UIBasicSprite.Type.Simple || this.mType == UIBasicSprite.Type.Filled || !base.hasBorder) && mainTexture != null)
		{
			Rect rect = this.mSprite.rect;
			int num = Mathf.RoundToInt(this.pixelSize * rect.width);
			int num2 = Mathf.RoundToInt(this.pixelSize * rect.height);
			if ((num & 1) == 1)
			{
				num++;
			}
			if ((num2 & 1) == 1)
			{
				num2++;
			}
			base.width = num;
			base.height = num2;
		}
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x0004A160 File Offset: 0x00048360
	public override void OnFill(List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
	{
		Texture mainTexture = this.mainTexture;
		if (mainTexture == null)
		{
			return;
		}
		Rect rect = (!(this.mSprite != null)) ? new Rect(0f, 0f, (float)mainTexture.width, (float)mainTexture.height) : this.mSprite.textureRect;
		Rect inner = rect;
		Vector4 border = this.border;
		inner.xMin += border.x;
		inner.yMin += border.y;
		inner.xMax -= border.z;
		inner.yMax -= border.w;
		float num = 1f / (float)mainTexture.width;
		float num2 = 1f / (float)mainTexture.height;
		rect.xMin *= num;
		rect.xMax *= num;
		rect.yMin *= num2;
		rect.yMax *= num2;
		inner.xMin *= num;
		inner.xMax *= num;
		inner.yMin *= num2;
		inner.yMax *= num2;
		int count = verts.Count;
		base.Fill(verts, uvs, cols, rect, inner);
		if (this.onPostFill != null)
		{
			this.onPostFill(this, count, verts, uvs, cols);
		}
	}

	// Token: 0x04000531 RID: 1329
	[HideInInspector]
	[SerializeField]
	private Sprite mSprite;

	// Token: 0x04000532 RID: 1330
	[HideInInspector]
	[SerializeField]
	private Shader mShader;

	// Token: 0x04000533 RID: 1331
	[HideInInspector]
	[SerializeField]
	private Vector4 mBorder = Vector4.zero;

	// Token: 0x04000534 RID: 1332
	[HideInInspector]
	[SerializeField]
	private bool mFixedAspect;

	// Token: 0x04000535 RID: 1333
	[HideInInspector]
	[SerializeField]
	private float mPixelSize = 1f;

	// Token: 0x04000536 RID: 1334
	public Sprite nextSprite;

	// Token: 0x04000537 RID: 1335
	[NonSerialized]
	private int mPMA = -1;
}
