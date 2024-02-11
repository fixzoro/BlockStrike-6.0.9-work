using System;
using UnityEngine;

// Token: 0x02000044 RID: 68
[AddComponentMenu("NGUI/Interaction/Button Activate")]
public class UIButtonActivate : MonoBehaviour
{
	// Token: 0x060001E9 RID: 489 RVA: 0x0000603A File Offset: 0x0000423A
	private void OnClick()
	{
		if (this.target != null)
		{
			NGUITools.SetActive(this.target, this.state);
		}
	}

	// Token: 0x04000100 RID: 256
	public GameObject target;

	// Token: 0x04000101 RID: 257
	public bool state = true;
}
