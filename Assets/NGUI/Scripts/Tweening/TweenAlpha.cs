using System;
using UnityEngine;

// Token: 0x020000CF RID: 207
[AddComponentMenu("NGUI/Tween/Tween Alpha")]
public class TweenAlpha : UITweener
{
	// Token: 0x170000BE RID: 190
	// (get) Token: 0x060006B1 RID: 1713 RVA: 0x000095E2 File Offset: 0x000077E2
	// (set) Token: 0x060006B2 RID: 1714 RVA: 0x000095EA File Offset: 0x000077EA
	[Obsolete("Use 'value' instead")]
	public float alpha
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

	// Token: 0x060006B3 RID: 1715 RVA: 0x000467C0 File Offset: 0x000449C0
	private void Cache()
	{
		this.mCached = true;
		this.mRect = base.GetComponent<UIRect>();
		this.mSr = base.GetComponent<SpriteRenderer>();
		if (this.mRect == null && this.mSr == null)
		{
			this.mLight = base.GetComponent<Light>();
			if (this.mLight == null)
			{
				Renderer component = base.GetComponent<Renderer>();
				if (component != null)
				{
					this.mMat = component.material;
				}
				if (this.mMat == null)
				{
					this.mRect = base.GetComponentInChildren<UIRect>();
				}
			}
			else
			{
				this.mBaseIntensity = this.mLight.intensity;
			}
		}
	}

	// Token: 0x170000BF RID: 191
	// (get) Token: 0x060006B4 RID: 1716 RVA: 0x00046880 File Offset: 0x00044A80
	// (set) Token: 0x060006B5 RID: 1717 RVA: 0x00046910 File Offset: 0x00044B10
	public float value
	{
		get
		{
			if (!this.mCached)
			{
				this.Cache();
			}
			if (this.mRect != null)
			{
				return this.mRect.alpha;
			}
			if (this.mSr != null)
			{
				return this.mSr.color.a;
			}
			return (!(this.mMat != null)) ? 1f : this.mMat.color.a;
		}
		set
		{
            if (gameObject.GetComponent<UIWidget>() != null)
            {
                gameObject.GetComponent<UIWidget>().UpdateWidget();
            }
            if (!this.mCached)
			{
				this.Cache();
			}
			if (this.mRect != null)
			{
				this.mRect.alpha = value;
			}
			else if (this.mSr != null)
			{
				Color color = this.mSr.color;
				color.a = value;
				this.mSr.color = color;
			}
			else if (this.mMat != null)
			{
				Color color2 = this.mMat.color;
				color2.a = value;
				this.mMat.color = color2;
			}
			else if (this.mLight != null)
			{
				this.mLight.intensity = this.mBaseIntensity * value;
			}
		}
	}

	// Token: 0x060006B6 RID: 1718 RVA: 0x000095F3 File Offset: 0x000077F3
	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.value = Mathf.Lerp(this.from, this.to, factor);
	}

	// Token: 0x060006B7 RID: 1719 RVA: 0x000469E0 File Offset: 0x00044BE0
	public static TweenAlpha Begin(GameObject go, float duration, float alpha, float delay = 0f)
	{
        TweenAlpha tweenAlpha = UITweener.Begin<TweenAlpha>(go, duration, delay);
		tweenAlpha.from = tweenAlpha.value;
		tweenAlpha.to = alpha;
		if (duration <= 0f)
		{
			tweenAlpha.Sample(1f, true);
			tweenAlpha.enabled = false;
		}
        return tweenAlpha;
	}

	// Token: 0x060006B8 RID: 1720 RVA: 0x0000960D File Offset: 0x0000780D
	public override void SetStartToCurrentValue()
	{
		this.from = this.value;
	}

	// Token: 0x060006B9 RID: 1721 RVA: 0x0000961B File Offset: 0x0000781B
	public override void SetEndToCurrentValue()
	{
		this.to = this.value;
	}

	// Token: 0x040004AA RID: 1194
	[Range(0f, 1f)]
	public float from = 1f;

	// Token: 0x040004AB RID: 1195
	[Range(0f, 1f)]
	public float to = 1f;

	// Token: 0x040004AC RID: 1196
	private bool mCached;

	// Token: 0x040004AD RID: 1197
	private UIRect mRect;

	// Token: 0x040004AE RID: 1198
	private Material mMat;

	// Token: 0x040004AF RID: 1199
	private Light mLight;

	// Token: 0x040004B0 RID: 1200
	private SpriteRenderer mSr;

	// Token: 0x040004B1 RID: 1201
	private float mBaseIntensity = 1f;
}
