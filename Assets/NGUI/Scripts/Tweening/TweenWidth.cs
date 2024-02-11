using System;
using UnityEngine;

// Token: 0x020000DE RID: 222
[RequireComponent(typeof(UIWidget))]
[AddComponentMenu("NGUI/Tween/Tween Width")]
public class TweenWidth : UITweener
{
	// Token: 0x170000D8 RID: 216
	// (get) Token: 0x06000734 RID: 1844 RVA: 0x00009D34 File Offset: 0x00007F34
	public UIWidget cachedWidget
	{
		get
		{
			if (this.mWidget == null)
			{
				this.mWidget = base.GetComponent<UIWidget>();
			}
			return this.mWidget;
		}
	}

	// Token: 0x170000D9 RID: 217
	// (get) Token: 0x06000735 RID: 1845 RVA: 0x00009D59 File Offset: 0x00007F59
	// (set) Token: 0x06000736 RID: 1846 RVA: 0x00009D61 File Offset: 0x00007F61
	[Obsolete("Use 'value' instead")]
	public int width
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

	// Token: 0x170000DA RID: 218
	// (get) Token: 0x06000737 RID: 1847 RVA: 0x00009D6A File Offset: 0x00007F6A
	// (set) Token: 0x06000738 RID: 1848 RVA: 0x00009D77 File Offset: 0x00007F77
	public int value
	{
		get
		{
			return this.cachedWidget.width;
		}
		set
		{
			this.cachedWidget.width = value;
		}
	}

	// Token: 0x06000739 RID: 1849 RVA: 0x00047AAC File Offset: 0x00045CAC
	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.value = Mathf.RoundToInt((float)this.from * (1f - factor) + (float)this.to * factor);
		if (this.updateTable)
		{
			if (this.mTable == null)
			{
				this.mTable = NGUITools.FindInParents<UITable>(base.gameObject);
				if (this.mTable == null)
				{
					this.updateTable = false;
					return;
				}
			}
			this.mTable.repositionNow = true;
		}
	}

	// Token: 0x0600073A RID: 1850 RVA: 0x00047B30 File Offset: 0x00045D30
	public static TweenWidth Begin(UIWidget widget, float duration, int width)
	{
		TweenWidth tweenWidth = UITweener.Begin<TweenWidth>(widget.gameObject, duration, 0f);
		tweenWidth.from = widget.width;
		tweenWidth.to = width;
		if (duration <= 0f)
		{
			tweenWidth.Sample(1f, true);
			tweenWidth.enabled = false;
		}
		return tweenWidth;
	}

	// Token: 0x0600073B RID: 1851 RVA: 0x00009D85 File Offset: 0x00007F85
	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue()
	{
		this.from = this.value;
	}

	// Token: 0x0600073C RID: 1852 RVA: 0x00009D93 File Offset: 0x00007F93
	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue()
	{
		this.to = this.value;
	}

	// Token: 0x0600073D RID: 1853 RVA: 0x00009DA1 File Offset: 0x00007FA1
	[ContextMenu("Assume value of 'From'")]
	private void SetCurrentValueToStart()
	{
		this.value = this.from;
	}

	// Token: 0x0600073E RID: 1854 RVA: 0x00009DAF File Offset: 0x00007FAF
	[ContextMenu("Assume value of 'To'")]
	private void SetCurrentValueToEnd()
	{
		this.value = this.to;
	}

	// Token: 0x040004F7 RID: 1271
	public int from = 100;

	// Token: 0x040004F8 RID: 1272
	public int to = 100;

	// Token: 0x040004F9 RID: 1273
	public bool updateTable;

	// Token: 0x040004FA RID: 1274
	private UIWidget mWidget;

	// Token: 0x040004FB RID: 1275
	private UITable mTable;
}
