using System;
using UnityEngine;

// Token: 0x020000D1 RID: 209
[RequireComponent(typeof(Camera))]
[AddComponentMenu("NGUI/Tween/Tween Field of View")]
public class TweenFOV : UITweener
{
	// Token: 0x170000C2 RID: 194
	// (get) Token: 0x060006C7 RID: 1735 RVA: 0x000096C8 File Offset: 0x000078C8
	public Camera cachedCamera
	{
		get
		{
			if (this.mCam == null)
			{
				this.mCam = base.GetComponent<Camera>();
			}
			return this.mCam;
		}
	}

	// Token: 0x170000C3 RID: 195
	// (get) Token: 0x060006C8 RID: 1736 RVA: 0x000096ED File Offset: 0x000078ED
	// (set) Token: 0x060006C9 RID: 1737 RVA: 0x000096F5 File Offset: 0x000078F5
	[Obsolete("Use 'value' instead")]
	public float fov
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

	// Token: 0x170000C4 RID: 196
	// (get) Token: 0x060006CA RID: 1738 RVA: 0x000096FE File Offset: 0x000078FE
	// (set) Token: 0x060006CB RID: 1739 RVA: 0x0000970B File Offset: 0x0000790B
	public float value
	{
		get
		{
			return this.cachedCamera.fieldOfView;
		}
		set
		{
			this.cachedCamera.fieldOfView = value;
		}
	}

	// Token: 0x060006CC RID: 1740 RVA: 0x00009719 File Offset: 0x00007919
	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.value = this.from * (1f - factor) + this.to * factor;
	}

	// Token: 0x060006CD RID: 1741 RVA: 0x00046C74 File Offset: 0x00044E74
	public static TweenFOV Begin(GameObject go, float duration, float to)
	{
		TweenFOV tweenFOV = UITweener.Begin<TweenFOV>(go, duration, 0f);
		tweenFOV.from = tweenFOV.value;
		tweenFOV.to = to;
		if (duration <= 0f)
		{
			tweenFOV.Sample(1f, true);
			tweenFOV.enabled = false;
		}
		return tweenFOV;
	}

	// Token: 0x060006CE RID: 1742 RVA: 0x00009738 File Offset: 0x00007938
	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue()
	{
		this.from = this.value;
	}

	// Token: 0x060006CF RID: 1743 RVA: 0x00009746 File Offset: 0x00007946
	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue()
	{
		this.to = this.value;
	}

	// Token: 0x060006D0 RID: 1744 RVA: 0x00009754 File Offset: 0x00007954
	[ContextMenu("Assume value of 'From'")]
	private void SetCurrentValueToStart()
	{
		this.value = this.from;
	}

	// Token: 0x060006D1 RID: 1745 RVA: 0x00009762 File Offset: 0x00007962
	[ContextMenu("Assume value of 'To'")]
	private void SetCurrentValueToEnd()
	{
		this.value = this.to;
	}

	// Token: 0x040004B9 RID: 1209
	public float from = 45f;

	// Token: 0x040004BA RID: 1210
	public float to = 45f;

	// Token: 0x040004BB RID: 1211
	private Camera mCam;
}
