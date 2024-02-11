using System;

// Token: 0x020002EE RID: 750
public class CellTree
{
	// Token: 0x06001CA8 RID: 7336 RVA: 0x00004734 File Offset: 0x00002934
	public CellTree()
	{
	}

	// Token: 0x06001CA9 RID: 7337 RVA: 0x00014AA9 File Offset: 0x00012CA9
	public CellTree(CellTreeNode root)
	{
		this.RootNode = root;
	}

	// Token: 0x1700041B RID: 1051
	// (get) Token: 0x06001CAA RID: 7338 RVA: 0x00014AB8 File Offset: 0x00012CB8
	// (set) Token: 0x06001CAB RID: 7339 RVA: 0x00014AC0 File Offset: 0x00012CC0
	public CellTreeNode RootNode { get; private set; }
}
