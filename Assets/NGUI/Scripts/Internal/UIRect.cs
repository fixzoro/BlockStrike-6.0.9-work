using System;
using UnityEngine;

// Token: 0x020000C0 RID: 192
public abstract class UIRect : UIMonoBehaviour
{
	// Token: 0x17000095 RID: 149
	// (get) Token: 0x06000610 RID: 1552 RVA: 0x00008EF7 File Offset: 0x000070F7
	public GameObject cachedGameObject
	{
		get
		{
			if (this.mGo == null)
			{
				this.mGo = base.gameObject;
			}
			return this.mGo;
		}
	}

	// Token: 0x17000096 RID: 150
	// (get) Token: 0x06000611 RID: 1553 RVA: 0x00008F1C File Offset: 0x0000711C
	public Transform cachedTransform
	{
		get
		{
			if (this.mTrans == null)
			{
				this.mTrans = base.transform;
			}
			return this.mTrans;
		}
	}

	// Token: 0x17000097 RID: 151
	// (get) Token: 0x06000612 RID: 1554 RVA: 0x00008F41 File Offset: 0x00007141
	public Camera anchorCamera
	{
		get
		{
			if (!this.mCam || !this.mAnchorsCached)
			{
				this.ResetAnchors();
			}
			return this.mCam;
		}
	}

	// Token: 0x17000098 RID: 152
	// (get) Token: 0x06000613 RID: 1555 RVA: 0x000435E8 File Offset: 0x000417E8
	public bool isFullyAnchored
	{
		get
		{
			return this.leftAnchor.target && this.rightAnchor.target && this.topAnchor.target && this.bottomAnchor.target;
		}
	}

	// Token: 0x17000099 RID: 153
	// (get) Token: 0x06000614 RID: 1556 RVA: 0x00008F6A File Offset: 0x0000716A
	public virtual bool isAnchoredHorizontally
	{
		get
		{
			return this.leftAnchor.target || this.rightAnchor.target;
		}
	}

	// Token: 0x1700009A RID: 154
	// (get) Token: 0x06000615 RID: 1557 RVA: 0x00008F94 File Offset: 0x00007194
	public virtual bool isAnchoredVertically
	{
		get
		{
			return this.bottomAnchor.target || this.topAnchor.target;
		}
	}

	// Token: 0x1700009B RID: 155
	// (get) Token: 0x06000616 RID: 1558 RVA: 0x00007F30 File Offset: 0x00006130
	public virtual bool canBeAnchored
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700009C RID: 156
	// (get) Token: 0x06000617 RID: 1559 RVA: 0x00008FBE File Offset: 0x000071BE
	public UIRect parent
	{
		get
		{
			if (!this.mParentFound)
			{
				this.mParentFound = true;
				this.mParent = NGUITools.FindInParents<UIRect>(this.cachedTransform.parent);
			}
			return this.mParent;
		}
	}

	// Token: 0x1700009D RID: 157
	// (get) Token: 0x06000618 RID: 1560 RVA: 0x00043648 File Offset: 0x00041848
	public UIRoot root
	{
		get
		{
			if (this.parent != null)
			{
				return this.mParent.root;
			}
			if (!this.mRootSet)
			{
				this.mRootSet = true;
				this.mRoot = NGUITools.FindInParents<UIRoot>(this.cachedTransform);
			}
			return this.mRoot;
		}
	}

	// Token: 0x1700009E RID: 158
	// (get) Token: 0x06000619 RID: 1561 RVA: 0x0004369C File Offset: 0x0004189C
	public bool isAnchored
	{
		get
		{
			return (this.leftAnchor.target || this.rightAnchor.target || this.topAnchor.target || this.bottomAnchor.target) && this.canBeAnchored;
		}
	}

	// Token: 0x1700009F RID: 159
	// (get) Token: 0x0600061A RID: 1562
	// (set) Token: 0x0600061B RID: 1563
	public abstract float alpha { get; set; }

	// Token: 0x0600061C RID: 1564
	public abstract float CalculateFinalAlpha(int frameID);

	// Token: 0x170000A0 RID: 160
	// (get) Token: 0x0600061D RID: 1565
	public abstract Vector3[] localCorners { get; }

	// Token: 0x170000A1 RID: 161
	// (get) Token: 0x0600061E RID: 1566
	public abstract Vector3[] worldCorners { get; }

	// Token: 0x170000A2 RID: 162
	// (get) Token: 0x0600061F RID: 1567 RVA: 0x00043708 File Offset: 0x00041908
	protected float cameraRayDistance
	{
		get
		{
			if (this.anchorCamera == null)
			{
				return 0f;
			}
			if (!this.mCam.orthographic)
			{
				Transform cachedTransform = this.cachedTransform;
				Transform transform = this.mCam.transform;
				Plane plane = new Plane(cachedTransform.rotation * Vector3.back, cachedTransform.position);
				Ray ray = new Ray(transform.position, transform.rotation * Vector3.forward);
				float result;
				if (plane.Raycast(ray, out result))
				{
					return result;
				}
			}
			return Mathf.Lerp(this.mCam.nearClipPlane, this.mCam.farClipPlane, 0.5f);
		}
	}

	// Token: 0x06000620 RID: 1568 RVA: 0x000437BC File Offset: 0x000419BC
	public virtual void Invalidate(bool includeChildren)
	{
		this.mChanged = true;
		if (includeChildren)
		{
			for (int i = 0; i < this.mChildren.size; i++)
			{
				this.mChildren.buffer[i].Invalidate(true);
			}
		}
	}

	// Token: 0x06000621 RID: 1569 RVA: 0x00043808 File Offset: 0x00041A08
	public virtual Vector3[] GetSides(Transform relativeTo)
	{
		if (this.anchorCamera != null)
		{
			return this.mCam.GetSides(this.cameraRayDistance, relativeTo);
		}
		Vector3 position = this.cachedTransform.position;
		for (int i = 0; i < 4; i++)
		{
			UIRect.mSides[i] = position;
		}
		if (relativeTo != null)
		{
			for (int j = 0; j < 4; j++)
			{
				UIRect.mSides[j] = relativeTo.InverseTransformPoint(UIRect.mSides[j]);
			}
		}
		return UIRect.mSides;
	}

	// Token: 0x06000622 RID: 1570 RVA: 0x000438B4 File Offset: 0x00041AB4
	protected Vector3 GetLocalPos(UIRect.AnchorPoint ac, Transform trans)
	{
		if (ac.targetCam == null)
		{
			this.FindCameraFor(ac);
		}
		if (this.anchorCamera == null || ac.targetCam == null)
		{
			return this.cachedTransform.localPosition;
		}
		Rect rect = ac.targetCam.rect;
		Vector3 vector = ac.targetCam.WorldToViewportPoint(ac.target.position);
		Vector3 vector2 = new Vector3(vector.x * rect.width + rect.x, vector.y * rect.height + rect.y, vector.z);
		vector2 = this.mCam.ViewportToWorldPoint(vector2);
		if (trans != null)
		{
			vector2 = trans.InverseTransformPoint(vector2);
		}
		vector2.x = Mathf.Floor(vector2.x + 0.5f);
		vector2.y = Mathf.Floor(vector2.y + 0.5f);
		return vector2;
	}


    // Token: 0x06000623 RID: 1571 RVA: 0x000439BC File Offset: 0x00041BBC
    protected virtual void OnEnable()
	{
		this.widgetUpdateFrame = true;
		this.mEnabled = true;
		this.mUpdateFrame = -1;
		if (this.updateAnchors == UIRect.AnchorUpdate.OnEnable)
		{
			this.mAnchorsCached = false;
			this.mUpdateAnchors = true;
		}
		if (this.mStarted)
		{
			this.OnInit();
		}
		this.mUpdateFrame = -1;
	}

	// Token: 0x06000624 RID: 1572 RVA: 0x00008FEE File Offset: 0x000071EE
	protected virtual void OnInit()
	{
		this.mChanged = true;
		this.mRootSet = false;
		this.mParentFound = false;
		if (this.parent != null)
		{
			this.mParent.mChildren.Add(this);
		}
	}

	// Token: 0x06000625 RID: 1573 RVA: 0x00043A10 File Offset: 0x00041C10
	protected virtual void OnDisable()
	{
		this.mEnabled = false;
		if (this.mParent)
		{
			this.mParent.mChildren.Remove(this);
		}
		this.mParent = null;
		this.mRoot = null;
		this.mRootSet = false;
		this.mParentFound = false;
	}

	// Token: 0x06000626 RID: 1574 RVA: 0x00009027 File Offset: 0x00007227
	protected virtual void Awake()
	{
		UIUpdateManager.AddItem(this);
		this.mStarted = false;
		this.mGo = base.gameObject;
		this.mTrans = base.transform;
	}

	// Token: 0x06000627 RID: 1575 RVA: 0x0000904E File Offset: 0x0000724E
	protected void Start()
	{
		this.mStarted = true;
		this.OnInit();
		this.OnStart();
	}

	// Token: 0x06000628 RID: 1576 RVA: 0x00043A64 File Offset: 0x00041C64
	public override void nUpdate(int frameCount)
	{
		if (!this.mEnabled)
		{
			return;
		}
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying)
        {
            this.widgetUpdateFrame = true;
        }
#endif
        if (this.widgetAreStatic && !this.widgetUpdateFrame && Application.isPlaying)
		{
			return;
		}
		if (!this.mCam)
		{
			nProfiler.BeginSample("if (!mCam)");
			this.ResetAndUpdateAnchors();
			this.mUpdateFrame = -1;
			nProfiler.EndSample();
		}
		else if (!this.mAnchorsCached)
		{
			nProfiler.BeginSample("else if (!mAnchorsCached)");
			this.ResetAnchors();
			nProfiler.EndSample();
		}
		if (this.mUpdateFrame != frameCount)
		{
			if (this.updateAnchors == UIRect.AnchorUpdate.OnUpdate || this.mUpdateAnchors)
			{
				this.UpdateAnchorsInternal(frameCount);
			}
			nProfiler.BeginSample("OnUpdate");
			this.OnUpdate();
			nProfiler.EndSample();
		}
	}

	// Token: 0x06000629 RID: 1577 RVA: 0x00043B30 File Offset: 0x00041D30
	protected void UpdateAnchorsInternal(int frame)
	{
		nProfiler.BeginSample("UIRect.UpdateAnchorsInternal");
		this.mUpdateFrame = frame;
		this.mUpdateAnchors = false;
		bool flag = false;
		nProfiler.BeginSample("leftAnchor.target");
		if (this.leftAnchor.target)
		{
			flag = true;
			if (this.leftAnchor.rect != null && this.leftAnchor.rect.mUpdateFrame != frame)
			{
				this.leftAnchor.rect.nUpdate(frame);
			}
		}
		nProfiler.EndSample();
		nProfiler.BeginSample("bottomAnchor.target");
		if (this.bottomAnchor.target)
		{
			flag = true;
			if (this.bottomAnchor.rect != null && this.bottomAnchor.rect.mUpdateFrame != frame)
			{
				this.bottomAnchor.rect.nUpdate(frame);
			}
		}
		nProfiler.EndSample();
		nProfiler.BeginSample("rightAnchor.target");
		if (this.rightAnchor.target)
		{
			flag = true;
			if (this.rightAnchor.rect != null && this.rightAnchor.rect.mUpdateFrame != frame)
			{
				this.rightAnchor.rect.nUpdate(frame);
			}
		}
		nProfiler.EndSample();
		nProfiler.BeginSample("topAnchor.target");
		if (this.topAnchor.target)
		{
			flag = true;
			if (this.topAnchor.rect != null && this.topAnchor.rect.mUpdateFrame != frame)
			{
				this.topAnchor.rect.nUpdate(frame);
			}
		}
		nProfiler.EndSample();
		nProfiler.BeginSample("if (anchored) OnAnchor();");
		if (flag)
		{
			this.OnAnchor();
		}
		nProfiler.EndSample();
		nProfiler.EndSample();
	}

	// Token: 0x0600062A RID: 1578 RVA: 0x00009063 File Offset: 0x00007263
	public void UpdateAnchors()
	{
		nProfiler.BeginSample("UIRect.UpdateAnchors");
		if (this.isAnchored)
		{
			this.mUpdateFrame = -1;
			this.mUpdateAnchors = true;
			this.UpdateAnchorsInternal(Time.frameCount);
		}
		nProfiler.EndSample();
	}

	// Token: 0x0600062B RID: 1579
	protected abstract void OnAnchor();

	// Token: 0x0600062C RID: 1580 RVA: 0x00009098 File Offset: 0x00007298
	public void SetAnchor(Transform t)
	{
		this.leftAnchor.target = t;
		this.rightAnchor.target = t;
		this.topAnchor.target = t;
		this.bottomAnchor.target = t;
		this.ResetAnchors();
		this.UpdateAnchors();
	}

	// Token: 0x0600062D RID: 1581 RVA: 0x00043D04 File Offset: 0x00041F04
	public void SetAnchor(GameObject go)
	{
		Transform target = (!(go != null)) ? null : go.transform;
		this.leftAnchor.target = target;
		this.rightAnchor.target = target;
		this.topAnchor.target = target;
		this.bottomAnchor.target = target;
		this.ResetAnchors();
		this.UpdateAnchors();
	}

	// Token: 0x0600062E RID: 1582 RVA: 0x00043D68 File Offset: 0x00041F68
	public void SetAnchor(GameObject go, int left, int bottom, int right, int top)
	{
		Transform target = (!(go != null)) ? null : go.transform;
		this.leftAnchor.target = target;
		this.rightAnchor.target = target;
		this.topAnchor.target = target;
		this.bottomAnchor.target = target;
		this.leftAnchor.relative = 0f;
		this.rightAnchor.relative = 1f;
		this.bottomAnchor.relative = 0f;
		this.topAnchor.relative = 1f;
		this.leftAnchor.absolute = left;
		this.rightAnchor.absolute = right;
		this.bottomAnchor.absolute = bottom;
		this.topAnchor.absolute = top;
		this.ResetAnchors();
		this.UpdateAnchors();
	}

	// Token: 0x0600062F RID: 1583 RVA: 0x00043E3C File Offset: 0x0004203C
	public void SetAnchor(GameObject go, float left, float bottom, float right, float top)
	{
		Transform target = (!(go != null)) ? null : go.transform;
		this.leftAnchor.target = target;
		this.rightAnchor.target = target;
		this.topAnchor.target = target;
		this.bottomAnchor.target = target;
		this.leftAnchor.relative = left;
		this.rightAnchor.relative = right;
		this.bottomAnchor.relative = bottom;
		this.topAnchor.relative = top;
		this.leftAnchor.absolute = 0;
		this.rightAnchor.absolute = 0;
		this.bottomAnchor.absolute = 0;
		this.topAnchor.absolute = 0;
		this.ResetAnchors();
		this.UpdateAnchors();
	}

	// Token: 0x06000630 RID: 1584 RVA: 0x00043F00 File Offset: 0x00042100
	public void SetAnchor(GameObject go, float left, int leftOffset, float bottom, int bottomOffset, float right, int rightOffset, float top, int topOffset)
	{
		Transform target = (!(go != null)) ? null : go.transform;
		this.leftAnchor.target = target;
		this.rightAnchor.target = target;
		this.topAnchor.target = target;
		this.bottomAnchor.target = target;
		this.leftAnchor.relative = left;
		this.rightAnchor.relative = right;
		this.bottomAnchor.relative = bottom;
		this.topAnchor.relative = top;
		this.leftAnchor.absolute = leftOffset;
		this.rightAnchor.absolute = rightOffset;
		this.bottomAnchor.absolute = bottomOffset;
		this.topAnchor.absolute = topOffset;
		this.ResetAnchors();
		this.UpdateAnchors();
	}

	// Token: 0x06000631 RID: 1585 RVA: 0x00043FC8 File Offset: 0x000421C8
	public void SetAnchor(float left, int leftOffset, float bottom, int bottomOffset, float right, int rightOffset, float top, int topOffset)
	{
		Transform parent = this.cachedTransform.parent;
		this.leftAnchor.target = parent;
		this.rightAnchor.target = parent;
		this.topAnchor.target = parent;
		this.bottomAnchor.target = parent;
		this.leftAnchor.relative = left;
		this.rightAnchor.relative = right;
		this.bottomAnchor.relative = bottom;
		this.topAnchor.relative = top;
		this.leftAnchor.absolute = leftOffset;
		this.rightAnchor.absolute = rightOffset;
		this.bottomAnchor.absolute = bottomOffset;
		this.topAnchor.absolute = topOffset;
		this.ResetAnchors();
		this.UpdateAnchors();
	}

	// Token: 0x06000632 RID: 1586 RVA: 0x00044084 File Offset: 0x00042284
	public void SetScreenRect(int left, int top, int width, int height)
	{
		this.SetAnchor(0f, left, 1f, -top - height, 0f, left + width, 1f, -top);
	}

	// Token: 0x06000633 RID: 1587 RVA: 0x000440B8 File Offset: 0x000422B8
	public void ResetAnchors()
	{
		this.mAnchorsCached = true;
		this.leftAnchor.rect = ((!this.leftAnchor.target) ? null : this.leftAnchor.target.GetComponent<UIRect>());
		this.bottomAnchor.rect = ((!this.bottomAnchor.target) ? null : this.bottomAnchor.target.GetComponent<UIRect>());
		this.rightAnchor.rect = ((!this.rightAnchor.target) ? null : this.rightAnchor.target.GetComponent<UIRect>());
		this.topAnchor.rect = ((!this.topAnchor.target) ? null : this.topAnchor.target.GetComponent<UIRect>());
		this.mCam = NGUITools.FindCameraForLayer(this.cachedGameObject.layer);
		this.FindCameraFor(this.leftAnchor);
		this.FindCameraFor(this.bottomAnchor);
		this.FindCameraFor(this.rightAnchor);
		this.FindCameraFor(this.topAnchor);
		this.mUpdateAnchors = true;
	}

	// Token: 0x06000634 RID: 1588 RVA: 0x000090D6 File Offset: 0x000072D6
	public void ResetAndUpdateAnchors()
	{
		this.ResetAnchors();
		this.UpdateAnchors();
	}

	// Token: 0x06000635 RID: 1589
	public abstract void SetRect(float x, float y, float width, float height);

	// Token: 0x06000636 RID: 1590 RVA: 0x000441F4 File Offset: 0x000423F4
	private void FindCameraFor(UIRect.AnchorPoint ap)
	{
		if (ap.target == null || ap.rect != null)
		{
			ap.targetCam = null;
		}
		else
		{
			ap.targetCam = NGUITools.FindCameraForLayer(ap.target.gameObject.layer);
		}
	}

	// Token: 0x06000637 RID: 1591 RVA: 0x0004424C File Offset: 0x0004244C
	public virtual void ParentHasChanged()
	{
		this.mParentFound = false;
		UIRect y = NGUITools.FindInParents<UIRect>(this.cachedTransform.parent);
		if (this.mParent != y)
		{
			if (this.mParent)
			{
				this.mParent.mChildren.Remove(this);
			}
			this.mParent = y;
			if (this.mParent)
			{
				this.mParent.mChildren.Add(this);
			}
			this.mRootSet = false;
		}
	}

	// Token: 0x06000638 RID: 1592
	protected abstract void OnStart();

	// Token: 0x06000639 RID: 1593 RVA: 0x0000574F File Offset: 0x0000394F
	protected virtual void OnUpdate()
	{
	}

#if UNITY_EDITOR
    /// <summary>
    /// This callback is sent inside the editor notifying us that some property has changed.
    /// </summary>

    protected virtual void OnValidate()
    {
        if (mEnabled && NGUITools.GetActive(this))
        {
            if (!Application.isPlaying) ResetAnchors();
            Invalidate(true);
        }
    }
#endif

    // Token: 0x04000442 RID: 1090
    public bool widgetAreStatic;

	// Token: 0x04000443 RID: 1091
	public bool widgetUpdateFrame = true;

	// Token: 0x04000444 RID: 1092
	public UIRect.AnchorPoint leftAnchor = new UIRect.AnchorPoint();

	// Token: 0x04000445 RID: 1093
	public UIRect.AnchorPoint rightAnchor = new UIRect.AnchorPoint(1f);

	// Token: 0x04000446 RID: 1094
	public UIRect.AnchorPoint bottomAnchor = new UIRect.AnchorPoint();

	// Token: 0x04000447 RID: 1095
	public UIRect.AnchorPoint topAnchor = new UIRect.AnchorPoint(1f);

	// Token: 0x04000448 RID: 1096
	public UIRect.AnchorUpdate updateAnchors;

	// Token: 0x04000449 RID: 1097
	[NonSerialized]
	protected GameObject mGo;

	// Token: 0x0400044A RID: 1098
	[NonSerialized]
	protected Transform mTrans;

	// Token: 0x0400044B RID: 1099
	[NonSerialized]
	protected BetterList<UIRect> mChildren = new BetterList<UIRect>();

	// Token: 0x0400044C RID: 1100
	[NonSerialized]
	protected bool mChanged = true;

	// Token: 0x0400044D RID: 1101
	[NonSerialized]
	protected bool mParentFound;

	// Token: 0x0400044E RID: 1102
	[NonSerialized]
	private bool mUpdateAnchors = true;

	// Token: 0x0400044F RID: 1103
	[NonSerialized]
	private int mUpdateFrame = -1;

	// Token: 0x04000450 RID: 1104
	[NonSerialized]
	private bool mAnchorsCached;

	// Token: 0x04000451 RID: 1105
	[NonSerialized]
	private UIRoot mRoot;

	// Token: 0x04000452 RID: 1106
	[NonSerialized]
	private UIRect mParent;

	// Token: 0x04000453 RID: 1107
	[NonSerialized]
	private bool mRootSet;

	// Token: 0x04000454 RID: 1108
	[NonSerialized]
	protected Camera mCam;

	// Token: 0x04000455 RID: 1109
	protected bool mStarted;

	// Token: 0x04000456 RID: 1110
	[NonSerialized]
	public float finalAlpha = 1f;

	// Token: 0x04000457 RID: 1111
	protected static Vector3[] mSides = new Vector3[4];

	// Token: 0x04000458 RID: 1112
	public bool mEnabled;

	// Token: 0x020000C1 RID: 193
	[Serializable]
	public class AnchorPoint
	{
		// Token: 0x0600063A RID: 1594 RVA: 0x00004734 File Offset: 0x00002934
		public AnchorPoint()
		{
		}

		// Token: 0x0600063B RID: 1595 RVA: 0x000090E4 File Offset: 0x000072E4
		public AnchorPoint(float relative)
		{
			this.relative = relative;
		}

		// Token: 0x0600063C RID: 1596 RVA: 0x000090F3 File Offset: 0x000072F3
		public void Set(float relative, float absolute)
		{
			this.relative = relative;
			this.absolute = Mathf.FloorToInt(absolute + 0.5f);
		}

		// Token: 0x0600063D RID: 1597 RVA: 0x0000910E File Offset: 0x0000730E
		public void Set(Transform target, float relative, float absolute)
		{
			this.target = target;
			this.relative = relative;
			this.absolute = Mathf.FloorToInt(absolute + 0.5f);
		}

		// Token: 0x0600063E RID: 1598 RVA: 0x00009130 File Offset: 0x00007330
		public void SetToNearest(float abs0, float abs1, float abs2)
		{
			this.SetToNearest(0f, 0.5f, 1f, abs0, abs1, abs2);
		}

		// Token: 0x0600063F RID: 1599 RVA: 0x000442D4 File Offset: 0x000424D4
		public void SetToNearest(float rel0, float rel1, float rel2, float abs0, float abs1, float abs2)
		{
			float num = Mathf.Abs(abs0);
			float num2 = Mathf.Abs(abs1);
			float num3 = Mathf.Abs(abs2);
			if (num < num2 && num < num3)
			{
				this.Set(rel0, abs0);
			}
			else if (num2 < num && num2 < num3)
			{
				this.Set(rel1, abs1);
			}
			else
			{
				this.Set(rel2, abs2);
			}
		}

		// Token: 0x06000640 RID: 1600 RVA: 0x0004433C File Offset: 0x0004253C
		public void SetHorizontal(Transform parent, float localPos)
		{
			if (this.rect)
			{
				Vector3[] sides = this.rect.GetSides(parent);
				float num = Mathf.Lerp(sides[0].x, sides[2].x, this.relative);
				this.absolute = Mathf.FloorToInt(localPos - num + 0.5f);
			}
			else
			{
				Vector3 position = this.target.position;
				if (parent != null)
				{
					position = parent.InverseTransformPoint(position);
				}
				this.absolute = Mathf.FloorToInt(localPos - position.x + 0.5f);
			}
		}

		// Token: 0x06000641 RID: 1601 RVA: 0x000443E0 File Offset: 0x000425E0
		public void SetVertical(Transform parent, float localPos)
		{
			if (this.rect)
			{
				Vector3[] sides = this.rect.GetSides(parent);
				float num = Mathf.Lerp(sides[3].y, sides[1].y, this.relative);
				this.absolute = Mathf.FloorToInt(localPos - num + 0.5f);
			}
			else
			{
				Vector3 position = this.target.position;
				if (parent != null)
				{
					position = parent.InverseTransformPoint(position);
				}
				this.absolute = Mathf.FloorToInt(localPos - position.y + 0.5f);
			}
		}

		// Token: 0x06000642 RID: 1602 RVA: 0x00044484 File Offset: 0x00042684
		public Vector3[] GetSides(Transform relativeTo)
		{
			if (this.target != null)
			{
				if (this.rect != null)
				{
					return this.rect.GetSides(relativeTo);
				}
				if (this.target.GetComponent<Camera>() != null)
				{
					return this.target.GetComponent<Camera>().GetSides(relativeTo);
				}
			}
			return null;
		}

		// Token: 0x04000459 RID: 1113
		public Transform target;

		// Token: 0x0400045A RID: 1114
		public float relative;

		// Token: 0x0400045B RID: 1115
		public int absolute;

		// Token: 0x0400045C RID: 1116
		[NonSerialized]
		public UIRect rect;

		// Token: 0x0400045D RID: 1117
		[NonSerialized]
		public Camera targetCam;
	}

	// Token: 0x020000C2 RID: 194
	[DoNotObfuscateNGUI]
	public enum AnchorUpdate
	{
		// Token: 0x0400045F RID: 1119
		OnEnable,
		// Token: 0x04000460 RID: 1120
		OnUpdate,
		// Token: 0x04000461 RID: 1121
		OnStart
	}
}
