using System;
using UnityEngine;

// Token: 0x0200003F RID: 63
[AddComponentMenu("NGUI/Interaction/Envelop Content")]
[RequireComponent(typeof(UIWidget))]
public class EnvelopContent : MonoBehaviour
{
	// Token: 0x060001C8 RID: 456 RVA: 0x00005E1C File Offset: 0x0000401C
	private void Start()
	{
		this.mStarted = true;
		this.Execute();
	}

	// Token: 0x060001C9 RID: 457 RVA: 0x00005E2B File Offset: 0x0000402B
	private void OnEnable()
	{
		if (this.mStarted)
		{
			this.Execute();
		}
	}

	// Token: 0x060001CA RID: 458 RVA: 0x00024380 File Offset: 0x00022580
	[ContextMenu("Execute")]
	public void Execute()
	{
		if (this.targetRoot == base.transform)
		{
			Debug.LogError("Target Root object cannot be the same object that has Envelop Content. Make it a sibling instead.", this);
		}
		else if (NGUITools.IsChild(this.targetRoot, base.transform))
		{
			Debug.LogError("Target Root object should not be a parent of Envelop Content. Make it a sibling instead.", this);
		}
		else
		{
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(base.transform.parent, this.targetRoot, !this.ignoreDisabled, true);
			float num = bounds.min.x + (float)this.padLeft;
			float num2 = bounds.min.y + (float)this.padBottom;
			float num3 = bounds.max.x + (float)this.padRight;
			float num4 = bounds.max.y + (float)this.padTop;
			UIWidget component = base.GetComponent<UIWidget>();
			component.SetRect(num, num2, num3 - num, num4 - num2);
			base.BroadcastMessage("UpdateAnchors", SendMessageOptions.DontRequireReceiver);
			NGUITools.UpdateWidgetCollider(base.gameObject);
		}
	}

	// Token: 0x040000D6 RID: 214
	public Transform targetRoot;

	// Token: 0x040000D7 RID: 215
	public int padLeft;

	// Token: 0x040000D8 RID: 216
	public int padRight;

	// Token: 0x040000D9 RID: 217
	public int padBottom;

	// Token: 0x040000DA RID: 218
	public int padTop;

	// Token: 0x040000DB RID: 219
	public bool ignoreDisabled = true;

	// Token: 0x040000DC RID: 220
	private bool mStarted;
}
