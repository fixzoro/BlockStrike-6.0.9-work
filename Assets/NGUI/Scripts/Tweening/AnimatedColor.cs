using System;
using UnityEngine;

// Token: 0x020000CB RID: 203
[ExecuteInEditMode]
[RequireComponent(typeof(UIWidget))]
public class AnimatedColor : MonoBehaviour
{
	// Token: 0x060006A2 RID: 1698 RVA: 0x000094D9 File Offset: 0x000076D9
	private void OnEnable()
	{
		this.mWidget = base.GetComponent<UIWidget>();
		this.LateUpdate();
	}

	// Token: 0x060006A3 RID: 1699 RVA: 0x000094ED File Offset: 0x000076ED
	private void LateUpdate()
	{
		this.mWidget.color = this.color;
	}

	// Token: 0x04000499 RID: 1177
	public Color color = Color.white;

	// Token: 0x0400049A RID: 1178
	private UIWidget mWidget;
}
