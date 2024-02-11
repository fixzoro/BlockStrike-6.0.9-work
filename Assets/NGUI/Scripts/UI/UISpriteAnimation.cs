using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000121 RID: 289
[AddComponentMenu("NGUI/UI/Sprite Animation")]
[RequireComponent(typeof(UISprite))]
[ExecuteInEditMode]
public class UISpriteAnimation : MonoBehaviour
{
	// Token: 0x170001C3 RID: 451
	// (get) Token: 0x06000A34 RID: 2612 RVA: 0x0000B770 File Offset: 0x00009970
	public int frames
	{
		get
		{
			return this.mSpriteNames.Count;
		}
	}

	// Token: 0x170001C4 RID: 452
	// (get) Token: 0x06000A35 RID: 2613 RVA: 0x0000B77D File Offset: 0x0000997D
	// (set) Token: 0x06000A36 RID: 2614 RVA: 0x0000B785 File Offset: 0x00009985
	public int framesPerSecond
	{
		get
		{
			return this.mFPS;
		}
		set
		{
			this.mFPS = value;
		}
	}

	// Token: 0x170001C5 RID: 453
	// (get) Token: 0x06000A37 RID: 2615 RVA: 0x0000B78E File Offset: 0x0000998E
	// (set) Token: 0x06000A38 RID: 2616 RVA: 0x0000B796 File Offset: 0x00009996
	public string namePrefix
	{
		get
		{
			return this.mPrefix;
		}
		set
		{
			if (this.mPrefix != value)
			{
				this.mPrefix = value;
				this.RebuildSpriteList();
			}
		}
	}

	// Token: 0x170001C6 RID: 454
	// (get) Token: 0x06000A39 RID: 2617 RVA: 0x0000B7B6 File Offset: 0x000099B6
	// (set) Token: 0x06000A3A RID: 2618 RVA: 0x0000B7BE File Offset: 0x000099BE
	public bool loop
	{
		get
		{
			return this.mLoop;
		}
		set
		{
			this.mLoop = value;
		}
	}

	// Token: 0x170001C7 RID: 455
	// (get) Token: 0x06000A3B RID: 2619 RVA: 0x0000B7C7 File Offset: 0x000099C7
	public bool isPlaying
	{
		get
		{
			return this.mActive;
		}
	}

	// Token: 0x06000A3C RID: 2620 RVA: 0x0000B7CF File Offset: 0x000099CF
	protected virtual void Start()
	{
		this.RebuildSpriteList();
	}

	// Token: 0x06000A3D RID: 2621 RVA: 0x000587D4 File Offset: 0x000569D4
	protected virtual void Update()
	{
		if (this.mActive && this.mSpriteNames.Count > 1 && Application.isPlaying && this.mFPS > 0)
		{
			this.mDelta += Mathf.Min(1f, RealTime.deltaTime);
			float num = 1f / (float)this.mFPS;
			while (num < this.mDelta)
			{
				this.mDelta = ((num <= 0f) ? 0f : (this.mDelta - num));
				if (++this.frameIndex >= this.mSpriteNames.Count)
				{
					this.frameIndex = 0;
					this.mActive = this.mLoop;
				}
				if (this.mActive)
				{
					this.mSprite.spriteName = this.mSpriteNames[this.frameIndex];
					if (this.mSnap)
					{
						this.mSprite.MakePixelPerfect();
					}
				}
			}
		}
	}

	// Token: 0x06000A3E RID: 2622 RVA: 0x000588E4 File Offset: 0x00056AE4
	public void RebuildSpriteList()
	{
		if (this.mSprite == null)
		{
			this.mSprite = base.GetComponent<UISprite>();
		}
		this.mSpriteNames.Clear();
		if (this.mSprite != null)
		{
			INGUIAtlas atlas = this.mSprite.atlas;
			if (atlas != null)
			{
				List<UISpriteData> spriteList = atlas.spriteList;
				int i = 0;
				int count = spriteList.Count;
				while (i < count)
				{
					UISpriteData uispriteData = spriteList[i];
					if (string.IsNullOrEmpty(this.mPrefix) || uispriteData.name.StartsWith(this.mPrefix))
					{
						this.mSpriteNames.Add(uispriteData.name);
					}
					i++;
				}
				this.mSpriteNames.Sort();
			}
		}
	}

	// Token: 0x06000A3F RID: 2623 RVA: 0x0000B7D7 File Offset: 0x000099D7
	public void Play()
	{
		this.mActive = true;
	}

	// Token: 0x06000A40 RID: 2624 RVA: 0x0000B7E0 File Offset: 0x000099E0
	public void Pause()
	{
		this.mActive = false;
	}

	// Token: 0x06000A41 RID: 2625 RVA: 0x000589AC File Offset: 0x00056BAC
	public void ResetToBeginning()
	{
		this.mActive = true;
		this.frameIndex = 0;
		if (this.mSprite != null && this.mSpriteNames.Count > 0)
		{
			this.mSprite.spriteName = this.mSpriteNames[this.frameIndex];
			if (this.mSnap)
			{
				this.mSprite.MakePixelPerfect();
			}
		}
	}

	// Token: 0x040006FD RID: 1789
	public int frameIndex;

	// Token: 0x040006FE RID: 1790
	[HideInInspector]
	[SerializeField]
	protected int mFPS = 30;

	// Token: 0x040006FF RID: 1791
	[SerializeField]
	[HideInInspector]
	protected string mPrefix = string.Empty;

	// Token: 0x04000700 RID: 1792
	[HideInInspector]
	[SerializeField]
	protected bool mLoop = true;

	// Token: 0x04000701 RID: 1793
	[HideInInspector]
	[SerializeField]
	protected bool mSnap = true;

	// Token: 0x04000702 RID: 1794
	protected UISprite mSprite;

	// Token: 0x04000703 RID: 1795
	protected float mDelta;

	// Token: 0x04000704 RID: 1796
	protected bool mActive = true;

	// Token: 0x04000705 RID: 1797
	protected List<string> mSpriteNames = new List<string>();
}
