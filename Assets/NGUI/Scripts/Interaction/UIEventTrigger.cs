using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200005A RID: 90
[AddComponentMenu("NGUI/Interaction/Event Trigger")]
public class UIEventTrigger : MonoBehaviour
{
	// Token: 0x17000018 RID: 24
	// (get) Token: 0x06000279 RID: 633 RVA: 0x0002831C File Offset: 0x0002651C
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

	// Token: 0x0600027A RID: 634 RVA: 0x00028360 File Offset: 0x00026560
	private void OnHover(bool isOver)
	{
		if (UIEventTrigger.current != null || !this.isColliderEnabled)
		{
			return;
		}
		UIEventTrigger.current = this;
		if (isOver)
		{
			EventDelegate.Execute(this.onHoverOver);
		}
		else
		{
			EventDelegate.Execute(this.onHoverOut);
		}
		UIEventTrigger.current = null;
	}

	// Token: 0x0600027B RID: 635 RVA: 0x000283B8 File Offset: 0x000265B8
	private void OnPress(bool pressed)
	{
		if (UIEventTrigger.current != null || !this.isColliderEnabled)
		{
			return;
		}
		UIEventTrigger.current = this;
		if (pressed)
		{
			EventDelegate.Execute(this.onPress);
		}
		else
		{
			EventDelegate.Execute(this.onRelease);
		}
		UIEventTrigger.current = null;
	}

	// Token: 0x0600027C RID: 636 RVA: 0x00028410 File Offset: 0x00026610
	private void OnSelect(bool selected)
	{
		if (UIEventTrigger.current != null || !this.isColliderEnabled)
		{
			return;
		}
		UIEventTrigger.current = this;
		if (selected)
		{
			EventDelegate.Execute(this.onSelect);
		}
		else
		{
			EventDelegate.Execute(this.onDeselect);
		}
		UIEventTrigger.current = null;
	}

	// Token: 0x0600027D RID: 637 RVA: 0x000067FB File Offset: 0x000049FB
	private void OnClick()
	{
		if (UIEventTrigger.current != null || !this.isColliderEnabled)
		{
			return;
		}
		UIEventTrigger.current = this;
		EventDelegate.Execute(this.onClick);
		UIEventTrigger.current = null;
	}

	// Token: 0x0600027E RID: 638 RVA: 0x00006830 File Offset: 0x00004A30
	private void OnDoubleClick()
	{
		if (UIEventTrigger.current != null || !this.isColliderEnabled)
		{
			return;
		}
		UIEventTrigger.current = this;
		EventDelegate.Execute(this.onDoubleClick);
		UIEventTrigger.current = null;
	}

	// Token: 0x0600027F RID: 639 RVA: 0x00006865 File Offset: 0x00004A65
	private void OnDragStart()
	{
		if (UIEventTrigger.current != null)
		{
			return;
		}
		UIEventTrigger.current = this;
		EventDelegate.Execute(this.onDragStart);
		UIEventTrigger.current = null;
	}

	// Token: 0x06000280 RID: 640 RVA: 0x0000688F File Offset: 0x00004A8F
	private void OnDragEnd()
	{
		if (UIEventTrigger.current != null)
		{
			return;
		}
		UIEventTrigger.current = this;
		EventDelegate.Execute(this.onDragEnd);
		UIEventTrigger.current = null;
	}

	// Token: 0x06000281 RID: 641 RVA: 0x000068B9 File Offset: 0x00004AB9
	private void OnDragOver(GameObject go)
	{
		if (UIEventTrigger.current != null || !this.isColliderEnabled)
		{
			return;
		}
		UIEventTrigger.current = this;
		EventDelegate.Execute(this.onDragOver);
		UIEventTrigger.current = null;
	}

	// Token: 0x06000282 RID: 642 RVA: 0x000068EE File Offset: 0x00004AEE
	private void OnDragOut(GameObject go)
	{
		if (UIEventTrigger.current != null || !this.isColliderEnabled)
		{
			return;
		}
		UIEventTrigger.current = this;
		EventDelegate.Execute(this.onDragOut);
		UIEventTrigger.current = null;
	}

	// Token: 0x06000283 RID: 643 RVA: 0x00006923 File Offset: 0x00004B23
	private void OnDrag(Vector2 delta)
	{
		if (UIEventTrigger.current != null)
		{
			return;
		}
		UIEventTrigger.current = this;
		EventDelegate.Execute(this.onDrag);
		UIEventTrigger.current = null;
	}

	// Token: 0x0400018F RID: 399
	public static UIEventTrigger current;

	// Token: 0x04000190 RID: 400
	public List<EventDelegate> onHoverOver = new List<EventDelegate>();

	// Token: 0x04000191 RID: 401
	public List<EventDelegate> onHoverOut = new List<EventDelegate>();

	// Token: 0x04000192 RID: 402
	public List<EventDelegate> onPress = new List<EventDelegate>();

	// Token: 0x04000193 RID: 403
	public List<EventDelegate> onRelease = new List<EventDelegate>();

	// Token: 0x04000194 RID: 404
	public List<EventDelegate> onSelect = new List<EventDelegate>();

	// Token: 0x04000195 RID: 405
	public List<EventDelegate> onDeselect = new List<EventDelegate>();

	// Token: 0x04000196 RID: 406
	public List<EventDelegate> onClick = new List<EventDelegate>();

	// Token: 0x04000197 RID: 407
	public List<EventDelegate> onDoubleClick = new List<EventDelegate>();

	// Token: 0x04000198 RID: 408
	public List<EventDelegate> onDragStart = new List<EventDelegate>();

	// Token: 0x04000199 RID: 409
	public List<EventDelegate> onDragEnd = new List<EventDelegate>();

	// Token: 0x0400019A RID: 410
	public List<EventDelegate> onDragOver = new List<EventDelegate>();

	// Token: 0x0400019B RID: 411
	public List<EventDelegate> onDragOut = new List<EventDelegate>();

	// Token: 0x0400019C RID: 412
	public List<EventDelegate> onDrag = new List<EventDelegate>();
}
