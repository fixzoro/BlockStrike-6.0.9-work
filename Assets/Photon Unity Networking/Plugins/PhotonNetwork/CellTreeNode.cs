using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002EF RID: 751
public class CellTreeNode
{
	// Token: 0x06001CAC RID: 7340 RVA: 0x00004734 File Offset: 0x00002934
	public CellTreeNode()
	{
	}

	// Token: 0x06001CAD RID: 7341 RVA: 0x00014AC9 File Offset: 0x00012CC9
	public CellTreeNode(byte id, CellTreeNode.ENodeType nodeType, CellTreeNode parent)
	{
		this.Id = id;
		this.NodeType = nodeType;
		this.Parent = parent;
	}

	// Token: 0x06001CAE RID: 7342 RVA: 0x00014AE6 File Offset: 0x00012CE6
	public void AddChild(CellTreeNode child)
	{
		if (this.Childs == null)
		{
			this.Childs = new List<CellTreeNode>(1);
		}
		this.Childs.Add(child);
	}

	// Token: 0x06001CAF RID: 7343 RVA: 0x0000574F File Offset: 0x0000394F
	public void Draw()
	{
	}

	// Token: 0x06001CB0 RID: 7344 RVA: 0x000B4B3C File Offset: 0x000B2D3C
	public void GetActiveCells(List<byte> activeCells, bool yIsUpAxis, Vector3 position)
	{
		if (this.NodeType != CellTreeNode.ENodeType.Leaf)
		{
			foreach (CellTreeNode cellTreeNode in this.Childs)
			{
				cellTreeNode.GetActiveCells(activeCells, yIsUpAxis, position);
			}
		}
		else if (this.IsPointNearCell(yIsUpAxis, position))
		{
			if (this.IsPointInsideCell(yIsUpAxis, position))
			{
				activeCells.Insert(0, this.Id);
				for (CellTreeNode parent = this.Parent; parent != null; parent = parent.Parent)
				{
					activeCells.Insert(0, parent.Id);
				}
			}
			else
			{
				activeCells.Add(this.Id);
			}
		}
	}

	// Token: 0x06001CB1 RID: 7345 RVA: 0x000B4C08 File Offset: 0x000B2E08
	public bool IsPointInsideCell(bool yIsUpAxis, Vector3 point)
	{
		if (point.x < this.TopLeft.x || point.x > this.BottomRight.x)
		{
			return false;
		}
		if (yIsUpAxis)
		{
			if (point.y >= this.TopLeft.y && point.y <= this.BottomRight.y)
			{
				return true;
			}
		}
		else if (point.z >= this.TopLeft.z && point.z <= this.BottomRight.z)
		{
			return true;
		}
		return false;
	}

	// Token: 0x06001CB2 RID: 7346 RVA: 0x000B4CB4 File Offset: 0x000B2EB4
	public bool IsPointNearCell(bool yIsUpAxis, Vector3 point)
	{
		if (this.maxDistance == 0f)
		{
			this.maxDistance = (this.Size.x + this.Size.y + this.Size.z) / 2f;
		}
		return (point - this.Center).sqrMagnitude <= this.maxDistance * this.maxDistance;
	}

	// Token: 0x040010A8 RID: 4264
	public byte Id;

	// Token: 0x040010A9 RID: 4265
	public Vector3 Center;

	// Token: 0x040010AA RID: 4266
	public Vector3 Size;

	// Token: 0x040010AB RID: 4267
	public Vector3 TopLeft;

	// Token: 0x040010AC RID: 4268
	public Vector3 BottomRight;

	// Token: 0x040010AD RID: 4269
	public CellTreeNode.ENodeType NodeType;

	// Token: 0x040010AE RID: 4270
	public CellTreeNode Parent;

	// Token: 0x040010AF RID: 4271
	public List<CellTreeNode> Childs;

	// Token: 0x040010B0 RID: 4272
	private float maxDistance;

	// Token: 0x020002F0 RID: 752
	public enum ENodeType
	{
		// Token: 0x040010B2 RID: 4274
		Root,
		// Token: 0x040010B3 RID: 4275
		Node,
		// Token: 0x040010B4 RID: 4276
		Leaf
	}
}
