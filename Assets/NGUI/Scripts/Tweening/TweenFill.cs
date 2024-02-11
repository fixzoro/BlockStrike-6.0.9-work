using System;
using UnityEngine;

// Token: 0x020000D2 RID: 210
[RequireComponent(typeof(UIBasicSprite))]
[AddComponentMenu("NGUI/Tween/Tween Fill")]
public class TweenFill : UITweener
{
	// Token: 0x060006D3 RID: 1747 RVA: 0x0000978E File Offset: 0x0000798E
	private void Cache()
	{
		this.mCached = true;
		this.mSprite = base.GetComponent<UISprite>();
	}

	// Token: 0x170000C5 RID: 197
	// (get) Token: 0x060006D4 RID: 1748 RVA: 0x000097A3 File Offset: 0x000079A3
	// (set) Token: 0x060006D5 RID: 1749 RVA: 0x000097D8 File Offset: 0x000079D8
	public float value
	{
		get
		{
			if (!this.mCached)
			{
				this.Cache();
			}
			if (this.mSprite != null)
			{
				return this.mSprite.fillAmount;
			}
			return 0f;
		}
		set
		{
			if (!this.mCached)
			{
				this.Cache();
			}
			if (this.mSprite != null)
			{
				this.mSprite.fillAmount = value;
			}
		}
	}

	// Token: 0x060006D6 RID: 1750 RVA: 0x00009808 File Offset: 0x00007A08
	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.value = Mathf.Lerp(this.from, this.to, factor);
	}

	// Token: 0x060006D7 RID: 1751 RVA: 0x00046CC0 File Offset: 0x00044EC0
	public static TweenFill Begin(GameObject go, float duration, float fill)
	{
		TweenFill tweenFill = UITweener.Begin<TweenFill>(go, duration, 0f);
		tweenFill.from = tweenFill.value;
		tweenFill.to = fill;
		if (duration <= 0f)
		{
			tweenFill.Sample(1f, true);
			tweenFill.enabled = false;
		}
		return tweenFill;
	}

	// Token: 0x060006D8 RID: 1752 RVA: 0x00009822 File Offset: 0x00007A22
	public override void SetStartToCurrentValue()
	{
		this.from = this.value;
	}

	// Token: 0x060006D9 RID: 1753 RVA: 0x00009830 File Offset: 0x00007A30
	public override void SetEndToCurrentValue()
	{
		this.to = this.value;
	}

	// Token: 0x040004BC RID: 1212
	[Range(0f, 1f)]
	public float from = 1f;

	// Token: 0x040004BD RID: 1213
	[Range(0f, 1f)]
	public float to = 1f;

	// Token: 0x040004BE RID: 1214
	private bool mCached;

	// Token: 0x040004BF RID: 1215
	private UIBasicSprite mSprite;
}
