using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000071 RID: 113
[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/NGUI Progress Bar")]
public class UIProgressBar : UIWidgetContainer
{
	// Token: 0x1700002F RID: 47
	// (get) Token: 0x06000342 RID: 834 RVA: 0x00007313 File Offset: 0x00005513
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

	// Token: 0x17000030 RID: 48
	// (get) Token: 0x06000343 RID: 835 RVA: 0x00007338 File Offset: 0x00005538
	public Camera cachedCamera
	{
		get
		{
			if (this.mCam == null)
			{
				this.mCam = NGUITools.FindCameraForLayer(base.gameObject.layer);
			}
			return this.mCam;
		}
	}

	// Token: 0x17000031 RID: 49
	// (get) Token: 0x06000344 RID: 836 RVA: 0x00007367 File Offset: 0x00005567
	// (set) Token: 0x06000345 RID: 837 RVA: 0x0000736F File Offset: 0x0000556F
	public UIWidget foregroundWidget
	{
		get
		{
			return this.mFG;
		}
		set
		{
			if (this.mFG != value)
			{
				this.mFG = value;
				this.mIsDirty = true;
			}
		}
	}

	// Token: 0x17000032 RID: 50
	// (get) Token: 0x06000346 RID: 838 RVA: 0x00007390 File Offset: 0x00005590
	// (set) Token: 0x06000347 RID: 839 RVA: 0x00007398 File Offset: 0x00005598
	public UIWidget backgroundWidget
	{
		get
		{
			return this.mBG;
		}
		set
		{
			if (this.mBG != value)
			{
				this.mBG = value;
				this.mIsDirty = true;
			}
		}
	}

	// Token: 0x17000033 RID: 51
	// (get) Token: 0x06000348 RID: 840 RVA: 0x000073B9 File Offset: 0x000055B9
	// (set) Token: 0x06000349 RID: 841 RVA: 0x000073C1 File Offset: 0x000055C1
	public UIProgressBar.FillDirection fillDirection
	{
		get
		{
			return this.mFill;
		}
		set
		{
			if (this.mFill != value)
			{
				this.mFill = value;
				if (this.mStarted)
				{
					this.ForceUpdate();
				}
			}
		}
	}

	// Token: 0x17000034 RID: 52
	// (get) Token: 0x0600034A RID: 842 RVA: 0x000073E7 File Offset: 0x000055E7
	// (set) Token: 0x0600034B RID: 843 RVA: 0x0000741B File Offset: 0x0000561B
	public float value
	{
		get
		{
			if (this.numberOfSteps > 1)
			{
				return Mathf.Round(this.mValue * (float)(this.numberOfSteps - 1)) / (float)(this.numberOfSteps - 1);
			}
			return this.mValue;
		}
		set
		{
			this.Set(value, true);
		}
	}

	// Token: 0x17000035 RID: 53
	// (get) Token: 0x0600034C RID: 844 RVA: 0x0002C3E8 File Offset: 0x0002A5E8
	// (set) Token: 0x0600034D RID: 845 RVA: 0x0002C434 File Offset: 0x0002A634
	public float alpha
	{
		get
		{
			if (this.mFG != null)
			{
				return this.mFG.alpha;
			}
			if (this.mBG != null)
			{
				return this.mBG.alpha;
			}
			return 1f;
		}
		set
		{
			if (this.mFG != null)
			{
				this.mFG.alpha = value;
				if (this.mFG.GetComponent<Collider>() != null)
				{
					this.mFG.GetComponent<Collider>().enabled = (this.mFG.alpha > 0.001f);
				}
				else if (this.mFG.GetComponent<Collider2D>() != null)
				{
					this.mFG.GetComponent<Collider2D>().enabled = (this.mFG.alpha > 0.001f);
				}
			}
			if (this.mBG != null)
			{
				this.mBG.alpha = value;
				if (this.mBG.GetComponent<Collider>() != null)
				{
					this.mBG.GetComponent<Collider>().enabled = (this.mBG.alpha > 0.001f);
				}
				else if (this.mBG.GetComponent<Collider2D>() != null)
				{
					this.mBG.GetComponent<Collider2D>().enabled = (this.mBG.alpha > 0.001f);
				}
			}
			if (this.thumb != null)
			{
				UIWidget component = this.thumb.GetComponent<UIWidget>();
				if (component != null)
				{
					component.alpha = value;
					if (component.GetComponent<Collider>() != null)
					{
						component.GetComponent<Collider>().enabled = (component.alpha > 0.001f);
					}
					else if (component.GetComponent<Collider2D>() != null)
					{
						component.GetComponent<Collider2D>().enabled = (component.alpha > 0.001f);
					}
				}
			}
		}
	}

	// Token: 0x17000036 RID: 54
	// (get) Token: 0x0600034E RID: 846 RVA: 0x00007425 File Offset: 0x00005625
	protected bool isHorizontal
	{
		get
		{
			return this.mFill == UIProgressBar.FillDirection.LeftToRight || this.mFill == UIProgressBar.FillDirection.RightToLeft;
		}
	}

	// Token: 0x17000037 RID: 55
	// (get) Token: 0x0600034F RID: 847 RVA: 0x0000743E File Offset: 0x0000563E
	protected bool isInverted
	{
		get
		{
			return this.mFill == UIProgressBar.FillDirection.RightToLeft || this.mFill == UIProgressBar.FillDirection.TopToBottom;
		}
	}

	// Token: 0x06000350 RID: 848 RVA: 0x0002C5EC File Offset: 0x0002A7EC
	public void Set(float val, bool notify = true)
	{
		val = Mathf.Clamp01(val);
		if (this.mValue != val)
		{
			float value = this.value;
			this.mValue = val;
			if (this.mStarted && value != this.value)
			{
				if (notify && NGUITools.GetActive(this) && EventDelegate.IsValid(this.onChange))
				{
					UIProgressBar.current = this;
					EventDelegate.Execute(this.onChange);
					UIProgressBar.current = null;
				}
				this.ForceUpdate();
			}
		}
	}

	// Token: 0x06000351 RID: 849 RVA: 0x0002C670 File Offset: 0x0002A870
	public void Start()
	{
		if (this.mStarted)
		{
			return;
		}
		this.mStarted = true;
		this.Upgrade();
		if (Application.isPlaying)
		{
			if (this.mBG != null)
			{
				this.mBG.autoResizeBoxCollider = true;
			}
			this.OnStart();
			if (UIProgressBar.current == null && this.onChange != null)
			{
				UIProgressBar.current = this;
				EventDelegate.Execute(this.onChange);
				UIProgressBar.current = null;
			}
		}
		this.ForceUpdate();
	}

	// Token: 0x06000352 RID: 850 RVA: 0x0000574F File Offset: 0x0000394F
	protected virtual void Upgrade()
	{
	}

	// Token: 0x06000353 RID: 851 RVA: 0x0000574F File Offset: 0x0000394F
	protected virtual void OnStart()
	{
	}

	// Token: 0x06000354 RID: 852 RVA: 0x00007458 File Offset: 0x00005658
	protected void Update()
	{
		if (this.mIsDirty)
		{
			this.ForceUpdate();
		}
	}

	// Token: 0x06000355 RID: 853 RVA: 0x0002C6FC File Offset: 0x0002A8FC
	protected void OnValidate()
	{
		if (NGUITools.GetActive(this))
		{
			this.Upgrade();
			this.mIsDirty = true;
			float num = Mathf.Clamp01(this.mValue);
			if (this.mValue != num)
			{
				this.mValue = num;
			}
			if (this.numberOfSteps < 0)
			{
				this.numberOfSteps = 0;
			}
			else if (this.numberOfSteps > 21)
			{
				this.numberOfSteps = 21;
			}
			this.ForceUpdate();
		}
		else
		{
			float num2 = Mathf.Clamp01(this.mValue);
			if (this.mValue != num2)
			{
				this.mValue = num2;
			}
			if (this.numberOfSteps < 0)
			{
				this.numberOfSteps = 0;
			}
			else if (this.numberOfSteps > 21)
			{
				this.numberOfSteps = 21;
			}
		}
	}

	// Token: 0x06000356 RID: 854 RVA: 0x0002C7C4 File Offset: 0x0002A9C4
	protected float ScreenToValue(Vector2 screenPos)
	{
		Transform cachedTransform = this.cachedTransform;
		Plane plane = new Plane(cachedTransform.rotation * Vector3.back, cachedTransform.position);
		Ray ray = this.cachedCamera.ScreenPointToRay(screenPos);
		float distance;
		if (!plane.Raycast(ray, out distance))
		{
			return this.value;
		}
		return this.LocalToValue(cachedTransform.InverseTransformPoint(ray.GetPoint(distance)));
	}

	// Token: 0x06000357 RID: 855 RVA: 0x0002C838 File Offset: 0x0002AA38
	protected virtual float LocalToValue(Vector2 localPos)
	{
		if (!(this.mFG != null))
		{
			return this.value;
		}
		Vector3[] localCorners = this.mFG.localCorners;
		Vector3 vector = localCorners[2] - localCorners[0];
		if (this.isHorizontal)
		{
			float num = (localPos.x - localCorners[0].x) / vector.x;
			return (!this.isInverted) ? num : (1f - num);
		}
		float num2 = (localPos.y - localCorners[0].y) / vector.y;
		return (!this.isInverted) ? num2 : (1f - num2);
	}

	// Token: 0x06000358 RID: 856 RVA: 0x0002C900 File Offset: 0x0002AB00
	public virtual void ForceUpdate()
	{
		this.mIsDirty = false;
		bool flag = false;
		if (this.mFG != null)
		{
			UIBasicSprite uibasicSprite = this.mFG as UIBasicSprite;
			if (this.isHorizontal)
			{
				if (uibasicSprite != null && uibasicSprite.type == UIBasicSprite.Type.Filled)
				{
					if (uibasicSprite.fillDirection == UIBasicSprite.FillDirection.Horizontal || uibasicSprite.fillDirection == UIBasicSprite.FillDirection.Vertical)
					{
						uibasicSprite.fillDirection = UIBasicSprite.FillDirection.Horizontal;
						uibasicSprite.invert = this.isInverted;
					}
					uibasicSprite.fillAmount = this.value;
				}
				else
				{
					this.mFG.drawRegion = ((!this.isInverted) ? new Vector4(0f, 0f, this.value, 1f) : new Vector4(1f - this.value, 0f, 1f, 1f));
					this.mFG.enabled = true;
					flag = (this.value < 0.001f);
				}
			}
			else if (uibasicSprite != null && uibasicSprite.type == UIBasicSprite.Type.Filled)
			{
				if (uibasicSprite.fillDirection == UIBasicSprite.FillDirection.Horizontal || uibasicSprite.fillDirection == UIBasicSprite.FillDirection.Vertical)
				{
					uibasicSprite.fillDirection = UIBasicSprite.FillDirection.Vertical;
					uibasicSprite.invert = this.isInverted;
				}
				uibasicSprite.fillAmount = this.value;
			}
			else
			{
				this.mFG.drawRegion = ((!this.isInverted) ? new Vector4(0f, 0f, 1f, this.value) : new Vector4(0f, 1f - this.value, 1f, 1f));
				this.mFG.enabled = true;
				flag = (this.value < 0.001f);
			}
		}
		if (this.thumb != null && (this.mFG != null || this.mBG != null))
		{
			Vector3[] array = (!(this.mFG != null)) ? this.mBG.localCorners : this.mFG.localCorners;
			Vector4 vector = (!(this.mFG != null)) ? this.mBG.border : this.mFG.border;
			Vector3[] array2 = array;
			int num = 0;
			array2[num].x = array2[num].x + vector.x;
			Vector3[] array3 = array;
			int num2 = 1;
			array3[num2].x = array3[num2].x + vector.x;
			Vector3[] array4 = array;
			int num3 = 2;
			array4[num3].x = array4[num3].x - vector.z;
			Vector3[] array5 = array;
			int num4 = 3;
			array5[num4].x = array5[num4].x - vector.z;
			Vector3[] array6 = array;
			int num5 = 0;
			array6[num5].y = array6[num5].y + vector.y;
			Vector3[] array7 = array;
			int num6 = 1;
			array7[num6].y = array7[num6].y - vector.w;
			Vector3[] array8 = array;
			int num7 = 2;
			array8[num7].y = array8[num7].y - vector.w;
			Vector3[] array9 = array;
			int num8 = 3;
			array9[num8].y = array9[num8].y + vector.y;
			Transform transform = (!(this.mFG != null)) ? this.mBG.cachedTransform : this.mFG.cachedTransform;
			for (int i = 0; i < 4; i++)
			{
				array[i] = transform.TransformPoint(array[i]);
			}
			if (this.isHorizontal)
			{
				Vector3 from = Vector3.Lerp(array[0], array[1], 0.5f);
				Vector3 to = Vector3.Lerp(array[2], array[3], 0.5f);
				this.SetThumbPosition(Vector3.Lerp(from, to, (!this.isInverted) ? this.value : (1f - this.value)));
			}
			else
			{
				Vector3 from2 = Vector3.Lerp(array[0], array[3], 0.5f);
				Vector3 to2 = Vector3.Lerp(array[1], array[2], 0.5f);
				this.SetThumbPosition(Vector3.Lerp(from2, to2, (!this.isInverted) ? this.value : (1f - this.value)));
			}
		}
		if (flag)
		{
			this.mFG.enabled = false;
		}
	}

	// Token: 0x06000359 RID: 857 RVA: 0x0002CDA0 File Offset: 0x0002AFA0
	protected void SetThumbPosition(Vector3 worldPos)
	{
		Transform parent = this.thumb.parent;
		if (parent != null)
		{
			worldPos = parent.InverseTransformPoint(worldPos);
			worldPos.x = Mathf.Round(worldPos.x);
			worldPos.y = Mathf.Round(worldPos.y);
			worldPos.z = 0f;
			if (Vector3.Distance(this.thumb.localPosition, worldPos) > 0.001f)
			{
				this.thumb.localPosition = worldPos;
			}
		}
		else if (Vector3.Distance(this.thumb.position, worldPos) > 1E-05f)
		{
			this.thumb.position = worldPos;
		}
	}

	// Token: 0x0600035A RID: 858 RVA: 0x0002CE54 File Offset: 0x0002B054
	public virtual void OnPan(Vector2 delta)
	{
		if (base.enabled)
		{
			switch (this.mFill)
			{
			case UIProgressBar.FillDirection.LeftToRight:
			{
				float value = Mathf.Clamp01(this.mValue + delta.x);
				this.value = value;
				this.mValue = value;
				break;
			}
			case UIProgressBar.FillDirection.RightToLeft:
			{
				float value2 = Mathf.Clamp01(this.mValue - delta.x);
				this.value = value2;
				this.mValue = value2;
				break;
			}
			case UIProgressBar.FillDirection.BottomToTop:
			{
				float value3 = Mathf.Clamp01(this.mValue + delta.y);
				this.value = value3;
				this.mValue = value3;
				break;
			}
			case UIProgressBar.FillDirection.TopToBottom:
			{
				float value4 = Mathf.Clamp01(this.mValue - delta.y);
				this.value = value4;
				this.mValue = value4;
				break;
			}
			}
		}
	}

	// Token: 0x04000261 RID: 609
	public static UIProgressBar current;

	// Token: 0x04000262 RID: 610
	public UIProgressBar.OnDragFinished onDragFinished;

	// Token: 0x04000263 RID: 611
	public Transform thumb;

	// Token: 0x04000264 RID: 612
	[SerializeField]
	[HideInInspector]
	protected UIWidget mBG;

	// Token: 0x04000265 RID: 613
	[HideInInspector]
	[SerializeField]
	protected UIWidget mFG;

	// Token: 0x04000266 RID: 614
	[SerializeField]
	[HideInInspector]
	protected float mValue = 1f;

	// Token: 0x04000267 RID: 615
	[SerializeField]
	[HideInInspector]
	protected UIProgressBar.FillDirection mFill;

	// Token: 0x04000268 RID: 616
	[NonSerialized]
	protected bool mStarted;

	// Token: 0x04000269 RID: 617
	[NonSerialized]
	protected Transform mTrans;

	// Token: 0x0400026A RID: 618
	[NonSerialized]
	protected bool mIsDirty;

	// Token: 0x0400026B RID: 619
	[NonSerialized]
	protected Camera mCam;

	// Token: 0x0400026C RID: 620
	[NonSerialized]
	protected float mOffset;

	// Token: 0x0400026D RID: 621
	public int numberOfSteps;

	// Token: 0x0400026E RID: 622
	public List<EventDelegate> onChange = new List<EventDelegate>();

	// Token: 0x02000072 RID: 114
	[DoNotObfuscateNGUI]
	public enum FillDirection
	{
		// Token: 0x04000270 RID: 624
		LeftToRight,
		// Token: 0x04000271 RID: 625
		RightToLeft,
		// Token: 0x04000272 RID: 626
		BottomToTop,
		// Token: 0x04000273 RID: 627
		TopToBottom
	}

	// Token: 0x02000073 RID: 115
	// (Invoke) Token: 0x0600035C RID: 860
	public delegate void OnDragFinished();
}
