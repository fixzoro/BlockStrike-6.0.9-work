using System;
using UnityEngine;

// Token: 0x020000D0 RID: 208
[AddComponentMenu("NGUI/Tween/Tween Color")]
public class TweenColor : UITweener
{
	// Token: 0x060006BB RID: 1723 RVA: 0x00046A28 File Offset: 0x00044C28
	private void Cache()
	{
		this.mCached = true;
		this.mWidget = base.GetComponent<UIWidget>();
		if (this.mWidget != null)
		{
			return;
		}
		this.mSr = base.GetComponent<SpriteRenderer>();
		if (this.mSr != null)
		{
			return;
		}
		Renderer renderer = base.GetComponent<Renderer>();
		if (renderer != null)
		{
			this.mMat = renderer.material;
			return;
		}
		this.mLight = base.GetComponent<Light>();
		if (this.mLight == null)
		{
			this.mWidget = base.GetComponentInChildren<UIWidget>();
		}
	}

	// Token: 0x170000C0 RID: 192
	// (get) Token: 0x060006BC RID: 1724 RVA: 0x00009647 File Offset: 0x00007847
	// (set) Token: 0x060006BD RID: 1725 RVA: 0x0000964F File Offset: 0x0000784F
	[Obsolete("Use 'value' instead")]
	public Color color
	{
		get
		{
			return this.value;
		}
		set
		{
			this.value = value;
		}
	}

	// Token: 0x170000C1 RID: 193
	// (get) Token: 0x060006BE RID: 1726 RVA: 0x00046AC4 File Offset: 0x00044CC4
	// (set) Token: 0x060006BF RID: 1727 RVA: 0x00046B5C File Offset: 0x00044D5C
	public Color value
	{
		get
		{
			if (!this.mCached)
			{
				this.Cache();
			}
			if (this.mWidget != null)
			{
				return this.mWidget.color;
			}
			if (this.mMat != null)
			{
				return this.mMat.color;
			}
			if (this.mSr != null)
			{
				return this.mSr.color;
			}
			if (this.mLight != null)
			{
				return this.mLight.color;
			}
			return Color.black;
		}
		set
		{
			if (!this.mCached)
			{
				this.Cache();
			}
			if (this.mWidget != null)
			{
				this.mWidget.color = value;
			}
			else if (this.mMat != null)
			{
				this.mMat.color = value;
			}
			else if (this.mSr != null)
			{
				this.mSr.color = value;
			}
			else if (this.mLight != null)
			{
				this.mLight.color = value;
				this.mLight.enabled = (value.r + value.g + value.b > 0.01f);
			}
		}
	}

	// Token: 0x060006C0 RID: 1728 RVA: 0x00009658 File Offset: 0x00007858
	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.value = Color.Lerp(this.from, this.to, factor);
	}

	// Token: 0x060006C1 RID: 1729 RVA: 0x00046C28 File Offset: 0x00044E28
	public static TweenColor Begin(GameObject go, float duration, Color color)
	{
		TweenColor tweenColor = UITweener.Begin<TweenColor>(go, duration, 0f);
		tweenColor.from = tweenColor.value;
		tweenColor.to = color;
		if (duration <= 0f)
		{
			tweenColor.Sample(1f, true);
			tweenColor.enabled = false;
		}
		return tweenColor;
	}

	// Token: 0x060006C2 RID: 1730 RVA: 0x00009672 File Offset: 0x00007872
	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue()
	{
		this.from = this.value;
	}

	// Token: 0x060006C3 RID: 1731 RVA: 0x00009680 File Offset: 0x00007880
	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue()
	{
		this.to = this.value;
	}

	// Token: 0x060006C4 RID: 1732 RVA: 0x0000968E File Offset: 0x0000788E
	[ContextMenu("Assume value of 'From'")]
	private void SetCurrentValueToStart()
	{
		this.value = this.from;
	}

	// Token: 0x060006C5 RID: 1733 RVA: 0x0000969C File Offset: 0x0000789C
	[ContextMenu("Assume value of 'To'")]
	private void SetCurrentValueToEnd()
	{
		this.value = this.to;
	}

	// Token: 0x040004B2 RID: 1202
	public Color from = Color.white;

	// Token: 0x040004B3 RID: 1203
	public Color to = Color.white;

	// Token: 0x040004B4 RID: 1204
	private bool mCached;

	// Token: 0x040004B5 RID: 1205
	private UIWidget mWidget;

	// Token: 0x040004B6 RID: 1206
	private Material mMat;

	// Token: 0x040004B7 RID: 1207
	private Light mLight;

	// Token: 0x040004B8 RID: 1208
	private SpriteRenderer mSr;
}
