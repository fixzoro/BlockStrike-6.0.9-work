using System;
using UnityEngine;

// Token: 0x0200007D RID: 125
[AddComponentMenu("NGUI/Interaction/NGUI Slider")]
[ExecuteInEditMode]
public class UISlider : UIProgressBar
{
	// Token: 0x17000044 RID: 68
	// (get) Token: 0x0600039C RID: 924 RVA: 0x0002831C File Offset: 0x0002651C
	public bool isColliderEnabled
	{
		get
		{
			Collider component = base.GetComponent<Collider>();
			if (component != null)
			{
				return component.enabled;
			}
			Collider2D component2 = base.GetComponent<Collider2D>();
			return component2 != null && component2.enabled;
		}
	}

	// Token: 0x17000045 RID: 69
	// (get) Token: 0x0600039D RID: 925 RVA: 0x00007529 File Offset: 0x00005729
	// (set) Token: 0x0600039E RID: 926 RVA: 0x00007531 File Offset: 0x00005731
	[Obsolete("Use 'value' instead")]
	public float sliderValue
	{
		get
		{
			return base.value;
		}
		set
		{
			base.value = value;
		}
	}

	// Token: 0x17000046 RID: 70
	// (get) Token: 0x0600039F RID: 927 RVA: 0x000076E7 File Offset: 0x000058E7
	// (set) Token: 0x060003A0 RID: 928 RVA: 0x0000574F File Offset: 0x0000394F
	[Obsolete("Use 'fillDirection' instead")]
	public bool inverted
	{
		get
		{
			return base.isInverted;
		}
		set
		{
		}
	}

	// Token: 0x060003A1 RID: 929 RVA: 0x0002F168 File Offset: 0x0002D368
	protected override void Upgrade()
	{
		if (this.direction != UISlider.Direction.Upgraded)
		{
			this.mValue = this.rawValue;
			if (this.foreground != null)
			{
				this.mFG = this.foreground.GetComponent<UIWidget>();
			}
			if (this.direction == UISlider.Direction.Horizontal)
			{
				this.mFill = ((!this.mInverted) ? UIProgressBar.FillDirection.LeftToRight : UIProgressBar.FillDirection.RightToLeft);
			}
			else
			{
				this.mFill = ((!this.mInverted) ? UIProgressBar.FillDirection.BottomToTop : UIProgressBar.FillDirection.TopToBottom);
			}
			this.direction = UISlider.Direction.Upgraded;
		}
	}

	// Token: 0x060003A2 RID: 930 RVA: 0x0002F1F8 File Offset: 0x0002D3F8
	protected override void OnStart()
	{
		GameObject go = (!(this.mBG != null) || (!(this.mBG.GetComponent<Collider>() != null) && !(this.mBG.GetComponent<Collider2D>() != null))) ? base.gameObject : this.mBG.gameObject;
		UIEventListener uieventListener = UIEventListener.Get(go);
		UIEventListener uieventListener2 = uieventListener;
		uieventListener2.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener2.onPress, new UIEventListener.BoolDelegate(this.OnPressBackground));
		UIEventListener uieventListener3 = uieventListener;
		uieventListener3.onDrag = (UIEventListener.VectorDelegate)Delegate.Combine(uieventListener3.onDrag, new UIEventListener.VectorDelegate(this.OnDragBackground));
		if (this.thumb != null && (this.thumb.GetComponent<Collider>() != null || this.thumb.GetComponent<Collider2D>() != null) && (this.mFG == null || this.thumb != this.mFG.cachedTransform))
		{
			UIEventListener uieventListener4 = UIEventListener.Get(this.thumb.gameObject);
			UIEventListener uieventListener5 = uieventListener4;
			uieventListener5.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener5.onPress, new UIEventListener.BoolDelegate(this.OnPressForeground));
			UIEventListener uieventListener6 = uieventListener4;
			uieventListener6.onDrag = (UIEventListener.VectorDelegate)Delegate.Combine(uieventListener6.onDrag, new UIEventListener.VectorDelegate(this.OnDragForeground));
		}
	}

	// Token: 0x060003A3 RID: 931 RVA: 0x0002F364 File Offset: 0x0002D564
	protected void OnPressBackground(GameObject go, bool isPressed)
	{
		if (UICamera.currentScheme == UICamera.ControlScheme.Controller)
		{
			return;
		}
		this.mCam = UICamera.currentCamera;
		base.value = base.ScreenToValue(UICamera.lastEventPosition);
		if (!isPressed && this.onDragFinished != null)
		{
			this.onDragFinished();
		}
	}

	// Token: 0x060003A4 RID: 932 RVA: 0x000076EF File Offset: 0x000058EF
	protected void OnDragBackground(GameObject go, Vector2 delta)
	{
		if (UICamera.currentScheme == UICamera.ControlScheme.Controller)
		{
			return;
		}
		this.mCam = UICamera.currentCamera;
		base.value = base.ScreenToValue(UICamera.lastEventPosition);
	}

	// Token: 0x060003A5 RID: 933 RVA: 0x0002F3B8 File Offset: 0x0002D5B8
	protected void OnPressForeground(GameObject go, bool isPressed)
	{
		if (UICamera.currentScheme == UICamera.ControlScheme.Controller)
		{
			return;
		}
		this.mCam = UICamera.currentCamera;
		if (isPressed)
		{
			this.mOffset = ((!(this.mFG == null)) ? (base.value - base.ScreenToValue(UICamera.lastEventPosition)) : 0f);
		}
		else if (this.onDragFinished != null)
		{
			this.onDragFinished();
		}
	}

	// Token: 0x060003A6 RID: 934 RVA: 0x00007719 File Offset: 0x00005919
	protected void OnDragForeground(GameObject go, Vector2 delta)
	{
		if (UICamera.currentScheme == UICamera.ControlScheme.Controller)
		{
			return;
		}
		this.mCam = UICamera.currentCamera;
		base.value = this.mOffset + base.ScreenToValue(UICamera.lastEventPosition);
	}

	// Token: 0x060003A7 RID: 935 RVA: 0x0000774A File Offset: 0x0000594A
	public override void OnPan(Vector2 delta)
	{
		if (base.enabled && this.isColliderEnabled)
		{
			base.OnPan(delta);
		}
	}

	// Token: 0x040002B6 RID: 694
	[HideInInspector]
	[SerializeField]
	private Transform foreground;

	// Token: 0x040002B7 RID: 695
	[SerializeField]
	[HideInInspector]
	private float rawValue = 1f;

	// Token: 0x040002B8 RID: 696
	[HideInInspector]
	[SerializeField]
	private UISlider.Direction direction = UISlider.Direction.Upgraded;

	// Token: 0x040002B9 RID: 697
	[SerializeField]
	[HideInInspector]
	protected bool mInverted;

	// Token: 0x0200007E RID: 126
	private enum Direction
	{
		// Token: 0x040002BB RID: 699
		Horizontal,
		// Token: 0x040002BC RID: 700
		Vertical,
		// Token: 0x040002BD RID: 701
		Upgraded
	}
}
