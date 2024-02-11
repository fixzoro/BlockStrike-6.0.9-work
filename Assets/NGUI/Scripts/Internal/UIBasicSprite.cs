using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x020000AE RID: 174
public abstract class UIBasicSprite : UIWidget
{
	// Token: 0x17000077 RID: 119
	// (get) Token: 0x06000589 RID: 1417 RVA: 0x000087DC File Offset: 0x000069DC
	// (set) Token: 0x0600058A RID: 1418 RVA: 0x000087E4 File Offset: 0x000069E4
	public virtual UIBasicSprite.Type type
	{
		get
		{
			return this.mType;
		}
		set
		{
			if (this.mType != value)
			{
				this.mType = value;
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x17000078 RID: 120
	// (get) Token: 0x0600058B RID: 1419 RVA: 0x000087FF File Offset: 0x000069FF
	// (set) Token: 0x0600058C RID: 1420 RVA: 0x00008807 File Offset: 0x00006A07
	public UIBasicSprite.Flip flip
	{
		get
		{
			return this.mFlip;
		}
		set
		{
			if (this.mFlip != value)
			{
				this.mFlip = value;
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x17000079 RID: 121
	// (get) Token: 0x0600058D RID: 1421 RVA: 0x00008822 File Offset: 0x00006A22
	// (set) Token: 0x0600058E RID: 1422 RVA: 0x0000882A File Offset: 0x00006A2A
	public UIBasicSprite.FillDirection fillDirection
	{
		get
		{
			return this.mFillDirection;
		}
		set
		{
			if (this.mFillDirection != value)
			{
				this.mFillDirection = value;
				this.mChanged = true;
			}
		}
	}

	// Token: 0x1700007A RID: 122
	// (get) Token: 0x0600058F RID: 1423 RVA: 0x00008846 File Offset: 0x00006A46
	// (set) Token: 0x06000590 RID: 1424 RVA: 0x0003EEE8 File Offset: 0x0003D0E8
	public float fillAmount
	{
		get
		{
			return this.mFillAmount;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (this.mFillAmount != num)
			{
				this.mFillAmount = num;
				this.mChanged = true;
			}
		}
	}

	// Token: 0x1700007B RID: 123
	// (get) Token: 0x06000591 RID: 1425 RVA: 0x0003EF18 File Offset: 0x0003D118
	public override int minWidth
	{
		get
		{
			if (this.type == UIBasicSprite.Type.Sliced || this.type == UIBasicSprite.Type.Advanced)
			{
				Vector4 vector = this.border * this.pixelSize;
				int num = Mathf.RoundToInt(vector.x + vector.z);
				return Mathf.Max(base.minWidth, ((num & 1) != 1) ? num : (num + 1));
			}
			return base.minWidth;
		}
	}

	// Token: 0x1700007C RID: 124
	// (get) Token: 0x06000592 RID: 1426 RVA: 0x0003EF88 File Offset: 0x0003D188
	public override int minHeight
	{
		get
		{
			if (this.type == UIBasicSprite.Type.Sliced || this.type == UIBasicSprite.Type.Advanced)
			{
				Vector4 vector = this.border * this.pixelSize;
				int num = Mathf.RoundToInt(vector.y + vector.w);
				return Mathf.Max(base.minHeight, ((num & 1) != 1) ? num : (num + 1));
			}
			return base.minHeight;
		}
	}

	// Token: 0x1700007D RID: 125
	// (get) Token: 0x06000593 RID: 1427 RVA: 0x0000884E File Offset: 0x00006A4E
	// (set) Token: 0x06000594 RID: 1428 RVA: 0x00008856 File Offset: 0x00006A56
	public bool invert
	{
		get
		{
			return this.mInvert;
		}
		set
		{
			if (this.mInvert != value)
			{
				this.mInvert = value;
				this.mChanged = true;
			}
		}
	}

	// Token: 0x1700007E RID: 126
	// (get) Token: 0x06000595 RID: 1429 RVA: 0x0003EFF8 File Offset: 0x0003D1F8
	public bool hasBorder
	{
		get
		{
			Vector4 border = this.border;
			return border.x != 0f || border.y != 0f || border.z != 0f || border.w != 0f;
		}
	}

	// Token: 0x1700007F RID: 127
	// (get) Token: 0x06000596 RID: 1430 RVA: 0x00008872 File Offset: 0x00006A72
	public virtual bool premultipliedAlpha
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000080 RID: 128
	// (get) Token: 0x06000597 RID: 1431 RVA: 0x00008875 File Offset: 0x00006A75
	public virtual float pixelSize
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x17000081 RID: 129
	// (get) Token: 0x06000598 RID: 1432 RVA: 0x0000887C File Offset: 0x00006A7C
	protected virtual Vector4 padding
	{
		get
		{
			return new Vector4(0f, 0f, 0f, 0f);
		}
	}

	// Token: 0x17000082 RID: 130
	// (get) Token: 0x06000599 RID: 1433 RVA: 0x0003F054 File Offset: 0x0003D254
	protected Vector4 drawingUVs
	{
		get
		{
			switch (this.mFlip)
			{
			case UIBasicSprite.Flip.Horizontally:
				return new Vector4(this.mOuterUV.xMax, this.mOuterUV.yMin, this.mOuterUV.xMin, this.mOuterUV.yMax);
			case UIBasicSprite.Flip.Vertically:
				return new Vector4(this.mOuterUV.xMin, this.mOuterUV.yMax, this.mOuterUV.xMax, this.mOuterUV.yMin);
			case UIBasicSprite.Flip.Both:
				return new Vector4(this.mOuterUV.xMax, this.mOuterUV.yMax, this.mOuterUV.xMin, this.mOuterUV.yMin);
			default:
				return new Vector4(this.mOuterUV.xMin, this.mOuterUV.yMin, this.mOuterUV.xMax, this.mOuterUV.yMax);
			}
		}
	}

	// Token: 0x17000083 RID: 131
	// (get) Token: 0x0600059A RID: 1434 RVA: 0x0003F148 File Offset: 0x0003D348
	protected Color drawingColor
	{
		get
		{
			Color color = base.color;
			color.a = this.finalAlpha;
			if (this.premultipliedAlpha)
			{
				color = NGUITools.ApplyPMA(color);
			}
			return color;
		}
	}

	// Token: 0x0600059B RID: 1435 RVA: 0x0003F17C File Offset: 0x0003D37C
	protected void Fill(List<Vector3> verts, List<Vector2> uvs, List<Color> cols, Rect outer, Rect inner)
	{
		this.mOuterUV = outer;
		this.mInnerUV = inner;
		Vector4 drawingDimensions = this.drawingDimensions;
		Vector4 drawingUVs = this.drawingUVs;
		Color drawingColor = this.drawingColor;
		switch (this.type)
		{
		case UIBasicSprite.Type.Simple:
			this.SimpleFill(verts, uvs, cols, ref drawingDimensions, ref drawingUVs, ref drawingColor);
			break;
		case UIBasicSprite.Type.Sliced:
			this.SlicedFill(verts, uvs, cols, ref drawingDimensions, ref drawingUVs, ref drawingColor);
			break;
		case UIBasicSprite.Type.Tiled:
			this.TiledFill(verts, uvs, cols, ref drawingDimensions, ref drawingColor);
			break;
		case UIBasicSprite.Type.Filled:
			this.FilledFill(verts, uvs, cols, ref drawingDimensions, ref drawingUVs, ref drawingColor);
			break;
		case UIBasicSprite.Type.Advanced:
			this.AdvancedFill(verts, uvs, cols, ref drawingDimensions, ref drawingUVs, ref drawingColor);
			break;
		}
	}

	// Token: 0x0600059C RID: 1436 RVA: 0x0003F238 File Offset: 0x0003D438
	protected void SimpleFill(List<Vector3> verts, List<Vector2> uvs, List<Color> cols, ref Vector4 v, ref Vector4 u, ref Color c)
	{
		verts.Add(new Vector3(v.x, v.y));
		verts.Add(new Vector3(v.x, v.w));
		verts.Add(new Vector3(v.z, v.w));
		verts.Add(new Vector3(v.z, v.y));
		uvs.Add(new Vector2(u.x, u.y));
		uvs.Add(new Vector2(u.x, u.w));
		uvs.Add(new Vector2(u.z, u.w));
		uvs.Add(new Vector2(u.z, u.y));
		if (!this.mApplyGradient)
		{
			cols.Add(c);
			cols.Add(c);
			cols.Add(c);
			cols.Add(c);
		}
		else
		{
			this.AddVertexColours(cols, ref c, 1, 1);
			this.AddVertexColours(cols, ref c, 1, 2);
			this.AddVertexColours(cols, ref c, 2, 2);
			this.AddVertexColours(cols, ref c, 2, 1);
		}
	}

	// Token: 0x0600059D RID: 1437 RVA: 0x0003F380 File Offset: 0x0003D580
	protected void SlicedFill(List<Vector3> verts, List<Vector2> uvs, List<Color> cols, ref Vector4 v, ref Vector4 u, ref Color gc)
	{
		Vector4 vector = this.border * this.pixelSize;
		if (vector.x == 0f && vector.y == 0f && vector.z == 0f && vector.w == 0f)
		{
			this.SimpleFill(verts, uvs, cols, ref v, ref u, ref gc);
			return;
		}
		UIBasicSprite.mTempPos[0].x = v.x;
		UIBasicSprite.mTempPos[0].y = v.y;
		UIBasicSprite.mTempPos[3].x = v.z;
		UIBasicSprite.mTempPos[3].y = v.w;
		if (this.mFlip == UIBasicSprite.Flip.Horizontally || this.mFlip == UIBasicSprite.Flip.Both)
		{
			UIBasicSprite.mTempPos[1].x = UIBasicSprite.mTempPos[0].x + vector.z;
			UIBasicSprite.mTempPos[2].x = UIBasicSprite.mTempPos[3].x - vector.x;
			UIBasicSprite.mTempUVs[3].x = this.mOuterUV.xMin;
			UIBasicSprite.mTempUVs[2].x = this.mInnerUV.xMin;
			UIBasicSprite.mTempUVs[1].x = this.mInnerUV.xMax;
			UIBasicSprite.mTempUVs[0].x = this.mOuterUV.xMax;
		}
		else
		{
			UIBasicSprite.mTempPos[1].x = UIBasicSprite.mTempPos[0].x + vector.x;
			UIBasicSprite.mTempPos[2].x = UIBasicSprite.mTempPos[3].x - vector.z;
			UIBasicSprite.mTempUVs[0].x = this.mOuterUV.xMin;
			UIBasicSprite.mTempUVs[1].x = this.mInnerUV.xMin;
			UIBasicSprite.mTempUVs[2].x = this.mInnerUV.xMax;
			UIBasicSprite.mTempUVs[3].x = this.mOuterUV.xMax;
		}
		if (this.mFlip == UIBasicSprite.Flip.Vertically || this.mFlip == UIBasicSprite.Flip.Both)
		{
			UIBasicSprite.mTempPos[1].y = UIBasicSprite.mTempPos[0].y + vector.w;
			UIBasicSprite.mTempPos[2].y = UIBasicSprite.mTempPos[3].y - vector.y;
			UIBasicSprite.mTempUVs[3].y = this.mOuterUV.yMin;
			UIBasicSprite.mTempUVs[2].y = this.mInnerUV.yMin;
			UIBasicSprite.mTempUVs[1].y = this.mInnerUV.yMax;
			UIBasicSprite.mTempUVs[0].y = this.mOuterUV.yMax;
		}
		else
		{
			UIBasicSprite.mTempPos[1].y = UIBasicSprite.mTempPos[0].y + vector.y;
			UIBasicSprite.mTempPos[2].y = UIBasicSprite.mTempPos[3].y - vector.w;
			UIBasicSprite.mTempUVs[0].y = this.mOuterUV.yMin;
			UIBasicSprite.mTempUVs[1].y = this.mInnerUV.yMin;
			UIBasicSprite.mTempUVs[2].y = this.mInnerUV.yMax;
			UIBasicSprite.mTempUVs[3].y = this.mOuterUV.yMax;
		}
		for (int i = 0; i < 3; i++)
		{
			int num = i + 1;
			for (int j = 0; j < 3; j++)
			{
				if (this.centerType != UIBasicSprite.AdvancedType.Invisible || i != 1 || j != 1)
				{
					int num2 = j + 1;
					verts.Add(new Vector3(UIBasicSprite.mTempPos[i].x, UIBasicSprite.mTempPos[j].y));
					verts.Add(new Vector3(UIBasicSprite.mTempPos[i].x, UIBasicSprite.mTempPos[num2].y));
					verts.Add(new Vector3(UIBasicSprite.mTempPos[num].x, UIBasicSprite.mTempPos[num2].y));
					verts.Add(new Vector3(UIBasicSprite.mTempPos[num].x, UIBasicSprite.mTempPos[j].y));
					uvs.Add(new Vector2(UIBasicSprite.mTempUVs[i].x, UIBasicSprite.mTempUVs[j].y));
					uvs.Add(new Vector2(UIBasicSprite.mTempUVs[i].x, UIBasicSprite.mTempUVs[num2].y));
					uvs.Add(new Vector2(UIBasicSprite.mTempUVs[num].x, UIBasicSprite.mTempUVs[num2].y));
					uvs.Add(new Vector2(UIBasicSprite.mTempUVs[num].x, UIBasicSprite.mTempUVs[j].y));
					if (!this.mApplyGradient)
					{
						cols.Add(gc);
						cols.Add(gc);
						cols.Add(gc);
						cols.Add(gc);
					}
					else
					{
						this.AddVertexColours(cols, ref gc, i, j);
						this.AddVertexColours(cols, ref gc, i, num2);
						this.AddVertexColours(cols, ref gc, num, num2);
						this.AddVertexColours(cols, ref gc, num, j);
					}
				}
			}
		}
	}

	// Token: 0x0600059E RID: 1438 RVA: 0x0003F994 File Offset: 0x0003DB94
	[DebuggerHidden]
	[DebuggerStepThrough]
	private void AddVertexColours(List<Color> cols, ref Color color, int x, int y)
	{
		Vector4 vector = this.border * this.pixelSize;
		if (this.type == UIBasicSprite.Type.Simple || (vector.x == 0f && vector.y == 0f && vector.z == 0f && vector.w == 0f))
		{
			if (y == 0 || y == 1)
			{
				cols.Add(color * this.mGradientBottom);
			}
			else if (y == 2 || y == 3)
			{
				cols.Add(color * this.mGradientTop);
			}
		}
		else
		{
			if (y == 0)
			{
				cols.Add(color * this.mGradientBottom);
			}
			if (y == 1)
			{
				Color b = Color.Lerp(this.mGradientBottom, this.mGradientTop, vector.y / (float)this.mHeight);
				cols.Add(color * b);
			}
			if (y == 2)
			{
				Color b2 = Color.Lerp(this.mGradientTop, this.mGradientBottom, vector.w / (float)this.mHeight);
				cols.Add(color * b2);
			}
			if (y == 3)
			{
				cols.Add(color * this.mGradientTop);
			}
		}
	}

	// Token: 0x0600059F RID: 1439 RVA: 0x0003FB0C File Offset: 0x0003DD0C
	protected void TiledFill(List<Vector3> verts, List<Vector2> uvs, List<Color> cols, ref Vector4 v, ref Color c)
	{
		Texture mainTexture = this.mainTexture;
		if (mainTexture == null)
		{
			return;
		}
		Vector2 a = new Vector2(this.mInnerUV.width * (float)mainTexture.width, this.mInnerUV.height * (float)mainTexture.height);
		a *= this.pixelSize;
		if (a.x < 2f || a.y < 2f)
		{
			return;
		}
		Vector4 padding = this.padding;
		Vector4 vector;
		Vector4 vector2;
		if (this.mFlip == UIBasicSprite.Flip.Horizontally || this.mFlip == UIBasicSprite.Flip.Both)
		{
			vector.x = this.mInnerUV.xMax;
			vector.z = this.mInnerUV.xMin;
			vector2.x = padding.z * this.pixelSize;
			vector2.z = padding.x * this.pixelSize;
		}
		else
		{
			vector.x = this.mInnerUV.xMin;
			vector.z = this.mInnerUV.xMax;
			vector2.x = padding.x * this.pixelSize;
			vector2.z = padding.z * this.pixelSize;
		}
		if (this.mFlip == UIBasicSprite.Flip.Vertically || this.mFlip == UIBasicSprite.Flip.Both)
		{
			vector.y = this.mInnerUV.yMax;
			vector.w = this.mInnerUV.yMin;
			vector2.y = padding.w * this.pixelSize;
			vector2.w = padding.y * this.pixelSize;
		}
		else
		{
			vector.y = this.mInnerUV.yMin;
			vector.w = this.mInnerUV.yMax;
			vector2.y = padding.y * this.pixelSize;
			vector2.w = padding.w * this.pixelSize;
		}
		float num = v.x;
		float num2 = v.y;
		float x = vector.x;
		float y = vector.y;
		while (num2 < v.w)
		{
			num2 += vector2.y;
			num = v.x;
			float num3 = num2 + a.y;
			float y2 = vector.w;
			if (num3 > v.w)
			{
				y2 = Mathf.Lerp(vector.y, vector.w, (v.w - num2) / a.y);
				num3 = v.w;
			}
			while (num < v.z)
			{
				num += vector2.x;
				float num4 = num + a.x;
				float x2 = vector.z;
				if (num4 > v.z)
				{
					x2 = Mathf.Lerp(vector.x, vector.z, (v.z - num) / a.x);
					num4 = v.z;
				}
				verts.Add(new Vector3(num, num2));
				verts.Add(new Vector3(num, num3));
				verts.Add(new Vector3(num4, num3));
				verts.Add(new Vector3(num4, num2));
				uvs.Add(new Vector2(x, y));
				uvs.Add(new Vector2(x, y2));
				uvs.Add(new Vector2(x2, y2));
				uvs.Add(new Vector2(x2, y));
				cols.Add(c);
				cols.Add(c);
				cols.Add(c);
				cols.Add(c);
				num += a.x + vector2.z;
			}
			num2 += a.y + vector2.w;
		}
	}

	// Token: 0x060005A0 RID: 1440 RVA: 0x0003FEEC File Offset: 0x0003E0EC
	protected void FilledFill(List<Vector3> verts, List<Vector2> uvs, List<Color> cols, ref Vector4 v, ref Vector4 u, ref Color c)
	{
		if (this.mFillAmount < 0.001f)
		{
			return;
		}
		if (this.mFillDirection == UIBasicSprite.FillDirection.Horizontal || this.mFillDirection == UIBasicSprite.FillDirection.Vertical)
		{
			if (this.mFillDirection == UIBasicSprite.FillDirection.Horizontal)
			{
				float num = (u.z - u.x) * this.mFillAmount;
				if (this.mInvert)
				{
					v.x = v.z - (v.z - v.x) * this.mFillAmount;
					u.x = u.z - num;
				}
				else
				{
					v.z = v.x + (v.z - v.x) * this.mFillAmount;
					u.z = u.x + num;
				}
			}
			else if (this.mFillDirection == UIBasicSprite.FillDirection.Vertical)
			{
				float num2 = (u.w - u.y) * this.mFillAmount;
				if (this.mInvert)
				{
					v.y = v.w - (v.w - v.y) * this.mFillAmount;
					u.y = u.w - num2;
				}
				else
				{
					v.w = v.y + (v.w - v.y) * this.mFillAmount;
					u.w = u.y + num2;
				}
			}
		}
		UIBasicSprite.mTempPos[0] = new Vector2(v.x, v.y);
		UIBasicSprite.mTempPos[1] = new Vector2(v.x, v.w);
		UIBasicSprite.mTempPos[2] = new Vector2(v.z, v.w);
		UIBasicSprite.mTempPos[3] = new Vector2(v.z, v.y);
		UIBasicSprite.mTempUVs[0] = new Vector2(u.x, u.y);
		UIBasicSprite.mTempUVs[1] = new Vector2(u.x, u.w);
		UIBasicSprite.mTempUVs[2] = new Vector2(u.z, u.w);
		UIBasicSprite.mTempUVs[3] = new Vector2(u.z, u.y);
		if (this.mFillAmount < 1f)
		{
			if (this.mFillDirection == UIBasicSprite.FillDirection.Radial90)
			{
				if (UIBasicSprite.RadialCut(UIBasicSprite.mTempPos, UIBasicSprite.mTempUVs, this.mFillAmount, this.mInvert, 0))
				{
					for (int i = 0; i < 4; i++)
					{
						verts.Add(UIBasicSprite.mTempPos[i]);
						uvs.Add(UIBasicSprite.mTempUVs[i]);
						cols.Add(c);
					}
				}
				return;
			}
			if (this.mFillDirection == UIBasicSprite.FillDirection.Radial180)
			{
				for (int j = 0; j < 2; j++)
				{
					float t = 0f;
					float t2 = 1f;
					float t3;
					float t4;
					if (j == 0)
					{
						t3 = 0f;
						t4 = 0.5f;
					}
					else
					{
						t3 = 0.5f;
						t4 = 1f;
					}
					UIBasicSprite.mTempPos[0].x = Mathf.Lerp(v.x, v.z, t3);
					UIBasicSprite.mTempPos[1].x = UIBasicSprite.mTempPos[0].x;
					UIBasicSprite.mTempPos[2].x = Mathf.Lerp(v.x, v.z, t4);
					UIBasicSprite.mTempPos[3].x = UIBasicSprite.mTempPos[2].x;
					UIBasicSprite.mTempPos[0].y = Mathf.Lerp(v.y, v.w, t);
					UIBasicSprite.mTempPos[1].y = Mathf.Lerp(v.y, v.w, t2);
					UIBasicSprite.mTempPos[2].y = UIBasicSprite.mTempPos[1].y;
					UIBasicSprite.mTempPos[3].y = UIBasicSprite.mTempPos[0].y;
					UIBasicSprite.mTempUVs[0].x = Mathf.Lerp(u.x, u.z, t3);
					UIBasicSprite.mTempUVs[1].x = UIBasicSprite.mTempUVs[0].x;
					UIBasicSprite.mTempUVs[2].x = Mathf.Lerp(u.x, u.z, t4);
					UIBasicSprite.mTempUVs[3].x = UIBasicSprite.mTempUVs[2].x;
					UIBasicSprite.mTempUVs[0].y = Mathf.Lerp(u.y, u.w, t);
					UIBasicSprite.mTempUVs[1].y = Mathf.Lerp(u.y, u.w, t2);
					UIBasicSprite.mTempUVs[2].y = UIBasicSprite.mTempUVs[1].y;
					UIBasicSprite.mTempUVs[3].y = UIBasicSprite.mTempUVs[0].y;
					float value = this.mInvert ? (this.mFillAmount * 2f - (float)(1 - j)) : (this.fillAmount * 2f - (float)j);
					if (UIBasicSprite.RadialCut(UIBasicSprite.mTempPos, UIBasicSprite.mTempUVs, Mathf.Clamp01(value), !this.mInvert, NGUIMath.RepeatIndex(j + 3, 4)))
					{
						for (int k = 0; k < 4; k++)
						{
							verts.Add(UIBasicSprite.mTempPos[k]);
							uvs.Add(UIBasicSprite.mTempUVs[k]);
							cols.Add(c);
						}
					}
				}
				return;
			}
			if (this.mFillDirection == UIBasicSprite.FillDirection.Radial360)
			{
				for (int l = 0; l < 4; l++)
				{
					float t5;
					float t6;
					if (l < 2)
					{
						t5 = 0f;
						t6 = 0.5f;
					}
					else
					{
						t5 = 0.5f;
						t6 = 1f;
					}
					float t7;
					float t8;
					if (l == 0 || l == 3)
					{
						t7 = 0f;
						t8 = 0.5f;
					}
					else
					{
						t7 = 0.5f;
						t8 = 1f;
					}
					UIBasicSprite.mTempPos[0].x = Mathf.Lerp(v.x, v.z, t5);
					UIBasicSprite.mTempPos[1].x = UIBasicSprite.mTempPos[0].x;
					UIBasicSprite.mTempPos[2].x = Mathf.Lerp(v.x, v.z, t6);
					UIBasicSprite.mTempPos[3].x = UIBasicSprite.mTempPos[2].x;
					UIBasicSprite.mTempPos[0].y = Mathf.Lerp(v.y, v.w, t7);
					UIBasicSprite.mTempPos[1].y = Mathf.Lerp(v.y, v.w, t8);
					UIBasicSprite.mTempPos[2].y = UIBasicSprite.mTempPos[1].y;
					UIBasicSprite.mTempPos[3].y = UIBasicSprite.mTempPos[0].y;
					UIBasicSprite.mTempUVs[0].x = Mathf.Lerp(u.x, u.z, t5);
					UIBasicSprite.mTempUVs[1].x = UIBasicSprite.mTempUVs[0].x;
					UIBasicSprite.mTempUVs[2].x = Mathf.Lerp(u.x, u.z, t6);
					UIBasicSprite.mTempUVs[3].x = UIBasicSprite.mTempUVs[2].x;
					UIBasicSprite.mTempUVs[0].y = Mathf.Lerp(u.y, u.w, t7);
					UIBasicSprite.mTempUVs[1].y = Mathf.Lerp(u.y, u.w, t8);
					UIBasicSprite.mTempUVs[2].y = UIBasicSprite.mTempUVs[1].y;
					UIBasicSprite.mTempUVs[3].y = UIBasicSprite.mTempUVs[0].y;
					float value2 = (!this.mInvert) ? (this.mFillAmount * 4f - (float)(3 - NGUIMath.RepeatIndex(l + 2, 4))) : (this.mFillAmount * 4f - (float)NGUIMath.RepeatIndex(l + 2, 4));
					if (UIBasicSprite.RadialCut(UIBasicSprite.mTempPos, UIBasicSprite.mTempUVs, Mathf.Clamp01(value2), this.mInvert, NGUIMath.RepeatIndex(l + 2, 4)))
					{
						for (int m = 0; m < 4; m++)
						{
							verts.Add(UIBasicSprite.mTempPos[m]);
							uvs.Add(UIBasicSprite.mTempUVs[m]);
							cols.Add(c);
						}
					}
				}
				return;
			}
		}
		for (int n = 0; n < 4; n++)
		{
			verts.Add(UIBasicSprite.mTempPos[n]);
			uvs.Add(UIBasicSprite.mTempUVs[n]);
			cols.Add(c);
		}
	}

	// Token: 0x060005A1 RID: 1441 RVA: 0x000408F8 File Offset: 0x0003EAF8
	protected void AdvancedFill(List<Vector3> verts, List<Vector2> uvs, List<Color> cols, ref Vector4 v, ref Vector4 u, ref Color c)
	{
		Texture mainTexture = this.mainTexture;
		if (mainTexture == null)
		{
			return;
		}
		Vector4 vector = this.border * this.pixelSize;
		if (vector.x == 0f && vector.y == 0f && vector.z == 0f && vector.w == 0f)
		{
			this.SimpleFill(verts, uvs, cols, ref v, ref u, ref c);
			return;
		}
		Vector2 a = new Vector2(this.mInnerUV.width * (float)mainTexture.width, this.mInnerUV.height * (float)mainTexture.height);
		a *= this.pixelSize;
		if (a.x < 1f)
		{
			a.x = 1f;
		}
		if (a.y < 1f)
		{
			a.y = 1f;
		}
		UIBasicSprite.mTempPos[0].x = v.x;
		UIBasicSprite.mTempPos[0].y = v.y;
		UIBasicSprite.mTempPos[3].x = v.z;
		UIBasicSprite.mTempPos[3].y = v.w;
		if (this.mFlip == UIBasicSprite.Flip.Horizontally || this.mFlip == UIBasicSprite.Flip.Both)
		{
			UIBasicSprite.mTempPos[1].x = UIBasicSprite.mTempPos[0].x + vector.z;
			UIBasicSprite.mTempPos[2].x = UIBasicSprite.mTempPos[3].x - vector.x;
			UIBasicSprite.mTempUVs[3].x = this.mOuterUV.xMin;
			UIBasicSprite.mTempUVs[2].x = this.mInnerUV.xMin;
			UIBasicSprite.mTempUVs[1].x = this.mInnerUV.xMax;
			UIBasicSprite.mTempUVs[0].x = this.mOuterUV.xMax;
		}
		else
		{
			UIBasicSprite.mTempPos[1].x = UIBasicSprite.mTempPos[0].x + vector.x;
			UIBasicSprite.mTempPos[2].x = UIBasicSprite.mTempPos[3].x - vector.z;
			UIBasicSprite.mTempUVs[0].x = this.mOuterUV.xMin;
			UIBasicSprite.mTempUVs[1].x = this.mInnerUV.xMin;
			UIBasicSprite.mTempUVs[2].x = this.mInnerUV.xMax;
			UIBasicSprite.mTempUVs[3].x = this.mOuterUV.xMax;
		}
		if (this.mFlip == UIBasicSprite.Flip.Vertically || this.mFlip == UIBasicSprite.Flip.Both)
		{
			UIBasicSprite.mTempPos[1].y = UIBasicSprite.mTempPos[0].y + vector.w;
			UIBasicSprite.mTempPos[2].y = UIBasicSprite.mTempPos[3].y - vector.y;
			UIBasicSprite.mTempUVs[3].y = this.mOuterUV.yMin;
			UIBasicSprite.mTempUVs[2].y = this.mInnerUV.yMin;
			UIBasicSprite.mTempUVs[1].y = this.mInnerUV.yMax;
			UIBasicSprite.mTempUVs[0].y = this.mOuterUV.yMax;
		}
		else
		{
			UIBasicSprite.mTempPos[1].y = UIBasicSprite.mTempPos[0].y + vector.y;
			UIBasicSprite.mTempPos[2].y = UIBasicSprite.mTempPos[3].y - vector.w;
			UIBasicSprite.mTempUVs[0].y = this.mOuterUV.yMin;
			UIBasicSprite.mTempUVs[1].y = this.mInnerUV.yMin;
			UIBasicSprite.mTempUVs[2].y = this.mInnerUV.yMax;
			UIBasicSprite.mTempUVs[3].y = this.mOuterUV.yMax;
		}
		for (int i = 0; i < 3; i++)
		{
			int num = i + 1;
			for (int j = 0; j < 3; j++)
			{
				if (this.centerType != UIBasicSprite.AdvancedType.Invisible || i != 1 || j != 1)
				{
					int num2 = j + 1;
					if (i == 1 && j == 1)
					{
						if (this.centerType == UIBasicSprite.AdvancedType.Tiled)
						{
							float x = UIBasicSprite.mTempPos[i].x;
							float x2 = UIBasicSprite.mTempPos[num].x;
							float y = UIBasicSprite.mTempPos[j].y;
							float y2 = UIBasicSprite.mTempPos[num2].y;
							float x3 = UIBasicSprite.mTempUVs[i].x;
							float y3 = UIBasicSprite.mTempUVs[j].y;
							for (float num3 = y; num3 < y2; num3 += a.y)
							{
								float num4 = x;
								float num5 = UIBasicSprite.mTempUVs[num2].y;
								float num6 = num3 + a.y;
								if (num6 > y2)
								{
									num5 = Mathf.Lerp(y3, num5, (y2 - num3) / a.y);
									num6 = y2;
								}
								while (num4 < x2)
								{
									float num7 = num4 + a.x;
									float num8 = UIBasicSprite.mTempUVs[num].x;
									if (num7 > x2)
									{
										num8 = Mathf.Lerp(x3, num8, (x2 - num4) / a.x);
										num7 = x2;
									}
									UIBasicSprite.Fill(verts, uvs, cols, num4, num7, num3, num6, x3, num8, y3, num5, c);
									num4 += a.x;
								}
							}
						}
						else if (this.centerType == UIBasicSprite.AdvancedType.Sliced)
						{
							UIBasicSprite.Fill(verts, uvs, cols, UIBasicSprite.mTempPos[i].x, UIBasicSprite.mTempPos[num].x, UIBasicSprite.mTempPos[j].y, UIBasicSprite.mTempPos[num2].y, UIBasicSprite.mTempUVs[i].x, UIBasicSprite.mTempUVs[num].x, UIBasicSprite.mTempUVs[j].y, UIBasicSprite.mTempUVs[num2].y, c);
						}
					}
					else if (i == 1)
					{
						if ((j == 0 && this.bottomType == UIBasicSprite.AdvancedType.Tiled) || (j == 2 && this.topType == UIBasicSprite.AdvancedType.Tiled))
						{
							float x4 = UIBasicSprite.mTempPos[i].x;
							float x5 = UIBasicSprite.mTempPos[num].x;
							float y4 = UIBasicSprite.mTempPos[j].y;
							float y5 = UIBasicSprite.mTempPos[num2].y;
							float x6 = UIBasicSprite.mTempUVs[i].x;
							float y6 = UIBasicSprite.mTempUVs[j].y;
							float y7 = UIBasicSprite.mTempUVs[num2].y;
							for (float num9 = x4; num9 < x5; num9 += a.x)
							{
								float num10 = num9 + a.x;
								float num11 = UIBasicSprite.mTempUVs[num].x;
								if (num10 > x5)
								{
									num11 = Mathf.Lerp(x6, num11, (x5 - num9) / a.x);
									num10 = x5;
								}
								UIBasicSprite.Fill(verts, uvs, cols, num9, num10, y4, y5, x6, num11, y6, y7, c);
							}
						}
						else if ((j == 0 && this.bottomType != UIBasicSprite.AdvancedType.Invisible) || (j == 2 && this.topType != UIBasicSprite.AdvancedType.Invisible))
						{
							UIBasicSprite.Fill(verts, uvs, cols, UIBasicSprite.mTempPos[i].x, UIBasicSprite.mTempPos[num].x, UIBasicSprite.mTempPos[j].y, UIBasicSprite.mTempPos[num2].y, UIBasicSprite.mTempUVs[i].x, UIBasicSprite.mTempUVs[num].x, UIBasicSprite.mTempUVs[j].y, UIBasicSprite.mTempUVs[num2].y, c);
						}
					}
					else if (j == 1)
					{
						if ((i == 0 && this.leftType == UIBasicSprite.AdvancedType.Tiled) || (i == 2 && this.rightType == UIBasicSprite.AdvancedType.Tiled))
						{
							float x7 = UIBasicSprite.mTempPos[i].x;
							float x8 = UIBasicSprite.mTempPos[num].x;
							float y8 = UIBasicSprite.mTempPos[j].y;
							float y9 = UIBasicSprite.mTempPos[num2].y;
							float x9 = UIBasicSprite.mTempUVs[i].x;
							float x10 = UIBasicSprite.mTempUVs[num].x;
							float y10 = UIBasicSprite.mTempUVs[j].y;
							for (float num12 = y8; num12 < y9; num12 += a.y)
							{
								float num13 = UIBasicSprite.mTempUVs[num2].y;
								float num14 = num12 + a.y;
								if (num14 > y9)
								{
									num13 = Mathf.Lerp(y10, num13, (y9 - num12) / a.y);
									num14 = y9;
								}
								UIBasicSprite.Fill(verts, uvs, cols, x7, x8, num12, num14, x9, x10, y10, num13, c);
							}
						}
						else if ((i == 0 && this.leftType != UIBasicSprite.AdvancedType.Invisible) || (i == 2 && this.rightType != UIBasicSprite.AdvancedType.Invisible))
						{
							UIBasicSprite.Fill(verts, uvs, cols, UIBasicSprite.mTempPos[i].x, UIBasicSprite.mTempPos[num].x, UIBasicSprite.mTempPos[j].y, UIBasicSprite.mTempPos[num2].y, UIBasicSprite.mTempUVs[i].x, UIBasicSprite.mTempUVs[num].x, UIBasicSprite.mTempUVs[j].y, UIBasicSprite.mTempUVs[num2].y, c);
						}
					}
					else if (j != 0 || this.bottomType != UIBasicSprite.AdvancedType.Invisible)
					{
						if (j != 2 || this.topType != UIBasicSprite.AdvancedType.Invisible)
						{
							if (i != 0 || this.leftType != UIBasicSprite.AdvancedType.Invisible)
							{
								if (i != 2 || this.rightType != UIBasicSprite.AdvancedType.Invisible)
								{
									UIBasicSprite.Fill(verts, uvs, cols, UIBasicSprite.mTempPos[i].x, UIBasicSprite.mTempPos[num].x, UIBasicSprite.mTempPos[j].y, UIBasicSprite.mTempPos[num2].y, UIBasicSprite.mTempUVs[i].x, UIBasicSprite.mTempUVs[num].x, UIBasicSprite.mTempUVs[j].y, UIBasicSprite.mTempUVs[num2].y, c);
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060005A2 RID: 1442 RVA: 0x000414C0 File Offset: 0x0003F6C0
	private static bool RadialCut(Vector2[] xy, Vector2[] uv, float fill, bool invert, int corner)
	{
		if (fill < 0.001f)
		{
			return false;
		}
		if ((corner & 1) == 1)
		{
			invert = !invert;
		}
		if (!invert && fill > 0.999f)
		{
			return true;
		}
		float num = Mathf.Clamp01(fill);
		if (invert)
		{
			num = 1f - num;
		}
		num *= 1.57079637f;
		float cos = Mathf.Cos(num);
		float sin = Mathf.Sin(num);
		UIBasicSprite.RadialCut(xy, cos, sin, invert, corner);
		UIBasicSprite.RadialCut(uv, cos, sin, invert, corner);
		return true;
	}

	// Token: 0x060005A3 RID: 1443 RVA: 0x00041540 File Offset: 0x0003F740
	private static void RadialCut(Vector2[] xy, float cos, float sin, bool invert, int corner)
	{
		int num = NGUIMath.RepeatIndex(corner + 1, 4);
		int num2 = NGUIMath.RepeatIndex(corner + 2, 4);
		int num3 = NGUIMath.RepeatIndex(corner + 3, 4);
		if ((corner & 1) == 1)
		{
			if (sin > cos)
			{
				cos /= sin;
				sin = 1f;
				if (invert)
				{
					xy[num].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
					xy[num2].x = xy[num].x;
				}
			}
			else if (cos > sin)
			{
				sin /= cos;
				cos = 1f;
				if (!invert)
				{
					xy[num2].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
					xy[num3].y = xy[num2].y;
				}
			}
			else
			{
				cos = 1f;
				sin = 1f;
			}
			if (!invert)
			{
				xy[num3].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
			}
			else
			{
				xy[num].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
			}
		}
		else
		{
			if (cos > sin)
			{
				sin /= cos;
				cos = 1f;
				if (!invert)
				{
					xy[num].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
					xy[num2].y = xy[num].y;
				}
			}
			else if (sin > cos)
			{
				cos /= sin;
				sin = 1f;
				if (invert)
				{
					xy[num2].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
					xy[num3].x = xy[num2].x;
				}
			}
			else
			{
				cos = 1f;
				sin = 1f;
			}
			if (invert)
			{
				xy[num3].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
			}
			else
			{
				xy[num].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
			}
		}
	}

	// Token: 0x060005A4 RID: 1444 RVA: 0x000417DC File Offset: 0x0003F9DC
	private static void Fill(List<Vector3> verts, List<Vector2> uvs, List<Color> cols, float v0x, float v1x, float v0y, float v1y, float u0x, float u1x, float u0y, float u1y, Color col)
	{
		verts.Add(new Vector3(v0x, v0y));
		verts.Add(new Vector3(v0x, v1y));
		verts.Add(new Vector3(v1x, v1y));
		verts.Add(new Vector3(v1x, v0y));
		uvs.Add(new Vector2(u0x, u0y));
		uvs.Add(new Vector2(u0x, u1y));
		uvs.Add(new Vector2(u1x, u1y));
		uvs.Add(new Vector2(u1x, u0y));
		cols.Add(col);
		cols.Add(col);
		cols.Add(col);
		cols.Add(col);
	}

	// Token: 0x040003CD RID: 973
	[SerializeField]
	[HideInInspector]
	protected UIBasicSprite.Type mType;

	// Token: 0x040003CE RID: 974
	[SerializeField]
	[HideInInspector]
	protected UIBasicSprite.FillDirection mFillDirection = UIBasicSprite.FillDirection.Radial360;

	// Token: 0x040003CF RID: 975
	[HideInInspector]
	[SerializeField]
	[Range(0f, 1f)]
	protected float mFillAmount = 1f;

	// Token: 0x040003D0 RID: 976
	[SerializeField]
	[HideInInspector]
	protected bool mInvert;

	// Token: 0x040003D1 RID: 977
	[SerializeField]
	[HideInInspector]
	protected UIBasicSprite.Flip mFlip;

	// Token: 0x040003D2 RID: 978
	[SerializeField]
	[HideInInspector]
	protected bool mApplyGradient;

	// Token: 0x040003D3 RID: 979
	[HideInInspector]
	[SerializeField]
	protected Color mGradientTop = Color.white;

	// Token: 0x040003D4 RID: 980
	[HideInInspector]
	[SerializeField]
	protected Color mGradientBottom = new Color(0.7f, 0.7f, 0.7f);

	// Token: 0x040003D5 RID: 981
	[NonSerialized]
	protected Rect mInnerUV = default(Rect);

	// Token: 0x040003D6 RID: 982
	[NonSerialized]
	protected Rect mOuterUV = default(Rect);

	// Token: 0x040003D7 RID: 983
	public UIBasicSprite.AdvancedType centerType = UIBasicSprite.AdvancedType.Sliced;

	// Token: 0x040003D8 RID: 984
	public UIBasicSprite.AdvancedType leftType = UIBasicSprite.AdvancedType.Sliced;

	// Token: 0x040003D9 RID: 985
	public UIBasicSprite.AdvancedType rightType = UIBasicSprite.AdvancedType.Sliced;

	// Token: 0x040003DA RID: 986
	public UIBasicSprite.AdvancedType bottomType = UIBasicSprite.AdvancedType.Sliced;

	// Token: 0x040003DB RID: 987
	public UIBasicSprite.AdvancedType topType = UIBasicSprite.AdvancedType.Sliced;

	// Token: 0x040003DC RID: 988
	protected static Vector2[] mTempPos = new Vector2[4];

	// Token: 0x040003DD RID: 989
	protected static Vector2[] mTempUVs = new Vector2[4];

	// Token: 0x020000AF RID: 175
	[DoNotObfuscateNGUI]
	public enum Type
	{
		// Token: 0x040003DF RID: 991
		Simple,
		// Token: 0x040003E0 RID: 992
		Sliced,
		// Token: 0x040003E1 RID: 993
		Tiled,
		// Token: 0x040003E2 RID: 994
		Filled,
		// Token: 0x040003E3 RID: 995
		Advanced
	}

	// Token: 0x020000B0 RID: 176
	[DoNotObfuscateNGUI]
	public enum FillDirection
	{
		// Token: 0x040003E5 RID: 997
		Horizontal,
		// Token: 0x040003E6 RID: 998
		Vertical,
		// Token: 0x040003E7 RID: 999
		Radial90,
		// Token: 0x040003E8 RID: 1000
		Radial180,
		// Token: 0x040003E9 RID: 1001
		Radial360
	}

	// Token: 0x020000B1 RID: 177
	[DoNotObfuscateNGUI]
	public enum AdvancedType
	{
		// Token: 0x040003EB RID: 1003
		Invisible,
		// Token: 0x040003EC RID: 1004
		Sliced,
		// Token: 0x040003ED RID: 1005
		Tiled
	}

	// Token: 0x020000B2 RID: 178
	[DoNotObfuscateNGUI]
	public enum Flip
	{
		// Token: 0x040003EF RID: 1007
		Nothing,
		// Token: 0x040003F0 RID: 1008
		Horizontally,
		// Token: 0x040003F1 RID: 1009
		Vertically,
		// Token: 0x040003F2 RID: 1010
		Both
	}
}
