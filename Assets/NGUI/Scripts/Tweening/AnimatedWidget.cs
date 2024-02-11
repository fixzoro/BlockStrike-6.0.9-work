using System;
using UnityEngine;

// Token: 0x020000CC RID: 204
[ExecuteInEditMode]
public class AnimatedWidget : MonoBehaviour
{
	// Token: 0x060006A5 RID: 1701 RVA: 0x0000951E File Offset: 0x0000771E
	private void OnEnable()
	{
		this.mWidget = base.GetComponent<UIWidget>();
		this.LateUpdate();
	}

	// Token: 0x060006A6 RID: 1702 RVA: 0x00009532 File Offset: 0x00007732
	private void LateUpdate()
	{
		if (this.mWidget != null)
		{
			this.mWidget.width = Mathf.RoundToInt(this.width);
			this.mWidget.height = Mathf.RoundToInt(this.height);
		}
	}

	// Token: 0x0400049B RID: 1179
	public float width = 1f;

	// Token: 0x0400049C RID: 1180
	public float height = 1f;

	// Token: 0x0400049D RID: 1181
	private UIWidget mWidget;
}
