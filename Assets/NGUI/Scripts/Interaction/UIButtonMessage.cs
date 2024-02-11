using System;
using UnityEngine;

// Token: 0x02000048 RID: 72
[AddComponentMenu("NGUI/Interaction/Button Message (Legacy)")]
public class UIButtonMessage : MonoBehaviour
{
	// Token: 0x06000201 RID: 513 RVA: 0x00006199 File Offset: 0x00004399
	private void Start()
	{
		this.mStarted = true;
	}

	// Token: 0x06000202 RID: 514 RVA: 0x000061A2 File Offset: 0x000043A2
	private void OnEnable()
	{
		if (this.mStarted)
		{
			this.OnHover(UICamera.IsHighlighted(base.gameObject));
		}
	}

	// Token: 0x06000203 RID: 515 RVA: 0x000061C0 File Offset: 0x000043C0
	private void OnHover(bool isOver)
	{
		if (base.enabled && ((isOver && this.trigger == UIButtonMessage.Trigger.OnMouseOver) || (!isOver && this.trigger == UIButtonMessage.Trigger.OnMouseOut)))
		{
			this.Send();
		}
	}

	// Token: 0x06000204 RID: 516 RVA: 0x000061F7 File Offset: 0x000043F7
	private void OnPress(bool isPressed)
	{
		if (base.enabled && ((isPressed && this.trigger == UIButtonMessage.Trigger.OnPress) || (!isPressed && this.trigger == UIButtonMessage.Trigger.OnRelease)))
		{
			this.Send();
		}
	}

	// Token: 0x06000205 RID: 517 RVA: 0x0000622E File Offset: 0x0000442E
	private void OnSelect(bool isSelected)
	{
		if (base.enabled && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
		{
			this.OnHover(isSelected);
		}
	}

	// Token: 0x06000206 RID: 518 RVA: 0x00006253 File Offset: 0x00004453
	private void OnClick()
	{
		if (base.enabled && this.trigger == UIButtonMessage.Trigger.OnClick)
		{
			this.Send();
		}
	}

	// Token: 0x06000207 RID: 519 RVA: 0x00006271 File Offset: 0x00004471
	private void OnDoubleClick()
	{
		if (base.enabled && this.trigger == UIButtonMessage.Trigger.OnDoubleClick)
		{
			this.Send();
		}
	}

	// Token: 0x06000208 RID: 520 RVA: 0x00025614 File Offset: 0x00023814
	private void Send()
	{
		if (string.IsNullOrEmpty(this.functionName))
		{
			return;
		}
		if (this.target == null)
		{
			this.target = base.gameObject;
		}
		if (this.includeChildren)
		{
			Transform[] componentsInChildren = this.target.GetComponentsInChildren<Transform>();
			int i = 0;
			int num = componentsInChildren.Length;
			while (i < num)
			{
				Transform transform = componentsInChildren[i];
				transform.gameObject.SendMessage(this.functionName, base.gameObject, SendMessageOptions.DontRequireReceiver);
				i++;
			}
		}
		else
		{
			this.target.SendMessage(this.functionName, base.gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x04000116 RID: 278
	public GameObject target;

	// Token: 0x04000117 RID: 279
	public string functionName;

	// Token: 0x04000118 RID: 280
	public UIButtonMessage.Trigger trigger;

	// Token: 0x04000119 RID: 281
	public bool includeChildren;

	// Token: 0x0400011A RID: 282
	private bool mStarted;

	// Token: 0x02000049 RID: 73
	[DoNotObfuscateNGUI]
	public enum Trigger
	{
		// Token: 0x0400011C RID: 284
		OnClick,
		// Token: 0x0400011D RID: 285
		OnMouseOver,
		// Token: 0x0400011E RID: 286
		OnMouseOut,
		// Token: 0x0400011F RID: 287
		OnPress,
		// Token: 0x04000120 RID: 288
		OnRelease,
		// Token: 0x04000121 RID: 289
		OnDoubleClick
	}
}
