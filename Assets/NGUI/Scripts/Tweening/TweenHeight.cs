using System;
using UnityEngine;

// Token: 0x020000D3 RID: 211
[AddComponentMenu("NGUI/Tween/Tween Height")]
[RequireComponent(typeof(UIWidget))]
public class TweenHeight : UITweener
{
	// Token: 0x170000C6 RID: 198
	// (get) Token: 0x060006DB RID: 1755 RVA: 0x00009856 File Offset: 0x00007A56
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

	// Token: 0x170000C7 RID: 199
	// (get) Token: 0x060006DC RID: 1756 RVA: 0x0000987B File Offset: 0x00007A7B
	// (set) Token: 0x060006DD RID: 1757 RVA: 0x00009883 File Offset: 0x00007A83
	[Obsolete("Use 'value' instead")]
	public int height
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

	// Token: 0x170000C8 RID: 200
	// (get) Token: 0x060006DE RID: 1758 RVA: 0x0000988C File Offset: 0x00007A8C
	// (set) Token: 0x060006DF RID: 1759 RVA: 0x00009899 File Offset: 0x00007A99
	public int value
	{
		get
		{
			return this.cachedWidget.height;
		}
		set
		{
			this.cachedWidget.height = value;
		}
	}

	// Token: 0x060006E0 RID: 1760 RVA: 0x00046D0C File Offset: 0x00044F0C
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

	// Token: 0x060006E1 RID: 1761 RVA: 0x00046D90 File Offset: 0x00044F90
	public static TweenHeight Begin(UIWidget widget, float duration, int height)
	{
		TweenHeight tweenHeight = UITweener.Begin<TweenHeight>(widget.gameObject, duration, 0f);
		tweenHeight.from = widget.height;
		tweenHeight.to = height;
		if (duration <= 0f)
		{
			tweenHeight.Sample(1f, true);
			tweenHeight.enabled = false;
		}
		return tweenHeight;
	}

	// Token: 0x060006E2 RID: 1762 RVA: 0x000098A7 File Offset: 0x00007AA7
	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue()
	{
		this.from = this.value;
	}

	// Token: 0x060006E3 RID: 1763 RVA: 0x000098B5 File Offset: 0x00007AB5
	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue()
	{
		this.to = this.value;
	}

	// Token: 0x060006E4 RID: 1764 RVA: 0x000098C3 File Offset: 0x00007AC3
	[ContextMenu("Assume value of 'From'")]
	private void SetCurrentValueToStart()
	{
		this.value = this.from;
	}

	// Token: 0x060006E5 RID: 1765 RVA: 0x000098D1 File Offset: 0x00007AD1
	[ContextMenu("Assume value of 'To'")]
	private void SetCurrentValueToEnd()
	{
		this.value = this.to;
	}

	// Token: 0x040004C0 RID: 1216
	public int from = 100;

	// Token: 0x040004C1 RID: 1217
	public int to = 100;

	// Token: 0x040004C2 RID: 1218
	public bool updateTable;

	// Token: 0x040004C3 RID: 1219
	private UIWidget mWidget;

	// Token: 0x040004C4 RID: 1220
	private UITable mTable;
}
