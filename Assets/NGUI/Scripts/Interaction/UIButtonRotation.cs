using System;
using UnityEngine;

// Token: 0x0200004B RID: 75
[AddComponentMenu("NGUI/Interaction/Button Rotation")]
public class UIButtonRotation : MonoBehaviour
{
	// Token: 0x06000213 RID: 531 RVA: 0x00025858 File Offset: 0x00023A58
	private void Start()
	{
		if (!this.mStarted)
		{
			this.mStarted = true;
			if (this.tweenTarget == null)
			{
				this.tweenTarget = base.transform;
			}
			this.mRot = this.tweenTarget.localRotation;
		}
	}

	// Token: 0x06000214 RID: 532 RVA: 0x00006398 File Offset: 0x00004598
	private void OnEnable()
	{
		if (this.mStarted)
		{
			this.OnHover(UICamera.IsHighlighted(base.gameObject));
		}
	}

	// Token: 0x06000215 RID: 533 RVA: 0x000258A8 File Offset: 0x00023AA8
	private void OnDisable()
	{
		if (this.mStarted && this.tweenTarget != null)
		{
			TweenRotation component = this.tweenTarget.GetComponent<TweenRotation>();
			if (component != null)
			{
				component.value = this.mRot;
				component.enabled = false;
			}
		}
	}

	// Token: 0x06000216 RID: 534 RVA: 0x000258FC File Offset: 0x00023AFC
	private void OnPress(bool isPressed)
	{
		if (base.enabled)
		{
			if (!this.mStarted)
			{
				this.Start();
			}
			TweenRotation.Begin(this.tweenTarget.gameObject, this.duration, (!isPressed) ? ((!UICamera.IsHighlighted(base.gameObject)) ? this.mRot : (this.mRot * Quaternion.Euler(this.hover))) : (this.mRot * Quaternion.Euler(this.pressed))).method = UITweener.Method.EaseInOut;
		}
	}

	// Token: 0x06000217 RID: 535 RVA: 0x00025994 File Offset: 0x00023B94
	private void OnHover(bool isOver)
	{
		if (base.enabled)
		{
			if (!this.mStarted)
			{
				this.Start();
			}
			TweenRotation.Begin(this.tweenTarget.gameObject, this.duration, (!isOver) ? this.mRot : (this.mRot * Quaternion.Euler(this.hover))).method = UITweener.Method.EaseInOut;
		}
	}

	// Token: 0x06000218 RID: 536 RVA: 0x000063B6 File Offset: 0x000045B6
	private void OnSelect(bool isSelected)
	{
		if (base.enabled && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
		{
			this.OnHover(isSelected);
		}
	}

	// Token: 0x04000129 RID: 297
	public Transform tweenTarget;

	// Token: 0x0400012A RID: 298
	public Vector3 hover = Vector3.zero;

	// Token: 0x0400012B RID: 299
	public Vector3 pressed = Vector3.zero;

	// Token: 0x0400012C RID: 300
	public float duration = 0.2f;

	// Token: 0x0400012D RID: 301
	private Quaternion mRot;

	// Token: 0x0400012E RID: 302
	private bool mStarted;
}
