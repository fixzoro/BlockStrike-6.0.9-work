using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000089 RID: 137
[AddComponentMenu("NGUI/Interaction/Wrap Content")]
public class UIWrapContent : MonoBehaviour
{
	// Token: 0x060003D3 RID: 979 RVA: 0x00030448 File Offset: 0x0002E648
	protected virtual void Start()
	{
		this.SortBasedOnScrollMovement();
		this.WrapContent();
		if (this.mScroll != null)
		{
			this.mScroll.GetComponent<UIPanel>().onClipMove = new UIPanel.OnClippingMoved(this.OnMove);
		}
		this.mFirstTime = false;
	}

	// Token: 0x060003D4 RID: 980 RVA: 0x0000791D File Offset: 0x00005B1D
	protected virtual void OnMove(UIPanel panel)
	{
		this.WrapContent();
	}

	// Token: 0x060003D5 RID: 981 RVA: 0x00030498 File Offset: 0x0002E698
	[ContextMenu("Sort Based on Scroll Movement")]
	public virtual void SortBasedOnScrollMovement()
	{
		if (!this.CacheScrollView())
		{
			return;
		}
		this.mChildren.Clear();
		for (int i = 0; i < this.mTrans.childCount; i++)
		{
			Transform child = this.mTrans.GetChild(i);
			if (!this.hideInactive || child.gameObject.activeInHierarchy)
			{
				this.mChildren.Add(child);
			}
		}
		if (this.mHorizontal)
		{
			this.mChildren.Sort(new Comparison<Transform>(UIGrid.SortHorizontal));
		}
		else
		{
			this.mChildren.Sort(new Comparison<Transform>(UIGrid.SortVertical));
		}
		this.ResetChildPositions();
	}

	// Token: 0x060003D6 RID: 982 RVA: 0x00030558 File Offset: 0x0002E758
	[ContextMenu("Sort Alphabetically")]
	public virtual void SortAlphabetically()
	{
		if (!this.CacheScrollView())
		{
			return;
		}
		this.mChildren.Clear();
		for (int i = 0; i < this.mTrans.childCount; i++)
		{
			Transform child = this.mTrans.GetChild(i);
			if (!this.hideInactive || child.gameObject.activeInHierarchy)
			{
				this.mChildren.Add(child);
			}
		}
		this.mChildren.Sort(new Comparison<Transform>(UIGrid.SortByName));
		this.ResetChildPositions();
	}

	// Token: 0x060003D7 RID: 983 RVA: 0x000305F0 File Offset: 0x0002E7F0
	protected bool CacheScrollView()
	{
		this.mTrans = base.transform;
		this.mPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
		this.mScroll = this.mPanel.GetComponent<UIScrollView>();
		if (this.mScroll == null)
		{
			return false;
		}
		if (this.mScroll.movement == UIScrollView.Movement.Horizontal)
		{
			this.mHorizontal = true;
		}
		else
		{
			if (this.mScroll.movement != UIScrollView.Movement.Vertical)
			{
				return false;
			}
			this.mHorizontal = false;
		}
		return true;
	}

	// Token: 0x060003D8 RID: 984 RVA: 0x0003067C File Offset: 0x0002E87C
	protected virtual void ResetChildPositions()
	{
		int i = 0;
		int count = this.mChildren.Count;
		while (i < count)
		{
			Transform transform = this.mChildren[i];
			transform.localPosition = ((!this.mHorizontal) ? new Vector3(0f, (float)(-(float)i * this.itemSize), 0f) : new Vector3((float)(i * this.itemSize), 0f, 0f));
			this.UpdateItem(transform, i);
			i++;
		}
	}

	// Token: 0x060003D9 RID: 985 RVA: 0x00030704 File Offset: 0x0002E904
	public virtual void WrapContent()
	{
		float num = (float)(this.itemSize * this.mChildren.Count) * 0.5f;
		Vector3[] worldCorners = this.mPanel.worldCorners;
		for (int i = 0; i < 4; i++)
		{
			Vector3 vector = worldCorners[i];
			vector = this.mTrans.InverseTransformPoint(vector);
			worldCorners[i] = vector;
		}
		Vector3 vector2 = Vector3.Lerp(worldCorners[0], worldCorners[2], 0.5f);
		bool flag = true;
		float num2 = num * 2f;
		if (this.mHorizontal)
		{
			float num3 = worldCorners[0].x - (float)this.itemSize;
			float num4 = worldCorners[2].x + (float)this.itemSize;
			int j = 0;
			int count = this.mChildren.Count;
			while (j < count)
			{
				Transform transform = this.mChildren[j];
				float num5 = transform.localPosition.x - vector2.x;
				if (num5 < -num)
				{
					Vector3 localPosition = transform.localPosition;
					localPosition.x += num2;
					num5 = localPosition.x - vector2.x;
					int num6 = Mathf.RoundToInt(localPosition.x / (float)this.itemSize);
					if (this.minIndex == this.maxIndex || (this.minIndex <= num6 && num6 <= this.maxIndex))
					{
						transform.localPosition = localPosition;
						this.UpdateItem(transform, j);
					}
					else
					{
						flag = false;
					}
				}
				else if (num5 > num)
				{
					Vector3 localPosition2 = transform.localPosition;
					localPosition2.x -= num2;
					num5 = localPosition2.x - vector2.x;
					int num7 = Mathf.RoundToInt(localPosition2.x / (float)this.itemSize);
					if (this.minIndex == this.maxIndex || (this.minIndex <= num7 && num7 <= this.maxIndex))
					{
						transform.localPosition = localPosition2;
						this.UpdateItem(transform, j);
					}
					else
					{
						flag = false;
					}
				}
				else if (this.mFirstTime)
				{
					this.UpdateItem(transform, j);
				}
				if (this.cullContent)
				{
					num5 += this.mPanel.clipOffset.x - this.mTrans.localPosition.x;
					if (!UICamera.IsPressed(transform.gameObject))
					{
						NGUITools.SetActive(transform.gameObject, num5 > num3 && num5 < num4, false);
					}
				}
				j++;
			}
		}
		else
		{
			float num8 = worldCorners[0].y - (float)this.itemSize;
			float num9 = worldCorners[2].y + (float)this.itemSize;
			int k = 0;
			int count2 = this.mChildren.Count;
			while (k < count2)
			{
				Transform transform2 = this.mChildren[k];
				float num10 = transform2.localPosition.y - vector2.y;
				if (num10 < -num)
				{
					Vector3 localPosition3 = transform2.localPosition;
					localPosition3.y += num2;
					num10 = localPosition3.y - vector2.y;
					int num11 = Mathf.RoundToInt(localPosition3.y / (float)this.itemSize);
					if (this.minIndex == this.maxIndex || (this.minIndex <= num11 && num11 <= this.maxIndex))
					{
						transform2.localPosition = localPosition3;
						this.UpdateItem(transform2, k);
					}
					else
					{
						flag = false;
					}
				}
				else if (num10 > num)
				{
					Vector3 localPosition4 = transform2.localPosition;
					localPosition4.y -= num2;
					num10 = localPosition4.y - vector2.y;
					int num12 = Mathf.RoundToInt(localPosition4.y / (float)this.itemSize);
					if (this.minIndex == this.maxIndex || (this.minIndex <= num12 && num12 <= this.maxIndex))
					{
						transform2.localPosition = localPosition4;
						this.UpdateItem(transform2, k);
					}
					else
					{
						flag = false;
					}
				}
				else if (this.mFirstTime)
				{
					this.UpdateItem(transform2, k);
				}
				if (this.cullContent)
				{
					num10 += this.mPanel.clipOffset.y - this.mTrans.localPosition.y;
					if (!UICamera.IsPressed(transform2.gameObject))
					{
						NGUITools.SetActive(transform2.gameObject, num10 > num8 && num10 < num9, false);
					}
				}
				k++;
			}
		}
		this.mScroll.restrictWithinPanel = !flag;
		this.mScroll.InvalidateBounds();
	}

	// Token: 0x060003DA RID: 986 RVA: 0x00007925 File Offset: 0x00005B25
	private void OnValidate()
	{
		if (this.maxIndex < this.minIndex)
		{
			this.maxIndex = this.minIndex;
		}
		if (this.minIndex > this.maxIndex)
		{
			this.maxIndex = this.minIndex;
		}
	}

	// Token: 0x060003DB RID: 987 RVA: 0x00030C08 File Offset: 0x0002EE08
	protected virtual void UpdateItem(Transform item, int index)
	{
		if (this.onInitializeItem != null)
		{
			int realIndex = (this.mScroll.movement != UIScrollView.Movement.Vertical) ? Mathf.RoundToInt(item.localPosition.x / (float)this.itemSize) : Mathf.RoundToInt(item.localPosition.y / (float)this.itemSize);
			this.onInitializeItem(item.gameObject, index, realIndex);
		}
	}

	// Token: 0x040002F1 RID: 753
	public int itemSize = 100;

	// Token: 0x040002F2 RID: 754
	public bool cullContent = true;

	// Token: 0x040002F3 RID: 755
	public int minIndex;

	// Token: 0x040002F4 RID: 756
	public int maxIndex;

	// Token: 0x040002F5 RID: 757
	public bool hideInactive;

	// Token: 0x040002F6 RID: 758
	public UIWrapContent.OnInitializeItem onInitializeItem;

	// Token: 0x040002F7 RID: 759
	protected Transform mTrans;

	// Token: 0x040002F8 RID: 760
	protected UIPanel mPanel;

	// Token: 0x040002F9 RID: 761
	protected UIScrollView mScroll;

	// Token: 0x040002FA RID: 762
	protected bool mHorizontal;

	// Token: 0x040002FB RID: 763
	protected bool mFirstTime = true;

	// Token: 0x040002FC RID: 764
	protected List<Transform> mChildren = new List<Transform>();

	// Token: 0x0200008A RID: 138
	// (Invoke) Token: 0x060003DD RID: 989
	public delegate void OnInitializeItem(GameObject go, int wrapIndex, int realIndex);
}
