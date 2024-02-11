using System;
using UnityEngine;

// Token: 0x020000DA RID: 218
[AddComponentMenu("NGUI/Tween/Tween Rotation")]
public class TweenRotation : UITweener
{
	// Token: 0x170000CF RID: 207
	// (get) Token: 0x0600070E RID: 1806 RVA: 0x00009B25 File Offset: 0x00007D25
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

	// Token: 0x170000D0 RID: 208
	// (get) Token: 0x0600070F RID: 1807 RVA: 0x00009B4A File Offset: 0x00007D4A
	// (set) Token: 0x06000710 RID: 1808 RVA: 0x00009B52 File Offset: 0x00007D52
	[Obsolete("Use 'value' instead")]
	public Quaternion rotation
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

	// Token: 0x170000D1 RID: 209
	// (get) Token: 0x06000711 RID: 1809 RVA: 0x00009B5B File Offset: 0x00007D5B
	// (set) Token: 0x06000712 RID: 1810 RVA: 0x00009B68 File Offset: 0x00007D68
	public Quaternion value
	{
		get
		{
			return this.cachedTransform.localRotation;
		}
		set
		{
			this.cachedTransform.localRotation = value;
		}
	}

	// Token: 0x06000713 RID: 1811 RVA: 0x000475C4 File Offset: 0x000457C4
	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.value = ((!this.quaternionLerp) ? Quaternion.Euler(new Vector3(Mathf.Lerp(this.from.x, this.to.x, factor), Mathf.Lerp(this.from.y, this.to.y, factor), Mathf.Lerp(this.from.z, this.to.z, factor))) : Quaternion.Slerp(Quaternion.Euler(this.from), Quaternion.Euler(this.to), factor));
	}

	// Token: 0x06000714 RID: 1812 RVA: 0x00047664 File Offset: 0x00045864
	public static TweenRotation Begin(GameObject go, float duration, Quaternion rot)
	{
		TweenRotation tweenRotation = UITweener.Begin<TweenRotation>(go, duration, 0f);
		tweenRotation.from = tweenRotation.value.eulerAngles;
		tweenRotation.to = rot.eulerAngles;
		if (duration <= 0f)
		{
			tweenRotation.Sample(1f, true);
			tweenRotation.enabled = false;
		}
		return tweenRotation;
	}

	// Token: 0x06000715 RID: 1813 RVA: 0x000476C0 File Offset: 0x000458C0
	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue()
	{
		this.from = this.value.eulerAngles;
	}

	// Token: 0x06000716 RID: 1814 RVA: 0x000476E4 File Offset: 0x000458E4
	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue()
	{
		this.to = this.value.eulerAngles;
	}

	// Token: 0x06000717 RID: 1815 RVA: 0x00009B76 File Offset: 0x00007D76
	[ContextMenu("Assume value of 'From'")]
	private void SetCurrentValueToStart()
	{
		this.value = Quaternion.Euler(this.from);
	}

	// Token: 0x06000718 RID: 1816 RVA: 0x00009B89 File Offset: 0x00007D89
	[ContextMenu("Assume value of 'To'")]
	private void SetCurrentValueToEnd()
	{
		this.value = Quaternion.Euler(this.to);
	}

	// Token: 0x040004E4 RID: 1252
	public Vector3 from;

	// Token: 0x040004E5 RID: 1253
	public Vector3 to;

	// Token: 0x040004E6 RID: 1254
	public bool quaternionLerp;

	// Token: 0x040004E7 RID: 1255
	private Transform mTrans;
}
