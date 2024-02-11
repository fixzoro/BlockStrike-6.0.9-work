using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200004D RID: 77
[AddComponentMenu("NGUI/Interaction/Center Scroll View on Child")]
public class UICenterOnChild : MonoBehaviour
{
	// Token: 0x17000015 RID: 21
	// (get) Token: 0x06000221 RID: 545 RVA: 0x00006431 File Offset: 0x00004631
	public GameObject centeredObject
	{
		get
		{
			return this.mCenteredObject;
		}
	}

	// Token: 0x06000222 RID: 546 RVA: 0x00006439 File Offset: 0x00004639
	private void Start()
	{
		this.Recenter();
	}

	// Token: 0x06000223 RID: 547 RVA: 0x00006441 File Offset: 0x00004641
	private void OnEnable()
	{
		if (this.mScrollView)
		{
			this.mScrollView.centerOnChild = this;
			this.Recenter();
		}
	}

	// Token: 0x06000224 RID: 548 RVA: 0x00006465 File Offset: 0x00004665
	private void OnDisable()
	{
		if (this.mScrollView)
		{
			this.mScrollView.centerOnChild = null;
		}
	}

	// Token: 0x06000225 RID: 549 RVA: 0x00006483 File Offset: 0x00004683
	private void OnDragFinished()
	{
		if (base.enabled)
		{
			this.Recenter();
		}
	}

	// Token: 0x06000226 RID: 550 RVA: 0x00006496 File Offset: 0x00004696
	private void OnValidate()
	{
		this.nextPageThreshold = Mathf.Abs(this.nextPageThreshold);
	}

	// Token: 0x06000227 RID: 551 RVA: 0x00025BF0 File Offset: 0x00023DF0
	[ContextMenu("Execute")]
	public void Recenter()
	{
		if (this.mScrollView == null)
		{
			this.mScrollView = NGUITools.FindInParents<UIScrollView>(base.gameObject);
			if (this.mScrollView == null)
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					base.GetType(),
					" requires ",
					typeof(UIScrollView),
					" on a parent object in order to work"
				}), this);
				base.enabled = false;
				return;
			}
			if (this.mScrollView)
			{
				this.mScrollView.centerOnChild = this;
			}
			if (this.mScrollView.horizontalScrollBar != null)
			{
				UIProgressBar horizontalScrollBar = this.mScrollView.horizontalScrollBar;
				horizontalScrollBar.onDragFinished = (UIProgressBar.OnDragFinished)Delegate.Combine(horizontalScrollBar.onDragFinished, new UIProgressBar.OnDragFinished(this.OnDragFinished));
			}
			if (this.mScrollView.verticalScrollBar != null)
			{
				UIProgressBar verticalScrollBar = this.mScrollView.verticalScrollBar;
				verticalScrollBar.onDragFinished = (UIProgressBar.OnDragFinished)Delegate.Combine(verticalScrollBar.onDragFinished, new UIProgressBar.OnDragFinished(this.OnDragFinished));
			}
		}
		if (this.mScrollView.panel == null)
		{
			return;
		}
		Transform transform = base.transform;
		if (transform.childCount == 0)
		{
			return;
		}
		Vector3[] worldCorners = this.mScrollView.panel.worldCorners;
		Vector3 vector = (worldCorners[2] + worldCorners[0]) * 0.5f;
		Vector3 vector2 = this.mScrollView.currentMomentum * this.mScrollView.momentumAmount;
		Vector3 a = NGUIMath.SpringDampen(ref vector2, 9f, 2f);
		Vector3 b = vector - a * 0.01f;
		float num = float.MaxValue;
		Transform target = null;
		int index = 0;
		int num2 = 0;
		UIGrid component = base.GetComponent<UIGrid>();
		List<Transform> list = null;
		if (component != null)
		{
			list = component.GetChildList();
			int i = 0;
			int count = list.Count;
			int num3 = 0;
			while (i < count)
			{
				Transform transform2 = list[i];
				if (transform2.gameObject.activeInHierarchy)
				{
					float num4 = Vector3.SqrMagnitude(transform2.position - b);
					if (num4 < num)
					{
						num = num4;
						target = transform2;
						index = i;
						num2 = num3;
					}
					num3++;
				}
				i++;
			}
		}
		else
		{
			int j = 0;
			int childCount = transform.childCount;
			int num5 = 0;
			while (j < childCount)
			{
				Transform child = transform.GetChild(j);
				if (child.gameObject.activeInHierarchy)
				{
					float num6 = Vector3.SqrMagnitude(child.position - b);
					if (num6 < num)
					{
						num = num6;
						target = child;
						index = j;
						num2 = num5;
					}
					num5++;
				}
				j++;
			}
		}
		if (this.nextPageThreshold > 0f && UICamera.currentTouch != null && this.mCenteredObject != null && this.mCenteredObject.transform == ((list == null) ? transform.GetChild(index) : list[index]))
		{
			Vector3 point = UICamera.currentTouch.totalDelta;
			point = base.transform.rotation * point;
			UIScrollView.Movement movement = this.mScrollView.movement;
			float num7;
			if (movement != UIScrollView.Movement.Horizontal)
			{
				if (movement != UIScrollView.Movement.Vertical)
				{
					num7 = point.magnitude;
				}
				else
				{
					num7 = -point.y;
				}
			}
			else
			{
				num7 = point.x;
			}
			if (Mathf.Abs(num7) > this.nextPageThreshold)
			{
				if (num7 > this.nextPageThreshold)
				{
					if (list != null)
					{
						if (num2 > 0)
						{
							target = list[num2 - 1];
						}
						else
						{
							target = ((!(base.GetComponent<UIWrapContent>() == null)) ? list[list.Count - 1] : list[0]);
						}
					}
					else if (num2 > 0)
					{
						target = transform.GetChild(num2 - 1);
					}
					else
					{
						target = ((!(base.GetComponent<UIWrapContent>() == null)) ? transform.GetChild(transform.childCount - 1) : transform.GetChild(0));
					}
				}
				else if (num7 < -this.nextPageThreshold)
				{
					if (list != null)
					{
						if (num2 < list.Count - 1)
						{
							target = list[num2 + 1];
						}
						else
						{
							target = ((!(base.GetComponent<UIWrapContent>() == null)) ? list[0] : list[list.Count - 1]);
						}
					}
					else if (num2 < transform.childCount - 1)
					{
						target = transform.GetChild(num2 + 1);
					}
					else
					{
						target = ((!(base.GetComponent<UIWrapContent>() == null)) ? transform.GetChild(0) : transform.GetChild(transform.childCount - 1));
					}
				}
			}
		}
		this.CenterOn(target, vector);
	}

	// Token: 0x06000228 RID: 552 RVA: 0x00026130 File Offset: 0x00024330
	private void CenterOn(Transform target, Vector3 panelCenter)
	{
		if (target != null && this.mScrollView != null && this.mScrollView.panel != null)
		{
			Transform cachedTransform = this.mScrollView.panel.cachedTransform;
			this.mCenteredObject = target.gameObject;
			Vector3 a = cachedTransform.InverseTransformPoint(target.position);
			Vector3 b = cachedTransform.InverseTransformPoint(panelCenter);
			Vector3 b2 = a - b;
			if (!this.mScrollView.canMoveHorizontally)
			{
				b2.x = 0f;
			}
			if (!this.mScrollView.canMoveVertically)
			{
				b2.y = 0f;
			}
			b2.z = 0f;
			SpringPanel.Begin(this.mScrollView.panel.cachedGameObject, cachedTransform.localPosition - b2, this.springStrength).onFinished = this.onFinished;
		}
		else
		{
			this.mCenteredObject = null;
		}
		if (this.onCenter != null)
		{
			this.onCenter(this.mCenteredObject);
		}
	}

	// Token: 0x06000229 RID: 553 RVA: 0x00026248 File Offset: 0x00024448
	public void CenterOn(Transform target)
	{
		if (this.mScrollView != null && this.mScrollView.panel != null)
		{
			Vector3[] worldCorners = this.mScrollView.panel.worldCorners;
			Vector3 panelCenter = (worldCorners[2] + worldCorners[0]) * 0.5f;
			this.CenterOn(target, panelCenter);
		}
	}

	// Token: 0x04000135 RID: 309
	public float springStrength = 8f;

	// Token: 0x04000136 RID: 310
	public float nextPageThreshold;

	// Token: 0x04000137 RID: 311
	public SpringPanel.OnFinished onFinished;

	// Token: 0x04000138 RID: 312
	public UICenterOnChild.OnCenterCallback onCenter;

	// Token: 0x04000139 RID: 313
	private UIScrollView mScrollView;

	// Token: 0x0400013A RID: 314
	private GameObject mCenteredObject;

	// Token: 0x0200004E RID: 78
	// (Invoke) Token: 0x0600022B RID: 555
	public delegate void OnCenterCallback(GameObject centeredObject);
}
