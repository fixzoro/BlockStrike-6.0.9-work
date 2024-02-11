using System;
using UnityEngine;

// Token: 0x02000047 RID: 71
[AddComponentMenu("NGUI/Interaction/Button Keys (Legacy)")]
[ExecuteInEditMode]
public class UIButtonKeys : UIKeyNavigation
{
	// Token: 0x060001FE RID: 510 RVA: 0x0000618B File Offset: 0x0000438B
	protected override void OnEnable()
	{
		this.Upgrade();
		base.OnEnable();
	}

	// Token: 0x060001FF RID: 511 RVA: 0x000254AC File Offset: 0x000236AC
	public void Upgrade()
	{
		if (this.onClick == null && this.selectOnClick != null)
		{
			this.onClick = this.selectOnClick.gameObject;
			this.selectOnClick = null;
			NGUITools.SetDirty(this, "last change");
		}
		if (this.onLeft == null && this.selectOnLeft != null)
		{
			this.onLeft = this.selectOnLeft.gameObject;
			this.selectOnLeft = null;
			NGUITools.SetDirty(this, "last change");
		}
		if (this.onRight == null && this.selectOnRight != null)
		{
			this.onRight = this.selectOnRight.gameObject;
			this.selectOnRight = null;
			NGUITools.SetDirty(this, "last change");
		}
		if (this.onUp == null && this.selectOnUp != null)
		{
			this.onUp = this.selectOnUp.gameObject;
			this.selectOnUp = null;
			NGUITools.SetDirty(this, "last change");
		}
		if (this.onDown == null && this.selectOnDown != null)
		{
			this.onDown = this.selectOnDown.gameObject;
			this.selectOnDown = null;
			NGUITools.SetDirty(this, "last change");
		}
	}

	// Token: 0x04000111 RID: 273
	public UIButtonKeys selectOnClick;

	// Token: 0x04000112 RID: 274
	public UIButtonKeys selectOnUp;

	// Token: 0x04000113 RID: 275
	public UIButtonKeys selectOnDown;

	// Token: 0x04000114 RID: 276
	public UIButtonKeys selectOnLeft;

	// Token: 0x04000115 RID: 277
	public UIButtonKeys selectOnRight;
}
