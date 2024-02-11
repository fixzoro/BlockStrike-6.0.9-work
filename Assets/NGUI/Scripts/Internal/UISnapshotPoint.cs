using System;
using UnityEngine;

// Token: 0x020000C3 RID: 195
[ExecuteInEditMode]
[AddComponentMenu("NGUI/Internal/Snapshot Point")]
public class UISnapshotPoint : MonoBehaviour
{
	// Token: 0x06000644 RID: 1604 RVA: 0x00009182 File Offset: 0x00007382
	private void Start()
	{
		if (base.tag != "EditorOnly")
		{
			base.tag = "EditorOnly";
		}
	}

	// Token: 0x04000462 RID: 1122
	public bool isOrthographic = true;

	// Token: 0x04000463 RID: 1123
	public float nearClip = -100f;

	// Token: 0x04000464 RID: 1124
	public float farClip = 100f;

	// Token: 0x04000465 RID: 1125
	[Range(10f, 80f)]
	public int fieldOfView = 35;

	// Token: 0x04000466 RID: 1126
	public float orthoSize = 30f;

	// Token: 0x04000467 RID: 1127
	public Texture2D thumbnail;
}
