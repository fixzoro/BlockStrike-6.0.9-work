using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x020000C4 RID: 196
[AddComponentMenu("NGUI/UI/Invisible Widget")]
[ExecuteInEditMode]
public class UIWidget : UIRect
{
	// Token: 0x170000A3 RID: 163
	// (get) Token: 0x06000646 RID: 1606 RVA: 0x000091A4 File Offset: 0x000073A4
	// (set) Token: 0x06000647 RID: 1607 RVA: 0x00044594 File Offset: 0x00042794
	public UIDrawCall.OnRenderCallback onRender
	{
		get
		{
			return this.mOnRender;
		}
		set
		{
			if (this.mOnRender != value)
			{
				if (this.drawCall != null && this.drawCall.onRender != null && this.mOnRender != null)
				{
					UIDrawCall uidrawCall = this.drawCall;
					uidrawCall.onRender = (UIDrawCall.OnRenderCallback)Delegate.Remove(uidrawCall.onRender, this.mOnRender);
				}
				this.mOnRender = value;
				if (this.drawCall != null)
				{
					UIDrawCall uidrawCall2 = this.drawCall;
					uidrawCall2.onRender = (UIDrawCall.OnRenderCallback)Delegate.Combine(uidrawCall2.onRender, value);
				}
			}
		}
	}

	// Token: 0x170000A4 RID: 164
	// (get) Token: 0x06000648 RID: 1608 RVA: 0x000091AC File Offset: 0x000073AC
	// (set) Token: 0x06000649 RID: 1609 RVA: 0x000091B4 File Offset: 0x000073B4
	public Vector4 drawRegion
	{
		get
		{
			return this.mDrawRegion;
		}
		set
		{
			if (this.mDrawRegion != value)
			{
				this.mDrawRegion = value;
				if (this.autoResizeBoxCollider)
				{
					this.ResizeCollider();
				}
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x170000A5 RID: 165
	// (get) Token: 0x0600064A RID: 1610 RVA: 0x000091E5 File Offset: 0x000073E5
	public Vector2 pivotOffset
	{
		get
		{
			return NGUIMath.GetPivotOffset(this.pivot);
		}
	}

	// Token: 0x170000A6 RID: 166
	// (get) Token: 0x0600064B RID: 1611 RVA: 0x000091F2 File Offset: 0x000073F2
	// (set) Token: 0x0600064C RID: 1612 RVA: 0x00044634 File Offset: 0x00042834
	public int width
	{
		get
		{
			return this.mWidth;
		}
		set
		{
			int minWidth = this.minWidth;
			if (value < minWidth)
			{
				value = minWidth;
			}
			if (this.mWidth != value && this.keepAspectRatio != UIWidget.AspectRatioSource.BasedOnHeight)
			{
				if (this.isAnchoredHorizontally)
				{
					if (this.leftAnchor.target != null && this.rightAnchor.target != null)
					{
						if (this.mPivot == UIWidget.Pivot.BottomLeft || this.mPivot == UIWidget.Pivot.Left || this.mPivot == UIWidget.Pivot.TopLeft)
						{
							NGUIMath.AdjustWidget(this, 0f, 0f, (float)(value - this.mWidth), 0f);
						}
						else if (this.mPivot == UIWidget.Pivot.BottomRight || this.mPivot == UIWidget.Pivot.Right || this.mPivot == UIWidget.Pivot.TopRight)
						{
							NGUIMath.AdjustWidget(this, (float)(this.mWidth - value), 0f, 0f, 0f);
						}
						else
						{
							int num = value - this.mWidth;
							num -= (num & 1);
							if (num != 0)
							{
								NGUIMath.AdjustWidget(this, (float)(-(float)num) * 0.5f, 0f, (float)num * 0.5f, 0f);
							}
						}
					}
					else if (this.leftAnchor.target != null)
					{
						NGUIMath.AdjustWidget(this, 0f, 0f, (float)(value - this.mWidth), 0f);
					}
					else
					{
						NGUIMath.AdjustWidget(this, (float)(this.mWidth - value), 0f, 0f, 0f);
					}
				}
				else
				{
					this.SetDimensions(value, this.mHeight);
				}
			}
			this.UpdateWidget();
		}
	}

	// Token: 0x170000A7 RID: 167
	// (get) Token: 0x0600064D RID: 1613 RVA: 0x000091FA File Offset: 0x000073FA
	// (set) Token: 0x0600064E RID: 1614 RVA: 0x000447D8 File Offset: 0x000429D8
	public int height
	{
		get
		{
			return this.mHeight;
		}
		set
		{
			int minHeight = this.minHeight;
			if (value < minHeight)
			{
				value = minHeight;
			}
			if (this.mHeight != value && this.keepAspectRatio != UIWidget.AspectRatioSource.BasedOnWidth)
			{
				if (this.isAnchoredVertically)
				{
					if (this.bottomAnchor.target != null && this.topAnchor.target != null)
					{
						if (this.mPivot == UIWidget.Pivot.BottomLeft || this.mPivot == UIWidget.Pivot.Bottom || this.mPivot == UIWidget.Pivot.BottomRight)
						{
							NGUIMath.AdjustWidget(this, 0f, 0f, 0f, (float)(value - this.mHeight));
						}
						else if (this.mPivot == UIWidget.Pivot.TopLeft || this.mPivot == UIWidget.Pivot.Top || this.mPivot == UIWidget.Pivot.TopRight)
						{
							NGUIMath.AdjustWidget(this, 0f, (float)(this.mHeight - value), 0f, 0f);
						}
						else
						{
							int num = value - this.mHeight;
							num -= (num & 1);
							if (num != 0)
							{
								NGUIMath.AdjustWidget(this, 0f, (float)(-(float)num) * 0.5f, 0f, (float)num * 0.5f);
							}
						}
					}
					else if (this.bottomAnchor.target != null)
					{
						NGUIMath.AdjustWidget(this, 0f, 0f, 0f, (float)(value - this.mHeight));
					}
					else
					{
						NGUIMath.AdjustWidget(this, 0f, (float)(this.mHeight - value), 0f, 0f);
					}
				}
				else
				{
					this.SetDimensions(this.mWidth, value);
				}
			}
			this.UpdateWidget();
		}
	}

	// Token: 0x170000A8 RID: 168
	// (get) Token: 0x0600064F RID: 1615 RVA: 0x00009202 File Offset: 0x00007402
	// (set) Token: 0x06000650 RID: 1616 RVA: 0x0004497C File Offset: 0x00042B7C
	public Color color
	{
		get
		{
			return this.mColor;
		}
		set
		{
			if (this.mColor != value)
			{
				bool includeChildren = this.mColor.a != value.a;
				this.mColor = value;
				this.Invalidate(includeChildren);
				if (this.widgetAreStatic)
				{
					this.UpdateWidget();
				}
			}
		}
	}

	// Token: 0x06000651 RID: 1617 RVA: 0x000449D4 File Offset: 0x00042BD4
	public void SetColorNoAlpha(Color c)
	{
		if (this.mColor.r != c.r || this.mColor.g != c.g || this.mColor.b != c.b)
		{
			this.mColor.r = c.r;
			this.mColor.g = c.g;
			this.mColor.b = c.b;
			this.Invalidate(false);
		}
	}

	// Token: 0x170000A9 RID: 169
	// (get) Token: 0x06000652 RID: 1618 RVA: 0x0000920A File Offset: 0x0000740A
	// (set) Token: 0x06000653 RID: 1619 RVA: 0x00009217 File Offset: 0x00007417
	public override float alpha
	{
		get
		{
			return this.mColor.a;
		}
		set
		{
			if (this.mColor.a != value)
			{
				this.mColor.a = value;
				this.Invalidate(true);
				if (this.widgetAreStatic)
				{
					this.UpdateWidget();
				}
			}
		}
	}

	// Token: 0x170000AA RID: 170
	// (get) Token: 0x06000654 RID: 1620 RVA: 0x0000924E File Offset: 0x0000744E
	public bool isVisible
	{
		get
		{
			return this.mIsVisibleByPanel && this.mIsVisibleByAlpha && this.mIsInFront && this.finalAlpha > 0.001f && NGUITools.GetActive(this);
		}
	}

	// Token: 0x170000AB RID: 171
	// (get) Token: 0x06000655 RID: 1621 RVA: 0x0000928A File Offset: 0x0000748A
	public bool hasVertices
	{
		get
		{
			return this.geometry != null && this.geometry.hasVertices;
		}
	}

	// Token: 0x170000AC RID: 172
	// (get) Token: 0x06000656 RID: 1622 RVA: 0x000092A5 File Offset: 0x000074A5
	// (set) Token: 0x06000657 RID: 1623 RVA: 0x000092AD File Offset: 0x000074AD
	public UIWidget.Pivot rawPivot
	{
		get
		{
			return this.mPivot;
		}
		set
		{
			if (this.mPivot != value)
			{
				this.mPivot = value;
				if (this.autoResizeBoxCollider)
				{
					this.ResizeCollider();
				}
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x170000AD RID: 173
	// (get) Token: 0x06000658 RID: 1624 RVA: 0x000092A5 File Offset: 0x000074A5
	// (set) Token: 0x06000659 RID: 1625 RVA: 0x00044A64 File Offset: 0x00042C64
	public UIWidget.Pivot pivot
	{
		get
		{
			return this.mPivot;
		}
		set
		{
			if (this.mPivot != value)
			{
				Vector3 vector = this.worldCorners[0];
				this.mPivot = value;
				this.mChanged = true;
				Vector3 vector2 = this.worldCorners[0];
				Transform cachedTransform = base.cachedTransform;
				Vector3 vector3 = cachedTransform.position;
				float z = cachedTransform.localPosition.z;
				vector3.x += vector.x - vector2.x;
				vector3.y += vector.y - vector2.y;
				base.cachedTransform.position = vector3;
				vector3 = base.cachedTransform.localPosition;
				vector3.x = Mathf.Round(vector3.x);
				vector3.y = Mathf.Round(vector3.y);
				vector3.z = z;
				base.cachedTransform.localPosition = vector3;
			}
		}
	}

	// Token: 0x170000AE RID: 174
	// (get) Token: 0x0600065A RID: 1626 RVA: 0x000092D9 File Offset: 0x000074D9
	// (set) Token: 0x0600065B RID: 1627 RVA: 0x00044B5C File Offset: 0x00042D5C
	public int depth
	{
		get
		{
			return this.mDepth;
		}
		set
		{
			if (this.mDepth != value)
			{
				if (this.panel != null)
				{
					this.panel.RemoveWidget(this);
				}
				this.mDepth = value;
				if (this.panel != null)
				{
					this.panel.AddWidget(this);
					if (!Application.isPlaying)
					{
						this.panel.SortWidgets();
						this.panel.RebuildAllDrawCalls();
					}
				}
				base.cachedTransform.localPosition = new Vector3(base.cachedTransform.localPosition.x, base.cachedTransform.localPosition.y, (float)this.mDepth * -0.01f);
#if UNITY_EDITOR
                NGUITools.SetDirty(this);
#endif
            }
        }
	}

	// Token: 0x170000AF RID: 175
	// (get) Token: 0x0600065C RID: 1628 RVA: 0x00044C1C File Offset: 0x00042E1C
	public int raycastDepth
	{
		get
		{
			if (this.panel == null)
			{
				this.CreatePanel();
			}
			return (!(this.panel != null)) ? this.mDepth : (this.mDepth + this.panel.depth * 1000);
		}
	}

	// Token: 0x170000B0 RID: 176
	// (get) Token: 0x0600065D RID: 1629 RVA: 0x00044C78 File Offset: 0x00042E78
	public override Vector3[] localCorners
	{
		get
		{
			Vector2 pivotOffset = this.pivotOffset;
			float num = -pivotOffset.x * (float)this.mWidth;
			float num2 = -pivotOffset.y * (float)this.mHeight;
			float x = num + (float)this.mWidth;
			float y = num2 + (float)this.mHeight;
			this.mCorners[0] = new Vector3(num, num2);
			this.mCorners[1] = new Vector3(num, y);
			this.mCorners[2] = new Vector3(x, y);
			this.mCorners[3] = new Vector3(x, num2);
			return this.mCorners;
		}
	}

	// Token: 0x170000B1 RID: 177
	// (get) Token: 0x0600065E RID: 1630 RVA: 0x00044D2C File Offset: 0x00042F2C
	public virtual Vector2 localSize
	{
		get
		{
			Vector3[] localCorners = this.localCorners;
			return localCorners[2] - localCorners[0];
		}
	}

	// Token: 0x170000B2 RID: 178
	// (get) Token: 0x0600065F RID: 1631 RVA: 0x00044D64 File Offset: 0x00042F64
	public Vector3 localCenter
	{
		get
		{
			Vector3[] localCorners = this.localCorners;
			return Vector3.Lerp(localCorners[0], localCorners[2], 0.5f);
		}
	}

	// Token: 0x170000B3 RID: 179
	// (get) Token: 0x06000660 RID: 1632 RVA: 0x00044D9C File Offset: 0x00042F9C
	public override Vector3[] worldCorners
	{
		get
		{
			Vector2 pivotOffset = this.pivotOffset;
			float num = -pivotOffset.x * (float)this.mWidth;
			float num2 = -pivotOffset.y * (float)this.mHeight;
			float x = num + (float)this.mWidth;
			float y = num2 + (float)this.mHeight;
			Transform cachedTransform = base.cachedTransform;
			this.mCorners[0] = cachedTransform.TransformPoint(num, num2, 0f);
			this.mCorners[1] = cachedTransform.TransformPoint(num, y, 0f);
			this.mCorners[2] = cachedTransform.TransformPoint(x, y, 0f);
			this.mCorners[3] = cachedTransform.TransformPoint(x, num2, 0f);
			return this.mCorners;
		}
	}

	// Token: 0x170000B4 RID: 180
	// (get) Token: 0x06000661 RID: 1633 RVA: 0x000092E1 File Offset: 0x000074E1
	public Vector3 worldCenter
	{
		get
		{
			return base.cachedTransform.TransformPoint(this.localCenter);
		}
	}

	// Token: 0x170000B5 RID: 181
	// (get) Token: 0x06000662 RID: 1634 RVA: 0x00044E74 File Offset: 0x00043074
	public virtual Vector4 drawingDimensions
	{
		get
		{
			Vector2 pivotOffset = this.pivotOffset;
			float num = -pivotOffset.x * (float)this.mWidth;
			float num2 = -pivotOffset.y * (float)this.mHeight;
			float num3 = num + (float)this.mWidth;
			float num4 = num2 + (float)this.mHeight;
			return new Vector4((this.mDrawRegion.x != 0f) ? Mathf.Lerp(num, num3, this.mDrawRegion.x) : num, (this.mDrawRegion.y != 0f) ? Mathf.Lerp(num2, num4, this.mDrawRegion.y) : num2, (this.mDrawRegion.z != 1f) ? Mathf.Lerp(num, num3, this.mDrawRegion.z) : num3, (this.mDrawRegion.w != 1f) ? Mathf.Lerp(num2, num4, this.mDrawRegion.w) : num4);
		}
	}

	// Token: 0x170000B6 RID: 182
	// (get) Token: 0x06000663 RID: 1635 RVA: 0x000092F4 File Offset: 0x000074F4
	// (set) Token: 0x06000664 RID: 1636 RVA: 0x000092FC File Offset: 0x000074FC
	public virtual Material material
	{
		get
		{
			return this.mMat;
		}
		set
		{
			if (this.mMat != value)
			{
				this.RemoveFromPanel();
				this.mMat = value;
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x170000B7 RID: 183
	// (get) Token: 0x06000665 RID: 1637 RVA: 0x00044F7C File Offset: 0x0004317C
	// (set) Token: 0x06000666 RID: 1638 RVA: 0x00009322 File Offset: 0x00007522
	public virtual Texture mainTexture
	{
		get
		{
			Material material = this.material;
			return (!(material != null)) ? null : material.mainTexture;
		}
		set
		{
			throw new NotImplementedException(base.GetType() + " has no mainTexture setter");
		}
	}

	// Token: 0x170000B8 RID: 184
	// (get) Token: 0x06000667 RID: 1639 RVA: 0x00044FA8 File Offset: 0x000431A8
	// (set) Token: 0x06000668 RID: 1640 RVA: 0x00009339 File Offset: 0x00007539
	public virtual Shader shader
	{
		get
		{
			Material material = this.material;
			return (!(material != null)) ? null : material.shader;
		}
		set
		{
			throw new NotImplementedException(base.GetType() + " has no shader setter");
		}
	}

	// Token: 0x170000B9 RID: 185
	// (get) Token: 0x06000669 RID: 1641 RVA: 0x00009350 File Offset: 0x00007550
	[Obsolete("There is no relative scale anymore. Widgets now have width and height instead")]
	public Vector2 relativeSize
	{
		get
		{
			return Vector2.one;
		}
	}

	// Token: 0x170000BA RID: 186
	// (get) Token: 0x0600066A RID: 1642 RVA: 0x00044FD4 File Offset: 0x000431D4
	public bool hasBoxCollider
	{
		get
		{
			BoxCollider x = base.GetComponent<Collider>() as BoxCollider;
			return x != null || base.GetComponent<BoxCollider2D>() != null;
		}
	}

	// Token: 0x0600066B RID: 1643 RVA: 0x00045008 File Offset: 0x00043208
	public void SetDimensions(int w, int h)
	{
		if (this.mWidth != w || this.mHeight != h)
		{
			this.mWidth = w;
			this.mHeight = h;
			if (this.keepAspectRatio == UIWidget.AspectRatioSource.BasedOnWidth)
			{
				this.mHeight = Mathf.RoundToInt((float)this.mWidth / this.aspectRatio);
			}
			else if (this.keepAspectRatio == UIWidget.AspectRatioSource.BasedOnHeight)
			{
				this.mWidth = Mathf.RoundToInt((float)this.mHeight * this.aspectRatio);
			}
			else if (this.keepAspectRatio == UIWidget.AspectRatioSource.Free)
			{
				this.aspectRatio = (float)this.mWidth / (float)this.mHeight;
			}
			this.mMoved = true;
			if (this.autoResizeBoxCollider)
			{
				this.ResizeCollider();
			}
			this.MarkAsChanged();
		}
	}

	// Token: 0x0600066C RID: 1644 RVA: 0x000450D0 File Offset: 0x000432D0
	public override Vector3[] GetSides(Transform relativeTo)
	{
		Vector2 pivotOffset = this.pivotOffset;
		float num = -pivotOffset.x * (float)this.mWidth;
		float num2 = -pivotOffset.y * (float)this.mHeight;
		float num3 = num + (float)this.mWidth;
		float num4 = num2 + (float)this.mHeight;
		float x = (num + num3) * 0.5f;
		float y = (num2 + num4) * 0.5f;
		Transform cachedTransform = base.cachedTransform;
		this.mCorners[0] = cachedTransform.TransformPoint(num, y, 0f);
		this.mCorners[1] = cachedTransform.TransformPoint(x, num4, 0f);
		this.mCorners[2] = cachedTransform.TransformPoint(num3, y, 0f);
		this.mCorners[3] = cachedTransform.TransformPoint(x, num2, 0f);
		if (relativeTo != null)
		{
			for (int i = 0; i < 4; i++)
			{
				this.mCorners[i] = relativeTo.InverseTransformPoint(this.mCorners[i]);
			}
		}
		return this.mCorners;
	}

	// Token: 0x0600066D RID: 1645 RVA: 0x00009357 File Offset: 0x00007557
	public override float CalculateFinalAlpha(int frameID)
	{
#if UNITY_EDITOR
        if (mAlphaFrameID != frameID || !Application.isPlaying)
#else
		if (mAlphaFrameID != frameID)
#endif
        {
            mAlphaFrameID = frameID;
            UpdateFinalAlpha(frameID);
        }
        return finalAlpha;
    }

	// Token: 0x0600066E RID: 1646 RVA: 0x00045210 File Offset: 0x00043410
	protected void UpdateFinalAlpha(int frameID)
	{
		if (!this.mIsVisibleByAlpha || !this.mIsInFront)
		{
			this.finalAlpha = 0f;
		}
		else if (this.isCalculateFinalAlpha)
		{
			this.finalAlpha = ((!(base.parent != null)) ? this.mColor.a : (base.parent.CalculateFinalAlpha(frameID) * this.mColor.a));
		}
		else
		{
			this.finalAlpha = this.mColor.a;
		}
	}

	// Token: 0x0600066F RID: 1647 RVA: 0x000452A4 File Offset: 0x000434A4
	public override void Invalidate(bool includeChildren)
	{
		this.mChanged = true;
		this.mAlphaFrameID = -1;
		if (this.panel != null)
		{
			bool visibleByPanel = (!this.hideIfOffScreen && !this.panel.hasCumulativeClipping) || this.panel.IsVisible(this);
			this.UpdateVisibility(this.CalculateCumulativeAlpha(Time.frameCount) > 0.001f, visibleByPanel);
			this.UpdateFinalAlpha(Time.frameCount);
			if (includeChildren)
			{
				base.Invalidate(true);
			}
		}
	}

	// Token: 0x06000670 RID: 1648 RVA: 0x00045330 File Offset: 0x00043530
	public float CalculateCumulativeAlpha(int frameID)
	{
		UIRect parent = base.parent;
		return (!(parent != null)) ? this.mColor.a : (parent.CalculateFinalAlpha(frameID) * this.mColor.a);
	}

	// Token: 0x06000671 RID: 1649 RVA: 0x00045374 File Offset: 0x00043574
	public override void SetRect(float x, float y, float width, float height)
	{
		Vector2 pivotOffset = this.pivotOffset;
		float num = Mathf.Lerp(x, x + width, pivotOffset.x);
		float num2 = Mathf.Lerp(y, y + height, pivotOffset.y);
		int num3 = Mathf.FloorToInt(width + 0.5f);
		int num4 = Mathf.FloorToInt(height + 0.5f);
		if (pivotOffset.x == 0.5f)
		{
			num3 = num3 >> 1 << 1;
		}
		if (pivotOffset.y == 0.5f)
		{
			num4 = num4 >> 1 << 1;
		}
		Transform transform = base.cachedTransform;
		Vector3 localPosition = transform.localPosition;
		localPosition.x = Mathf.Floor(num + 0.5f);
		localPosition.y = Mathf.Floor(num2 + 0.5f);
		if (num3 < this.minWidth)
		{
			num3 = this.minWidth;
		}
		if (num4 < this.minHeight)
		{
			num4 = this.minHeight;
		}
		transform.localPosition = localPosition;
		this.width = num3;
		this.height = num4;
		if (base.isAnchored)
		{
			transform = transform.parent;
			if (this.leftAnchor.target)
			{
				this.leftAnchor.SetHorizontal(transform, x);
			}
			if (this.rightAnchor.target)
			{
				this.rightAnchor.SetHorizontal(transform, x + width);
			}
			if (this.bottomAnchor.target)
			{
				this.bottomAnchor.SetVertical(transform, y);
			}
			if (this.topAnchor.target)
			{
				this.topAnchor.SetVertical(transform, y + height);
			}
#if UNITY_EDITOR
            NGUITools.SetDirty(this);
#endif
        }
    }

	// Token: 0x06000672 RID: 1650 RVA: 0x00045514 File Offset: 0x00043714
	public void ResizeCollider()
	{
		BoxCollider component = base.GetComponent<BoxCollider>();
		if (component != null)
		{
			NGUITools.UpdateWidgetCollider(this, component);
		}
		else
		{
			NGUITools.UpdateWidgetCollider(this, base.GetComponent<BoxCollider2D>());
		}
	}

	// Token: 0x06000673 RID: 1651 RVA: 0x0004554C File Offset: 0x0004374C
	[DebuggerHidden]
	[DebuggerStepThrough]
	public static int FullCompareFunc(UIWidget left, UIWidget right)
	{
		int num = UIPanel.CompareFunc(left.panel, right.panel);
		return (num != 0) ? num : UIWidget.PanelCompareFunc(left, right);
	}

	// Token: 0x06000674 RID: 1652 RVA: 0x00045580 File Offset: 0x00043780
	[DebuggerStepThrough]
	[DebuggerHidden]
	public static int PanelCompareFunc(UIWidget left, UIWidget right)
	{
		if (left.mDepth < right.mDepth)
		{
			return -1;
		}
		if (left.mDepth > right.mDepth)
		{
			return 1;
		}
		Material material = left.material;
		Material material2 = right.material;
		if (material == material2)
		{
			return 0;
		}
		if (material == null)
		{
			return 1;
		}
		if (material2 == null)
		{
			return -1;
		}
		return (material.GetInstanceID() >= material2.GetInstanceID()) ? 1 : -1;
	}

	// Token: 0x06000675 RID: 1653 RVA: 0x00009379 File Offset: 0x00007579
	public Bounds CalculateBounds()
	{
		return this.CalculateBounds(null);
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x00045604 File Offset: 0x00043804
	public Bounds CalculateBounds(Transform relativeParent)
	{
		if (relativeParent == null)
		{
			Vector3[] localCorners = this.localCorners;
			Bounds result = new Bounds(localCorners[0], Vector3.zero);
			for (int i = 1; i < 4; i++)
			{
				result.Encapsulate(localCorners[i]);
			}
			return result;
		}
		Matrix4x4 worldToLocalMatrix = relativeParent.worldToLocalMatrix;
		Vector3[] worldCorners = this.worldCorners;
		Bounds result2 = new Bounds(worldToLocalMatrix.MultiplyPoint3x4(worldCorners[0]), Vector3.zero);
		for (int j = 1; j < 4; j++)
		{
			result2.Encapsulate(worldToLocalMatrix.MultiplyPoint3x4(worldCorners[j]));
		}
		return result2;
	}

	// Token: 0x06000677 RID: 1655 RVA: 0x000456C8 File Offset: 0x000438C8
	public void SetDirty()
	{
		if (this.drawCall != null)
		{
			this.drawCall.isDirty = true;
		}
		else if (this.isVisible && this.hasVertices)
		{
			this.CreatePanel();
		}
	}

	// Token: 0x06000678 RID: 1656 RVA: 0x00009382 File Offset: 0x00007582
	public void RemoveFromPanel()
	{
        if (panel != null)
        {
            panel.RemoveWidget(this);
            panel = null;
        }
        drawCall = null;
#if UNITY_EDITOR
        mOldTex = null;
        mOldShader = null;
#endif
    }


#if UNITY_EDITOR
    [System.NonSerialized] Texture mOldTex;
    [System.NonSerialized] Shader mOldShader;

    /// <summary>
    /// This callback is sent inside the editor notifying us that some property has changed.
    /// </summary>

    protected override void OnValidate()
    {
        if (NGUITools.GetActive(this))
        {
            base.OnValidate();

            // Prior to NGUI 2.7.0 width and height was specified as transform's local scale
            if ((mWidth == 100 || mWidth == minWidth) &&
                (mHeight == 100 || mHeight == minHeight) && cachedTransform.localScale.magnitude > 8f)
            {
                UpgradeFrom265();
                cachedTransform.localScale = Vector3.one;
            }

            if (mWidth < minWidth) mWidth = minWidth;
            if (mHeight < minHeight) mHeight = minHeight;
            if (autoResizeBoxCollider) ResizeCollider();

            // If the texture is changing, we need to make sure to rebuild the draw calls
            if (mOldTex != mainTexture || mOldShader != shader)
            {
                mOldTex = mainTexture;
                mOldShader = shader;
            }

            aspectRatio = (keepAspectRatio == AspectRatioSource.Free) ?
                (float)mWidth / mHeight : Mathf.Max(0.01f, aspectRatio);

            if (keepAspectRatio == AspectRatioSource.BasedOnHeight)
            {
                mWidth = Mathf.RoundToInt(mHeight * aspectRatio);
            }
            else if (keepAspectRatio == AspectRatioSource.BasedOnWidth)
            {
                mHeight = Mathf.RoundToInt(mWidth / aspectRatio);
            }

            if (!Application.isPlaying)
            {
                if (panel != null)
                {
                    panel.RemoveWidget(this);
                    panel = null;
                }
                CreatePanel();
            }
        }
        else
        {
            if (mWidth < minWidth) mWidth = minWidth;
            if (mHeight < minHeight) mHeight = minHeight;
        }
    }
#endif

    // Token: 0x06000679 RID: 1657 RVA: 0x00045714 File Offset: 0x00043914
    public virtual void MarkAsChanged()
	{
		if (NGUITools.GetActive(this))
		{
			this.mChanged = true;
#if UNITY_EDITOR
            NGUITools.SetDirty(this);
#endif
            if (this.panel != null && base.enabled && NGUITools.GetActive(base.gameObject) && !this.mPlayMode)
			{
				this.SetDirty();
				this.CheckLayer();
#if UNITY_EDITOR
                // Mark the panel as dirty so it gets updated
                if (material != null) NGUITools.SetDirty(panel.gameObject);
#endif
            }
        }
	}

	// Token: 0x0600067A RID: 1658 RVA: 0x00045778 File Offset: 0x00043978
	public UIPanel CreatePanel()
	{
		if (this.mStarted && this.panel == null && base.enabled && NGUITools.GetActive(base.cachedGameObject))
		{
			this.panel = UIPanel.Find(base.cachedTransform, true, base.cachedGameObject.layer);
			if (this.panel != null)
			{
				this.mParentFound = false;
				this.panel.AddWidget(this);
				this.CheckLayer();
				this.Invalidate(true);
			}
		}
		return this.panel;
	}

	// Token: 0x0600067B RID: 1659 RVA: 0x00045810 File Offset: 0x00043A10
	public void CheckLayer()
	{
		if (this.panel != null && this.panel.cachedGameObject.layer != base.cachedGameObject.layer)
		{
			UnityEngine.Debug.LogWarning("You can't place widgets on a layer different than the UIPanel that manages them.\nIf you want to move widgets to a different layer, parent them to a new panel instead.", this);
			base.cachedGameObject.layer = this.panel.cachedGameObject.layer;
		}
	}

	// Token: 0x0600067C RID: 1660 RVA: 0x00045874 File Offset: 0x00043A74
	public override void ParentHasChanged()
	{
		base.ParentHasChanged();
		if (this.panel != null)
		{
			UIPanel y = UIPanel.Find(base.cachedTransform, true, base.cachedGameObject.layer);
			if (this.panel != y)
			{
				this.RemoveFromPanel();
				this.CreatePanel();
			}
		}
	}

	// Token: 0x0600067D RID: 1661 RVA: 0x000093AF File Offset: 0x000075AF
	protected override void Awake()
	{
		base.Awake();
		this.mPlayMode = Application.isPlaying;
	}

	// Token: 0x0600067E RID: 1662 RVA: 0x000093C2 File Offset: 0x000075C2
	protected override void OnInit()
	{
		base.OnInit();
		this.RemoveFromPanel();
		this.mMoved = true;
		this.nUpdate(Time.frameCount);
#if UNITY_EDITOR
        NGUITools.SetDirty(this);
#endif
    }

    // Token: 0x0600067F RID: 1663 RVA: 0x000458D0 File Offset: 0x00043AD0
    protected virtual void UpgradeFrom265()
	{
		Vector3 localScale = base.cachedTransform.localScale;
		this.mWidth = Mathf.Abs(Mathf.RoundToInt(localScale.x));
		this.mHeight = Mathf.Abs(Mathf.RoundToInt(localScale.y));
		NGUITools.UpdateWidgetCollider(base.gameObject, true);
	}

	// Token: 0x06000680 RID: 1664 RVA: 0x00045924 File Offset: 0x00043B24
	protected override void OnStart()
	{
#if UNITY_EDITOR
        if (GetComponent<UIPanel>() != null)
        {
            UnityEngine.Debug.LogError("Widgets and panels should not be on the same object! Widget must be a child of the panel.", this);
        }
        else if (!Application.isPlaying && GetComponents<UIWidget>().Length > 1)
        {
            UnityEngine.Debug.LogError("You should not place more than one widget on the same object. Weird stuff will happen!", this);
        }
#endif
        base.cachedTransform.localPosition = new Vector3(base.cachedTransform.localPosition.x, base.cachedTransform.localPosition.y, (float)this.mDepth * -0.01f);
		this.CreatePanel();
	}

	// Token: 0x06000681 RID: 1665 RVA: 0x0004597C File Offset: 0x00043B7C
	protected override void OnAnchor()
	{
		nProfiler.BeginSample("UIWidget.OnAnchor");
		Transform cachedTransform = base.cachedTransform;
		Transform parent = cachedTransform.parent;
		Vector3 localPosition = cachedTransform.localPosition;
		Vector2 pivotOffset = this.pivotOffset;
		nProfiler.BeginSample("1");
		float num;
		float num2;
		float num3;
		float num4;
		if (this.leftAnchor.target == this.bottomAnchor.target && this.leftAnchor.target == this.rightAnchor.target && this.leftAnchor.target == this.topAnchor.target)
		{
			nProfiler.BeginSample("1");
			nProfiler.BeginSample("1");
			Vector3[] sides = this.leftAnchor.GetSides(parent);
			nProfiler.EndSample();
			if (sides != null)
			{
				nProfiler.BeginSample("2");
				num = NGUIMath.Lerp(sides[0].x, sides[2].x, this.leftAnchor.relative) + (float)this.leftAnchor.absolute;
				num2 = NGUIMath.Lerp(sides[0].x, sides[2].x, this.rightAnchor.relative) + (float)this.rightAnchor.absolute;
				num3 = NGUIMath.Lerp(sides[3].y, sides[1].y, this.bottomAnchor.relative) + (float)this.bottomAnchor.absolute;
				num4 = NGUIMath.Lerp(sides[3].y, sides[1].y, this.topAnchor.relative) + (float)this.topAnchor.absolute;
				this.mIsInFront = true;
				nProfiler.EndSample();
			}
			else
			{
				nProfiler.BeginSample("3");
				Vector3 localPos = base.GetLocalPos(this.leftAnchor, parent);
				num = localPos.x + (float)this.leftAnchor.absolute;
				num3 = localPos.y + (float)this.bottomAnchor.absolute;
				num2 = localPos.x + (float)this.rightAnchor.absolute;
				num4 = localPos.y + (float)this.topAnchor.absolute;
				this.mIsInFront = (!this.hideIfOffScreen || localPos.z >= 0f);
				nProfiler.EndSample();
			}
			nProfiler.EndSample();
		}
		else
		{
			this.mIsInFront = true;
			if (this.leftAnchor.target)
			{
				Vector3[] sides2 = this.leftAnchor.GetSides(parent);
				if (sides2 != null)
				{
					num = NGUIMath.Lerp(sides2[0].x, sides2[2].x, this.leftAnchor.relative) + (float)this.leftAnchor.absolute;
				}
				else
				{
					num = base.GetLocalPos(this.leftAnchor, parent).x + (float)this.leftAnchor.absolute;
				}
			}
			else
			{
				num = localPosition.x - pivotOffset.x * (float)this.mWidth;
			}
			if (this.rightAnchor.target)
			{
				Vector3[] sides3 = this.rightAnchor.GetSides(parent);
				if (sides3 != null)
				{
					num2 = NGUIMath.Lerp(sides3[0].x, sides3[2].x, this.rightAnchor.relative) + (float)this.rightAnchor.absolute;
				}
				else
				{
					num2 = base.GetLocalPos(this.rightAnchor, parent).x + (float)this.rightAnchor.absolute;
				}
			}
			else
			{
				num2 = localPosition.x - pivotOffset.x * (float)this.mWidth + (float)this.mWidth;
			}
			if (this.bottomAnchor.target)
			{
				Vector3[] sides4 = this.bottomAnchor.GetSides(parent);
				if (sides4 != null)
				{
					num3 = NGUIMath.Lerp(sides4[3].y, sides4[1].y, this.bottomAnchor.relative) + (float)this.bottomAnchor.absolute;
				}
				else
				{
					num3 = base.GetLocalPos(this.bottomAnchor, parent).y + (float)this.bottomAnchor.absolute;
				}
			}
			else
			{
				num3 = localPosition.y - pivotOffset.y * (float)this.mHeight;
			}
			if (this.topAnchor.target)
			{
				Vector3[] sides5 = this.topAnchor.GetSides(parent);
				if (sides5 != null)
				{
					num4 = NGUIMath.Lerp(sides5[3].y, sides5[1].y, this.topAnchor.relative) + (float)this.topAnchor.absolute;
				}
				else
				{
					num4 = base.GetLocalPos(this.topAnchor, parent).y + (float)this.topAnchor.absolute;
				}
			}
			else
			{
				num4 = localPosition.y - pivotOffset.y * (float)this.mHeight + (float)this.mHeight;
			}
		}
		nProfiler.EndSample();
		nProfiler.BeginSample("2");
		Vector3 vector = new Vector3(Mathf.Lerp(num, num2, pivotOffset.x), Mathf.Lerp(num3, num4, pivotOffset.y), localPosition.z);
		vector.x = Mathf.Round(vector.x);
		vector.y = Mathf.Round(vector.y);
		int num5 = Mathf.FloorToInt(num2 - num + 0.5f);
		int num6 = Mathf.FloorToInt(num4 - num3 + 0.5f);
		if (this.keepAspectRatio != UIWidget.AspectRatioSource.Free && this.aspectRatio != 0f)
		{
			if (this.keepAspectRatio == UIWidget.AspectRatioSource.BasedOnHeight)
			{
				num5 = Mathf.RoundToInt((float)num6 * this.aspectRatio);
			}
			else
			{
				num6 = Mathf.RoundToInt((float)num5 / this.aspectRatio);
			}
		}
		if (num5 < this.minWidth)
		{
			num5 = this.minWidth;
		}
		if (num6 < this.minHeight)
		{
			num6 = this.minHeight;
		}
		if (Vector3.SqrMagnitude(localPosition - vector) > 0.001f)
		{
			base.cachedTransform.localPosition = vector;
			if (this.mIsInFront)
			{
				this.mChanged = true;
			}
		}
		if (this.mWidth != num5 || this.mHeight != num6)
		{
			this.mWidth = num5;
			this.mHeight = num6;
			if (this.mIsInFront)
			{
				this.mChanged = true;
			}
			if (this.autoResizeBoxCollider)
			{
				this.ResizeCollider();
			}
		}
		nProfiler.EndSample();
		nProfiler.EndSample();
	}

	// Token: 0x06000682 RID: 1666 RVA: 0x000093E2 File Offset: 0x000075E2
	protected override void OnUpdate()
	{
		if (this.panel == null)
		{
			this.CreatePanel();
		}
#if UNITY_EDITOR
        else if (!mPlayMode) ParentHasChanged();
#endif
    }

    // Token: 0x06000683 RID: 1667 RVA: 0x000093FC File Offset: 0x000075FC
    private void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			this.MarkAsChanged();
		}
	}

	// Token: 0x06000684 RID: 1668 RVA: 0x0000940A File Offset: 0x0000760A
	protected override void OnDisable()
	{
		this.RemoveFromPanel();
		base.OnDisable();
	}

	// Token: 0x06000685 RID: 1669 RVA: 0x00009418 File Offset: 0x00007618
	private void OnDestroy()
	{
		this.RemoveFromPanel();
	}

#if UNITY_EDITOR
    static int mHandles = -1;

    /// <summary>
    /// Whether widgets will show handles with the Move Tool, or just the View Tool.
    /// </summary>

    static public bool showHandlesWithMoveTool
    {
        get
        {
            if (mHandles == -1)
            {
                mHandles = UnityEditor.EditorPrefs.GetInt("NGUI Handles", 1);
            }
            return (mHandles == 1);
        }
        set
        {
            int val = value ? 1 : 0;

            if (mHandles != val)
            {
                mHandles = val;
                UnityEditor.EditorPrefs.SetInt("NGUI Handles", mHandles);
            }
        }
    }

    /// <summary>
    /// Whether the widget should have some form of handles shown.
    /// </summary>

    static public bool showHandles
    {
        get
        {
#if UNITY_4_3 || UNITY_4_5
			if (showHandlesWithMoveTool)
			{
				return UnityEditor.Tools.current == UnityEditor.Tool.Move;
			}
			return UnityEditor.Tools.current == UnityEditor.Tool.View;
#else
            return UnityEditor.Tools.current == UnityEditor.Tool.Rect;
#endif
        }
    }

    /// <summary>
    /// Draw some selectable gizmos.
    /// </summary>

    void OnDrawGizmos()
    {
        if (isVisible && NGUITools.GetActive(this))
        {
            if (UnityEditor.Selection.activeGameObject == gameObject && showHandles) return;

            Color outline = new Color(1f, 1f, 1f, 0.2f);

            float adjustment = (root != null) ? 0.05f : 0.001f;
            Vector2 offset = pivotOffset;
            Vector3 center = new Vector3(mWidth * (0.5f - offset.x), mHeight * (0.5f - offset.y), -mDepth * adjustment);
            Vector3 size = new Vector3(mWidth, mHeight, 1f);

            // Draw the gizmo
            Gizmos.matrix = cachedTransform.localToWorldMatrix;
            Gizmos.color = (UnityEditor.Selection.activeGameObject == cachedTransform) ? Color.white : outline;
            Gizmos.DrawWireCube(center, size);

            // Make the widget selectable
            size.z = 0.01f;
            Gizmos.color = Color.clear;
            Gizmos.DrawCube(center, size);
        }
    }
#endif // UNITY_EDITOR

    // Token: 0x06000686 RID: 1670 RVA: 0x00009420 File Offset: 0x00007620
    public bool UpdateVisibility(bool visibleByAlpha, bool visibleByPanel)
	{
		if (this.mIsVisibleByAlpha != visibleByAlpha || this.mIsVisibleByPanel != visibleByPanel)
		{
			this.mChanged = true;
			this.mIsVisibleByAlpha = visibleByAlpha;
			this.mIsVisibleByPanel = visibleByPanel;
			return true;
		}
		return false;
	}

	// Token: 0x06000687 RID: 1671 RVA: 0x00046030 File Offset: 0x00044230
	[ContextMenu("Update Wigdet")]
	public void UpdateWidget()
	{
		if (!this.widgetUpdateFrame)
		{
			this.widgetUpdateFrame = true;
		}
		if (this.panel != null && this.panel.widgetAreStatic)
		{
			this.panel.widgetUpdateFrame = true;
		}
	}

	// Token: 0x06000688 RID: 1672 RVA: 0x00009452 File Offset: 0x00007652
	public bool UpdateTransform()
	{
		if (!base.isActiveAndEnabled)
		{
			return true;
		}
		this.mMoved = true;
		return this.UpdateTransform(Time.frameCount);
	}

	// Token: 0x06000689 RID: 1673 RVA: 0x0004607C File Offset: 0x0004427C
	public bool UpdateTransform(int frame)
	{
		this.mPlayMode = Application.isPlaying;
#if UNITY_EDITOR
        if (mMoved || !mPlayMode)
#else
		if (mMoved)
#endif
        {
            this.mMoved = true;
			this.mMatrixFrame = -1;
			base.cachedTransform.hasChanged = false;
			Vector2 pivotOffset = this.pivotOffset;
			float num = -pivotOffset.x * (float)this.mWidth;
			float num2 = -pivotOffset.y * (float)this.mHeight;
			float x = num + (float)this.mWidth;
			float y = num2 + (float)this.mHeight;
			this.mOldV0 = this.panel.worldToLocal.MultiplyPoint3x4(base.cachedTransform.TransformPoint(num, num2, 0f));
			this.mOldV1 = this.panel.worldToLocal.MultiplyPoint3x4(base.cachedTransform.TransformPoint(x, y, 0f));
		}
		else if (base.cachedTransform.hasChanged)
		{
			this.mMatrixFrame = -1;
			base.cachedTransform.hasChanged = false;
			Vector2 pivotOffset2 = this.pivotOffset;
			float num3 = -pivotOffset2.x * (float)this.mWidth;
			float num4 = -pivotOffset2.y * (float)this.mHeight;
			float x2 = num3 + (float)this.mWidth;
			float y2 = num4 + (float)this.mHeight;
			Vector3 b = this.panel.worldToLocal.MultiplyPoint3x4(base.cachedTransform.TransformPoint(num3, num4, 0f));
			Vector3 b2 = this.panel.worldToLocal.MultiplyPoint3x4(base.cachedTransform.TransformPoint(x2, y2, 0f));
			if (Vector3.SqrMagnitude(this.mOldV0 - b) > 1E-06f || Vector3.SqrMagnitude(this.mOldV1 - b2) > 1E-06f)
			{
				this.mMoved = true;
				this.mOldV0 = b;
				this.mOldV1 = b2;
			}
		}
		if (this.mMoved && this.onChange != null)
		{
			this.onChange();
		}
		return this.mMoved || this.mChanged;
	}

	// Token: 0x0600068A RID: 1674 RVA: 0x00046284 File Offset: 0x00044484
	public bool UpdateGeometry(int frame)
	{
		float num = this.CalculateFinalAlpha(frame);
		if (this.mIsVisibleByAlpha && this.mLastAlpha != num)
		{
			this.mChanged = true;
		}
		this.mLastAlpha = num;
		if (this.mChanged)
		{
			if (this.mIsVisibleByAlpha && num > 0.001f && this.shader != null)
			{
				bool hasVertices = this.geometry.hasVertices;
				if (this.fillGeometry)
				{
					this.geometry.Clear();
					this.OnFill(this.geometry.verts, this.geometry.uvs, this.geometry.cols);
				}
				if (this.geometry.hasVertices)
				{
					if (this.mMatrixFrame != frame)
					{
						this.mLocalToPanel = this.panel.worldToLocal * base.cachedTransform.localToWorldMatrix;
						this.mMatrixFrame = frame;
					}
					this.geometry.ApplyTransform(this.mLocalToPanel, this.panel.generateNormals);
					this.mMoved = false;
					this.mChanged = false;
					return true;
				}
				this.mChanged = false;
				return hasVertices;
			}
			else if (this.geometry.hasVertices)
			{
				if (this.fillGeometry)
				{
					this.geometry.Clear();
				}
				this.mMoved = false;
				this.mChanged = false;
				return true;
			}
		}
		else if (this.mMoved && this.geometry.hasVertices)
		{
			if (this.mMatrixFrame != frame)
			{
				this.mLocalToPanel = this.panel.worldToLocal * base.cachedTransform.localToWorldMatrix;
				this.mMatrixFrame = frame;
			}
			this.geometry.ApplyTransform(this.mLocalToPanel, this.panel.generateNormals);
			this.mMoved = false;
			this.mChanged = false;
			return true;
		}
		this.mMoved = false;
		this.mChanged = false;
		return false;
	}

	// Token: 0x0600068B RID: 1675 RVA: 0x00009473 File Offset: 0x00007673
	public void WriteToBuffers(List<Vector3> v, List<Vector2> u, List<Color> c, List<Vector3> n, List<Vector4> t, List<Vector4> u2)
	{
		this.geometry.WriteToBuffers(v, u, c, n, t, u2);
	}

	// Token: 0x0600068C RID: 1676 RVA: 0x00046478 File Offset: 0x00044678
	public virtual void MakePixelPerfect()
	{
		Vector3 localPosition = base.cachedTransform.localPosition;
		localPosition.z = Mathf.Round(localPosition.z);
		localPosition.x = Mathf.Round(localPosition.x);
		localPosition.y = Mathf.Round(localPosition.y);
		base.cachedTransform.localPosition = localPosition;
		Vector3 localScale = base.cachedTransform.localScale;
		base.cachedTransform.localScale = new Vector3(Mathf.Sign(localScale.x), Mathf.Sign(localScale.y), 1f);
	}

	// Token: 0x170000BB RID: 187
	// (get) Token: 0x0600068D RID: 1677 RVA: 0x00009489 File Offset: 0x00007689
	public virtual int minWidth
	{
		get
		{
			return 2;
		}
	}

	// Token: 0x170000BC RID: 188
	// (get) Token: 0x0600068E RID: 1678 RVA: 0x00009489 File Offset: 0x00007689
	public virtual int minHeight
	{
		get
		{
			return 2;
		}
	}

	// Token: 0x170000BD RID: 189
	// (get) Token: 0x0600068F RID: 1679 RVA: 0x0000948C File Offset: 0x0000768C
	// (set) Token: 0x06000690 RID: 1680 RVA: 0x0000574F File Offset: 0x0000394F
	public virtual Vector4 border
	{
		get
		{
			return Vector4.zero;
		}
		set
		{
		}
	}

	// Token: 0x06000691 RID: 1681 RVA: 0x0000574F File Offset: 0x0000394F
	public virtual void OnFill(List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
	{
	}

	// Token: 0x04000468 RID: 1128
	public bool isCalculateFinalAlpha;

	// Token: 0x04000469 RID: 1129
	[HideInInspector]
	[SerializeField]
	protected Color mColor = Color.white;

	// Token: 0x0400046A RID: 1130
	[SerializeField]
	[HideInInspector]
	protected UIWidget.Pivot mPivot = UIWidget.Pivot.Center;

	// Token: 0x0400046B RID: 1131
	[SerializeField]
	[HideInInspector]
	protected int mWidth = 100;

	// Token: 0x0400046C RID: 1132
	[HideInInspector]
	[SerializeField]
	protected int mHeight = 100;

	// Token: 0x0400046D RID: 1133
	[SerializeField]
	[HideInInspector]
	protected int mDepth;

	// Token: 0x0400046E RID: 1134
	[SerializeField]
	[Tooltip("Custom material, if desired")]
	[HideInInspector]
	protected Material mMat;

	// Token: 0x0400046F RID: 1135
	public UIWidget.OnDimensionsChanged onChange;

	// Token: 0x04000470 RID: 1136
	public UIWidget.OnPostFillCallback onPostFill;

	// Token: 0x04000471 RID: 1137
	public UIDrawCall.OnRenderCallback mOnRender;

	// Token: 0x04000472 RID: 1138
	public bool autoResizeBoxCollider;

	// Token: 0x04000473 RID: 1139
	public bool hideIfOffScreen;

	// Token: 0x04000474 RID: 1140
	public UIWidget.AspectRatioSource keepAspectRatio;

	// Token: 0x04000475 RID: 1141
	public float aspectRatio = 1f;

	// Token: 0x04000476 RID: 1142
	public UIWidget.HitCheck hitCheck;

	// Token: 0x04000477 RID: 1143
	[NonSerialized]
	public UIPanel panel;

	// Token: 0x04000478 RID: 1144
	[NonSerialized]
	public UIGeometry geometry = new UIGeometry();

	// Token: 0x04000479 RID: 1145
	[NonSerialized]
	public bool fillGeometry = true;

	// Token: 0x0400047A RID: 1146
	[NonSerialized]
	protected bool mPlayMode = true;

	// Token: 0x0400047B RID: 1147
	[NonSerialized]
	protected Vector4 mDrawRegion = new Vector4(0f, 0f, 1f, 1f);

	// Token: 0x0400047C RID: 1148
	[NonSerialized]
	private Matrix4x4 mLocalToPanel;

	// Token: 0x0400047D RID: 1149
	[NonSerialized]
	private bool mIsVisibleByAlpha = true;

	// Token: 0x0400047E RID: 1150
	[NonSerialized]
	private bool mIsVisibleByPanel = true;

	// Token: 0x0400047F RID: 1151
	[NonSerialized]
	private bool mIsInFront = true;

	// Token: 0x04000480 RID: 1152
	[NonSerialized]
	private float mLastAlpha;

	// Token: 0x04000481 RID: 1153
	[NonSerialized]
	private bool mMoved;

	// Token: 0x04000482 RID: 1154
	[NonSerialized]
	public UIDrawCall drawCall;

	// Token: 0x04000483 RID: 1155
	[NonSerialized]
	protected Vector3[] mCorners = new Vector3[4];

	// Token: 0x04000484 RID: 1156
	[NonSerialized]
	private int mAlphaFrameID = -1;

	// Token: 0x04000485 RID: 1157
	private int mMatrixFrame = -1;

	// Token: 0x04000486 RID: 1158
	private Vector3 mOldV0;

	// Token: 0x04000487 RID: 1159
	private Vector3 mOldV1;

	// Token: 0x020000C5 RID: 197
	[DoNotObfuscateNGUI]
	public enum Pivot
	{
		// Token: 0x04000489 RID: 1161
		TopLeft,
		// Token: 0x0400048A RID: 1162
		Top,
		// Token: 0x0400048B RID: 1163
		TopRight,
		// Token: 0x0400048C RID: 1164
		Left,
		// Token: 0x0400048D RID: 1165
		Center,
		// Token: 0x0400048E RID: 1166
		Right,
		// Token: 0x0400048F RID: 1167
		BottomLeft,
		// Token: 0x04000490 RID: 1168
		Bottom,
		// Token: 0x04000491 RID: 1169
		BottomRight
	}

	// Token: 0x020000C6 RID: 198
	[DoNotObfuscateNGUI]
	public enum AspectRatioSource
	{
		// Token: 0x04000493 RID: 1171
		Free,
		// Token: 0x04000494 RID: 1172
		BasedOnWidth,
		// Token: 0x04000495 RID: 1173
		BasedOnHeight
	}

	// Token: 0x020000C7 RID: 199
	// (Invoke) Token: 0x06000693 RID: 1683
	public delegate void OnDimensionsChanged();

	// Token: 0x020000C8 RID: 200
	// (Invoke) Token: 0x06000697 RID: 1687
	public delegate void OnPostFillCallback(UIWidget widget, int bufferOffset, List<Vector3> verts, List<Vector2> uvs, List<Color> cols);

	// Token: 0x020000C9 RID: 201
	// (Invoke) Token: 0x0600069B RID: 1691
	public delegate bool HitCheck(Vector3 worldPos);
}
