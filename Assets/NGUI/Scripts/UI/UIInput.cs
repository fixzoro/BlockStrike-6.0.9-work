#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_WP_8_1 || UNITY_BLACKBERRY)
#define MOBILE
#endif

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Token: 0x02000109 RID: 265
[AddComponentMenu("NGUI/UI/Input Field")]
public class UIInput : MonoBehaviour
{
	// Token: 0x17000155 RID: 341
	// (get) Token: 0x060008E4 RID: 2276 RVA: 0x0000A772 File Offset: 0x00008972
	// (set) Token: 0x060008E5 RID: 2277 RVA: 0x0000A78B File Offset: 0x0000898B
	public string defaultText
	{
		get
		{
			if (this.mDoInit)
			{
				this.Init();
			}
			return this.mDefaultText;
		}
		set
		{
			if (this.mDoInit)
			{
				this.Init();
			}
			this.mDefaultText = value;
			this.UpdateLabel();
		}
	}

	// Token: 0x17000156 RID: 342
	// (get) Token: 0x060008E6 RID: 2278 RVA: 0x0000A7AB File Offset: 0x000089AB
	// (set) Token: 0x060008E7 RID: 2279 RVA: 0x0000A7C4 File Offset: 0x000089C4
	public Color defaultColor
	{
		get
		{
			if (this.mDoInit)
			{
				this.Init();
			}
			return this.mDefaultColor;
		}
		set
		{
			this.mDefaultColor = value;
			if (!this.isSelected)
			{
				this.label.color = value;
			}
		}
	}

	// Token: 0x17000157 RID: 343
	// (get) Token: 0x060008E8 RID: 2280 RVA: 0x0000A7E4 File Offset: 0x000089E4
	public bool inputShouldBeHidden
	{
		get
		{
			return this.hideInput && this.label != null && !this.label.multiLine && this.inputType != UIInput.InputType.Password;
		}
	}

	// Token: 0x17000158 RID: 344
	// (get) Token: 0x060008E9 RID: 2281 RVA: 0x0000A821 File Offset: 0x00008A21
	// (set) Token: 0x060008EA RID: 2282 RVA: 0x0000A829 File Offset: 0x00008A29
	[Obsolete("Use UIInput.value instead")]
	public string text
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

	// Token: 0x17000159 RID: 345
	// (get) Token: 0x060008EB RID: 2283 RVA: 0x0000A832 File Offset: 0x00008A32
	// (set) Token: 0x060008EC RID: 2284 RVA: 0x0000A84B File Offset: 0x00008A4B
	public string value
	{
		get
		{
			if (this.mDoInit)
			{
				this.Init();
			}
			return this.mValue;
		}
		set
		{
			this.Set(value, true);
		}
	}

	// Token: 0x060008ED RID: 2285 RVA: 0x00050C5C File Offset: 0x0004EE5C
	public void Set(string value, bool notify = true)
	{
        #if UNITY_EDITOR
        if(value == "" && UIChat.actived)
        {
            return;
        }
        #endif
        if (this.mDoInit)
		{
			this.Init();
		}
		if (value == this.value)
		{
			return;
		}
		UIInput.mDrawStart = 0;
		value = this.Validate(value);
        if (this.isSelected && UIInput.mKeyboard != null && this.mCached != value)
        {
            UIInput.mKeyboard.text = value;
            this.mCached = value;
        }
        if (this.mValue != value)
		{
			this.mValue = value;
			this.mLoadSavedValue = false;
			if (this.isSelected)
			{
				if (string.IsNullOrEmpty(value))
				{
					this.mSelectionStart = 0;
					this.mSelectionEnd = 0;
				}
				else
				{
					this.mSelectionStart = value.Length;
					this.mSelectionEnd = this.mSelectionStart;
				}
			}
			else if (this.mStarted)
			{
				this.SaveToPlayerPrefs(value);
			}
			this.UpdateLabel();
			if (notify)
			{
				this.ExecuteOnChange();
			}
		}
	}

	// Token: 0x1700015A RID: 346
	// (get) Token: 0x060008EE RID: 2286 RVA: 0x0000A855 File Offset: 0x00008A55
	// (set) Token: 0x060008EF RID: 2287 RVA: 0x0000A85D File Offset: 0x00008A5D
	[Obsolete("Use UIInput.isSelected instead")]
	public bool selected
	{
		get
		{
			return this.isSelected;
		}
		set
		{
			this.isSelected = value;
		}
	}

	// Token: 0x1700015B RID: 347
	// (get) Token: 0x060008F0 RID: 2288 RVA: 0x0000A866 File Offset: 0x00008A66
	// (set) Token: 0x060008F1 RID: 2289 RVA: 0x0000A873 File Offset: 0x00008A73
	public bool isSelected
	{
		get
		{
			return UIInput.selection == this;
		}
		set
		{
			if (!value)
			{
				if (this.isSelected)
				{
					UICamera.selectedObject = null;
				}
			}
			else
			{
				UICamera.selectedObject = base.gameObject;
			}
		}
	}

	// Token: 0x1700015C RID: 348
	// (get) Token: 0x060008F2 RID: 2290 RVA: 0x00050D7C File Offset: 0x0004EF7C
	// (set) Token: 0x060008F3 RID: 2291 RVA: 0x0000A89C File Offset: 0x00008A9C
	public int cursorPosition
	{
		get
		{
			if (UIInput.mKeyboard != null && !this.inputShouldBeHidden)
			{
				return this.value.Length;
			}
			return (!this.isSelected) ? this.value.Length : this.mSelectionEnd;
		}
		set
		{
			if (this.isSelected)
			{
				if (UIInput.mKeyboard != null && !this.inputShouldBeHidden)
				{
					return;
				}
				this.mSelectionEnd = value;
				this.UpdateLabel();
			}
		}
	}

	// Token: 0x1700015D RID: 349
	// (get) Token: 0x060008F4 RID: 2292 RVA: 0x0000A8CC File Offset: 0x00008ACC
	// (set) Token: 0x060008F5 RID: 2293 RVA: 0x0000A906 File Offset: 0x00008B06
	public int selectionStart
	{
		get
		{
			if (UIInput.mKeyboard != null && !this.inputShouldBeHidden)
			{
				return 0;
			}
			return (!this.isSelected) ? this.value.Length : this.mSelectionStart;
		}
		set
		{
			if (this.isSelected)
			{
				if (UIInput.mKeyboard != null && !this.inputShouldBeHidden)
				{
					return;
				}
				this.mSelectionStart = value;
				this.UpdateLabel();
			}
		}
	}

	// Token: 0x1700015E RID: 350
	// (get) Token: 0x060008F6 RID: 2294 RVA: 0x00050D7C File Offset: 0x0004EF7C
	// (set) Token: 0x060008F7 RID: 2295 RVA: 0x0000A89C File Offset: 0x00008A9C
	public int selectionEnd
	{
		get
		{
			if (UIInput.mKeyboard != null && !this.inputShouldBeHidden)
			{
				return this.value.Length;
			}
			return (!this.isSelected) ? this.value.Length : this.mSelectionEnd;
		}
		set
		{
			if (this.isSelected)
			{
				if (UIInput.mKeyboard != null && !this.inputShouldBeHidden)
				{
					return;
				}
				this.mSelectionEnd = value;
				this.UpdateLabel();
			}
		}
	}

	// Token: 0x1700015F RID: 351
	// (get) Token: 0x060008F8 RID: 2296 RVA: 0x0000A936 File Offset: 0x00008B36
	public UITexture caret
	{
		get
		{
			return this.mCaret;
		}
	}

	// Token: 0x060008F9 RID: 2297 RVA: 0x00050DCC File Offset: 0x0004EFCC
	public string Validate(string val)
	{
        if (string.IsNullOrEmpty(val)) return "";

        StringBuilder sb = new StringBuilder(val.Length);

        for (int i = 0; i < val.Length; ++i)
        {
            char c = val[i];
            if (onValidate != null) c = onValidate(sb.ToString(), sb.Length, c);
            else if (validation != Validation.None) c = Validate(sb.ToString(), sb.Length, c);
            if (c != 0) sb.Append(c);
        }

        if (characterLimit > 0 && sb.Length > characterLimit)
            return sb.ToString(0, characterLimit);
        return sb.ToString();
    }

	// Token: 0x060008FA RID: 2298 RVA: 0x00050E9C File Offset: 0x0004F09C
	public void Start()
	{
		this.mGameObject = base.gameObject;
		if (this.mStarted)
		{
			return;
		}
		if (this.selectOnTab != null)
		{
			UIKeyNavigation uikeyNavigation = base.GetComponent<UIKeyNavigation>();
			if (uikeyNavigation == null)
			{
				uikeyNavigation = base.gameObject.AddComponent<UIKeyNavigation>();
				uikeyNavigation.onDown = this.selectOnTab;
			}
			this.selectOnTab = null;
			NGUITools.SetDirty(this, "last change");
		}
		if (this.mLoadSavedValue && !string.IsNullOrEmpty(this.savedAs))
		{
			this.LoadValue();
		}
		else
		{
			this.value = this.mValue.Replace("\\n", "\n");
		}
		this.mStarted = true;
	}

	// Token: 0x060008FB RID: 2299 RVA: 0x00050F58 File Offset: 0x0004F158
	protected void Init()
	{
		if (this.mDoInit && this.label != null)
		{
			this.mDoInit = false;
			this.mDefaultText = this.label.text;
			this.mDefaultColor = this.label.color;
			this.mEllipsis = this.label.overflowEllipsis;
			if (this.label.alignment == NGUIText.Alignment.Justified)
			{
				this.label.alignment = NGUIText.Alignment.Left;
				Debug.LogWarning("Input fields using labels with justified alignment are not supported at this time", this);
			}
			this.mAlignment = this.label.alignment;
			this.mPosition = this.label.cachedTransform.localPosition.x;
			this.UpdateLabel();
		}
	}

	// Token: 0x060008FC RID: 2300 RVA: 0x0000A93E File Offset: 0x00008B3E
	protected void SaveToPlayerPrefs(string val)
	{
		if (!string.IsNullOrEmpty(this.savedAs))
		{
			if (string.IsNullOrEmpty(val))
			{
				PlayerPrefs.DeleteKey(this.savedAs);
			}
			else
			{
				PlayerPrefs.SetString(this.savedAs, val);
			}
		}
	}

	// Token: 0x060008FD RID: 2301 RVA: 0x00051018 File Offset: 0x0004F218
	protected virtual void OnSelect(GameObject go, bool isSelected)
	{
		if (this.mGameObject != go)
		{
			return;
		}
		if (isSelected)
		{
			if (this.label != null)
			{
				this.label.supportEncoding = false;
			}
#if !MOBILE
            if (mOnGUI == null)
                mOnGUI = gameObject.AddComponent<UIInputOnGUI>();
#endif
            this.OnSelectEvent();
		}
		else
		{
			this.OnDeselectEvent();
		}
	}

	// Token: 0x060008FE RID: 2302 RVA: 0x0005106C File Offset: 0x0004F26C
	protected void OnSelectEvent()
	{
		this.mSelectTime = Time.frameCount;
		UIInput.selection = this;
		if (this.mDoInit)
		{
			this.Init();
		}
		if (this.label != null)
		{
			this.mEllipsis = this.label.overflowEllipsis;
			this.label.overflowEllipsis = false;
		}
		if (this.label != null && NGUITools.GetActive(this))
		{
			this.mSelectMe = Time.frameCount;
		}
	}

	// Token: 0x060008FF RID: 2303 RVA: 0x000510F0 File Offset: 0x0004F2F0
	protected void OnDeselectEvent()
	{
		if (this.mDoInit)
		{
			this.Init();
		}
		if (this.label != null)
		{
			this.label.overflowEllipsis = this.mEllipsis;
		}
		if (this.label != null && NGUITools.GetActive(this))
		{
			this.mValue = this.value;
			if (UIInput.mKeyboard != null)
			{
				UIInput.mWaitForKeyboard = false;
				UIInput.mKeyboard.active = false;
				UIInput.mKeyboard = null;
			}
			if (string.IsNullOrEmpty(this.mValue))
			{
				this.label.text = this.mDefaultText;
				this.label.color = this.mDefaultColor;
			}
			else
			{
				this.label.text = this.mValue;
			}
			Input.imeCompositionMode = IMECompositionMode.Auto;
			this.label.alignment = this.mAlignment;
		}
		UIInput.selection = null;
		this.UpdateLabel();
		if (this.submitOnUnselect)
		{
			this.Submit();
		}
	}

	// Token: 0x06000900 RID: 2304 RVA: 0x000511F8 File Offset: 0x0004F3F8
	protected virtual void Update()
	{
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        if (!this.isSelected || this.mSelectTime == Time.frameCount)
		{
			return;
		}
		if (this.mDoInit)
		{
			this.Init();
		}
		if (UIInput.mWaitForKeyboard)
		{
			if (UIInput.mKeyboard != null && !UIInput.mKeyboard.active)
			{
				return;
			}
			UIInput.mWaitForKeyboard = false;
		}
		if (this.mSelectMe != -1 && this.mSelectMe != Time.frameCount)
		{
			this.mSelectMe = -1;
			this.mSelectionEnd = ((!string.IsNullOrEmpty(this.mValue)) ? this.mValue.Length : 0);
			UIInput.mDrawStart = 0;
			this.mSelectionStart = ((!this.selectAllTextOnFocus) ? this.mSelectionEnd : 0);
			this.label.color = this.activeTextColor;
			RuntimePlatform platform = Application.platform;
			if (platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.Android)
			{
				TouchScreenKeyboardType touchScreenKeyboardType;
				string text;
				if (this.inputShouldBeHidden)
				{
					TouchScreenKeyboard.hideInput = true;
					touchScreenKeyboardType = (TouchScreenKeyboardType)this.keyboardType;
					text = "|";
				}
				else if (this.inputType == UIInput.InputType.Password)
				{
					TouchScreenKeyboard.hideInput = false;
					touchScreenKeyboardType = (TouchScreenKeyboardType)this.keyboardType;
					text = this.mValue;
					this.mSelectionStart = this.mSelectionEnd;
				}
				else
				{
					TouchScreenKeyboard.hideInput = false;
					touchScreenKeyboardType = (TouchScreenKeyboardType)this.keyboardType;
					text = this.mValue;
					this.mSelectionStart = this.mSelectionEnd;
				}
				UIInput.mWaitForKeyboard = true;
				UIInput.mKeyboard = ((this.inputType != UIInput.InputType.Password) ? TouchScreenKeyboard.Open(text, touchScreenKeyboardType, !this.inputShouldBeHidden && this.inputType == UIInput.InputType.AutoCorrect, this.label.multiLine && !this.hideInput, false, false, this.defaultText) : TouchScreenKeyboard.Open(text, touchScreenKeyboardType, false, false, true));
			}
			else
			{
				Vector2 compositionCursorPos = (!(UICamera.current != null) || !(UICamera.current.cachedCamera != null)) ? this.label.worldCorners[0] : UICamera.current.cachedCamera.WorldToScreenPoint(this.label.worldCorners[0]);
				compositionCursorPos.y = (float)Screen.height - compositionCursorPos.y;
				Input.imeCompositionMode = IMECompositionMode.On;
				Input.compositionCursorPos = compositionCursorPos;
			}
			this.UpdateLabel();
			if (string.IsNullOrEmpty(Input.inputString))
			{
				return;
			}
		}
		if (UIInput.mKeyboard != null)
		{
			string text2 = (!UIInput.mKeyboard.done && UIInput.mKeyboard.active) ? UIInput.mKeyboard.text : this.mCached;
			if (this.inputShouldBeHidden)
			{
				if (text2 != "|")
				{
					if (!string.IsNullOrEmpty(text2))
					{
						this.Insert(text2.Substring(1));
					}
					else if (!UIInput.mKeyboard.done && UIInput.mKeyboard.active)
					{
						this.DoBackspace();
					}
					UIInput.mKeyboard.text = "|";
				}
			}
			else if (this.mCached != text2)
			{
				this.mCached = text2;
				if (!UIInput.mKeyboard.done && UIInput.mKeyboard.active)
				{
					this.value = text2;
				}
			}
			if (UIInput.mKeyboard.done || !UIInput.mKeyboard.active)
			{
				if (!UIInput.mKeyboard.wasCanceled)
				{
					this.Submit();
				}
				UIInput.mKeyboard = null;
				this.isSelected = false;
				this.mCached = string.Empty;
			}
		}
		else
		{
            string compositionString = Input.compositionString;
			if (string.IsNullOrEmpty(compositionString) && !string.IsNullOrEmpty(Input.inputString))
			{
				foreach (char c in Input.inputString)
				{
					if (c >= ' ')
					{
						if (c != '')
						{
							if (c != '')
							{
								if (c != '')
								{
									if (c != '')
									{
										if (c != '')
										{
											this.Insert(c.ToString());
										}
									}
								}
							}
						}
					}
				}
			}
			if (UIInput.mLastIME != compositionString)
			{
				this.mSelectionEnd = ((!string.IsNullOrEmpty(compositionString)) ? (this.mValue.Length + compositionString.Length) : this.mSelectionStart);
				UIInput.mLastIME = compositionString;
				this.UpdateLabel();
				this.ExecuteOnChange();
			}
		}
		if (this.mCaret != null && this.mNextBlink < RealTime.time)
		{
			this.mNextBlink = RealTime.time + 0.5f;
			this.mCaret.enabled = !this.mCaret.enabled;
		}
		if (this.isSelected && this.mLastAlpha != this.label.finalAlpha)
		{
			this.UpdateLabel();
		}
		if (this.mCam == null)
		{
			this.mCam = UICamera.FindCameraForLayer(base.gameObject.layer);
		}
		if (this.mCam != null)
		{
			bool flag = false;
			if (this.label.multiLine)
			{
				bool flag2 = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
				if (this.onReturnKey == UIInput.OnReturnKey.Submit)
				{
					flag = flag2;
				}
				else
				{
					flag = !flag2;
				}
			}
			if (UICamera.GetKeyDown(this.mCam.submitKey0) || (this.mCam.submitKey0 == KeyCode.Return && UICamera.GetKeyDown(KeyCode.KeypadEnter)))
			{
				if (flag)
				{
					this.Insert("\n");
				}
				else
				{
					if (UICamera.controller.current != null)
					{
						UICamera.controller.clickNotification = UICamera.ClickNotification.None;
					}
					UICamera.currentKey = this.mCam.submitKey0;
					this.Submit();
				}
			}
			if (UICamera.GetKeyDown(this.mCam.submitKey1) || (this.mCam.submitKey1 == KeyCode.Return && UICamera.GetKeyDown(KeyCode.KeypadEnter)))
			{
				if (flag)
				{
					this.Insert("\n");
				}
				else
				{
					if (UICamera.controller.current != null)
					{
						UICamera.controller.clickNotification = UICamera.ClickNotification.None;
					}
					UICamera.currentKey = this.mCam.submitKey1;
					this.Submit();
				}
			}
			if (!this.mCam.useKeyboard && UICamera.GetKeyUp(KeyCode.Tab))
			{
				this.OnKey(this.mGameObject, KeyCode.Tab);
			}
		}
	}

	// Token: 0x06000901 RID: 2305 RVA: 0x00051908 File Offset: 0x0004FB08
	private void OnKey(GameObject go, KeyCode key)
	{
		if (this.mGameObject != go)
		{
			return;
		}
		int frameCount = Time.frameCount;
		if (UIInput.mIgnoreKey == frameCount)
		{
			return;
		}
		if (this.mCam != null && (key == this.mCam.cancelKey0 || key == this.mCam.cancelKey1))
		{
			UIInput.mIgnoreKey = frameCount;
			this.isSelected = false;
		}
		else if (key == KeyCode.Tab)
		{
			UIInput.mIgnoreKey = frameCount;
			this.isSelected = false;
			UIKeyNavigation component = base.GetComponent<UIKeyNavigation>();
			if (component != null)
			{
				component.OnKey(KeyCode.Tab);
			}
		}
	}

	// Token: 0x06000902 RID: 2306 RVA: 0x000519B0 File Offset: 0x0004FBB0
	protected void DoBackspace()
	{
		if (!string.IsNullOrEmpty(this.mValue))
		{
			if (this.mSelectionStart == this.mSelectionEnd)
			{
				if (this.mSelectionStart < 1)
				{
					return;
				}
				this.mSelectionEnd--;
			}
			this.Insert(string.Empty);
		}
	}

#if !MOBILE
    /// <summary>
    /// Handle the specified event.
    /// </summary>

    public virtual bool ProcessEvent(Event ev)
    {
        if (label == null) return false;

        RuntimePlatform rp = Application.platform;

        bool isMac = (
            rp == RuntimePlatform.OSXEditor ||
            rp == RuntimePlatform.OSXPlayer);

        bool ctrl = isMac ?
            ((ev.modifiers & EventModifiers.Command) != 0) :
            ((ev.modifiers & EventModifiers.Control) != 0);

        // http://www.tasharen.com/forum/index.php?topic=10780.0
        if ((ev.modifiers & EventModifiers.Alt) != 0) ctrl = false;

        bool shift = ((ev.modifiers & EventModifiers.Shift) != 0);

        switch (ev.keyCode)
        {
            case KeyCode.Backspace:
                {
                    ev.Use();
                    DoBackspace();
                    return true;
                }

            case KeyCode.Delete:
                {
                    ev.Use();

                    if (!string.IsNullOrEmpty(mValue))
                    {
                        if (mSelectionStart == mSelectionEnd)
                        {
                            if (mSelectionStart >= mValue.Length) return true;
                            ++mSelectionEnd;
                        }
                        Insert("");
                    }
                    return true;
                }

            case KeyCode.LeftArrow:
                {
                    ev.Use();

                    if (!string.IsNullOrEmpty(mValue))
                    {
                        mSelectionEnd = Mathf.Max(mSelectionEnd - 1, 0);
                        if (!shift) mSelectionStart = mSelectionEnd;
                        UpdateLabel();
                    }
                    return true;
                }

            case KeyCode.RightArrow:
                {
                    ev.Use();

                    if (!string.IsNullOrEmpty(mValue))
                    {
                        mSelectionEnd = Mathf.Min(mSelectionEnd + 1, mValue.Length);
                        if (!shift) mSelectionStart = mSelectionEnd;
                        UpdateLabel();
                    }
                    return true;
                }

            case KeyCode.PageUp:
                {
                    ev.Use();

                    if (!string.IsNullOrEmpty(mValue))
                    {
                        mSelectionEnd = 0;
                        if (!shift) mSelectionStart = mSelectionEnd;
                        UpdateLabel();
                    }
                    return true;
                }

            case KeyCode.PageDown:
                {
                    ev.Use();

                    if (!string.IsNullOrEmpty(mValue))
                    {
                        mSelectionEnd = mValue.Length;
                        if (!shift) mSelectionStart = mSelectionEnd;
                        UpdateLabel();
                    }
                    return true;
                }

            case KeyCode.Home:
                {
                    ev.Use();

                    if (!string.IsNullOrEmpty(mValue))
                    {
                        if (label.multiLine)
                        {
                            mSelectionEnd = label.GetCharacterIndex(mSelectionEnd, KeyCode.Home);
                        }
                        else mSelectionEnd = 0;

                        if (!shift) mSelectionStart = mSelectionEnd;
                        UpdateLabel();
                    }
                    return true;
                }

            case KeyCode.End:
                {
                    ev.Use();

                    if (!string.IsNullOrEmpty(mValue))
                    {
                        if (label.multiLine)
                        {
                            mSelectionEnd = label.GetCharacterIndex(mSelectionEnd, KeyCode.End);
                        }
                        else mSelectionEnd = mValue.Length;

                        if (!shift) mSelectionStart = mSelectionEnd;
                        UpdateLabel();
                    }
                    return true;
                }

            case KeyCode.UpArrow:
                {
                    ev.Use();

                    if (!string.IsNullOrEmpty(mValue))
                    {
                        mSelectionEnd = label.GetCharacterIndex(mSelectionEnd, KeyCode.UpArrow);
                        if (mSelectionEnd != 0) mSelectionEnd += mDrawStart;
                        if (!shift) mSelectionStart = mSelectionEnd;
                        UpdateLabel();
                    }
                    return true;
                }

            case KeyCode.DownArrow:
                {
                    ev.Use();

                    if (!string.IsNullOrEmpty(mValue))
                    {
                        mSelectionEnd = label.GetCharacterIndex(mSelectionEnd, KeyCode.DownArrow);
                        if (mSelectionEnd != label.processedText.Length) mSelectionEnd += mDrawStart;
                        else mSelectionEnd = mValue.Length;
                        if (!shift) mSelectionStart = mSelectionEnd;
                        UpdateLabel();
                    }
                    return true;
                }

            // Select all
            case KeyCode.A:
                {
                    if (ctrl)
                    {
                        ev.Use();
                        mSelectionStart = 0;
                        mSelectionEnd = mValue.Length;
                        UpdateLabel();
                    }
                    return true;
                }

            // Copy
            case KeyCode.C:
                {
                    if (ctrl)
                    {
                        ev.Use();
                        NGUITools.clipboard = GetSelection();
                    }
                    return true;
                }

            // Paste
            case KeyCode.V:
                {
                    if (ctrl)
                    {
                        ev.Use();
                        Insert(NGUITools.clipboard);
                    }
                    return true;
                }

            // Cut
            case KeyCode.X:
                {
                    if (ctrl)
                    {
                        ev.Use();
                        NGUITools.clipboard = GetSelection();
                        Insert("");
                    }
                    return true;
                }
        }
        return false;
    }
#endif

    // Token: 0x06000903 RID: 2307 RVA: 0x00051A04 File Offset: 0x0004FC04
    protected virtual void Insert(string text)
	{
        string left = GetLeftText();
        string right = GetRightText();
        int rl = right.Length;

        StringBuilder sb = new StringBuilder(left.Length + right.Length + text.Length);
        sb.Append(left);

        // Append the new text
        for (int i = 0, imax = text.Length; i < imax; ++i)
        {
            // If we have an input validator, validate the input first
            char c = text[i];

            if (c == '\b')
            {
                DoBackspace();
                continue;
            }

            // Can't go past the character limit
            if (characterLimit > 0 && sb.Length + rl >= characterLimit) break;

            if (onValidate != null) c = onValidate(sb.ToString(), sb.Length, c);
            else if (validation != Validation.None) c = Validate(sb.ToString(), sb.Length, c);

            // Append the character if it hasn't been invalidated
            if (c != 0) sb.Append(c);
        }

        // Advance the selection
        mSelectionStart = sb.Length;
        mSelectionEnd = mSelectionStart;

        // Append the text that follows it, ensuring that it's also validated after the inserted value
        for (int i = 0, imax = right.Length; i < imax; ++i)
        {
            char c = right[i];
            if (onValidate != null) c = onValidate(sb.ToString(), sb.Length, c);
            else if (validation != Validation.None) c = Validate(sb.ToString(), sb.Length, c);
            if (c != 0) sb.Append(c);
        }

        mValue = sb.ToString();
        UpdateLabel();
        ExecuteOnChange();
    }

	// Token: 0x06000904 RID: 2308 RVA: 0x00051BBC File Offset: 0x0004FDBC
	protected string GetLeftText()
	{
        int min = Mathf.Min(mSelectionStart, mSelectionEnd);
        return (string.IsNullOrEmpty(mValue) || min < 0) ? "" : mValue.Substring(0, min);
    }

	// Token: 0x06000905 RID: 2309 RVA: 0x00051C24 File Offset: 0x0004FE24
	protected string GetRightText()
	{
        int max = Mathf.Max(mSelectionStart, mSelectionEnd);
        return (string.IsNullOrEmpty(mValue) || max >= mValue.Length) ? "" : mValue.Substring(max);
    }

	// Token: 0x06000906 RID: 2310 RVA: 0x00051C7C File Offset: 0x0004FE7C
	protected string GetSelection()
	{
        if (string.IsNullOrEmpty(mValue) || mSelectionStart == mSelectionEnd)
        {
            return "";
        }
        else
        {
            int min = Mathf.Min(mSelectionStart, mSelectionEnd);
            int max = Mathf.Max(mSelectionStart, mSelectionEnd);
            return mValue.Substring(min, max - min);
        }
    }

	// Token: 0x06000907 RID: 2311 RVA: 0x00051CE4 File Offset: 0x0004FEE4
	protected int GetCharUnderMouse()
	{
        Vector3[] corners = label.worldCorners;
        Ray ray = UICamera.currentRay;
        Plane p = new Plane(corners[0], corners[1], corners[2]);
        float dist;
        return p.Raycast(ray, out dist) ? mDrawStart + label.GetCharacterIndexAtPosition(ray.GetPoint(dist), false) : 0;
    }

	// Token: 0x06000908 RID: 2312 RVA: 0x00051D60 File Offset: 0x0004FF60
	protected virtual void OnPress(GameObject go, bool isPressed)
	{
        if (isPressed && isSelected && label != null &&
            (UICamera.currentScheme == UICamera.ControlScheme.Mouse ||
             UICamera.currentScheme == UICamera.ControlScheme.Touch))
        {
#if !UNITY_EDITOR && (UNITY_WP8 || UNITY_WP_8_1)
			if (mKeyboard != null) mKeyboard.active = true;
#endif
            selectionEnd = GetCharUnderMouse();
            if (!Input.GetKey(KeyCode.LeftShift) &&
                !Input.GetKey(KeyCode.RightShift)) selectionStart = mSelectionEnd;
        }
    }

	// Token: 0x06000909 RID: 2313 RVA: 0x00051DEC File Offset: 0x0004FFEC
	protected virtual void OnDrag(GameObject go, Vector2 delta)
	{
		if (this.mGameObject != go)
		{
			return;
		}
        if (label != null &&
            (UICamera.currentScheme == UICamera.ControlScheme.Mouse ||
             UICamera.currentScheme == UICamera.ControlScheme.Touch))
        {
            selectionEnd = GetCharUnderMouse();
        }
    }

	// Token: 0x0600090A RID: 2314 RVA: 0x00051E40 File Offset: 0x00050040
	private void OnEnable()
	{
		UICamera.onDrag = (UICamera.VectorDelegate)Delegate.Combine(UICamera.onDrag, new UICamera.VectorDelegate(this.OnDrag));
		UICamera.onSelect = (UICamera.BoolDelegate)Delegate.Combine(UICamera.onSelect, new UICamera.BoolDelegate(this.OnSelect));
		UICamera.onPress = (UICamera.BoolDelegate)Delegate.Combine(UICamera.onPress, new UICamera.BoolDelegate(this.OnPress));
		UICamera.onKey = (UICamera.KeyCodeDelegate)Delegate.Combine(UICamera.onKey, new UICamera.KeyCodeDelegate(this.OnKey));
	}

	// Token: 0x0600090B RID: 2315 RVA: 0x00051ED0 File Offset: 0x000500D0
	private void OnDisable()
	{
		this.Cleanup();
		UICamera.onDrag = (UICamera.VectorDelegate)Delegate.Remove(UICamera.onDrag, new UICamera.VectorDelegate(this.OnDrag));
		UICamera.onSelect = (UICamera.BoolDelegate)Delegate.Remove(UICamera.onSelect, new UICamera.BoolDelegate(this.OnSelect));
		UICamera.onPress = (UICamera.BoolDelegate)Delegate.Remove(UICamera.onPress, new UICamera.BoolDelegate(this.OnPress));
		UICamera.onKey = (UICamera.KeyCodeDelegate)Delegate.Remove(UICamera.onKey, new UICamera.KeyCodeDelegate(this.OnKey));
	}

	// Token: 0x0600090C RID: 2316 RVA: 0x00051F68 File Offset: 0x00050168
	protected virtual void Cleanup()
	{
		if (this.mHighlight)
		{
			this.mHighlight.enabled = false;
		}
		if (this.mCaret)
		{
			this.mCaret.enabled = false;
		}
		if (this.mBlankTex)
		{
			NGUITools.Destroy(this.mBlankTex);
			this.mBlankTex = null;
		}
	}

	// Token: 0x0600090D RID: 2317 RVA: 0x00051FD0 File Offset: 0x000501D0
	public void Submit()
	{
		if (NGUITools.GetActive(this))
		{
			this.mValue = this.value;
			if (UIInput.current == null)
			{
				UIInput.current = this;
				EventDelegate.Execute(this.onSubmit);
				UIInput.current = null;
			}
			this.SaveToPlayerPrefs(this.mValue);
		}
	}

    // Token: 0x0600090E RID: 2318 RVA: 0x00052028 File Offset: 0x00050228
    public void UpdateLabel()
	{
        if (!((object)label != null))
        {
            return;
        }
        if (mDoInit)
        {
            Init();
        }
        bool isSelected = this.isSelected;
        string value = this.value;
        bool flag = string.IsNullOrEmpty(value) && string.IsNullOrEmpty(Input.compositionString);
        label.color = ((!flag || isSelected) ? activeTextColor : mDefaultColor);
        string text;
        if (flag)
        {
            text = ((!isSelected) ? mDefaultText : string.Empty);
            label.alignment = mAlignment;
        }
        else
        {
            if (inputType == InputType.Password)
            {
                text = string.Empty;
                string str = "*";
                INGUIFont bitmapFont = label.bitmapFont;
                if (bitmapFont != null && bitmapFont.bmFont != null && bitmapFont.bmFont.GetGlyph(42) == null)
                {
                    str = "x";
                }
                int i = 0;
                for (int length = value.Length; i < length; i++)
                {
                    text += str;
                }
            }
            else
            {
                text = value;
            }
            int num = (isSelected ? Mathf.Min(text.Length, cursorPosition) : 0);
            string str2 = text.Substring(0, num);
            if (isSelected)
            {
                str2 += Input.compositionString;
            }
            text = str2 + text.Substring(num, text.Length - num);
            if (isSelected && label.overflowMethod == UILabel.Overflow.ClampContent && label.maxLineCount == 1)
            {
                int num2 = label.CalculateOffsetToFit(text);
                if (num2 == 0)
                {
                    mDrawStart = 0;
                    label.alignment = mAlignment;
                }
                else if (num < mDrawStart)
                {
                    mDrawStart = num;
                    label.alignment = NGUIText.Alignment.Left;
                }
                else if (num2 < mDrawStart)
                {
                    mDrawStart = num2;
                    label.alignment = NGUIText.Alignment.Left;
                }
                else
                {
                    num2 = label.CalculateOffsetToFit(text.Substring(0, num));
                    if (num2 > mDrawStart)
                    {
                        mDrawStart = num2;
                        label.alignment = NGUIText.Alignment.Right;
                    }
                }
                if (mDrawStart != 0)
                {
                    text = text.Substring(mDrawStart, text.Length - mDrawStart);
                }
            }
            else
            {
                mDrawStart = 0;
                label.alignment = mAlignment;
            }
        }
        label.text = text;
        if (isSelected && (mKeyboard == null || inputShouldBeHidden))
        {
            int num3 = mSelectionStart - mDrawStart;
            int num4 = mSelectionEnd - mDrawStart;
            if ((object)mBlankTex == null)
            {
                mBlankTex = new Texture2D(2, 2, (TextureFormat)5, false);
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        mBlankTex.SetPixel(k, j, Color.white);
                    }
                }
                mBlankTex.Apply();
            }
            if (num3 != num4)
            {
                if ((object)mHighlight == null)
                {
                    mHighlight = label.cachedGameObject.AddWidget<UITexture>();
                    (mHighlight).name = "Input Highlight";
                    mHighlight.mainTexture = (Texture)(object)mBlankTex;
                    mHighlight.fillGeometry = false;
                    mHighlight.pivot = label.pivot;
                    mHighlight.SetAnchor(label.cachedTransform);
                }
                else
                {
                    mHighlight.pivot = label.pivot;
                    mHighlight.mainTexture = (Texture)(object)mBlankTex;
                    mHighlight.MarkAsChanged();
                    ((Behaviour)mHighlight).enabled = true;
                }
            }
            if ((object)mCaret == null)
            {
                mCaret = label.cachedGameObject.AddWidget<UITexture>();
                (mCaret).name = "Input Caret";
                mCaret.mainTexture = (Texture)(object)mBlankTex;
                mCaret.fillGeometry = false;
                mCaret.pivot = label.pivot;
                mCaret.SetAnchor(label.cachedTransform);
            }
            else
            {
                mCaret.pivot = label.pivot;
                mCaret.mainTexture = (Texture)(object)mBlankTex;
                mCaret.MarkAsChanged();
                ((Behaviour)mCaret).enabled = true;
            }
            if (num3 != num4)
            {
                label.PrintOverlay(num3, num4, mCaret.geometry, mHighlight.geometry, caretColor, selectionColor);
                ((Behaviour)mHighlight).enabled = mHighlight.geometry.hasVertices;
            }
            else
            {
                label.PrintOverlay(num3, num4, mCaret.geometry, null, caretColor, selectionColor);
                if ((object)mHighlight != null)
                {
                    ((Behaviour)mHighlight).enabled = false;
                }
            }
            mNextBlink = RealTime.time + 0.5f;
            mLastAlpha = label.finalAlpha;
        }
        else
        {
            Cleanup();
        }
    }

	// Token: 0x0600090F RID: 2319 RVA: 0x000525D8 File Offset: 0x000507D8
	protected char Validate(string text, int pos, char ch)
	{
        // Validation is disabled
        if (validation == Validation.None || !enabled) return ch;

        if (validation == Validation.Integer)
        {
            // Integer number validation
            if (ch >= '0' && ch <= '9') return ch;
            if (ch == '-' && pos == 0 && !text.Contains("-")) return ch;
        }
        else if (validation == Validation.Float)
        {
            // Floating-point number
            if (ch >= '0' && ch <= '9') return ch;
            if (ch == '-' && pos == 0 && !text.Contains("-")) return ch;
            if (ch == '.' && !text.Contains(".")) return ch;
        }
        else if (validation == Validation.Alphanumeric)
        {
            // All alphanumeric characters
            if (ch >= 'A' && ch <= 'Z') return ch;
            if (ch >= 'a' && ch <= 'z') return ch;
            if (ch >= '0' && ch <= '9') return ch;
        }
        else if (validation == Validation.Username)
        {
            // Lowercase and numbers
            if (ch >= 'A' && ch <= 'Z') return (char)(ch - 'A' + 'a');
            if (ch >= 'a' && ch <= 'z') return ch;
            if (ch >= '0' && ch <= '9') return ch;
        }
        else if (validation == Validation.Filename)
        {
            if (ch == ':') return (char)0;
            if (ch == '/') return (char)0;
            if (ch == '\\') return (char)0;
            if (ch == '<') return (char)0;
            if (ch == '>') return (char)0;
            if (ch == '|') return (char)0;
            if (ch == '^') return (char)0;
            if (ch == '*') return (char)0;
            if (ch == ';') return (char)0;
            if (ch == '"') return (char)0;
            if (ch == '`') return (char)0;
            if (ch == '\t') return (char)0;
            if (ch == '\n') return (char)0;
            return ch;
        }
        else if (validation == Validation.Name)
        {
            char lastChar = (text.Length > 0) ? text[Mathf.Clamp(pos, 0, text.Length - 1)] : ' ';
            char nextChar = (text.Length > 0) ? text[Mathf.Clamp(pos + 1, 0, text.Length - 1)] : '\n';

            if (ch >= 'a' && ch <= 'z')
            {
                // Space followed by a letter -- make sure it's capitalized
                if (lastChar == ' ') return (char)(ch - 'a' + 'A');
                return ch;
            }
            else if (ch >= 'A' && ch <= 'Z')
            {
                // Uppercase letters are only allowed after spaces (and apostrophes)
                if (lastChar != ' ' && lastChar != '\'') return (char)(ch - 'A' + 'a');
                return ch;
            }
            else if (ch == '\'')
            {
                // Don't allow more than one apostrophe
                if (lastChar != ' ' && lastChar != '\'' && nextChar != '\'' && !text.Contains("'")) return ch;
            }
            else if (ch == ' ')
            {
                // Don't allow more than one space in a row
                if (lastChar != ' ' && lastChar != '\'' && nextChar != ' ' && nextChar != '\'') return ch;
            }
        }
        return (char)0;
    }

	// Token: 0x06000910 RID: 2320 RVA: 0x0000A977 File Offset: 0x00008B77
	protected void ExecuteOnChange()
	{
		if (UIInput.current == null && EventDelegate.IsValid(this.onChange))
		{
			UIInput.current = this;
			EventDelegate.Execute(this.onChange);
			UIInput.current = null;
		}
	}

	// Token: 0x06000911 RID: 2321 RVA: 0x0000A9B0 File Offset: 0x00008BB0
	public void RemoveFocus()
	{
		this.isSelected = false;
	}

	// Token: 0x06000912 RID: 2322 RVA: 0x0000A9B9 File Offset: 0x00008BB9
	public void SaveValue()
	{
		this.SaveToPlayerPrefs(this.mValue);
	}

	// Token: 0x06000913 RID: 2323 RVA: 0x000528D4 File Offset: 0x00050AD4
	public void LoadValue()
	{
		if (!string.IsNullOrEmpty(this.savedAs))
		{
			string text = this.mValue.Replace("\\n", "\n");
			this.mValue = string.Empty;
			this.value = ((!PlayerPrefs.HasKey(this.savedAs)) ? text : PlayerPrefs.GetString(this.savedAs));
		}
	}

	// Token: 0x0400061D RID: 1565
	private GameObject mGameObject;

	// Token: 0x0400061E RID: 1566
	public static UIInput current;

	// Token: 0x0400061F RID: 1567
	public static UIInput selection;

	// Token: 0x04000620 RID: 1568
	public UILabel label;

	// Token: 0x04000621 RID: 1569
	public UIInput.InputType inputType;

	// Token: 0x04000622 RID: 1570
	public UIInput.OnReturnKey onReturnKey;

	// Token: 0x04000623 RID: 1571
	public UIInput.KeyboardType keyboardType;

	// Token: 0x04000624 RID: 1572
	public bool hideInput;

	// Token: 0x04000625 RID: 1573
	[NonSerialized]
	public bool selectAllTextOnFocus = true;

	// Token: 0x04000626 RID: 1574
	public bool submitOnUnselect;

	// Token: 0x04000627 RID: 1575
	public UIInput.Validation validation;

	// Token: 0x04000628 RID: 1576
	public int characterLimit;

	// Token: 0x04000629 RID: 1577
	public string savedAs;

	// Token: 0x0400062A RID: 1578
	[HideInInspector]
	[SerializeField]
	private GameObject selectOnTab;

	// Token: 0x0400062B RID: 1579
	public Color activeTextColor = Color.white;

	// Token: 0x0400062C RID: 1580
	public Color caretColor = new Color(1f, 1f, 1f, 0.8f);

	// Token: 0x0400062D RID: 1581
	public Color selectionColor = new Color(1f, 0.8745098f, 0.5529412f, 0.5f);

	// Token: 0x0400062E RID: 1582
	public List<EventDelegate> onSubmit = new List<EventDelegate>();

	// Token: 0x0400062F RID: 1583
	public List<EventDelegate> onChange = new List<EventDelegate>();

	// Token: 0x04000630 RID: 1584
	public UIInput.OnValidate onValidate;

	// Token: 0x04000631 RID: 1585
	[SerializeField]
	[HideInInspector]
	protected string mValue;

#if !MOBILE
    [System.NonSerialized] UIInputOnGUI mOnGUI;
#endif

    // Token: 0x04000632 RID: 1586
    [NonSerialized]
	protected string mDefaultText = string.Empty;

	// Token: 0x04000633 RID: 1587
	[NonSerialized]
	protected Color mDefaultColor = Color.white;

	// Token: 0x04000634 RID: 1588
	[NonSerialized]
	protected float mPosition;

	// Token: 0x04000635 RID: 1589
	[NonSerialized]
	protected bool mDoInit = true;

	// Token: 0x04000636 RID: 1590
	[NonSerialized]
	protected NGUIText.Alignment mAlignment = NGUIText.Alignment.Left;

	// Token: 0x04000637 RID: 1591
	[NonSerialized]
	protected bool mLoadSavedValue = true;

	// Token: 0x04000638 RID: 1592
	protected static int mDrawStart;

	// Token: 0x04000639 RID: 1593
	protected static string mLastIME = string.Empty;

	// Token: 0x0400063A RID: 1594
	protected static TouchScreenKeyboard mKeyboard;

	// Token: 0x0400063B RID: 1595
	private static bool mWaitForKeyboard;

	// Token: 0x0400063C RID: 1596
	[NonSerialized]
	protected int mSelectionStart;

	// Token: 0x0400063D RID: 1597
	[NonSerialized]
	protected int mSelectionEnd;

	// Token: 0x0400063E RID: 1598
	[NonSerialized]
	protected UITexture mHighlight;

	// Token: 0x0400063F RID: 1599
	[NonSerialized]
	protected UITexture mCaret;

	// Token: 0x04000640 RID: 1600
	[NonSerialized]
	protected Texture2D mBlankTex;

	// Token: 0x04000641 RID: 1601
	[NonSerialized]
	protected float mNextBlink;

	// Token: 0x04000642 RID: 1602
	[NonSerialized]
	protected float mLastAlpha;

	// Token: 0x04000643 RID: 1603
	[NonSerialized]
	protected string mCached = string.Empty;

	// Token: 0x04000644 RID: 1604
	[NonSerialized]
	protected int mSelectMe = -1;

	// Token: 0x04000645 RID: 1605
	[NonSerialized]
	protected int mSelectTime = -1;

	// Token: 0x04000646 RID: 1606
	[NonSerialized]
	protected bool mStarted;

	// Token: 0x04000647 RID: 1607
	[NonSerialized]
	private UICamera mCam;

	// Token: 0x04000648 RID: 1608
	[NonSerialized]
	private bool mEllipsis;

	// Token: 0x04000649 RID: 1609
	private static int mIgnoreKey;

	// Token: 0x0400064A RID: 1610
	[NonSerialized]
	public Action onUpArrow;

	// Token: 0x0400064B RID: 1611
	[NonSerialized]
	public Action onDownArrow;

	// Token: 0x0200010A RID: 266
	[DoNotObfuscateNGUI]
	public enum InputType
	{
		// Token: 0x0400064D RID: 1613
		Standard,
		// Token: 0x0400064E RID: 1614
		AutoCorrect,
		// Token: 0x0400064F RID: 1615
		Password
	}

	// Token: 0x0200010B RID: 267
	[DoNotObfuscateNGUI]
	public enum Validation
	{
		// Token: 0x04000651 RID: 1617
		None,
		// Token: 0x04000652 RID: 1618
		Integer,
		// Token: 0x04000653 RID: 1619
		Float,
		// Token: 0x04000654 RID: 1620
		Alphanumeric,
		// Token: 0x04000655 RID: 1621
		Username,
		// Token: 0x04000656 RID: 1622
		Name,
		// Token: 0x04000657 RID: 1623
		Filename
	}

	// Token: 0x0200010C RID: 268
	[DoNotObfuscateNGUI]
	public enum KeyboardType
	{
		// Token: 0x04000659 RID: 1625
		Default,
		// Token: 0x0400065A RID: 1626
		ASCIICapable,
		// Token: 0x0400065B RID: 1627
		NumbersAndPunctuation,
		// Token: 0x0400065C RID: 1628
		URL,
		// Token: 0x0400065D RID: 1629
		NumberPad,
		// Token: 0x0400065E RID: 1630
		PhonePad,
		// Token: 0x0400065F RID: 1631
		NamePhonePad,
		// Token: 0x04000660 RID: 1632
		EmailAddress
	}

	// Token: 0x0200010D RID: 269
	[DoNotObfuscateNGUI]
	public enum OnReturnKey
	{
		// Token: 0x04000662 RID: 1634
		Default,
		// Token: 0x04000663 RID: 1635
		Submit,
		// Token: 0x04000664 RID: 1636
		NewLine
	}

	// Token: 0x0200010E RID: 270
	// (Invoke) Token: 0x06000915 RID: 2325
	public delegate char OnValidate(string text, int charIndex, char addedChar);
}
