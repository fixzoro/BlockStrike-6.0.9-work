using System;
using UnityEngine;

// Token: 0x02000051 RID: 81
[AddComponentMenu("NGUI/Interaction/Drag and Drop Container")]
public class UIDragDropContainer : MonoBehaviour
{
	// Token: 0x06000236 RID: 566 RVA: 0x000064CD File Offset: 0x000046CD
	protected virtual void Start()
	{
		if (this.reparentTarget == null)
		{
			this.reparentTarget = base.transform;
		}
	}

	// Token: 0x0400013C RID: 316
	public Transform reparentTarget;
}
