using System;
using UnityEngine;

// Token: 0x02000059 RID: 89
[AddComponentMenu("NGUI/Interaction/Draggable Camera")]
[RequireComponent(typeof(Camera))]
public class UIDraggableCamera : MonoBehaviour
{
	// Token: 0x17000017 RID: 23
	// (get) Token: 0x0600026F RID: 623 RVA: 0x000067EA File Offset: 0x000049EA
	// (set) Token: 0x06000270 RID: 624 RVA: 0x000067F2 File Offset: 0x000049F2
	public Vector2 currentMomentum
	{
		get
		{
			return this.mMomentum;
		}
		set
		{
			this.mMomentum = value;
		}
	}

	// Token: 0x06000271 RID: 625 RVA: 0x00027D1C File Offset: 0x00025F1C
	private void Start()
	{
		this.mCam = base.GetComponent<Camera>();
		this.mTrans = base.transform;
		this.mRoot = NGUITools.FindInParents<UIRoot>(base.gameObject);
		if (this.rootForBounds == null)
		{
			Debug.LogError(NGUITools.GetHierarchy(base.gameObject) + " needs the 'Root For Bounds' parameter to be set", this);
			base.enabled = false;
		}
	}

	// Token: 0x06000272 RID: 626 RVA: 0x00027D88 File Offset: 0x00025F88
	private Vector3 CalculateConstrainOffset()
	{
		if (this.rootForBounds == null || this.rootForBounds.childCount == 0)
		{
			return Vector3.zero;
		}
		Vector3 vector = new Vector3(this.mCam.rect.xMin * (float)Screen.width, this.mCam.rect.yMin * (float)Screen.height, 0f);
		Vector3 vector2 = new Vector3(this.mCam.rect.xMax * (float)Screen.width, this.mCam.rect.yMax * (float)Screen.height, 0f);
		vector = this.mCam.ScreenToWorldPoint(vector);
		vector2 = this.mCam.ScreenToWorldPoint(vector2);
		Vector2 minRect = new Vector2(this.mBounds.min.x, this.mBounds.min.y);
		Vector2 maxRect = new Vector2(this.mBounds.max.x, this.mBounds.max.y);
		return NGUIMath.ConstrainRect(minRect, maxRect, vector, vector2);
	}

	// Token: 0x06000273 RID: 627 RVA: 0x00027ED0 File Offset: 0x000260D0
	public bool ConstrainToBounds(bool immediate)
	{
		if (this.mTrans != null && this.rootForBounds != null)
		{
			Vector3 b = this.CalculateConstrainOffset();
			if (b.sqrMagnitude > 0f)
			{
				if (immediate)
				{
					this.mTrans.position -= b;
				}
				else
				{
					SpringPosition springPosition = SpringPosition.Begin(base.gameObject, this.mTrans.position - b, 13f);
					springPosition.ignoreTimeScale = true;
					springPosition.worldSpace = true;
				}
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000274 RID: 628 RVA: 0x00027F6C File Offset: 0x0002616C
	public void Press(bool isPressed)
	{
		if (isPressed)
		{
			this.mDragStarted = false;
		}
		if (this.rootForBounds != null)
		{
			this.mPressed = isPressed;
			if (isPressed)
			{
				this.mBounds = NGUIMath.CalculateAbsoluteWidgetBounds(this.rootForBounds);
				this.mMomentum = Vector2.zero;
				this.mScroll = 0f;
				SpringPosition component = base.GetComponent<SpringPosition>();
				if (component != null)
				{
					component.enabled = false;
				}
			}
			else if (this.dragEffect == UIDragObject.DragEffect.MomentumAndSpring)
			{
				this.ConstrainToBounds(false);
			}
		}
	}

	// Token: 0x06000275 RID: 629 RVA: 0x00028000 File Offset: 0x00026200
	public void Drag(Vector2 delta)
	{
		if (this.smoothDragStart && !this.mDragStarted)
		{
			this.mDragStarted = true;
			return;
		}
		UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
		if (this.mRoot != null)
		{
			delta *= this.mRoot.pixelSizeAdjustment;
		}
		Vector2 vector = Vector2.Scale(delta, -this.scale);
		this.mTrans.localPosition += (Vector3)vector;
		this.mMomentum = Vector2.Lerp(this.mMomentum, this.mMomentum + vector * (0.01f * this.momentumAmount), 0.67f);
		if (this.dragEffect != UIDragObject.DragEffect.MomentumAndSpring && this.ConstrainToBounds(true))
		{
			this.mMomentum = Vector2.zero;
			this.mScroll = 0f;
		}
	}

	// Token: 0x06000276 RID: 630 RVA: 0x000280EC File Offset: 0x000262EC
	public void Scroll(float delta)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject))
		{
			if (Mathf.Sign(this.mScroll) != Mathf.Sign(delta))
			{
				this.mScroll = 0f;
			}
			this.mScroll += delta * this.scrollWheelFactor;
		}
	}

	// Token: 0x06000277 RID: 631 RVA: 0x0002814C File Offset: 0x0002634C
	private void Update()
	{
		float deltaTime = RealTime.deltaTime;
		if (this.mPressed)
		{
			SpringPosition component = base.GetComponent<SpringPosition>();
			if (component != null)
			{
				component.enabled = false;
			}
			this.mScroll = 0f;
		}
		else
		{
			this.mMomentum += this.scale * (this.mScroll * 20f);
			this.mScroll = NGUIMath.SpringLerp(this.mScroll, 0f, 20f, deltaTime);
			if (this.mMomentum.magnitude > 0.01f)
			{
				this.mTrans.localPosition += (Vector3)NGUIMath.SpringDampen(ref this.mMomentum, 9f, deltaTime);
				this.mBounds = NGUIMath.CalculateAbsoluteWidgetBounds(this.rootForBounds);
				if (!this.ConstrainToBounds(this.dragEffect == UIDragObject.DragEffect.None))
				{
					SpringPosition component2 = base.GetComponent<SpringPosition>();
					if (component2 != null)
					{
						component2.enabled = false;
					}
				}
				return;
			}
			this.mScroll = 0f;
		}
		NGUIMath.SpringDampen(ref this.mMomentum, 9f, deltaTime);
	}

	// Token: 0x04000181 RID: 385
	public Transform rootForBounds;

	// Token: 0x04000182 RID: 386
	public Vector2 scale = Vector2.one;

	// Token: 0x04000183 RID: 387
	public float scrollWheelFactor;

	// Token: 0x04000184 RID: 388
	public UIDragObject.DragEffect dragEffect = UIDragObject.DragEffect.MomentumAndSpring;

	// Token: 0x04000185 RID: 389
	public bool smoothDragStart = true;

	// Token: 0x04000186 RID: 390
	public float momentumAmount = 35f;

	// Token: 0x04000187 RID: 391
	private Camera mCam;

	// Token: 0x04000188 RID: 392
	private Transform mTrans;

	// Token: 0x04000189 RID: 393
	private bool mPressed;

	// Token: 0x0400018A RID: 394
	private Vector2 mMomentum = Vector2.zero;

	// Token: 0x0400018B RID: 395
	private Bounds mBounds;

	// Token: 0x0400018C RID: 396
	private float mScroll;

	// Token: 0x0400018D RID: 397
	private UIRoot mRoot;

	// Token: 0x0400018E RID: 398
	private bool mDragStarted;
}
