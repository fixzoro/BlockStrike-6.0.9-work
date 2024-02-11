using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200006A RID: 106
[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Popup List")]
public class UIPopupList : UIWidgetContainer
{
	// Token: 0x17000020 RID: 32
	// (get) Token: 0x06000302 RID: 770 RVA: 0x00007108 File Offset: 0x00005308
	// (set) Token: 0x06000303 RID: 771 RVA: 0x0002A67C File Offset: 0x0002887C
	public UnityEngine.Object ambigiousFont
	{
		get
		{
			if (this.trueTypeFont != null)
			{
				return this.trueTypeFont;
			}
			if (this.bitmapFont != null)
			{
				return this.bitmapFont;
			}
			return this.font;
		}
		set
		{
			if (value is Font)
			{
				this.trueTypeFont = (value as Font);
				this.bitmapFont = null;
				this.font = null;
			}
			else if (value is INGUIFont)
			{
				this.bitmapFont = value;
				this.trueTypeFont = null;
				this.font = null;
			}
		}
	}

	// Token: 0x17000021 RID: 33
	// (get) Token: 0x06000304 RID: 772 RVA: 0x00007140 File Offset: 0x00005340
	// (set) Token: 0x06000305 RID: 773 RVA: 0x00007148 File Offset: 0x00005348
	[Obsolete("Use EventDelegate.Add(popup.onChange, YourCallback) instead, and UIPopupList.current.value to determine the state")]
	public UIPopupList.LegacyEvent onSelectionChange
	{
		get
		{
			return this.mLegacyEvent;
		}
		set
		{
			this.mLegacyEvent = value;
		}
	}

	// Token: 0x17000022 RID: 34
	// (get) Token: 0x06000306 RID: 774 RVA: 0x00007151 File Offset: 0x00005351
	public static bool isOpen
	{
		get
		{
			return UIPopupList.current != null && (UIPopupList.mChild != null || UIPopupList.mFadeOutComplete > Time.unscaledTime);
		}
	}

	// Token: 0x17000023 RID: 35
	// (get) Token: 0x06000307 RID: 775 RVA: 0x00007185 File Offset: 0x00005385
	// (set) Token: 0x06000308 RID: 776 RVA: 0x0000718D File Offset: 0x0000538D
	public virtual string value
	{
		get
		{
			return this.mSelectedItem;
		}
		set
		{
			this.Set(value, true);
		}
	}

	// Token: 0x17000024 RID: 36
	// (get) Token: 0x06000309 RID: 777 RVA: 0x0002A6D4 File Offset: 0x000288D4
	public virtual object data
	{
		get
		{
			int num = this.items.IndexOf(this.mSelectedItem);
			return (num <= -1 || num >= this.itemData.Count) ? null : this.itemData[num];
		}
	}

	// Token: 0x17000025 RID: 37
	// (get) Token: 0x0600030A RID: 778 RVA: 0x0002A720 File Offset: 0x00028920
	public Action callback
	{
		get
		{
			int num = this.items.IndexOf(this.mSelectedItem);
			return (num <= -1 || num >= this.itemCallbacks.Count) ? null : this.itemCallbacks[num];
		}
	}

	// Token: 0x17000026 RID: 38
	// (get) Token: 0x0600030B RID: 779 RVA: 0x0002831C File Offset: 0x0002651C
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

	// Token: 0x17000027 RID: 39
	// (get) Token: 0x0600030C RID: 780 RVA: 0x00007197 File Offset: 0x00005397
	protected bool isValid
	{
		get
		{
			return this.bitmapFont != null || this.trueTypeFont != null;
		}
	}

	// Token: 0x17000028 RID: 40
	// (get) Token: 0x0600030D RID: 781 RVA: 0x0002A76C File Offset: 0x0002896C
	protected int activeFontSize
	{
		get
		{
			if (this.trueTypeFont != null || this.bitmapFont == null)
			{
				return this.fontSize;
			}
			INGUIFont inguifont = this.bitmapFont as INGUIFont;
			return (inguifont == null) ? this.fontSize : inguifont.defaultSize;
		}
	}

	// Token: 0x17000029 RID: 41
	// (get) Token: 0x0600030E RID: 782 RVA: 0x0002A7C8 File Offset: 0x000289C8
	protected float activeFontScale
	{
		get
		{
			if (this.trueTypeFont != null || this.bitmapFont == null)
			{
				return 1f;
			}
			INGUIFont inguifont = this.bitmapFont as INGUIFont;
			return (inguifont == null) ? 1f : ((float)this.fontSize / (float)inguifont.defaultSize);
		}
	}

	// Token: 0x1700002A RID: 42
	// (get) Token: 0x0600030F RID: 783 RVA: 0x0002A828 File Offset: 0x00028A28
	protected float fitScale
	{
		get
		{
			if (this.separatePanel)
			{
				float num = (float)this.items.Count * ((float)this.fontSize + this.padding.y) + this.padding.y;
				float y = NGUITools.screenSize.y;
				if (num > y)
				{
					return y / num;
				}
			}
			else if (this.mPanel != null && this.mPanel.anchorCamera != null && this.mPanel.anchorCamera.orthographic)
			{
				float num2 = (float)this.items.Count * ((float)this.fontSize + this.padding.y) + this.padding.y;
				float height = this.mPanel.height;
				if (num2 > height)
				{
					return height / num2;
				}
			}
			return 1f;
		}
	}

	// Token: 0x06000310 RID: 784 RVA: 0x0002A914 File Offset: 0x00028B14
	public void Set(string value, bool notify = true)
	{
		if (this.mSelectedItem != value)
		{
			this.mSelectedItem = value;
			if (this.mSelectedItem == null)
			{
				return;
			}
			if (notify && this.mSelectedItem != null)
			{
				this.TriggerCallbacks();
			}
			if (!this.keepValue)
			{
				this.mSelectedItem = null;
			}
		}
	}

	// Token: 0x06000311 RID: 785 RVA: 0x000071B9 File Offset: 0x000053B9
	public virtual void Clear()
	{
		this.items.Clear();
		this.itemData.Clear();
		this.itemCallbacks.Clear();
	}

	// Token: 0x06000312 RID: 786 RVA: 0x000071DC File Offset: 0x000053DC
	public virtual void AddItem(string text)
	{
		this.items.Add(text);
		this.itemData.Add(text);
		this.itemCallbacks.Add(null);
	}

	// Token: 0x06000313 RID: 787 RVA: 0x00007202 File Offset: 0x00005402
	public virtual void AddItem(string text, Action del)
	{
		this.items.Add(text);
		this.itemCallbacks.Add(del);
	}

	// Token: 0x06000314 RID: 788 RVA: 0x0000721C File Offset: 0x0000541C
	public virtual void AddItem(string text, object data, Action del = null)
	{
		this.items.Add(text);
		this.itemData.Add(data);
		this.itemCallbacks.Add(del);
	}

	// Token: 0x06000315 RID: 789 RVA: 0x0002A970 File Offset: 0x00028B70
	public virtual void RemoveItem(string text)
	{
		int num = this.items.IndexOf(text);
		if (num != -1)
		{
			this.items.RemoveAt(num);
			this.itemData.RemoveAt(num);
			if (num < this.itemCallbacks.Count)
			{
				this.itemCallbacks.RemoveAt(num);
			}
		}
	}

	// Token: 0x06000316 RID: 790 RVA: 0x0002A9C8 File Offset: 0x00028BC8
	public virtual void RemoveItemByData(object data)
	{
		int num = this.itemData.IndexOf(data);
		if (num != -1)
		{
			this.items.RemoveAt(num);
			this.itemData.RemoveAt(num);
			if (num < this.itemCallbacks.Count)
			{
				this.itemCallbacks.RemoveAt(num);
			}
		}
	}

	// Token: 0x06000317 RID: 791 RVA: 0x0002AA20 File Offset: 0x00028C20
	protected void TriggerCallbacks()
	{
		if (!this.mExecuting)
		{
			this.mExecuting = true;
			UIPopupList uipopupList = UIPopupList.current;
			UIPopupList.current = this;
			if (this.mLegacyEvent != null)
			{
				this.mLegacyEvent(this.mSelectedItem);
			}
			if (EventDelegate.IsValid(this.onChange))
			{
				EventDelegate.Execute(this.onChange);
			}
			else if (this.eventReceiver != null && !string.IsNullOrEmpty(this.functionName))
			{
				this.eventReceiver.SendMessage(this.functionName, this.mSelectedItem, SendMessageOptions.DontRequireReceiver);
			}
			Action callback = this.callback;
			if (callback != null)
			{
				callback();
			}
			UIPopupList.current = uipopupList;
			this.mExecuting = false;
		}
	}

	// Token: 0x06000318 RID: 792 RVA: 0x0002AAE0 File Offset: 0x00028CE0
	protected virtual void OnEnable()
	{
		this.mGameObject = base.gameObject;
		if (EventDelegate.IsValid(this.onChange))
		{
			this.eventReceiver = null;
			this.functionName = null;
		}
		if (this.font != null)
		{
			if (this.font.isDynamic)
			{
				this.trueTypeFont = this.font.dynamicFont;
				this.fontStyle = this.font.dynamicFontStyle;
				this.mUseDynamicFont = true;
			}
			else if (this.bitmapFont == null)
			{
				this.bitmapFont = this.font;
				this.mUseDynamicFont = false;
			}
			this.font = null;
		}
		INGUIFont inguifont = this.bitmapFont as INGUIFont;
		if (this.textScale != 0f)
		{
			this.fontSize = ((inguifont == null) ? 16 : Mathf.RoundToInt((float)inguifont.defaultSize * this.textScale));
			this.textScale = 0f;
		}
		if (this.trueTypeFont == null && inguifont != null && inguifont.isDynamic && inguifont.replacement == null)
		{
			this.trueTypeFont = inguifont.dynamicFont;
			this.bitmapFont = null;
		}
		UICamera.onKey = (UICamera.KeyCodeDelegate)Delegate.Combine(UICamera.onKey, new UICamera.KeyCodeDelegate(this.OnKey));
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Combine(UICamera.onClick, new UICamera.VoidDelegate(this.OnClick));
		UICamera.onDoubleClick = (UICamera.VoidDelegate)Delegate.Combine(UICamera.onDoubleClick, new UICamera.VoidDelegate(this.OnDoubleClick));
		UICamera.onSelect = (UICamera.BoolDelegate)Delegate.Combine(UICamera.onSelect, new UICamera.BoolDelegate(this.OnSelect));
	}

	// Token: 0x06000319 RID: 793 RVA: 0x0002ACA4 File Offset: 0x00028EA4
	protected virtual void OnValidate()
	{
		Font x = this.trueTypeFont;
		INGUIFont inguifont = this.bitmapFont as INGUIFont;
		this.bitmapFont = null;
		this.trueTypeFont = null;
		if (x != null && (inguifont == null || !this.mUseDynamicFont))
		{
			this.bitmapFont = null;
			this.trueTypeFont = x;
			this.mUseDynamicFont = true;
		}
		else if (inguifont != null)
		{
			if (inguifont.replacement == null)
			{
				if (inguifont.isDynamic)
				{
					this.trueTypeFont = inguifont.dynamicFont;
					this.fontStyle = inguifont.dynamicFontStyle;
					this.fontSize = inguifont.defaultSize;
					this.mUseDynamicFont = true;
				}
				else
				{
					this.bitmapFont = (inguifont as UnityEngine.Object);
					this.mUseDynamicFont = false;
				}
			}
		}
		else
		{
			this.trueTypeFont = x;
			this.mUseDynamicFont = true;
		}
	}

	// Token: 0x0600031A RID: 794 RVA: 0x0002AD7C File Offset: 0x00028F7C
	public virtual void Start()
	{
		if (this.mStarted)
		{
			return;
		}
		this.mStarted = true;
		if (this.keepValue)
		{
			string value = this.mSelectedItem;
			this.mSelectedItem = null;
			this.value = value;
		}
		else
		{
			this.mSelectedItem = null;
		}
		if (this.textLabel != null)
		{
			EventDelegate.Add(this.onChange, new EventDelegate.Callback(this.textLabel.SetCurrentSelection));
			this.textLabel = null;
		}
	}

	// Token: 0x0600031B RID: 795 RVA: 0x00007242 File Offset: 0x00005442
	protected virtual void OnLocalize()
	{
		if (this.isLocalized)
		{
			this.TriggerCallbacks();
		}
	}

	// Token: 0x0600031C RID: 796 RVA: 0x0002AE00 File Offset: 0x00029000
	protected virtual void Highlight(UILabel lbl, bool instant)
	{
		if (this.mHighlight != null)
		{
			this.mHighlightedLabel = lbl;
			Vector3 highlightPosition = this.GetHighlightPosition();
			if (!instant && this.isAnimated)
			{
				TweenPosition.Begin(this.mHighlight.gameObject, 0.1f, highlightPosition).method = UITweener.Method.EaseOut;
				if (!this.mTweening)
				{
					this.mTweening = true;
					base.StartCoroutine("UpdateTweenPosition");
				}
			}
			else
			{
				this.mHighlight.cachedTransform.localPosition = highlightPosition;
			}
		}
	}

	// Token: 0x0600031D RID: 797 RVA: 0x0002AE90 File Offset: 0x00029090
	protected virtual Vector3 GetHighlightPosition()
	{
		if (this.mHighlightedLabel == null || this.mHighlight == null)
		{
			return Vector3.zero;
		}
		Vector4 border = this.mHighlight.border;
		float num = 1f;
		INGUIAtlas inguiatlas = this.atlas as INGUIAtlas;
		if (inguiatlas != null)
		{
			num = inguiatlas.pixelSize;
		}
		float num2 = border.x * num;
		float y = border.w * num;
		return this.mHighlightedLabel.cachedTransform.localPosition + new Vector3(-num2, y, 1f);
	}

	// Token: 0x0600031E RID: 798 RVA: 0x0002AF28 File Offset: 0x00029128
	protected virtual IEnumerator UpdateTweenPosition()
	{
		if (this.mHighlight != null && this.mHighlightedLabel != null)
		{
			TweenPosition tp = this.mHighlight.GetComponent<TweenPosition>();
			while (tp != null && tp.enabled)
			{
				tp.to = this.GetHighlightPosition();
				yield return null;
			}
		}
		this.mTweening = false;
		yield break;
	}

	// Token: 0x0600031F RID: 799 RVA: 0x0002AF44 File Offset: 0x00029144
	protected virtual void OnItemHover(GameObject go, bool isOver)
	{
		if (isOver)
		{
			UILabel component = go.GetComponent<UILabel>();
			this.Highlight(component, false);
		}
	}

	// Token: 0x06000320 RID: 800 RVA: 0x00007255 File Offset: 0x00005455
	protected virtual void OnItemPress(GameObject go, bool isPressed)
	{
		if (isPressed && this.selection == UIPopupList.Selection.OnPress)
		{
			this.OnItemClick(go);
		}
	}

	// Token: 0x06000321 RID: 801 RVA: 0x0002AF68 File Offset: 0x00029168
	protected virtual void OnItemClick(GameObject go)
	{
		this.Select(go.GetComponent<UILabel>(), true);
		UIEventListener component = go.GetComponent<UIEventListener>();
		this.value = (component.parameter as string);
		UIPlaySound[] components = base.GetComponents<UIPlaySound>();
		int i = 0;
		int num = components.Length;
		while (i < num)
		{
			UIPlaySound uiplaySound = components[i];
			if (uiplaySound.trigger == UIPlaySound.Trigger.OnClick)
			{
				NGUITools.PlaySound(uiplaySound.audioClip, uiplaySound.volume, 1f);
			}
			i++;
		}
		this.CloseSelf();
	}

	// Token: 0x06000322 RID: 802 RVA: 0x0000726F File Offset: 0x0000546F
	private void Select(UILabel lbl, bool instant)
	{
		this.Highlight(lbl, instant);
	}

	// Token: 0x06000323 RID: 803 RVA: 0x0002AFE8 File Offset: 0x000291E8
	protected virtual void OnNavigate(KeyCode key)
	{
		if (base.enabled && UIPopupList.current == this)
		{
			int num = this.mLabelList.IndexOf(this.mHighlightedLabel);
			if (num == -1)
			{
				num = 0;
			}
			if (key == KeyCode.UpArrow)
			{
				if (num > 0)
				{
					this.Select(this.mLabelList[num - 1], false);
				}
			}
			else if (key == KeyCode.DownArrow && num + 1 < this.mLabelList.Count)
			{
				this.Select(this.mLabelList[num + 1], false);
			}
		}
	}

	// Token: 0x06000324 RID: 804 RVA: 0x0002B090 File Offset: 0x00029290
	protected virtual void OnKey(GameObject go, KeyCode key)
	{
		if (go != this.mGameObject)
		{
			return;
		}
		if (base.enabled && UIPopupList.current == this && (key == UICamera.current.cancelKey0 || key == UICamera.current.cancelKey1))
		{
			this.OnSelect(go, false);
		}
	}

	// Token: 0x06000325 RID: 805 RVA: 0x0002B0F4 File Offset: 0x000292F4
	protected virtual void OnDisable()
	{
		this.CloseSelf();
		UICamera.onKey = (UICamera.KeyCodeDelegate)Delegate.Remove(UICamera.onKey, new UICamera.KeyCodeDelegate(this.OnKey));
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Remove(UICamera.onClick, new UICamera.VoidDelegate(this.OnClick));
		UICamera.onDoubleClick = (UICamera.VoidDelegate)Delegate.Remove(UICamera.onDoubleClick, new UICamera.VoidDelegate(this.OnDoubleClick));
		UICamera.onSelect = (UICamera.BoolDelegate)Delegate.Remove(UICamera.onSelect, new UICamera.BoolDelegate(this.OnSelect));
	}

	// Token: 0x06000326 RID: 806 RVA: 0x0000574F File Offset: 0x0000394F
	protected virtual void OnSelect(GameObject go, bool isSelected)
	{
	}

	// Token: 0x06000327 RID: 807 RVA: 0x00007279 File Offset: 0x00005479
	public static void Close()
	{
		if (UIPopupList.current != null)
		{
			UIPopupList.current.CloseSelf();
			UIPopupList.current = null;
		}
	}

	// Token: 0x06000328 RID: 808 RVA: 0x0002B18C File Offset: 0x0002938C
	public virtual void CloseSelf()
	{
		if (UIPopupList.mChild != null && UIPopupList.current == this)
		{
			base.StopCoroutine("CloseIfUnselected");
			this.mSelection = null;
			this.mLabelList.Clear();
			if (this.isAnimated)
			{
				UIWidget[] componentsInChildren = UIPopupList.mChild.GetComponentsInChildren<UIWidget>();
				int i = 0;
				int num = componentsInChildren.Length;
				while (i < num)
				{
					UIWidget uiwidget = componentsInChildren[i];
					Color color = uiwidget.color;
					color.a = 0f;
					TweenColor.Begin(uiwidget.gameObject, 0.15f, color).method = UITweener.Method.EaseOut;
					i++;
				}
				Collider[] componentsInChildren2 = UIPopupList.mChild.GetComponentsInChildren<Collider>();
				int j = 0;
				int num2 = componentsInChildren2.Length;
				while (j < num2)
				{
					componentsInChildren2[j].enabled = false;
					j++;
				}
				UnityEngine.Object.Destroy(UIPopupList.mChild, 0.15f);
				UIPopupList.mFadeOutComplete = Time.unscaledTime + Mathf.Max(0.1f, 0.15f);
			}
			else
			{
				UnityEngine.Object.Destroy(UIPopupList.mChild);
				UIPopupList.mFadeOutComplete = Time.unscaledTime + 0.1f;
			}
			this.mBackground = null;
			this.mHighlight = null;
			UIPopupList.mChild = null;
			UIPopupList.current = null;
		}
	}

	// Token: 0x06000329 RID: 809 RVA: 0x0002B2C8 File Offset: 0x000294C8
	protected virtual void AnimateColor(UIWidget widget)
	{
		Color color = widget.color;
		widget.color = new Color(color.r, color.g, color.b, 0f);
		TweenColor.Begin(widget.cachedGameObject, 0.15f, color).method = UITweener.Method.EaseOut;
	}

	// Token: 0x0600032A RID: 810 RVA: 0x0002B318 File Offset: 0x00029518
	protected virtual void AnimatePosition(UIWidget widget, bool placeAbove, float bottom)
	{
		Vector3 localPosition = widget.cachedTransform.localPosition;
		Vector3 localPosition2 = (!placeAbove) ? new Vector3(localPosition.x, 0f, localPosition.z) : new Vector3(localPosition.x, bottom, localPosition.z);
		widget.cachedTransform.localPosition = localPosition2;
		GameObject cachedGameObject = widget.cachedGameObject;
		TweenPosition.Begin(cachedGameObject, 0.15f, localPosition).method = UITweener.Method.EaseOut;
	}

	// Token: 0x0600032B RID: 811 RVA: 0x0002B390 File Offset: 0x00029590
	protected virtual void AnimateScale(UIWidget widget, bool placeAbove, float bottom)
	{
		GameObject cachedGameObject = widget.cachedGameObject;
		Transform cachedTransform = widget.cachedTransform;
		float fitScale = this.fitScale;
		float num = (float)this.activeFontSize * this.activeFontScale + this.mBgBorder * 2f;
		cachedTransform.localScale = new Vector3(fitScale, fitScale * num / (float)widget.height, fitScale);
		TweenScale.Begin(cachedGameObject, 0.15f, Vector3.one).method = UITweener.Method.EaseOut;
		if (placeAbove)
		{
			Vector3 localPosition = cachedTransform.localPosition;
			cachedTransform.localPosition = new Vector3(localPosition.x, localPosition.y - fitScale * (float)widget.height + fitScale * num, localPosition.z);
			TweenPosition.Begin(cachedGameObject, 0.15f, localPosition).method = UITweener.Method.EaseOut;
		}
	}

	// Token: 0x0600032C RID: 812 RVA: 0x0000729B File Offset: 0x0000549B
	protected void Animate(UIWidget widget, bool placeAbove, float bottom)
	{
		this.AnimateColor(widget);
		this.AnimatePosition(widget, placeAbove, bottom);
	}

	// Token: 0x0600032D RID: 813 RVA: 0x0002B44C File Offset: 0x0002964C
	protected virtual void OnClick(GameObject go)
	{
		if (go != this.mGameObject)
		{
			return;
		}
		if (this.mOpenFrame == Time.frameCount)
		{
			return;
		}
		if (UIPopupList.mChild == null)
		{
			if (this.openOn == UIPopupList.OpenOn.DoubleClick || this.openOn == UIPopupList.OpenOn.Manual)
			{
				return;
			}
			if (this.openOn == UIPopupList.OpenOn.RightClick && UICamera.currentTouchID != -2)
			{
				return;
			}
			this.Show();
		}
		else if (this.mHighlightedLabel != null)
		{
			this.OnItemPress(this.mHighlightedLabel.gameObject, true);
		}
	}

	// Token: 0x0600032E RID: 814 RVA: 0x000072AD File Offset: 0x000054AD
	protected virtual void OnDoubleClick(GameObject go)
	{
		if (go != this.mGameObject)
		{
			return;
		}
		if (this.openOn == UIPopupList.OpenOn.DoubleClick)
		{
			this.Show();
		}
	}

	// Token: 0x0600032F RID: 815 RVA: 0x0002B4EC File Offset: 0x000296EC
	private IEnumerator CloseIfUnselected()
	{
		GameObject sel;
		do
		{
			yield return null;
			sel = UICamera.selectedObject;
		}
		while (!(sel != this.mSelection) || (!(sel == null) && (sel == UIPopupList.mChild || NGUITools.IsChild(UIPopupList.mChild.transform, sel.transform))));
		this.CloseSelf();
		yield break;
	}

	// Token: 0x06000330 RID: 816 RVA: 0x0002B508 File Offset: 0x00029708
	public virtual void Show()
	{
		if (base.enabled && NGUITools.GetActive(this.mGameObject) && UIPopupList.mChild == null && this.isValid && this.items.Count > 0)
		{
			this.mLabelList.Clear();
			base.StopCoroutine("CloseIfUnselected");
			UICamera.selectedObject = (UICamera.hoveredObject ?? this.mGameObject);
			this.mSelection = UICamera.selectedObject;
			this.source = this.mSelection;
			if (this.source == null)
			{
				Debug.LogError("Popup list needs a source object...");
				return;
			}
			this.mOpenFrame = Time.frameCount;
			if (this.mPanel == null)
			{
				this.mPanel = UIPanel.Find(base.transform);
				if (this.mPanel == null)
				{
					return;
				}
			}
			UIPopupList.mChild = new GameObject("Drop-down List");
			UIPopupList.mChild.layer = this.mGameObject.layer;
			if (this.separatePanel)
			{
				if (base.GetComponent<Collider>() != null)
				{
					Rigidbody rigidbody = UIPopupList.mChild.AddComponent<Rigidbody>();
					rigidbody.isKinematic = true;
				}
				else if (base.GetComponent<Collider2D>() != null)
				{
					Rigidbody2D rigidbody2D = UIPopupList.mChild.AddComponent<Rigidbody2D>();
					rigidbody2D.isKinematic = true;
				}
				UIPanel uipanel = UIPopupList.mChild.AddComponent<UIPanel>();
				uipanel.depth = 1000000;
				uipanel.sortingOrder = this.mPanel.sortingOrder;
			}
			UIScrollView uiscrollView = NGUITools.AddChild(UIPopupList.mChild).AddComponent<UIScrollView>();
			uiscrollView.gameObject.AddComponent<Rigidbody>().isKinematic = true;
			uiscrollView.movement = UIScrollView.Movement.Vertical;
			uiscrollView.panel.clipping = UIDrawCall.Clipping.SoftClip;
			uiscrollView.panel.depth = 1000001;
			uiscrollView.disableDragIfFits = true;
			UIPopupList.current = this;
			Transform cachedTransform = this.mPanel.cachedTransform;
			Transform transform = UIPopupList.mChild.transform;
			transform.parent = cachedTransform;
			Transform parent = cachedTransform;
			if (this.separatePanel)
			{
				UIRoot uiroot = this.mPanel.GetComponentInParent<UIRoot>();
				if (uiroot == null && UIRoot.list.Count != 0)
				{
					uiroot = UIRoot.list[0];
				}
				if (uiroot != null)
				{
					parent = uiroot.transform;
				}
			}
			Vector3 vector;
			Vector3 a;
			if (this.openOn == UIPopupList.OpenOn.Manual && this.mSelection != this.mGameObject)
			{
				this.startingPosition = UICamera.lastEventPosition;
				vector = cachedTransform.InverseTransformPoint(this.mPanel.anchorCamera.ScreenToWorldPoint(this.startingPosition));
				a = vector;
				transform.localPosition = vector;
				this.startingPosition = transform.position;
			}
			else
			{
				Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(cachedTransform, base.transform, false, false);
				vector = bounds.min;
				a = bounds.max;
				transform.localPosition = vector;
				this.startingPosition = transform.position;
			}
			base.StartCoroutine("CloseIfUnselected");
			float fitScale = this.fitScale;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
			int num = (!this.separatePanel) ? NGUITools.CalculateNextDepth(this.mPanel.gameObject) : 0;
			if (this.background2DSprite != null)
			{
				UI2DSprite ui2DSprite = UIPopupList.mChild.AddWidget<UI2DSprite>(num);
				ui2DSprite.sprite2D = this.background2DSprite;
				this.mBackground = ui2DSprite;
			}
			else
			{
				if (!(this.atlas != null))
				{
					return;
				}
				this.mBackground = UIPopupList.mChild.AddSprite(this.atlas as INGUIAtlas, this.backgroundSprite, num);
			}
			bool flag = this.position == UIPopupList.Position.Above;
			if (this.position == UIPopupList.Position.Auto)
			{
				UICamera uicamera = UICamera.FindCameraForLayer(this.mSelection.layer);
				if (uicamera != null)
				{
					flag = (uicamera.cachedCamera.WorldToViewportPoint(this.startingPosition).y < 0.5f);
				}
			}
			this.mBackground.pivot = UIWidget.Pivot.TopLeft;
			this.mBackground.color = this.backgroundColor;
			this.mBackground.gameObject.AddComponent<UIDragScrollView>().scrollView = uiscrollView;
			uiscrollView.panel.SetAnchor(this.mBackground.cachedGameObject, 0, 0, 0, 0);
			uiscrollView.panel.updateAnchors = UIRect.AnchorUpdate.OnUpdate;
			Vector4 border = this.mBackground.border;
			this.mBgBorder = border.y;
			this.mBackground.cachedTransform.localPosition = new Vector3(0f, (!flag) ? ((float)this.overlap) : (border.y * 2f - (float)this.overlap), 0f);
			if (this.highlight2DSprite != null)
			{
				UI2DSprite ui2DSprite2 = UIPopupList.mChild.AddWidget<UI2DSprite>(num + 1);
				ui2DSprite2.sprite2D = this.highlight2DSprite;
				this.mHighlight = ui2DSprite2;
			}
			else
			{
				if (!(this.atlas != null))
				{
					return;
				}
				this.mHighlight = uiscrollView.gameObject.AddSprite(this.atlas as INGUIAtlas, this.highlightSprite, num + 1);
			}
			float num2 = 0f;
			float num3 = 0f;
			if (this.mHighlight.hasBorder)
			{
				num2 = this.mHighlight.border.w;
				num3 = this.mHighlight.border.x;
			}
			this.mHighlight.pivot = UIWidget.Pivot.TopLeft;
			this.mHighlight.color = this.highlightColor;
			float num4 = (float)this.activeFontSize * this.activeFontScale;
			float num5 = num4 + this.padding.y;
			float num6 = 0f;
			float num7 = (!flag) ? (-this.padding.y - border.y + (float)this.overlap) : (border.y - this.padding.y - (float)this.overlap);
			float num8 = border.y * 2f + this.padding.y;
			List<UILabel> list = new List<UILabel>();
			if (!this.items.Contains(this.mSelectedItem))
			{
				this.mSelectedItem = null;
			}
			int i = 0;
			int count = this.items.Count;
			while (i < count)
			{
				string text = this.items[i];
				UILabel uilabel = uiscrollView.gameObject.AddWidget<UILabel>(this.mBackground.depth + 2);
				uilabel.name = i.ToString();
				uilabel.pivot = UIWidget.Pivot.TopLeft;
				uilabel.bitmapFont = (this.bitmapFont as INGUIFont);
				uilabel.trueTypeFont = this.trueTypeFont;
				uilabel.fontSize = this.fontSize;
				uilabel.fontStyle = this.fontStyle;
				uilabel.text = ((!this.isLocalized) ? text : Localization.Get(text, true));
				uilabel.modifier = this.textModifier;
				uilabel.color = this.textColor;
				uilabel.cachedTransform.localPosition = new Vector3(border.x + this.padding.x - uilabel.pivotOffset.x, num7, -1f);
				uilabel.overflowMethod = UILabel.Overflow.ResizeFreely;
				uilabel.alignment = this.alignment;
				uilabel.symbolStyle = NGUIText.SymbolStyle.Colored;
				uilabel.gameObject.AddComponent<UIDragScrollView>().scrollView = uiscrollView;
				list.Add(uilabel);
				num8 += num5;
				num7 -= num5;
				num6 = Mathf.Max(num6, uilabel.printedSize.x);
				UIEventListener uieventListener = UIEventListener.Get(uilabel.gameObject);
				uieventListener.onHover = new UIEventListener.BoolDelegate(this.OnItemHover);
				uieventListener.onClick = new UIEventListener.VoidDelegate(this.OnItemClick);
				uieventListener.parameter = text;
				if (this.mSelectedItem == text || (i == 0 && string.IsNullOrEmpty(this.mSelectedItem)))
				{
					this.Highlight(uilabel, true);
				}
				this.mLabelList.Add(uilabel);
				i++;
			}
			num6 = Mathf.Max(num6, a.x - vector.x - (border.x + this.padding.x) * 2f);
			float num9 = num6;
			Vector3 vector2 = new Vector3(num9 * 0.5f, -num4 * 0.5f, 0f);
			Vector3 vector3 = new Vector3(num9, num4 + this.padding.y, 1f);
			int j = 0;
			int count2 = list.Count;
			while (j < count2)
			{
				UILabel uilabel2 = list[j];
				NGUITools.AddWidgetCollider(uilabel2.gameObject);
				uilabel2.autoResizeBoxCollider = false;
				BoxCollider component = uilabel2.GetComponent<BoxCollider>();
				if (component != null)
				{
					vector2.z = component.center.z;
					component.center = vector2;
					component.size = vector3;
				}
				else
				{
					BoxCollider2D component2 = uilabel2.GetComponent<BoxCollider2D>();
					component2.offset = vector2;
					component2.size = vector3;
				}
				j++;
			}
			int width = Mathf.RoundToInt(num6);
			num6 += (border.x + this.padding.x) * 2f;
			num7 -= border.y;
			this.mBackground.width = Mathf.RoundToInt(num6);
			this.mBackground.height = Mathf.RoundToInt(num8);
			if (this.minHeight > 0)
			{
				this.mBackground.height = Mathf.Min(this.mBackground.height, this.minHeight);
			}
			int k = 0;
			int count3 = list.Count;
			while (k < count3)
			{
				UILabel uilabel3 = list[k];
				uilabel3.overflowMethod = UILabel.Overflow.ShrinkContent;
				uilabel3.width = width;
				k++;
			}
			float num10 = 2f;
			INGUIAtlas inguiatlas = this.atlas as INGUIAtlas;
			if (inguiatlas != null)
			{
				num10 *= inguiatlas.pixelSize;
			}
			float f = num6 - (border.x + this.padding.x) * 2f + num3 * num10;
			float f2 = num4 + num2 * num10;
			this.mHighlight.width = Mathf.RoundToInt(f);
			this.mHighlight.height = Mathf.RoundToInt(f2);
			if (this.isAnimated)
			{
				this.AnimateColor(this.mBackground);
				if (Time.timeScale == 0f || Time.timeScale >= 0.1f)
				{
					float bottom = num7 + num4;
					this.Animate(this.mHighlight, flag, bottom);
					int l = 0;
					int count4 = list.Count;
					while (l < count4)
					{
						this.Animate(list[l], flag, bottom);
						l++;
					}
					this.AnimateScale(this.mBackground, flag, bottom);
				}
			}
			if (flag)
			{
				float num11 = border.y * fitScale;
				vector.y = a.y - border.y * fitScale;
				a.y = vector.y + ((float)this.mBackground.height - border.y * 2f) * fitScale;
				a.x = vector.x + (float)this.mBackground.width * fitScale;
				transform.localPosition = new Vector3(vector.x, a.y - num11, vector.z);
			}
			else
			{
				a.y = vector.y + border.y * fitScale;
				vector.y = a.y - (float)this.mBackground.height * fitScale;
				a.x = vector.x + (float)this.mBackground.width * fitScale;
			}
			UIPanel uipanel2 = this.mPanel;
			for (;;)
			{
				UIRect parent2 = uipanel2.parent;
				if (parent2 == null)
				{
					break;
				}
				UIPanel componentInParent = parent2.GetComponentInParent<UIPanel>();
				if (componentInParent == null)
				{
					break;
				}
				uipanel2 = componentInParent;
			}
			if (cachedTransform != null)
			{
				vector = cachedTransform.TransformPoint(vector);
				a = cachedTransform.TransformPoint(a);
				vector = uipanel2.cachedTransform.InverseTransformPoint(vector);
				a = uipanel2.cachedTransform.InverseTransformPoint(a);
				float pixelSizeAdjustment = UIRoot.GetPixelSizeAdjustment(this.mGameObject);
				vector /= pixelSizeAdjustment;
				a /= pixelSizeAdjustment;
			}
			Vector3 localPosition = transform.localPosition;
			localPosition.x = Mathf.Round(localPosition.x);
			localPosition.y = Mathf.Round(localPosition.y);
			transform.localPosition = localPosition;
			transform.parent = parent;
		}
		else
		{
			this.OnSelect(this.mGameObject, false);
		}
	}

	// Token: 0x04000217 RID: 535
	private const float animSpeed = 0.15f;

	// Token: 0x04000218 RID: 536
	public int minHeight;

	// Token: 0x04000219 RID: 537
	private GameObject mGameObject;

	// Token: 0x0400021A RID: 538
	public static UIPopupList current;

	// Token: 0x0400021B RID: 539
	protected static GameObject mChild;

	// Token: 0x0400021C RID: 540
	protected static float mFadeOutComplete;

	// Token: 0x0400021D RID: 541
	public UnityEngine.Object atlas;

	// Token: 0x0400021E RID: 542
	public UnityEngine.Object bitmapFont;

	// Token: 0x0400021F RID: 543
	public Font trueTypeFont;

	// Token: 0x04000220 RID: 544
	public int fontSize = 16;

	// Token: 0x04000221 RID: 545
	public FontStyle fontStyle;

	// Token: 0x04000222 RID: 546
	public string backgroundSprite;

	// Token: 0x04000223 RID: 547
	public string highlightSprite;

	// Token: 0x04000224 RID: 548
	public Sprite background2DSprite;

	// Token: 0x04000225 RID: 549
	public Sprite highlight2DSprite;

	// Token: 0x04000226 RID: 550
	public UIPopupList.Position position;

	// Token: 0x04000227 RID: 551
	public UIPopupList.Selection selection;

	// Token: 0x04000228 RID: 552
	public NGUIText.Alignment alignment = NGUIText.Alignment.Left;

	// Token: 0x04000229 RID: 553
	public List<string> items = new List<string>();

	// Token: 0x0400022A RID: 554
	public List<object> itemData = new List<object>();

	// Token: 0x0400022B RID: 555
	public List<Action> itemCallbacks = new List<Action>();

	// Token: 0x0400022C RID: 556
	public Vector2 padding = new Vector3(4f, 4f);

	// Token: 0x0400022D RID: 557
	public Color textColor = Color.white;

	// Token: 0x0400022E RID: 558
	public Color backgroundColor = Color.white;

	// Token: 0x0400022F RID: 559
	public Color highlightColor = new Color(0.882352948f, 0.784313738f, 0.5882353f, 1f);

	// Token: 0x04000230 RID: 560
	public bool isAnimated = true;

	// Token: 0x04000231 RID: 561
	public bool isLocalized;

	// Token: 0x04000232 RID: 562
	public UILabel.Modifier textModifier;

	// Token: 0x04000233 RID: 563
	public bool separatePanel = true;

	// Token: 0x04000234 RID: 564
	public int overlap;

	// Token: 0x04000235 RID: 565
	public UIPopupList.OpenOn openOn;

	// Token: 0x04000236 RID: 566
	public List<EventDelegate> onChange = new List<EventDelegate>();

	// Token: 0x04000237 RID: 567
	[HideInInspector]
	[SerializeField]
	protected string mSelectedItem;

	// Token: 0x04000238 RID: 568
	[HideInInspector]
	[SerializeField]
	protected UIPanel mPanel;

	// Token: 0x04000239 RID: 569
	[SerializeField]
	[HideInInspector]
	protected UIBasicSprite mBackground;

	// Token: 0x0400023A RID: 570
	[HideInInspector]
	[SerializeField]
	protected UIBasicSprite mHighlight;

	// Token: 0x0400023B RID: 571
	[HideInInspector]
	[SerializeField]
	protected UILabel mHighlightedLabel;

	// Token: 0x0400023C RID: 572
	[SerializeField]
	[HideInInspector]
	protected List<UILabel> mLabelList = new List<UILabel>();

	// Token: 0x0400023D RID: 573
	[SerializeField]
	[HideInInspector]
	protected float mBgBorder;

	// Token: 0x0400023E RID: 574
	[Tooltip("Whether the selection will be persistent even after the popup list is closed. By default the selection is cleared when the popup is closed so that the same selection can be chosen again the next time the popup list is opened. If enabled, the selection will persist, but selecting the same choice in succession will not result in the onChange notification being triggered more than once.")]
	public bool keepValue;

	// Token: 0x0400023F RID: 575
	[NonSerialized]
	protected GameObject mSelection;

	// Token: 0x04000240 RID: 576
	[NonSerialized]
	protected int mOpenFrame;

	// Token: 0x04000241 RID: 577
	[HideInInspector]
	[SerializeField]
	private GameObject eventReceiver;

	// Token: 0x04000242 RID: 578
	[SerializeField]
	[HideInInspector]
	private string functionName = "OnSelectionChange";

	// Token: 0x04000243 RID: 579
	[HideInInspector]
	[SerializeField]
	private float textScale;

	// Token: 0x04000244 RID: 580
	[SerializeField]
	[HideInInspector]
	private UIFont font;

	// Token: 0x04000245 RID: 581
	[HideInInspector]
	[SerializeField]
	private UILabel textLabel;

	// Token: 0x04000246 RID: 582
	[NonSerialized]
	public Vector3 startingPosition;

	// Token: 0x04000247 RID: 583
	private UIPopupList.LegacyEvent mLegacyEvent;

	// Token: 0x04000248 RID: 584
	[NonSerialized]
	protected bool mExecuting;

	// Token: 0x04000249 RID: 585
	protected bool mUseDynamicFont;

	// Token: 0x0400024A RID: 586
	[NonSerialized]
	protected bool mStarted;

	// Token: 0x0400024B RID: 587
	protected bool mTweening;

	// Token: 0x0400024C RID: 588
	public GameObject source;

	// Token: 0x0200006B RID: 107
	[DoNotObfuscateNGUI]
	public enum Position
	{
		// Token: 0x0400024E RID: 590
		Auto,
		// Token: 0x0400024F RID: 591
		Above,
		// Token: 0x04000250 RID: 592
		Below
	}

	// Token: 0x0200006C RID: 108
	[DoNotObfuscateNGUI]
	public enum Selection
	{
		// Token: 0x04000252 RID: 594
		OnPress,
		// Token: 0x04000253 RID: 595
		OnClick
	}

	// Token: 0x0200006D RID: 109
	[DoNotObfuscateNGUI]
	public enum OpenOn
	{
		// Token: 0x04000255 RID: 597
		ClickOrTap,
		// Token: 0x04000256 RID: 598
		RightClick,
		// Token: 0x04000257 RID: 599
		DoubleClick,
		// Token: 0x04000258 RID: 600
		Manual
	}

	// Token: 0x0200006E RID: 110
	// (Invoke) Token: 0x06000332 RID: 818
	public delegate void LegacyEvent(string val);
}
