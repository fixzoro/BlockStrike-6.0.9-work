using System;
using UnityEngine;

// Token: 0x020000B7 RID: 183
[AddComponentMenu("NGUI/Internal/Event Listener")]
public class UIEventListener : MonoBehaviour
{
	// Token: 0x17000092 RID: 146
	// (get) Token: 0x060005D7 RID: 1495 RVA: 0x00042E34 File Offset: 0x00041034
	private bool isColliderEnabled
	{
		get
		{
			if (!this.needsActiveCollider)
			{
				return true;
			}
			Collider component = base.GetComponent<Collider>();
			if (component != null)
			{
				return component.enabled;
			}
			Collider2D component2 = base.GetComponent<Collider2D>();
			return component2 != null && component2.enabled;
		}
	}

	// Token: 0x060005D8 RID: 1496 RVA: 0x00008AE7 File Offset: 0x00006CE7
	private void Start()
	{
		this.mGameObject = base.gameObject;
	}

	// Token: 0x060005D9 RID: 1497 RVA: 0x00042E84 File Offset: 0x00041084
	private void OnEnable()
	{
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Combine(UICamera.onClick, new UICamera.VoidDelegate(this.OnClick));
		UICamera.onDoubleClick = (UICamera.VoidDelegate)Delegate.Combine(UICamera.onDoubleClick, new UICamera.VoidDelegate(this.OnDoubleClick));
		UICamera.onHover = (UICamera.BoolDelegate)Delegate.Combine(UICamera.onHover, new UICamera.BoolDelegate(this.OnHover));
		UICamera.onPress = (UICamera.BoolDelegate)Delegate.Combine(UICamera.onPress, new UICamera.BoolDelegate(this.OnPress));
		UICamera.onSelect = (UICamera.BoolDelegate)Delegate.Combine(UICamera.onSelect, new UICamera.BoolDelegate(this.OnSelect));
		UICamera.onScroll = (UICamera.FloatDelegate)Delegate.Combine(UICamera.onScroll, new UICamera.FloatDelegate(this.OnScroll));
		UICamera.onDragStart = (UICamera.VoidDelegate)Delegate.Combine(UICamera.onDragStart, new UICamera.VoidDelegate(this.OnDragStart));
		UICamera.onDrag = (UICamera.VectorDelegate)Delegate.Combine(UICamera.onDrag, new UICamera.VectorDelegate(this.OnDrag));
		UICamera.onDragOver = (UICamera.ObjectDelegate)Delegate.Combine(UICamera.onDragOver, new UICamera.ObjectDelegate(this.OnDragOver));
		UICamera.onDragOut = (UICamera.ObjectDelegate)Delegate.Combine(UICamera.onDragOut, new UICamera.ObjectDelegate(this.OnDragOut));
		UICamera.onDragEnd = (UICamera.VoidDelegate)Delegate.Combine(UICamera.onDragEnd, new UICamera.VoidDelegate(this.OnDragEnd));
		UICamera.onDrop = (UICamera.ObjectDelegate)Delegate.Combine(UICamera.onDrop, new UICamera.ObjectDelegate(this.OnDrop));
		UICamera.onKey = (UICamera.KeyCodeDelegate)Delegate.Combine(UICamera.onKey, new UICamera.KeyCodeDelegate(this.OnKey));
		UICamera.onTooltip = (UICamera.BoolDelegate)Delegate.Combine(UICamera.onTooltip, new UICamera.BoolDelegate(this.OnTooltip));
	}

	// Token: 0x060005DA RID: 1498 RVA: 0x00043054 File Offset: 0x00041254
	private void OnDisable()
	{
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Remove(UICamera.onClick, new UICamera.VoidDelegate(this.OnClick));
		UICamera.onDoubleClick = (UICamera.VoidDelegate)Delegate.Remove(UICamera.onDoubleClick, new UICamera.VoidDelegate(this.OnDoubleClick));
		UICamera.onHover = (UICamera.BoolDelegate)Delegate.Remove(UICamera.onHover, new UICamera.BoolDelegate(this.OnHover));
		UICamera.onPress = (UICamera.BoolDelegate)Delegate.Remove(UICamera.onPress, new UICamera.BoolDelegate(this.OnPress));
		UICamera.onSelect = (UICamera.BoolDelegate)Delegate.Remove(UICamera.onSelect, new UICamera.BoolDelegate(this.OnSelect));
		UICamera.onScroll = (UICamera.FloatDelegate)Delegate.Remove(UICamera.onScroll, new UICamera.FloatDelegate(this.OnScroll));
		UICamera.onDragStart = (UICamera.VoidDelegate)Delegate.Remove(UICamera.onDragStart, new UICamera.VoidDelegate(this.OnDragStart));
		UICamera.onDrag = (UICamera.VectorDelegate)Delegate.Remove(UICamera.onDrag, new UICamera.VectorDelegate(this.OnDrag));
		UICamera.onDragOver = (UICamera.ObjectDelegate)Delegate.Remove(UICamera.onDragOver, new UICamera.ObjectDelegate(this.OnDragOver));
		UICamera.onDragOut = (UICamera.ObjectDelegate)Delegate.Remove(UICamera.onDragOut, new UICamera.ObjectDelegate(this.OnDragOut));
		UICamera.onDragEnd = (UICamera.VoidDelegate)Delegate.Remove(UICamera.onDragEnd, new UICamera.VoidDelegate(this.OnDragEnd));
		UICamera.onDrop = (UICamera.ObjectDelegate)Delegate.Remove(UICamera.onDrop, new UICamera.ObjectDelegate(this.OnDrop));
		UICamera.onKey = (UICamera.KeyCodeDelegate)Delegate.Remove(UICamera.onKey, new UICamera.KeyCodeDelegate(this.OnKey));
		UICamera.onTooltip = (UICamera.BoolDelegate)Delegate.Remove(UICamera.onTooltip, new UICamera.BoolDelegate(this.OnTooltip));
	}

	// Token: 0x060005DB RID: 1499 RVA: 0x00008AF5 File Offset: 0x00006CF5
	private void OnSubmit()
	{
		if (this.isColliderEnabled && this.onSubmit != null)
		{
			this.onSubmit(this.mGameObject);
		}
	}

	// Token: 0x060005DC RID: 1500 RVA: 0x00008B1E File Offset: 0x00006D1E
	private void OnClick(GameObject go)
	{
		if (this.mGameObject != go)
		{
			return;
		}
		if (this.isColliderEnabled && this.onClick != null)
		{
			this.onClick(this.mGameObject);
		}
	}

	// Token: 0x060005DD RID: 1501 RVA: 0x00008B59 File Offset: 0x00006D59
	private void OnDoubleClick(GameObject go)
	{
		if (this.mGameObject != go)
		{
			return;
		}
		if (this.isColliderEnabled && this.onDoubleClick != null)
		{
			this.onDoubleClick(this.mGameObject);
		}
	}

	// Token: 0x060005DE RID: 1502 RVA: 0x00008B94 File Offset: 0x00006D94
	private void OnHover(GameObject go, bool isOver)
	{
		if (this.mGameObject != go)
		{
			return;
		}
		if (this.isColliderEnabled && this.onHover != null)
		{
			this.onHover(this.mGameObject, isOver);
		}
	}

	// Token: 0x060005DF RID: 1503 RVA: 0x00008BD0 File Offset: 0x00006DD0
	private void OnPress(GameObject go, bool isPressed)
	{
		if (this.mGameObject != go)
		{
			return;
		}
		if (this.isColliderEnabled && this.onPress != null)
		{
			this.onPress(this.mGameObject, isPressed);
		}
	}

	// Token: 0x060005E0 RID: 1504 RVA: 0x00008C0C File Offset: 0x00006E0C
	private void OnSelect(GameObject go, bool selected)
	{
		if (this.mGameObject != go)
		{
			return;
		}
		if (this.isColliderEnabled && this.onSelect != null)
		{
			this.onSelect(this.mGameObject, selected);
		}
	}

	// Token: 0x060005E1 RID: 1505 RVA: 0x00008C48 File Offset: 0x00006E48
	private void OnScroll(GameObject go, float delta)
	{
		if (this.mGameObject != go)
		{
			return;
		}
		if (this.isColliderEnabled && this.onScroll != null)
		{
			this.onScroll(this.mGameObject, delta);
		}
	}

	// Token: 0x060005E2 RID: 1506 RVA: 0x00008C84 File Offset: 0x00006E84
	private void OnDragStart(GameObject go)
	{
		if (this.mGameObject != go)
		{
			return;
		}
		if (this.onDragStart != null)
		{
			this.onDragStart(this.mGameObject);
		}
	}

	// Token: 0x060005E3 RID: 1507 RVA: 0x00008CB4 File Offset: 0x00006EB4
	private void OnDrag(GameObject go, Vector2 delta)
	{
		if (this.mGameObject != go)
		{
			return;
		}
		if (this.onDrag != null)
		{
			this.onDrag(this.mGameObject, delta);
		}
	}

	// Token: 0x060005E4 RID: 1508 RVA: 0x00008CE5 File Offset: 0x00006EE5
	private void OnDragOver(GameObject go, GameObject obj)
	{
		if (this.mGameObject != go)
		{
			return;
		}
		if (this.isColliderEnabled && this.onDragOver != null)
		{
			this.onDragOver(this.mGameObject);
		}
	}

	// Token: 0x060005E5 RID: 1509 RVA: 0x00008D20 File Offset: 0x00006F20
	private void OnDragOut(GameObject go, GameObject obj)
	{
		if (this.mGameObject != go)
		{
			return;
		}
		if (this.isColliderEnabled && this.onDragOut != null)
		{
			this.onDragOut(this.mGameObject);
		}
	}

	// Token: 0x060005E6 RID: 1510 RVA: 0x00008D5B File Offset: 0x00006F5B
	private void OnDragEnd(GameObject go)
	{
		if (this.mGameObject != go)
		{
			return;
		}
		if (this.onDragEnd != null)
		{
			this.onDragEnd(this.mGameObject);
		}
	}

	// Token: 0x060005E7 RID: 1511 RVA: 0x00008D8B File Offset: 0x00006F8B
	private void OnDrop(GameObject go, GameObject obj)
	{
		if (this.mGameObject != go)
		{
			return;
		}
		if (this.isColliderEnabled && this.onDrop != null)
		{
			this.onDrop(this.mGameObject, obj);
		}
	}

	// Token: 0x060005E8 RID: 1512 RVA: 0x00008DC7 File Offset: 0x00006FC7
	private void OnKey(GameObject go, KeyCode key)
	{
		if (this.mGameObject != go)
		{
			return;
		}
		if (this.isColliderEnabled && this.onKey != null)
		{
			this.onKey(this.mGameObject, key);
		}
	}

	// Token: 0x060005E9 RID: 1513 RVA: 0x00008E03 File Offset: 0x00007003
	private void OnTooltip(GameObject go, bool show)
	{
		if (this.mGameObject != go)
		{
			return;
		}
		if (this.isColliderEnabled && this.onTooltip != null)
		{
			this.onTooltip(this.mGameObject, show);
		}
	}

	// Token: 0x060005EA RID: 1514 RVA: 0x00043224 File Offset: 0x00041424
	public void Clear()
	{
		this.onSubmit = null;
		this.onClick = null;
		this.onDoubleClick = null;
		this.onHover = null;
		this.onPress = null;
		this.onSelect = null;
		this.onScroll = null;
		this.onDragStart = null;
		this.onDrag = null;
		this.onDragOver = null;
		this.onDragOut = null;
		this.onDragEnd = null;
		this.onDrop = null;
		this.onKey = null;
		this.onTooltip = null;
	}

	// Token: 0x060005EB RID: 1515 RVA: 0x0004329C File Offset: 0x0004149C
	public static UIEventListener Get(GameObject go)
	{
		UIEventListener uieventListener = go.GetComponent<UIEventListener>();
		if (uieventListener == null)
		{
			uieventListener = go.AddComponent<UIEventListener>();
		}
		return uieventListener;
	}

	// Token: 0x04000429 RID: 1065
	private GameObject mGameObject;

	// Token: 0x0400042A RID: 1066
	public object parameter;

	// Token: 0x0400042B RID: 1067
	public UIEventListener.VoidDelegate onSubmit;

	// Token: 0x0400042C RID: 1068
	public UIEventListener.VoidDelegate onClick;

	// Token: 0x0400042D RID: 1069
	public UIEventListener.VoidDelegate onDoubleClick;

	// Token: 0x0400042E RID: 1070
	public UIEventListener.BoolDelegate onHover;

	// Token: 0x0400042F RID: 1071
	public UIEventListener.BoolDelegate onPress;

	// Token: 0x04000430 RID: 1072
	public UIEventListener.BoolDelegate onSelect;

	// Token: 0x04000431 RID: 1073
	public UIEventListener.FloatDelegate onScroll;

	// Token: 0x04000432 RID: 1074
	public UIEventListener.VoidDelegate onDragStart;

	// Token: 0x04000433 RID: 1075
	public UIEventListener.VectorDelegate onDrag;

	// Token: 0x04000434 RID: 1076
	public UIEventListener.VoidDelegate onDragOver;

	// Token: 0x04000435 RID: 1077
	public UIEventListener.VoidDelegate onDragOut;

	// Token: 0x04000436 RID: 1078
	public UIEventListener.VoidDelegate onDragEnd;

	// Token: 0x04000437 RID: 1079
	public UIEventListener.ObjectDelegate onDrop;

	// Token: 0x04000438 RID: 1080
	public UIEventListener.KeyCodeDelegate onKey;

	// Token: 0x04000439 RID: 1081
	public UIEventListener.BoolDelegate onTooltip;

	// Token: 0x0400043A RID: 1082
	public bool needsActiveCollider = true;

	// Token: 0x020000B8 RID: 184
	// (Invoke) Token: 0x060005ED RID: 1517
	public delegate void VoidDelegate(GameObject go);

	// Token: 0x020000B9 RID: 185
	// (Invoke) Token: 0x060005F1 RID: 1521
	public delegate void BoolDelegate(GameObject go, bool state);

	// Token: 0x020000BA RID: 186
	// (Invoke) Token: 0x060005F5 RID: 1525
	public delegate void FloatDelegate(GameObject go, float delta);

	// Token: 0x020000BB RID: 187
	// (Invoke) Token: 0x060005F9 RID: 1529
	public delegate void VectorDelegate(GameObject go, Vector2 delta);

	// Token: 0x020000BC RID: 188
	// (Invoke) Token: 0x060005FD RID: 1533
	public delegate void ObjectDelegate(GameObject go, GameObject obj);

	// Token: 0x020000BD RID: 189
	// (Invoke) Token: 0x06000601 RID: 1537
	public delegate void KeyCodeDelegate(GameObject go, KeyCode key);
}
