using System;
using UnityEngine;

// Token: 0x0200005B RID: 91
[AddComponentMenu("NGUI/Interaction/Forward Events (Legacy)")]
public class UIForwardEvents : MonoBehaviour
{
	// Token: 0x06000285 RID: 645 RVA: 0x0000694D File Offset: 0x00004B4D
	private void OnHover(bool isOver)
	{
		if (this.onHover && this.target != null)
		{
			this.target.SendMessage("OnHover", isOver, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x06000286 RID: 646 RVA: 0x00006982 File Offset: 0x00004B82
	private void OnPress(bool pressed)
	{
		if (this.onPress && this.target != null)
		{
			this.target.SendMessage("OnPress", pressed, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x06000287 RID: 647 RVA: 0x000069B7 File Offset: 0x00004BB7
	private void OnClick()
	{
		if (this.onClick && this.target != null)
		{
			this.target.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x06000288 RID: 648 RVA: 0x000069E6 File Offset: 0x00004BE6
	private void OnDoubleClick()
	{
		if (this.onDoubleClick && this.target != null)
		{
			this.target.SendMessage("OnDoubleClick", SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x06000289 RID: 649 RVA: 0x00006A15 File Offset: 0x00004C15
	private void OnSelect(bool selected)
	{
		if (this.onSelect && this.target != null)
		{
			this.target.SendMessage("OnSelect", selected, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x0600028A RID: 650 RVA: 0x00006A4A File Offset: 0x00004C4A
	private void OnDrag(Vector2 delta)
	{
		if (this.onDrag && this.target != null)
		{
			this.target.SendMessage("OnDrag", delta, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x0600028B RID: 651 RVA: 0x00006A7F File Offset: 0x00004C7F
	private void OnDrop(GameObject go)
	{
		if (this.onDrop && this.target != null)
		{
			this.target.SendMessage("OnDrop", go, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x0600028C RID: 652 RVA: 0x00006AAF File Offset: 0x00004CAF
	private void OnSubmit()
	{
		if (this.onSubmit && this.target != null)
		{
			this.target.SendMessage("OnSubmit", SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x0600028D RID: 653 RVA: 0x00006ADE File Offset: 0x00004CDE
	private void OnScroll(float delta)
	{
		if (this.onScroll && this.target != null)
		{
			this.target.SendMessage("OnScroll", delta, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x0400019D RID: 413
	public GameObject target;

	// Token: 0x0400019E RID: 414
	public bool onHover;

	// Token: 0x0400019F RID: 415
	public bool onPress;

	// Token: 0x040001A0 RID: 416
	public bool onClick;

	// Token: 0x040001A1 RID: 417
	public bool onDoubleClick;

	// Token: 0x040001A2 RID: 418
	public bool onSelect;

	// Token: 0x040001A3 RID: 419
	public bool onDrag;

	// Token: 0x040001A4 RID: 420
	public bool onDrop;

	// Token: 0x040001A5 RID: 421
	public bool onSubmit;

	// Token: 0x040001A6 RID: 422
	public bool onScroll;
}
