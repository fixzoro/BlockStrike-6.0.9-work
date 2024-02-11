using System;
using System.Linq;
using UnityEngine;

// Token: 0x0200013D RID: 317
[ExecuteInEditMode]
[AddComponentMenu("NGUI/Atlas3D/MeshAtlas")]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshAtlas : MonoBehaviour
{
	// Token: 0x170001FD RID: 509
	// (get) Token: 0x06000B45 RID: 2885 RVA: 0x0000C26C File Offset: 0x0000A46C
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

	// Token: 0x170001FE RID: 510
	// (get) Token: 0x06000B46 RID: 2886 RVA: 0x0000C291 File Offset: 0x0000A491
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

	// Token: 0x170001FF RID: 511
	// (get) Token: 0x06000B47 RID: 2887 RVA: 0x0000C2B6 File Offset: 0x0000A4B6
	// (set) Token: 0x06000B48 RID: 2888 RVA: 0x0000C2BE File Offset: 0x0000A4BE
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

	// Token: 0x17000200 RID: 512
	// (get) Token: 0x06000B49 RID: 2889 RVA: 0x0000C2DE File Offset: 0x0000A4DE
	// (set) Token: 0x06000B4A RID: 2890 RVA: 0x0000C2E6 File Offset: 0x0000A4E6
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

	// Token: 0x17000201 RID: 513
	// (get) Token: 0x06000B4B RID: 2891 RVA: 0x0000C306 File Offset: 0x0000A506
	// (set) Token: 0x06000B4C RID: 2892 RVA: 0x0000C30E File Offset: 0x0000A50E
	public Rect spriteSize
	{
		get
		{
			return this.mSpriteSize;
		}
		set
		{
			if (this.mSpriteSize != value)
			{
				this.mSpriteSize = value;
				this.UpdateUVs();
			}
		}
	}

	// Token: 0x17000202 RID: 514
	// (get) Token: 0x06000B4D RID: 2893 RVA: 0x0000C32E File Offset: 0x0000A52E
	// (set) Token: 0x06000B4E RID: 2894 RVA: 0x0000C336 File Offset: 0x0000A536
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

	// Token: 0x17000203 RID: 515
	// (get) Token: 0x06000B4F RID: 2895 RVA: 0x0000C351 File Offset: 0x0000A551
	// (set) Token: 0x06000B50 RID: 2896 RVA: 0x0000C359 File Offset: 0x0000A559
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

	// Token: 0x17000204 RID: 516
	// (get) Token: 0x06000B51 RID: 2897 RVA: 0x0000C374 File Offset: 0x0000A574
	// (set) Token: 0x06000B52 RID: 2898 RVA: 0x0000C37C File Offset: 0x0000A57C
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

	// Token: 0x17000205 RID: 517
	// (get) Token: 0x06000B53 RID: 2899 RVA: 0x0000C397 File Offset: 0x0000A597
	// (set) Token: 0x06000B54 RID: 2900 RVA: 0x0000C39F File Offset: 0x0000A59F
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

	// Token: 0x17000206 RID: 518
	// (get) Token: 0x06000B55 RID: 2901 RVA: 0x0000C3BA File Offset: 0x0000A5BA
	// (set) Token: 0x06000B56 RID: 2902 RVA: 0x0000C3C2 File Offset: 0x0000A5C2
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

	// Token: 0x17000207 RID: 519
	// (get) Token: 0x06000B57 RID: 2903 RVA: 0x0000C3E2 File Offset: 0x0000A5E2
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

	// Token: 0x17000208 RID: 520
	// (get) Token: 0x06000B58 RID: 2904 RVA: 0x0000C407 File Offset: 0x0000A607
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

	// Token: 0x17000209 RID: 521
	// (get) Token: 0x06000B59 RID: 2905 RVA: 0x0000C42C File Offset: 0x0000A62C
	// (set) Token: 0x06000B5A RID: 2906 RVA: 0x0005C0CC File Offset: 0x0005A2CC
	public UIAtlas atlas
	{
		get
		{
			return this.mAtlas;
		}
		set
		{
			if (this.mAtlas != value)
			{
				this.mAtlas = value;
				if (this.mAtlas != null)
				{
					MeshAtlas.lastAtlas = value;
					this.atlasMaterial = this.mAtlas.spriteMaterial;
					this.meshRenderer.sharedMaterial = this.atlasMaterial;
					if (string.IsNullOrEmpty(this.mSpriteName))
					{
						this.spriteName = MeshAtlas.lastSpriteName;
					}
					else
					{
						this.UpdateUVs();
					}
				}
			}
		}
	}

	// Token: 0x1700020A RID: 522
	// (get) Token: 0x06000B5B RID: 2907 RVA: 0x0000C434 File Offset: 0x0000A634
	// (set) Token: 0x06000B5C RID: 2908 RVA: 0x0000C43C File Offset: 0x0000A63C
	public string spriteName
	{
		get
		{
			return this.mSpriteName;
		}
		set
		{
			if (this.mSpriteName != value)
			{
				this.mSpriteName = value;
				MeshAtlas.lastSpriteName = value;
				this.UpdateUVs();
			}
		}
	}

	// Token: 0x06000B5D RID: 2909 RVA: 0x0000C462 File Offset: 0x0000A662
	private void OnEnable()
	{
		this.UpdateMesh();
	}

	// Token: 0x06000B5E RID: 2910 RVA: 0x0000C46A File Offset: 0x0000A66A
	private void Reset()
	{
		this.DisableMesh();
		if (Application.isEditor)
		{
			this.UpdateMesh();
		}
	}

	// Token: 0x06000B5F RID: 2911 RVA: 0x0005C150 File Offset: 0x0005A350
	public void UpdateMesh()
	{
		if (this.originalMesh == null)
		{
			this.originalMesh = this.meshFilter.mesh;
			this.originalMaterial = this.meshFilter.GetComponent<Renderer>().sharedMaterial;
			if (this.atlas == null && MeshAtlas.lastAtlas != null)
			{
				this.atlas = MeshAtlas.lastAtlas;
			}
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

	// Token: 0x06000B60 RID: 2912 RVA: 0x0005C1E0 File Offset: 0x0005A3E0
	public void EnableMesh()
	{
		if (this.atlas == null)
		{
			return;
		}
		if (this.atlasMesh != null)
		{
			return;
		}
		this.atlasMesh = new Mesh();
		this.atlasMesh.name = this.originalMesh.name + "_Atlas";
		this.UpdateVertices();
		this.UpdateFlip();
		this.atlasMesh.uv2 = this.originalMesh.uv2;
		this.atlasMesh.normals = this.originalMesh.normals;
		this.UpdateColor();
		this.atlasMesh.tangents = this.originalMesh.tangents;
		this.UpdateUVs();
		if (this.atlasMaterial == null)
		{
			this.atlasMaterial = this.atlas.spriteMaterial;
		}
		this.meshFilter.GetComponent<Renderer>().sharedMaterial = this.atlasMaterial;
		this.meshFilter.mesh = this.atlasMesh;
	}

	// Token: 0x06000B61 RID: 2913 RVA: 0x0005C2E0 File Offset: 0x0005A4E0
	public void DisableMesh()
	{
		if (this.atlasMesh != null)
		{
			this.atlasMaterial = this.meshFilter.GetComponent<Renderer>().sharedMaterial;
			UnityEngine.Object.DestroyImmediate(this.atlasMesh);
			this.atlasMesh = null;
		}
		if (this.originalMesh != null)
		{
			this.meshFilter.mesh = this.originalMesh;
		}
		if (this.originalMaterial != null)
		{
			this.meshFilter.GetComponent<Renderer>().sharedMaterial = this.originalMaterial;
		}
	}

	// Token: 0x06000B62 RID: 2914 RVA: 0x0005C370 File Offset: 0x0005A570
	public void UpdateSettings()
	{
		this.UpdateVertices();
		this.UpdateFlip();
		this.atlasMesh.uv2 = this.originalMesh.uv2;
		this.atlasMesh.normals = this.originalMesh.normals;
		this.UpdateColor();
		this.atlasMesh.tangents = this.originalMesh.tangents;
		this.UpdateUVs();
	}

	// Token: 0x06000B63 RID: 2915 RVA: 0x0005C3D8 File Offset: 0x0005A5D8
	private void UpdateUVs()
	{
		if (this.atlasMesh == null || this.atlas == null || string.IsNullOrEmpty(this.spriteName))
		{
			return;
		}
		UISpriteData sprite = this.atlas.GetSprite(this.spriteName);
		if (sprite == null || this.atlas.texture == null)
		{
			return;
		}
		Rect rect = new Rect((float)sprite.x, (float)sprite.y, (float)sprite.width, (float)sprite.height);
		Rect rect2 = NGUIMath.ConvertToTexCoords(rect, this.atlas.texture.width, this.atlas.texture.height);
		Vector2[] uv = this.originalMesh.uv;
		for (int i = 0; i < uv.Length; i++)
		{
			uv[i].x = uv[i].x * rect2.width * this.spriteSize.width + rect2.x + this.spriteSize.x;
			uv[i].y = uv[i].y * rect2.height * this.spriteSize.height + rect2.y + this.spriteSize.y;
		}
		this.atlasMesh.uv = uv;
	}

	// Token: 0x06000B64 RID: 2916 RVA: 0x0005C558 File Offset: 0x0005A758
	private void UpdateFlip()
	{
		if (this.atlasMesh == null)
		{
			return;
		}
		if (this.flip)
		{
			this.atlasMesh.triangles = this.originalMesh.triangles.Reverse<int>().ToArray<int>();
		}
		else
		{
			this.atlasMesh.triangles = this.originalMesh.triangles;
		}
	}

	// Token: 0x06000B65 RID: 2917 RVA: 0x0005C5C0 File Offset: 0x0005A7C0
	private void UpdateVertices()
	{
		if (this.atlasMesh == null)
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
		this.atlasMesh.vertices = vertices;
	}

	// Token: 0x06000B66 RID: 2918 RVA: 0x0005C6D8 File Offset: 0x0005A8D8
	private void UpdateColor()
	{
		if (this.atlasMesh == null)
		{
			return;
		}
		Color[] array = new Color[this.atlasMesh.uv.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = this.mColor;
		}
		this.atlasMesh.colors = array;
	}

	// Token: 0x06000B67 RID: 2919 RVA: 0x0000C482 File Offset: 0x0000A682
	public bool HasSprite(string spriteName)
	{
		return !(this.atlas != null) && this.atlas.GetSprite(spriteName) != null;
	}

	// Token: 0x0400078D RID: 1933
	public static UIAtlas lastAtlas;

	// Token: 0x0400078E RID: 1934
	public static string lastSpriteName;

	// Token: 0x0400078F RID: 1935
	public Mesh originalMesh;

	// Token: 0x04000790 RID: 1936
	private Mesh atlasMesh;

	// Token: 0x04000791 RID: 1937
	private MeshFilter mMeshFilter;

	// Token: 0x04000792 RID: 1938
	private MeshRenderer mMeshRenderer;

	// Token: 0x04000793 RID: 1939
	public Vector3 mScale = Vector3.one;

	// Token: 0x04000794 RID: 1940
	public Vector3 mPivot = Vector3.zero;

	// Token: 0x04000795 RID: 1941
	public Rect mSpriteSize = new Rect(0f, 0f, 1f, 1f);

	// Token: 0x04000796 RID: 1942
	public bool mMirrorX;

	// Token: 0x04000797 RID: 1943
	public bool mMirrorY;

	// Token: 0x04000798 RID: 1944
	public bool mMirrorZ;

	// Token: 0x04000799 RID: 1945
	public bool mFlip;

	// Token: 0x0400079A RID: 1946
	public Color mColor = Color.white;

	// Token: 0x0400079B RID: 1947
	public Material originalMaterial;

	// Token: 0x0400079C RID: 1948
	public Material atlasMaterial;

	// Token: 0x0400079D RID: 1949
	public UIAtlas mAtlas;

	// Token: 0x0400079E RID: 1950
	public string mSpriteName;

	// Token: 0x0400079F RID: 1951
	private GameObject mGameObject;

	// Token: 0x040007A0 RID: 1952
	private Transform mTransform;
}
