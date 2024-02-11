using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000052 RID: 82
[AddComponentMenu("NGUI/Interaction/Drag and Drop Item")]
public class UIDragDropItem : MonoBehaviour
{
	// Token: 0x06000239 RID: 569 RVA: 0x000264B0 File Offset: 0x000246B0
	public static bool IsDragged(GameObject go)
	{
		foreach (UIDragDropItem uidragDropItem in UIDragDropItem.draggedItems)
		{
			if (uidragDropItem.gameObject == go)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600023A RID: 570 RVA: 0x00006512 File Offset: 0x00004712
	protected virtual void Awake()
	{
		this.mTrans = base.transform;
		this.mCollider = base.GetComponent<Collider>();
		this.mCollider2D = base.GetComponent<Collider2D>();
	}

	// Token: 0x0600023B RID: 571 RVA: 0x0000574F File Offset: 0x0000394F
	protected virtual void OnEnable()
	{
	}

	// Token: 0x0600023C RID: 572 RVA: 0x00026520 File Offset: 0x00024720
	protected virtual void OnDisable()
	{
		if (this.mDragging)
		{
			this.StopDragging(null);
			UICamera.onPress = (UICamera.BoolDelegate)Delegate.Remove(UICamera.onPress, new UICamera.BoolDelegate(this.OnGlobalPress));
			UICamera.onClick = (UICamera.VoidDelegate)Delegate.Remove(UICamera.onClick, new UICamera.VoidDelegate(this.OnGlobalClick));
			UICamera.onMouseMove = (UICamera.MoveDelegate)Delegate.Remove(UICamera.onMouseMove, new UICamera.MoveDelegate(this.OnDrag));
		}
	}

	// Token: 0x0600023D RID: 573 RVA: 0x00006538 File Offset: 0x00004738
	protected virtual void Start()
	{
		this.mButton = base.GetComponent<UIButton>();
		this.mDragScrollView = base.GetComponent<UIDragScrollView>();
	}

	// Token: 0x0600023E RID: 574 RVA: 0x000265A0 File Offset: 0x000247A0
	protected virtual void OnPress(bool isPressed)
	{
		if (!this.interactable || UICamera.currentTouchID == -2 || UICamera.currentTouchID == -3)
		{
			return;
		}
		if (isPressed)
		{
			if (!this.mPressed)
			{
				this.mTouch = UICamera.currentTouch;
				this.mDragStartTime = RealTime.time + this.pressAndHoldDelay;
				this.mPressed = true;
			}
		}
		else if (this.mPressed && this.mTouch == UICamera.currentTouch)
		{
			this.mPressed = false;
			if (!this.mDragging || !this.clickToDrag)
			{
				this.mTouch = null;
			}
		}
	}

	// Token: 0x0600023F RID: 575 RVA: 0x0002664C File Offset: 0x0002484C
	protected virtual void OnClick()
	{
		if (this.clickToDrag && !this.mDragging && UICamera.currentTouchID == -1 && UIDragDropItem.draggedItems.Count == 0)
		{
			this.mTouch = UICamera.currentTouch;
			UIDragDropItem uidragDropItem = this.StartDragging();
			if (this.clickToDrag && uidragDropItem != null)
			{
				UICamera.onMouseMove = (UICamera.MoveDelegate)Delegate.Combine(UICamera.onMouseMove, new UICamera.MoveDelegate(uidragDropItem.OnDrag));
				UICamera.onPress = (UICamera.BoolDelegate)Delegate.Combine(UICamera.onPress, new UICamera.BoolDelegate(uidragDropItem.OnGlobalPress));
				UICamera.onClick = (UICamera.VoidDelegate)Delegate.Combine(UICamera.onClick, new UICamera.VoidDelegate(uidragDropItem.OnGlobalClick));
			}
		}
	}

	// Token: 0x06000240 RID: 576 RVA: 0x00026714 File Offset: 0x00024914
	protected void OnGlobalPress(GameObject go, bool state)
	{
		if (state && UICamera.currentTouchID != -1)
		{
			this.StopDragging(null);
			UICamera.onPress = (UICamera.BoolDelegate)Delegate.Remove(UICamera.onPress, new UICamera.BoolDelegate(this.OnGlobalPress));
			UICamera.onClick = (UICamera.VoidDelegate)Delegate.Remove(UICamera.onClick, new UICamera.VoidDelegate(this.OnGlobalClick));
			UICamera.onMouseMove = (UICamera.MoveDelegate)Delegate.Remove(UICamera.onMouseMove, new UICamera.MoveDelegate(this.OnDrag));
		}
	}

	// Token: 0x06000241 RID: 577 RVA: 0x0002679C File Offset: 0x0002499C
	protected void OnGlobalClick(GameObject go)
	{
		if (UICamera.currentTouchID == -1)
		{
			this.StopDragging(go);
		}
		else
		{
			this.StopDragging(null);
		}
		UICamera.onPress = (UICamera.BoolDelegate)Delegate.Remove(UICamera.onPress, new UICamera.BoolDelegate(this.OnGlobalPress));
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Remove(UICamera.onClick, new UICamera.VoidDelegate(this.OnGlobalClick));
		UICamera.onMouseMove = (UICamera.MoveDelegate)Delegate.Remove(UICamera.onMouseMove, new UICamera.MoveDelegate(this.OnDrag));
	}

	// Token: 0x06000242 RID: 578 RVA: 0x00006552 File Offset: 0x00004752
	protected virtual void Update()
	{
		if (this.restriction == UIDragDropItem.Restriction.PressAndHold && this.mPressed && !this.mDragging && this.mDragStartTime < RealTime.time)
		{
			this.StartDragging();
		}
	}

	// Token: 0x06000243 RID: 579 RVA: 0x00026828 File Offset: 0x00024A28
	protected virtual void OnDragStart()
	{
		if (!this.interactable)
		{
			return;
		}
		if (!base.enabled || this.mTouch != UICamera.currentTouch)
		{
			return;
		}
		if (this.restriction != UIDragDropItem.Restriction.None)
		{
			if (this.restriction == UIDragDropItem.Restriction.Horizontal)
			{
				Vector2 totalDelta = this.mTouch.totalDelta;
				if (Mathf.Abs(totalDelta.x) < Mathf.Abs(totalDelta.y))
				{
					return;
				}
			}
			else if (this.restriction == UIDragDropItem.Restriction.Vertical)
			{
				Vector2 totalDelta2 = this.mTouch.totalDelta;
				if (Mathf.Abs(totalDelta2.x) > Mathf.Abs(totalDelta2.y))
				{
					return;
				}
			}
			else if (this.restriction == UIDragDropItem.Restriction.PressAndHold)
			{
				return;
			}
		}
		this.StartDragging();
	}

	// Token: 0x06000244 RID: 580 RVA: 0x000268F4 File Offset: 0x00024AF4
	public virtual UIDragDropItem StartDragging()
	{
		if (!this.interactable || !base.transform || !base.transform.parent)
		{
			return null;
		}
		if (this.mDragging)
		{
			return null;
		}
		if (this.cloneOnDrag)
		{
			this.mPressed = false;
			GameObject gameObject = base.transform.parent.gameObject.AddChild(base.gameObject);
			gameObject.transform.localPosition = base.transform.localPosition;
			gameObject.transform.localRotation = base.transform.localRotation;
			gameObject.transform.localScale = base.transform.localScale;
			UIButtonColor component = gameObject.GetComponent<UIButtonColor>();
			if (component != null)
			{
				component.defaultColor = base.GetComponent<UIButtonColor>().defaultColor;
			}
			if (this.mTouch != null && this.mTouch.pressed == base.gameObject)
			{
				this.mTouch.current = gameObject;
				this.mTouch.pressed = gameObject;
				this.mTouch.dragged = gameObject;
				this.mTouch.last = gameObject;
			}
			UIDragDropItem component2 = gameObject.GetComponent<UIDragDropItem>();
			component2.mTouch = this.mTouch;
			component2.mPressed = true;
			component2.mDragging = true;
			component2.Start();
			component2.OnClone(base.gameObject);
			component2.OnDragDropStart();
			if (UICamera.currentTouch == null)
			{
				UICamera.currentTouch = this.mTouch;
			}
			this.mTouch = null;
			UICamera.Notify(base.gameObject, "OnPress", false);
			UICamera.Notify(base.gameObject, "OnHover", false);
			return component2;
		}
		this.mDragging = true;
		this.OnDragDropStart();
		return this;
	}

	// Token: 0x06000245 RID: 581 RVA: 0x0000574F File Offset: 0x0000394F
	protected virtual void OnClone(GameObject original)
	{
	}

	// Token: 0x06000246 RID: 582 RVA: 0x00026ABC File Offset: 0x00024CBC
	protected virtual void OnDrag(Vector2 delta)
	{
		if (!this.interactable)
		{
			return;
		}
		if (!this.mDragging || !base.enabled || this.mTouch != UICamera.currentTouch)
		{
			return;
		}
		if (this.mRoot != null)
		{
			this.OnDragDropMove(delta * this.mRoot.pixelSizeAdjustment);
		}
		else
		{
			this.OnDragDropMove(delta);
		}
	}

	// Token: 0x06000247 RID: 583 RVA: 0x00026B30 File Offset: 0x00024D30
	protected virtual void OnDragEnd()
	{
		if (!this.interactable)
		{
			return;
		}
		if (!base.enabled || this.mTouch != UICamera.currentTouch)
		{
			return;
		}
		this.StopDragging((!(UICamera.lastHit.collider != null)) ? null : UICamera.lastHit.collider.gameObject);
	}

	// Token: 0x06000248 RID: 584 RVA: 0x0000658D File Offset: 0x0000478D
	public void StopDragging(GameObject go = null)
	{
		if (this.mDragging)
		{
			this.mDragging = false;
			this.OnDragDropRelease(go);
		}
	}

	// Token: 0x06000249 RID: 585 RVA: 0x00026B98 File Offset: 0x00024D98
	protected virtual void OnDragDropStart()
	{
		if (!UIDragDropItem.draggedItems.Contains(this))
		{
			UIDragDropItem.draggedItems.Add(this);
		}
		if (this.mDragScrollView != null)
		{
			this.mDragScrollView.enabled = false;
		}
		if (this.mButton != null)
		{
			this.mButton.isEnabled = false;
		}
		else if (this.mCollider != null)
		{
			this.mCollider.enabled = false;
		}
		else if (this.mCollider2D != null)
		{
			this.mCollider2D.enabled = false;
		}
		this.mParent = this.mTrans.parent;
		this.mRoot = NGUITools.FindInParents<UIRoot>(this.mParent);
		this.mGrid = NGUITools.FindInParents<UIGrid>(this.mParent);
		this.mTable = NGUITools.FindInParents<UITable>(this.mParent);
		if (UIDragDropRoot.root != null)
		{
			this.mTrans.parent = UIDragDropRoot.root;
		}
		Vector3 localPosition = this.mTrans.localPosition;
		localPosition.z = 0f;
		this.mTrans.localPosition = localPosition;
		TweenPosition component = base.GetComponent<TweenPosition>();
		if (component != null)
		{
			component.enabled = false;
		}
		SpringPosition component2 = base.GetComponent<SpringPosition>();
		if (component2 != null)
		{
			component2.enabled = false;
		}
		NGUITools.MarkParentAsChanged(base.gameObject);
		if (this.mTable != null)
		{
			this.mTable.repositionNow = true;
		}
		if (this.mGrid != null)
		{
			this.mGrid.repositionNow = true;
		}
	}

	// Token: 0x0600024A RID: 586 RVA: 0x000065A8 File Offset: 0x000047A8
	protected virtual void OnDragDropMove(Vector2 delta)
	{
		if (this.mParent != null)
		{
			this.mTrans.localPosition += this.mTrans.InverseTransformDirection(delta);
		}
	}

	// Token: 0x0600024B RID: 587 RVA: 0x00026D40 File Offset: 0x00024F40
	protected virtual void OnDragDropRelease(GameObject surface)
	{
		if (!this.cloneOnDrag)
		{
			UIDragScrollView[] componentsInChildren = base.GetComponentsInChildren<UIDragScrollView>();
			foreach (UIDragScrollView uidragScrollView in componentsInChildren)
			{
				uidragScrollView.scrollView = null;
			}
			if (this.mButton != null)
			{
				this.mButton.isEnabled = true;
			}
			else if (this.mCollider != null)
			{
				this.mCollider.enabled = true;
			}
			else if (this.mCollider2D != null)
			{
				this.mCollider2D.enabled = true;
			}
			UIDragDropContainer uidragDropContainer = (!surface) ? null : NGUITools.FindInParents<UIDragDropContainer>(surface);
			if (uidragDropContainer != null)
			{
				this.mTrans.parent = ((!(uidragDropContainer.reparentTarget != null)) ? uidragDropContainer.transform : uidragDropContainer.reparentTarget);
				Vector3 localPosition = this.mTrans.localPosition;
				localPosition.z = 0f;
				this.mTrans.localPosition = localPosition;
			}
			else
			{
				this.mTrans.parent = this.mParent;
			}
			this.mParent = this.mTrans.parent;
			this.mGrid = NGUITools.FindInParents<UIGrid>(this.mParent);
			this.mTable = NGUITools.FindInParents<UITable>(this.mParent);
			if (this.mDragScrollView != null)
			{
				base.Invoke("EnableDragScrollView", 0.001f);
			}
			NGUITools.MarkParentAsChanged(base.gameObject);
			if (this.mTable != null)
			{
				this.mTable.repositionNow = true;
			}
			if (this.mGrid != null)
			{
				this.mGrid.repositionNow = true;
			}
		}
		this.OnDragDropEnd(surface);
		if (this.cloneOnDrag)
		{
			this.DestroySelf();
		}
	}

	// Token: 0x0600024C RID: 588 RVA: 0x000065E2 File Offset: 0x000047E2
	protected virtual void DestroySelf()
	{
		NGUITools.Destroy(base.gameObject);
	}

	// Token: 0x0600024D RID: 589 RVA: 0x000065EF File Offset: 0x000047EF
	protected virtual void OnDragDropEnd(GameObject surface)
	{
		UIDragDropItem.draggedItems.Remove(this);
		this.mParent = null;
	}

	// Token: 0x0600024E RID: 590 RVA: 0x00006604 File Offset: 0x00004804
	protected void EnableDragScrollView()
	{
		if (this.mDragScrollView != null)
		{
			this.mDragScrollView.enabled = true;
		}
	}

	// Token: 0x0600024F RID: 591 RVA: 0x00006623 File Offset: 0x00004823
	protected void OnApplicationFocus(bool focus)
	{
		if (!focus)
		{
			this.StopDragging(null);
		}
	}

	// Token: 0x0400013D RID: 317
	[Tooltip("What kind of restriction is applied to the drag & drop logic before dragging is made possible.")]
	public UIDragDropItem.Restriction restriction;

	// Token: 0x0400013E RID: 318
	[Tooltip("By default, dragging only happens while holding the mouse button / touch. If desired, you can opt to use a click-based approach instead. Note that this only works with a mouse.")]
	public bool clickToDrag;

	// Token: 0x0400013F RID: 319
	[Tooltip("Whether a copy of the item will be dragged instead of the item itself.")]
	public bool cloneOnDrag;

	// Token: 0x04000140 RID: 320
	[Tooltip("Whether this drag and drop item can be interacted with. If not, only tooltips will work.")]
	public bool interactable = true;

	// Token: 0x04000141 RID: 321
	[Tooltip("How long the user has to press on an item before the drag action activates.")]
	[HideInInspector]
	public float pressAndHoldDelay = 1f;

	// Token: 0x04000142 RID: 322
	[NonSerialized]
	protected Transform mTrans;

	// Token: 0x04000143 RID: 323
	[NonSerialized]
	protected Transform mParent;

	// Token: 0x04000144 RID: 324
	[NonSerialized]
	protected Collider mCollider;

	// Token: 0x04000145 RID: 325
	[NonSerialized]
	protected Collider2D mCollider2D;

	// Token: 0x04000146 RID: 326
	[NonSerialized]
	protected UIButton mButton;

	// Token: 0x04000147 RID: 327
	[NonSerialized]
	protected UIRoot mRoot;

	// Token: 0x04000148 RID: 328
	[NonSerialized]
	protected UIGrid mGrid;

	// Token: 0x04000149 RID: 329
	[NonSerialized]
	protected UITable mTable;

	// Token: 0x0400014A RID: 330
	[NonSerialized]
	protected float mDragStartTime;

	// Token: 0x0400014B RID: 331
	[NonSerialized]
	protected UIDragScrollView mDragScrollView;

	// Token: 0x0400014C RID: 332
	[NonSerialized]
	protected bool mPressed;

	// Token: 0x0400014D RID: 333
	[NonSerialized]
	protected bool mDragging;

	// Token: 0x0400014E RID: 334
	[NonSerialized]
	protected UICamera.MouseOrTouch mTouch;

	// Token: 0x0400014F RID: 335
	[NonSerialized]
	public static List<UIDragDropItem> draggedItems = new List<UIDragDropItem>();

	// Token: 0x02000053 RID: 83
	[DoNotObfuscateNGUI]
	public enum Restriction
	{
		// Token: 0x04000151 RID: 337
		None,
		// Token: 0x04000152 RID: 338
		Horizontal,
		// Token: 0x04000153 RID: 339
		Vertical,
		// Token: 0x04000154 RID: 340
		PressAndHold
	}
}
