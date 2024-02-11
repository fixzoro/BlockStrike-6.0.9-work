using System;
using UnityEngine;

// Token: 0x02000050 RID: 80
[AddComponentMenu("NGUI/Interaction/Drag Camera")]
[ExecuteInEditMode]
public class UIDragCamera : MonoBehaviour
{
	// Token: 0x06000231 RID: 561 RVA: 0x000064A9 File Offset: 0x000046A9
	private void Awake()
	{
		if (this.draggableCamera == null)
		{
			this.draggableCamera = NGUITools.FindInParents<UIDraggableCamera>(base.gameObject);
		}
	}

	// Token: 0x06000232 RID: 562 RVA: 0x000263A8 File Offset: 0x000245A8
	private void OnPress(bool isPressed)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && this.draggableCamera != null && this.draggableCamera.enabled)
		{
			this.draggableCamera.Press(isPressed);
		}
	}

	// Token: 0x06000233 RID: 563 RVA: 0x00026400 File Offset: 0x00024600
	private void OnDrag(Vector2 delta)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && this.draggableCamera != null && this.draggableCamera.enabled)
		{
			this.draggableCamera.Drag(delta);
		}
	}

	// Token: 0x06000234 RID: 564 RVA: 0x00026458 File Offset: 0x00024658
	private void OnScroll(float delta)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && this.draggableCamera != null && this.draggableCamera.enabled)
		{
			this.draggableCamera.Scroll(delta);
		}
	}

	// Token: 0x0400013B RID: 315
	public UIDraggableCamera draggableCamera;
}
