using System;
using UnityEngine;

// Token: 0x02000057 RID: 87
[AddComponentMenu("NGUI/Interaction/Drag-Resize Widget")]
public class UIDragResize : MonoBehaviour
{
	// Token: 0x06000262 RID: 610 RVA: 0x0002781C File Offset: 0x00025A1C
	private void OnDragStart()
	{
		if (this.target != null)
		{
			Vector3[] worldCorners = this.target.worldCorners;
			this.mPlane = new Plane(worldCorners[0], worldCorners[1], worldCorners[3]);
			Ray currentRay = UICamera.currentRay;
			float distance;
			if (this.mPlane.Raycast(currentRay, out distance))
			{
				this.mRayPos = currentRay.GetPoint(distance);
				this.mLocalPos = this.target.cachedTransform.localPosition;
				this.mWidth = this.target.width;
				this.mHeight = this.target.height;
				this.mDragging = true;
			}
		}
	}

	// Token: 0x06000263 RID: 611 RVA: 0x000278DC File Offset: 0x00025ADC
	private void OnDrag(Vector2 delta)
	{
		if (this.mDragging && this.target != null)
		{
			Ray currentRay = UICamera.currentRay;
			float distance;
			if (this.mPlane.Raycast(currentRay, out distance))
			{
				Transform cachedTransform = this.target.cachedTransform;
				cachedTransform.localPosition = this.mLocalPos;
				this.target.width = this.mWidth;
				this.target.height = this.mHeight;
				Vector3 b = currentRay.GetPoint(distance) - this.mRayPos;
				cachedTransform.position += b;
				Vector3 vector = Quaternion.Inverse(cachedTransform.localRotation) * (cachedTransform.localPosition - this.mLocalPos);
				cachedTransform.localPosition = this.mLocalPos;
				NGUIMath.ResizeWidget(this.target, this.pivot, vector.x, vector.y, this.minWidth, this.minHeight, this.maxWidth, this.maxHeight);
				if (this.updateAnchors)
				{
					this.target.BroadcastMessage("UpdateAnchors");
				}
			}
		}
	}

	// Token: 0x06000264 RID: 612 RVA: 0x000066EB File Offset: 0x000048EB
	private void OnDragEnd()
	{
		this.mDragging = false;
	}

	// Token: 0x0400016C RID: 364
	public UIWidget target;

	// Token: 0x0400016D RID: 365
	public UIWidget.Pivot pivot = UIWidget.Pivot.BottomRight;

	// Token: 0x0400016E RID: 366
	public int minWidth = 100;

	// Token: 0x0400016F RID: 367
	public int minHeight = 100;

	// Token: 0x04000170 RID: 368
	public int maxWidth = 100000;

	// Token: 0x04000171 RID: 369
	public int maxHeight = 100000;

	// Token: 0x04000172 RID: 370
	public bool updateAnchors;

	// Token: 0x04000173 RID: 371
	private Plane mPlane;

	// Token: 0x04000174 RID: 372
	private Vector3 mRayPos;

	// Token: 0x04000175 RID: 373
	private Vector3 mLocalPos;

	// Token: 0x04000176 RID: 374
	private int mWidth;

	// Token: 0x04000177 RID: 375
	private int mHeight;

	// Token: 0x04000178 RID: 376
	private bool mDragging;
}
