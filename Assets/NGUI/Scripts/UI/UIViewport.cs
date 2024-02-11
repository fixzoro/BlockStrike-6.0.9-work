using System;
using UnityEngine;

// Token: 0x02000131 RID: 305
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("NGUI/UI/Viewport Camera")]
public class UIViewport : MonoBehaviour
{
	// Token: 0x06000AB8 RID: 2744 RVA: 0x0000BD46 File Offset: 0x00009F46
	private void Start()
	{
		this.mCam = base.GetComponent<Camera>();
		if (this.sourceCamera == null)
		{
			this.sourceCamera = Camera.main;
		}
	}

	// Token: 0x06000AB9 RID: 2745 RVA: 0x0005B4E0 File Offset: 0x000596E0
	private void LateUpdate()
	{
		if (this.topLeft != null && this.bottomRight != null)
		{
			if (this.topLeft.gameObject.activeInHierarchy)
			{
				Vector3 vector = this.sourceCamera.WorldToScreenPoint(this.topLeft.position);
				Vector3 vector2 = this.sourceCamera.WorldToScreenPoint(this.bottomRight.position);
				Rect rect = new Rect(vector.x / (float)Screen.width, vector2.y / (float)Screen.height, (vector2.x - vector.x) / (float)Screen.width, (vector.y - vector2.y) / (float)Screen.height);
				float num = this.fullSize * rect.height;
				if (rect != this.mCam.rect)
				{
					this.mCam.rect = rect;
				}
				if (this.mCam.orthographicSize != num)
				{
					this.mCam.orthographicSize = num;
				}
				this.mCam.enabled = true;
			}
			else
			{
				this.mCam.enabled = false;
			}
		}
	}

	// Token: 0x04000764 RID: 1892
	public Camera sourceCamera;

	// Token: 0x04000765 RID: 1893
	public Transform topLeft;

	// Token: 0x04000766 RID: 1894
	public Transform bottomRight;

	// Token: 0x04000767 RID: 1895
	public float fullSize = 1f;

	// Token: 0x04000768 RID: 1896
	private Camera mCam;
}
