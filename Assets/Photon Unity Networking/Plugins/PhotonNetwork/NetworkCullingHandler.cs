using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002F1 RID: 753
[RequireComponent(typeof(PhotonView))]
public class NetworkCullingHandler : MonoBehaviour, IPunObservable
{
	// Token: 0x06001CB4 RID: 7348 RVA: 0x000B4D28 File Offset: 0x000B2F28
	private void OnEnable()
	{
		if (this.pView == null)
		{
			this.pView = base.GetComponent<PhotonView>();
			if (!this.pView.isMine)
			{
				return;
			}
		}
		if (this.cullArea == null)
		{
			this.cullArea = UnityEngine.Object.FindObjectOfType<CullArea>();
		}
		this.previousActiveCells = new List<byte>(0);
		this.activeCells = new List<byte>(0);
		this.currentPosition = (this.lastPosition = base.transform.position);
	}

	// Token: 0x06001CB5 RID: 7349 RVA: 0x000B4DB4 File Offset: 0x000B2FB4
	private void Start()
	{
		if (!this.pView.isMine)
		{
			return;
		}
		if (PhotonNetwork.inRoom && this.cullArea.NumberOfSubdivisions == 0)
		{
			this.pView.group = this.cullArea.FIRST_GROUP_ID;
			PhotonNetwork.SetInterestGroups(this.cullArea.FIRST_GROUP_ID, true);
		}
	}

	// Token: 0x06001CB6 RID: 7350 RVA: 0x000B4E18 File Offset: 0x000B3018
	private void Update()
	{
		if (!this.pView.isMine)
		{
			return;
		}
		this.lastPosition = this.currentPosition;
		this.currentPosition = base.transform.position;
		if (this.currentPosition != this.lastPosition && this.HaveActiveCellsChanged())
		{
			this.UpdateInterestGroups();
		}
	}

	// Token: 0x06001CB7 RID: 7351 RVA: 0x000B4E7C File Offset: 0x000B307C
	private void OnGUI()
	{
		if (!this.pView.isMine)
		{
			return;
		}
		string text = "Inside cells:\n";
		string text2 = "Subscribed cells:\n";
		for (int i = 0; i < this.activeCells.Count; i++)
		{
			if (i <= this.cullArea.NumberOfSubdivisions)
			{
				text = text + this.activeCells[i] + " | ";
			}
			text2 = text2 + this.activeCells[i] + " | ";
		}
		GUI.Label(new Rect(20f, (float)Screen.height - 120f, 200f, 40f), "<color=white>PhotonView Group: " + this.pView.group + "</color>", new GUIStyle
		{
			alignment = TextAnchor.UpperLeft,
			fontSize = 16
		});
		GUI.Label(new Rect(20f, (float)Screen.height - 100f, 200f, 40f), "<color=white>" + text + "</color>", new GUIStyle
		{
			alignment = TextAnchor.UpperLeft,
			fontSize = 16
		});
		GUI.Label(new Rect(20f, (float)Screen.height - 60f, 200f, 40f), "<color=white>" + text2 + "</color>", new GUIStyle
		{
			alignment = TextAnchor.UpperLeft,
			fontSize = 16
		});
	}

	// Token: 0x06001CB8 RID: 7352 RVA: 0x000B5000 File Offset: 0x000B3200
	private bool HaveActiveCellsChanged()
	{
		if (this.cullArea.NumberOfSubdivisions == 0)
		{
			return false;
		}
		this.previousActiveCells = new List<byte>(this.activeCells);
		this.activeCells = this.cullArea.GetActiveCells(base.transform.position);
		while (this.activeCells.Count <= this.cullArea.NumberOfSubdivisions)
		{
			this.activeCells.Add(this.cullArea.FIRST_GROUP_ID);
		}
		return this.activeCells.Count != this.previousActiveCells.Count || this.activeCells[this.cullArea.NumberOfSubdivisions] != this.previousActiveCells[this.cullArea.NumberOfSubdivisions];
	}

	// Token: 0x06001CB9 RID: 7353 RVA: 0x000B50D4 File Offset: 0x000B32D4
	private void UpdateInterestGroups()
	{
		List<byte> list = new List<byte>(0);
		foreach (byte item in this.previousActiveCells)
		{
			if (!this.activeCells.Contains(item))
			{
				list.Add(item);
			}
		}
		PhotonNetwork.SetInterestGroups(list.ToArray(), this.activeCells.ToArray());
	}

	// Token: 0x06001CBA RID: 7354 RVA: 0x000B515C File Offset: 0x000B335C
	public void OnPhotonSerializeView(PhotonStream stream)
	{
		while (this.activeCells.Count <= this.cullArea.NumberOfSubdivisions)
		{
			this.activeCells.Add(this.cullArea.FIRST_GROUP_ID);
		}
		if (this.cullArea.NumberOfSubdivisions == 1)
		{
			this.orderIndex = ++this.orderIndex % this.cullArea.SUBDIVISION_FIRST_LEVEL_ORDER.Length;
			this.pView.group = this.activeCells[this.cullArea.SUBDIVISION_FIRST_LEVEL_ORDER[this.orderIndex]];
		}
		else if (this.cullArea.NumberOfSubdivisions == 2)
		{
			this.orderIndex = ++this.orderIndex % this.cullArea.SUBDIVISION_SECOND_LEVEL_ORDER.Length;
			this.pView.group = this.activeCells[this.cullArea.SUBDIVISION_SECOND_LEVEL_ORDER[this.orderIndex]];
		}
		else if (this.cullArea.NumberOfSubdivisions == 3)
		{
			this.orderIndex = ++this.orderIndex % this.cullArea.SUBDIVISION_THIRD_LEVEL_ORDER.Length;
			this.pView.group = this.activeCells[this.cullArea.SUBDIVISION_THIRD_LEVEL_ORDER[this.orderIndex]];
		}
	}

	// Token: 0x040010B5 RID: 4277
	private int orderIndex;

	// Token: 0x040010B6 RID: 4278
	private CullArea cullArea;

	// Token: 0x040010B7 RID: 4279
	private List<byte> previousActiveCells;

	// Token: 0x040010B8 RID: 4280
	private List<byte> activeCells;

	// Token: 0x040010B9 RID: 4281
	private PhotonView pView;

	// Token: 0x040010BA RID: 4282
	private Vector3 lastPosition;

	// Token: 0x040010BB RID: 4283
	private Vector3 currentPosition;
}
