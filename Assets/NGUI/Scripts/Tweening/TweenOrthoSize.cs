using System;
using UnityEngine;

// Token: 0x020000D8 RID: 216
[AddComponentMenu("NGUI/Tween/Tween Orthographic Size")]
[RequireComponent(typeof(Camera))]
public class TweenOrthoSize : UITweener
{
	// Token: 0x170000C9 RID: 201
	// (get) Token: 0x060006F6 RID: 1782 RVA: 0x000099C2 File Offset: 0x00007BC2
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

	// Token: 0x170000CA RID: 202
	// (get) Token: 0x060006F7 RID: 1783 RVA: 0x000099E7 File Offset: 0x00007BE7
	// (set) Token: 0x060006F8 RID: 1784 RVA: 0x000099EF File Offset: 0x00007BEF
	[Obsolete("Use 'value' instead")]
	public float orthoSize
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

	// Token: 0x170000CB RID: 203
	// (get) Token: 0x060006F9 RID: 1785 RVA: 0x000099F8 File Offset: 0x00007BF8
	// (set) Token: 0x060006FA RID: 1786 RVA: 0x00009A05 File Offset: 0x00007C05
	public float value
	{
		get
		{
			return this.cachedCamera.orthographicSize;
		}
		set
		{
			this.cachedCamera.orthographicSize = value;
		}
	}

	// Token: 0x060006FB RID: 1787 RVA: 0x00009A13 File Offset: 0x00007C13
	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.value = this.from * (1f - factor) + this.to * factor;
	}

	// Token: 0x060006FC RID: 1788 RVA: 0x00047444 File Offset: 0x00045644
	public static TweenOrthoSize Begin(GameObject go, float duration, float to)
	{
		TweenOrthoSize tweenOrthoSize = UITweener.Begin<TweenOrthoSize>(go, duration, 0f);
		tweenOrthoSize.from = tweenOrthoSize.value;
		tweenOrthoSize.to = to;
		if (duration <= 0f)
		{
			tweenOrthoSize.Sample(1f, true);
			tweenOrthoSize.enabled = false;
		}
		return tweenOrthoSize;
	}

	// Token: 0x060006FD RID: 1789 RVA: 0x00009A32 File Offset: 0x00007C32
	public override void SetStartToCurrentValue()
	{
		this.from = this.value;
	}

	// Token: 0x060006FE RID: 1790 RVA: 0x00009A40 File Offset: 0x00007C40
	public override void SetEndToCurrentValue()
	{
		this.to = this.value;
	}

	// Token: 0x040004DC RID: 1244
	public float from = 1f;

	// Token: 0x040004DD RID: 1245
	public float to = 1f;

	// Token: 0x040004DE RID: 1246
	private Camera mCam;
}
