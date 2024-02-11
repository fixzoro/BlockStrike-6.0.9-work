using System;
using System.Linq;
using UnityEngine;

// Token: 0x0200054C RID: 1356
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class MeshEditor : MonoBehaviour
{
	// Token: 0x170004A7 RID: 1191
	// (get) Token: 0x06002DDA RID: 11738 RVA: 0x0001FE02 File Offset: 0x0001E002
	public GameObject cachedGameObject
	{
		get
		{
			if (this.mGameObject == null)
			{
				this.mGameObject = base.gameObject;
			}
			return this.mGameObject;
		}
	}

	// Token: 0x170004A8 RID: 1192
	// (get) Token: 0x06002DDB RID: 11739 RVA: 0x0001FE27 File Offset: 0x0001E027
	public Transform cachedTransform
	{
		get
		{
			if (this.mTransform == null)
			{
				this.mTransform = base.transform;
			}
			return this.mTransform;
		}
	}

	// Token: 0x170004A9 RID: 1193
	// (get) Token: 0x06002DDC RID: 11740 RVA: 0x0001FE4C File Offset: 0x0001E04C
	// (set) Token: 0x06002DDD RID: 11741 RVA: 0x0001FE54 File Offset: 0x0001E054
	public Vector3 scale
	{
		get
		{
			return this.mScale;
		}
		set
		{
			if (this.mScale != value)
			{
				this.mScale = value;
				this.UpdateVertices();
			}
		}
	}

	// Token: 0x170004AA RID: 1194
	// (get) Token: 0x06002DDE RID: 11742 RVA: 0x0001FE74 File Offset: 0x0001E074
	// (set) Token: 0x06002DDF RID: 11743 RVA: 0x0001FE7C File Offset: 0x0001E07C
	public Vector3 pivot
	{
		get
		{
			return this.mPivot;
		}
		set
		{
			if (this.mPivot != value)
			{
				this.mPivot = value;
				this.UpdateVertices();
			}
		}
	}

	// Token: 0x170004AB RID: 1195
	// (get) Token: 0x06002DE0 RID: 11744 RVA: 0x0001FE9C File Offset: 0x0001E09C
	// (set) Token: 0x06002DE1 RID: 11745 RVA: 0x0001FEA4 File Offset: 0x0001E0A4
	public bool mirrorX
	{
		get
		{
			return this.mMirrorX;
		}
		set
		{
			if (this.mMirrorX != value)
			{
				this.mMirrorX = value;
				this.UpdateVertices();
			}
		}
	}

	// Token: 0x170004AC RID: 1196
	// (get) Token: 0x06002DE2 RID: 11746 RVA: 0x0001FEBF File Offset: 0x0001E0BF
	// (set) Token: 0x06002DE3 RID: 11747 RVA: 0x0001FEC7 File Offset: 0x0001E0C7
	public bool mirrorY
	{
		get
		{
			return this.mMirrorY;
		}
		set
		{
			if (this.mMirrorY != value)
			{
				this.mMirrorY = value;
				this.UpdateVertices();
			}
		}
	}

	// Token: 0x170004AD RID: 1197
	// (get) Token: 0x06002DE4 RID: 11748 RVA: 0x0001FEE2 File Offset: 0x0001E0E2
	// (set) Token: 0x06002DE5 RID: 11749 RVA: 0x0001FEEA File Offset: 0x0001E0EA
	public bool mirrorZ
	{
		get
		{
			return this.mMirrorZ;
		}
		set
		{
			if (this.mMirrorZ != value)
			{
				this.mMirrorZ = value;
				this.UpdateVertices();
			}
		}
	}

	// Token: 0x170004AE RID: 1198
	// (get) Token: 0x06002DE6 RID: 11750 RVA: 0x0001FF05 File Offset: 0x0001E105
	// (set) Token: 0x06002DE7 RID: 11751 RVA: 0x0001FF0D File Offset: 0x0001E10D
	public bool flip
	{
		get
		{
			return this.mFlip;
		}
		set
		{
			if (this.mFlip != value)
			{
				this.mFlip = value;
				this.UpdateFlip();
			}
		}
	}

	// Token: 0x170004AF RID: 1199
	// (get) Token: 0x06002DE8 RID: 11752 RVA: 0x0001FF28 File Offset: 0x0001E128
	// (set) Token: 0x06002DE9 RID: 11753 RVA: 0x0001FF30 File Offset: 0x0001E130
	public Color color
	{
		get
		{
			return this.mColor;
		}
		set
		{
			if (this.mColor != value)
			{
				this.mColor = value;
				this.UpdateColor();
			}
		}
	}

	// Token: 0x170004B0 RID: 1200
	// (get) Token: 0x06002DEA RID: 11754 RVA: 0x0001FF50 File Offset: 0x0001E150
	// (set) Token: 0x06002DEB RID: 11755 RVA: 0x0001FF58 File Offset: 0x0001E158
	public Rect uvRect
	{
		get
		{
			return this.mUVRect;
		}
		set
		{
			if (this.mUVRect != value)
			{
				this.mUVRect = value;
				this.UpdateUV();
			}
		}
	}

	// Token: 0x170004B1 RID: 1201
	// (get) Token: 0x06002DEC RID: 11756 RVA: 0x0001FF78 File Offset: 0x0001E178
	public MeshFilter meshFilter
	{
		get
		{
			if (this.mMeshFilter == null)
			{
				this.mMeshFilter = base.GetComponent<MeshFilter>();
			}
			return this.mMeshFilter;
		}
	}

	// Token: 0x170004B2 RID: 1202
	// (get) Token: 0x06002DED RID: 11757 RVA: 0x0001FF9D File Offset: 0x0001E19D
	public MeshRenderer meshRenderer
	{
		get
		{
			if (this.mMeshRenderer == null)
			{
				this.mMeshRenderer = base.GetComponent<MeshRenderer>();
			}
			return this.mMeshRenderer;
		}
	}

	// Token: 0x06002DEE RID: 11758 RVA: 0x0001FFC2 File Offset: 0x0001E1C2
	private void OnEnable()
	{
		this.UpdateMesh();
	}

	// Token: 0x06002DEF RID: 11759 RVA: 0x0001FFCA File Offset: 0x0001E1CA
	private void Reset()
	{
		this.DisableMesh();
		if (Application.isEditor)
		{
			this.UpdateMesh();
		}
	}

	// Token: 0x06002DF0 RID: 11760 RVA: 0x0001FFE2 File Offset: 0x0001E1E2
	public void UpdateMesh()
	{
		if (this.originalMesh == null)
		{
			this.originalMesh = this.meshFilter.mesh;
		}
		if (base.enabled)
		{
			this.EnableMesh();
		}
		else
		{
			this.DisableMesh();
		}
	}

	// Token: 0x06002DF1 RID: 11761 RVA: 0x0010ACA4 File Offset: 0x00108EA4
	public void EnableMesh()
	{
		this.editMesh = new Mesh();
		this.editMesh.name = this.originalMesh.name + "_Editor";
		this.UpdateVertices();
		this.UpdateFlip();
		this.editMesh.uv = this.originalMesh.uv;
		this.editMesh.uv2 = this.originalMesh.uv2;
		this.editMesh.normals = this.originalMesh.normals;
		this.editMesh.tangents = this.originalMesh.tangents;
		this.UpdateUV();
		this.UpdateColor();
		this.meshFilter.mesh = this.editMesh;
	}

	// Token: 0x06002DF2 RID: 11762 RVA: 0x0010AD60 File Offset: 0x00108F60
	public void DisableMesh()
	{
		if (this.editMesh != null)
		{
			UnityEngine.Object.DestroyImmediate(this.editMesh);
			this.editMesh = null;
		}
		if (this.originalMesh != null)
		{
			this.meshFilter.mesh = this.originalMesh;
		}
	}

	// Token: 0x06002DF3 RID: 11763 RVA: 0x00020022 File Offset: 0x0001E222
	public void UpdateSettings()
	{
		this.UpdateVertices();
		this.UpdateFlip();
		this.UpdateUV();
		this.UpdateColor();
	}

	// Token: 0x06002DF4 RID: 11764 RVA: 0x0010ADB4 File Offset: 0x00108FB4
	private void UpdateFlip()
	{
		if (this.editMesh == null)
		{
			return;
		}
		if (this.flip)
		{
			this.editMesh.triangles = this.originalMesh.triangles.Reverse<int>().ToArray<int>();
		}
		else
		{
			this.editMesh.triangles = this.originalMesh.triangles;
		}
	}

	// Token: 0x06002DF5 RID: 11765 RVA: 0x0010AE1C File Offset: 0x0010901C
	private void UpdateVertices()
	{
		if (this.editMesh == null)
		{
			return;
		}
		Vector3[] vertices = this.originalMesh.vertices;
		for (int i = 0; i < vertices.Length; i++)
		{
			vertices[i].x = (vertices[i].x + this.mPivot.x) * this.mScale.x * (float)((!this.mMirrorX) ? 1 : -1);
			vertices[i].y = (vertices[i].y + this.mPivot.y) * this.mScale.y * (float)((!this.mMirrorY) ? 1 : -1);
			vertices[i].z = (vertices[i].z + this.mPivot.z) * this.mScale.z * (float)((!this.mMirrorZ) ? 1 : -1);
		}
		this.editMesh.vertices = vertices;
	}

	// Token: 0x06002DF6 RID: 11766 RVA: 0x0010AF34 File Offset: 0x00109134
	private void UpdateUV()
	{
		if (this.editMesh == null)
		{
			return;
		}
		Vector2[] uv = this.originalMesh.uv;
		for (int i = 0; i < uv.Length; i++)
		{
			uv[i].x = uv[i].x * this.uvRect.width + this.uvRect.x;
			uv[i].y = uv[i].y * this.uvRect.height + this.uvRect.y;
		}
		this.editMesh.uv = uv;
	}

	// Token: 0x06002DF7 RID: 11767 RVA: 0x0010AFF0 File Offset: 0x001091F0
	private void UpdateColor()
	{
		if (this.editMesh == null)
		{
			return;
		}
		Color[] array = new Color[this.originalMesh.uv.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = this.mColor;
		}
		this.editMesh.colors = array;
	}

	// Token: 0x04001D65 RID: 7525
	public Mesh originalMesh;

	// Token: 0x04001D66 RID: 7526
	private Mesh editMesh;

	// Token: 0x04001D67 RID: 7527
	private MeshFilter mMeshFilter;

	// Token: 0x04001D68 RID: 7528
	public Vector3 mScale = Vector3.one;

	// Token: 0x04001D69 RID: 7529
	public Vector3 mPivot = Vector3.zero;

	// Token: 0x04001D6A RID: 7530
	public bool mMirrorX;

	// Token: 0x04001D6B RID: 7531
	public bool mMirrorY;

	// Token: 0x04001D6C RID: 7532
	public bool mMirrorZ;

	// Token: 0x04001D6D RID: 7533
	public bool mFlip;

	// Token: 0x04001D6E RID: 7534
	public Rect mUVRect = new Rect(0f, 0f, 1f, 1f);

	// Token: 0x04001D6F RID: 7535
	public Color mColor = Color.white;

	// Token: 0x04001D70 RID: 7536
	private GameObject mGameObject;

	// Token: 0x04001D71 RID: 7537
	private Transform mTransform;

	// Token: 0x04001D72 RID: 7538
	private MeshRenderer mMeshRenderer;
}
