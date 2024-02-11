using System;
using UnityEngine;

// Token: 0x02000117 RID: 279
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Orthographic Camera")]
public class UIOrthoCamera : MonoBehaviour
{
	// Token: 0x060009A4 RID: 2468 RVA: 0x0000B256 File Offset: 0x00009456
	private void Start()
	{
		this.mCam = base.GetComponent<Camera>();
		this.mTrans = base.transform;
		this.mCam.orthographic = true;
	}

	// Token: 0x060009A5 RID: 2469 RVA: 0x000548B8 File Offset: 0x00052AB8
	private void Update()
	{
		float num = this.mCam.rect.yMin * (float)Screen.height;
		float num2 = this.mCam.rect.yMax * (float)Screen.height;
		float num3 = (num2 - num) * 0.5f * this.mTrans.lossyScale.y;
		if (!Mathf.Approximately(this.mCam.orthographicSize, num3))
		{
			this.mCam.orthographicSize = num3;
		}
	}

	// Token: 0x040006AD RID: 1709
	private Camera mCam;

	// Token: 0x040006AE RID: 1710
	private Transform mTrans;
}
