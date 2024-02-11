using System;
using UnityEngine;

// Token: 0x020000DB RID: 219
[AddComponentMenu("NGUI/Tween/Tween Scale")]
public class TweenScale : UITweener
{
	// Token: 0x170000D2 RID: 210
	// (get) Token: 0x0600071A RID: 1818 RVA: 0x00009BBA File Offset: 0x00007DBA
	public Transform cachedTransform
	{
		get
		{
			if (this.mTrans == null)
			{
				this.mTrans = base.transform;
			}
			return this.mTrans;
		}
	}

	// Token: 0x170000D3 RID: 211
	// (get) Token: 0x0600071B RID: 1819 RVA: 0x00009BDF File Offset: 0x00007DDF
	// (set) Token: 0x0600071C RID: 1820 RVA: 0x00009BEC File Offset: 0x00007DEC
	public Vector3 value
	{
		get
		{
			return this.cachedTransform.localScale;
		}
		set
		{
			this.cachedTransform.localScale = value;
		}
	}

	// Token: 0x170000D4 RID: 212
	// (get) Token: 0x0600071D RID: 1821 RVA: 0x00009BFA File Offset: 0x00007DFA
	// (set) Token: 0x0600071E RID: 1822 RVA: 0x00009C02 File Offset: 0x00007E02
	[Obsolete("Use 'value' instead")]
	public Vector3 scale
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

	// Token: 0x0600071F RID: 1823 RVA: 0x00047708 File Offset: 0x00045908
	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.value = this.from * (1f - factor) + this.to * factor;
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

	// Token: 0x06000720 RID: 1824 RVA: 0x00047790 File Offset: 0x00045990
	public static TweenScale Begin(GameObject go, float duration, Vector3 scale)
	{
		TweenScale tweenScale = UITweener.Begin<TweenScale>(go, duration, 0f);
		tweenScale.from = tweenScale.value;
		tweenScale.to = scale;
		if (duration <= 0f)
		{
			tweenScale.Sample(1f, true);
			tweenScale.enabled = false;
		}
		return tweenScale;
	}

	// Token: 0x06000721 RID: 1825 RVA: 0x00009C0B File Offset: 0x00007E0B
	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue()
	{
		this.from = this.value;
	}

	// Token: 0x06000722 RID: 1826 RVA: 0x00009C19 File Offset: 0x00007E19
	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue()
	{
		this.to = this.value;
	}

	// Token: 0x06000723 RID: 1827 RVA: 0x00009C27 File Offset: 0x00007E27
	[ContextMenu("Assume value of 'From'")]
	private void SetCurrentValueToStart()
	{
		this.value = this.from;
	}

	// Token: 0x06000724 RID: 1828 RVA: 0x00009C35 File Offset: 0x00007E35
	[ContextMenu("Assume value of 'To'")]
	private void SetCurrentValueToEnd()
	{
		this.value = this.to;
	}

	// Token: 0x040004E8 RID: 1256
	public Vector3 from = Vector3.one;

	// Token: 0x040004E9 RID: 1257
	public Vector3 to = Vector3.one;

	// Token: 0x040004EA RID: 1258
	public bool updateTable;

	// Token: 0x040004EB RID: 1259
	private Transform mTrans;

	// Token: 0x040004EC RID: 1260
	private UITable mTable;
}
