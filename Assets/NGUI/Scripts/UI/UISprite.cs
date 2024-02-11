using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000120 RID: 288
[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Sprite")]
public class UISprite : UIBasicSprite
{
	// Token: 0x170001B3 RID: 435
	// (get) Token: 0x06000A13 RID: 2579 RVA: 0x00057C00 File Offset: 0x00055E00
	// (set) Token: 0x06000A14 RID: 2580 RVA: 0x0000AAA1 File Offset: 0x00008CA1
	public override Texture mainTexture
	{
		get
		{
			Material material = null;
			INGUIAtlas inguiatlas = this.mAtlas as INGUIAtlas;
			if (inguiatlas != null)
			{
				material = inguiatlas.spriteMaterial;
			}
			return (!(material != null)) ? null : material.mainTexture;
		}
		set
		{
			base.mainTexture = value;
		}
	}

	// Token: 0x170001B4 RID: 436
	// (get) Token: 0x06000A15 RID: 2581 RVA: 0x00057C40 File Offset: 0x00055E40
	// (set) Token: 0x06000A16 RID: 2582 RVA: 0x0000AA98 File Offset: 0x00008C98
	public override Material material
	{
		get
		{
			Material material = base.material;
			if (material != null)
			{
				return material;
			}
			INGUIAtlas inguiatlas = this.mAtlas as INGUIAtlas;
			if (inguiatlas != null)
			{
				return inguiatlas.spriteMaterial;
			}
			return null;
		}
		set
		{
			base.material = value;
		}
	}

	// Token: 0x170001B5 RID: 437
	// (get) Token: 0x06000A17 RID: 2583 RVA: 0x0000B5D3 File Offset: 0x000097D3
	// (set) Token: 0x06000A18 RID: 2584 RVA: 0x00057C7C File Offset: 0x00055E7C
	public INGUIAtlas atlas
	{
		get
		{
			return this.mAtlas as INGUIAtlas;
		}
		set
		{
			if (this.mAtlas as INGUIAtlas != value)
			{
				base.RemoveFromPanel();
				this.mAtlas = (value as UnityEngine.Object);
				this.mSpriteSet = false;
				this.mSprite = null;
				if (string.IsNullOrEmpty(this.mSpriteName))
				{
					INGUIAtlas inguiatlas = this.mAtlas as INGUIAtlas;
					if (inguiatlas != null && inguiatlas.spriteList.Count > 0)
					{
						this.SetAtlasSprite(inguiatlas.spriteList[0]);
						this.mSpriteName = this.mSprite.name;
					}
				}
				if (!string.IsNullOrEmpty(this.mSpriteName))
				{
					string spriteName = this.mSpriteName;
					this.mSpriteName = string.Empty;
					this.spriteName = spriteName;
					this.MarkAsChanged();
				}
			}
		}
	}

	// Token: 0x06000A19 RID: 2585 RVA: 0x00057D40 File Offset: 0x00055F40
	public UISpriteData GetSprite(string spriteName)
	{
		INGUIAtlas atlas = this.atlas;
		if (atlas == null)
		{
			return null;
		}
		return atlas.GetSprite(spriteName);
	}

	// Token: 0x06000A1A RID: 2586 RVA: 0x0000B5E0 File Offset: 0x000097E0
	public override void MarkAsChanged()
	{
		this.mSprite = null;
		this.mSpriteSet = false;
		base.MarkAsChanged();
	}

	// Token: 0x170001B6 RID: 438
	// (get) Token: 0x06000A1B RID: 2587 RVA: 0x0000B5F6 File Offset: 0x000097F6
	// (set) Token: 0x06000A1C RID: 2588 RVA: 0x00057D64 File Offset: 0x00055F64
	public string spriteName
	{
		get
		{
			return this.mSpriteName;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				if (string.IsNullOrEmpty(this.mSpriteName))
				{
					return;
				}
				this.mSpriteName = string.Empty;
				this.mSprite = null;
				this.mChanged = true;
				this.mSpriteSet = false;
			}
			else if (this.mSpriteName != value)
			{
				this.mSpriteName = value;
				this.mSprite = null;
				this.mChanged = true;
				this.mSpriteSet = false;
			}
		}
	}

	// Token: 0x170001B7 RID: 439
	// (get) Token: 0x06000A1D RID: 2589 RVA: 0x0000B5FE File Offset: 0x000097FE
	public bool isValid
	{
		get
		{
			return this.GetAtlasSprite() != null;
		}
	}

	// Token: 0x170001B8 RID: 440
	// (get) Token: 0x06000A1E RID: 2590 RVA: 0x0000B60C File Offset: 0x0000980C
	// (set) Token: 0x06000A1F RID: 2591 RVA: 0x0000B61A File Offset: 0x0000981A
	[Obsolete("Use 'centerType' instead")]
	public bool fillCenter
	{
		get
		{
			return this.centerType != UIBasicSprite.AdvancedType.Invisible;
		}
		set
		{
			if (value != (this.centerType != UIBasicSprite.AdvancedType.Invisible))
			{
				this.centerType = ((!value) ? UIBasicSprite.AdvancedType.Invisible : UIBasicSprite.AdvancedType.Sliced);
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x170001B9 RID: 441
	// (get) Token: 0x06000A20 RID: 2592 RVA: 0x0000B647 File Offset: 0x00009847
	// (set) Token: 0x06000A21 RID: 2593 RVA: 0x0000B64F File Offset: 0x0000984F
	public bool applyGradient
	{
		get
		{
			return this.mApplyGradient;
		}
		set
		{
			if (this.mApplyGradient != value)
			{
				this.mApplyGradient = value;
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x170001BA RID: 442
	// (get) Token: 0x06000A22 RID: 2594 RVA: 0x0000B66A File Offset: 0x0000986A
	// (set) Token: 0x06000A23 RID: 2595 RVA: 0x0000B672 File Offset: 0x00009872
	public Color gradientTop
	{
		get
		{
			return this.mGradientTop;
		}
		set
		{
			if (this.mGradientTop != value)
			{
				this.mGradientTop = value;
				if (this.mApplyGradient)
				{
					this.MarkAsChanged();
				}
			}
		}
	}

	// Token: 0x170001BB RID: 443
	// (get) Token: 0x06000A24 RID: 2596 RVA: 0x0000B69D File Offset: 0x0000989D
	// (set) Token: 0x06000A25 RID: 2597 RVA: 0x0000B6A5 File Offset: 0x000098A5
	public Color gradientBottom
	{
		get
		{
			return this.mGradientBottom;
		}
		set
		{
			if (this.mGradientBottom != value)
			{
				this.mGradientBottom = value;
				if (this.mApplyGradient)
				{
					this.MarkAsChanged();
				}
			}
		}
	}

	// Token: 0x170001BC RID: 444
	// (get) Token: 0x06000A26 RID: 2598 RVA: 0x00057DE0 File Offset: 0x00055FE0
	public override Vector4 border
	{
		get
		{
			UISpriteData atlasSprite = this.GetAtlasSprite();
			if (atlasSprite == null)
			{
				return base.border;
			}
			return new Vector4((float)atlasSprite.borderLeft + this.mBorder.x, (float)atlasSprite.borderBottom + this.mBorder.y, (float)atlasSprite.borderRight + this.mBorder.z, (float)atlasSprite.borderTop + this.mBorder.w);
		}
	}

	// Token: 0x170001BD RID: 445
	// (get) Token: 0x06000A27 RID: 2599 RVA: 0x00057E54 File Offset: 0x00056054
	protected override Vector4 padding
	{
		get
		{
			UISpriteData atlasSprite = this.GetAtlasSprite();
			Vector4 result = new Vector4(0f, 0f, 0f, 0f);
			if (atlasSprite != null)
			{
				result.x = (float)atlasSprite.paddingLeft + this.mPadding.x;
				result.y = (float)atlasSprite.paddingBottom + this.mPadding.y;
				result.z = (float)atlasSprite.paddingRight + this.mPadding.z;
				result.w = (float)atlasSprite.paddingTop + this.mPadding.w;
			}
			return result;
		}
	}

	// Token: 0x170001BE RID: 446
	// (get) Token: 0x06000A28 RID: 2600 RVA: 0x00057EF4 File Offset: 0x000560F4
	public override float pixelSize
	{
		get
		{
			if (this.mAtlas == null)
			{
				return 1f;
			}
			INGUIAtlas inguiatlas = this.mAtlas as INGUIAtlas;
			if (inguiatlas != null)
			{
				return inguiatlas.pixelSize;
			}
			return 1f;
		}
	}

	// Token: 0x170001BF RID: 447
	// (get) Token: 0x06000A29 RID: 2601 RVA: 0x00057F38 File Offset: 0x00056138
	public override int minWidth
	{
		get
		{
			if (this.type == UIBasicSprite.Type.Sliced || this.type == UIBasicSprite.Type.Advanced)
			{
				float pixelSize = this.pixelSize;
				Vector4 vector = this.border * this.pixelSize;
				int num = Mathf.RoundToInt(vector.x + vector.z);
				UISpriteData atlasSprite = this.GetAtlasSprite();
				if (atlasSprite != null)
				{
					num += Mathf.RoundToInt(pixelSize * ((float)(atlasSprite.paddingLeft + atlasSprite.paddingRight) + this.mPadding.x + this.mPadding.y));
				}
				return Mathf.Max(base.minWidth, ((num & 1) != 1) ? num : (num + 1));
			}
			return base.minWidth;
		}
	}

	// Token: 0x170001C0 RID: 448
	// (get) Token: 0x06000A2A RID: 2602 RVA: 0x00057FEC File Offset: 0x000561EC
	public override int minHeight
	{
		get
		{
			if (this.type == UIBasicSprite.Type.Sliced || this.type == UIBasicSprite.Type.Advanced)
			{
				float pixelSize = this.pixelSize;
				Vector4 vector = this.border * this.pixelSize;
				int num = Mathf.RoundToInt(vector.y + vector.w);
				UISpriteData atlasSprite = this.GetAtlasSprite();
				if (atlasSprite != null)
				{
					num += Mathf.RoundToInt(pixelSize * ((float)(atlasSprite.paddingTop + atlasSprite.paddingBottom) + this.mPadding.x + this.mPadding.w));
				}
				return Mathf.Max(base.minHeight, ((num & 1) != 1) ? num : (num + 1));
			}
			return base.minHeight;
		}
	}

	// Token: 0x170001C1 RID: 449
	// (get) Token: 0x06000A2B RID: 2603 RVA: 0x000580A0 File Offset: 0x000562A0
	public override Vector4 drawingDimensions
	{
		get
		{
			Vector2 pivotOffset = base.pivotOffset;
			float num = -pivotOffset.x * (float)this.mWidth;
			float num2 = -pivotOffset.y * (float)this.mHeight;
			float num3 = num + (float)this.mWidth;
			float num4 = num2 + (float)this.mHeight;
			if (this.GetAtlasSprite() != null && this.mType != UIBasicSprite.Type.Tiled)
			{
				int num5 = this.mSprite.paddingLeft + (int)this.mPadding.x;
				int num6 = this.mSprite.paddingBottom + (int)this.mPadding.y;
				int num7 = this.mSprite.paddingRight + (int)this.mPadding.z;
				int num8 = this.mSprite.paddingTop + (int)this.mPadding.w;
				if (this.mType != UIBasicSprite.Type.Simple)
				{
					float pixelSize = this.pixelSize;
					if (pixelSize != 1f)
					{
						num5 = Mathf.RoundToInt(pixelSize * (float)num5);
						num6 = Mathf.RoundToInt(pixelSize * (float)num6);
						num7 = Mathf.RoundToInt(pixelSize * (float)num7);
						num8 = Mathf.RoundToInt(pixelSize * (float)num8);
					}
				}
				int num9 = this.mSprite.width + num5 + num7;
				int num10 = this.mSprite.height + num6 + num8;
				float num11 = 1f;
				float num12 = 1f;
				if (num9 > 0 && num10 > 0 && (this.mType == UIBasicSprite.Type.Simple || this.mType == UIBasicSprite.Type.Filled))
				{
					if ((num9 & 1) != 0)
					{
						num7++;
					}
					if ((num10 & 1) != 0)
					{
						num8++;
					}
					num11 = 1f / (float)num9 * (float)this.mWidth;
					num12 = 1f / (float)num10 * (float)this.mHeight;
				}
				if (this.mFlip == UIBasicSprite.Flip.Horizontally || this.mFlip == UIBasicSprite.Flip.Both)
				{
					num += (float)num7 * num11;
					num3 -= (float)num5 * num11;
				}
				else
				{
					num += (float)num5 * num11;
					num3 -= (float)num7 * num11;
				}
				if (this.mFlip == UIBasicSprite.Flip.Vertically || this.mFlip == UIBasicSprite.Flip.Both)
				{
					num2 += (float)num8 * num12;
					num4 -= (float)num6 * num12;
				}
				else
				{
					num2 += (float)num6 * num12;
					num4 -= (float)num8 * num12;
				}
			}
			if (this.mDrawRegion.x != 0f || this.mDrawRegion.y != 0f || this.mDrawRegion.z != 1f || this.mDrawRegion.w != 0f)
			{
				Vector4 vector = (!(this.mAtlas != null)) ? Vector4.zero : (this.border * this.pixelSize);
				float num13 = vector.x + vector.z;
				float num14 = vector.y + vector.w;
				float x = Mathf.Lerp(num, num3 - num13, this.mDrawRegion.x);
				float y = Mathf.Lerp(num2, num4 - num14, this.mDrawRegion.y);
				float z = Mathf.Lerp(num + num13, num3, this.mDrawRegion.z);
				float w = Mathf.Lerp(num2 + num14, num4, this.mDrawRegion.w);
				return new Vector4(x, y, z, w);
			}
			return new Vector4(num, num2, num3, num4);
		}
	}

	// Token: 0x170001C2 RID: 450
	// (get) Token: 0x06000A2C RID: 2604 RVA: 0x000583FC File Offset: 0x000565FC
	public override bool premultipliedAlpha
	{
		get
		{
			INGUIAtlas inguiatlas = this.mAtlas as INGUIAtlas;
			return inguiatlas != null && inguiatlas.premultipliedAlpha;
		}
	}

	// Token: 0x06000A2D RID: 2605 RVA: 0x00058424 File Offset: 0x00056624
	public UISpriteData GetAtlasSprite()
	{
		if (!this.mSpriteSet)
		{
			this.mSprite = null;
		}
		if (this.mSprite == null)
		{
			INGUIAtlas inguiatlas = this.mAtlas as INGUIAtlas;
			if (inguiatlas != null)
			{
				if (!string.IsNullOrEmpty(this.mSpriteName))
				{
					UISpriteData sprite = inguiatlas.GetSprite(this.mSpriteName);
					if (sprite == null)
					{
						return null;
					}
					this.SetAtlasSprite(sprite);
				}
				if (this.mSprite == null && inguiatlas.spriteList.Count > 0)
				{
					UISpriteData uispriteData = inguiatlas.spriteList[0];
					if (uispriteData == null)
					{
						return null;
					}
					this.SetAtlasSprite(uispriteData);
					if (this.mSprite == null)
					{
						Debug.LogError((inguiatlas as UnityEngine.Object).name + " seems to have a null sprite!");
						return null;
					}
					this.mSpriteName = this.mSprite.name;
				}
			}
		}
		return this.mSprite;
	}

	// Token: 0x06000A2E RID: 2606 RVA: 0x00058504 File Offset: 0x00056704
	protected void SetAtlasSprite(UISpriteData sp)
	{
		this.mChanged = true;
		this.mSpriteSet = true;
		if (sp != null)
		{
			this.mSprite = sp;
			this.mSpriteName = this.mSprite.name;
		}
		else
		{
			this.mSpriteName = ((this.mSprite == null) ? string.Empty : this.mSprite.name);
			this.mSprite = sp;
		}
	}

	// Token: 0x06000A2F RID: 2607 RVA: 0x00058570 File Offset: 0x00056770
	public override void MakePixelPerfect()
	{
		if (!this.isValid)
		{
			return;
		}
		base.MakePixelPerfect();
		if (this.mType == UIBasicSprite.Type.Tiled)
		{
			return;
		}
		UISpriteData atlasSprite = this.GetAtlasSprite();
		if (atlasSprite == null)
		{
			return;
		}
		Texture mainTexture = this.mainTexture;
		if (mainTexture == null)
		{
			return;
		}
		if ((this.mType == UIBasicSprite.Type.Simple || this.mType == UIBasicSprite.Type.Filled || !atlasSprite.hasBorder) && mainTexture != null)
		{
			int num = Mathf.RoundToInt(this.pixelSize * ((float)(atlasSprite.width + atlasSprite.paddingLeft + atlasSprite.paddingRight) + this.mPadding.x + this.mPadding.y));
			int num2 = Mathf.RoundToInt(this.pixelSize * ((float)(atlasSprite.height + atlasSprite.paddingTop + atlasSprite.paddingBottom) + this.mPadding.z + this.mPadding.w));
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

	// Token: 0x06000A30 RID: 2608 RVA: 0x0000B6D0 File Offset: 0x000098D0
	protected override void OnInit()
	{
		if (!this.mFillCenter)
		{
			this.mFillCenter = true;
			this.centerType = UIBasicSprite.AdvancedType.Invisible;
		}
		base.OnInit();
	}

	// Token: 0x06000A31 RID: 2609 RVA: 0x0000B702 File Offset: 0x00009902
	protected override void OnUpdate()
	{
		base.OnUpdate();
		if (this.mChanged || !this.mSpriteSet)
		{
			this.mSpriteSet = true;
			this.mSprite = null;
			this.mChanged = true;
		}
	}

	// Token: 0x06000A32 RID: 2610 RVA: 0x00058688 File Offset: 0x00056888
	public override void OnFill(List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
	{
		Texture mainTexture = this.mainTexture;
		if (mainTexture == null)
		{
			return;
		}
		if ((!this.mSpriteSet || this.mSprite == null) && this.GetAtlasSprite() == null)
		{
			return;
		}
		Rect rect = new Rect((float)this.mSprite.x, (float)this.mSprite.y, (float)this.mSprite.width, (float)this.mSprite.height);
		Rect rect2 = new Rect((float)(this.mSprite.x + this.mSprite.borderLeft), (float)(this.mSprite.y + this.mSprite.borderTop), (float)(this.mSprite.width - this.mSprite.borderLeft - this.mSprite.borderRight), (float)(this.mSprite.height - this.mSprite.borderBottom - this.mSprite.borderTop));
		rect = NGUIMath.ConvertToTexCoords(rect, mainTexture.width, mainTexture.height);
		rect2 = NGUIMath.ConvertToTexCoords(rect2, mainTexture.width, mainTexture.height);
		int count = verts.Count;
		base.Fill(verts, uvs, cols, rect, rect2);
		if (this.onPostFill != null)
		{
			this.onPostFill(this, count, verts, uvs, cols);
		}
	}

	// Token: 0x040006F6 RID: 1782
	[SerializeField]
	[HideInInspector]
	private Vector4 mBorder = Vector4.zero;

	// Token: 0x040006F7 RID: 1783
	[SerializeField]
	[HideInInspector]
	private Vector4 mPadding = Vector4.zero;

	// Token: 0x040006F8 RID: 1784
	[SerializeField]
	[HideInInspector]
	private UnityEngine.Object mAtlas;

	// Token: 0x040006F9 RID: 1785
	[SerializeField]
	[HideInInspector]
	private string mSpriteName;

	// Token: 0x040006FA RID: 1786
	[SerializeField]
	[HideInInspector]
	private bool mFillCenter = true;

	// Token: 0x040006FB RID: 1787
	[NonSerialized]
	protected UISpriteData mSprite;

	// Token: 0x040006FC RID: 1788
	[NonSerialized]
	private bool mSpriteSet;
}
