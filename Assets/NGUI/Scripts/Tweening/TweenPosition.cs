using System;
using UnityEngine;

// Token: 0x020000D9 RID: 217
[AddComponentMenu("NGUI/Tween/Tween Position")]
public class TweenPosition : UITweener
{
	// Token: 0x170000CC RID: 204
	// (get) Token: 0x06000700 RID: 1792 RVA: 0x00009A56 File Offset: 0x00007C56
	public Transform cachedTransform
	{
		get
		{
			if (this.mTrans == null)
			{
				this.mTrans = base.transform;
			}
			return this.mTrans;
		}
	}

	// Token: 0x170000CD RID: 205
	// (get) Token: 0x06000701 RID: 1793 RVA: 0x00009A7B File Offset: 0x00007C7B
	// (set) Token: 0x06000702 RID: 1794 RVA: 0x00009A83 File Offset: 0x00007C83
	[Obsolete("Use 'value' instead")]
	public Vector3 position
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

	// Token: 0x170000CE RID: 206
	// (get) Token: 0x06000703 RID: 1795 RVA: 0x00009A8C File Offset: 0x00007C8C
	// (set) Token: 0x06000704 RID: 1796 RVA: 0x00047490 File Offset: 0x00045690
	public Vector3 value
	{
		get
		{
			return (!this.worldSpace) ? this.cachedTransform.localPosition : this.cachedTransform.position;
		}
		set
		{
			if (this.mRect == null || !this.mRect.isAnchored || this.worldSpace)
			{
				if (this.worldSpace)
				{
					this.cachedTransform.position = value;
				}
				else
				{
					this.cachedTransform.localPosition = value;
				}
			}
			else
			{
				value -= this.cachedTransform.localPosition;
				NGUIMath.MoveRect(this.mRect, value.x, value.y);
			}
		}
	}

	// Token: 0x06000705 RID: 1797 RVA: 0x00009AB4 File Offset: 0x00007CB4
	private void Awake()
	{
		this.mRect = base.GetComponent<UIRect>();
	}

	// Token: 0x06000706 RID: 1798 RVA: 0x00009AC2 File Offset: 0x00007CC2
	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.value = this.from * (1f - factor) + this.to * factor;
	}

	// Token: 0x06000707 RID: 1799 RVA: 0x00047524 File Offset: 0x00045724
	public static TweenPosition Begin(GameObject go, float duration, Vector3 pos)
	{
		TweenPosition tweenPosition = UITweener.Begin<TweenPosition>(go, duration, 0f);
		tweenPosition.from = tweenPosition.value;
		tweenPosition.to = pos;
		if (duration <= 0f)
		{
			tweenPosition.Sample(1f, true);
			tweenPosition.enabled = false;
		}
		return tweenPosition;
	}

	// Token: 0x06000708 RID: 1800 RVA: 0x00047570 File Offset: 0x00045770
	public static TweenPosition Begin(GameObject go, float duration, Vector3 pos, bool worldSpace)
	{
		TweenPosition tweenPosition = UITweener.Begin<TweenPosition>(go, duration, 0f);
		tweenPosition.worldSpace = worldSpace;
		tweenPosition.from = tweenPosition.value;
		tweenPosition.to = pos;
		if (duration <= 0f)
		{
			tweenPosition.Sample(1f, true);
			tweenPosition.enabled = false;
		}
		return tweenPosition;
	}

	// Token: 0x06000709 RID: 1801 RVA: 0x00009AED File Offset: 0x00007CED
	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue()
	{
		this.from = this.value;
	}

	// Token: 0x0600070A RID: 1802 RVA: 0x00009AFB File Offset: 0x00007CFB
	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue()
	{
		this.to = this.value;
	}

	// Token: 0x0600070B RID: 1803 RVA: 0x00009B09 File Offset: 0x00007D09
	[ContextMenu("Assume value of 'From'")]
	private void SetCurrentValueToStart()
	{
		this.value = this.from;
	}

	// Token: 0x0600070C RID: 1804 RVA: 0x00009B17 File Offset: 0x00007D17
	[ContextMenu("Assume value of 'To'")]
	private void SetCurrentValueToEnd()
	{
		this.value = this.to;
	}

	// Token: 0x040004DF RID: 1247
	public Vector3 from;

	// Token: 0x040004E0 RID: 1248
	public Vector3 to;

	// Token: 0x040004E1 RID: 1249
	[HideInInspector]
	public bool worldSpace;

	// Token: 0x040004E2 RID: 1250
	private Transform mTrans;

	// Token: 0x040004E3 RID: 1251
	private UIRect mRect;
}
