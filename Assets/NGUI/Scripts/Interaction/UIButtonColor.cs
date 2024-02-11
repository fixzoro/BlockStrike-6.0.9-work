using System;
using UnityEngine;

// Token: 0x02000045 RID: 69
[AddComponentMenu("NGUI/Interaction/Button Color")]
[ExecuteInEditMode]
public class UIButtonColor : UIWidgetContainer
{
	// Token: 0x17000012 RID: 18
	// (get) Token: 0x060001EB RID: 491 RVA: 0x0000605E File Offset: 0x0000425E
	// (set) Token: 0x060001EC RID: 492 RVA: 0x00006066 File Offset: 0x00004266
	public UIButtonColor.State state
	{
		get
		{
			return this.mState;
		}
		set
		{
			this.SetState(value, false);
		}
	}

	// Token: 0x17000013 RID: 19
	// (get) Token: 0x060001ED RID: 493 RVA: 0x00006070 File Offset: 0x00004270
	// (set) Token: 0x060001EE RID: 494 RVA: 0x00025054 File Offset: 0x00023254
	public Color defaultColor
	{
		get
		{
			if (!this.mInitDone)
			{
				this.OnInit();
			}
			return this.mDefaultColor;
		}
		set
		{
			if (!this.mInitDone)
			{
				this.OnInit();
			}
			this.mDefaultColor = value;
			UIButtonColor.State state = this.mState;
			this.mState = UIButtonColor.State.Disabled;
			this.SetState(state, false);
		}
	}

	// Token: 0x17000014 RID: 20
	// (get) Token: 0x060001EF RID: 495 RVA: 0x00006089 File Offset: 0x00004289
	// (set) Token: 0x060001F0 RID: 496 RVA: 0x00006091 File Offset: 0x00004291
	public virtual bool isEnabled
	{
		get
		{
			return base.enabled;
		}
		set
		{
			base.enabled = value;
		}
	}

	// Token: 0x060001F1 RID: 497 RVA: 0x0000609A File Offset: 0x0000429A
	public void ResetDefaultColor()
	{
		this.defaultColor = this.mStartingColor;
	}

	// Token: 0x060001F2 RID: 498 RVA: 0x000060A8 File Offset: 0x000042A8
	public void CacheDefaultColor()
	{
		if (!this.mInitDone)
		{
			this.OnInit();
		}
	}

	// Token: 0x060001F3 RID: 499 RVA: 0x000060BB File Offset: 0x000042BB
	private void Start()
	{
		if (!this.mInitDone)
		{
			this.OnInit();
		}
		if (!this.isEnabled)
		{
			this.SetState(UIButtonColor.State.Disabled, true);
		}
	}

	// Token: 0x060001F4 RID: 500 RVA: 0x00025090 File Offset: 0x00023290
	protected virtual void OnInit()
	{
		this.mInitDone = true;
		if (this.tweenTarget == null && !Application.isPlaying)
		{
			this.tweenTarget = base.gameObject;
		}
		if (this.tweenTarget != null)
		{
			this.mWidget = this.tweenTarget.GetComponent<UIWidget>();
		}
		if (this.mWidget != null)
		{
			this.mDefaultColor = this.mWidget.color;
			this.mStartingColor = this.mDefaultColor;
		}
		else if (this.tweenTarget != null)
		{
			Renderer renderer = this.tweenTarget.GetComponent<Renderer>();
			if (renderer != null)
			{
				this.mDefaultColor = ((!Application.isPlaying) ? renderer.sharedMaterial.color : renderer.material.color);
				this.mStartingColor = this.mDefaultColor;
			}
			else
			{
				Light light = this.tweenTarget.GetComponent<Light>();
				if (light != null)
				{
					this.mDefaultColor = light.color;
					this.mStartingColor = this.mDefaultColor;
				}
				else
				{
					this.tweenTarget = null;
					this.mInitDone = false;
				}
			}
		}
	}

	// Token: 0x060001F5 RID: 501 RVA: 0x000251C8 File Offset: 0x000233C8
	protected virtual void OnEnable()
	{
		if (this.mInitDone)
		{
			this.OnHover(UICamera.IsHighlighted(base.gameObject));
		}
		if (UICamera.currentTouch != null)
		{
			if (UICamera.currentTouch.pressed == base.gameObject)
			{
				this.OnPress(true);
			}
			else if (UICamera.currentTouch.current == base.gameObject)
			{
				this.OnHover(true);
			}
		}
	}

	// Token: 0x060001F6 RID: 502 RVA: 0x00025244 File Offset: 0x00023444
	protected virtual void OnDisable()
	{
		if (this.mInitDone && this.mState != UIButtonColor.State.Normal)
		{
			this.SetState(UIButtonColor.State.Normal, true);
			if (this.tweenTarget != null)
			{
				TweenColor component = this.tweenTarget.GetComponent<TweenColor>();
				if (component != null)
				{
					component.value = this.mDefaultColor;
					component.enabled = false;
				}
			}
		}
	}

	// Token: 0x060001F7 RID: 503 RVA: 0x000252AC File Offset: 0x000234AC
	protected virtual void OnHover(bool isOver)
	{
		if (this.isEnabled)
		{
			if (!this.mInitDone)
			{
				this.OnInit();
			}
			if (this.tweenTarget != null)
			{
				this.SetState((!isOver) ? UIButtonColor.State.Normal : UIButtonColor.State.Hover, false);
			}
		}
	}

	// Token: 0x060001F8 RID: 504 RVA: 0x000252FC File Offset: 0x000234FC
	protected virtual void OnPress(bool isPressed)
	{
		if (this.isEnabled)
		{
			if (!this.mInitDone)
			{
				this.OnInit();
			}
			if (this.tweenTarget != null)
			{
				if (isPressed)
				{
					this.SetState(UIButtonColor.State.Pressed, false);
				}
				else if (UICamera.currentTouch != null && UICamera.currentTouch.current == base.gameObject)
				{
					if (UICamera.currentScheme == UICamera.ControlScheme.Controller)
					{
						this.SetState(UIButtonColor.State.Hover, false);
					}
					else if (UICamera.currentScheme == UICamera.ControlScheme.Mouse && UICamera.hoveredObject == base.gameObject)
					{
						this.SetState(UIButtonColor.State.Hover, false);
					}
					else
					{
						this.SetState(UIButtonColor.State.Normal, false);
					}
				}
				else
				{
					this.SetState(UIButtonColor.State.Normal, false);
				}
			}
		}
	}

	// Token: 0x060001F9 RID: 505 RVA: 0x000060E1 File Offset: 0x000042E1
	protected virtual void OnDragOver()
	{
		if (this.isEnabled)
		{
			if (!this.mInitDone)
			{
				this.OnInit();
			}
			if (this.tweenTarget != null)
			{
				this.SetState(UIButtonColor.State.Pressed, false);
			}
		}
	}

	// Token: 0x060001FA RID: 506 RVA: 0x00006118 File Offset: 0x00004318
	protected virtual void OnDragOut()
	{
		if (this.isEnabled)
		{
			if (!this.mInitDone)
			{
				this.OnInit();
			}
			if (this.tweenTarget != null)
			{
				this.SetState(UIButtonColor.State.Normal, false);
			}
		}
	}

	// Token: 0x060001FB RID: 507 RVA: 0x0000614F File Offset: 0x0000434F
	public virtual void SetState(UIButtonColor.State state, bool instant)
	{
		if (!this.mInitDone)
		{
			this.mInitDone = true;
			this.OnInit();
		}
		if (this.mState != state)
		{
			this.mState = state;
			this.UpdateColor(instant);
		}
	}

	// Token: 0x060001FC RID: 508 RVA: 0x000253C8 File Offset: 0x000235C8
	public void UpdateColor(bool instant)
	{
		if (!this.mInitDone)
		{
			return;
		}
		if (this.tweenTarget != null)
		{
			TweenColor tweenColor;
			switch (this.mState)
			{
			case UIButtonColor.State.Hover:
				tweenColor = TweenColor.Begin(this.tweenTarget, this.duration, this.hover);
				break;
			case UIButtonColor.State.Pressed:
				tweenColor = TweenColor.Begin(this.tweenTarget, this.duration, this.pressed);
				break;
			case UIButtonColor.State.Disabled:
				tweenColor = TweenColor.Begin(this.tweenTarget, this.duration, this.disabledColor);
				break;
			default:
				tweenColor = TweenColor.Begin(this.tweenTarget, this.duration, this.mDefaultColor);
				break;
			}
			if (instant && tweenColor != null)
			{
				tweenColor.value = tweenColor.to;
				tweenColor.enabled = false;
			}
		}
	}

	// Token: 0x04000102 RID: 258
	public GameObject tweenTarget;

	// Token: 0x04000103 RID: 259
	public Color hover = new Color(0.882352948f, 0.784313738f, 0.5882353f, 1f);

	// Token: 0x04000104 RID: 260
	public Color pressed = new Color(0.7176471f, 0.6392157f, 0.482352942f, 1f);

	// Token: 0x04000105 RID: 261
	public Color disabledColor = Color.grey;

	// Token: 0x04000106 RID: 262
	public float duration = 0.2f;

	// Token: 0x04000107 RID: 263
	[NonSerialized]
	protected Color mStartingColor;

	// Token: 0x04000108 RID: 264
	[NonSerialized]
	protected Color mDefaultColor;

	// Token: 0x04000109 RID: 265
	[NonSerialized]
	protected bool mInitDone;

	// Token: 0x0400010A RID: 266
	[NonSerialized]
	protected UIWidget mWidget;

	// Token: 0x0400010B RID: 267
	[NonSerialized]
	protected UIButtonColor.State mState;

	// Token: 0x02000046 RID: 70
	[DoNotObfuscateNGUI]
	public enum State
	{
		// Token: 0x0400010D RID: 269
		Normal,
		// Token: 0x0400010E RID: 270
		Hover,
		// Token: 0x0400010F RID: 271
		Pressed,
		// Token: 0x04000110 RID: 272
		Disabled
	}
}
