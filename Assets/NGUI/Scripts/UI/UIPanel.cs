using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000118 RID: 280
[AddComponentMenu("NGUI/UI/Panel")]
[ExecuteInEditMode]
public class UIPanel : UIRect
{
    public bool widgetsAreStatic = false;


    // Token: 0x17000196 RID: 406
    // (get) Token: 0x060009A8 RID: 2472 RVA: 0x0000B2A4 File Offset: 0x000094A4
    // (set) Token: 0x060009A9 RID: 2473 RVA: 0x0000B2AC File Offset: 0x000094AC
    public string sortingLayerName
	{
		get
		{
			return this.mSortingLayerName;
		}
		set
		{
			if (this.mSortingLayerName != value)
			{
				this.mSortingLayerName = value;
				this.UpdateDrawCalls(UIPanel.list.IndexOf(this));
			}
		}
	}

	// Token: 0x17000197 RID: 407
	// (get) Token: 0x060009AA RID: 2474 RVA: 0x00054A28 File Offset: 0x00052C28
	public static int nextUnusedDepth
	{
		get
		{
			int num = int.MinValue;
			int i = 0;
			int count = UIPanel.list.Count;
			while (i < count)
			{
				num = Mathf.Max(num, UIPanel.list[i].depth);
				i++;
			}
			return (num != int.MinValue) ? (num + 1) : 0;
		}
	}

	// Token: 0x17000198 RID: 408
	// (get) Token: 0x060009AB RID: 2475 RVA: 0x0000B2D7 File Offset: 0x000094D7
	public override bool canBeAnchored
	{
		get
		{
			return this.mClipping != UIDrawCall.Clipping.None;
		}
	}

	// Token: 0x17000199 RID: 409
	// (get) Token: 0x060009AC RID: 2476 RVA: 0x0000B2E5 File Offset: 0x000094E5
	// (set) Token: 0x060009AD RID: 2477 RVA: 0x00054A84 File Offset: 0x00052C84
	public override float alpha
	{
		get
		{
			return this.mAlpha;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (this.mAlpha != num)
			{
				bool flag = this.mAlpha > 0.001f;
				this.mAlphaFrameID = -1;
				this.mResized = true;
				this.mAlpha = num;
				int i = 0;
				int count = this.drawCalls.Count;
				while (i < count)
				{
					this.drawCalls[i].isDirty = true;
					i++;
				}
				this.Invalidate(!flag && this.mAlpha > 0.001f);
			}
		}
	}

	// Token: 0x1700019A RID: 410
	// (get) Token: 0x060009AE RID: 2478 RVA: 0x0000B2ED File Offset: 0x000094ED
	// (set) Token: 0x060009AF RID: 2479 RVA: 0x0000B2F5 File Offset: 0x000094F5
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
				this.mDepth = value;
#if UNITY_EDITOR
                NGUITools.SetDirty(this);
#endif
                UIPanel.list.Sort(new Comparison<UIPanel>(UIPanel.CompareFunc));
			}
		}
	}

	// Token: 0x1700019B RID: 411
	// (get) Token: 0x060009B0 RID: 2480 RVA: 0x0000B320 File Offset: 0x00009520
	// (set) Token: 0x060009B1 RID: 2481 RVA: 0x0000B328 File Offset: 0x00009528
	public int sortingOrder
	{
		get
		{
			return this.mSortingOrder;
		}
		set
		{
			if (this.mSortingOrder != value)
			{
				this.mSortingOrder = value;
#if UNITY_EDITOR
                NGUITools.SetDirty(this);
#endif
                this.UpdateDrawCalls(UIPanel.list.IndexOf(this));
			}
		}
	}

	// Token: 0x060009B2 RID: 2482 RVA: 0x00054B14 File Offset: 0x00052D14
	public static int CompareFunc(UIPanel a, UIPanel b)
	{
		if (!(a != b) || !(a != null) || !(b != null))
		{
			return 0;
		}
		if (a.mDepth < b.mDepth)
		{
			return -1;
		}
		if (a.mDepth > b.mDepth)
		{
			return 1;
		}
		return (a.GetInstanceID() >= b.GetInstanceID()) ? 1 : -1;
	}

	// Token: 0x1700019C RID: 412
	// (get) Token: 0x060009B3 RID: 2483 RVA: 0x00054B88 File Offset: 0x00052D88
	public float width
	{
		get
		{
			return this.GetViewSize().x;
		}
	}

	// Token: 0x1700019D RID: 413
	// (get) Token: 0x060009B4 RID: 2484 RVA: 0x00054BA4 File Offset: 0x00052DA4
	public float height
	{
		get
		{
			return this.GetViewSize().y;
		}
	}

	// Token: 0x1700019E RID: 414
	// (get) Token: 0x060009B5 RID: 2485 RVA: 0x0000B34E File Offset: 0x0000954E
	public bool halfPixelOffset
	{
		get
		{
			return this.mHalfPixelOffset;
		}
	}

	// Token: 0x1700019F RID: 415
	// (get) Token: 0x060009B6 RID: 2486 RVA: 0x0000B356 File Offset: 0x00009556
	public bool usedForUI
	{
		get
		{
			return base.anchorCamera != null && this.mCam.orthographic;
		}
	}

	// Token: 0x170001A0 RID: 416
	// (get) Token: 0x060009B7 RID: 2487 RVA: 0x00054BC0 File Offset: 0x00052DC0
	public Vector3 drawCallOffset
	{
		get
		{
			if (base.anchorCamera != null && this.mCam.orthographic)
			{
				Vector2 windowSize = this.GetWindowSize();
				float num = (!(base.root != null)) ? 1f : base.root.pixelSizeAdjustment;
				float num2 = num / windowSize.y / this.mCam.orthographicSize;
				bool flag = this.mHalfPixelOffset;
				bool flag2 = this.mHalfPixelOffset;
				if ((Mathf.RoundToInt(windowSize.x) & 1) == 1)
				{
					flag = !flag;
				}
				if ((Mathf.RoundToInt(windowSize.y) & 1) == 1)
				{
					flag2 = !flag2;
				}
				return new Vector3((!flag) ? 0f : (-num2), (!flag2) ? 0f : num2);
			}
			return Vector3.zero;
		}
	}

	// Token: 0x170001A1 RID: 417
	// (get) Token: 0x060009B8 RID: 2488 RVA: 0x0000B377 File Offset: 0x00009577
	// (set) Token: 0x060009B9 RID: 2489 RVA: 0x0000B37F File Offset: 0x0000957F
	public UIDrawCall.Clipping clipping
	{
		get
		{
			return this.mClipping;
		}
		set
		{
			if (this.mClipping != value)
			{
				this.mResized = true;
				this.mClipping = value;
				this.mMatrixFrame = -1;
#if UNITY_EDITOR
                if (!Application.isPlaying) UpdateDrawCalls();
#endif
            }
        }
	}

	// Token: 0x170001A2 RID: 418
	// (get) Token: 0x060009BA RID: 2490 RVA: 0x0000B3A2 File Offset: 0x000095A2
	public UIPanel parentPanel
	{
		get
		{
			return this.mParentPanel;
		}
	}

	// Token: 0x170001A3 RID: 419
	// (get) Token: 0x060009BB RID: 2491 RVA: 0x00054CA4 File Offset: 0x00052EA4
	public int clipCount
	{
		get
		{
			int num = 0;
			UIPanel uipanel = this;
			while (uipanel != null)
			{
				if (uipanel.mClipping == UIDrawCall.Clipping.SoftClip || uipanel.mClipping == UIDrawCall.Clipping.TextureMask)
				{
					num++;
				}
				uipanel = uipanel.mParentPanel;
			}
			return num;
		}
	}

	// Token: 0x170001A4 RID: 420
	// (get) Token: 0x060009BC RID: 2492 RVA: 0x0000B3AA File Offset: 0x000095AA
	public bool hasClipping
	{
		get
		{
			return this.mClipping == UIDrawCall.Clipping.SoftClip || this.mClipping == UIDrawCall.Clipping.TextureMask;
		}
	}

	// Token: 0x170001A5 RID: 421
	// (get) Token: 0x060009BD RID: 2493 RVA: 0x0000B3C4 File Offset: 0x000095C4
	public bool hasCumulativeClipping
	{
		get
		{
			return this.clipCount != 0;
		}
	}

	// Token: 0x170001A6 RID: 422
	// (get) Token: 0x060009BE RID: 2494 RVA: 0x0000B3D2 File Offset: 0x000095D2
	[Obsolete("Use 'hasClipping' or 'hasCumulativeClipping' instead")]
	public bool clipsChildren
	{
		get
		{
			return this.hasCumulativeClipping;
		}
	}

	// Token: 0x170001A7 RID: 423
	// (get) Token: 0x060009BF RID: 2495 RVA: 0x0000B3DA File Offset: 0x000095DA
	// (set) Token: 0x060009C0 RID: 2496 RVA: 0x00054CEC File Offset: 0x00052EEC
	public Vector2 clipOffset
	{
		get
		{
			return this.mClipOffset;
		}
		set
		{
			if (Mathf.Abs(this.mClipOffset.x - value.x) > 0.001f || Mathf.Abs(this.mClipOffset.y - value.y) > 0.001f)
			{
				this.mClipOffset = value;
				this.InvalidateClipping();
				if (this.onClipMove != null)
				{
					this.onClipMove(this);
				}
#if UNITY_EDITOR
                if (!Application.isPlaying) UpdateDrawCalls();
#endif
            }
        }
	}

	// Token: 0x060009C1 RID: 2497 RVA: 0x00054D64 File Offset: 0x00052F64
	private void InvalidateClipping()
	{
		this.mResized = true;
		this.mMatrixFrame = -1;
		int i = 0;
		int count = UIPanel.list.Count;
		while (i < count)
		{
			UIPanel uipanel = UIPanel.list[i];
			if (uipanel != this && uipanel.parentPanel == this)
			{
				uipanel.InvalidateClipping();
			}
			i++;
		}
	}

	// Token: 0x170001A8 RID: 424
	// (get) Token: 0x060009C2 RID: 2498 RVA: 0x0000B3E2 File Offset: 0x000095E2
	// (set) Token: 0x060009C3 RID: 2499 RVA: 0x0000B3EA File Offset: 0x000095EA
	public Texture2D clipTexture
	{
		get
		{
			return this.mClipTexture;
		}
		set
		{
			if (this.mClipTexture != value)
			{
				this.mClipTexture = value;
#if UNITY_EDITOR
                if (!Application.isPlaying) UpdateDrawCalls();
#endif
            }
        }
	}

	// Token: 0x170001A9 RID: 425
	// (get) Token: 0x060009C4 RID: 2500 RVA: 0x0000B404 File Offset: 0x00009604
	// (set) Token: 0x060009C5 RID: 2501 RVA: 0x0000B40C File Offset: 0x0000960C
	[Obsolete("Use 'finalClipRegion' or 'baseClipRegion' instead")]
	public Vector4 clipRange
	{
		get
		{
			return this.baseClipRegion;
		}
		set
		{
			this.baseClipRegion = value;
		}
	}

	// Token: 0x170001AA RID: 426
	// (get) Token: 0x060009C6 RID: 2502 RVA: 0x0000B415 File Offset: 0x00009615
	// (set) Token: 0x060009C7 RID: 2503 RVA: 0x00054DCC File Offset: 0x00052FCC
	public Vector4 baseClipRegion
	{
		get
		{
			return this.mClipRange;
		}
		set
		{
			if (Mathf.Abs(this.mClipRange.x - value.x) > 0.001f || Mathf.Abs(this.mClipRange.y - value.y) > 0.001f || Mathf.Abs(this.mClipRange.z - value.z) > 0.001f || Mathf.Abs(this.mClipRange.w - value.w) > 0.001f)
			{
				this.mResized = true;
				this.mClipRange = value;
				this.mMatrixFrame = -1;
				UIScrollView component = base.GetComponent<UIScrollView>();
				if (component != null)
				{
					component.UpdatePosition();
				}
				if (this.onClipMove != null)
				{
					this.onClipMove(this);
				}
#if UNITY_EDITOR
                if (!Application.isPlaying) UpdateDrawCalls();
#endif
            }
        }
	}

	// Token: 0x170001AB RID: 427
	// (get) Token: 0x060009C8 RID: 2504 RVA: 0x00054EA8 File Offset: 0x000530A8
	public Vector4 finalClipRegion
	{
		get
		{
			Vector2 viewSize = this.GetViewSize();
			if (this.mClipping != UIDrawCall.Clipping.None)
			{
				return new Vector4(this.mClipRange.x + this.mClipOffset.x, this.mClipRange.y + this.mClipOffset.y, viewSize.x, viewSize.y);
			}
			Vector4 result = new Vector4(0f, 0f, viewSize.x, viewSize.y);
			Vector3 vector = base.anchorCamera.WorldToScreenPoint(base.cachedTransform.position);
			vector.x -= viewSize.x * 0.5f;
			vector.y -= viewSize.y * 0.5f;
			result.x -= vector.x;
			result.y -= vector.y;
			return result;
		}
	}

	// Token: 0x170001AC RID: 428
	// (get) Token: 0x060009C9 RID: 2505 RVA: 0x0000B41D File Offset: 0x0000961D
	// (set) Token: 0x060009CA RID: 2506 RVA: 0x0000B425 File Offset: 0x00009625
	public Vector2 clipSoftness
	{
		get
		{
			return this.mClipSoftness;
		}
		set
		{
			if (this.mClipSoftness != value)
			{
				this.mClipSoftness = value;
#if UNITY_EDITOR
                if (!Application.isPlaying) UpdateDrawCalls();
#endif
            }
        }
	}

	// Token: 0x170001AD RID: 429
	// (get) Token: 0x060009CB RID: 2507 RVA: 0x00054FA0 File Offset: 0x000531A0
	public override Vector3[] localCorners
	{
		get
		{
			if (this.mClipping == UIDrawCall.Clipping.None)
			{
				Vector3[] worldCorners = this.worldCorners;
				Transform cachedTransform = base.cachedTransform;
				for (int i = 0; i < 4; i++)
				{
					worldCorners[i] = cachedTransform.InverseTransformPoint(worldCorners[i]);
				}
				return worldCorners;
			}
			float num = this.mClipOffset.x + this.mClipRange.x - 0.5f * this.mClipRange.z;
			float num2 = this.mClipOffset.y + this.mClipRange.y - 0.5f * this.mClipRange.w;
			float x = num + this.mClipRange.z;
			float y = num2 + this.mClipRange.w;
			UIPanel.mCorners[0] = new Vector3(num, num2);
			UIPanel.mCorners[1] = new Vector3(num, y);
			UIPanel.mCorners[2] = new Vector3(x, y);
			UIPanel.mCorners[3] = new Vector3(x, num2);
			return UIPanel.mCorners;
		}
	}

	// Token: 0x170001AE RID: 430
	// (get) Token: 0x060009CC RID: 2508 RVA: 0x000550D4 File Offset: 0x000532D4
	public override Vector3[] worldCorners
	{
		get
		{
			if (this.mClipping != UIDrawCall.Clipping.None)
			{
				float num = this.mClipOffset.x + this.mClipRange.x - 0.5f * this.mClipRange.z;
				float num2 = this.mClipOffset.y + this.mClipRange.y - 0.5f * this.mClipRange.w;
				float x = num + this.mClipRange.z;
				float y = num2 + this.mClipRange.w;
				Transform cachedTransform = base.cachedTransform;
				UIPanel.mCorners[0] = cachedTransform.TransformPoint(num, num2, 0f);
				UIPanel.mCorners[1] = cachedTransform.TransformPoint(num, y, 0f);
				UIPanel.mCorners[2] = cachedTransform.TransformPoint(x, y, 0f);
				UIPanel.mCorners[3] = cachedTransform.TransformPoint(x, num2, 0f);
			}
			else
			{
				if (base.anchorCamera != null)
				{
					return this.mCam.GetWorldCorners(base.cameraRayDistance);
				}
				Vector2 viewSize = this.GetViewSize();
				float num3 = -0.5f * viewSize.x;
				float num4 = -0.5f * viewSize.y;
				float x2 = num3 + viewSize.x;
				float y2 = num4 + viewSize.y;
				UIPanel.mCorners[0] = new Vector3(num3, num4);
				UIPanel.mCorners[1] = new Vector3(num3, y2);
				UIPanel.mCorners[2] = new Vector3(x2, y2);
				UIPanel.mCorners[3] = new Vector3(x2, num4);
				if (this.anchorOffset && (this.mCam == null || this.mCam.transform.parent != base.cachedTransform))
				{
					Vector3 position = base.cachedTransform.position;
					for (int i = 0; i < 4; i++)
					{
						UIPanel.mCorners[i] += position;
					}
				}
			}
			return UIPanel.mCorners;
		}
	}

	// Token: 0x060009CD RID: 2509 RVA: 0x0005532C File Offset: 0x0005352C
	public override Vector3[] GetSides(Transform relativeTo)
	{
		if (this.mClipping != UIDrawCall.Clipping.None)
		{
			float num = this.mClipOffset.x + this.mClipRange.x - 0.5f * this.mClipRange.z;
			float num2 = this.mClipOffset.y + this.mClipRange.y - 0.5f * this.mClipRange.w;
			float num3 = num + this.mClipRange.z;
			float num4 = num2 + this.mClipRange.w;
			float x = (num + num3) * 0.5f;
			float y = (num2 + num4) * 0.5f;
			Transform cachedTransform = base.cachedTransform;
			UIRect.mSides[0] = cachedTransform.TransformPoint(num, y, 0f);
			UIRect.mSides[1] = cachedTransform.TransformPoint(x, num4, 0f);
			UIRect.mSides[2] = cachedTransform.TransformPoint(num3, y, 0f);
			UIRect.mSides[3] = cachedTransform.TransformPoint(x, num2, 0f);
			if (relativeTo != null)
			{
				for (int i = 0; i < 4; i++)
				{
					UIRect.mSides[i] = relativeTo.InverseTransformPoint(UIRect.mSides[i]);
				}
			}
			return UIRect.mSides;
		}
		if (base.anchorCamera != null && this.anchorOffset)
		{
			Vector3[] sides = this.mCam.GetSides(base.cameraRayDistance);
			Vector3 position = base.cachedTransform.position;
			for (int j = 0; j < 4; j++)
			{
				sides[j] += position;
			}
			if (relativeTo != null)
			{
				for (int k = 0; k < 4; k++)
				{
					sides[k] = relativeTo.InverseTransformPoint(sides[k]);
				}
			}
			return sides;
		}
		return base.GetSides(relativeTo);
	}

	// Token: 0x060009CE RID: 2510 RVA: 0x0000B43F File Offset: 0x0000963F
	public override void Invalidate(bool includeChildren)
	{
		this.mAlphaFrameID = -1;
		base.Invalidate(includeChildren);
	}

	// Token: 0x060009CF RID: 2511 RVA: 0x0005555C File Offset: 0x0005375C
	public override float CalculateFinalAlpha(int frameID)
	{
#if UNITY_EDITOR
        if (mAlphaFrameID != frameID || !Application.isPlaying)
#else
		if (mAlphaFrameID != frameID)
#endif
        {
            this.mAlphaFrameID = frameID;
			UIRect parent = base.parent;
			this.finalAlpha = ((!(base.parent != null)) ? this.mAlpha : (parent.CalculateFinalAlpha(frameID) * this.mAlpha));
		}
		return this.finalAlpha;
	}

	// Token: 0x060009D0 RID: 2512 RVA: 0x000555BC File Offset: 0x000537BC
	public override void SetRect(float x, float y, float width, float height)
	{
		int num = Mathf.FloorToInt(width + 0.5f);
		int num2 = Mathf.FloorToInt(height + 0.5f);
		num = num >> 1 << 1;
		num2 = num2 >> 1 << 1;
		Transform transform = base.cachedTransform;
		Vector3 localPosition = transform.localPosition;
		localPosition.x = Mathf.Floor(x + 0.5f);
		localPosition.y = Mathf.Floor(y + 0.5f);
		if (num < 2)
		{
			num = 2;
		}
		if (num2 < 2)
		{
			num2 = 2;
		}
		this.baseClipRegion = new Vector4(localPosition.x, localPosition.y, (float)num, (float)num2);
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

	// Token: 0x060009D1 RID: 2513 RVA: 0x000556F4 File Offset: 0x000538F4
	public bool IsVisible(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
	{
		this.UpdateTransformMatrix();
		a = this.worldToLocal.MultiplyPoint3x4(a);
		b = this.worldToLocal.MultiplyPoint3x4(b);
		c = this.worldToLocal.MultiplyPoint3x4(c);
		d = this.worldToLocal.MultiplyPoint3x4(d);
		UIPanel.mTemp[0] = a.x;
		UIPanel.mTemp[1] = b.x;
		UIPanel.mTemp[2] = c.x;
		UIPanel.mTemp[3] = d.x;
		float num = Mathf.Min(UIPanel.mTemp);
		float num2 = Mathf.Max(UIPanel.mTemp);
		UIPanel.mTemp[0] = a.y;
		UIPanel.mTemp[1] = b.y;
		UIPanel.mTemp[2] = c.y;
		UIPanel.mTemp[3] = d.y;
		float num3 = Mathf.Min(UIPanel.mTemp);
		float num4 = Mathf.Max(UIPanel.mTemp);
		return num2 >= this.mMin.x && num4 >= this.mMin.y && num <= this.mMax.x && num3 <= this.mMax.y;
	}

	// Token: 0x060009D2 RID: 2514 RVA: 0x0005582C File Offset: 0x00053A2C
	public bool IsVisible(Vector3 worldPos)
	{
		if (this.mAlpha < 0.001f)
		{
			return false;
		}
		if (this.mClipping == UIDrawCall.Clipping.None || this.mClipping == UIDrawCall.Clipping.ConstrainButDontClip)
		{
			return true;
		}
		this.UpdateTransformMatrix();
		Vector3 vector = this.worldToLocal.MultiplyPoint3x4(worldPos);
		return vector.x >= this.mMin.x && vector.y >= this.mMin.y && vector.x <= this.mMax.x && vector.y <= this.mMax.y;
	}

	// Token: 0x060009D3 RID: 2515 RVA: 0x000558DC File Offset: 0x00053ADC
	public bool IsVisible(UIWidget w)
	{
		UIPanel uipanel = this;
		Vector3[] array = null;
		while (uipanel != null)
		{
			if ((uipanel.mClipping == UIDrawCall.Clipping.None || uipanel.mClipping == UIDrawCall.Clipping.ConstrainButDontClip) && !w.hideIfOffScreen)
			{
				uipanel = uipanel.mParentPanel;
			}
			else
			{
				if (array == null)
				{
					array = w.worldCorners;
				}
				if (!uipanel.IsVisible(array[0], array[1], array[2], array[3]))
				{
					return false;
				}
				uipanel = uipanel.mParentPanel;
			}
		}
		return true;
	}

	// Token: 0x060009D4 RID: 2516 RVA: 0x00055980 File Offset: 0x00053B80
	public bool Affects(UIWidget w)
	{
		if (w == null)
		{
			return false;
		}
		UIPanel panel = w.panel;
		if (panel == null)
		{
			return false;
		}
		UIPanel uipanel = this;
		while (uipanel != null)
		{
			if (uipanel == panel)
			{
				return true;
			}
			if (!uipanel.hasCumulativeClipping)
			{
				return false;
			}
			uipanel = uipanel.mParentPanel;
		}
		return false;
	}

	// Token: 0x060009D5 RID: 2517 RVA: 0x0000B44F File Offset: 0x0000964F
	[ContextMenu("Force Refresh")]
	public void RebuildAllDrawCalls()
	{
		this.mRebuild = true;
	}

	// Token: 0x060009D6 RID: 2518 RVA: 0x000559E8 File Offset: 0x00053BE8
	public void SetDirty()
	{
		int i = 0;
		int count = this.drawCalls.Count;
		while (i < count)
		{
			this.drawCalls[i].isDirty = true;
			i++;
		}
		this.Invalidate(true);
	}

	// Token: 0x060009D7 RID: 2519 RVA: 0x00055A2C File Offset: 0x00053C2C
	protected override void Awake()
	{
		base.Awake();
		this.mHalfPixelOffset = (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.XBOX360 || Application.platform == RuntimePlatform.WindowsEditor);
		if (this.mHalfPixelOffset && SystemInfo.graphicsDeviceVersion.Contains("Direct3D"))
		{
			this.mHalfPixelOffset = (SystemInfo.graphicsShaderLevel < 40);
		}
	}

	// Token: 0x060009D8 RID: 2520 RVA: 0x00055AA0 File Offset: 0x00053CA0
	private void FindParent()
	{
		Transform parent = base.cachedTransform.parent;
		this.mParentPanel = ((!(parent != null)) ? null : NGUITools.FindInParents<UIPanel>(parent.gameObject));
	}

	// Token: 0x060009D9 RID: 2521 RVA: 0x0000B458 File Offset: 0x00009658
	public override void ParentHasChanged()
	{
		base.ParentHasChanged();
		this.FindParent();
	}

	// Token: 0x060009DA RID: 2522 RVA: 0x0000B466 File Offset: 0x00009666
	protected override void OnStart()
	{
		this.mLayer = base.cachedGameObject.layer;
	}

	// Token: 0x060009DB RID: 2523 RVA: 0x0000B479 File Offset: 0x00009679
	protected override void OnEnable()
	{
		this.mRebuild = true;
		this.mAlphaFrameID = -1;
		this.mMatrixFrame = -1;
		this.OnStart();
		base.OnEnable();
		this.mMatrixFrame = -1;
	}

	// Token: 0x060009DC RID: 2524 RVA: 0x00055ADC File Offset: 0x00053CDC
	protected override void OnInit()
	{
		if (UIPanel.list.Contains(this))
		{
			return;
		}
		base.OnInit();
		this.FindParent();
		if (base.GetComponent<Rigidbody>() == null && this.mParentPanel == null)
		{
			UICamera uicamera = (!(base.anchorCamera != null)) ? null : this.mCam.GetComponent<UICamera>();
			if (uicamera != null && (uicamera.eventType == UICamera.EventType.UI_3D || uicamera.eventType == UICamera.EventType.World_3D))
			{
				Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
				rigidbody.isKinematic = true;
				rigidbody.useGravity = false;
			}
		}
		this.mRebuild = true;
		this.mAlphaFrameID = -1;
		this.mMatrixFrame = -1;
		UIPanel.list.Add(this);
		UIPanel.list.Sort(new Comparison<UIPanel>(UIPanel.CompareFunc));
	}

	// Token: 0x060009DD RID: 2525 RVA: 0x00055BC0 File Offset: 0x00053DC0
	protected override void OnDisable()
	{
		int i = 0;
		int count = this.drawCalls.Count;
		while (i < count)
		{
			UIDrawCall uidrawCall = this.drawCalls[i];
			if (uidrawCall != null)
			{
				UIDrawCall.Destroy(uidrawCall);
			}
			i++;
		}
		this.drawCalls.Clear();
		UIPanel.list.Remove(this);
		this.mAlphaFrameID = -1;
		this.mMatrixFrame = -1;
		if (UIPanel.list.Count == 0)
		{
			UIDrawCall.ReleaseAll();
			UIPanel.mUpdateFrame = -1;
		}
		base.OnDisable();
	}

	// Token: 0x060009DE RID: 2526 RVA: 0x00055C50 File Offset: 0x00053E50
	private void UpdateTransformMatrix()
	{
		nProfiler.BeginSample("UIPanel.UpdateTransformMatrix");
		int num = Time.frameCount;
		if (this.mHasMoved || this.mMatrixFrame != num)
		{
			this.mMatrixFrame = num + 120;
			this.worldToLocal = base.cachedTransform.worldToLocalMatrix;
			Vector2 vector = this.GetViewSize() * 0.5f;
			float num2 = this.mClipOffset.x + this.mClipRange.x;
			float num3 = this.mClipOffset.y + this.mClipRange.y;
			this.mMin.x = num2 - vector.x;
			this.mMin.y = num3 - vector.y;
			this.mMax.x = num2 + vector.x;
			this.mMax.y = num3 + vector.y;
		}
		nProfiler.EndSample();
	}

	// Token: 0x060009DF RID: 2527 RVA: 0x00055D38 File Offset: 0x00053F38
	protected override void OnAnchor()
	{
		if (this.mClipping == UIDrawCall.Clipping.None)
		{
			return;
		}
		Transform cachedTransform = base.cachedTransform;
		Transform parent = cachedTransform.parent;
		Vector2 viewSize = this.GetViewSize();
		Vector2 vector = cachedTransform.localPosition;
		float num;
		float num2;
		float num3;
		float num4;
		if (this.leftAnchor.target == this.bottomAnchor.target && this.leftAnchor.target == this.rightAnchor.target && this.leftAnchor.target == this.topAnchor.target)
		{
			Vector3[] sides = this.leftAnchor.GetSides(parent);
			if (sides != null)
			{
				num = NGUIMath.Lerp(sides[0].x, sides[2].x, this.leftAnchor.relative) + (float)this.leftAnchor.absolute;
				num2 = NGUIMath.Lerp(sides[0].x, sides[2].x, this.rightAnchor.relative) + (float)this.rightAnchor.absolute;
				num3 = NGUIMath.Lerp(sides[3].y, sides[1].y, this.bottomAnchor.relative) + (float)this.bottomAnchor.absolute;
				num4 = NGUIMath.Lerp(sides[3].y, sides[1].y, this.topAnchor.relative) + (float)this.topAnchor.absolute;
			}
			else
			{
				Vector2 vector2 = base.GetLocalPos(this.leftAnchor, parent);
				num = vector2.x + (float)this.leftAnchor.absolute;
				num3 = vector2.y + (float)this.bottomAnchor.absolute;
				num2 = vector2.x + (float)this.rightAnchor.absolute;
				num4 = vector2.y + (float)this.topAnchor.absolute;
			}
		}
		else
		{
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
				num = this.mClipRange.x - 0.5f * viewSize.x;
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
				num2 = this.mClipRange.x + 0.5f * viewSize.x;
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
				num3 = this.mClipRange.y - 0.5f * viewSize.y;
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
				num4 = this.mClipRange.y + 0.5f * viewSize.y;
			}
		}
		num -= vector.x + this.mClipOffset.x;
		num2 -= vector.x + this.mClipOffset.x;
		num3 -= vector.y + this.mClipOffset.y;
		num4 -= vector.y + this.mClipOffset.y;
		float x = Mathf.Lerp(num, num2, 0.5f);
		float y = Mathf.Lerp(num3, num4, 0.5f);
		float num5 = num2 - num;
		float num6 = num4 - num3;
		float num7 = Mathf.Max(2f, this.mClipSoftness.x);
		float num8 = Mathf.Max(2f, this.mClipSoftness.y);
		if (num5 < num7)
		{
			num5 = num7;
		}
		if (num6 < num8)
		{
			num6 = num8;
		}
		this.baseClipRegion = new Vector4(x, y, num5, num6);
	}

	// Token: 0x060009E0 RID: 2528 RVA: 0x000562E0 File Offset: 0x000544E0
	public override void nLateUpdate(int fc)
	{
		if (!this.mEnabled)
		{
			return;
		}
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying)
        {
            this.widgetUpdateFrame = true;
        }
#endif
        if (this.widgetAreStatic && !this.widgetUpdateFrame && Application.isPlaying)
		{
			return;
		}
		if (this.frameCount <= fc && UIPanel.mUpdateFrame != fc)
		{
			UIPanel.mUpdateFrame = fc;
			this.frameCount = UIPanel.mUpdateFrame + this.frameCountSkip;
			nProfiler.BeginSample("1");
			int i = 0;
			int count = UIPanel.list.Count;
			while (i < count)
			{
				UIPanel.list[i].UpdateSelf();
				i++;
			}
			nProfiler.EndSample();
			int num = 3000;
			nProfiler.BeginSample("2");
			int j = 0;
			int count2 = UIPanel.list.Count;
			while (j < count2)
			{
				UIPanel uipanel = UIPanel.list[j];
				if (uipanel.renderQueue == UIPanel.RenderQueue.Automatic)
				{
					nProfiler.BeginSample("1");
					uipanel.startingRenderQueue = num;
					uipanel.UpdateDrawCalls(j);
					num += uipanel.drawCalls.Count;
					nProfiler.EndSample();
				}
				else if (uipanel.renderQueue == UIPanel.RenderQueue.StartAt)
				{
					nProfiler.BeginSample("2");
					uipanel.UpdateDrawCalls(j);
					if (uipanel.drawCalls.Count != 0)
					{
						num = Mathf.Max(num, uipanel.startingRenderQueue + uipanel.drawCalls.Count);
					}
					nProfiler.EndSample();
				}
				else
				{
					nProfiler.BeginSample("3");
					uipanel.UpdateDrawCalls(j);
					if (uipanel.drawCalls.Count != 0)
					{
						num = Mathf.Max(num, uipanel.startingRenderQueue + 1);
					}
					nProfiler.EndSample();
				}
				j++;
			}
			nProfiler.EndSample();
		}
	}

	// Token: 0x060009E1 RID: 2529 RVA: 0x00056494 File Offset: 0x00054694
	private void UpdateSelf()
	{
		if (this.widgetAreStatic && !this.widgetUpdateFrame && Application.isPlaying)
		{
			return;
		}
		nProfiler.BeginSample("UIPanel.UpdateSelf");
		this.widgetUpdateFrame = false;
		this.mHasMoved = base.cachedTransform.hasChanged;
		this.UpdateTransformMatrix();
		this.UpdateWidgets();
		nProfiler.BeginSample("1");
		if (this.mRebuild)
		{
			nProfiler.BeginSample("1");
			this.mRebuild = false;
			this.FillAllDrawCalls();
			nProfiler.EndSample();
		}
		else
		{
			nProfiler.BeginSample("2");
			bool needsCulling = this.mCam == null || this.mCam.useOcclusionCulling;
			int i = 0;
			while (i < this.drawCalls.Count)
			{
				UIDrawCall uidrawCall = this.drawCalls[i];
				if (uidrawCall.isDirty && !this.FillDrawCall(uidrawCall, needsCulling))
				{
					UIDrawCall.Destroy(uidrawCall);
					this.drawCalls.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}
			nProfiler.EndSample();
		}
		nProfiler.EndSample();
		nProfiler.BeginSample("2");
		if (this.mUpdateScroll)
		{
			nProfiler.BeginSample("1");
			this.mUpdateScroll = false;
			UIScrollView component = base.GetComponent<UIScrollView>();
			if (component != null)
			{
				component.UpdateScrollbars();
			}
		}
		nProfiler.EndSample();
		nProfiler.BeginSample("3");
		if (this.mHasMoved)
		{
			this.mHasMoved = false;
			this.mTrans.hasChanged = false;
		}
		nProfiler.EndSample();
		nProfiler.EndSample();
	}

	// Token: 0x060009E2 RID: 2530 RVA: 0x0000B4A3 File Offset: 0x000096A3
	public void SortWidgets()
	{
		this.mSortWidgets = false;
		this.widgets.Sort(new Comparison<UIWidget>(UIWidget.PanelCompareFunc));
	}

	// Token: 0x060009E3 RID: 2531 RVA: 0x00056628 File Offset: 0x00054828
	private void FillAllDrawCalls()
	{
		for (int i = 0; i < this.drawCalls.Count; i++)
		{
			UIDrawCall.Destroy(this.drawCalls[i]);
		}
		this.drawCalls.Clear();
		Material material = null;
		Texture texture = null;
		Shader shader = null;
		UIDrawCall uidrawCall = null;
		int num = 0;
		bool needsBounds = this.mCam == null || this.mCam.useOcclusionCulling;
		if (this.mSortWidgets)
		{
			this.SortWidgets();
		}
		for (int j = 0; j < this.widgets.Count; j++)
		{
			UIWidget uiwidget = this.widgets[j];
			if (uiwidget.isVisible && uiwidget.hasVertices)
			{
				Material material2 = uiwidget.material;
				if (this.onCreateMaterial != null)
				{
					material2 = this.onCreateMaterial(uiwidget, material2);
				}
				Texture mainTexture = uiwidget.mainTexture;
				Shader shader2 = uiwidget.shader;
				if (material != material2 || texture != mainTexture || shader != shader2)
				{
					if (uidrawCall != null && uidrawCall.verts.Count != 0)
					{
						this.drawCalls.Add(uidrawCall);
						uidrawCall.UpdateGeometry(num, needsBounds);
						uidrawCall.onRender = this.mOnRender;
						this.mOnRender = null;
						num = 0;
						uidrawCall = null;
					}
					material = material2;
					texture = mainTexture;
					shader = shader2;
				}
				if (material != null || shader != null || texture != null)
				{
					if (uidrawCall == null)
					{
						bool flag = true;
						if (flag)
						{
							uidrawCall = UIDrawCall.Create(this, material, texture, shader);
							uidrawCall.depthStart = uiwidget.depth;
							uidrawCall.depthEnd = uidrawCall.depthStart;
							uidrawCall.panel = this;
							uidrawCall.onCreateDrawCall = this.onCreateDrawCall;
						}
					}
					else
					{
						int depth = uiwidget.depth;
						if (depth < uidrawCall.depthStart)
						{
							uidrawCall.depthStart = depth;
						}
						if (depth > uidrawCall.depthEnd)
						{
							uidrawCall.depthEnd = depth;
						}
					}
					uiwidget.drawCall = uidrawCall;
					if (uidrawCall != null)
					{
						num++;
						if (this.generateNormals)
						{
							uiwidget.WriteToBuffers(uidrawCall.verts, uidrawCall.uvs, uidrawCall.cols, uidrawCall.norms, uidrawCall.tans, (!this.generateUV2) ? null : uidrawCall.uv2);
						}
						else
						{
							uiwidget.WriteToBuffers(uidrawCall.verts, uidrawCall.uvs, uidrawCall.cols, null, null, (!this.generateUV2) ? null : uidrawCall.uv2);
						}
						if (uiwidget.mOnRender != null)
						{
							if (this.mOnRender == null)
							{
								this.mOnRender = uiwidget.mOnRender;
							}
							else
							{
								this.mOnRender = (UIDrawCall.OnRenderCallback)Delegate.Combine(this.mOnRender, uiwidget.mOnRender);
							}
						}
					}
				}
			}
			else
			{
				uiwidget.drawCall = null;
			}
		}
		if (uidrawCall != null && uidrawCall.verts.Count != 0)
		{
			this.drawCalls.Add(uidrawCall);
			uidrawCall.UpdateGeometry(num, needsBounds);
			uidrawCall.onRender = this.mOnRender;
			this.mOnRender = null;
		}
	}

	// Token: 0x060009E4 RID: 2532 RVA: 0x000569A0 File Offset: 0x00054BA0
	public bool FillDrawCall(UIDrawCall dc)
	{
		bool needsCulling = this.mCam == null || this.mCam.useOcclusionCulling;
		return this.FillDrawCall(dc, needsCulling);
	}

	// Token: 0x060009E5 RID: 2533 RVA: 0x000569D8 File Offset: 0x00054BD8
	public bool FillDrawCall(UIDrawCall dc, bool needsCulling)
	{
		nProfiler.BeginSample("UIPanel.FillDrawCall");
		if (dc != null)
		{
			dc.isDirty = false;
			int num = 0;
			int i = 0;
			while (i < this.widgets.Count)
			{
				UIWidget uiwidget = this.widgets[i];
				if (uiwidget == null)
				{
#if UNITY_EDITOR
                    Debug.LogError("This should never happen");
#endif
                    this.widgets.RemoveAt(i);
				}
				else
				{
					if (uiwidget.drawCall == dc)
					{
						if (uiwidget.isVisible && uiwidget.hasVertices)
						{
							num++;
							nProfiler.BeginSample("1");
							if (this.generateNormals)
							{
								nProfiler.BeginSample("generateNormals");
								uiwidget.WriteToBuffers(dc.verts, dc.uvs, dc.cols, dc.norms, dc.tans, (!this.generateUV2) ? null : dc.uv2);
								nProfiler.EndSample();
							}
							else
							{
								nProfiler.BeginSample("NogenerateNormals");
								uiwidget.WriteToBuffers(dc.verts, dc.uvs, dc.cols, null, null, (!this.generateUV2) ? null : dc.uv2);
								nProfiler.EndSample();
							}
							nProfiler.EndSample();
							nProfiler.BeginSample("2");
							if (uiwidget.mOnRender != null)
							{
								if (this.mOnRender == null)
								{
									this.mOnRender = uiwidget.mOnRender;
								}
								else
								{
									this.mOnRender = (UIDrawCall.OnRenderCallback)Delegate.Combine(this.mOnRender, uiwidget.mOnRender);
								}
							}
							nProfiler.EndSample();
						}
						else
						{
							uiwidget.drawCall = null;
						}
					}
					i++;
				}
			}
			if (dc.verts.Count != 0)
			{
				nProfiler.BeginSample("3");
				dc.UpdateGeometry(num, needsCulling);
				dc.onRender = this.mOnRender;
				this.mOnRender = null;
				nProfiler.EndSample();
				nProfiler.EndSample();
				return true;
			}
		}
		nProfiler.EndSample();
		return false;
	}

    private void UpdateDrawCalls()
    {
        UpdateDrawCalls(0);
    }

    // Token: 0x060009E6 RID: 2534 RVA: 0x00056BC4 File Offset: 0x00054DC4
    private void UpdateDrawCalls(int sortOrder)
	{
		Transform cachedTransform = base.cachedTransform;
		bool usedForUI = this.usedForUI;
		if (this.clipping != UIDrawCall.Clipping.None)
		{
			this.drawCallClipRange = this.finalClipRegion;
			this.drawCallClipRange.z = this.drawCallClipRange.z * 0.5f;
			this.drawCallClipRange.w = this.drawCallClipRange.w * 0.5f;
		}
		else
		{
			this.drawCallClipRange = Vector4.zero;
		}
		int width = Screen.width;
		int height = Screen.height;
		if (this.drawCallClipRange.z == 0f)
		{
			this.drawCallClipRange.z = (float)width * 0.5f;
		}
		if (this.drawCallClipRange.w == 0f)
		{
			this.drawCallClipRange.w = (float)height * 0.5f;
		}
		if (this.halfPixelOffset)
		{
			this.drawCallClipRange.x = this.drawCallClipRange.x - 0.5f;
			this.drawCallClipRange.y = this.drawCallClipRange.y + 0.5f;
		}
		Vector3 vector;
		if (usedForUI)
		{
			Transform parent = base.cachedTransform.parent;
			vector = base.cachedTransform.localPosition;
			if (this.clipping != UIDrawCall.Clipping.None)
			{
				vector.x = (float)Mathf.RoundToInt(vector.x);
				vector.y = (float)Mathf.RoundToInt(vector.y);
			}
			if (parent != null)
			{
				vector = parent.TransformPoint(vector);
			}
			vector += this.drawCallOffset;
		}
		else
		{
			vector = cachedTransform.position;
		}
		Quaternion rotation = cachedTransform.rotation;
		Vector3 lossyScale = cachedTransform.lossyScale;
		for (int i = 0; i < this.drawCalls.Count; i++)
		{
			UIDrawCall uidrawCall = this.drawCalls[i];
			Transform cachedTransform2 = uidrawCall.cachedTransform;
			cachedTransform2.position = vector;
			cachedTransform2.rotation = rotation;
			cachedTransform2.localScale = lossyScale;
			uidrawCall.renderQueue = ((this.renderQueue != UIPanel.RenderQueue.Explicit) ? (this.startingRenderQueue + i) : this.startingRenderQueue);
			uidrawCall.alwaysOnScreen = (this.alwaysOnScreen && (this.mClipping == UIDrawCall.Clipping.None || this.mClipping == UIDrawCall.Clipping.ConstrainButDontClip));
			uidrawCall.sortingOrder = ((!this.useSortingOrder) ? 0 : ((this.mSortingOrder != 0 || this.renderQueue != UIPanel.RenderQueue.Automatic) ? this.mSortingOrder : sortOrder));
			uidrawCall.sortingLayerName = ((!this.useSortingOrder) ? null : this.mSortingLayerName);
			uidrawCall.clipTexture = this.mClipTexture;
		}
	}

	// Token: 0x060009E7 RID: 2535 RVA: 0x00056E6C File Offset: 0x0005506C
	private void UpdateLayers()
	{
		nProfiler.BeginSample("UIPanel.UpdateLayers");
		if (this.mLayer != base.cachedGameObject.layer)
		{
			this.mLayer = this.mGo.layer;
			int i = 0;
			int count = this.widgets.Count;
			while (i < count)
			{
				UIWidget uiwidget = this.widgets[i];
				if (uiwidget && uiwidget.parent == this)
				{
					uiwidget.gameObject.layer = this.mLayer;
				}
				i++;
			}
			base.ResetAnchors();
			for (int j = 0; j < this.drawCalls.Count; j++)
			{
				this.drawCalls[j].gameObject.layer = this.mLayer;
			}
		}
		nProfiler.EndSample();
	}

	// Token: 0x060009E8 RID: 2536 RVA: 0x00056F48 File Offset: 0x00055148
	private void UpdateWidgets()
	{
		nProfiler.BeginSample("UIPanel.UpdateWidgets");
		bool flag = false;
		bool flag2 = false;
		bool hasCumulativeClipping = this.hasCumulativeClipping;
		nProfiler.BeginSample("1");
		if (!this.cullWhileDragging)
		{
			for (int i = 0; i < UIScrollView.list.size; i++)
			{
				UIScrollView uiscrollView = UIScrollView.list[i];
				if (uiscrollView.panel == this && uiscrollView.isDragging)
				{
					flag2 = true;
				}
			}
		}
		nProfiler.EndSample();
		nProfiler.BeginSample("2");
		if (this.mForced != flag2)
		{
			this.mForced = flag2;
			this.mResized = true;
		}
		nProfiler.EndSample();
		nProfiler.BeginSample("3");
		int num = Time.frameCount;
		int j = 0;
		int count = this.widgets.Count;
		while (j < count)
		{
			UIWidget uiwidget = this.widgets[j];
			if (uiwidget.panel == this && uiwidget.enabled)
			{
#if UNITY_EDITOR
                // When an object is dragged from Project view to Scene view, its Z is...
                // odd, to say the least. Force it if possible.
                if (!Application.isPlaying)
                {
                    Transform t = uiwidget.cachedTransform;

                    if (t.hideFlags != HideFlags.HideInHierarchy)
                    {
                        t = (t.parent != null && t.parent.hideFlags == HideFlags.HideInHierarchy) ?
                            t.parent : null;
                    }

                    if (t != null)
                    {
                        for (; ; )
                        {
                            if (t.parent == null) break;
                            if (t.parent.hideFlags == HideFlags.HideInHierarchy) t = t.parent;
                            else break;
                        }

                        if (t != null)
                        {
                            Vector3 pos = t.localPosition;
                            pos.x = Mathf.Round(pos.x);
                            pos.y = Mathf.Round(pos.y);
                            pos.z = 0f;

                            if (Vector3.SqrMagnitude(t.localPosition - pos) > 0.0001f)
                                t.localPosition = pos;
                        }
                    }
                }
#endif
                if (!uiwidget.widgetAreStatic || uiwidget.widgetUpdateFrame || !Application.isPlaying)
				{
					if (Application.isPlaying)
					{
						uiwidget.widgetUpdateFrame = false;
					}
					if (uiwidget.UpdateTransform(num) || this.mResized || (this.mHasMoved && !this.alwaysOnScreen))
					{
						bool visibleByAlpha = flag2 || uiwidget.CalculateCumulativeAlpha(num) > 0.001f;
						uiwidget.UpdateVisibility(visibleByAlpha, flag2 || this.alwaysOnScreen || (!hasCumulativeClipping && !uiwidget.hideIfOffScreen) || this.IsVisible(uiwidget));
					}
					if (uiwidget.UpdateGeometry(num))
					{
						flag = true;
						if (!this.mRebuild)
						{
							if (uiwidget.drawCall != null)
							{
								uiwidget.drawCall.isDirty = true;
							}
							else
							{
								this.FindDrawCall(uiwidget);
							}
						}
					}
				}
			}
			j++;
		}
		nProfiler.EndSample();
		nProfiler.BeginSample("4");
		if (flag && this.onGeometryUpdated != null)
		{
			this.onGeometryUpdated();
		}
		this.mResized = false;
		nProfiler.EndSample();
		nProfiler.EndSample();
	}

	// Token: 0x060009E9 RID: 2537 RVA: 0x0005719C File Offset: 0x0005539C
	public UIDrawCall FindDrawCall(UIWidget w)
	{
		Material material = w.material;
		Texture mainTexture = w.mainTexture;
		Shader shader = w.shader;
		int depth = w.depth;
		for (int i = 0; i < this.drawCalls.Count; i++)
		{
			UIDrawCall uidrawCall = this.drawCalls[i];
			int num = (i != 0) ? (this.drawCalls[i - 1].depthEnd + 1) : int.MinValue;
			int num2 = (i + 1 != this.drawCalls.Count) ? (this.drawCalls[i + 1].depthStart - 1) : int.MaxValue;
			if (num <= depth && num2 >= depth)
			{
				if (uidrawCall.baseMaterial == material && uidrawCall.shader == shader && uidrawCall.mainTexture == mainTexture)
				{
					if (w.isVisible)
					{
						w.drawCall = uidrawCall;
						if (w.hasVertices)
						{
							uidrawCall.isDirty = true;
						}
						return uidrawCall;
					}
				}
				else
				{
					this.mRebuild = true;
				}
				return null;
			}
		}
		this.mRebuild = true;
		return null;
	}

	// Token: 0x060009EA RID: 2538 RVA: 0x000572D8 File Offset: 0x000554D8
	public void AddWidget(UIWidget w)
	{
		this.mUpdateScroll = true;
		if (this.widgets.Count == 0)
		{
			this.widgets.Add(w);
		}
		else if (this.mSortWidgets)
		{
			this.widgets.Add(w);
			this.SortWidgets();
		}
		else if (UIWidget.PanelCompareFunc(w, this.widgets[0]) == -1)
		{
			this.widgets.Insert(0, w);
		}
		else
		{
			int i = this.widgets.Count;
			while (i > 0)
			{
				if (UIWidget.PanelCompareFunc(w, this.widgets[--i]) != -1)
				{
					this.widgets.Insert(i + 1, w);
					break;
				}
			}
		}
		this.FindDrawCall(w);
	}

	// Token: 0x060009EB RID: 2539 RVA: 0x000573B0 File Offset: 0x000555B0
	public void RemoveWidget(UIWidget w)
	{
		if (this.widgets.Remove(w) && w.drawCall != null)
		{
			int depth = w.depth;
			if (depth == w.drawCall.depthStart || depth == w.drawCall.depthEnd)
			{
				this.mRebuild = true;
			}
			w.drawCall.isDirty = true;
			w.drawCall = null;
		}
	}

	// Token: 0x060009EC RID: 2540 RVA: 0x0000B4C3 File Offset: 0x000096C3
	public void Refresh()
	{
		this.mRebuild = true;
		UIPanel.mUpdateFrame = -1;
		if (UIPanel.list.Count > 0)
		{
			UIPanel.list[0].nLateUpdate(Time.frameCount);
		}
	}

	// Token: 0x060009ED RID: 2541 RVA: 0x00057424 File Offset: 0x00055624
	public virtual Vector3 CalculateConstrainOffset(Vector2 min, Vector2 max)
	{
		Vector4 finalClipRegion = this.finalClipRegion;
		float num = finalClipRegion.z * 0.5f;
		float num2 = finalClipRegion.w * 0.5f;
		Vector2 minRect = new Vector2(min.x, min.y);
		Vector2 maxRect = new Vector2(max.x, max.y);
		Vector2 minArea = new Vector2(finalClipRegion.x - num, finalClipRegion.y - num2);
		Vector2 maxArea = new Vector2(finalClipRegion.x + num, finalClipRegion.y + num2);
		if (this.softBorderPadding && this.clipping == UIDrawCall.Clipping.SoftClip)
		{
			minArea.x += this.mClipSoftness.x;
			minArea.y += this.mClipSoftness.y;
			maxArea.x -= this.mClipSoftness.x;
			maxArea.y -= this.mClipSoftness.y;
		}
		return NGUIMath.ConstrainRect(minRect, maxRect, minArea, maxArea);
	}

	// Token: 0x060009EE RID: 2542 RVA: 0x0005753C File Offset: 0x0005573C
	public bool ConstrainTargetToBounds(Transform target, ref Bounds targetBounds, bool immediate)
	{
		Vector3 vector = targetBounds.min;
		Vector3 vector2 = targetBounds.max;
		float num = 1f;
		if (this.mClipping == UIDrawCall.Clipping.None)
		{
			UIRoot root = base.root;
			if (root != null)
			{
				num = root.pixelSizeAdjustment;
			}
		}
		if (num != 1f)
		{
			vector /= num;
			vector2 /= num;
		}
		Vector3 b = this.CalculateConstrainOffset(vector, vector2) * num;
		if (b.sqrMagnitude > 0f)
		{
			if (immediate)
			{
				target.localPosition += b;
				targetBounds.center += b;
				SpringPosition component = target.GetComponent<SpringPosition>();
				if (component != null)
				{
					component.enabled = false;
				}
			}
			else
			{
				SpringPosition springPosition = SpringPosition.Begin(target.gameObject, target.localPosition + b, 13f);
				springPosition.ignoreTimeScale = true;
				springPosition.worldSpace = false;
			}
			return true;
		}
		return false;
	}

	// Token: 0x060009EF RID: 2543 RVA: 0x00057648 File Offset: 0x00055848
	public bool ConstrainTargetToBounds(Transform target, bool immediate)
	{
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(base.cachedTransform, target);
		return this.ConstrainTargetToBounds(target, ref bounds, immediate);
	}

	// Token: 0x060009F0 RID: 2544 RVA: 0x0000B4F7 File Offset: 0x000096F7
	public static UIPanel Find(Transform trans)
	{
		return UIPanel.Find(trans, false, -1);
	}

	// Token: 0x060009F1 RID: 2545 RVA: 0x0000B501 File Offset: 0x00009701
	public static UIPanel Find(Transform trans, bool createIfMissing)
	{
		return UIPanel.Find(trans, createIfMissing, -1);
	}

	// Token: 0x060009F2 RID: 2546 RVA: 0x0005766C File Offset: 0x0005586C
	public static UIPanel Find(Transform trans, bool createIfMissing, int layer)
	{
		UIPanel uipanel = NGUITools.FindInParents<UIPanel>(trans);
		if (uipanel != null)
		{
			return uipanel;
		}
		while (trans.parent != null)
		{
			trans = trans.parent;
		}
		return (!createIfMissing) ? null : NGUITools.CreateUI(trans, false, layer);
	}

	// Token: 0x060009F3 RID: 2547 RVA: 0x000576C0 File Offset: 0x000558C0
	public Vector2 GetWindowSize()
	{
		UIRoot root = base.root;
		Vector2 vector = NGUITools.screenSize;
		if (root != null)
		{
			vector *= root.GetPixelSizeAdjustment(Mathf.RoundToInt(vector.y));
		}
		return vector;
	}

	// Token: 0x060009F4 RID: 2548 RVA: 0x00057700 File Offset: 0x00055900
	public Vector2 GetViewSize()
	{
		if (this.mClipping != UIDrawCall.Clipping.None)
		{
			return new Vector2(this.mClipRange.z, this.mClipRange.w);
		}
		return NGUITools.screenSize;
	}

#if UNITY_EDITOR
    /// <summary>
    /// Draw a visible pink outline for the clipped area.
    /// </summary>

    void OnDrawGizmos()
    {
        if (anchorCamera == null) return;

        bool clip = (mClipping != UIDrawCall.Clipping.None);
        Transform t = clip ? transform : mCam.transform;

        Vector3[] corners = worldCorners;
        for (int i = 0; i < 4; ++i) corners[i] = t.InverseTransformPoint(corners[i]);
        Vector3 pos = Vector3.Lerp(corners[0], corners[2], 0.5f);
        Vector3 size = corners[2] - corners[0];

        GameObject go = UnityEditor.Selection.activeGameObject;
        bool isUsingThisPanel = (go != null) && (NGUITools.FindInParents<UIPanel>(go) == this);
        bool isSelected = (UnityEditor.Selection.activeGameObject == gameObject);
        bool detailedView = (isSelected && isUsingThisPanel);
        bool detailedClipped = detailedView && mClipping == UIDrawCall.Clipping.SoftClip;

        Gizmos.matrix = t.localToWorldMatrix;

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6
		if (isUsingThisPanel && !clip && mCam.isOrthoGraphic)
#else
        if (isUsingThisPanel && !clip && mCam.orthographic)
#endif
        {
            UIRoot rt = root;

            if (rt != null && rt.scalingStyle != UIRoot.Scaling.Flexible)
            {
                float width = rt.manualWidth;
                float height = rt.manualHeight;

                float x0 = -0.5f * width;
                float y0 = -0.5f * height;
                float x1 = x0 + width;
                float y1 = y0 + height;

                corners[0] = new Vector3(x0, y0);
                corners[1] = new Vector3(x0, y1);
                corners[2] = new Vector3(x1, y1);
                corners[3] = new Vector3(x1, y0);

                Vector3 szPos = Vector3.Lerp(corners[0], corners[2], 0.5f);
                Vector3 szSize = corners[2] - corners[0];

                Gizmos.color = new Color(0f, 0.75f, 1f);
                Gizmos.DrawWireCube(szPos, szSize);
            }
        }
        Gizmos.color = (isUsingThisPanel && !detailedClipped) ? new Color(1f, 0f, 0.5f) : new Color(0.5f, 0f, 0.5f);
        Gizmos.DrawWireCube(pos, size);

        if (detailedView)
        {
            if (detailedClipped)
            {
                Gizmos.color = new Color(1f, 0f, 0.5f);
                size.x -= mClipSoftness.x * 2f;
                size.y -= mClipSoftness.y * 2f;
                Gizmos.DrawWireCube(pos, size);
            }
        }
    }
#endif // UNITY_EDITOR

    // Token: 0x040006AF RID: 1711
    public static List<UIPanel> list = new List<UIPanel>();

	// Token: 0x040006B0 RID: 1712
	public UIPanel.OnGeometryUpdated onGeometryUpdated;

	// Token: 0x040006B1 RID: 1713
	public bool showInPanelTool = true;

	// Token: 0x040006B2 RID: 1714
	public bool generateNormals;

	// Token: 0x040006B3 RID: 1715
	public bool generateUV2;

	// Token: 0x040006B4 RID: 1716
	public int frameCountSkip;

	// Token: 0x040006B5 RID: 1717
	public bool cullWhileDragging = true;

	// Token: 0x040006B6 RID: 1718
	public bool alwaysOnScreen;

	// Token: 0x040006B7 RID: 1719
	public bool anchorOffset;

	// Token: 0x040006B8 RID: 1720
	public bool softBorderPadding = true;

	// Token: 0x040006B9 RID: 1721
	public UIPanel.RenderQueue renderQueue;

	// Token: 0x040006BA RID: 1722
	public int startingRenderQueue = 3000;

	// Token: 0x040006BB RID: 1723
	[NonSerialized]
	public List<UIWidget> widgets = new List<UIWidget>();

	// Token: 0x040006BC RID: 1724
	[NonSerialized]
	public List<UIDrawCall> drawCalls = new List<UIDrawCall>();

	// Token: 0x040006BD RID: 1725
	[NonSerialized]
	public Matrix4x4 worldToLocal = Matrix4x4.identity;

	// Token: 0x040006BE RID: 1726
	[NonSerialized]
	public Vector4 drawCallClipRange = new Vector4(0f, 0f, 1f, 1f);

	// Token: 0x040006BF RID: 1727
	public UIPanel.OnClippingMoved onClipMove;

	// Token: 0x040006C0 RID: 1728
	public UIPanel.OnCreateMaterial onCreateMaterial;

	// Token: 0x040006C1 RID: 1729
	public UIDrawCall.OnCreateDrawCall onCreateDrawCall;

	// Token: 0x040006C2 RID: 1730
	[SerializeField]
	[HideInInspector]
	private Texture2D mClipTexture;

	// Token: 0x040006C3 RID: 1731
	[SerializeField]
	[HideInInspector]
	private float mAlpha = 1f;

	// Token: 0x040006C4 RID: 1732
	[HideInInspector]
	[SerializeField]
	private UIDrawCall.Clipping mClipping;

	// Token: 0x040006C5 RID: 1733
	[HideInInspector]
	[SerializeField]
	private Vector4 mClipRange = new Vector4(0f, 0f, 300f, 200f);

	// Token: 0x040006C6 RID: 1734
	[HideInInspector]
	[SerializeField]
	private Vector2 mClipSoftness = new Vector2(4f, 4f);

	// Token: 0x040006C7 RID: 1735
	[HideInInspector]
	[SerializeField]
	private int mDepth;

	// Token: 0x040006C8 RID: 1736
	[SerializeField]
	[HideInInspector]
	private int mSortingOrder;

	// Token: 0x040006C9 RID: 1737
	[SerializeField]
	[HideInInspector]
	private string mSortingLayerName;

	// Token: 0x040006CA RID: 1738
	private bool mRebuild;

	// Token: 0x040006CB RID: 1739
	private bool mResized;

	// Token: 0x040006CC RID: 1740
	[SerializeField]
	private Vector2 mClipOffset = Vector2.zero;

	// Token: 0x040006CD RID: 1741
	private int mMatrixFrame = -1;

	// Token: 0x040006CE RID: 1742
	private int mAlphaFrameID;

	// Token: 0x040006CF RID: 1743
	private int mLayer = -1;

	// Token: 0x040006D0 RID: 1744
	private static float[] mTemp = new float[4];

	// Token: 0x040006D1 RID: 1745
	private Vector2 mMin = Vector2.zero;

	// Token: 0x040006D2 RID: 1746
	private Vector2 mMax = Vector2.zero;

	// Token: 0x040006D3 RID: 1747
	private bool mHalfPixelOffset;

	// Token: 0x040006D4 RID: 1748
	private bool mSortWidgets;

	// Token: 0x040006D5 RID: 1749
	private bool mUpdateScroll;

	// Token: 0x040006D6 RID: 1750
	public bool useSortingOrder;

	// Token: 0x040006D7 RID: 1751
	private UIPanel mParentPanel;

	// Token: 0x040006D8 RID: 1752
	private static Vector3[] mCorners = new Vector3[4];

	// Token: 0x040006D9 RID: 1753
	private static int mUpdateFrame = -1;

	// Token: 0x040006DA RID: 1754
	private int frameCount = -1;

	// Token: 0x040006DB RID: 1755
	[NonSerialized]
	private bool mHasMoved;

	// Token: 0x040006DC RID: 1756
	private UIDrawCall.OnRenderCallback mOnRender;

	// Token: 0x040006DD RID: 1757
	private bool mForced;

	// Token: 0x02000119 RID: 281
	[DoNotObfuscateNGUI]
	public enum RenderQueue
	{
		// Token: 0x040006DF RID: 1759
		Automatic,
		// Token: 0x040006E0 RID: 1760
		StartAt,
		// Token: 0x040006E1 RID: 1761
		Explicit
	}

	// Token: 0x0200011A RID: 282
	// (Invoke) Token: 0x060009F6 RID: 2550
	public delegate void OnGeometryUpdated();

	// Token: 0x0200011B RID: 283
	// (Invoke) Token: 0x060009FA RID: 2554
	public delegate void OnClippingMoved(UIPanel panel);

	// Token: 0x0200011C RID: 284
	// (Invoke) Token: 0x060009FE RID: 2558
	public delegate Material OnCreateMaterial(UIWidget widget, Material mat);
}
