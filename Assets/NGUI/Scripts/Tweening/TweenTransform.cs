using System;
using UnityEngine;

// Token: 0x020000DC RID: 220
[AddComponentMenu("NGUI/Tween/Tween Transform")]
public class TweenTransform : UITweener
{
	// Token: 0x06000726 RID: 1830 RVA: 0x000477DC File Offset: 0x000459DC
	protected override void OnUpdate(float factor, bool isFinished)
	{
		if (this.to != null)
		{
			if (this.mTrans == null)
			{
				this.mTrans = base.transform;
				this.mPos = this.mTrans.position;
				this.mRot = this.mTrans.rotation;
				this.mScale = this.mTrans.localScale;
			}
			if (this.from != null)
			{
				this.mTrans.position = this.from.position * (1f - factor) + this.to.position * factor;
				this.mTrans.localScale = this.from.localScale * (1f - factor) + this.to.localScale * factor;
				this.mTrans.rotation = Quaternion.Slerp(this.from.rotation, this.to.rotation, factor);
			}
			else
			{
				this.mTrans.position = this.mPos * (1f - factor) + this.to.position * factor;
				this.mTrans.localScale = this.mScale * (1f - factor) + this.to.localScale * factor;
				this.mTrans.rotation = Quaternion.Slerp(this.mRot, this.to.rotation, factor);
			}
			if (this.parentWhenFinished && isFinished)
			{
				this.mTrans.parent = this.to;
			}
		}
	}

	// Token: 0x06000727 RID: 1831 RVA: 0x00009C43 File Offset: 0x00007E43
	public static TweenTransform Begin(GameObject go, float duration, Transform to)
	{
		return TweenTransform.Begin(go, duration, null, to);
	}

	// Token: 0x06000728 RID: 1832 RVA: 0x000479A4 File Offset: 0x00045BA4
	public static TweenTransform Begin(GameObject go, float duration, Transform from, Transform to)
	{
		TweenTransform tweenTransform = UITweener.Begin<TweenTransform>(go, duration, 0f);
		tweenTransform.from = from;
		tweenTransform.to = to;
		if (duration <= 0f)
		{
			tweenTransform.Sample(1f, true);
			tweenTransform.enabled = false;
		}
		return tweenTransform;
	}

	// Token: 0x040004ED RID: 1261
	public Transform from;

	// Token: 0x040004EE RID: 1262
	public Transform to;

	// Token: 0x040004EF RID: 1263
	public bool parentWhenFinished;

	// Token: 0x040004F0 RID: 1264
	private Transform mTrans;

	// Token: 0x040004F1 RID: 1265
	private Vector3 mPos;

	// Token: 0x040004F2 RID: 1266
	private Quaternion mRot;

	// Token: 0x040004F3 RID: 1267
	private Vector3 mScale;
}
