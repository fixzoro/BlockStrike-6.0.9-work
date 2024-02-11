using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Token: 0x02000041 RID: 65
[AddComponentMenu("NGUI/Interaction/Typewriter Effect")]
[RequireComponent(typeof(UILabel))]
public class TypewriterEffect : MonoBehaviour
{
	// Token: 0x1700000E RID: 14
	// (get) Token: 0x060001D3 RID: 467 RVA: 0x00005EED File Offset: 0x000040ED
	public bool isActive
	{
		get
		{
			return this.mActive;
		}
	}

	// Token: 0x060001D4 RID: 468 RVA: 0x00005EF5 File Offset: 0x000040F5
	public void ResetToBeginning()
	{
		this.Finish();
		this.mReset = true;
		this.mActive = true;
		this.mNextChar = 0f;
		this.mCurrentOffset = 0;
		this.Update();
	}

	// Token: 0x060001D5 RID: 469 RVA: 0x00024504 File Offset: 0x00022704
	public void Finish()
	{
		if (this.mActive)
		{
			this.mActive = false;
			if (!this.mReset)
			{
				this.mCurrentOffset = this.mFullText.Length;
				this.mFade.Clear();
				this.mLabel.text = this.mFullText;
			}
			if (this.keepFullDimensions && this.scrollView != null)
			{
				this.scrollView.UpdatePosition();
			}
			TypewriterEffect.current = this;
			EventDelegate.Execute(this.onFinished);
			TypewriterEffect.current = null;
		}
	}

	// Token: 0x060001D6 RID: 470 RVA: 0x00005F23 File Offset: 0x00004123
	private void OnEnable()
	{
		this.mReset = true;
		this.mActive = true;
	}

	// Token: 0x060001D7 RID: 471 RVA: 0x00005F33 File Offset: 0x00004133
	private void OnDisable()
	{
		this.Finish();
	}

	// Token: 0x060001D8 RID: 472 RVA: 0x0002459C File Offset: 0x0002279C
	private void Update()
	{
		if (!this.mActive)
		{
			return;
		}
		if (this.mReset)
		{
			this.mCurrentOffset = 0;
			this.mReset = false;
			this.mLabel = base.GetComponent<UILabel>();
			this.mFullText = this.mLabel.processedText;
			this.mFade.Clear();
			if (this.keepFullDimensions && this.scrollView != null)
			{
				this.scrollView.UpdatePosition();
			}
		}
		if (string.IsNullOrEmpty(this.mFullText))
		{
			return;
		}
		int length = this.mFullText.Length;
		while (this.mCurrentOffset < length && this.mNextChar <= RealTime.time)
		{
			int num = this.mCurrentOffset;
			this.charsPerSecond = Mathf.Max(1, this.charsPerSecond);
			if (this.mLabel.supportEncoding)
			{
				while (NGUIText.ParseSymbol(this.mFullText, ref this.mCurrentOffset))
				{
				}
			}
			this.mCurrentOffset++;
			if (this.mCurrentOffset > length)
			{
				break;
			}
			float num2 = 1f / (float)this.charsPerSecond;
			char c = (num >= length) ? '\n' : this.mFullText[num];
			if (c == '\n')
			{
				num2 += this.delayOnNewLine;
			}
			else if (num + 1 == length || this.mFullText[num + 1] <= ' ')
			{
				if (c == '.')
				{
					if (num + 2 < length && this.mFullText[num + 1] == '.' && this.mFullText[num + 2] == '.')
					{
						num2 += this.delayOnPeriod * 3f;
						num += 2;
					}
					else
					{
						num2 += this.delayOnPeriod;
					}
				}
				else if (c == '!' || c == '?')
				{
					num2 += this.delayOnPeriod;
				}
			}
			if (this.mNextChar == 0f)
			{
				this.mNextChar = RealTime.time + num2;
			}
			else
			{
				this.mNextChar += num2;
			}
			if (this.fadeInTime != 0f)
			{
				TypewriterEffect.FadeEntry item = default(TypewriterEffect.FadeEntry);
				item.index = num;
				item.alpha = 0f;
				item.text = this.mFullText.Substring(num, this.mCurrentOffset - num);
				this.mFade.Add(item);
			}
			else
			{
				this.mLabel.text = ((!this.keepFullDimensions) ? this.mFullText.Substring(0, this.mCurrentOffset) : (this.mFullText.Substring(0, this.mCurrentOffset) + "[00]" + this.mFullText.Substring(this.mCurrentOffset)));
				if (!this.keepFullDimensions && this.scrollView != null)
				{
					this.scrollView.UpdatePosition();
				}
			}
		}
		if (this.mCurrentOffset >= length && this.mFade.size == 0)
		{
			this.mLabel.text = this.mFullText;
			TypewriterEffect.current = this;
			EventDelegate.Execute(this.onFinished);
			TypewriterEffect.current = null;
			this.mActive = false;
		}
		else if (this.mFade.size != 0)
		{
			int i = 0;
			while (i < this.mFade.size)
			{
				TypewriterEffect.FadeEntry value = this.mFade[i];
				value.alpha += RealTime.deltaTime / this.fadeInTime;
				if (value.alpha < 1f)
				{
					this.mFade[i] = value;
					i++;
				}
				else
				{
					this.mFade.RemoveAt(i);
				}
			}
			if (this.mFade.size == 0)
			{
				if (this.keepFullDimensions)
				{
					this.mLabel.text = this.mFullText.Substring(0, this.mCurrentOffset) + "[00]" + this.mFullText.Substring(this.mCurrentOffset);
				}
				else
				{
					this.mLabel.text = this.mFullText.Substring(0, this.mCurrentOffset);
				}
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int j = 0; j < this.mFade.size; j++)
				{
					TypewriterEffect.FadeEntry fadeEntry = this.mFade[j];
					if (j == 0)
					{
						stringBuilder.Append(this.mFullText.Substring(0, fadeEntry.index));
					}
					stringBuilder.Append('[');
					stringBuilder.Append(NGUIText.EncodeAlpha(fadeEntry.alpha));
					stringBuilder.Append(']');
					stringBuilder.Append(fadeEntry.text);
				}
				if (this.keepFullDimensions)
				{
					stringBuilder.Append("[00]");
					stringBuilder.Append(this.mFullText.Substring(this.mCurrentOffset));
				}
				this.mLabel.text = stringBuilder.ToString();
			}
		}
	}

	// Token: 0x040000E0 RID: 224
	public static TypewriterEffect current;

	// Token: 0x040000E1 RID: 225
	public int charsPerSecond = 20;

	// Token: 0x040000E2 RID: 226
	public float fadeInTime;

	// Token: 0x040000E3 RID: 227
	public float delayOnPeriod;

	// Token: 0x040000E4 RID: 228
	public float delayOnNewLine;

	// Token: 0x040000E5 RID: 229
	public UIScrollView scrollView;

	// Token: 0x040000E6 RID: 230
	public bool keepFullDimensions;

	// Token: 0x040000E7 RID: 231
	public List<EventDelegate> onFinished = new List<EventDelegate>();

	// Token: 0x040000E8 RID: 232
	private UILabel mLabel;

	// Token: 0x040000E9 RID: 233
	private string mFullText = string.Empty;

	// Token: 0x040000EA RID: 234
	private int mCurrentOffset;

	// Token: 0x040000EB RID: 235
	private float mNextChar;

	// Token: 0x040000EC RID: 236
	private bool mReset = true;

	// Token: 0x040000ED RID: 237
	private bool mActive;

	// Token: 0x040000EE RID: 238
	private BetterList<TypewriterEffect.FadeEntry> mFade = new BetterList<TypewriterEffect.FadeEntry>();

	// Token: 0x02000042 RID: 66
	private struct FadeEntry
	{
		// Token: 0x040000EF RID: 239
		public int index;

		// Token: 0x040000F0 RID: 240
		public string text;

		// Token: 0x040000F1 RID: 241
		public float alpha;
	}
}
