using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200005C RID: 92
[AddComponentMenu("NGUI/Interaction/Grid")]
public class UIGrid : UIWidgetContainer
{
	// Token: 0x17000019 RID: 25
	// (set) Token: 0x0600028F RID: 655 RVA: 0x00006B31 File Offset: 0x00004D31
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

	// Token: 0x06000290 RID: 656 RVA: 0x00028468 File Offset: 0x00026668
	public List<Transform> GetChildList()
	{
		Transform transform = base.transform;
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if ((!this.hideInactive || (child && child.gameObject.activeSelf)) && !UIDragDropItem.IsDragged(child.gameObject))
			{
				list.Add(child);
			}
		}
		if (this.sorting != UIGrid.Sorting.None && this.arrangement != UIGrid.Arrangement.CellSnap)
		{
			if (this.sorting == UIGrid.Sorting.Alphabetic)
			{
				list.Sort(new Comparison<Transform>(UIGrid.SortByName));
			}
			else if (this.sorting == UIGrid.Sorting.Horizontal)
			{
				list.Sort(new Comparison<Transform>(UIGrid.SortHorizontal));
			}
			else if (this.sorting == UIGrid.Sorting.Vertical)
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

	// Token: 0x06000291 RID: 657 RVA: 0x00028584 File Offset: 0x00026784
	public Transform GetChild(int index)
	{
		List<Transform> childList = this.GetChildList();
		return (index >= childList.Count) ? null : childList[index];
	}

	// Token: 0x06000292 RID: 658 RVA: 0x00006B47 File Offset: 0x00004D47
	public int GetIndex(Transform trans)
	{
		return this.GetChildList().IndexOf(trans);
	}

	// Token: 0x06000293 RID: 659 RVA: 0x00006B55 File Offset: 0x00004D55
	[Obsolete("Use gameObject.AddChild or transform.parent = gridTransform")]
	public void AddChild(Transform trans)
	{
		if (trans != null)
		{
			trans.parent = base.transform;
			this.ResetPosition(this.GetChildList());
		}
	}

	// Token: 0x06000294 RID: 660 RVA: 0x00006B55 File Offset: 0x00004D55
	[Obsolete("Use gameObject.AddChild or transform.parent = gridTransform")]
	public void AddChild(Transform trans, bool sort)
	{
		if (trans != null)
		{
			trans.parent = base.transform;
			this.ResetPosition(this.GetChildList());
		}
	}

	// Token: 0x06000295 RID: 661 RVA: 0x000285B4 File Offset: 0x000267B4
	public bool RemoveChild(Transform t)
	{
		List<Transform> childList = this.GetChildList();
		if (childList.Remove(t))
		{
			this.ResetPosition(childList);
			return true;
		}
		return false;
	}

	// Token: 0x06000296 RID: 662 RVA: 0x00006B7B File Offset: 0x00004D7B
	protected virtual void Init()
	{
		this.mInitDone = true;
		this.mPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
	}

	// Token: 0x06000297 RID: 663 RVA: 0x000285E0 File Offset: 0x000267E0
	protected virtual void Start()
	{
		if (!this.mInitDone)
		{
			this.Init();
		}
		bool flag = this.animateSmoothly;
		this.animateSmoothly = false;
		this.Reposition();
		this.animateSmoothly = flag;
		base.enabled = false;
	}

	// Token: 0x06000298 RID: 664 RVA: 0x00006B95 File Offset: 0x00004D95
	protected virtual void Update()
	{
		this.Reposition();
		base.enabled = false;
	}

	// Token: 0x06000299 RID: 665 RVA: 0x00006BA4 File Offset: 0x00004DA4
	private void OnValidate()
	{
		if (!Application.isPlaying && NGUITools.GetActive(this))
		{
			this.Reposition();
		}
	}

	// Token: 0x0600029A RID: 666 RVA: 0x00006BC1 File Offset: 0x00004DC1
	public static int SortByName(Transform a, Transform b)
	{
		return string.Compare(a.name, b.name);
	}

	// Token: 0x0600029B RID: 667 RVA: 0x00028620 File Offset: 0x00026820
	public static int SortHorizontal(Transform a, Transform b)
	{
		return a.localPosition.x.CompareTo(b.localPosition.x);
	}

	// Token: 0x0600029C RID: 668 RVA: 0x00028650 File Offset: 0x00026850
	public static int SortVertical(Transform a, Transform b)
	{
		return b.localPosition.y.CompareTo(a.localPosition.y);
	}

	// Token: 0x0600029D RID: 669 RVA: 0x0000574F File Offset: 0x0000394F
	protected virtual void Sort(List<Transform> list)
	{
	}

	// Token: 0x0600029E RID: 670 RVA: 0x00028680 File Offset: 0x00026880
	[ContextMenu("Execute")]
	public virtual void Reposition()
	{
		if (Application.isPlaying && !this.mInitDone && NGUITools.GetActive(base.gameObject))
		{
			this.Init();
		}
		if (this.sorted)
		{
			this.sorted = false;
			if (this.sorting == UIGrid.Sorting.None)
			{
				this.sorting = UIGrid.Sorting.Alphabetic;
			}
			NGUITools.SetDirty(this, "last change");
		}
		List<Transform> childList = this.GetChildList();
		this.ResetPosition(childList);
		if (this.keepWithinPanel)
		{
			this.ConstrainWithinPanel();
		}
		if (this.onReposition != null)
		{
			this.onReposition();
		}
	}

	// Token: 0x0600029F RID: 671 RVA: 0x0002871C File Offset: 0x0002691C
	public void ConstrainWithinPanel()
	{
		if (this.mPanel != null)
		{
			this.mPanel.ConstrainTargetToBounds(base.transform, true);
			UIScrollView component = this.mPanel.GetComponent<UIScrollView>();
			if (component != null)
			{
				component.UpdateScrollbars(true);
			}
		}
	}

	// Token: 0x060002A0 RID: 672 RVA: 0x0002876C File Offset: 0x0002696C
	protected virtual void ResetPosition(List<Transform> list)
	{
		this.mReposition = false;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int i = 0;
		int count = list.Count;
		while (i < count)
		{
			Transform transform = list[i];
			Vector3 vector = transform.localPosition;
			float z = vector.z;
			if (this.arrangement == UIGrid.Arrangement.CellSnap)
			{
				if (this.cellWidth > 0f)
				{
					vector.x = Mathf.Round(vector.x / this.cellWidth) * this.cellWidth;
				}
				if (this.cellHeight > 0f)
				{
					vector.y = Mathf.Round(vector.y / this.cellHeight) * this.cellHeight;
				}
			}
			else
			{
				vector = ((this.arrangement != UIGrid.Arrangement.Horizontal) ? new Vector3(this.cellWidth * (float)num2, -this.cellHeight * (float)num, z) : new Vector3(this.cellWidth * (float)num, -this.cellHeight * (float)num2, z));
			}
			if (this.animateSmoothly && Application.isPlaying && (this.pivot != UIWidget.Pivot.TopLeft || Vector3.SqrMagnitude(transform.localPosition - vector) >= 0.0001f))
			{
				SpringPosition springPosition = SpringPosition.Begin(transform.gameObject, vector, 15f);
				springPosition.updateScrollView = true;
				springPosition.ignoreTimeScale = true;
			}
			else
			{
				transform.localPosition = vector;
			}
			num3 = Mathf.Max(num3, num);
			num4 = Mathf.Max(num4, num2);
			if (++num >= this.maxPerLine && this.maxPerLine > 0)
			{
				num = 0;
				num2++;
			}
			i++;
		}
		if (this.pivot != UIWidget.Pivot.TopLeft)
		{
			Vector2 pivotOffset = NGUIMath.GetPivotOffset(this.pivot);
			float num5;
			float num6;
			if (this.arrangement == UIGrid.Arrangement.Horizontal)
			{
				num5 = Mathf.Lerp(0f, (float)num3 * this.cellWidth, pivotOffset.x);
				num6 = Mathf.Lerp((float)(-(float)num4) * this.cellHeight, 0f, pivotOffset.y);
			}
			else
			{
				num5 = Mathf.Lerp(0f, (float)num4 * this.cellWidth, pivotOffset.x);
				num6 = Mathf.Lerp((float)(-(float)num3) * this.cellHeight, 0f, pivotOffset.y);
			}
			foreach (Transform transform2 in list)
			{
				SpringPosition component = transform2.GetComponent<SpringPosition>();
				if (component != null)
				{
					component.enabled = false;
					SpringPosition springPosition2 = component;
					springPosition2.target.x = springPosition2.target.x - num5;
					SpringPosition springPosition3 = component;
					springPosition3.target.y = springPosition3.target.y - num6;
					component.enabled = true;
				}
				else
				{
					Vector3 localPosition = transform2.localPosition;
					localPosition.x -= num5;
					localPosition.y -= num6;
					transform2.localPosition = localPosition;
				}
			}
		}
	}

	// Token: 0x040001A7 RID: 423
	public UIGrid.Arrangement arrangement;

	// Token: 0x040001A8 RID: 424
	public UIGrid.Sorting sorting;

	// Token: 0x040001A9 RID: 425
	public UIWidget.Pivot pivot;

	// Token: 0x040001AA RID: 426
	public int maxPerLine;

	// Token: 0x040001AB RID: 427
	public float cellWidth = 200f;

	// Token: 0x040001AC RID: 428
	public float cellHeight = 200f;

	// Token: 0x040001AD RID: 429
	public bool animateSmoothly;

	// Token: 0x040001AE RID: 430
	public bool hideInactive;

	// Token: 0x040001AF RID: 431
	public bool keepWithinPanel;

	// Token: 0x040001B0 RID: 432
	public UIGrid.OnReposition onReposition;

	// Token: 0x040001B1 RID: 433
	public Comparison<Transform> onCustomSort;

	// Token: 0x040001B2 RID: 434
	[SerializeField]
	[HideInInspector]
	private bool sorted;

	// Token: 0x040001B3 RID: 435
	protected bool mReposition;

	// Token: 0x040001B4 RID: 436
	protected UIPanel mPanel;

	// Token: 0x040001B5 RID: 437
	protected bool mInitDone;

	// Token: 0x0200005D RID: 93
	[DoNotObfuscateNGUI]
	public enum Arrangement
	{
		// Token: 0x040001B7 RID: 439
		Horizontal,
		// Token: 0x040001B8 RID: 440
		Vertical,
		// Token: 0x040001B9 RID: 441
		CellSnap
	}

	// Token: 0x0200005E RID: 94
	[DoNotObfuscateNGUI]
	public enum Sorting
	{
		// Token: 0x040001BB RID: 443
		None,
		// Token: 0x040001BC RID: 444
		Alphabetic,
		// Token: 0x040001BD RID: 445
		Horizontal,
		// Token: 0x040001BE RID: 446
		Vertical,
		// Token: 0x040001BF RID: 447
		Custom
	}

	// Token: 0x0200005F RID: 95
	// (Invoke) Token: 0x060002A2 RID: 674
	public delegate void OnReposition();
}
