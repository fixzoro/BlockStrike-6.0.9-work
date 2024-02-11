using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000122 RID: 290
[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Sprite Collection")]
public class UISpriteCollection : UIBasicSprite
{
	// Token: 0x170001C8 RID: 456
	// (get) Token: 0x06000A43 RID: 2627 RVA: 0x00058A1C File Offset: 0x00056C1C
	// (set) Token: 0x06000A44 RID: 2628 RVA: 0x0000AAA1 File Offset: 0x00008CA1
	public override Texture mainTexture
	{
		get
		{
			Material material = null;
			INGUIAtlas atlas = this.atlas;
			if (atlas != null)
			{
				material = atlas.spriteMaterial;
			}
			return (!(material != null)) ? null : material.mainTexture;
		}
		set
		{
			base.mainTexture = value;
		}
	}

	// Token: 0x170001C9 RID: 457
	// (get) Token: 0x06000A45 RID: 2629 RVA: 0x00058A58 File Offset: 0x00056C58
	// (set) Token: 0x06000A46 RID: 2630 RVA: 0x0000AA98 File Offset: 0x00008C98
	public override Material material
	{
		get
		{
			Material material = base.material;
			if (material != null)
			{
				return material;
			}
			INGUIAtlas atlas = this.atlas;
			if (atlas != null)
			{
				return atlas.spriteMaterial;
			}
			return null;
		}
		set
		{
			base.material = value;
		}
	}

	// Token: 0x170001CA RID: 458
	// (get) Token: 0x06000A47 RID: 2631 RVA: 0x0000B7FC File Offset: 0x000099FC
	// (set) Token: 0x06000A48 RID: 2632 RVA: 0x0000B809 File Offset: 0x00009A09
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
				this.mSprites.Clear();
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x170001CB RID: 459
	// (get) Token: 0x06000A49 RID: 2633 RVA: 0x00058A90 File Offset: 0x00056C90
	public override float pixelSize
	{
		get
		{
			INGUIAtlas atlas = this.atlas;
			if (atlas != null)
			{
				return atlas.pixelSize;
			}
			return 1f;
		}
	}

	// Token: 0x170001CC RID: 460
	// (get) Token: 0x06000A4A RID: 2634 RVA: 0x00058AB8 File Offset: 0x00056CB8
	public override bool premultipliedAlpha
	{
		get
		{
			INGUIAtlas atlas = this.atlas;
			return atlas != null && atlas.premultipliedAlpha;
		}
	}

	// Token: 0x170001CD RID: 461
	// (get) Token: 0x06000A4B RID: 2635 RVA: 0x00058ADC File Offset: 0x00056CDC
	public override Vector4 border
	{
		get
		{
			if (this.mSprite == null)
			{
				return base.border;
			}
			return new Vector4((float)this.mSprite.borderLeft, (float)this.mSprite.borderBottom, (float)this.mSprite.borderRight, (float)this.mSprite.borderTop);
		}
	}

	// Token: 0x170001CE RID: 462
	// (get) Token: 0x06000A4C RID: 2636 RVA: 0x00058B30 File Offset: 0x00056D30
	protected override Vector4 padding
	{
		get
		{
			Vector4 result = new Vector4(0f, 0f, 0f, 0f);
			if (this.mSprite != null)
			{
				result.x = (float)this.mSprite.paddingLeft;
				result.y = (float)this.mSprite.paddingBottom;
				result.z = (float)this.mSprite.paddingRight;
				result.w = (float)this.mSprite.paddingTop;
			}
			return result;
		}
	}

	// Token: 0x06000A4D RID: 2637 RVA: 0x00058BB0 File Offset: 0x00056DB0
	public override void OnFill(List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
	{
		Texture mainTexture = this.mainTexture;
		if (mainTexture == null)
		{
			return;
		}
		int count = verts.Count;
		foreach (KeyValuePair<object, UISpriteCollection.Sprite> keyValuePair in this.mSprites)
		{
			UISpriteCollection.Sprite value = keyValuePair.Value;
			if (value.enabled)
			{
				this.mSprite = value.sprite;
				if (this.mSprite != null)
				{
					Color c = value.color;
					c.a = this.finalAlpha;
					if (c.a != 0f)
					{
						Rect rect = new Rect((float)this.mSprite.x, (float)this.mSprite.y, (float)this.mSprite.width, (float)this.mSprite.height);
						Rect rect2 = new Rect((float)(this.mSprite.x + this.mSprite.borderLeft), (float)(this.mSprite.y + this.mSprite.borderTop), (float)(this.mSprite.width - this.mSprite.borderLeft - this.mSprite.borderRight), (float)(this.mSprite.height - this.mSprite.borderBottom - this.mSprite.borderTop));
						this.mOuterUV = NGUIMath.ConvertToTexCoords(rect, mainTexture.width, mainTexture.height);
						this.mInnerUV = NGUIMath.ConvertToTexCoords(rect2, mainTexture.width, mainTexture.height);
						this.mFlip = value.flip;
						Vector4 drawingDimensions = value.GetDrawingDimensions(this.pixelSize);
						Vector4 drawingUVs = base.drawingUVs;
						if (this.premultipliedAlpha)
						{
							c = NGUITools.ApplyPMA(c);
						}
						int count2 = verts.Count;
						switch (value.type)
						{
						case UIBasicSprite.Type.Simple:
							base.SimpleFill(verts, uvs, cols, ref drawingDimensions, ref drawingUVs, ref c);
							break;
						case UIBasicSprite.Type.Sliced:
							base.SlicedFill(verts, uvs, cols, ref drawingDimensions, ref drawingUVs, ref c);
							break;
						case UIBasicSprite.Type.Tiled:
							base.TiledFill(verts, uvs, cols, ref drawingDimensions, ref c);
							break;
						case UIBasicSprite.Type.Filled:
							base.FilledFill(verts, uvs, cols, ref drawingDimensions, ref drawingUVs, ref c);
							break;
						case UIBasicSprite.Type.Advanced:
							base.AdvancedFill(verts, uvs, cols, ref drawingDimensions, ref drawingUVs, ref c);
							break;
						}
						if (value.rot != 0f)
						{
							float num = value.rot * 0.0174532924f;
							float f = num * 0.5f;
							float num2 = Mathf.Sin(f);
							float num3 = Mathf.Cos(f);
							float num4 = num2 * 2f;
							float num5 = num2 * num4;
							float num6 = num3 * num4;
							int i = count2;
							int count3 = verts.Count;
							while (i < count3)
							{
								Vector3 value2 = verts[i];
								value2 = new Vector3((1f - num5) * value2.x - num6 * value2.y, num6 * value2.x + (1f - num5) * value2.y, value2.z);
								value2.x += value.pos.x;
								value2.y += value.pos.y;
								verts[i] = value2;
								i++;
							}
						}
						else
						{
							int j = count2;
							int count4 = verts.Count;
							while (j < count4)
							{
								Vector3 value3 = verts[j];
								value3.x += value.pos.x;
								value3.y += value.pos.y;
								verts[j] = value3;
								j++;
							}
						}
					}
				}
			}
		}
		this.mSprite = null;
		if (this.onPostFill != null)
		{
			this.onPostFill(this, count, verts, uvs, cols);
		}
	}

	// Token: 0x06000A4E RID: 2638 RVA: 0x00058FD4 File Offset: 0x000571D4
	public void Add(object obj, string spriteName, Vector2 pos, float width, float height)
	{
		this.AddSprite(obj, spriteName, pos, width, height, new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), new Vector2(0.5f, 0.5f), 0f, UIBasicSprite.Type.Simple, UIBasicSprite.Flip.Nothing, true);
	}

	// Token: 0x06000A4F RID: 2639 RVA: 0x00059020 File Offset: 0x00057220
	public void Add(object obj, string spriteName, Vector2 pos, float width, float height, Color32 color)
	{
		this.AddSprite(obj, spriteName, pos, width, height, color, new Vector2(0.5f, 0.5f), 0f, UIBasicSprite.Type.Simple, UIBasicSprite.Flip.Nothing, true);
	}

	// Token: 0x06000A50 RID: 2640 RVA: 0x00059054 File Offset: 0x00057254
	public void AddSprite(object id, string spriteName, Vector2 pos, float width, float height, Color32 color, Vector2 pivot, float rot = 0f, UIBasicSprite.Type type = UIBasicSprite.Type.Simple, UIBasicSprite.Flip flip = UIBasicSprite.Flip.Nothing, bool enabled = true)
	{
		if (this.mAtlas == null)
		{
			Debug.LogError("Atlas must be assigned first");
			return;
		}
		UISpriteCollection.Sprite value = default(UISpriteCollection.Sprite);
		INGUIAtlas atlas = this.atlas;
		if (atlas != null)
		{
			value.sprite = atlas.GetSprite(spriteName);
		}
		if (value.sprite == null)
		{
			return;
		}
		value.pos = pos;
		value.rot = rot;
		value.width = width;
		value.height = height;
		value.color = color;
		value.pivot = pivot;
		value.type = type;
		value.flip = flip;
		value.enabled = enabled;
		this.mSprites[id] = value;
		if (enabled && !this.mChanged)
		{
			this.MarkAsChanged();
		}
	}

	// Token: 0x06000A51 RID: 2641 RVA: 0x00059124 File Offset: 0x00057324
	public UISpriteCollection.Sprite? GetSprite(object id)
	{
		UISpriteCollection.Sprite value;
		if (this.mSprites.TryGetValue(id, out value))
		{
			return new UISpriteCollection.Sprite?(value);
		}
		return null;
	}

	// Token: 0x06000A52 RID: 2642 RVA: 0x0000B83F File Offset: 0x00009A3F
	public bool RemoveSprite(object id)
	{
		if (this.mSprites.Remove(id))
		{
			if (!this.mChanged)
			{
				this.MarkAsChanged();
			}
			return true;
		}
		return false;
	}

	// Token: 0x06000A53 RID: 2643 RVA: 0x0000B866 File Offset: 0x00009A66
	public bool SetSprite(object id, UISpriteCollection.Sprite sp)
	{
		this.mSprites[id] = sp;
		if (!this.mChanged)
		{
			this.MarkAsChanged();
		}
		return true;
	}

	// Token: 0x06000A54 RID: 2644 RVA: 0x0000B887 File Offset: 0x00009A87
	[ContextMenu("Clear")]
	public void Clear()
	{
		if (this.mSprites.Count != 0)
		{
			this.mSprites.Clear();
			this.MarkAsChanged();
		}
	}

	// Token: 0x06000A55 RID: 2645 RVA: 0x00059154 File Offset: 0x00057354
	public bool IsActive(object id)
	{
		UISpriteCollection.Sprite sprite;
		return this.mSprites.TryGetValue(id, out sprite) && sprite.enabled;
	}

	// Token: 0x06000A56 RID: 2646 RVA: 0x00059180 File Offset: 0x00057380
	public bool SetActive(object id, bool visible)
	{
		UISpriteCollection.Sprite value;
		if (this.mSprites.TryGetValue(id, out value))
		{
			if (value.enabled != visible)
			{
				value.enabled = visible;
				this.mSprites[id] = value;
				if (!this.mChanged)
				{
					this.MarkAsChanged();
				}
			}
			return true;
		}
		return false;
	}

	// Token: 0x06000A57 RID: 2647 RVA: 0x000591D8 File Offset: 0x000573D8
	public bool SetPosition(object id, Vector2 pos, bool visible = true)
	{
		UISpriteCollection.Sprite value;
		if (this.mSprites.TryGetValue(id, out value))
		{
			if (value.pos != pos)
			{
				value.pos = pos;
				value.enabled = visible;
				this.mSprites[id] = value;
				if (!this.mChanged)
				{
					this.MarkAsChanged();
				}
			}
			else if (value.enabled != visible)
			{
				value.enabled = visible;
				this.mSprites[id] = value;
				if (!this.mChanged)
				{
					this.MarkAsChanged();
				}
			}
			return true;
		}
		return false;
	}

	// Token: 0x06000A58 RID: 2648 RVA: 0x00059274 File Offset: 0x00057474
	private static Vector2 Rotate(Vector2 pos, float rot)
	{
		float num = rot * 0.0174532924f;
		float f = num * 0.5f;
		float num2 = Mathf.Sin(f);
		float num3 = Mathf.Cos(f);
		float num4 = num2 * 2f;
		float num5 = num2 * num4;
		float num6 = num3 * num4;
		return new Vector2((1f - num5) * pos.x - num6 * pos.y, num6 * pos.x + (1f - num5) * pos.y);
	}

	// Token: 0x06000A59 RID: 2649 RVA: 0x0000B8AA File Offset: 0x00009AAA
	public object GetCurrentSpriteID()
	{
		return this.GetCurrentSpriteID(UICamera.lastWorldPosition);
	}

	// Token: 0x06000A5A RID: 2650 RVA: 0x0000B8B7 File Offset: 0x00009AB7
	public UISpriteCollection.Sprite? GetCurrentSprite()
	{
		return this.GetCurrentSprite(UICamera.lastWorldPosition);
	}

	// Token: 0x06000A5B RID: 2651 RVA: 0x000592F0 File Offset: 0x000574F0
	public object GetCurrentSpriteID(Vector3 worldPos)
	{
		Vector2 a = this.mTrans.InverseTransformPoint(worldPos);
		foreach (KeyValuePair<object, UISpriteCollection.Sprite> keyValuePair in this.mSprites)
		{
			UISpriteCollection.Sprite value = keyValuePair.Value;
			Vector2 pos = a - value.pos;
			if (value.rot != 0f)
			{
				pos = UISpriteCollection.Rotate(pos, -value.rot);
			}
			Vector4 drawingDimensions = value.GetDrawingDimensions(this.pixelSize);
			if (pos.x >= drawingDimensions.x)
			{
				if (pos.y >= drawingDimensions.y)
				{
					if (pos.x <= drawingDimensions.z)
					{
						if (pos.y <= drawingDimensions.w)
						{
							return keyValuePair.Key;
						}
					}
				}
			}
		}
		return null;
	}

	// Token: 0x06000A5C RID: 2652 RVA: 0x00059410 File Offset: 0x00057610
	public UISpriteCollection.Sprite? GetCurrentSprite(Vector3 worldPos)
	{
		Vector2 a = this.mTrans.InverseTransformPoint(worldPos);
		foreach (KeyValuePair<object, UISpriteCollection.Sprite> keyValuePair in this.mSprites)
		{
			UISpriteCollection.Sprite value = keyValuePair.Value;
			Vector2 pos = a - value.pos;
			if (value.rot != 0f)
			{
				pos = UISpriteCollection.Rotate(pos, -value.rot);
			}
			Vector4 drawingDimensions = value.GetDrawingDimensions(this.pixelSize);
			if (pos.x >= drawingDimensions.x)
			{
				if (pos.y >= drawingDimensions.y)
				{
					if (pos.x <= drawingDimensions.z)
					{
						if (pos.y <= drawingDimensions.w)
						{
							return new UISpriteCollection.Sprite?(keyValuePair.Value);
						}
					}
				}
			}
		}
		return null;
	}

	// Token: 0x06000A5D RID: 2653 RVA: 0x00059540 File Offset: 0x00057740
	protected void OnClick()
	{
		if (this.onClick != null)
		{
			object currentSpriteID = this.GetCurrentSpriteID();
			if (currentSpriteID != null)
			{
				this.onClick(currentSpriteID);
			}
		}
	}

	// Token: 0x06000A5E RID: 2654 RVA: 0x00059574 File Offset: 0x00057774
	protected void OnPress(bool isPressed)
	{
		if (this.onPress != null)
		{
			if (isPressed && this.mLastPress != null)
			{
				return;
			}
			if (isPressed)
			{
				this.mLastPress = this.GetCurrentSpriteID();
				if (this.mLastPress != null)
				{
					this.onPress(this.mLastPress, true);
				}
			}
			else if (this.mLastPress != null)
			{
				this.onPress(this.mLastPress, false);
				this.mLastPress = null;
			}
		}
	}

	// Token: 0x06000A5F RID: 2655 RVA: 0x000595F8 File Offset: 0x000577F8
	protected void OnHover(bool isOver)
	{
		if (this.onHover != null)
		{
			if (isOver)
			{
				UICamera.onMouseMove = (UICamera.MoveDelegate)Delegate.Combine(UICamera.onMouseMove, new UICamera.MoveDelegate(this.OnMove));
				this.OnMove(Vector2.zero);
			}
			else
			{
				UICamera.onMouseMove = (UICamera.MoveDelegate)Delegate.Remove(UICamera.onMouseMove, new UICamera.MoveDelegate(this.OnMove));
			}
		}
	}

	// Token: 0x06000A60 RID: 2656 RVA: 0x00059668 File Offset: 0x00057868
	protected void OnMove(Vector2 delta)
	{
		if (!this || this.onHover == null)
		{
			return;
		}
		object currentSpriteID = this.GetCurrentSpriteID();
		if (this.mLastHover != currentSpriteID)
		{
			if (this.mLastHover != null)
			{
				this.onHover(this.mLastHover, false);
			}
			this.mLastHover = currentSpriteID;
			if (this.mLastHover != null)
			{
				this.onHover(this.mLastHover, true);
			}
		}
	}

	// Token: 0x06000A61 RID: 2657 RVA: 0x0000B8C4 File Offset: 0x00009AC4
	protected void OnDrag(Vector2 delta)
	{
		if (this.onDrag != null && this.mLastPress != null)
		{
			this.onDrag(this.mLastPress, delta);
		}
	}

	// Token: 0x06000A62 RID: 2658 RVA: 0x000596E0 File Offset: 0x000578E0
	protected void OnTooltip(bool show)
	{
		if (this.onTooltip != null)
		{
			if (show)
			{
				if (this.mLastTooltip != null)
				{
					this.onTooltip(this.mLastTooltip, false);
				}
				this.mLastTooltip = this.GetCurrentSpriteID();
				if (this.mLastTooltip != null)
				{
					this.onTooltip(this.mLastTooltip, true);
				}
			}
			else
			{
				this.onTooltip(this.mLastTooltip, false);
				this.mLastTooltip = null;
			}
		}
	}

	// Token: 0x04000706 RID: 1798
	[HideInInspector]
	[SerializeField]
	private UnityEngine.Object mAtlas;

	// Token: 0x04000707 RID: 1799
	[NonSerialized]
	private Dictionary<object, UISpriteCollection.Sprite> mSprites = new Dictionary<object, UISpriteCollection.Sprite>();

	// Token: 0x04000708 RID: 1800
	[NonSerialized]
	private UISpriteData mSprite;

	// Token: 0x04000709 RID: 1801
	public UISpriteCollection.OnHoverCB onHover;

	// Token: 0x0400070A RID: 1802
	public UISpriteCollection.OnPressCB onPress;

	// Token: 0x0400070B RID: 1803
	public UISpriteCollection.OnClickCB onClick;

	// Token: 0x0400070C RID: 1804
	public UISpriteCollection.OnDragCB onDrag;

	// Token: 0x0400070D RID: 1805
	public UISpriteCollection.OnTooltipCB onTooltip;

	// Token: 0x0400070E RID: 1806
	[NonSerialized]
	private object mLastHover;

	// Token: 0x0400070F RID: 1807
	[NonSerialized]
	private object mLastPress;

	// Token: 0x04000710 RID: 1808
	[NonSerialized]
	private object mLastTooltip;

	// Token: 0x02000123 RID: 291
	public struct Sprite
	{
		// Token: 0x06000A63 RID: 2659 RVA: 0x00059764 File Offset: 0x00057964
		public Vector4 GetDrawingDimensions(float pixelSize)
		{
			float num = -this.pivot.x * this.width;
			float num2 = -this.pivot.y * this.height;
			float num3 = num + this.width;
			float num4 = num2 + this.height;
			if (this.sprite != null && this.type != UIBasicSprite.Type.Tiled)
			{
				int num5 = this.sprite.paddingLeft;
				int num6 = this.sprite.paddingBottom;
				int num7 = this.sprite.paddingRight;
				int num8 = this.sprite.paddingTop;
				if (this.type != UIBasicSprite.Type.Simple && pixelSize != 1f)
				{
					num5 = Mathf.RoundToInt(pixelSize * (float)num5);
					num6 = Mathf.RoundToInt(pixelSize * (float)num6);
					num7 = Mathf.RoundToInt(pixelSize * (float)num7);
					num8 = Mathf.RoundToInt(pixelSize * (float)num8);
				}
				int num9 = this.sprite.width + num5 + num7;
				int num10 = this.sprite.height + num6 + num8;
				float num11 = 1f;
				float num12 = 1f;
				if (num9 > 0 && num10 > 0 && (this.type == UIBasicSprite.Type.Simple || this.type == UIBasicSprite.Type.Filled))
				{
					if ((num9 & 1) != 0)
					{
						num7++;
					}
					if ((num10 & 1) != 0)
					{
						num8++;
					}
					num11 = 1f / (float)num9 * this.width;
					num12 = 1f / (float)num10 * this.height;
				}
				if (this.flip == UIBasicSprite.Flip.Horizontally || this.flip == UIBasicSprite.Flip.Both)
				{
					num += (float)num7 * num11;
					num3 -= (float)num5 * num11;
				}
				else
				{
					num += (float)num5 * num11;
					num3 -= (float)num7 * num11;
				}
				if (this.flip == UIBasicSprite.Flip.Vertically || this.flip == UIBasicSprite.Flip.Both)
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
			return new Vector4(num, num2, num3, num4);
		}

		// Token: 0x04000711 RID: 1809
		public UISpriteData sprite;

		// Token: 0x04000712 RID: 1810
		public Vector2 pos;

		// Token: 0x04000713 RID: 1811
		public float rot;

		// Token: 0x04000714 RID: 1812
		public float width;

		// Token: 0x04000715 RID: 1813
		public float height;

		// Token: 0x04000716 RID: 1814
		public Color32 color;

		// Token: 0x04000717 RID: 1815
		public Vector2 pivot;

		// Token: 0x04000718 RID: 1816
		public UIBasicSprite.Type type;

		// Token: 0x04000719 RID: 1817
		public UIBasicSprite.Flip flip;

		// Token: 0x0400071A RID: 1818
		public bool enabled;
	}

	// Token: 0x02000124 RID: 292
	// (Invoke) Token: 0x06000A65 RID: 2661
	public delegate void OnHoverCB(object obj, bool isOver);

	// Token: 0x02000125 RID: 293
	// (Invoke) Token: 0x06000A69 RID: 2665
	public delegate void OnPressCB(object obj, bool isPressed);

	// Token: 0x02000126 RID: 294
	// (Invoke) Token: 0x06000A6D RID: 2669
	public delegate void OnClickCB(object obj);

	// Token: 0x02000127 RID: 295
	// (Invoke) Token: 0x06000A71 RID: 2673
	public delegate void OnDragCB(object obj, Vector2 delta);

	// Token: 0x02000128 RID: 296
	// (Invoke) Token: 0x06000A75 RID: 2677
	public delegate void OnTooltipCB(object obj, bool show);
}
