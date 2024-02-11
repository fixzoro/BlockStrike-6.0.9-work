using System;
using System.Collections.Generic;
using AnimationOrTween;
using UnityEngine;

// Token: 0x02000066 RID: 102
[AddComponentMenu("NGUI/Interaction/Play Animation")]
[ExecuteInEditMode]
public class UIPlayAnimation : MonoBehaviour
{
	// Token: 0x1700001E RID: 30
	// (get) Token: 0x060002D2 RID: 722 RVA: 0x00006DCB File Offset: 0x00004FCB
	private bool dualState
	{
		get
		{
			return this.trigger == Trigger.OnPress || this.trigger == Trigger.OnHover;
		}
	}

	// Token: 0x060002D3 RID: 723 RVA: 0x00029738 File Offset: 0x00027938
	private void Awake()
	{
		UIButton component = base.GetComponent<UIButton>();
		if (component != null)
		{
			this.dragHighlight = component.dragHighlight;
		}
		if (this.eventReceiver != null && EventDelegate.IsValid(this.onFinished))
		{
			this.eventReceiver = null;
			this.callWhenFinished = null;
		}
	}

	// Token: 0x060002D4 RID: 724 RVA: 0x00029794 File Offset: 0x00027994
	private void Start()
	{
		this.mStarted = true;
		if (this.target == null && this.animator == null)
		{
			this.animator = base.GetComponentInChildren<Animator>();
		}
		if (this.animator != null)
		{
			if (this.animator.enabled)
			{
				this.animator.enabled = false;
			}
			return;
		}
		if (this.target == null)
		{
			this.target = base.GetComponentInChildren<Animation>();
		}
		if (this.target != null && this.target.enabled)
		{
			this.target.enabled = false;
		}
	}

	// Token: 0x060002D5 RID: 725 RVA: 0x00029850 File Offset: 0x00027A50
	private void OnEnable()
	{
		if (this.mStarted)
		{
			this.OnHover(UICamera.IsHighlighted(base.gameObject));
		}
		if (UICamera.currentTouch != null)
		{
			if (this.trigger == Trigger.OnPress || this.trigger == Trigger.OnPressTrue)
			{
				this.mActivated = (UICamera.currentTouch.pressed == base.gameObject);
			}
			if (this.trigger == Trigger.OnHover || this.trigger == Trigger.OnHoverTrue)
			{
				this.mActivated = (UICamera.currentTouch.current == base.gameObject);
			}
		}
		UIToggle component = base.GetComponent<UIToggle>();
		if (component != null)
		{
			EventDelegate.Add(component.onChange, new EventDelegate.Callback(this.OnToggle));
		}
	}

	// Token: 0x060002D6 RID: 726 RVA: 0x00029914 File Offset: 0x00027B14
	private void OnDisable()
	{
		UIToggle component = base.GetComponent<UIToggle>();
		if (component != null)
		{
			EventDelegate.Remove(component.onChange, new EventDelegate.Callback(this.OnToggle));
		}
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x0002994C File Offset: 0x00027B4C
	private void OnHover(bool isOver)
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.trigger == Trigger.OnHover || (this.trigger == Trigger.OnHoverTrue && isOver) || (this.trigger == Trigger.OnHoverFalse && !isOver))
		{
			this.Play(isOver, this.dualState);
		}
	}

	// Token: 0x060002D8 RID: 728 RVA: 0x000299A4 File Offset: 0x00027BA4
	private void OnPress(bool isPressed)
	{
		if (!base.enabled)
		{
			return;
		}
		if (UICamera.currentTouchID == -2 || UICamera.currentTouchID == -3)
		{
			return;
		}
		if (this.trigger == Trigger.OnPress || (this.trigger == Trigger.OnPressTrue && isPressed) || (this.trigger == Trigger.OnPressFalse && !isPressed))
		{
			this.Play(isPressed, this.dualState);
		}
	}

	// Token: 0x060002D9 RID: 729 RVA: 0x00006DE5 File Offset: 0x00004FE5
	private void OnClick()
	{
		if (UICamera.currentTouchID == -2 || UICamera.currentTouchID == -3)
		{
			return;
		}
		if (base.enabled && this.trigger == Trigger.OnClick)
		{
			this.Play(true, false);
		}
	}

	// Token: 0x060002DA RID: 730 RVA: 0x00006E1E File Offset: 0x0000501E
	private void OnDoubleClick()
	{
		if (UICamera.currentTouchID == -2 || UICamera.currentTouchID == -3)
		{
			return;
		}
		if (base.enabled && this.trigger == Trigger.OnDoubleClick)
		{
			this.Play(true, false);
		}
	}

	// Token: 0x060002DB RID: 731 RVA: 0x00029A14 File Offset: 0x00027C14
	private void OnSelect(bool isSelected)
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.trigger == Trigger.OnSelect || (this.trigger == Trigger.OnSelectTrue && isSelected) || (this.trigger == Trigger.OnSelectFalse && !isSelected))
		{
			this.Play(isSelected, this.dualState);
		}
	}

	// Token: 0x060002DC RID: 732 RVA: 0x00029A70 File Offset: 0x00027C70
	private void OnToggle()
	{
		if (!base.enabled || UIToggle.current == null)
		{
			return;
		}
		if (this.trigger == Trigger.OnActivate || (this.trigger == Trigger.OnActivateTrue && UIToggle.current.value) || (this.trigger == Trigger.OnActivateFalse && !UIToggle.current.value))
		{
			this.Play(UIToggle.current.value, this.dualState);
		}
	}

	// Token: 0x060002DD RID: 733 RVA: 0x00029AF4 File Offset: 0x00027CF4
	private void OnDragOver()
	{
		if (base.enabled && this.dualState)
		{
			if (UICamera.currentTouch.dragged == base.gameObject)
			{
				this.Play(true, true);
			}
			else if (this.dragHighlight && this.trigger == Trigger.OnPress)
			{
				this.Play(true, true);
			}
		}
	}

	// Token: 0x060002DE RID: 734 RVA: 0x00006E59 File Offset: 0x00005059
	private void OnDragOut()
	{
		if (base.enabled && this.dualState && UICamera.hoveredObject != base.gameObject)
		{
			this.Play(false, true);
		}
	}

	// Token: 0x060002DF RID: 735 RVA: 0x00006E8E File Offset: 0x0000508E
	private void OnDrop(GameObject go)
	{
		if (base.enabled && this.trigger == Trigger.OnPress && UICamera.currentTouch.dragged != base.gameObject)
		{
			this.Play(false, true);
		}
	}

	// Token: 0x060002E0 RID: 736 RVA: 0x00006EC9 File Offset: 0x000050C9
	public void Play(bool forward)
	{
		this.Play(forward, true);
	}

	// Token: 0x060002E1 RID: 737 RVA: 0x00029B60 File Offset: 0x00027D60
	public void Play(bool forward, bool onlyIfDifferent)
	{
		if (this.target || this.animator)
		{
			if (onlyIfDifferent)
			{
				if (this.mActivated == forward)
				{
					return;
				}
				this.mActivated = forward;
			}
			if (this.clearSelection && UICamera.selectedObject == base.gameObject)
			{
				UICamera.selectedObject = null;
			}
			int num = (int)(-(int)this.playDirection);
			Direction direction = (Direction)((!forward) ? num : ((int)this.playDirection));
			ActiveAnimation activeAnimation = (!this.target) ? ActiveAnimation.Play(this.animator, this.clipName, direction, this.ifDisabledOnPlay, this.disableWhenFinished) : ActiveAnimation.Play(this.target, this.clipName, direction, this.ifDisabledOnPlay, this.disableWhenFinished);
			if (activeAnimation != null)
			{
				if (this.resetOnPlay)
				{
					activeAnimation.Reset();
				}
				for (int i = 0; i < this.onFinished.Count; i++)
				{
					EventDelegate.Add(activeAnimation.onFinished, new EventDelegate.Callback(this.OnFinished), true);
				}
			}
		}
	}

	// Token: 0x060002E2 RID: 738 RVA: 0x00006ED3 File Offset: 0x000050D3
	public void PlayForward()
	{
		this.Play(true);
	}

	// Token: 0x060002E3 RID: 739 RVA: 0x00006EDC File Offset: 0x000050DC
	public void PlayReverse()
	{
		this.Play(false);
	}

	// Token: 0x060002E4 RID: 740 RVA: 0x00029C8C File Offset: 0x00027E8C
	private void OnFinished()
	{
		if (UIPlayAnimation.current == null)
		{
			UIPlayAnimation.current = this;
			EventDelegate.Execute(this.onFinished);
			if (this.eventReceiver != null && !string.IsNullOrEmpty(this.callWhenFinished))
			{
				this.eventReceiver.SendMessage(this.callWhenFinished, SendMessageOptions.DontRequireReceiver);
			}
			this.eventReceiver = null;
			UIPlayAnimation.current = null;
		}
	}

	// Token: 0x040001E7 RID: 487
	public static UIPlayAnimation current;

	// Token: 0x040001E8 RID: 488
	public Animation target;

	// Token: 0x040001E9 RID: 489
	public Animator animator;

	// Token: 0x040001EA RID: 490
	public string clipName;

	// Token: 0x040001EB RID: 491
	public Trigger trigger;

	// Token: 0x040001EC RID: 492
	public Direction playDirection = Direction.Forward;

	// Token: 0x040001ED RID: 493
	public bool resetOnPlay;

	// Token: 0x040001EE RID: 494
	public bool clearSelection;

	// Token: 0x040001EF RID: 495
	public EnableCondition ifDisabledOnPlay;

	// Token: 0x040001F0 RID: 496
	public DisableCondition disableWhenFinished;

	// Token: 0x040001F1 RID: 497
	public List<EventDelegate> onFinished = new List<EventDelegate>();

	// Token: 0x040001F2 RID: 498
	[HideInInspector]
	[SerializeField]
	private GameObject eventReceiver;

	// Token: 0x040001F3 RID: 499
	[SerializeField]
	[HideInInspector]
	private string callWhenFinished;

	// Token: 0x040001F4 RID: 500
	private bool mStarted;

	// Token: 0x040001F5 RID: 501
	private bool mActivated;

	// Token: 0x040001F6 RID: 502
	private bool dragHighlight;
}
