using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000B3 RID: 179
[ExecuteInEditMode]
[AddComponentMenu("NGUI/Internal/Draw Call")]
public class UIDrawCall : MonoBehaviour
{
	// Token: 0x17000084 RID: 132
	// (get) Token: 0x060005A7 RID: 1447 RVA: 0x000088D1 File Offset: 0x00006AD1
	[Obsolete("Use UIDrawCall.activeList")]
	public static BetterList<UIDrawCall> list
	{
		get
		{
			return UIDrawCall.mActiveList;
		}
	}

	// Token: 0x17000085 RID: 133
	// (get) Token: 0x060005A8 RID: 1448 RVA: 0x000088D1 File Offset: 0x00006AD1
	public static BetterList<UIDrawCall> activeList
	{
		get
		{
			return UIDrawCall.mActiveList;
		}
	}

	// Token: 0x17000086 RID: 134
	// (get) Token: 0x060005A9 RID: 1449 RVA: 0x000088D8 File Offset: 0x00006AD8
	public static BetterList<UIDrawCall> inactiveList
	{
		get
		{
			return UIDrawCall.mInactiveList;
		}
	}

	// Token: 0x17000087 RID: 135
	// (get) Token: 0x060005AA RID: 1450 RVA: 0x000088DF File Offset: 0x00006ADF
	// (set) Token: 0x060005AB RID: 1451 RVA: 0x000088E7 File Offset: 0x00006AE7
	public int renderQueue
	{
		get
		{
			return this.mRenderQueue;
		}
		set
		{
			if (this.mRenderQueue != value)
			{
				this.mRenderQueue = value;
				if (this.mDynamicMat != null)
				{
					this.mDynamicMat.renderQueue = value;
#if UNITY_EDITOR
                    if (mRenderer != null) mRenderer.enabled = isActive;
#endif
                }
            }
		}
	}

	// Token: 0x17000088 RID: 136
	// (get) Token: 0x060005AC RID: 1452 RVA: 0x00008919 File Offset: 0x00006B19
	// (set) Token: 0x060005AD RID: 1453 RVA: 0x00008921 File Offset: 0x00006B21
	public int sortingOrder
	{
		get
		{
			return this.mSortingOrder;
		}
		set
		{
			if (this.mSortingOrder != value)
			{
				this.mSortingOrder = value;
				if (this.mRenderer != null)
				{
					this.mRenderer.sortingOrder = value;
				}
			}
		}
	}

	// Token: 0x17000089 RID: 137
	// (get) Token: 0x060005AE RID: 1454 RVA: 0x00041904 File Offset: 0x0003FB04
	// (set) Token: 0x060005AF RID: 1455 RVA: 0x00008953 File Offset: 0x00006B53
	public string sortingLayerName
	{
		get
		{
			if (!string.IsNullOrEmpty(this.mSortingLayerName))
			{
				return this.mSortingLayerName;
			}
			if (this.mRenderer == null)
			{
				return null;
			}
			this.mSortingLayerName = this.mRenderer.sortingLayerName;
			return this.mSortingLayerName;
		}
		set
		{
			if (this.mRenderer != null && this.mSortingLayerName != value)
			{
				this.mSortingLayerName = value;
				this.mRenderer.sortingLayerName = value;
			}
		}
	}

	// Token: 0x1700008A RID: 138
	// (get) Token: 0x060005B0 RID: 1456 RVA: 0x0000898A File Offset: 0x00006B8A
	public int finalRenderQueue
	{
		get
		{
			return (!(this.mDynamicMat != null)) ? this.mRenderQueue : this.mDynamicMat.renderQueue;
		}
	}

#if UNITY_EDITOR

    /// <summary>
    /// Whether the draw call is currently active.
    /// </summary>

    public bool isActive
    {
        get
        {
            return mActive;
        }
        set
        {
            if (mActive != value)
            {
                mActive = value;

                if (mRenderer != null)
                {
                    mRenderer.enabled = value;
                    NGUITools.SetDirty(gameObject);
                }
            }
        }
    }
    bool mActive = true;
#endif

    // Token: 0x1700008B RID: 139
    // (get) Token: 0x060005B1 RID: 1457 RVA: 0x000089B3 File Offset: 0x00006BB3
    public Transform cachedTransform
	{
		get
		{
			if (this.mTrans == null)
			{
				this.mTrans = base.transform;
			}
			return this.mTrans;
		}
	}

	// Token: 0x1700008C RID: 140
	// (get) Token: 0x060005B2 RID: 1458 RVA: 0x000089D8 File Offset: 0x00006BD8
	// (set) Token: 0x060005B3 RID: 1459 RVA: 0x000089E0 File Offset: 0x00006BE0
	public Material baseMaterial
	{
		get
		{
			return this.mMaterial;
		}
		set
		{
			if (this.mMaterial != value)
			{
				this.mMaterial = value;
				this.mRebuildMat = true;
			}
		}
	}

	// Token: 0x1700008D RID: 141
	// (get) Token: 0x060005B4 RID: 1460 RVA: 0x00008A01 File Offset: 0x00006C01
	public Material dynamicMaterial
	{
		get
		{
			return this.mDynamicMat;
		}
	}

	// Token: 0x1700008E RID: 142
	// (get) Token: 0x060005B5 RID: 1461 RVA: 0x00008A09 File Offset: 0x00006C09
	// (set) Token: 0x060005B6 RID: 1462 RVA: 0x00008A11 File Offset: 0x00006C11
	public Texture mainTexture
	{
		get
		{
			return this.mTexture;
		}
		set
		{
			this.mTexture = value;
			if (this.mBlock == null)
			{
				this.mBlock = new MaterialPropertyBlock();
			}
			this.mBlock.SetTexture("_MainTex", value ?? Texture2D.whiteTexture);
		}
	}

	// Token: 0x1700008F RID: 143
	// (get) Token: 0x060005B7 RID: 1463 RVA: 0x00008A4D File Offset: 0x00006C4D
	// (set) Token: 0x060005B8 RID: 1464 RVA: 0x00008A55 File Offset: 0x00006C55
	public Shader shader
	{
		get
		{
			return this.mShader;
		}
		set
		{
			if (this.mShader != value)
			{
				this.mShader = value;
				this.mRebuildMat = true;
			}
		}
	}

	// Token: 0x17000090 RID: 144
	// (get) Token: 0x060005B9 RID: 1465 RVA: 0x00008A76 File Offset: 0x00006C76
	public int triangles
	{
		get
		{
			return (!(this.mMesh != null)) ? 0 : this.mTriangles;
		}
	}

	// Token: 0x17000091 RID: 145
	// (get) Token: 0x060005BA RID: 1466 RVA: 0x00008A95 File Offset: 0x00006C95
	public bool isClipped
	{
		get
		{
			return this.mClipCount != 0;
		}
	}

	// Token: 0x060005BB RID: 1467 RVA: 0x00041954 File Offset: 0x0003FB54
	private void CreateMaterial()
	{
        //this.panel.clipping = UIDrawCall.Clipping.TextureMask;
        this.mTextureClip = false;
        this.mLegacyShader = false;
        this.mClipCount = this.panel.clipCount;
        string text = (!(this.mShader != null)) ? ((!(this.mMaterial != null)) ? "Unlit/Transparent Colored" : this.mMaterial.shader.name) : this.mShader.name;
        text = text.Replace("GUI/Text Shader", "Unlit/Text");
        if (text.Length > 2 && text[text.Length - 2] == ' ')
        {
            int num = (int)text[text.Length - 1];
            if (num > 48 && num <= 57)
            {
                text = text.Substring(0, text.Length - 2);
            }
        }
        if (text.StartsWith("Hidden/"))
        {
            text = text.Substring(7);
        }
        text = text.Replace(" (SoftClip)", string.Empty);
        text = text.Replace(" (TextureClip)", string.Empty);
        if (this.panel != null && this.panel.clipping == UIDrawCall.Clipping.TextureMask)
        {
            this.mTextureClip = true;
            this.shader = Shader.Find("Hidden/" + text + " (TextureClip)");
        }
        else if (this.mClipCount != 0)
        {
            this.shader = Shader.Find(string.Concat(new object[]
            {
                "Hidden/",
                text,
                " ",
                this.mClipCount
            }));
            if (this.shader == null)
            {
                this.shader = Shader.Find(text + " " + this.mClipCount);
            }
            if (this.shader == null && this.mClipCount == 1)
            {
                this.mLegacyShader = true;
                this.shader = Shader.Find(text + " (SoftClip)");
            }
        }
        else
        {
            this.shader = Shader.Find(text);
        }
        if (this.shader == null)
        {
            this.shader = Shader.Find("Unlit/Transparent Colored");
        }
        if (this.mMaterial != null)
        {
            this.mDynamicMat = new Material(this.mMaterial);
            this.mDynamicMat.name = "[NGUI] " + this.mMaterial.name;
            this.mDynamicMat.hideFlags = (HideFlags.DontSave | HideFlags.NotEditable);
            this.mDynamicMat.CopyPropertiesFromMaterial(this.mMaterial);
            string[] shaderKeywords = this.mMaterial.shaderKeywords;
            for (int i = 0; i < shaderKeywords.Length; i++)
            {
                this.mDynamicMat.EnableKeyword(shaderKeywords[i]);
            }
            if (this.shader != null)
            {
                this.mDynamicMat.shader = this.shader;
            }
            else if (this.mClipCount != 0)
            {
                Debug.LogError(string.Concat(new object[]
                {
                    text,
                    " shader doesn't have a clipped shader version for ",
                    this.mClipCount,
                    " clip regions"
                }));
            }
        }
        else
        {
            this.mDynamicMat = new Material(this.shader);
            this.mDynamicMat.name = "[NGUI] " + this.shader.name;
            this.mDynamicMat.hideFlags = (HideFlags.DontSave | HideFlags.NotEditable);
        }
    }

	// Token: 0x060005BC RID: 1468 RVA: 0x00041CD0 File Offset: 0x0003FED0
	private Material RebuildMaterial()
	{
		NGUITools.DestroyImmediate(this.mDynamicMat);
		this.CreateMaterial();
		this.mDynamicMat.renderQueue = this.mRenderQueue;
		if (this.mRenderer != null)
		{
			this.mRenderer.sharedMaterials = new Material[]
			{
				this.mDynamicMat
			};
			this.mRenderer.sortingLayerName = this.mSortingLayerName;
			this.mRenderer.sortingOrder = this.mSortingOrder;
		}
		return this.mDynamicMat;
	}

	// Token: 0x060005BD RID: 1469 RVA: 0x00041D54 File Offset: 0x0003FF54
	private void UpdateMaterials()
	{
		if (this.panel == null)
		{
			return;
		}
		if (this.mRebuildMat || this.mDynamicMat == null || this.mClipCount != this.panel.clipCount || this.mTextureClip != (this.panel.clipping == UIDrawCall.Clipping.TextureMask))
		{
			this.RebuildMaterial();
			this.mRebuildMat = false;
		}
	}

	// Token: 0x060005BE RID: 1470 RVA: 0x00041DCC File Offset: 0x0003FFCC
	public void UpdateGeometry(int widgetCount, bool needsBounds)
	{
		this.widgetCount = widgetCount;
		int count = this.verts.Count;
		if (count > 0 && count == this.uvs.Count && count == this.cols.Count && count % 4 == 0)
		{
			if (UIDrawCall.mColorSpace == ColorSpace.Uninitialized)
			{
				UIDrawCall.mColorSpace = QualitySettings.activeColorSpace;
			}
			if (UIDrawCall.mColorSpace == ColorSpace.Linear)
			{
				for (int i = 0; i < count; i++)
				{
					Color value = this.cols[i];
					value.r = Mathf.GammaToLinearSpace(value.r);
					value.g = Mathf.GammaToLinearSpace(value.g);
					value.b = Mathf.GammaToLinearSpace(value.b);
					value.a = Mathf.GammaToLinearSpace(value.a);
					this.cols[i] = value;
				}
			}
			if (this.mFilter == null)
			{
				this.mFilter = base.gameObject.GetComponent<MeshFilter>();
			}
			if (this.mFilter == null)
			{
				this.mFilter = base.gameObject.AddComponent<MeshFilter>();
			}
			if (count < 65000)
			{
				int num = (count >> 1) * 3;
				bool flag = this.mIndices == null || this.mIndices.Length != num;
				if (this.mMesh == null)
				{
					this.mMesh = new Mesh();
					this.mMesh.hideFlags = HideFlags.DontSave;
					this.mMesh.name = ((!(this.mMaterial != null)) ? "[NGUI] Mesh" : ("[NGUI] " + this.mMaterial.name));
					if (UIDrawCall.dx9BugWorkaround == 0)
					{
						this.mMesh.MarkDynamic();
					}
					flag = true;
				}
				bool flag2 = this.uvs.Count != count || this.cols.Count != count || this.uv2.Count != count || this.norms.Count != count || this.tans.Count != count;
				if (!flag2 && this.panel != null && this.panel.renderQueue != UIPanel.RenderQueue.Automatic)
				{
					flag2 = (this.mMesh == null || this.mMesh.vertexCount != this.verts.Count);
				}
				this.mTriangles = count >> 1;
				if (this.mMesh.vertexCount != count)
				{
					this.mMesh.Clear();
					flag = true;
				}
				bool flag3 = this.uv2 != null && this.uv2.Count == count;
				bool flag4 = this.norms != null && this.norms.Count == count;
				bool flag5 = this.tans != null && this.tans.Count == count;
				if (this.mTempVerts == null || this.mTempVerts.Length < count)
				{
					this.mTempVerts = new Vector3[count];
				}
				if (this.mTempUV0 == null || this.mTempUV0.Length < count)
				{
					this.mTempUV0 = new Vector2[count];
				}
				if (this.mTempCols == null || this.mTempCols.Length < count)
				{
					this.mTempCols = new Color[count];
				}
				if (flag3 && (this.mTempUV2 == null || this.mTempUV2.Length < count))
				{
					this.mTempUV2 = new Vector2[count];
				}
				if (flag4 && (this.mTempNormals == null || this.mTempNormals.Length < count))
				{
					this.mTempNormals = new Vector3[count];
				}
				if (flag5 && (this.mTempTans == null || this.mTempTans.Length < count))
				{
					this.mTempTans = new Vector4[count];
				}
				this.verts.CopyTo(this.mTempVerts);
				this.uvs.CopyTo(this.mTempUV0);
				this.cols.CopyTo(this.mTempCols);
				if (flag4)
				{
					this.norms.CopyTo(this.mTempNormals);
				}
				if (flag5)
				{
					this.tans.CopyTo(this.mTempTans);
				}
				if (flag3)
				{
					int j = 0;
					int count2 = this.verts.Count;
					while (j < count2)
					{
						this.mTempUV2[j] = this.uv2[j];
						j++;
					}
				}
				this.mMesh.vertices = this.mTempVerts;
				this.mMesh.uv = this.mTempUV0;
				this.mMesh.colors = this.mTempCols;
				this.mMesh.uv2 = ((!flag3) ? null : this.mTempUV2);
				this.mMesh.normals = ((!flag4) ? null : this.mTempNormals);
				this.mMesh.tangents = ((!flag5) ? null : this.mTempTans);
				if (flag)
				{
					this.mIndices = this.GenerateCachedIndexBuffer(count, num);
					this.mMesh.triangles = this.mIndices;
				}
				if (flag2 || !this.alwaysOnScreen)
				{
					this.mMesh.RecalculateBounds();
				}
				this.mFilter.mesh = this.mMesh;
			}
			else
			{
				this.mTriangles = 0;
				if (this.mMesh != null)
				{
					this.mMesh.Clear();
				}
				Debug.LogError("Too many vertices on one panel: " + count);
			}
			if (this.mRenderer == null)
			{
				this.mRenderer = base.gameObject.GetComponent<MeshRenderer>();
			}
			if (this.mRenderer == null)
			{
				this.mRenderer = base.gameObject.AddComponent<MeshRenderer>();
			}
			if (this.mIsNew)
			{
				this.mIsNew = false;
				if (this.onCreateDrawCall != null)
				{
					this.onCreateDrawCall(this, this.mFilter, this.mRenderer);
				}
			}
			this.UpdateMaterials();
		}
		else
		{
			if (this.mFilter.mesh != null)
			{
				this.mFilter.mesh.Clear();
			}
			Debug.LogError("UIWidgets must fill the buffer with 4 vertices per quad. Found " + count);
		}
		this.verts.Clear();
		this.uvs.Clear();
		this.uv2.Clear();
		this.cols.Clear();
		this.norms.Clear();
		this.tans.Clear();
	}

	// Token: 0x060005BF RID: 1471 RVA: 0x0004249C File Offset: 0x0004069C
	private int[] GenerateCachedIndexBuffer(int vertexCount, int indexCount)
	{
		int i = 0;
		int count = UIDrawCall.mCache.Count;
		while (i < count)
		{
			int[] array = UIDrawCall.mCache[i];
			if (array != null && array.Length == indexCount)
			{
				return array;
			}
			i++;
		}
		int[] array2 = new int[indexCount];
		int num = 0;
		for (int j = 0; j < vertexCount; j += 4)
		{
			array2[num++] = j;
			array2[num++] = j + 1;
			array2[num++] = j + 2;
			array2[num++] = j + 2;
			array2[num++] = j + 3;
			array2[num++] = j;
		}
		if (UIDrawCall.mCache.Count > 10)
		{
			UIDrawCall.mCache.RemoveAt(0);
		}
		UIDrawCall.mCache.Add(array2);
		return array2;
	}

	// Token: 0x060005C0 RID: 1472 RVA: 0x00042578 File Offset: 0x00040778
	private void OnWillRenderObject()
	{
		this.UpdateMaterials();
		if (this.mBlock != null)
		{
			this.mRenderer.SetPropertyBlock(this.mBlock);
		}
		if (this.onRender != null)
		{
			this.onRender(this.mDynamicMat ?? this.mMaterial);
		}
		if (this.mDynamicMat == null || this.mClipCount == 0)
		{
			return;
		}
		if (this.mTextureClip)
		{
			Vector4 drawCallClipRange = this.panel.drawCallClipRange;
			Vector2 clipSoftness = this.panel.clipSoftness;
			Vector2 vector = new Vector2(1000f, 1000f);
			if (clipSoftness.x > 0f)
			{
				vector.x = drawCallClipRange.z / clipSoftness.x;
			}
			if (clipSoftness.y > 0f)
			{
				vector.y = drawCallClipRange.w / clipSoftness.y;
			}
			this.mDynamicMat.SetVector(UIDrawCall.ClipRange[0], new Vector4(-drawCallClipRange.x / drawCallClipRange.z, -drawCallClipRange.y / drawCallClipRange.w, 1f / drawCallClipRange.z, 1f / drawCallClipRange.w));
			this.mDynamicMat.SetTexture("_ClipTex", this.clipTexture);
		}
		else if (!this.mLegacyShader)
		{
			UIPanel parentPanel = this.panel;
			int num = 0;
			while (parentPanel != null)
			{
				if (parentPanel.hasClipping)
				{
					float angle = 0f;
					Vector4 drawCallClipRange2 = parentPanel.drawCallClipRange;
					if (parentPanel != this.panel)
					{
						Vector3 vector2 = parentPanel.cachedTransform.InverseTransformPoint(this.panel.cachedTransform.position);
						drawCallClipRange2.x -= vector2.x;
						drawCallClipRange2.y -= vector2.y;
						Vector3 eulerAngles = this.panel.cachedTransform.rotation.eulerAngles;
						Vector3 eulerAngles2 = parentPanel.cachedTransform.rotation.eulerAngles;
						Vector3 vector3 = eulerAngles2 - eulerAngles;
						vector3.x = NGUIMath.WrapAngle(vector3.x);
						vector3.y = NGUIMath.WrapAngle(vector3.y);
						vector3.z = NGUIMath.WrapAngle(vector3.z);
						if (Mathf.Abs(vector3.x) > 0.001f || Mathf.Abs(vector3.y) > 0.001f)
						{
							Debug.LogWarning("Panel can only be clipped properly if X and Y rotation is left at 0", this.panel);
						}
						angle = vector3.z;
					}
					this.SetClipping(num++, drawCallClipRange2, parentPanel.clipSoftness, angle);
				}
				parentPanel = parentPanel.parentPanel;
			}
		}
		else
		{
			Vector2 clipSoftness2 = this.panel.clipSoftness;
			Vector4 drawCallClipRange3 = this.panel.drawCallClipRange;
			Vector2 mainTextureOffset = new Vector2(-drawCallClipRange3.x / drawCallClipRange3.z, -drawCallClipRange3.y / drawCallClipRange3.w);
			Vector2 mainTextureScale = new Vector2(1f / drawCallClipRange3.z, 1f / drawCallClipRange3.w);
			Vector2 v = new Vector2(1000f, 1000f);
			if (clipSoftness2.x > 0f)
			{
				v.x = drawCallClipRange3.z / clipSoftness2.x;
			}
			if (clipSoftness2.y > 0f)
			{
				v.y = drawCallClipRange3.w / clipSoftness2.y;
			}
			this.mDynamicMat.mainTextureOffset = mainTextureOffset;
			this.mDynamicMat.mainTextureScale = mainTextureScale;
			this.mDynamicMat.SetVector("_ClipSharpness", v);
		}
	}

	// Token: 0x060005C1 RID: 1473 RVA: 0x00042944 File Offset: 0x00040B44
	private void SetClipping(int index, Vector4 cr, Vector2 soft, float angle)
	{
		angle *= -0.0174532924f;
		Vector2 vector = new Vector2(1000f, 1000f);
		if (soft.x > 0f)
		{
			vector.x = cr.z / soft.x;
		}
		if (soft.y > 0f)
		{
			vector.y = cr.w / soft.y;
		}
		if (index < UIDrawCall.ClipRange.Length)
		{
			this.mDynamicMat.SetVector(UIDrawCall.ClipRange[index], new Vector4(-cr.x / cr.z, -cr.y / cr.w, 1f / cr.z, 1f / cr.w));
			this.mDynamicMat.SetVector(UIDrawCall.ClipArgs[index], new Vector4(vector.x, vector.y, Mathf.Sin(angle), Mathf.Cos(angle)));
		}
	}

	// Token: 0x060005C2 RID: 1474 RVA: 0x00042A4C File Offset: 0x00040C4C
	private void Awake()
	{
		if (UIDrawCall.dx9BugWorkaround == -1)
		{
			RuntimePlatform platform = Application.platform;
			UIDrawCall.dx9BugWorkaround = (((platform != RuntimePlatform.WindowsPlayer && platform != RuntimePlatform.XBOX360) || SystemInfo.graphicsShaderLevel >= 40 || !SystemInfo.graphicsDeviceVersion.Contains("Direct3D")) ? 0 : 1);
		}
		if (UIDrawCall.ClipRange == null)
		{
			UIDrawCall.ClipRange = new int[]
			{
				Shader.PropertyToID("_ClipRange0"),
				Shader.PropertyToID("_ClipRange1"),
				Shader.PropertyToID("_ClipRange2"),
				Shader.PropertyToID("_ClipRange4")
			};
		}
		if (UIDrawCall.ClipArgs == null)
		{
			UIDrawCall.ClipArgs = new int[]
			{
				Shader.PropertyToID("_ClipArgs0"),
				Shader.PropertyToID("_ClipArgs1"),
				Shader.PropertyToID("_ClipArgs2"),
				Shader.PropertyToID("_ClipArgs3")
			};
		}
	}

	// Token: 0x060005C3 RID: 1475 RVA: 0x00008AA3 File Offset: 0x00006CA3
	private void OnEnable()
	{
		this.mRebuildMat = true;
	}

	// Token: 0x060005C4 RID: 1476 RVA: 0x00042B38 File Offset: 0x00040D38
	private void OnDisable()
	{
		this.depthStart = int.MaxValue;
		this.depthEnd = int.MinValue;
		this.panel = null;
		this.manager = null;
		this.mMaterial = null;
		this.mTexture = null;
		this.clipTexture = null;
		if (this.mRenderer != null)
		{
			this.mRenderer.sharedMaterials = new Material[0];
		}
		NGUITools.DestroyImmediate(this.mDynamicMat);
		this.mDynamicMat = null;
	}

	// Token: 0x060005C5 RID: 1477 RVA: 0x00008AAC File Offset: 0x00006CAC
	private void OnDestroy()
	{
		NGUITools.DestroyImmediate(this.mMesh);
		this.mMesh = null;
	}

	// Token: 0x060005C6 RID: 1478 RVA: 0x00008AC0 File Offset: 0x00006CC0
	public static UIDrawCall Create(UIPanel panel, Material mat, Texture tex, Shader shader)
	{
#if UNITY_EDITOR
        string name = null;
        if (tex != null) name = tex.name;
        else if (shader != null) name = shader.name;
        else if (mat != null) name = mat.name;
        return Create(name, panel, mat, tex, shader);
#else
		return Create(null, panel, mat, tex, shader);
#endif
    }

    // Token: 0x060005C7 RID: 1479 RVA: 0x00042BB4 File Offset: 0x00040DB4
    private static UIDrawCall Create(string name, UIPanel pan, Material mat, Texture tex, Shader shader)
	{
		UIDrawCall uidrawCall = UIDrawCall.Create(name);
		uidrawCall.gameObject.layer = pan.cachedGameObject.layer;
		uidrawCall.baseMaterial = mat;
		uidrawCall.mainTexture = tex;
		uidrawCall.shader = shader;
		uidrawCall.renderQueue = pan.startingRenderQueue;
		uidrawCall.sortingOrder = pan.sortingOrder;
		uidrawCall.manager = pan;
		return uidrawCall;
	}

	// Token: 0x060005C8 RID: 1480 RVA: 0x00042C14 File Offset: 0x00040E14
	private static UIDrawCall Create(string name)
	{
#if SHOW_HIDDEN_OBJECTS && UNITY_EDITOR
		name = (name != null) ? "_UIDrawCall [" + name + "]" : "DrawCall";
#endif
        if (mInactiveList.size > 0)
        {
            UIDrawCall dc = mInactiveList.Pop();
            mActiveList.Add(dc);
            if (name != null) dc.name = name;
            NGUITools.SetActive(dc.gameObject, true);
            return dc;
        }
        

#if UNITY_EDITOR
        // If we're in the editor, create the game object with hide flags set right away
        GameObject go = UnityEditor.EditorUtility.CreateGameObjectWithHideFlags(name,
#if SHOW_HIDDEN_OBJECTS
			HideFlags.DontSave | HideFlags.NotEditable, typeof(UIDrawCall));
#else
            HideFlags.HideAndDontSave, typeof(UIDrawCall));
#endif
        UIDrawCall newDC = go.GetComponent<UIDrawCall>();
#else
		GameObject go = new GameObject(name);
        go.hideFlags = HideFlags.HideInHierarchy;
		DontDestroyOnLoad(go);
		UIDrawCall newDC = go.AddComponent<UIDrawCall>();
#endif
        // Create the draw call
        mActiveList.Add(newDC);
        return newDC;
    }

	// Token: 0x060005C9 RID: 1481 RVA: 0x00042C94 File Offset: 0x00040E94
	public static void ClearAll()
	{
		bool isPlaying = Application.isPlaying;
		int i = UIDrawCall.mActiveList.size;
		while (i > 0)
		{
			UIDrawCall uidrawCall = UIDrawCall.mActiveList[--i];
			if (uidrawCall)
			{
				if (isPlaying)
				{
					NGUITools.SetActive(uidrawCall.gameObject, false);
				}
				else
				{
					NGUITools.DestroyImmediate(uidrawCall.gameObject);
				}
			}
		}
		UIDrawCall.mActiveList.Clear();
	}

	// Token: 0x060005CA RID: 1482 RVA: 0x00008ACC File Offset: 0x00006CCC
	public static void ReleaseAll()
	{
		UIDrawCall.ClearAll();
		UIDrawCall.ReleaseInactive();
	}

	// Token: 0x060005CB RID: 1483 RVA: 0x00042D08 File Offset: 0x00040F08
	public static void ReleaseInactive()
	{
		int i = UIDrawCall.mInactiveList.size;
		while (i > 0)
		{
			UIDrawCall uidrawCall = UIDrawCall.mInactiveList[--i];
			if (uidrawCall)
			{
				NGUITools.DestroyImmediate(uidrawCall.gameObject);
			}
		}
		UIDrawCall.mInactiveList.Clear();
	}

	// Token: 0x060005CC RID: 1484 RVA: 0x00042D5C File Offset: 0x00040F5C
	public static int Count(UIPanel panel)
	{
		int num = 0;
		for (int i = 0; i < UIDrawCall.mActiveList.size; i++)
		{
			if (UIDrawCall.mActiveList[i].manager == panel)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060005CD RID: 1485 RVA: 0x00042DA8 File Offset: 0x00040FA8
	public static void Destroy(UIDrawCall dc)
	{
		if (dc)
		{
			if (dc.onCreateDrawCall != null)
			{
				NGUITools.Destroy(dc.gameObject);
				return;
			}
			dc.onRender = null;
			if (Application.isPlaying)
			{
				if (UIDrawCall.mActiveList.Remove(dc))
				{
					NGUITools.SetActive(dc.gameObject, false);
					UIDrawCall.mInactiveList.Add(dc);
					dc.mIsNew = true;
				}
			}
			else
			{
				UIDrawCall.mActiveList.Remove(dc);
				NGUITools.DestroyImmediate(dc.gameObject);
			}
		}
	}

	// Token: 0x040003F3 RID: 1011
	private const int maxIndexBufferCache = 10;

	// Token: 0x040003F4 RID: 1012
	private static BetterList<UIDrawCall> mActiveList = new BetterList<UIDrawCall>();

	// Token: 0x040003F5 RID: 1013
	private static BetterList<UIDrawCall> mInactiveList = new BetterList<UIDrawCall>();

	// Token: 0x040003F6 RID: 1014
	[HideInInspector]
	[NonSerialized]
	public int widgetCount;

	// Token: 0x040003F7 RID: 1015
	[HideInInspector]
	[NonSerialized]
	public int depthStart = int.MaxValue;

	// Token: 0x040003F8 RID: 1016
	[HideInInspector]
	[NonSerialized]
	public int depthEnd = int.MinValue;

	// Token: 0x040003F9 RID: 1017
	[HideInInspector]
	[NonSerialized]
	public UIPanel manager;

	// Token: 0x040003FA RID: 1018
	[HideInInspector]
	[NonSerialized]
	public UIPanel panel;

	// Token: 0x040003FB RID: 1019
	[HideInInspector]
	[NonSerialized]
	public Texture2D clipTexture;

	// Token: 0x040003FC RID: 1020
	[HideInInspector]
	[NonSerialized]
	public bool alwaysOnScreen;

	// Token: 0x040003FD RID: 1021
	[HideInInspector]
	[NonSerialized]
	public List<Vector3> verts = new List<Vector3>();

	// Token: 0x040003FE RID: 1022
	[HideInInspector]
	[NonSerialized]
	public List<Vector3> norms = new List<Vector3>();

	// Token: 0x040003FF RID: 1023
	[HideInInspector]
	[NonSerialized]
	public List<Vector4> tans = new List<Vector4>();

	// Token: 0x04000400 RID: 1024
	[HideInInspector]
	[NonSerialized]
	public List<Vector2> uvs = new List<Vector2>();

	// Token: 0x04000401 RID: 1025
	[HideInInspector]
	[NonSerialized]
	public List<Vector4> uv2 = new List<Vector4>();

	// Token: 0x04000402 RID: 1026
	[HideInInspector]
	[NonSerialized]
	public List<Color> cols = new List<Color>();

	// Token: 0x04000403 RID: 1027
	[NonSerialized]
	private Material mMaterial;

	// Token: 0x04000404 RID: 1028
	[NonSerialized]
	private Texture mTexture;

	// Token: 0x04000405 RID: 1029
	[NonSerialized]
	private Shader mShader;

	// Token: 0x04000406 RID: 1030
	[NonSerialized]
	private int mClipCount;

	// Token: 0x04000407 RID: 1031
	[NonSerialized]
	private Transform mTrans;

	// Token: 0x04000408 RID: 1032
	[NonSerialized]
	private Mesh mMesh;

	// Token: 0x04000409 RID: 1033
	[NonSerialized]
	private MeshFilter mFilter;

	// Token: 0x0400040A RID: 1034
	[NonSerialized]
	private MeshRenderer mRenderer;

	// Token: 0x0400040B RID: 1035
	[NonSerialized]
	private Material mDynamicMat;

	// Token: 0x0400040C RID: 1036
	[NonSerialized]
	private int[] mIndices;

	// Token: 0x0400040D RID: 1037
	[NonSerialized]
	private Vector3[] mTempVerts;

	// Token: 0x0400040E RID: 1038
	[NonSerialized]
	private Vector2[] mTempUV0;

	// Token: 0x0400040F RID: 1039
	[NonSerialized]
	private Vector2[] mTempUV2;

	// Token: 0x04000410 RID: 1040
	[NonSerialized]
	private Color[] mTempCols;

	// Token: 0x04000411 RID: 1041
	[NonSerialized]
	private Vector3[] mTempNormals;

	// Token: 0x04000412 RID: 1042
	[NonSerialized]
	private Vector4[] mTempTans;

	// Token: 0x04000413 RID: 1043
	[NonSerialized]
	private bool mRebuildMat = true;

	// Token: 0x04000414 RID: 1044
	[NonSerialized]
	private bool mLegacyShader;

	// Token: 0x04000415 RID: 1045
	[NonSerialized]
	private int mRenderQueue = 3000;

	// Token: 0x04000416 RID: 1046
	[NonSerialized]
	private int mTriangles;

	// Token: 0x04000417 RID: 1047
	[NonSerialized]
	public bool isDirty;

	// Token: 0x04000418 RID: 1048
	[NonSerialized]
	private bool mTextureClip;

	// Token: 0x04000419 RID: 1049
	[NonSerialized]
	private bool mIsNew = true;

	// Token: 0x0400041A RID: 1050
	public UIDrawCall.OnRenderCallback onRender;

	// Token: 0x0400041B RID: 1051
	public UIDrawCall.OnCreateDrawCall onCreateDrawCall;

	// Token: 0x0400041C RID: 1052
	[NonSerialized]
	private string mSortingLayerName;

	// Token: 0x0400041D RID: 1053
	[NonSerialized]
	private int mSortingOrder;

	// Token: 0x0400041E RID: 1054
	private static ColorSpace mColorSpace = ColorSpace.Uninitialized;

	// Token: 0x0400041F RID: 1055
	private static List<int[]> mCache = new List<int[]>(10);

	// Token: 0x04000420 RID: 1056
	protected MaterialPropertyBlock mBlock;

	// Token: 0x04000421 RID: 1057
	private static int[] ClipRange = null;

	// Token: 0x04000422 RID: 1058
	private static int[] ClipArgs = null;

	// Token: 0x04000423 RID: 1059
	private static int dx9BugWorkaround = -1;

	// Token: 0x020000B4 RID: 180
	[DoNotObfuscateNGUI]
	public enum Clipping
	{
		// Token: 0x04000425 RID: 1061
		None,
		// Token: 0x04000426 RID: 1062
		TextureMask,
		// Token: 0x04000427 RID: 1063
		SoftClip = 3,
		// Token: 0x04000428 RID: 1064
		ConstrainButDontClip
	}

	// Token: 0x020000B5 RID: 181
	// (Invoke) Token: 0x060005CF RID: 1487
	public delegate void OnRenderCallback(Material mat);

	// Token: 0x020000B6 RID: 182
	// (Invoke) Token: 0x060005D3 RID: 1491
	public delegate void OnCreateDrawCall(UIDrawCall dc, MeshFilter filter, MeshRenderer ren);
}
