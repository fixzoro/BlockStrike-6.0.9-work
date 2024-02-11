using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000043 RID: 67
[AddComponentMenu("NGUI/Interaction/Button")]
public class UIButton : UIButtonColor
{
	// Token: 0x1700000F RID: 15
	// (get) Token: 0x060001DA RID: 474 RVA: 0x00024AC8 File Offset: 0x00022CC8
	// (set) Token: 0x060001DB RID: 475 RVA: 0x00024B1C File Offset: 0x00022D1C
	public override bool isEnabled
	{
		get
		{
			if (!base.enabled)
			{
				return false;
			}
			Collider collider = base.GetComponent<Collider>();
			if (collider && collider.enabled)
			{
				return true;
			}
			Collider2D component = base.GetComponent<Collider2D>();
			return component && component.enabled;
		}
		set
		{
			if (this.isEnabled != value)
			{
				Collider collider = base.GetComponent<Collider>();
				if (collider != null)
				{
					collider.enabled = value;
					UIButton[] components = base.GetComponents<UIButton>();
					foreach (UIButton uibutton in components)
					{
						uibutton.SetState((!value) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal, false);
					}
				}
				else
				{
					Collider2D component = base.GetComponent<Collider2D>();
					if (component != null)
					{
						component.enabled = value;
						UIButton[] components2 = base.GetComponents<UIButton>();
						foreach (UIButton uibutton2 in components2)
						{
							uibutton2.SetState((!value) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal, false);
						}
					}
					else
					{
						base.enabled = value;
					}
				}
			}
		}
	}

	// Token: 0x17000010 RID: 16
	// (get) Token: 0x060001DC RID: 476 RVA: 0x00005F4E File Offset: 0x0000414E
	// (set) Token: 0x060001DD RID: 477 RVA: 0x00024BF8 File Offset: 0x00022DF8
	public string normalSprite
	{
		get
		{
			if (!this.mInitDone)
			{
				this.OnInit();
			}
			return this.mNormalSprite;
		}
		set
		{
			if (!this.mInitDone)
			{
				this.OnInit();
			}
			if (this.mSprite != null && !string.IsNullOrEmpty(this.mNormalSprite) && this.mNormalSprite == this.mSprite.spriteName)
			{
				this.mNormalSprite = value;
				this.SetSprite(value);
				NGUITools.SetDirty(this.mSprite, "last change");
			}
			else
			{
				this.mNormalSprite = value;
				if (this.mState == UIButtonColor.State.Normal)
				{
					this.SetSprite(value);
				}
			}
		}
	}

	// Token: 0x17000011 RID: 17
	// (get) Token: 0x060001DE RID: 478 RVA: 0x00005F67 File Offset: 0x00004167
	// (set) Token: 0x060001DF RID: 479 RVA: 0x00024C90 File Offset: 0x00022E90
	public Sprite normalSprite2D
	{
		get
		{
			if (!this.mInitDone)
			{
				this.OnInit();
			}
			return this.mNormalSprite2D;
		}
		set
		{
			if (!this.mInitDone)
			{
				this.OnInit();
			}
			if (this.mSprite2D != null && this.mNormalSprite2D == this.mSprite2D.sprite2D)
			{
				this.mNormalSprite2D = value;
				this.SetSprite(value);
				NGUITools.SetDirty(this.mSprite, "last change");
			}
			else
			{
				this.mNormalSprite2D = value;
				if (this.mState == UIButtonColor.State.Normal)
				{
					this.SetSprite(value);
				}
			}
		}
	}

	// Token: 0x060001E0 RID: 480 RVA: 0x00024D18 File Offset: 0x00022F18
	protected override void OnInit()
	{
		base.OnInit();
		this.mSprite = (this.mWidget as UISprite);
		this.mSprite2D = (this.mWidget as UI2DSprite);
		if (this.mSprite != null)
		{
			this.mNormalSprite = this.mSprite.spriteName;
		}
		if (this.mSprite2D != null)
		{
			this.mNormalSprite2D = this.mSprite2D.sprite2D;
		}
	}

	// Token: 0x060001E1 RID: 481 RVA: 0x00005F80 File Offset: 0x00004180
	protected override void OnEnable()
	{
		if (this.isEnabled)
		{
			if (this.mInitDone)
			{
				this.OnHover(UICamera.hoveredObject == base.gameObject);
			}
		}
		else
		{
			this.SetState(UIButtonColor.State.Disabled, true);
		}
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x00005FBB File Offset: 0x000041BB
	protected override void OnDragOver()
	{
		if (this.isEnabled && (this.dragHighlight || UICamera.currentTouch.pressed == base.gameObject))
		{
			base.OnDragOver();
		}
	}

	// Token: 0x060001E3 RID: 483 RVA: 0x00005FF3 File Offset: 0x000041F3
	protected override void OnDragOut()
	{
		if (this.isEnabled && (this.dragHighlight || UICamera.currentTouch.pressed == base.gameObject))
		{
			base.OnDragOut();
		}
	}

	// Token: 0x060001E4 RID: 484 RVA: 0x00024D94 File Offset: 0x00022F94
	protected virtual void OnClick()
	{
		if (UIButton.current == null && this.isEnabled && UICamera.currentTouchID != -2 && UICamera.currentTouchID != -3)
		{
			UIButton.current = this;
			EventDelegate.Execute(this.onClick);
			UIButton.current = null;
		}
	}

	// Token: 0x060001E5 RID: 485 RVA: 0x00024DEC File Offset: 0x00022FEC
	public override void SetState(UIButtonColor.State state, bool immediate)
	{
		base.SetState(state, immediate);
		if (this.mSprite != null)
		{
			switch (state)
			{
			case UIButtonColor.State.Normal:
				this.SetSprite(this.mNormalSprite);
				break;
			case UIButtonColor.State.Hover:
				this.SetSprite((!string.IsNullOrEmpty(this.hoverSprite)) ? this.hoverSprite : this.mNormalSprite);
				break;
			case UIButtonColor.State.Pressed:
				this.SetSprite(this.pressedSprite);
				break;
			case UIButtonColor.State.Disabled:
				this.SetSprite(this.disabledSprite);
				break;
			}
		}
		else if (this.mSprite2D != null)
		{
			switch (state)
			{
			case UIButtonColor.State.Normal:
				this.SetSprite(this.mNormalSprite2D);
				break;
			case UIButtonColor.State.Hover:
				this.SetSprite((!(this.hoverSprite2D == null)) ? this.hoverSprite2D : this.mNormalSprite2D);
				break;
			case UIButtonColor.State.Pressed:
				this.SetSprite(this.pressedSprite2D);
				break;
			case UIButtonColor.State.Disabled:
				this.SetSprite(this.disabledSprite2D);
				break;
			}
		}
	}

	// Token: 0x060001E6 RID: 486 RVA: 0x00024F24 File Offset: 0x00023124
	protected void SetSprite(string sp)
	{
		if (this.mSprite != null && !string.IsNullOrEmpty(sp) && this.mSprite.spriteName != sp)
		{
			this.mSprite.spriteName = sp;
			if (this.pixelSnap)
			{
				this.mSprite.MakePixelPerfect();
			}
		}
	}

	// Token: 0x060001E7 RID: 487 RVA: 0x00024F88 File Offset: 0x00023188
	protected void SetSprite(Sprite sp)
	{
		if (sp != null && this.mSprite2D != null && this.mSprite2D.sprite2D != sp)
		{
			this.mSprite2D.sprite2D = sp;
			if (this.pixelSnap)
			{
				this.mSprite2D.MakePixelPerfect();
			}
		}
	}

	// Token: 0x040000F2 RID: 242
	public static UIButton current;

	// Token: 0x040000F3 RID: 243
	public bool dragHighlight;

	// Token: 0x040000F4 RID: 244
	public string hoverSprite;

	// Token: 0x040000F5 RID: 245
	public string pressedSprite;

	// Token: 0x040000F6 RID: 246
	public string disabledSprite;

	// Token: 0x040000F7 RID: 247
	public Sprite hoverSprite2D;

	// Token: 0x040000F8 RID: 248
	public Sprite pressedSprite2D;

	// Token: 0x040000F9 RID: 249
	public Sprite disabledSprite2D;

	// Token: 0x040000FA RID: 250
	public bool pixelSnap;

	// Token: 0x040000FB RID: 251
	public List<EventDelegate> onClick = new List<EventDelegate>();

	// Token: 0x040000FC RID: 252
	[NonSerialized]
	private UISprite mSprite;

	// Token: 0x040000FD RID: 253
	[NonSerialized]
	private UI2DSprite mSprite2D;

	// Token: 0x040000FE RID: 254
	[NonSerialized]
	private string mNormalSprite;

	// Token: 0x040000FF RID: 255
	[NonSerialized]
	private Sprite mNormalSprite2D;
}
