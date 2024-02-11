using System;
using UnityEngine;

// Token: 0x02000054 RID: 84
[AddComponentMenu("NGUI/Interaction/Drag and Drop Root")]
public class UIDragDropRoot : MonoBehaviour
{
	// Token: 0x06000251 RID: 593 RVA: 0x00006632 File Offset: 0x00004832
	private void OnEnable()
	{
		UIDragDropRoot.root = base.transform;
	}

	// Token: 0x06000252 RID: 594 RVA: 0x0000663F File Offset: 0x0000483F
	private void OnDisable()
	{
		if (UIDragDropRoot.root == base.transform)
		{
			UIDragDropRoot.root = null;
		}
	}

	// Token: 0x04000155 RID: 341
	public static Transform root;
}
