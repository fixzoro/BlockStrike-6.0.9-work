using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000080 RID: 128
[AddComponentMenu("NGUI/Interaction/Table")]
public class UITable : UIWidgetContainer
{
	// Token: 0x17000047 RID: 71
	// (set) Token: 0x060003AC RID: 940 RVA: 0x00007794 File Offset: 0x00005994
	public bool repositionNow
	{
		set
		{
			if (value)
			{
				this.mReposition = true;
				base.enabled = true;
			}
		}
	}

	// Token: 0x060003AD RID: 941 RVA: 0x0002F468 File Offset: 0x0002D668
	public List<Transform> GetChildList()
	{
		Transform transform = base.transform;
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (!this.hideInactive || (child && NGUITools.GetActive(child.gameObject)))
			{
				list.Add(child);
			}
		}
		if (this.sorting != UITable.Sorting.None)
		{
			if (this.sorting == UITable.Sorting.Alphabetic)
			{
				list.Sort(new Comparison<Transform>(UIGrid.SortByName));
			}
			else if (this.sorting == UITable.Sorting.Horizontal)
			{
				list.Sort(new Comparison<Transform>(UIGrid.SortHorizontal));
			}
			else if (this.sorting == UITable.Sorting.Vertical)
			{
				list.Sort(new Comparison<Transform>(UIGrid.SortVertical));
			}
			else if (this.onCustomSort != null)
			{
				list.Sort(this.onCustomSort);
			}
			else
			{
				this.Sort(list);
			}
		}
		return list;
	}

	// Token: 0x060003AE RID: 942 RVA: 0x000077AA File Offset: 0x000059AA
	protected virtual void Sort(List<Transform> list)
	{
		list.Sort(new Comparison<Transform>(UIGrid.SortByName));
	}

	// Token: 0x060003AF RID: 943 RVA: 0x000077BE File Offset: 0x000059BE
	protected virtual void Start()
	{
		this.Init();
		this.Reposition();
		base.enabled = false;
	}

	// Token: 0x060003B0 RID: 944 RVA: 0x000077D3 File Offset: 0x000059D3
	protected virtual void Init()
	{
		this.mInitDone = true;
		this.mPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
	}

	// Token: 0x060003B1 RID: 945 RVA: 0x000077ED File Offset: 0x000059ED
	protected virtual void LateUpdate()
	{
		if (this.mReposition)
		{
			this.Reposition();
		}
		base.enabled = false;
	}

	// Token: 0x060003B2 RID: 946 RVA: 0x00007807 File Offset: 0x00005A07
	private void OnValidate()
	{
		if (!Application.isPlaying && NGUITools.GetActive(this))
		{
			this.Reposition();
		}
	}

	// Token: 0x060003B3 RID: 947 RVA: 0x0002F568 File Offset: 0x0002D768
	protected void RepositionVariableSize(List<Transform> children)
	{
		float num = 0f;
		float num2 = 0f;
		int num3 = (this.columns <= 0) ? 1 : (children.Count / this.columns + 1);
		int num4 = (this.columns <= 0) ? children.Count : this.columns;
		Bounds[,] array = new Bounds[num3, num4];
		Bounds[] array2 = new Bounds[num4];
		Bounds[] array3 = new Bounds[num3];
		int num5 = 0;
		int num6 = 0;
		int i = 0;
		int count = children.Count;
		while (i < count)
		{
			Transform transform = children[i];
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(transform, !this.hideInactive);
			Vector3 localScale = transform.localScale;
			bounds.min = Vector3.Scale(bounds.min, localScale);
			bounds.max = Vector3.Scale(bounds.max, localScale);
			array[num6, num5] = bounds;
			array2[num5].Encapsulate(bounds);
			array3[num6].Encapsulate(bounds);
			if (++num5 >= this.columns && this.columns > 0)
			{
				num5 = 0;
				num6++;
			}
			i++;
		}
		num5 = 0;
		num6 = 0;
		Vector2 pivotOffset = NGUIMath.GetPivotOffset(this.cellAlignment);
		int j = 0;
		int count2 = children.Count;
		while (j < count2)
		{
			Transform transform2 = children[j];
			Bounds bounds2 = array[num6, num5];
			Bounds bounds3 = array2[num5];
			Bounds bounds4 = array3[num6];
			Vector3 localPosition = transform2.localPosition;
			localPosition.x = num + bounds2.extents.x - bounds2.center.x;
			localPosition.x -= Mathf.Lerp(0f, bounds2.max.x - bounds2.min.x - bounds3.max.x + bounds3.min.x, pivotOffset.x) - this.padding.x;
			if (this.direction == UITable.Direction.Down)
			{
				localPosition.y = -num2 - bounds2.extents.y - bounds2.center.y;
				localPosition.y += Mathf.Lerp(bounds2.max.y - bounds2.min.y - bounds4.max.y + bounds4.min.y, 0f, pivotOffset.y) - this.padding.y;
			}
			else
			{
				localPosition.y = num2 + bounds2.extents.y - bounds2.center.y;
				localPosition.y -= Mathf.Lerp(0f, bounds2.max.y - bounds2.min.y - bounds4.max.y + bounds4.min.y, pivotOffset.y) - this.padding.y;
			}
			num += bounds3.size.x + this.padding.x * 2f;
			transform2.localPosition = localPosition;
			if (++num5 >= this.columns && this.columns > 0)
			{
				num5 = 0;
				num6++;
				num = 0f;
				num2 += bounds4.size.y + this.padding.y * 2f;
			}
			j++;
		}
		if (this.pivot != UIWidget.Pivot.TopLeft)
		{
			pivotOffset = NGUIMath.GetPivotOffset(this.pivot);
			Bounds bounds5 = NGUIMath.CalculateRelativeWidgetBounds(base.transform);
			float num7 = Mathf.Lerp(0f, bounds5.size.x, pivotOffset.x);
			float num8 = Mathf.Lerp(-bounds5.size.y, 0f, pivotOffset.y);
			Transform transform3 = base.transform;
			for (int k = 0; k < transform3.childCount; k++)
			{
				Transform child = transform3.GetChild(k);
				SpringPosition component = child.GetComponent<SpringPosition>();
				if (component != null)
				{
					component.enabled = false;
					SpringPosition springPosition = component;
					springPosition.target.x = springPosition.target.x - num7;
					SpringPosition springPosition2 = component;
					springPosition2.target.y = springPosition2.target.y - num8;
					component.enabled = true;
				}
				else
				{
					Vector3 localPosition2 = child.localPosition;
					localPosition2.x -= num7;
					localPosition2.y -= num8;
					child.localPosition = localPosition2;
				}
			}
		}
	}

	// Token: 0x060003B4 RID: 948 RVA: 0x0002FA90 File Offset: 0x0002DC90
	[ContextMenu("Execute")]
	public virtual void Reposition()
	{
		if (Application.isPlaying && !this.mInitDone && NGUITools.GetActive(this))
		{
			this.Init();
		}
		this.mReposition = false;
		Transform transform = base.transform;
		List<Transform> childList = this.GetChildList();
		if (childList.Count > 0)
		{
			this.RepositionVariableSize(childList);
		}
		if (this.keepWithinPanel && this.mPanel != null)
		{
			this.mPanel.ConstrainTargetToBounds(transform, true);
			UIScrollView component = this.mPanel.GetComponent<UIScrollView>();
			if (component != null)
			{
				component.UpdateScrollbars(true);
			}
		}
		if (this.onReposition != null)
		{
			this.onReposition();
		}
	}

	// Token: 0x040002BE RID: 702
	public int columns;

	// Token: 0x040002BF RID: 703
	public UITable.Direction direction;

	// Token: 0x040002C0 RID: 704
	public UITable.Sorting sorting;

	// Token: 0x040002C1 RID: 705
	public UIWidget.Pivot pivot;

	// Token: 0x040002C2 RID: 706
	public UIWidget.Pivot cellAlignment;

	// Token: 0x040002C3 RID: 707
	public bool hideInactive = true;

	// Token: 0x040002C4 RID: 708
	public bool keepWithinPanel;

	// Token: 0x040002C5 RID: 709
	public Vector2 padding = Vector2.zero;

	// Token: 0x040002C6 RID: 710
	public UITable.OnReposition onReposition;

	// Token: 0x040002C7 RID: 711
	public Comparison<Transform> onCustomSort;

	// Token: 0x040002C8 RID: 712
	protected UIPanel mPanel;

	// Token: 0x040002C9 RID: 713
	protected bool mInitDone;

	// Token: 0x040002CA RID: 714
	protected bool mReposition;

	// Token: 0x02000081 RID: 129
	[DoNotObfuscateNGUI]
	public enum Direction
	{
		// Token: 0x040002CC RID: 716
		Down,
		// Token: 0x040002CD RID: 717
		Up
	}

	// Token: 0x02000082 RID: 130
	[DoNotObfuscateNGUI]
	public enum Sorting
	{
		// Token: 0x040002CF RID: 719
		None,
		// Token: 0x040002D0 RID: 720
		Alphabetic,
		// Token: 0x040002D1 RID: 721
		Horizontal,
		// Token: 0x040002D2 RID: 722
		Vertical,
		// Token: 0x040002D3 RID: 723
		Custom
	}

	// Token: 0x02000083 RID: 131
	// (Invoke) Token: 0x060003B6 RID: 950
	public delegate void OnReposition();
}
