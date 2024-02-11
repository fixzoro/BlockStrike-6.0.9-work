using System;
using System.Collections.Generic;
using AnimationOrTween;
using UnityEngine;

// Token: 0x02000069 RID: 105
[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Play Tween")]
public class UIPlayTween : MonoBehaviour
{
	// Token: 0x060002EF RID: 751 RVA: 0x00006FD8 File Offset: 0x000051D8
	private void Awake()
	{
		this.mGameObject = base.gameObject;
		if (this.eventReceiver != null && EventDelegate.IsValid(this.onFinished))
		{
			this.eventReceiver = null;
			this.callWhenFinished = null;
		}
	}

	// Token: 0x060002F0 RID: 752 RVA: 0x00007015 File Offset: 0x00005215
	private void Start()
	{
		this.mStarted = true;
		if (this.tweenTarget == null)
		{
			this.tweenTarget = base.gameObject;
		}
	}

	// Token: 0x060002F1 RID: 753 RVA: 0x00029E1C File Offset: 0x0002801C
	private void OnEnable()
	{
		if (this.mStarted)
		{
			this.OnHover(this.mGameObject, UICamera.IsHighlighted(this.mGameObject));
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
		UICamera.onDragOver = (UICamera.ObjectDelegate)Delegate.Combine(UICamera.onDragOver, new UICamera.ObjectDelegate(this.OnDragOver));
		UICamera.onHover = (UICamera.BoolDelegate)Delegate.Combine(UICamera.onHover, new UICamera.BoolDelegate(this.OnHover));
		UICamera.onDragOut = (UICamera.ObjectDelegate)Delegate.Combine(UICamera.onDragOut, new UICamera.ObjectDelegate(this.OnDragOut));
		UICamera.onPress = (UICamera.BoolDelegate)Delegate.Combine(UICamera.onPress, new UICamera.BoolDelegate(this.OnPress));
		UICamera.onDoubleClick = (UICamera.VoidDelegate)Delegate.Combine(UICamera.onDoubleClick, new UICamera.VoidDelegate(this.OnDoubleClick));
		UICamera.onSelect = (UICamera.BoolDelegate)Delegate.Combine(UICamera.onSelect, new UICamera.BoolDelegate(this.OnSelect));
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Combine(UICamera.onClick, new UICamera.VoidDelegate(this.OnClick));
	}

	// Token: 0x060002F2 RID: 754 RVA: 0x00029FC8 File Offset: 0x000281C8
	private void OnDisable()
	{
		UIToggle component = base.GetComponent<UIToggle>();
		if (component != null)
		{
			EventDelegate.Remove(component.onChange, new EventDelegate.Callback(this.OnToggle));
		}
	}

	// Token: 0x060002F3 RID: 755 RVA: 0x0000703B File Offset: 0x0000523B
	private void OnDragOver(GameObject go, GameObject obj)
	{
		if (go != this.mGameObject)
		{
			return;
		}
		if (this.trigger == Trigger.OnHover)
		{
			this.OnHover(go, true);
		}
	}

	// Token: 0x060002F4 RID: 756 RVA: 0x0002A000 File Offset: 0x00028200
	private void OnHover(GameObject go, bool isOver)
	{
		if (go != this.mGameObject)
		{
			return;
		}
		if (base.enabled && (this.trigger == Trigger.OnHover || (this.trigger == Trigger.OnHoverTrue && isOver) || (this.trigger == Trigger.OnHoverFalse && !isOver)))
		{
			if (isOver == this.mActivated)
			{
				return;
			}
			if (!isOver && UICamera.hoveredObject != null && UICamera.hoveredObject.transform.IsChildOf(base.transform))
			{
				UICamera.onHover = (UICamera.BoolDelegate)Delegate.Combine(UICamera.onHover, new UICamera.BoolDelegate(this.CustomHoverListener));
				isOver = true;
				if (this.mActivated)
				{
					return;
				}
			}
			this.mActivated = (isOver && this.trigger == Trigger.OnHover);
			this.Play(isOver);
		}
	}

	// Token: 0x060002F5 RID: 757 RVA: 0x0002A0E8 File Offset: 0x000282E8
	private void CustomHoverListener(GameObject go, bool isOver)
	{
		if (!this)
		{
			return;
		}
		GameObject gameObject = base.gameObject;
		if (!gameObject || !go || (!(go == gameObject) && !go.transform.IsChildOf(base.transform)))
		{
			this.OnHover(this.mGameObject, false);
			UICamera.onHover = (UICamera.BoolDelegate)Delegate.Remove(UICamera.onHover, new UICamera.BoolDelegate(this.CustomHoverListener));
		}
	}

	// Token: 0x060002F6 RID: 758 RVA: 0x00007063 File Offset: 0x00005263
	private void OnDragOut(GameObject go, GameObject obj)
	{
		if (go != this.mGameObject)
		{
			return;
		}
		if (base.enabled && this.mActivated)
		{
			this.mActivated = false;
			this.Play(false);
		}
	}

	// Token: 0x060002F7 RID: 759 RVA: 0x0002A178 File Offset: 0x00028378
	private void OnPress(GameObject go, bool isPressed)
	{
		if (go != this.mGameObject)
		{
			return;
		}
		if (base.enabled && (this.trigger == Trigger.OnPress || (this.trigger == Trigger.OnPressTrue && isPressed) || (this.trigger == Trigger.OnPressFalse && !isPressed)))
		{
			this.mActivated = (isPressed && this.trigger == Trigger.OnPress);
			this.Play(isPressed);
		}
	}

	// Token: 0x060002F8 RID: 760 RVA: 0x0000709B File Offset: 0x0000529B
	private void OnClick(GameObject go)
	{
		if (go != this.mGameObject)
		{
			return;
		}
		if (base.enabled && this.trigger == Trigger.OnClick)
		{
			this.Play(true);
		}
	}

	// Token: 0x060002F9 RID: 761 RVA: 0x000070CC File Offset: 0x000052CC
	private void OnDoubleClick(GameObject go)
	{
		if (go != this.mGameObject)
		{
			return;
		}
		if (base.enabled && this.trigger == Trigger.OnDoubleClick)
		{
			this.Play(true);
		}
	}

	// Token: 0x060002FA RID: 762 RVA: 0x0002A1F4 File Offset: 0x000283F4
	private void OnSelect(GameObject go, bool isSelected)
	{
		if (go != this.mGameObject)
		{
			return;
		}
		if (base.enabled && (this.trigger == Trigger.OnSelect || (this.trigger == Trigger.OnSelectTrue && isSelected) || (this.trigger == Trigger.OnSelectFalse && !isSelected)))
		{
			this.mActivated = (isSelected && this.trigger == Trigger.OnSelect);
			this.Play(isSelected);
		}
	}

	// Token: 0x060002FB RID: 763 RVA: 0x0002A274 File Offset: 0x00028474
	private void OnToggle()
	{
		if (!base.enabled || UIToggle.current == null)
		{
			return;
		}
		if (this.trigger == Trigger.OnActivate || (this.trigger == Trigger.OnActivateTrue && UIToggle.current.value) || (this.trigger == Trigger.OnActivateFalse && !UIToggle.current.value))
		{
			this.Play(UIToggle.current.value);
		}
	}

	// Token: 0x060002FC RID: 764 RVA: 0x0002A2F0 File Offset: 0x000284F0
	private void Update()
	{
		if (this.disableWhenFinished != DisableCondition.DoNotDisable && this.mTweens != null)
		{
			bool flag = true;
			bool flag2 = true;
			int i = 0;
			int num = this.mTweens.Length;
			while (i < num)
			{
				UITweener uitweener = this.mTweens[i];
				if (uitweener.tweenGroup == this.tweenGroup)
				{
					if (uitweener.enabled)
					{
						flag = false;
						break;
					}
					if (uitweener.direction != (Direction)this.disableWhenFinished)
					{
						flag2 = false;
					}
				}
				i++;
			}
			if (flag)
			{
				if (flag2)
				{
					NGUITools.SetActive(this.tweenTarget, false);
				}
				this.mTweens = null;
			}
		}
	}

	// Token: 0x060002FD RID: 765 RVA: 0x000070FF File Offset: 0x000052FF
	public void Play()
	{
		this.Play(true);
	}

	// Token: 0x060002FE RID: 766 RVA: 0x0002A39C File Offset: 0x0002859C
	public void Play(bool forward)
	{
		this.mActive = 0;
		GameObject gameObject = (!(this.tweenTarget == null)) ? this.tweenTarget : base.gameObject;
		if (!NGUITools.GetActive(gameObject))
		{
			if (this.ifDisabledOnPlay != EnableCondition.EnableThenPlay)
			{
				return;
			}
			NGUITools.SetActive(gameObject, true);
		}
		this.mTweens = ((!this.includeChildren) ? gameObject.GetComponents<UITweener>() : gameObject.GetComponentsInChildren<UITweener>());
		if (this.mTweens.Length == 0)
		{
			if (this.disableWhenFinished != DisableCondition.DoNotDisable)
			{
				NGUITools.SetActive(this.tweenTarget, false);
			}
		}
		else
		{
			bool flag = false;
			if (this.playDirection == Direction.Reverse)
			{
				forward = !forward;
			}
			int i = 0;
			int num = this.mTweens.Length;
			while (i < num)
			{
				UITweener uitweener = this.mTweens[i];
				if (uitweener.tweenGroup == this.tweenGroup)
				{
					if (!flag && !NGUITools.GetActive(gameObject))
					{
						flag = true;
						NGUITools.SetActive(gameObject, true);
					}
					this.mActive++;
					if (this.playDirection == Direction.Toggle)
					{
						EventDelegate.Add(uitweener.onFinished, new EventDelegate.Callback(this.OnFinished), true);
						uitweener.Toggle();
					}
					else
					{
						if (this.resetOnPlay || (this.resetIfDisabled && !uitweener.enabled))
						{
							uitweener.Play(forward);
							uitweener.ResetToBeginning();
						}
						EventDelegate.Add(uitweener.onFinished, new EventDelegate.Callback(this.OnFinished), true);
						uitweener.Play(forward);
					}
				}
				i++;
			}
		}
	}

	// Token: 0x060002FF RID: 767 RVA: 0x0002A534 File Offset: 0x00028734
	private void OnFinished()
	{
		if (--this.mActive == 0 && UIPlayTween.current == null)
		{
			UIPlayTween.current = this;
			EventDelegate.Execute(this.onFinished);
			if (this.eventReceiver != null && !string.IsNullOrEmpty(this.callWhenFinished))
			{
				this.eventReceiver.SendMessage(this.callWhenFinished, SendMessageOptions.DontRequireReceiver);
			}
			this.eventReceiver = null;
			UIPlayTween.current = null;
		}
	}

	// Token: 0x04000205 RID: 517
	public static UIPlayTween current;

	// Token: 0x04000206 RID: 518
	public GameObject tweenTarget;

	// Token: 0x04000207 RID: 519
	public int tweenGroup;

	// Token: 0x04000208 RID: 520
	public Trigger trigger;

	// Token: 0x04000209 RID: 521
	public Direction playDirection = Direction.Forward;

	// Token: 0x0400020A RID: 522
	public bool resetOnPlay;

	// Token: 0x0400020B RID: 523
	public bool resetIfDisabled;

	// Token: 0x0400020C RID: 524
	public EnableCondition ifDisabledOnPlay;

	// Token: 0x0400020D RID: 525
	public DisableCondition disableWhenFinished;

	// Token: 0x0400020E RID: 526
	public bool includeChildren;

	// Token: 0x0400020F RID: 527
	public List<EventDelegate> onFinished = new List<EventDelegate>();

	// Token: 0x04000210 RID: 528
	[SerializeField]
	[HideInInspector]
	private GameObject eventReceiver;

	// Token: 0x04000211 RID: 529
	[HideInInspector]
	[SerializeField]
	private string callWhenFinished;

	// Token: 0x04000212 RID: 530
	private UITweener[] mTweens;

	// Token: 0x04000213 RID: 531
	private bool mStarted;

	// Token: 0x04000214 RID: 532
	private int mActive;

	// Token: 0x04000215 RID: 533
	private bool mActivated;

	// Token: 0x04000216 RID: 534
	private GameObject mGameObject;
}
