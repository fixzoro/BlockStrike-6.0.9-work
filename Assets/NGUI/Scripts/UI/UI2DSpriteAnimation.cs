using System;
using UnityEngine;

// Token: 0x020000E8 RID: 232
public class UI2DSpriteAnimation : MonoBehaviour
{
	// Token: 0x1700011B RID: 283
	// (get) Token: 0x060007E8 RID: 2024 RVA: 0x00006089 File Offset: 0x00004289
	public bool isPlaying
	{
		get
		{
			return base.enabled;
		}
	}

	// Token: 0x1700011C RID: 284
	// (get) Token: 0x060007E9 RID: 2025 RVA: 0x0000A1E5 File Offset: 0x000083E5
	// (set) Token: 0x060007EA RID: 2026 RVA: 0x0000A1ED File Offset: 0x000083ED
	public int framesPerSecond
	{
		get
		{
			return this.framerate;
		}
		set
		{
			this.framerate = value;
		}
	}

	// Token: 0x060007EB RID: 2027 RVA: 0x0004A2EC File Offset: 0x000484EC
	public void Play()
	{
		if (this.frames != null && this.frames.Length > 0)
		{
			if (!base.enabled && !this.loop)
			{
				int num = (this.framerate <= 0) ? (this.frameIndex - 1) : (this.frameIndex + 1);
				if (num < 0 || num >= this.frames.Length)
				{
					this.frameIndex = ((this.framerate >= 0) ? 0 : (this.frames.Length - 1));
				}
			}
			base.enabled = true;
			this.UpdateSprite();
		}
	}

	// Token: 0x060007EC RID: 2028 RVA: 0x0000A1F6 File Offset: 0x000083F6
	public void Pause()
	{
		base.enabled = false;
	}

	// Token: 0x060007ED RID: 2029 RVA: 0x0000A1FF File Offset: 0x000083FF
	public void ResetToBeginning()
	{
		this.frameIndex = ((this.framerate >= 0) ? 0 : (this.frames.Length - 1));
		this.UpdateSprite();
	}

	// Token: 0x060007EE RID: 2030 RVA: 0x0000A229 File Offset: 0x00008429
	private void Start()
	{
		this.Play();
	}

	// Token: 0x060007EF RID: 2031 RVA: 0x0004A390 File Offset: 0x00048590
	private void Update()
	{
		if (this.frames == null || this.frames.Length == 0)
		{
			base.enabled = false;
		}
		else if (this.framerate != 0)
		{
			float num = (!this.ignoreTimeScale) ? Time.time : RealTime.time;
			if (this.mUpdate < num)
			{
				this.mUpdate = num;
				int num2 = (this.framerate <= 0) ? (this.frameIndex - 1) : (this.frameIndex + 1);
				if (!this.loop && (num2 < 0 || num2 >= this.frames.Length))
				{
					base.enabled = false;
					return;
				}
				this.frameIndex = NGUIMath.RepeatIndex(num2, this.frames.Length);
				this.UpdateSprite();
			}
		}
	}

	// Token: 0x060007F0 RID: 2032 RVA: 0x0004A460 File Offset: 0x00048660
	private void UpdateSprite()
	{
		if (this.mUnitySprite == null && this.mNguiSprite == null)
		{
			this.mUnitySprite = base.GetComponent<SpriteRenderer>();
			this.mNguiSprite = base.GetComponent<UI2DSprite>();
			if (this.mUnitySprite == null && this.mNguiSprite == null)
			{
				base.enabled = false;
				return;
			}
		}
		float num = (!this.ignoreTimeScale) ? Time.time : RealTime.time;
		if (this.framerate != 0)
		{
			this.mUpdate = num + Mathf.Abs(1f / (float)this.framerate);
		}
		if (this.mUnitySprite != null)
		{
			this.mUnitySprite.sprite = this.frames[this.frameIndex];
		}
		else if (this.mNguiSprite != null)
		{
			this.mNguiSprite.nextSprite = this.frames[this.frameIndex];
		}
	}

	// Token: 0x04000538 RID: 1336
	public int frameIndex;

	// Token: 0x04000539 RID: 1337
	[SerializeField]
	protected int framerate = 20;

	// Token: 0x0400053A RID: 1338
	public bool ignoreTimeScale = true;

	// Token: 0x0400053B RID: 1339
	public bool loop = true;

	// Token: 0x0400053C RID: 1340
	public Sprite[] frames;

	// Token: 0x0400053D RID: 1341
	private SpriteRenderer mUnitySprite;

	// Token: 0x0400053E RID: 1342
	private UI2DSprite mNguiSprite;

	// Token: 0x0400053F RID: 1343
	private float mUpdate;
}
