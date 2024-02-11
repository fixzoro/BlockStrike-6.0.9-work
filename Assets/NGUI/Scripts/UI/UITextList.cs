using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Token: 0x0200012C RID: 300
[AddComponentMenu("NGUI/UI/Text List")]
public class UITextList : MonoBehaviour
{
	// Token: 0x170001D1 RID: 465
	// (get) Token: 0x06000A88 RID: 2696 RVA: 0x0005A2E8 File Offset: 0x000584E8
	protected BetterList<UITextList.Paragraph> paragraphs
	{
		get
		{
			if (this.mParagraphs == null && !UITextList.mHistory.TryGetValue(base.name, out this.mParagraphs))
			{
				this.mParagraphs = new BetterList<UITextList.Paragraph>();
				UITextList.mHistory.Add(base.name, this.mParagraphs);
			}
			return this.mParagraphs;
		}
	}

	// Token: 0x170001D2 RID: 466
	// (get) Token: 0x06000A89 RID: 2697 RVA: 0x0000BA73 File Offset: 0x00009C73
	public int paragraphCount
	{
		get
		{
			return this.paragraphs.size;
		}
	}

	// Token: 0x170001D3 RID: 467
	// (get) Token: 0x06000A8A RID: 2698 RVA: 0x0000BA80 File Offset: 0x00009C80
	public bool isValid
	{
		get
		{
			return this.textLabel != null && this.textLabel.trueTypeFont != null;
		}
	}

	// Token: 0x170001D4 RID: 468
	// (get) Token: 0x06000A8B RID: 2699 RVA: 0x0000BAA7 File Offset: 0x00009CA7
	// (set) Token: 0x06000A8C RID: 2700 RVA: 0x0005A344 File Offset: 0x00058544
	public float scrollValue
	{
		get
		{
			return this.mScroll;
		}
		set
		{
			value = Mathf.Clamp01(value);
			if (this.isValid && this.mScroll != value)
			{
				if (this.scrollBar != null)
				{
					this.scrollBar.value = value;
				}
				else
				{
					this.mScroll = value;
					this.UpdateVisibleText();
				}
			}
		}
	}

	// Token: 0x170001D5 RID: 469
	// (get) Token: 0x06000A8D RID: 2701 RVA: 0x0000BAAF File Offset: 0x00009CAF
	protected float lineHeight
	{
		get
		{
			return (!(this.textLabel != null)) ? 20f : ((float)this.textLabel.fontSize + this.textLabel.effectiveSpacingY);
		}
	}

	// Token: 0x170001D6 RID: 470
	// (get) Token: 0x06000A8E RID: 2702 RVA: 0x0005A3A0 File Offset: 0x000585A0
	protected int scrollHeight
	{
		get
		{
			if (!this.isValid)
			{
				return 0;
			}
			int num = Mathf.FloorToInt((float)this.textLabel.height / this.lineHeight);
			return Mathf.Max(0, this.mTotalLines - num);
		}
	}

	// Token: 0x06000A8F RID: 2703 RVA: 0x0000BAE4 File Offset: 0x00009CE4
	public void Clear()
	{
		this.paragraphs.Clear();
		this.UpdateVisibleText();
	}

	// Token: 0x06000A90 RID: 2704 RVA: 0x0005A3E4 File Offset: 0x000585E4
	private void Start()
	{
		if (this.textLabel == null)
		{
			this.textLabel = base.GetComponentInChildren<UILabel>();
		}
		if (this.scrollBar != null)
		{
			EventDelegate.Add(this.scrollBar.onChange, new EventDelegate.Callback(this.OnScrollBar));
		}
		this.textLabel.overflowMethod = UILabel.Overflow.ClampContent;
		if (this.style == UITextList.Style.Chat)
		{
			this.textLabel.pivot = UIWidget.Pivot.BottomLeft;
			this.scrollValue = 1f;
		}
		else
		{
			this.textLabel.pivot = UIWidget.Pivot.TopLeft;
			this.scrollValue = 0f;
		}
	}

	// Token: 0x06000A91 RID: 2705 RVA: 0x0000BAF7 File Offset: 0x00009CF7
	private void Update()
	{
		if (this.isValid && (this.textLabel.width != this.mLastWidth || this.textLabel.height != this.mLastHeight))
		{
			this.Rebuild();
		}
	}

	// Token: 0x06000A92 RID: 2706 RVA: 0x0005A488 File Offset: 0x00058688
	public void OnScroll(float val)
	{
		int scrollHeight = this.scrollHeight;
		if (scrollHeight != 0)
		{
			val *= this.lineHeight;
			this.scrollValue = this.mScroll - val / (float)scrollHeight;
		}
	}

	// Token: 0x06000A93 RID: 2707 RVA: 0x0005A4C0 File Offset: 0x000586C0
	public void OnDrag(Vector2 delta)
	{
		int scrollHeight = this.scrollHeight;
		if (scrollHeight != 0)
		{
			float num = delta.y / this.lineHeight;
			this.scrollValue = this.mScroll + num / (float)scrollHeight;
		}
	}

	// Token: 0x06000A94 RID: 2708 RVA: 0x0000BB36 File Offset: 0x00009D36
	private void OnScrollBar()
	{
		this.mScroll = UIProgressBar.current.value;
		this.UpdateVisibleText();
	}

	// Token: 0x06000A95 RID: 2709 RVA: 0x0000BB4E File Offset: 0x00009D4E
	public void Add(string text)
	{
		this.Add(text, true);
	}

	// Token: 0x06000A96 RID: 2710 RVA: 0x0005A4FC File Offset: 0x000586FC
	protected void Add(string text, bool updateVisible)
	{
		UITextList.Paragraph paragraph;
		if (this.paragraphs.size < this.paragraphHistory)
		{
			paragraph = new UITextList.Paragraph();
		}
		else
		{
			paragraph = this.mParagraphs[0];
			this.mParagraphs.RemoveAt(0);
		}
		paragraph.text = text;
		this.mParagraphs.Add(paragraph);
		this.Rebuild();
	}

	// Token: 0x06000A97 RID: 2711 RVA: 0x0005A560 File Offset: 0x00058760
	protected void Rebuild()
	{
		if (this.isValid)
		{
			this.mLastWidth = this.textLabel.width;
			this.mLastHeight = this.textLabel.height;
			this.textLabel.UpdateNGUIText();
			NGUIText.rectHeight = 1000000;
			NGUIText.regionHeight = 1000000;
			this.mTotalLines = 0;
			for (int i = 0; i < this.paragraphs.size; i++)
			{
				UITextList.Paragraph paragraph = this.mParagraphs.buffer[i];
				string text;
				NGUIText.WrapText(paragraph.text, out text, false, true, false);
				paragraph.lines = text.Split(new char[]
				{
					'\n'
				});
				this.mTotalLines += paragraph.lines.Length;
			}
			this.mTotalLines = 0;
			int j = 0;
			int size = this.mParagraphs.size;
			while (j < size)
			{
				this.mTotalLines += this.mParagraphs.buffer[j].lines.Length;
				j++;
			}
			if (this.scrollBar != null)
			{
				UIScrollBar uiscrollBar = this.scrollBar as UIScrollBar;
				if (uiscrollBar != null)
				{
					uiscrollBar.barSize = ((this.mTotalLines != 0) ? (1f - (float)this.scrollHeight / (float)this.mTotalLines) : 1f);
				}
			}
			this.UpdateVisibleText();
		}
	}

	// Token: 0x06000A98 RID: 2712 RVA: 0x0005A6D4 File Offset: 0x000588D4
	protected void UpdateVisibleText()
	{
		if (this.isValid)
		{
			if (this.mTotalLines == 0)
			{
				this.textLabel.text = string.Empty;
				return;
			}
			int num = Mathf.FloorToInt((float)this.textLabel.height / this.lineHeight);
			int num2 = Mathf.Max(0, this.mTotalLines - num);
			int num3 = Mathf.RoundToInt(this.mScroll * (float)num2);
			if (num3 < 0)
			{
				num3 = 0;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num4 = 0;
			int size = this.paragraphs.size;
			while (num > 0 && num4 < size)
			{
				UITextList.Paragraph paragraph = this.mParagraphs.buffer[num4];
				int num5 = 0;
				int num6 = paragraph.lines.Length;
				while (num > 0 && num5 < num6)
				{
					string value = paragraph.lines[num5];
					if (num3 > 0)
					{
						num3--;
					}
					else
					{
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append("\n");
						}
						stringBuilder.Append(value);
						num--;
					}
					num5++;
				}
				num4++;
			}
			this.textLabel.text = stringBuilder.ToString();
		}
	}

	// Token: 0x04000740 RID: 1856
	public UILabel textLabel;

	// Token: 0x04000741 RID: 1857
	public UIProgressBar scrollBar;

	// Token: 0x04000742 RID: 1858
	public UITextList.Style style;

	// Token: 0x04000743 RID: 1859
	public int paragraphHistory = 100;

	// Token: 0x04000744 RID: 1860
	protected char[] mSeparator = new char[]
	{
		'\n'
	};

	// Token: 0x04000745 RID: 1861
	protected float mScroll;

	// Token: 0x04000746 RID: 1862
	protected int mTotalLines;

	// Token: 0x04000747 RID: 1863
	protected int mLastWidth;

	// Token: 0x04000748 RID: 1864
	protected int mLastHeight;

	// Token: 0x04000749 RID: 1865
	private BetterList<UITextList.Paragraph> mParagraphs;

	// Token: 0x0400074A RID: 1866
	private static Dictionary<string, BetterList<UITextList.Paragraph>> mHistory = new Dictionary<string, BetterList<UITextList.Paragraph>>();

	// Token: 0x0200012D RID: 301
	[DoNotObfuscateNGUI]
	public enum Style
	{
		// Token: 0x0400074C RID: 1868
		Text,
		// Token: 0x0400074D RID: 1869
		Chat
	}

	// Token: 0x0200012E RID: 302
	protected class Paragraph
	{
		// Token: 0x0400074E RID: 1870
		public string text;

		// Token: 0x0400074F RID: 1871
		public string[] lines;
	}
}
