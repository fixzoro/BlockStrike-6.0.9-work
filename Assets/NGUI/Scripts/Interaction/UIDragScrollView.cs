using System;
using UnityEngine;

// Token: 0x02000058 RID: 88
[AddComponentMenu("NGUI/Interaction/Drag Scroll View")]
public class UIDragScrollView : MonoBehaviour
{
	// Token: 0x06000266 RID: 614 RVA: 0x00027A00 File Offset: 0x00025C00
	private void OnEnable()
	{
		this.mGameObject = base.gameObject;
		this.mTrans = base.transform;
		if (this.scrollView == null && this.draggablePanel != null)
		{
			this.scrollView = this.draggablePanel;
			this.draggablePanel = null;
		}
		if (this.mStarted && (this.mAutoFind || this.mScroll == null))
		{
			this.FindScrollView();
		}
		UICamera.onPress = (UICamera.BoolDelegate)Delegate.Combine(UICamera.onPress, new UICamera.BoolDelegate(this.OnPress));
		UICamera.onDrag = (UICamera.VectorDelegate)Delegate.Combine(UICamera.onDrag, new UICamera.VectorDelegate(this.OnDrag));
		UICamera.onScroll = (UICamera.FloatDelegate)Delegate.Combine(UICamera.onScroll, new UICamera.FloatDelegate(this.OnScroll));
		UICamera.onPan = (UICamera.VectorDelegate)Delegate.Combine(UICamera.onPan, new UICamera.VectorDelegate(this.OnPan));
	}

	// Token: 0x06000267 RID: 615 RVA: 0x000066F4 File Offset: 0x000048F4
	private void Start()
	{
		this.mStarted = true;
		this.FindScrollView();
	}

	// Token: 0x06000268 RID: 616 RVA: 0x00027B08 File Offset: 0x00025D08
	private void FindScrollView()
	{
		UIScrollView uiscrollView = NGUITools.FindInParents<UIScrollView>(this.mTrans);
		if (this.scrollView == null || (this.mAutoFind && uiscrollView != this.scrollView))
		{
			this.scrollView = uiscrollView;
			this.mAutoFind = true;
		}
		else if (this.scrollView == uiscrollView)
		{
			this.mAutoFind = true;
		}
		this.mScroll = this.scrollView;
	}

	// Token: 0x06000269 RID: 617 RVA: 0x00027B88 File Offset: 0x00025D88
	private void OnDisable()
	{
		if (this.mPressed && this.mScroll != null && this.mScroll.GetComponentInChildren<UIWrapContent>() == null)
		{
			this.mScroll.Press(false);
			this.mScroll = null;
		}
		UICamera.onPress = (UICamera.BoolDelegate)Delegate.Remove(UICamera.onPress, new UICamera.BoolDelegate(this.OnPress));
		UICamera.onDrag = (UICamera.VectorDelegate)Delegate.Remove(UICamera.onDrag, new UICamera.VectorDelegate(this.OnDrag));
		UICamera.onScroll = (UICamera.FloatDelegate)Delegate.Remove(UICamera.onScroll, new UICamera.FloatDelegate(this.OnScroll));
		UICamera.onPan = (UICamera.VectorDelegate)Delegate.Remove(UICamera.onPan, new UICamera.VectorDelegate(this.OnPan));
	}

	// Token: 0x0600026A RID: 618 RVA: 0x00027C5C File Offset: 0x00025E5C
	private void OnPress(GameObject go, bool pressed)
	{
		if (this.mGameObject != go)
		{
			return;
		}
		this.mPressed = pressed;
		if (this.mAutoFind && this.mScroll != this.scrollView)
		{
			this.mScroll = this.scrollView;
			this.mAutoFind = false;
		}
		if (this.scrollView && base.enabled && NGUITools.GetActive(base.gameObject))
		{
			this.scrollView.Press(pressed);
			if (!pressed && this.mAutoFind)
			{
				this.scrollView = NGUITools.FindInParents<UIScrollView>(this.mTrans);
				this.mScroll = this.scrollView;
			}
		}
	}

	// Token: 0x0600026B RID: 619 RVA: 0x00006703 File Offset: 0x00004903
	private void OnDrag(GameObject go, Vector2 delta)
	{
		if (this.mGameObject != go)
		{
			return;
		}
		if (this.scrollView && NGUITools.GetActive(this))
		{
			this.scrollView.Drag();
		}
	}

	// Token: 0x0600026C RID: 620 RVA: 0x0000673D File Offset: 0x0000493D
	private void OnScroll(GameObject go, float delta)
	{
		if (this.mGameObject != go)
		{
			return;
		}
		if (this.scrollView && NGUITools.GetActive(this))
		{
			this.scrollView.Scroll(delta);
		}
	}

	// Token: 0x0600026D RID: 621 RVA: 0x00006778 File Offset: 0x00004978
	public void OnPan(GameObject go, Vector2 delta)
	{
		if (this.mGameObject != go)
		{
			return;
		}
		if (this.scrollView && NGUITools.GetActive(this))
		{
			this.scrollView.OnPan(delta);
		}
	}

	// Token: 0x04000179 RID: 377
	public UIScrollView scrollView;

	// Token: 0x0400017A RID: 378
	[SerializeField]
	[HideInInspector]
	private UIScrollView draggablePanel;

	// Token: 0x0400017B RID: 379
	private Transform mTrans;

	// Token: 0x0400017C RID: 380
	private UIScrollView mScroll;

	// Token: 0x0400017D RID: 381
	private bool mAutoFind;

	// Token: 0x0400017E RID: 382
	private bool mStarted;

	// Token: 0x0400017F RID: 383
	private GameObject mGameObject;

	// Token: 0x04000180 RID: 384
	[NonSerialized]
	private bool mPressed;
}
