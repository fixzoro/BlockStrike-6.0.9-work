using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000110 RID: 272
[AddComponentMenu("NGUI/UI/Label")]
[ExecuteInEditMode]
public class UILabel : UIWidget
{
	// Token: 0x17000160 RID: 352
	// (get) Token: 0x0600091B RID: 2331 RVA: 0x0000A9F7 File Offset: 0x00008BF7
	public int finalFontSize
	{
		get
		{
			if (this.trueTypeFont)
			{
				return Mathf.RoundToInt(this.mScale * (float)this.mFinalFontSize);
			}
			return Mathf.RoundToInt((float)this.mFontSize * this.mScale);
		}
	}

	// Token: 0x17000161 RID: 353
	// (get) Token: 0x0600091C RID: 2332 RVA: 0x0000AA30 File Offset: 0x00008C30
	// (set) Token: 0x0600091D RID: 2333 RVA: 0x0000AA38 File Offset: 0x00008C38
	private bool shouldBeProcessed
	{
		get
		{
			return this.mShouldBeProcessed;
		}
		set
		{
			if (value)
			{
				this.mChanged = true;
				this.mShouldBeProcessed = true;
			}
			else
			{
				this.mShouldBeProcessed = false;
			}
		}
	}

	// Token: 0x17000162 RID: 354
	// (get) Token: 0x0600091E RID: 2334 RVA: 0x0000AA5A File Offset: 0x00008C5A
	public override bool isAnchoredHorizontally
	{
		get
		{
			return base.isAnchoredHorizontally || this.mOverflow == UILabel.Overflow.ResizeFreely;
		}
	}

	// Token: 0x17000163 RID: 355
	// (get) Token: 0x0600091F RID: 2335 RVA: 0x0000AA73 File Offset: 0x00008C73
	public override bool isAnchoredVertically
	{
		get
		{
			return base.isAnchoredVertically || this.mOverflow == UILabel.Overflow.ResizeFreely || this.mOverflow == UILabel.Overflow.ResizeHeight;
		}
	}

	// Token: 0x17000164 RID: 356
	// (get) Token: 0x06000920 RID: 2336 RVA: 0x000529E8 File Offset: 0x00050BE8
	// (set) Token: 0x06000921 RID: 2337 RVA: 0x0000AA98 File Offset: 0x00008C98
	public override Material material
	{
		get
		{
			if (this.mMat != null)
			{
				return this.mMat;
			}
			INGUIFont bitmapFont = this.bitmapFont;
			if (bitmapFont != null)
			{
				return bitmapFont.material;
			}
			if (this.mTrueTypeFont != null)
			{
				return this.mTrueTypeFont.material;
			}
			return null;
		}
		set
		{
			base.material = value;
		}
	}

	// Token: 0x17000165 RID: 357
	// (get) Token: 0x06000922 RID: 2338 RVA: 0x00052A40 File Offset: 0x00050C40
	// (set) Token: 0x06000923 RID: 2339 RVA: 0x0000AAA1 File Offset: 0x00008CA1
	public override Texture mainTexture
	{
		get
		{
			INGUIFont bitmapFont = this.bitmapFont;
			if (bitmapFont != null)
			{
				return bitmapFont.texture;
			}
			if (this.mTrueTypeFont != null)
			{
				Material material = this.mTrueTypeFont.material;
				if (material != null)
				{
					return material.mainTexture;
				}
			}
			return null;
		}
		set
		{
			base.mainTexture = value;
		}
	}

	// Token: 0x17000166 RID: 358
	// (get) Token: 0x06000924 RID: 2340 RVA: 0x0000AAAA File Offset: 0x00008CAA
	// (set) Token: 0x06000925 RID: 2341 RVA: 0x0000AAB7 File Offset: 0x00008CB7
	[Obsolete("Use UILabel.bitmapFont instead")]
	public UnityEngine.Object font
	{
		get
		{
			return this.bitmapFont as UnityEngine.Object;
		}
		set
		{
			this.bitmapFont = (value as INGUIFont);
		}
	}

	// Token: 0x17000167 RID: 359
	// (get) Token: 0x06000926 RID: 2342 RVA: 0x0000AAC5 File Offset: 0x00008CC5
	// (set) Token: 0x06000927 RID: 2343 RVA: 0x0000AAD2 File Offset: 0x00008CD2
	public INGUIFont bitmapFont
	{
		get
		{
			return this.mFont as INGUIFont;
		}
		set
		{
			if (this.mFont as INGUIFont != value)
			{
				base.RemoveFromPanel();
				this.mFont = (UIFont)value;
				this.mTrueTypeFont = null;
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x17000168 RID: 360
	// (get) Token: 0x06000928 RID: 2344 RVA: 0x00052A94 File Offset: 0x00050C94
	// (set) Token: 0x06000929 RID: 2345 RVA: 0x00052AB8 File Offset: 0x00050CB8
	public INGUIAtlas atlas
	{
		get
		{
			INGUIFont bitmapFont = this.bitmapFont;
			if (bitmapFont != null)
			{
				return bitmapFont.atlas;
			}
			return null;
		}
		set
		{
			INGUIFont bitmapFont = this.bitmapFont;
			if (bitmapFont != null)
			{
				bitmapFont.atlas = value;
			}
		}
	}

	// Token: 0x17000169 RID: 361
	// (get) Token: 0x0600092A RID: 2346 RVA: 0x00052ADC File Offset: 0x00050CDC
	// (set) Token: 0x0600092B RID: 2347 RVA: 0x00052B18 File Offset: 0x00050D18
	public Font trueTypeFont
	{
		get
		{
			if (this.mTrueTypeFont != null)
			{
				return this.mTrueTypeFont;
			}
			INGUIFont bitmapFont = this.bitmapFont;
			if (bitmapFont != null)
			{
				return bitmapFont.dynamicFont;
			}
			return null;
		}
		set
		{
			if (this.mTrueTypeFont != value)
			{
				this.SetActiveFont(null);
				base.RemoveFromPanel();
				this.mTrueTypeFont = value;
				this.shouldBeProcessed = true;
				this.mFont = null;
				this.SetActiveFont(value);
				this.ProcessAndRequest();
				if (this.mActiveTTF != null)
				{
					base.MarkAsChanged();
				}
				if (this.widgetAreStatic)
				{
					base.UpdateWidget();
				}
			}
		}
	}

	// Token: 0x1700016A RID: 362
	// (get) Token: 0x0600092C RID: 2348 RVA: 0x0000AB04 File Offset: 0x00008D04
	// (set) Token: 0x0600092D RID: 2349 RVA: 0x00052B90 File Offset: 0x00050D90
	public UnityEngine.Object ambigiousFont
	{
		get
		{
			return (UnityEngine.Object)this.mFont ?? (UnityEngine.Object)this.mTrueTypeFont;
		}
		set
		{
			INGUIFont inguifont = value as INGUIFont;
			if (inguifont != null)
			{
				this.bitmapFont = inguifont;
			}
			else
			{
				this.trueTypeFont = (value as Font);
			}
		}
	}

	// Token: 0x1700016B RID: 363
	// (get) Token: 0x0600092E RID: 2350 RVA: 0x0000AB19 File Offset: 0x00008D19
	// (set) Token: 0x0600092F RID: 2351 RVA: 0x00052BC4 File Offset: 0x00050DC4
	public string text
	{
		get
		{
			return this.mText;
		}
		set
		{
            if (this.mText == value)
			{
				return;
			}
			if (string.IsNullOrEmpty(value))
			{
				if (!string.IsNullOrEmpty(this.mText))
				{
					this.mText = string.Empty;
					this.MarkAsChanged();
					this.ProcessAndRequest();
					if (this.autoResizeBoxCollider)
					{
						base.ResizeCollider();
					}
					if (this.widgetAreStatic)
					{
                        base.UpdateWidget();
					}
				}
			}
			else if (this.mText != value)
			{
				this.mText = value;
				this.MarkAsChanged();
				this.ProcessAndRequest();
				if (this.autoResizeBoxCollider)
				{
					base.ResizeCollider();
				}
				if (this.widgetAreStatic)
				{
					base.UpdateWidget();
				}
			}
		}
	}

	// Token: 0x1700016C RID: 364
	// (get) Token: 0x06000930 RID: 2352 RVA: 0x00052C84 File Offset: 0x00050E84
	public int defaultFontSize
	{
		get
		{
			INGUIFont bitmapFont = this.bitmapFont;
			if (bitmapFont != null)
			{
				return bitmapFont.defaultSize;
			}
			if (this.trueTypeFont != null)
			{
				return this.mFontSize;
			}
			return 16;
		}
	}

	// Token: 0x1700016D RID: 365
	// (get) Token: 0x06000931 RID: 2353 RVA: 0x0000AB21 File Offset: 0x00008D21
	// (set) Token: 0x06000932 RID: 2354 RVA: 0x0000AB29 File Offset: 0x00008D29
	public int fontSize
	{
		get
		{
			return this.mFontSize;
		}
		set
		{
			value = Mathf.Clamp(value, 0, 256);
			if (this.mFontSize != value)
			{
				this.mFontSize = value;
				this.shouldBeProcessed = true;
				this.ProcessAndRequest();
			}
		}
	}

	// Token: 0x1700016E RID: 366
	// (get) Token: 0x06000933 RID: 2355 RVA: 0x0000AB59 File Offset: 0x00008D59
	// (set) Token: 0x06000934 RID: 2356 RVA: 0x0000AB61 File Offset: 0x00008D61
	public FontStyle fontStyle
	{
		get
		{
			return this.mFontStyle;
		}
		set
		{
			if (this.mFontStyle != value)
			{
				this.mFontStyle = value;
				this.shouldBeProcessed = true;
				this.ProcessAndRequest();
			}
		}
	}

	// Token: 0x1700016F RID: 367
	// (get) Token: 0x06000935 RID: 2357 RVA: 0x0000AB83 File Offset: 0x00008D83
	// (set) Token: 0x06000936 RID: 2358 RVA: 0x0000AB8B File Offset: 0x00008D8B
	public NGUIText.Alignment alignment
	{
		get
		{
			return this.mAlignment;
		}
		set
		{
			if (this.mAlignment != value)
			{
				this.mAlignment = value;
				this.shouldBeProcessed = true;
				this.ProcessAndRequest();
			}
		}
	}

	// Token: 0x17000170 RID: 368
	// (get) Token: 0x06000937 RID: 2359 RVA: 0x0000ABAD File Offset: 0x00008DAD
	// (set) Token: 0x06000938 RID: 2360 RVA: 0x0000ABB5 File Offset: 0x00008DB5
	public bool applyGradient
	{
		get
		{
			return this.mApplyGradient;
		}
		set
		{
			if (this.mApplyGradient != value)
			{
				this.mApplyGradient = value;
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x17000171 RID: 369
	// (get) Token: 0x06000939 RID: 2361 RVA: 0x0000ABD0 File Offset: 0x00008DD0
	// (set) Token: 0x0600093A RID: 2362 RVA: 0x0000ABD8 File Offset: 0x00008DD8
	public Color gradientTop
	{
		get
		{
			return this.mGradientTop;
		}
		set
		{
			if (this.mGradientTop != value)
			{
				this.mGradientTop = value;
				if (this.mApplyGradient)
				{
					this.MarkAsChanged();
				}
			}
		}
	}

	// Token: 0x17000172 RID: 370
	// (get) Token: 0x0600093B RID: 2363 RVA: 0x0000AC03 File Offset: 0x00008E03
	// (set) Token: 0x0600093C RID: 2364 RVA: 0x0000AC0B File Offset: 0x00008E0B
	public Color gradientBottom
	{
		get
		{
			return this.mGradientBottom;
		}
		set
		{
			if (this.mGradientBottom != value)
			{
				this.mGradientBottom = value;
				if (this.mApplyGradient)
				{
					this.MarkAsChanged();
				}
			}
		}
	}

	// Token: 0x17000173 RID: 371
	// (get) Token: 0x0600093D RID: 2365 RVA: 0x0000AC36 File Offset: 0x00008E36
	// (set) Token: 0x0600093E RID: 2366 RVA: 0x0000AC3E File Offset: 0x00008E3E
	public int spacingX
	{
		get
		{
			return this.mSpacingX;
		}
		set
		{
			if (this.mSpacingX != value)
			{
				this.mSpacingX = value;
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x17000174 RID: 372
	// (get) Token: 0x0600093F RID: 2367 RVA: 0x0000AC59 File Offset: 0x00008E59
	// (set) Token: 0x06000940 RID: 2368 RVA: 0x0000AC61 File Offset: 0x00008E61
	public int spacingY
	{
		get
		{
			return this.mSpacingY;
		}
		set
		{
			if (this.mSpacingY != value)
			{
				this.mSpacingY = value;
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x17000175 RID: 373
	// (get) Token: 0x06000941 RID: 2369 RVA: 0x0000AC7C File Offset: 0x00008E7C
	// (set) Token: 0x06000942 RID: 2370 RVA: 0x0000AC84 File Offset: 0x00008E84
	public bool useFloatSpacing
	{
		get
		{
			return this.mUseFloatSpacing;
		}
		set
		{
			if (this.mUseFloatSpacing != value)
			{
				this.mUseFloatSpacing = value;
				this.shouldBeProcessed = true;
			}
		}
	}

	// Token: 0x17000176 RID: 374
	// (get) Token: 0x06000943 RID: 2371 RVA: 0x0000ACA0 File Offset: 0x00008EA0
	// (set) Token: 0x06000944 RID: 2372 RVA: 0x0000ACA8 File Offset: 0x00008EA8
	public float floatSpacingX
	{
		get
		{
			return this.mFloatSpacingX;
		}
		set
		{
			if (!Mathf.Approximately(this.mFloatSpacingX, value))
			{
				this.mFloatSpacingX = value;
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x17000177 RID: 375
	// (get) Token: 0x06000945 RID: 2373 RVA: 0x0000ACC8 File Offset: 0x00008EC8
	// (set) Token: 0x06000946 RID: 2374 RVA: 0x0000ACD0 File Offset: 0x00008ED0
	public float floatSpacingY
	{
		get
		{
			return this.mFloatSpacingY;
		}
		set
		{
			if (!Mathf.Approximately(this.mFloatSpacingY, value))
			{
				this.mFloatSpacingY = value;
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x17000178 RID: 376
	// (get) Token: 0x06000947 RID: 2375 RVA: 0x0000ACF0 File Offset: 0x00008EF0
	public float effectiveSpacingY
	{
		get
		{
			return (!this.mUseFloatSpacing) ? ((float)this.mSpacingY) : this.mFloatSpacingY;
		}
	}

	// Token: 0x17000179 RID: 377
	// (get) Token: 0x06000948 RID: 2376 RVA: 0x0000AD0F File Offset: 0x00008F0F
	public float effectiveSpacingX
	{
		get
		{
			return (!this.mUseFloatSpacing) ? ((float)this.mSpacingX) : this.mFloatSpacingX;
		}
	}

	// Token: 0x1700017A RID: 378
	// (get) Token: 0x06000949 RID: 2377 RVA: 0x0000AD2E File Offset: 0x00008F2E
	// (set) Token: 0x0600094A RID: 2378 RVA: 0x0000AD36 File Offset: 0x00008F36
	public bool overflowEllipsis
	{
		get
		{
			return this.mOverflowEllipsis;
		}
		set
		{
			if (this.mOverflowEllipsis != value)
			{
				this.mOverflowEllipsis = value;
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x1700017B RID: 379
	// (get) Token: 0x0600094B RID: 2379 RVA: 0x0000AD51 File Offset: 0x00008F51
	// (set) Token: 0x0600094C RID: 2380 RVA: 0x0000AD59 File Offset: 0x00008F59
	public int overflowWidth
	{
		get
		{
			return this.mOverflowWidth;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			if (this.mOverflowWidth != value)
			{
				this.mOverflowWidth = value;
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x1700017C RID: 380
	// (get) Token: 0x0600094D RID: 2381 RVA: 0x0000AD7E File Offset: 0x00008F7E
	// (set) Token: 0x0600094E RID: 2382 RVA: 0x0000AD86 File Offset: 0x00008F86
	public int overflowHeight
	{
		get
		{
			return this.mOverflowHeight;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			if (this.mOverflowHeight != value)
			{
				this.mOverflowHeight = value;
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x1700017D RID: 381
	// (get) Token: 0x0600094F RID: 2383 RVA: 0x0000ADAB File Offset: 0x00008FAB
	private bool keepCrisp
	{
		get
		{
			return this.trueTypeFont != null && this.keepCrispWhenShrunk != UILabel.Crispness.Never && this.keepCrispWhenShrunk == UILabel.Crispness.Always;
		}
	}

	// Token: 0x1700017E RID: 382
	// (get) Token: 0x06000950 RID: 2384 RVA: 0x0000ADD4 File Offset: 0x00008FD4
	// (set) Token: 0x06000951 RID: 2385 RVA: 0x0000ADDC File Offset: 0x00008FDC
	public bool supportEncoding
	{
		get
		{
			return this.mEncoding;
		}
		set
		{
			if (this.mEncoding != value)
			{
				this.mEncoding = value;
				this.shouldBeProcessed = true;
			}
		}
	}

	// Token: 0x1700017F RID: 383
	// (get) Token: 0x06000952 RID: 2386 RVA: 0x0000ADF8 File Offset: 0x00008FF8
	// (set) Token: 0x06000953 RID: 2387 RVA: 0x0000AE00 File Offset: 0x00009000
	public NGUIText.SymbolStyle symbolStyle
	{
		get
		{
			return this.mSymbols;
		}
		set
		{
			if (this.mSymbols != value)
			{
				this.mSymbols = value;
				this.shouldBeProcessed = true;
			}
		}
	}

	// Token: 0x17000180 RID: 384
	// (get) Token: 0x06000954 RID: 2388 RVA: 0x0000AE1C File Offset: 0x0000901C
	// (set) Token: 0x06000955 RID: 2389 RVA: 0x0000AE24 File Offset: 0x00009024
	public UILabel.Overflow overflowMethod
	{
		get
		{
			return this.mOverflow;
		}
		set
		{
			if (this.mOverflow != value)
			{
				this.mOverflow = value;
				this.shouldBeProcessed = true;
			}
		}
	}

	// Token: 0x17000181 RID: 385
	// (get) Token: 0x06000956 RID: 2390 RVA: 0x0000AE40 File Offset: 0x00009040
	// (set) Token: 0x06000957 RID: 2391 RVA: 0x0000AE48 File Offset: 0x00009048
	[Obsolete("Use 'width' instead")]
	public int lineWidth
	{
		get
		{
			return base.width;
		}
		set
		{
			base.width = value;
		}
	}

	// Token: 0x17000182 RID: 386
	// (get) Token: 0x06000958 RID: 2392 RVA: 0x0000AE51 File Offset: 0x00009051
	// (set) Token: 0x06000959 RID: 2393 RVA: 0x0000AE59 File Offset: 0x00009059
	[Obsolete("Use 'height' instead")]
	public int lineHeight
	{
		get
		{
			return base.height;
		}
		set
		{
			base.height = value;
		}
	}

	// Token: 0x17000183 RID: 387
	// (get) Token: 0x0600095A RID: 2394 RVA: 0x0000AE62 File Offset: 0x00009062
	// (set) Token: 0x0600095B RID: 2395 RVA: 0x0000AE70 File Offset: 0x00009070
	public bool multiLine
	{
		get
		{
			return this.mMaxLineCount != 1;
		}
		set
		{
			if (this.mMaxLineCount != 1 != value)
			{
				this.mMaxLineCount = ((!value) ? 1 : 0);
				this.shouldBeProcessed = true;
			}
		}
	}

	// Token: 0x17000184 RID: 388
	// (get) Token: 0x0600095C RID: 2396 RVA: 0x0000AE9E File Offset: 0x0000909E
	public override Vector3[] localCorners
	{
		get
		{
			if (this.shouldBeProcessed)
			{
				this.ProcessText(false, true);
			}
			return base.localCorners;
		}
	}

	// Token: 0x17000185 RID: 389
	// (get) Token: 0x0600095D RID: 2397 RVA: 0x0000AEB9 File Offset: 0x000090B9
	public override Vector3[] worldCorners
	{
		get
		{
			if (this.shouldBeProcessed)
			{
				this.ProcessText(false, true);
			}
			return base.worldCorners;
		}
	}

	// Token: 0x17000186 RID: 390
	// (get) Token: 0x0600095E RID: 2398 RVA: 0x0000AED4 File Offset: 0x000090D4
	public override Vector4 drawingDimensions
	{
		get
		{
			if (this.shouldBeProcessed)
			{
				this.ProcessText(false, true);
			}
			return base.drawingDimensions;
		}
	}

	// Token: 0x17000187 RID: 391
	// (get) Token: 0x0600095F RID: 2399 RVA: 0x0000AEEF File Offset: 0x000090EF
	// (set) Token: 0x06000960 RID: 2400 RVA: 0x0000AEF7 File Offset: 0x000090F7
	public int maxLineCount
	{
		get
		{
			return this.mMaxLineCount;
		}
		set
		{
			if (this.mMaxLineCount != value)
			{
				this.mMaxLineCount = Mathf.Max(value, 0);
				this.shouldBeProcessed = true;
				if (this.overflowMethod == UILabel.Overflow.ShrinkContent)
				{
					this.MakePixelPerfect();
				}
			}
		}
	}

	// Token: 0x17000188 RID: 392
	// (get) Token: 0x06000961 RID: 2401 RVA: 0x0000AF2A File Offset: 0x0000912A
	// (set) Token: 0x06000962 RID: 2402 RVA: 0x0000AF32 File Offset: 0x00009132
	public UILabel.Effect effectStyle
	{
		get
		{
			return this.mEffectStyle;
		}
		set
		{
			if (this.mEffectStyle != value)
			{
				this.mEffectStyle = value;
				this.shouldBeProcessed = true;
			}
		}
	}

	// Token: 0x17000189 RID: 393
	// (get) Token: 0x06000963 RID: 2403 RVA: 0x0000AF4E File Offset: 0x0000914E
	// (set) Token: 0x06000964 RID: 2404 RVA: 0x0000AF56 File Offset: 0x00009156
	public Color effectColor
	{
		get
		{
			return this.mEffectColor;
		}
		set
		{
			if (this.mEffectColor != value)
			{
				this.mEffectColor = value;
				if (this.mEffectStyle != UILabel.Effect.None)
				{
					this.shouldBeProcessed = true;
				}
			}
		}
	}

	// Token: 0x1700018A RID: 394
	// (get) Token: 0x06000965 RID: 2405 RVA: 0x0000AF82 File Offset: 0x00009182
	// (set) Token: 0x06000966 RID: 2406 RVA: 0x0000AF8A File Offset: 0x0000918A
	public Vector2 effectDistance
	{
		get
		{
			return this.mEffectDistance;
		}
		set
		{
			if (this.mEffectDistance != value)
			{
				this.mEffectDistance = value;
				this.shouldBeProcessed = true;
			}
		}
	}

	// Token: 0x1700018B RID: 395
	// (get) Token: 0x06000967 RID: 2407 RVA: 0x0000AFAB File Offset: 0x000091AB
	public int quadsPerCharacter
	{
		get
		{
			if (this.mEffectStyle == UILabel.Effect.Shadow)
			{
				return 2;
			}
			if (this.mEffectStyle == UILabel.Effect.Outline)
			{
				return 5;
			}
			if (this.mEffectStyle == UILabel.Effect.Outline8)
			{
				return 9;
			}
			return 1;
		}
	}

	// Token: 0x1700018C RID: 396
	// (get) Token: 0x06000968 RID: 2408 RVA: 0x0000AFD9 File Offset: 0x000091D9
	// (set) Token: 0x06000969 RID: 2409 RVA: 0x0000AFE4 File Offset: 0x000091E4
	[Obsolete("Use 'overflowMethod == UILabel.Overflow.ShrinkContent' instead")]
	public bool shrinkToFit
	{
		get
		{
			return this.mOverflow == UILabel.Overflow.ShrinkContent;
		}
		set
		{
			if (value)
			{
				this.overflowMethod = UILabel.Overflow.ShrinkContent;
			}
		}
	}

	// Token: 0x1700018D RID: 397
	// (get) Token: 0x0600096A RID: 2410 RVA: 0x00052CC0 File Offset: 0x00050EC0
	public string processedText
	{
		get
		{
			if (this.mLastWidth != this.mWidth || this.mLastHeight != this.mHeight)
			{
				this.mLastWidth = this.mWidth;
				this.mLastHeight = this.mHeight;
				this.mShouldBeProcessed = true;
			}
			if (this.shouldBeProcessed)
			{
				this.ProcessText(false, true);
			}
			return this.mProcessedText;
		}
	}

	// Token: 0x1700018E RID: 398
	// (get) Token: 0x0600096B RID: 2411 RVA: 0x0000AFF3 File Offset: 0x000091F3
	public Vector2 printedSize
	{
		get
		{
			if (this.shouldBeProcessed)
			{
				this.ProcessText(false, true);
			}
			return this.mCalculatedSize;
		}
	}

	// Token: 0x1700018F RID: 399
	// (get) Token: 0x0600096C RID: 2412 RVA: 0x0000B00E File Offset: 0x0000920E
	public override Vector2 localSize
	{
		get
		{
			if (this.shouldBeProcessed)
			{
				this.ProcessText(false, true);
			}
			return base.localSize;
		}
	}

	// Token: 0x17000190 RID: 400
	// (get) Token: 0x0600096D RID: 2413 RVA: 0x0000B029 File Offset: 0x00009229
	private bool isValid
	{
		get
		{
			return this.mFont != null || this.mTrueTypeFont != null;
		}
	}

	// Token: 0x17000191 RID: 401
	// (get) Token: 0x0600096E RID: 2414 RVA: 0x0000B04B File Offset: 0x0000924B
	// (set) Token: 0x0600096F RID: 2415 RVA: 0x0000B053 File Offset: 0x00009253
	public UILabel.Modifier modifier
	{
		get
		{
			return this.mModifier;
		}
		set
		{
			if (this.mModifier != value)
			{
				this.mModifier = value;
				this.MarkAsChanged();
				this.ProcessAndRequest();
			}
		}
	}

	// Token: 0x06000970 RID: 2416 RVA: 0x0000B074 File Offset: 0x00009274
	protected override void OnInit()
	{
		base.OnInit();
		UILabel.mList.Add(this);
		this.SetActiveFont(this.trueTypeFont);
	}

	// Token: 0x06000971 RID: 2417 RVA: 0x0000B093 File Offset: 0x00009293
	protected override void OnDisable()
	{
		this.SetActiveFont(null);
		UILabel.mList.Remove(this);
		base.OnDisable();
	}

	// Token: 0x06000972 RID: 2418 RVA: 0x00052D28 File Offset: 0x00050F28
	protected void SetActiveFont(Font fnt)
	{
		if (this.mActiveTTF != fnt)
		{
			Font font = this.mActiveTTF;
			int num;
			if (font != null && UILabel.mFontUsage.TryGetValue(font, out num))
			{
				num = Mathf.Max(0, --num);
				if (num == 0)
				{
					UILabel.mFontUsage.Remove(font);
				}
				else
				{
					UILabel.mFontUsage[font] = num;
				}
			}
			this.mActiveTTF = fnt;
			if (fnt != null)
			{
				int num2 = 0;
				UILabel.mFontUsage[fnt] = num2 + 1;
			}
		}
	}

	// Token: 0x17000192 RID: 402
	// (get) Token: 0x06000973 RID: 2419 RVA: 0x00052DC4 File Offset: 0x00050FC4
	public string printedText
	{
		get
		{
			if (!string.IsNullOrEmpty(this.mText))
			{
				if (this.mModifier == UILabel.Modifier.None)
				{
					return this.mText;
				}
				if (this.mModifier == UILabel.Modifier.ToLowercase)
				{
					return this.mText.ToLower();
				}
				if (this.mModifier == UILabel.Modifier.ToUppercase)
				{
					return this.mText.ToUpper();
				}
				if (this.mModifier == UILabel.Modifier.Custom && this.customModifier != null)
				{
					return this.customModifier(this.mText);
				}
			}
			return this.mText;
		}
	}

	// Token: 0x06000974 RID: 2420 RVA: 0x00052E58 File Offset: 0x00051058
	private static void OnFontChanged(Font font)
	{
		for (int i = 0; i < UILabel.mList.size; i++)
		{
			UILabel uilabel = UILabel.mList[i];
			if (uilabel != null)
			{
				Font trueTypeFont = uilabel.trueTypeFont;
				if (trueTypeFont == font)
				{
					trueTypeFont.RequestCharactersInTexture(uilabel.mText, uilabel.mFinalFontSize, uilabel.mFontStyle);
					uilabel.MarkAsChanged();
					if (uilabel.panel == null)
					{
						uilabel.CreatePanel();
					}
					if (UILabel.mTempDrawcalls == null)
					{
						UILabel.mTempDrawcalls = new BetterList<UIDrawCall>();
					}
					if (uilabel.drawCall != null && !UILabel.mTempDrawcalls.Contains(uilabel.drawCall))
					{
						UILabel.mTempDrawcalls.Add(uilabel.drawCall);
					}
				}
			}
		}
		if (UILabel.mTempDrawcalls != null)
		{
			int j = 0;
			int size = UILabel.mTempDrawcalls.size;
			while (j < size)
			{
				UIDrawCall uidrawCall = UILabel.mTempDrawcalls[j];
				if (uidrawCall.panel != null)
				{
					uidrawCall.panel.FillDrawCall(uidrawCall);
				}
				j++;
			}
			UILabel.mTempDrawcalls.Clear();
		}
		TimerManager.In(0.1f, delegate()
		{
			string text = string.Empty;
			for (int k = 0; k < UILabel.mList.size; k++)
			{
				UILabel uilabel2 = UILabel.mList[k];
				if (uilabel2 != null)
				{
					text = uilabel2.text;
					uilabel2.text = "1#2";
					uilabel2.text = text;
				}
			}
		});
	}

	// Token: 0x06000975 RID: 2421 RVA: 0x0000B0AE File Offset: 0x000092AE
	public override Vector3[] GetSides(Transform relativeTo)
	{
		if (this.shouldBeProcessed)
		{
			this.ProcessText(false, true);
		}
		return base.GetSides(relativeTo);
	}

	// Token: 0x06000976 RID: 2422 RVA: 0x00052FB4 File Offset: 0x000511B4
	protected override void UpgradeFrom265()
	{
		this.ProcessText(true, true);
		if (this.mShrinkToFit)
		{
			this.overflowMethod = UILabel.Overflow.ShrinkContent;
			this.mMaxLineCount = 0;
		}
		if (this.mMaxLineWidth != 0)
		{
			base.width = this.mMaxLineWidth;
			this.overflowMethod = ((this.mMaxLineCount <= 0) ? UILabel.Overflow.ShrinkContent : UILabel.Overflow.ResizeHeight);
		}
		else
		{
			this.overflowMethod = UILabel.Overflow.ResizeFreely;
		}
		if (this.mMaxLineHeight != 0)
		{
			base.height = this.mMaxLineHeight;
		}
		if (this.mFont != null)
		{
			int defaultFontSize = this.defaultFontSize;
			if (base.height < defaultFontSize)
			{
				base.height = defaultFontSize;
			}
			this.fontSize = defaultFontSize;
		}
		this.mMaxLineWidth = 0;
		this.mMaxLineHeight = 0;
		this.mShrinkToFit = false;
		NGUITools.UpdateWidgetCollider(base.gameObject, true);
	}

	// Token: 0x06000977 RID: 2423 RVA: 0x00053088 File Offset: 0x00051288
	protected override void OnAnchor()
	{
		if (this.mOverflow == UILabel.Overflow.ResizeFreely)
		{
			if (base.isFullyAnchored)
			{
				this.mOverflow = UILabel.Overflow.ShrinkContent;
			}
		}
		else if (this.mOverflow == UILabel.Overflow.ResizeHeight && this.topAnchor.target != null && this.bottomAnchor.target != null)
		{
			this.mOverflow = UILabel.Overflow.ShrinkContent;
		}
		base.OnAnchor();
	}

	// Token: 0x06000978 RID: 2424 RVA: 0x0000B0CA File Offset: 0x000092CA
	private void ProcessAndRequest()
	{
#if UNITY_EDITOR
        if (!Application.isPlaying && !NGUITools.GetActive(this)) return;
        if (!mAllowProcessing) return;
#endif
        if (ambigiousFont != null) ProcessText();
    }

#if UNITY_EDITOR
    // Used to ensure that we don't process font more than once inside OnValidate function below
    [System.NonSerialized] bool mAllowProcessing = true;
    [System.NonSerialized] bool mUsingTTF = true;

    /// <summary>
    /// Validate the properties.
    /// </summary>

    protected override void OnValidate()
    {
        base.OnValidate();

        if (NGUITools.GetActive(this))
        {
            Font ttf = mTrueTypeFont;
            UIFont fnt = mFont;

            // If the true type font was not used before, but now it is, clear the font reference
            if (!mUsingTTF && ttf != null) fnt = null;
            else if (mUsingTTF && fnt != null) ttf = null;

            mFont = null;
            mTrueTypeFont = null;
            mAllowProcessing = false;

#if DYNAMIC_FONT
			SetActiveFont(null);
#endif
            if (fnt != null)
            {
                bitmapFont = fnt;
                mUsingTTF = false;
            }
            else if (ttf != null)
            {
                trueTypeFont = ttf;
                mUsingTTF = true;
            }

            shouldBeProcessed = true;
            mAllowProcessing = true;
            ProcessAndRequest();
            if (autoResizeBoxCollider) ResizeCollider();
        }
    }
#endif

    // Token: 0x06000979 RID: 2425 RVA: 0x0000B0E5 File Offset: 0x000092E5
    protected override void OnEnable()
	{
		base.OnEnable();
		if (!UILabel.mTexRebuildAdded)
		{
			UILabel.mTexRebuildAdded = true;
			Font.textureRebuilt += UILabel.OnFontChanged;
		}
	}

	// Token: 0x0600097A RID: 2426 RVA: 0x00053100 File Offset: 0x00051300
	protected override void OnStart()
	{
		base.OnStart();
		if (this.mLineWidth > 0f)
		{
			this.mMaxLineWidth = Mathf.RoundToInt(this.mLineWidth);
			this.mLineWidth = 0f;
		}
		if (!this.mMultiline)
		{
			this.mMaxLineCount = 1;
			this.mMultiline = true;
		}
		this.mPremultiply = (this.material != null && this.material.shader != null && this.material.shader.name.Contains("Premultiplied"));
		this.ProcessAndRequest();
	}

	// Token: 0x0600097B RID: 2427 RVA: 0x0000B10E File Offset: 0x0000930E
	public override void MarkAsChanged()
	{
		this.shouldBeProcessed = true;
		base.MarkAsChanged();
	}

	// Token: 0x0600097C RID: 2428 RVA: 0x000531A8 File Offset: 0x000513A8
	public void ProcessText(bool legacyMode = false, bool full = true)
	{
		if (!this.isValid)
		{
			return;
		}
		this.mChanged = true;
		this.shouldBeProcessed = false;
		float num = this.mDrawRegion.z - this.mDrawRegion.x;
		float num2 = this.mDrawRegion.w - this.mDrawRegion.y;
		NGUIText.rectWidth = ((!legacyMode) ? base.width : ((this.mMaxLineWidth == 0) ? 1000000 : this.mMaxLineWidth));
		NGUIText.rectHeight = ((!legacyMode) ? base.height : ((this.mMaxLineHeight == 0) ? 1000000 : this.mMaxLineHeight));
		NGUIText.regionWidth = ((num == 1f) ? NGUIText.rectWidth : Mathf.RoundToInt((float)NGUIText.rectWidth * num));
		NGUIText.regionHeight = ((num2 == 1f) ? NGUIText.rectHeight : Mathf.RoundToInt((float)NGUIText.rectHeight * num2));
		this.mFinalFontSize = Mathf.Abs((!legacyMode) ? this.defaultFontSize : Mathf.RoundToInt(base.cachedTransform.localScale.x));
		this.mScale = 1f;
		if (NGUIText.regionWidth < 1 || NGUIText.regionHeight < 0)
		{
			this.mProcessedText = string.Empty;
			return;
		}
		bool flag = this.trueTypeFont != null;
		if (flag && this.keepCrisp)
		{
			UIRoot root = base.root;
			if (root != null)
			{
				this.mDensity = ((!(root != null)) ? 1f : root.pixelSizeAdjustment);
			}
		}
		else
		{
			this.mDensity = 1f;
		}
		if (full)
		{
			this.UpdateNGUIText();
		}
		if (this.mOverflow == UILabel.Overflow.ResizeFreely)
		{
			if (this.mOverflowWidth > 0)
			{
				NGUIText.rectWidth = this.mOverflowWidth;
				NGUIText.regionWidth = this.mOverflowWidth;
			}
			else
			{
				NGUIText.rectWidth = 1000000;
				NGUIText.regionWidth = 1000000;
			}
			if (this.mOverflowHeight > 0)
			{
				NGUIText.rectHeight = this.mOverflowHeight;
				NGUIText.regionHeight = this.mOverflowHeight;
			}
			else
			{
				NGUIText.rectHeight = 1000000;
				NGUIText.regionHeight = 1000000;
			}
		}
		else if (this.mOverflow == UILabel.Overflow.ResizeFreely || this.mOverflow == UILabel.Overflow.ResizeHeight)
		{
			NGUIText.rectHeight = 1000000;
			NGUIText.regionHeight = 1000000;
		}
		if (this.mFinalFontSize > 0)
		{
			bool keepCrisp = this.keepCrisp;
			for (int i = this.mFinalFontSize; i > 0; i--)
			{
				if (keepCrisp)
				{
					this.mFinalFontSize = i;
					NGUIText.fontSize = this.mFinalFontSize;
				}
				else
				{
					this.mScale = (float)i / (float)this.mFinalFontSize;
					INGUIFont bitmapFont = this.bitmapFont;
					if (bitmapFont != null)
					{
						NGUIText.fontScale = (float)this.mFontSize / (float)bitmapFont.defaultSize * this.mScale;
					}
					else
					{
						NGUIText.fontScale = this.mScale;
					}
				}
				NGUIText.Update(false);
				bool flag2 = NGUIText.WrapText(this.printedText, out this.mProcessedText, false, false, this.mOverflow == UILabel.Overflow.ClampContent && this.mOverflowEllipsis);
				if (this.mOverflow != UILabel.Overflow.ShrinkContent || flag2)
				{
					if (this.mOverflow == UILabel.Overflow.ResizeFreely)
					{
						this.mCalculatedSize = NGUIText.CalculatePrintedSize(this.mProcessedText);
						if (!flag2 && this.mOverflowWidth > 0)
						{
							if (--i > 1)
							{
								goto IL_560;
							}
							break;
						}
						else
						{
							int num3 = Mathf.Max(this.minWidth, Mathf.RoundToInt(this.mCalculatedSize.x));
							if (num != 1f)
							{
								num3 = Mathf.RoundToInt((float)num3 / num);
							}
							int num4 = Mathf.Max(this.minHeight, Mathf.RoundToInt(this.mCalculatedSize.y));
							if (num2 != 1f)
							{
								num4 = Mathf.RoundToInt((float)num4 / num2);
							}
							if ((num3 & 1) == 1)
							{
								num3++;
							}
							if ((num4 & 1) == 1)
							{
								num4++;
							}
							if (this.mWidth != num3 || this.mHeight != num4)
							{
								this.mWidth = num3;
								this.mHeight = num4;
								if (this.onChange != null)
								{
									this.onChange();
								}
							}
						}
					}
					else if (this.mOverflow == UILabel.Overflow.ResizeHeight)
					{
						this.mCalculatedSize = NGUIText.CalculatePrintedSize(this.mProcessedText);
						int num5 = Mathf.Max(this.minHeight, Mathf.RoundToInt(this.mCalculatedSize.y));
						if (num2 != 1f)
						{
							num5 = Mathf.RoundToInt((float)num5 / num2);
						}
						if ((num5 & 1) == 1)
						{
							num5++;
						}
						if (this.mHeight != num5)
						{
							this.mHeight = num5;
							if (this.onChange != null)
							{
								this.onChange();
							}
						}
					}
					else
					{
						this.mCalculatedSize = NGUIText.CalculatePrintedSize(this.mProcessedText);
					}
					if (legacyMode)
					{
						base.width = Mathf.RoundToInt(this.mCalculatedSize.x);
						base.height = Mathf.RoundToInt(this.mCalculatedSize.y);
						base.cachedTransform.localScale = Vector3.one;
					}
					break;
				}
				if (--i <= 1)
				{
					break;
				}
				IL_560:;
			}
		}
		else
		{
			base.cachedTransform.localScale = Vector3.one;
			this.mProcessedText = string.Empty;
			this.mScale = 1f;
		}
		if (full)
		{
			NGUIText.bitmapFont = null;
			NGUIText.dynamicFont = null;
		}
	}

	// Token: 0x0600097D RID: 2429 RVA: 0x00053760 File Offset: 0x00051960
	public override void MakePixelPerfect()
	{
		if (this.ambigiousFont != null)
		{
			Vector3 localPosition = base.cachedTransform.localPosition;
			localPosition.x = (float)Mathf.RoundToInt(localPosition.x);
			localPosition.y = (float)Mathf.RoundToInt(localPosition.y);
			localPosition.z = (float)Mathf.RoundToInt(localPosition.z);
			base.cachedTransform.localPosition = localPosition;
			base.cachedTransform.localScale = Vector3.one;
			if (this.mOverflow == UILabel.Overflow.ResizeFreely)
			{
				this.AssumeNaturalSize();
			}
			else
			{
				int width = base.width;
				int height = base.height;
				UILabel.Overflow overflow = this.mOverflow;
				if (overflow != UILabel.Overflow.ResizeHeight)
				{
					this.mWidth = 100000;
				}
				this.mHeight = 100000;
				this.mOverflow = UILabel.Overflow.ShrinkContent;
				this.ProcessText(false, true);
				this.mOverflow = overflow;
				int num = Mathf.RoundToInt(this.mCalculatedSize.x);
				int num2 = Mathf.RoundToInt(this.mCalculatedSize.y);
				num = Mathf.Max(num, base.minWidth);
				num2 = Mathf.Max(num2, base.minHeight);
				if ((num & 1) == 1)
				{
					num++;
				}
				if ((num2 & 1) == 1)
				{
					num2++;
				}
				this.mWidth = Mathf.Max(width, num);
				this.mHeight = Mathf.Max(height, num2);
				this.MarkAsChanged();
			}
		}
		else
		{
			base.MakePixelPerfect();
		}
	}

	// Token: 0x0600097E RID: 2430 RVA: 0x000538D0 File Offset: 0x00051AD0
	public void AssumeNaturalSize()
	{
		if (this.ambigiousFont != null)
		{
			this.mWidth = 100000;
			this.mHeight = 100000;
			this.ProcessText(false, true);
			this.mWidth = Mathf.RoundToInt(this.mCalculatedSize.x);
			this.mHeight = Mathf.RoundToInt(this.mCalculatedSize.y);
			if ((this.mWidth & 1) == 1)
			{
				this.mWidth++;
			}
			if ((this.mHeight & 1) == 1)
			{
				this.mHeight++;
			}
			this.MarkAsChanged();
		}
	}

	// Token: 0x0600097F RID: 2431 RVA: 0x0000B11D File Offset: 0x0000931D
	[Obsolete("Use UILabel.GetCharacterAtPosition instead")]
	public int GetCharacterIndex(Vector3 worldPos)
	{
		return this.GetCharacterIndexAtPosition(worldPos, false);
	}

	// Token: 0x06000980 RID: 2432 RVA: 0x0000B127 File Offset: 0x00009327
	[Obsolete("Use UILabel.GetCharacterAtPosition instead")]
	public int GetCharacterIndex(Vector2 localPos)
	{
		return this.GetCharacterIndexAtPosition(localPos, false);
	}

	// Token: 0x06000981 RID: 2433 RVA: 0x00053978 File Offset: 0x00051B78
	public int GetCharacterIndexAtPosition(Vector3 worldPos, bool precise)
	{
		Vector2 localPos = base.cachedTransform.InverseTransformPoint(worldPos);
		return this.GetCharacterIndexAtPosition(localPos, precise);
	}

	// Token: 0x06000982 RID: 2434 RVA: 0x000539A0 File Offset: 0x00051BA0
	public int GetCharacterIndexAtPosition(Vector2 localPos, bool precise)
	{
		if (this.isValid)
		{
			string processedText = this.processedText;
			if (string.IsNullOrEmpty(processedText))
			{
				return 0;
			}
			this.UpdateNGUIText();
			if (precise)
			{
				NGUIText.PrintExactCharacterPositions(processedText, UILabel.mTempVerts, UILabel.mTempIndices);
			}
			else
			{
				NGUIText.PrintApproximateCharacterPositions(processedText, UILabel.mTempVerts, UILabel.mTempIndices);
			}
			if (UILabel.mTempVerts.Count > 0)
			{
				this.ApplyOffset(UILabel.mTempVerts, 0);
				int result = (!precise) ? NGUIText.GetApproximateCharacterIndex(UILabel.mTempVerts, UILabel.mTempIndices, localPos) : NGUIText.GetExactCharacterIndex(UILabel.mTempVerts, UILabel.mTempIndices, localPos);
				UILabel.mTempVerts.Clear();
				UILabel.mTempIndices.Clear();
				NGUIText.bitmapFont = null;
				NGUIText.dynamicFont = null;
				return result;
			}
			NGUIText.bitmapFont = null;
			NGUIText.dynamicFont = null;
		}
		return 0;
	}

	// Token: 0x06000983 RID: 2435 RVA: 0x00053A78 File Offset: 0x00051C78
	public string GetWordAtPosition(Vector3 worldPos)
	{
		int characterIndexAtPosition = this.GetCharacterIndexAtPosition(worldPos, true);
		return this.GetWordAtCharacterIndex(characterIndexAtPosition);
	}

	// Token: 0x06000984 RID: 2436 RVA: 0x00053A98 File Offset: 0x00051C98
	public string GetWordAtPosition(Vector2 localPos)
	{
		int characterIndexAtPosition = this.GetCharacterIndexAtPosition(localPos, true);
		return this.GetWordAtCharacterIndex(characterIndexAtPosition);
	}

	// Token: 0x06000985 RID: 2437 RVA: 0x00053AB8 File Offset: 0x00051CB8
	public string GetWordAtCharacterIndex(int characterIndex)
	{
		string printedText = this.printedText;
		if (characterIndex != -1 && characterIndex < printedText.Length)
		{
			int num = printedText.LastIndexOfAny(new char[]
			{
				' ',
				'\n'
			}, characterIndex) + 1;
			int num2 = printedText.IndexOfAny(new char[]
			{
				' ',
				'\n',
				',',
				'.'
			}, characterIndex);
			if (num2 == -1)
			{
				num2 = printedText.Length;
			}
			if (num != num2)
			{
				int num3 = num2 - num;
				if (num3 > 0)
				{
					string text = printedText.Substring(num, num3);
					return NGUIText.StripSymbols(text);
				}
			}
		}
		return null;
	}

	// Token: 0x06000986 RID: 2438 RVA: 0x0000B131 File Offset: 0x00009331
	public string GetUrlAtPosition(Vector3 worldPos)
	{
		return this.GetUrlAtCharacterIndex(this.GetCharacterIndexAtPosition(worldPos, true));
	}

	// Token: 0x06000987 RID: 2439 RVA: 0x0000B141 File Offset: 0x00009341
	public string GetUrlAtPosition(Vector2 localPos)
	{
		return this.GetUrlAtCharacterIndex(this.GetCharacterIndexAtPosition(localPos, true));
	}

	// Token: 0x06000988 RID: 2440 RVA: 0x00053B48 File Offset: 0x00051D48
	public string GetUrlAtCharacterIndex(int characterIndex)
	{
		string printedText = this.printedText;
		if (characterIndex != -1 && characterIndex < printedText.Length - 6)
		{
			int num;
			if (printedText[characterIndex] == '[' && printedText[characterIndex + 1] == 'u' && printedText[characterIndex + 2] == 'r' && printedText[characterIndex + 3] == 'l' && printedText[characterIndex + 4] == '=')
			{
				num = characterIndex;
			}
			else
			{
				num = printedText.LastIndexOf("[url=", characterIndex);
			}
			if (num == -1)
			{
				return null;
			}
			num += 5;
			int num2 = printedText.IndexOf("]", num);
			if (num2 == -1)
			{
				return null;
			}
			int num3 = printedText.IndexOf("[/url]", num2);
			if (num3 == -1 || characterIndex <= num3)
			{
				return printedText.Substring(num, num2 - num);
			}
		}
		return null;
	}

	// Token: 0x06000989 RID: 2441 RVA: 0x00053C20 File Offset: 0x00051E20
	public int GetCharacterIndex(int currentIndex, KeyCode key)
	{
		if (this.isValid)
		{
			string processedText = this.processedText;
			if (string.IsNullOrEmpty(processedText))
			{
				return 0;
			}
			int defaultFontSize = this.defaultFontSize;
			this.UpdateNGUIText();
			NGUIText.PrintApproximateCharacterPositions(processedText, UILabel.mTempVerts, UILabel.mTempIndices);
			if (UILabel.mTempVerts.Count > 0)
			{
				this.ApplyOffset(UILabel.mTempVerts, 0);
				int i = 0;
				int count = UILabel.mTempIndices.Count;
				while (i < count)
				{
					if (UILabel.mTempIndices[i] == currentIndex)
					{
						Vector2 pos = UILabel.mTempVerts[i];
						if (key == KeyCode.UpArrow)
						{
							pos.y += (float)defaultFontSize + this.effectiveSpacingY;
						}
						else if (key == KeyCode.DownArrow)
						{
							pos.y -= (float)defaultFontSize + this.effectiveSpacingY;
						}
						else if (key == KeyCode.Home)
						{
							pos.x -= 1000f;
						}
						else if (key == KeyCode.End)
						{
							pos.x += 1000f;
						}
						int approximateCharacterIndex = NGUIText.GetApproximateCharacterIndex(UILabel.mTempVerts, UILabel.mTempIndices, pos);
						if (approximateCharacterIndex == currentIndex)
						{
							break;
						}
						UILabel.mTempVerts.Clear();
						UILabel.mTempIndices.Clear();
						return approximateCharacterIndex;
					}
					else
					{
						i++;
					}
				}
				UILabel.mTempVerts.Clear();
				UILabel.mTempIndices.Clear();
			}
			NGUIText.bitmapFont = null;
			NGUIText.dynamicFont = null;
			if (key == KeyCode.UpArrow || key == KeyCode.Home)
			{
				return 0;
			}
			if (key == KeyCode.DownArrow || key == KeyCode.End)
			{
				return processedText.Length;
			}
		}
		return currentIndex;
	}

	// Token: 0x0600098A RID: 2442 RVA: 0x00053DE4 File Offset: 0x00051FE4
	public void PrintOverlay(int start, int end, UIGeometry caret, UIGeometry highlight, Color caretColor, Color highlightColor)
	{
		if (caret != null)
		{
			caret.Clear();
		}
		if (highlight != null)
		{
			highlight.Clear();
		}
		if (!this.isValid)
		{
			return;
		}
		string processedText = this.processedText;
		this.UpdateNGUIText();
		int count = caret.verts.Count;
		Vector2 item = new Vector2(0.5f, 0.5f);
		float finalAlpha = this.finalAlpha;
		if (highlight != null && start != end)
		{
			int count2 = highlight.verts.Count;
			NGUIText.PrintCaretAndSelection(processedText, start, end, caret.verts, highlight.verts);
			if (highlight.verts.Count > count2)
			{
				this.ApplyOffset(highlight.verts, count2);
				Color item2 = new Color(highlightColor.r, highlightColor.g, highlightColor.b, highlightColor.a * finalAlpha);
				int i = count2;
				int count3 = highlight.verts.Count;
				while (i < count3)
				{
					highlight.uvs.Add(item);
					highlight.cols.Add(item2);
					i++;
				}
			}
		}
		else
		{
			NGUIText.PrintCaretAndSelection(processedText, start, end, caret.verts, null);
		}
		this.ApplyOffset(caret.verts, count);
		Color item3 = new Color(caretColor.r, caretColor.g, caretColor.b, caretColor.a * finalAlpha);
		int j = count;
		int count4 = caret.verts.Count;
		while (j < count4)
		{
			caret.uvs.Add(item);
			caret.cols.Add(item3);
			j++;
		}
		NGUIText.bitmapFont = null;
		NGUIText.dynamicFont = null;
	}

	// Token: 0x17000193 RID: 403
	// (get) Token: 0x0600098B RID: 2443 RVA: 0x00053F94 File Offset: 0x00052194
	private bool premultipliedAlphaShader
	{
		get
		{
			INGUIFont bitmapFont = this.bitmapFont;
			return bitmapFont != null && bitmapFont.premultipliedAlphaShader;
		}
	}

	// Token: 0x17000194 RID: 404
	// (get) Token: 0x0600098C RID: 2444 RVA: 0x00053FB8 File Offset: 0x000521B8
	private bool packedFontShader
	{
		get
		{
			INGUIFont bitmapFont = this.bitmapFont;
			return bitmapFont != null && bitmapFont.packedFontShader;
		}
	}

	// Token: 0x0600098D RID: 2445 RVA: 0x00053FDC File Offset: 0x000521DC
	public override void OnFill(List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
	{
		if (!this.isValid)
		{
			return;
		}
		int num = verts.Count;
		Color color = base.color;
		color.a = this.finalAlpha;
		if (this.premultipliedAlphaShader)
		{
			color = NGUITools.ApplyPMA(color);
		}
		string processedText = this.processedText;
		int count = verts.Count;
		this.UpdateNGUIText();
		NGUIText.tint = color;
		NGUIText.Print(processedText, verts, uvs, cols);
		NGUIText.bitmapFont = null;
		NGUIText.dynamicFont = null;
		Vector2 vector = this.ApplyOffset(verts, count);
		if (this.packedFontShader)
		{
			return;
		}
		if (this.effectStyle != UILabel.Effect.None)
		{
			int count2 = verts.Count;
			vector.x = this.mEffectDistance.x;
			vector.y = this.mEffectDistance.y;
			this.ApplyShadow(verts, uvs, cols, num, count2, vector.x, -vector.y);
			if (this.effectStyle == UILabel.Effect.Outline || this.effectStyle == UILabel.Effect.Outline8)
			{
				num = count2;
				count2 = verts.Count;
				this.ApplyShadow(verts, uvs, cols, num, count2, -vector.x, vector.y);
				num = count2;
				count2 = verts.Count;
				this.ApplyShadow(verts, uvs, cols, num, count2, vector.x, vector.y);
				num = count2;
				count2 = verts.Count;
				this.ApplyShadow(verts, uvs, cols, num, count2, -vector.x, -vector.y);
				if (this.effectStyle == UILabel.Effect.Outline8)
				{
					num = count2;
					count2 = verts.Count;
					this.ApplyShadow(verts, uvs, cols, num, count2, -vector.x, 0f);
					num = count2;
					count2 = verts.Count;
					this.ApplyShadow(verts, uvs, cols, num, count2, vector.x, 0f);
					num = count2;
					count2 = verts.Count;
					this.ApplyShadow(verts, uvs, cols, num, count2, 0f, vector.y);
					num = count2;
					count2 = verts.Count;
					this.ApplyShadow(verts, uvs, cols, num, count2, 0f, -vector.y);
				}
			}
		}
		if (NGUIText.symbolStyle == NGUIText.SymbolStyle.NoOutline)
		{
			int i = 0;
			int count3 = cols.Count;
			while (i < count3)
			{
				if (cols[i].r == -1f)
				{
					cols[i] = Color.white;
				}
				i++;
			}
		}
		if (this.onPostFill != null)
		{
			this.onPostFill(this, num, verts, uvs, cols);
		}
	}

	// Token: 0x0600098E RID: 2446 RVA: 0x0005424C File Offset: 0x0005244C
	public Vector2 ApplyOffset(List<Vector3> verts, int start)
	{
		Vector2 pivotOffset = base.pivotOffset;
		float num = Mathf.Lerp(0f, (float)(-(float)this.mWidth), pivotOffset.x);
		float num2 = Mathf.Lerp((float)this.mHeight, 0f, pivotOffset.y) + Mathf.Lerp(this.mCalculatedSize.y - (float)this.mHeight, 0f, pivotOffset.y);
		num = Mathf.Round(num);
		num2 = Mathf.Round(num2);
		int i = start;
		int count = verts.Count;
		while (i < count)
		{
			Vector3 value = verts[i];
			value.x += num;
			value.y += num2;
			verts[i] = value;
			i++;
		}
		return new Vector2(num, num2);
	}

	// Token: 0x0600098F RID: 2447 RVA: 0x0005431C File Offset: 0x0005251C
	public void ApplyShadow(List<Vector3> verts, List<Vector2> uvs, List<Color> cols, int start, int end, float x, float y)
	{
		Color color = this.mEffectColor;
		color.a *= this.finalAlpha;
		if (this.premultipliedAlphaShader)
		{
			color = NGUITools.ApplyPMA(color);
		}
		Color value = color;
		for (int i = start; i < end; i++)
		{
			verts.Add(verts[i]);
			uvs.Add(uvs[i]);
			cols.Add(cols[i]);
			Vector3 value2 = verts[i];
			value2.x += x;
			value2.y += y;
			verts[i] = value2;
			Color color2 = cols[i];
			if (color2.a == 1f)
			{
				cols[i] = value;
			}
			else
			{
				Color value3 = color;
				value3.a = color2.a * color.a;
				cols[i] = value3;
			}
		}
	}

	// Token: 0x06000990 RID: 2448 RVA: 0x0005440C File Offset: 0x0005260C
	public int CalculateOffsetToFit(string text)
	{
		this.UpdateNGUIText();
		NGUIText.encoding = false;
		NGUIText.symbolStyle = NGUIText.SymbolStyle.None;
		int result = NGUIText.CalculateOffsetToFit(text);
		NGUIText.bitmapFont = null;
		NGUIText.dynamicFont = null;
		return result;
	}

	// Token: 0x06000991 RID: 2449 RVA: 0x00054440 File Offset: 0x00052640
	public void SetCurrentProgress()
	{
		if (UIProgressBar.current != null)
		{
			this.text = UIProgressBar.current.value.ToString("F");
		}
	}

	// Token: 0x06000992 RID: 2450 RVA: 0x0000B151 File Offset: 0x00009351
	public void SetCurrentPercent()
	{
		if (UIProgressBar.current != null)
		{
			this.text = Mathf.RoundToInt(UIProgressBar.current.value * 100f) + "%";
		}
	}

	// Token: 0x06000993 RID: 2451 RVA: 0x0005447C File Offset: 0x0005267C
	public void SetCurrentSelection()
	{
		if (UIPopupList.current != null)
		{
			this.text = ((!UIPopupList.current.isLocalized) ? UIPopupList.current.value : Localization.Get(UIPopupList.current.value, true));
		}
	}

	// Token: 0x06000994 RID: 2452 RVA: 0x0000B18D File Offset: 0x0000938D
	public bool Wrap(string text, out string final)
	{
		return this.Wrap(text, out final, 1000000);
	}

	// Token: 0x06000995 RID: 2453 RVA: 0x000544D0 File Offset: 0x000526D0
	public bool Wrap(string text, out string final, int height)
	{
		this.UpdateNGUIText();
		NGUIText.rectHeight = height;
		NGUIText.regionHeight = height;
		bool result = NGUIText.WrapText(text, out final, false);
		NGUIText.bitmapFont = null;
		NGUIText.dynamicFont = null;
		return result;
	}

	// Token: 0x06000996 RID: 2454 RVA: 0x00054508 File Offset: 0x00052708
	public void UpdateNGUIText()
	{
		Font trueTypeFont = this.trueTypeFont;
		bool flag = trueTypeFont != null;
		NGUIText.fontSize = this.mFinalFontSize;
		NGUIText.fontStyle = this.mFontStyle;
		NGUIText.rectWidth = this.mWidth;
		NGUIText.rectHeight = this.mHeight;
		NGUIText.regionWidth = Mathf.RoundToInt((float)this.mWidth * (this.mDrawRegion.z - this.mDrawRegion.x));
		NGUIText.regionHeight = Mathf.RoundToInt((float)this.mHeight * (this.mDrawRegion.w - this.mDrawRegion.y));
		NGUIText.gradient = (this.mApplyGradient && !this.packedFontShader);
		NGUIText.gradientTop = this.mGradientTop;
		NGUIText.gradientBottom = this.mGradientBottom;
		NGUIText.encoding = this.mEncoding;
		NGUIText.premultiply = this.mPremultiply;
		NGUIText.symbolStyle = this.mSymbols;
		NGUIText.maxLines = this.mMaxLineCount;
		NGUIText.spacingX = this.effectiveSpacingX;
		NGUIText.spacingY = this.effectiveSpacingY;
		INGUIFont bitmapFont = this.bitmapFont;
		if (flag)
		{
			NGUIText.fontScale = this.mScale;
		}
		else if (bitmapFont != null)
		{
			NGUIText.fontScale = (float)this.mFontSize / (float)bitmapFont.defaultSize * this.mScale;
		}
		else
		{
			NGUIText.fontScale = this.mScale;
		}
		if (bitmapFont != null)
		{
			if (trueTypeFont != null)
			{
				NGUIText.dynamicFont = trueTypeFont;
				NGUIText.bitmapFont = null;
			}
			else
			{
				NGUIText.dynamicFont = null;
				NGUIText.bitmapFont = bitmapFont;
			}
		}
		else
		{
			NGUIText.dynamicFont = trueTypeFont;
			NGUIText.bitmapFont = null;
		}
		if (flag && this.keepCrisp)
		{
			UIRoot root = base.root;
			if (root != null)
			{
				NGUIText.pixelDensity = ((!(root != null)) ? 1f : root.pixelSizeAdjustment);
			}
		}
		else
		{
			NGUIText.pixelDensity = 1f;
		}
		if (this.mDensity != NGUIText.pixelDensity)
		{
			this.ProcessText(false, false);
			NGUIText.rectWidth = this.mWidth;
			NGUIText.rectHeight = this.mHeight;
			NGUIText.regionWidth = Mathf.RoundToInt((float)this.mWidth * (this.mDrawRegion.z - this.mDrawRegion.x));
			NGUIText.regionHeight = Mathf.RoundToInt((float)this.mHeight * (this.mDrawRegion.w - this.mDrawRegion.y));
		}
		if (this.alignment == NGUIText.Alignment.Automatic)
		{
			UIWidget.Pivot pivot = base.pivot;
			if (pivot == UIWidget.Pivot.Left || pivot == UIWidget.Pivot.TopLeft || pivot == UIWidget.Pivot.BottomLeft)
			{
				NGUIText.alignment = NGUIText.Alignment.Left;
			}
			else if (pivot == UIWidget.Pivot.Right || pivot == UIWidget.Pivot.TopRight || pivot == UIWidget.Pivot.BottomRight)
			{
				NGUIText.alignment = NGUIText.Alignment.Right;
			}
			else
			{
				NGUIText.alignment = NGUIText.Alignment.Center;
			}
		}
		else
		{
			NGUIText.alignment = this.alignment;
		}
		NGUIText.Update();
	}

	// Token: 0x06000997 RID: 2455 RVA: 0x0000B19C File Offset: 0x0000939C
	private void OnApplicationPause(bool paused)
	{
		if (!paused && this.mTrueTypeFont != null)
		{
			this.Invalidate(false);
		}
	}

	// Token: 0x04000665 RID: 1637
	public UILabel.Crispness keepCrispWhenShrunk = UILabel.Crispness.Always;

	// Token: 0x04000666 RID: 1638
	[HideInInspector]
	[SerializeField]
	private Font mTrueTypeFont;

	// Token: 0x04000667 RID: 1639
	[HideInInspector]
	[SerializeField]
    private UIFont mFont;

	// Token: 0x04000668 RID: 1640
	[Multiline(6)]
	[SerializeField]
	[HideInInspector]
	private string mText = string.Empty;

	// Token: 0x04000669 RID: 1641
	[SerializeField]
	[HideInInspector]
	private int mFontSize = 16;

	// Token: 0x0400066A RID: 1642
	[HideInInspector]
	[SerializeField]
	private FontStyle mFontStyle;

	// Token: 0x0400066B RID: 1643
	[SerializeField]
	[HideInInspector]
	private NGUIText.Alignment mAlignment;

	// Token: 0x0400066C RID: 1644
	[HideInInspector]
	[SerializeField]
	private bool mEncoding;

	// Token: 0x0400066D RID: 1645
	[SerializeField]
	[HideInInspector]
	private int mMaxLineCount = 1;

	// Token: 0x0400066E RID: 1646
	[HideInInspector]
	[SerializeField]
	private UILabel.Effect mEffectStyle;

	// Token: 0x0400066F RID: 1647
	[HideInInspector]
	[SerializeField]
	private Color mEffectColor = Color.black;

	// Token: 0x04000670 RID: 1648
	[HideInInspector]
	[SerializeField]
	private NGUIText.SymbolStyle mSymbols = NGUIText.SymbolStyle.Normal;

	// Token: 0x04000671 RID: 1649
	[HideInInspector]
	[SerializeField]
	private Vector2 mEffectDistance = Vector2.one;

	// Token: 0x04000672 RID: 1650
	[SerializeField]
	[HideInInspector]
	private UILabel.Overflow mOverflow = UILabel.Overflow.ResizeHeight;

	// Token: 0x04000673 RID: 1651
	[HideInInspector]
	[SerializeField]
	private bool mApplyGradient;

	// Token: 0x04000674 RID: 1652
	[SerializeField]
	[HideInInspector]
	private Color mGradientTop = Color.white;

	// Token: 0x04000675 RID: 1653
	[SerializeField]
	[HideInInspector]
	private Color mGradientBottom = new Color(0.7f, 0.7f, 0.7f);

	// Token: 0x04000676 RID: 1654
	[SerializeField]
	[HideInInspector]
	private int mSpacingX;

	// Token: 0x04000677 RID: 1655
	[SerializeField]
	[HideInInspector]
	private int mSpacingY;

	// Token: 0x04000678 RID: 1656
	[HideInInspector]
	[SerializeField]
	private bool mUseFloatSpacing;

	// Token: 0x04000679 RID: 1657
	[SerializeField]
	[HideInInspector]
	private float mFloatSpacingX;

	// Token: 0x0400067A RID: 1658
	[SerializeField]
	[HideInInspector]
	private float mFloatSpacingY;

	// Token: 0x0400067B RID: 1659
	[HideInInspector]
	[SerializeField]
	private bool mOverflowEllipsis;

	// Token: 0x0400067C RID: 1660
	[SerializeField]
	[HideInInspector]
	private int mOverflowWidth;

	// Token: 0x0400067D RID: 1661
	[HideInInspector]
	[SerializeField]
	private int mOverflowHeight;

	// Token: 0x0400067E RID: 1662
	[SerializeField]
	[HideInInspector]
	private UILabel.Modifier mModifier;

	// Token: 0x0400067F RID: 1663
	[SerializeField]
	[HideInInspector]
	private bool mShrinkToFit;

	// Token: 0x04000680 RID: 1664
	[HideInInspector]
	[SerializeField]
	private int mMaxLineWidth;

	// Token: 0x04000681 RID: 1665
	[HideInInspector]
	[SerializeField]
	private int mMaxLineHeight;

	// Token: 0x04000682 RID: 1666
	[HideInInspector]
	[SerializeField]
	private float mLineWidth;

	// Token: 0x04000683 RID: 1667
	[SerializeField]
	[HideInInspector]
	private bool mMultiline = true;

	// Token: 0x04000684 RID: 1668
	private Font mActiveTTF;

	// Token: 0x04000685 RID: 1669
	private float mDensity = 1f;

	// Token: 0x04000686 RID: 1670
	private bool mShouldBeProcessed = true;

	// Token: 0x04000687 RID: 1671
	private string mProcessedText;

	// Token: 0x04000688 RID: 1672
	private bool mPremultiply;

	// Token: 0x04000689 RID: 1673
	private Vector2 mCalculatedSize = Vector2.zero;

	// Token: 0x0400068A RID: 1674
	private float mScale = 1f;

	// Token: 0x0400068B RID: 1675
	private int mFinalFontSize;

	// Token: 0x0400068C RID: 1676
	private int mLastWidth;

	// Token: 0x0400068D RID: 1677
	private int mLastHeight;

	// Token: 0x0400068E RID: 1678
	public UILabel.ModifierFunc customModifier;

	// Token: 0x0400068F RID: 1679
	private static BetterList<UILabel> mList = new BetterList<UILabel>();

	// Token: 0x04000690 RID: 1680
	private static Dictionary<Font, int> mFontUsage = new Dictionary<Font, int>();

	// Token: 0x04000691 RID: 1681
	[NonSerialized]
	private static BetterList<UIDrawCall> mTempDrawcalls;

	// Token: 0x04000692 RID: 1682
	private static bool mTexRebuildAdded = false;

	// Token: 0x04000693 RID: 1683
	private static List<Vector3> mTempVerts = new List<Vector3>();

	// Token: 0x04000694 RID: 1684
	private static List<int> mTempIndices = new List<int>();

	// Token: 0x02000111 RID: 273
	[DoNotObfuscateNGUI]
	public enum Effect
	{
		// Token: 0x04000697 RID: 1687
		None,
		// Token: 0x04000698 RID: 1688
		Shadow,
		// Token: 0x04000699 RID: 1689
		Outline,
		// Token: 0x0400069A RID: 1690
		Outline8
	}

	// Token: 0x02000112 RID: 274
	[DoNotObfuscateNGUI]
	public enum Overflow
	{
		// Token: 0x0400069C RID: 1692
		ShrinkContent,
		// Token: 0x0400069D RID: 1693
		ClampContent,
		// Token: 0x0400069E RID: 1694
		ResizeFreely,
		// Token: 0x0400069F RID: 1695
		ResizeHeight
	}

	// Token: 0x02000113 RID: 275
	[DoNotObfuscateNGUI]
	public enum Crispness
	{
		// Token: 0x040006A1 RID: 1697
		Never,
		// Token: 0x040006A2 RID: 1698
		OnDesktop,
		// Token: 0x040006A3 RID: 1699
		Always
	}

	// Token: 0x02000114 RID: 276
	[DoNotObfuscateNGUI]
	public enum Modifier
	{
		// Token: 0x040006A5 RID: 1701
		None,
		// Token: 0x040006A6 RID: 1702
		ToUppercase,
		// Token: 0x040006A7 RID: 1703
		ToLowercase,
		// Token: 0x040006A8 RID: 1704
		Custom = 255
	}

	// Token: 0x02000115 RID: 277
	// (Invoke) Token: 0x0600099A RID: 2458
	public delegate string ModifierFunc(string s);
}
