using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000061 RID: 97
[AddComponentMenu("NGUI/Interaction/Key Binding")]
public class UIKeyBinding : MonoBehaviour
{
	// Token: 0x1700001B RID: 27
	// (get) Token: 0x060002B0 RID: 688 RVA: 0x00028C54 File Offset: 0x00026E54
	public string captionText
	{
		get
		{
			string text = NGUITools.KeyToCaption(this.keyCode);
			if (this.modifier == UIKeyBinding.Modifier.None || this.modifier == UIKeyBinding.Modifier.Any)
			{
				return text;
			}
			return this.modifier + "+" + text;
		}
	}

	// Token: 0x060002B1 RID: 689 RVA: 0x00028C9C File Offset: 0x00026E9C
	public static bool IsBound(KeyCode key)
	{
		int i = 0;
		int count = UIKeyBinding.mList.Count;
		while (i < count)
		{
			UIKeyBinding uikeyBinding = UIKeyBinding.mList[i];
			if (uikeyBinding != null && uikeyBinding.keyCode == key)
			{
				return true;
			}
			i++;
		}
		return false;
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x00006C6E File Offset: 0x00004E6E
	protected virtual void OnEnable()
	{
		UIKeyBinding.mList.Add(this);
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x00006C7B File Offset: 0x00004E7B
	protected virtual void OnDisable()
	{
		UIKeyBinding.mList.Remove(this);
	}

	// Token: 0x060002B4 RID: 692 RVA: 0x00028CF0 File Offset: 0x00026EF0
	protected virtual void Start()
	{
		UIInput component = base.GetComponent<UIInput>();
		this.mIsInput = (component != null);
		if (component != null)
		{
			EventDelegate.Add(component.onSubmit, new EventDelegate.Callback(this.OnSubmit));
		}
	}

	// Token: 0x060002B5 RID: 693 RVA: 0x00006C89 File Offset: 0x00004E89
	protected virtual void OnSubmit()
	{
		//if (UICamera.currentKey == this.keyCode && this.IsModifierActive())
		//{
		//	this.mIgnoreUp = true;
		//}
	}

	// Token: 0x060002B6 RID: 694 RVA: 0x00006CAD File Offset: 0x00004EAD
	protected virtual bool IsModifierActive()
	{
		return UIKeyBinding.IsModifierActive(this.modifier);
	}

	// Token: 0x060002B7 RID: 695 RVA: 0x00028D38 File Offset: 0x00026F38
	public static bool IsModifierActive(UIKeyBinding.Modifier modifier)
	{
		if (modifier == UIKeyBinding.Modifier.Any)
		{
			return true;
		}
		if (modifier == UIKeyBinding.Modifier.Alt)
		{
			if (UICamera.GetKey(KeyCode.LeftAlt) || UICamera.GetKey(KeyCode.RightAlt))
			{
				return true;
			}
		}
		else if (modifier == UIKeyBinding.Modifier.Ctrl)
		{
			if (UICamera.GetKey(KeyCode.LeftControl) || UICamera.GetKey(KeyCode.RightControl))
			{
				return true;
			}
		}
		else if (modifier == UIKeyBinding.Modifier.Shift)
		{
			if (UICamera.GetKey(KeyCode.LeftShift) || UICamera.GetKey(KeyCode.RightShift))
			{
				return true;
			}
		}
		else if (modifier == UIKeyBinding.Modifier.None)
		{
			return !UICamera.GetKey(KeyCode.LeftAlt) && !UICamera.GetKey(KeyCode.RightAlt) && !UICamera.GetKey(KeyCode.LeftControl) && !UICamera.GetKey(KeyCode.RightControl) && !UICamera.GetKey(KeyCode.LeftShift) && !UICamera.GetKey(KeyCode.RightShift);
		}
		return false;
	}

	// Token: 0x060002B8 RID: 696 RVA: 0x00028E74 File Offset: 0x00027074
	protected virtual void Update()
	{
		if (this.keyCode != KeyCode.Numlock && UICamera.inputHasFocus)
		{
			return;
		}
		if (this.keyCode == KeyCode.None || !this.IsModifierActive())
		{
			return;
		}
		bool flag = UICamera.GetKeyDown(this.keyCode);
		bool flag2 = UICamera.GetKeyUp(this.keyCode);
		if (flag)
		{
			this.mPress = true;
		}
		if (this.action == UIKeyBinding.Action.PressAndClick || this.action == UIKeyBinding.Action.All)
		{
			if (flag)
			{
				UICamera.currentTouchID = -1;
				UICamera.currentKey = this.keyCode;
				this.OnBindingPress(true);
			}
			if (this.mPress && flag2)
			{
				UICamera.currentTouchID = -1;
				UICamera.currentKey = this.keyCode;
				this.OnBindingPress(false);
				this.OnBindingClick();
			}
		}
		if ((this.action == UIKeyBinding.Action.Select || this.action == UIKeyBinding.Action.All) && flag2)
		{
			if (this.mIsInput)
			{
				if (!this.mIgnoreUp && (this.keyCode == KeyCode.Numlock || !UICamera.inputHasFocus) && this.mPress)
				{
					UICamera.selectedObject = base.gameObject;
				}
				this.mIgnoreUp = false;
			}
			else if (this.mPress)
			{
				UICamera.hoveredObject = base.gameObject;
			}
		}
		if (flag2)
		{
			this.mPress = false;
		}
	}

	// Token: 0x060002B9 RID: 697 RVA: 0x00006CBA File Offset: 0x00004EBA
	protected virtual void OnBindingPress(bool pressed)
	{
		UICamera.Notify(base.gameObject, "OnPress", pressed);
	}

	// Token: 0x060002BA RID: 698 RVA: 0x00006CD2 File Offset: 0x00004ED2
	protected virtual void OnBindingClick()
	{
		UICamera.Notify(base.gameObject, "OnClick", null);
	}

	// Token: 0x060002BB RID: 699 RVA: 0x00006CE5 File Offset: 0x00004EE5
	public override string ToString()
	{
		return UIKeyBinding.GetString(this.keyCode, this.modifier);
	}

	// Token: 0x060002BC RID: 700 RVA: 0x00006CF8 File Offset: 0x00004EF8
	public static string GetString(KeyCode keyCode, UIKeyBinding.Modifier modifier)
	{
		return (modifier == UIKeyBinding.Modifier.None) ? NGUITools.KeyToCaption(keyCode) : (modifier + "+" + NGUITools.KeyToCaption(keyCode));
	}

	// Token: 0x060002BD RID: 701 RVA: 0x00028FDC File Offset: 0x000271DC
	public static bool GetKeyCode(string text, out KeyCode key, out UIKeyBinding.Modifier modifier)
	{
		key = KeyCode.None;
		modifier = UIKeyBinding.Modifier.None;
		if (string.IsNullOrEmpty(text))
		{
			return true;
		}
		if (text.Length > 2 && text.Contains("+") && text[text.Length - 1] != '+')
		{
			string[] array = text.Split(new char[]
			{
				'+'
			}, 2);
			key = NGUITools.CaptionToKey(array[1]);
			try
			{
				modifier = (UIKeyBinding.Modifier)((int)Enum.Parse(typeof(UIKeyBinding.Modifier), array[0]));
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}
		modifier = UIKeyBinding.Modifier.None;
		key = NGUITools.CaptionToKey(text);
		return true;
	}

	// Token: 0x060002BE RID: 702 RVA: 0x00029098 File Offset: 0x00027298
	public static UIKeyBinding.Modifier GetActiveModifier()
	{
		UIKeyBinding.Modifier result = UIKeyBinding.Modifier.None;
		if (UICamera.GetKey(KeyCode.LeftAlt) || UICamera.GetKey(KeyCode.RightAlt))
		{
			result = UIKeyBinding.Modifier.Alt;
		}
		else if (UICamera.GetKey(KeyCode.LeftShift) || UICamera.GetKey(KeyCode.RightShift))
		{
			result = UIKeyBinding.Modifier.Shift;
		}
		else if (UICamera.GetKey(KeyCode.LeftControl) || UICamera.GetKey(KeyCode.RightControl))
		{
			result = UIKeyBinding.Modifier.Ctrl;
		}
		return result;
	}

	// Token: 0x040001C6 RID: 454
	private static List<UIKeyBinding> mList = new List<UIKeyBinding>();

	// Token: 0x040001C7 RID: 455
	public KeyCode keyCode;

	// Token: 0x040001C8 RID: 456
	public UIKeyBinding.Modifier modifier;

	// Token: 0x040001C9 RID: 457
	public UIKeyBinding.Action action;

	// Token: 0x040001CA RID: 458
	[NonSerialized]
	private bool mIgnoreUp;

	// Token: 0x040001CB RID: 459
	[NonSerialized]
	private bool mIsInput;

	// Token: 0x040001CC RID: 460
	[NonSerialized]
	private bool mPress;

	// Token: 0x02000062 RID: 98
	[DoNotObfuscateNGUI]
	public enum Action
	{
		// Token: 0x040001CE RID: 462
		PressAndClick,
		// Token: 0x040001CF RID: 463
		Select,
		// Token: 0x040001D0 RID: 464
		All
	}

	// Token: 0x02000063 RID: 99
	[DoNotObfuscateNGUI]
	public enum Modifier
	{
		// Token: 0x040001D2 RID: 466
		Any,
		// Token: 0x040001D3 RID: 467
		Shift,
		// Token: 0x040001D4 RID: 468
		Ctrl,
		// Token: 0x040001D5 RID: 469
		Alt,
		// Token: 0x040001D6 RID: 470
		None
	}
}
