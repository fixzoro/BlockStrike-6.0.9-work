using System;
using UnityEngine;

// Token: 0x0200013E RID: 318
[AddComponentMenu("NGUI/Atlas3D/SkinnedMeshAtlas")]
[ExecuteInEditMode]
[RequireComponent(typeof(SkinnedMeshRenderer))]
public class SkinnedMeshAtlas : MonoBehaviour
{
	// Token: 0x06000B69 RID: 2921 RVA: 0x0000C4AF File Offset: 0x0000A6AF
	private void OnEnable()
	{
		this.UpdateMesh();
	}

	// Token: 0x06000B6A RID: 2922 RVA: 0x0000C4B7 File Offset: 0x0000A6B7
	private void Reset()
	{
		this.DisableMesh();
		if (Application.isEditor)
		{
			this.UpdateMesh();
		}
	}

	// Token: 0x1700020B RID: 523
	// (get) Token: 0x06000B6B RID: 2923 RVA: 0x0000C4CF File Offset: 0x0000A6CF
	public SkinnedMeshRenderer mf
	{
		get
		{
			if (this._mf == null)
			{
				this._mf = base.gameObject.GetComponent<SkinnedMeshRenderer>();
			}
			return this._mf;
		}
	}

	// Token: 0x06000B6C RID: 2924 RVA: 0x0005C73C File Offset: 0x0005A93C
	public void UpdateMesh()
	{
		if (this.originalMesh == null)
		{
			this.originalMesh = base.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
			this.originalMaterial = this.mf.GetComponent<Renderer>().sharedMaterial;
			if (this.atlas == null && SkinnedMeshAtlas.lastUsedAtlas != null)
			{
				this.atlas = SkinnedMeshAtlas.lastUsedAtlas;
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

	// Token: 0x06000B6D RID: 2925 RVA: 0x0005C7D0 File Offset: 0x0005A9D0
	public void EnableMesh()
	{
		if (this.mesh == null)
		{
			this.mesh = (Mesh)UnityEngine.Object.Instantiate(this.originalMesh);
			this.mesh.name = this.originalMesh.name + "_Atlas";
			this.UpdateUVs();
			if (this.customMaterial != null)
			{
				this.mf.GetComponent<Renderer>().sharedMaterial = this.customMaterial;
			}
			this.mf.sharedMesh = this.mesh;
		}
	}

	// Token: 0x06000B6E RID: 2926 RVA: 0x0005C864 File Offset: 0x0005AA64
	public void DisableMesh()
	{
		if (this.mesh != null)
		{
			this.customMaterial = this.mf.GetComponent<Renderer>().sharedMaterial;
			UnityEngine.Object.DestroyImmediate(this.mesh);
			this.mesh = null;
		}
		if (this.originalMesh != null)
		{
			this.mf.sharedMesh = this.originalMesh;
		}
		if (this.originalMaterial != null)
		{
			this.mf.GetComponent<Renderer>().sharedMaterial = this.originalMaterial;
		}
	}

	// Token: 0x06000B6F RID: 2927 RVA: 0x0005C8F4 File Offset: 0x0005AAF4
	public void UpdateUVs()
	{
		if (this.mesh == null)
		{
			return;
		}
		if (this.atlas == null)
		{
			return;
		}
		if (string.IsNullOrEmpty(this.spriteName))
		{
			return;
		}
		UISpriteData sprite = this.atlas.GetSprite(this.spriteName);
		if (sprite == null)
		{
			return;
		}
		if (this.atlas.texture == null)
		{
			return;
		}
		this.mSprite = sprite;
		Rect rect = new Rect((float)this.mSprite.x, (float)this.mSprite.y, (float)this.mSprite.width, (float)this.mSprite.height);
		Rect rect2 = NGUIMath.ConvertToTexCoords(rect, this.atlas.texture.width, this.atlas.texture.height);
		Vector2[] uv = this.originalMesh.uv;
		for (int i = 0; i < uv.Length; i++)
		{
			uv[i].x = uv[i].x * rect2.width + rect2.x;
			uv[i].y = uv[i].y * rect2.height + rect2.y;
		}
		this.mesh.uv = uv;
	}

	// Token: 0x1700020C RID: 524
	// (get) Token: 0x06000B70 RID: 2928 RVA: 0x0000C4F9 File Offset: 0x0000A6F9
	// (set) Token: 0x06000B71 RID: 2929 RVA: 0x0005CA50 File Offset: 0x0005AC50
	[SerializeField]
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
				this.mSpriteSet = false;
				this.mSprite = null;
				if (this.mAtlas != null)
				{
					SkinnedMeshAtlas.lastUsedAtlas = value;
					this.customMaterial = this.mAtlas.spriteMaterial;
					this.mf.GetComponent<Renderer>().sharedMaterial = this.customMaterial;
					if (this.originalMaterial != null && !string.IsNullOrEmpty(this.originalMaterial.name))
					{
						for (int i = 0; i < this.mAtlas.spriteList.Count; i++)
						{
							if (this.mAtlas.spriteList[i].name == this.originalMaterial.name)
							{
								this.SetAtlasSprite(this.mAtlas.spriteList[i]);
								this.mSpriteName = this.mSprite.name;
								break;
							}
						}
					}
					if (string.IsNullOrEmpty(this.mSpriteName) && this.mAtlas != null && this.mAtlas.spriteList.Count > 0)
					{
						this.SetAtlasSprite(this.mAtlas.spriteList[0]);
						this.mSpriteName = this.mSprite.name;
					}
					if (!string.IsNullOrEmpty(this.mSpriteName))
					{
						string spriteName = this.mSpriteName;
						this.mSpriteName = string.Empty;
						this.spriteName = spriteName;
					}
				}
			}
		}
	}

	// Token: 0x1700020D RID: 525
	// (get) Token: 0x06000B72 RID: 2930 RVA: 0x0000C501 File Offset: 0x0000A701
	// (set) Token: 0x06000B73 RID: 2931 RVA: 0x0005CBE8 File Offset: 0x0005ADE8
	[SerializeField]
	public string spriteName
	{
		get
		{
			return this.mSpriteName;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				if (string.IsNullOrEmpty(this.mSpriteName))
				{
					return;
				}
				this.mSpriteName = string.Empty;
				this.mSprite = null;
				this.mSpriteSet = false;
			}
			else if (this.mSpriteName != value)
			{
				this.mSpriteName = value;
				this.mSprite = null;
				this.mSpriteSet = false;
				this.UpdateUVs();
			}
		}
	}

	// Token: 0x1700020E RID: 526
	// (get) Token: 0x06000B74 RID: 2932 RVA: 0x0000C509 File Offset: 0x0000A709
	public bool isValid
	{
		get
		{
			return this.GetAtlasSprite() != null;
		}
	}

	// Token: 0x06000B75 RID: 2933 RVA: 0x0005CC5C File Offset: 0x0005AE5C
	public UISpriteData GetAtlasSprite()
	{
		if (!this.mSpriteSet)
		{
			this.mSprite = null;
		}
		if (this.mSprite == null && this.mAtlas != null)
		{
			if (!string.IsNullOrEmpty(this.mSpriteName))
			{
				UISpriteData sprite = this.mAtlas.GetSprite(this.mSpriteName);
				if (sprite == null)
				{
					return null;
				}
				this.SetAtlasSprite(sprite);
			}
			if (this.mSprite == null && this.mAtlas.spriteList.Count > 0)
			{
				UISpriteData uispriteData = this.mAtlas.spriteList[0];
				if (uispriteData == null)
				{
					return null;
				}
				this.SetAtlasSprite(uispriteData);
				if (this.mSprite == null)
				{
					Debug.LogError(this.mAtlas.name + " seems to have a null sprite!");
					return null;
				}
				this.mSpriteName = this.mSprite.name;
			}
		}
		return this.mSprite;
	}

	// Token: 0x06000B76 RID: 2934 RVA: 0x0005CD48 File Offset: 0x0005AF48
	private void SetAtlasSprite(UISpriteData sp)
	{
		this.mSpriteSet = true;
		if (sp != null)
		{
			this.mSprite = sp;
			this.mSpriteName = this.mSprite.name;
		}
		else
		{
			this.mSpriteName = ((this.mSprite == null) ? string.Empty : this.mSprite.name);
			this.mSprite = sp;
		}
	}

	// Token: 0x06000B77 RID: 2935 RVA: 0x0005CDAC File Offset: 0x0005AFAC
	public void UpdateAllMeshes()
	{
		MeshAtlas[] array = NGUITools.FindActive<MeshAtlas>();
		int i = 0;
		int num = array.Length;
		while (i < num)
		{
			MeshAtlas meshAtlas = array[i];
			if (meshAtlas.enabled && this.originalMesh == meshAtlas.originalMesh)
			{
				meshAtlas.UpdateMesh();
			}
			i++;
		}
	}

	// Token: 0x06000B78 RID: 2936 RVA: 0x0000574F File Offset: 0x0000394F
	public void UpdateMeshTextures()
	{
	}

	// Token: 0x040007A1 RID: 1953
	public static UIAtlas lastUsedAtlas;

	// Token: 0x040007A2 RID: 1954
	public Mesh originalMesh;

	// Token: 0x040007A3 RID: 1955
	private SkinnedMeshRenderer _mf;

	// Token: 0x040007A4 RID: 1956
	private Mesh mesh;

	// Token: 0x040007A5 RID: 1957
	public Material originalMaterial;

	// Token: 0x040007A6 RID: 1958
	public Material customMaterial;

	// Token: 0x040007A7 RID: 1959
	public UIAtlas mAtlas;

	// Token: 0x040007A8 RID: 1960
	public string mSpriteName;

	// Token: 0x040007A9 RID: 1961
	public UISpriteData mSprite;

	// Token: 0x040007AA RID: 1962
	private bool mSpriteSet;
}
