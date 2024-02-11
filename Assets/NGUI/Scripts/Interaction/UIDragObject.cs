using System;
using UnityEngine;

// Token: 0x02000055 RID: 85
[AddComponentMenu("NGUI/Interaction/Drag Object")]
[ExecuteInEditMode]
public class UIDragObject : MonoBehaviour
{
	// Token: 0x17000016 RID: 22
	// (get) Token: 0x06000254 RID: 596 RVA: 0x0000665C File Offset: 0x0000485C
	// (set) Token: 0x06000255 RID: 597 RVA: 0x00006664 File Offset: 0x00004864
	public Vector3 dragMovement
	{
		get
		{
			return this.scale;
		}
		set
		{
			this.scale = value;
		}
	}

	// Token: 0x06000256 RID: 598 RVA: 0x00026F84 File Offset: 0x00025184
	private void OnEnable()
	{
		if (this.scrollWheelFactor != 0f)
		{
			this.scrollMomentum = this.scale * this.scrollWheelFactor;
			this.scrollWheelFactor = 0f;
		}
		if (this.contentRect == null && this.target != null && Application.isPlaying)
		{
			UIWidget component = this.target.GetComponent<UIWidget>();
			if (component != null)
			{
				this.contentRect = component;
			}
		}
		this.mTargetPos = ((!(this.target != null)) ? Vector3.zero : this.target.position);
	}

	// Token: 0x06000257 RID: 599 RVA: 0x0000666D File Offset: 0x0000486D
	private void OnDisable()
	{
		this.mStarted = false;
	}

	// Token: 0x06000258 RID: 600 RVA: 0x0002703C File Offset: 0x0002523C
	private void FindPanel()
	{
		this.panelRegion = ((!(this.target != null)) ? null : UIPanel.Find(this.target.transform.parent));
		if (this.panelRegion == null)
		{
			this.restrictWithinPanel = false;
		}
	}

	// Token: 0x06000259 RID: 601 RVA: 0x00027094 File Offset: 0x00025294
	private void UpdateBounds()
	{
		if (this.contentRect)
		{
			Transform cachedTransform = this.panelRegion.cachedTransform;
			Matrix4x4 worldToLocalMatrix = cachedTransform.worldToLocalMatrix;
			Vector3[] worldCorners = this.contentRect.worldCorners;
			for (int i = 0; i < 4; i++)
			{
				worldCorners[i] = worldToLocalMatrix.MultiplyPoint3x4(worldCorners[i]);
			}
			this.mBounds = new Bounds(worldCorners[0], Vector3.zero);
			for (int j = 1; j < 4; j++)
			{
				this.mBounds.Encapsulate(worldCorners[j]);
			}
		}
		else
		{
			this.mBounds = NGUIMath.CalculateRelativeWidgetBounds(this.panelRegion.cachedTransform, this.target);
		}
	}

	// Token: 0x0600025A RID: 602 RVA: 0x0002716C File Offset: 0x0002536C
	private void OnPress(bool pressed)
	{
		if (UICamera.currentTouchID == -2 || UICamera.currentTouchID == -3)
		{
			return;
		}
		float timeScale = Time.timeScale;
		if (timeScale < 0.01f && timeScale != 0f)
		{
			return;
		}
		if (base.enabled && NGUITools.GetActive(base.gameObject) && this.target != null)
		{
			if (pressed)
			{
				if (!this.mPressed)
				{
					this.mTouchID = UICamera.currentTouchID;
					this.mPressed = true;
					this.mStarted = false;
					this.CancelMovement();
					if (this.restrictWithinPanel && this.panelRegion == null)
					{
						this.FindPanel();
					}
					if (this.restrictWithinPanel)
					{
						this.UpdateBounds();
					}
					this.CancelSpring();
					Transform transform = UICamera.currentCamera.transform;
					this.mPlane = new Plane(((!(this.panelRegion != null)) ? transform.rotation : this.panelRegion.cachedTransform.rotation) * Vector3.back, UICamera.lastWorldPosition);
				}
			}
			else if (this.mPressed && this.mTouchID == UICamera.currentTouchID)
			{
				this.mPressed = false;
				if (this.restrictWithinPanel && this.dragEffect == UIDragObject.DragEffect.MomentumAndSpring && this.panelRegion.ConstrainTargetToBounds(this.target, ref this.mBounds, false))
				{
					this.CancelMovement();
				}
			}
		}
	}

	// Token: 0x0600025B RID: 603 RVA: 0x000272F8 File Offset: 0x000254F8
	private void OnDrag(Vector2 delta)
	{
		if (this.mPressed && this.mTouchID == UICamera.currentTouchID && base.enabled && NGUITools.GetActive(base.gameObject) && this.target != null)
		{
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
			Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);
			float distance = 0f;
			if (this.mPlane.Raycast(ray, out distance))
			{
				Vector3 point = ray.GetPoint(distance);
				Vector3 vector = point - this.mLastPos;
				this.mLastPos = point;
				if (!this.mStarted)
				{
					this.mStarted = true;
					vector = Vector3.zero;
				}
				if (vector.x != 0f || vector.y != 0f)
				{
					vector = this.target.InverseTransformDirection(vector);
					vector.Scale(this.scale);
					vector = this.target.TransformDirection(vector);
				}
				if (this.dragEffect != UIDragObject.DragEffect.None)
				{
					this.mMomentum = Vector3.Lerp(this.mMomentum, this.mMomentum + vector * (0.01f * this.momentumAmount), 0.67f);
				}
				Vector3 localPosition = this.target.localPosition;
				this.Move(vector);
				if (this.restrictWithinPanel)
				{
					this.mBounds.center = this.mBounds.center + (this.target.localPosition - localPosition);
					if (this.dragEffect != UIDragObject.DragEffect.MomentumAndSpring && this.panelRegion.ConstrainTargetToBounds(this.target, ref this.mBounds, true))
					{
						this.CancelMovement();
					}
				}
			}
		}
	}

	// Token: 0x0600025C RID: 604 RVA: 0x000274C4 File Offset: 0x000256C4
	private void Move(Vector3 worldDelta)
	{
		if (this.panelRegion != null)
		{
			this.mTargetPos += worldDelta;
			Transform parent = this.target.parent;
			Rigidbody component = this.target.GetComponent<Rigidbody>();
			if (parent != null)
			{
				Vector3 vector = parent.worldToLocalMatrix.MultiplyPoint3x4(this.mTargetPos);
				vector.x = Mathf.Round(vector.x);
				vector.y = Mathf.Round(vector.y);
				if (component != null)
				{
					vector = parent.localToWorldMatrix.MultiplyPoint3x4(vector);
					component.position = vector;
				}
				else
				{
					this.target.localPosition = vector;
				}
			}
			else if (component != null)
			{
				component.position = this.mTargetPos;
			}
			else
			{
				this.target.position = this.mTargetPos;
			}
			UIScrollView component2 = this.panelRegion.GetComponent<UIScrollView>();
			if (component2 != null)
			{
				component2.UpdateScrollbars(true);
			}
		}
		else
		{
			this.target.position += worldDelta;
		}
	}

	// Token: 0x0600025D RID: 605 RVA: 0x000275F8 File Offset: 0x000257F8
	private void LateUpdate()
	{
		if (this.target == null)
		{
			return;
		}
		float deltaTime = RealTime.deltaTime;
		this.mMomentum -= this.mScroll;
		this.mScroll = NGUIMath.SpringLerp(this.mScroll, Vector3.zero, 20f, deltaTime);
		if (this.mMomentum.magnitude < 0.0001f)
		{
			return;
		}
		if (!this.mPressed)
		{
			if (this.panelRegion == null)
			{
				this.FindPanel();
			}
			this.Move(NGUIMath.SpringDampen(ref this.mMomentum, 9f, deltaTime));
			if (this.restrictWithinPanel && this.panelRegion != null)
			{
				this.UpdateBounds();
				if (this.panelRegion.ConstrainTargetToBounds(this.target, ref this.mBounds, this.dragEffect == UIDragObject.DragEffect.None))
				{
					this.CancelMovement();
				}
				else
				{
					this.CancelSpring();
				}
			}
			NGUIMath.SpringDampen(ref this.mMomentum, 9f, deltaTime);
			if (this.mMomentum.magnitude < 0.0001f)
			{
				this.CancelMovement();
			}
		}
		else
		{
			NGUIMath.SpringDampen(ref this.mMomentum, 9f, deltaTime);
		}
	}

	// Token: 0x0600025E RID: 606 RVA: 0x0002773C File Offset: 0x0002593C
	public void CancelMovement()
	{
		if (this.target != null)
		{
			Vector3 localPosition = this.target.localPosition;
			localPosition.x = (float)Mathf.RoundToInt(localPosition.x);
			localPosition.y = (float)Mathf.RoundToInt(localPosition.y);
			localPosition.z = (float)Mathf.RoundToInt(localPosition.z);
			this.target.localPosition = localPosition;
		}
		this.mTargetPos = ((!(this.target != null)) ? Vector3.zero : this.target.position);
		this.mMomentum = Vector3.zero;
		this.mScroll = Vector3.zero;
	}

	// Token: 0x0600025F RID: 607 RVA: 0x000277F0 File Offset: 0x000259F0
	public void CancelSpring()
	{
		SpringPosition component = this.target.GetComponent<SpringPosition>();
		if (component != null)
		{
			component.enabled = false;
		}
	}

	// Token: 0x06000260 RID: 608 RVA: 0x00006676 File Offset: 0x00004876
	private void OnScroll(float delta)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject))
		{
			this.mScroll -= this.scrollMomentum * (delta * 0.05f);
		}
	}

	// Token: 0x04000156 RID: 342
	public Transform target;

	// Token: 0x04000157 RID: 343
	public UIPanel panelRegion;

	// Token: 0x04000158 RID: 344
	public Vector3 scrollMomentum = Vector3.zero;

	// Token: 0x04000159 RID: 345
	public bool restrictWithinPanel;

	// Token: 0x0400015A RID: 346
	public UIRect contentRect;

	// Token: 0x0400015B RID: 347
	public UIDragObject.DragEffect dragEffect = UIDragObject.DragEffect.MomentumAndSpring;

	// Token: 0x0400015C RID: 348
	public float momentumAmount = 35f;

	// Token: 0x0400015D RID: 349
	[SerializeField]
	protected Vector3 scale = new Vector3(1f, 1f, 0f);

	// Token: 0x0400015E RID: 350
	[SerializeField]
	[HideInInspector]
	private float scrollWheelFactor;

	// Token: 0x0400015F RID: 351
	private Plane mPlane;

	// Token: 0x04000160 RID: 352
	private Vector3 mTargetPos;

	// Token: 0x04000161 RID: 353
	private Vector3 mLastPos;

	// Token: 0x04000162 RID: 354
	private Vector3 mMomentum = Vector3.zero;

	// Token: 0x04000163 RID: 355
	private Vector3 mScroll = Vector3.zero;

	// Token: 0x04000164 RID: 356
	private Bounds mBounds;

	// Token: 0x04000165 RID: 357
	private int mTouchID;

	// Token: 0x04000166 RID: 358
	private bool mStarted;

	// Token: 0x04000167 RID: 359
	private bool mPressed;

	// Token: 0x02000056 RID: 86
	[DoNotObfuscateNGUI]
	public enum DragEffect
	{
		// Token: 0x04000169 RID: 361
		None,
		// Token: 0x0400016A RID: 362
		Momentum,
		// Token: 0x0400016B RID: 363
		MomentumAndSpring
	}
}
