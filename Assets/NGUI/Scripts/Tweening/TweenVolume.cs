using System;
using UnityEngine;

// Token: 0x020000DD RID: 221
[AddComponentMenu("NGUI/Tween/Tween Volume")]
[RequireComponent(typeof(AudioSource))]
public class TweenVolume : UITweener
{
	// Token: 0x170000D5 RID: 213
	// (get) Token: 0x0600072A RID: 1834 RVA: 0x000479EC File Offset: 0x00045BEC
	public AudioSource audioSource
	{
		get
		{
			if (this.mSource == null)
			{
				this.mSource = base.GetComponent<AudioSource>();
				if (this.mSource == null)
				{
					this.mSource = base.GetComponent<AudioSource>();
					if (this.mSource == null)
					{
						Debug.LogError("TweenVolume needs an AudioSource to work with", this);
						base.enabled = false;
					}
				}
			}
			return this.mSource;
		}
	}

	// Token: 0x170000D6 RID: 214
	// (get) Token: 0x0600072B RID: 1835 RVA: 0x00009C6C File Offset: 0x00007E6C
	// (set) Token: 0x0600072C RID: 1836 RVA: 0x00009C74 File Offset: 0x00007E74
	[Obsolete("Use 'value' instead")]
	public float volume
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

	// Token: 0x170000D7 RID: 215
	// (get) Token: 0x0600072D RID: 1837 RVA: 0x00009C7D File Offset: 0x00007E7D
	// (set) Token: 0x0600072E RID: 1838 RVA: 0x00009CA5 File Offset: 0x00007EA5
	public float value
	{
		get
		{
			return (!(this.audioSource != null)) ? 0f : this.mSource.volume;
		}
		set
		{
			if (this.audioSource != null)
			{
				this.mSource.volume = value;
			}
		}
	}

	// Token: 0x0600072F RID: 1839 RVA: 0x00009CC4 File Offset: 0x00007EC4
	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.value = this.from * (1f - factor) + this.to * factor;
		this.mSource.enabled = (this.mSource.volume > 0.01f);
	}

	// Token: 0x06000730 RID: 1840 RVA: 0x00047A5C File Offset: 0x00045C5C
	public static TweenVolume Begin(GameObject go, float duration, float targetVolume)
	{
		TweenVolume tweenVolume = UITweener.Begin<TweenVolume>(go, duration, 0f);
		tweenVolume.from = tweenVolume.value;
		tweenVolume.to = targetVolume;
		if (targetVolume > 0f)
		{
			AudioSource audioSource = tweenVolume.audioSource;
			audioSource.enabled = true;
			audioSource.Play();
		}
		return tweenVolume;
	}

	// Token: 0x06000731 RID: 1841 RVA: 0x00009D00 File Offset: 0x00007F00
	public override void SetStartToCurrentValue()
	{
		this.from = this.value;
	}

	// Token: 0x06000732 RID: 1842 RVA: 0x00009D0E File Offset: 0x00007F0E
	public override void SetEndToCurrentValue()
	{
		this.to = this.value;
	}

	// Token: 0x040004F4 RID: 1268
	[Range(0f, 1f)]
	public float from = 1f;

	// Token: 0x040004F5 RID: 1269
	[Range(0f, 1f)]
	public float to = 1f;

	// Token: 0x040004F6 RID: 1270
	private AudioSource mSource;
}
