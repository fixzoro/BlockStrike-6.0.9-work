using System;
using UnityEngine;

// Token: 0x0200004C RID: 76
[AddComponentMenu("NGUI/Interaction/Button Scale")]
public class UIButtonScale : MonoBehaviour
{
	// Token: 0x0600021A RID: 538 RVA: 0x00025A54 File Offset: 0x00023C54
	private void Start()
	{
		if (!this.mStarted)
		{
			this.mStarted = true;
			if (this.tweenTarget == null)
			{
				this.tweenTarget = base.transform;
			}
			this.mScale = this.tweenTarget.localScale;
		}
	}

	// Token: 0x0600021B RID: 539 RVA: 0x000063DB File Offset: 0x000045DB
	private void OnEnable()
	{
		if (this.mStarted)
		{
			this.OnHover(UICamera.IsHighlighted(base.gameObject));
		}
	}

	// Token: 0x0600021C RID: 540 RVA: 0x00025AA4 File Offset: 0x00023CA4
	private void OnDisable()
	{
		if (this.mStarted && this.tweenTarget != null)
		{
			TweenScale component = this.tweenTarget.GetComponent<TweenScale>();
			if (component != null)
			{
				component.value = this.mScale;
				component.enabled = false;
			}
		}
	}

	// Token: 0x0600021D RID: 541 RVA: 0x00025AF8 File Offset: 0x00023CF8
	private void OnPress(bool isPressed)
	{
		if (base.enabled)
		{
			if (!this.mStarted)
			{
				this.Start();
			}
			TweenScale.Begin(this.tweenTarget.gameObject, this.duration, (!isPressed) ? ((!UICamera.IsHighlighted(base.gameObject)) ? this.mScale : Vector3.Scale(this.mScale, this.hover)) : Vector3.Scale(this.mScale, this.pressed)).method = UITweener.Method.EaseInOut;
		}
	}

	// Token: 0x0600021E RID: 542 RVA: 0x00025B88 File Offset: 0x00023D88
	private void OnHover(bool isOver)
	{
		if (base.enabled)
		{
			if (!this.mStarted)
			{
				this.Start();
			}
			TweenScale.Begin(this.tweenTarget.gameObject, this.duration, (!isOver) ? this.mScale : Vector3.Scale(this.mScale, this.hover)).method = UITweener.Method.EaseInOut;
		}
	}

	// Token: 0x0600021F RID: 543 RVA: 0x000063F9 File Offset: 0x000045F9
	private void OnSelect(bool isSelected)
	{
		if (base.enabled && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
		{
			this.OnHover(isSelected);
		}
	}

	// Token: 0x0400012F RID: 303
	public Transform tweenTarget;

	// Token: 0x04000130 RID: 304
	public Vector3 hover = new Vector3(1.1f, 1.1f, 1.1f);

	// Token: 0x04000131 RID: 305
	public Vector3 pressed = new Vector3(1.05f, 1.05f, 1.05f);

	// Token: 0x04000132 RID: 306
	public float duration = 0.2f;

	// Token: 0x04000133 RID: 307
	private Vector3 mScale;

	// Token: 0x04000134 RID: 308
	private bool mStarted;
}
