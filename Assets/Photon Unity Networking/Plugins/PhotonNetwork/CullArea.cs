using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002ED RID: 749
public class CullArea : MonoBehaviour
{
	// Token: 0x17000418 RID: 1048
	// (get) Token: 0x06001C9B RID: 7323 RVA: 0x00014A04 File Offset: 0x00012C04
	// (set) Token: 0x06001C9C RID: 7324 RVA: 0x00014A0C File Offset: 0x00012C0C
	public int CellCount { get; private set; }

	// Token: 0x17000419 RID: 1049
	// (get) Token: 0x06001C9D RID: 7325 RVA: 0x00014A15 File Offset: 0x00012C15
	// (set) Token: 0x06001C9E RID: 7326 RVA: 0x00014A1D File Offset: 0x00012C1D
	public CellTree CellTree { get; private set; }

	// Token: 0x1700041A RID: 1050
	// (get) Token: 0x06001C9F RID: 7327 RVA: 0x00014A26 File Offset: 0x00012C26
	// (set) Token: 0x06001CA0 RID: 7328 RVA: 0x00014A2E File Offset: 0x00012C2E
	public Dictionary<int, GameObject> Map { get; private set; }

	// Token: 0x06001CA1 RID: 7329 RVA: 0x00014A37 File Offset: 0x00012C37
	private void Awake()
	{
		this.idCounter = this.FIRST_GROUP_ID;
		this.CreateCellHierarchy();
	}

	// Token: 0x06001CA2 RID: 7330 RVA: 0x00014A4B File Offset: 0x00012C4B
	public void OnDrawGizmos()
	{
		this.idCounter = this.FIRST_GROUP_ID;
		if (this.RecreateCellHierarchy)
		{
			this.CreateCellHierarchy();
		}
		this.DrawCells();
	}

	// Token: 0x06001CA3 RID: 7331 RVA: 0x000B44E4 File Offset: 0x000B26E4
	private void CreateCellHierarchy()
	{
		if (!this.IsCellCountAllowed())
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"There are too many cells created by your subdivision options. Maximum allowed number of cells is ",
					(int)(250 - this.FIRST_GROUP_ID),
					". Current number of cells is ",
					this.CellCount,
					"."
				}));
				return;
			}
			Application.Quit();
		}
		byte id;
		this.idCounter = (byte)((id = this.idCounter) + 1);
		CellTreeNode cellTreeNode = new CellTreeNode(id, CellTreeNode.ENodeType.Root, null);
		if (this.YIsUpAxis)
		{
			this.Center = new Vector2(base.transform.position.x, base.transform.position.y);
			this.Size = new Vector2(base.transform.localScale.x, base.transform.localScale.y);
			cellTreeNode.Center = new Vector3(this.Center.x, this.Center.y, 0f);
			cellTreeNode.Size = new Vector3(this.Size.x, this.Size.y, 0f);
			cellTreeNode.TopLeft = new Vector3(this.Center.x - this.Size.x / 2f, this.Center.y - this.Size.y / 2f, 0f);
			cellTreeNode.BottomRight = new Vector3(this.Center.x + this.Size.x / 2f, this.Center.y + this.Size.y / 2f, 0f);
		}
		else
		{
			this.Center = new Vector2(base.transform.position.x, base.transform.position.z);
			this.Size = new Vector2(base.transform.localScale.x, base.transform.localScale.z);
			cellTreeNode.Center = new Vector3(this.Center.x, 0f, this.Center.y);
			cellTreeNode.Size = new Vector3(this.Size.x, 0f, this.Size.y);
			cellTreeNode.TopLeft = new Vector3(this.Center.x - this.Size.x / 2f, 0f, this.Center.y - this.Size.y / 2f);
			cellTreeNode.BottomRight = new Vector3(this.Center.x + this.Size.x / 2f, 0f, this.Center.y + this.Size.y / 2f);
		}
		this.CreateChildCells(cellTreeNode, 1);
		this.CellTree = new CellTree(cellTreeNode);
		this.RecreateCellHierarchy = false;
	}

	// Token: 0x06001CA4 RID: 7332 RVA: 0x000B4828 File Offset: 0x000B2A28
	private void CreateChildCells(CellTreeNode parent, int cellLevelInHierarchy)
	{
		if (cellLevelInHierarchy > this.NumberOfSubdivisions)
		{
			return;
		}
		int num = (int)this.Subdivisions[cellLevelInHierarchy - 1].x;
		int num2 = (int)this.Subdivisions[cellLevelInHierarchy - 1].y;
		float num3 = parent.Center.x - parent.Size.x / 2f;
		float num4 = parent.Size.x / (float)num;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				float num5 = num3 + (float)i * num4 + num4 / 2f;
				byte id;
				this.idCounter = (byte)((id = this.idCounter) + 1);
				CellTreeNode cellTreeNode = new CellTreeNode(id, (this.NumberOfSubdivisions != cellLevelInHierarchy) ? CellTreeNode.ENodeType.Node : CellTreeNode.ENodeType.Leaf, parent);
				if (this.YIsUpAxis)
				{
					float num6 = parent.Center.y - parent.Size.y / 2f;
					float num7 = parent.Size.y / (float)num2;
					float num8 = num6 + (float)j * num7 + num7 / 2f;
					cellTreeNode.Center = new Vector3(num5, num8, 0f);
					cellTreeNode.Size = new Vector3(num4, num7, 0f);
					cellTreeNode.TopLeft = new Vector3(num5 - num4 / 2f, num8 - num7 / 2f, 0f);
					cellTreeNode.BottomRight = new Vector3(num5 + num4 / 2f, num8 + num7 / 2f, 0f);
				}
				else
				{
					float num9 = parent.Center.z - parent.Size.z / 2f;
					float num10 = parent.Size.z / (float)num2;
					float num11 = num9 + (float)j * num10 + num10 / 2f;
					cellTreeNode.Center = new Vector3(num5, 0f, num11);
					cellTreeNode.Size = new Vector3(num4, 0f, num10);
					cellTreeNode.TopLeft = new Vector3(num5 - num4 / 2f, 0f, num11 - num10 / 2f);
					cellTreeNode.BottomRight = new Vector3(num5 + num4 / 2f, 0f, num11 + num10 / 2f);
				}
				parent.AddChild(cellTreeNode);
				this.CreateChildCells(cellTreeNode, cellLevelInHierarchy + 1);
			}
		}
	}

	// Token: 0x06001CA5 RID: 7333 RVA: 0x00014A70 File Offset: 0x00012C70
	private void DrawCells()
	{
		if (this.CellTree != null && this.CellTree.RootNode != null)
		{
			this.CellTree.RootNode.Draw();
		}
		else
		{
			this.RecreateCellHierarchy = true;
		}
	}

	// Token: 0x06001CA6 RID: 7334 RVA: 0x000B4A98 File Offset: 0x000B2C98
	private bool IsCellCountAllowed()
	{
		int num = 1;
		int num2 = 1;
		foreach (Vector2 vector in this.Subdivisions)
		{
			num *= (int)vector.x;
			num2 *= (int)vector.y;
		}
		this.CellCount = num * num2;
		return this.CellCount <= (int)(250 - this.FIRST_GROUP_ID);
	}

	// Token: 0x06001CA7 RID: 7335 RVA: 0x000B4B0C File Offset: 0x000B2D0C
	public List<byte> GetActiveCells(Vector3 position)
	{
		List<byte> list = new List<byte>(0);
		this.CellTree.RootNode.GetActiveCells(list, this.YIsUpAxis, position);
		return list;
	}

	// Token: 0x04001097 RID: 4247
	private const int MAX_NUMBER_OF_ALLOWED_CELLS = 250;

	// Token: 0x04001098 RID: 4248
	public const int MAX_NUMBER_OF_SUBDIVISIONS = 3;

	// Token: 0x04001099 RID: 4249
	public readonly byte FIRST_GROUP_ID = 1;

	// Token: 0x0400109A RID: 4250
	public readonly int[] SUBDIVISION_FIRST_LEVEL_ORDER = new int[]
	{
		0,
		1,
		1,
		1
	};

	// Token: 0x0400109B RID: 4251
	public readonly int[] SUBDIVISION_SECOND_LEVEL_ORDER = new int[]
	{
		0,
		2,
		1,
		2,
		0,
		2,
		1,
		2
	};

	// Token: 0x0400109C RID: 4252
	public readonly int[] SUBDIVISION_THIRD_LEVEL_ORDER = new int[]
	{
		0,
		3,
		2,
		3,
		1,
		3,
		2,
		3,
		1,
		3,
		2,
		3
	};

	// Token: 0x0400109D RID: 4253
	public Vector2 Center;

	// Token: 0x0400109E RID: 4254
	public Vector2 Size = new Vector2(25f, 25f);

	// Token: 0x0400109F RID: 4255
	public Vector2[] Subdivisions = new Vector2[3];

	// Token: 0x040010A0 RID: 4256
	public int NumberOfSubdivisions;

	// Token: 0x040010A1 RID: 4257
	public bool YIsUpAxis;

	// Token: 0x040010A2 RID: 4258
	public bool RecreateCellHierarchy;

	// Token: 0x040010A3 RID: 4259
	private byte idCounter;
}
