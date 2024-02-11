using System;
using UnityEngine;

// Token: 0x0200004A RID: 74
[AddComponentMenu("NGUI/Interaction/Button Offset")]
public class UIButtonOffset : MonoBehaviour
{
	// Token: 0x0600020A RID: 522 RVA: 0x000256B8 File Offset: 0x000238B8
	private void Start()
	{
		if (!this.mStarted)
		{
			this.mStarted = true;
			if (this.tweenTarget == null)
			{
				this.tweenTarget = base.transform;
			}
			this.mPos = this.tweenTarget.localPosition;
		}
	}

	// Token: 0x0600020B RID: 523 RVA: 0x000062C3 File Offset: 0x000044C3
	private void OnEnable()
	{
		if (this.mStarted)
		{
			this.OnHover(UICamera.IsHighlighted(base.gameObject));
		}
	}

	// Token: 0x0600020C RID: 524 RVA: 0x00025708 File Offset: 0x00023908
	private void OnDisable()
	{
		if (this.mStarted && this.tweenTarget != null)
		{
			TweenPosition component = this.tweenTarget.GetComponent<TweenPosition>();
			if (component != null)
			{
				component.value = this.mPos;
				component.enabled = false;
			}
		}
	}

	// Token: 0x0600020D RID: 525 RVA: 0x0002575C File Offset: 0x0002395C
	private void OnPress(bool isPressed)
	{
		this.mPressed = isPressed;
		if (base.enabled)
		{
			if (!this.mStarted)
			{
				this.Start();
			}
			TweenPosition.Begin(this.tweenTarget.gameObject, this.duration, (!isPressed) ? ((!UICamera.IsHighlighted(base.gameObject)) ? this.mPos : (this.mPos + this.hover)) : (this.mPos + this.pressed)).method = UITweener.Method.EaseInOut;
		}
	}

	// Token: 0x0600020E RID: 526 RVA: 0x000257F0 File Offset: 0x000239F0
	private void OnHover(bool isOver)
	{
		if (base.enabled)
		{
			if (!this.mStarted)
			{
				this.Start();
			}
			TweenPosition.Begin(this.tweenTarget.gameObject, this.duration, (!isOver) ? this.mPos : (this.mPos + this.hover)).method = UITweener.Method.EaseInOut;
		}
	}

	// Token: 0x0600020F RID: 527 RVA: 0x000062E1 File Offset: 0x000044E1
	private void OnDragOver()
	{
		if (this.mPressed)
		{
			TweenPosition.Begin(this.tweenTarget.gameObject, this.duration, this.mPos + this.hover).method = UITweener.Method.EaseInOut;
		}
	}

	// Token: 0x06000210 RID: 528 RVA: 0x0000631B File Offset: 0x0000451B
	private void OnDragOut()
	{
		if (this.mPressed)
		{
			TweenPosition.Begin(this.tweenTarget.gameObject, this.duration, this.mPos).method = UITweener.Method.EaseInOut;
		}
	}

	// Token: 0x06000211 RID: 529 RVA: 0x0000634A File Offset: 0x0000454A
	private void OnSelect(bool isSelected)
	{
		if (base.enabled && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
		{
			this.OnHover(isSelected);
		}
	}

	// Token: 0x04000122 RID: 290
	public Transform tweenTarget;

	// Token: 0x04000123 RID: 291
	public Vector3 hover = Vector3.zero;

	// Token: 0x04000124 RID: 292
	public Vector3 pressed = new Vector3(2f, -2f);

	// Token: 0x04000125 RID: 293
	public float duration = 0.2f;

	// Token: 0x04000126 RID: 294
	[NonSerialized]
	private Vector3 mPos;

	// Token: 0x04000127 RID: 295
	[NonSerialized]
	private bool mStarted;

	// Token: 0x04000128 RID: 296
	[NonSerialized]
	private bool mPressed;
}
