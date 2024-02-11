using System;
using UnityEngine;

// Token: 0x020000CA RID: 202
[ExecuteInEditMode]
public class AnimatedAlpha : MonoBehaviour
{
	// Token: 0x0600069F RID: 1695 RVA: 0x000094A6 File Offset: 0x000076A6
	private void OnEnable()
	{
		this.mWidget = base.GetComponent<UIWidget>();
		this.mPanel = base.GetComponent<UIPanel>();
		this.LateUpdate();
	}

	// Token: 0x060006A0 RID: 1696 RVA: 0x00046510 File Offset: 0x00044710
	private void LateUpdate()
	{
		if (this.mWidget != null)
		{
			this.mWidget.alpha = this.alpha;
		}
		if (this.mPanel != null)
		{
			this.mPanel.alpha = this.alpha;
		}
	}

	// Token: 0x04000496 RID: 1174
	[Range(0f, 1f)]
	public float alpha = 1f;

	// Token: 0x04000497 RID: 1175
	private UIWidget mWidget;

	// Token: 0x04000498 RID: 1176
	private UIPanel mPanel;
}
