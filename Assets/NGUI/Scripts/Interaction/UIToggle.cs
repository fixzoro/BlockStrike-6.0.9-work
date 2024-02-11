using System;
using System.Collections.Generic;
using AnimationOrTween;
using UnityEngine;

// Token: 0x02000084 RID: 132
[AddComponentMenu("NGUI/Interaction/Toggle")]
[ExecuteInEditMode]
public class UIToggle : UIWidgetContainer
{
	// Token: 0x17000048 RID: 72
	// (get) Token: 0x060003BB RID: 955 RVA: 0x00007855 File Offset: 0x00005A55
	// (set) Token: 0x060003BC RID: 956 RVA: 0x0002FB4C File Offset: 0x0002DD4C
	public bool value
	{
		get
		{
			return (!this.mStarted) ? this.startsActive : this.mIsActive;
		}
		set
		{
			if (!this.mStarted)
			{
				this.startsActive = value;
			}
			else if (this.group == 0 || value || this.optionCanBeNone || !this.mStarted)
			{
				this.Set(value, true);
			}
		}
	}

	// Token: 0x17000049 RID: 73
	// (get) Token: 0x060003BD RID: 957 RVA: 0x0002831C File Offset: 0x0002651C
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

	// Token: 0x1700004A RID: 74
	// (get) Token: 0x060003BE RID: 958 RVA: 0x00007873 File Offset: 0x00005A73
	// (set) Token: 0x060003BF RID: 959 RVA: 0x0000787B File Offset: 0x00005A7B
	[Obsolete("Use 'value' instead")]
	public bool isChecked
	{
		get
		{
			return this.value;
		}
		set
		{
			this.value = value;
		}
	}

	// Token: 0x060003C0 RID: 960 RVA: 0x0002FBA0 File Offset: 0x0002DDA0
	public static UIToggle GetActiveToggle(int group)
	{
		for (int i = 0; i < UIToggle.list.size; i++)
		{
			UIToggle uitoggle = UIToggle.list[i];
			if (uitoggle != null && uitoggle.group == group && uitoggle.mIsActive)
			{
				return uitoggle;
			}
		}
		return null;
	}

	// Token: 0x060003C1 RID: 961 RVA: 0x00007884 File Offset: 0x00005A84
	private void OnEnable()
	{
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Combine(UICamera.onClick, new UICamera.VoidDelegate(this.OnClick));
		UIToggle.list.Add(this);
	}

	// Token: 0x060003C2 RID: 962 RVA: 0x000078B1 File Offset: 0x00005AB1
	private void OnDisable()
	{
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Remove(UICamera.onClick, new UICamera.VoidDelegate(this.OnClick));
		UIToggle.list.Remove(this);
	}

	// Token: 0x060003C3 RID: 963 RVA: 0x0002FBFC File Offset: 0x0002DDFC
	public void Start()
	{
		this.mGameObject = base.gameObject;
		if (this.mStarted)
		{
			return;
		}
		if (this.startsChecked)
		{
			this.startsChecked = false;
			this.startsActive = true;
		}
		if (!Application.isPlaying)
		{
			if (this.checkSprite != null && this.activeSprite == null)
			{
				this.activeSprite = this.checkSprite;
				this.checkSprite = null;
			}
			if (this.checkAnimation != null && this.activeAnimation == null)
			{
				this.activeAnimation = this.checkAnimation;
				this.checkAnimation = null;
			}
			if (Application.isPlaying && this.activeSprite != null)
			{
				this.activeSprite.alpha = ((!this.invertSpriteState) ? ((!this.startsActive) ? 0f : 1f) : ((!this.startsActive) ? 1f : 0f));
			}
			if (EventDelegate.IsValid(this.onChange))
			{
				this.eventReceiver = null;
				this.functionName = null;
			}
		}
		else
		{
			this.mIsActive = !this.startsActive;
			this.mStarted = true;
			bool flag = this.instantTween;
			this.instantTween = true;
			this.Set(this.startsActive, true);
			this.instantTween = flag;
		}
	}

	// Token: 0x060003C4 RID: 964 RVA: 0x0002FD74 File Offset: 0x0002DF74
	private void OnClick(GameObject go)
	{
		if (this.mGameObject != go)
		{
			return;
		}
		if (base.enabled && this.isColliderEnabled && UICamera.currentTouchID != -2)
		{
			this.value = !this.value;
		}
	}

	// Token: 0x060003C5 RID: 965 RVA: 0x0002FDC4 File Offset: 0x0002DFC4
	public void Set(bool state, bool notify = true)
	{
		if (this.validator != null && !this.validator(state))
		{
			return;
		}
		if (!this.mStarted)
		{
			this.mIsActive = state;
			this.startsActive = state;
			if (this.activeSprite != null)
			{
				this.activeSprite.alpha = ((!this.invertSpriteState) ? ((!state) ? 0f : 1f) : ((!state) ? 1f : 0f));
			}
		}
		else if (this.mIsActive != state)
		{
			if (this.group != 0 && state)
			{
				int i = 0;
				int size = UIToggle.list.size;
				while (i < size)
				{
					UIToggle uitoggle = UIToggle.list[i];
					if (uitoggle != this && uitoggle.group == this.group)
					{
						uitoggle.Set(false, true);
					}
					if (UIToggle.list.size != size)
					{
						size = UIToggle.list.size;
						i = 0;
					}
					else
					{
						i++;
					}
				}
			}
			this.mIsActive = state;
			if (this.activeSprite != null)
			{
				if (this.instantTween || !NGUITools.GetActive(this))
				{
					this.activeSprite.alpha = ((!this.invertSpriteState) ? ((!this.mIsActive) ? 0f : 1f) : ((!this.mIsActive) ? 1f : 0f));
				}
				else
				{
					TweenAlpha.Begin(this.activeSprite.gameObject, 0.15f, (!this.invertSpriteState) ? ((!this.mIsActive) ? 0f : 1f) : ((!this.mIsActive) ? 1f : 0f), 0f);
				}
			}
			if (notify && UIToggle.current == null)
			{
				UIToggle uitoggle2 = UIToggle.current;
				UIToggle.current = this;
				if (EventDelegate.IsValid(this.onChange))
				{
					EventDelegate.Execute(this.onChange);
				}
				else if (this.eventReceiver != null && !string.IsNullOrEmpty(this.functionName))
				{
					this.eventReceiver.SendMessage(this.functionName, this.mIsActive, SendMessageOptions.DontRequireReceiver);
				}
				UIToggle.current = uitoggle2;
			}
			if (this.animator != null)
			{
				ActiveAnimation activeAnimation = ActiveAnimation.Play(this.animator, null, (!state) ? Direction.Reverse : Direction.Forward, EnableCondition.IgnoreDisabledState, DisableCondition.DoNotDisable);
				if (activeAnimation != null && (this.instantTween || !NGUITools.GetActive(this)))
				{
					activeAnimation.Finish();
				}
			}
			else if (this.activeAnimation != null)
			{
				ActiveAnimation activeAnimation2 = ActiveAnimation.Play(this.activeAnimation, null, (!state) ? Direction.Reverse : Direction.Forward, EnableCondition.IgnoreDisabledState, DisableCondition.DoNotDisable);
				if (activeAnimation2 != null && (this.instantTween || !NGUITools.GetActive(this)))
				{
					activeAnimation2.Finish();
				}
			}
			else if (this.tween != null)
			{
				bool active = NGUITools.GetActive(this);
				if (this.tween.tweenGroup != 0)
				{
					UITweener[] componentsInChildren = this.tween.GetComponentsInChildren<UITweener>(true);
					int j = 0;
					int num = componentsInChildren.Length;
					while (j < num)
					{
						UITweener uitweener = componentsInChildren[j];
						if (uitweener.tweenGroup == this.tween.tweenGroup)
						{
							uitweener.Play(state);
							if (this.instantTween || !active)
							{
								uitweener.tweenFactor = ((!state) ? 0f : 1f);
							}
						}
						j++;
					}
				}
				else
				{
					this.tween.Play(state);
					if (this.instantTween || !active)
					{
						this.tween.tweenFactor = ((!state) ? 0f : 1f);
					}
				}
			}
		}
	}

	// Token: 0x040002D4 RID: 724
	public static BetterList<UIToggle> list = new BetterList<UIToggle>();

	// Token: 0x040002D5 RID: 725
	public static UIToggle current;

	// Token: 0x040002D6 RID: 726
	public int group;

	// Token: 0x040002D7 RID: 727
	public UIWidget activeSprite;

	// Token: 0x040002D8 RID: 728
	public bool invertSpriteState;

	// Token: 0x040002D9 RID: 729
	public Animation activeAnimation;

	// Token: 0x040002DA RID: 730
	public Animator animator;

	// Token: 0x040002DB RID: 731
	public UITweener tween;

	// Token: 0x040002DC RID: 732
	public bool startsActive;

	// Token: 0x040002DD RID: 733
	public bool instantTween;

	// Token: 0x040002DE RID: 734
	public bool optionCanBeNone;

	// Token: 0x040002DF RID: 735
	public List<EventDelegate> onChange = new List<EventDelegate>();

	// Token: 0x040002E0 RID: 736
	public UIToggle.Validate validator;

	// Token: 0x040002E1 RID: 737
	[SerializeField]
	[HideInInspector]
	private UISprite checkSprite;

	// Token: 0x040002E2 RID: 738
	[SerializeField]
	[HideInInspector]
	private Animation checkAnimation;

	// Token: 0x040002E3 RID: 739
	[HideInInspector]
	[SerializeField]
	private GameObject eventReceiver;

	// Token: 0x040002E4 RID: 740
	[HideInInspector]
	[SerializeField]
	private string functionName = "OnActivate";

	// Token: 0x040002E5 RID: 741
	[SerializeField]
	[HideInInspector]
	private bool startsChecked;

	// Token: 0x040002E6 RID: 742
	private bool mIsActive = true;

	// Token: 0x040002E7 RID: 743
	private bool mStarted;

	// Token: 0x040002E8 RID: 744
	private GameObject mGameObject;

	// Token: 0x02000085 RID: 133
	// (Invoke) Token: 0x060003C7 RID: 967
	public delegate bool Validate(bool choice);
}
